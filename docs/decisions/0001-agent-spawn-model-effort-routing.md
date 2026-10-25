# 0001 Agent Spawn: Model Is Set at Spawn, Effort Is Inherited

Date: 2026-10-25

<!-- Ported from cricket-city-unity docs/decisions/0001-agent-spawn-model-effort-routing.md @ 8da403c97 (accepted there 2026-07-17); stripped of Cricket history. -->

## Status

Accepted. Its model-band half is superseded by `0002-role-based-agent-dispatch.md`;
the finding below stands.

## Context

The `unity-task-router` skill scores a task and decides whether to spawn a
specialist. Spawning goes through the `Agent` tool and the agent definitions in
`.claude/agents/*.md`. Checked against the tool contract:

- `Agent` accepts a per-call `model` parameter. It has **no** `effort`
  parameter.
- A spawned agent's model is the explicit `model` passed, else its frontmatter
  `model`.
- A spawned agent's **effort is inherited from the live session** and cannot be
  set at spawn time.

A router that claims to "set model and effort from the score" fails silently
twice: the frontmatter default runs when `model` is omitted, and the scored
effort is never applied.

## Decision

1. The router states the real mechanism: `model` per spawn, effort inherited.
2. Default mode: when a spawn needs a model other than the frontmatter default,
   pass `model` explicitly. When the task needs a different effort than the
   session, tell the user the exact `/model <model>, <effort>` command and wait.
3. "My way" fallback: when the user opts into the assistant's judgment (for
   example "dùng cách của bạn", "default to your way"), skip the effort-match
   question and spawn with the routed model; effort rides the session. The mode
   holds for the rest of the session unless revoked.

## Alternatives Considered

1. Bake `effort` into each agent's frontmatter: static per agent, too coarse.
2. Route every spawn through the `Workflow` tool (per-call `model` and
   `effort`): heavy, needs explicit user opt-in every time. Kept for bulk
   mechanical fan-out only (`0002`).

## Consequences

Positive:

- The router matches the `Agent` tool contract; no phantom effort setting.

Tradeoffs:

- Effort control depends on the session's setting.

## Follow-Up

- Revisit if the `Agent` tool gains a per-call `effort` parameter.
