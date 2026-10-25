#!/usr/bin/env python3
# Ported from cricket-city-unity tools/addressables/check.py @ 8da403c97; stripped of
# Cricket specifics. --static (rules A0/A4 on project files) is new in the template.
"""Flag every placement-rule violation in a build, whatever its size.

  python -I tools/addressables/check.py [report]           # exit 1 if any ERROR
  python -I tools/addressables/check.py --static           # project files only, no build needed

Build-report rules (`.claude/rules/asset-structure.md` → References follow download
order; plan rule ids from artifacts/unity-template-addressables-plan.html in brackets):
  E1  boot content (00_local/, 01_init/) hard-references 02_partial/            [A1]
  E2  a feature leaf hard-references a sibling leaf                            [A2]
  E3  anything outside a numbered content pack hard-references into one
  E4  a group downloads at boot that is not on the expected-boot list
  E5  a built bundle depends on a group that is not built                      [A5]
  E6  00_local content hard-references an asset that does not ship in the player [A7]
  E7  something under Assets/_Unused/ ships (an entry, or copied in by a reference) [A6]
  E8  a shared folder hard-references a leaf (sharing must go up)              [A3]
  W1  an asset is duplicated into several bundles (implicit copies) — size wasted [W1]

Project-file rules (--static; Assets/AddressableAssetsData/ only, no build report):
  A4  one asset GUID, one group: an entry registered twice, or an explicit entry
      inside another group's folder entry
  A0  an entry lives outside Assets/game.assetbundles/ — WARN until plan P2 has
      moved the content (set A0_IS_ERROR below then)
  and a table of every group: Include in Build, bundle mode, resolved load path,
  whether it ships inside the player (the input to E6); plus WARN setup drift —
  a registered group with no asset, a group with no BundledAssetGroupSchema (not
  built), a group asset the settings do not list.

Editor twin: none yet. TODO(project): plan P5 asks for an Editor window with the
same rules and a batch exit code — keep the two rule sets in step when it exists.

audit.py says where the megabytes are; check.py says which references are wrong.
Run both after every Addressables build.
"""
import argparse
import os
import re
import sys
from collections import defaultdict

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))   # works under python -I
from buildlayout import (PROFILE, Layout, in_player_groups, project_groups, resolve,  # noqa: E402
                         unregistered_group_assets)

# == Project configuration =====================================================
# Groups allowed to download before the first interactive screen. A new entry here
# is a decision: it makes every user's boot bigger. Template boot path on
# 2026-10-25: the neutral template boot owns exactly these two groups.
EXPECTED_BOOT = {"game.local", "game.init"}
# Groups whose bundles ship inside the player. None = derive from the schemas'
# load paths resolved for the build profile (buildlayout.in_player_groups).
IN_PLAYER = None

ROOT_DIR = r"(?:^|/)game\.assetbundles/"
BOOT_SRC = re.compile(ROOT_DIR + r"(00_local|01_init)/")
LOCAL_SRC = re.compile(ROOT_DIR + r"00_local/")
PARTIAL = re.compile(ROOT_DIR + r"02_partial/")
# Numbered content packs, loaded by address through IStageAssetGateway; nothing
# outside a pack may hard-reference into one. TODO(project): after plan P2 these
# packs live under 02_partial and E2 covers them; drop or repoint this then.
PACK = re.compile(r"(?:^|/)game\.contents\.gameplay/Runtime/Contents/\d{3}/")
UNUSED = "Assets/_Unused/"
# (sibling scope, regex whose last group names the leaf). Leaves in one scope may
# not reference each other. TODO(project): add nested levels (an album's stages,
# a feature's sub-modes) as extra rows with their own scope.
LEAF = [
    (0, re.compile(ROOT_DIR + r"02_partial/(?!00_shared/)([^/]+)/")),
]
# Folders shared between leaves: they may not reference down into a leaf.
# Shared folders are named `00_shared` (owner, 2026-10-25; plan A3).
ALL_SHARED = [
    re.compile(ROOT_DIR + r"02_partial/00_shared/"),
]
DELIVERABLE_ROOT = "Assets/game.assetbundles/"
A0_IS_ERROR = False
# == End of project configuration ==============================================


def leaf(path):
    """The sibling-scope key of a path, or None for shared folders."""
    for scope, rx in LEAF:
        m = rx.search(path)
        if m:
            return (scope, m.group(m.lastindex))
    return None


def shared_references_down(source):
    return any(rx.search(source) for rx in ALL_SHARED)


def short(path):
    return re.sub(r"^.*?/(game\.assetbundles|game\.contents\.gameplay/Runtime/Contents)/", "", path)


