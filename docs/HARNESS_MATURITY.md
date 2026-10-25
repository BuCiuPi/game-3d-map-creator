# Harness Maturity Ladder

How far this repository's harness has actually come, and what the next rung
costs.

Each level is judged on artifacts you can open and read. A level counts as
achieved only when its criteria are inspectable in the repository, not when a
mechanism exists that could satisfy them. **Capability is not maturity; use
is.** A command, hook, or template nobody runs leaves the level where it was.

## Levels

### H0: Bare Environment

No repository-level instructions. The model gets a prompt and guesses.

Criteria: no agent instructions, no intake policy, no story, decision, or
validation artifacts.

### H1: Scaffolding and Policy

Static operating instructions, risk lanes, templates, and source-of-truth rules
exist, and they are true for this repository. An agent can follow a documented
workflow.

Criteria:

- `CLAUDE.md` and `AGENTS.md` point agents at the operating docs.
- `docs/HARNESS.md`, `docs/FEATURE_INTAKE.md`, `docs/CONTEXT_RULES.md` exist
  with no unfilled stack placeholders.
- A project task router exists and every skill and role is reachable from it.
- Templates exist under `docs/templates/`.
- `docs/TEST_MATRIX.md` defines proof kinds with real commands.

### H2: Durable Records

Work leaves a written trail that outlives the session, in a form the next agent
finds without being told.

Criteria:

- Decisions are recorded in `docs/decisions/` and kept current (superseded, not
  deleted).
- Story packets exist for normal and high-risk work under `docs/stories/`.
- Proof is recorded per story in `docs/TEST_MATRIX.md`.
- Intake classifications are recorded in `docs/INTAKE_LOG.md`.

### H3: Observability

The harness can see its own failure modes, because friction is written down
rather than rediscovered every session.

Criteria:

- `docs/TRACE_LOG.md` receives rows for normal and high-risk work, per
  `docs/TRACE_SPEC.md`.
- Friction is concrete, not "docs confusing".
- `docs/HARNESS_AUDIT.md` is run periodically and its findings acted on.
- Recurring friction becomes items in `docs/HARNESS_BACKLOG.md`.

### H4: Enforced Gates

The harness mechanically refuses work that skips a required step, rather than
politely asking.

Criteria:

- At least one gate blocks rather than advises.
- Gates fail open on infrastructure problems, so they never wedge the team.
- Gate behavior is itself tested.

### H5: Self-Improving

Friction reliably becomes harness change, and each change is checked against
what it predicted.

Criteria:

- Backlog items carry predicted impact written before implementation.
- Closed items record the actual outcome and compare it honestly.
- Improvements that did not help are recorded as such, not silently reverted.
- High-risk harness changes pause for human confirmation.
- Generic improvements are harvested back to the shared framework.

## Current Assessment

| Level | Status | Evidence |
| --- | --- | --- |
| H0 | Passed | repository exists with the harness installed (framework `833b45a`, pack `unity-client`, 2026-10-25) |
| H1 | Partial | `CLAUDE.md`, `AGENTS.md`, harness docs, templates and a router reaching every skill and agent exist; `docs/TEST_MATRIX.md` has proof steps but none has been run and recorded (2026-10-25) |
| H2 | Not achieved | 0 decision records, 0 story folders, 1 intake row (2026-10-25) |
| H3 | Not achieved | 1 trace row (the bootstrap), no audit run (2026-10-25) |
| H4 | Partial | `.claude/hooks/intake-gate.js` blocks and fails open; its behaviour is tested in the framework, not here (2026-10-25) |
| H5 | Not achieved | no backlog items with predicted impact (2026-10-25) |

## What The Next Rung Costs

- **H1 (Partial → Achieved):** run the proof ladder once (compile, EditMode, boot) and record it in `docs/TEST_MATRIX.md`.
- **H2 (Not achieved):** record the plan's P0 decisions (main platform, delivery tiers, profiles) as decision records, and write a story per normal task.
