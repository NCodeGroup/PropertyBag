# Copyright © NCode Group
# SPDX-License-Identifier: Apache-2.0

#requires -version 7
<#
.SYNOPSIS
    Tests build/Compare-Packages.ps1 against fixture package folders, offline.
.DESCRIPTION
    Covers two folders of identical packages whose entries carry different timestamps, which match; an entry whose
    bytes differ; an entry in only one copy of a package; a package in only one folder; and an expected folder that
    holds no package. Each fixture package is a zip archive written here, and each case checks the exit code and every
    line the comparison prints. Exits 1 on the first failed check. build/gate.ps1 runs it.
#>
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem
$script = Join-Path $PSScriptRoot 'Compare-Packages.ps1'
$utf8 = [Text.UTF8Encoding]::new($false)
$packed = [DateTimeOffset]::new(2026, 11, 20, 10, 0, 0, [TimeSpan]::Zero)

# Writes a zip archive at an absolute path, one entry for each name in $Entries, stamped with $Time.
function New-Package {
    param([string]$Path, [hashtable]$Entries, [DateTimeOffset]$Time = $packed)
    $zip = [IO.Compression.ZipFile]::Open($Path, 'Create')
    try {
        foreach ($name in $Entries.Keys) {
            $entry = $zip.CreateEntry($name)
            $entry.LastWriteTime = $Time
            $stream = $entry.Open()
            try {
                $bytes = $utf8.GetBytes($Entries[$name])
                $stream.Write($bytes, 0, $bytes.Length)
            }
            finally { $stream.Dispose() }
        }
    }
    finally { $zip.Dispose() }
}

# $Expected and $Actual map each file name to its entries (name to text), and $ActualTime stamps the actual folder's
# entries; a file name with a null map is written as a plain text file.
function Invoke-Case {
    param(
        [string]$Name,
        [hashtable]$Expected,
        [hashtable]$Actual,
        [int]$Code,
        [string[]]$Lines,
        [DateTimeOffset]$ActualTime = $packed
    )
    $root = Join-Path ([IO.Path]::GetTempPath()) "compare-packages-test-$([guid]::NewGuid().ToString('N'))"
    try {
        $folders = @{ expected = $Expected; actual = $Actual }
        foreach ($side in $folders.Keys) {
            $folder = Join-Path $root $side
            New-Item -ItemType Directory -Path $folder -Force | Out-Null
            $time = if ($side -eq 'actual') { $ActualTime } else { $packed }
            foreach ($file in $folders[$side].Keys) {
                $path = Join-Path $folder $file
                if ($null -eq $folders[$side][$file]) { [IO.File]::WriteAllText($path, 'text', $utf8) }
                else { New-Package -Path $path -Entries $folders[$side][$file] -Time $time }
            }
        }

        $printed = @(& $script -Expected (Join-Path $root 'expected') -Actual (Join-Path $root 'actual'))

        $actualCode = $LASTEXITCODE
        if ($actualCode -ne $Code) { throw "exited $actualCode instead of $Code" }
        if (($printed -join "`n") -cne ($Lines -join "`n")) { throw "printed:`n$($printed -join "`n")" }
    }
    catch { Write-Host "FAILED: $Name`n$_"; exit 1 }
    finally { if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force } }
    Write-Host "ok: $Name"
}

function Get-Set { @{ 'A.1.0.0.nupkg' = @{ 'lib/a.dll' = 'one'; 'a.nuspec' = 'spec' }; 'A.1.0.0.snupkg' = @{ 'lib/a.pdb' = 'pdb' } } }

Invoke-Case 'same entries, later timestamps' -Expected (Get-Set) -Actual (Get-Set) -ActualTime $packed.AddDays(1) -Code 0 `
    -Lines 'Packages match: 2 packages, 3 entries; zip entry timestamps are not compared.'

$changed = Get-Set
$changed['A.1.0.0.nupkg']['lib/a.dll'] = 'two'
Invoke-Case 'an entry whose bytes differ' -Expected (Get-Set) -Actual $changed -Code 1 `
    -Lines 'Packages differ:', 'A.1.0.0.nupkg: lib/a.dll differs'

$swapped = Get-Set
$swapped['A.1.0.0.nupkg'].Remove('a.nuspec')
$swapped['A.1.0.0.nupkg']['lib/b.xml'] = 'xml'
Invoke-Case 'an entry in only one package' -Expected (Get-Set) -Actual $swapped -Code 1 `
    -Lines 'Packages differ:', `
    'A.1.0.0.nupkg: a.nuspec is only in the expected package', `
    'A.1.0.0.nupkg: lib/b.xml is only in the actual package'

Invoke-Case 'a package in only one folder' `
    -Expected @{ 'A.1.0.0.nupkg' = @{ 'a.nuspec' = 'spec' }; 'B.1.0.0.nupkg' = @{ 'b.nuspec' = 'spec' } } `
    -Actual @{ 'A.1.0.0.nupkg' = @{ 'a.nuspec' = 'spec' }; 'C.1.0.0.nupkg' = @{ 'c.nuspec' = 'spec' } } -Code 1 `
    -Lines 'Packages differ:', `
    'Only in the actual folder: C.1.0.0.nupkg', `
    'Only in the expected folder: B.1.0.0.nupkg'

Invoke-Case 'no package in the expected folder' -Expected @{ 'readme.txt' = $null } -Actual (Get-Set) -Code 1 `
    -Lines 'Packages differ:', 'No .nupkg or .snupkg file in the expected folder.'
# Every case passed.
exit 0
