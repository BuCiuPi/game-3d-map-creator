# Trace Specification

A **trace** is the record of what actually happened during a task: what the
agent did, what it read, what it changed, what broke, and what about the harness
made the work harder than it should have been.

Traces live in `docs/TRACE_LOG.md` as plain markdown. There is no database and
no CLI — the log is the record. Its value is entirely in the friction column:
that is the raw material `docs/IMPROVEMENT_PROTOCOL.md` turns into harness
improvements.

## What Gets A Trace

Not every task. Recording a trace for a one-line copy fix is bookkeeping for its
own sake.

| Lane | Trace? | Depth |
| --- | --- | --- |
| Tiny | Only if it hit friction or changed harness rules | Standard |
| Normal | Yes | Standard |
| High-risk | Yes | Detailed |

A tiny task that ran clean needs no entry. A tiny task that revealed a stale
doc, a missing rule, or a contradiction **does** — that is exactly the signal
the log exists to capture.

## How A Trace Closes An Intake

Every normal or high-risk row in `docs/INTAKE_LOG.md` is closed by a trace row
that cites its stamp in the `Intake` column. The `Stop` hook
`.claude/hooks/trace-gate.js` treats an intake as traced when its stamp string
(`YYYY-MM-DD HH:MM`) appears anywhere in `docs/TRACE_LOG.md`; for each untraced
normal or high-risk intake it reminds once. It looks only at intakes the current
git user added, old enough for the work to have finished and recent enough to
still matter (`traceMinAgeMinutes`, `traceWindowHours` in
`.claude/harness.config.json`). One trace row may close several intakes — list
their stamps comma-separated. `TRACE_GATE_OFF=1` disables the hook.

Trace only work the current session did.

## Depth Tiers

### Standard — one table row

Add a row at the top of the `docs/TRACE_LOG.md` table:

```text
| Date (UTC) | Intake | Task | Lane | Outcome | Changed | Friction |
```

| Field | Content |
| --- | --- |
| Date (UTC) | `YYYY-MM-DD` |
| Intake | The intake stamp(s) this trace closes, `YYYY-MM-DD HH:MM`, comma-separated; `—` for a tiny task that had no intake row |
| Task | One line naming the outcome or attempted outcome; link the story packet when there is one |
| Lane | `tiny` · `normal` · `high-risk` |
| Outcome | `completed` · `partial` · `blocked` · `failed`, plus the proof actually run |
| Changed | The changed-file set at a useful level (paths or an area name), or a commit sha |
| Friction | The concrete pain, or `none` — see below |

### Detailed — a row plus a section

High-risk work adds a `### YYYY-MM-DD — Task` section below the table with:

- **Actions** — the sequence actually taken, not the plan.
- **Read** — files and commands consulted. This is what makes a trace
  reviewable against `docs/CONTEXT_RULES.md`: it shows whether the right
  context was loaded for the lane.
- **Decisions** — scope calls, validation choices, and explicit non-goals.
  This summarizes; it does not replace a durable decision record. If the work
  changed behavior, architecture, a contract, auth, or a validation
  requirement, write `docs/decisions/NNNN-*.md` too.
- **Errors** — real blockers hit along the way, or `none`.
- **Notes** — anything a later reviewer needs that no other field holds
  (e.g. one trace covering several stories).

Duration and token estimates are not tracked. They are unreliable and nothing
consumes them.

## Friction Capture

This is the part that matters. Fill it when any of these happened:

- You had to infer a missing rule or guess the source of truth.
- Required validation was unclear, unavailable, or too expensive to run.
- A doc, story, or decision was stale or contradicted another.
- You repeated a manual step that should be a template, script, or checklist.
- A requested change was out of scope but is likely to matter later.

How to write it:

- Name the concrete pain, not a mood.
- Say what was missing or what contradicted what.
- If it should become work, add an item to `docs/HARNESS_BACKLOG.md` and link
  it from the friction cell.
- Write `none` only when you actively checked and found nothing.

Good:

```text
CONTEXT_RULES says read the knowledge base before changing request validation,
but no knowledge doc covers the validation middleware — had to read its source
to find where errors are mapped. Backlog: "knowledge base missing validation
middleware recipe".
```

Weak:

```text
docs confusing
```

## Review Checklist

Before the final response:

- The depth matches the lane.
- `Intake` cites every intake stamp the work answered.
- `Changed` reflects the real changed-file set, not the intended one.
- `Outcome` is honest — `partial` and `blocked` are useful; a false
  `completed` poisons the log.
- `Friction` names something concrete or is a deliberate `none`.
- Friction worth acting on became a backlog item.
