# LINQ Take: NeoVM and GAS comparison

PR title: **Optimize LINQ Take to avoid unnecessary iterations**

Base: `upstream/master-n3`, commit `e682139a`. Measurements use the repository's .NET 10 test projects, `TestEngine(true)`, compiler version `TestingEngine`, and extended debug information. No external profiler is required.

## Reproduction

Run `dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter FullyQualifiedName~UnitTest_LinqTake --logger "trx;LogFileName=take-comparison.trx"`.

The TRX `StdOut` includes both opcode lists and every measured invocation. `UnitTest_LinqTake` compiles two small contracts with the same public entry point, `Take(int[] source, BigInteger count)`, and the same options:

- Original: an embedded snapshot of the former generic extension method, including its null-source validation.
- Optimized: a call to the current framework extension, not a copied optimized implementation.

Each contract is deployed in a separate, identically configured engine. Invocation GAS is captured immediately after the call; deployment cost is not included. Both versions must return the same complete prefix, in the same order, and reject null sources. Absolute GAS snapshots and a strict improvement assertion protect `Take(0)`, `Take(1)` and `Take(2)`.

For the project-generated `Contract_Linq` artifact, run the `UnitTest_Linq` tests again if assembly cleanup reports that artifacts were updated. The first run can execute the previously compiled artifact; only the subsequent run validates the regenerated artifact.

## GAS with full compiler optimization

All nonempty measurements below use the **same six-element collection**: `[7, -1, 7, 0, -100, 5]`. Values are datoshi (1 GAS = 100,000,000 datoshi).

| Source length | Count | Original | Optimized | Saving |
|---:|---:|---:|---:|---:|
| 6 | -1 | 1,148,430 | 1,145,580 | 2,850 |
| 6 | 0 | 1,148,430 | 1,145,580 | 2,850 |
| 6 | 1 | 1,397,580 | 1,394,670 | 2,910 |
| 6 | 2 | 1,646,730 | 1,643,430 | 3,300 |
| 6 | 5 | 2,394,180 | 2,389,710 | 4,470 |
| 6 | 6 | 2,640,780 | 2,638,470 | 2,310 |
| 6 | 10 | 2,640,780 | 2,638,830 | 1,950 |
| 6 | 2^200 | 2,640,870 | 2,638,920 | 1,950 |
| 0 | -1 or 0 | 1,084,710 | 1,084,410 | 300 |
| 0 | positive small count | 1,084,710 | 1,085,100 | **-390** |
| 0 | 2^200 | 1,084,800 | 1,085,190 | **-390** |

A negative `-2^200` count also preserves the empty result and saves 2,850 datoshi for the six-element source, or 300 for the empty source.

### Without compiler optimization

| Source length | Count | Original | Optimized | Saving |
|---:|---:|---:|---:|---:|
| 6 | 0 | 1,148,610 | 1,145,700 | 2,910 |
| 6 | 1 | 1,397,850 | 1,394,880 | 2,970 |
| 6 | 2 | 1,647,090 | 1,643,730 | 3,360 |
| 6 | 6 | 2,641,470 | 2,639,130 | 2,340 |
| 6 | 10 | 2,641,470 | 2,639,550 | 1,920 |
| 0 | 0 | 1,084,860 | 1,084,530 | 330 |
| 0 | positive small count | 1,084,860 | 1,085,280 | **-420** |

### Trade-off

This is **not a universal GAS or bytecode-size reduction**. A positive count with an empty source pays for the new early count guard: +390 datoshi (0.00000390 GAS, approximately 0.036%) with full optimization, or +420 without it. The differential test explicitly checks this overhead; it is not hidden by disabling GAS assertions. All other paths in the measured matrix improve, including counts equal to or exceeding the collection length.

The implementation is retained because the requested early-termination cases demonstrably improve, null validation and results are preserved, and the empty-positive cost is small and explicitly quantified. Maintainers can evaluate this trade-off rather than assuming every input is cheaper.

## Generated NeoVM opcodes

These lists cover the generic Take method, including its prologue and return. The byte sizes below cover the entire small comparison contract, including its wrapper and null-validation helper, not only Take.

### Optimization = All

Original: **39 instructions**, **87 script bytes**.

```text
INITSLOT LDARG0 CALL NEWARRAY0 STLOC0 LDARG0 DUP STLOC1 SIZE STLOC2
PUSH0 STLOC3 JMP LDLOC1 LDLOC3 PICKITEM STLOC4 LDARG1 PUSH0 LE
JMPIFNOT JMP LDLOC0 LDLOC4 APPEND LDARG1 DUP DEC STARG1 DROP
LDLOC3 INC STLOC3 LDLOC3 LDLOC2 JMPLT LDLOC0 JMP RET
```

