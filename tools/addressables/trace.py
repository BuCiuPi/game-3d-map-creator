#!/usr/bin/env python3
# Ported from cricket-city-unity tools/addressables/trace.py @ 8da403c97; stripped of
# Cricket specifics.
"""Why does one group pull another? Lists the asset -> asset edges behind a bundle dependency.

  python -I tools/addressables/trace.py <target-group-regex>                  # every group that pulls it
  python -I tools/addressables/trace.py <target-group-regex> <source-regex>   # only from these sources
  python -I tools/addressables/trace.py 'com\\.vfx\\.third_party$' '^ui_local$' --report buildlayout_….json

Regexes match group names. An edge's source is the asset holding the reference;
fix it there (move the target up, repoint to an identical local copy, or make the
reference soft). Assets that are not addressable entries travel inside the bundle
of whatever references them, so they never appear as a target here.
"""
import argparse
import os
import sys
from collections import defaultdict

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))   # works under python -I
from buildlayout import Layout, resolve  # noqa: E402

ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
ap.add_argument("target", help="regex on the target asset's group name")
ap.add_argument("source", nargs="?", help="regex on the source asset's group name")
ap.add_argument("--report", help="buildlayout_*.json path or name (default: the newest)")
args = ap.parse_args()
sys.stdout.reconfigure(encoding="utf-8")

lay = Layout(resolve(args.report))
by_target = defaultdict(lambda: {"size": 0, "users": set(), "groups": set()})
for src, dst in lay.asset_edges(args.source, args.target):
    t = by_target[(dst["group"], dst["path"])]
    t["size"] = dst["size"]
    t["users"].add(src["path"])
    t["groups"].add(src["group"])

print(f"{lay.name}: {len(by_target)} target assets")
for (group, path), t in sorted(by_target.items()):
    print(f"\n[{group}] {path}  ({t['size'] / 1e3:.1f} KB)")
    print(f"   pulled by {len(t['users'])} asset(s) in {sorted(t['groups'])}:")
    for u in sorted(t["users"]):
        print(f"      {u}")

groups = {g for g, _ in by_target}
for g in sorted(groups):
    print(f"\n{g}: bundle dependents = {lay.dependents(g)}")
