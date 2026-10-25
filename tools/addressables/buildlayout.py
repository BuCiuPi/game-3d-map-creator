# Ported from cricket-city-unity tools/addressables/buildlayout.py @ 8da403c97; stripped of
# Cricket specifics. Project values live in the "Project configuration" block below.
"""Shared model of an Addressables build report (buildlayout_*.json) and of the
project's Addressables settings on disk.

Stdlib only. Every tool in this folder imports it; none of them talk to Unity.
The graph is bundle-level: a label-packed or pack-separately group contributes
only the bundles actually reached, not its whole size.

Two inputs:
  * a build report, written by every Addressables build under
    Library/com.unity.addressables/BuildReports/ (Layout);
  * the settings, group and schema assets under Assets/AddressableAssetsData/
    (project_groups), read for load paths, Include in Build and entries.
"""
import json
import os
import re
from collections import defaultdict, deque

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
REPORTS = os.path.join(ROOT, "Library", "com.unity.addressables", "BuildReports")
ADDRESSABLES_DATA = os.path.join(ROOT, "Assets", "AddressableAssetsData")

# == Project configuration =====================================================
# The template's values on 2026-10-25 (.claude/knowledge/03-build-and-profiles.md,
# .claude/knowledge/02-runtime-and-wiring.md). A project made from the template
# edits this block, not the algorithms below. Re-check each value against a HAR
# whenever boot changes.

# Groups whose bundles download first, before anything else asks for them.
# The neutral template shell is split by availability. Both groups currently
# resolve through Local.LoadPath; profile modernization can move `game.init`
# later without changing the folder or runtime ownership contract.
INIT_GROUPS = ["game.local", "game.init"]
# Scenes loaded through Addressables before the first interactive screen.
# The only boot scene is in Build Settings and is not Addressable.
BOOT_SCENES = []
# Groups loaded by address before the first interactive screen, so no bundle graph
# reaches them. Observed from code, not yet measured: GameBootstrapper waits for
# GameMasterDataWrapper, which loads every `MasterData.DT_*` address. Then
# ChapterAddressablePreloader downloads the first chapter's preload label.
# The neutral shell loads no Jigsaw master-data or chapter pack.
BOOT_BY_ADDRESS = []
# Loaded by address on entering a feature, so no bundle graph reaches them:
# (entered-group regex, group loaded, regex of which of its bundles).
# Example shape: (re.compile(r"^game\.bundles\.\d{3}$"), "avatars", re.compile(r"_assets_")).
# TODO(project): fill from a HAR of a first feature entry; empty in the template.
ENTRY_BY_ADDRESS = []
# A feature group is a unit a player enters (an album pack, a mini-game, a mode).
# Its first-entry cost is its closure minus boot. A feature `X.Y` whose `X` is
# also a feature is entered from `X`, so its cost also excludes `X`'s closure.
# No feature group ships in the template. Projects use this namespace when they
# add a real 02_partial feature and then narrow the expression to their contract.
FEATURE_GROUP = r"game\.partial\..+"
# The profile whose load paths decide which groups ship inside the player.
PROFILE = "Default"
# A load path that resolves to the player's own data folder ships in the player.
# Anything else (a CDN, a hosting service) downloads. Plan rule A7: availability
# comes from the resolved path, never from a group or variable named "Local".
IN_PLAYER_MARKER = "Addressables.RuntimePath"
# Name prefix given to excluded groups (Include in Build off). TODO(project): adopt
# or drop; the template has no excluded group yet.
DEPRECATED_PREFIX = "[Deprecated] "
# == End of project configuration ==============================================

_FEATURE = re.compile(rf"^{FEATURE_GROUP}$")


