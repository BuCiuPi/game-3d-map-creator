# Stack Hints

Starting points for `harness-bootstrap`. These are hints, not facts about any
repository: every domain, command, root, and glob you write must be confirmed
against the repository first (the directory exists, the script or target is
defined, the tool is configured). Drop what is not there; add what the evidence
shows.

## Detection Procedure

1. List top-level files and directories; read every build manifest and lockfile
   found at the root and one level down.
2. Match against the **Detection** row of each stack below. A repository can
   match several (a monorepo with a Go service and a React app); record each
   with its root.
3. Package manager comes from the lockfile: `pnpm-lock.yaml` → pnpm,
   `yarn.lock` → yarn, `package-lock.json` → npm, `bun.lock`/`bun.lockb` → bun.
   Script commands use that manager.
4. Read versions from manifests (engine version file, `go` directive,
   `engines`, framework dependency versions). Never guess a version.
5. Read CI workflows to learn which proofs the team already runs; those
   commands are the strongest candidates for `docs/TOOL_REGISTRY.md`.
6. No match → **Generic**.

## Installed Packs

When `.claude/harness-manifest.json` lists `packs`, start from the row each
pack maps to, then confirm against the repository as usual. Hints the manifest
records for a pack (`guardedRoots`, `proofTypes`, `routerDomains`) take
precedence over the row.

| Pack | Start from row | Extra hints when the manifest records none |
| --- | --- | --- |
| `unity-client` | Unity | — |
| `cocos-client` | Cocos Creator (TypeScript) | — |
| `go-backend` | Go Service | — |
| `fe-wrapper` | React / Next Frontend or Monorepo (and Node / TypeScript Backend for any server app inside it) | — |
| `config-data` | Generic | domains: table schema, data rows, validators, converters/exporters, generated output; proof: schema validation, converter run, diff of generated output; `guardedRoots`: the validator, converter, and generated-output roots, not the raw tables if non-engineers edit them |
| `game-design` | Generic | domains: game design doc, systems, economy and balance, content, dev handoff; proof: doc review, balance-sheet check, handoff checklist; `guardedRoots`: none unless the repo holds scripts or tools (design docs are markdown and never gated) |
| `agent-team` | none: kind-neutral, always beside a stack pack | no domains, roots or proof kinds; it adds the `test` and `escalate` roles and the role tiers (`docs/TEAM.md`, roster and "Router Rows") |

A pack that is not in this table: use Generic and report the missing row.

---

## Unity

| Aspect | Hints |
| --- | --- |
| Detection | `ProjectSettings/ProjectVersion.txt` (editor version), `Assets/`, `Packages/manifest.json`, `*.asmdef` files, `*.unity` scenes, `*.prefab` |
| Typical domains | gameplay/game logic, UI screens (uGUI or UI Toolkit — check which the live code uses), DI/composition (VContainer/Zenject `LifetimeScope`/installers), networking/API gateways, data/ScriptableObjects/master data, asset pipeline (Addressables groups, bundles), editor tooling (`Editor/` folders), platform bridge (WebGL `.jslib`, native plugins), build/CI |
| Proof commands | compile: `dotnet build` of a generated `.csproj` if the repo builds that way, else editor recompile; tests: `<Unity> -batchmode -projectPath . -runTests -testPlatform EditMode -testResults <file>` (PlayMode likewise); with mcp-unity: `recompile_scripts`, `run_tests`, `get_console_logs`; build: the repo's build script or `-executeMethod <BuildClass.Method>` |
| `guardedRoots` | `Assets/`, `Packages/`, `ProjectSettings/`, plus `scripts/`, `ci/`, `tools/` if present |
| `exemptRoots` candidates | generated code folders, imported third-party plugin folders under `Assets/Plugins/` that are never hand-edited |
| Rule globs | `"**/*.cs"`, `"Assets/**/UI/**/*.cs"`, `"Assets/**"` (asset placement), `"**/*.asmdef"`, `"**/*.uxml"`, `"**/*.uss"` |
| High-impact surfaces (`+2`) | composition root / `LifetimeScope`s, app bootstrapper, any `.asmdef`, auth, WebGL bridge classes, shared packages under `Packages/` or submodules, Addressables group settings |
| Tools | codebase-memory MCP for C#; mcp-unity (editor must have this worktree open); an asset-graph MCP if configured; Figma MCP for UI work |
| Notes | `Library/`, `Temp/`, `Logs/` are large and ignored; `.meta` GUIDs must survive renames; prefer editor-driven edits for scenes/prefabs over raw YAML |

