# 1012. The template's CI job builds each commit twice, and the project's workflow rebuilds releases

- **Status:** Accepted
- **Date:** 2026-11-21
- **Deciders:** NCode.PropertyBag maintainers
- **Amends:** 1010

## Context

ADR-1010 has `Directory.Build.props` set `Copyright` without the year, and `.github/workflows/reproducible-build.yml`
packs every pull request and push to `main` twice and rebuilds the newest release every week.

The project template does the first two itself (project-template ADR-0154): `build/ProjectTemplate.props` sets
`Copyright` to `Copyright © NCode Group`, and the `reproducible-build` job of `.github/workflows/ci.yml` runs
`.template/Test-ReproducibleBuild.ps1`, which packs twice from clean `bin` and `obj` folders and compares the packages
entry by entry, on every pull request and push to `main`, and before `tag-release.yml` tags a release. The template
rebuilds no release.

## Decision

**The template's build and CI job make each commit's packages reproducible, and this project's workflow only rebuilds
releases.**

1. `Copyright` comes from `build/ProjectTemplate.props`; `Directory.Build.props` doesn't set it.
2. The `reproducible-build` job of `ci.yml` builds each pull request and push to `main` twice; `reproducible-build.yml`
   has no job that does.
3. ADR-1010's decisions 3 to 5 hold.

This amends ADR-1010's decisions 1 and 2.

## Options considered

- **Keep both per-commit checks.** Rejected: two jobs build every pull request twice for the same evidence.
- **Drop the template's job.** Rejected: `ci.yml` takes the template's changes, and `tag-release.yml` runs it before
  every release.
- **The template's job per commit plus the weekly release rebuild** (chosen).

## Consequences

- The per-commit check builds twice in one folder, so it doesn't vary the checkout's path; the weekly rebuild does,
  because it builds the tag in a folder other than the one `publish.yml` packed it in.
- Neither job is a required check.

## References

- [ADR-1010](1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md)
- [project-template ADR-0154](project-template/0154-a-library-build-takes-nothing-from-the-clock-and-ci-builds-it-twice.md)
- [NCodeGroup/project-template#454](https://github.com/NCodeGroup/project-template/issues/454)
- `.github/workflows/ci.yml`, `.github/workflows/reproducible-build.yml`, `.template/Test-ReproducibleBuild.ps1`
