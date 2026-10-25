# Learnings

This file is the lightweight learning index. It should stay small enough to read
at the start of every task.

For distilled guidance, read:

- `docs/LEARNINGS-SUMMARY.md`

For reusable procedures, start at the project's task router skill under
`.claude/skills/`.

For raw history, read only when investigating why a rule exists:

- `docs/learnings/Learnings.raw-YYYY-MM-DD.md` (one archive per harvest)

## Current Operating Rules

- Use `docs/LEARNINGS-SUMMARY.md` as the default task-start learning context.
- Use raw learning history only when the summary is insufficient or the task
  touches a domain with archived details.
- When a learning grows into a repeatable workflow, extract it into a skill and
  replace the raw prose with a link to that skill.
- When a learning is a rule about code under specific paths, move it into
  `.claude/rules/` and leave a pointer here.
- Keep this file as an index plus the most recent durable deltas; avoid letting
  it become a task diary.
- When this file grows past what is comfortable to read at task start, harvest
  it: distill into `docs/LEARNINGS-SUMMARY.md`, archive the raw entries to
  `docs/learnings/Learnings.raw-YYYY-MM-DD.md`, and keep only the newest deltas
  here. The `learning-distillation` skill under `.claude/skills/` runs that
  harvest.

## Recent Durable Deltas

Add entries newest first, in this shape:

```md
**[YYYY-MM-DD] - Short title**
- Observation: What was found, with the concrete evidence.
- Action: What to do differently from now on.
- Confidence: high | medium | low
```
