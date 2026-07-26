# Unsupported C# syntax diagnostics

Neo smart contracts support a deterministic subset of C#. The diagnostics below
identify syntax that cannot be lowered safely to NeoVM instructions.

| Diagnostic | Unsupported construct | Recommended alternative |
| --- | --- | --- |
| <a id="nc4033"></a>NC4033 | `unsafe` blocks and pointer operations | Use managed values such as arrays, `ByteString`, and framework types. |
| <a id="nc4034"></a>NC4034 | Anonymous `delegate` expressions | Use a supported lambda expression or a named method. |
| <a id="nc4035"></a>NC4035 | Iterator blocks using `yield` | Build and return a bounded collection explicitly. |
| <a id="nc4036"></a>NC4036 | LINQ query-expression syntax | Use supported collection helpers or explicit bounded loops. |
| <a id="nc4037"></a>NC4037 | Dynamic binding | Use a statically known supported type. |
| <a id="nc4038"></a>NC4038 | `async` methods | Keep contract execution synchronous and deterministic. |
| <a id="nc4039"></a>NC4039 | `await` expressions | Call a supported synchronous contract API instead. |
| <a id="nc4040"></a>NC4040 | Exception filters with `when` | Test the condition inside a supported catch block. |
| <a id="nc4042"></a>NC4042 | Local function declarations | Move the function to a private contract method. |
| <a id="nc4045"></a>NC4045 | `await foreach` | Use a supported bounded synchronous loop. |
| <a id="nc4046"></a>NC4046 | Native-sized integers (`nint` and `nuint`) | Use an explicitly sized integer type or `BigInteger`. |
| <a id="nc4047"></a>NC4047 | Top-level statements | Put contract code in a class derived from `SmartContract`. |
| <a id="nc4048"></a>NC4048 | Function pointers | Use a supported delegate or direct method call. |
| <a id="nc4049"></a>NC4049 | Global using directives | Add ordinary using directives to each source file that needs them. |
| <a id="nc4050"></a>NC4050 | List patterns | Use explicit length and element comparisons. |
| <a id="nc4051"></a>NC4051 | UTF-8 string literals with the `u8` suffix | Use a string literal or an explicit supported byte array. |
| <a id="nc4053"></a>NC4053 | File-local types | Use a private or internal type with a unique name. |
| <a id="nc4054"></a>NC4054 | `ref readonly` and `in` parameters | Pass the supported value type by value. |
| <a id="nc4059"></a>NC4059 | Using statements and using declarations | Manage the supported resource explicitly without relying on `Dispose`. |
