# Assurance Case

This page states NCode.PropertyBag's security requirements, the threats it considers, and why the design and the way it
is built meet them. [architecture.md](architecture.md) describes the components; [README.md](../README.md) is the
user-facing summary.

## Security Requirements

[//]: # "Maintainers: list what the project is expected to do as R1, R2, ..., and what users must not rely on."

1. **R1 Type-safe retrieval.** A value is returned only under the key it was stored with: `PropertyBagKey` equality
   compares both `Type` and `Name` (ordinal), and `TryGetValue<T>` returns `true` only when the stored object is `null`
   or a `T` (an `is T` test, not a cast); otherwise it returns `false` with `default` and never throws
   `InvalidCastException`.
2. **R2 No external input or output.** The packages read no file, network, environment variable or configuration, start
   no process or thread, and serialize, deserialize or load no code; a bag holds only the objects its caller stores, in
   memory, for the bag's lifetime.
3. **R3 Fail-safe lookups.** A missing key is reported, never thrown: `TryGetValue<T>`, `TryGet<T>` and
   `IReadOnlyDictionary.TryGetValue` return `false`, and the `IReadOnlyDictionary` indexer returns `null`.
4. **R4 Independent clones.** `Clone()` returns a new bag whose entries change without changing the original's; a value
   that implements `ICloneable` is copied through its own `Clone()`, any other value is shared by reference.
5. **R5 Registration never overrides.** `AddPropertyBag()` uses `TryAddSingleton`, so an `IPropertyBagFactory` the
   application registered first is kept.
6. **R6 Traceable packages.** CI builds the packages from the tagged commit of a published GitHub release,
   deterministically and with Source Link, refusing a tag that differs from `version.json`; nuget.org signs them, and
   each release's packages carry a signed build-provenance attestation.

Users must not rely on:

- **Thread safety:** `DefaultPropertyBag` and its scopes hold no lock; concurrent use of one bag needs the caller's
  synchronization.
- **Isolation:** `IReadOnlyPropertyBag` is a narrower view, not a protection: code holding a `DefaultPropertyBag`
  through it can cast it back to `IPropertyBag`, and every caller in the process is trusted.
- **Confidentiality:** values are ordinary managed objects, neither encrypted nor cleared; `Remove` only drops the bag's
  reference.
- **Bounded size:** a bag holds every entry it is given; an application that lets untrusted input choose key names or
  values bounds them itself.
- **Out-of-order scopes:** each scope restores the value its key held when the scope was created, so nested scopes
  disposed out of order leave the value the last disposed one captured.
- **Deep copies:** `Clone()` shares values that don't implement `ICloneable` and runs the `Clone()` of those that do.
- **A fixed factory:** `PropertyBagFactory.DefaultFactory` and `DefaultPropertyBagFactory.Singleton` are settable
  process-wide, so any code in the process can replace the factory `PropertyBagFactory.Create()` uses.

## Threat Model

[//]: # "Maintainers: add a table (Threat, In scope, Countered by) and state the trust boundary."

Not written yet.

## Secure Design Principles Applied

[//]: # "Maintainers: say how the design applies economy of mechanism, fail-safe defaults, complete mediation, least
privilege and open design."

Not written yet.

## Common Weaknesses Countered

[//]: # "Maintainers: list the CWE ids the design counters, each with the requirement that counters it."

Not written yet.

## Evidence

[//]: # "Maintainers: add the tests and the coverage the project keeps."

- **Gate:** `./build/dod.ps1` builds in Release with warnings as errors
  ([ADR-0100](decisions/project-template/0100-warnings-are-errors-in-every-project.md)), runs the analyzers and the
  banned-API list in `build/BannedSymbols.txt`
  ([ADR-0102](decisions/project-template/0102-disallowed-apis-are-banned-at-build-time.md)), and runs the tests; CI runs
  the commands on every pull request.
- **Static analysis:** with CodeQL's default setup configured and analyzing at least one language, the repository
  ruleset blocks a pull request that adds a CodeQL alert at warning level or above, or a security alert of any severity
  ([ADR-0005](decisions/project-template/0005-the-default-branch-accepts-changes-only-through-pull-requests.md),
  [ADR-0030](decisions/project-template/0030-the-code-scanning-rule-is-added-only-where-codeql-analyzes-a-language.md)).
- **Review:** every change reaches the default branch through a pull request with passing CI and every conversation
  resolved, and the ruleset refuses force pushes and deletion
  ([ADR-0005](decisions/project-template/0005-the-default-branch-accepts-changes-only-through-pull-requests.md),
  [ADR-0014](decisions/project-template/0014-every-pull-request-discussion-is-remediated-and-replied-to-before-it-merges.md)).
  NCode Review reviews each pull request and approves a clean one.
- **Dependencies:** pinned, and moved by reviewed updates
  ([ADR-0008](decisions/project-template/0008-external-inputs-are-pinned-and-kept-current.md)) that wait three days for
  a new version ([ADR-0009](decisions/project-template/0009-new-dependency-versions-wait-a-cooldown.md)). Dependabot proposes the updates. Central package management keeps one version of each package
  ([ADR-0101](decisions/project-template/0101-central-package-management.md)).
- **Release integrity:** packages are deterministic and source-linked
  ([ADR-0130](decisions/project-template/0130-source-link-and-symbol-packages.md)). The `build` job of
  `.github/workflows/main.yml` builds and packs them from the tagged commit of a published GitHub release, whose tag must
  match `version.json`
  ([ADR-0138](decisions/project-template/0138-package-versions-come-from-version-json-through-nerdbank-gitversioning.md));
  its `publish` job, which holds no repository permission, pushes them to nuget.org with the repository's NuGet API key,
  and its `provenance` job signs a build-provenance attestation for them and attaches it to the release
  ([ADR-0139](decisions/project-template/0139-releases-carry-build-provenance.md)); nuget.org signs each package it
  accepts.
  [SECURITY.md](../SECURITY.md#verify-a-package) says how to verify a package.
- **Response:** [SECURITY.md](../SECURITY.md) defines private reporting, coordinated disclosure and credit.

## Security Reviews

[//]: # "Maintainers: record each security review, newest first: its date, who did it, what it covered (the security requirements and the trust boundary above) and what it found. The OpenSSF Best Practices gold level asks for one in the last five years."

None recorded yet.
