# 04 — Port Map From cricket-city-unity

Reference: repository `cricket-city-unity` @ `8da403c97` (read-only)
Owner decisions: 2026-10-25, the template owner (decision `0007`): follow cricket
in names and content; no UI agent; no Unity MCP (Unity CLI instead); also port the
service/gateway stack, `epic-planning`, the bundle audit tools, and the harness
decisions.

> **Current-state amendment (2026-10-25):** decision
> `0008-no-default-internal-packages.md` and package-exit C5 supersede every
> package/submodule/Jigsaw assumption below. This file is a historical import
> ledger, not live architecture. API skills are now provider-neutral and dormant
> until a derived project mounts a pinned Git UPM capability. Do not copy source
> or package modules from Cricket.

Classes: **A** copy as-is · **B** copy then strip Cricket facts · **C** do not port.

| Component | Source path | Class | Strip / replace | Template path | Status |
| --- | --- | --- | --- | --- | --- |
| Router | `.claude/skills/unity-task-router/SKILL.md` | B | domains, `+2` surfaces, notes rewritten for the template; procedure, roles, scoring, Fable gate kept | `.claude/skills/unity-task-router/SKILL.md` | ported |
| `CLAUDE.md`, `AGENTS.md` | repo root | B | product facts from the template's code; harness sections kept | `CLAUDE.md`, `AGENTS.md` | ported |
| Game-logic implementer | `.claude/agents/unity-game-logic-developer.md` | B | Cricket surfaces, MessagePipe buses, WebGL bridge, Unity MCP tools | `.claude/agents/unity-game-logic-developer.md` | ported |
| Reviewer | `.claude/agents/unity-code-reviewer.md` | B | same, plus Cricket decision references | `.claude/agents/unity-code-reviewer.md` | ported |
| Documenter | `.claude/agents/unity-documenter.md` | B | decision numbers | `.claude/agents/unity-documenter.md` | ported |
| Feature service slicing | `.claude/skills/feature-domain-service/` | B | rewritten for `AppServices` and `com.paul.common.networking` | `.claude/skills/feature-domain-service/` | ported |
| Gateway wiring | `.claude/skills/gateway-wiring/` | B | networking-v2 and YGG gateways → `com.paul.common.networking`, `LobbyLifeTimeScope` | `.claude/skills/gateway-wiring/` | ported |
| API codegen | `.claude/skills/api-codegen/`, `.claude/api-codegen-config.md` | B | templates rewritten on `com.paul.common.networking` base types; game values `TODO(project)`; the gateway maps to the model with `Project`, as cricket (owner, 2026-10-25) | same paths | ported; not compiled (networking package not mounted) |
| Bundle audit | `.claude/skills/bundle-optimization/`, `tools/addressables/*.py` | B | Cricket groups, YGG packs, CDN hosts → a "Project configuration" block per script; `check.py --static` added | same paths | ported; `--static` runs on the template (exit 0); report tools proven on another project's build reports |
| Number sheets | `.claude/skills/tmp-sprite-number-sheet/` | B | Cricket sheet names; the template has no sprite-number view yet, so the skill states the contract one must meet | same path | ported |
| Editor scripting | `.claude/skills/unity-editor-scripting/` | B | Map Tool, Unity MCP probes → Unity CLI | same path | ported |
| Epic planning | `.claude/skills/epic-planning/` | B | Jira project, Outline, Sheets, Slack, people → `TODO(project)` | same path | ported |
| `encode-invariant`, `onboard-repository`, `audit-onboarding-proposal` | `.claude/skills/` | A | already the same skills from the framework core | same paths | kept |
| Rules | `.claude/rules/{csharp-code-style,csharp-type-choice,ui-code,asset-structure}.md` | B | Cricket paths; where cricket and the template's recorded conventions differ, the template's win | same paths | ported |
| Harness decisions | `docs/decisions/0001-agent-spawn…`, `0032`, `0033`, `0081`, `0084`, `0086` | B | renumbered `0001`–`0006`; Cricket history dropped | `docs/decisions/0001`–`0006` | ported |
| UITK agents retired | `docs/decisions/0100-uitk-agents-are-retired.md` | B | folded into `0007` (no UI agent) | `docs/decisions/0007-harness-follows-cricket-city.md` | ported |
| Plugin scope | `.claude/settings.json` `enabledPlugins` | B | only `superpowers: false` (decision `0003`); cricket's `codex`, `unity`, `atlassian` plugin toggles not copied | `.claude/settings.json` | ported |
| Hooks | `.claude/hooks/intake-gate.js`, `trace-gate.js` | C | the template's framework hooks are newer (gate roots from `.claude/harness.config.json`) | — | kept framework's |
| Knowledge base | `.claude/knowledge/01`–`11` | C | Cricket's own code; the template has its own `01`–`03` | — | — |
| Framework reference docs | `.claude/project-references/` | C | describes `ygg.game.shared`, not `com.paul.*` | — | — |
| Game wiring skills | `interior-game-wiring`, `world-tour-game-wiring`, `new-game-kickoff` | C | Cricket game content | — | — |
| BE contract sync | `bruno-sync`, `docs/api-docs/` | C | Cricket backend | — | — |
| Unity MCP | `.mcp.json` (`com.gamelovers.mcp-unity`) | C | owner: Unity CLI instead | — | — |
| Asset graph MCP | `unity-asset-graph` tools | C | not configured for the template | — | — |
| Worktree pool | `docs/WORKTREE_POOL.md` | C (on request) | optional personal workflow; generic, not chosen | — | — |
| Product decisions | messaging tiers, auth, scene routing, World Tour ids, … | C | Cricket product rulings | — | — |

## Never Port

- Cricket product code, names, ids, addresses, group names, swagger groups,
  endpoints, Jira keys, Outline collections, Slack channels, people. The owner's
  plan says the template "learns capabilities from Cricket City … but copies no
  YGG/Cricket/HyperEel code, names or addresses".

## Identity To Replace

- Product name, engine version, assemblies, paths → this repository's
  (`CLAUDE.md`, `.claude/knowledge/01`–`03`).
- Game-only values (backend URL, swagger, auth, Jira project, CDN) →
  `TODO(project)`, filled by the game created from the template.
