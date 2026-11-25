# Semantic Differences

This page records intentional or currently accepted behavior differences between Neo C# contract compilation and standard .NET semantics.

<a id="booltryparse"></a>

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

## Checked and unchecked integer operations

NeoVM integers are arbitrary precision, so the compiler applies the declared
C# operand width when lowering fixed-width integer operations. In an
`unchecked` context, operations that overflow their declared width wrap as
they do in C#. In a `checked` context, the compiler emits an overflow fault.

The signed edge case `int.MinValue / -1`, `long.MinValue / -1`, and their
remainder equivalents fault in both checked and unchecked contexts, matching
the .NET behavior for these operations. Compound assignments and `DivRem`
use the same rule. See [#1983](https://github.com/neo-project/neo-devpack-dotnet/pull/1983)
and [#1984](https://github.com/neo-project/neo-devpack-dotnet/pull/1984).

Unsigned bitwise complement is constrained to the declared unsigned width;
for example, `~(uint)0` produces `uint.MaxValue` instead of the signed VM
value `-1`. See [#2004](https://github.com/neo-project/neo-devpack-dotnet/pull/2004).

## Narrowing numeric conversions

Checked narrowing conversions validate the destination range before converting.
Unchecked conversions retain the destination type's low-order bits, including
for nullable operands when the source value is non-null. A null nullable
operand remains null when the destination is nullable. Converting a null
nullable value to a non-nullable destination faults. See
[#2001](https://github.com/neo-project/neo-devpack-dotnet/pull/2001).

## `typeof`

`typeof(T)` does not produce a .NET `System.Type` object in Neo contract code. The compiler lowers the expression to the simple type name string, for example `typeof(int)` becomes `"Int32"`.

This behavior exists primarily so supported enum helpers such as `Enum.Parse(typeof(MyEnum), value)` and `Enum.GetNames(typeof(MyEnum))` can identify the enum type during contract compilation.

Guidance:
- Do not use `typeof` for .NET reflection or `System.Type` behavior in contracts. Reflection is outside the supported contract surface.
- Treat direct `typeof` results as Neo string semantics, not standard .NET type metadata.

## Lambdas that capture `foreach` variables

Neo currently stores captured local variables in shared contract fields. A lambda created inside a `foreach` loop therefore observes the most recently assigned iteration value when it is invoked after the loop. Standard C# creates a distinct captured variable for each iteration.

The analyzer reports `NC4063` when a lambda references its enclosing `foreach` variable. Avoid retaining that lambda across iterations; compute the value eagerly or pass it explicitly instead.

## Exceptions

An exception carries no object inside a contract: it is represented by its message, a UTF-8 `ByteString`.

- `throw new Exception("message")` throws the message. Only a constructor with zero or one argument is supported (`NC2005` otherwise), and instantiating an exception type declared inside the contract is rejected with `NC1002` (`throw new MyError()` compiles but throws the generic message `"exception"`).
- The variable of a `catch` clause holds the message. `ex.Message` and `ex.ToString()` evaluate to it; every other instance member of an exception (`Source`, `StackTrace`, `InnerException`, `GetType()`, ...) is rejected with `NC1002`. See [#2358](https://github.com/neo-project/neo-devpack-dotnet/pull/2358).
- The type written in a `catch` clause is not checked at runtime. `catch (InvalidOperationException e)` catches every catchable exception, exactly like `catch (Exception e)`, so code that relies on different `catch` clauses for different exception types must branch on the message instead; the Neo analyzer reports such a clause with `NC4027`. Only one `catch` clause is allowed (`NC2006`) and exception filters are not supported (`NC2007`).
- Not every fault can be caught. A `throw`, a division by zero and an out-of-range index reach the `catch` block. `ExecutionEngine.Assert`, `ExecutionEngine.Abort`, `checked` overflow faults, failed `int.Parse`/`BigInteger.Parse` calls, invoking a `null` delegate and VM type faults such as reading `.Length` of `null` abort the execution without running any `catch` or `finally` block. Validate inputs before performing the operation instead of relying on `try`/`catch` for these cases.

```csharp
try
{
    Transfer(receiver, amount);
}
catch (Exception ex)
{
    // The declared type would not filter anything; branch on the message.
    if (ex.Message.Contains("insufficient"))
        OnInsufficientBalance(receiver);
    return false;
}
```

## `ToString()` on boxed value types

A value type boxed into `object` is stored as the VM stack item, not as a .NET boxed value. Calling `ToString()` on the boxed expression therefore converts the raw stack item to bytes: `((object)5).ToString()` yields the single byte `0x05`, and `((object)'中').ToString()` yields the little-endian bytes `0x2D 0x4E` of the integer `20013`. Call `ToString()` while the expression still has its declared type, or cast back to the value type first: `((int)obj).ToString()`.

## Class-library members

Only members of class-library types that have a compiler lowering are supported. Any other member fails with `NC1002`: methods and properties report `Unknown method`, constructors of types that cannot be built inside a contract (`new Guid()`, `new DateTime()`, `new string(char[])`) report `Unknown constructor`, and static fields of such types (`Guid.Empty`, `TimeSpan.Zero`) report `Unknown field`. See [#2357](https://github.com/neo-project/neo-devpack-dotnet/pull/2357).
