# NCode.PropertyBag

Instructions for every coding agent in this repository. This file is an index: read each file it lists, in order, then the sections below. Where the project's instructions differ from the template's, the project's win. Claude Code reads this file through `CLAUDE.md` and imports each `@` file. GitHub Copilot reads it directly and loads every `.github/instructions/*.instructions.md` file on its own.

This file belongs to the project: template updates never change it. Keep it under 200 lines; put anything longer in a listed file.

## Instructions

- @.github/instructions/project-template.instructions.md — the plan-then-execute workflow and base conventions, from the project template. Template updates replace it; never edit it. To change one of its rules, override it below or in a listed file.
- `.github/instructions/csharp.instructions.md` — C# conventions, from the project template; read it before editing `.cs` files.
- `.github/instructions/csharp-tests.instructions.md` — C# test conventions, from the project template; read it too before editing a test project's `.cs` files.
- `docs/decisions/` — this project's architecture decision records, and in `project-template/` the template's; read the ones a change touches before deciding otherwise.
- Project instruction files: add one line for each, `@<path> — what it covers`.

## Commands

Run from the repository root in PowerShell 7.

- Build: `dotnet build`
- Test: `dotnet test`
- Coverage: `./build/Test-Coverage.ps1` runs the tests with coverage, lists every line no test runs, and fails below 98% line or branch coverage; CI runs it after the Definition of Done
- Gate: `./build/gate.ps1` (the full gate CI runs) or `./build/gate.ps1 -Fast` (Debug, no pack): `dotnet tool restore`, the README samples check, then the Definition of Done, `./build/dod.ps1`, whose Format stage runs the CSharpier check; it ends with `Definition of Done: PASSED`. Record a public API change with `./build/Update-PublicApi.ps1`
- Format: `dotnet tool restore` once, then `dotnet csharpier format .`
- Samples: `dotnet mdsnippets` copies the README samples from the tests

## Landing

How `/ship-plan` lands a reviewed plan. `integrate`: `pull-request` pushes the plan's branch and opens a pull request; `direct` pushes to the default branch. `release`: `none`; `tag` tags the plan's `Release:` version, or the next free one when another plan landed it first; `tag+release` also publishes a release from that version's section of `changelog`. On GitHub, `.github/workflows/tag-release.yml` tags and publishes it as the pull request merges, from the `Release:` line `/ship-plan` writes in its description or the changelog's newest section, and for a .NET library starts `.github/workflows/publish.yml`; `/ship-plan` run after the merge does only what that workflow didn't. `changelog`: the file every plan's last step writes, in the Keep a Changelog format, `CHANGELOG.md` by default. `pre-release`: `untracked` keeps the project pre-release until a version tag of 1.0.0 or later: nothing it released or stored has a reader that must keep working, so every text describes the product as it is, the changelog's `## [Unreleased]` is a catalog of what the product does that each plan edits in place, and a breaking change needs no deprecation or migration path, as ADR-0015 in `docs/decisions/project-template/` says; `tracked`, the default when the line is missing, tracks every change from the first release, for a project whose releases or stored data already have readers. `draft`: `no`, the default when the line is missing, which a template update never changes, so a project lands as it did; `yes` opens every pull request as a draft, as `/ship-plan draft` does, and a new project that runs NCode Review starts with it, since its approval lets a ready pull request merge, so marking it ready is the person's review (ADR-0022 in `docs/decisions/project-template/`); and `/run-issues` opens its pull requests as drafts either way. `follow-ups`: `issues` files a plan's follow-ups as issues in this repository; `<owner/repo>` files them in that repository; `none` files none, and `/ship-plan` lists them for you to record. `follow-up-label`: the label those issues get, `follow-up` by default; write a name with spaces as it is, such as `tech debt`. `gate`: `none`; a command, such as `./build/dod.ps1 -Fast`, that `/ship-plan`, `/run-issues`, `/address-feedback` and `/run-feedback` run from the repository root before each push, stopping when it exits non-zero or changes a file. `protect`: `none`; `hook` makes `.githooks/pre-push` refuse any push to the default branch, for a host that can't require pull requests, such as a private repository on GitHub Free. `pr-format`: `none`; `<file>#<heading>`, such as `AGENTS.md#Pull requests`, names the section that sets the title and description of the pull requests `/ship-plan` opens, which still end with their `Closes` lines. `review-bots`: the review bots, by login and separated by commas, whose findings `/run-issues`, `/address-feedback` and `/run-feedback` may fix, `ncode-review` by default, or `none`; `/run-issues` and `/run-feedback` judge every one of NCode Review's findings, whatever its severity, and fix each that holds, and a bot's finding alone never changes CI, the template, agent configuration, the build or the dependencies: `/address-feedback` and `/run-feedback` change them only when you approve it in their start question. `max-review-rounds`: `3`; after that many rounds of review feedback on a pull request, `/run-issues` and `/run-feedback` comment no further `/review`, so NCode Review isn't asked to review again, and still fix and answer every finding; they recommend the merge once every finding is fixed, declined or answered and the review of the last push they asked for is in. `prelude`: `none`; one PowerShell command that sets up the shell the workflow's commands run in, such as `./build/env.ps1`, a script in the repository, or `nvs use 26`, to put a tool installed through a version manager on `PATH`. `/ship-plan`, `/run-issues`, `/address-feedback` and `/run-feedback` run it before the gate, and executors and coordinators before each Done-when command, dot-sourced in the same PowerShell; a plan names it under Context for the executor, so no step repeats it. When it fails, they stop and show its error. Write it on one line, without a leading `. ` or `& ` operator, a `#` comment, or `;`, `|`, `&`, `&&` or `||`; `.template/Get-PlanState.ps1` prints any other value as `prelude=invalid`, and `/ship-plan` stops on it. `/run-issues` runs only when the host requires pull requests into the default branch, or `protect` is `hook` and the hooks are enabled.