def check_report(path, profile):
    lay = Layout(resolve(path))
    in_player = IN_PLAYER if IN_PLAYER is not None else in_player_groups(profile)
    findings = defaultdict(list)

    for src, dst in lay.asset_edges():
        s, d = src["path"], dst["path"]
        if BOOT_SRC.search(s) and PARTIAL.search(d):
            findings["E1 boot content hard-references 02_partial"].append((src, dst))
        ls, ld = leaf(s), leaf(d)
        if ls and ld and ls[0] == ld[0] and ls[1] != ld[1]:
            findings["E2 sibling reference"].append((src, dst))
        if ld and shared_references_down(s):
            findings["E8 shared folder hard-references a leaf"].append((src, dst))
        if PACK.search(d) and not PACK.search(s):
            findings["E3 reference into a numbered content pack"].append((src, dst))
        if LOCAL_SRC.search(s) and dst["group"] not in in_player:
            findings["E6 00_local reaches a downloaded group"].append((src, dst))

    boot = lay.boot()
    unexpected = defaultdict(int)
    for r in boot:
        g = lay.bundles[r]["group"]
        if g not in EXPECTED_BOOT and not g.startswith("(no group)"):
            unexpected[g] += lay.bundles[r]["size"]

    built = set(lay.group_size)
    dangling = sorted({lay.bundles[r]["group"] for r, deps in lay.bundle_deps.items()
                       if any(lay.bundles[d]["group"] not in built for d in deps)})

    # Implicit duplicates: one source asset copied into several bundles.
    file_bundle = {rid: e["data"].get("Bundle", {}).get("rid") for rid, e in lay._rids.items()
                   if e["type"]["class"] == "BuildLayout/File"}
    other = {e["data"].get("AssetGuid"): e["data"] for e in lay._rids.values()
             if e["type"]["class"] == "BuildLayout/DataFromOtherAsset"}
    dupes = []
    for dup in lay.duplicated:
        info = other.get(dup["AssetGuid"])
        if not info or not info.get("AssetPath", "").startswith(("Assets/", "Packages/")):
            continue
        groups = {lay.bundles[file_bundle[f["rid"]]]["group"]
                  for o in dup["DuplicatedObjects"] for f in o["IncludedInBundleFiles"]
                  if file_bundle.get(f["rid"]) in lay.bundles}
        size = info.get("SerializedSize", 0) + info.get("StreamedSize", 0)
        if len(groups) > 1:
            dupes.append((size * (len(groups) - 1), info["AssetPath"], sorted(groups)))

    # Parked assets that still ship: an explicit entry, or an implicit copy pulled in by a reference.
    parked = [(a["path"], a["group"], "entry") for a in lay.assets.values() if a["path"].startswith(UNUSED)]
    for rid, e in lay._rids.items():
        if e["type"]["class"] != "BuildLayout/File":
            continue
        bundle = lay.bundles.get(file_bundle.get(rid))
        for ref in e["data"].get("OtherAssets", []):
            p = lay._rids.get(ref.get("rid"), {}).get("data", {}).get("AssetPath", "")
            if p.startswith(UNUSED):
                parked.append((p, bundle["group"] if bundle else "?", "implicit"))
    parked = sorted(set(parked))

    # -- report ----------------------------------------------------------------
    errors = 0
    print(f"{lay.name}")
    for rule in sorted(findings):
        edges = findings[rule]
        errors += len(edges)
        print(f"\nERROR {rule}: {len(edges)}")
        for src, dst in sorted(edges, key=lambda e: (e[1]["group"], e[1]["path"]))[:40]:
            print(f"   {short(src['path'])}\n      -> [{dst['group']}] {short(dst['path'])} ({dst['size'] / 1e3:.1f} KB)")
        if len(edges) > 40:
            print(f"   ... {len(edges) - 40} more (trace.py for the full list)")
    if unexpected:
        errors += len(unexpected)
        print(f"\nERROR E4 unexpected group at boot: {len(unexpected)}")
        for g, size in sorted(unexpected.items(), key=lambda kv: -kv[1]):
            print(f"   {size / 1e6:7.2f} MB  {g}   (trace.py '{re.escape(g)}$' to find who pulls it)")
    if dangling:
        errors += len(dangling)
        print(f"\nERROR E5 depends on an unbuilt group: {dangling}")
    if parked:
        errors += len(parked)
        print(f"\nERROR E7 parked asset ships (Assets/_Unused/): {len(parked)}")
        for p, group, how in parked[:40]:
            print(f"   [{group}] {how}  {p}   (trace.py '{re.escape(group)}$' to find who references it)")
    if dupes:
        waste = sum(d[0] for d in dupes)
        print(f"\nWARN  W1 implicit duplicates: {len(dupes)} assets, ~{waste / 1e6:.2f} MB extra")
        for w, p, groups in sorted(dupes, reverse=True)[:15]:
            print(f"   {w / 1e3:8.1f} KB  {short(p)}  in {groups}")

    print(f"\n{errors} error(s), {len(dupes)} warning(s)")
    return errors


