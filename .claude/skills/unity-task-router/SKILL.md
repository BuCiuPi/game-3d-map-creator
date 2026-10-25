---
name: unity-task-router
description: 'Use at the START of any Unity development task in this repo before doing the work — bootstrap, DT/DA core, feature service, gateway/API activation, uGUI shell, Addressables, Editor tooling, Git UPM, player settings, template boundaries, review, or general Unity C#. Classifies the domain and risk lane, then selects the cheapest correct inline skill or specialist role.'
---

# Unity Task Router

<!-- Ported from cricket-city-unity .claude/skills/unity-task-router/SKILL.md @ 8da403c97; domains rewritten for the template (decision 0007). -->

A thin classifier + dispatcher. It runs in the live conversation. It does NOT
spawn sub-subagents and does NOT fan out. It picks one route and gets out of the
way, biased toward staying inline.

This is unity-template: the team's shared Unity 6 starting project
(`6000.3.10f1`, URP, 2D) that new Unity games are created from
(`.claude/knowledge/01-project-brief.md`). A change here reaches every project
created from it afterwards, so "what belongs in the template" is itself a
domain. The UI is **uGUI** with TextMesh Pro; there is no UI agent (decision
`0007`). The live surface is the package-free neutral bootstrap/UI shell under
`Assets/game.bootstrap/`, `Assets/game.ui/`, and `Assets/game.core/`. Business
features and optional capabilities are added by derived projects around their
first real consumer; they are not preloaded into this template.

## Procedure

1. **Check it's a Unity task.** If the request is not Unity work (e.g. a Python
   script, a docs-only edit outside the harness), **say nothing about routing
   and proceed normally.** Do not announce a no-op.

2. **Classify, score, and announce the whole role plan in your first reply.**
   - **Domain** — pick one (see the routing table).
   - **Lane** — `tiny` / `normal` / `high-risk` per `docs/FEATURE_INTAKE.md`,
     including its risk checklist and hard gates (the `game.core` public API,
     Git UPM dependency changes, GUID-changing asset moves). Uncertainty about a
     hard gate promotes the lane.
   - **Score** — see "Scoring" below.

   A task passes through several roles, so announce the **sequence**, not just the
   next step. The user should see the whole arc up front and be able to stop it
   early:

   `Routing: <domain> / <lane> · Score <n>/10 → <role> (<route>) → <role> → …`

   > Routing: feature-service / normal · Score 5/10 → implement (inline) → review → document (story)

