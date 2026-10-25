# Documentation Map

This directory holds the operating harness for coding agents and the record of
the work they do.

Markdown is the only source of truth. Everything the harness knows is a tracked
file — there is no database, CLI, or bootstrap step.

## Product Docs

- `architecture/template-core-data.md`: the DT/DA contract and Addressables placement tiers.
- `../artifacts/unity-template-addressables-plan.html`: the owner's thin-core and delivery-tier plan.
- `../.claude/knowledge/01-project-brief.md`: purpose, heritage, high-risk areas, open questions.

## Operating Docs

- `HARNESS.md`: the operating model, principles, and task loop.
- `FEATURE_INTAKE.md`: how prompts become tiny, normal, or high-risk work.
- `CONTEXT_RULES.md`: what to read per lane and phase, and when to stop.
- `WORKFLOW.md`: the authority gate, proof, and invariant method. Lanes and
  required records still follow the files above.

## Orientation

- `ONBOARDING.md`: a developer's path from a fresh machine to a first change.
- `ARCHITECTURE.md`: modules, boundaries, dependency direction, contracts.
- `GLOSSARY.md`: harness terms.
- `AGENT_COMPATIBILITY.md`: how Codex and other agents use this Claude-first
  harness.

## The Record

- `INTAKE_LOG.md`: intake classifications, newest first.
- `TEST_MATRIX.md`: behavior mapped to proof, per story.
- `TRACE_LOG.md`: what happened per task, and the friction it hit.
- `HARNESS_BACKLOG.md`: harness improvements, from proposed to outcome.
- `TOOL_REGISTRY.md`: optional tools the project equips, and the degrade ladder.
- `LEARNINGS-SUMMARY.md`: distilled, task-start lessons (index in
  `../Learnings.md`).
- `contracts/harness-artifacts-v1.md`: where each artifact lives, its shape, and
  how the hooks parse it.

## Harness Meta

- `TRACE_SPEC.md`: trace depth tiers, intake citation, and friction capture.
- `IMPROVEMENT_PROTOCOL.md`: how friction becomes a harness fix.
- `HARNESS_AUDIT.md`: periodic drift checklist.
- `HARNESS_COMPONENTS.md`: which file covers each harness responsibility,
  and the gaps.
- `HARNESS_MATURITY.md`: the H0 to H5 ladder and where this repository stands.

## Folders

- `stories/`: story packets — see `stories/README.md`.
- `decisions/`: durable decisions, `NNNN-kebab-title.md`, superseded but never
  deleted.
- `plans/active/`, `plans/completed/`: execution plans and harness-improvement
  experiments, moved to `completed/` when closed.
- `learnings/`: raw learning archives, `Learnings.raw-YYYY-MM-DD.md`.
- `templates/`: story, high-risk story set, decision, exec plan, validation
  report, spec intake, application runbook, and harness-improvement formats.
- `patterns/`: reusable methods — `encoding-invariants.md` turns an accepted rule
  into a check.
- `contracts/`: interface contracts for the harness artifacts.

## Outside `docs/`

- `../AGENTS.md`: entry instructions for every agent.
- `../Learnings.md`: the learning index and recent durable deltas.
- `../.claude/knowledge/00-Master-Index.md`: the codebase knowledge base.
- `../.claude/rules/`: coding rules scoped by `paths:` frontmatter.
- `../.claude/skills/`: skills, including the project's task router skill.
- `../.claude/agents/`: `unity-game-logic-developer`, `unity-code-reviewer`,
  `unity-documenter` (decision `0007`); design runs on the built-in `Plan` agent.
- `../.claude/ONBOARDING.md`: working with AI in this repository.
- `../.claude/harness.config.json`: which paths the intake gate guards, and the
  gates' time windows.
