# Copyright © NCode Group
# SPDX-License-Identifier: Apache-2.0

#requires -version 7
<#
.SYNOPSIS
    Tests build/Assert-ApiMap.ps1 against fixture folders, offline.
.DESCRIPTION
    Covers a map that lists every shipped line once, whatever the files' comment and empty lines; a shipped line no
    entry lists; a line of the second file no entry lists; a line an entry lists that is not shipped; a line two
    entries list; no map file; and two files that hold no shipped line. Each fixture is a folder under the temp path,
    not a repository, that holds the two PublicAPI.Shipped.txt files and the map. Each case checks the exit code and every line the check prints. Exits 1 on
    the first failed check. build/gate.ps1 runs it before the check.
#>
$ErrorActionPreference = 'Stop'
$script = Join-Path $PSScriptRoot 'Assert-ApiMap.ps1'
$utf8 = [Text.UTF8Encoding]::new($false)
$abstractionsPath = 'src/NCode.PropertyBag.Abstractions/PublicAPI.Shipped.txt'
$implementationPath = 'src/NCode.PropertyBag/PublicAPI.Shipped.txt'
$mapPath = 'docs/site/_components/data/api-map.json'
$type = 'NCode.PropertyBag.PropertyBagKey'
$name = 'NCode.PropertyBag.PropertyBagKey.Name.get -> string!'
$factory = 'NCode.PropertyBag.PropertyBagFactory'

# An entry of the map, which lists the lines it covers.
function Entry([string]$Id, [string[]]$Api) { @{ id = $Id; api = @($Api) } }

# $Entries is the map's entries, or $null for no map file. Each fixture holds the two PublicAPI.Shipped.txt files: the
# Abstractions file holds a comment, an empty line and two lines, the second with trailing white space, and the
# implementation file holds a comment and one line.
function Invoke-Case {
    param(
        [string]$Case,
        [hashtable[]]$Entries,
        [string[]]$Problems = @(),
        [switch]$NoShippedLines
    )
    $root = Join-Path ([IO.Path]::GetTempPath()) "api-map-test-$([guid]::NewGuid().ToString('N'))"
    New-Item -ItemType Directory -Path $root | Out-Null
    try {
        $abstractions = Join-Path $root $abstractionsPath
        New-Item -ItemType Directory -Path (Split-Path -Parent $abstractions) -Force | Out-Null
        [IO.File]::WriteAllText($abstractions, $(if ($NoShippedLines) { "#nullable enable`n" } else { "#nullable enable`n`n$type`n$name  `n" }), $utf8)
        $implementation = Join-Path $root $implementationPath
        New-Item -ItemType Directory -Path (Split-Path -Parent $implementation) -Force | Out-Null
        [IO.File]::WriteAllText($implementation, $(if ($NoShippedLines) { "#nullable enable`n" } else { "#nullable enable`n$factory`n" }), $utf8)
        if ($null -ne $Entries) {
            $map = Join-Path $root $mapPath
            New-Item -ItemType Directory -Path (Split-Path -Parent $map) -Force | Out-Null
            $json = @{ groups = @(); entries = @($Entries) } | ConvertTo-Json -Depth 5
            [IO.File]::WriteAllText($map, "$json`n", $utf8)
        }

        # The check sets no exit code when it passes, so start from a clean one.
        $global:LASTEXITCODE = 0
        $printed = @(& $script -Root $root)

        $code = $LASTEXITCODE
        $expectedCode = if ($Problems) { 1 } else { 0 }
        if ($code -ne $expectedCode) { throw "exited $code instead of $expectedCode" }
        $expected = if ($Problems) { @('API map: FAILED.') + $Problems } else { @() }
        if (($printed -join "`n") -cne ($expected -join "`n")) { throw "printed:`n$($printed -join "`n")" }
    }
    catch { Write-Host "FAILED: $Case`n$_"; exit 1 }
    finally { Remove-Item -LiteralPath $root -Recurse -Force }
    Write-Host "ok: $Case"
}

Invoke-Case 'clean map' -Entries (Entry 'property-bag-key' @($type)), (Entry 'property-bag-key-name' @($name)), (Entry 'property-bag-factory' @($factory))
Invoke-Case 'a shipped line no entry lists' -Entries (Entry 'property-bag-key' @($type)), (Entry 'property-bag-factory' @($factory)) `
    -Problems "API map: no entry for '$name'"
Invoke-Case 'a line of the second file no entry lists' -Entries (Entry 'property-bag-key' @($type)), (Entry 'property-bag-key-name' @($name)) `
    -Problems "API map: no entry for '$factory'"
Invoke-Case 'a line that is not shipped' -Entries (Entry 'property-bag-key' @($type)), (Entry 'property-bag-key-name' @($name, 'NCode.PropertyBag.PropertyBagKey.Gone() -> void')), (Entry 'property-bag-factory' @($factory)) `
    -Problems "API map: 'NCode.PropertyBag.PropertyBagKey.Gone() -> void' (entry property-bag-key-name) is not in PublicAPI.Shipped.txt"
Invoke-Case 'a line two entries list' -Entries (Entry 'property-bag-key' @($type, $name)), (Entry 'property-bag-key-name' @($name)), (Entry 'property-bag-factory' @($factory)) `
    -Problems "API map: '$name' is listed 2 times: property-bag-key, property-bag-key-name"
Invoke-Case 'no map file' -Entries $null -Problems "API map: $mapPath does not exist"
Invoke-Case 'no shipped line in either file' -Entries @() -NoShippedLines -Problems "API map: $abstractionsPath and $implementationPath hold no shipped line"
# Every case passed. The last one left the check's exit code, 1, in $LASTEXITCODE, which a caller may read.
exit 0
