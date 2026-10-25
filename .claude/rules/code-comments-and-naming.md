---
paths:
  - "**/*.cs"
  - "**/*.ts"
  - "**/*.tsx"
  - "**/*.js"
  - "**/*.jsx"
  - "**/*.mjs"
  - "**/*.cjs"
  - "**/*.go"
  - "**/*.py"
  - "**/*.java"
  - "**/*.kt"
  - "**/*.swift"
  - "**/*.rs"
  - "**/*.c"
  - "**/*.cpp"
  - "**/*.h"
  - "**/*.hpp"
  - "**/*.lua"
  - "**/*.sh"
  - "**/*.ps1"
---

# Code Comments and Naming — any language

Status: **Guideline** (framework default) · Source: harness core
Checked by: not mechanically checked

Read this before writing or editing source code. A stack rule in
`.claude/rules/`, a linter or formatter configuration, or the owner's
instruction overrides any line here; the owner may promote it to a gate.

## Comments

- **Comment why, not what.** The default for a function, a field, or a new
  file is no comment. A clean name over clean code is already the explanation.
- **The test: is the thing being explained visible in this file?** Its steps,
  name, parameters, and return are visible: no comment. An ordering another
  file depends on, a side effect outside the function, a coupling the type
  system does not show, a value chosen by an external system: one line naming
  the constraint.
- **No doc comment that restates the signature.** Public API docs follow the
  stack's tooling convention; do not add them to private code by habit.
- **The story of a bug is not a code comment.** One line naming the constraint
  is the budget; the causal chain goes in the trace or the decision record.
- **Rationale for a design goes in `docs/decisions/`**, linked from the code by
  file name only when the code would otherwise read as a mistake.
- **No section banners or test-phase labels** (`// Arrange`, `// Act`) unless the
  file already uses them.

## Naming

- **Match the family, do not invent.** When a type already has a family of
  methods for a job (`bind*`, `init*`, `on*`, `apply*`, `handle*`), a new member
  takes the family's shape. Match the nearest convention in the file, then the
  module, then the repository.
- **Do not rename existing names while passing through.** A rename is its own
  change, with its own diff.
- **Never name a module, namespace, or package after a symbol the language or
  framework imports implicitly** (a logging, time, or random facility). It
  shadows that symbol for every file underneath, and the break appears in a
  file you did not touch.

## Guards

- **Prefer guard clauses to nesting.** Return or fail early on the invalid case;
  keep the main path at the lowest indentation.
- **Do not defend against your own contract.** A required dependency or wired
  reference that is missing is a bug that must fail loudly at first use, not
  degrade silently. Check only what can legitimately be absent (an optional
  dependency, external input, a lookup that may miss).
- **Do not ship a validator or defensive check nobody asked for.** If the code
  seems to need one, propose it as a separate option.

## Dead Code

- **Delete, do not comment out.** Version control keeps the old code.
- **No unused imports, parameters, variables, or private members** left behind
  by the change.
- **No unreachable branches or flags that nothing sets.** A guard on a
  condition that can never be true is dead by construction.
- **Removing something is not a licence to explain it.** The reason belongs in
  the trace or the commit, not in a comment where the code used to be.

## Exceptions

- A stack rule or a configured tool that states otherwise wins for its paths.
