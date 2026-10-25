---
paths:
  - "Assets/**"
---

<!-- Ported from cricket-city-unity .claude/rules/asset-structure.md @ 8da403c97; stripped of Cricket specifics. -->

# Asset Structure Rules

Source of truth: the owner's plan `artifacts/unity-template-addressables-plan.html`
(Vietnamese; "Approved for phased implementation", 2026-10-24) and
`docs/architecture/template-core-data.md` (DT/DA placement). The rule's shape
follows the reference harness (`docs/decisions/0007-harness-follows-cricket-city.md`).
Read this file before creating, moving, or importing any asset or C# file.

Status: the reference-direction rules and the `.meta`/GUID rules are **Gate**
(engine behaviour and the plan's checker rules A1–A6). The tier table is the
plan's **target**: `Assets/game.assetbundles/` does not exist yet and is built
slice by slice in plan phase P2. Until a feature's slice migrates, a new asset
goes next to that feature's existing assets, and no new top-level content root
is added.
Checked by: `tools/addressables/check.py`, build report tools, and bootstrap
boundary tests. Setup runs through `Game Template/Addressables/*` in
`Assets/game.bootstrap/Editor/Addressables/`.

All explicit game-owned Addressable entries live under
`Assets/game.assetbundles/` in the groups listed in
`.claude/knowledge/03-build-and-profiles.md`.

## Where files go

- **Code** → a module folder `Assets/game.<module>/{Runtime,Editor,Tests}` with
  an asmdef (`Assets/game.core/` is the model). Never into Assembly-CSharp;
  never deliverable content in code modules. Code, Editor tooling and
  non-addressable authoring files never go under `Assets/game.assetbundles/`.
- **Deliverable assets** → `Assets/game.assetbundles/`. The folder decides the
  availability; the layout rule decides the group, the bundle, and the address:

| Put it in | When | Loads |
| --- | --- | --- |
| `00_local/ui/shell/` | the boot loading overlay and the fatal/recovery message overlay: concrete, neutral, game-owned prefabs wired into the bootstrap scene | with the player |
| `00_local/data/bootstrap/{assets,tables}/` | `DA_`/`DT_` assets only boot needs | with the player |
| `01_init/ui/00_shared/` | the toast and the generic message/confirm dialog (0–2 actions), used only after global DI is ready | boot, after Addressables initialization |
| `01_init/00_shared/data/{assets,tables}/` | shared `DA_`/`DT_` needed after initialization and before any feature entry | boot, after initialization |
| `02_partial/<feature>/` | everything one feature owns: its screens, popups, feature loading, art | first entry into the feature |
| `02_partial/<feature>/data/{assets,tables}/` | that feature's `DA_`/`DT_` | with the feature; its scope loads and releases it |

Current rules create `game.local` for `00_local` and `game.init` for `01_init`.
A project that adds a feature uses `game.partial.<feature>` and extends the
template-owned setup in the same changeset.

- **Data needed before Addressables initialization is serialized directly into
  the bootstrap scene or object.** It never loads itself through Addressables.
- **The boot recovery path works without the catalog or global DI.** The
  loading and recovery overlays do not depend on Addressables instantiation, a
  global message bus, or a package-owned prefab as their base (plan §05).
- **No core business UI.** Shop, quest, chapter, VIP, liveops and map loading
  are feature UI in `02_partial/<feature>/`; the template adds no popup manager
  placeholder.
- **`Assets/AddressableAssetsData/` holds only settings, groups, schemas,
  content state and layout metadata** — never source assets.
- **Optional Git UPM packages are code contracts, not content roots.** Never add
  project content to a package or recreate the retired repository submodules.
- **Sharing goes up, never sideways**: leaf → its `00_shared/` →
  `01_init/00_shared/` → `00_local/`. Feature A never references feature B. When
  two siblings need the same asset, move the one copy up to their common shared
  folder — never keep a copy in each, and never point one at the other's. A
  shared folder is always named `00_shared` (owner, 2026-10-25), so it sorts
  first beside its leaves; a folder named `shared` is a leaf.
- **Keep `01_init` lean.** It downloads during boot; anything reached only inside
  a feature belongs in `02_partial/`.
- **Content borrowed from another project is reimplemented in this tree and in
  local namespaces** — never copied as that project's module, path or address
  (plan P1 and "Core adoption").
- **Unused assets are parked, not deleted**: move them (with `.meta`) to
  `Assets/_Unused/` at the same path they had below `Assets/game.assetbundles/`,
  and record the batch in the change's intake row (`docs/INTAKE_LOG.md`).
  Nothing may reference that folder, and it never ships (plan A6).

## References follow download order

A hard reference (`{fileID, guid, type: 2}`) makes the referencing bundle depend
on the target's bundle, so the target downloads **no later than** the referencer
— whole bundle, not just the asset. Measure with the `bundle-optimization` skill,
not from paths.

0. **`00_local` reaches only what ships in the player.** It is shown before any
   download, so everything it references lives in `00_local/` (or a group on the
   local load path). An asset both `00_local` and `01_init` use moves **up** into
   `00_local`.
1. **Init holds NO hard reference to partial — use `AssetReference`.** Nothing
   downloaded at boot (`00_local`, `01_init`, boot data, boot prefabs) may
   hard-reference anything under `02_partial/` (plan A1). Partial may
   hard-reference init.
2. **A serialized field that holds an asset defaults to `AssetReference`.** Reach
   for a hard reference only when the target downloads with, or before, the
   field's owner (same bundle, or a row above in the Loads column). Use plain
   `AssetReference` — never `AssetReferenceT<T>`; the type goes on
   `LoadAssetAsync<T>`. Use `AssetReferenceSprite` only when Unity's sprite
   sub-object semantics require it. Changing an existing field migrates its
   serialized values to the `m_AssetGUID` block, or they silently go empty.
3. **Place an asset by who uses it, not by where it came from.** Its home is the
   latest-downloading folder that still arrives no later than every user. A
   widget's art filed under the first feature that used it pulls that feature's
   bundle into everything else that shows the widget.
4. **A small asset never lives inside a big leaf it is shared out of.** Bundles
   pack together, so one reference ships the whole bundle: a 1 KB material can
   cost megabytes. Move the small asset out; never accept the edge.
5. **Availability comes from the resolved build and load paths, not from the
   folder or group name.** A `02_partial` group on a Local schema needs the
   owner's confirmation (plan A7 and the "Partial content on Local schema" risk).
6. **An identical file in another leaf is not a substitute.** Repointing to a
   byte-identical copy works only when that copy downloads with the referencer.
   A copy in a sibling feature's folder is a sideways reference and chains the
   two features (plan A2) — move a single copy up instead.
7. **Moving an asset moves its dependencies too.** A material keeps its old
   bundle attached through its textures until they move with it (or already sit
   in a folder that downloads earlier). Move with the `.meta`; the GUID must
   survive.
8. **Excluding a group** (Include in Build off) takes zero bundle dependents
   (plan A5), no address or label loads, and the owner's approval in the plan's
   exception registry (A0; not created yet).

## Hard rules

1. **A folder name is a runtime contract.** Addresses, labels and bundle names
   derive from paths. Never rename or renumber an existing folder without a
   migration; new siblings take the next number after the highest in use.
2. Reference, don't copy. Byte-duplicating an existing asset is a review
   defect — and a renamed copy is still a copy. Two users → one copy in their
   common shared folder. Editor tools reference runtime assets, themes override
   only what changes, and game modes use prefab variants — never
   per-consumer copies.
3. **Membership comes from the SmartAddresser layout rules** — never from
   hand-editing a group asset or group YAML (plan "Generation"). One GUID, one
   group (plan A4). Setup is idempotent: running it twice produces no semantic
   diff (P2 exit gate). The only template rule asset is
   `Assets/AddressableAssetsData/AssetLayoutRules/template_LayoutRuleData.asset`;
   apply it through `TemplateAddressablesSetup`.
   After adding, moving or renaming files under a covered folder, apply the
   rules: a file in a covered folder that was never applied ships nowhere.
4. **Every shared folder is its own bundle.** An asset in a folder no rule
   covers is silently copied into every bundle that references it — add a rule
   or move it, never leave it uncovered (plan W1 measures it).
5. **Inside a feature leaf, `DA_`/`DT_` assets go in `data/assets/` and
   `data/tables/`** (`docs/architecture/template-core-data.md`). No other leaf
   subfolder layout is stated yet; match the feature's existing layout and add
   the convention here when the owner states one. The `DA_`/`DT_` prefix is for
   search only — the type decides the data shape and the tier decides delivery.
6. **Editor tooling applies a project-owned Preset when one exists; it never
   invents importer policy in C#.** The thin template ships no canonical art or
   audio import preset, so a derived project's first asset pipeline must add and
   document its own.
7. **Editor tooling applies a Preset; it never writes importer properties in
   C#.** Load with `AssetDatabase.LoadAssetAtPath<Preset>(path)`, then
   `preset.ApplyTo(importer)`. A C# copy of "what a 2D sprite is" is a second
   source of truth that silently drifts from the one the art team actually
   applies by hand in the Inspector — and only theirs is real. A missing preset
   is a hard stop, never a fall-back to hardcoded settings.

   Decide whether to reimport by diffing the importer's own serialized state
   around the apply, which names no property and keeps a CI re-run from churning
   `.meta` files in every PR:

   ```csharp
   var before = EditorJsonUtility.ToJson(importer);
   if (!preset.ApplyTo(importer)) return PresetOutcome.NotApplicable;
   if (string.Equals(before, EditorJsonUtility.ToJson(importer), StringComparison.Ordinal))
       return PresetOutcome.Unchanged;          // nothing reached disk — no SaveAndReimport
   importer.SaveAndReimport();
   ```

## Naming

- Namespaces: `Game.<Module>.<Layer>` = asmdef root namespace = folder
  (`Game.Core.Runtime`, `Game.Bootstrap.Runtime`, `Game.UI.Runtime`). Never mint
  names from another product.
- Assemblies: `game.<area>.<layer>[.editor|.tests.editmode]`
  (`game.core.runtime`, `game.bootstrap.runtime`,
  `game.bootstrap.addressables.editor`).
- New asset files: `P_` prefab, `T_` texture, `M_` material, `DT_` data table,
  `DA_` data asset, `AN_` animation, `BGM_` music; scenes `NN_Name.unity`
  (`00_LoadingScene`). Do not rename an existing shipping asset merely to match
  convention; its address changes.
- Leaf folders: one `<feature>` folder per feature under `02_partial/`; its
  casing and any order prefix are not stated yet — the first slice migrated in
  P7 sets it, and this line records it.
- Groups and labels come from the layout rules — never add a group or label by
  hand.

## PR checklist (assets)

- [ ] Folder matches the table above — tier, section, leaf, and shared scope
      (or, before the slice migrates, sits beside its feature's existing assets)
- [ ] No reference to a sibling leaf's assets
- [ ] Nothing downloaded at boot hard-references `02_partial/`; new asset fields
      are `AssetReference` unless the target downloads with or before the owner
- [ ] Nothing new under `Assets/AddressableAssetsData/` but settings, schemas,
      content state and rules; nothing copied into or out of a Git UPM package
- [ ] After a build, the bundle diff (`bundle-optimization` skill) shows only the
      groups you meant to change
- [ ] Named per conventions before first commit (renames change addresses)
- [ ] No byte-duplicate of an existing asset — compare md5, not just file names
- [ ] Import settings from preset
- [ ] Layout rule applied; the file appears in the expected group
- [ ] Moved with `.meta`; the GUID survived
