#!/usr/bin/env python3
"""
Remove Claude references from git history using git-filter-repo
This is the modern, recommended approach for rewriting git history.
"""

import subprocess
import sys
import re

def clean_commit_message(message):
    """Remove Claude references from commit message."""
    # Remove Claude bot attribution lines
    message = re.sub(r'🤖 Generated with \[Claude Code\]\(https://claude\.ai/code\)\n*', '', message)
    message = re.sub(r'Co-Authored-By: Claude <noreply@anthropic\.com>\n*', '', message)
    message = re.sub(r'Co-authored-by: Claude <noreply@anthropic\.com>\n*', '', message)
    
    # Remove the word Claude/claude (optional - comment out if too aggressive)
    # message = re.sub(r'\bClaude\b', '', message, flags=re.IGNORECASE)
    
    # Clean up extra newlines
    message = re.sub(r'\n\n+', '\n\n', message)
    message = message.strip() + '\n'
    
    return message

def main():
    print("This script will remove Claude references from git history.")
    print("Prerequisites: You need 'git-filter-repo' installed.")
    print("Install with: pip install git-filter-repo")
    print()
    
    # Check if git-filter-repo is available
    try:
        subprocess.run(['git', 'filter-repo', '--version'], 
                      capture_output=True, check=True)
    except (subprocess.CalledProcessError, FileNotFoundError):
        print("ERROR: git-filter-repo is not installed.")
        print("Install it with: pip install git-filter-repo")
        sys.exit(1)
    
    response = input("This will rewrite history. Continue? (yes/no): ")
    if response.lower() != 'yes':
        print("Cancelled.")
        sys.exit(0)
    
    # Create backup tag
    print("Creating backup tag...")
    subprocess.run(['git', 'tag', 'backup-before-claude-removal'], 
                  capture_output=True)
    
    # Create the callback script for git-filter-repo
    callback_script = '''
import re

def clean_message(message):
    # Remove Claude bot attribution lines
    message = re.sub(r'🤖 Generated with \\[Claude Code\\]\\(https://claude\\.ai/code\\)\\n*', '', message)
    message = re.sub(r'Co-Authored-By: Claude <noreply@anthropic\\.com>\\n*', '', message)
    message = re.sub(r'Co-authored-by: Claude <noreply@anthropic\\.com>\\n*', '', message)
    
    # Clean up extra newlines
    message = re.sub(r'\\n\\n+', '\\n\\n', message)
    return message.strip()

def message_callback(message):
    return clean_message(message)
'''
    
    # Write callback to temp file
    with open('.git-filter-repo-callback.py', 'w') as f:
        f.write(callback_script)
    
    # Run git-filter-repo
    print("Rewriting history...")
    try:
        subprocess.run([
            'git', 'filter-repo',
            '--message-callback', 
            'return clean_message(message)',
            '--force'
        ], check=True)
    except subprocess.CalledProcessError as e:
        print(f"Error during filtering: {e}")
        sys.exit(1)
    finally:
        # Clean up temp file
        import os
        if os.path.exists('.git-filter-repo-callback.py'):
            os.remove('.git-filter-repo-callback.py')
    
    print()
    print("✅ History rewriting complete!")
    print()
    print("NEXT STEPS:")
    print("1. Review changes: git log --oneline -20")
    print("2. Check a specific commit: git show <commit-hash>")
    print("3. If good, force push: git push --force-with-lease origin <branch>")
    print("4. To undo: git reset --hard backup-before-claude-removal")
    print()
    print("⚠️  WARNING: Coordinate with your team before force pushing!")

if __name__ == '__main__':
    main()