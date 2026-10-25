---
name: epic-planning
description: "Turn one feature's spec into a Jira epic and its Unity Tasks: read the spec from the project's spec, sheet, chat, design and Jira sources, preview the breakdown as a local page, then create or update the epic and Tasks in the active sprint in the team's description format. Use when asked to plan, break down or ticket a feature epic (e.g. a tutorial, an item shop), or to fill an existing epic with tickets. Needs the Project configuration section filled. User-requested only; it writes to Jira, and every write sits behind an owner gate."
---

<!-- Ported from cricket-city-unity .claude/skills/epic-planning/SKILL.md @ 8da403c97; stripped of Cricket specifics. -->

# Epic Planning

You read a feature from wherever it is written down and turn it into one Jira
epic with the Unity Tasks the team will pick up. Worked examples: see
**Project configuration**.

- **Jira owns the scope.** A ticket must stand on its own: no "see Slack", no
  "see the plan". Harness records (intake, stories, decisions) stay in the repo.
- **Planning is not execution.** No code, no branch. The work is done later,
  one Task at a time, through `unity-task-router`.
- **Unity side only.** Ticket Unity work. Server, Art and QA tickets belong to
  their teams; never create, edit or re-parent them. A backend gap is a
  `Depends on:` line in our ticket plus a `Blocks` link from the server team's
  ticket; a link is not an edit of theirs.
- **The owner decides; you prepare.** Every Jira write sits behind a gate. A
  plan in chat is not approval; wait for "create it".
- **User-requested only.** Never start this skill on your own initiative.

Several epics at once (a whole new game, a release program) is not this skill:
plan each epic separately, after the owner has agreed how they split.

## Project configuration

A game created from this template fills this section once. **If a value the
current phase needs is still `TODO(project)`, stop and ask the owner for it**;
never guess a project key, a field id or a person.

| Key | Value |
| --- | --- |
| Jira site and `cloudId` | `TODO(project): site URL; cloudId from getAccessibleAtlassianResources` |
| Jira project key | `TODO(project): key, e.g. ABC` |
| Board and sprint | `TODO(project): board name; the active sprint is read from any issue in project = <KEY> AND sprint in openSprints()` |
| Sprint field | `TODO(project): customfield id that holds the sprint (read with view: evidence)` |
| Start date field | `TODO(project): customfield id, or "none"` |
| Team field | `TODO(project): customfield id and the team's option id, or "none"` |
| Component for Unity work | `TODO(project): component name, and whether it auto-assigns a lead` |
| Team label | `TODO(project): the label every Unity ticket carries` |
| Issue types | `TODO(project): confirm the Epic / Story / Task type names` |
| Estimates | `TODO(project): Original Estimate in time (hours per day) or story points` |
| Summary formats | `TODO(project): epic summary shape; ticket summary shape, e.g. "[<Feature>] - <what>"` |
| Fix Version use | `TODO(project): does an epic carry the release's Fix Version` |
| Spec source | `TODO(project): the doc tool (Outline, Confluence, Notion…), the collection, and the page to start from` |
| Backend design | `TODO(project): where BE designs live, e.g. "<Feature> (BE)" pages` |
| Config sheets | `TODO(project): Google Sheets ids and tabs for prices, rewards, config; or "none"` |
| Chat | `TODO(project): Slack channel per feature, e.g. #<game>-feature-<name>` |
| Design | `TODO(project): Figma file key(s)` |
| API contract | `TODO(project): swagger URL or request collection the api-codegen skill reads` |
| People | `TODO(project): the owner; how assignees are looked up (email domain)` |
| Branch name | `TODO(project): branch pattern for a ticket, e.g. dev/<you>/<KEY>-<n>-<slug>` |
| Worked examples | `TODO(project): two epics whose shape new epics copy` |
| Description format | `TODO(project): confirm or replace the default in **Descriptions** below` |

## Phase 1: Research (no gate)

Read everything before slicing anything. A source that does not exist is skipped, not reported.

