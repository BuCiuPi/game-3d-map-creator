# Developer Onboarding

unity-template is the shared Unity 6 project used to start new game repositories.

## 1. Prerequisites

| Tool | Version | Why |
| --- | --- | --- |
| Git | recent | source control; no submodule support is required |
| Unity | `6000.3.10f1` | Editor and batch proof |
| Node.js | current LTS | harness hooks |
| Claude Code | current | optional primary agent client |

## 2. Get The Code

Clone normally:

```text
git clone https://github.com/hoangphuc25071995/unity-template.git
```

The repository has no private package or submodule prerequisite.

## 3. Open And Run

Open the repository in Unity Hub with `6000.3.10f1` and let Package Manager
resolve `Packages/manifest.json`.
Open `Assets/game.contents/gameScenes/00_LoadingScene.unity` and press Play.
The expected result is the neutral global UI shell and
`TemplateBootState.Ready`; there is no lobby or authentication flow.

## 4. Test

Before a pull request, run compile, all EditMode tests, and the PlayMode boot
when bootstrap/UI changes.
Addressables changes also run static check and content build/report audit.
Commands are in `.claude/knowledge/03-build-and-profiles.md`; behavior-to-proof
mapping is in `docs/TEST_MATRIX.md`.

## 5. Work Loop

1. Classify with `docs/FEATURE_INTAKE.md`.
2. Add the intake record required by the lane.
3. Start Unity work through `.claude/skills/unity-task-router/SKILL.md`.
4. Read the matching rule before editing.
5. Run and record the proof; never claim an unrun check.
6. Update story, decision, matrix and trace artifacts owed by the lane.

## 6. Where Things Live

| Path | Holds |
| --- | --- |
| `Assets/game.core/` | DT/DA primitives |
| `Assets/game.bootstrap/` | neutral boot, tests, Addressables setup/build entry |
| `Assets/game.ui/` | minimal shell UI |
| `Assets/game.assetbundles/` | explicit Addressable source assets |
| `Assets/game.contents/gameScenes/` | loading scene only |
| `Assets/AddressableAssetsData/` | settings, groups, schemas, layout metadata |
| `Packages/` | manifest and lockfile; no submodules |
| `.claude/`, `docs/` | operating harness and durable record |
| `artifacts/` | owner-facing local HTML plans |

## 7. Optional Networking

Do not add networking during project initialization.
When the first API feature exists, follow
`docs/architecture/networking-on-demand.md`: add the selected Git UPM package at
an accepted commit, declare required Git packages directly, regenerate the
lockfile, inspect its current API, then generate and test the feature.

## 8. Troubleshooting

| Problem | Action |
| --- | --- |
| Feature-intake hook blocks a write | classify the work and add the required intake row |
| Package resolve fails after optional Git UPM change | verify `?path=/package#<sha>` order, repository access, and every direct Git declaration |
| Shell does not reach ready | run `game.bootstrap.tests.playmode` and inspect the loading scene's serialized references |
| Addressables group drift | run `TemplateAddressablesSetup.ApplyTemplateLayout`, `check.py --static`, then a content build |

## 9. Further Reading

| Document | Covers |
| --- | --- |
| `CLAUDE.md` | project summary and harness entry |
| `docs/ARCHITECTURE.md` | current module boundaries |
| `docs/HARNESS.md` | operating loop |
| `.claude/knowledge/00-Master-Index.md` | detailed repository knowledge |
| `docs/architecture/template-core-data.md` | DT/DA contract |
| `docs/architecture/networking-on-demand.md` | Git UPM API capability recipe |
