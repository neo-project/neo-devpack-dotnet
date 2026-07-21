### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------------------------------------------------
NC4060  | Method   | Error    | BitOperationsUsageAnalyzer
NC4062  | Syntax   | Error    | CollectionSpreadUsageAnalyzer
NC2010  | Syntax   | Error    | ArrayRangeUsageAnalyzer
NC4061  | Syntax   | Error    | ExtendedPropertyPatternAnalyzer

### Removed Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------------------------------------------------
NC4032  | Usage    | Error    | Superseded by NC4036 for unsupported query expressions

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|------------------------------------------------
NC4010  | Usage        | Error        | Usage        | Warning      | All reported by-reference forms are unsupported by the compiler