## Cocos Creator (TypeScript)

| Aspect | Hints |
| --- | --- |
| Detection | `project.json` or `package.json` with a `creator` version field, `settings/` (project settings), `assets/` with `*.ts` + `*.meta`, `*.scene`/`*.prefab` under `assets/`, `extensions/`, `tsconfig.json` extending `./temp/tsconfig.cocos.json` |
| Typical domains | TypeScript feature/domain logic (managers, controllers), UI components and prefabs, scene/prefab/asset (UUIDs, `.meta`, bundles, loader), animation (Spine/skeletal), state/backend/economy, config/proto/generated code, render cost/performance, build/deploy (build templates, `extensions/`), platform bridge to a web wrapper |
| Proof commands | type-check: `npx tsc --noEmit` (needs `temp/` declarations from an editor open; record "not run" if absent); tests: `<pm> test` if Jest/Vitest is configured; lint: `<pm> run lint` if defined; build: Cocos Creator CLI `<CocosCreator> --project . --build "platform=web-mobile;debug=false"` or the repo's build script; preview in the editor is manual proof |
| `guardedRoots` | `assets/`, `extensions/`, `settings/`, plus `scripts/`, `tools/`, `build-templates/` if present |
| `exemptRoots` candidates | generated proto/config TypeScript output, `assets/resources/` data produced by a generator |
| Rule globs | `"assets/**/*.ts"`, `"assets/**/*.prefab"`, `"assets/**/*.scene"`, `"assets/**/*.meta"`, `"extensions/**"` |
| High-impact surfaces (`+2`) | startup scene / game director / loader, global managers and event contracts, persistence and save formats, purchase/economy, bundle membership, generated config/proto, submodule pointers |
| Tools | codebase-memory MCP; Figma MCP for UI; a browser panel or Playwright for web preview checks |
| Notes | `library/`, `temp/`, `local/`, `build/` are generated; never hand-edit `.meta` UUIDs; event register/unregister must be symmetrical |

## Node / TypeScript Backend

| Aspect | Hints |
| --- | --- |
| Detection | `package.json` with a server framework (`express`, `fastify`, `@nestjs/core`, `koa`, `hono`), `tsconfig.json`, `src/` with a server entry, ORM/schema (`prisma/schema.prisma`, `drizzle.config.*`, `typeorm`, `knex` migrations), `Dockerfile`, `docker-compose*.yml` |
| Typical domains | `http-route` (routes/controllers/handlers), service/domain logic, data & migrations, auth/identity/sessions, async jobs/queues/schedulers, external integrations (payments, storage, messaging), config/environment, API contract (OpenAPI/GraphQL schema, generated clients), container/runtime, CI/deploy, test infrastructure |
| Proof commands | type-check: `<pm> run typecheck` or `npx tsc --noEmit`; tests: `<pm> test` (Jest/Vitest/node:test), focused `<pm> test -- <pattern>`; lint: `<pm> run lint`; build: `<pm> run build`; integration/e2e: the repo's script, often against `docker compose` — starting services is a runtime change, record it |
| `guardedRoots` | `src/`, `test/` or `tests/`, `prisma/` or `migrations/`, `scripts/`, plus `apps/`/`packages/` in a workspace |
| `exemptRoots` candidates | generated clients or ORM output committed to the repo (`src/generated/`) |
| Rule globs | `"src/**/*.ts"`, `"**/*.test.ts"`, `"**/*.spec.ts"`, `"**/migrations/**"`, `"prisma/**"` |
| High-impact surfaces (`+2`) | app bootstrap and middleware order, auth/session/token code, database schema and migrations, transaction boundaries, public API shapes and generated clients, queue/event contracts, deploy config |
| Tools | codebase-memory MCP; Context7 for library docs; a database MCP if configured (read-only queries first); Docker CLI |
| Notes | never put secrets or tokens in fixtures or docs; regenerate generated clients rather than editing them |

## Go Service

