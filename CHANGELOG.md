# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog 1.1.0](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Public API tracking with `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` in each packable project. The recorded surface equals that of v1.2.0; the public API is unchanged.
- Each package is validated against version 1.2.0 on nuget.org when it is packed (`PackageValidationBaselineVersion`), so a binary-breaking change to the public API fails the pack.
- `CODE_OF_CONDUCT.md`, `GOVERNANCE.md`, `ROADMAP.md`, `docs/architecture.md` and `docs/assurance-case.md`.

### Changed

- Updated the project from project-template v0.41.0 to v0.104.0 (build, analyzers, CI and documentation); the library's behavior is unchanged.
- `AssemblyVersion` now moves only with the major version: every later 1.x release keeps `1.2.0.0`, the `AssemblyVersion` v1.2.0 shipped with, and 2.0.0 moves it to `2.0.0.0`; before, it was the full version. `FileVersion` and `InformationalVersion` keep the exact version.
- Package and assembly versions come from `version.json` through Nerdbank.GitVersioning instead of the release tag: a release sets `version.json` to its version first, and CI refuses a release whose tag differs.

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

[unreleased]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.1.1...v1.2.0
[1.1.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/NCodeGroup/PropertyBag/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/NCodeGroup/PropertyBag/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/NCodeGroup/PropertyBag/releases/tag/v1.0.0
