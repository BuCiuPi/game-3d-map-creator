# Knowledge Index Template

`.claude/knowledge/` is the agent's map of the code: where things live, how they
are wired, and how to add the common things. `00-Master-Index.md` is the entry
point; `CLAUDE.md` tells agents to read it, and each file is loaded only when a
task touches its area (`docs/CONTEXT_RULES.md`).

## Writing Rules (all knowledge files)

- **Every claim cites a path** — `path/to/file.ext` plus a symbol or line
  (`src/app.ts:42`, `internal/orders/service.go` `Service.Create`). A claim
  without a citation is deleted or turned into a `TODO(harness-bootstrap)`.
- **Observed, not policy.** Code shows current behavior. Write "the bootstrap
  registers X before Y", not "X must be registered before Y", unless a doc or
  decision says so — then cite it.
- **Discover with the graph, confirm with text.** Use codebase-memory MCP
  (`get_architecture`, `search_graph`, `trace_path`, `get_code_snippet`) to find
  entry points, call paths, and implementations; confirm each with Grep/Read.
  Use Grep/Glob for config, serialized data, and literal values.
- **Link, do not duplicate.** If a skill, rule, or doc already covers a topic,
  write one line pointing at it and say when to load it.
- **Budget:** 80–300 lines per file. Prefer tables and short recipes to prose.
  A file that grows past that splits by subsystem.
- **Header** on each file: one-line purpose, `Verified against <short rev> on
  <YYYY-MM-DD>`, and the directories it covers.
- **Never overwrite owner files.** Existing files stay; new files are numbered
  after them.

## `00-Master-Index.md` Shape

```markdown
# Master Index — <Project> Agent Knowledge

<One paragraph: what the repository is, the live code surface, frozen or
generated areas. Cited.>

## Harness Bootstrap

| Field | Value |
| --- | --- |
| Bootstrapped | <YYYY-MM-DD> at `<short rev>` |
| Last refresh | <YYYY-MM-DD> at `<short rev>` or — |
| Router | `.claude/skills/<router>/SKILL.md` (created / upgraded) |

## Stack

| Stack | Root | Versions | Evidence |
| --- | --- | --- | --- |
| <name from stack-hints> | `<root>/` | <language/framework versions from manifests> | `<manifest paths>` |

## Files in This Knowledge Base

| File | Type | Description |
| --- | --- | --- |
| [01-Entry-and-App-State.md](01-Entry-and-App-State.md) | Reference | <one line> |
| [02-Data-Models.md](02-Data-Models.md) | Reference | <one line> |
| [03-Module-Composition.md](03-Module-Composition.md) | Task | <one line> |
| [04-How-To-Add-X.md](04-How-To-Add-X.md) | Task | <one line> |
| [05-Build-and-Deploy.md](05-Build-and-Deploy.md) | Reference + Task | <one line> |

Type: **Reference** answers "what does X do / where is X"; **Task** answers
"how do I add or wire X".

## Analyzed Directories

| Directory | Purpose |
| --- | --- |
| `<dir>/` | <purpose, cited> |

Refresh mode compares this table with the top-level layout to find drift.

## Key Dependencies

| Package | Role |
| --- | --- |
| <name from the manifest> | <what the code uses it for, with one usage path> |

## Key Architectural Patterns

- **<Pattern>** — <one line>, e.g. `<path>`.

## Related Skills, Rules, and Docs

| Location | Covers | When to load |
| --- | --- | --- |
| `.claude/skills/<skill>/` | <topic> | <trigger> |
| `.claude/rules/<rule>.md` | <topic> (auto-loads for `<globs>`) | automatic |
| `docs/<doc>.md` | <topic> | <trigger> |

### When to Use What

- "What does X do / where is X?" → reference files.
- "How do I add or wire X?" → task files, then the skill named in the recipe.

## Open Gaps

- `TODO(harness-bootstrap): <missing evidence and where it would come from>`
```

## First Knowledge Files

Write 3–6. Skip a file only when the repository genuinely has no such area, and
say so in the index.

### 01 — Entry and App State

- Every process or app entry point: `main`, server bootstrap, engine startup
  scene/script, CLI entry, worker entry. Path and symbol each.
- Startup order as a numbered list: config load → dependency construction →
  connections → route/scene/screen registration → listen/run.
- Where application state lives (stores, singletons, session, caches,
  persisted saves) and who owns each mutation.
- Configuration and environment: how config is loaded, precedence between
  sources, which keys are required (names only, never values or secrets).
- Shutdown and error boundaries: top-level handlers, graceful shutdown.

### 02 — Data Models

- Core domain types and where they are defined; DTOs vs persisted models vs
  view models, and the mappers between them.
- Persistence: schema or migration location, ORM/query layer, how a migration
  is created and applied (command from `docs/TOOL_REGISTRY.md`).
- Serialization and contracts: API schemas (OpenAPI/proto/GraphQL), generated
  code and its generator — generated files are regenerated, never edited.
- Static or master data, enums, and identifiers that must stay stable.

### 03 — Wiring, DI, or Module Composition

- The composition root: where dependencies are constructed or registered (DI
  container, module system, context tree, plugin registry, router mounting).
- Scope/lifetime rules observed in code (singleton vs per-request vs
  per-scene), with paths.
- Module boundaries and allowed dependency direction, as shown by imports,
  package layout, or workspace config; cite any lint rule that enforces it.
- Event/message buses: where events are declared, published, subscribed, and
  unsubscribed.

### 04 — How To Add X

Recipes for the 3–6 most common additions this codebase shows (a route, a
screen, a job, a migration, a service, a component, a config key). Each recipe:

1. **Reference implementation** — the sibling to copy, by path.
2. **Steps** — files to create or touch, in order, with the registration point.
3. **Proof** — the command that shows it works (from `docs/TEST_MATRIX.md`).
4. **Traps** — known ordering or lifecycle pitfalls, cited.

If a skill already scaffolds the addition, the recipe is one line pointing at
it.

### 05 — Build and Deploy

- Install, build, run, test, lint commands that exist (package scripts, Make
  targets, engine CLI) and the working directory for each.
- Build variants: environments, flags, targets, and where each is selected.
- CI: checked-in workflows (path, trigger, what they run) or the external
  pipeline the repo names; what is unverified.
- Release/deploy path and who may run it; never instructions to deploy.
- Generated artifacts and output directories that must not be hand-edited.

### 06 — Optional Domain Deep Dive

One file for a central subsystem the router routes to often (the game loop, the
payment flow, the sync engine). Same rules: cited, observed, linked to its skill.
