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

## `char` and `string` character helpers

The compiler supports a contract-oriented subset of `char` and `string` helper methods. Character classification and casing helpers are ASCII-oriented rather than full .NET Unicode category operations.

Examples:
- `char.IsLetter`, `char.IsUpper`, and `char.IsLower` check the `A-Z` and `a-z` ranges.
- `char.ToUpper`, `char.ToLower`, `char.ToUpperInvariant`, and `char.ToLowerInvariant` convert ASCII letters and leave other characters unchanged.
- `string.ToUpper` and `string.ToLower` apply the same ASCII-oriented casing behavior to each character.
- `char.GetNumericValue` returns integer values for `0-9` and `-1` for other characters. It does not return `double` values or implement the full .NET Unicode numeric category behavior.

This keeps contract execution deterministic and avoids culture-dependent behavior.

## `typeof`

`typeof(T)` does not produce a .NET `System.Type` object in Neo contract code. The compiler lowers the expression to the simple type name string, for example `typeof(int)` becomes `"Int32"`.

This behavior exists primarily so supported enum helpers such as `Enum.Parse(typeof(MyEnum), value)` and `Enum.GetNames(typeof(MyEnum))` can identify the enum type during contract compilation.

Guidance:
- Do not use `typeof` for .NET reflection or `System.Type` behavior in contracts. Reflection is outside the supported contract surface.
- Treat direct `typeof` results as Neo string semantics, not standard .NET type metadata.
