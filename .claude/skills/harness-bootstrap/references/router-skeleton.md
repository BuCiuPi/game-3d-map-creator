# Router Skeleton

The shape every project task router follows. `harness-bootstrap` creates
`.claude/skills/<project>-task-router/SKILL.md` from the template below, or
upgrades an existing router in place to contain every part of it.

Everything in `<angle brackets>` is filled from evidence. Keep the arithmetic,
thresholds, and role rules unless the project already has its own calibrated
values; then keep the project's values and record the mapping.

Keep the `description` single-quoted: it contains `: `, which breaks a plain
YAML scalar. Write any apostrophe inside it as `''`.

## Upgrading An Existing Router

Checklist, in order. Nothing in the existing router is removed.

1. Keep the folder, `name`, and every reference file. Extend the `description`
   trigger list with any missing trigger phrases; do not shorten it.
2. Copy every existing domain/route into the domain table. A route written as
   prose or a bullet list becomes a row with the same target. Keep its wording
   in the `When` cell.
3. Keep every hard gate, guardrail, and "never" rule verbatim, under the
   section the skeleton gives them (hard gates go into the lane step).
4. Keep every skill and agent reference. If the existing router names a skill
   or agent that does not exist, keep the row and report the dangling target.
5. Add the skeleton parts it lacks: routing line, lanes, scoring, role sequence,
   role ↔ phase map, spawn gate, inline-when-unsure.
6. If it has its own scoring scale (for example 0–100 with lane bands), keep
   it. Add a short "Mapping" table from its bands to the skeleton's bands
   (lane, spawn threshold, design-first threshold) instead of a second score.
7. Add standing rows that are missing (`code-review`, `invariant-guard`,
   `harness-maintenance`, `general`).
8. Add the Roles table's `Model` column when it is missing (`session` for every
   role unless the project or the `agent-team` pack sets a tier). With
   `agent-team` installed, add the `test` and `escalate` roles from `docs/TEAM.md`
   ("Router Rows").
9. Point the role table at the agents present in `.claude/agents/`: design →
   `architect`, implement → the pack implementers, review → the pack's stack
   reviewer or `code-reviewer`, document → `documenter`. Keep any existing
   project agent mapped to the role it already plays.

## Template

````markdown
---
name: <project>-task-router
description: 'Use at the START of any development task in this repo before doing the work — <list the real task kinds: add/fix/refactor/review <surfaces>, e.g. endpoints, screens, migrations, jobs, build config>. Classifies the task by domain and risk lane, scores it, and routes it to the cheapest correct path: load an in-context skill, or propose spawning a specialist agent. Triggers on requests like "add/fix/build/refactor/review … [<real nouns from this codebase>]".'
---

# <Project> Task Router

A thin classifier + dispatcher. It runs in the live conversation. It does NOT
spawn sub-subagents and does NOT fan out. It picks one route and gets out of the
way, biased toward staying inline.

<2–4 lines: what this repository is and where the live code surface is, cited
from the knowledge index. Name frozen or heritage areas, if any.>

## Procedure

1. **Check it is a development task for this repo.** If not (a question, an
   unrelated script, a docs-only edit outside the harness), say nothing about
   routing and proceed normally. Do not announce a no-op.

2. **Classify, score, and announce the whole role plan in your first reply.**
   - **Domain** — pick one primary domain from the routing table (a secondary
     only when its validation boundary is also affected).
   - **Lane** — `tiny` / `normal` / `high-risk` per `docs/FEATURE_INTAKE.md`,
     including its project risk checklist and hard gates. Uncertainty about a
     hard gate promotes the lane.
   - **Score** — see "Scoring".

   Announce the sequence, not just the next step, so the user can stop it early:

   `Routing: <domain> / <lane> · Score <n>/10 → <role> (<route>) → <role> → …`

