# Pass Implementation Checklist

Progress tracker for the HIR → MIR → LIR pipeline passes in the Neo C# DevPack compiler. Status uses:

- `[ ]` not started
- `[~]` in progress / partial
- `[x]` completed

---

## 0) Shared Analysis / Infrastructure
- [~] Dominator / IDom / DF (dominators + loop detection in place; DF still pending)
- [ ] Liveness & pruned SSA assistance
- [~] Loop analysis (natural loops available; needs normalisation integration)
- [ ] Value numbering hash-cons tables
- [~] Cost model loading (basic MIR gas summary only)
- [~] IR/LIR verifiers & dumps (verifiers exist, dump/graphviz tooling missing)
- [ ] Pass manager (dependencies / stats / timing)

## 1) HIR Passes
### 1.1 Legalisation & Normalisation
- [ ] Syntax legalisation (float/async/etc rejection pass)
- [ ] Controlled desugaring (foreach/using/init)
- [ ] Attribute collection (`Inline`, `Pure`, etc.)
- [ ] String → ByteString normalisation
- [ ] Property/indexer normalisation
- [ ] Switch density metadata

### 1.2 Semantic lowering
- [ ] Devirtualisation
- [ ] Monomorphisation
- [ ] Closure conversion

### 1.3 Simplification / Optimisation
- [x] Constant folding / propagation (incl. concat)
- [ ] Algebraic identities (x+0, double-not, etc.)
- [~] Copy propagation + DCE (block-local copy-prop + DCE in place)
- [x] Control-flow simplification (dead branch removal, straight-line merges)
- [~] Branch normalisation (constant cases handled; inversion pending)

### 1.4 Inter-procedural
- [ ] Attribute-driven inlining
- [ ] Small pure inline whitelist

### 1.5 Checks
- [ ] Null/Bounds/Checked insertion
- [~] Check elimination (Assume-removal, guarded hoisting only)
- [ ] Fail policy selection

### 1.6 DevPack Awareness
- [~] Intrinsic binding (catalog present, pass integration pending)
- [ ] Non-deterministic syscall enforcement

## 2) MIR Passes
### 2.1 Verification & Canonicalisation
- [x] MIR verifier (type/phi/effect)
- [~] Compare/convert canonical form
- [~] Syscall normalisation (metadata exists; dedicated pass pending)

### 2.2 Sparse dataflow & expressions
- [x] SCCP
- [~] Copy-prop + DCE (DCE done; copy-prop partial)
- [x] GVN / CSE for pure ops
- [ ] Algebraic simplification / strength reduction

### 2.3 Aggregates & containers
- [~] SROA (struct scalar replacement limited to simple cases)
- [x] Redundant length / HasKey elimination
- [~] Versioned redundant read elimination (block-local caches only)

### 2.4 Guard optimisation
- [~] Guard hoist / sink (within-block elimination only)
- [~] Guard redundancy removal (dominated duplicates removed)
- [ ] Fail-branch merging

### 2.5 Loops & layout
- [~] Loop simplify (header/preheader canonical form)
- [ ] LCSSA
- [~] LICM (basic invariant hoisting)
- [ ] Tail duplication
- [ ] Induction/range analysis

### 2.6 Calls & ABI hardening
- [ ] Micro-inlining (pure wrappers)
- [ ] Param/return pack adjustments
- [ ] Tail-call marking

### 2.7 Cost / Gas awareness
- [x] Gas / size summary
- [ ] Cost-aware inline / hoist policies

## 3) LIR Passes
### 3.1 Instruction selection
- [x] Table-driven selection (incl. syscall ids)
- [ ] Compare+branch fusion
- [~] Constant folding (limited to earlier IR; stack peephole todo)

### 3.2 Stack scheduling
- [x] Sethi–Ullman ordering
- [~] Shuffle minimisation (cheap DUP/OVER in place, deeper heuristics pending)
- [x] Phi elimination via edge copies
- [~] Near reuse + deep PICK avoidance
- [~] Peak stack heuristics

### 3.3 Peephole
- [x] Shuffle cancellation (DUP;DROP etc.)
- [x] Algebraic identities (PUSH0;ADD, double NOT)
- [~] Control-flow tightening (branch normalisation + jump threading; more cases pending)
- [ ] Constant encoding / merging

### 3.4 Layout & encoding
- [~] Fallthrough optimisation & condition inversion
- [ ] Hot-path linearisation
- [x] Two-pass label fixup
- [ ] Shortest constant encoding selection
- [ ] Large-constant reuse

### 3.5 Verification & reporting
- [x] LIR verifier
- [~] Metrics (stack depth tracked; consolidated reporting pending)
- [ ] Source map (offset → span)

## 4) DevPack / NeoVM Specific
- [~] Storage.* write barrier
- [ ] Contract.Call CallFlags effects
- [ ] Runtime classification
- [ ] StorageMap canonicalisation
- [ ] Event/Notify normalisation → manifest

## 5) Config / Switches
- [ ] Profile configs (debug/release-size/release-gas)
- [ ] CLI switches (`--pipeline`, dumps, cost-model, pass toggles)
- [ ] Syscall whitelist policy configuration

---

**Next steps**: focus on MIR loop simplification (preheaders, redundant read barriers) and LIR constant encoding/layout improvements, adding tests and metrics per completion criteria.
