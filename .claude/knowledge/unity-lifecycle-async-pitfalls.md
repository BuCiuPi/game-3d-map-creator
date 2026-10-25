# Unity Lifecycle, Serialization and Async — Pitfalls for Agents

Type: Reference (stack-level). This file is about Unity, not about this
repository; repository facts live in the numbered knowledge files listed in
`00-Master-Index.md`. Each item is a failure that compiles cleanly and breaks
later, which is why it is written down.

## Lifecycle order

- `Awake` → `OnEnable` → `Start` → `Update` loop → `OnDisable` → `OnDestroy`.
  `Awake` and `OnEnable` run back-to-back per object; `Start` runs only after
  every object in the loaded scene has had `Awake`. Use `Awake` for
  self-contained setup and `Start` (or a DI entry point) for anything that reads
  another object.
- An inactive GameObject runs no `Awake` until first activated. A component
  disabled in the prefab still gets `Awake`, but not `OnEnable`/`Start`.
- Subscribe/unsubscribe pairs belong in `OnEnable`/`OnDisable` when the object
  is pooled or toggled; `OnDestroy` alone leaks while the object is inactive.
- `OnDestroy` is not called on objects that were never activated.
- Script execution order between unrelated components is undefined unless set
  in Project Settings or by `[DefaultExecutionOrder]`; do not depend on it.

## Unity null and destroyed objects

- `UnityEngine.Object` overrides `==`: a destroyed object compares equal to
  `null` while the C# reference is still alive. `?.`, `??`, `is null`, and
  pattern matching bypass that override and call into a dead native object.
  Use `== null` / `!= null` (or the implicit bool) for Unity objects.
- `Destroy` is deferred to end of frame; `DestroyImmediate` is for editor code.

## Serialization

- Unity serializes **fields** (`public` or `[SerializeField]`), not properties.
  A positional `record` or an auto-property serializes nothing and the asset
  silently loads defaults.
- Serialized enums must fit a signed 32-bit int (`byte`, `sbyte`, `short`,
  `ushort`, `int`). `uint`/`long`/`ulong` raise "Unsupported enum type" in the
  console at inspector-draw time — the compile stays green.
- `[SerializeReference]` stores the managed type by assembly + namespace + class
  name. Renaming or moving such a type needs
  `[MovedFrom(true, sourceNamespace: ..., sourceAssembly: ..., sourceClassName: ...)]`,
  or assets referencing the old identity fail to load and **drop the object on
  the next save**. Every such type must be `[Serializable]`.
- Unity never calls `Dispose()` on `[SerializeReference]` objects; the owning
  component must fan out from `OnDestroy`.
- A class rename keeps prefab/scene links only if the `.cs.meta` (GUID) moves
  with it and the file name still matches the class name.
- `JsonUtility` ignores properties, dictionaries, and polymorphism; most
  projects use Newtonsoft with `[JsonProperty]` on private backing fields.

## Async (UniTask)

- No `async void` outside Unity event handlers; exceptions vanish. Return
  `UniTask`/`UniTaskVoid` and call `.Forget()` for fire-and-forget.
- Pass a `CancellationToken` to every awaited operation that accepts one.
  `this.GetCancellationTokenOnDestroy()` ties work to a MonoBehaviour; a linked
  `CancellationTokenSource` covers an interruptible sub-operation. Cancel and
  dispose owned sources in `OnDestroy`.
- `.AttachExternalCancellation(ct)` makes a non-cancellable await (an
  Addressables or web request handle) stop waiting; it does not cancel the
  underlying operation.
- `SuppressCancellationThrow()` allocates and turns cancellation into a silent
  success-looking result. Use it only when a caller reads the bool.
- Independent awaits (several API calls, asset loads) run in parallel with
  `UniTask.WhenAll`. Sequential awaits are for genuine dependencies and
  order-sensitive side effects.
- `await UniTask.Delay`/`WaitForSeconds`, never `Task.Delay`; `UniTask.Yield`
  or `NextFrame` to split heavy work across frames.
- Never block: `.GetAwaiter().GetResult()` or `.Result` on the main thread
  deadlocks or freezes, and WebGL has no other thread.

## Reactive subscriptions (R3 / UniRx)

- Every `Subscribe` returns an `IDisposable`; end it in `.AddTo(this)` or
  `.AddTo(_disposables)`. A naked subscribe on a static or long-lived subject
  keeps the subscriber alive after its scene unloads.
- An async handler uses `SubscribeAwait`, not `Subscribe` with an async lambda
  (the lambda becomes `async void`).
- Expose `Observable<T>`, not `Subject<T>` or `ReactiveProperty<T>`; handing out
  the property hands out the setter.

## Dependency injection (VContainer, Zenject)

- VContainer's source generator processes only `public` `[Inject]` members;
  a non-public one falls back to reflection (slower, and at risk of IL2CPP
  stripping). Batch two or more dependencies into one injected method or
  constructor.
- A registration in a child scope shadows the parent's silently: consumers in
  the child get a second instance. Register app-lifetime services once at the
  composition root; register scene-lifetime services in the scene scope.
- A parent container cannot resolve a child scope's registrations, so anything
  a global modal or overlay needs must live in the root (or a scope the modal
  is built from).
- Plain C# services get dependencies through the constructor; field injection
  arrives too late for construction-time use. In a MonoBehaviour injected at
  runtime, read injected fields only after the injection call.
- Resolving an unregistered type throws rather than injecting null; leaving an
  implementation deliberately unregistered for a build profile is a seam, not a
  bug.

## Build profiles and defines

- Code inside `#if <define>` is not compiled in the Editor's current profile. A
  production-only compile break (a caller outside a guard touching a type
  inside one) is invisible in the Editor. Keep a guarded member and every caller
  and payload type it touches under the same guard, and check both sides of
  every guard a diff edits.
- A define that no code reads but the Editor still lists is inert; code gated on
  it is dead.

## Tests and editor verification

- An EditMode test assembly needs `includePlatforms: ["Editor"]`, the test
  runner references, and usually `defineConstraints: ["UNITY_INCLUDE_TESTS"]`.
  A normal Editor recompile may not compile it, so a stale test can stay green
  until the test run itself.
- The Test Runner advances only while the Editor is focused; an unfocused
  Editor can report zero tests mid-import. A run that matches 0 tests is a
  filter miss, never a pass.
- Compare failures by test **name** against a clean-HEAD baseline; a suite with
  pre-existing red tells nothing from counts alone.
- `Assert.Pass`, `Assert.Ignore`, and `Assert.Inconclusive` signal by throwing;
  a hand-rolled runner must treat them as pass or skip, not failure.
- Editor play mode often cannot pass platform auth or a web-host handshake;
  verify the configuration hand-over in the Editor and leave the end-to-end
  confirmation to a device or platform build.
