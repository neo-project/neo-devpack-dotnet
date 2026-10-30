# Compiler defect: `StorageMap` construction emits no object, then stores from an empty stack

**Severity: high** — the compiler reports success, but the produced contract faults on the
first call, so every affected contract is dead on arrival.

**Found by:** the differential fuzz harness (`--storage-map-probe`, `--failure-triage`).
The failure was invisible to the differential alone because both the optimized and the
unoptimized build fail identically.

## Summary

A contract that constructs `StorageMap` compiles without error. The generated script then
faults at run time with `Out of stack bounds: 0/0`, before the map is ever used:

```asm
// MapProbe.Construct(int)
00000000: INITSLOT 1, 1
00000003: STLOC 0        ; pops from an empty stack — nothing was pushed for the object
00000004: LDARG 0
00000005: JMP <00000007>
00000007: RET
```

`STLOC 0` stores the constructed object into the local slot, but no object-creation
sequence was emitted before it.

## Minimal reproduction

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class MapProbe : SmartContract
{
    public static int Construct(int a)
    {
        var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x02 });
        return a;
    }
}
```

Compiling this yields `Success` with no diagnostics. Deploying and calling `construct(7)`
throws `TestException: Out of stack bounds: 0/0`.

## Evidence

Every operation was compiled and executed in isolation, against a baseline that is known
to work:

| Case | Result |
|---|---|
| `Storage.Put` / `Storage.Get` (baseline) | **7** |
| Custom class constructor declared in the contract (control) | **7** |
| `new StorageMap(Storage.CurrentContext, byte[])` | FAULT `Out of stack bounds: 0/0` |
| `new StorageMap(byte[])` | FAULT `Out of stack bounds: 0/0` |
| `StorageMap.Put` | FAULT `Out of stack bounds: 0/0` |
| `StorageMap.Get` | FAULT `Out of stack bounds: 0/0` |
| `new StorageMap(...).Find(...)` (chained) | FAULT `Swap index is out of stack bounds: 1/1` |

The baseline and the control both work, which rules out the calling convention and object
creation in general. Only the `StorageMap` constructors fail, and they all share one
property: **they are declared `extern`**.

## Root cause

`src/Neo.Compiler.CSharp/MethodConvert/Expression/ObjectCreationExpression.cs:53-62`:

```csharp
bool needCreateObject = !type.DeclaringSyntaxReferences.IsEmpty && !constructor.IsExtern;
if (needCreateObject)
{
    if (TryOptimizedObjectCreation(model, expression, type, constructor))
        return;
    CreateObject(model, type);
}
if (!constructor.DeclaringSyntaxReferences.IsEmpty)
    CallInstanceMethod(model, constructor, needCreateObject, arguments);
```

For a type declared in source whose constructor is `extern`:

- `needCreateObject` evaluates to `false`, so `CreateObject` is skipped — nothing is
  pushed for the instance.
- The second condition, `!constructor.DeclaringSyntaxReferences.IsEmpty`, is still `true`,
  because the *type* is declared in source even though the *constructor* is extern. So
  `CallInstanceMethod` runs with `needCreateObject: false`.

The two decisions disagree: creation is suppressed while the call is still emitted. The
result is a constructor call with no receiver on the stack.

`StorageMap` matches this shape exactly — all six of its constructors are `extern`
(`src/Neo.SmartContract.Framework/Services/StorageMap.cs:94-131`) — and it is the only
public framework type a contract would construct in this way.

The compiler has no `StorageMap` support at all: `StorageMap` appears **zero** times in
`src/Neo.Compiler.CSharp`. `TryProcessSystemConstructors`
(`src/Neo.Compiler.CSharp/MethodConvert/System/SystemCall.cs:259`) handles `Exception`,
`string`, `StringBuilder`, and `BigInteger(byte[])` only, so `StorageMap` falls through to
the generic path above.

## Why the existing tests do not catch it

`tests/Neo.Compiler.CSharp.TestContracts/Contract_Foreach.cs:276` declares
`TestIteratorForEach()`, which constructs a `StorageMap` and iterates it. **No unit test
calls that method** — it exists only as a contract method. The whole test suite therefore
passes while the construct is unusable.

This is the same blind spot the differential oracle has: a construct that fails
identically in every build produces no divergence, so nothing is reported. The
`SuspiciousFailure` triage (`FailureTriage.cs`) exists to close it, and is what surfaced
this: the triage reports "method failed on all N inputs without succeeding once".

## Suggested fix

Two options, in order of preference:

1. **Reject it.** In `ConvertObjectCreationExpression`, when `constructor.IsExtern` and the
   type is not handled by `TryProcessSystemConstructors`, raise a `CompilationException`.
   The compiler would then refuse `StorageMap` instead of emitting a contract that always
   faults. This is a few lines and cannot regress working code, because no working
   `StorageMap` construction exists.

2. **Support it.** Add `StorageMap` (and `LocalStorageMap`) to `TryProcessSystemConstructors`
   and lower the map operations to their `Storage` equivalents, with the prefix prepended to
   each key. That is the real feature, and it is what the framework's own `AccessControl`
   and `NEP-11` bases assume exists, but it is a larger change and needs the intended
   key-encoding semantics confirmed against `Storage.Put`/`Get`.

Until either lands, the fuzzer should keep reporting it: the `StorageMap` feature family is
expected to fail, and the finding is the signal that it still does.
