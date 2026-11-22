# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog 1.1.0](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Package validation compares each package with the published 2.1.1 package.
- The project keeps one owner and aims for the OpenSSF Best Practices passing level: `GOVERNANCE.md`, `ROADMAP.md` and `docs/best-practices-badge.md` say so, and the silver and gold criteria that ask for a second owner or the owner's confirmation of the organization's two-factor methods stay unmet ([#50](https://github.com/NCodeGroup/PropertyBag/issues/50), [ADR-1014](docs/decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md)).
- Updated the project from project-template v0.111.0 to v0.111.3 (the `/best-practices` command, whose Branch-Protection row says the NCode Review Approver's approval meets the ruleset's required approval, and the template's update script); the update changes no behavior of the library ([#61](https://github.com/NCodeGroup/PropertyBag/issues/61)).

## [2.1.1] - 2026-11-21

### Changed

- Package validation compares each package with the published 2.1.0 package.
- Updated the project from project-template v0.108.0 to v0.111.0 (workflow commands, the baseline workflow, decision records and documentation); the update changes no behavior of the library.
- NCode Review approves a clean pull request as the NCode Review Approver, whose approval meets the `main` ruleset's required approval ([project-template ADR-0036](docs/decisions/project-template/0036-a-required-approval-is-met-by-ncode-reviews-approver-app.md)).
- The `reproducible-build` job of `.github/workflows/ci.yml` packs every pull request and push to `main` twice with `.template/Test-ReproducibleBuild.ps1`, `.github/workflows/reproducible-build.yml` only rebuilds the newest release every Monday, and the packages' copyright, `Copyright © NCode Group` as before, comes from the template's `build/ProjectTemplate.props` ([ADR-1012](docs/decisions/1012-the-template-ci-job-builds-each-commit-twice-and-the-project-workflow-rebuilds-releases.md)).
- After each stable release, `raise-baseline.yml` pushes the branch that raises the package validation baseline and opens an issue whose link opens its pull request, instead of opening a draft pull request ([project-template ADR-0036](docs/decisions/project-template/0036-a-required-approval-is-met-by-ncode-reviews-approver-app.md)).
- The public constructor of `DefaultPropertyBagScope` throws `ArgumentNullException` for a `null` property bag, instead of failing later in `Dispose` ([#47](https://github.com/NCodeGroup/PropertyBag/issues/47)).

## [2.1.0] - 2026-11-20

### Added

- `GetValue`, `GetValueOrDefault` and `ContainsKey` on `IReadOnlyPropertyBag`; `GetValue` throws `KeyNotFoundException` naming the key when the bag holds no value for it ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).
- `TryAdd`, `GetOrAdd` and `TryRemove` on `IPropertyBag`, each composed from `TryGetValue` and a write ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).
- `ConcurrentPropertyBagDictionary`, a bag that is safe from several threads, with atomic `TryAdd`, `GetOrAdd` and `TryRemove`, and enumeration over a snapshot ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).
- `LayeredPropertyBag` and `CreateChild()`: a child bag that reads through to its parent, keeps its own writes and removals, and never writes the parent ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).
- `ReadOnlyPropertyBag` and `AsReadOnly()`: a read-only view that cannot be cast back to `IPropertyBag` ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).
- The guides "Layer a child bag over a parent" and "Share a bag across threads" on the documentation site, and sections on the read and write helpers and protected views ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).

### Changed

- `PropertyBagKey.ToString()` and `PropertyBagKey<T>.ToString()` return the name and the value type, such as `UserName (System.String)`, where they returned the struct's type name, such as `NCode.PropertyBag.PropertyBagKey` ([ADR-1011](docs/decisions/1011-new-capabilities-are-extension-members-and-new-types-and-the-interfaces-stay-unchanged-within-a-major-version.md)).
- Package validation compares each package with the published 2.0.2 package.

## [2.0.2] - 2026-11-20

### Added