3. **Route per the table.** Default to inline when on the inline/spawn boundary.

   | Domain | When | Route | Target |
   |---|---|---|---|
   | `template-boundary` | What stays in the template and what becomes a feature module: adding or removing a business module, stripping Jigsaw-era code, the thin-core plan phases | **Inline only, always ≥ normal** (record a decision) | `artifacts/unity-template-addressables-plan.html`; `docs/architecture/template-core-data.md`; `.claude/knowledge/01-project-brief.md` |
   | `bootstrap-di` | `TemplateBootstrap`, shell ownership, a new composition/DI scope after its package is mounted, or boot order | **Inline only, always ≥ normal**; high-risk for a new scope or changed boot order | `.claude/knowledge/02-runtime-and-wiring.md` |
   | `core-data` | `Assets/game.core/` — `DataAsset<TData>`, `DataTable<TRow>`, their validation and EditMode tests | Inline; **high-risk** when the public API changes (every project copies it) | `docs/architecture/template-core-data.md`; `.claude/rules/csharp-type-choice.md` |
   | `game-logic` | A project feature's gameplay, state, services, R3 events, presenters, or master data | Inline by default; spawn if large/multi-file | `unity-game-logic-developer` if spawned |
   | `feature-integration` | Screens, navigation, popups and composition owned by a real feature module | Inline by default; spawn if large | `unity-game-logic-developer` if spawned |
   | `feature-service` | A server-backed feature slice: service, gateway, API endpoint, and its first networking capability | Inline, always ≥ normal | first read `docs/architecture/networking-on-demand.md`; then `feature-domain-service` → `gateway-wiring` → `api-codegen` after package configuration |
   | `ui-screen-build` | Build or change a uGUI screen, popup (Data/Presenter/View), HUD, prefab wiring, from Figma or not | Inline | `.claude/rules/ui-code.md` + the nearest existing Presenter/View pair; `tmp-sprite-number-sheet` for sprite digits |
   | `addressables` | `Assets/AddressableAssetsData/` groups, profiles, SmartAddresser rules, `Game Template/Addressables/*`, moves/renames, download sizes | Inline for one new entry; **high-risk** for moves, renames, regrouping, profile or load-path changes | `.claude/rules/asset-structure.md`; `bundle-optimization` (`tools/addressables/`) |
   | `editor-tooling` | Editor automation: `Editor/` tooling, menu items, scripted prefab/scene edits, GUID-preserving renames, migrations, headless EditMode runs | Inline | load `unity-editor-scripting`; gameplay code it touches → `game-logic` |
   | `packages` | `Packages/manifest.json`, lockfile, optional Git UPM capabilities, third-party plugins under `Assets/` | **Inline, always high-risk** (every derived project inherits defaults; confirm first) | `.claude/knowledge/01-project-brief.md`; `docs/architecture/networking-on-demand.md` |
   | `build-profiles` | Define symbols, `ProjectSettings/`, player identity, build scenes, CI | **Inline only, always ≥ normal** | `.claude/knowledge/03-build-and-profiles.md` |
   | `code-review` | Review existing Unity code / a branch / a PR | **Auto-spawn** (read-only, ungated) | `unity-code-reviewer` |
   | `general-scripting` | General Unity C# (MonoBehaviour, R3, Addressables, or an already-mounted async/DI package) not above, or small edits | Inline | load user-level `unity-developer` skill; declaring a new data type → `.claude/rules/csharp-type-choice.md` |
   | `invariant-guard` | Enforce an accepted rule with a mechanical check (EditMode test, validator, or script), or stop a documented violation from recurring | Inline | load `encode-invariant`; it stops when no accepted authority states the rule |
   | `feature-planning` | Break one feature's spec into a Jira epic and tickets | Inline, **user-requested only** (writes to Jira behind an owner gate) | load `epic-planning` |
   | `harness-maintenance` | Harness status, onboarding, tuning, shipping to the team, distilling learnings, proposing improvements back to the framework | Inline | `harness-lifecycle` (entry point), `learning-distillation`, `harness-bootstrap` (refresh), `onboard-repository` + `audit-onboarding-proposal` (user-invoked), `harvest` |
   | `not-unity` | Handled in step 1 | — | — |

   **`code-review` the domain vs `review` the role** — same agent, two uses. The
   *domain* is when reviewing existing code IS the task you were asked to do. The
   *role* is the validation pass over work this task just produced. Both
   auto-spawn `unity-code-reviewer` ungated.

4. **Run the role sequence.** This step is what makes the roles happen — they are
   not optional extras hanging off the tables below. Skip a role only when its
   trigger did not fire.

   | Order | Role | Fires when | Gated? |
   |---|---|---|---|
   | 1 | `design` | score ≥ 8, or the `+1 novel design` adjuster fired | **yes** |
   | 2 | `implement` | a product delta exists | only if spawned |
   | 3 | `review` | lane ≥ normal | no — auto-spawn |
   | 4 | `document` | lane ≥ normal | no — auto-spawn |

   The tiny lane runs `implement` alone. `docs/FEATURE_INTAKE.md` says "patch
   directly; no story", so `review` and `document` do not fire there.

5. **Spawn gate.** Before any spawn other than the auto-spawned `review` and
   `document`, state your read and the brief, then WAIT for the user's ok. Example:
   > Routing: game-logic / normal, large. I'll spawn
   > `unity-game-logic-developer` with: [files, brief]. Sound right?

   A spawned implementer cannot ask the user anything mid-run: it returns its plan
   and stops. Relay the plan, get the user's ok, then resume it with
   `SendMessage` to implement.

   `review` and `document` auto-spawn ungated: both produce artifacts
   `docs/FEATURE_INTAKE.md` already mandates for the lane, so asking permission to
   produce them is friction, not safety.