def reports():
    """Report file names, oldest first (names sort by timestamp)."""
    if not os.path.isdir(REPORTS):
        raise SystemExit(f"no build reports in {REPORTS} - build Addressables first "
                         "(or pass a report path; check.py --static needs none)")
    found = sorted(f for f in os.listdir(REPORTS) if f.startswith("buildlayout_") and f.endswith(".json"))
    if not found:
        raise SystemExit(f"no buildlayout_*.json in {REPORTS} - build Addressables first")
    return found


def resolve(arg, index=-1):
    """A report path from an explicit path/name, or the Nth newest (-1 = latest)."""
    if arg:
        return arg if os.path.isfile(arg) else os.path.join(REPORTS, arg)
    names = reports()
    if len(names) < abs(index):
        raise SystemExit(f"need {abs(index)} build reports in {REPORTS}, found {len(names)}")
    return os.path.join(REPORTS, names[index])


class Layout:
    def __init__(self, path):
        self.path = path
        self.name = os.path.basename(path)
        with open(path, encoding="utf-8-sig") as handle:
            data = json.load(handle)
        self._rids = {e["rid"]: e for e in data["references"]["RefIds"]}
        self.duplicated = data.get("DuplicatedAssets", [])

        self.bundles = {}                   # bundle rid -> {name, group, size}
        self.bundle_deps = defaultdict(set)  # bundle rid -> bundle rids it depends on
        self.assets = {}                    # asset rid -> {path, group, bundle, size, refs}
        for rid, entry in self._rids.items():
            kind, d = entry["type"]["class"], entry["data"]
            if kind == "BuildLayout/Bundle":
                self.bundles[rid] = {"name": d["Name"], "group": self._group_name(d), "size": d["FileSize"]}
                for ref in d.get("Dependencies", []):
                    if ref.get("rid") in self._rids:
                        self.bundle_deps[rid].add(ref["rid"])
            elif kind == "BuildLayout/ExplicitAsset":
                bundle = self._data(d.get("Bundle"))
                self.assets[rid] = {
                    "path": d["AssetPath"],
                    "guid": d.get("Guid"),
                    "group": self._group_name(bundle) if bundle else "?",
                    "bundle": d.get("Bundle", {}).get("rid"),
                    "size": d.get("SerializedSize", 0) + d.get("StreamedSize", 0),
                    "refs": [r["rid"] for r in d.get("ExternallyReferencedAssets", []) + d.get("InternalReferencedExplicitAssets", [])],
                }

        self.group_size = defaultdict(int)
        self.group_bundles = defaultdict(set)
        for rid, b in self.bundles.items():
            self.group_size[b["group"]] += b["size"]
            self.group_bundles[b["group"]].add(rid)

    def _data(self, ref):
        e = self._rids.get(ref.get("rid")) if isinstance(ref, dict) else None
        return e["data"] if e else None

    def _group_name(self, bundle):
        g = self._data(bundle.get("Group"))
        return g["Name"] if g else "(no group)"

    # -- graph -----------------------------------------------------------------

    def closure(self, bundle_rids):
        seen, queue = set(bundle_rids), deque(bundle_rids)
        while queue:
            for dep in self.bundle_deps[queue.popleft()]:
                if dep not in seen:
                    seen.add(dep)
                    queue.append(dep)
        return seen

    def group_closure(self, group):
        return self.closure(self.group_bundles.get(group, set()))

    def boot(self):
        """Bundles downloaded before the first interactive screen: the init groups'
        closure, the boot scenes' closure, and BOOT_BY_ADDRESS. A HAR stays the
        ground truth (har.py)."""
        starts = set()
        for group in INIT_GROUPS + BOOT_BY_ADDRESS:
            starts |= self.group_bundles.get(group, set())
        starts |= {a["bundle"] for a in self.assets.values()
                   if any(a["path"].lower().endswith(scene) for scene in BOOT_SCENES)}
        starts.discard(None)
        return self.closure(starts)

    def entry(self, group, already=None):
        """Bundles a first visit to `group` adds on top of `already` (default: boot),
        including ENTRY_BY_ADDRESS."""
        starts = set(self.group_bundles.get(group, set()))
        for entered, extra, bundle_name in ENTRY_BY_ADDRESS:
            if entered.match(group):
                starts |= {r for r in self.group_bundles.get(extra, set()) if bundle_name.search(self.bundles[r]["name"])}
        return self.closure(starts) - (self.boot() if already is None else already)

    def size(self, bundle_rids):
        return sum(self.bundles[r]["size"] for r in bundle_rids)

    def by_group(self, bundle_rids):
        out = defaultdict(int)
        for r in bundle_rids:
            out[self.bundles[r]["group"]] += self.bundles[r]["size"]
        return out

    def dependents(self, group):
        """Other groups with a bundle that depends on a bundle of `group`."""
        targets = self.group_bundles.get(group, set())
        return sorted({self.bundles[r]["group"] for r, deps in self.bundle_deps.items()
                       if deps & targets and self.bundles[r]["group"] != group})

    def asset_edges(self, source_group=None, target_group=None):
        """(source asset, target asset) pairs that cross groups."""
        for a in self.assets.values():
            if source_group and not re.search(source_group, a["group"]):
                continue
            for rid in a["refs"]:
                t = self.assets.get(rid)
                if not t or t["group"] == a["group"]:
                    continue
                if target_group and not re.search(target_group, t["group"]):
                    continue
                yield a, t

    def features(self):
        """Built groups matching FEATURE_GROUP, parents before their sub-features."""
        return sorted(g for g in self.group_size if _FEATURE.match(g))

    def parent(self, group):
        """The feature a sub-feature is entered from, or None for a top-level one."""
        head = group.rsplit(".", 1)[0]
        return head if head != group and head in self.group_size and _FEATURE.match(head) else None

    def first_entry(self, group, boot=None):
        """A feature's first-entry bundles: minus boot, and minus its parent's closure."""
        boot = self.boot() if boot is None else boot
        parent = self.parent(group)
        already = boot | self.group_closure(parent) if parent else boot
        return self.entry(group, already)


