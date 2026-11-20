# 1002. README samples are compiled tests that MarkdownSnippets copies into the README

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode.PropertyBag maintainers

## Context

A sample copied by hand into a README drifts from the API it shows, and nothing notices. The convenience-extensions
sample of `README.md` declared `connectionString` twice and did not compile
([#37](https://github.com/NCodeGroup/PropertyBag/issues/37)). `README.md` is the readme of both packages, and it is
the page nuget.org renders.

## Decision

**Every README sample is a compiled xUnit test, MarkdownSnippets copies it into `README.md`, and `build/gate.ps1`
fails when the README differs from its tests.**

1. The samples live in `tests/NCode.PropertyBag.Tests/ReadmeSamplesTests.cs`, one fact per sample between
   `// begin-snippet:` and `// end-snippet`, asserting what the sample's comments claim, and in
   `tests/NCode.PropertyBag.Tests/ReadmeSamples/MyService.cs`, the sample service with its `using` lines, in namespace
   `ReadmeSamples`. Each sample's key is `Readme<Name>`. Both files depart from the test convention of mirroring one
   production type and nesting sample types, because they test the README. Each sample's fact is named
   `<Key>Sample_Runs` and asserts what that one sample claims, and the facts are grouped in regions named for the
   README's sections, instead of the `Method_State_Expected` names and `{MethodName} Tests` regions of the test
   conventions.
2. MarkdownSnippets.Tool 29.0.0, pinned in `.config/dotnet-tools.json` with `rollForward: false`, copies the samples
   with the `InPlaceOverwrite` convention and `OmitSnippetLinks` from `mdsnippets.json`. The file excludes `*.mdx` and
   `*.astro`, so the site's pages and components are left as they are
   ([ADR-1004](1004-site-samples-are-compiled-tests-in-a-project-of-their-own-included-by-key-when-the-site-is-built.md)).
3. `build/gate.ps1` runs `dotnet mdsnippets` and fails when it changes a file.
4. A sample is edited in its test, then `dotnet mdsnippets` is run, never in the README.

## Options considered

- Hand-copied samples. Rejected: nothing checks them.
- A test that compares README code blocks with source. Rejected: custom parsing that MarkdownSnippets already does.
- MarkdownSnippets `SourceTransform` with a `README.source.md` file. Rejected: two files for one README and a
  generated header.
- Snippet-source links. Rejected: repository-relative links break on nuget.org.
- MarkdownSnippets `InPlaceOverwrite` (chosen): the README keeps its name, and stays the package readme.

## Consequences

- The samples show CSharpier's formatting, so a long line wraps in the README.
- A README edit inside a `<!-- snippet: -->` block is overwritten by the next `dotnet mdsnippets`.
- Dependabot updates the tool with the other NuGet packages.
- A region name in a test needs a space, or MarkdownSnippets reads it as a snippet.

## References

- `tests/NCode.PropertyBag.Tests/ReadmeSamplesTests.cs`, `tests/NCode.PropertyBag.Tests/ReadmeSamples/MyService.cs`,
  `mdsnippets.json`, `build/gate.ps1`
- [#37](https://github.com/NCodeGroup/PropertyBag/issues/37)
- [Project-template ADR-0148](project-template/0148-csharp-is-formatted-in-the-gate-and-files-carry-the-spdx-header.md):
  CSharpier formats the code, and the gate checks it.
- [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets)
