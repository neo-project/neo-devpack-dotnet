# Observation: constant array sizes above the VM limit compile without a diagnostic

**Severity: low** — the contract compiles, can be deployed, and faults on every call that reaches
that line. Nothing is miscompiled; the compiler accepts a size the VM can never allocate.

## Symptom

```csharp
public static int Huge()
{
    var values = new int[65536];
    return values.Length;      // FAULT: The array size is out of valid range, 65536/[0, 2048]
}
```

The same source compiles without a warning. `new byte[65536]` is a different case: the byte
buffer path validates the size against `ExecutionEngineLimits.MaxItemSize` and reports
`InvalidArgument`, which `UnitTest_ArraySizeDiagnostics` covers.

## Why it is not fixed here

`MaxArraySize` is not exposed by `Neo.VM` (the limits object exposes `MaxItemSize`,
`MaxShift`, `MaxStackSize`, `MaxInvocationStackSize`, `MaxComparableSize`, and
`MaxTryNestingDepth`, but not the array size), so the compiler would have to hard-code the
protocol constant. That is a decision for the maintainers rather than a defect fix: rejecting
the size at build time is friendlier, but it also turns a runtime failure into a build failure
for code that may never run.

## How it surfaced

The multi-dimensional array family generated `new int[65536]` (a constant size taken from the
generator's constant pool) and every execution of the method faulted. The failure triage
classified it as systematic because it failed on all inputs, which is what a missing
compile-time check looks like from the outside.

The semantic matrix probe reproduced it directly:

| Shape | VM | CLR |
|---|---|---|
| `new int[2048]` | THROW `MaxStackSize exceed: 2052/2048` (test engine stack, not the array limit) | 2048 |
| `new int[2049]` | THROW `array size is out of valid range, 2049/[0, 2048]` | 2049 |

The 2048 case only fails because the probe's engine has a small evaluation stack; on a
deployment engine it allocates. The array-size limit itself is 2048 per array.
