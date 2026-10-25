# Ported from cricket-city-unity .claude/skills/tmp-sprite-number-sheet/build_number_sprite_asset.py
# @ 8da403c97; stripped of Cricket specifics.
"""Write the TMP_SpriteAsset for a sliced number sprite sheet.

Reads the slice the artist already made in the Sprite Editor (from the .png.meta)
and emits the .asset beside it: material, Face Info, Glyph Table and Sprite
Character Table. Step 3 of .claude/skills/tmp-sprite-number-sheet/SKILL.md.

Usage:
    python -I build_number_sprite_asset.py <path-to-sheet.png> [--force]
"""

import argparse
import hashlib
import os
import re
import sys

# TextMeshPro's own GUIDs. In the template they match
# Assets/TextMesh Pro/Shaders/TMP_Sprite.shader.meta and the m_Script of
# Assets/TextMesh Pro/Resources/Sprite Assets/EmojiOne.asset.
TMP_SPRITE_SHADER_GUID = "cf81c85f95fe47e1a27f6ae460cf182c"
TMP_SPRITE_ASSET_SCRIPT_GUID = "84a92b25f83d49b9bc132d206b370281"

# The digits are pulled together by this fraction of the glyph height so the
# number does not read as gappy. Measured off the reference project's sheets
# (50 x 71 glyphs); eyeball the first render at another cell size.
TIGHTEN_RATIO = 0.05


def num(value):
    """Format a float the way Unity serialises one — no trailing zeros."""
    text = f"{value:.5f}".rstrip("0").rstrip(".")
    return text if text not in ("", "-0") else "0"


def parse_meta(meta_path):
    """Pull the texture guid and every sliced sprite out of a .png.meta."""
    with open(meta_path, encoding="utf-8") as handle:
        text = handle.read()

    guid_match = re.search(r"^guid:\s*([0-9a-f]{32})", text, re.MULTILINE)
    if not guid_match:
        sys.exit(f"no guid in {meta_path}")

    sprites = []
    for block in re.finditer(
        r"- serializedVersion: 2\s*\n\s*name:\s*(\S+)\s*\n"
        r"\s*rect:\s*\n\s*serializedVersion: 2\s*\n"
        r"\s*x:\s*(-?\d+)\s*\n\s*y:\s*(-?\d+)\s*\n"
        r"\s*width:\s*(\d+)\s*\n\s*height:\s*(\d+)",
        text,
    ):
        name, x, y, width, height = block.groups()
        id_match = re.search(
            rf"name:\s*{re.escape(name)}\s*\n(?:.*\n)*?\s*internalID:\s*(-?\d+)", text
        )
        if not id_match:
            sys.exit(f"no internalID for sprite {name}")
        sprites.append(
            {
                "name": name,
                "x": int(x),
                "y": int(y),
                "w": int(width),
                "h": int(height),
                "file_id": int(id_match.group(1)),
            }
        )

    return guid_match.group(1), sprites


def digit_of(sprite_name):
    """The digit a sprite shows, read from its name — never from its position."""
    match = re.search(r"_(\d)$", sprite_name)
    if not match:
        sys.exit(
            f"sprite '{sprite_name}' does not end in _<digit>; "
            "rename the slices to the digit they actually show"
        )
    return int(match.group(1))


