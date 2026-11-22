# 1015. The packages depend on their target framework's release band

- **Status:** Accepted
- **Date:** 2026-11-22
- **Deciders:** NCode Group
- **Amends:** 1002

## Context

NCode.PropertyBag and NCode.PropertyBag.Abstractions target `net10.0` alone, the Long Term Support release
([ADR-1002](1002-the-packages-target-only-supported-lts-dotnet-versions.md)). NCode.PropertyBag depends on
`Microsoft.Extensions.DependencyInjection.Abstractions`, and a package's dependency version is a floor that sets the
version for every application that references it. The Microsoft.Extensions 11.0 packages carry a `net10.0` asset, so
restore accepts them, but they belong to .NET 11, a Standard Term Support release. An ASP.NET Core application on
.NET 10 runs on the `Microsoft.AspNetCore.App` 10 shared framework, which carries the 10.0 Microsoft.Extensions
assemblies; a package above that version wins over the shared framework and ships with the application, beside every
other Microsoft.Extensions, ASP.NET Core and EF Core assembly at 10, a combination neither release tests, outside the
shared framework's servicing. NCodeGroup/identity enforces one band in its hosts and lists this package's floor as its
one exception ([NCodeGroup/identity#895](https://github.com/NCodeGroup/identity/issues/895)). Dependabot's `nuget`
group offers major updates with the rest.

## Decision

**Every Microsoft.Extensions package the packages depend on is on the release band of their target framework: major
version 10 for `net10.0`.**

1. `Directory.Packages.props` pins the Microsoft.Extensions packages at one patch of the band, the newest past the
   cooldown.
2. Dependabot ignores semver-major updates of `Microsoft.Extensions.*`: the band moves with the target framework, as
   one planned migration, together with ADR-1002's target.
3. A newer band is never taken for a feature: a capability that needs one waits for the target framework that carries
   it.

## Options considered

- Take each newest Microsoft.Extensions major. Rejected: it raises every consumer's floor above its shared framework,
  so an ASP.NET Core 10 application runs a mixed band.
- Multi-target `net10.0` and `net11.0` with a dependency group each. Rejected: ADR-1002 targets no Standard Term
  Support release.
- Depend on the target framework's band (chosen).

## Consequences

- A consumer on .NET 10 resolves the dependency-injection abstractions of its own shared framework.
- Lowering the floor from 11.0.0 to 10.0.15 breaks no consumer, so it ships in a patch release.
- Moving to the next LTS target changes the target framework, the Microsoft.Extensions pins and this ignore rule in
  one migration.

## References

- `Directory.Packages.props`
- `.github/dependabot.yml`, the `nuget` entry's `ignore` rule
- [ADR-1002](1002-the-packages-target-only-supported-lts-dotnet-versions.md)
- [#70](https://github.com/NCodeGroup/PropertyBag/issues/70)