- integrate: pull-request
- release: tag+release
- changelog: CHANGELOG.md
- pre-release: tracked
- draft: yes
- follow-ups: issues
- follow-up-label: follow-up
- gate: ./build/gate.ps1 -Fast
- protect: none
- pr-format: none
- review-bots: ncode-review
- max-review-rounds: 999
- prelude: none

## Conventions

- Public API changes are recorded with `./build/Update-PublicApi.ps1`; the shipped surface is baselined from the `v1.2.0` release.
- A published GitHub release starts `.github/workflows/publish.yml`, which pushes the packages to nuget.org through trusted publishing, with no stored API key (ADR-0135 in `docs/decisions/project-template/`). The Landing section sets `release: tag+release`, so `.github/workflows/tag-release.yml` tags and publishes each release as its pull request merges and starts `publish.yml` (ADR-0016 and ADR-0137, and [ADR-0016](docs/decisions/project-template/0016-releases-are-tagged-and-published-when-their-pull-request-merges.md)). `publish.yml` calls `release-provenance.yml` and `raise-baseline.yml` after the push (ADR-0150 and ADR-0151). Its one-time setup is on nuget.org and in the repository: a trusted publishing policy for `publish.yml`, the `nuget` environment with required reviewers, and the variable `NUGET_USER`.
- Nerdbank.GitVersioning takes every package's version from `version.json`, not the tag: `version` is the version the next release ships, and a plan with a `Release:` line sets it to that version, because `publish.yml` publishes nothing when the tag and `version.json` disagree. `assemblyVersion` has `major` precision, so `AssemblyVersion` is `<major>.0.0.0` for every release of a major version.
- CSharpier formats the code ([ADR-0148](docs/decisions/project-template/0148-csharp-is-formatted-in-the-gate-and-files-carry-the-spdx-header.md)): run `dotnet csharpier format .` before committing. Every C# file starts with the two lines `// Copyright © NCode Group` and `// SPDX-License-Identifier: Apache-2.0`.
- Tests gate coverage at 98% line and branch, and CI publishes the report to Codecov from every push to the default branch ([ADR-0026](docs/decisions/project-template/0026-tests-gate-coverage-at-the-gold-thresholds-and-a-release-waits-for-them.md)).
- Public names pass the .NET naming analyzers without suppressions ([ADR-1004](docs/decisions/1004-public-names-pass-the-naming-analyzers-without-suppressions.md)).
- This repository keeps `.github/workflows/code-review-auto-merge.yml` and so accepts the template's ADR-0006 exception that lets a Dependabot bump of the NCode Review pin auto-merge.
- The packages target only the LTS .NET versions Microsoft supports ([ADR-1002](docs/decisions/1002-the-packages-target-only-supported-lts-dotnet-versions.md)), and documentation names no .NET version or target framework moniker: the project files are the one source, a README shows them through the Shields.io dynamic XML badge that reads `TargetFramework` or `TargetFrameworks` from the project file, and `CHANGELOG.md` records when a framework is added or dropped.
- The C# samples in `README.md`, `src/NCode.PropertyBag/README.md` and `src/NCode.PropertyBag.Abstractions/README.md` are copied by MarkdownSnippets from `tests/NCode.PropertyBag.Tests/ReadmeSamplesTests.cs` and `tests/NCode.PropertyBag.Tests/ReadmeSamples/` ([ADR-1003](docs/decisions/1003-readme-samples-are-compiled-tests-that-markdownsnippets-copies-into-the-readmes.md)): change a sample in its test, between `// begin-snippet: <name>` and `// end-snippet`, then run `dotnet mdsnippets`; never edit a README's `<!-- snippet: -->` block, which `./build/gate.ps1` checks against the tests. A `#region` name in a test needs a space.
- Each package's README is its project's own `README.md`, the page nuget.org shows, with absolute links only; the OpenSSF Scorecard badge is only in the root `README.md`, since nuget.org doesn't render it. A README table has at most three short columns, and a README code line stays under 100 characters.
