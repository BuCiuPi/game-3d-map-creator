#!/usr/bin/env python3
# Ported from cricket-city-unity tools/addressables/diff.py @ 8da403c97; stripped of
# Cricket specifics. Feature entries come from buildlayout.FEATURE_GROUP.
"""Compare two build reports: what a change did, and whether it did anything else.

  python -I tools/addressables/diff.py                       # the two newest reports
  python -I tools/addressables/diff.py <old.json> <new.json>

Read the result in this order:
  1. groups byte-identical — a placement fix should leave all but a few untouched;
  2. unexpected growth — a group growing by more than what you moved means an
     excluded or moved asset was copied in implicitly;
  3. dependents on unbuilt groups — must be empty after an exclusion;
  4. boot and per-feature entry deltas — the number you were after.
Font-atlas churn (TMP dynamic SDF assets) moves whichever group holds the font by
a few hundred KB between builds on its own; revert the font asset, do not chase it.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))   # works under python -I
from buildlayout import Layout, mb, resolve  # noqa: E402

if len(sys.argv) not in (1, 3):
    raise SystemExit(__doc__)
sys.stdout.reconfigure(encoding="utf-8")

old = Layout(resolve(sys.argv[1] if len(sys.argv) > 2 else None, -2))
new = Layout(resolve(sys.argv[2] if len(sys.argv) > 2 else None, -1))


def summary(lay):
    boot = lay.boot()
    rows = {"total": sum(lay.group_size.values()), "boot": lay.size(boot)}
    for g in lay.features():
        rows[f"{g} entry"] = lay.size(lay.first_entry(g, boot))
    return rows


print(f"{old.name}  ->  {new.name}")
a, b = summary(old), summary(new)
for k in dict.fromkeys(list(a) + list(b)):
    d = b.get(k, 0) - a.get(k, 0)
    print(f"  {k:40s} {mb(a.get(k, 0))} -> {mb(b.get(k, 0))}   {d / 1e6:+.2f}")

groups = sorted(set(old.group_size) | set(new.group_size))
same = [g for g in groups if old.group_size.get(g) == new.group_size.get(g)]
print(f"\ngroups byte-identical: {len(same)} of {len(groups)}")
for g in groups:
    d = new.group_size.get(g, 0) - old.group_size.get(g, 0)
    if d:
        tag = " (removed)" if g not in new.group_size else " (added)" if g not in old.group_size else ""
        print(f"  {d / 1e3:+12.1f} KB  {g}{tag}")

built = set(new.group_size)
dangling = sorted({new.bundles[r]["group"] for r, deps in new.bundle_deps.items()
                   if any(new.bundles[d]["group"] not in built for d in deps)})
print(f"\ndependents on unbuilt groups: {dangling or 'none'}")
