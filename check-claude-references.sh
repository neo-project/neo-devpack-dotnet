#!/bin/bash

echo "Checking for Claude references in git history..."
echo "=============================================="
echo ""

echo "Commits with Claude in the message:"
echo "-----------------------------------"
git log --oneline --all | grep -i "claude" || echo "No commits found with 'claude' in the subject"

echo ""
echo "Detailed Claude references in commit bodies:"
echo "-------------------------------------------"
git log --format="%h %s" --grep="[Cc]laude" --all -i | while read commit; do
    hash=$(echo $commit | cut -d' ' -f1)
    subject=$(echo $commit | cut -d' ' -f2-)
    echo ""
    echo "Commit: $hash - $subject"
    git show -s --format=%b $hash | grep -i "claude" | sed 's/^/  > /'
done

echo ""
echo "Summary:"
echo "--------"
count=$(git log --grep="[Cc]laude" --all -i | grep -c "commit")
echo "Total commits with Claude references: $count"

echo ""
echo "To remove these references, you can run:"
echo "  ./remove-claude-from-history.sh    (uses git filter-branch)"
echo "  ./remove-claude-filter-repo.py     (uses git-filter-repo - recommended)"