def build(png_path, force=False):
    meta_path = png_path + ".meta"
    if not os.path.exists(meta_path):
        sys.exit(f"missing {meta_path}")

    name = os.path.splitext(os.path.basename(png_path))[0]
    asset_path = os.path.join(os.path.dirname(png_path), name + ".asset")
    if os.path.exists(asset_path) and not force:
        sys.exit(f"{asset_path} already exists; pass --force to overwrite")

    texture_guid, sprites = parse_meta(meta_path)
    if not sprites:
        sys.exit("sheet is not sliced — slice it as Grid By Cell Size first")

    by_digit = {}
    for sprite in sprites:
        digit = digit_of(sprite["name"])
        if digit in by_digit:
            sys.exit(f"two sprites claim digit {digit}")
        by_digit[digit] = sprite

    missing = [d for d in range(10) if d not in by_digit]
    if missing:
        sys.exit(f"no sprite for digit(s) {missing}")

    ordered = [by_digit[d] for d in range(10)]
    glyph_height = ordered[0]["h"]
    if any(s["h"] != glyph_height for s in ordered):
        sys.exit("glyphs differ in height — re-slice with a uniform cell")

    tighten = TIGHTEN_RATIO * glyph_height
    # Stable across runs: Python's hash() is salted per process, which would give
    # the material a new fileID on every regeneration.
    digest = hashlib.sha1(name.encode("utf-8")).hexdigest()
    material_file_id = -(int(digest[:15], 16) + 10**17)

    characters = []
    glyphs = []
    for index, sprite in enumerate(ordered):
        characters.append(
            "  - m_ElementType: 2\n"
            "    m_Unicode: 65534\n"
            f"    m_GlyphIndex: {index}\n"
            "    m_Scale: 1\n"
            f"    m_Name: {sprite['name']}"
        )
        glyphs.append(
            f"  - m_Index: {index}\n"
            "    m_Metrics:\n"
            f"      m_Width: {sprite['w']}\n"
            f"      m_Height: {sprite['h']}\n"
            f"      m_HorizontalBearingX: {num(-tighten / 2)}\n"
            f"      m_HorizontalBearingY: {num(sprite['h'] / 2)}\n"
            f"      m_HorizontalAdvance: {num(sprite['w'] - tighten)}\n"
            "    m_GlyphRect:\n"
            f"      m_X: {sprite['x']}\n"
            f"      m_Y: {sprite['y']}\n"
            f"      m_Width: {sprite['w']}\n"
            f"      m_Height: {sprite['h']}\n"
            "    m_Scale: 1\n"
            "    m_AtlasIndex: 0\n"
            "    m_ClassDefinitionType: 0\n"
            f"    sprite: {{fileID: {sprite['file_id']}, guid: {texture_guid}, type: 3}}"
        )

    asset = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &{material_file_id}
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {name} Material
  m_Shader: {{fileID: 4800000, guid: {TMP_SPRITE_SHADER_GUID}, type: 3}}
  m_Parent: {{fileID: 0}}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords: []
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {{}}
  disabledShaderPasses: []
  m_LockedProperties:
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _MainTex:
        m_Texture: {{fileID: 2800000, guid: {texture_guid}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    m_Ints: []
    m_Floats:
    - _ColorMask: 15
    - _CullMode: 0
    - _Stencil: 0
    - _StencilComp: 8
    - _StencilOp: 0
    - _StencilReadMask: 255
    - _StencilWriteMask: 255
    - _UseUIAlphaClip: 0
    m_Colors:
    - _ClipRect: {{r: -32767, g: -32767, b: 32767, a: 32767}}
    - _Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_BuildTextureStacks: []
  m_AllowLocking: 1
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {TMP_SPRITE_ASSET_SCRIPT_GUID}, type: 3}}
  m_Name: {name}
  m_EditorClassIdentifier: Unity.TextMeshPro::TMPro.TMP_SpriteAsset
  m_Version: 1.1.0
  m_FaceInfo:
    m_FaceIndex: 0
    m_FamilyName:
    m_StyleName:
    m_PointSize: {glyph_height}
    m_Scale: 1
    m_UnitsPerEM: 0
    m_LineHeight: 0
    m_AscentLine: 25
    m_CapLine: 0
    m_MeanLine: 0
    m_Baseline: 0
    m_DescentLine: -25
    m_SuperscriptOffset: 0
    m_SuperscriptSize: 0
    m_SubscriptOffset: 0
    m_SubscriptSize: 0
    m_UnderlineOffset: 0
    m_UnderlineThickness: 0
    m_StrikethroughOffset: 0
    m_StrikethroughThickness: 0
    m_TabWidth: 0
  m_Material: {{fileID: {material_file_id}}}
  spriteSheet: {{fileID: 2800000, guid: {texture_guid}, type: 3}}
  m_SpriteCharacterTable:
{chr(10).join(characters)}
  m_GlyphTable:
{chr(10).join(glyphs)}
  spriteInfoList: []
  fallbackSpriteAssets: []
"""

    with open(asset_path, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(asset)

    print(f"wrote {asset_path}")
    print(f"  glyph {ordered[0]['w']}x{glyph_height}, point size {glyph_height}")
    print(f"  bearingX {num(-tighten / 2)}  bearingY {num(glyph_height / 2)}  "
          f"advance {num(ordered[0]['w'] - tighten)}")
    print("  sheet order top to bottom: "
          + ",".join(str(digit_of(s["name"])) for s in sprites))


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("png", help="path to the sliced sprite sheet PNG")
    parser.add_argument("--force", action="store_true", help="overwrite an existing .asset")
    args = parser.parse_args()
    build(args.png, args.force)
