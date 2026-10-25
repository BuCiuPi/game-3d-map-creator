# 0005 Docs Live on the Branch of the Work

Date: 2026-10-25

<!-- Ported from cricket-city-unity docs/decisions/0084-docs-live-on-the-branch-of-the-work.md @ 8da403c97. -->

## Status

Accepted

## Context

A standing "planning" branch for stories, decisions, and plans separates the
record from the code it describes; reviews then see code without its story, and
the two drift.

## Decision

Docs are written on the branch of the work they describe. A feature branch
carries its story, decision records, and `docs/TEST_MATRIX.md` proof rows next
to its code; docs-only work goes on whatever branch the work is happening on.
There is no standing planning branch, and no agent steers docs toward one.

## Alternatives Considered

1. A long-lived docs branch: merges late, and the record stops matching the code.

## Consequences

Positive:

- A pull request carries its own record.

Tradeoffs:

- `docs/INTAKE_LOG.md` and `docs/TRACE_LOG.md` take rows from many branches;
  `.gitattributes` marks them `merge=union`.

## Follow-Up

- None.
