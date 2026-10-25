# 0007 The Template Harness Follows Cricket City

Date: 2026-10-25

## Status

Accepted (owner, 2026-10-25)

## Context

The harness was installed from the framework (core + `unity-client` pack,
framework `833b45a`) and bootstrapped on 2026-10-24 with the framework's generic
names: router `unity-template-task-router`, agents `architect`, `code-reviewer`,
`documenter`, `unity-gameplay-developer`, `unity-reviewer`, `unity-ui-developer`,
`unity-figma-designer`, and `unity-`-prefixed pack skills. The owner rejected
that shape: the template's harness must follow cricket-city-unity, the team's
reference harness, in names and in content. The template is what new Unity games
start from, so every game inherits whatever the template carries.

## Decision

Owner answers, 2026-10-25:

1. **Names and content follow cricket-city-unity** (repository
   `cricket-city-unity` @ `8da403c97`, read-only), stripped of Cricket product
   facts:
   - router `unity-task-router`, with cricket's role/model table (`0002`);
   - agents `unity-game-logic-developer`, `unity-code-reviewer`,
     `unity-documenter`; design runs on the built-in `Plan` agent with Fable;
   - skills `api-codegen`, `bundle-optimization`, `tmp-sprite-number-sheet`,
     `unity-editor-scripting`, `encode-invariant`, `onboard-repository`,
     `audit-onboarding-proposal`;
   - rules `csharp-code-style.md`, `csharp-type-choice.md`, `ui-code.md`,
     `asset-structure.md`;
   - `CLAUDE.md` in cricket's shape.

   The framework files they replace are retired from this repository.
2. **No UI agent.** UI is uGUI and is built inline against
   `.claude/rules/ui-code.md`, as in cricket (where UITK agents were retired).
   `unity-ui-developer` and `unity-figma-designer` are retired.
3. **No Unity MCP.** The `com.coplaydev.unity-mcp` package is removed from
   `Packages/manifest.json`; the team will drive Unity through the Unity CLI.
   No agent declares `mcp__mcp-unity__*` or `mcp__unity-asset-graph__*` tools.
   Proof runs through the batchmode commands in
   `.claude/knowledge/03-build-and-profiles.md`.
4. **Also ported:** the server-feature stack (`feature-domain-service`,
   `gateway-wiring`, `api-codegen`) rewritten for `com.paul.common.networking`;
   `epic-planning` with its team values left as `TODO(project)`; the
   Addressables audit tools (`tools/addressables/`) behind `bundle-optimization`;
   and the harness decisions `0001`–`0006`.
5. **Follow-up answers (owner, 2026-10-25):**
   - the API gateway maps wire → model itself (`Project(result, data =>
     data.ToModel())`) and returns `Result<TModel>`, as cricket's owned gateways
     do; the service never sees `ApiData` (`api-codegen`, `gateway-wiring`,
     `feature-domain-service`);
   - a shared asset folder is named `00_shared`, never `shared`
     (`.claude/rules/asset-structure.md`, `tools/addressables/check.py`);
   - the owner picks the Unity CLI; until then proof runs through Unity batch
     mode or the Editor;
   - the template does not mount `com.paul.common.networking`: it has no
     backend. A game made from it mounts the package with its first endpoint,
     as the API skills' precondition says.
6. **Not ported:** Cricket's game-specific skills (`interior-game-wiring`,
   `world-tour-game-wiring`, `new-game-kickoff`, `bruno-sync`), its knowledge
   files and `.claude/project-references/`, and its product decisions. The
   port map is `.claude/knowledge/04-port-map-from-cricket.md`.

## Alternatives Considered

1. Keep the framework's names and only pour cricket's depth into them: rejected
   by the owner.
2. Keep the UI agents: rejected; cricket builds uGUI inline.
3. Switch to cricket's `com.gamelovers.mcp-unity`, or keep CoplayDev's server:
   rejected; Unity CLI instead.

## Consequences

Positive:

- A game created from the template starts with the same router, roles, skills,
  and rules as cricket, so the team works the same way in every Unity repo.
- Router and agent names are no longer template-specific; a game keeps them
  and only refreshes the knowledge and router domains.

Tradeoffs:

- **Do not re-run the framework installer (`install_harness.ps1`) against this
  repository as it stands.** It re-adds every managed file that is missing, so
  it would bring back the retired agents, skills, and rules. Re-running needs a
  retire list the installer honours (proposed to the framework).
- The template now diverges from the framework pack; improvements found here go
  back through the `harvest` skill by hand.

- `harness-bootstrap` (a framework skill) still names the framework's roles and a
  `<project>-task-router` skeleton. On a refresh — here or in a game created from
  the template — keep `unity-task-router` and the roles above; let refresh
  rewrite only the domain table, the `+2` surfaces, and the knowledge.

## Follow-Up

- Framework: let a repository retire managed files so a reinstall respects it.
- Write the Unity CLI commands into `.claude/knowledge/03-build-and-profiles.md`
  once the owner has chosen the tool.
