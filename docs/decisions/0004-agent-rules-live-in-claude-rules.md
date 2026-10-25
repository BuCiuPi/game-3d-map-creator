# 0004 Agent Rules Live in `.claude/rules/`

Date: 2026-10-25

<!-- Ported from cricket-city-unity docs/decisions/0081-agent-rules-live-in-claude-rules.md @ 8da403c97; stripped of Cricket history. -->

## Status

Accepted

## Context

Coding rules kept in a separate agent folder (`.agents/`, `.agent/`) are read
only when an agent remembers to read them. Claude Code loads a file in
`.claude/rules/` automatically when a file matching its `paths:` frontmatter is
read.

## Decision

1. Rules for the code agents write live in `.claude/rules/` with quoted
   `paths:` globs: `csharp-code-style.md` and `csharp-type-choice.md`
   (`**/*.cs`), `ui-code.md` (UI scripts, presenters, views), `asset-structure.md`
   (`Assets/**`), plus `code-comments-and-naming.md` and `docs-writing.md`.
2. `.claude/skills/` is the only skill source. There is no `.agents/` or
   `.agent/` mirror; Codex and other agents read `.claude/skills/` and
   `.claude/rules/` as markdown (`docs/AGENT_COMPATIBILITY.md`). The template's
   own commit `1fe9173` removed the old embedded `.agent/`, `skills/`, and
   `openspec/` trees; do not recreate them.
3. A subagent or non-Claude agent reads the matching rule before writing code,
   not after review.

## Alternatives Considered

1. Keep a mirrored copy per agent platform: two sources of truth that drift.

## Consequences

Positive:

- Rules load when the matching file is read, without anyone remembering.

Tradeoffs:

- Claude Code loads a rule on a *read* of a matching file; it is not documented
  to load on a *write* of a new file. Read a neighbouring file first.

## Follow-Up

- None.
