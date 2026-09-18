#!/usr/bin/env python3
"""Select conservative PR validation; pushes and manual runs remain full."""
import json
import os
from pathlib import Path
import subprocess


def select(paths):
    selected = set()
    for path in paths:
        if path.startswith('docs/csharp-syntax/') and path.endswith('.md'):
            selected.update(('syntax', 'analyzer'))
        elif path.startswith('docs/diagnostics/') and path.endswith('.md'):
            # Diagnostic links are validated by analyzer tests.
            selected.add('analyzer')
        elif path.startswith('docs/') and path.endswith('.md'):
            # Ordinary prose documentation is independent of the executable
            # tree. Machine-checked docs were handled above.
            continue
        elif path in ('README.md', 'CONTRIBUTING.md', 'CHANGELOG.md', 'CODE_OF_CONDUCT.md', 'SECURITY.md'):
            continue
        elif path.startswith('tests/Neo.SmartContract.Analyzer.UnitTests/') and path.endswith('.cs'):
            selected.add('analyzer')
        else:
            # Includes shared config, production code, fixtures, renames out of
            # safe directories, and Markdown embedded in packages or templates.
            return {'full'}
    return selected


def plan(event_name, event):
    if event_name != 'pull_request':
        return {'full'}
    try:
        pr = event['pull_request']
        # Compare the whole PR, not just the last push. Disable rename detection
        # to include both sides of a move. Keep filenames NUL-delimited.
        output = subprocess.check_output([
            'git', 'diff', '--name-only', '--no-renames', '-z',
            f"{pr['base']['sha']}...{pr['head']['sha']}", '--',
        ])
        paths = output.decode('utf-8').split('\0')
        return select([p for p in paths if p])
    except (KeyError, UnicodeError, subprocess.CalledProcessError):
        return {'full'}


def main():
    event = json.loads(Path(os.environ['GITHUB_EVENT_PATH']).read_text())
    selected = plan(os.environ['GITHUB_EVENT_NAME'], event)
    values = {key: str(key in selected).lower() for key in ('full', 'analyzer', 'syntax')}
    values['dotnet'] = str(bool(selected)).lower()
    with open(os.environ['GITHUB_OUTPUT'], 'a') as output:
        output.writelines(f'{key}={value}\n' for key, value in values.items())
    summary = 'Selected validation: ' + (', '.join(sorted(selected)) or 'documentation-only; .NET skipped')
    print(summary)
    with open(os.environ['GITHUB_STEP_SUMMARY'], 'a') as output:
        output.write(summary + '\n')


if __name__ == '__main__':
    main()
