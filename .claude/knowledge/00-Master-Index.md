# Master Index — Unity Template Agent Knowledge

unity-template is the team's shared Unity 6 starting project, not a game.
Its live runtime is a neutral bootstrap/UI shell plus DT/DA primitives.
Load this index, then the one relevant knowledge file.

## Harness Bootstrap

| Field | Value |
| --- | --- |
| Bootstrapped | 2026-10-25 at `1fe9173`, framework `833b45a`, pack `unity-client` |
| Last product refresh | package-baseline-prune C8.2B, `9423722` |
| Router | `.claude/skills/unity-task-router/SKILL.md` |

A project created from this template inherits the harness.
Run `harness-bootstrap` in refresh mode in that project to rewrite domains and
knowledge for the actual game.

## Stack

| Stack | Root | Versions | Evidence |
| --- | --- | --- | --- |
| Unity | `./` | Unity `6000.3.10f1`, URP `17.3.0` with the 2D renderer | `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json` |
| Runtime libraries | `Packages/` | R3, Addressables `2.9.0`, Test Framework `1.6.0`, uGUI `2.0.0`, SmartAddresser; NuGetForUnity manages the four-DLL R3 closure | `Packages/manifest.json`, `Packages/packages-lock.json`, `Assets/packages.config` |
| Internal packages | none | added by a derived project through pinned Git UPM only | decision `0008`, `docs/architecture/networking-on-demand.md` |

## Files in This Knowledge Base

| File | Description |
| --- | --- |
| [01-project-brief.md](01-project-brief.md) | purpose, current boundary, heritage, open questions |
| [02-runtime-and-wiring.md](02-runtime-and-wiring.md) | assemblies, neutral boot, UI and handle ownership |
| [03-build-and-profiles.md](03-build-and-profiles.md) | opening, proof commands, Addressables configuration |
| [04-port-map-from-cricket.md](04-port-map-from-cricket.md) | historical harness research and port decisions |
| [05-map-to-environment-pipeline.md](05-map-to-environment-pipeline.md) | pointer to the portable `docs/map-to-environment/` pack, plus this repo's constraints on building it |

The unnumbered files are generic Unity references; repository facts belong in
the numbered files.

## Analyzed Directories

| Directory | Purpose |
| --- | --- |
| `Assets/game.core/` | DT/DA primitives and EditMode tests |
| `Assets/game.bootstrap/` | neutral runtime boot, tests, Addressables setup/build entry |
| `Assets/game.ui/` | loading, recovery, Toast, MessageOverlay and global shell views |
| `Assets/game.assetbundles/` | all explicit game-owned Addressable source assets |
| `Assets/game.contents/gameScenes/` | preserved loading scene only |
| `Assets/AddressableAssetsData/` | settings, two groups, schemas, layout metadata and content state |
| `Packages/` | manifest and lockfile; no repository submodules |

The template has no vendored plugin/art root. `Assets/TextMesh Pro/` and
`Assets/Packages/` are retained support surfaces for TMP and R3; they are not
template modules. The package baseline was consumer-audited in decision `0012`.

## Patterns

- Boot: one `TemplateBootstrap`; local loading/recovery exists before catalog or DI.
- UI: post-init Toast and MessageOverlay are children of `P_GlobalUiShell`.
- Data: `DataTable<TRow>` and `DataAsset<TData>`; no game master data.
- Content: SmartAddresser owns `game.local` and `game.init` membership.
- Handles: the object that loads or instantiates owns release.
- Optional capabilities: install only at first consumer through pinned Git UPM.

## Related Docs

| Location | Use |
| --- | --- |
| `docs/architecture/template-core-data.md` | DT/DA contract and delivery placement |
| `docs/architecture/networking-on-demand.md` | first API package recipe |
| `artifacts/unity-template-addressables-plan.html` | delivery roadmap and gates |
| `artifacts/unity-template-package-exit-plan.html` | completed package-exit checklist |
| `.claude/rules/*.md` | code and asset conventions |
| `docs/decisions/` | durable rulings |

## When to Use What

- Boot, assemblies, shell ownership → `02-runtime-and-wiring.md`.
- Open, test, build, groups, profiles → `03-build-and-profiles.md`.
- Template boundary → `01-project-brief.md`, then decision `0008`.
- DT/DA placement → `docs/architecture/template-core-data.md`.
- API package setup → `docs/architecture/networking-on-demand.md`.