Optimized: **41 instructions**, **91 script bytes**.

```text
INITSLOT LDARG0 CALL NEWARRAY0 STLOC0 LDARG1 PUSH0 LE JMPIFNOT
LDLOC0 JMP LDARG0 DUP STLOC1 SIZE STLOC2 PUSH0 STLOC3 JMP
LDLOC1 LDLOC3 PICKITEM STLOC4 LDLOC0 LDLOC4 APPEND LDARG1 DEC
DUP STARG1 JMPIF JMP LDLOC3 INC STLOC3 LDLOC3 LDLOC2 JMPLT
LDLOC0 JMP RET
```

### Optimization = None

Original: **44 instructions**, **95 script bytes**.

```text
INITSLOT NOP LDARG0 CALL NEWARRAY0 STLOC0 LDARG0 DUP STLOC1 SIZE
STLOC2 PUSH0 STLOC3 JMP LDLOC1 LDLOC3 PICKITEM STLOC4 NOP
LDARG1 PUSH0 LE JMPIFNOT JMP NOP LDLOC0 LDLOC4 APPEND LDARG1
DUP DEC STARG1 DROP NOP LDLOC3 INC STLOC3 LDLOC3 LDLOC2 JMPLT
NOP LDLOC0 JMP RET
```

Optimized: **47 instructions**, **100 script bytes**.

```text
INITSLOT NOP LDARG0 CALL NEWARRAY0 STLOC0 LDARG1 PUSH0 LE
JMPIFNOT LDLOC0 JMP NOP LDARG0 DUP STLOC1 SIZE STLOC2 PUSH0
STLOC3 JMP LDLOC1 LDLOC3 PICKITEM STLOC4 NOP LDLOC0 LDLOC4
APPEND LDARG1 DEC DUP STARG1 JMPIF JMP NOP NOP LDLOC3 INC
STLOC3 LDLOC3 LDLOC2 JMPLT NOP LDLOC0 JMP RET
```

### What the compiler already optimizes

- This foreach is lowered to indexed NeoVM array traversal (`SIZE`, `PICKITEM`, `INC`, `JMPLT`), not a .NET `MoveNext` call.
- The original count guard remains **after** `PICKITEM`, even with `OptimizationType.All`. It therefore does not prevent the initial access for zero/negative counts, or the extra access after taking a proper prefix.
- The new guard is **before** `SIZE` and loop initialization. Null-source validation (`LDARG0 CALL`) remains before both list creation and the guard.
- The compiler already lowers the decremented-count comparison with zero to a conditional `JMPIF`, without a separate `PUSH0`/`NUMEQUAL`. The original discarded postfix value uses `DUP DEC STARG1 DROP`; the new prefix expression uses `DEC DUP STARG1` and branches directly.
- Full optimization removes NOPs and simplifies the sequences, but does **not** make the original loop equivalent to the new one. Both compiler modes still show distinct sequences and measurable savings.
- Statically there are more instructions and bytes, but fewer instructions execute on the early-return and proper-prefix paths. For `Take(1)` and `Take(2)` on the six-element source, the break avoids the subsequent index increment, loop condition and extra `PICKITEM`.

## Existing contract assertions and regression coverage

The existing `Contract_Linq.Take` entry point retains its `int` count parameter. Its historical fixtures differ from the comparison contract (including wrapper conversion and invocation setup), so their absolute totals must not be mixed:

| Existing fixture | Original baseline assertion | Current verified assertion |
|---|---:|---:|
| Take(0), three elements | 1,147,770 | 1,144,920 |
| Take(2), six elements | 1,646,160 | 1,642,860 |

`Contract_Linq.TakeBigInteger` exposes the framework's real count range without changing the existing wrapper. `UnitTest_Linq` checks empty sources, negative and zero counts, all returned prefix values and their order, duplicate values, smaller/equal/larger counts, `int` extremes, `BigInteger` values outside `int` including ±2^200, and null-source faults even for nonpositive counts. The generated artifact includes the new entry point.

Validation: `UnitTest_Linq` 31/31 passed; `UnitTest_LinqTake` 2/2 passed (None and All); `UnitTest_LinqAverage` 40/40 passed; `HelperTest.Test_Take` 1/1 passed. The final solution build passed, and `git diff --check` reported no issues. These are the relevant tests, not a run of the entire solution test suite. No production API or compiler changes are needed.
