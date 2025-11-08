# IR System Design for the Neo C# DevPack Compiler
Layered HIR → MIR → LIR pipeline aligned with Neo N3 NeoVM and the C# DevPack ecosystem.

## Document Map
- `docs/HIR-Design.md` – detailed high-level IR specification and pipeline.
- `docs/MIR-Design.md` – detailed mid-level IR specification and optimisation playbook.
- `docs/LIR-Design.md` – detailed low-level IR specification, scheduling, and emission strategy.
- `docs/IR-LIR.md` – quick reference for LIR implementation notes and stack-focused guidance.
- `docs/HIR-Attributes.md` – mapping between Roslyn attributes and HIR metadata.

Audience: compiler and tooling engineers designing or extending the layered optimizing compiler for Neo N3 (NeoVM) using C# DevPack.  
Scope: end-to-end IR architecture, target-specific constraints, and pass pipelines.  
Non-goal: no source listings; this is a design reference and process guide.

---

## 0. Why Layered IR Matters
Neo C# DevPack compiles C# smart contracts. Existing tooling (e.g., neon) lowers IL straight to NeoVM with limited optimisation hooks. Introducing a structured IR stack (HIR → MIR → LIR) yields clearer semantics, safer and richer optimisations, explicit gas/size control, and a maintainable evolution path while staying fully compliant with Neo N3 constraints and DevPack APIs.

---

## 1. Design Goals and Principles
- **Target-aligned semantics**  
  Preserve relevant C# semantics in HIR, normalize to canonical primitives in MIR, and emit target-faithful sequences in LIR where every instruction maps 1:1 to a NeoVM opcode or syscall.
- **Determinism and security**  
  Enforce Neo N3 contract constraints: no floating point, bounded resources, deterministic interop; forbid or gate non-deterministic features at the earliest IR.
- **Gas/size observability**  
  Track instruction count, byte size, peak stack, and gas estimates across layers. Expose Release profiles (gas-focused vs. size-focused) and a Debug profile (mapping fidelity).
- **Composable middle-end**  
  Keep MIR in strict SSA with an explicit effects model so classic optimisations remain safe around Storage/Runtime/Contract operations.
- **Roslyn-first frontend**  
  Build from Roslyn’s `IOperation` + CFG for a sugar-reduced semantic view. Apply small controlled desugarings (foreach, using, initializers) only when semantics remain unchanged.

---

## 2. Neo N3 / NeoVM Alignment
- **VM model**  
  Stack-based (evaluation + alt stack), deterministic. Scalars: BigInteger, Bool; byte domains: ByteString (immutable), Buffer (mutable); containers: Array, Struct, Map, Iterator; special: Null, InteropInterface. No floating point; arithmetic uses arbitrary-precision integers (bounded by VM policy).
- **Syscalls and gas**  
  Interop services via `SYSCALL` with 4-byte ids; each opcode/syscall has a gas cost. Storage operations are expensive, crypto hashes moderate, some Runtime calls cheap but impure.
- **Contract artifacts**  
  Output: NEF (script + checksum) and manifest (name, methods, events, permissions, trusts, features, safe methods). Debug info optionally maps offsets to source.
- **Determinism policy**  
  Allowed: Neo N3 deterministic syscalls (Runtime.*, Storage.*, Crypto.*, Contract.* with CallFlags), Iterator/Ledger/StdLib per policy. Restrict environment-dependent services (oracle/network/time) unless explicitly whitelisted. Even when allowed (e.g., Runtime.Time), tag effects and preserve order.

---

## 3. Shared Concepts Across IR Layers
- **CFG + SSA**  
  Functions are CFGs; blocks terminate with a single terminator. HIR and MIR use pruned SSA for scalars (Phi at merges). LIR is non-SSA.
- **Type mapping**  
  - C# → IR:  
    `bool → Bool`, integral types → `Int` (width hints optional), `string → ByteString` (UTF-8), `byte[] → Buffer` or `ByteString`, serializable `struct/class → Struct`, `Dictionary → Map`, tuple → `Struct`.
  - IR → NeoVM:  
    Byte/Bool/BigInteger/ByteString/Buffer/Array/Struct/Map/InteropInterface/Null.
- **Effects and purity**  
  Effects classify side effects (Runtime, StorageRead, StorageWrite, Interop, Crypto, Abort, None). Pure = CSE/LICM eligible; ReadOnly may move across reads but not writes. Ordering enforced via effect tokens or memory SSA.
- **Attributes and metadata**  
  Capture method attributes (`[MethodImpl]`, custom `[Inline]`, `[Pure]`, `[ReadOnly]`, `[Safe]`, etc.) and propagate through IR for optimisation and manifest.  
  HIR signatures now embed attribute tags (`HirAttribute`) collected during import (e.g., `[DisplayName]`, `[Safe]`, `[Syscall]` on constructors). Downstream passes can query this metadata to preserve ABI annotations, respect read-only semantics, or recognise intrinsic aliases.
