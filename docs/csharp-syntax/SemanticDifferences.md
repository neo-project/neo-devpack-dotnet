# Semantic Differences

This page records intentional or currently accepted behavior differences between Neo C# contract compilation and standard .NET semantics.

## `bool.TryParse`

The compiler currently preserves an extended NeoVM-oriented bool parsing policy.

Accepted true literals:
- `"true"`
- `"TRUE"`
- `"True"`
- `"t"`
- `"T"`
- `"1"`
- `"yes"`
- `"YES"`
- `"y"`
- `"Y"`

Accepted false literals:
- `"false"`
- `"FALSE"`
- `"False"`
- `"f"`
- `"F"`
- `"0"`
- `"no"`
- `"NO"`
- `"n"`
- `"N"`

Notes:
- This intentionally diverges from .NET `bool.TryParse`, which only accepts `true` and `false` case-insensitively.
- Whitespace-padded inputs such as `" true "` are still rejected.
