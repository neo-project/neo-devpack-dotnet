#!/bin/bash

# This script removes Claude references from git history
# It will rewrite history, so use with caution!

echo "This will rewrite git history to remove Claude references."
echo "This is a destructive operation that will change commit hashes."
echo "Make sure you have a backup and coordinate with your team."
echo ""
read -p "Are you sure you want to continue? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Operation cancelled."
    exit 1
fi

# Create a backup branch
echo "Creating backup branch..."
git branch backup-before-claude-removal

# Use git filter-branch to remove Claude references from commit messages
echo "Rewriting commit messages..."
git filter-branch -f --msg-filter '
    # Remove Claude bot attribution lines
    sed -e "/🤖 Generated with \[Claude Code\]/d" \
        -e "/Co-Authored-By: Claude <noreply@anthropic\.com>/d" \
        -e "/Co-authored-by: Claude <noreply@anthropic\.com>/d" \
        -e "s/Claude//g" \
        -e "s/claude//g" \
        -e "/^[[:space:]]*$/d" | \
    # Remove trailing empty lines
    awk "/^$/ {empty++; next} {for(i=0;i<empty;i++) print \"\"; empty=0; print}"
' -- --all

echo ""
echo "History rewriting complete!"
echo ""
echo "IMPORTANT NEXT STEPS:"
echo "1. Review the changes with: git log --oneline -20"
echo "2. If everything looks good, force push with: git push --force-with-lease origin <branch-name>"
echo "3. If you need to undo, run: git reset --hard backup-before-claude-removal"
echo ""
echo "WARNING: Force pushing will rewrite history on the remote."
echo "Coordinate with your team before force pushing!"