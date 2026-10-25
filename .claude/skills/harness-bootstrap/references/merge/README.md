# Merge Snippets

The harness ships seed files (`CLAUDE.md`, `.claude/settings.json`,
`.gitattributes`) for repositories that do not have them yet. When a target
repository already has one of these files, do NOT copy the seed over it. Merge
the snippet below by hand, keep every existing line, and show the user the diff
before writing.

| Snippet | Target | Merge rule |
| --- | --- | --- |
| `claude-harness-section.md` | `CLAUDE.md` | Append the section |
| `settings-hooks.json` | `.claude/settings.json` | Add hook entries under `hooks` |
| `settings-permissions.json` | `.claude/settings.json` | Add missing entries to `permissions.allow` |
| `gitattributes-lines.txt` | `.gitattributes` | Append missing lines |

## `claude-harness-section.md` into `CLAUDE.md`

1. Search the existing file for a `## Harness` heading. If one exists, compare
   it with the snippet and update only the lines that differ; keep any
   project-specific sentences the team added.
2. Otherwise append the snippet at the end of the file, after a blank line.
   Leave every other section untouched.
3. Keep `@AGENTS.md` and `@docs/FEATURE_INTAKE.md` as bare lines, each on its
   own line, never inside backticks or a code block; wrapping them disables the
   import.
4. If the file already imports either path with a bare `@` line, do not add a
   second import of it.
5. Replace "the project's task router skill" with the router's real name once
   `harness-bootstrap` has created it.

## `settings-hooks.json` into `.claude/settings.json`

The snippet is the value of the `hooks` key. Merge it as data, not text:

1. Parse the existing file. If it does not parse (comments, trailing commas),
   stop and ask the user rather than rewriting it.
2. If there is no `hooks` key, add the snippet as `hooks`.
3. Otherwise, for each event (`PreToolUse`, `Stop`), append the snippet's
   matcher group to the existing array. Skip it when an entry with the same
   `command` is already present anywhere under that event.
4. Never remove or reorder existing keys: `permissions`, `env`,
   `enabledPlugins`, other hook events, and existing hook entries all stay.
5. Write back with the file's existing indentation, then re-parse the result to
   prove it is valid JSON.

If the team does not want hooks in the tracked settings file, the same merge
can target `.claude/settings.local.json` instead.

## `settings-permissions.json` into `.claude/settings.json`

The snippet is the value of the `permissions` key: a conservative list of
read-only commands and codebase-memory read tools that run without a prompt.
Merge it as data, in the same pass as the hooks:

1. Parse the existing file (same rule as above: stop and ask if it does not
   parse).
2. If there is no `permissions` key, add the snippet as `permissions`. If there
   is one with no `allow` array, add the snippet's `allow`.
3. Otherwise append each snippet entry to `permissions.allow` when the exact
   string is not already present. Keep the existing order and entries.
4. Never touch `permissions.deny`, `permissions.ask`, or
   `defaultMode`. If an entry from the snippet also appears in `deny` or `ask`,
   do not add it to `allow`; report the conflict and keep the team's choice.
5. Write back and re-parse, as above.

The list grants reads only. Never add a write, install, network, or
shell-wide entry to it during a merge; propose those to the user instead.

## `gitattributes-lines.txt` into `.gitattributes`

1. For each line, check whether the path already has an attribute line.
2. If the path is absent, append the line (with the comment block from the
   seed `.gitattributes` above the first one you add).
3. If the path already has a different `merge=` driver, keep the existing line
   and tell the user; do not override a team's merge driver silently.
4. Leave every other line untouched.
