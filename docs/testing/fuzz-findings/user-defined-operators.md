# Compiler defects: user-defined operators are not lowered to operator methods

**Severity: high** — `==`/`!=` produce a wrong result silently; `<`/`>`/`<=`/`>=` and
user-defined implicit conversions fault at runtime; arithmetic operator uses are rejected
with a misleading diagnostic.

**Found by:** the new `OperatorOverloads` feature family and the `--operator-probe`
differential probe. The comparison runs the same method body on the CLR and on the Neo VM,
so a difference is unambiguous.

## Summary

| Shape | Neo VM | C# | Result |
|---|---|---|---|
| `operator==` compares X only, operands `(7,3)`/`(7,9)` | 0 | 1 | **wrong value** |
| `operator!=` compares X only, same operands | 1 | 0 | **wrong value** |
| `operator==` compares all fields | 1 | 1 | correct (by coincidence) |
| `operator<` compares X only | FAULT `Specified cast is not valid` | 1 | **faults** |
| implicit `operator Vec(int)`, `Vec v = 7;` | FAULT `index of PrimitiveType is out of range` | 708 | **faults** |
| explicit `operator int(Vec)`, `(int)v` | 7003 | 7003 | correct |
| `operator+(Vec, Vec)` use | compile rejected (`NC2001 Unsupported integer type 'Vec'`) | 706 | rejected |

## Defect 1 — comparison operators emit a raw VM opcode instead of calling the operator method

```csharp
public struct Vec
{
    public int X;
    public int Y;
    public Vec(int x, int y) { X = x; Y = y; }
    public static bool operator ==(Vec a, Vec b) { return a.X == b.X; }   // Y ignored on purpose
    public static bool operator !=(Vec a, Vec b) { return a.X != b.X; }
}

public static int Probe(int a, int b)
{
    Vec p = new Vec(7, 3);
    Vec q = new Vec(7, 9);
    return p == q ? 1 : 0;
}
```

C# returns `1` (`X` fields are equal). The VM returns `0`. The disassembly shows that the
operator method is never called — `BinaryExpression.EmitBinaryOperator` maps `==` to
`EQUAL` and applies it to the two struct arrays, which compares them structurally:

```asm
// OperatorProbeContract.Probe(int, int)
00000019: LDLOC 0
0000001a: LDLOC 1
0000001b: EQUAL
```

`op_Equality` is not even part of the compiled script. A user-defined `==` whose semantics
differ from deep structural equality (comparing a subset of fields, normalizing values,
cross-type comparisons) therefore returns the wrong answer with no diagnostic. The same
applies to `!=` (`NOTEQUAL`). Relational operators (`<`, `>`, `<=`, `>=`) can only appear
with a user-defined operator, and the raw `LT`/`GT`/`LE`/`GE` applied to struct values
faults at runtime with `Specified cast is not valid`.

Note the full-field `operator==` matches C# **by coincidence**: deep structural equality of
the two field arrays happens to agree with `a.X == b.X && a.Y == b.Y`. That is what makes
this defect easy to miss in hand-written tests.

## Defect 2 — user-defined implicit conversion faults on execution

```csharp
public struct Vec
{
    public int X;
    public int Y;
    public static implicit operator Vec(int x) { return new Vec(x, x + 1); }
}

public static int Probe(int a, int b)
{
    Vec v = 7;              // implicit operator Vec(int)
    return v.X * 100 + v.Y; // C#: 708
}
```

The contract compiles, but every call faults with
`TestException: An unhandled exception was thrown. The index of PrimitiveType is out of range, 1/[0, 1).`
The explicit conversion direction (`(int)v` → `op_Explicit`) is routed to the operator
method correctly and returns the right value, so the two directions of the same feature
take different code paths in the compiler.

## Defect 3 — arithmetic operator uses are rejected with a misleading diagnostic

`Vec c = p + q;` is rejected with
`NC2001 Unsupported integer type 'FuzzContract.Vec'. Supported integer types: sbyte, byte, ...`.
The compiler falls into the integer path (`EnsureIntegerInRange`) because the
binary-expression converter does not check whether the operator is user-defined. The
rejection itself is safe; the diagnostic names the wrong cause, and the operator token
is never mentioned.

## Root cause

`MethodConvert/Expression/BinaryExpression.cs` dispatches on the operator token only. When
Roslyn binds the expression to a user-defined operator method
(`IMethodSymbol.MethodKind == UserDefinedOperator`), the operands still go through the raw
opcode path (`ADD`, `EQUAL`, `LT`, ...) instead of a call to the operator method, and the
integer range check then trips over the struct type. The fix is to detect the
user-defined-operator symbol and convert the expression as a static invocation of that
method (the same routing `op_Explicit` already receives).

## Reproduction

```
dotnet run --project tests/Neo.Compiler.CSharp.Fuzzing -c Release -- --operator-probe
```

The `OperatorOverloads` feature family keeps these shapes in the random corpus, so the
differential oracles re-check them on every campaign.
