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

[Unreleased]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.0...HEAD
