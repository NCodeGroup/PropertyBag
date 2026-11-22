# Roadmap

What NCode.PropertyBag intends to do over the next year. Open issues hold the detail and the current state; this page
is the direction. It is reviewed at least once a year and when a release changes it.

[//]: # "Maintainers: replace the prompts with the project's own plans, and review this page at least once a year."

## Scope

NCode.PropertyBag covers what [README.md](README.md) describes. A feature outside that starts as an issue, where the
maintainers say whether it fits.

## Next Twelve Months

[//]: # "Maintainers: list what the project plans to do in the next twelve months, one bullet each."

- **Supply chain.** Keep external inputs pinned and current
  ([ADR-0008](docs/decisions/project-template/0008-external-inputs-are-pinned-and-kept-current.md)),
  and raise the OpenSSF Scorecard score where the repository controls it
  ([ADR-0020](docs/decisions/project-template/0020-github-projects-are-scored-by-openssf-scorecard.md)).
- **Quality.** Keep line and branch coverage at 98% or more, and property tests on the keys and the default bag.
- **OpenSSF Best Practices.** Earn and hold the passing level of
  [project 16954](https://www.bestpractices.dev/projects/16954), keeping each answer's evidence current
  ([ADR-1014](docs/decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md),
  [docs/best-practices-badge.md](docs/best-practices-badge.md)).

## Not Planned

[//]: # "Maintainers: list what the project rules out, and why."

- **OpenSSF Best Practices silver and gold.** Both ask for a second owner of the GitHub organization and the nuget.org
  account (`access_continuity`, `bus_factor`, `two_person_review`), which the project doesn't add, and gold also for an
  owner's confirmation of the organization's two-factor methods (`require_2FA`, `secure_2FA`), which isn't given
  ([ADR-1014](docs/decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md)).
