#requires -version 7
<#
.SYNOPSIS
    The project's gate: restores the local tools, then runs the Definition of Done, build/dod.ps1, whose Format stage runs the CSharpier check (project-template ADR-0148).
.DESCRIPTION
    build/dod.ps1 is template-owned, so this script wraps it, as project-template ADR-0117 says. -Fast and -Filter are
    passed on to build/dod.ps1.
#>
[CmdletBinding()]
param([switch]$Fast, [string]$Filter)
$ErrorActionPreference = 'Stop'
Push-Location (Split-Path -Parent $PSScriptRoot)
try {
    dotnet tool restore
    if ($LASTEXITCODE) { exit $LASTEXITCODE }
    $dod = @{}
    if ($Fast) { $dod.Fast = $true }
    if ($Filter) { $dod.Filter = $Filter }
    & (Join-Path $PSScriptRoot 'dod.ps1') @dod
    exit $LASTEXITCODE
}
finally { Pop-Location }
