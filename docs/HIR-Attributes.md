# HIR Attribute Mapping

This note describes how Roslyn symbol metadata is captured when building High-level IR (HIR) signatures. The goal is to retain source-level intent (ABI display names, safety contracts, syscall aliases) so downstream passes and manifest generation can rely on a consistent, compiler-driven view.

## Overview

During import, every `IMethodSymbol` (and its paired accessor, if any) is examined and translated into a collection of `HirAttribute` instances. The resulting list is stored on the `HirSignature` that HIR nodes reference.

```
# Roslyn                                # HIR signature
public class Token {
    [Safe]
    [DisplayName("balanceOf")]
    public static BigInteger BalanceOf(UInt160 owner) { ... }
}

=> HirSignature.Attributes =
   [
     HirMethodNameAttribute("balanceOf"),
     HirSafeAttribute()
   ]
```

## Supported mappings

| Source attribute | HIR attribute             | Notes |
| ---------------- | ------------------------- | ----- |
| `System.ComponentModel.DisplayNameAttribute` | `HirMethodNameAttribute` | Applied to methods, property accessors, and events. First non-empty constructor argument wins; duplicates from associated symbols are ignored. |
| `Neo.SmartContract.Framework.Attributes.SafeAttribute` | `HirSafeAttribute` | Marks methods that must remain read-only. Propagates from accessors to the canonical method signature. |
| `Neo.SmartContract.Framework.Attributes.SyscallAttribute` | `HirIntrinsicTagAttribute` | Records `(category,name)` data when the syscall is known to the intrinsic catalog. Applied to methods and constructors (e.g., `StorageMap`). Multiple annotations collapse to unique pairs. |
| `Neo.SmartContract.Framework.Attributes.NoReentrantAttribute` / `NoReentrantMethodAttribute` | `HirNoReentrantAttribute` | Captures the storage prefix/key used for reentrancy guards; defaults (0xFF, `"noReentrant"`) are preserved. |
| `System.Runtime.CompilerServices.MethodImplAttribute` (AggressiveInlining) | `HirInlineAttribute` | Records aggressive inline hints; used as a signal for optimisation heuristics. |

Additional attributes are recognised through existing tooling (`AbiMethod` still inspects Roslyn symbols directly) and can be surfaced in HIR as needed. Examples under consideration include future payable markers or custom attributes that steer optimisation heuristics.

## Usage downstream

- **Manifest generation** now consumes HIR metadata: the ABI builder prefers `HirMethodNameAttribute` and `HirSafeAttribute` when emitting manifest entries, falling back to Roslyn attributes only when HIR was disabled.
- **Intrinsic selection** benefits from `HirIntrinsicTagAttribute` because even constructor invocations that involve syscall-backed helpers are clearly linked to their catalog entries.
- **Debug/ABI tooling** preserves display names and method aliases without extra Roslyn queries.
- **Reentrancy analysis** uses `HirNoReentrantAttribute`; guarded methods are emitted under `extra.reentrancyGuards` in the manifest and security tooling skips warnings for those entry points.
- **Inline heuristics** can rely on `HirInlineAttribute` when the developer has opted into `[MethodImpl(MethodImplOptions.AggressiveInlining)]`.

The attribute builder is intentionally conservative; unsupported annotations are ignored rather than causing failures. This keeps the pipeline resilient while laying the groundwork for richer metadata flows.
