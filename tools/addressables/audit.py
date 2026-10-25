#!/usr/bin/env python3
# Ported from cricket-city-unity tools/addressables/audit.py @ 8da403c97; stripped of
# Cricket specifics. Features come from buildlayout.FEATURE_GROUP.
"""Where every in-build group downloads: boot, a feature's first entry, or on demand.

  python -I tools/addressables/audit.py                 # latest build report
  python -I tools/addressables/audit.py buildlayout_2026.10.25.13.35.09.json

Needs a build report (every Addressables build writes one under
Library/com.unity.addressables/BuildReports/). Boot and feature entry follow the
project configuration in buildlayout.py.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))   # works under python -I
from buildlayout import Layout, mb, resolve  # noqa: E402

if len(sys.argv) > 2 or (len(sys.argv) == 2 and sys.argv[1] in ("-h", "--help")):
    raise SystemExit(__doc__)
sys.stdout.reconfigure(encoding="utf-8")

lay = Layout(resolve(sys.argv[1] if len(sys.argv) > 1 else None))
boot = lay.boot()
features = lay.features()
top = [g for g in features if not lay.parent(g)]
sub = [g for g in features if lay.parent(g)]

# A sub-feature is entered from its parent, so its cost excludes the parent's.
# A top-level feature is entered from the first screen, so its cost is everything
# past boot, including any other feature it reaches — that is the cost a player pays.
phase = {}
for r in boot:
    phase[r] = "boot"
entries = {}
for g in top + sub:
    entries[g] = lay.first_entry(g, boot)
    for r in entries[g]:
        phase.setdefault(r, f"{g} entry")

print(f"{lay.name}: {len(lay.group_size)} groups, {len(lay.bundles)} bundles, {mb(sum(lay.group_size.values())).strip()}")
print(f"boot (modelled): {mb(lay.size(boot)).strip()} in {len(boot)} bundles")
if not features:
    print("no feature groups in this build (buildlayout.FEATURE_GROUP matched nothing)")
for g in top + sub:
    print(f"{g} first entry: {mb(lay.size(entries[g])).strip()} in {len(entries[g])} bundles")

print("\nwhen      size        group (bundles reached / bundles in group)")
rows = {}
for r, b in lay.bundles.items():
    key = (phase.get(r, "on demand"), b["group"])
    size, n = rows.get(key, (0, 0))
    rows[key] = (size + b["size"], n + 1)
order = ["boot"] + [f"{g} entry" for g in top + sub] + ["on demand"]
width = max(len(w) for w in order)
for (when, group), (size, n) in sorted(rows.items(), key=lambda kv: (order.index(kv[0][0]) if kv[0][0] in order else 99, -kv[1][0])):
    total = len(lay.group_bundles[group])
    split = f" ({n}/{total})" if n != total else ""
    print(f"{when:{width}s} {mb(size)}  {group}{split}")
