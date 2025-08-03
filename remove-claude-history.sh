#!/bin/bash

echo "Removing Claude references from git history..."
echo "This will rewrite commit history and change commit hashes."
echo ""

# Create backup
echo "Creating backup tag..."
git tag backup-before-claude-removal 2>/dev/null || echo "Backup tag already exists"

# Use git filter-branch to remove Claude references from commit messages and authorship
echo "Rewriting git history to remove Claude references..."

export FILTER_BRANCH_SQUELCH_WARNING=1

git filter-branch -f --msg-filter '
    # Remove Claude bot attribution lines
    sed -e "/🤖 Generated with \[Claude Code\]/d" \
        -e "/Co-Authored-By: Claude <noreply@anthropic\.com>/d" \
        -e "/Co-authored-by: Claude <noreply@anthropic\.com>/d" \
        -e "/^[[:space:]]*$/N;/^\n$/d"
' --env-filter '
    # Keep original author unless it is Claude
    if [ "$GIT_AUTHOR_EMAIL" = "noreply@anthropic.com" ]; then
        export GIT_AUTHOR_NAME="System"
        export GIT_AUTHOR_EMAIL="noreply@example.com"
    fi
    if [ "$GIT_COMMITTER_EMAIL" = "noreply@anthropic.com" ]; then
        export GIT_COMMITTER_NAME="System"
        export GIT_COMMITTER_EMAIL="noreply@example.com"
    fi
' -- feature/contract-invocation-system

echo ""
echo "✅ Claude references removed from git history!"
echo ""
echo "Next steps:"
echo "1. Review changes: git log --oneline -10"
echo "2. Force push: git push --force-with-lease origin feature/contract-invocation-system"
echo "3. To undo: git reset --hard backup-before-claude-removal"