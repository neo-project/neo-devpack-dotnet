# Mid-level IR (MIR) Design

Detailed reference for the Neo C# DevPack compiler’s MIR layer. Complements the overview in `docs/README-IR-Design-Neo.md` and bridges HIR semantics to the LIR backend.

---

## Purpose
- Provide a strict, canonical SSA IR with explicit guards and side-effect ordering.
- Act as the primary optimisation workhorse: SCCP, GVN/CSE, SROA, LICM, loop transforms, guard hoisting/sinking, redundant read elimination, ABI hardening.
- Normalise DevPack intrinsics into syscall families with fixed signatures and effect tags so that every operation maps directly to NeoVM sequences.

---

## Scope & Boundaries
- No implicit exceptions: all potential failures appear as `Guard*` nodes with defined on-fail behaviour (`Abort` or branch to a fail block).
- No high-level sugar: properties/indexers already normalised; generics monomorphised; closures converted; receivers explicit.
- Function signatures hardened: explicit receiver as first parameter, single return (multi via `Struct`), monomorphised generics, closure-converted.

---

## Type System
- Scalars: `Bool`, `Int` (BigInteger semantics with optional width hints), `ByteString`, `Buffer`, `Void`.
- Aggregates/containers: `Array<T>`, `Struct{fields}`, `Map<K,V>`. `InteropInterface` remains opaque at syscall boundaries.
- No dedicated `Null` type; `ConstNull` may exist where semantics require, but operations are fully typed.

### SSA recap
- MIR stays in pruned SSA: each definition dominates its uses, and φ nodes model value merges at control-flow joins.
- Construction follows a dominance-frontier placement of φ nodes (pruned by liveness when possible) and a dominance-order renaming pass that assigns every definition a unique version.
- Leaving SSA happens only during lowering: insert parallel copies to materialise φ moves, split critical edges where necessary, and rely on copy coalescing before register/stack allocation.
- This SSA foundation powers sparse conditional constant propagation, GVN/CSE, aggressive dead code elimination, loop optimisations (LICM, strength reduction), and precise guard reasoning (null/bounds elimination). Memory/effect ordering is provided by the token or by a Memory SSA overlay as described below.

---

## Effect Model (Ordering)
- **Recommended:** single `EffectToken` (memory token) SSA chain. Every effectful operation (storage writes/reads, runtime interop, unknown call) consumes and produces the token. Pure/ReadOnly ops do not touch it; only pure ops move freely across tokens. ReadOnly ops may move across other reads but not writes.
- **Advanced alternative:** Memory/Effect SSA (`χ/μ`) to model regions. Adopt only if finer-grained motion outweighs complexity.

---

## Canonical Primitive Set
- **Values & CFG:** `Const*`, `Arg(i)`, `Phi`; terminators `Br`, `CondBr`, `Switch`, `Ret`, `Abort`, `Unreachable`.
- **Arithmetic/bitwise/compare/convert:** same as HIR but fully normalised (signedness flags, canonical predicates).
- **Aggregates/containers/bytes:** `StructPack/Get/Set`, `ArrayNew/Len/Get/Set`, `MapNew/Len/Get/Set/Has/Del`, `Concat` (ByteString), `Slice`, `BufferNew/Set/Copy`, `Length`.
- **Calls:**  
  `Call(fnRef, args)` with attributes: `IsPure`, `ReadOnly`, or `Unknown` (consumes token).  
  `Syscall(category, id/name, args, gasHint?)` with effect tag (`StorageRead/Write`, `Runtime`, `Interop`, `Crypto`). Examples: `Storage.Get/Put/Delete`, `Runtime.CheckWitness/Time/GasLeft/Notify/Trigger`, `Contract.Call`, `Crypto` hashes, Iterator/Ledger/StdLib per catalog.
- **Guards:** `GuardNull`, `GuardBounds`, `CheckedAdd/Sub/Mul` with `onFail` policy (`Abort` or branch target).
- **Static slots:** `StaticFieldLoad(slot, type)` and `StaticFieldStore(slot, value, type)` to model NeoVM static storage. Both consume and produce the effect token so that loads observe preceding stores deterministically.

---

## DevPack Normalisation
- Intrinsic binding resolves symbols to canonical syscalls with ids (per Neo N3 signatures), effect tags, and optional cost hints.
- StorageMap helpers lower to `StorageContext` + prefixed keys consistent with DevPack semantics (or remain canonical ops if the backend supports them directly).
- `Contract.Call`: explicit parameters (UInt160 script-hash, method name, `CallFlags`, packed argument array); effect = `Interop`; treat as barrier for container versioning unless flags mark it read-only.

---

## Optimisation Pipeline (Typical)
1. **Verify & Canonicalise**: type and Phi checks, signedness normalisation, guard sanity, effect-token chain connectivity. An initial sweep also removes unreachable blocks so subsequent passes operate on a minimal CFG.
2. **Sparse Dataflow**: SCCP to fold constants, prune dead branches/blocks, simplify switches; perform copy propagation and DCE.
3. **GVN/CSE**: value-number pure ops (and ReadOnly where safe regarding tokens).
4. **SROA**: scalar replacement of aggregates; eliminate redundant pack/unpack; keep aggregate lifetimes short.
5. **Containers & Bytes**: redundant read elimination using “versioned objects” (each write creates a new container version; reads CSE only within a stable version and absent global barriers). Fold repeated `Length/HasKey` on stable versions.
6. **Guards**: hoist dominating guards (LICM-aware), sink to nearest use, remove redundant guards proved by dominance/value-range analysis, merge equivalent fail paths.
7. **Loops**: loop simplify (normalize headers/latches), optional LCSSA, LICM across pure/read-only operations, induction analysis and strength reduction, tail duplication to linearise hot paths.
8. **Calls/ABI Hardening**: micro-inline trivial pure helpers, re-pack returns just-in-time, mark legal tail calls (optional).
9. **Cost Awareness** (optional): combine opcode-like costs with syscall gas hints to steer inlining/hoisting decisions.
10. **Final Verify**: ensure SSA integrity, guard targets, effect chain correctness, and LIR mapping coverage.

---

## Verification Rules
- SSA correctness: Phi placement, dominance, single terminators per block.
- Effect chain: single connected token from entry to exits; every effectful op consumes/produces it; pure ops leave it untouched.
- Syscall validation: ids from whitelist, signatures match intrinsic catalog, disallowed interops rejected.
- Guards: fail targets exist; `Abort` terminators used correctly; no hidden failures.

---

## Outputs & Metadata
- Selection-ready MIR per function.
- Cost summaries (instruction count, block count, estimated gas) per function.
- Intrinsic usage summary to inform manifest permissions.

---

## Testing Guidance
- Unit tests: verifier (Phi, effect token), SCCP/GVN/SROA/LICM/Guard transforms.
- Golden MIR dumps for key DevPack scenarios.
- Differential tests: run MIR→LIR→NeoVM (or neon) on representative snippets and compare behaviour/gas metrics.
