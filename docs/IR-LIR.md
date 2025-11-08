# Low-Level IR Overview

> For the broader HIR → MIR → LIR architecture and optimisation pipelines, see `docs/README-IR-Design-Neo.md`. For the complete LIR specification, refer to `docs/LIR-Design.md`.

## Goals
- Model the compiler backend with NeoVM-aligned instructions where every Stack-LIR opcode maps one-to-one to a NeoVM opcode or syscall.
- Use a two-phase representation (VReg-LIR -> Stack-LIR) so that expression-level optimisations can run before explicit stack scheduling.

## Representation Layers
- **VReg-LIR:** SSA-like virtual register graph. Nodes carry lightweight `LirType` tags and optional source spans and remain agnostic of stack layout to enable global reordering.
- **Stack-LIR:** Final NeoVM-style instruction stream. Each instruction has a precise stack effect and basic blocks terminate with `JMP`/`JMPIF`/`RET`/`ABORT`.

## Instruction Selection
- Table-driven mapping from MIR primitives to `VNode` instances. Syscall identifiers are fixed during selection, and container operations map one-to-one.
- Early peepholes: constant folding, algebraic identities such as `Add(x, 0)`, and collapsing pure constant containers.

## Stack Scheduling
- Apply Sethi-Ullman numbering to choose evaluation order and minimise peak stack height.
- Insert shuffle instructions sparingly: favour `DUP`/`OVER`/`SWAP`, cap deep `PICK`/`ROLL`, and rewrite into cheaper sequences when depth grows.
- Eliminate phi nodes by normalising block entry stack shapes; predecessor edges perform the required moves.

## Peephole Optimisation (Stack-LIR)
- Shuffle cancellation: `DUP : DROP`, `SWAP : SWAP`, `ROT : ROT : ROT`.
- Constant algebra: `PUSH0 : ADD` -> drop, convert small integer pushes to smallest encoding.
- Control flow tightening: remove jumps to the next block, merge empty linear blocks, reorient conditions to prefer fallthrough.

## Resource Estimation and Validation
- Stack simulation yields maximum depth and flags underflow; use as regression metric.
- Instruction and gas estimates come from a base cost table plus syscall hints to characterise hot paths.
- Validation checks stack balance, known jump targets, immediate ranges, and syscall whitelisting.

## Debugging & Mapping
- Stack-LIR instructions carry `SourceSpan` that feeds into an offset->source map during emission.
- Dump facilities: textual VReg/Stack-LIR views, Graphviz (blocks + edges + annotated stack heights), verifier trace, and statistics output.

## Encoding & Emission
- Two-pass encoding: first pass records label offsets, second fills relative jump immediates.
- Constant encoding strategy picks the shortest `PUSH*`/`PUSHDATA*` form; large constants favour reuse via `DUP`.
- Output artefacts: NEF bytecode, manifest, debug map (offset->span).

## Alignment with NeoVM
- Every Stack-LIR opcode has an explicit NeoVM opcode counterpart; no hidden backend transformations.
- Relative offsets for control flow, canonical syscall identifiers, and strict adherence to container/bytestring semantics.
- Configuration knobs: debug builds keep scheduling conservative for stable source mapping; release profiles skew for gas or size depending on target.

## Testing Strategy
- Golden dumps for VReg-LIR, Stack-LIR, and emitted bytecode to guard against accidental changes.
- Semantic regression by diffing execution with the NeoVM interpreter for representative contracts.
- Fuzzing: random expression/control-flow generation, verifying stack discipline and behavioural parity.

## Optimisation System (HIR → MIR → LIR)
- General principles:
  - Determinism & safety: allow only NeoVM-whitelisted syscalls, forbid non-deterministic primitives and floating point, and respect the effect-ordering discipline of each layer.
  - Layer responsibilities:
    - **HIR:** high-level legalisation, monomorphisation, closure conversion, CFG shaping, semantic checks.
    - **MIR:** strict SSA optimisations (SCCP, GVN/CSE, LICM, copy propagation, DCE, loop work, guard reduction, SROA, ABI tightening).
    - **LIR:** instruction selection, stack scheduling, peephole clean-up, layout/jump tightening, constant encoding, resource estimation.
  - Metrics: peak stack depth, instruction count, byte size, gas estimates. Debug builds favour stable mappings; release builds enable gas/size-aware heuristics.

### HIR Optimisation
- Goals & invariants:
  - Preserve high-level semantics and rich types while eliminating unsupported features before MIR.
  - Build pruned SSA with effect tokens limiting motion; keep structured control flow when viable.
  - Tag intrinsics with purity/read-only metadata.
