# High-level IR (HIR) Design

Detailed notes for the Neo C# DevPack compiler’s HIR layer. Complements the overview in `docs/README-IR-Design-Neo.md` and feeds into the MIR/LIR design documents.

---

## Purpose
- Preserve rich C# semantics and types while normalizing syntactic sugar into a compact, stable primitive set.
- Host semantic transforms that require language knowledge: monomorphisation, closure conversion, devirtualisation, property/indexer normalisation, attribute-driven inlining, and safety-check insertion/pruning.
- Emit a pruned-SSA, target-neutral IR that lowers cleanly to MIR and, ultimately, NeoVM bytecode.

---

## Scope & Target Constraints
- Target platform: Neo N3 / NeoVM (stack machine, deterministic execution, no floating point).
- DevPack API surface (Storage, Runtime, Contract, Crypto, Iterator, Ledger, StdLib) represented as intrinsics carrying effect metadata.
- Disallow or gate reflection, threads/async/iterator state machines, and policy-restricted nondeterministic APIs (e.g., `Runtime.Time` configurable).

---

## Design Principles
- **Effect awareness:** tag intrinsics with effect categories; optionally thread a virtual effect token to constrain reordering (the definitive effect chain is enforced in MIR).
- **Canonical forms:** normalise language constructs (properties, indexers, pattern forms) into a small node set to reduce pass surface and backend complexity.
- **Diagnostics fidelity:** carry source spans and attributes for precise diagnostics, source maps, and attribute-driven optimisation decisions.

---

## Type System (C# → HIR)
- Scalars: `Bool`, `Int` (BigInteger semantics with optional width hints), `ByteString` (UTF-8), `Buffer` (mutable byte array), `Null` (HIR only), `Void`.
- Aggregates: `Array<T>`, `Struct{fields}`, `Map<K,V>`. `Tuple`/`ValueTuple` map to `Struct`. Serializable `class`/`struct` map to `Struct` semantics before or during MIR lowering.
- C# mappings:
  - `bool → Bool`
  - integral types (`sbyte`…`ulong`, `BigInteger`) → `Int`
  - `string → ByteString`
  - `byte[] → Buffer` or `ByteString` (based on mutability/API)
  - `struct`/`class` (supported subset) → `Struct`
  - `Dictionary` → `Map`, `Tuple` → `Struct`
- Unsupported: floating point, `dynamic`.

---

## Control Flow & SSA
- Functions are CFGs ending with a single terminator (`Br`, `CondBr`, `Switch`, `Ret`, `Abort`, and optionally `Throw` if exceptions are temporarily modelled).
- Use pruned SSA for scalars: Phi nodes at merge blocks, dominance-based renaming, values and terminators carry `SourceSpan`.
- Optional: maintain a virtual effect token SSA value if early effect sequencing is desired (recommended off in HIR; enforced at MIR).

### SSA specifics
- **Definition.** In SSA every variable version is assigned exactly once and every use references that dominating definition. Control-flow merges expose φ nodes that select the incoming value per predecessor; φ nodes are compile-time merge constructs, not runtime instructions.
- **Why it matters.** SSA enables sparse dataflow (SCCP), straightforward GVN/CSE, aggressive dead code elimination, value-range reasoning, loop optimisations (LICM, strength reduction), and sets up high-quality register allocation once lowered.
- **Core mechanics.** Dominance governs legality (`def` must dominate all `use`s). Construction walks the dominance tree: place φ nodes at dominance frontiers (prefer pruned SSA guided by liveness), rename assignments to fresh versions (`x0`, `x1`, …) while tracking stacks per symbol, and drop unused φ/definitions. Side effects stay outside of scalar SSA: thread a memory/effect token or adopt Memory SSA (χ/μ) when sequencing must be explicit.
- **Lowering back out.** Before leaving SSA, insert parallel copies to materialise φ moves, split critical edges when needed, then rely on copy coalescing or allocator heuristics to minimise resulting moves.

---

## Canonical Node Set
- **Constants & parameters:** `ConstInt/Bool/ByteString/Null`, `Arg(i)`; locals appear as SSA temps.
- **Arithmetic / bitwise / compare:** `Add`, `Sub`, `Mul`, `Div`, `Mod`, `BitAnd/Or/Xor/Not`, `Shl`, `Shr`, `Cmp` (`Eq/Ne/Lt/Le/Gt/Ge` with signedness).
- **Conversions:** `Conv(SignExtend | ZeroExtend | Narrow | ToBool | ToByteString | ToBuffer)`.
- **Aggregates & containers:** `NewStruct`, `StructGet/Set`, `ArrayNew/Len/Get/Set/Slice`, `MapNew/Len/Get/Set/Has/Del`.
- **Calls & intrinsics:** `Call(calleeSymbol, args)` for regular calls (with `CallVirt` retained only until devirtualised); `IntrinsicCall(category, name, args, effect, purity?)` for DevPack APIs.
- **Safety checks:** `NullCheck`, `BoundsCheck`, `CheckedAdd/Sub/Mul` with `FailPolicy` (`Abort`, `Assume`, `PathSplit`). The current optimisation pipeline prunes guards emitted with the `Assume` policy because they introduce no observable behaviour.
- **Control flow:** `Br`, `CondBr`, `Switch`, `Ret`, `Abort`, `Unreachable`, plus optional `Try/Catch/Finally/Throw` pending policy (must be lowered pre-MIR).
- **Static storage:** `StaticFieldLoad(slot, type)` / `StaticFieldStore(slot, value, type)` for NeoVM static-field accesses. The importer allocates slots through `CompilationContext.AddStaticField`, and these nodes carry `Runtime` effects so MIR can sequence them against other memory operations.

