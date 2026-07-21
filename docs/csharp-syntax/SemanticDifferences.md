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

NeoVM stores contract strings as UTF-8 byte strings. As a result, string `Length`, indexing, and slicing use encoded byte offsets instead of the UTF-16 code-unit offsets used by .NET. For example, `"é".Length` evaluates to `2` on NeoVM instead of `1`, and `"é"[0]` returns the first encoded byte (`195`) instead of the .NET character value (`233`). Contracts that may process non-ASCII text should choose explicit byte-oriented behavior or constrain and validate their input.

## `typeof`

`typeof(T)` does not produce a .NET `System.Type` object in Neo contract code. The compiler lowers the expression to the simple type name string, for example `typeof(int)` becomes `"Int32"`.

This behavior exists primarily so supported enum helpers such as `Enum.Parse(typeof(MyEnum), value)` and `Enum.GetNames(typeof(MyEnum))` can identify the enum type during contract compilation.

Guidance:
- Do not use `typeof` for .NET reflection or `System.Type` behavior in contracts. Reflection is outside the supported contract surface.
- Treat direct `typeof` results as Neo string semantics, not standard .NET type metadata.

## Lambdas that capture `foreach` variables

Neo currently stores captured local variables in shared contract fields. A lambda created inside a `foreach` loop therefore observes the most recently assigned iteration value when it is invoked after the loop. Standard C# creates a distinct captured variable for each iteration.

The analyzer reports `NC4063` when a lambda references its enclosing `foreach` variable. Avoid retaining that lambda across iterations; compute the value eagerly or pass it explicitly instead.