6. **Inline route.** Announce the lane, load the named skill, and continue the
   task in this conversation — no subagent. This is the path for iterative work.

7. **Prove, record, report.** Hand back to the task loop in `docs/HARNESS.md`:
   prove against `docs/TEST_MATRIX.md` and add the story's proof row, write any
   `docs/decisions/` record the lane owes, log it in `docs/TRACE_LOG.md` (normal and
   high-risk always; tiny only on friction or a harness-rule change —
   `docs/TRACE_SPEC.md`), then report what changed and what was not attempted.

## Roles (which model)

Dispatch has two axes. The **role** decides the model; the **score** below decides
the lane, whether to spawn, and what proof is owed. The score does NOT pick a model
(decision `0002-role-based-agent-dispatch`).

| Role | Trigger | Route | Model |
|---|---|---|---|
| `orchestrate` | always | this session | Opus |
| `design` | score ≥ 8, or the `+1 novel design` adjuster fired | spawn built-in `Plan`, iterate via `SendMessage` | **Fable** |
| `implement` | product delta | inline, or spawn `unity-game-logic-developer` per score | Opus |
| `review` | lane ≥ normal, after implement | auto-spawn `unity-code-reviewer` (ungated) | Opus |
| `document` | lane ≥ normal, after review | auto-spawn `unity-documenter` (ungated) | Opus |

`role` is not the same word as the context *phase* in `docs/CONTEXT_RULES.md`.
They map 1:1, and each role inherits that phase's read table:

| Role | Context phase | Read table |
|---|---|---|
| `orchestrate` | Intake | intake |
| `design` | Planning | planning |
| `implement` | Implementation | implementation |
| `review` | Validation | validation |
| `document` | Documentation | documentation |

## Scoring (lane, spawn, proof)

After classifying domain + lane, compute a 0–10 score. Thresholds are tunable —
calibrate by trial and error from `docs/TRACE_LOG.md` friction.

**Score = lane baseline + adjusters (clamp to 1–10):**
- Baseline: `tiny → 2`, `normal → 5`, `high-risk → 6`
- `+2` touches a high-impact surface: `TemplateBootstrap` or any new
  `LifetimeScope`; the
  `game.core` public API; any `.asmdef`; Addressables groups, profiles or load
  paths; `Packages/manifest.json`, lockfile, or an optional Git UPM package; define symbols
  or `ProjectSettings/`; prefab or `.meta` GUID structure
- `+1` the result needs an Editor, Play Mode, or device check the agent cannot
  run itself (how a screen looks, boot on a device, a bundle download)
- `+1` cross-module or multi-file blast radius
- `+1` novel design, no established pattern · `−1` well-trodden pattern
  (a skill scaffolds it, or a near-identical popup/service exists to copy)
- `−2` purely mechanical (rename, text/config value, comment) —
  **also suppresses the multi-file `+1`** (a mechanical change is
  cheap regardless of fan-out)

**Map:**

| Score | Route | `design` first? | `document` after? |
|---|---|---|---|
| 1–3 | Inline always | no | no (tiny lane = no story) |
| 4–7 | Domain table decides; spawn when ≥ 6 | only if `+1 novel design` fired | story |
| 8–10 | Spawn | **yes** | story + decision record |

Domain rules in the routing table are hard. An "inline only" domain stays
**inline** at any score; the score only breaks ties where the table already says
"inline by default; spawn if large".

**Why baseline 6 for high-risk:** plain high-risk (a phased feature with no scary
surface) lands at 6 and goes straight to implement; high-risk that *also* trips a
`+2` surface climbs to 8 and buys a Fable design consult first. The adjuster
decides whether design is worth paying for, instead of flattening every high-risk
task to 10.

**Effort.** The ladder is `low / medium / high / xhigh / max`. **`medium` is the
house default** (decision `0002`); raise it for a task that measurably needs
more. Effort is NOT scored.

