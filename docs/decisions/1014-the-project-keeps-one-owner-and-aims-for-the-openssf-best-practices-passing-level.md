# 1014. The project keeps one owner and aims for the OpenSSF Best Practices passing level

- **Status:** Accepted
- **Date:** 2026-11-21
- **Deciders:** NCode.PropertyBag maintainers

## Context

[@polewskm](https://github.com/polewskm) is the one owner of the NCodeGroup GitHub organization, the one person with
write access, and owns the nuget.org account `NCode` ([GOVERNANCE.md](../../GOVERNANCE.md#continuity)).

The project is [project 16954](https://www.bestpractices.dev/projects/16954) on bestpractices.dev. Its saved answers,
read from https://www.bestpractices.dev/projects/16954.json on 2026-11-21, meet or rule out every passing criterion
except the SUGGESTED `dynamic_analysis_enable_assertions`, which is left unanswered and whose evidence
[assurance-case.md](../assurance-case.md#evidence) holds.

Silver's `access_continuity` (MUST) and `bus_factor` (SHOULD) and gold's `bus_factor` and `two_person_review` (MUST)
need a second owner or maintainer. Gold's `require_2FA` (MUST) and `secure_2FA` (SHOULD) need an owner to confirm the
organization's two-factor methods. Gold's `contributors_unassociated` (MUST) needs two contributors from different
organizations. Project-template ADR-0020 names passing as the most one writer reaches.

## Decision

**The project keeps one owner and aims for the OpenSSF Best Practices passing level; it doesn't pursue silver or
gold.**

1. No second owner of the organization or the nuget.org account and no second maintainer is added: `access_continuity`,
   `bus_factor` and `two_person_review` stay Unmet, citing this record.
2. The owner doesn't confirm the organization's two-factor methods: `require_2FA` and `secure_2FA` stay Unmet, citing
   this record.
3. Every passing criterion, the SUGGESTED ones included, is answered with evidence on `main`; a change that moves cited
   evidence updates the saved answer, with the prompt `/best-practices` writes.
4. The documents project-template ADR-0023 ships stay and are kept current, since passing answers cite them, such as
   `know_secure_design` and `know_common_errors`.

## Options considered

- **Add a second owner and aim for silver.** Rejected: the maintainers keep one owner.
- **Confirm the two-factor methods.** Rejected: gold still needs `two_person_review` and `contributors_unassociated`,
  so it earns no level.
- **Keep one owner and aim for passing** (chosen): every passing criterion is within one maintainer's reach.

## Consequences

- Continuity rests on the repository holding everything and on trusted publishing with no key to hand over, and no one
  else can administer the organization, the repository or the nuget.org account.
- Silver and gold stay below 100%, and Scorecard's CII-Best-Practices check scores at most 5 once passing is earned
  (project-template ADR-0020), so its code-scanning alert is dismissed as won't fix (project-template ADR-0035).
- A second person with write access is the signal to write a record that supersedes this one.

## References

- [#50](https://github.com/NCodeGroup/PropertyBag/issues/50)
- [GOVERNANCE.md](../../GOVERNANCE.md#continuity)
- [best-practices-badge.md](../best-practices-badge.md)
- [best-practices-criteria.md](../best-practices-criteria.md)
- [project-template ADR-0020](project-template/0020-github-projects-are-scored-by-openssf-scorecard.md)
- [project-template ADR-0023](project-template/0023-a-project-ships-the-documents-the-best-practices-silver-level-asks-for.md)
- [project-template ADR-0035](project-template/0035-a-sole-maintainer-dismisses-the-scorecard-alerts-one-person-cannot-clear.md)
