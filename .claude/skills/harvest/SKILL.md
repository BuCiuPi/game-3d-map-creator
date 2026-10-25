---
name: harvest
description: Contribute improvements from this repository back to the shared harness framework. Finds skills, rules, agents, knowledge, harness doc fixes, and recurring TRACE_LOG friction that are generic to this stack or to every repository, strips project specifics, and writes a proposal with the generalized files and their target framework path (core or a pack) under `.agent-runtime/harvest/<date>/`. Use when the user asks to harvest, upstream, contribute back, or share harness improvements with the framework or other repositories, or after an audit finds generic fixes. Never edits the framework; a human carries the proposal over. Not for compressing Learnings.md (use learning-distillation).
---

# Harvest

Every repository tunes its copy of the harness. Some of that tuning is true
only here; some would help every repository of the same kind, or every
repository at all. This skill finds the second kind, generalizes it, and
packages it as a proposal for the shared framework.

## Hard Rules

- **Never edit the framework.** Write only under `.agent-runtime/harvest/`.
  Do not open, clone, or write to the framework repository even if it is on
  disk; the user carries the proposal over and runs the framework's own
  validation.
- **Never edit this repository's harness either.** Harvest reads; it does not
  tidy. A stale file found on the way is reported, not fixed. The one write
  outside the proposal is an optional `docs/TRACE_LOG.md` row (step 7).
- **No commits, no pushes, no installs.**
- **Generic or nothing.** A candidate that loses its meaning once project
  names, paths, and numbers are stripped stays here. Say so and move on.
- **Evidence travels with the proposal.** Each item cites the local file, the
  trace rows, or the decision that shows it worked here.

## Procedure

### 1. Read the install state

- `.claude/harness-manifest.json`: `framework` (the version installed),
  `files` (each framework-shipped path and the hash it was installed with),
  `merged`, `pendingMerges`, and `packs` (the installed packs, when present).
- `git check-ignore -q .agent-runtime && echo ignored`. If `.agent-runtime/` is
  not ignored, tell the user before writing; do not edit ignore files.

Paths in `files` are framework-owned; everything else under `.claude/` and
`docs/` is this repository's own.

### 2. Find candidates

| Source | Candidate | How to find it |
| --- | --- | --- |
| `.claude/skills/*/` not in `files` | a skill made here | list skills; drop the project task router (it is always project-specific) |
| `.claude/rules/*.md` not in `files` | a rule made here | list rules |
| `.claude/agents/*.md` not in `files` | a role made here | list agents |
| `.claude/knowledge/*.md` | a stack-generic lesson inside project knowledge | read the index; look for "how this stack behaves" content rather than "how this product works" |
| framework files whose hash differs from `files` | a local fix to a shipped file | hash each listed path (`sha256sum`, or `Get-FileHash -Algorithm SHA256`) and compare; files with filled stack placeholders differ by design, so look only at changes outside the filled placeholders |
| `docs/TRACE_LOG.md` Friction column | friction that recurs (two or more rows) and names the harness: a doc, a template, a hook, a skill, the router | read the column; group rows by the component they name |
| `docs/HARNESS_BACKLOG.md` | an improvement whose recorded outcome says it helped | read closed items |
| `docs/LEARNINGS-SUMMARY.md`, `Learnings.md` | a lesson about the harness or the stack, not the product | read the sections |

Skip anything that is product knowledge, a one-off, or not yet proven in use.

### 3. Classify each candidate

Ask: *would this be true in another repository of the same kind with a
different product?* Then *in a repository of any kind?*

| Verdict | Target in the framework |
| --- | --- |
| true for any repository | `harness/core/<same relative path>` |
| true for this stack only | `harness/packs/<pack>/<same relative path>`, where `<pack>` is the installed pack for the stack (from `packs` in the manifest, or the README pack table) |
| true only here | not harvested; state why |

A fix to a framework-shipped file targets the path the file came from: core
unless the file belongs to a pack.

### 4. Generalize

For each harvested item, produce the framework version:

- Strip project, product, team, and company names; absolute paths; ticket keys;
  decision numbers and slugs from this repository; incident dates; and counts
  that describe only this repository. Keep the lesson the incident taught.
- Replace each stack-specific fact the framework cannot know with a stack
  placeholder comment: `<!-- stack` followed by a colon, one sentence naming
  what belongs there and where `harness-bootstrap` finds the evidence, then
  `-->`.
- Follow the pack contract when the target is a pack: agent and skill names
  carry the pack's stack prefix, rule names carry the language or area prefix,
  and no path may collide with a core path or another pack's.
- Keep required frontmatter: skills need `name` and `description`; agents need
  `name`, `description` (when to use and when not to), and `tools`; rules need
  quoted `paths:` globs.
- A rule stays a guideline unless a configured tool enforces it; the framework
  cannot know a repository owner's mandate.
- UTF-8 without BOM, LF line endings.

### 5. Write the proposal

Under `.agent-runtime/harvest/<YYYY-MM-DD>/`:

```text
PROPOSAL.md                       the summary below
files/harness/core/...            new or replacement files, mirroring the framework layout
files/harness/packs/<pack>/...
patches/<name>.diff               for a fix to an existing framework file: a unified diff
                                  against the installed version, with the hunk explained
```

`PROPOSAL.md`:

```markdown
# Harvest Proposal: <YYYY-MM-DD>

Source framework version: <manifest framework>
Installed packs: <list>

| # | Kind | Local source | Framework target | New / patch | Why generic | Stripped | Evidence |
| --- | --- | --- | --- | --- | --- | --- | --- |

## Not Harvested
| Candidate | Reason |
| --- | --- |

## Carry-Over Steps
1. Copy `files/` into the framework checkout, keeping paths.
2. Apply `patches/` to the named files.
3. Run the framework's validation and the pack contract checks.
4. Reinstall into this repository to replace the local copies with the
   framework versions.
```

### 6. Self-check

Before reporting, grep everything under the proposal's `files/` and `patches/`
for:

- this repository's name, its package or module name, top-level namespaces,
  the README title, the remote organization, and any product nouns from the
  knowledge index;
- absolute paths (a drive letter followed by `:\`, `/Users/`, `/home/`);
- decision references (`docs/decisions/` followed by digits) and ticket keys.

Any hit is fixed or the item is moved to Not Harvested.

### 7. Report

Tell the user: the proposal path, the items harvested with their targets, the
items rejected and why, any self-check hits that were fixed, and the carry-over
steps. Harvest writes only under the ignored `.agent-runtime/` folder, so it
owes no intake row; log a `docs/TRACE_LOG.md` row only if the run hit friction.
