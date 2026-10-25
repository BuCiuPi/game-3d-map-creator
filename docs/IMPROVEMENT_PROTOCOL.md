# Improvement Protocol

How harness friction becomes a harness fix.

```text
friction in docs/TRACE_LOG.md
  + drift found by docs/HARNESS_AUDIT.md
  + corrections a human had to make
        |
        v
  backlog item in docs/HARNESS_BACKLOG.md  (with predicted impact)
        |
        v
  human review
        |
        v
  implementation in the lane the item's risk calls for
        |
        v
  close the item with the actual outcome
```

There is no proposal-generating command. A human or an agent reads the log and
writes the item — the judgment about what is worth fixing is the valuable part
of that step, and it is not automatable from row counts.

## Triggers — when each step happens

A loop step with no trigger does not happen. Prose alone is not a trigger: a
step that only a document asks for gets skipped, while a step a hook enforces
gets done.

- **Capture — at the end of every normal/high-risk task.** Enforced by the
  `Stop` hook `.claude/hooks/trace-gate.js`: when a session ends a turn and a
  normal or high-risk intake has no `docs/TRACE_LOG.md` row citing its stamp, it
  names that intake once. Trace only work the current session did.
- **Promote — in the same session a project rule is stated.** An agent's private
  memory is per user and per machine: teammates, other agents, CI, and another
  workstation never see it. When the owner states a rule about *this project*
  (not about how they like to work), write it where the repository loads it —
  `.claude/rules/`, a skill, `CLAUDE.md`, or a knowledge doc — and let private
  memory hold at most a pointer.
- **Harvest — at each audit.** Read the Friction column since the last audit
  and any private memory index; move anything that recurs into
  `docs/LEARNINGS-SUMMARY.md`, a rule, a skill, or a backlog item.
- **Audit — weekly, and at the end of any multi-day initiative, whichever comes
  first.** Besides drift, record the counts of every mechanical check on the
  newest artifacts, so each audit shows whether the violations each check exists
  for went up or down.

<!-- stack: list the mechanical checks whose error and warning counts each audit records (lint, type-check, invariant checks added with docs/patterns/encoding-invariants.md), with the exact command for each. Leave this comment until at least one such check exists. -->

## When To Open An Item

Open one when the same friction shows up **twice**, or once if it was expensive.
Concretely:

- The same missing rule or stale doc appears in two trace entries.
- A human had to correct the same class of agent mistake more than once.
- `docs/HARNESS_AUDIT.md` finds the same drift category on two consecutive runs.
- A single task lost significant time to a gap that will obviously recur.

One-off confusion that nothing else corroborates is not a backlog item. It is a
trace row, and if it recurs the log will say so.

## Write The Item

Use the template at the top of `docs/HARNESS_BACKLOG.md`. Two fields carry the
weight:

- **Current Pain** — cite the evidence. Link the trace rows or name the tasks.
  An item with no evidence is a preference, not an improvement.
- **Predicted Impact** — what should measurably change once this lands. Write
  it *before* implementing, so closing the item is an honest comparison rather
  than a retrofitted justification.

## Review Rules

- **Tiny** items that only clarify a doc may be implemented directly.
- **Normal** items need a story packet or explicit acceptance in the item.
- **High-risk** items — anything changing source-of-truth hierarchy,
  architecture direction, validation requirements, or risk policy — need a
  durable decision record in `docs/decisions/` **before** the change, and human
  confirmation per the Harness Change Policy in `docs/HARNESS.md`.

## Close The Loop

An item is not done when the change lands. Close it by editing the item to add:

```md
### Actual Outcome

What measurably changed, and how you know. Compare against Predicted Impact.
If the prediction was wrong, say so — that is the useful part.
```

Then set `### Status` to `implemented` — for a change proven by rerun, only
after a Keep (see Prove It With A Fresh Rerun). An item that was tried and did
not help gets closed as `rejected` with the outcome recorded — a documented
failed improvement is worth more than a silent revert, because it stops the next
agent proposing the same thing.

## Validating An Improvement

Compare predicted against actual using the same sources that produced the item:

- Does the friction still appear in new `docs/TRACE_LOG.md` entries?
- Does `docs/HARNESS_AUDIT.md` still report the drift category?
- Did the correction a human kept making stop being necessary?

If none of those changed, the item did not work regardless of what shipped.

## Prove It With A Fresh Rerun

A change to how agents behave is proven by a fresh rerun of the job that exposed
the friction, not by the change landing. The trace-log checks above remain the
long-run confirmation.

1. **Keep the baseline.** Before changing anything, record the job, its accepted
   outcome, the failure and its evidence, the human steering it needed, and the
   worker, revision, tools, and authority. No observed baseline, no experiment —
   and one run is not enough to diagnose a worker limitation.
2. **Fix the earliest gap.** Trace the failure to the first owner that could have
   prevented it: missing context, capability, domain owner, authority, proof, or
   environment. Before editing, write the hypothesis — the smallest change and its
   owner, the result expected on the job, the evidence that would weaken it, and
   when to remove it. Change only that.
3. **Rerun fresh.** A new agent session on the same job from an equivalent
   starting state, holding steady the worker (model and role), task class, tools,
   authority, and external conditions. Record whether the change was available,
   actually retrieved or invoked, and relevant.
4. **Keep, revise, or remove.**
   - **Keep** when the rerun exercised the change and the job improved enough to
     justify its cost.
   - **Revise** when the owner is right but the change was hard to find or use;
     revise it, then rerun again.
   - **Remove** when it added noise, duplicated a better owner, or did not help.

Record the experiment with `docs/templates/harness-improvement.md` under
`docs/plans/active/`, and move it to `docs/plans/completed/` only after the rerun
decision. That decision sets the backlog item's `### Status`: Keep →
`implemented`, Remove → `rejected`, Revise or pending fresh rerun → `accepted`.