3. **Route per the table.** Default to inline on the inline/spawn boundary.

   | Domain | When | Route | Target |
   |---|---|---|---|
   | `<domain>` | <signals: paths, modules, nouns that put a task here> | <route> | <skill or agent that exists, or —> |
   | `code-review` | Review existing code / a branch / a PR | **Auto-spawn** (read-only, ungated) | `<stack reviewer>` when a pack ships one, else `code-reviewer` |
   | `invariant-guard` | Enforce an accepted rule with a mechanical check, or stop a documented violation recurring | Inline | load `encode-invariant`; it stops when no accepted authority states the rule |
   | `harness-maintenance` | Harness status, onboarding, tuning (add knowledge, skills, rules, roles; port from a reference repo), shipping to the team, distilling learnings, contributing improvements back to the framework | Inline | `harness-lifecycle` (entry point), `learning-distillation`, `harness-bootstrap` (refresh), `onboard-repository`, `audit-onboarding-proposal`, `harvest` |
   | `general` | Small edits not covered above | Inline | nearest rule in `.claude/rules/` |

   Route vocabulary — use exactly these:
   - **Inline** — load the named skill and work in this conversation.
   - **Inline by default; spawn if large/multi-file** — the score breaks the tie.
   - **Inline only, always ≥ normal** — contract surfaces that must never be
     delegated blind (a wire protocol, auth handshake, public API shape).
   - **Inline, always high-risk** — no proven end-to-end path exists yet;
     confirm with a human first.
   - **Spawn, gated** — propose the spawn and wait for the user's ok.
   - **Auto-spawn** — read-only or artifact-only work the lane already
     mandates; no permission needed.

   **`code-review` the domain vs `review` the role** — same reviewer, two uses.
   The domain is when reviewing existing code IS the task. The role is the
   validation pass over work this task just produced. Both auto-spawn ungated.

4. **Run the role sequence.** Skip a role only when its trigger did not fire.

   | Order | Role | Fires when | Gated? |
   |---|---|---|---|
   | 1 | `design` | score ≥ 8, or the `+1 novel design` adjuster fired | **yes** |
   | 2 | `implement` | a product delta exists | only if spawned |
   | 3 | `review` | lane ≥ normal | no — auto-spawn |
   | 4 | `document` | lane ≥ normal | no — auto-spawn |

   The tiny lane runs `implement` alone: `docs/FEATURE_INTAKE.md` says patch
   directly, no story, so `review` and `document` do not fire there.

   <With the `agent-team` pack only: `test` fires after `implement` and before
   `review` (lane ≥ normal with a behaviour change; before `implement` as
   test-first on high-risk value-moving or auth work), and `escalate` fires at
   any step where a `docs/TEAM.md` trigger does. Both auto-spawn ungated.
   Without the pack, delete this paragraph.>

5. **Spawn gate.** For gated spawns, state your read and the brief, then WAIT
   for the user's ok before spawning:
   > Routing: <domain> / normal. I'll spawn `<agent>` with: [inputs, target
   > folder, brief]. Sound right?

   `review` and `document` auto-spawn ungated: both produce artifacts the lane
   already mandates, so asking permission is friction, not safety.

   **"My way" fallback.** When the user has told you to default to your own
   judgment, skip the confirm questions and proceed; still print the routing
   line. This holds for the rest of the session unless revoked.

6. **Inline route.** Announce the lane, load the named skill, and continue the
   task in this conversation — no subagent. This is the path for iterative work.

7. **Prove, record, report.** Hand back to the task loop in `docs/HARNESS.md`:
   prove against `docs/TEST_MATRIX.md` and add the proof row, write any
   `docs/decisions/` record the lane owes, log it in `docs/TRACE_LOG.md`
   (normal and high-risk always; tiny only on friction or a harness-rule change
   — `docs/TRACE_SPEC.md`), then report what changed and what was not attempted.

## Roles

The **role** decides who does the work; the **score** decides the lane, whether
to spawn, and what proof is owed. The score does not pick a model.

| Role | Trigger | Route | Agent | Model |
|---|---|---|---|---|
| `orchestrate` | always | this session | — | session |
| `design` | score ≥ 8, or `+1 novel design` fired | spawn, gated; continue with `SendMessage` | `architect` | `<tier or session>` |
| `implement` | product delta | inline, or spawn per score | `<pack implementers in .claude/agents/, one line each with the domains it serves>`, else inline | `<tier or session>` |
| `review` | lane ≥ normal, after implement | auto-spawn (ungated) | `<stack reviewer>` when a pack ships one, else `code-reviewer` | `<tier or session>` |
| `document` | lane ≥ normal, after review | auto-spawn (ungated) | `documenter` | `<tier or session>` |

