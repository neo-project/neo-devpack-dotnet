#!/bin/bash

echo "Removing ALL Claude references from git history..."

# Clean up any existing filter-branch backups
rm -rf .git/refs/original/ 2>/dev/null

export FILTER_BRANCH_SQUELCH_WARNING=1

git filter-branch -f --msg-filter '
    # Remove all Claude-related lines more aggressively
    sed -e "/Generated with.*Claude/d" \
        -e "/Co-Authored-By.*Claude/d" \
        -e "/Co-authored-by.*Claude/d" \
        -e "/claude\.ai/d" \
        -e "/🤖.*Claude/d" \
        -e "/🚀.*Claude/d" \
        -e "s/Claude//g" \
        -e "s/claude//g" \
        -e "/^[[:space:]]*$/d" | \
    # Remove multiple consecutive blank lines
    awk "BEGIN{blank=0} /^$/{blank++; if(blank<=1) print; next} {blank=0; print}"
' --env-filter '
    # Replace Claude authorship
    if [ "$GIT_AUTHOR_EMAIL" = "noreply@anthropic.com" ]; then
        export GIT_AUTHOR_NAME="System"
        export GIT_AUTHOR_EMAIL="system@local"
    fi
    if [ "$GIT_COMMITTER_EMAIL" = "noreply@anthropic.com" ]; then
        export GIT_COMMITTER_NAME="System"  
        export GIT_COMMITTER_EMAIL="system@local"
    fi
' -- feature/contract-invocation-system

echo "✅ All Claude references removed!"
echo "Checking result..."
git log --format="%s%n%b" -5 | grep -i "claude" && echo "⚠️  Still found Claude references" || echo "✅ No Claude references found"