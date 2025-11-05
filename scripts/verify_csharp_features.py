#!/usr/bin/env python3
"""Compare local C# syntax checklists against Microsoft's "What's new in C#" feature lists."""

from __future__ import annotations

import argparse
import html
import re
import sys
from dataclasses import dataclass
from html.parser import HTMLParser
from pathlib import Path
from typing import Dict, List, Set

import urllib.error
import urllib.request

BASE_URL = "https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-{}"

@dataclass
class FeaturePage:
    version: int
    headings: List[str]

IGNORED_ANCHOR_PREFIXES = (
    "c-version",
    "ms-",
    "see-also",
    "embedded-video",
    "feedback",
)


class HeadingExtractor(HTMLParser):
    def __init__(self) -> None:
        super().__init__()
        self.in_h2 = False
        self.current_id: str | None = None
        self.text_parts: List[str] = []
        self.headings: List[tuple[str, str]] = []

    def handle_starttag(self, tag: str, attrs: list[tuple[str, str | None]]) -> None:
        if tag == "h2":
            self.in_h2 = True
            self.current_id = None
            for name, value in attrs:
                if name == "id" and value:
                    self.current_id = value
        elif self.in_h2 and tag == "code":
            self.text_parts.append("`")

    def handle_endtag(self, tag: str) -> None:
        if tag == "h2" and self.in_h2:
            text = html.unescape("".join(self.text_parts)).strip()
            if self.current_id and text:
                self.headings.append((self.current_id, text))
            self.in_h2 = False
            self.current_id = None
            self.text_parts.clear()
        elif self.in_h2 and tag == "code":
            self.text_parts.append("`")

    def handle_data(self, data: str) -> None:
        if self.in_h2:
            self.text_parts.append(data)


def fetch_feature_page(version: int) -> FeaturePage:
    url = BASE_URL.format(version)
    try:
        with urllib.request.urlopen(url) as response:
            content = response.read().decode("utf-8", errors="replace")
    except urllib.error.HTTPError as exc:
        if exc.code == 404:
            return FeaturePage(version, [])
        raise
    parser = HeadingExtractor()
    parser.feed(content)
    headings = []
    for anchor, title in parser.headings:
        normalized_title = " ".join(title.split())
        if normalized_title.lower() in {"in this article", "see also"}:
            continue
        if anchor.startswith(IGNORED_ANCHOR_PREFIXES):
            continue
        headings.append(anchor)
    return FeaturePage(version, headings)


def parse_local_ids(path: Path) -> Set[str]:
    ids: Set[str] = set()
    pattern = re.compile(r"^###\s+([a-z0-9_\-]+)\s+-", re.MULTILINE)
    for match in pattern.finditer(path.read_text(encoding="utf-8")):
        ids.add(match.group(1))
    return ids


def anchor_to_id(anchor: str) -> str:
    return anchor.replace("-", "_")


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("versions", nargs="*", type=int, default=list(range(1, 14)), help="C# versions to verify (default 1-13)")
    args = parser.parse_args()

    repo_root = Path(__file__).resolve().parents[1]
    syntax_dir = repo_root / "docs" / "csharp-syntax"

    missing: Dict[int, Set[str]] = {}

    for version in args.versions:
        page = fetch_feature_page(version)
        anchor_ids = {anchor_to_id(anchor) for anchor in page.headings}
        local_file = syntax_dir / f"csharp-{version}.md"
        if not local_file.exists():
            print(f"Missing checklist file: {local_file}", file=sys.stderr)
            continue
        local_ids = parse_local_ids(local_file)
        missing_ids = anchor_ids - local_ids
        if missing_ids:
            missing[version] = missing_ids

    if not missing:
        print("All checklists contain entries for every feature heading.")
        return

    print("Missing features:")
    for version, ids in sorted(missing.items()):
        print(f"  C# {version}: {', '.join(sorted(ids))}")
    sys.exit(1)


if __name__ == "__main__":
    main()
