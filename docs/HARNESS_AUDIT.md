# Harness Audit

A periodic check for drift between what the harness *says* and what is actually
in the repository. Run it when the harness feels stale, before a batch of harness
docs merges to the integration branch, or when the same confusion keeps
recurring.

It is a checklist, not a command. Every check below is a shell one-liner whose
output you read and judge — there is no score to game and no tool to install.

## Checks

### 1. Broken doc links

Markdown links pointing at files that no longer exist. This is the check that
catches the most real rot.

Each link must resolve relative to **its own file's directory**, so the check
walks files rather than globbing paths:

```bash
find docs .claude -name '*.md' | while read -r f; do
  d=$(dirname "$f")
  grep -oE '\]\([^)]+\.md[^)]*\)' "$f" 2>/dev/null \
    | sed -E 's/^\]\(//; s/[)#].*$//' \
    | while read -r link; do
        case "$link" in http*|/*) continue;; esac
        [ -e "$d/$link" ] || echo "$f -> $link"
      done
done
```

<!-- stack: if this repository keeps imported or generated markdown that should not be link-checked (vendored docs, exported wikis), add `-not -path '<dir>/*'` exclusions to the find above. -->

Fix by repointing or removing the link. Two distinct faults show up here: a link
to a file that never existed or was deleted, and a link written relative to
`docs/` from a file that lives in `docs/plans/` — the second resolves fine in
some renderers and not on disk, which is why this check tests the filesystem.

### 2. Stories with no proof row

Every normal/high-risk story should appear in `docs/TEST_MATRIX.md`. Stories are
either a folder or a single file, so check both:

```bash
ls -d docs/stories/*/ 2>/dev/null | while read -r d; do
  name=$(basename "$d")
  grep -q "$name" docs/TEST_MATRIX.md || echo "NO MATRIX ROW: $name"
done
for f in docs/stories/*.md; do
  name=$(basename "$f" .md)
  [ "$name" = "README" ] && continue
  grep -q "$name" docs/TEST_MATRIX.md || echo "NO MATRIX ROW: $name"
done
```

A story with no row is either unfinished work nobody tracked, or a story that
should be closed out.

### 3. Stale in-flight rows

`docs/TEST_MATRIX.md` rows sitting in `planned` or `in_progress`. Read the list
and ask, per row, whether it is genuinely in flight or quietly abandoned.

```bash
sed '/^## Status Values/q' docs/TEST_MATRIX.md | grep -nE '\| (planned|in.progress) \|'
```

The `sed` stops before the Status Values table, whose own rows would otherwise
match.

### 4. Backlog items with no outcome

Items marked `implemented` that never recorded what actually happened, per
`docs/IMPROVEMENT_PROTOCOL.md`.

The `## Items` guard matters — without it this matches the template at the top of
the file, whose Status line lists every value including `implemented`.

```bash
awk '/^## Items/{go=1} !go{next}
     /^### Title/{getline; getline; t=$0}
     /^### Status/{getline; getline; if ($0=="implemented") print "NO OUTCOME: " t}' \
  docs/HARNESS_BACKLOG.md
```

Cross-check each name it prints against an `### Actual Outcome` section. Missing
one means the improvement loop never closed.

### 5. Untraced intakes

Normal and high-risk intakes whose stamp no trace row cites. The Stop hook
reminds once per intake; this check catches the ones that slipped through.

```bash
grep -E '^\| [0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2} \|.*\| (normal|high-risk) \|' docs/INTAKE_LOG.md \
  | sed -E 's/^\| ([0-9-]+ [0-9:]+) .*/\1/' \
  | while read -r stamp; do
      grep -qF "$stamp" docs/TRACE_LOG.md || echo "UNTRACED: $stamp"
    done
```

### 6. Tools that no longer resolve

Every row in `docs/TOOL_REGISTRY.md` should still be reachable by its documented
presence check. Walk the table and run each check. An `mcp`/`skill` row that is
absent this session is normal (see the degrade ladder); a `cli` row whose script
was deleted is drift.

<!-- stack: add the one-liner that lists this repository's registered cli tools or verification scripts (for example `ls <scripts-dir>/`), once docs/TOOL_REGISTRY.md has cli rows. -->

### 7. Rules that match nothing

Every file in `.claude/rules/` declares `paths:` globs. A rule whose globs match
no tracked file never loads and silently stops governing anything.

```bash
for r in .claude/rules/*.md; do
  awk 'NR==1 && /^---$/ {fm=1; next}
       fm && /^---$/ {exit}
       fm && /^paths:/ {p=1; next}
       fm && p && /^[[:space:]]*-/ {sub(/^[[:space:]]*-[[:space:]]*/, ""); gsub(/["\047]/, ""); print; next}
       fm && p && !/^[[:space:]]/ {p=0}' "$r" \
    | while read -r g; do
        [ -n "$(git ls-files -- ":(glob)$g" | head -1)" ] || echo "$r: no match for $g"
      done
done
```

This reads `paths:` written as a YAML list (one `- glob` per line), the form the
rules in this harness use.

### 8. Decisions that contradict the code

The expensive one, and the reason the others exist. Spot-check the most recent
`docs/decisions/` records against reality — does the rule still hold? A decision
the codebase quietly stopped following should be superseded by a new record, not
left standing.

```bash
ls -t docs/decisions/*.md | head -5
```

## Reading The Result

There is no numeric score. Judge by category:

| Signal | Meaning |
| --- | --- |
| Checks 1-2 clean | Docs and stories are internally consistent. |
| Check 3 growing over runs | Work is being started and not closed out. |
| Check 4 non-empty | The improvement loop is open at one end — items land, outcomes never get recorded. |
| Check 5 non-empty | Work finished without a trace; its friction is lost. |
| Check 7 non-empty | A rule exists on paper and governs nothing. |
| Check 8 finds a contradiction | Highest severity. A stale decision actively misleads the next agent. |

Anything that recurs across two audits belongs in `docs/HARNESS_BACKLOG.md` per
`docs/IMPROVEMENT_PROTOCOL.md`.
