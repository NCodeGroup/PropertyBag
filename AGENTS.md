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
- Definition of Done: `./build/dod.ps1` (the full gate CI runs) or `./build/dod.ps1 -Fast` (Debug, no pack); it ends with `Definition of Done: PASSED`. Record a public API change with `./build/Update-PublicApi.ps1`

## Landing

How `/ship-plan` lands a reviewed plan. `integrate`: `pull-request` pushes the plan's branch and opens a pull request; `direct` pushes to the default branch. `release`: `none`; `tag` tags the plan's `Release:` version, or the next free one when another plan landed it first; `tag+release` also publishes a release from that version's section of `changelog`. On GitHub, `.github/workflows/tag-release.yml` tags and publishes it as the pull request merges, from the `Release:` line `/ship-plan` writes in its description or the changelog's newest section, and for a .NET library starts `.github/workflows/publish.yml`; `/ship-plan` run after the merge does only what that workflow didn't. `changelog`: the file every plan's last step writes, in the Keep a Changelog format, `CHANGELOG.md` by default. `pre-release`: `untracked` keeps the project pre-release until a version tag of 1.0.0 or later: nothing it released or stored has a reader that must keep working, so every text describes the product as it is, the changelog's `## [Unreleased]` is a catalog of what the product does that each plan edits in place, and a breaking change needs no deprecation or migration path, as ADR-0015 in `docs/decisions/project-template/` says; `tracked`, the default when the line is missing, tracks every change from the first release, for a project whose releases or stored data already have readers. `draft`: `no`, the default when the line is missing, which a template update never changes, so a project lands as it did; `yes` opens every pull request as a draft, as `/ship-plan draft` does, and a new project that runs NCode Review starts with it, since its approval lets a ready pull request merge, so marking it ready is the person's review (ADR-0022 in `docs/decisions/project-template/`); and `/run-issues` opens its pull requests as drafts either way. `follow-ups`: `issues` files a plan's follow-ups as issues in this repository; `<owner/repo>` files them in that repository; `none` files none, and `/ship-plan` lists them for you to record. `follow-up-label`: the label those issues get, `follow-up` by default; write a name with spaces as it is, such as `tech debt`. `gate`: `none`; a command, such as `./build/dod.ps1 -Fast`, that `/ship-plan`, `/run-issues`, `/address-feedback` and `/run-feedback` run from the repository root before each push, stopping when it exits non-zero or changes a file. `protect`: `none`; `hook` makes `.githooks/pre-push` refuse any push to the default branch, for a host that can't require pull requests, such as a private repository on GitHub Free. `pr-format`: `none`; `<file>#<heading>`, such as `AGENTS.md#Pull requests`, names the section that sets the title and description of the pull requests `/ship-plan` opens, which still end with their `Closes` lines. `review-bots`: the review bots, by login and separated by commas, whose findings `/run-issues`, `/address-feedback` and `/run-feedback` may fix, `ncode-review` by default, or `none`; `/run-issues` and `/run-feedback` judge every one of NCode Review's findings, whatever its severity, and fix each that holds, and a bot's finding alone never changes CI, the template, agent configuration, the build or the dependencies: `/address-feedback` and `/run-feedback` change them only when you approve it in their start question. `max-review-rounds`: `3`; after that many rounds of review feedback on a pull request, `/run-issues` and `/run-feedback` comment no further `/review`, so NCode Review isn't asked to review again, and still fix and answer every finding; they recommend the merge once every finding is fixed, declined or answered and the review of the last push they asked for is in. `prelude`: `none`; one PowerShell command that sets up the shell the workflow's commands run in, such as `./build/env.ps1`, a script in the repository, or `nvs use 26`, to put a tool installed through a version manager on `PATH`. `/ship-plan`, `/run-issues`, `/address-feedback` and `/run-feedback` run it before the gate, and executors and coordinators before each Done-when command, dot-sourced in the same PowerShell; a plan names it under Context for the executor, so no step repeats it. When it fails, they stop and show its error. Write it on one line, without a leading `. ` or `& ` operator, a `#` comment, or `;`, `|`, `&`, `&&` or `||`; `.template/Get-PlanState.ps1` prints any other value as `prelude=invalid`, and `/ship-plan` stops on it. `/run-issues` runs only when the host requires pull requests into the default branch, or `protect` is `hook` and the hooks are enabled.

- integrate: pull-request
- release: none
- changelog: CHANGELOG.md
- pre-release: tracked
- draft: yes
- follow-ups: issues
- follow-up-label: follow-up
- gate: ./build/dod.ps1 -Fast
- protect: none
- pr-format: none
- review-bots: ncode-review
- max-review-rounds: 3
- prelude: none

## Conventions

- Public API changes are recorded with `./build/Update-PublicApi.ps1`; the shipped surface is baselined from the `v1.2.0` release.
- Releases publish from a GitHub release through the `publish` job of `.github/workflows/main.yml` (NuGet API key); the template's `publish.yml` is not used. After `publish`, `main.yml`'s `provenance` job calls `release-provenance.yml` and its `baseline` job calls `raise-baseline.yml`, as the template's `publish.yml` does (the template's ADR-0151). `release-provenance.yml`'s backfill by hand accepts only runs of `publish.yml`, so when the `provenance` job fails, re-run that job in its `main.yml` run while the run's `packages` artifact is kept (14 days). Nerdbank.GitVersioning takes every package's version from `version.json`, not the tag: before publishing release `vX.Y.Z`, set `version` in `version.json` to `X.Y.Z` on the commit the release tags, or `main.yml` fails the release.
- This repository keeps `.github/workflows/code-review-auto-merge.yml` and so accepts the template's ADR-0006 exception that lets a Dependabot bump of the NCode Review pin auto-merge.
