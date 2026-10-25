# Tuning Playbook

Tuning adds what only this repository knows. Every artifact cites evidence:
a file, a doc the owner gave, or the owner's answer.

## Which Artifact

| You have | Write | Where | Template |
| --- | --- | --- | --- |
| Facts about the product, domain, architecture, data | knowledge file | `.claude/knowledge/NN-name.md`, listed in `00-Master-Index.md` | `templates/knowledge.md` |
| What the product is and how it relates to sibling repos | project brief (knowledge 01) | `.claude/knowledge/01-product-brief.md` | `templates/project-brief.md` |
| A convention to apply whenever certain files are edited | rule | `.claude/rules/<area>.md` with `paths:` | core `harness-bootstrap/references/rule-template.md` |
| A procedure the team repeats (multi-step, with proof) | skill | `.claude/skills/<project>-<name>/SKILL.md` | `templates/skill.md` |
| A specialist with its own scope, tools, and handback | role | `.claude/agents/<project>-<role>.md` | `templates/role.md` |
| A new kind of task the router does not route | router row | the router skill's routing table | — |
| Material to bring over from another repository | port map, then ports | `.claude/knowledge/NN-port-map-from-<repo>.md` | `templates/port-map.md` |
| A rule that must never be broken again | mechanical check | via the `encode-invariant` skill | — |

Prefer the smallest artifact: a knowledge line over a rule, a rule over a
skill, a skill over a role. A role is worth it only when the work needs its
own tool set or a separate reviewer.

## Naming

- Project skills and roles carry the project prefix (`<project>-…`) so they
  never collide with framework or pack names on reinstall.
- Knowledge files are numbered `01-`, `02-` in reading order; pack seeds keep
  their unnumbered `<stack>-…` names.

## Pack First

Before writing a skill or role, list the installed packs' skills and roles
(`.claude/harness-manifest.json` `packs`, then `.claude/skills/`,
`.claude/agents/`).

- The pack covers it → add the project facts to that skill's "In this
  repository" section or a `<!-- stack:` slot, or write a thin project skill
  that says "follow `<pack-skill>`, plus these differences".
- The pack covers it badly for every repository of this kind → also note it
  for `harvest`, so the framework improves.
- Nothing covers it → write a project skill.

## Porting From A Reference Repository

1. The reference repository is read-only. Record its path and commit.
2. Write the port map first: every candidate component with class
   **A** copy as-is, **B** copy then strip (list what to strip: product ids,
   integrations, coupling), **C** do not port (and why).
3. Port one component at a time with its tests. A port without its tests is
   not done.
4. Replace the reference's product identity with this repository's, from
   config or the project brief.
5. Add a provenance line: `Ported from <repo> <path> @ <commit>`.
6. Prove it (build, tests) and add the proof row to `docs/TEST_MATRIX.md`.
7. Turn the porting steps into a project skill (`<project>-port-from-<repo>`)
   if more than one component will be ported.

## Wiring Checklist (every artifact)

- [ ] Router: new skill or role reachable from a row or the role table.
- [ ] Knowledge index: new knowledge file listed; skills and rules under Related Docs.
- [ ] `docs/TEST_MATRIX.md`: proof row when the artifact defines a check.
- [ ] `CLAUDE.md` / `AGENTS.md`: only when the artifact changes how every task starts.
- [ ] Intake row before, trace row after.

## Owner Interview (when facts are missing)

Ask only what the repository cannot answer, in one batch:

1. What is this product, for whom, and what is in and out of scope now?
2. Which sibling repositories exist, and what flows between them (specs,
   contracts, config, builds)?
3. Which areas are high-risk (money, accounts, data, live operations)?
4. What does "done" mean here: which checks must pass before merge?
5. Which conventions does the team already follow that are not written down?
6. Is there a reference repository to port from, and what must not be copied?
