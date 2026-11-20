# 1002. The packages target only supported LTS .NET versions

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode Group

## Context

A target framework that Microsoft no longer supports gets no security fixes. Microsoft's
[.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core), read 2026-11-20, lists .NET 8
as out of support since 2026-11-10, and .NET 10, a Long Term Support release, as supported until 2028-11-14. The test
project targets only `net10.0`, so a `net8.0` build ships untested. Every target framework is one more build to
restore, test and pack. NCode.Buffers decides the same in its ADR-1004.

## Decision

**NCode.PropertyBag.Abstractions and NCode.PropertyBag target only the Long Term Support .NET versions Microsoft
supports: `net10.0` alone.**

1. Each project file sets `<TargetFramework>net10.0</TargetFramework>`.
2. Removing a target framework is a breaking change: it ships in a new major version, and its `PKV006` package
   validation error is recorded in the project's `CompatibilitySuppressions.xml` until the validation baseline reaches
   that major version.
3. A Standard Term Support version, such as .NET 11, is not targeted.
4. Documentation names no .NET version: the project files are the one source, which the READMEs show through a badge.

## Options considered

- Keep `net8.0`. Rejected: the packages keep shipping, untested, for a runtime without security fixes.
- Also target Standard Term Support versions. Rejected: each adds one more build for 24 months of support.
- Target only supported LTS versions (chosen).

## Consequences

- Applications on .NET 8 stay on 1.x; 2.0 needs .NET 10.
- One target framework builds, tests and packs.
- Dropping .NET 10 once it leaves support is another major version.

## References

- `src/NCode.PropertyBag.Abstractions/NCode.PropertyBag.Abstractions.csproj`, `src/NCode.PropertyBag/NCode.PropertyBag.csproj`
- [ADR-0136](project-template/0136-packages-follow-semantic-versioning-and-the-assembly-version-moves-only-with-the-major.md):
  packages follow Semantic Versioning and the assembly version moves only with the major.
- [ADR-0147](project-template/0147-packages-are-validated-against-the-last-published-package.md): packages are
  validated against the last published package.
