---
name: unity-editor-scripting
description: "Cross-cutting Unity editor-automation mechanics for this repo — reusable techniques for prefab/scene automation, Editor tooling and migration work. Use when renaming a C# class without breaking prefab/scene refs (GUID-preserving), wiring [SerializeField]/[SerializeReference] fields on a prefab via a scripted Editor pass, running EditMode test logic in batch mode or while the Editor holds the project, or verifying a change with Unity CLI batch-mode commands and Editor probes (compile log, reflect-and-invoke). NOT for authoring gameplay code — this is the editor-side plumbing."
---

<!-- Ported from cricket-city-unity .claude/skills/unity-editor-scripting/SKILL.md @ 8da403c97; stripped of Cricket specifics. -->

# Unity Editor-Scripting Mechanics

Reusable, repo-agnostic editor-side techniques. These recur across nearly every
implementation task; keep them in one place. Each is a *mechanic*, not a feature —
compose them into the task at hand.

**No Unity MCP.** The owner dropped the Editor bridge (decision 0007): no Unity
MCP or asset-graph tools, and `com.coplaydev.unity-mcp` is removed from
`Packages/manifest.json`; do not add one back. The agent's Editor proof
is the Unity CLI in batch mode — the commands in
`.claude/knowledge/03-build-and-profiles.md` → *Proof Commands* — and anything that
needs the open Editor is a menu item the owner clicks.

| Need | How, in this template |
| --- | --- |
| Compile | `"<Unity 6000.3.10f1>/Editor/Unity.exe" -batchmode -quit -projectPath . -logFile -`, then grep the log for `error CS` |
| EditMode tests | `... -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode.xml -logFile Logs/editmode.log` (no `-quit`; `-runTests` exits on its own) |
| Run an Editor pass | a static method, invoked with `... -batchmode -quit -projectPath . -executeMethod <Namespace.Class.Method> -logFile -`, or its `[MenuItem]` clicked by the owner |
| Existing Editor menus | `Game Template/Addressables/Apply Template Layout` and `Game Template/Addressables/Build Content` |

Batch mode needs only a normal clone and **fails while the Editor has the same project open** (one Unity instance per
project). Ask the owner to close the Editor, or run it on a second worktree (§3).

---

## 1. GUID-preserving class rename

Renaming a MonoBehaviour or ScriptableObject class must keep prefab/scene `m_Script`
references intact. The `.cs.meta` carries the script GUID, so move **both** files:

```bash
git mv Foo.cs Bar.cs
git mv Foo.cs.meta Bar.cs.meta      # ← the GUID lives here
```

- The class name MUST match the new file name (Unity's MonoScript association is by
  file name + GUID).
- All `[SerializeField]`s deserialize by field name onto the renamed class — no YAML
  edit. A renamed **field** needs `[FormerlySerializedAs("old")]`.
- A `[SerializeReference]` type renamed or moved across namespace or assembly needs
  `[MovedFrom(...)]`, or assets drop those objects on next save.
- **To overwrite the renamed `.cs` when the read-gate blocks reading it:** `rm Bar.cs`
  (keep `Bar.cs.meta`), then `Write` a fresh file — Unity re-associates by file name,
  GUID intact.
- Verify: `git status` shows `R` for both files; the `.cs.meta` `guid:` is unchanged.

A type rename *within the same namespace* is near-zero churn — existing `using`s
still resolve. Count the type's references outside its own file before deciding the
rename is small.

## 2. Wire prefab fields via a scripted Editor pass

Never hand-edit serialized YAML (especially `[SerializeReference]` managed-references).
Let Unity write it: load prefab contents → modify → save.

> Without an Editor bridge the agent cannot run arbitrary C# in the open Editor.
> Write the body below into a committed `Editor/` class — in an editor assembly such
> as `game.bootstrap.addressables.editor` (`Assets/game.bootstrap/Editor/Addressables/`) — as a **static**
> method with a `[MenuItem("Game Template/...")]`. Then either run it headless with
> `-executeMethod Namespace.Class.Method` (Editor closed), or have the owner click
> the menu item.

Skeleton (adapt the body):

```csharp
var root = PrefabUtility.LoadPrefabContents(PrefabPath);   // PrefabPath = "Assets/...prefab"
try
{
    var comp = root.GetComponentInChildren<TargetType>(true);   // or AddComponent if attaching
    // For plain [SerializeField] object/component refs, use SerializedObject — more reliable than reflection:
    var so = new SerializedObject(comp);
    so.FindProperty("presenter").objectReferenceValue = /* ref */;
    so.ApplyModifiedProperties();
    EditorUtility.SetDirty(comp);
    PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
}
finally { PrefabUtility.UnloadPrefabContents(root); }
```

