# 0002 Role-Based Agent Dispatch: Opus Implements, Fable Designs

Date: 2026-10-25

<!-- Ported from cricket-city-unity docs/decisions/0032-role-based-agent-dispatch.md @ 8da403c97 (accepted there 2026-07-27, amended the same day); stripped of Cricket history. -->

## Status

Accepted

## Context

A router with one axis (a 0–10 risk score) mapped to a model and an effort band
mixes two questions: *what kind of work is this* (design, implement, review,
document) and *how risky is it*. Effort cannot be set per spawn at all
(`0001-agent-spawn-model-effort-routing.md`), so an effort column on the score
map is never enforceable.

## Decision

Dispatch has two axes, implemented in `.claude/skills/unity-task-router/SKILL.md`.

**1. Role decides the model.**

| Role | Trigger | Route | Model |
| --- | --- | --- | --- |
| `orchestrate` | always | main session | Opus |
| `design` | score ≥ 8, or the `+1 novel design` adjuster fired | spawn built-in `Plan`, iterate via `SendMessage` | **Fable** |
| `implement` | product delta | inline, or spawn per score | Opus |
| `review` | lane ≥ normal, after implement | auto-spawn `unity-code-reviewer` (ungated) | Opus |
| `document` | lane ≥ normal, after review | auto-spawn `unity-documenter` (ungated) | Opus |

**2. Score decides lane, spawn, and proof** — never the model.

| Score | Route | `design` first? | `document` after? |
| --- | --- | --- | --- |
| 1–3 | inline always | no | no (tiny lane = no story) |
| 4–7 | domain table decides; spawn when ≥ 6 | only if `+1 novel design` fired | story |
| 8–10 | spawn | yes | story + decision record |

Domain rules in the routing table stay hard; "inline only" domains stay inline
at any score.

**3. Fable is the design consultant only** — never implementer, reviewer, or
documenter. If the live session runs on Fable and the task crosses into a
product delta (C#, prefab, scene, data asset), stop and ask before the first
code edit. Harness and planning artifacts need no gate.

**4. Verbatim relay.** The orchestrator is a courier, not a judge: it pastes
Fable's design to the user, who decides. Opus never overrules Fable on design.

**5. Bulk mechanical fan-out** (mass rename, codegen sweeps) uses the `Workflow`
tool, whose `agent()` takes per-call `model` and `effort`.

**6. Agent frontmatter pins `model: opus`.** Spawns of `unity-game-logic-developer`,
`unity-code-reviewer`, and `unity-documenter` pass no `model`. The `design`
spawn uses the built-in `Plan` agent and must pass `model: "fable"`.

**7. Role maps to context phase** in `docs/CONTEXT_RULES.md`: `orchestrate` →
Intake, `design` → Planning, `implement` → Implementation, `review` →
Validation, `document` → Documentation.

**8. Sequencing.** The router's procedure runs `design` → `implement` →
`review` → `document` and announces the whole arc up front:
`Routing: <domain> / <lane> · Score <n>/10 → <role> (<route>) → …`.
`review` and `document` fire at lane ≥ normal and auto-spawn ungated; `design`
and a spawned `implement` are gated on the user's ok.

**9. Effort.** `medium` is the house default (carried from the reference harness, decision `0007`). Effort
is not scored; never prompt for a downgrade; ask first only when a task
genuinely needs `max`.

## Alternatives Considered

1. Two model bands by score (Sonnet for 4–7, Opus for 8–10): cheaper; the owner
   chose one predictable implementer model instead.
2. Run Fable inline via `/model fable`: re-bills the whole context at Fable
   rates every turn; a spawned design agent starts from the brief only.

## Consequences

Positive:

- One predictable implementer; design gets a frontier model only when it pays.
- Each spawned role inherits a prescribed read set.

Tradeoffs:

- Fable costs more per token; bounded by being design-only and spawn-scoped.
- The verbatim-relay rule depends on orchestrator discipline.

## Follow-Up

- Revisit if the `Agent` tool gains per-call `effort`.
