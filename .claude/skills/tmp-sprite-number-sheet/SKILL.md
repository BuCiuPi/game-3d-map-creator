---
name: tmp-sprite-number-sheet
description: "Build or update a TMP_SpriteAsset number sheet for a sprite-number view (the reference view is UITMPSpriteNumberView, which this template does not have yet) — texture import, grid slice, Face Info, Glyph Table, Sprite Character Table, material and addressable entry, with a generator that writes the asset from the sliced .png.meta. Use when adding a new number style (a T_NumberSpriteSheet_* sheet), when digits render as blank boxes or the wrong digit, when a number jitters as its value changes, or when wiring or writing a sprite-number view on a prefab."
---

<!-- Ported from cricket-city-unity .claude/skills/tmp-sprite-number-sheet/SKILL.md @ 8da403c97; stripped of Cricket specifics. -->

# TMP Number Sprite Sheet

A number rendered from art, not a font: a sprite-number view turns a value into
`<sprite name="5">` or `<sprite=5>` markup, and TMP resolves that against a
`TMP_SpriteAsset`. Getting the sheet wrong shows up as blank boxes, the wrong
digit, or digits that jitter sideways as the value changes.

The Editor inspector only exposes half of this. Face Info and the Glyph Table
each have to be set by hand, and nothing warns you when they are missing — which
is how a sheet ships looking finished and renders nothing.

## Where this lives

| | |
| --- | --- |
| Sheets | none yet. Place one by who uses it (`.claude/rules/asset-structure.md` → *Where files go*): a style used across features → `Assets/game.assetbundles/01_init/ui/00_shared/`; one feature's style → `Assets/game.assetbundles/02_partial/<feature>/` |
| Runtime view | **none in the template.** No `<sprite>`-emitting number view is part of the neutral shell. Write it in the derived feature that first needs it; §5 is the contract it must meet. |
| Reference geometry | the reference project's `T_NumberSpriteSheet_Blue`, `_BlueDark`, `_YellowBig` (table in §2) — measure your own sheet before applying the numbers |
| TMP GUIDs | `Assets/TextMesh Pro/Shaders/TMP_Sprite.shader.meta`, and the `m_Script` of `Assets/TextMesh Pro/Resources/Sprite Assets/EmojiOne.asset` |

Each sheet is one `.png` plus one `.asset` beside it. The `.asset` holds both the
`TMP_SpriteAsset` and its material, in the same file.

## 1. Texture import

Match the reference sheets:

| Setting | Value |
| --- | --- |
| Texture Type | Sprite (2D and UI) — `textureType: 8` |
| Sprite Mode | Multiple — `spriteMode: 2` |
| Pixels Per Unit | 100 |
| Mesh Type | Tight — `spriteMeshType: 1` |
| Extrude Edges | 1 |
| Generate Physics Shape | on — `spriteGenerateFallbackPhysicsShape: 1` |
| Generate Mipmap | off |
| Wrap / Filter | Clamp / Bilinear |
| Format | RGB(A) Compressed ASTC 6x6 block — the reference project's WebGL value. TODO(project): the template's main platform is open (`.claude/knowledge/01-project-brief.md`); set the format per platform once it is chosen |

A freshly imported PNG already lands on Sprite/Multiple, so the two that actually
need changing are **Mesh Type** and **Generate Physics Shape** — both default to
the wrong value.

## 2. Slice — grid, never automatic

Use **Grid By Cell Size**, not Automatic. Automatic slicing gives every digit its
own tight bounds, so `1` ends up narrower than `8` and the number **jitters
horizontally** every time a digit changes. A uniform cell is what keeps it
monospaced.

Read the cell off the sheet: it is `height / 10`, then find the content offset.

```
cell height = texture height / 10
Unity rect y for row i (top row = i 0) = texHeight - (i + 1) * cellHeight   # no spacing
```

Reference geometry:

