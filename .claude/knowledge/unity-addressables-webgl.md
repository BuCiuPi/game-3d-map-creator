# Addressables, Bundles and WebGL Constraints

Type: Reference (stack-level). How Addressables and the WebGL player behave, for
agents placing assets, reviewing references, or auditing download size. The
repository's own tiers, groups, and tools are recorded in
`.claude/rules/asset-structure.md` and the numbered knowledge files.

## The bundle model

- The dependency graph is **bundle-level**. A hard reference from an asset in
  bundle A to an asset in bundle B makes A depend on all of B: loading A
  downloads B first, whole.
- **Pack Together** groups ship as one bundle the moment any asset in them is
  referenced. **Pack Separately / by label** groups ship only the bundles
  reached. A 1 KB material inside a packed group can cost the group's full size.
- A **soft reference** (`AssetReference`, serialized as `m_AssetGUID`) creates
  no bundle dependency; the target loads only when code calls
  `LoadAssetAsync`, and the target must be an addressable entry.
- Assets that are **not** addressable entries travel inside every bundle that
  references them — an implicit duplicate per referencing bundle. Shared
  non-entry assets belong in a covered shared group.
- Boot cost = everything reachable from the bundles and scenes loaded before
  the first interactive screen, plus groups loaded by address at boot. A
  feature's first-entry cost = its closure minus boot.

## Handles and memory

- Every `LoadAssetAsync` / `InstantiateAsync` returns a handle; release it
  (`Addressables.Release` / `ReleaseInstance`) when the owner is destroyed. An
  unreleased handle pins its bundle in memory.
- `LoadSceneAsync` returns a scene handle; unload through Addressables, not
  `SceneManager`, or the bundle stays loaded.
- Await handles with a cancellation token attached; a destroyed owner otherwise
  resumes on a dead object.

## Build reports and measurement

- Each Addressables build can write a build report / build layout
  (`Library/com.unity.addressables/` and the build-layout file). It lists every
  bundle, its size, its dependencies, and implicit duplicates — the ground truth
  for "what pulls what".
- Compare two builds group by group; a group that grew by more than what you
  moved means something got copied in implicitly.
- CI and local builds can hash bundles differently; match a runtime capture to a
  build by which bundles and sizes, not by hash.
- A network capture (HAR) is the runtime truth for what downloaded and when.
  Turn off the browser's "Disable cache" before trusting cache results.

## Editing groups

- Editing a group or schema `.asset` on disk while the Editor is open is lost:
  the Editor builds from memory. Toggle settings in the Editor, or refresh and
  confirm before building.
- Excluding a group (*Include in Build* off) is safe only with zero bundle
  dependents and no load by address or label; prove both on a fresh build.
- Dynamic TextMeshPro font atlases re-bake and change size between builds;
  revert them rather than chasing the delta.

## WebGL player constraints

- **Single-threaded.** No `Thread`, `Task.Run`, `Parallel`, or thread pool work;
  blocking waits freeze the tab. Async is cooperative (UniTask on the player
  loop).
- **IL2CPP / AOT.** Generic code paths that are only reached via reflection can
  be stripped or fail code generation. Prefer concrete `JsonConverter` types
  over generic ones; keep reflection-discovered types alive with `link.xml` or
  `[Preserve]`.
- **Memory.** The heap is fixed or grows in steps; mobile Safari is the tightest
  target. Unload unused bundles and avoid per-frame allocations (LINQ, string
  formatting, boxing, `new List<>` in `Update`).
- **`DownloadDependenciesAsync` does not keep bundles for later loads** on WebGL:
  a pre-download races the real load and fetches twice. Do not pre-download.
- **Caching** goes through the browser and IndexedDB; it differs per browser and
  can be cleared by the user.
- **JavaScript bridge.** `.jslib` functions and messages from the page
  (`SendMessage` to a named GameObject) are a contract with the web host: a
  renamed method or changed payload breaks silently. Change both sides in
  lockstep.
- **Server payload quirks.** A backend that returns `{}` where a list is
  expected throws in Newtonsoft; normalize with a concrete converter that maps
  `{}` to an empty array.
- **Editor is not the platform.** Editor play runs Mono, multithreaded, with the
  Editor's memory; a user-visible WebGL change is verified on a WebGL build, and
  on a device for mobile browsers.

## Dependency hygiene

- UPM git-URL packages pin a tag or commit; `manifest.json` and
  `packages-lock.json` change together.
- An editor-only DLL (importers, spreadsheet clients, code generators) has only
  the Editor platform enabled in its `.meta`. Package installers and upgrades
  often regenerate `.meta` files with "Any Platform", silently shipping editor
  code in the player.