- **Source mapping**  
  IR nodes carry source spans. LIR → NEF emission preserves offset ↔ source mapping for debugging.
- **SSA primer**  
  Static Single Assignment (SSA) form means every value has exactly one defining assignment and every use references that unique definition; merges in the CFG are expressed with explicit φ (phi) nodes. SSA makes use-def chains trivial, models dominance directly, and drastically simplifies classical dataflow analysis. Key pillars: dominance (definitions must dominate their uses), φ-functions (merge values per predecessor), systematic renaming (each assignment becomes a fresh version such as `x0`, `x1`), and an effects story for memory (token-threading or memory SSA when sequencing is required).  
  Typical construction follows Cytron et al.: build the CFG/dominator tree, place φ nodes at dominance frontiers (optionally pruned by liveness), rename all definitions/uses with version stacks, and prune redundant φ/definitions through DCE. Practical variants include minimal/pruned/semi-pruned SSA, CSSA (edge-split) for cleaner lowering, and Memory SSA (χ/μ nodes) for side effects. Leaving SSA before codegen involves inserting parallel copies for φ moves, breaking critical edges where needed, then coalescing copies via register allocation heuristics. SSA directly powers sparse conditional constant propagation (SCCP), global value numbering (GVN/CSE), dead code elimination, loop optimisations (LICM, strength reduction), value-range and null/bounds-check elimination, and high-quality register allocation after out-of-SSA.

---

## 4. High-level IR (HIR)
Purpose: retain high-level C# semantics and types, normalize sugar into a compact node set, and perform semantic transforms needing high-level insight.
See `docs/HIR-Design.md` for a detailed catalogue of HIR types, nodes, and pipelines.

### Scope & boundaries
- Present: structured control flow (if/switch/loops), pattern constructs via CFG, optional exception modeling if policy allows temporary representation; generics to be monomorphized, closures to be converted, properties/indexers pre-lowering, DevPack intrinsics represented explicitly.
- Absent: floating point, unrestricted reflection, threads/async, nondeterministic syscalls (unless explicitly whitelisted).
- Effects: `IntrinsicCall` carries effect tags and optional gas hints; optional virtual effect token from HIR onward.

### Canonical nodes
Scalars/aggregates; arithmetic/bitwise/compare/convert; calls (`Call`, `CallVirt` → devirtualized `Call`); `IntrinsicCall` with `(category, name, args, effect, purity)`; checks (`NullCheck`, `BoundsCheck`, `Checked*`); control flow terminators (`Br`, `CondBr`, `Switch`, `Ret`, `Abort`, `Unreachable`, plus optional Try/Catch/Throw if supported temporarily).

### Transform highlights
- Import from Roslyn (`IOperation` + CFG), capture attributes, bind DevPack intrinsics via symbol catalog, apply limited desugaring (foreach, using).
- Legality filter: reject floats, dynamic binding, reflection, unsupported syscalls, threads/async/iterator.
- Semantic lowering: devirtualization, monomorphization, closure conversion, property/indexer normalization.
- Optimization: budgeted inlining, constant folding/propagation, copy propagation, DCE, CFG simplification, switch tuning, check insertion/elimination with dominance reasoning.
- Exception policy: lower Try/Catch/Throw to Abort or explicit flows before MIR if exceptions are disallowed.

### Output expectations
Pruned SSA with effect tags; only constructs representable in MIR; forbidden features already filtered.

---

## 5. Mid-level IR (MIR)
Purpose: strict SSA canonical IR for classic optimisations; no implicit exceptions; every operation must translate to NeoVM sequences.
See `docs/MIR-Design.md` for detailed MIR node definitions and pipelines.

### Scope & boundaries
No implicit exceptions; error paths modeled as explicit guards. Containers and byte ops remain as primitives. DevPack intrinsics are normalized to canonical syscalls with fixed signatures/effects.

### Canonical primitives
SSA values (`Const*`, `Arg`, `Phi`); control flow (`Br`, `CondBr`, `Switch`, `Ret`, `Abort`, `Unreachable`); arithmetic/bitwise/compare/convert; aggregate/container/byte primitives; `Call` with purity metadata; `Syscall(category,id,args)` with effect + optional gas hint; guards (`GuardNull`, `GuardBounds`, `Checked*`).

### DevPack normalization
Intrinsic catalog maps symbols to `(category, name, signature, effect, purity, syscall id)`. `Contract.Call` surfaces hash/method/flags/args; ensure `CallFlags` is constant when possible; effect = Interop. StorageMap helpers normalized to explicit storage context operations.