| Aspect | Hints |
| --- | --- |
| Detection | `go.mod` (module path, `go` directive), `cmd/<app>/main.go`, `internal/`, `pkg/`, `Makefile`, `.golangci.yml`, `sqlc.yaml`, `migrations/`, `api/` or `*.proto`, `Dockerfile` |
| Typical domains | HTTP/gRPC handlers and routing, services, repositories and transactions, database migrations, auth/identity, async integration (queues, schedulers, object storage), environment config, API contract and generated clients, container runtime, CI/deploy, testing |
| Proof commands | build: `go build ./...`; vet: `go vet ./...`; tests: `go test ./...`, focused `go test ./internal/<pkg> -run <Test>`, `-race` where CI uses it; lint: `golangci-lint run` if configured; generators: `go generate ./...`, `sqlc generate`, `buf generate` only when the repo defines them; Make targets that wrap these |
| `guardedRoots` | `cmd/`, `internal/`, `pkg/`, `api/`, `migrations/`, `scripts/`, `deploy/` if present |
| `exemptRoots` candidates | generated code directories (`*/gen/`, sqlc or protobuf output) |
| Rule globs | `"**/*.go"`, `"**/*_test.go"`, `"**/migrations/**/*.sql"`, `"**/*.proto"`, `"Makefile"` |
| High-impact surfaces (`+2`) | `main`/server wiring and middleware chain, auth middleware, transaction ownership, schema migrations, public routes and generated clients, config loading, deploy manifests |
| Tools | codebase-memory MCP; Context7 for library docs; Docker CLI |
| Notes | do not edit generated `*.pb.go` / sqlc output; acceptance tests may need a database — record "not run" when unavailable |

## React / Next Frontend or Monorepo

| Aspect | Hints |
| --- | --- |
| Detection | `next.config.*`, `vite.config.*`, `react` in `package.json`, `app/` or `pages/` (Next), `src/main.tsx` (Vite); monorepo: `pnpm-workspace.yaml`, `turbo.json`, `nx.json`, `lerna.json`, `apps/*`, `packages/*` |
| Typical domains | pages/routes, UI components and design system, state and data fetching, auth/session/wallet, API client and contracts, shared packages (cross-app blast radius), styling/theming, build/static export/hosting, embedded runtime bridge (iframe/canvas/game wrapper), i18n, testing |
| Proof commands | type-check: `<pm> run typecheck` or `tsc --noEmit -p <app>`; lint: `<pm> run lint`; tests: `<pm> test` (Vitest/Jest); build: `<pm> run build` or `next build`; monorepo: `turbo run lint typecheck test build --filter=<pkg>...` or `nx affected -t lint test build`; e2e: Playwright/Cypress if configured; visual: a browser check is manual proof |
| `guardedRoots` | `apps/`, `packages/` (monorepo); `src/`, `app/`, `pages/`, `components/`, `lib/` (single app); plus `scripts/`, `e2e/` |
| `exemptRoots` candidates | generated API clients, copied build output from another repo (e.g. an embedded game build folder) |
| Rule globs | `"**/*.tsx"`, `"**/components/**"`, `"app/**/page.tsx"`, `"app/**/route.ts"`, `"packages/<pkg>/**"`, `"**/*.module.css"` |
| High-impact surfaces (`+2`) | shared packages consumed by several apps, auth/session/wallet flows, API client contracts, root layout/app bootstrap, build and hosting config, environment variable exposure to the client |
| Tools | codebase-memory MCP; Context7 for library docs; Figma MCP for UI; a browser panel or Playwright for real-interface checks; the hosting platform's MCP if configured (read-only) |
| Notes | client-exposed env vars (`NEXT_PUBLIC_*`, `VITE_*`) are public; a change in a shared package needs proof in each consuming app |

## Generic

| Aspect | Hints |
| --- | --- |
| Detection | none of the above matched; infer languages from file-extension counts and the build entry (`Makefile`, `justfile`, `Taskfile.yml`, `build.gradle`, `pom.xml`, `pyproject.toml`, `Cargo.toml`, `*.sln`) |
| Typical domains | feature work per top-level module, data/persistence, configuration, build/CI, documentation, review |
| Proof commands | whatever the build entry and CI workflows define; README commands only after confirming the target exists |
| `guardedRoots` | top-level directories that contain source, tests, scripts, or build/CI config |
| `exemptRoots` candidates | generated or vendored directories |
| Rule globs | by extension of the dominant languages, e.g. `"**/*.py"`, `"**/*.rs"`, `"**/*.java"` |
| High-impact surfaces (`+2`) | entry points, shared libraries, persistence schema, public interfaces, build/deploy config |
| Tools | codebase-memory MCP; Context7 for library docs |
| Notes | prefer `TODO(harness-bootstrap)` over guessing when the build is not discoverable |
