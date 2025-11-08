# Neo C# DevPack Compiler – Structure and Architecture

This document describes the structure, layering, and responsibilities of the Neo C# DevPack compiler in this repository. It is a living guide for contributors and users to understand where things live, how data flows, and how to extend or debug the compiler.

Audience: compiler engineers, DevPack contributors, and advanced users.  
Scope: project structure, module responsibilities, IR layering (HIR → MIR → LIR), pipelines, verification, emission, configs, extensibility, and testing.  
Non‑goal: this file does not include source code; see component‑specific docs under `docs/`.

---

## 1. High‑level Overview

- Target: Neo N3 (NeoVM) smart contracts written in C# via DevPack.
- Input: C# source (Roslyn analysis, no IL emission required).
- Output: NEF script (Neo executable format), manifest, and optional debug info (source map).
- Architecture: three‑layer IR pipeline (HIR → MIR → LIR) with verifiers and pass pipelines at each stage.
- Determinism & safety: no floating point; interop/syscalls are whitelisted; side effects are explicitly ordered.

---

## 2. Project Layout (enforced)

All compiler code lives under the Compiler project: `src/Neo.Compiler.CSharp`. Solution scaffolding, docs, configs, and tests live outside but contain no compiler logic.

```
.
├─ neo-devpack-dotnet.sln               # Solution only
├─ docs/                                # Design docs and reference (no code)
├─ configs/                             # Profiles, syscall policies, cost models
├─ src/
│  ├─ Neo.Compiler.CSharp/              # CORE COMPILER PROJECT (all compiler code here)
│  │  ├─ Frontend/                      # Roslyn + Intrinsics + Policy gate
│  │  ├─ IR/                            # HIR/MIR/LIR definitions, verifiers, printers
│  │  ├─ Lowering/                      # HIR→MIR, MIR→LIR
│  │  ├─ Passes/                        # HIR/MIR/LIR optimization/transforms
│  │  ├─ Backend/                       # Emitter, Manifest, DebugInfo, CostModel, Reports
│  │  └─ Utils/                         # Shared utilities (analyses, helpers)
│  └─ Neo.Compiler.CSharp.CLI/          # OPTIONAL CLI host (no compiler logic)
└─ tests/                               # Unit, golden, e2e, fuzz (no compiler logic)
```

Code ownership rule:
- Do not add compiler logic outside `src/Neo.Compiler.CSharp`.
- The CLI project is a thin host that references the compiler assembly.

---

## 3. Subsystems and Responsibilities

### 3.1 Frontend (`src/Neo.Compiler.CSharp/Frontend`)
- Roslyn Integration:
  - Build `Compilation/SemanticModel`, obtain `IOperation` + `ControlFlowGraph` (CFG).
  - Optional controlled desugaring (safe syntactic rewrites: `foreach`, `using`, object/collection initializers).
- Intrinsic Catalog:
  - Maps DevPack symbols (Storage/Runtime/Contract/Crypto/Iterator/Ledger/StdLib) to “intrinsic” descriptors: category, name/id, signature, effect, purity, and cost/gas hints.
- Policy Gate:
  - Enforces determinism and platform policy: bans floats, reflection, async/iterators, and disallowed syscalls.
- Smart-contract surface detection:
  - Public, non-abstract subclasses of `Neo.SmartContract.Framework.SmartContract` are treated as contract entry points; their public methods become ABI methods (respecting `[DisplayName]`, `[Safe]`, inline, and reentrancy annotations) while C# `event` declarations surface as manifest events with delegate parameter metadata.

Output: normalized, policy‑checked HIR with attributes and source spans.

### 3.2 IR Layering (`src/Neo.Compiler.CSharp/IR`)
- Common:
  - Shared types (Bool, Int, ByteString, Buffer, Array, Struct, Map, Void, Null(HIR only)), source spans, attributes, and general analyses (dominator tree, dominance frontiers, liveness, loop detection).
- HIR (High‑level IR):
  - High‑level nodes (structured control flow, intrinsics, checks).
  - Pruned SSA for scalars recommended; optional virtual effect token.
  - Verifier (types/SSA/features) and printer (text / DOT).
- MIR (Mid‑level IR):
  - Strict SSA; no exceptions; all checks become explicit Guards with on‑fail behavior (Abort | Branch).
  - Effect ordering via single effect token (recommended) or Memory/Effect SSA (advanced).
  - Canonical primitives with syscall normalization; verifier and printer.
- LIR (Low‑level IR):
  - Two‑phase recommended: VReg‑LIR (value‑based) → Stack‑LIR (NeoVM‑like opcodes).
  - Verifier for stack balance, immediates, branch fixups; printer.