Rules:
- **One-shot editor scripts go in a committed `Editor/` folder**, because a menu item
  or `-executeMethod` target must compile to be invoked, and are **deleted after**
  running — say so in the commit.
- **In batch mode, fail loudly.** An `-executeMethod` run exits 0 unless the method
  throws or calls `EditorApplication.Exit(1)`. Throw on any failed assertion, and
  write a result line to the log (`Debug.Log("[pass] ...")`) to grep for.
- **`AddComponent` can silently return `null`** — e.g. a `[RequireComponent(typeof(RectTransform))]`
  makes `AddComponent` return null on a prefab whose root has a plain `Transform`; an
  abstract type does too. Always **verify by re-loading** the prefab and checking
  `GetComponent<T>()` == the added component. A "WIRED" log string is not proof.
- **Batch a wave in one pass**: per prefab assert the expected root component exists,
  add + set fields, save, then assert the resolve chain — faster and safer than
  per-prefab agents, and surfaces "component not on root" instantly.
- **Disk path → Unity asset path** mapping matters: an installed Git UPM package
  appears to the Editor as `Packages/<package-name>/` even when its cache is under
  `Library/PackageCache/`. Treat package-cache files as read-only.
- **Editor-namespace shadow trap:** inside a namespace with a segment that matches a
  root namespace, a qualified name resolves against the nearer segment first. The
  current template namespaces avoid repeated product segments. Prefix a using
  with `global::` if a derived project introduces a namespace segment that
  shadows a root type.

## 3. EditMode logic: batch mode first, a direct-runner when the Editor stays open

**First choice: the Unity CLI.** With the Editor closed, `-runTests -testPlatform
EditMode` compiles and runs every EditMode assembly headless and writes NUnit XML
(`.agent-runtime/editmode.xml`). Read the XML (`<test-case ... result="Failed">`),
not the exit code alone; a compile error shows up in the log as `error CS` and an
empty or missing result file. The template's suite is `game.core.tests.editmode`.

**When the Editor must stay open** (the owner is working in it, so batch mode on this
checkout is locked), the Test Runner window only advances while the Editor is
**focused** (it stalls otherwise; a first run can report 0 tests mid-import). Then
drive the pure-C# logic **synchronously** from a committed `Editor/` menu-item runner
the owner clicks:

- Never block the main thread — no `.GetAwaiter().GetResult()`. Make the entry
  `async`/`await`; synchronously-completing awaits run to completion before returning.
- Design seams so tests need no MonoBehaviour: keep the logic's interface independent
  of the GameObject that hosts it, so fakes are plain classes. Do not `AddComponent`
  a MonoBehaviour defined in a throwaway assembly; use plain-class fakes and a bare
  `GameObject`.
- All-synchronous ops drain fully inside a `Pump().Forget()` before it returns →
  re-entrancy/order is observable in-line.
- Write results to a temp file (under `.agent-runtime/`), then read back.
- **You can also drive the committed `[Test]` suite by reflection** — find the fixture
  type across `AppDomain.CurrentDomain.GetAssemblies()`, then per test: construct →
  `[SetUp]` → invoke → `[TearDown]`, unwrapping `TargetInvocationException`. The test
  asmdef IS loaded in a normal editor (its DLL is in `Library/ScriptAssemblies`), so
  this needs no Test Runner. In the reference project this ran 25/25 tests across
  three fixtures with the editor unfocused.
- ⚠️ **Push a `UnityEngine.TestTools.Logging.LogScope` per test** when doing that
  (`Activator.CreateInstance(t, true)`, dispose in `finally`). Without one, every test
  calling `LogAssert.Expect` dies with `InvalidOperationException: No log scope is
  available` — and since that call often sits *before* the act, the test reports
  **red having asserted nothing at all**, which reads exactly like a real regression.
  Two of the 25 above failed this way until the scope was added.
- ⚠️ **Treat `SuccessException` as PASS, `IgnoreException`/`InconclusiveException` as
  SKIP.** `Assert.Pass`, `Assert.Ignore` and `Assert.Inconclusive` signal by
  THROWING; NUnit's own runner reads them as green, and a naive `catch (Exception)`
  reports them red — inventing a regression that does not exist. Discriminate on
  `GetType().Name`.
