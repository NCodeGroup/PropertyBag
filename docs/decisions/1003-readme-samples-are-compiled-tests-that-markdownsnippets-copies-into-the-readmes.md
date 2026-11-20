# 1003. README samples are compiled tests that MarkdownSnippets copies into the READMEs

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode Group

## Context

A sample copied by hand into a README drifts from the API it shows, and nothing notices: a renamed member or a sample
that never compiled reaches readers unchecked. Three READMEs show samples: the repository's, and the two package
READMEs, which nuget.org renders.

## Decision

**Every README sample is a compiled xUnit test, MarkdownSnippets copies it into the READMEs, and `build/gate.ps1`
fails when a README differs from its test.**

1. The samples live in `tests/NCode.PropertyBag.Tests/ReadmeSamplesTests.cs`, one fact per sample named
   `<Sample>Sample_Runs` that asserts what the sample's comments claim, and in
   `tests/NCode.PropertyBag.Tests/ReadmeSamples/`, the sample types with their `using` lines, in namespace
   `ReadmeSamples`. These files depart from the test naming and region conventions, because they test the READMEs.
2. MarkdownSnippets.Tool, pinned in `.config/dotnet-tools.json` with `rollForward: false`, copies the samples with the
   `InPlaceOverwrite` convention and `OmitSnippetLinks` from `mdsnippets.json`.
3. `build/gate.ps1` runs `dotnet mdsnippets` and fails when it changes a Markdown file.
4. A sample is edited in its test, never in a README.

## Options considered

- Hand-copied samples. Rejected: nothing checks them.
- A custom test that parses README code blocks. Rejected: it re-implements what MarkdownSnippets does.
- Snippet-source links. Rejected: repository-relative links break on nuget.org.
- MarkdownSnippets `InPlaceOverwrite` (chosen): the READMEs keep their names, and each package README stays the package
  readme.

## Consequences

- The samples show CSharpier's formatting, so a long line wraps in the README.
- A README edit inside a `<!-- snippet: -->` block is overwritten by the next `dotnet mdsnippets`.
- Dependabot updates the tool with the other NuGet packages.
- A region name in a test needs a space, or MarkdownSnippets reads it as a snippet.

## References

- `tests/NCode.PropertyBag.Tests/ReadmeSamplesTests.cs`, `tests/NCode.PropertyBag.Tests/ReadmeSamples/`,
  `mdsnippets.json`, `build/gate.ps1`
- NCode.Buffers ADR-1005, which this follows.
- [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets)