### 3.3 Lowering (`src/Neo.Compiler.CSharp/Lowering`)
- HIR → MIR:
  - Remove exceptions per policy; materialize Guards (Null/Bounds/Checked*).
  - Harden calling convention: explicit receiver (first param), single return (multi via Struct).
- MIR → LIR:
  - Instruction selection (table/pattern driven) to VReg‑LIR.
  - Prepare fused patterns (cmp+branch) and container/bytes ops for selection.

### 3.4 Passes (`src/Neo.Compiler.CSharp/Passes`)
- HIR passes:
  - Devirtualization, monomorphization, closure conversion.
  - Attribute‑guided inlining, constants/algebraic simplifications, DCE/CopyProp/CF simplify.
  - Check insertion and elimination strategies.
- MIR passes:
  - SCCP, GVN/CSE, CopyProp/DCE, SROA.
  - Guard hoist/sink/elide, LICM, loop simplify + tail duplication, redundant container read elimination (versioned objects), ABI hardening.
- LIR passes:
  - Selector (MIR→VReg‑LIR), Stack Scheduler (VReg‑LIR→Stack‑LIR), Peephole, Layout (fallthrough/jump threading), constant encoding minimization.

> **Status note:** the current implementation ships with a starter set of passes (HIR guard pruning, MIR unreachable block pruning, and LIR peephole cleanups). The remaining passes listed above are part of the planned roadmap and should be added before promoting the new pipeline as the default.

### 3.5 Backend (`src/Neo.Compiler.CSharp/Backend`)
- Emitter:
  - Encode NeoVM opcodes, PUSHDATA sizing, branch fixups (two‑pass assembly), and SYSCALL ids.
  - Assemble NEF (script + checksum).
- Manifest:
  - Build manifest (methods/events/permissions/trust/features/safe) from HIR/MIR metadata + intrinsic usage.
- DebugInfo:
  - Offset ↔ source mapping (preserved across selection/scheduling).
- CostModel:
  - Opcode base costs and syscall cost hints; estimators for per‑path/worst‑case gas.
- Reports:
  - Peak stack, instruction count, code size, gas estimate (per function and for the whole script).

---

## 4. Data Flow and Build Pipeline

1) Frontend (Roslyn) → HIR  
   - IOperation+CFG → HIR graph with attributes and source spans; intrinsics bound; policies enforced.

2) HIR optimization  
   - (Currently) constant folding, phi simplification (uniform inputs), dead-code elimination for pure values, constant-branch simplification, and pruning of `Assume` guards, followed by removal of unreachable blocks.  
   - (Planned) Devirt/Monomorph/Closure → Inline/ConstFold/GVN‑lite → Check insertion/elision → DCE/CF Simplify.

3) Lower HIR → MIR  
   - Exceptions removed or converted; Guards materialized; ABI hardened.

4) MIR optimization  
   - (Currently) prune unreachable blocks.  
   - (Planned) Verify → SCCP → GVN/CSE → SROA → Guard hoist/sink → LICM/Loop → RedundantReads → ABI finalize.

5) MIR → LIR selection  
   - Instruction selection to VReg‑LIR.

6) LIR shaping  
   - Stack scheduling (currently a linear scheduler; roadmap includes Sethi–Ullman ordering, shuffle minimisation, and Phi edge materialisation) → Peephole → Layout → Encoding minimisation.

7) Verification and emission  
   - LIR verifier (stack/jumps/immediates) → NEF + manifest + debug map → reports.

Build profiles (configurable under `configs/profiles/`):
- Debug: minimal inlining/motion, retain checks and mapping fidelity.
- Release‑size: conservative expansion, size‑focused peepholes and encodings.
- Release‑gas: aggressive MIR + stack scheduling, hot‑path layout, gas‑aware heuristics.

---

## 5. IR Layer Summaries

- HIR (High‑level IR):
  - Preserves high‑level semantics and types; fits semantic transforms (generics, closures, inlining).
  - IntrinsicCall carries category/effect/purity; checks represented as nodes (Null/Bounds/Checked*).
  - Structured control flow allowed; exceptions optional but must be lowered before MIR.

- MIR (Mid‑level IR):
  - Strict SSA; no exceptions; explicit Guards for failures; effect ordering via token/MemorySSA.
  - All operations must map to target sequences; DevPack interops normalized to syscall families.

- LIR (Low‑level IR):
  - Target‑close; two‑phase recommended: VReg‑LIR → Stack‑LIR (NeoVM‑like opcodes).
  - Focus on instruction selection, stack scheduling, peepholes, block layout, and emission fidelity.

