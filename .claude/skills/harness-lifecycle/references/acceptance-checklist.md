# Harness Acceptance Checklist

Use in `status` mode. A step is **Done** only when every line holds; **Partial**
when some do; **Not started** when none do. Cite the evidence for each line.

## 1. Init

- [ ] `.claude/harness-manifest.json` exists and parses.
- [ ] `packs` lists the repository kinds that fit (a repo with code of two kinds
      takes both packs); none selected that does not fit.
- [ ] A dry run of the installer would add nothing: every framework file for the
      recorded packs is present (ask the user to run the installer with `-Plan`
      if you cannot).
- [ ] `pendingMerges` is empty, or each entry is scheduled for `harness-bootstrap`.
- [ ] The recorded framework version has no `+dirty` suffix (else: commit the
      framework and reinstall before shipping).

## 2. Onboarding

- [ ] The router skill exists, names every skill and role in the repository,
      and its domains match the real code or docs layout.
- [ ] `.claude/knowledge/00-Master-Index.md` has a bootstrap record and lists
      every knowledge file.
- [ ] `.claude/harness.config.json` `guardedRoots` / `guardedDocRoots` exist on disk.
- [ ] `<!-- stack:` placeholders remain only where the text says they may stay.
- [ ] Every `TODO(harness-bootstrap)` names what would resolve it.
- [ ] Repository without code: marked as such, with a refresh planned.

## 3. Tuning

- [ ] A project brief exists (what the product is, users, scope, constraints,
      sibling repositories and how work flows between them).
- [ ] The repository's own conventions that are evidenced are rules with
      `paths:` that match real files.
- [ ] Repeatable procedures the team uses are skills, reachable from the router.
- [ ] Specialists the team needs beyond the framework roles exist as roles, with
      tools that exist and a handback format.
- [ ] Nothing duplicates a pack skill wholesale; overlaps are thin extensions.
- [ ] Ported material has a port map and provenance lines.

## 4. Instruction

- [ ] `CLAUDE.md` names the router and keeps the bare `@AGENTS.md` and
      `@docs/FEATURE_INTAKE.md` lines.
- [ ] No link in `CLAUDE.md`, `AGENTS.md`, `.claude/ONBOARDING.md`, or the
      knowledge index points at a missing file.
- [ ] `.gitignore` carries `/.agent-runtime/`.
- [ ] No secret, personal absolute path, `CLAUDE.local.md`, or
      `settings.local.json` is staged.
- [ ] The harness is committed (or a commit with exact paths is proposed).

## 5. Loop

- [ ] Both hooks pass `node --check` and are wired in `.claude/settings.json`.
- [ ] `docs/INTAKE_LOG.md` has rows; every normal/high-risk intake older than
      the trace window has a `docs/TRACE_LOG.md` row.
- [ ] `Learnings.md` has a recent delta, or no task has finished yet.
- [ ] Proof kinds in `docs/TEST_MATRIX.md` name commands that exist.
