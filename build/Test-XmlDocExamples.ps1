# Copyright © NCode Group
# SPDX-License-Identifier: Apache-2.0

#requires -version 7
<#
.SYNOPSIS
    Compiles every example in the XML documentation of src/, so each example the packages ship to IntelliSense
    compiles as written.
.DESCRIPTION
    Each <code> block of a /// comment becomes its own static class in artifacts/xml-examples/Examples.cs: a type or
    member the example declares with a leading `public` stays at class scope, and the other lines become a method body,
    with #line directives that point each error at its source line. The script builds that file against both packages.
    A <code> tag that is not alone on its /// line fails the check, so no example is skipped. An example may use
    `propertyBag` (an IPropertyBag), `services` (an IServiceCollection), `userName` (a string), `GetPropertyBag()` and
    the class `CustomPropertyBagFactory`; the script declares them. build/gate.ps1 runs it.
#>
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$source = Join-Path $root 'src'
$project = Join-Path $root 'src/NCode.PropertyBag/NCode.PropertyBag.csproj'
# Under the repository, so its global.json and nuget.config apply; artifacts/ is ignored by git and CSharpier.
$out = Join-Path $root 'artifacts/xml-examples'
New-Item -ItemType Directory -Force -Path $out | Out-Null

$framework = @(([xml](Get-Content -Raw -LiteralPath $project)).Project.PropertyGroup.TargetFramework | Where-Object { $_ })[0]
if (-not $framework) { throw "XML examples: $project names no TargetFramework." }

$code = [Text.StringBuilder]::new()
foreach ($line in 'using Microsoft.Extensions.DependencyInjection;', 'using NCode.PropertyBag;', 'using static Context;',
    '#pragma warning disable', 'internal static class Context', '{', '    public static IPropertyBag propertyBag = null!;',
    '    public static IServiceCollection services = null!;', '    public static string userName = "";',
    '    public static IReadOnlyPropertyBag GetPropertyBag() => propertyBag;', '}',
    'internal sealed class CustomPropertyBagFactory : IPropertyBagFactory', '{',
    '    public IPropertyBag Create() => PropertyBagFactory.Create();', '}') { [void]$code.AppendLine($line) }

# Splits an example into the declarations that stay at class scope, each a run of lines from a leading `public` at
# the example's own indent to the line where its braces balance and it ends with `;` or `}`, and the body lines.
function Split-Example {
    param([string[]]$Lines, [int]$FirstLine)
    $declarations = [Collections.Generic.List[object]]::new()
    $body = [Collections.Generic.List[object]]::new()
    for ($i = 0; $i -lt $Lines.Length; $i++) {
        if ($Lines[$i] -cmatch '^public ') {
            $start = $i
            $depth = 0
            for (; $i -lt $Lines.Length; $i++) {
                $depth += ([regex]::Matches($Lines[$i], '\{')).Count - ([regex]::Matches($Lines[$i], '\}')).Count
                if ($depth -le 0 -and $Lines[$i].TrimEnd() -match '[;}]$') { break }
            }
            $declarations.Add([pscustomobject]@{ Line = $FirstLine + $start; Text = $Lines[$start..([Math]::Min($i, $Lines.Length - 1))] })
        }
        else { $body.Add([pscustomobject]@{ Line = $FirstLine + $i; Text = $Lines[$i] }) }
    }
    [pscustomobject]@{ Declarations = $declarations; Body = $body }
}

$count = 0
foreach ($file in Get-ChildItem -LiteralPath $source -Filter *.cs -Recurse | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | Sort-Object FullName) {
    $lines = [IO.File]::ReadAllLines($file.FullName)
    $relative = [IO.Path]::GetRelativePath($root, $file.FullName) -replace '\\', '/'
    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ($lines[$i].Trim() -cne '/// <code>') { continue }
        $start = $i + 2
        $example = [Collections.Generic.List[string]]::new()
        for ($i++; $i -lt $lines.Length -and $lines[$i].Trim() -cne '/// </code>'; $i++) {
            $text = $lines[$i] -replace '^\s*/// ?', ''
            $example.Add($text.Replace('&lt;', '<').Replace('&gt;', '>').Replace('&amp;', '&'))
        }
        if ($i -ge $lines.Length) { throw "XML examples: the <code> block at $relative line $($start - 1) has no </code>." }
        $count++
        $parts = Split-Example -Lines $example.ToArray() -FirstLine $start
        [void]$code.AppendLine("internal static class Example$count")
        [void]$code.AppendLine('{')
        # The compiler opens the file a #line directive names, so it takes the absolute path.
        foreach ($declaration in $parts.Declarations) {
            [void]$code.AppendLine("#line $($declaration.Line) `"$($file.FullName)`"")
            foreach ($text in $declaration.Text) { [void]$code.AppendLine(($text -creplace '^public static ', 'internal static ')) }
            [void]$code.AppendLine('#line default')
        }
        [void]$code.AppendLine('    private static void Run()')
        [void]$code.AppendLine('    {')
        $next = -1
        foreach ($row in $parts.Body) {
            if ($row.Line -ne $next) { [void]$code.AppendLine("#line $($row.Line) `"$($file.FullName)`"") }
            [void]$code.AppendLine($row.Text)
            $next = $row.Line + 1
        }
        [void]$code.AppendLine('#line default')
        [void]$code.AppendLine('    }')
        [void]$code.AppendLine('}')
    }
}
if (-not $count) { throw "XML examples: $source has no <code> block; the extraction is broken." }
# The loop above takes only a <code> tag alone on its /// line; fail on any other instead of dropping its example.
$tags = @(Get-ChildItem -LiteralPath $source -Filter *.cs -Recurse | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
    Select-String -Pattern '^\s*///.*<code\b' -CaseSensitive)
if ($tags.Count -ne $count) {
    "XML examples: FAILED. $($tags.Count) <code> tags in $source, but $count examples extracted; put each <code> and </code> alone on its /// line:"
    $tags | Where-Object { $_.Line.Trim() -cne '/// <code>' } | ForEach-Object { "  $([IO.Path]::GetRelativePath($root, $_.Path) -replace '\\', '/'):$($_.LineNumber)" }
    exit 1
}

$utf8 = [Text.UTF8Encoding]::new($false)
[IO.File]::WriteAllText((Join-Path $out 'Examples.cs'), $code.ToString(), $utf8)
$reference = [IO.Path]::GetRelativePath($out, $project)
[IO.File]::WriteAllText((Join-Path $out 'Examples.csproj'), @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>$framework</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="$reference" />
  </ItemGroup>
</Project>
"@, $utf8)
# The scratch project takes none of the repository's build settings: it builds the examples as a consumer would.
foreach ($name in 'Directory.Build.props', 'Directory.Build.targets', 'Directory.Packages.props') {
    [IO.File]::WriteAllText((Join-Path $out $name), "<Project />`n", $utf8)
}

dotnet build (Join-Path $out 'Examples.csproj') --nologo -v quiet
if ($LASTEXITCODE) { "XML examples: FAILED. An example in the XML documentation of $source does not compile; the errors above name its file and line."; exit 1 }
"XML examples: PASSED ($count examples)."
exit 0