def mb(n):
    return f"{n / 1e6:7.2f} MB"


# -- project files (no build needed) ------------------------------------------

def _read(path):
    with open(path, encoding="utf-8", errors="replace") as handle:
        return handle.read()


def guid_index(roots=("Assets", "Packages")):
    """guid -> project-relative asset path, from every .meta under `roots`.
    Packages whose submodule is not checked out contribute nothing."""
    index = {}
    for top in roots:
        base = os.path.join(ROOT, top)
        for folder, dirs, files in os.walk(base):
            dirs[:] = [d for d in dirs if not d.startswith(".")]
            for f in files:
                if not f.endswith(".meta"):
                    continue
                full = os.path.join(folder, f)
                with open(full, encoding="utf-8", errors="replace") as handle:
                    for line in handle:
                        if line.startswith("guid:"):
                            rel = os.path.relpath(full[:-5], ROOT).replace(os.sep, "/")
                            index[line.split(":", 1)[1].strip()] = rel
                            break
    return index


def _profile_values(settings_text, profile):
    """Variable name -> raw value for one profile of AddressableAssetSettings.asset."""
    names = dict(re.findall(r"- m_Id: (\w+)\n\s+m_Name: (.*)\n\s+m_InlineUsage", settings_text))
    block = re.search(rf"m_ProfileName: {re.escape(profile)}\n\s+m_Values:\n((?:\s+- m_Id: \w+\n\s+m_Value: .*\n)+)",
                      settings_text)
    if not block:
        raise SystemExit(f"no Addressables profile named {profile!r} in AddressableAssetSettings.asset")
    by_id = dict(re.findall(r"- m_Id: (\w+)\n\s+m_Value: (.*)", block.group(1)))
    return names, {k: v.strip().strip("'\"") for k, v in by_id.items()}


