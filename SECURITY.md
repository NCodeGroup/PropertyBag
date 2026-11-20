# Security Policy

## Report a Vulnerability

Report a vulnerability in NCode.PropertyBag privately. Never put it in a public issue, pull request or discussion: that tells attackers about it before a fix exists.

On the repository's **Security** tab, choose **Report a vulnerability**. The report reaches the maintainers only, and the fix is discussed in a private security advisory until it is released. If the tab has no such button, open an issue that asks for a private contact, and leave out every detail of the vulnerability.

[//]: # "Maintainers: the button appears once private vulnerability reporting is turned on in the repository's security settings."

Include:

- what is affected: the package and its version, and the file, endpoint or feature;
- how to reproduce it, as steps or a proof of concept;
- what it lets an attacker do;
- any fix or mitigation you know of, and how you'd like to be credited.

## What to Expect

- An acknowledgement of the report within 14 days, then whether it is confirmed.
- A fix developed in private and released as soon as it is ready, with an entry under `### Security` in the changelog.
- Disclosure coordinated with you: the security advisory is published once the fix is released. Keep the details private until then.
- Credit for the report in the advisory, unless you'd rather not be named.

## Supported Versions

Fixes ship as a patch of the latest minor version of the current major version. Before 1.0.0, only the latest release gets fixes.

| Version | Fixes |
| --- | --- |
| The latest minor of the current major, such as 2.3.x while 2.3 is the newest minor | Yes |
| Every older version | No |

## Verify a Package

The packages are built in CI from a tagged commit of this repository, deterministically and with Source Link. They are published to nuget.org by `.github/workflows/publish.yml` when a GitHub release is published. The job that builds and tests holds no credential; the job that pushes runs in the protected `nuget` environment, where a maintainer approves each publication, and exchanges its short-lived GitHub identity for a short-lived nuget.org key through trusted publishing, so no long-lived API key is stored. Each package's version is its release's tag, and the build refuses a release whose tag differs from the version in `version.json`.

- Each package's PDB maps every source file to its commit in this repository, and its symbol package (`.snupkg`) is on the nuget.org symbol server. With Source Link and the nuget.org symbol server turned on in your debugger, you step into the exact source a package was built from. NuGet Package Explorer shows whether a package's Source Link and deterministic build are valid.
- nuget.org signs every package it accepts: `dotnet nuget verify --all <package>.nupkg` checks that signature.
- Each release published since build provenance was added carries a signed build-provenance attestation for its packages, made by the `provenance` job of `.github/workflows/publish.yml` through `.github/workflows/release-provenance.yml`, and its GitHub release carries the packages, the provenance file (`<tag>.intoto.jsonl`) and an SPDX SBOM (`<tag>.spdx.json`). `gh attestation verify <package>.nupkg --repo NCodeGroup/PropertyBag` checks that a package was built by this repository's workflows from the tagged commit. Releases up to v1.2.0 carry none.
- [OpenSSF Scorecard](https://scorecard.dev/viewer/?uri=github.com/NCodeGroup/PropertyBag) scores this repository's supply-chain practices, such as pinned dependencies, token permissions and branch protection, on each push to `main` and every week: `.github/workflows/scorecard.yml` runs it and publishes the score.

## Verify a Release

While the repository is public, the Tag and publish release workflow signs each release tag with [gitsign](https://github.com/sigstore/gitsign), through Sigstore's keyless signing, so no signing key exists to leak. Tags that workflow creates are signed; v1.2.0 and the tags before it are not. With gitsign installed, check a tag in a clone of the repository:

```shell
gitsign verify-tag --certificate-identity https://github.com/NCodeGroup/PropertyBag/.github/workflows/tag-release.yml@refs/heads/main --certificate-oidc-issuer https://token.actions.githubusercontent.com <tag>
```

## Scope

In scope: the code, build and CI configuration in this repository and the packages built from it, including a vulnerability that comes from how NCode.PropertyBag uses a dependency.

Out of scope: a vulnerability in a dependency itself. Report it to that project. Once it publishes a fix, an issue or pull request here that moves to the fixed version is welcome, though Dependabot usually proposes it first.
