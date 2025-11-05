#!/usr/bin/env python3
"""Regenerate docs/csharp-syntax/UnsupportedFeatures.md from the versioned checklists."""

from __future__ import annotations

import argparse
import re
from dataclasses import dataclass
from pathlib import Path


@dataclass(slots=True)
class UnsupportedEntry:
    version_heading: str
    items: list[tuple[str, str]]


def load_ordered_checklists(repo_root: Path) -> list[Path]:
    index_path = repo_root / "docs" / "CSharpSyntaxCheckList.md"
    ordered: list[Path] = []
    docs_dir = repo_root / "docs"
    syntax_dir = docs_dir / "csharp-syntax"
    if index_path.exists():
        index_text = index_path.read_text(encoding="utf-8")
        for match in re.finditer(r"- \[[^\]]+\]\((csharp-syntax/[\w\-.]+)\)", index_text):
            candidate = docs_dir / match.group(1)
            if candidate.exists():
                ordered.append(candidate)
    seen = {path.resolve() for path in ordered}
    for path in sorted(syntax_dir.glob("csharp-*.md")):
        resolved = path.resolve()
        if resolved not in seen:
            ordered.append(path)
            seen.add(resolved)
    return ordered


def parse_checklists(paths: list[Path]) -> list[UnsupportedEntry]:
    entries: list[UnsupportedEntry] = []
    for path in paths:
        text = path.read_text(encoding="utf-8")
        heading_match = re.search(r"^#\s+(.+)$", text, flags=re.MULTILINE)
        if not heading_match:
            continue
        heading = heading_match.group(1)
        items: list[tuple[str, str]] = []
        section_pattern = re.compile(
            r"^###\s+([a-z0-9_\-]+)\s+-\s+([^\n]+)\n\nStatus:\s*([A-Za-z]+)",
            flags=re.MULTILINE,
        )
        for feature_id, title, status in section_pattern.findall(text):
            if status.strip().lower() == "unsupported":
                items.append((feature_id, title.strip()))
        if items:
            entries.append(UnsupportedEntry(heading, items))
    return entries


def render(entries: list[UnsupportedEntry]) -> str:
    lines: list[str] = [
        "# Unsupported C# Features in Neo Compiler",
        "",
        "The versioned syntax checklists flag every feature the Neo compiler currently rejects. "
        "This page is generated from those checklists so the status remains accurate.",
        "",
        "## Summary by Version",
        "",
    ]
    for entry in entries:
        lines.append(f"- **{entry.version_heading}**  ")
        for feature_id, title in entry.items:
            lines.append(f"  - {title} (`{feature_id}`)")
        lines.append("")
    lines.extend(
        [
            "## Next Actions",
            "",
            "1. Confirm with the compiler team which gaps are expected versus candidates for future support.",
            "2. File GitHub issues or backlog items for each unsupported feature that should be implemented.",
            "3. Update the version checklists and rerun this script whenever support status changes.",
            "",
        ]
    )
    return "\n".join(lines)


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--check",
        action="store_true",
        help="Verify the generated content matches the current file without writing changes.",
    )
    args = parser.parse_args()

    repo_root = Path(__file__).resolve().parents[1]
    checklist_paths = load_ordered_checklists(repo_root)
    entries = parse_checklists(checklist_paths)
    output_path = repo_root / "docs" / "csharp-syntax" / "UnsupportedFeatures.md"
    rendered = render(entries)

    if args.check:
        if not output_path.exists():
            raise SystemExit("UnsupportedFeatures.md is missing; run the script without --check to create it.")
        current = output_path.read_text(encoding="utf-8")
        if current != rendered:
            raise SystemExit(
                "UnsupportedFeatures.md is out of date. Run 'python scripts/update_unsupported_features.py' to regenerate it."
            )
        return

    output_path.write_text(rendered, encoding="utf-8")


if __name__ == "__main__":
    main()
