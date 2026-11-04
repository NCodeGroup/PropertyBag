# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog 1.1.0](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Public API tracking with `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` in each packable project. The recorded surface equals that of v1.2.0; the public API is unchanged.

### Changed

- Updated the project from project-template v0.41.0 to v0.61.2 (build, analyzers, CI and documentation); the library's behavior is unchanged.
- `AssemblyVersion` now moves only with the major version (for example `1.0.0.0` for every 1.x release), where it was previously the full version. `FileVersion` and `InformationalVersion` keep the exact version.

[Unreleased]: https://github.com/NCodeGroup/PropertyBag/compare/v1.2.0...HEAD