| Source | How | What to take |
| --- | --- | --- |
| Spec | The configured doc tool's MCP or connector: search the feature, fetch the page, read its comments. Start from the configured start page | Rules, flows, screens. Read comments: BE answers live there. An archived page is superseded |
| BE design | The configured backend design location, usually linked from the spec | Endpoints and states. Documented is not verified |
| Sheets | `google-workspace` skill, if installed; use the derived project's documented data source | Prices, rewards, and configuration values the tasks must quote; the neutral template has no business master data |
| Chat | Slack MCP on the configured channel: read the channel and every thread | Changes after the spec. The newest dated message wins |
| Figma | Figma MCP on the linked node | Which screens exist. No link means the Task asks Art for one |
| Jira | `searchJiraIssuesUsingJql`: `parent = <epic>`, `text ~ "<feature>"`, the server team's ticket | The existing epic and children (adopt, never duplicate), the BE ticket key |
| Contract | The configured API contract, read-only (the `api-codegen` skill knows its sources) | GET only. Never spend, claim or progress anything on a dev server to learn a shape |
| Code | `codebase-memory`, then grep | What exists, where the feature lands, what to remove |
| Images | Attachments in the spec (download signed attachment URLs promptly; they expire), plus any Figma screenshots the owner pastes | The screens each UI Task builds |

Save images in `.local-plans/<EPIC-KEY>-<slug>/`, named for what they show
(`friends-popup.png`), never in the repo. List `.local-plans/` in
`.git/info/exclude` the first time. The owner's Figma screenshots win over
older spec images of the same screen.

## Phase 2: Breakdown (Gate 1)

Slice into Tasks a teammate can finish and QA can check:

- **One Task per spec section or per technical seam.** A tutorial: one Task per
  section plus the shared pieces (progress service, highlight, dialogue, script
  data, debug tools).
- **Story or Task**, decided by who can check the result. Show each ticket's
  type, and whether it goes to QA or Done, at Gate 1. No Sub-tasks.

  | Type | Use when | After merge |
  | --- | --- | --- |
  | Story | A player can see or do it, and QA tests it | QA |
  | Task | Asset, data, configuration, internal work | QA when QA can check it on dev; Done for internal tools and plumbing |

- A service or contract Task comes first; UI Tasks name it in `Depends on:`.
- Removing old behaviour is its own Task (`Remove the old tutorial`).
- Release batches (an umbrella epic for a release): a new feature gets its own
  epic, linked `Relates` to the umbrella; every ticket in the batch carries the
  batch label. Tweaks go straight under the umbrella.

**Gate 1: the owner checks before anything exists in Jira.** Show exactly what
will be written:

- The epic: summary, labels, links and its full description.
- One table row per ticket (`Type | Summary | Depends on | Priority | Start date | Estimate`), then
  each Task's full description.
- Tickets that already exist and will be adopted or edited, with their keys.
- The images each ticket will carry.

More than a handful of tickets: write it as a local HTML page in
`.local-plans/<EPIC-KEY>-<slug>/index.html` (images beside it, so it opens
offline; never publish it to the cloud) and open it in the browser. Then stop.
When the owner asks for changes, apply them and show only what changed; repeat
until they say "create it".

## Phase 3: Create in Jira (Gate 2)

Only on "create it", and only the text the owner approved; anything new needs
another look. Check `parent = <epic>` again first, and update the epic if it
exists (an epic QA already made is filled, not replaced).

