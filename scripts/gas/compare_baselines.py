#!/usr/bin/env python3
"""Compare the GAS baselines of the working tree with those of a base revision.

Usage: compare_baselines.py <base-sha> [output.json]

Writes a Markdown table to stdout (suitable for GITHUB_STEP_SUMMARY) and an optional JSON file.
Read-only: it only runs `git ls-tree` / `git show` and never writes to the repository.
"""
import json
import subprocess
import sys
from pathlib import Path

ROOTS = ('tests/Neo.Compiler.CSharp.UnitTests/GasBaselines', 'tests/Neo.SmartContract.Framework.UnitTests/GasBaselines')


def git(*args):
	return subprocess.check_output(('git',) + args)


def base_measurements(sha):
	result = {}
	for root in ROOTS:
		names = git('ls-tree', '-r', '--name-only', '-z', sha, '--', root).decode('utf-8').split('\0')
		for name in filter(None, names):
			if name.endswith('.json'):
				result.update(json.loads(git('show', f'{sha}:{name}')))
	return result


def head_measurements():
	result = {}
	for root in ROOTS:
		for path in sorted(Path(root).rglob('*.json')):
			result.update(json.loads(path.read_text(encoding='utf-8')))
	return result


def compare(base, head):
	rows = []
	for key in sorted(set(base) | set(head)):
		old, new = base.get(key), head.get(key)
		if old == new:
			continue
		status = 'added' if old is None else 'removed' if new is None else 'increased' if new > old else 'decreased'
		rows.append({'test': key, 'base': old, 'head': new, 'status': status,
					 'delta': (new or 0) - (old or 0)})
	return rows


def main():
	if len(sys.argv) < 2:
		print(__doc__)
		return 2
	base, head = base_measurements(sys.argv[1]), head_measurements()
	rows = compare(base, head)
	totals = {s: sum(1 for r in rows if r['status'] == s) for s in ('added', 'removed', 'increased', 'decreased')}
	totals['unchanged'] = len(set(base) & set(head)) - totals['increased'] - totals['decreased']
	totals['gas_delta'] = sum(r['delta'] for r in rows)
	if len(sys.argv) > 2:
		Path(sys.argv[2]).write_text(json.dumps({'totals': totals, 'changes': rows}, indent=2, sort_keys=True), encoding='utf-8')
	print('## GAS changes against the target branch\n')
	if not rows:
		print('No GAS differences.')
		return 0
	print('| Status | Count |\n|---|---|')
	for key in ('added', 'removed', 'increased', 'decreased', 'unchanged'):
		print(f'| {key} | {totals[key]} |')
	print(f"\nNet GAS delta: {totals['gas_delta']}\n")
	print('| Test | Base | PR | Delta |\n|---|---:|---:|---:|')
	for r in rows[:200]:
		name = r['test'].replace('|', '\\|')
		print(f"| `{name}` | {'' if r['base'] is None else r['base']} | {'' if r['head'] is None else r['head']} | {r['delta']:+d} |")
	if len(rows) > 200:
		print(f'\n{len(rows) - 200} more changes in the gas-baseline-diff artifact.')
	return 0


if __name__ == '__main__':
	sys.exit(main())
