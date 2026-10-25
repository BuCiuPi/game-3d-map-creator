# Glossary

Harness terms, alphabetical. Product and domain terms belong in the knowledge
base (`.claude/knowledge/`), not here.

## Agent

An AI coding collaborator working in the repository. In Claude Code, also a
subagent definition in `.claude/agents/<name>.md` that plays one **role**.

## Backlog Item

A proposed harness improvement in `docs/HARNESS_BACKLOG.md`, carrying its
predicted impact before implementation and its actual outcome at close. See
`docs/IMPROVEMENT_PROTOCOL.md`.

## Bootstrap

The `harness-bootstrap` skill run after install: it detects the stack, creates
the router, writes the first knowledge and rules, and fills the stack
placeholders from evidence. Its refresh mode catches later drift.

## Capability

The purpose string a workflow step looks a tool up by, in kebab case. Steps name
the capability, not the tool, so they survive a tool change. Registered in
`docs/TOOL_REGISTRY.md`.

## Core

The stack-neutral part of the harness installed into every repository: the
process docs, templates, hooks, and harness skills. This repository retired the
core roles in favour of cricket-city-unity's (decision `0007`).

## Decision Record

A durable `docs/decisions/NNNN-kebab-title.md` explaining why an architecture,
contract, policy, or product choice was made. Superseded, never deleted.

## Degrade Ladder

How work proceeds with partial tooling: an unregistered capability is skipped
cleanly; a registered tool that fails its presence check makes the run degraded,
and the proof must say so. See `docs/TOOL_REGISTRY.md`.

## Distill

Compressing `Learnings.md` into `docs/LEARNINGS-SUMMARY.md` and promoting
recurring lessons into rules, skills, or knowledge. The `learning-distillation`
skill does it.

## Domain

An area of the codebase the router recognizes by its signals (paths, modules,
nouns), each with a route and a target skill or agent.

## Feature Intake

The classification that turns a prompt into tiny, normal, or high-risk work
before implementation. Defined in `docs/FEATURE_INTAKE.md`; recorded in
`docs/INTAKE_LOG.md`.

## Friction

Whatever made a task harder than it should have been: a missing rule, a stale
doc, a contradiction, a repeated manual step. Recorded per task in the Friction
column of `docs/TRACE_LOG.md`; recurring friction becomes a backlog item.

## Gate

A rule marked mandatory by the owner or enforced by a tool; a violation blocks
review. Contrast **guideline**. Also a hook that stops an agent, such as the
**intake gate**.

## Guideline

A preferred default. A deviation is an informational review note, never a
blocker.

## Harness

The repository-level operating system that tells humans and agents how to turn
intent into safe, proven changes: markdown docs, rules, skills, roles, knowledge,
and a few hooks.

## Harness Delta

A change to docs, rules, conventions, knowledge, or decisions that makes the
next task easier. One of the two outputs a task may produce.

## Harvest

Proposing generic improvements found in this repository back to the shared
framework. The `harvest` skill writes a proposal under `.agent-runtime/harvest/`;
a human carries it over.

## Intake Gate

The `PreToolUse` hook `.claude/hooks/intake-gate.js`. It blocks the first code
write when `docs/INTAKE_LOG.md` holds no recent entry, fails open on
infrastructure errors, and never gates markdown.

## Knowledge

Curated, code-derived reference and recipe files in `.claude/knowledge/`,
entered through `00-Master-Index.md`. Load the one file the task needs.

## Lane

The risk class set at intake: **tiny**, **normal**, or **high-risk**. It
decides what proof is owed, which artifacts are written, and whether a human
must confirm before code.

## Maturity Level

A rung H0 to H5 in `docs/HARNESS_MATURITY.md`, judged on artifacts you can open,
not on mechanisms that merely exist.

## Pack

A stack-specific layer installed on top of core: roles (implementers and a stack
reviewer), rules, skills, knowledge seeds, and the hints bootstrap uses. A
repository lists its installed packs in `.claude/harness-manifest.json`.

## Product Delta

A product-facing change: code, tests, assets, data, configuration. The other
output a task may produce.

## Proof

Evidence that a change does what it claims: a command that was run and its
result, or an explicit deferred note naming who will run it. Proof kinds and
their commands live in `docs/TEST_MATRIX.md`.

## Retrieval Trigger

A condition that tells an agent to read more context mid-task, such as touching
a contract or crossing a module boundary. Defined in `docs/CONTEXT_RULES.md`.

## Role

One of the four kinds of step in a task: **design** (built-in `Plan` on Fable),
**implement** (inline or `unity-game-logic-developer`), **review**
(`unity-code-reviewer`), **document** (`unity-documenter`). The role, not the
task's size, picks the agent and model (decision `0002`).

## Router

The project task router skill (`.claude/skills/<project>-task-router/`). It
runs at the start of every task, classifies domain and lane, scores the task,
and names the route and role sequence.

## Rule

A short convention file in `.claude/rules/` with `paths:` frontmatter. Claude
Code loads it when a matching file is touched; other agents read it by hand.

## Score

The router's 1 to 10 number: lane baseline plus adjusters. It decides whether to
spawn, whether design comes first, and what records are owed. It never picks a
model or an effort level.

## Skill

A packaged instruction set in `.claude/skills/<name>/SKILL.md`, loaded inline
when its description matches the task.

## Story Packet

A story file or folder under `docs/stories/` describing one feature's contract,
design notes, and validation expectations.

## Trace

The record of what a task actually did: intake stamp, lane, outcome, proof,
changed paths, and friction. Lives in `docs/TRACE_LOG.md`; format in
`docs/TRACE_SPEC.md`.

## Trace Depth Tier

How much a trace owes per lane: one row for normal work, a row plus a section
for high-risk work, and for tiny work only when it hit friction.
