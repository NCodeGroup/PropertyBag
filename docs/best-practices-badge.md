# OpenSSF Best Practices Badge

A maintainer registers NCode.PropertyBag at [bestpractices.dev](https://www.bestpractices.dev) and answers the criteria
there, which is the source of truth; this page doesn't copy the answers. It maps the criteria that need a document to
the evidence this repository holds, so registering is answering, not researching.
[best-practices-criteria.md](best-practices-criteria.md), which the template keeps current, maps every criterion of the
three levels to the answer to expect and its evidence, and holds the prompt that registers the project.

## The Badge

```html
<a href="https://www.bestpractices.dev/projects/16954"><img alt="OpenSSF Best Practices"
  src="https://www.bestpractices.dev/projects/16954/badge"></a>
```

The root [README.md](../README.md) shows it; the package READMEs don't, because it describes the repository rather than
a package.

OpenSSF Scorecard's CII-Best-Practices check scores the badge level: in progress 2, passing 5, silver 7, gold 10
([ADR-0020](decisions/project-template/0020-github-projects-are-scored-by-openssf-scorecard.md)).

## Evidence Map

The criteria that need a document, at every level, point to these:

| Criteria | Where |
| --- | --- |
| `governance`, `roles_responsibilities` | [GOVERNANCE.md](../GOVERNANCE.md) |
| `code_of_conduct` | [CODE_OF_CONDUCT.md](../CODE_OF_CONDUCT.md) |
| `documentation_roadmap` | [ROADMAP.md](../ROADMAP.md) |
| `documentation_architecture` | [architecture.md](architecture.md) |
| `documentation_security`, `assurance_case`, `implement_secure_design` | [assurance-case.md](assurance-case.md) |
| `signed_releases` | Met for each release from the next one on: the `provenance` job of `.github/workflows/publish.yml` signs a build-provenance attestation for the packages and attaches it to the GitHub release as `<tag>.intoto.jsonl` ([ADR-0139](decisions/project-template/0139-releases-carry-build-provenance.md)); releases up to v1.2.0 carry none. [SECURITY.md](../SECURITY.md#verify-a-package) says how to verify a package |
| Vulnerability response and credit | [SECURITY.md](../SECURITY.md) |
| Coding standards and their enforcement | [CONTRIBUTING.md](../CONTRIBUTING.md), `.editorconfig`, CSharpier and the analyzers, in the gate |
| Test coverage and policy | `./build/Test-Coverage.ps1` (98% line and branch) and [CONTRIBUTING.md](../CONTRIBUTING.md) |
| Test policy (`test_policy_mandated`) | [CONTRIBUTING.md](../CONTRIBUTING.md), which says that a change that adds or changes behavior adds or updates the automated tests that cover it |
| `dynamic_analysis_enable_assertions`, `input_validation`, `regression_tests_added50`, `security_review` | [assurance-case.md](assurance-case.md): Evidence, Input Validation and Security Reviews |
| `build_repeatable`, `build_reproducible` | The `reproducible-build` job of `.github/workflows/ci.yml`, `.github/workflows/reproducible-build.yml`, [ADR-1010](decisions/1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md) and [ADR-1012](decisions/1012-the-template-ci-job-builds-each-commit-twice-and-the-project-workflow-rebuilds-releases.md) |
| `internationalization` | [architecture.md](architecture.md#components): the packages show people no text |
| `small_tasks` | Open issues labeled [`good first issue`](https://github.com/NCodeGroup/PropertyBag/issues?q=is%3Aissue%20is%3Aopen%20label%3A%22good%20first%20issue%22), which [CONTRIBUTING.md](../CONTRIBUTING.md) names; when none is open, file one real small task with the label before answering Met |

## Where It Stands

[//]: # "Maintainers: once registered, say which level the site shows and what keeps the next one from being earned, naming the criteria, not a percentage."

Registered as [project 16954](https://www.bestpractices.dev/en/projects/16954). The project aims for the passing level
([ADR-1014](decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md)). On
2026-11-21 the site showed passing in progress: every MUST and SHOULD criterion is answered, and
`dynamic_analysis_enable_assertions` is the one unanswered criterion. Its evidence is the Assertions while testing item
of [assurance-case.md](assurance-case.md#evidence), and answering it Met earns passing. Each saved justification links
to its evidence on `main` by line, so after a change that moves a cited line, run `/best-practices` and answer again
with the prompt it writes. Silver stops at `access_continuity` and gold at the criteria below, which stay unmet.

## Needs a Person

- `access_continuity` (silver MUST), `bus_factor` (silver SHOULD, gold MUST) and `two_person_review` (gold MUST):
  unmet, because the project keeps one owner and adds no second one
  ([ADR-1014](decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md),
  [GOVERNANCE.md](../GOVERNANCE.md#continuity)).
- `contributors_unassociated` (gold MUST): two significant contributors from different organizations.
- `require_2FA` (gold MUST) and `secure_2FA` (gold SHOULD): unmet, because the owner doesn't confirm the
  organization's two-factor methods on bestpractices.dev
  ([ADR-1014](decisions/1014-the-project-keeps-one-owner-and-aims-for-the-openssf-best-practices-passing-level.md)).
  GitHub's API reported the organization's two-factor requirement enabled on 2026-11-20.
- `security_review` (gold MUST): the review of 2026-11-20 in [assurance-case.md](assurance-case.md#security-reviews)
  meets it until 2031-11-20; record the next one there before then.
- `dco` (silver SHOULD): unmet. The project doesn't ask for `Signed-off-by`
  ([ADR-0023](decisions/project-template/0023-a-project-ships-the-documents-the-best-practices-silver-level-asks-for.md)).

## Also Supported

OpenSSF Scorecard runs on every push to `main` and once a week (`.github/workflows/scorecard.yml`).
