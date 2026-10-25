---
paths:
  - "**/*.cs"
---

<!-- Ported from cricket-city-unity .claude/rules/csharp-type-choice.md @ 8da403c97; stripped of Cricket specifics. -->

# C# Type Choice — class, record, struct

Status: **Guideline**, not a gate. Authority: the owner adopted the reference
harness's rule on 2026-10-25
(`docs/decisions/0007-harness-follows-cricket-city.md`). Items marked
**(engine)** are not style: they are code that silently does not work in Unity,
and review treats them as defects.
Checked by: review (`unity-code-reviewer`); not mechanically checked.

Read this before declaring a new data-carrying type. It is a **guideline, not a
gate**: a wrong choice here is a suggestion in review (ℹ️ INFO), never a
blocker. The point is that the choice is made deliberately rather than by
copying whatever file was open.

## The one sentence

`record` is not a new kind of type — it is a `class` (or `struct`) with
compiler-generated `Equals`, `GetHashCode`, `==`, `ToString`, and a copy
constructor. Choosing between them is choosing **what `==` should mean**.

- Same values ⇒ same thing → `record`
- Has its own identity ⇒ two instances are different even with equal fields
  → `class`

## Decision table

| Use | When |
| --- | --- |
| `class` | Unity object (`MonoBehaviour`, `ScriptableObject`) · part of an inheritance hierarchy · holds mutable state · has identity (two users with the same name are not the same user) · anything Unity must serialize |
| `record` | Wire payload / DTO · pure data carrier with no inheritance · value semantics wanted · you want a readable `ToString` for debugging |
| `struct` | Small (roughly ≤ 16 bytes), short-lived, allocated per frame or per element, and never null |
| `readonly record struct` | The above, plus value equality — e.g. an id, a coordinate, a position |

## Unity-specific rules that override the table

1. **(engine) `MonoBehaviour` and `ScriptableObject` must be `class`.** Engine
   requirement, no discretion. The template's `DataTable<TRow>` and
   `DataAsset<TData>` (`Assets/game.core/Runtime/Data/`) are `ScriptableObject`s.

2. **(engine) Unity serializes FIELDS, not properties.** A positional record
   (`record Foo(int X, int Y)`) generates *properties*, so Unity serializes
   nothing and the asset silently loads with defaults. Any `[Serializable]`
   type that Unity serializes stays a plain `class` or `struct` with
   `[SerializeField] private` fields. This repo has zero `record` types in
   `Assets/game.*` and zero `[Serializable] record`, and should keep the
   second at zero.

3. **`init` means nothing to Unity's serializer.** It writes fields directly by
   reflection, so `init` neither protects nor breaks anything — it is a
   compile-time guarantee only.

4. **Prefer `struct` on per-frame paths.** GC spikes are visible on WebGL and
   low-end mobile. The template's main platform is still open
   (`.claude/knowledge/01-project-brief.md`), so assume the tight one. A class
   allocated in `Update`, in a per-piece loop, or per path point is a real cost;
   a struct is not.

5. **Watch boxing.** A `struct` stored in a field or collection typed as an
   `interface` (or `object`) boxes on every assignment, which undoes the reason
   you chose a struct. Either keep the concrete type, or use a class.

6. **(engine)** **Template:** carried from the template's earlier rule. A
   serialized enum field must have an underlying type that fits a signed 32-bit
   int (`byte`, `sbyte`, `short`, `ushort`, `int`). `uint`, `long`, `ulong`
   throw "Unsupported enum type" at inspector-draw time — a console error, not a
   compile error.

## Two traps that make `record` misleading

**Collections.** The generated `Equals` compares *all instance fields* using
`EqualityComparer<T>.Default`. For a `List<>` or `Dictionary<>` that is
**reference** comparison — so two structurally identical records are not equal.
It looks like value equality and is not.

```csharp
public record Bag { public List<int> Items = new() { 1, 2 }; }
new Bag() == new Bag()   // false
```

If a record holds a collection, either say so in a comment or use `class`.

**Inheritance.** Record equality includes `EqualityContract`, which is the
runtime type. A base and a derived instance are never equal even with identical
values:

```csharp
record Base { public int X; }
record Derived : Base { }
Base a = new Base { X = 1 };
Base b = new Derived { X = 1 };
a == b   // false
```

So the API layer splits by family, and the split is worth copying:

| Family | Choice | Why |
| --- | --- | --- |
| A base payload extended per game or per feature (a shared user-state type with a game-specific subclass) | `class` | It is **inherited**, so record equality would be a trap |
| Response leaves — one endpoint's payload, nothing derives from it | `record` | Standalone, pure data, no inheritance |

**Template:** the payloads and models today are all classes
(`UserProfileApiData`, `UserProfileModel`, popup data such as
`PopupPauseData`), and DT rows derive from `DataTableRow`
(`Assets/game.core/Runtime/Data/DataTableRow.cs`) — keep those families
`class`. A new response leaf may be a `record`.

## What `record` does NOT give you

- **Immutability.** `record Foo { public int X { get; set; } }` is fully
  mutable. Immutability comes from `init` or `readonly`, not from `record`.
- **A value type.** `record` means `record class` — reference type, heap
  allocated, nullable. `record struct` is a different thing.
- **A usable `with`** when every member is a get-only computed property.
  `with` needs `init` or `set` accessors; without them it compiles and does
  nothing.

## Note for the wire/model split

A data class shaped as `[JsonProperty] private` fields with public computed
getters works well as a `record`, because the two generated members read
different things:

- `Equals` compares **all instance fields, including private** → compares
  `_node`, which is what you want.
- `ToString` prints **public members only** → prints `Node`, which is what you
  want to read.

Keep them reference types. A `record struct` breaks the null-tolerant mapping
(`data == null ? default : …`) that a data → model layer relies on and tests.

**Template:** the existing wire class `UserProfileApiData` puts
`[JsonProperty]` on public auto-properties instead (the reference harness's
data classes use the private-field shape). Match the shape the `api-codegen`
skill generates for new endpoints; do not reshape existing classes in passing.

## References

- `api-codegen` and `feature-domain-service` skills — the wire/model split
- `docs/architecture/template-core-data.md` — `DataTable<TRow>` / `DataAsset<TData>`
- `.claude/rules/asset-structure.md` — where the file itself goes
