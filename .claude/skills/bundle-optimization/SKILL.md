---
name: bundle-optimization
description: "Audit and cut this project's Addressables download sizes — boot (before the lobby), each feature's first entry, and total build — and verify the result. Use when the user asks to audit/re-audit a build or build report, why a bundle downloads at boot or at feature entry, what pulls a bundle in, whether a group can be excluded from the build, to check the Addressables setup on disk, to check a new build after an Addressables change, or to analyze a HAR for bundle downloads, duplicates, errors or image caching."
---

<!-- Ported from cricket-city-unity .claude/skills/bundle-optimization/SKILL.md @ 8da403c97; stripped of Cricket specifics. -->

# Bundle Optimization

A measure → trace → fix → rebuild → diff loop. Every step has a tool; you decide
what to change. The template baseline was established on 2026-10-25: 2 groups,
3 bundles, 0.02 MB total/modelled boot, with two consecutive reports
byte-identical by group. Reports remain local under `Library/`; regenerate them
before judging a later change.

The placement rules this loop enforces live in `.claude/rules/asset-structure.md`
→ *References follow download order*; `unity-code-reviewer` checks them on
review. Engine background: `.claude/knowledge/unity-addressables-webgl.md`.
Settings, groups and profiles: `.claude/knowledge/03-build-and-profiles.md`.
Every excluded group and why: TODO(project) — create
`docs/ADDRESSABLE_GROUP_EXCLUSIONS.md` with the first exclusion and add an entry
for each one after it.

## The model

- **Boot** = the closure of `game.local` and `game.init`, configured in
  `EXPECTED_BOOT`. The loading scene itself ships through Build Settings, not an
  Addressables bundle. Confirm the model against a runtime capture once the
  project has a chosen platform/host.
- **A feature's first entry** = its closure minus boot (a sub-feature `X.Y` also
  minus its parent `X`), plus `ENTRY_BY_ADDRESS`. Feature groups match
  `FEATURE_GROUP` in `buildlayout.py`; the convention is
  `game.partial.<feature>`. The baseline has no feature group.
- **What ships in the player is decided by the resolved load path**, not by a group
  or variable named "Local" (plan rule A7). In the template's only profile,
  `Default`, every group loads from `Local.LoadPath` =
  `{UnityEngine.AddressableAssets.Addressables.RuntimePath}/[BuildTarget]`, so every
  built bundle ships inside the player; remote catalog is off. A profile whose
  load path is a CDN turns the same groups into downloads. Only scenes listed in
  Build Settings (`00_LoadingScene`) ship outside Addressables.
- **Everything else is "on demand"** — loaded by address or label later, or never.
- The graph is **bundle-level**. A pack-together group ships whole the moment one
  of its assets is referenced — a 1 KB material can pull a 12 MB pack. A
  pack-separately group ships only the bundles reached. Both baseline groups are
  pack-together.
- **Hard references** (`{fileID, guid, type: 2}`) create bundle dependencies. A soft
  `AssetReference` (`m_AssetGUID`) does not, and loads only when code asks.
- Assets that are not addressable entries travel inside the bundle of whatever
  references them. Moving one moves its bytes into the new referencer's bundle.
- **Current layout:** every explicit game-owned entry is under
  `Assets/game.assetbundles/{00_local,01_init}`. SmartAddresser is the membership
  owner and one GUID has one group. `02_partial` appears with the first real
  feature, not as an empty placeholder.

## Tools (`tools/addressables/`, stdlib Python, no Unity needed)

| Tool | Answers |
|---|---|
| `check.py --static` | without a build: every registered group's Include in Build, bundle mode, resolved load path and whether it ships in the player; setup drift; A4 one GUID one group; A0 entries outside `Assets/game.assetbundles/` |
| `check.py [report]` | every rule violation, whatever its size — E1 boot → partial hard ref, E2 sibling feature-leaf ref, E3 reference into another numbered/feature leaf, E4 unexpected group at boot, E5 dependent on an unbuilt group, E6 `00_local` reaching a group that does not ship in the player, E7 a parked `Assets/_Unused/` asset ships, E8 shared folder → leaf, W1 implicit duplicates. Exit 1 on any error |
| `audit.py [report]` | total, boot, each feature's entry, every group by when it downloads |
| `trace.py <target> [source]` | the asset → asset edges behind "X pulls Y" (regexes on group names) |
| `diff.py [old new]` | what a build changed: totals, per-group bytes, identical count, dependents on unbuilt groups |
| `har.py <file.har>` | runtime truth: per-phase downloads, errors, duplicates, excluded-group requests, image 200/304 |

Run them with `python -I tools/addressables/<tool>.py`. Every project-specific value
sits in the "Project configuration" block at the top of `buildlayout.py` (boot,
features, profile) and `check.py` (expected boot groups, tier and leaf paths).
Change those, not the algorithms.

There is no C# checker twin yet. TODO(project): plan P5 asks for an Editor window
with the same rules and a batch exit code; keep the two rule sets in step once it
exists. Template setup/build menus are `Game Template/Addressables/*` and live
under `Assets/game.bootstrap/Editor/Addressables/`.

Reports default to the newest in `Library/com.unity.addressables/BuildReports/`
(`diff.py`: the newest two). A report is written by every Addressables build.
On 2026-10-25 the parsers were checked against Addressables `2.9.0` reports, the
template's version (`Packages/manifest.json`).

## The loop

1. **Check + audit** — `check.py --static`, then on a build `check.py` and
   `audit.py`. Every `check.py` error is a rule violation to fix regardless of size
   (E1 is a hard rule). Then pick the biggest avoidable cost from `audit.py`: a
   group at boot that only one feature needs, a pack pulled by one screen, a group
   nothing uses. Adding a group to `EXPECTED_BOOT` in `check.py` is a decision, not
   a fix.
