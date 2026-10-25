---
paths:
  - "docs/**/*.md"
---

# Docs Writing — the harness record under docs/

Status: **Guideline** (framework default) · Source: harness core
Checked by: not mechanically checked

Read this before writing or editing a doc under `docs/`. Markdown in the
repository is the only source of truth, so every line here is something the
next agent will believe.

## Rules

- **Concise.** Say it once, in the fewest words that stay unambiguous. Lead
  with the answer; put detail below it. One topic per file.
- **Link, do not duplicate.** Point at the file that owns a fact
  (`docs/FEATURE_INTAKE.md`, a rule, a knowledge file, a decision) instead of
  copying it. A copied fact goes stale on its own.
- **Evidence over adjectives.** Numbers, paths, commands, and their output:
  "build 52.1 to 47.5 MB", not "much smaller"; "`<command>` passed", not
  "tested thoroughly". Cite `path` or `path:line` for claims about code.
- **Dates are absolute.** `YYYY-MM-DD`, never "yesterday", "last sprint", or
  "recently". A count or version that will drift carries the date it was read,
  or better, the command that re-derives it.
- **Repository-relative paths in backticks.** Never an absolute machine path,
  a personal directory, or a local port that only one machine uses.
- **Update the stale line; do not contradict it.** When a change makes an
  existing line false, fix that line in the same change. Two docs in
  disagreement are worse than one wrong doc.
- **Use the template.** Stories, decisions, validation reports, and plans start
  from `docs/templates/`; keep their headings so tools and agents can find
  sections.
- **Logs are append-only.** `docs/INTAKE_LOG.md` and `docs/TRACE_LOG.md` are
  newest first; a correction is a new row, never an edit to an old one.
- **Decisions are superseded, never deleted.** Mark the old record
  `Superseded by NNNN` and say narrowly what is replaced.
- **No task diaries in durable docs.** What was tried first, and in what order,
  belongs in the trace. Stories and decisions record outcomes and reasons.
- **Tables for maps, prose for reasons.** A list of files, owners, or commands
  is a table; a why is a sentence.
- **Mark unknowns.** Write `TODO: <what is missing and where it would come
  from>` rather than a plausible guess.

## Exceptions

- `docs/templates/` and stack placeholder comments keep their instructional
  wording until `harness-bootstrap` fills them.