| Field | Epic | Task / Story |
| --- | --- | --- |
| Summary | the configured epic summary shape (the feature name) | the configured ticket summary shape |
| Parent | — | the epic |
| Labels | the team label (+ batch label) | the team label (+ batch label) |
| Components | — | the configured Unity component (required) |
| Team field | the configured team option | same |
| Start date field | first day of work | the day the Task starts |
| Sprint field | — | the active sprint id: read it from any `sprint in openSprints()` issue with `view: evidence` |
| Priority | from the schedule | service/contract Tasks one step higher |
| Assignee | empty | whoever the owner names, looked up by **email** with `lookupJiraAccountId` (a name search can miss); otherwise empty. If the component auto-assigns its lead and nobody was named, clear it with `editJiraIssue` after creating |
| Fix Version | the release, when the epic ships in one (`getJiraProjectVersions`) | same as the epic |
| Links | `Relates` to the umbrella and to overlapping epics | `Blocks` from each ticket it depends on; `Relates` to the bug or ticket it replaces |

Skip a field the configuration marks "none". **No Server or Art tickets.** Set
the estimate in the configured unit: a placeholder the assignee adjusts later.

**Images.** A Task that builds or changes a screen carries that screen; the epic
carries the main screens. Logic-only Tasks get no image.

1. `uploadAttachmentToJiraIssue` (Atlassian `executeWrite`) with `filePath`
   returns an `uploadCommand`. Run it exactly as given; its JSON `data.id` is the
   `fileId`. Upload each file once.
2. `editJiraIssue` with `contentFormat: html`, `inlineFileId`,
   `inlineFileCollection` and `inlineFileName` embeds it at the end of the
   description. The same `fileId` can be embedded in several issues; each gets
   its own attachment.

The upload is a `curl` to `api.media.atlassian.com`; it needs a Bash permission
for that host. If it is denied, stop and say which permission is missing.

Create the epic, then the tickets in parallel batches, then the edits and links
(`createJiraIssueLink`, `linkType: Blocks`).
Adopting a ticket sets the same Parent, Labels, Components, Sprint and Fix Version as a new one.
**Verify with one count:** `parent = <epic> AND component = <Unity component>
AND labels = <team label> AND sprint in openSprints()` equals the number of
tickets you made plus any you adopted. Report the number and the keys.

If a Jira write is denied, stop and tell the owner which write was blocked;
they can allow it in `/permissions` and ask you to continue.

## Descriptions

Short, plain, written for a teammate. No schedule, owners, dated history,
child lists or BE question lists.

Default format until the project confirms or replaces it (Project
configuration, "Description format"):

**Epic**:

```markdown
<One or two sentences: what the player gets.>

**<Part name>** — only when the feature has parts (Tutorial 1, Tutorial 2; the items)
* <the flow in one line each>

**How it works**
* <rules a reader needs: who owns what, what is off, what happens on quit>

**Notes**
* <only what a reader would trip on>

**Links:** [Spec](…) · [BE design](…) · [Figma](…)
```

**Task or Story**:

```markdown
**What**
<One sentence the player or the client would notice.>

**How it works**
* <3-5 bullets: the flow, the endpoint, where values come from>

**Done when**
* <checks QA can run on dev; EditMode tests when there is logic>

**Technical**   — optional, for service/plumbing Tasks
<Where it lives and which existing slice it copies.>

**Notes**       — optional
* <a replaced ticket, a value that is easy to get wrong>

**Depends on:** <the Task or the server team's ticket>   — omit when none
**Links:** [<Spec section>](…)
```

Jira's markdown turns `_` into italics: put names like `#game-feature-x` and
`first_time_user_experience` in backticks.

## Phase 4: Park a Plan (optional)

When the owner asks to save the plan, or the technical detail is too long for
Jira, write `docs/plans/active/<KEY>-<n>-<slug>.md` from
`docs/templates/exec-plan.md`: sources with read dates, the BE contract, code
touch points and a code sketch per Task. Link it from the epic's **Links**
line. Planning creates no story packet; when a ticket is worked on, intake
(`docs/FEATURE_INTAKE.md`) decides whether one is owed.

## Executing a Task Later

When asked to work on a ticket from such an epic: read it and its epic in Jira,
the plan if there is one, and the configured chat since the plan's read date.
Then `unity-task-router`, intake, and a branch named by the configured pattern.
Move the ticket's status only when the owner asks.
