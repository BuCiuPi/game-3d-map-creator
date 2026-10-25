# US-XXX Story Title

## Status

planned

## Lane

tiny | normal | high-risk

## Behavior

Describe the behavior this story must make true.

## Relevant Docs

- `.claude/knowledge/...`
- `docs/decisions/...`

## Acceptance Criteria

- Criterion 1.
- Criterion 2.

## Surfaces Touched

Check any that apply; each pushes the lane toward high-risk (see docs/FEATURE_INTAKE.md):

- [ ] `TemplateBootstrap`, boot order, or a new `LifetimeScope`
- [ ] The `game.core` public API
- [ ] Addressables groups, profiles, load paths, or asset moves that change GUIDs
- [ ] `Packages/manifest.json`, `packages-lock.json`, or a Git UPM dependency
- [ ] Define symbols or `ProjectSettings/`
- [ ] What belongs in the template (adding or removing a module)
- [ ] A contract another component or team depends on

## Design Notes

- Bootstrap and scopes:
- Services and gateways:
- Data (DT/DA, master data):
- UI (Data/Presenter/View, prefabs):
- Addressables placement:

## Validation

| Proof | Expected |
| --- | --- |
| `compile` | every C# change |
| `editmode` | `Assets/game.core/` data primitives and any plain C# logic |
| `boot` | bootstrap, lifetime scope, Addressables init, scene routing |
| `editor-evidence` | UI screens, popups, prefab wiring, Addressables menu passes |
| `addressables-build` | group, profile, layout-rule or asset-move changes |
| `platform` | a device build of a game made from the template |

## Harness Delta

Any knowledge/spec doc or decision updated because of this story.

## Evidence

Add commands, screenshots, or links after validation exists.
