---
name: encode-invariant
description: Convert accepted repository rules into the smallest repository-native mechanical validation with positive and negative proof and precise enforcement-level reporting. Use for requests to enforce architecture, reliability, security, or quality boundaries; prevent a documented violation from recurring; add structural guards; or turn accepted rules into validation. Do not use to infer or invent policy from conventions, code patterns, tests, defaults, or undocumented preferences.
---

# Encode Invariant

Turn an accepted rule into a focused guard without creating new product policy.

## 1. Read The Owners

Read `CLAUDE.md`, `AGENTS.md`, `docs/WORKFLOW.md`, the matching `.claude/rules/`
file, and the complete encoding pattern in `docs/patterns/encoding-invariants.md`.
Locate the repository's product or technical authority and its native validation
owner.

## 2. Pass The Authority Gate

Cite the accepted source and restate the exact scope, allowed behavior,
forbidden behavior, and authorized exceptions. Stop before edits if authority
is absent or materially ambiguous.

Do not promote conventions, code patterns, tests, defaults, tool behavior, or
undocumented preferences into policy. An existing check without accepted
authority is a mismatch to report, not authority to expand.

## 3. Design The Smallest Guard

Reuse the repository's existing test, build, task, lint, scan, or validation
owner. Choose the lowest deterministic layer that sees the complete accepted
scope. Avoid a parallel framework or duplicated source of truth.

Make failures actionable: name the violating item, the broken rule, the
authority source, and a concrete compliant next action.

## 4. Implement And Prove

Implement only the authorized boundary. Add or run:

- positive proof showing a conforming case passes; and
- negative proof showing the targeted violation fails for the intended rule and
  diagnostic.

Use recoverable fixtures or test mutations for negative proof. Preserve
unrelated work and never leave a deliberate violation in product files.

## 5. Discover Enforcement Without Expanding It

Inspect the checked-in validation and CI paths. Do not install hooks, choose a
CI provider, change merge policy, or mutate external branch protection unless
the user separately authorizes that action.

Report each level independently:

- local validation command and observed result;
- optional hook availability, if any;
- CI invocation discovered or absent, plus observed run status only when
  verified; and
- branch-protection enforcement verified or unverified.

Source presence is not execution; CI presence is not a required check; a green
check is not proof of merge blocking.

## In This Repository

- **Log the intake for every guard.** Emit the `docs/FEATURE_INTAKE.md` lane
  block and add a `docs/INTAKE_LOG.md` row before the first write. Do not count on
  `.claude/hooks/intake-gate.js` to stop you: it only sees writes under the
  `guardedRoots` in `.claude/harness.config.json`, skips the `exemptRoots` and
  documentation files, accepts any recent intake row, and lets the write through
  on any error. It is not a validation owner — it checks that an intake is
  logged, nothing else.
- **Authority** lives in `CLAUDE.md`, `AGENTS.md`, `.claude/rules/`,
  `docs/HARNESS.md`, `docs/FEATURE_INTAKE.md`, `docs/CONTEXT_RULES.md`, and
  `docs/decisions/`. A file is authority only for what it states; its examples and
  checklists are not extra invariants.
- **Precedence.** `docs/WORKFLOW.md` and `docs/patterns/encoding-invariants.md`
  supply the authority gate, proof, and method. Lanes and required records follow
  `CLAUDE.md`, `docs/FEATURE_INTAKE.md`, and
  `docs/contracts/harness-artifacts-v1.md`. Record the guard's proof in
  `docs/TEST_MATRIX.md` and its run in `docs/TRACE_LOG.md`.

- **Validation owners that exist today:** the EditMode test assembly `game.core.tests.editmode` (`Assets/game.core/Tests/EditMode/`), which already checks DT validation (null rows, empty and duplicate IDs). Prefer an EditMode test for anything plain C# can show, and an Editor validation pass for asset, group and prefab rules (`unity-editor-scripting`); the plan's P5 template-native asset checker will be the owner for Addressables rules.
- **Proof mechanics:** EditMode tests need the Unity Editor or batch mode. A normal clone is complete; there is no package-submodule prerequisite. When Unity is unavailable, report the proof as not run.
- **Enforcement levels here:** no CI is checked in, so a check is local until CI exists. Branch protection on `hoangphuc25071995/unity-template` is unverified; commits so far land directly on `main`.

## Handback

Return accepted authority, encoded scope, changed owner, actionable diagnostic,
positive and negative results, the four enforcement levels, and unresolved
risks or authority gaps.
