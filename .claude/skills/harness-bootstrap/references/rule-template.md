# Rule Template

A rule is a short, file-scoped convention in `.claude/rules/<topic>.md`.

## How Rules Load

Claude Code reads `.claude/rules/*.md` on its own:

- A rule **with** `paths:` frontmatter is loaded when the agent reads a file
  matching one of its globs. It costs nothing until that kind of file is
  touched, so it is the right home for language- or folder-specific
  conventions.
- A rule **without** `paths:` loads in every session, like `CLAUDE.md`. Use that
  only for a rule that truly applies to all work; prefer `docs/LEARNINGS-SUMMARY.md`
  or `CLAUDE.md` for those.

Globs are matched against repository-relative paths. Quote every glob in YAML
(`"**/*.ts"`); an unquoted leading `*` is a YAML alias and breaks the file.

## Where A Rule's Content May Come From

Only from evidence, cited at the top of the rule:

- lint, format, and type-check configuration (`eslint` / `biome` / `prettier`
  configs, `tsconfig` strictness flags, `.golangci.yml`, `.editorconfig`,
  analyzer rule sets);
- an existing skill, doc, contributing guide, or decision that states the
  convention;
- an owner's explicit correction recorded in `Learnings.md` or
  `docs/LEARNINGS-SUMMARY.md`.

A pattern merely observed in the code is not authority. It can be written as a
**guideline** that cites two or three examples, never as a gate.

## Gate vs Guideline

Mark every rule file, and every item where they mix:

- **Gate** — the owner stated it as mandatory, or a configured tool enforces it.
  A violation blocks review. Name the enforcing command when one exists, so the
  agent runs it instead of eyeballing.
- **Guideline** — a preferred default. A deviation is an informational review
  note, never a blocker.

Do not restate what a linter already enforces line by line; say "enforced by
`<command>`; run it" and spend the words on what the tool cannot see.

## Template

```markdown
---
paths:
  - "<glob>"
  - "<glob>"
---

# <Topic> — <one-line scope>

Status: **Gate** | **Guideline** · Source: `<path to config, doc, or decision>`
Checked by: `<command from docs/TOOL_REGISTRY.md>` | not mechanically checked

Read this before <creating / editing> <kind of file>.

## Rules

- **<Rule>.** <One or two lines. Why, if the reason is not obvious.>
  Example: `<path>` · Counter-example to avoid: `<path>` (if one exists).

## Exceptions

- <Authorized exception, cited> — or "None stated."
```

Keep one topic per file and under ~80 lines. Name files by topic
(`go-errors.md`, `react-components.md`, `migrations.md`), not by stack.

## Example Globs

| Stack or area | Globs |
| --- | --- |
| TypeScript (any) | `"**/*.ts"`, `"**/*.tsx"` |
| TypeScript tests | `"**/*.test.ts"`, `"**/*.spec.ts"`, `"**/__tests__/**"` |
| Cocos Creator scripts | `"assets/**/*.ts"` |
| Cocos Creator serialized assets | `"assets/**/*.prefab"`, `"assets/**/*.scene"`, `"assets/**/*.meta"` |
| Node service source | `"src/**/*.ts"`, `"apps/<service>/src/**/*.ts"` |
| React components | `"**/*.tsx"`, `"**/components/**"` |
| Next.js routes | `"app/**/page.tsx"`, `"app/**/route.ts"`, `"pages/**"` |
| Monorepo package | `"packages/<pkg>/**"`, `"apps/<app>/**"` |
| Styles | `"**/*.css"`, `"**/*.scss"`, `"**/*.module.css"` |
| Go | `"**/*.go"` |
| Go tests | `"**/*_test.go"` |
| C# | `"**/*.cs"` |
| Python | `"**/*.py"` |
| SQL migrations | `"**/migrations/**/*.sql"`, `"prisma/**"` |
| Protobuf / schemas | `"**/*.proto"`, `"**/openapi*.{yaml,yml,json}"` |
| CI workflows | `".github/workflows/*.yml"` |
| Containers | `"**/Dockerfile*"`, `"**/docker-compose*.yml"`, `"**/compose*.yaml"` |
| Build scripts | `"Makefile"`, `"**/*.mk"`, `"scripts/**"` |

Pick the narrowest globs that cover the convention; a rule scoped to
`"**/*"` should not have `paths:` at all.
