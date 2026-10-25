# Ship Checklist

Make the harness something the whole team gets on clone.

1. Run `status`; fix every dangling link and resolve `pendingMerges`.
2. `.gitignore`: append `/.agent-runtime/` when missing. Keep team lines and order.
3. Keep out of the commit:
   - `.agent-runtime/` (logs, backups, staged merges);
   - `CLAUDE.local.md`, `.claude/settings.local.json` (personal);
   - any secret, token, or personal absolute path (grep `D:\\`, `C:\\Users`,
     `/Users/`, `token`, `secret`, `password` in the harness paths).
4. Harness paths to commit (only those that exist):
   `CLAUDE.md`, `AGENTS.md`, `Learnings.md`, `.gitattributes`, `.gitignore`,
   `.claude/agents/`, `.claude/rules/`, `.claude/skills/`, `.claude/knowledge/`,
   `.claude/hooks/`, `.claude/settings.json`, `.claude/harness.config.json`,
   `.claude/harness-manifest.json`, `.claude/ONBOARDING.md`, `docs/` harness files
   and the docs structure the packs added.
5. Check what is staged before committing: `git status --short`,
   `git diff --cached --stat`. A file you did not expect means stop and ask.
6. Commit only on request: `git commit -- <paths>` with one subject line, for
   example `chore: add agent harness`.
7. PR body (short): what the team gets (router, roles, skills, gates), how to
   start (open in Claude Code, accept workspace trust once, describe the task),
   and what is still TODO.