def _expand(value, by_name, depth=0):
    """Expand [Variable] references to other profile variables; leave the rest."""
    if depth > 5:
        return value
    out = re.sub(r"\[([^\]]+)\]", lambda m: by_name.get(m.group(1), m.group(0)), value)
    return out if out == value else _expand(out, by_name, depth + 1)


def project_groups(profile=PROFILE, index=None):
    """Every group registered in AddressableAssetSettings.asset, read from disk:
    name, path, include_in_build, bundle_mode, load_path (resolved for `profile`),
    in_player, entries [(guid, address, asset path or None)]."""
    settings_path = os.path.join(ADDRESSABLES_DATA, "AddressableAssetSettings.asset")
    if not os.path.isfile(settings_path):
        raise SystemExit(f"missing {settings_path}")
    settings = _read(settings_path)
    names, by_id = _profile_values(settings, profile)
    by_name = {names[i]: v for i, v in by_id.items() if i in names}
    index = guid_index() if index is None else index

    block = re.search(r"m_GroupAssets:\n((?:\s*- \{.*\}\n)+)", settings)
    group_guids = re.findall(r"guid: (\w+)", block.group(1)) if block else []
    groups = []
    for gguid in group_guids:
        rel = index.get(gguid)
        if not rel:
            groups.append({"name": f"(unresolved group {gguid})", "path": None, "include_in_build": None,
                           "bundle_mode": None, "load_path": None, "in_player": None, "entries": []})
            continue
        text = _read(os.path.join(ROOT, rel))
        name = re.search(r"m_GroupName: (.*)", text).group(1).strip().strip("'\"")
        entries = [(g, a.strip().strip("'\""), index.get(g))
                   for g, a in re.findall(r"^  - m_GUID: (\w+)\n    m_Address: (.*)$", text, re.M)]
        group = {"name": name, "path": rel, "include_in_build": None, "bundle_mode": None,
                 "load_path": None, "in_player": None, "entries": entries}
        for sguid in re.findall(r"- \{fileID: \d+, guid: (\w+), type: 2\}", text.split("m_SchemaSet:", 1)[-1]):
            spath = index.get(sguid)
            if not spath:
                continue
            schema = _read(os.path.join(ROOT, spath))
            include = re.search(r"^\s*m_IncludeInBuild: (\d)", schema, re.M)
            load = re.search(r"m_LoadPath:\n\s+m_Id: (.*)", schema)
            if not (include and load):
                continue      # not a BundledAssetGroupSchema
            raw = load.group(1).strip()
            resolved = _expand(by_id.get(raw, raw), by_name)
            mode = re.search(r"m_BundleMode: (\d)", schema)
            group.update(include_in_build=include.group(1) == "1",
                         bundle_mode={"0": "together", "1": "separately", "2": "by label"}.get(
                             mode.group(1) if mode else "", "?"),
                         load_path=resolved, in_player=IN_PLAYER_MARKER in resolved)
        groups.append(group)
    return groups


def unregistered_group_assets(groups):
    """Group assets under Assets/AddressableAssetsData/ that the settings do not list
    (Addressables ignores them)."""
    registered = {g["path"] for g in groups if g["path"]}
    found = []
    for folder, _, files in os.walk(ADDRESSABLES_DATA):
        for f in files:
            if not f.endswith(".asset"):
                continue
            full = os.path.join(folder, f)
            rel = os.path.relpath(full, ROOT).replace(os.sep, "/")
            if rel not in registered and re.search(r"^  m_GroupName: ", _read(full), re.M):
                found.append(rel)
    return sorted(found)


def in_player_groups(profile=PROFILE):
    """Names of built groups whose load path resolves inside the player."""
    return {g["name"] for g in project_groups(profile) if g["in_player"] and g["include_in_build"]}


def excluded_groups(profile=PROFILE):
    """Names of registered groups with Include in Build off."""
    return [g["name"] for g in project_groups(profile) if g["include_in_build"] is False]
