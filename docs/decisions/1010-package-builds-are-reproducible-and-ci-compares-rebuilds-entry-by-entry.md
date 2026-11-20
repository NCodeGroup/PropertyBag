# 1010. Package builds are reproducible, and CI compares rebuilds entry by entry

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode.PropertyBag maintainers

## Context

Consumers, and the OpenSSF Best Practices criteria `build_repeatable` and `build_reproducible`, need a rebuild of a
commit or of a release tag to give the same packages as the original build.

The compiler is deterministic, and `ContinuousIntegrationBuild` is on in CI. A `.nupkg` is a zip file whose entries
carry the time they were packed, so two packs never match byte for byte. `build/ProjectTemplate.props` stamps the build
clock's year into `Copyright`, so a rebuild in a later year gives different packages.

## Decision

**Nothing a build puts out reads the clock, and CI compares rebuilds of the packages entry by entry.**

1. `Directory.Build.props` sets `Copyright` to `Copyright © NCode Group`, the text every C# file's header carries, so the
   build clock's year no longer reaches the packages.
2. `.github/workflows/reproducible-build.yml` packs every pull request and every push to `main` twice, from checkouts in
   different folders, and compares the two sets of packages.
3. Every Monday and on demand, the same workflow rebuilds the newest release's tag and compares the result with the
   packages attached to that release.
4. `build/Compare-Packages.ps1` compares the package file names, the entry names and the SHA-256 of each entry's bytes,
   not the zip entries' timestamps.
5. The jobs run on Linux, as `publish.yml` does.

## Options considered

- **Byte-compare the `.nupkg` files.** Rejected: two packs of one commit differ in the zip entries' timestamps alone.
- **Compare only the DLLs.** Rejected: it misses the `.nuspec`, the XML documentation and the PDBs.
- **Rewrite the zip timestamps when packing.** Rejected: it changes what ships.
- **Compare entry by entry** (chosen): every entry's name and bytes must match, and only the timestamps, which carry no
  content, are left out.

## Consequences

- A rebuild on Windows differs from one on Linux, because the generated sources embedded in the PDB get CRLF line
  endings.
- A rebuild uses the newest SDK that `global.json` allows, so a compiler change in a later SDK can make an older
  release's rebuild differ, which the weekly job reports.
- Releases before v2.0.2 carry the build year in `Copyright`, so their rebuilds match only within that year.

## References

- [#46](https://github.com/NCodeGroup/PropertyBag/issues/46)
- [NCodeGroup/project-template#454](https://github.com/NCodeGroup/project-template/issues/454)
- [project-template ADR-0130](project-template/0130-source-link-and-symbol-packages.md)
- `.github/workflows/publish.yml`, `.github/workflows/reproducible-build.yml`, `build/Compare-Packages.ps1`