- **Discover fixtures, do not list them.** Filter `AppDomain.CurrentDomain.GetAssemblies()`
  to the project's own test assemblies and take every non-abstract type owning a
  `TestAttribute` method. A hardcoded list only ever proves the suites you already
  suspected — in the reference project the one genuine regression from a change
  lived in a fixture a targeted run would never have loaded. `[UnityTest]` methods
  are excluded for free, since they carry `UnityTestAttribute`, not `TestAttribute`.
- **Always pair the run with a clean-HEAD baseline**, made in a separate worktree
  (`git worktree add --detach <scratch-dir> HEAD`, run the same tests there in
  batch mode, `git worktree remove <scratch-dir>`) — never
  by stashing in the checkout the Editor has open, where `git stash pop` has silently
  failed to restore tracked files. The worktree's first batch run imports a fresh
  `Library/`, so it is slow; start it early. A suite with pre-existing red tells you
  nothing from one number: a with-fix run of 944/9/953 looked alarming until the
  baseline came back 942/9/951 with **the same 9 failures by name**. Diff the failure
  NAMES, not the counts.
- ⚠️ **Recompile first.** Have the owner focus the Editor (or Assets → Refresh) and
  confirm a clean console before clicking the runner — for a new file, confirm the
  new type exists in the compiled assembly. Skip this and the runner reflects over
  stale DLLs and confidently "proves" the pre-edit code.

**Still commit the real `[UnityTest]` asmdef suite** (runs in batch mode / a focused
editor / CI) — just don't block the gate on it when only the runtime assembly is
reachable. Discoverable EditMode asmdef = `includePlatforms:["Editor"]` +
`UnityEngine.TestRunner`/`UnityEditor.TestRunner` refs + `overrideReferences:true` &
`precompiledReferences:["nunit.framework.dll"]` + `defineConstraints:["UNITY_INCLUDE_TESTS"]`.

⚠️ **A plain compile may not compile the EditMode test asmdef.** In the reference
project an Editor compile check left `UNITY_INCLUDE_TESTS`-constrained test code
uncompiled, so a stale test call against a changed signature stayed green. Not
re-measured here with the batch `-quit` compile; assume the same and use the
`-runTests` command as the compile gate for test code.

## 4. Editor verification probes

> **Visual proof is manual.** Without an Editor bridge there is no screenshot tool,
> so ask the owner for the screenshot, or state that Editor evidence was not captured.
> Do not substitute a clean compile for it. Pair a hand-supplied screenshot with a
> `CanvasDump` (a menu item listing active `Canvas` by `sortingOrder`) to spot
> occlusion/layer issues.

- **Read the log, not the exit code.** Batch-mode output goes to `-logFile -`
  (stdout) or a file you name; the open Editor writes `%LOCALAPPDATA%\Unity\Editor\Editor.log`
  on Windows. Grep for `error CS`, `Exception` and your own `[pass]`/`[fail]` lines.
- **Drive the REAL control, not a util shortcut.** To reproduce a UI "not showing" bug,
  reflect the private button field and `onClick.Invoke()` — a direct util call can mask
  a call-site difference (e.g. a forwarded `CancellationToken` that self-cancels the
  navigation). In the reference project the real click left the page stack at 0 (stuck)
  while the util call with no token rendered it.
- **Reflect state, don't trust logs.** Read the field or public state that should
  have changed. The current boot seam is `TemplateBootstrap.State`,
  `TemplateBootstrap.LastError`, and `TemplateBootstrap.Shell`.
- **Editor play is not the platform.** The template has no auth or host bridge,
  but any derived feature that depends on a device SDK, native plugin, browser
  host, or platform lifecycle still needs a platform-build confirmation.
- **The project lock is the usual batch-mode failure.** "Multiple Unity instances
  cannot open the same project" means the Editor is open on this checkout; nothing ran.
  Do not report that as a test result.

---

## When these compose

- **Class → file rename that a prefab points to** → §1, then verify with §2's reload-check.
- **Attach a component to many prefabs / make a view a page** → §2 (attach + verify),
  watch the `AddComponent`-null trap.
- **Prove engine logic without a device** → §3 batch `-runTests` (or the direct-runner
  while the Editor stays open) for unit truth, §4 probes for runtime behavior, then
  defer visual/platform proof to the human gate.
