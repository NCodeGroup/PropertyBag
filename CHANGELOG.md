# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog 1.1.0](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Public API tracking with `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` in each packable project.
- Banned-API and null-forgiving-operator analyzers, and the `build/dod.ps1` Definition of Done script.
- `CONTRIBUTING.md`, `SECURITY.md` and architecture decision records in `docs/decisions/`.

### Changed

- Updated the project from project-template v0.41.0 to v0.61.2: CI now runs the Definition of Done, the NCode Review workflow is refreshed, and Dependabot groups security updates.
- Test helper types are sealed, as the new analyzers require.

[Unreleased]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.0...HEAD
