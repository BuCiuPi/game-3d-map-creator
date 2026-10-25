## Harness

Claude Code loads this file into every session, but it does not auto-load
`AGENTS.md`. The bare `@` lines below import the always-required harness
context (the "Must in all lanes" set from `docs/CONTEXT_RULES.md`) at
context-load time. Never wrap them in backticks; that disables the import.

@AGENTS.md

@docs/FEATURE_INTAKE.md

Start tasks with the project's task router skill (in `.claude/skills/`), then
read `.claude/knowledge/00-Master-Index.md` to find the knowledge files for the
area you will change. Read the proof table at the top of `docs/TEST_MATRIX.md`
and grep it for the rows covering that area rather than reading it whole.

Roles live in `.claude/agents/`: `architect` designs, the stack's implementers
build, the stack reviewer or `code-reviewer` reviews, and `documenter` records;
the router names which run. `.claude/ONBOARDING.md` orients a new session, and
the `harvest` skill proposes generic improvements back to the framework.

**Markdown is the only source of truth.** There is no database and no CLI; the
files in `docs/` are the record. Before the first code write, classify the work
per `docs/FEATURE_INTAKE.md`, emit the lane block to the user, and append a row
to `docs/INTAKE_LOG.md`. A `PreToolUse` hook blocks the first code write when no
recent intake is logged; docs and markdown are never gated.

On completion, write what the lane owes: the story packet, its
`docs/TEST_MATRIX.md` proof row, a `docs/decisions/NNNN-*.md` record when
architecture or a contract changed, and a `docs/TRACE_LOG.md` row citing the
intake timestamp. See `docs/contracts/harness-artifacts-v1.md` for where each
artifact lives. A `Stop` hook reminds once when normal or high-risk work has no
trace.

Lane-dependent context (`README.md`, `docs/HARNESS.md`, `docs/ARCHITECTURE.md`,
`docs/CONTEXT_RULES.md`, product docs, stories, decisions) is intentionally not
imported; read it per lane, as `docs/CONTEXT_RULES.md` prescribes.
