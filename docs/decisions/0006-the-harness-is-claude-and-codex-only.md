# 0006 The Harness Is Claude and Codex Only

Date: 2026-10-25

<!-- Ported from cricket-city-unity docs/decisions/0086-the-harness-is-claude-and-codex-only.md @ 8da403c97; stripped of Cricket history. -->

## Status

Accepted

## Context

Adapters for many agent platforms multiply the surfaces that must stay in sync.
The team uses Claude Code, with Codex as the second agent.

## Decision

The harness surface is `.claude/` plus `CLAUDE.md` for Claude, and `AGENTS.md`
for Codex.

- Rules live in `.claude/rules/` with `paths:` frontmatter (`0004`).
- `onboard-repository` and `audit-onboarding-proposal` are Claude skills:
  user-invoked only (`disable-model-invocation: true`), and the audit runs with
  `context: fork` in a fresh session before any proposed patch is applied.
- No `.agents/`, `.agent/`, Cursor, Copilot, or Antigravity adapters.
- Historical rows in `docs/INTAKE_LOG.md`, `docs/TRACE_LOG.md`,
  `docs/TEST_MATRIX.md`, stories, and older decisions stay as written.

## Alternatives Considered

1. Keep multi-platform adapters: more surfaces than agents actually used.

## Consequences

Positive:

- One place to change a rule or skill.

Tradeoffs:

- Another agent platform reads the markdown directly, without adapters.

## Follow-Up

- None.
