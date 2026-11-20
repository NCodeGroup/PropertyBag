# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog 1.1.0](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.2.1] - 2026-11-20

### Added

- Public API tracking with `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` in each packable project. The recorded surface equals that of v1.2.0; the public API is unchanged.
- Each package is validated against version 1.2.0 on nuget.org when it is packed (`PackageValidationBaselineVersion`), so a binary-breaking change to the public API fails the pack.
- `CODE_OF_CONDUCT.md`, `GOVERNANCE.md`, `ROADMAP.md`, `docs/architecture.md` and `docs/assurance-case.md`.
- Each release's packages carry a signed build-provenance attestation: after the push to nuget.org, `.github/workflows/publish.yml` attaches the packages, the provenance file (`<tag>.intoto.jsonl`) and an SPDX SBOM (`<tag>.spdx.json`) to the GitHub release, and `gh attestation verify <package> --repo NCodeGroup/PropertyBag` checks a package ([#30](https://github.com/NCodeGroup/PropertyBag/issues/30)).
- After each stable release, CI opens a draft pull request that raises the package validation baseline to the released version ([#30](https://github.com/NCodeGroup/PropertyBag/issues/30)).
- `docs/assurance-case.md` states the security requirements, the threat model, the secure design principles applied and the weaknesses countered, and `docs/architecture.md` describes the components ([#31](https://github.com/NCodeGroup/PropertyBag/issues/31)).
- Code coverage: `./build/Test-Coverage.ps1` fails below 98% line or branch coverage and CI runs it on every pull request, and `.github/workflows/coverage.yml` publishes the report to Codecov from every push to `main`. The tests cover `PropertyBagFactory`, `DefaultRegistration`, scopes over a key that held a value, and an empty bag's enumeration and count.
- `./build/gate.ps1` checks the formatting with CSharpier, pinned in `.config/dotnet-tools.json`, before the Definition of Done; CI and the local gate run it.
- Each release's tag is signed with gitsign, and `SECURITY.md` says how to verify it ([#24](https://github.com/NCodeGroup/PropertyBag/issues/24)).
- A documentation site at https://propertybag.ncode.io/, built from `docs/site/` by the shared NCodeGroup/docs toolchain: a quick start, a guide for each task, the concepts behind keys and the default bag, and an API map of every public member, which the gate checks against both `PublicAPI.Shipped.txt` files. `.github/workflows/docs.yml` builds it on every pull request and publishes it from `main` ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1003](docs/decisions/1003-the-documentation-site-is-narrative-guides-built-by-a-shared-toolchain-outside-this-repository.md), [ADR-1006](docs/decisions/1006-the-documentation-site-is-published-to-github-pages-from-the-default-branch.md), [ADR-1007](docs/decisions/1007-the-documentation-site-is-served-at-propertybag-ncode-io.md)).
- Every C# sample on the site is a compiled test in `tests/NCode.PropertyBag.Site.Tests` that uses only the public API, and the gate checks every sample's marker and key ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1004](docs/decisions/1004-site-samples-are-compiled-tests-in-a-project-of-their-own-included-by-key-when-the-site-is-built.md)).
- The C# samples in `README.md` are compiled tests that `dotnet mdsnippets` copies into it, and the gate fails when the README and the tests differ ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1002](docs/decisions/1002-readme-samples-are-compiled-tests-that-markdownsnippets-copies-into-the-readme.md)).

### Changed

- Updated the project from project-template v0.41.0 to v0.104.0 (build, analyzers, CI and documentation); the library's behavior is unchanged.
- `AssemblyVersion` now moves only with the major version: every later 1.x release keeps `1.2.0.0`, the `AssemblyVersion` v1.2.0 shipped with, and 2.0.0 moves it to `2.0.0.0`; before, it was the full version. `FileVersion` and `InformationalVersion` keep the exact version.
- Package and assembly versions come from `version.json` through Nerdbank.GitVersioning instead of the release tag: a release sets `version.json` to its version first, and CI refuses a release whose tag differs.
- The solution file is `NCode.PropertyBag.slnx`, and `dotnet build` no longer creates packages: `./build/dod.ps1` and CI pack with `dotnet pack` ([#29](https://github.com/NCodeGroup/PropertyBag/issues/29)).
- Each package's description and tags are set in its project file, as v1.2.0 published them, since the `project.props` files that held them were no longer imported; the project files keep only the settings `build/ProjectTemplate.props` doesn't make, and no file starts with a byte order mark ([#29](https://github.com/NCodeGroup/PropertyBag/issues/29)).
- The projects are in `src/` and the test project in `tests/`, the layout the project template gives .NET projects; the packages and their contents are unchanged.
- Releases are tagged and published when their pull request merges, and the packages reach nuget.org through trusted publishing, with no stored API key, from `.github/workflows/publish.yml` after a maintainer approves the `nuget` environment; `.github/workflows/main.yml` and the `NUGET_APIKEY` secret it used are gone ([#24](https://github.com/NCodeGroup/PropertyBag/issues/24)).
- Every C# file starts with `// Copyright © NCode Group` and `// SPDX-License-Identifier: Apache-2.0` in place of the Apache license preamble, and CSharpier formats the code.
- `PropertyBagKey<T>` compares two keys by name, since both always have the type `T`; the result of `Equals` does not change.
- The project's icon is the NCode brand icon the other NCode packages carry.
- Each package's project website on nuget.org is the documentation site, https://propertybag.ncode.io/, instead of the repository ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1007](docs/decisions/1007-the-documentation-site-is-served-at-propertybag-ncode-io.md)).
- `README.md` links to the documentation site, lists `IPropertyBagFactory` under `NCode.PropertyBag`, the package that holds it, and leaves the release history to this changelog; its convenience-extensions sample compiles, where it declared `connectionString` twice ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37)).

### Fixed

- Disposing the scope that `DefaultPropertyBag.Scope` returns restores the value its key held before, as `IPropertyBag.Scope`, `IPropertyBagScope` and the README say, instead of removing the key; a key that held no value is still removed, and a second `Dispose` does nothing ([ADR-1000](docs/decisions/1000-disposing-a-scope-restores-the-previous-value.md)).

## [1.2.0] - 2026-01-19

### Added

- `IPropertyBag.Set`, `IPropertyBag.Scope` and the `Set` extension accept `null` for reference types and nullable value types and store it explicitly, which differs from removing the key.

### Changed

- **Breaking:** `TryGetValue` and the `TryGet` extension return `true`, with a `null` value, for a key that holds `null`; before, they returned `false`. Check the value for `null` where a stored `null` should count as missing.

## [1.1.1] - 2026-01-18

### Fixed

- The `TryGet` extension's documentation example declares its variable before the call: an inline `out string? userName` declaration gives `CallerArgumentExpression` a key name other than the variable's.

## [1.1.0] - 2026-01-18

### Added

- Both packages target .NET 8.0 as well as .NET 10.0.

## [1.0.1] - 2026-01-18

### Fixed

- Both packages carry the NCode Group icon, which v1.0.0 left out because line-ending normalization had corrupted the image.

## [1.0.0] - 2026-01-18

### Added

- `NCode.PropertyBag.Abstractions`: `IPropertyBag`, `IReadOnlyPropertyBag`, `IPropertyBagScope`, the strongly typed keys `PropertyBagKey` and `PropertyBagKey<T>`, and the `Set` and `TryGet` extensions that name the key after the caller's argument expression.
- `NCode.PropertyBag`: `DefaultPropertyBag`, with lazy storage, scoped values and cloning, `DefaultPropertyBagScope`, the factories `IPropertyBagFactory`, `DefaultPropertyBagFactory` and `PropertyBagFactory`, and `AddPropertyBag()` for `Microsoft.Extensions.DependencyInjection`.
- Both packages target .NET 10.0.

[unreleased]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.1...HEAD
[1.2.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.1.1...v1.2.0
[1.1.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/NCodeGroup/PropertyBag/releases/tag/v1.0.0
