#!/usr/bin/env python3
# Ported from cricket-city-unity tools/addressables/har.py @ 8da403c97; stripped of
# Cricket specifics. Phase marks come from buildlayout.FEATURE_GROUP.
"""Runtime truth from a browser HAR: what actually downloaded, when, and how.

  python -I tools/addressables/har.py <file.har>

Needs a WebGL (or other browser-hosted) build whose bundles are fetched over
HTTP. The template has no WebGL host yet (the main platform is an open question,
.claude/knowledge/01-project-brief.md); on a native build, use the device's
network capture or Addressables' own profiler instead.

Record the HAR with DevTools "Disable cache" OFF (otherwise every image and bundle
looks uncached), on a cold profile for download sizes or a warm one for cache
behaviour. Phases are split at the first request of each feature group's own
bundle (buildlayout.FEATURE_GROUP): boot = everything before the first one, then
one phase per feature entered.

Checks: bundle errors/aborts, the same bundle URL fetched twice, requests for any
group excluded from the build (read from the project's Addressables settings), and
image cache status (304 = cached and revalidated; 200 on a repeat = not cached).
"""
import json
import os
import re
import sys
from collections import Counter, defaultdict
from urllib.parse import urlparse

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))   # works under python -I
from buildlayout import DEPRECATED_PREFIX, FEATURE_GROUP, excluded_groups  # noqa: E402

if len(sys.argv) != 2 or sys.argv[1] in ("-h", "--help"):
    raise SystemExit(__doc__)
sys.stdout.reconfigure(encoding="utf-8")

with open(sys.argv[1], encoding="utf-8-sig") as handle:
    entries = sorted(json.load(handle)["log"]["entries"], key=lambda e: e["startedDateTime"])
if not entries:
    raise SystemExit(f"{sys.argv[1]}: no requests")


def name(e):
    return urlparse(e["request"]["url"]).path.split("/")[-1]


def size(e):
    r = e["response"]
    return r.get("_transferSize") if (r.get("_transferSize") or 0) > 0 else (r.get("content", {}).get("size") or 0)


def stem(n):
    """Bundle name without content hashes, lower-case: comparable across builds."""
    return re.sub(r"\.bundle$", "", re.sub(r"_[0-9a-f]{32}", "", n.lower()))


bundles = [e for e in entries if name(e).endswith(".bundle")]
catalogs = sorted({name(e) for e in entries if "catalog" in name(e)})
print(f"{len(entries)} requests, {len(bundles)} bundles, {entries[0]['startedDateTime']} -> {entries[-1]['startedDateTime']}")
print(f"catalogs: {catalogs}")
print(f"bundle statuses: {dict(Counter(e['response']['status'] for e in bundles))}")

errors = [name(e) for e in bundles if e["response"]["status"] == 0 or e["response"].get("_error")]
print(f"bundle errors/aborts: {len(errors)} {errors[:10]}")
dupes = [(n, c) for n, c in Counter(e["request"]["url"] for e in bundles).items() if c > 1]
print(f"same bundle URL fetched more than once: {len(dupes)} {[urlparse(u).path.split('/')[-1] for u, _ in dupes][:10]}")
excluded = [g.replace(DEPRECATED_PREFIX, "").lower() for g in excluded_groups()]
hits = sorted({name(e) for e in bundles if any(stem(name(e)).startswith(g + "_") for g in excluded)})
print(f"requests for excluded groups ({len(excluded)} excluded): {hits or 'none'}")

# Phases: split at the first request of each feature group's own bundle.
feature_bundle = re.compile(rf"({FEATURE_GROUP})_", re.I)
marks = {}
for e in bundles:
    m = feature_bundle.match(stem(name(e)))
    if m:
        marks.setdefault(m.group(1), e["startedDateTime"])
cuts = [("boot", "")] + sorted(((f"{g} entry", t) for g, t in marks.items()), key=lambda x: x[1])
width = max(20, max(len(label) for label, _ in cuts))
print(f"\n  {'phase':{width}s} bundles   downloaded")
phase_of = {}
for i, (label, start) in enumerate(cuts):
    end = cuts[i + 1][1] if i + 1 < len(cuts) else "~"
    rows = [e for e in bundles if start <= e["startedDateTime"] < end]
    for e in rows:
        phase_of[id(e)] = label
    print(f"  {label:{width}s} {len(rows):4d}   {sum(size(e) for e in rows) / 1e6:8.2f} MB")

print("\nbundles by phase (largest first):")
by_phase = defaultdict(list)
for e in bundles:
    by_phase[phase_of.get(id(e), "?")].append(e)
for label, _ in cuts:
    for e in sorted(by_phase[label], key=size, reverse=True)[:12]:
        print(f"  {label:{width}s} {size(e) / 1e6:7.2f} MB  {stem(name(e))}")

images = [e for e in entries if re.search(r"\.(png|jpe?g|webp)(\?|$)", e["request"]["url"], re.I)]
print(f"\nimages ({len(images)}): status, bytes over the wire, file")
for e in images:
    print(f"  {e['startedDateTime'][11:19]}  {e['response']['status']}  {size(e):>9}  {urlparse(e['request']['url']).path[-70:]}")
