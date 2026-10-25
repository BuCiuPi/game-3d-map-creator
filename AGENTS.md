# Agent Instructions

This is a **Claude-first** repo. Keep `CLAUDE.md`, `.claude/skills/`, and
`.claude/agents/` as the main working harness for Claude Code.

Non-Claude agents should still use the same harness, but as plain markdown
guidance:

- Read `CLAUDE.md`, `docs/HARNESS.md`, `docs/FEATURE_INTAKE.md`, and
  `docs/CONTEXT_RULES.md` before implementation work.
- Read `.claude/skills/unity-task-router/SKILL.md` at the start of Unity tasks
  and apply its routing decision inline when Claude skills/subagents are not
  available.
- Treat `.claude/agents/*.md` as specialist prompt references. If the platform
  cannot spawn those agents, load the relevant specialist file and do the work in
  the current conversation.
- Treat Claude model names as role labels, not effort hints: `Opus` implements,
  reviews, and documents; `Fable` is design only (decision `0002`). Effort is not
  derived from the task — run at your platform's high routine effort.
- See `docs/AGENT_COMPATIBILITY.md` for the small adapter rules for Codex and
  future agents. Do not reorganize the Claude harness unless explicitly asked.

## Harness

**unity-template** is the team's shared Unity 6 starting project (Unity
6000.3.10f1, URP, 2D, uGUI): the project new Unity games are created from, not a
game (`.claude/knowledge/01-project-brief.md`). Shared team code comes from the
The live template is the neutral bootstrap/UI shell plus DT/DA core under
`Assets/game.bootstrap/`, `Assets/game.ui/`, and `Assets/game.core/`. It has no
internal package or submodule by default; a derived project adds a capability
through pinned Git UPM only when its first real consumer appears (decision
`0008`). Addressable source assets live under `Assets/game.assetbundles/`; only
`game.local` and `game.init` ship in the baseline. The harness follows
cricket-city-unity's operating lessons without importing its game/package
modules (decision `0007`).

This repo runs an operating harness for coding agents. Before work, read:

- `CLAUDE.md` — project overview, architecture, key patterns (auto-loaded by
  Claude Code).
- `docs/AGENT_COMPATIBILITY.md` — how non-Claude agents should interpret this
  Claude-first harness.
- `docs/HARNESS.md` — the operating model and task loop.
- `docs/FEATURE_INTAKE.md` — risk lanes and the risk checklist.
- `docs/CONTEXT_RULES.md` — what to read per lane and phase.
- `.claude/knowledge/00-Master-Index.md` — codebase knowledge base (load the one
  relevant file).
- `.claude/rules/ui-code.md` — UI implementation conventions (Presenter/View ownership, field prefixes).
- `docs/patterns/encoding-invariants.md` — how to turn an accepted rule into a
  mechanical check (Claude skill: `.claude/skills/encode-invariant/`).

Start any Unity task with the `unity-task-router` skill
(`.claude/skills/unity-task-router`): it classifies the task, picks the cheapest
correct path (inline skill vs a spawned specialist agent in `.claude/agents/`),
and names the role for each step (the role, not the score, picks the model).

Every task has two possible outputs: a **product delta** (code,
prefabs, data, tests) and a **harness delta** (docs, conventions, or a
`docs/decisions/` record that eases the next task). Prove changes the Unity way —
compile-clean, EditMode tests where they exist, a Play Mode boot check from
`00_LoadingScene`, Editor screenshots for wiring/UI, all through the Editor or the
Unity CLI (there is no Unity MCP, decision `0007`), and an explicit deferred note
for platform proof. Never claim a check passed that you did not run.

## Planning Rules

- **Show Diffs/Code Implementations**: All implementation plans must include exact diffs or specific code implementation blocks. This ensures the user can accurately review the proposed changes before execution.

## Custom Rules Reference

This project utilizes modular rule sets located in the `.claude/rules/` directory.
Before writing code in a domain, read the matching file (Claude Code auto-loads
them through their `paths:` frontmatter; other agents read them by hand). The set
today is:

- `csharp-code-style.md` — comments, `#region` order, naming families,
  logging, guards. Applies to every C# edit in the repo, not to one domain.
- `csharp-type-choice.md` — `class` / `record` / `struct`.
- `ui-code.md` — Presenter/View ownership and UI field prefixes.
- `asset-structure.md` — where assets live, Addressables tiers, GUID safety.
- `code-comments-and-naming.md` — comments and naming in code.
- `docs-writing.md` — writing under `docs/`.

List the directory rather than trusting this list if a file seems missing.
