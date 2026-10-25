# Working With AI In This Repository

A five-minute orientation for anyone, human or agent, starting to work here with
Claude Code or another coding agent. Read it once; afterwards `CLAUDE.md`
(auto-loaded) and `docs/CONTEXT_RULES.md` tell you what to read per task.
Humans setting up a machine start with `docs/ONBOARDING.md`.

## 1. What Loads On Its Own

| When | What | How |
| --- | --- | --- |
| every session | `CLAUDE.md` | Claude Code reads it at start |
| every session | `AGENTS.md`, `docs/FEATURE_INTAKE.md` | bare `@` import lines in `CLAUDE.md` |
| a matching file is touched | `.claude/rules/<topic>.md` | its `paths:` frontmatter |
| the task matches a description | `.claude/skills/<name>/SKILL.md` | skill auto-triggering |
| before the first code write | `.claude/hooks/intake-gate.js` | `PreToolUse` hook |
| when a session stops | `.claude/hooks/trace-gate.js` | `Stop` hook |

Everything else is read on purpose, per lane and phase, as
`docs/CONTEXT_RULES.md` prescribes. Non-Claude agents read all of it by hand;
see `docs/AGENT_COMPATIBILITY.md`.

## 2. The Loop

Every task runs the same loop:

1. **Intake**: classify the work as tiny, normal, or high-risk
   (`docs/FEATURE_INTAKE.md`), show the lane block, and log a row in
   `docs/INTAKE_LOG.md`.
2. **Route**: the project task router picks the domain, scores the task, and
   announces the role sequence.
3. **Work**: inline with a skill, or through a spawned role agent.
4. **Prove**: run the proof `docs/TEST_MATRIX.md` names for the area; never
   claim a check that was not run.
5. **Trace**: record what the lane owes (story, proof row, decision record) and
   a `docs/TRACE_LOG.md` row with concrete friction.
6. **Distill**: when `Learnings.md` grows, `learning-distillation` compresses it
   and promotes recurring lessons into rules, skills, or knowledge.
7. **Harvest**: when something here would help every repository of this kind,
   `harvest` writes a proposal for the shared framework.

## 3. The Router

`unity-task-router` (`.claude/skills/unity-task-router/`), the same router and
role model as cricket-city-unity (decision `0007`). Domains:

- `template-boundary` — what stays in the template and what becomes a feature module
- `bootstrap-di` — `TemplateBootstrap`, shell ownership, boot order, or a real
  derived-project composition scope after its DI package is mounted
- `core-data` — `Assets/game.core/` DT/DA primitives and tests
- `game-logic` — gameplay and stage loading, master data, R3 event hubs
- `lobby-integration` — lobby screens and flows, navigation
- `feature-service` — a server-backed feature: service, gateway, endpoint
- `ui-screen-build` — uGUI screens, popups, HUD, prefabs (inline; no UI agent)
- `addressables` — groups, profiles, SmartAddresser rules, asset moves, download sizes
- `editor-tooling` — Editor menus, scripted prefab/scene edits, batch passes
- `packages` — `Packages/manifest.json`, its lockfile, and optional Git UPM capabilities
- `build-profiles` — define symbols, `ProjectSettings/`, player identity, build scenes
- `code-review`, `general-scripting`, `invariant-guard`, `feature-planning`,
  `harness-maintenance` — the standing rows

Start every Unity task with the router. Its first reply is a routing line:

```text
Routing: <domain> / <lane> · Score <n>/10 → <role> (<route>) → <role> → ...
```

You can stop or redirect the sequence at that line. When unsure between inline
and spawning an agent, the router stays inline.

## 4. Roles

Work moves through four roles. The role picks the model; the score only decides
whether a role fires and whether to spawn (decision `0002`).

| Role | Fires when | Who | Model |
| --- | --- | --- | --- |
| design | score 8 or more, or a novel design | built-in `Plan` agent, spawned with `model: "fable"`; its design is relayed verbatim | Fable |
| implement | there is a product change | inline, or `unity-game-logic-developer` when the score says spawn | Opus |
| review | lane normal or high-risk | `unity-code-reviewer`, read-only, auto-spawned | Opus |
| document | lane normal or high-risk | `unity-documenter`: stories, decisions, proof and trace rows from decisions already made | Opus |

There is no UI agent: screens are built inline against `.claude/rules/ui-code.md`.
There is no Unity MCP: Editor proof runs in the Editor or through the Unity CLI
(decision `0007`).

## 5. Where Knowledge Lives

| Need | Look in |
| --- | --- |
| how the code works, how to add X | `.claude/knowledge/00-Master-Index.md`, then the one file it points to |
| conventions for a kind of file | `.claude/rules/` (loads by path) |
| why something is the way it is | `docs/decisions/` |
| what a feature was meant to do | `docs/stories/` |
| how a change is proven | `docs/TEST_MATRIX.md`, `docs/TOOL_REGISTRY.md` |
| what went wrong before | `docs/LEARNINGS-SUMMARY.md`, the Friction column of `docs/TRACE_LOG.md` |
| harness terms | `docs/GLOSSARY.md` |

## 6. Extending The Harness

The installer gives every repository the same core. Tuning it for this
repository is normal work: log an intake like any other change, and keep each
addition small, evidence-backed, and reachable from the router.

**Add a skill** when a multi-step procedure recurs:

1. Create `.claude/skills/<name>/SKILL.md` with `name` and `description`
   frontmatter; the description says when to use it and when not to.
2. Keep the body to the procedure; put long references in `references/`.
3. Add a router row that points at it, and a `docs/TOOL_REGISTRY.md` row.

**Add a rule** when a convention for one kind of file keeps being corrected:

1. Create `.claude/rules/<topic>.md` with quoted `paths:` globs, following
   `.claude/skills/harness-bootstrap/references/rule-template.md`.
2. Mark it gate or guideline, and cite its authority (a config, a decision, an
   owner's instruction). An observed pattern is at most a guideline.

**Add a role** when a kind of work needs its own context and boundary:

1. Create `.claude/agents/<name>.md` with `name`, `description` (when to use,
   when not to), and `tools` frontmatter; the body names scope, inputs to read,
   procedure, proof to run, and handback format.
2. Map it to a role in the router's role table.

**Add knowledge** when an area took real reading to understand: write a file in
`.claude/knowledge/`, cite paths for every claim, and add it to the index.

Never delete or rename a shipped harness file to customize it; the installer
tracks them. Change behavior by adding files or editing your copy, and record
the reason in a decision when the change is high-risk.

## 7. Lifecycle Of The Harness Here

1. **Init**: the installer copies core and the selected packs.
2. **Onboarding**: `onboard-repository` maps the repository; `harness-bootstrap`
   makes the harness true for it (router, knowledge, rules, placeholders).
3. **Tuning**: the team adds its own skills, rules, roles, and knowledge.
4. **Instruction**: `CLAUDE.md`, `AGENTS.md`, and the router are committed so
   the whole team shares them.
5. **Loop**: every task runs intake, route, work, prove, trace; learnings are
   distilled; generic improvements are harvested back to the framework.

The `harness-lifecycle` skill drives all five: ask "harness status" to see
where this repository stands and what to do next, or name a mode (`onboard`,
`tune`, `ship`, `refresh`).