- Key passes:
  - Legalise/canonicalise syntax sugar (e.g., `foreach`, pattern matching, LINQ), surface struct/array ops, normalise strings to UTF-8 byte strings, reject banned constructs early.
  - Monomorphisation & closure conversion: instantiate generics per use, reuse duplicates, lower lambdas to static functions with explicit environment structs, staticise delegates.
  - Devirtualise and inline: turn `callvirt` into direct calls when type flow allows; inline small/pure helpers with budgets and recursion guards; avoid crossing effectful boundaries unless declared safe.
  - Simplify: constant folding/propagation (including byte strings), algebraic identities, copy propagation, DCE, dead-branch elimination, empty-block merging, switch density tuning.
  - Check strategy: insert null/bounds checks as required, remove dominated checks, choose abort vs. guard splitting for later MIR handling.
  - Intrinsic normalisation: unify DevPack signatures, tag purity (`Crypto.*` pure, `Storage.Get` read-only, `Storage.Put` write) to guide motion.
  - Control-flow shaping: loop simplify, optional tail-call recognition, branch inversion for consistent patterns.
- Verification & observability:
  - Type/SSA verifier, effect-token sanity, unsupported-node detection.
  - Dumps (`-fdump-hir`) for regression; debug builds limit aggressive motion for mapping fidelity.
- Recommended pipeline:
  1. Legalise + intrinsic binding.
  2. Monomorphisation, closure conversion, devirtualisation.
  3. Budgeted inlining.
  4. Constant folding/SCCP, GVN/CSE, copy propagation.
  5. Check insertion & reduction (interleaved as needed).
  6. DCE, control-flow simplification, switch tuning.
  7. Verify + dump.

### MIR Optimisation
- Goals & invariants:
  - Maintain strict SSA with explicit guards and no implicit exceptions.
  - Thread effect tokens through all side-effecting instructions; pure/read-only ops can move between tokens.
  - Ensure every node maps directly to LIR sequences or syscalls.
- Key passes:
  - Verify SSA dominance, guard targets, token uniqueness; canonicalise comparisons/casts.
  - SCCP → DCE → CFG simplify; GVN/CSE for pure ops and tagged syscalls; algebraic simplifications and strength reduction where semantics permit.
  - SROA/aggregate splitting to keep struct/map values scalar and short-lived.
  - Container redundancies: eliminate repeated `Length/HasKey/Get` when SSA definitions match and no intervening writes occur (per-object version counters); treat storage/runtime calls as global write barriers.
  - Guard optimisation: hoist to dominating points, sink closer to uses, remove redundant guards, merge equivalent failure paths.
  - Loop work: loop simplify + optional LCSSA, LICM for pure/read-only ops, induction/range analysis for bounds elimination, tail duplication to streamline hot paths.
  - Call/ABI tuning: micro-inline trivial pure helpers, scalarise arguments/returns post-SROA, flag legal tail calls.
  - Syscall normalisation: fix IDs and signatures; treat crypto syscalls as CSE-capable, enforce strict ordering for storage writes.
- Verification & observability:
  - SSA/token verifier, syscall signature checks, guard validation.
  - Dumps (`-fdump-mir`) and instruction/block statistics per pass.
- Recommended pipeline:
  1. Verify + canonicalise.
  2. SCCP → DCE → CFG simplify.
  3. GVN/CSE + algebraic simplification + copy propagation.
  4. SROA + follow-up copy propagation/DCE.
  5. Guard hoist/sink/elimination.
  6. Loop simplify, LICM, tail duplication.
  7. Container redundancy elimination (version-based).
  8. ABI hardening (argument/result scalarisation).
  9. Verify + dump + resource estimates.

### LIR Optimisation
- Goals & invariants:
  - “What you see is what NeoVM executes”: each Stack-LIR opcode maps 1:1 to NeoVM instructions or syscalls.
  - No explicit tokens—effect ordering is preserved by selection and layout; prioritise stack depth, shuffle count, jump count, size, and gas.
- Key stages:
  - Instruction selection: pattern-match MIR nodes to VReg-LIR, fold constant aggregates, shorten compare+branch pairs, canonicalise intrinsics for unique lowering paths.
  - Stack scheduling: apply Sethi-Ullman numbering, exploit commutativity, maintain a stack model (`DUP/OVER/SWAP/PICK/ROLL` with depth limits), resolve φ nodes on predecessors, enforce block entry shapes.
  - Peephole/layout: cancel redundant shuffles, fold identities (`PUSH0` + `ADD`), deduplicate repeated gets/lengths, reverse conditions for fallthrough, linearise hot paths, remove empty blocks.
  - Constant/encoding optimisation: pick shortest push forms, prefer `DUP` over re-emitting large immediates, reuse constants, fix syscall IDs.
  - Verification/metrics: simulate stack effects (record peak depth), validate immediates and syscall IDs, emit source maps, collect instruction/byte/gas counts.
- Recommended pipeline:
  1. Instruction selection (MIR → VReg-LIR).
  2. Stack scheduling with φ resolution and block entry alignment.
  3. Peephole passes to fixed point.
  4. Block layout & jump tightening.
  5. Constant/encoding minimisation.
  6. Verify + resource estimation + dumps.
