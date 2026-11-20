# Copyright © NCode Group
# SPDX-License-Identifier: Apache-2.0

#requires -version 7
<#
.SYNOPSIS
    Tests build/Test-XmlDocExamples.ps1 against fixture source folders.
.DESCRIPTION
    Each case writes a C# file whose /// comments hold <code> examples, runs the check on that folder with a scratch
    project of its own under artifacts/, and checks its exit code and output: an example that compiles passes; an
    example that declares a `public` member before its statements passes; an example that does not compile fails and
    names the fixture's file and line; and a <code> tag with other text on its line fails the count check, so no
    example is skipped. The cases build against the library, so they run online only as far as a restore does.
    Exits 1 on the first failed check. build/gate.ps1 runs it before the check.
#>
$ErrorActionPreference = 'Stop'
$script = Join-Path $PSScriptRoot 'Test-XmlDocExamples.ps1'
$root = Split-Path -Parent $PSScriptRoot
$utf8 = [Text.UTF8Encoding]::new($false)

# $Comment is the /// lines of the fixture file, $Expect the exit code, and $Says text the output must hold; a {file}
# in it stands for the fixture file's full path.
function Invoke-Case {
    param([string]$Case, [string[]]$Comment, [int]$Expect, [string]$Says = '')
    $id = [guid]::NewGuid().ToString('N')
    $source = Join-Path ([IO.Path]::GetTempPath()) "xml-examples-test-$id"
    $out = Join-Path $root "artifacts/xml-examples-test-$id"
    New-Item -ItemType Directory -Path $source | Out-Null
    try {
        $file = Join-Path $source 'Fixture.cs'
        [IO.File]::WriteAllText($file, (($Comment + 'public static class Fixture { }') -join "`n") + "`n", $utf8)
        $output = @(pwsh -NoProfile -NonInteractive -File $script -Source $source -Out $out 2>&1 | ForEach-Object { "$_" })
        $code = $LASTEXITCODE
        if ($code -ne $Expect) { "FAILED: $Case exited $code instead of $Expect. Output: $($output -join ' | ')"; exit 1 }
        $want = $Says.Replace('{file}', $file)
        if ($want -and -not (($output -join "`n").Contains($want))) { "FAILED: $Case did not print '$want'. Output: $($output -join ' | ')"; exit 1 }
        "ok: $Case"
    }
    finally {
        Remove-Item -LiteralPath $source -Recurse -Force
        if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Recurse -Force }
    }
}

Invoke-Case 'an example that compiles' -Expect 0 -Says 'XML examples: PASSED (1 examples).' -Comment @(
    '/// <code>'
    '/// var key = new PropertyBagKey&lt;string&gt;("UserName");'
    '/// propertyBag.SetValue(key, "Alice");'
    '/// </code>'
)
Invoke-Case 'an example that declares a member first' -Expect 0 -Says 'XML examples: PASSED (1 examples).' -Comment @(
    '/// <code>'
    '/// public static readonly PropertyBagKey&lt;int&gt; AgeKey = new("Age");'
    '///'
    '/// propertyBag.SetValue(AgeKey, 30);'
    '/// </code>'
)
# The broken call is on line 3 of the fixture file, which the error must name.
Invoke-Case 'an example that does not compile' -Expect 1 -Says '{file}(3,' -Comment @(
    '/// <code>'
    '/// var key = new PropertyBagKey&lt;string&gt;("UserName");'
    '/// propertyBag.NoSuchMethod(key);'
    '/// </code>'
)
Invoke-Case 'a <code> tag with other text on its line' -Expect 1 -Says 'XML examples: FAILED. 2 <code> tags' -Comment @(
    '/// <code>'
    '/// var key = new PropertyBagKey&lt;string&gt;("UserName");'
    '/// </code>'
    '/// <code>var other = 1;</code>'
)
# Every case passed.
exit 0