See also:
- [HIR‑Design‑Neo](./HIR-Design-Neo.md)
- [MIR‑Design‑Neo](./MIR-Design-Neo.md)
- [LIR‑Design‑Neo](./LIR-Design-Neo.md)

---

## 6. Effects, Purity, and Determinism

- Effects: `None`, `Runtime`, `StorageRead`, `StorageWrite`, `Interop`, `Crypto`, `Abort`.
- Purity:
  - `Pure`: CSE/LICM safe; no token touch.
  - `ReadOnly`: can move across reads, not across writes.
- Ordering:
  - Recommended single effect token (simple, conservative).
- Determinism:
  - No floating point; syscalls are allow‑listed (policy under `configs/policies/`).
  - Runtime.Time and similar services are configured by policy and always tagged as effects.

---

## 7. Manifest and Artifacts

- NEF: assembled script with checksum, ready for deployment.
- Manifest: methods/parameters/returns, events, permissions (interops/contracts), trust, features/safe methods.
- Debug Map: instruction offset ↔ source span mapping.
- Reports: peak stack, instruction count, code size, gas estimate.

---

## 8. Verification and Observability

- Verifiers:
  - HIR: type/SSA/feature bans/intrinsic effects.
  - MIR: strict SSA; guard legality; effect chain connectivity; syscall whitelist/signatures.
  - LIR: stack balance, terminators, immediates, jump fixups.
- Dumps/Visualizations:
  - Textual dumps for HIR/MIR/LIR; DOT graphs for CFG; pass‑wise metrics (blocks, insts, peak stack, size, gas).
- Golden outputs (under `tests/.../golden/`):
  - Stable textual snapshots per IR layer and emitted bytes; used for regression and review.

---

## 9. Extensibility

- Intrinsic Catalog:
  - Add a new DevPack intrinsic by registering namespace/type/method → (category, syscall id, effect, purity, signature, cost hint).
- Pass Manager:
  - Add a pass by implementing the pass interface and declaring dependencies/invalidation; plug it into the appropriate pipeline and profile.
- Policies:
  - Update syscall whitelist or constraints via `configs/policies/syscall-whitelist.json`.
- Cost Model:
  - Update opcode and syscall costs in `configs/cost-model/`.

---

## 10. Testing Matrix

- Unit Tests:
  - Verifiers (HIR/MIR/LIR), analyses (dom/DF/liveness), individual passes, selector/scheduler microtests, peepholes.
- Golden Tests:
  - Known inputs with expected HIR/MIR/LIR dumps and NEF bytes; diffs on change.
- End‑to‑End (E2E):
  - Compile contracts and run on a NeoVM emulator; assert state changes/events/returns.
- Fuzz/Property:
  - Random small programs to check stack safety, guard correctness, and equivalence properties.

---

## 11. Contribution Guidelines (brief)

- Keep compiler logic inside `src/Neo.Compiler.CSharp`.
- Update relevant docs under `docs/` when changing IR schemas, pipelines, or policies.
- Provide/verifiy golden outputs for new passes or selection/scheduling changes.
- Ensure verifiers pass at each IR layer before opening PRs.
- For user‑visible changes to manifest or emission, include end‑to‑end tests.

---

## 12. Glossary (selected)

- SSA: Static Single Assignment; each definition unique; uses refer to dominating defs.
- Phi (φ): SSA merge node at CFG joins; eliminated before codegen via copies/stack shuffles.
- Guard: explicit runtime check with defined on‑fail control flow (Abort|Branch).
- SROA: Scalar Replacement of Aggregates; exposes struct fields/elements as scalars.
- GVN/CSE: Global Value Numbering / Common Subexpression Elimination.
- LICM: Loop‑Invariant Code Motion.
- Sethi–Ullman: heuristic to schedule expression trees minimizing peak stack/register pressure.

---

## 13. Pointers to Detailed Docs

- IR Overview and Rationale: [README‑IR‑Design‑Neo](./README-IR-Design-Neo.md)
- HIR Layer: [HIR‑Design‑Neo](./HIR-Design-Neo.md)
- MIR Layer: [MIR‑Design‑Neo](./MIR-Design-Neo.md)
- LIR Layer: [LIR‑Design‑Neo](./LIR-Design-Neo.md)
- Pass Pipelines: [PASS_PIPELINE](./PASS_PIPELINE.md)
- Intrinsics and Syscalls: [INTRINSICS](./INTRINSICS.md)
- Cost/Gas Model: [COST_MODEL](./COST_MODEL.md)
- Dumps/Formats: [DUMP_FORMATS](./DUMP_FORMATS.md)
- Debug Info: [DEBUG_MAP](./DEBUG_MAP.md)
- Design Decisions: [DESIGN_DECISIONS](./DESIGN_DECISIONS.md)
