# Governance

NCode.PropertyBag belongs to NCode Group, under `Apache-2.0`. This page says who does
what and how decisions are made.

## Roles

[//]: # "Maintainers: name each maintainer in the Who column, such as @octocat."

| Role | Who | Responsibilities |
| --- | --- | --- |
| Maintainer | The repository's owners | Sets direction, triages issues, reviews and merges pull requests, publishes each GitHub release, answers vulnerability reports, owns the repository's settings and the nuget.org packages |
| Contributor | Anyone | Opens issues and pull requests, under [CONTRIBUTING.md](CONTRIBUTING.md) and the [Code of Conduct](CODE_OF_CONDUCT.md) |
| Automation | `ncode-review[bot]`, Dependabot, GitHub Actions | Reviews and approves a clean pull request, proposes dependency updates, runs CI and pushes a published release's packages to nuget.org; never merges a pull request except the pin bump `code-review-auto-merge.yml` merges once a maintainer approves it |

Only a maintainer has write access. A contributor who keeps contributing reviewed, merged work can be invited to become
a maintainer; the existing maintainers decide, and the change is recorded in this file.

## Decisions

- Day-to-day decisions are made in issues and pull requests. Anyone can comment; a maintainer decides.
- A decision someone would otherwise re-argue is recorded as an architecture decision record in
  [docs/decisions/](docs/decisions/README.md), before or with the pull request that builds it. A change consumers can
  observe, such as to the public API, behavior or security posture, starts as an issue.
- Where maintainers disagree, they discuss it on the issue; with no agreement, the change doesn't happen.

## Releases

Versions follow Semantic Versioning. A maintainer sets `version.json` to the release's version, then publishes the
GitHub release, which tags it; the `publish` job of `.github/workflows/main.yml` then pushes its packages to nuget.org
with the repository's NuGet API key.
[SECURITY.md](SECURITY.md) says how to verify a package.

## Continuity

The repository, its issues, decision records, build, release automation and documentation are all in the
GitHub organization that owns it, so anyone given access can carry
on with no knowledge held elsewhere. nuget.org publishing uses the repository's `NUGET_APIKEY` secret, which a new owner replaces with an API key of their own.

[//]: # "Maintainers: name the owners, and keep the list current."

Owners today: the repository's owners and the owners of the nuget.org packages. Continuity
needs at least two people who can administer both; until there are
two, it is incomplete, and the OpenSSF Best Practices silver level can't be claimed
([docs/best-practices-badge.md](docs/best-practices-badge.md)).

## Changing This Document

Governance changes through a pull request to this file, which a maintainer merges.
