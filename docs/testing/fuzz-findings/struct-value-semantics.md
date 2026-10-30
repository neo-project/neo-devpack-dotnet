# Compiler defects: struct assignment shares storage, nested structs fault

**Severity: high** — both produce a wrong contract. The first returns an incorrect value
silently; the second faults on every call.

**Found by:** the cross-language differential, after the struct feature family was added to
the fuzzer. The comparison runs the same method body on the CLR and on the Neo VM, so a
difference is unambiguous.

## Summary

| Shape | Neo VM | C# | Result |
|---|---|---|---|
| flat struct field access | 17 | 17 | correct |
| **struct assignment (`var q = p;`)** | **198** | **106** | **wrong value** |
| **nested struct field write** | FAULT `Invalid type for SETITEM: Any` | 17 | **faults** |
| **nested struct field read** | FAULT `Invalid type for PICKITEM: Any` | 2 | **faults** |
| struct passed to a method | 13 | 13 | correct |

## Defect 1 — assignment copies the reference instead of the value

```csharp
public struct Point { public int X; public int Y; }

public static int Copy(int a)
{
    var p = new Point { X = a, Y = a + 1 };
    var q = p;      // C#: q is an independent copy
    q.X = 99;       // C#: does not affect p
    return p.X + q.X;
}
```

With `a = 7`, C# returns `7 + 99 = 106`. The VM returns `99 + 99 = 198`: writing through
`q` also changed `p`, so the two names still refer to the same storage.

The disassembly shows why. A struct is represented as an array and its fields are reached by
index:

```asm
// StructProbeContract.Add(StructProbeContract.Point, int)
00000003: LDARG 0
00000004: PUSH 0
00000005: PICKITEM      ; p.X
```

`var q = p;` is emitted as a plain reference assignment, with no duplication of the
underlying array.

**Why this is the worse of the two defects:** the contract compiles, runs, and returns a
plausible number. There is no fault, no diagnostic, and no exception to notice. A contract
that relies on value semantics for a struct silently computes the wrong result.

## Defect 2 — nested structs fault on field access

```csharp
public struct Point { public int X; public int Y; }
public struct Nested { public Point Origin; public int Scale; }

public static int Probe(int a, int b)
{
    var n = new Nested();
    n.Origin.X = a;     // FAULT: Invalid type for SETITEM: Any
    n.Origin.Y = b;
    n.Scale = 2;
    return n.Origin.X * n.Scale + n.Origin.Y;
}
```

The inner struct field is not initialised when the outer struct is created, so the store
targets a null item. Reading works the same way: `n.Origin.X` without a prior write faults
with `Invalid type for PICKITEM: Any`.

A struct that contains another struct is therefore unusable, both for reading and for
writing.

## Reproduction

```
dotnet tests/Neo.Compiler.CSharp.Fuzzing/bin/Release/net10.0/neofuzz.dll --struct-probe
```

The probe compiles each shape on its own, executes it on the VM, runs the same body on the
CLR, and prints both results side by side.

## Impact

`struct` is an ordinary language feature and the compiler accepts it without complaint:

- `docs/csharp-syntax/csharp-1.md` states that value type struct definitions compile
  successfully, and the test contracts already declare several (`Contract_Records.cs`
  declares `record struct Position`).
- No existing test asserts copy semantics or exercises a nested struct, which is why the
  suite passes while both shapes are broken.

## Suggested direction

- **Assignment.** When the assigned type is a value type, duplicate the underlying storage
  rather than copying the reference. A struct is an array in the VM, so this means
  producing a fresh array with the same items at the assignment site.
- **Nested structs.** Initialise a struct-valued field to a zero-initialised struct of that
  type when the outer struct is created, so the first field write has a target.
- Alternatively, until both are implemented, reject value-type nesting and value-type
  assignment with a diagnostic, so contracts fail to compile instead of computing wrong
  values or faulting at run time.

## Status

Fixed in PR #2203 (`fix/struct-value-semantics`, commit `ceb72310`).

- The default value of a struct-typed field is now a zero-initialised struct of the same
  shape, built recursively, so nested field access has a target.
- A struct store rebuilds the field array from copies of its fields, recursively for nested
  structs. This is emitted for local declarations, assignments, fields, properties, array
  elements, object initializers, and arguments.

Re-run after the fix (`--struct-probe`, seed 981001): all shapes below the summary table
match the CLR, and the campaign's instruction coverage rose from 1183 to 1511 because the
struct code paths now execute instead of faulting.
