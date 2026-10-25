<!-- Ported from cricket-city-unity .claude/skills/bundle-optimization/fixes.md @ 8da403c97; stripped of Cricket specifics. -->

# Fix patterns

Smallest first. Each came from a real cut in the reference project; record each
cut you make here (the commit, the bytes it freed, the build it was measured on)
and, for an exclusion, the registry entry (TODO(project):
`docs/ADDRESSABLE_GROUP_EXCLUSIONS.md`, created with the first exclusion).

## 1. Repoint to an identical local copy — no asset moves

**When:** the feature that holds the reference already has its own copy of the
target (same name nearby is the usual tell), but the reference still points at the
original in another group.

**Preconditions:**
- md5 of the file matches; for a material, the body matches apart from its own
  texture GUIDs, and *those* textures match by md5 and import settings.
- The copy lives in the **same bundle as the referencer**, or a bundle it already
  depends on. A byte-identical copy in another feature's bundle is a trap: it
  chains this feature to that bundle.

**How:** replace the GUID in the referencing YAML (`.anim`, `.controller`, `.prefab`,
`.mat`). Keep the `fileID` (main object of a material is `2100000`, of a clip
`7400000`; a sprite sub-asset is `21300000`). Grep afterwards that no old GUID
remains. With the Editor open, prefer assigning the field or `Material.SetTexture`
in a scripted Editor pass (`unity-editor-scripting` §2).

**Typical cases:**
- A sub-game's animation clips pointed at another pack's materials although the
  sub-game had byte-identical copies of its own; repointing freed the whole other
  pack from that sub-game's entry (12.65 MB in the reference project).
- An animator controller pointed at another pack's clips while its own folder held
  the same clips; repointing freed two packs (14.2 MB).
- **Rejected:** an overlay sprite had a byte-identical copy — in a *sibling*
  sub-game's folder. Pointing there would have made one sibling download the
  other. Moved instead (pattern 2).

## 2. Move a misplaced asset — sharing goes up

**When:** the target has no copy on the referencing side, or several features need it.

**Preconditions:** the full preflight in `SKILL.md`. In particular, carry its own
dependencies (a material's textures) along, and pick the folder by who uses it:
one feature → that feature (`02_partial/<feature>/`); several features →
`02_partial/00_shared/`; boot content → `01_init/`. The old owner may end up
referencing the new location — fine when the old owner is itself being excluded.

**How:** move the project-owned file with its `.meta` so the GUID survives.
Never move or copy source out of an installed Git UPM package; add a deliberate
project-owned replacement with a new GUID and migrate consumers instead. Remove
project folders left empty with their `.meta`, re-apply the layout, then build.

**Typical cases:**
- VFX shader graph and materials used by two sub-games moved from one pack to the
  shared folder: both sub-games stopped pulling that pack.
- A digit sprite set used by the boot HUD *and* a live-ops feature moved to
  `01_init/` (moving it into the game folder would have made the live-ops feature
  pull the game into boot).
- Skill VFX moved from one sub-game folder to the skills' shared folder.

## 3. Hard reference → soft `AssetReference`

**When:** a table or component always loaded early (init data or a boot prefab)
references content that is only needed later or per feature.

**Preconditions:** the consumer can load asynchronously; someone owns the handle
(load, cache, release) — `AddressableStageAssetGateway` is the template's pattern
to copy (`.claude/knowledge/02-runtime-and-wiring.md`, *Stage Loading*). Accept that
the first use downloads the target's bundle — for a pack-together target that is
the whole pack.

**How:** change the field to `AssetReference` (never a new `AssetReferenceT<T>`),
migrate each serialized value from `{fileID: 11400000, guid: X, type: 2}` to the
block
```yaml
      m_AssetGUID: X
      m_SubObjectName:
      m_SubObjectType:
      m_SubObjectGUID:
      m_EditorAssetChanged: 0
```
and load with `Addressables.LoadAssetAsync<T>(reference)` + `ToUniTask`, awaited with
the owner's cancellation token. The target must be an addressable entry. An
unmigrated value silently becomes empty.

**Typical cases:** a map-theme table's map field, a skill table's filter fields, a
lobby popup's preview skeletons, a bubble table's rows — together they dropped two
packs from boot (−17 MB in the reference project). In the template,
`ChapterDataTable.completed_image` is already soft (an `AssetReferenceSprite`,
loaded on demand in `ChapterPagePresenter.cs`): that soft shape is the one to
follow, with plain `AssetReference` for new fields.

## 4. Exclude a group from the build

**When:** nothing the client can reach uses the group any more.

**Preconditions — all of them:**
- `diff.py`/`audit.py` on a fresh build: no bundle depends on it.
- `trace.py '<group>'`: no source the client can reach.
- No load by address or label (grep its addresses and labels in C# and data —
  including composed address strings).
- Soft references into it (`AssetReference` rows in data tables) are only
  reachable from content the client cannot open — say so in the registry entry.
- Reachability: show why the content is unreachable (no lobby entry, no route, no
  remote config that could enable it), not just that nothing references it today.

**How:** untick *Include in Build* in the Groups window (never de-register or
delete the group — an addressable entry belongs to exactly one group, and a
de-registered group's entries become orphans). Rebuild, `diff.py` (0 dependents,
every other group unchanged), HAR (0 requests), then the registry entry.
TODO(project): decide whether excluded groups get the `[Deprecated] ` name prefix
(`buildlayout.DEPRECATED_PREFIX`, which `har.py` strips when matching bundles).

**Typical case:** in the reference project, 25 legacy content packs were excluded
over a week, each with its own registry entry.
