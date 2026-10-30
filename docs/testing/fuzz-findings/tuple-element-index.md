# Compiler defect: tuple element access past `Item2` reads the wrong index

**Severity: high** — any tuple with three or more elements faults at run time when an element
is read or written by name. The compiler accepts the code and emits a valid contract, so the
failure only shows up on execution.

**Found by:** the struct matrix probe, after tuple values were added to the copy checks. The
same tuple that copies correctly still could not be read by element name.

## Symptom

```csharp
public static int Three(int a, int b)
{
    var t = (a, b, 3);
    return t.Item3;          // FAULT: the index of VMArray is out of range, 4/[0, 3)
}
```

The generated code reads index 4 from an array of three items:

```asm
LDLOC0        ; t
PUSH4         ; element index, should be 2
PICKITEM
```

## Measured indices

| Element | Emitted index | Storage size | Result |
|---|---|---|---|
| `Item1` | 0 | n | correct |
| `Item2` | 1 | n | correct |
| `Item3` | 4 | 3 | out of range |
| `Item4` | 5 | 4 | out of range |
| `Item7` | 8 | 7 | out of range |
| `Item8` | 9 | 8 | out of range |

From `Item3` on the emitted index is `N + 1`, so the defect appears as soon as a tuple has a
third element.

## Cause

`FieldLayoutHelpers.GetInstanceFieldIndex` positions a field by its place in
`field.ContainingType.GetFields()`:

```csharp
int index = System.Array.IndexOf(field.ContainingType.GetFields(), field);
```

`GetFields()` is `GetAllMembers().OfType<IFieldSymbol>()`, and for a tuple type `GetAllMembers`
returns more than the declared elements (the tuple implements several interfaces whose members
are merged in). The position of `Item3` and later elements therefore no longer matches the
array that `PACKSTRUCT` produced at creation time, which holds exactly the tuple elements.

`Item1` and `Item2` happen to line up, which is why the existing tuple tests pass: they use
two-element tuples, or read the result through the array (`tuple[2]`) rather than through
`ItemN`.

## Fix direction

Resolve tuple elements through `INamedTypeSymbol.TupleElements`, whose order matches the
storage order used when the tuple is created. The copy helper already relies on that order,
which is why tuple assignment now works while element access does not.

`ConvertMemberAccessExpression` and `ConvertMemberAccessAssignment` both call
`GetInstanceFieldIndex` directly, so both need the tuple-aware index. A follow-up change will
add it together with a test that reads and writes `Item3` and `Item7`.

## Second finding while fixing the index: `DivRem` packs its tuple in reverse

The element index fix cannot ship alone. `EmitDivRem`
(`MethodConvert/System/SystemCall.BigInteger.cs`) builds the C# tuple `(Quotient, Remainder)`
but packs the remainder first:

```csharp
methodConvert.Mod();      // stack: l/r, l%r
methodConvert.Pack(2);    // index 0 = l%r, index 1 = l/r
```

Because `PACK` stores the first popped item at index 0, the stored order is
`[remainder, quotient]`, the opposite of the declared `(int Quotient, int Remainder)`.

Measured on master-n3 (`0fe9cf79`):

| Expression | Emitted index | Stored item | Result |
|---|---|---|---|
| `int.DivRem(10, 3).Quotient` | 1 | quotient = 3 | right by accident |
| `int.DivRem(10, 3).Remainder` | 3 | out of range | fault |

The two defects cancel out for `Quotient` only. Deconstruction shows the swap directly:

```csharp
var (q, r) = int.DivRem(10, 3);   // C#: q = 3, r = 1; the VM stores q = 1, r = 3
```

`Contract_Integer.DivRemByte/Short/Int/Long/SByte` return the array, and
`UnitTest_Integer` asserts `result[0] == Remainder` and `result[1] == Quotient`, so the
tests currently encode the reversed order.

A fix needs three parts, and they have to land together:

1. `EmitDivRem` swaps the two items before `Pack(2)`, so index 0 holds the quotient.
2. `GetInstanceFieldIndex` resolves tuple elements through `TupleElements` (the change
   described above), so `ItemN` maps to `N - 1`.
3. `UnitTest_Integer` expectations follow the C# order, and ABI callers that deconstruct the
   result see `(quotient, remainder)`.

All three landed in PR #2204:

| Test | Before | After |
|---|---|---|
| `t.Item3` on a 3-element tuple | fault, index 4/3 | 3 |
| `t.Item7` on a 7-element tuple | fault, index 8/7 | 7 |
| `t.Item8` on an 8-element tuple | fault, index 9/8 | 8 |
| `var (q, r) = int.DivRem(10, 3)` | q = 1, r = 3 | q = 3, r = 1 |
| `int.DivRem(10, 3).Remainder` | fault, index 3/2 | 1 |