**Confirm-gate (before executing):**
- Session effort is already at or above what the task needs → proceed silently.
  **Never prompt for a downgrade.**
- The task genuinely needs `max` → state why, ask first.
- Bulk mechanical fan-out wanting a cheap model at `low` effort → that is a
  `Workflow` job, not an `Agent` spawn. `Agent` cannot set effort; `Workflow`'s
  `agent()` takes per-call `model` **and** `effort`.
- The live session model is **Fable** and the task is code implementation → Fable
  gate below, ask first.

**Apply by path:**

Spawn mechanism (verified against the `Agent` tool + agent frontmatter): the tool
accepts a per-call `model` but has **no** `effort` parameter. Effort is always
inherited from the live session and cannot be set at spawn time. See decision
`0001-agent-spawn-model-effort-routing`.

- **Spawn — `implement` / `review` / `document`:** every `.claude/agents/*.md`
  pins `model: opus`, so the frontmatter default is already correct. Do **not**
  pass `model` on these calls.
- **Spawn — `design`:** the design consultant is the built-in `Plan` agent, which
  has no project frontmatter. You MUST pass `model` explicitly:
  `Agent({ subagent_type: "Plan", model: "fable", prompt: <brief> })`.
  Continue the design conversation with `SendMessage` to that agent — a fresh
  `Agent` call starts over and loses the thread.
- **Spawn gate:** `design` and `implement` are gated — state your read and the
  brief, then wait for the user's ok. `review` and `document` auto-spawn ungated.
- **Spawn (fallback — "my-way"):** when the user has told you to default to your
  own judgment (e.g. "dùng cách của bạn", "default to your way"), skip the
  confirm-gate questions and proceed at your best judgment without asking. Still
  print the routing line so the choice stays visible. This mode holds for the rest
  of the session unless the user revokes it.
- **Inline:** continue in this conversation. Do NOT switch the live model, and do
  NOT print an effort advisory — the session is already at the house default.

**Fable gate:**

Fable is the **design consultant only** — strategy, architecture, tradeoffs,
execplans. Never an implementer, reviewer, or documenter.

- **Spawn Fable only as the `design` role.** Every other spawned role runs Opus.
- **Relay Fable's output verbatim.** The orchestrator is a **courier, not a
  judge**: it does not accept, reject, or summarise away the design. Subagent
  reports are not shown to the user by default, so paste the design and let the
  user decide. Opus must never overrule Fable on design — otherwise the cheaper
  model gates the more capable one and you silently lose what you paid for.
- **If the live session is running on Fable** and the task crosses from
  planning into implementation (any product delta: C#, prefab, scene,
  data asset), STOP before the first code edit and ask, e.g.:
  > This session is on Fable, which this project reserves for strategy/planning.
  > Implement inline on Fable anyway, or route per the score
  > (spawn `<agent>` / `/model <model>`, <effort>)?
  Wait for the answer before touching code. Harness/planning artifacts
  (stories, `docs/` files, decisions, skill/agent docs) need no gate.

## Notes

- **The UI is uGUI and has no implementer agent.** Generic shell views live in
  `Assets/game.ui/`; Addressable UI prefabs live under
  `Assets/game.assetbundles/`. Screen work routes inline against
  `.claude/rules/ui-code.md` (decision `0007`).
- **No Unity MCP.** Editor proof goes through the Unity CLI / batchmode commands
  in `.claude/knowledge/03-build-and-profiles.md` (decision `0007`).
- **No business code ships in the template.** A new game capability starts in a
  derived feature module. Flag speculative business code added to the shared
  template or any attempt to restore the retired Jigsaw/package surface.
- **A project created from this template keeps this router's name** and the
  roles. In that project, run `harness-bootstrap` in refresh mode to rewrite the
  domain table, the `+2` surfaces, and the knowledge for the game.
- When unsure between inline and spawn, choose inline.
- This skill complements normal skill auto-triggering; it does not replace it.
