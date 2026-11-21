# Copyright © NCode Group
# SPDX-License-Identifier: Apache-2.0

#requires -version 7
<#
.SYNOPSIS
    Compares the NuGet packages in two folders entry by entry.
.DESCRIPTION
    Lists the *.nupkg and *.snupkg files at the top of the expected and the actual folder, and reports a package that is
    in only one folder, an entry that is in only one of the two copies of a package, and an entry whose bytes differ
    (SHA-256). A package file is a zip archive whose entries carry the time they were packed, so the check compares
    entry names and the hash of each entry's bytes, never the package's own bytes or an entry's timestamp. Prints each
    problem on its own line after 'Packages differ:' and exits 1; exits 0 when every package and entry matches. Exits 1
    when the expected folder holds no package. .github/workflows/reproducible-build.yml compares
    a rebuild of a release with the release's packages.
.PARAMETER Expected
    The folder that holds the packages to compare against, such as the packages of a release.
.PARAMETER Actual
    The folder that holds the rebuilt packages.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Expected,
    [Parameter(Mandatory)][string]$Actual
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem

$expectedFolder = (Resolve-Path -LiteralPath $Expected).Path
$actualFolder = (Resolve-Path -LiteralPath $Actual).Path

function Get-PackageFiles([string]$Folder) {
    $files = [Collections.Generic.Dictionary[string, IO.FileInfo]]::new([StringComparer]::Ordinal)
    foreach ($file in @(Get-ChildItem -LiteralPath $Folder -File | Where-Object { $_.Extension -in '.nupkg', '.snupkg' })) {
        $files[$file.Name] = $file
    }
    $files
}

# Maps each entry's full name to the hex SHA-256 of its bytes.
function Get-EntryHashes([IO.FileInfo]$File) {
    $hashes = [Collections.Generic.Dictionary[string, string]]::new([StringComparer]::Ordinal)
    $zip = [IO.Compression.ZipFile]::OpenRead($File.FullName)
    try {
        foreach ($entry in $zip.Entries) {
            $stream = $entry.Open()
            try {
                $memory = [IO.MemoryStream]::new()
                try {
                    $stream.CopyTo($memory)
                    $hashes[$entry.FullName] = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($memory.ToArray()))
                }
                finally { $memory.Dispose() }
            }
            finally { $stream.Dispose() }
        }
    }
    finally { $zip.Dispose() }
    $hashes
}

$expectedFiles = Get-PackageFiles $expectedFolder
$actualFiles = Get-PackageFiles $actualFolder

if ($expectedFiles.Count -eq 0) {
    Write-Output 'Packages differ:'
    Write-Output 'No .nupkg or .snupkg file in the expected folder.'
    exit 1
}

$problems = [Collections.Generic.List[string]]::new()
$packages = 0
$entries = 0

foreach ($name in $expectedFiles.Keys) {
    if (-not $actualFiles.ContainsKey($name)) { $problems.Add("Only in the expected folder: $name") }
}
foreach ($name in $actualFiles.Keys) {
    if (-not $expectedFiles.ContainsKey($name)) { $problems.Add("Only in the actual folder: $name") }
}

foreach ($name in $expectedFiles.Keys) {
    if (-not $actualFiles.ContainsKey($name)) { continue }
    $packages++
    $expectedEntries = Get-EntryHashes $expectedFiles[$name]
    $actualEntries = Get-EntryHashes $actualFiles[$name]
    foreach ($entry in $expectedEntries.Keys) {
        if (-not $actualEntries.ContainsKey($entry)) { $problems.Add("${name}: $entry is only in the expected package"); continue }
        $entries++
        if ($expectedEntries[$entry] -cne $actualEntries[$entry]) { $problems.Add("${name}: $entry differs") }
    }
    foreach ($entry in $actualEntries.Keys) {
        if (-not $expectedEntries.ContainsKey($entry)) { $problems.Add("${name}: $entry is only in the actual package") }
    }
}

if ($problems.Count -gt 0) {
    $sorted = $problems.ToArray()
    [Array]::Sort($sorted, [StringComparer]::Ordinal)
    Write-Output 'Packages differ:'
    $sorted | ForEach-Object { Write-Output $_ }
    exit 1
}

Write-Output "Packages match: $packages packages, $entries entries; zip entry timestamps are not compared."
exit 0
