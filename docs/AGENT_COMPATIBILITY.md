# Agent Compatibility

This harness is Claude-first. `CLAUDE.md`, `.claude/skills/`, `.claude/agents/`,
and `.claude/rules/` are the primary working harness. This document explains how
Codex and any other non-Claude agent use that harness without changing Claude's
flow.

## Core Rule

Do not move, rename, or weaken the Claude harness to suit another agent.
Non-Claude agents read the same files as ordinary markdown and run the same
process inline when their platform lacks a Claude feature.

The project task router in `.claude/skills/` is the single canonical router.
Do not select a same-named user-level, plugin, generated, or mirrored router
instead. Do not create a mirror directory of skills or rules for another agent
to discover; mirrors drift. Point the other agent at `.claude/` directly.

## Startup

For any implementation task, read:

1. `AGENTS.md`
2. `CLAUDE.md`
3. `docs/HARNESS.md`
4. `docs/FEATURE_INTAKE.md`
5. `docs/CONTEXT_RULES.md`
6. The project task router's `SKILL.md`, then apply its routing inline.

`CLAUDE.md` imports `AGENTS.md` and `docs/FEATURE_INTAKE.md` with bare `@`
lines. Claude Code resolves them; other agents read those files by hand.

## Claude Concepts on Other Agents

| Claude concept | Non-Claude interpretation |
| --- | --- |
| `Read`, `Grep`, `Glob`, `Bash`, `Edit`, `Write` tools | the platform's equivalent file, search, shell, and edit tools |
| Skill (`.claude/skills/<name>/SKILL.md`) | an instruction document; read it and apply it inline when the task matches its description |
| Subagent (`.claude/agents/<name>.md`) | a specialist playbook; load it and do that role's work in the current conversation |
| Role (`design`, `implement`, `review`, `document`) | the same four steps, run in sequence inline; keep each role's boundary (the reviewer does not fix, the documenter does not decide) |
| Spawn gate | if spawning is unavailable, state the intended route and continue inline unless the step needs the user's approval |
| Router score | decides the lane, whether to spawn, and what proof is owed; not the model and not effort |
| Model names in agent frontmatter | role labels; use your platform's equivalent tier for that role |
| Rule with `paths:` frontmatter | read the matching rule by hand before writing code under those paths |
| Hooks (`.claude/hooks/*.js`) | not run for you; follow what they enforce by discipline: log the intake before the first code write, write the trace before finishing |
| `.claude/settings.json` permissions | Claude-only; use your platform's own approval model |
| Unity CLI / batchmode (no Unity MCP, decision `0007`) | run recompile, EditMode tests and builds through the Unity CLI commands in `.claude/knowledge/03-build-and-profiles.md`, or in the Editor; report Editor proofs you could not run as not run |
| codebase-memory MCP (user-level, not in this repository) | code graph search; fall back to text search and file reads |

## Tooling Expectations

Prefer the repository's configured tools when available:

- Code discovery: codebase-memory graph tools first, then text search and file
  reads for strings, configs, and non-code files.
- Proof: the commands in `docs/TEST_MATRIX.md` and `docs/TOOL_REGISTRY.md`.

If a tool is unavailable, use the best platform-native fallback and report what
was not proven. Never claim a compile, test, screenshot, or device proof that was
not actually produced.

- No Unity MCP: the team drives Unity through the Unity CLI (decision `0007`); no agent declares `mcp__mcp-unity__*` tools.
- codebase-memory MCP: index each checkout before use (`index_repository`).
- Unity batch mode needs the Unity `6000.3.10f1` install. A normal clone is complete; no submodule initialization is required.

## What Not To Do

- Do not rename `.claude/` to a generic directory.
- Do not move the router out of `.claude/skills/`.
- Do not replace Claude subagent files with generic agent files.
- Do not change routing or role policy to fit a non-Claude platform. Routing
  policy is the owner's call and goes through the Harness Change Policy in
  `docs/HARNESS.md`.
- Do not weaken a validation requirement to fit a weaker agent environment.
