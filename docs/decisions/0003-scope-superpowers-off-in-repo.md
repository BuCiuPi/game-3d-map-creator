# 0003 Scope the Superpowers Plugin Off in This Repo

Date: 2026-10-25

<!-- Ported from cricket-city-unity docs/decisions/0033-scope-superpowers-off-in-repo.md @ 8da403c97; stripped of Cricket history. -->

## Status

Accepted

## Context

The user-level `superpowers` plugin injects its own process (brainstorm, plan,
execute) at session start. This repository already carries that discipline in a
Unity-specific form: intake lanes (`docs/FEATURE_INTAKE.md`), the
`unity-task-router` role sequence, proof rows (`docs/TEST_MATRIX.md`), and the
trace (`docs/TRACE_LOG.md`). Two process layers give an agent two contradicting
procedures.

## Decision

1. Disable the plugin for this repository in `.claude/settings.json`:

   ```json
   "enabledPlugins": {
     "superpowers@claude-plugins-official": false
   }
   ```

   A project-level `false` overrides a user-level `true` (verified in
   cricket-city-unity on 2026-07-27).
2. Keep it enabled globally; it helps in repositories without a harness.
3. Point at `docs/decisions/` from `CLAUDE.md`'s "AI Harness" section, so
   precedence rules are visible at context-load time.

## Alternatives Considered

1. Disable it user-wide: costs every other project to fix a problem that only
   bites where a harness exists.

## Consequences

Positive:

- One process layer: the harness.

Tradeoffs:

- A teammate who relies on `superpowers` gets it everywhere except here.

## Follow-Up

- None.