- FsCheck property tests for `PropertyBagKey`, `PropertyBagKey<T>`, `PropertyBagDictionary` and the name-inferring `SetValue` and `TryGetValue` extensions, with a fixed seed so every run is deterministic; OpenSSF Scorecard's Fuzzing check counts them.
- `.github/workflows/reproducible-build.yml` packs every pull request and push to `main` twice, from checkouts in different folders, and every Monday rebuilds the newest release's tag and compares it with the packages attached to the release; `build/Compare-Packages.ps1` compares two folders of packages entry by entry, and the gate tests it offline ([#46](https://github.com/NCodeGroup/PropertyBag/issues/46), [ADR-1010](docs/decisions/1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md)).
- The README shows the OpenSSF Best Practices badge of project 16954 ([#46](https://github.com/NCodeGroup/PropertyBag/issues/46)).
- `docs/assurance-case.md` lists the packages' inputs and the checks each makes, the assertions the tests run with, the regression tests of the scope fix and the reproducible-build evidence, and records a security review of the library and its release workflows; `docs/architecture.md` says the packages show people no text ([#46](https://github.com/NCodeGroup/PropertyBag/issues/46)).

### Changed

- Package validation compares each package with the published 2.0.1 package.
- Updated the project from project-template v0.104.0 to v0.108.0 (workflow commands, release rules and documentation); the update itself changes no behavior of the library.
- The packages' copyright is `Copyright © NCode Group`, without the year of the build, so a rebuild of a release's tag in a later year gives the same packages ([#46](https://github.com/NCodeGroup/PropertyBag/issues/46), [ADR-1010](docs/decisions/1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md)).
- `GOVERNANCE.md` names the maintainer, @polewskm, and the owners of the organization and the packages ([#46](https://github.com/NCodeGroup/PropertyBag/issues/46)).

## [2.0.1] - 2026-11-20

### Added

- A documentation site at https://propertybag.ncode.io/, built from `docs/site/` by the shared NCodeGroup/docs toolchain: a quick start, a guide for each task, the concepts behind keys and the default bag, and an API map of every public member, which the gate checks against both `PublicAPI.Shipped.txt` files. `.github/workflows/docs.yml` builds it on every pull request and publishes it from `main` ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1005](docs/decisions/1005-the-documentation-site-is-narrative-guides-built-by-a-shared-toolchain-outside-this-repository.md), [ADR-1008](docs/decisions/1008-the-documentation-site-is-published-to-github-pages-from-the-default-branch.md), [ADR-1009](docs/decisions/1009-the-documentation-site-is-served-at-propertybag-ncode-io.md)).
- Every C# sample on the site is a compiled test in `tests/NCode.PropertyBag.Site.Tests` that uses only the public API, and the gate checks every sample's marker and key ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1006](docs/decisions/1006-site-samples-are-compiled-tests-in-a-project-of-their-own-included-by-key-when-the-site-is-built.md)).
- The gate compiles every example in the XML documentation of `src/` against both packages (`build/Test-XmlDocExamples.ps1`), so each example IntelliSense shows builds as written ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37)).

### Changed

- Package validation compares each package with the published 2.0.0 package.
- Each package's project website on nuget.org is the documentation site, https://propertybag.ncode.io/, instead of the repository ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37), [ADR-1009](docs/decisions/1009-the-documentation-site-is-served-at-propertybag-ncode-io.md)).
- The repository README and both package READMEs link to the documentation site ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37)).
- The XML documentation of `IReadOnlyPropertyBag.Clone` says that how values are copied is up to the implementation, and that the default implementation clones each `ICloneable` value, as the README and the documentation site say; it described every copy as shallow.
- The README samples check is `build/Assert-Markdown.ps1`, which the gate tests offline first; it reads file names as git stores them and fails when git fails, where a failure read as "nothing changed" ([#37](https://github.com/NCodeGroup/PropertyBag/issues/37)).

## [2.0.0] - 2026-11-20

### Added

- Each package has its own README on nuget.org, and the README samples are compiled and run as tests, so each one builds as written ([ADR-1003](docs/decisions/1003-readme-samples-are-compiled-tests-that-markdownsnippets-copies-into-the-readmes.md)).
- Public API tracking with `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` in each packable project.
- Each package is validated against version 1.2.0 on nuget.org when it is packed (`PackageValidationBaselineVersion`), so a binary-breaking change to the public API fails the pack.
- `CODE_OF_CONDUCT.md`, `GOVERNANCE.md`, `ROADMAP.md`, `docs/architecture.md` and `docs/assurance-case.md`.
- Each release's packages carry a signed build-provenance attestation: after the push to nuget.org, `.github/workflows/publish.yml` attaches the packages, the provenance file (`<tag>.intoto.jsonl`) and an SPDX SBOM (`<tag>.spdx.json`) to the GitHub release, and `gh attestation verify <package> --repo NCodeGroup/PropertyBag` checks a package ([#30](https://github.com/NCodeGroup/PropertyBag/issues/30)).
- After each stable release, CI opens a draft pull request that raises the package validation baseline to the released version ([#30](https://github.com/NCodeGroup/PropertyBag/issues/30)).
- `docs/assurance-case.md` states the security requirements, the threat model, the secure design principles applied and the weaknesses countered, and `docs/architecture.md` describes the components ([#31](https://github.com/NCodeGroup/PropertyBag/issues/31)).
- Code coverage: `./build/Test-Coverage.ps1` fails below 98% line or branch coverage and CI runs it on every pull request, and `.github/workflows/coverage.yml` publishes the report to Codecov from every push to `main`. The tests cover `PropertyBagFactory`, `DefaultRegistration`, scopes over a key that held a value, and an empty bag's enumeration and count.
- `./build/gate.ps1` checks that the READMEs' samples match the tests, then runs the Definition of Done, whose Format stage checks the formatting with CSharpier, pinned in `.config/dotnet-tools.json`; CI and the local gate run it.
- Each release's tag is signed with gitsign, and `SECURITY.md` says how to verify it ([#24](https://github.com/NCodeGroup/PropertyBag/issues/24)).

### Changed

- **Breaking:** `IPropertyBag.Set<T>(key, value)` is `IPropertyBag.SetValue<T>(key, value)`, and the extensions `Set<T>(value)` and `TryGet<T>(out value)` are `SetValue<T>(value)` and `TryGetValue<T>(out value)`; `TryGetValue` is in the new `ReadOnlyPropertyBagExtensions` class. Rename `Set` to `SetValue` and `TryGet` to `TryGetValue` at each call site ([ADR-1004](docs/decisions/1004-public-names-pass-the-naming-analyzers-without-suppressions.md)).
- **Breaking:** `DefaultPropertyBag` is `PropertyBagDictionary`; rename it where code names the type ([ADR-1004](docs/decisions/1004-public-names-pass-the-naming-analyzers-without-suppressions.md)).
- **Breaking:** `PropertyBagDictionary`, `DefaultPropertyBagFactory` and `DefaultPropertyBagScope` are sealed; compose them instead of deriving from them.
- `SetValue(value)`, `TryGetValue(out value)` and `AddPropertyBag()` throw `ArgumentNullException` for a `null` bag or service collection.
- The README is rewritten around the 2.0 API, with a usage section per feature and an API overview, and the package descriptions and tags describe each package.
- Updated the project from project-template v0.41.0 to v0.104.0 (build, analyzers, CI and documentation); the update itself changes no behavior of the library.
- `AssemblyVersion` moves only with the major version: it is `2.0.0.0` for every 2.x release, while `FileVersion` and `InformationalVersion` keep the exact version.
- Package and assembly versions come from `version.json` through Nerdbank.GitVersioning instead of the release tag: a release sets `version.json` to its version first, and CI refuses a release whose tag differs.
- The solution file is `NCode.PropertyBag.slnx`, and `dotnet build` no longer creates packages: `./build/dod.ps1` and CI pack with `dotnet pack` ([#29](https://github.com/NCodeGroup/PropertyBag/issues/29)).
- Each package's description and tags are set in its project file and describe that package, since the `project.props` files that held them were no longer imported; the project files keep only the settings `build/ProjectTemplate.props` doesn't make, and no file starts with a byte order mark ([#29](https://github.com/NCodeGroup/PropertyBag/issues/29)).
- The projects are in `src/` and the test project in `tests/`, the layout the project template gives .NET projects; moving them changes no package contents.
- Releases are tagged and published when their pull request merges, and the packages reach nuget.org through trusted publishing, with no stored API key, from `.github/workflows/publish.yml` after a maintainer approves the `nuget` environment; `.github/workflows/main.yml` and the `NUGET_APIKEY` secret it used are gone ([#24](https://github.com/NCodeGroup/PropertyBag/issues/24)).
- Every C# file starts with `// Copyright © NCode Group` and `// SPDX-License-Identifier: Apache-2.0` in place of the Apache license preamble, and CSharpier formats the code.
- `PropertyBagKey<T>` compares two keys by name, since both always have the type `T`; the result of `Equals` does not change.
- The project's icon is the NCode brand icon the other NCode packages carry.

### Removed

- **Breaking:** The `net8.0` target: both packages target `net10.0`, since .NET 8 is out of support. Applications on .NET 8 stay on 1.2.0 ([ADR-1002](docs/decisions/1002-the-packages-target-only-supported-lts-dotnet-versions.md)).

### Fixed

- **Breaking:** Disposing the scope that `PropertyBagDictionary.Scope` returns restores the value its key held before, as `IPropertyBag.Scope`, `IPropertyBagScope` and the README say, instead of removing the key; a key that held no value is still removed, and a second `Dispose` does nothing. Code that relied on the key being removed after a scope over an existing value calls `Remove` after disposing the scope ([ADR-1000](docs/decisions/1000-disposing-a-scope-restores-the-previous-value.md)).

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

[unreleased]: https://github.com/NCodeGroup/PropertyBag/compare/v2.1.1...HEAD
[2.1.1]: https://github.com/NCodeGroup/PropertyBag/compare/v2.1.0...v2.1.1
[2.1.0]: https://github.com/NCodeGroup/PropertyBag/compare/v2.0.2...v2.1.0
[2.0.2]: https://github.com/NCodeGroup/PropertyBag/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/NCodeGroup/PropertyBag/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.0...v2.0.0
[1.2.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.1.1...v1.2.0
[1.1.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/NCodeGroup/PropertyBag/releases/tag/v1.0.0
