---
name: harness-lifecycle
description: One entry point for taking this repository's agent harness from install to team use, and for checking where it stands. Modes - status (score the five steps Init, Onboarding, Tuning, Instruction, Loop with evidence and list the next actions), onboard (map the repository and run harness-bootstrap), tune (add the repository's own knowledge, skills, rules, roles, router rows, or port them from a reference repository), ship (make the harness committable for the team), and refresh (re-check after the framework or the code changed). Use when asked "harness status", "nghiệm thu harness", "onboard this repo", "tune the harness", "add a skill/rule/role/knowledge for this repo", "port skills from <repo>", "prepare the harness for the team", or "is the harness ready".
---

# Harness Lifecycle

The harness reaches a repository in five steps. This skill tells you which step
the repository is at, then drives the next one. The specialised work stays in
the skills it calls; this skill only sequences them and checks their output.

| Step | Done by | Done when |
| --- | --- | --- |
| 1. Init | the framework installer (`setup_harness`) | every framework file is present for the recorded packs |
| 2. Onboarding | `onboard-repository`, `harness-bootstrap` | router, knowledge index, gate roots, placeholders reflect the real repository |
| 3. Tuning | this skill, mode `tune` | the repository's own domain, conventions, and procedures are captured |
| 4. Instruction | this skill, mode `ship` | the team can clone and get the same harness |
| 5. Loop | every task | intake → route → work → prove → trace → distill → harvest is running |

Hard rules, in every mode:

- Never delete or rename an existing skill, rule, role, or knowledge file. Propose it.
- Never commit, push, or open a PR unless the user asks; when asked, commit with
  `git commit -- <paths>` and only harness paths.
- Team instructions win over harness text. Do not edit a tracked team file
  outside the `## Harness` section without showing the diff first.
- Log an intake row before the first write (`docs/FEATURE_INTAKE.md`). Changes
  to hooks, permissions, or `.claude/settings.json` are high-risk.
- Evidence over memory: every claim in a report cites a file, a command output,
  or a count.

## Choose The Mode

No mode given → run `status`, show the table, and offer the next mode.

## Mode: status

Score each step against `references/acceptance-checklist.md`. For each step
report **Done / Partial / Not started**, the evidence, and the exact next
action. Use these checks; run them, do not guess:

- Init: `.claude/harness-manifest.json` exists; `packs` recorded; any
  `pendingMerges`; framework version recorded (a `+dirty` version means the
  framework was uncommitted when installed — say so).
- Onboarding: `grep -rn "<!-- stack:" CLAUDE.md AGENTS.md docs .claude` (count,
  and which remain by design); router skill exists and names every skill and
  role (list unreached names with
  `for d in .claude/skills/*/ .claude/agents/*.md; do n=$(basename "$d" .md); grep -q "$n" .claude/skills/*-task-router/SKILL.md .claude/skills/route-*/SKILL.md 2>/dev/null || echo "$n"; done`
  — `basename` strips the trailing `/` that `ls` prints on Windows Git Bash);
  `.claude/knowledge/00-Master-Index.md` has a bootstrap record;
  `.claude/harness.config.json` roots exist on disk; `TODO(harness-bootstrap)` count.
- Tuning: project skills, rules, roles, knowledge files beyond what the
  framework installed (compare with the manifest `files`/`seeds`); a project
  brief exists; every project skill is reachable from the router.
- Instruction: `git ls-files` for the harness paths; `.gitignore` covers
  `/.agent-runtime/`; `CLAUDE.md` names the router; nothing in the harness
  points at a path that does not exist (check links in `CLAUDE.md`, `AGENTS.md`,
  `.claude/ONBOARDING.md`).
- Loop: `docs/INTAKE_LOG.md` rows, `docs/TRACE_LOG.md` rows, intakes without a
  trace, last `Learnings.md` delta, hooks pass `node --check`. With the
  `agent-team` pack, also count escalations per area
  (`grep -o "escalate: [^|]*" docs/TRACE_LOG.md`) and spawned `test` runs. Two
  or more escalations in one area point at missing knowledge or a missing
  skill there.

Output: the five-row table, then "To finish, do these N things" in order, each
with its lane. Stop there unless the user picks one.

## Mode: onboard

1. If `harness-manifest.json` lists `pendingMerges` or `<!-- stack:` remains,
   or the knowledge index has no bootstrap record: run `harness-bootstrap`.
2. If the repository has code and no evidence capsule yet: run
   `onboard-repository` first, so bootstrap writes knowledge from evidence.
3. Repository with no code yet: say so. Bootstrap fills what it can and leaves
   `TODO(harness-bootstrap)`; plan a `refresh` once the first code lands.
4. Finish with `status`.

## Mode: tune

Tuning captures what only this repository knows. Follow
`references/tuning-playbook.md`; it has the procedure and a template for each
artifact. In short:

1. **Brief first.** If `.claude/knowledge/` has no product or project brief,
   interview the owner (or read the given docs) and write one from
   `references/templates/project-brief.md`. Every later artifact cites it.
2. **Decide the artifact** with the playbook's decision table: knowledge (facts),
   rule (a convention checked on matching files), skill (a repeatable
   procedure), role (a specialist with its own tools and handback), router row
   (a new kind of task).
3. **Pack first.** If an installed pack already ships a skill or role for it,
   extend that skill's project section or add a thin project skill that names
   the pack skill and adds only what differs. Never fork a pack skill wholesale.
4. **Porting from a reference repository**: write the port map first
   (`references/templates/port-map.md`: class A copy as-is, B copy then strip,
   C do not port), then port one component at a time with its tests and a
   provenance line. The reference repository is read-only.
5. **Wire it**: add the router row or role mapping, list it in the knowledge
   index's Related Docs, and add proof rows to `docs/TEST_MATRIX.md` when the
   artifact defines a check.
6. **Check it**: the artifact's `paths:` globs match real files (rules); the
   skill's commands exist (skills); the role's tools exist (roles). Then trace.

## Mode: ship

Follow `references/ship-checklist.md`:

1. Resolve every `pendingMerges` entry (via `harness-bootstrap`) and every
   dangling reference found by `status`.
2. Ensure `.gitignore` carries `/.agent-runtime/` (append; never reorder team lines).
3. Make sure no secret, absolute personal path, or machine-local file is in
   the harness (`CLAUDE.local.md`, `settings.local.json`, `.agent-runtime/`
   stay out).
4. Propose the commit: list the exact paths, one subject line, and a short PR
   body (what the team gets, how to use the router). Commit only on request,
   with `git commit -- <paths>`.

## Mode: refresh

Run after a framework reinstall (`[UPDATE]`, `[UPSTREAM]`, `[MERGE-NEEDED]`
lines) or after the code moved: `harness-bootstrap` in refresh mode, then
`status`. Upstream seed changes are merged by keeping the repository's filled
content and taking the framework's new structure.

## Handback

Always end with the five-step table (current state) and the single next action.