---

## DevPack Intrinsics (Catalog-bound)
- Storage: `Get` (`StorageRead`), `Put/Delete` (`StorageWrite`).
- Runtime: `CheckWitness`, `GasLeft`, `Time`, `Notify`, `Trigger` (effect = `Runtime`; gating applied to `Time`).
- Contract: `Call` (effect = `Interop`, `CallFlags` validated).
- Crypto: `SHA256`, `RIPEMD160`, `Keccak256`, `Murmur32` (effect = `Crypto`, treat as pure).
- Iterator / Ledger / StdLib: categorised with effects and normalised signatures.
- Each `IntrinsicCall` embeds `(category, name, effect, optional purity, optional gas hint)` sourced from the intrinsic catalog.
- Constructors and helpers annotated with `[Syscall]` (e.g., `StorageMap` initialisers) also route through the intrinsic catalog, ensuring the importer emits `HirIntrinsicCall` rather than generic `HirCall`.

---

## Frontend Construction (Roslyn)
- Build HIR from Roslyn `IOperation` + `ControlFlowGraph`, obtaining a sugar-reduced, bound semantic tree with explicit CFG (dominance/post-dominance).
- Short-circuit bools, pattern matching, `using`, `foreach` arrive in uniform IO/CFG form.
- Controlled desugaring (optional pre-HIR):  
  `foreach` → enumerator + `try/finally.Dispose`;  
  `using` → `try/finally.Dispose`;  
  object/collection initialisers → `new` + assignments/add calls.  
  Avoid async/iterator/LINQ rewrites—either reject or rely on IO normalisation.
- Attributes: capture `[MethodImpl]`, `[Inline]/[NoInline]`, `[Pure]`, `[ReadOnly]`, `[Safe]`, etc., and attach to `HirFunction`/`Call`.

---

## HIR Transforms & Pipeline
- **Legality filter:** reject floats, dynamic, reflection, threads/async/iterators, disallowed intrinsics.
- **Semantic lowering:** devirtualise `CallVirt` when receiver type is known; monomorphise generics (emit closed instances, deduplicate clones); convert closures (lambda → static function + environment struct); normalise properties/indexers (map to methods or `StructGet/Set`).
- **Optimisation:** attribute/budget-guided inlining (small wrappers, pure helpers; avoid crossing effectful calls unless `ReadOnly`), constant folding/propagation, algebraic simplification, boolean normalisation, copy propagation, DCE, CFG simplification (empty-block merges, straight-line merges), dead-branch elimination, switch normalisation (dense vs. sparse metadata). Insert null/bounds/checked checks and remove dominated checks using dominance/range analysis. *(Implementation status: the current pipeline performs constant folding, phi simplification, dead-code elimination for pure values, constant-branch simplification, and removal of `Assume` guards, followed by unreachable-block pruning; the remaining stages are planned.)*
- **Exception policy:** if temporary exception modelling is allowed, lower to `Abort` or explicit error-code blocks before MIR.

### Attributes & Metadata
- Roslyn symbols on methods and their accessors feed into `HirAttribute`s when building signatures. Current mapping recognises:
  - `[DisplayName]` → `HirMethodNameAttribute`, preserving ABI/event aliases.
  - `[Safe]` → `HirSafeAttribute`, signposting read-only semantics for manifest generation and optimisation policy.
  - `[Syscall]` (including on constructors) → `HirIntrinsicTagAttribute`, linking the method to an intrinsic entry even when the call is expressed via C# surface syntax.
  - `[NoReentrant]` / `[NoReentrantMethod]` → `HirNoReentrantAttribute`, preserving the guard prefix/key used by DevPack modifiers (for method variants the key defaults to the method name).
  - `[MethodImpl(MethodImplOptions.AggressiveInlining)]` → `HirInlineAttribute`, allowing optimisation passes to see user-specified inline hints.
- Attribute extraction is idempotent; duplicates across property accessors and backing methods collapse to a single tag.
- Manifest/ABI generation prefers these tags when constructing method entries, falling back to Roslyn reflection only if the HIR pipeline is disabled.
- Null-safety helpers (`EnsureNonNull`) now materialise explicit `HirNullCheck` instructions so guard lowering into MIR produces concrete checks that are verifiable downstream.

---

## Invariants & Verification
- Pruned SSA with well-typed instructions and single terminators per block.
- No unsupported features or unrecognised intrinsics; DevPack calls tagged with effects.
- Optional effect token (if used) forms a single chain covering all effectful operations.
- All nodes carry `SourceSpan` for debugging.

---

## Outputs & Metadata
- HIR functions with normalised signatures, attributes, effect annotations, and source locations.
- Collected intrinsic dependencies (useful for manifest permission inference).
- Optional early metrics: instruction count, block count, effect summary.

---

## Testing Guidance
- Maintain golden HIR dumps for representative DevPack scenarios (Storage CRUD, `CheckWitness`, `Contract.Call`, events).
- Unit-test the HIR verifier (SSA dominance, Phi well-formedness, effect tagging, check insertion).
- Differential tests: compare behaviour (via interpreter or end-to-end pipeline) against expected NeoVM execution for small programs.
