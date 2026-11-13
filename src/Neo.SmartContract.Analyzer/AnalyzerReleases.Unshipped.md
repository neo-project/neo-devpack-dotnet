### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|---------------------------------
NC4065  | Usage    | Info     | StringConcatenationInLoopAnalyzer
NC4066  | Syntax   | Error    | CollectionWithElementUsageAnalyzer
NC4067  | Syntax   | Error    | UnionDeclarationUsageAnalyzer

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|------
NC4027  | Usage        | Error        | Usage        | Warning      | Typed catch clauses change behavior because Neo contracts catch System.Exception only.
