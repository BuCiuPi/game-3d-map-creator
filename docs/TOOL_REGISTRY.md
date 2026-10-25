# Tool Registry

Which optional tools this project equips, what purpose each serves, and how an
agent decides whether one is actually usable right now.

This is the **extension base**: a workflow step asks "what is present for this
purpose?" and adapts to the answer. Nothing in the core harness may depend on
any entry here — a missing tool is a fact to report, never a blocker.

The registry is this file. There is no command to register or probe a tool; you
read the table and check presence yourself.

## Registry

| Tool | Kind | Capability | Presence check | Purpose |
| --- | --- | --- | --- | --- |
| unity-task-router | skill | task-routing | `.claude/skills/unity-task-router/SKILL.md` exists | classify, score and route every Unity task; names the role and model per step |
| unity-editor-scripting | skill | editor-automation | `.claude/skills/unity-editor-scripting/SKILL.md` exists | Editor menus, batch passes, GUID-safe edits, headless proof |
| bundle-optimization | skill | bundle-audit | `.claude/skills/bundle-optimization/SKILL.md` exists | boot, first-entry and total download size; what pulls a bundle in |
| tmp-sprite-number-sheet | skill | asset-authoring | `.claude/skills/tmp-sprite-number-sheet/SKILL.md` exists | build or fix a `TMP_SpriteAsset` number sheet |
| api-codegen | skill | api-scaffold | `.claude/skills/api-codegen/SKILL.md` and `.claude/api-codegen-config.md` exist | scaffold request/response/data/model classes and an owned gateway method for an endpoint |
| gateway-wiring | skill | api-wiring | `.claude/skills/gateway-wiring/SKILL.md` exists | wire a request through gateway → service → DI scope |
| feature-domain-service | skill | service-slicing | `.claude/skills/feature-domain-service/SKILL.md` exists | slice a backend feature into its own service |
| epic-planning | skill | work-planning | `.claude/skills/epic-planning/SKILL.md` exists; its Project configuration table filled | turn a feature spec into a Jira epic and Tasks; user-requested only, writes to Jira |
| harness-lifecycle | skill | harness-lifecycle | `.claude/skills/harness-lifecycle/SKILL.md` exists | harness status, onboarding, tuning |
| encode-invariant | skill | invariant-encoding | `.claude/skills/encode-invariant/SKILL.md` exists | turn an accepted rule into a mechanical check |
| Unity Editor / batch mode | binary | engine-build, unit-test | `C:/Program Files/Unity/Hub/Editor/6000.3.10f1/Editor/Unity.exe` exists | recompile, EditMode tests, PlayMode boot check, and Addressables build; commands are in `.claude/knowledge/03-build-and-profiles.md` |
| `tools/addressables/*.py` | script | bundle-audit | `tools/addressables/check.py` exists and `python -I tools/addressables/check.py --help` runs | `check.py --static` checks the Addressables setup without a build; with a build report: `check.py`, `audit.py`, `trace.py`, `diff.py`; `har.py` reads a browser HAR (driven by `bundle-optimization`) |
| `Game Template/Addressables/*` menus | editor-menu | editor-automation | `Assets/game.bootstrap/Editor/Addressables/TemplateAddressablesSetup.cs` exists | apply deterministic local/init groups and build Addressables content |

Add a row when the project equips a new tool. Keep `Capability` in kebab-case —
that string is how a workflow step finds the tool, so a step and the tools that
serve it must agree on it exactly.

## Kinds

| Kind | Reached via | `present` means |
| --- | --- | --- |
| `mcp` | an MCP server exposed to the agent runtime | the server is connected **this session** and answers a cheap read |
| `skill` | a Claude skill or `.claude/skills/` directory | the skill is listed or its `SKILL.md` resolves on disk |
| `cli` / `binary` | a command on `PATH` or a repo-relative path | it resolves and runs |
| `http` | a URL | the endpoint answers |

For `mcp` and `skill`, presence is a **runtime** fact, not a repo fact. A row in
this table records that the project intends to use the tool; only the agent
runtime can see whether the server is actually connected right now. Confirm at
call time with the cheap read in the Presence-check column rather than assuming.

## Degrade Ladder

The table states intent; the agent applies policy based on what resolved:

| Tools present for the capability | Posture | Behavior |
| --- | --- | --- |
| none registered | Inactive | Clean skip. Note `capability X: inactive` in the trace. Not drift. |
| registered, none or some present | Degraded | Run with what resolves. Say so in the proof, and do not claim the check you could not run. |
| all present | Full | Normal operation. |

A registered tool that fails its presence check is a **degraded** run worth
naming, not a silent skip. A capability nobody registered is simply inactive —
that is what keeps the harness working on a fresh clone with no MCP servers.

The rule that matters: never report a validation as passed because a tool was
*supposed* to run it. If the tool that runs a check is not connected, the check
did not happen — say that, per `docs/TEST_MATRIX.md`. Watch for results that
read as green when they are not: a test run whose filter matched nothing ("0/0
passed") is a miss, never a pass; a count that includes suites or containers is
not a count of passing tests; and a build tool that reports success before its
output is refreshed is not proof on its own.

## Capability Vocabulary

Reuse an existing string before coining a new one; coin new ones in kebab-case:

```text
task-routing · code-graph · design-source · design-authoring
contract-verification · invariant-encoding · api-test-sync · ui-audit
```

```text
engine-build · unit-test · editor-automation · bundle-audit
api-scaffold · api-wiring · service-slicing · asset-authoring · work-planning
```