| Sheet | Texture | Cell | Rect | Offset / spacing |
| --- | --- | --- | --- | --- |
| Blue, BlueDark | 80 × 710 | 71 | x 16, w 50, h 71 | offset x 16, no spacing |
| YellowBig | 151 × 1730 | 173 | x 19, w 111, h 151 | offset x 19 y 20, spacing y 20 |

**The digit order is a property of the artwork, not a convention.** Blue runs
`0,1,…,9` top to bottom. YellowBig runs `1,2,…,9,0` — the zero is at the bottom.
Unity's slicer names cells `_0.._9` by position regardless, so on a sheet like
YellowBig the generated names are wrong by one and must be renamed to the digit
they actually show. Open the sheet and look before trusting the names.

Name each sprite `<TextureName>_<digit>`; the texture itself takes the `T_` prefix
(`.claude/rules/asset-structure.md` → *Naming*).

## 3. The TMP_SpriteAsset

Four parts, and the Editor fills in none of them for you.

**Face Info** — `m_PointSize` is the glyph height, and `m_HorizontalBearingY` of
half that height is what centres the digits on the baseline. Leave the rest at
zero.

```yaml
m_PointSize: <glyph height>     # 71 for Blue, 151 for YellowBig
m_Scale: 1
m_AscentLine: 25
m_Baseline: 0
m_DescentLine: -25
```

**Glyph Table** — one entry per digit, `m_Index` 0-9. The rect is the slice rect
from step 2. The metrics are the part that is easy to get wrong:

```
tighten          = 0.05 * glyphHeight
HorizontalBearingX = -tighten / 2
HorizontalBearingY = glyphHeight / 2
HorizontalAdvance  = glyphWidth - tighten
```

That relationship holds identically on Blue and BlueDark (`-1.775 / 35.5 / 46.45`
for a 50 × 71 glyph). It pulls the digits slightly together so they do not read as
gappy. It has not yet been proven at a different cell size, so eyeball the first
render at the new size and adjust `tighten` if the spacing looks off.

**Sprite Character Table** — one entry per digit, and `m_GlyphIndex` must equal
the digit. `m_Unicode: 65534` is TMP's "no unicode, name lookup only" marker.

```yaml
- m_ElementType: 2
  m_Unicode: 65534
  m_GlyphIndex: 0
  m_Scale: 1
  m_Name: <TextureName>_0
```

**Material and texture** — the `.asset` carries its own material using the
`TextMeshPro/Sprite` shader, with `_MainTex` pointing at the PNG. `spriteSheet`
and `m_Material` on the sprite asset both point back at it.

Every `sprite:` fileID in the Glyph Table must equal the matching `internalID` in
the `.png.meta`. If they disagree the glyph resolves to nothing and you get a
blank box with no console error.

## 4. Addressables

Both the `.png` and the `.asset` are addressable entries, in the group of the
folder they sit in. The SmartAddresser layout rule assigns the group and labels —
never add the entry or a label by hand (`.claude/rules/asset-structure.md` →
*Naming*). TODO(project): the layout rules for `Assets/game.assetbundles/` arrive
with plan P2; until then, confirm with `python -I tools/addressables/check.py --static`
which group the folder lands in.

A sheet used by a boot HUD must sit in a tier that downloads no later than that
HUD (`01_init/`), or the HUD's bundle pulls the sheet's whole group into boot —
see the `bundle-optimization` skill.

## 5. Wire the sprite-number view

The template has no view yet (see *Where this lives*). Whatever view is written or
ported must meet this contract, which is how the reference `UITMPSpriteNumberView`
behaves:

- **Its text field must be assigned.** In the reference view it is a plain
  `[SerializeField]` with no null guard, so an unassigned field throws on the first
  `SetValue`. A new view validates it in `OnValidate` or fails loudly on first use.
- **Sprite Asset** on that `TextMeshProUGUI` must be the sheet — set it on the
  component, or via a Material Preset, not globally.
- **Sprite Render Mode** decides which lookup is used, and the two modes need
  different sheets:

