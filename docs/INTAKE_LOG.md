# Intake Log

Append-only record of feature-intake classifications. This is the durable home
for the classification block that `docs/FEATURE_INTAKE.md` requires before the
first code write — plain text, version-controlled, readable without tooling.

Two hooks read this file:

- `.claude/hooks/intake-gate.js` (`PreToolUse`) blocks the first write to a
  guarded code path when the newest entry is older than its window
  (`intakeWindowHours`), and tells you to classify. Guarded paths and the window
  come from `.claude/harness.config.json` (`guardedRoots`, `exemptRoots`; when
  `guardedRoots` is empty every path outside `exemptRoots` is guarded); markdown
  and text files are never gated, so
  recording an intake is itself never blocked. `INTAKE_GATE_OFF=1` disables it.
- `.claude/hooks/trace-gate.js` (`Stop`) reminds once for each `normal` or
  `high-risk` row whose stamp no `docs/TRACE_LOG.md` row cites.
  `TRACE_GATE_OFF=1` disables it.

## How To Record One

Add a row at the **top** of the table (newest first). Use a UTC stamp in the
form `YYYY-MM-DD HH:MM`:

```bash
date -u '+%Y-%m-%d %H:%M'
```

| Column | Value |
| --- | --- |
| Date (UTC) | The stamp, `YYYY-MM-DD HH:MM`. The trace that closes this work cites it verbatim |
| Type | One input type from `docs/FEATURE_INTAKE.md`, exactly as written there |
| Lane | `tiny` · `normal` · `high-risk` |
| Summary | One line naming the actual work, plus the risk flags it touched |

Rows are never rewritten. If a classification turns out wrong or the work grows,
add a new row with the new lane.

## Merging

`.gitattributes` marks this file `merge=union`, so branches that each add rows
merge without conflicts. After a merge, check that the table is still
newest-first and that no row was duplicated.

## Log

| Date (UTC) | Type | Lane | Summary |
| --- | --- | --- | --- |
| 2026-10-25 11:49 | maintenance request | high-risk | Reduce the inherited UPM/Git/NuGet package baseline to capabilities with a live template consumer, preserving R3, boot, UI, URP and Addressables behavior. Flags: packages, precompiled assemblies, ProjectSettings, serialized GUIDs, template boundary |
| 2026-10-25 10:32 | maintenance request | high-risk | Remove zero-consumer vendored art/editor/runtime assets from the shared template, preserving only capabilities with a live template consumer. Flags: serialized assets, asmdefs, packages, template boundary, large binary deletion |
| 2026-10-25 09:54 | maintenance request | high-risk | Remove the unused mobile service SDK cohort: Firebase, EDM4U, generated repositories, native Android/iOS/tvOS integrations, IAP/cloud service packages, and stale service build templates. Flags: serialized assets, packages, build profiles, template boundary, auth / SDKs |
| 2026-10-25 09:36 | maintenance request | high-risk | Correction to 2026-10-25 09:35: the stale build-profile removals also touch serialized assets. Flags: serialized assets, build profiles, template boundary, auth / SDKs |
| 2026-10-25 09:35 | maintenance request | high-risk | Establish a neutral template identity and platform baseline; remove stale Android build profiles and Jigsaw app configs, clear inherited scripting defines, and preserve SDK roots for a separate prune. Flags: build profiles, template boundary, auth / SDKs |
| 2026-10-25 05:14 | change request | high-risk | Execute the approved package-exit plan incrementally: establish a green compile baseline, map and retire Jigsaw/package consumers, replace only neutral bootstrap/UI capabilities, then remove both submodules and prove a fresh clone. Flags: packages, asmdefs, bootstrap, serialized GUIDs, Addressables |
| 2026-10-25 03:59 | harness improvement | high-risk | Tune the template harness to follow cricket-city-unity @ 8da403c97 (owner, 2026-10-25): cricket names and depth for router, agents, skills, rules; UI inline, no UI agent; port service/gateway stack, epic-planning, bundle audit tools, harness decisions; drop Unity MCP (package `com.coplaydev.unity-mcp` removed, Unity CLI instead). Flags: packages, harness topology |
| 2026-10-24 18:40 | harness improvement | normal | Harness init (core + unity-client, framework 833b45a) and bootstrap of the shared Unity template: router, knowledge from the real code, placeholders, gate roots, `.gitignore` merge; no product code. Flags: none |
