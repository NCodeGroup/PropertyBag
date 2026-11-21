# 1013. NCode Review v1.5.0 is adopted inside the cooldown for the approver

- **Status:** Accepted
- **Date:** 2026-11-21
- **Deciders:** NCode.PropertyBag maintainers
- **Amends:** 1007

## Context

ADR-1007 decision 3 has NCode Review (NCodeGroup/code-review) keep project-template ADR-0009's three-day cooldown,
because it receives the organization's keys and `.github/workflows/code-review-auto-merge.yml` merges its pin without a
person.

NCode Review v1.5.0 adds the NCode Review Approver, whose approval meets the `main` ruleset's required approval
(project-template ADR-0036). The maintainers want the approver in place for this repository's pull requests, and v1.5.0
was published on 2026-11-21, inside the cooldown.

## Decision

**The pin of NCode Review at v1.5.0 is the one release of it adopted inside the cooldown; every later release waits
three days.**

1. `.github/workflows/code-review.yml` pins v1.5.0 by commit SHA, and a comment there names this record.
2. The maintainers check how v1.5.0 mints and scopes the approver token before they merge the pull request that adopts
   it.
3. ADR-1007's decision 3 holds for every other release of NCode Review and for every other dependency.

This amends ADR-1007's decision 3 for that one release.

## Options considered

- **Keep v1.4.2 until v1.5.0 is three days old.** Rejected: the approver is wanted now, and the maintainers write and
  release NCode Review.
- **Exempt NCode Review from the cooldown.** Rejected: its keys and the auto-merge of its pin are the exposure the
  cooldown guards against.
- **Adopt v1.5.0 once, by this record** (chosen).

## Consequences

- From its first run, v1.5.0 receives the review app key, the Bedrock key and the approver key before it is three days
  old.
- A Dependabot bump of the pin after v1.5.0 waits three days.

## References

- [ADR-1007](1007-ncodegroup-docs-releases-are-taken-without-a-cooldown-during-the-build-out.md)
- [project-template ADR-0009](project-template/0009-new-dependency-versions-wait-a-cooldown.md)
- [project-template ADR-0036](project-template/0036-a-required-approval-is-met-by-ncode-reviews-approver-app.md)
- `.github/workflows/code-review.yml`