### Transform highlights
- Verify + canonicalize types, comparison forms, effect chains, guard structure.
- SCCP → DCE → CFG simplify; GVN/CSE for pure/readonly operations; algebraic simplification/strength reduction respecting BigInteger semantics.
- SROA/aggregate splitting; eliminate redundant pack/unpack; container read elimination using per-object version counters; treat storage writes as global kills.
- Guard hoist/sink/elimination; merge equivalent fail paths.
- Loop pipeline: loop simplify (+optional LCSSA), LICM respecting effect barriers, induction range analysis, tail duplication for fallthrough.
- Call/ABI tuning: micro-inline trivial pure helpers, scalarize args/returns, tag tail calls.
- Syscall normalization: finalize ids/signatures; treat crypto syscalls as CSE eligible, storage writes as strict ordering points.
- Accumulate opcode/syscall cost hints for release heuristics.

### Output expectations
Selection-ready MIR where each node maps to LIR; effectful operations ordered; CFG minimal and verified.

---

## 6. Low-level IR (LIR)
Purpose: target-close representation mapping 1:1 to NeoVM instructions/syscalls with explicit stack effects.
See `docs/LIR-Design.md` for a detailed LIR specification.

### Representation
- **VReg-LIR (optional):** virtual-register nodes capturing dependencies (`VAdd`, `VGetItem`, `VSyscall`, etc.) for a final simplification pass.
- **Stack-LIR (final):** NeoVM-aligned opcodes (`PUSH*`, `PUSHDATA*`, `DUP/SWAP/OVER/ROT/PICK/ROLL`, arithmetic/logic, container ops, control flow, `SYSCALL`, `ABORT`).

### Instruction selection (MIR → VReg-LIR)
Table-driven mapping; fold constant aggregates; combine compare + branch patterns into `NUMEQUAL/GT/...` plus conditional jumps; canonicalize intrinsics for unique lowering.

### Stack scheduling (VReg-LIR → Stack-LIR)
Minimize peak stack depth and shuffles using Sethi–Ullman numbering, commutativity tweaks, depth-limited `PICK/ROLL`, `DUP/OVER` for nearby reuses. Resolve Phi nodes with predecessor-edge moves; enforce block entry stack shapes.

### Peephole & layout
Cancel redundant shuffles (`DUP;DROP`, `SWAP;SWAP`), fold identities (`PUSH0;ADD`), deduplicate `GETITEM`/`LENGTH` on stable operands, reverse conditions for fallthrough, linearize hot paths, remove empty blocks, thread jumps.

### Constants & encoding
Pick shortest push forms; prefer `DUP` over re-pushing large immediates; reuse constants by deferring drops; finalize syscall ids and store 4-byte payloads; select gas/size-optimal encodings.

### Verification & metrics
Simulate stack to catch underflow and record peak depth; validate jumps/immediates/syscalls; emit offset→source maps; report instruction count, code size, gas estimate.

### Pipeline
1. Instruction selection.  
2. Stack scheduling with Phi resolution and block-alignment.  
3. Peephole clean-up (multi-round).  
4. Block layout and jump tightening.  
5. Constant/encoding minimization.  
6. Verification, resource estimation, dumps.

### Build hints
Debug: cap aggressive scheduling and peepholes, maintain stable layout. Release gas/size: enable deep scheduling (including deep `PICK` rewrites), run full peephole suite, bias layout per target (hot-path vs. compact).

---

## 7. Effects, Purity, and Safety (DevPack-centric)
- **Effects taxonomy:** `StorageRead`, `StorageWrite`, `Runtime`, `Interop`, `Crypto`, `Abort`, `None`.
- **Policy enforcement:** HIR verification bans disallowed interops (configurable); MIR checks syscall signatures/effects; storage and contract calls treated as ordering barriers.
- **Alias strategy (MIR):** versioned-object heuristic—each write assigns a new version id; reads can be CSE’d only when version id matches and no global barrier intervenes (storage writes, unknown interops, etc.).

---

## 8. ABI, Manifest, and Artifacts
- HIR preserves language-level signatures; MIR enforces explicit receiver + single-return (aggregate for multi-values); LIR obeys NeoVM stack calling conventions.
- Manifest generation derives from IR metadata: methods, parameters, returns, events, safe methods, permissions/trusts/features inferred from intrinsics.
- Methods marked with `[NoReentrant]`/`[NoReentrantMethod]` surface in the manifest under `extra.reentrancyGuards`, carrying the guard prefix/key so security tooling can suppress false positives.
- Inline hints (`[MethodImpl(MethodImplOptions.AggressiveInlining)]`) are preserved in HIR so middle-end optimisation can consult them.
- NEF emission encodes script + checksum; calculates contract hash if required; reports code size and peak stack.
- Debug information maintains offset→source mapping; selection/scheduling must preserve spans.