`Model` is `session` (inherit the live session's model) unless the project sets
a tier. With the `agent-team` pack, take the tiers and the `test` and `escalate`
rows from `docs/TEAM.md` (roster and "Router Rows"), and keep the repository's high-risk domains inline
so a cheaper implement tier never writes them. A reviewer's tier is never below
the implementer's.

Core ships `architect`, `code-reviewer`, and `documenter`; packs add the
implementers and a stack reviewer that extends `code-reviewer`. Name only agents
that exist in `.claude/agents/`. If a repository has removed a core role, fall
back to: built-in `Plan` for design, built-in `general-purpose` read-only with a
brief built from `.claude/rules/`, the changed files, and the lane's proof rows
for review, and inline writing for document.

Each role maps 1:1 to a context phase in `docs/CONTEXT_RULES.md` and inherits
that phase's read table:

| Role | Context phase |
|---|---|
| `orchestrate` | Intake |
| `design` | Planning |
| `implement` | Implementation |
| `review` | Validation |
| `document` | Documentation |

With `agent-team`: `test` → Validation, `escalate` → Planning.

**Spawn mechanics.** The `Agent` tool takes a per-call `model` but no `effort`;
effort is inherited from the live session. Pass the role's `Model` cell as
`model` on every spawn when it names a tier; omit it for `session`. The call's
`model` overrides an agent's frontmatter, so tiers live here and framework
agents stay `model: inherit` (editing their frontmatter makes them local copies
that stop receiving framework updates). Continue a design conversation with
`SendMessage`; a fresh `Agent` call starts over.

**Relay design verbatim.** The orchestrator is a courier, not a judge: paste
the design output and let the user decide. Subagent reports are not shown to the
user by default.

**Design-model gate (only if the project reserves a model for design).** If the
live session runs on that model and the task crosses into implementation, stop
before the first code edit and ask whether to implement inline anyway or route
per the score. Harness and planning artifacts need no gate.

**Effort.** Effort is not scored. If the session is already at or above what
the task needs, proceed silently; never prompt for a downgrade. If the task
genuinely needs the maximum, say why and ask first. Bulk mechanical fan-out that
wants a cheap model at low effort is a workflow job, not an `Agent` spawn.

## Scoring

**Score = lane baseline + adjusters, clamped to 1–10.** Thresholds are
tunable; calibrate them from `docs/TRACE_LOG.md` friction.

- Baseline: `tiny → 2`, `normal → 5`, `high-risk → 6`
- `+2` touches a high-impact surface: <list from FEATURE_INTAKE hard gates —
  composition root / DI container, app bootstrap, auth, persistence schema or
  migrations, public API or wire contracts, shared packages or submodules,
  build/deploy configuration>
- `+1` user-visible platform change that needs a device, browser, or
  environment check the agent cannot run itself
- `+1` cross-module or multi-file blast radius
- `+1` novel design, no established pattern · `−1` well-trodden pattern (a
  skill scaffolds it, or a near-identical sibling exists to copy)
- `−2` purely mechanical (rename, text/config value, comment) — **also
  suppresses the multi-file `+1`**: a mechanical change is cheap regardless of
  fan-out

| Score | Route | `design` first? | `document` after? |
|---|---|---|---|
| 1–3 | Inline always | no | no (tiny lane = no story) |
| 4–7 | Domain table decides; spawn when ≥ 6 | only if `+1 novel design` fired | story |
| 8–10 | Spawn | **yes** | story + decision record |

Domain rules in the routing table are hard. An "inline only" domain stays
inline at any score; the score only breaks ties where the table already says
"inline by default; spawn if large".

**Why baseline 6 for high-risk:** plain high-risk (a phased feature with no
scary surface) lands at 6 and goes straight to implement; high-risk that also
trips a `+2` surface climbs to 8 and buys a design consult first. The adjuster
decides whether design is worth paying for, instead of flattening every
high-risk task to 10.

## Notes

- When unsure between inline and spawn, choose inline.
- This skill complements normal skill auto-triggering; it does not replace it.
- <Project-specific notes kept from an upgraded router, or evidence-backed
  traps: frozen areas, contract surfaces, generated code that must be
  regenerated rather than edited.>
````

## Worked Examples

Illustrations of the arithmetic only. Do not copy their domains into a
project; derive domains from that project's code.

**Example from a Unity project.** Request: "Add a reward popup presenter that
calls a new claim endpoint and register the service in the global DI scope."
Domain `game-logic`, lane `normal` (bounded feature, but it edits the DI
composition root). Score: baseline 5, `+2` composition-root surface, `+1`
multi-file (presenter, view prefab, service, gateway, scope) = 8. Score 8 means
design first, then spawn implement (both gated), then review and document with
a story and a decision record.

> Routing: game-logic / normal · Score 8/10 → design (architect, gated) → implement (spawn, gated) → review (auto) → document (story + decision)

**Example from a TypeScript service.** Request: "Add `GET /orders/:id/history`
following the existing orders controller." Domain `http-route`, lane `normal`
(new endpoint, no schema change, no auth change). Score: baseline 5, `+1`
multi-file (route, controller, service, test), `−1` well-trodden (a
near-identical sibling route exists) = 5. Score 5 means the domain table
decides — inline by default — with no design step; review and document fire
because the lane is normal.

> Routing: http-route / normal · Score 5/10 → implement (inline) → review (auto) → document (story)
