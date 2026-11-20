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
3. **R3 Fail-safe lookups.** A missing key is reported, never thrown: both `TryGetValue<T>` overloads and
   `IReadOnlyDictionary.TryGetValue` return `false`, and the `IReadOnlyDictionary` indexer returns `null`.
4. **R4 Independent clones.** `Clone()` returns a new bag whose entries change without changing the original's; a value
   that implements `ICloneable` is copied through its own `Clone()`, any other value is shared by reference.
5. **R5 Registration never overrides.** `AddPropertyBag()` uses `TryAddSingleton`, so an `IPropertyBagFactory` the
   application registered first is kept.
6. **R6 Traceable packages.** CI builds the packages from the tagged commit of a published GitHub release,
   deterministically and with Source Link, refusing a tag that differs from `version.json`; a rebuild of a release's tag
   gives the same packages, entry for entry
   ([ADR-1010](decisions/1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md)); nuget.org
   signs them, and each release's packages carry a signed build-provenance attestation.

Users must not rely on:

- **Thread safety:** `PropertyBagDictionary`, `LayeredPropertyBag` and their scopes hold no lock; concurrent use of one
  of them needs the caller's synchronization, or a `ConcurrentPropertyBagDictionary`.
- **Isolation:** `IReadOnlyPropertyBag` is a narrower view, not a protection: code holding a `PropertyBagDictionary`
  through it can cast it back to `IPropertyBag`; `AsReadOnly()` returns a `ReadOnlyPropertyBag`, which can't be cast
  back, but every caller in the process is still trusted.
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

The library has no trust boundary of its own: it runs in the application's process with the application's permissions,
and every caller of its API is trusted code. Untrusted data reaches it only as values and key names the application
chooses to store, after validating them. The trust boundary this project guards is the supply chain, from this
repository's reviewed code through CI to the packages on nuget.org.

| Threat | In scope | Countered by |
| --- | --- | --- |
| A value is read as a type other than the one stored (type confusion) | Yes | R1 |
| Two components use one key name for different types and overwrite each other | Yes | R1: the key includes the type, so only the same name and type collide |
| Keys chosen to collide slow the dictionary (hash flooding) | Yes | `PropertyBagKey.GetHashCode` combines `Type` and `Name` with `HashCode.Combine`, whose seed is random per process, over .NET's randomized string hash |
| A missing or mistyped key throws in the caller | Yes | R1, R3 |
| A change to a clone leaks into the original | Yes | R4 |
| The library's registration replaces the application's own factory | Yes | R5 |
| Untrusted data arrives through files, the network or deserialization | No: the packages have no such input | R2 |
| Unbounded entries from untrusted input exhaust memory | No: the application bounds what it stores | — |
| Concurrent writes corrupt a bag | Yes for `ConcurrentPropertyBagDictionary`, which takes a lock; no for the other bags, whose caller synchronizes | — |
| Hostile code in the same process reads a bag or replaces the default factory | No: .NET has no isolation within a process | — |
| A tampered or unreviewed package reaches consumers | Yes | R6, and the review and release evidence below |
| A vulnerable dependency | Yes | One runtime dependency, `Microsoft.Extensions.DependencyInjection.Abstractions`, pinned centrally and updated by Dependabot; NuGet advisories fail CI |

## Input Validation

The packages take no untrusted input of their own (R2). Their only inputs are the arguments of their public API, from
code in the same process, which the Threat Model trusts. Each public entry point and the check it makes:

- **`PropertyBagKey(Type, string)` and `PropertyBagKey<T>(string)`** check nothing: a `null` type or name, like a
  `default` key, compares and hashes without error, and a name is compared ordinally, never parsed.
- **`SetValue<T>`, `Scope<T>`, `Remove` and `TryGetValue<T>` of `PropertyBagDictionary`** take a key that is a struct,
  never `null`, store each value as an opaque object that is never parsed, formatted, serialized or run, and test a
  stored value's type with `is T` on every read (R1).