---

## 9. Frontend Notes (Roslyn Integration)
- Inputs: Roslyn `Compilation`/`SemanticModel`; attributes via `ISymbol.GetAttributes`; semantics via `IOperation` + CFG.
- Intrinsic binding: map `IMethodSymbol` to DevPack entry via intrinsic catalog (category, signature, effect, purity, syscall id, cost hint).
- Controlled desugaring: foreach → enumerator + `Dispose`; `using` → `try/finally`; object/collection initializers → `new` + assignments/adds. Avoid async/iterator rewriting; either model or reject unsupported constructs.

---

## 10. Optimization Profiles
- **Debug:**  
  HIR – Legalize, limited inline, const fold, DCE, control-flow simplify.  
  MIR – Verify, SCCP, copy-prop/DCE, limited GVN, Verify.  
  LIR – Selection, cautious scheduling, minimal peephole, Verify, emit rich sourcemaps.
- **Release (size):**  
  HIR – Legalize, devirtualize/monomorphize, inline under size budget, const fold/GVN-lite, check insertion/pruning, DCE/CF simplify.  
  MIR – Verify, SCCP, GVN/CSE, SROA, copy-prop/DCE, guard hoist/sink, LICM-lite, tail duplication, Verify.  
  LIR – Selection, scheduling biased to fewer shuffles, size-focused peephole, fallthrough layout, constant encoding minimization, Verify.
- **Release (gas/throughput):**  
  HIR – As above with more aggressive inlining and guard elimination when provably safe.  
  MIR – Full SCCP/GVN/CSE/SROA/LICM, loop transforms, container redundancy elimination, ABI hardening, cost-guided inliner.  
  LIR – Selection, aggressive scheduling, multi-round peephole, hot-path layout, Verify, emit cost reports.

---

## 11. Verification & Diagnostics
- **HIR:** type + SSA checks; reject floats/async/iterator/reflection/disallowed interops; ensure intrinsic metadata; effect token sanity.
- **MIR:** SSA dominance, guard target validity, effect token single chain, syscall whitelist.
- **LIR:** stack balance per instruction/block, terminators valid, encodable immediates/jumps, syscall ids resolvable.
- **Observability:** IR dumps, DOT graphs, per-pass metrics (blocks, instructions, peak stack, size, gas), source-mapped diagnostics.

---

## 12. Cost and Resource Modeling
- Maintain opcode cost tables aligned with current NeoVM releases.
- Syscall cost hints come from intrinsic catalog and network policy.
- Combine opcode + syscall costs for worst-case and average path estimates; use in optimisation heuristics and final reports.

---

## 13. Testing Strategy
- **Unit:** IR validators, individual passes, selector/scheduler micro-tests, peephole catalog cases.
- **Golden:** stable textual dumps for representative contracts (Storage, CheckWitness, Contract.Call, events). Review diffs per pass change.
- **Differential:** compare behaviour and gas/size against neon/emulator for matching snippets; fuzz small programs for semantic invariants (stack safety, guard correctness).
- **End-to-end:** compile and execute on NeoVM emulator; verify storage/log outputs and manifest permissions.

---

## 14. Extensibility & Versioning
- **Intrinsic catalog:** central registry covering Storage/Runtime/Contract/Crypto/Iterator/Ledger/StdLib with signatures, effect, purity, syscall id, cost hint.
- **IR schema versioning:** semantic version per layer; document migrations; version dumps.
- **Backend targets:** HIR/MIR remain largely target-neutral, enabling alternative back-ends (other stack/register VMs) by swapping LIR/selector/scheduler.

---

## 15. DevPack Practical Notes
- Treat integers as `Int` with BigInteger semantics; respect checked/unchecked contexts.
- Normalize strings to `ByteString` (UTF-8); differentiate when mutable `Buffer` is required.
- Recognize StorageContext/StorageMap idioms; normalize to explicit Storage calls.
- Map event emissions to `Runtime.Notify` with structured payloads and reflect in manifest.
- Ensure `Contract.Call` uses compile-time `CallFlags`; treat as Interop effect and alias barrier.
- Maintain determinism via syscall allowlists; consistently tag Runtime.Time if admitted.
- Track resource limits (stack depth, instruction count) and warn when nearing boundaries.

---

## 16. Acceptance Criteria
- Implement HIR/MIR/LIR designs with validators and dumps; intrinsic catalog covers core DevPack namespaces.
- Frontend produces HIR from Roslyn for representative contracts; forbidden features rejected at HIR verification.
- MIR optimizations yield selection-ready IR; LIR passes verify; NEF + manifest + debug map emitted.
- Regression metrics (gas/size/peak stack) tracked; CI golden tests and differential runs against NeoVM (and optionally neon) remain green.

---
