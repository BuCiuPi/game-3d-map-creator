# Feature Intake

Every implementation prompt enters this gate before code changes. The human does
not need to classify risk — the harness does.

## When the Gate Fires

The gate fires on the prompt that first asks for code — **not** on the first
prompt of the conversation. A discussion that opens as a question and *becomes*
an implementation re-enters here at the pivot. Having already answered in
advisory mode does not exempt the work that follows.

Fire it the moment any of these is true:

- the user asks for something to be built, changed, scaffolded, or wired;
- you are about to call Write or Edit on code for the first time in this task;
- a proposal you made gets approved ("do it", "go ahead", "build stages 0-2");
- an in-flight task grows past what the last classification covered.

## Required Output — Before the First Write

The block under [Output](#output) is **mandatory and user-visible**, not
something you "should be able to say". Emit it in the response immediately
preceding your first file write, then record it by appending a row at the top of
the `docs/INTAKE_LOG.md` table:

```text
| 2026-01-31 09:15 | change request | tiny | One line naming the actual work |
```

The columns are `| Date (UTC) | Type | Lane | Summary |`. The stamp is UTC in the
form `YYYY-MM-DD HH:MM` — `date -u '+%Y-%m-%d %H:%M'`. The lane is exactly one of
`tiny`, `normal`, `high-risk`.

If you cannot fill in every line of that block, you do not yet understand the
task well enough to start writing files.

> Recording is not optional bookkeeping. A classification that exists only in
> chat is invisible to `docs/HARNESS_AUDIT.md` and to the next agent. The
> `PreToolUse` hook `.claude/hooks/intake-gate.js` reads that log and blocks the
> first write to code when no recent entry exists. Which paths count as code is
> set in `.claude/harness.config.json` (`guardedRoots`, `exemptRoots`; when
> `guardedRoots` is empty, every path outside `exemptRoots` is guarded; markdown
> and text files are never gated). `INTAKE_GATE_OFF=1` disables it.
>
> The intake stamp is also how the work is closed: the `Stop` hook
> `.claude/hooks/trace-gate.js` reminds once for each normal or high-risk intake
> whose stamp no `docs/TRACE_LOG.md` row cites (see `docs/TRACE_SPEC.md`).

## Intake Flow

```text
User prompt
    |
    v
Classify input type
    |
    v
Restate as a work item
    |
    v
Find affected code, knowledge docs, rules, and conventions
    |
    v
Run the project risk checklist
    |
    v
Choose lane: tiny, normal, or high-risk
```

## Input Types

| Type | Use when | Typical artifact |
| --- | --- | --- |
| `change request` | Fixing, refining, or extending existing behavior | Direct patch or story |
| `new spec` | Story-sized behavior not yet built | Story packet |
| `spec slice` | One bounded slice of an already-specced feature | Story packet |
| `new initiative` | Adding a whole subsystem, service, or product area end-to-end | Initiative + stories |
| `maintenance request` | Dependency, build, tooling, or configuration work | Story or decision |
| `harness improvement` | Improving the docs, rules, or conventions agents rely on | Docs update or decision |

> These are the exact strings to use in the `docs/INTAKE_LOG.md` Type column.
> This table is authoritative — nothing validates the value, so consistency is
> what makes the log greppable later.

## Lanes

### Tiny

Low-risk, narrow edits contained to one area.

Examples: a single-file change with no cross-cutting surface, styling-only
changes, copy or log-line changes, a localized bug fix with an obvious cause.

Requirements:
- Patch directly; no story needed.
- Keep any affected knowledge doc current.
- Leave the project building; run the quick check that fits.

### Normal

Story-sized behavior with a bounded blast radius (a new screen or component, a
new endpoint or handler, a new service method, a new event subscription).

Requirements:
- Create or update one story from `docs/templates/story.md`.
- Note the expected proof (see `docs/TEST_MATRIX.md`).
- Implement the smallest vertical slice.

### High-Risk

Work that can affect contracts, shared framework code, build or deployment
behavior, data, money, or multiple surfaces at once.

Requirements:
- Use `docs/templates/high-risk-story/` (`overview.md`, `design.md`,
  `execplan.md`, `validation.md`).
- Ask for human confirmation before implementation when direction is ambiguous.
- Record a durable decision in `docs/decisions/NNNN-*.md` (from
  `docs/templates/decision.md`) when behavior, architecture, a contract, auth, or
  a validation requirement changes meaningfully.

## Project Risk Checklist

Mark one flag for each item the work touches.

| Risk flag | Applies when the work touches |
| --- | --- |
| Composition root | `TemplateBootstrap`, a new `LifetimeScope`, boot order, or shell ownership |
| Core API | `Assets/game.core/Runtime/` public types (`DataAsset<TData>`, `DataTable<TRow>`, `DataTableRow`) |
| Serialized assets | prefabs, scenes, ScriptableObjects, `.meta` GUIDs, asset moves or renames |
| Addressables | `Assets/AddressableAssetsData/` groups, profiles, load paths, SmartAddresser layout rules, string addresses in code |
| Packages | `Packages/manifest.json`, `Packages/packages-lock.json`, optional Git UPM capabilities, vendored plugins under `Assets/` |
| Build profiles | `ProjectSettings/` (define symbols, player identity, build scenes), `#if *_PROFILE` / `USE_*` gates |
| Template boundary | adding or removing a module that every new project will inherit |
| Auth / SDKs | adding authentication, networking, Firebase, AppsFlyer, ads, analytics, or backend behavior |

## Classification

```text
0-1 flags:
  tiny or normal, based on code impact and blast radius

2-3 flags:
  normal, with stronger validation

4+ flags:
  high-risk

Any hard gate:
  high-risk unless the human explicitly narrows scope
```

Hard gates (high-risk regardless of flag count):

- Editing shared framework code that other packages, services, or products
  consume.
- Changing a public or cross-system contract.
- Auth / session flow changes.
- Anything that risks money, economy, or user-owned data.
- Removing or weakening a validation expectation.

- Changing the `game.core` public API (`DataAsset<TData>`, `DataTable<TRow>`) without a decision record.
- Adding or changing an optional Git UPM capability, or editing a vendored plugin under `Assets/`.
- Moving or renaming addressable assets in a way that changes `.meta` GUIDs or addresses.

## Output

At the end of intake, the agent should be able to say:

```text
Lane: normal
Reason: adds one endpoint handler and its input validation; no shared contract,
        auth, or data-schema surface touched.
Docs: .claude/knowledge/<the-relevant-file>.md
Proof: unit test for the new logic + integration test for the endpoint.
```