- Build hints:
  - Debug: cap aggressive scheduling/peepholes, maintain stable layout for debugability.
  - Release (gas/size): enable deep scheduling heuristics, run full peephole suite, bias layout according to target (hot-path vs. compact).

### Configuration Matrix & Metrics
- Build modes:
  - **Debug:** limit inlining, tone down LICM/loop motion, preserve checks, reduce layout rewrites for predictable debugging.
  - **Release-gas:** aggressive but budgeted inlining, enable GVN/CSE/SROA/guard elimination/LICM, allow deeper stack rewrites, bias layout for hot paths.
  - **Release-size:** conservative inlining/unrolling, prefer shuffle sequences that minimise byte size, avoid block duplication.
- DevPack focus:
  - Treat `Crypto.*` as pure (eligible for CSE/LICM), `Runtime.*` as read-only (do not cross writes), `Storage.*` as write-sensitive (strict ordering).
  - Differentiate `Contract.Call` permissions (read/write) when modelling effects.
- Regression tracking:
  - Report instruction/block counts, peak stack depth, byte size, gas estimates after key passes.
  - Use fuzzing/random IR generation with NeoVM comparison for semantic guards.
  - Maintain golden HIR/MIR/LIR dumps to keep pipelines reproducible.

## SSA Primer
- Definition: Static Single Assignment (SSA) is an IR property where every variable is written exactly once and every read observes a unique, dominating definition. Control-flow joins introduce φ (phi) merge nodes that are compile-time constructs rather than runtime instructions.
- Motivation:
  - Trivial use-def chains because each name has one defining instruction.
  - Enables sparse, near-linear optimisations such as SCCP, GVN/CSE, dead code elimination, LICM, and value-range analysis.
  - Improves register allocation quality after φ-related copies are coalesced when leaving SSA.
  - Makes control-flow merges explicit instead of relying on implicit copies.
- Core concepts:
  - Dominance: a definition must dominate all of its uses in the control-flow graph.
  - φ nodes select the predecessor-specific value at merge blocks.
  - Renaming assigns fresh versions (`x0`, `x1`, …) to every write and rewrites uses to the nearest dominating version.
  - Side effects require additional modelling (memory SSA, χ/μ nodes, or explicit effect tokens) because plain SSA covers only scalar values.
- Example:
  ```c
  // Source
  int x = 0;
  if (c) { x = 1; }
  else   { x = 2; }
  y = x + 3;
  ```
  ```plain
  // SSA
  x0 = 0
  if (c) goto B1 else B2
  B1: x1 = 1; goto B3
  B2: x2 = 2; goto B3
  B3: x3 = φ(x1, x2)
  y0 = x3 + 3
  ```
- How SSA is built:
  1. Construct the control-flow graph and dominance tree.
  2. Place φ nodes at dominance frontiers of each definition (Cytron et al. algorithm).
  3. Rename variables by walking the dominance tree with per-variable version stacks.
  4. Prune unused φ nodes and dead definitions.
- Common variants:
  - Minimal SSA inserts every theoretically required φ node.
  - Pruned SSA (standard practice) uses liveness to avoid redundant φ nodes.
  - Semi-pruned approximates pruned SSA at lower cost.
  - CSSA and edge-split forms simplify later phases such as code motion.
  - Memory SSA threads memory versions (χ/μ functions) or uses a single “memory token” SSA value to sequence effects.
  - SSI (Static Single Information) splits values on predicates to encode path-specific facts.
- Leaving SSA before code generation:
  - Insert parallel copy sequences on incoming edges to realise φ semantics.
  - Break critical edges if necessary to host the copies.
  - Coalesce copies aggressively prior to register allocation.
  - Leverage SSA-friendly allocators (graph colouring, PBQP) with copy coalescing.
- Optimisations that benefit directly: SCCP, constant folding, GVN/CSE, dead code elimination, LICM, loop optimisations, and guard/bounds-check elimination.
- Implications for Neo compiler layers:
  - HIR: build pruned SSA early, retain φ nodes, and thread an effect token through Storage/Runtime/Contract intrinsics.
  - MIR: remain in strict SSA after lowering high-level constructs; run SSA-driven passes (SCCP, GVN, LICM, SROA, guard hoisting) and consider memory SSA or lightweight versioning for containers/storage.
  - LIR: eliminate SSA prior to stack lowering by inserting edge copies; rely on instruction selection and scheduling to preserve effect order.
- References:
  - Cytron et al., “Efficiently Computing Static Single Assignment Form and the Control Dependence Graph”, 1991.
  - Briggs et al., “Practical Improvements to the Construction and Destruction of SSA Form”, 1998.
  - Sreedhar & Gao, “A Linear Time Algorithm for Placing φ-nodes”.