| Mode | Emits | Needs |
| --- | --- | --- |
| `SpriteName` | `<sprite name="5">` | character named exactly `5` |
| `SpriteIndex` | `<sprite=5>` | glyph index 5 is digit 5 |

**Sheets named `<TextureName>_<digit>` only work in `SpriteIndex` mode.** Their
characters are named `T_NumberSpriteSheet_Blue_0`, not `0`, so a `SpriteName`
lookup finds nothing and renders blank. Either set the mode to `SpriteIndex`, or
name the characters as bare digits — pick one per sheet and stay consistent.

## Reading the font size off the design

Because `m_PointSize` is set to the glyph height, TMP's sprite scale reduces to an
identity worth remembering:

```
spriteScale   = fontSize / faceInfo.pointSize
renderedHeight = glyphHeight * spriteScale = fontSize     # when pointSize == glyphHeight
```

**The rendered digit height in UI units equals Font Size.** The template's canvases
use a reference resolution of 1080 × 1920
(`.claude/knowledge/02-runtime-and-wiring.md` → *UI*). When the design artboard has
the same size, a design pixel is a UI unit: take the height of the digit instance
in the design and type it into Font Size. A 95.81px digit is Font Size 96.
TODO(project): confirm the design artboard size; if it differs, scale by
`1080 / artboardWidth`.

Cross-check before trusting it — the design box and the sprite must have the same
aspect ratio, or the mockup was squashed and the height alone will not match.
Per-digit width on screen is `horizontalAdvance * fontSize / pointSize`.

This identity breaks the moment `m_PointSize` is anything other than the glyph
height, which is one more reason to keep them equal.

## Gotchas

- **`N0` formatting emits thousand separators.** If a derived feature chooses
  `"N0"`, its formatter emits the separator; the template has no currency helper.
  `10000` becomes `10,000` and the comma is looked up as a sprite. A 10-glyph sheet
  has no comma, so it renders as nothing. Either add a comma glyph, or feed the
  view a string you formatted yourself. Short counters (levels) are safe until they
  reach four digits.
- **`SpriteIndex` mode maps `+` to `<sprite=12>`** in the reference view. That needs
  a 13-glyph sheet. On a 10-glyph sheet a `+` renders blank. Likewise `-`, `%`, `x`
  need their own glyphs.
- **A missing glyph is silent.** No console warning, no fallback — just an empty
  box. When a digit disappears, check the Glyph Table before anything else.
- **Face Info left at zero renders zero-height glyphs.** The sheet looks correct
  in the inspector and draws nothing in the scene.

## Generator

`build_number_sprite_asset.py` beside this file writes the `.asset` from the
`.png.meta`, deriving every glyph rect, metric and fileID from the slice the
artist already made. It is the whole of step 3.

```bash
python -I .claude/skills/tmp-sprite-number-sheet/build_number_sprite_asset.py \
  "Assets/game.assetbundles/<tier>/.../T_NumberSpriteSheet_<Style>.png" [--force]
```

It refuses to run on an unsliced sheet, on missing or duplicate digits, and on
non-uniform cell heights, and it reads the digit from each sprite name rather than
from its position, so a sheet ordered `1..9,0` comes out right. The shader and
script GUIDs it writes are TextMeshPro's; they match the template's
`Assets/TextMesh Pro/` copies (*Where this lives*). Re-check them if TMP is ever
replaced by a modified copy.

After it runs: focus the Editor to import (or run the batch-mode compile command in
`.claude/knowledge/03-build-and-profiles.md` → *Proof Commands*, which imports
too), open the asset and confirm the Sprite Character Table lists 0-9 and the
Glyph Table has ten rects, then let the layout make it addressable and drop it on
the text component.

## Verify

Set the value to `1234567890` once. Every digit should appear, in order, evenly
spaced, none blank. Then step a value through `9 → 10 → 99 → 100`: the digits must
not shift sideways beyond the advance. The look is owner evidence: a screenshot
from the Editor, recorded as `editor-evidence` in `docs/TEST_MATRIX.md`.