- **The extensions `SetValue<T>(value)` and `TryGetValue<T>(out value)`** throw `ArgumentNullException` for a `null`
  bag (`ArgumentNullException.ThrowIfNull`), and their key name is the caller's argument expression, text fixed at
  compile time, unless the caller passes `name`.
- **`AddPropertyBag()`** throws `ArgumentNullException` for a `null` service collection.
- **`DefaultPropertyBagScope`'s public constructor** checks nothing, and a `null` bag fails in `Dispose` with
  `NullReferenceException` ([#47](https://github.com/NCodeGroup/PropertyBag/issues/47)).
- **The setters of `PropertyBagFactory.DefaultFactory` and `DefaultPropertyBagFactory.Singleton`** check nothing: a
  `null` `DefaultFactory` falls back to `DefaultPropertyBagFactory.Singleton`, and a `null` `Singleton` makes
  `PropertyBagFactory.Create()` throw `NullReferenceException` while `DefaultFactory` is unset or `null`.
- **`Clone()`** runs the `Clone()` of each stored value that implements `ICloneable`, code the application stored.

An application that stores untrusted data validates and bounds it before the call (see Bounded size above).

## Secure Design Principles Applied

[//]: # "Maintainers: say how the design applies economy of mechanism, fail-safe defaults, complete mediation, least
privilege and open design."

- **Economy of mechanism:** two small packages: five abstractions (three interfaces, two key structs) plus extensions,
  and an implementation of a dictionary-backed bag, a concurrent one behind a single lock, a layered bag, a read-only
  wrapper, a scope, a factory and two extension classes (one registration method, plus `CreateChild` and `AsReadOnly`);
  one runtime dependency; no configuration.
- **Fail-safe defaults:** lookups of a missing or mistyped key return `false` and `default` (R1, R3); registration never
  overrides the application's (R5); an empty bag allocates nothing.
- **Complete mediation:** every read goes through one lookup that compares the whole key, type and name, and checks the
  stored value's type on each read; nothing caches an earlier answer.
- **Least privilege:** the library needs no permission or capability (R2). In CI, `publish.yml` defaults to
  no permission; the `pack` job reads the repository and holds no credential, the `publish` job runs in the protected
  `nuget` environment with only an OIDC token, and write permissions are granted only to the `provenance` and `baseline`
  jobs, each exactly what its called workflow declares.
- **Open design:** the source is public under Apache-2.0 and Source Link maps each package to it; no stored secret
  publishes a package: nuget.org trusted publishing exchanges the `publish` job's short-lived OIDC token for a
  short-lived key.
- **Least common mechanism:** each bag owns its dictionary; the only shared state is the two settable factory
  properties.
- **Psychological acceptability:** typed keys make the safe call the natural one, and nullable annotations say where
  `null` is allowed.

## Common Weaknesses Countered

[//]: # "Maintainers: list the CWE ids the design counters, each with the requirement that counters it."

- **[CWE-843](https://cwe.mitre.org/data/definitions/843.html) Access of Resource Using Incompatible Type ('Type
  Confusion'):** a value is returned only under the key and type it was stored with (R1).
- **[CWE-704](https://cwe.mitre.org/data/definitions/704.html) Incorrect Type Conversion or Cast:** `TryGetValue<T>`
  tests the stored object with `is T`, never a cast (R1).
- **[CWE-407](https://cwe.mitre.org/data/definitions/407.html) Inefficient Algorithmic Complexity:** randomized key
  hashing (see the Threat Model).
- **[CWE-476](https://cwe.mitre.org/data/definitions/476.html) NULL Pointer Dereference:** nullable reference types with
  warnings as errors, the null-forgiving operator banned outside tests
  ([ADR-0100](decisions/project-template/0100-warnings-are-errors-in-every-project.md)), and an explicit `null` value
  contract (R1).
- **[CWE-502](https://cwe.mitre.org/data/definitions/502.html) Deserialization of Untrusted Data:** the packages
  deserialize nothing (R2).
- **[CWE-1104](https://cwe.mitre.org/data/definitions/1104.html) Use of Unmaintained Third Party Components:** pinned
  dependencies moved by Dependabot
  ([ADR-0008](decisions/project-template/0008-external-inputs-are-pinned-and-kept-current.md)).
- **[CWE-494](https://cwe.mitre.org/data/definitions/494.html) Download of Code Without Integrity Check:** nuget.org's
  signature and the release's provenance attestation let a consumer check a package (R6,
  [SECURITY.md](../SECURITY.md#verify-a-package)).

## Evidence

[//]: # "Maintainers: add the tests and the coverage the project keeps."

- **Gate:** `./build/gate.ps1` checks the formatting with CSharpier
  ([ADR-0148](decisions/project-template/0148-csharp-is-formatted-in-the-gate-and-files-carry-the-spdx-header.md)) and runs
  `./build/dod.ps1`, which builds in Release with warnings as errors
  ([ADR-0100](decisions/project-template/0100-warnings-are-errors-in-every-project.md)), runs the analyzers and the
  banned-API list in `build/BannedSymbols.txt`
  ([ADR-0102](decisions/project-template/0102-disallowed-apis-are-banned-at-build-time.md)), and runs the tests; CI runs
  the gate on every pull request, then `./build/Test-Coverage.ps1`, which fails below 98% line or branch coverage
  ([ADR-0026](decisions/project-template/0026-tests-gate-coverage-at-the-gold-thresholds-and-a-release-waits-for-them.md)).
- **Property tests:** FsCheck tests in `tests/NCode.PropertyBag.Tests`, the `*PropertyTests` classes, check generated
  keys and operation sequences: key equality and hash codes agree with the type and the ordinal name (R1), a value is
  read back only under its key's type (R1), the default bag matches a dictionary model under any mix of sets and
  removes, nested scopes restore each enclosing value as they are disposed, and a clone and its original stay
  independent. They replay one fixed seed, `20261120,12345`, so every run checks the same generated cases.
- **Assertions while testing:** the library has no `Debug.Assert` or other check that a Release build removes: its
  checks are `ArgumentNullException.ThrowIfNull` guards that run in every build, so the tests run every check the code
  has. The tests' own assertions (xUnit, and the FsCheck properties) check every result. CI runs the tests twice: in
  Release in the gate, and in Debug, the `dotnet test` default, in `./build/Test-Coverage.ps1`, where `DEBUG` is
  defined.
- **Regression tests:** the fix that makes disposing a scope restore the previous value
  ([ADR-1000](decisions/1000-disposing-a-scope-restores-the-previous-value.md), pull request
  [#39](https://github.com/NCodeGroup/PropertyBag/pull/39)) added `Dispose_FromScopeMethod_CalledTwice_RestoresOnce`
  and `Scope_WhenKeyHeldNull_RestoresNull` and turned the nested-scope test into `NestedScopes_RestoreInReverseOrder`,
  in `tests/NCode.PropertyBag.Tests/DefaultPropertyBagScopeTests.cs`; each fails without the fix.
- **Static analysis:** with CodeQL's default setup configured and analyzing at least one language, the repository
  ruleset blocks a pull request that adds a CodeQL alert at warning level or above, or a security alert of any severity
  ([ADR-0005](decisions/project-template/0005-the-default-branch-accepts-changes-only-through-pull-requests.md),
  [ADR-0030](decisions/project-template/0030-the-code-scanning-rule-is-added-only-where-codeql-analyzes-a-language.md)).
- **Dismissed alerts:** CodeQL alerts #4 and #5 (`actions/untrusted-checkout/high` and
  `actions/untrusted-checkout-toctou/high`, `code-review.yml` lines 58 to 66 when raised) flagged a step that checked
  the pull request's head out into `pr/` in a `pull_request_target` and `issue_comment` workflow; that step was removed
  in commit dede4be on 2026-10-30, and CodeQL's analysis of `main` no longer reports it. The workflow checks out only
  the default branch, runs only for pull requests from this repository's branches, and the NCode Review action fetches
  the pull request to read it, building and running nothing from it, as `code-review.yml`'s header says. The alerts
  stay dismissed as won't fix.
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
  ([ADR-0130](decisions/project-template/0130-source-link-and-symbol-packages.md)). The `pack` job of
  `.github/workflows/publish.yml` builds and packs them from the tagged commit of a published GitHub release, whose tag
  must match `version.json` and name a commit `main` holds
  ([ADR-0138](decisions/project-template/0138-package-versions-come-from-version-json-through-nerdbank-gitversioning.md));
  its `publish` job, which runs in the protected `nuget` environment after a maintainer approves, pushes them to
  nuget.org through trusted publishing with no stored key, and its `provenance` job signs a build-provenance
  attestation for them and attaches it to the release
  ([ADR-0139](decisions/project-template/0139-releases-carry-build-provenance.md)); nuget.org signs each package it
  accepts.
  [SECURITY.md](../SECURITY.md#verify-a-package) says how to verify a package.
- **Reproducible builds:** `.github/workflows/reproducible-build.yml` packs every pull request and push to `main` twice,
  from checkouts in different folders, and every Monday rebuilds the newest release's tag and compares the packages with
  the ones attached to its GitHub release, each with `build/Compare-Packages.ps1`, entry by entry
  ([ADR-1010](decisions/1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md)). On
  2026-11-20 a rebuild of `v2.0.1` in a Linux container matched its released packages entry for entry.
- **Response:** [SECURITY.md](../SECURITY.md) defines private reporting, coordinated disclosure and credit.

## Security Reviews

[//]: # "Maintainers: record each security review, newest first: its date, who did it, what it covered (the security requirements and the trust boundary above) and what it found. The OpenSSF Best Practices gold level asks for one in the last five years."

### 2026-11-20: the library and its release workflows

- **Who:** [@polewskm](https://github.com/polewskm), the maintainer, with an agent-assisted review: GitHub Copilot read
  the code and wrote these findings while planning [#46](https://github.com/NCodeGroup/PropertyBag/issues/46), and the
  maintainer accepts them by merging the pull request that records them.
- **Scope:** the security requirements R1 to R6 and the trust boundary above: every C# file in `src/` at commit
  f4f264d, the twelve workflows in `.github/workflows/` and `.github/actions/install-dotnet`, and a rebuild of
  `v2.0.1`. The NCode Review action's own code, in NCodeGroup/code-review, is outside it.
- **Method:** each public entry point read against R1 to R5 and the tests that check them; each workflow's triggers,
  permissions, action pins, secrets and the places pull request or issue content reaches a step listed; the `v2.0.1`
  tag rebuilt in `mcr.microsoft.com/dotnet/sdk:10.0.404` and its packages compared with the release's.
- **Findings:**
  1. No vulnerability in the library; R1 to R5 hold as stated.
  2. Low, robustness: `DefaultPropertyBagScope`'s public constructor takes a `null` bag and fails only in `Dispose`;
     every caller is trusted code, so it is no vulnerability;
     [#47](https://github.com/NCodeGroup/PropertyBag/issues/47) tracks the fix.
  3. Workflows: every action from outside the repository is pinned to a full commit SHA; each workflow's top-level
     permissions are `{}` or `contents: read`, with write permissions granted per job; no `run:` script expands an
     expression from an event or an input, values reach scripts through `env:`; the two `pull_request_target`
     workflows run only for pull requests from this repository's branches, which only people with write access push,
     and check out no pull request code (see Dismissed alerts). Accepted risk: a pull request's text reaches NCode
     Review's model, which its author could try to steer into approving; an approval never merges by itself, a
     maintainer merges.
  4. Branch protection: the `main` ruleset requires pull requests, the `build-and-test` check, resolved conversations
     and CodeQL, but no approving review and no approval of the last push (OpenSSF Scorecard alert #8); recommended:
     one approval and approval of the last push.
  5. Release integrity: the rebuilt `v2.0.1` packages match the released ones entry for entry; `Copyright` held the
     build's year, which `Directory.Build.props` removes
     ([ADR-1010](decisions/1010-package-builds-are-reproducible-and-ci-compares-rebuilds-entry-by-entry.md),
     [NCodeGroup/project-template#454](https://github.com/NCodeGroup/project-template/issues/454)).
