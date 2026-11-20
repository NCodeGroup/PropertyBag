# Architecture

[//]: # "Maintainers: describe the components, as a diagram and a table (Component, Role), and the rules the design
keeps."

How NCode.PropertyBag is built. [assurance-case.md](assurance-case.md) explains why the design meets its security
requirements.

## Components

Not described yet.

## Design Rules

- Decisions the design rests on are recorded in [decisions/](decisions/README.md).
- Warnings are errors ([ADR-0100](decisions/project-template/0100-warnings-are-errors-in-every-project.md)), and
  disallowed APIs are banned at build time
  ([ADR-0102](decisions/project-template/0102-disallowed-apis-are-banned-at-build-time.md)).
- The public API is tracked and evolves compatibly
  ([ADR-0131](decisions/project-template/0131-public-api-is-tracked-and-evolves-compatibly.md)), and each package is
  validated against the last published package
  ([ADR-0147](decisions/project-template/0147-packages-are-validated-against-the-last-published-package.md)).

## Build and Release

CI runs the project's commands on every pull request (`.github/workflows/ci.yml`), and `.github/workflows/main.yml`
builds, tests and packs every push and pull request too. A maintainer publishes each release as a GitHub release, once
`version.json` holds its version
([ADR-0138](decisions/project-template/0138-package-versions-come-from-version-json-through-nerdbank-gitversioning.md));
the `publish` job of `main.yml` then pushes the release's packages to nuget.org with the repository's NuGet API key.
Its `provenance` job then calls `release-provenance.yml`, which attests the packages and attaches them, a signed
provenance file and an SBOM to the release
([ADR-0139](decisions/project-template/0139-releases-carry-build-provenance.md)), and for a stable version its
`baseline` job calls `raise-baseline.yml`, which opens the pull request that raises the package validation baseline
([ADR-0150](decisions/project-template/0150-a-workflow-raises-the-package-validation-baseline-after-each-release.md),
[ADR-0151](decisions/project-template/0151-release-follow-ups-are-jobs-publish-calls.md)).
