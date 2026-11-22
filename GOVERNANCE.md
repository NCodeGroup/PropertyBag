# Governance

NCode.PropertyBag belongs to NCode Group, under `Apache-2.0`. This page says who does
what and how decisions are made.

## Roles

[//]: # "Maintainers: name each maintainer in the Who column, such as @octocat."

| Role | Who | Responsibilities |
| --- | --- | --- |
| Maintainer | [@polewskm](https://github.com/polewskm) | Sets direction, triages issues, reviews and merges pull requests, approves each publication in the `nuget` environment, answers vulnerability reports, owns the repository's settings and the nuget.org packages |
| Contributor | Anyone | Opens issues and pull requests, under [CONTRIBUTING.md](CONTRIBUTING.md) and the [Code of Conduct](CODE_OF_CONDUCT.md) |
| Automation | `ncode-review[bot]`, `ncode-review-approver[bot]`, Dependabot, GitHub Actions | Reviews a pull request and approves a clean one, proposes dependency updates, runs CI, tags and publishes releases; never merges a pull request except the pin bump `code-review-auto-merge.yml` merges once the required checks pass and, where the ruleset requires an approval, a maintainer approves it |

Only a maintainer has write access. A contributor who keeps contributing reviewed, merged work can be invited to become
a maintainer; the existing maintainers decide, and the change is recorded in this file.

## Decisions

- Day-to-day decisions are made in issues and pull requests. Anyone can comment; a maintainer decides.
- A decision someone would otherwise re-argue is recorded as an architecture decision record in
  [docs/decisions/](docs/decisions/README.md), before or with the pull request that builds it. A change consumers can
  observe, such as to the public API, behavior or security posture, starts as an issue.
- Where maintainers disagree, they discuss it on the issue; with no agreement, the change doesn't happen.

## Releases

Versions follow Semantic Versioning. A release is tagged and published when its pull request merges, which
sets `version.json` to the release's version, and its packages reach nuget.org only after a maintainer approves the
`nuget` environment. `.github/workflows/publish.yml` then attaches the packages, a signed build-provenance file and an
SBOM to the release, and for a stable version pushes the branch that raises the package validation baseline and
opens an issue whose link opens its pull request, which a maintainer merges once CI passes.
[SECURITY.md](SECURITY.md) says how to verify a package.

## Continuity

The repository, its issues, decision records, build, release automation and documentation are all in the
GitHub organization that owns it, so anyone given access can carry
on with no knowledge held elsewhere. nuget.org publishing uses trusted publishing, with no stored API key to hand over.

[//]: # "Maintainers: name the owners, and keep the list current."

Owners today, as of 2026-11-20: [@polewskm](https://github.com/polewskm) is the one owner of the NCodeGroup GitHub
organization and the one administrator of this repository, and the nuget.org packages belong to the nuget.org account
`NCode`. The project keeps one owner and adds no second one
([ADR-1014](docs/decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md)):
continuity rests on the repository and the trusted publishing above, and the OpenSSF Best Practices criteria that ask
for a second owner stay unmet, so the project aims for the passing level
([docs/best-practices-badge.md](docs/best-practices-badge.md)).

## Changing This Document

Governance changes through a pull request to this file, which a maintainer merges.
