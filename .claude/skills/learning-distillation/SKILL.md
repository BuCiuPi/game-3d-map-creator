---
name: learning-distillation
description: Maintain and compress project learning memory. Use when Learnings.md grows large, when the user asks to distill, clean up, summarize, or optimize learning memory, when recurring lessons should become skills, or when CLAUDE.md/AGENTS.md token load should be optimized.
---

# Learning Distillation

Use this to turn raw learning history into compact operational memory.

## Procedure

1. Read `Learnings.md`, `docs/LEARNINGS-SUMMARY.md`, the archives under
   `docs/learnings/`, the skill list under `.claude/skills/`, and the task
   router skill (the `*-task-router` skill named in `CLAUDE.md`).
2. Group learnings by durable workflow, not by date.
3. Classify each group:
   - **Always-on rule**: belongs in `docs/LEARNINGS-SUMMARY.md`.
   - **File-scoped convention**: belongs in a `.claude/rules/*.md` file whose
     `paths:` globs match the files it governs. Mark it a gate only when the
     owner has stated it as one.
   - **Repeatable procedure**: belongs in a dedicated skill under
     `.claude/skills/<skill-name>/`.
   - **Codebase fact** (where something lives, how it is wired): belongs in the
     matching `.claude/knowledge/` file, listed in
     `.claude/knowledge/00-Master-Index.md`.
   - **Forensic context**: archive under `docs/learnings/`.
   - **Obsolete/noisy**: summarize only if it explains a current rule.
4. Before trimming, copy the current `Learnings.md` verbatim to
   `docs/learnings/Learnings.raw-YYYY-MM-DD.md` (UTC date; add a suffix if that
   file already exists).
5. Create or update skills only for workflows with enough reusable procedure to
   justify triggerable context (see the extraction test below).
6. Link every new or renamed skill from the router: add or update the domain
   row in the task router's routing table so the router loads it. A skill the
   router never names is found only by description matching.
7. Keep `Learnings.md` small: index, current deltas, and links.
8. Update `CLAUDE.md` and `AGENTS.md` only so agents read the summary by default
   and raw archives only when needed; never move recipes into them.
9. When a learning implies a mechanical check, do not write the check here:
   hand it to the `encode-invariant` skill, which requires accepted authority.

## Skill Extraction Test

Extract a learning group into a skill when it has at least two of:

- A multi-step procedure agents repeat.
- Fragile ordering or lifecycle rules.
- Concrete file locations or validation gates.
- Recurring review traps.
- A domain-specific decision tree.

Do not create a skill for a single isolated fact; put it in the summary.

## Output Shape

- `docs/LEARNINGS-SUMMARY.md`: distilled default context.
- `docs/learnings/Learnings.raw-YYYY-MM-DD.md`: raw archive.
- `.claude/skills/<skill-name>/SKILL.md`: reusable workflow, linked from the
  task router's routing table.
- `.claude/rules/<topic>.md`: file-scoped conventions with `paths:` globs.
- `.claude/knowledge/<NN>-<Topic>.md`: codebase facts, indexed in
  `00-Master-Index.md`.
- `Learnings.md`: lightweight index plus recent deltas.

## Guardrails

- Preserve raw history before trimming.
- Never delete or rename an existing skill, rule, or agent while distilling;
  update it in place and keep its trigger description accurate.
- Do not let `CLAUDE.md` or `AGENTS.md` become reference manuals.
- Prefer links to skills/docs over repeating long recipes.
- Keep trigger descriptions explicit so skills load only when relevant.
- Distillation that changes a harness rule is logged in `docs/TRACE_LOG.md`
  per `docs/TRACE_SPEC.md`.
