# Learnings Summary

This is the distilled, task-start summary. Keep it concise and update it after
harvesting raw learnings from `Learnings.md` (raw archives live in
`docs/learnings/Learnings.raw-YYYY-MM-DD.md`).

## Always-On Rules

- The neutral template has no application composition root or DI package; mount
  a container and add its first scope only with a real derived-project consumer,
  and treat boot-order changes as high-risk.
- Keep `.meta` GUIDs: move and rename assets in the Editor, never in the file system (`.claude/rules/asset-structure.md`).
- `game.core` stays free of business code and of YGG/Cricket/HyperEel names (`docs/architecture/template-core-data.md`).
- Business code is absent from the template; do not restore the retired Jigsaw surface.
- A normal clone is complete. Optional capabilities use direct, commit-pinned Git UPM only when a real consumer exists.

## Skill Router

- Start any task at the project's task router skill under `.claude/skills/`; it
  routes to the current skills and agents.

- Any development task: `.claude/skills/unity-task-router/`.
- Services, data, gameplay: `.claude/agents/unity-game-logic-developer.md`.
- UI: inline, against `.claude/rules/ui-code.md` (no UI agent, decision `0007`).
- Editor menus and batch passes: `.claude/skills/unity-editor-scripting/`.
- Bundle size: `.claude/skills/bundle-optimization/`.
- Review: `.claude/agents/unity-code-reviewer.md`.
- Harness work: `.claude/skills/harness-lifecycle/`.

## Domain Lessons

_None yet._ Add lessons only from harvested `Learnings.md` entries or `docs/TRACE_LOG.md` friction.

## Review Traps To Remember

- Treat agent review verdicts as leads, not gospel; confirm with code and
  concrete file evidence.

_None yet._

## Maintenance Rules

- Archive raw learnings after harvest instead of deleting them.
- Promote repeatable multi-step patterns into skills.
- Promote path-scoped coding rules into `.claude/rules/`.
- Keep `CLAUDE.md` and `AGENTS.md` as routers, not long reference manuals.