def check_static(profile):
    groups = project_groups(profile)
    print(f"Assets/AddressableAssetsData — profile {profile!r}, {len(groups)} registered groups\n")
    print(f"  {'group':34s} {'build':9s} {'mode':10s} {'entries':>7s}  {'player':6s} load path")
    for g in groups:
        build = "no schema" if g["include_in_build"] is None else ("on" if g["include_in_build"] else "OFF")
        player = "-" if g["in_player"] is None else ("yes" if g["in_player"] else "no")
        print(f"  {g['name']:34s} {build:9s} {g['bundle_mode'] or '-':10s} {len(g['entries']):7d}  "
              f"{player:6s} {g['load_path'] or '-'}")

    errors = warnings = 0
    # Setup drift: what the Groups window shows may not be what is on disk.
    missing = [g["name"] for g in groups if g["path"] is None]
    schemaless = [g["name"] for g in groups if g["path"] and g["include_in_build"] is None]
    orphans = unregistered_group_assets(groups)
    if missing or schemaless or orphans:
        warnings += len(missing) + len(schemaless) + len(orphans)
        print("\nWARN  setup drift (the Editor may regenerate these; confirm in the Groups window before trusting a build):")
        for name in missing:
            print(f"   registered in AddressableAssetSettings.asset, no asset on disk: {name}")
        for name in schemaless:
            print(f"   no BundledAssetGroupSchema, so no bundle is built for it: {name}")
        for path in orphans:
            print(f"   group asset not registered in the settings (ignored by Addressables): {path}")
    owners = defaultdict(list)
    folders = {}
    for g in groups:
        for guid, address, path in g["entries"]:
            owners[guid].append((g["name"], path or address))
            if path:
                folders[path.rstrip("/") + "/"] = g["name"]
    twice = {k: v for k, v in owners.items() if len(v) > 1}
    nested = []
    for g in groups:
        for guid, address, path in g["entries"]:
            if not path:
                continue
            parts = path.split("/")
            for i in range(len(parts) - 1, 0, -1):
                owner = folders.get("/".join(parts[:i]) + "/")
                if owner and owner != g["name"]:
                    nested.append((path, g["name"], "/".join(parts[:i]), owner))
                    break
    if twice or nested:
        errors += len(twice) + len(nested)
        print(f"\nERROR A4 one GUID, one group: {len(twice)} registered twice, {len(nested)} inside another group's folder entry")
        for guid, where in sorted(twice.items())[:40]:
            print(f"   {guid}  {where}")
        for path, group, folder, owner in sorted(nested)[:40]:
            print(f"   [{group}] {path}\n      inside [{owner}] {folder}/")

    outside = defaultdict(list)
    unresolved = defaultdict(int)
    for g in groups:
        for guid, address, path in g["entries"]:
            where = path or (address if address.startswith(("Assets/", "Packages/")) else None)
            if where is None:
                unresolved[g["name"]] += 1
            elif not where.startswith(DELIVERABLE_ROOT):
                outside[g["name"]].append(where)
    n = sum(len(v) for v in outside.values())
    if n:
        label = "ERROR" if A0_IS_ERROR else "WARN "
        errors += n if A0_IS_ERROR else 0
        warnings += 0 if A0_IS_ERROR else n
        print(f"\n{label} A0 entries outside {DELIVERABLE_ROOT}: {n} in {len(outside)} groups")
        for group, paths in sorted(outside.items(), key=lambda kv: -len(kv[1])):
            roots = sorted({"/".join(p.split("/")[:3]) for p in paths})
            print(f"   {len(paths):5d}  [{group}]  under {', '.join(roots[:4])}{' ...' if len(roots) > 4 else ''}")
    if unresolved:
        print(f"\nnote: {sum(unresolved.values())} entries whose GUID has no .meta here and whose address is not a path "
              f"(a package submodule not checked out?): {dict(unresolved)}")

    print(f"\n{errors} error(s), {warnings} warning(s)")
    return errors


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    ap = argparse.ArgumentParser(description=__doc__.split("\n\n")[0])
    ap.add_argument("report", nargs="?", help="buildlayout_*.json path or name (default: the newest)")
    ap.add_argument("--static", action="store_true", help="check project files only; no build report needed")
    ap.add_argument("--profile", default=None, help="Addressables profile for load paths (default: buildlayout.PROFILE)")
    args = ap.parse_args()
    profile = args.profile or PROFILE
    errors = check_static(profile) if args.static else check_report(args.report, profile)
    sys.exit(1 if errors else 0)