2. **Trace** — `trace.py '<target>' ['<source>']`. The edge's *source asset* is where
   the fix goes. Confirm it in the YAML: grep the target's GUID in the source file.
3. **Check before touching anything** — the preflight list below.
4. **Fix** — one of the four patterns in `fixes.md`, smallest first.
5. **Build** — re-apply the layout first if anything moved, then invoke
   `TemplateAddressablesSetup.BuildContent` from the menu or Unity
   `-executeMethod`. Then **`diff.py`** — read it in the order its docstring gives.
6. **Exclude** once `trace.py` shows no source the client can reach and
   `diff.py`/`audit.py` show no dependents: *Include in Build* off, rebuild,
   `diff.py`, registry entry.
7. **Runtime proof** — on a browser-hosted build, a HAR through `har.py` (0 errors,
   0 duplicates, 0 excluded requests, phase sizes ≈ `audit.py`); on a native build,
   the device's network or Addressables profiler capture. Plus eyes on whatever
   moved. The template has no WebGL host; the main platform is an open question
   (`.claude/knowledge/01-project-brief.md`).

## Preflight before moving or repointing an asset

- [ ] **Every user** of the asset: a GUID grep across all of `Assets/`.
      If a derived project installed a package that can serialize project asset
      references, inspect its mounted package path too. A folder-scoped search
      produces false "unused" and "missing group" findings.
- [ ] **Everything it references** — textures and shaders it uses must move too,
      or they keep the old bundle attached. Stop at assets already in a shared group.
- [ ] **Where it goes**: sharing goes *up*. Used by one feature → that feature's
      folder (`02_partial/<feature>/`). By several features → `02_partial/00_shared/`.
      By boot content → `01_init/`. Never into a folder whose bundle downloads
      earlier than needed, and never into another feature's bundle. Before plan P2
      lands, a move goes to its target tier as part of a vertical slice (plan P2,
      P7), not into another legacy folder.
- [ ] **A copy may already exist** — look for the same file name in the using
      feature's folders and compare md5 *and* import settings (`.meta` minus the
      guid line).
- [ ] **Name collision** in the target folder.
- [ ] **Layout rule coverage** of the target folder — SmartAddresser rules live in
      `Assets/AddressableAssetsData/AssetLayoutRules/`; check the rule, or an
      existing sibling's group membership. Never hand-edit group YAML: the layout
      owns membership.
- [ ] **Nobody loads it by address or label** — grep the address and labels in C#
      and data before excluding anything. Addresses in this project are often
      composed strings (`$"{DataTableBase}DT_..."` in `GameMasterDataWrapper.cs`,
      `pack.NNN.*` labels), so grep the pieces too.
- [ ] The **GUID survives**: move the file *with* its `.meta` (`git mv`, or `mv` both).

## Re-applying the layout after a move

Use `Game Template/Addressables/Apply Template Layout` or execute
`TemplateAddressablesSetup.ApplyTemplateLayout`. It is the one project-owned
entry; a second application must make no semantic change.

## Verify after a build

- `diff.py`: groups byte-identical except the ones you meant to change. A group
  growing by more than what you moved = something got copied in implicitly.
- `dependents on unbuilt groups: none` after any exclusion.
- Boot/entry deltas match the size of what you cut.
- HAR or device capture (`har.py`), then visually check every scene that used a
  moved or repointed asset. A capture proves bytes, not rendering.

## Pitfalls

- **The Editor builds its in-memory setup.** Apply the template layout and save
  before building; confirm the committed settings list only `game.local` and
  `game.init` unless the change intentionally adds a feature.
- **A shared folder pointing into one feature drags that whole feature** into
  everyone who uses the shared folder. In the reference project, shared HUD
  prefabs referencing one game's textures made every other board download that
  game (+45.8 MB). `check.py` E8.
- **Editing a schema/group `.asset` while the Editor is open** — the Editor builds
  from memory. After editing YAML on disk, have the owner refresh and confirm the
  checkbox in the Groups window before building. Prefer toggling *Include in
  Build* in the Editor.
- **Renaming a group** through Addressables renames and can move its schema
  assets; check where they landed (`Assets/AddressableAssetsData/AssetGroups/Schemas/`)
  and that the GUIDs are unchanged.
- **TMP dynamic font atlases** can re-bake and move their owning group by a few
  hundred KB. Revert unintended font-asset churn; do not chase that delta.
- **`DownloadDependenciesAsync` on WebGL** can race the real load and fetch the
  same content twice. The baseline bootstrap does not pre-download; measure any
  future feature prefetch with a HAR before keeping it.
- **A HAR with DevTools "Disable cache" on** shows every image and bundle as
  uncached. Check the checkbox before trusting cache results.
- **CI and local builds hash differently.** Match a HAR to a build by behaviour
  (which bundles, which sizes), not by bundle hash.
- **Scanning a subset of `Assets/`** for group entries produces false missing or
  unused findings. Scan the whole project-owned asset tree.
- **Soft refs are plain `AssetReference`**; the type goes on `LoadAssetAsync<T>`.
  Use `AssetReferenceSprite` only for sprite sub-object semantics. Changing a
  field from hard to soft needs its YAML migrated to the `m_AssetGUID` block.

## Proof owed per change

| Change | Proof |
|---|---|
| Move / repoint | `diff.py` clean + visual check of every scene that used it |
| Hard → soft ref | compile + PlayMode (or device) of the loading path |
| Exclude group | `diff.py` (0 dependents) + HAR with 0 requests for it + registry entry |
| Settings (CRC, compression, load path) | build report + a dev profile if a timing claim is made |

Record each in `docs/TEST_MATRIX.md` under the `addressables-build`,
`editor-evidence` and `platform` proof types.
