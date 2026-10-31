# Neo Compiler Fuzzing & Differential Analysis

A harness that fuzzes the Neo N3 smart contract compiler and cross-checks its output
against independent oracles, so that compiler defects surface as early as possible.

## Why this exists

The compiler has four layers where a defect can hide, and each needs a different
detection strategy:

| Layer | Typical defect | Detection method |
|---|---|---|
| Front end (Roslyn binding) | unsupported syntax mishandled | compile succeeds but with `NC5001` internal-error diagnostic |
| `MethodConvert` (C# → Neo opcodes) | wrong opcode sequence, wrong stack order | behaviour differential vs. plain C# |
| Optimizer (16 strategies) | rewrite changes observable behaviour | behaviour differential across optimization levels |
| Artifact emission | wrong ABI, non-deterministic output | ABI differential + determinism check |

A naive random-byte fuzzer only reaches the first layer, because almost every input
is rejected with a `CS1xxx` syntax diagnostic. The generator in this harness therefore
produces *well-formed* programs from a typed expression grammar, and the mutator
perturbs them around the boundaries the generator respects.

## Oracles

Five oracles run on every input that compiles cleanly.

### 1. Internal-error detection

`CompilationContext.Compile()` converts unexpected internal exceptions into a
diagnostic with id `NC5001` (`UnexpectedCompilerError`). Any input producing `NC5001`
is reported as `InternalError`: the compiler hit a state it did not expect, which is a
defect regardless of whether the compilation eventually succeeded.

Any exception that is *not* a `CompilationException` escapes `Compile()` entirely and is
reported as `CompilerCrash`.

### 2. Cross-language behaviour differential (strongest)

The same method body is compiled twice:

- by the Neo compiler, then executed on the Neo VM;
- by Roslyn as an ordinary .NET assembly, then executed on the CLR.

The two must agree on every input. The reference is the C# language itself, so a
divergence is a compiler defect rather than a disagreement between two implementations.
Argument sets are boundary-heavy (`0`, `1`, `-1`, `int.MaxValue`, `long.MinValue`, …)
because that is where overflow and sign-handling bugs concentrate.

### 3. Optimizer-equivalence differential

The same source is compiled twice — once with the experimental optimizer applied and
once without — and both artifacts are executed on the VM. The optimizer rewrites
scripts aggressively (jump compression, peephole rewrites, unreachable-code removal).
Any rewrite that changes observable behaviour is a consensus-level defect: the same
source would behave differently depending on compiler settings.

This oracle needs no reference implementation, so it cannot produce false positives.

> Note: `Optimize=None` is *not* the same as "unoptimized". The compiler always runs
> `BasicOptimizer.CompressJumps`, so a separate compilation with `Optimize=None` is
> used as the baseline rather than the raw script.

### 4. ABI and determinism

- The method set, parameter counts, parameter types, and return types in the manifest
  must match across optimization levels.
- Compiling the identical source twice must produce byte-identical scripts and
  checksums. Divergence indicates hidden state or non-deterministic ordering.

The constructor `_initialize` is excluded from both comparisons. Its presence
legitimately differs between optimization levels, for two independent reasons:

- `BasicOptimizer.RemoveNops` shrinks the instruction list, which can bring
  `_initialize` down to the single instruction that `RemoveEmptyInitialize` treats as
  empty. The `None` build keeps it, the `Basic` and `All` builds drop it.
- `InitStaticToConst` folds a static-field read into a constant and drops the
  matching store. Once that removes the last static-field access from the script,
  `Peephole.RemoveInitialize` removes the now-empty `_initialize` from the manifest.
  A contract with `private static int counter = 7;` therefore exposes `_initialize`
  under `None` and `Basic`, but not under `All`.

In both cases the removed constructor initializes nothing that is still live, so no
observable behaviour differs. Run `--init-abi-probe` to see the effect across the
three optimization levels for several static-initialization shapes.

### 5. Constructors are not business methods

`_initialize` is invoked by the VM at deploy time, not through the public call path,
so the behaviour comparison does not invoke it either.

## Self-test: proving the oracles are live

A fuzzer reporting "no defects" only means something if its oracles are known to fire.
`--self-test` feeds each oracle a deliberately divergent pair and asserts the
divergence is reported:

```bash
dotnet run --project tests/Neo.Compiler.CSharp.Fuzzing -- --self-test
```

Exit code `0` means every oracle is live; `3` means at least one is broken. The
self-test covers all five oracles: cross-language behaviour, optimizer equivalence,
determinism, ABI, and the internal-error diagnostic id.

## Coverage-guided search

The fuzzer is coverage-guided rather than a blind random search.

After a contract is deployed and executed on the VM, the engine records which
instructions were actually reached. The harness turns that into a coverage signal:

- A global set of instruction offsets accumulates across the whole run. Its size is the
  coverage measure: while it keeps growing, the search is still reaching new compiler
  output; once it flattens, the search has saturated.
- An input is retained in the corpus only when it reached instructions that had not
  been seen before. Inputs that add nothing new are dropped, so the corpus converges on
  the inputs that actually exercise new code.
- 40% of the next inputs are mutations of corpus entries. Because the corpus only holds
  productive entries, this raises the fraction of inputs that compile cleanly and keeps
  deepening the search rather than restarting it.

The coverage data comes from `Neo.SmartContract.Testing`'s per-contract coverage
dictionary, which is internal. The fuzz project is granted access through
`InternalsVisibleTo("neofuzz")` in `src/Neo.SmartContract.Testing/AssemblyInfo.cs` —
the name must match the fuzz project's `AssemblyName`, which is `neofuzz`.

To measure the benefit rather than assume it, run the same seed twice:

```bash
# With coverage guidance (the default)
dotnet .../neofuzz.dll --iterations 120 --seed 777

# Without it: the corpus keeps every input that compiled
dotnet .../neofuzz.dll --iterations 120 --seed 777 --no-coverage-guidance
```

Compare the `instr=` and `corpus=` columns in the progress output.

Measured on this repository, seed 777, 120 iterations, same binary:

| Metric | Guidance on | Guidance off |
|---|---|---|
| Distinct instructions reached | **1152** | 1050 |
| Corpus entries | 22 | 92 |
| Inputs that compiled | 90 | 92 |

Coverage guidance reached 102 more instructions (9.7%) for the same number of
iterations, while keeping the corpus an order of magnitude smaller.

### Guarding against pathological inputs

The VM executes synchronously, so a contract with a very long loop cannot be
interrupted without corrupting the engine. Two mechanisms keep this from stalling a
campaign:

- The generator bounds loop counts. An earlier version emitted
  `for (int i = 0; i < a; i++)` with `a` taken from the argument set; combined with
  `int.MaxValue` that is practically an infinite loop on the VM. Capping the bound
  raised throughput by a factor of 54 (120 iterations: 451s → 8.3s) without reducing
  coverage.
- A per-iteration wall-clock budget (30s) turns any remaining pathological input into a
  `SlowExecution` finding with a reproducer, and ends the campaign. A fuzzer that
  silently burns its whole time budget on one input would hide that it had stalled.

## Cross-contract calls

Ten percent of the iterations generate a two-contract source: `ContractA` calls
`ContractB` through `System.Contract.Call`. The caller receives the callee's hash as a
`UInt160` parameter rather than embedding it, because the hash only exists after
deployment; a `ContractReference` placeholder is resolved to the deployed hash at
execution time.

This crosses compilation units — the caller's method-token table records the callee's
hash and signature, and the callee is a separate artifact with its own entry point — so
there is no plain C# reference and only the VM oracles apply.

Two failure modes are specific to this path, and both are silent without a probe:

- **The caller must declare permission.** Without `[ContractPermission("*", "*")]` the VM
  refuses the call with `Cannot Call Method ... From Contract ...`, both builds fault
  identically, and the differential reports agreement — indistinguishable from a clean
  compiler.
- **Method names must come from the ABI.** The compiler lower-cases only the first
  character, so `ForwardTwice` becomes `forwardTwice`, not `forwardtwice`. A guessed name
  faults with `Method "..." doesn't exist`.

`--multi-contract-probe` asserts that the callee is deployed, the call returns a value,
instructions were executed, and the callee's storage was written. The same check runs as
part of `--self-test`, and the campaign reports a `multi=` count so that a path which
never runs is visible instead of looking like a clean result.

## Persisting the corpus

Without persistence every run starts from nothing: the same programs are rediscovered,
the same plateau is reached, and a previous campaign's work is lost. The `--corpus`
option makes coverage accumulate:

```bash
dotnet .../neofuzz.dll --iterations 500 --corpus ./fuzz-corpus
```

Entries are stored one JSON file per entry, named after a hash of the source, so writing
an input that is already present is a no-op. On start-up the offsets each entry reached
are restored as well — without that, every restored entry would look like it adds
nothing and would never be mutated again.

Measured across three consecutive runs sharing one corpus directory:

| Run | Restored | Coverage reached | New entries | Corpus on disk |
|---|---|---|---|---|
| 1 (seed 1) | — | 1145 | 22 | 22 |
| 2 (seed 1) | 22 entries / 1145 offsets | **1298** | 11 | 33 |
| 3 (seed 2) | 33 entries / 1298 offsets | **1460** | 22 | 55 |

Each run continues from where the previous one stopped rather than repeating it. The
nightly CI job caches this directory, which is what makes the scheduled campaign
cumulative.

## Feature coverage

The generator produces twelve feature families. Each one is checked by
`--rejection-probe` to confirm it still compiles cleanly, so the fuzzer spends its
time reaching the code generator rather than on inputs the compiler rejects by design.

| Family | What it exercises |
|---|---|
| `RandomExpressions` | type-driven random expression trees, structurally different on every run |
| `Arithmetic` | arithmetic operators, `checked`/`unchecked`, `BigInteger` and `Math` system calls |
| `ControlFlow` | `for`/`while`/`switch`, including `default` ordering |
| `Collections` | arrays, element access, `Array.Reverse` |
| `Strings` | `string` length, indexing, concatenation |
| `Conversions` | widening, narrowing, `BigInteger` promotion |
| `ExceptionHandling` | `try`/`catch`/`finally`, nested handlers |
| `Lambdas` | `Func<>` delegates, block-bodied lambdas, closures |
| `PatternMatching` | type patterns, `switch` expressions |
| `AsyncAwait` | generic helpers, delegates, static state (the compiler rejects `await` itself) |
| `StaticState` | static fields with initializers, explicit static constructors |
| `NarrowTypes` | `byte`/`short`/`char`/`uint` arithmetic, zero comparisons |
| `LongJumps` | `try`/`catch` with a distant handler, forcing long-form jump compression |
| `Storage` | stateful contracts that write, overwrite, and delete storage |
| `StorageMap` | prefixed storage through `StorageMap`, a different key-encoding path |
| `InterContract` | cross-contract dispatch through `System.Contract.Call` |
| `Generics` | generic methods, generic classes, and generic constraints |
| `Interfaces` | virtual dispatch through inheritance and `override` |
| `Enums` | enum declaration, comparison, and underlying type conversion |
| `Events` | event declaration and invocation |
| `Nullable` | nullable lifting, unboxing, and the coalescing operators |

### Random expressions

`RandomExpressions` is the family that matters most for coverage. The other families are
templates with randomised constants: every run produces the same program structure. This
family instead walks a typed grammar (`ExpressionGenerator.cs`) where each node carries
the type it produces and the operator chosen is legal for that type, so the generated
program differs structurally on every call while still compiling.

Two rules keep the output useful rather than pathological: division always uses a divisor
that cannot be zero, and only the system methods the compiler actually supports are
called (there are 70 of them; anything else is rejected with `NC1002`).

The tree is rendered once and reused for both the contract and the plain C# reference, so
the two can never drift apart.

### Capability limits discovered by probing

Probing the families revealed what the compiler actually supports, which contradicts
some assumptions worth recording:

- **Interfaces are not supported at all** (`NC2009`, raised in
  `MethodConvert/Helpers/CallHelpers.cs`). Virtual dispatch is only reachable through
  class inheritance and `override`, so that is what the family generates.
- **The event symbol itself cannot be used** (`NC2001`). An event can be declared, but
  it has to be invoked directly rather than through `?.Invoke`.
- `System.Convert` helpers are not callable from a contract (`NC1002`), which constrains
  what a generic method may do with its type parameter.

Families with no plain C# equivalent — `Storage`, `StorageMap`, and `InterContract`,
because the CLR has no storage and no `System.Contract.Call` — produce no reference
source and are covered by the optimizer-equivalence and storage oracles only.

That decision is centralised in `SourceGenerator.HasClrEquivalent`. It has to be
explicit and complete: a family that produces a reference which does not compile is
silently dropped by the cross-language oracle, which removes it from coverage without
any visible failure. The `--reference-coverage` probe and the
`EveryGeneratedFeatureFamilyReachesTheCrossLanguageOracle` test exist to catch exactly
that — and did, when `InterContract` and `StorageMap` were first added.

## Usage

```bash
# Build
dotnet build tests/Neo.Compiler.CSharp.Fuzzing -c Release

# Self-test: verify the oracles detect planted differences
dotnet tests/Neo.Compiler.CSharp.Fuzzing/bin/Release/net10.0/neofuzz.dll --self-test

# Run the fuzzer
dotnet tests/Neo.Compiler.CSharp.Fuzzing/bin/Release/net10.0/neofuzz.dll \
    --iterations 500 --timeout 600 --output ./fuzz-findings

# Reproduce a specific seed
dotnet .../neofuzz.dll --seed 12345 --iterations 100
```

### Options

| Option | Meaning |
|---|---|
| `--seed <n>` | Random seed (default `20261021`). A failing seed reproduces exactly. |
| `--iterations <n>` | Number of fuzz iterations (default `500`). |
| `--timeout <sec>` | Wall-clock budget. |
| `--output <dir>` | Writes `findings.json` plus one `repro-NNN-<outcome>.cs` per finding. |
| `--self-test` | Verify the oracles are live; exits `3` on failure. |
| `--rejection-probe` | Show why generated sources are rejected, grouped by diagnostic id. |
| `--reference-coverage` | Verify every feature family produces a compilable plain C# reference. |
| `--abi-probe` | Print the ABI the compiler produces for a sample contract. |
| `--init-abi-probe` | Compare the ABI across optimization levels for static-initialization shapes. |
| `--biginteger-repro` | Focused reproduction for the nested `BigInteger` call shape. |
| `--generate-artifact <Contract>` | Regenerate a testing artifact for a contract (see below). |
| `--repo-root <path>` | Repository root used by `--generate-artifact`. |
| `--no-coverage-guidance` | Disable coverage-guided corpus selection, to measure its benefit. |
| `--corpus <dir>` | Persist the corpus to a directory, restoring it on the next run. |
| `--multi-contract-probe` | Verify the cross-contract path deploys the callee and reaches it. |
| `--help` | Usage text. |

### Exit codes

| Code | Meaning |
|---|---|
| `0` | No defect found. |
| `1` | At least one defect found. |
| `2` | Harness failure. |
| `3` | Self-test failed: an oracle is not detecting planted differences. |

## Layout

```
tests/Neo.Compiler.CSharp.Fuzzing/
  CompilerHarness.cs              Drives CompilationEngine; classifies outcomes
  SourceGenerator.cs              Typed expression grammar -> well-formed contracts
  SourceMutator.cs                Structural/textual mutation around boundaries
  FuzzRunner.cs                   Orchestrates generation, mutation, analysis
  DifferentialAnalyzers.cs        ABI and determinism oracles
  BehaviorDifferentialAnalyzer.cs Executes artifacts on the VM, isolates state
  CrossLanguageOracle.cs          Roslyn reference execution
  OracleSelfTest.cs               Proves the oracles fire
  FuzzFinding.cs / Program.cs     Result model and CLI
  AbiProbe.cs / RejectionProbe.cs / InitializeAbiProbe.cs / BigIntegerRepro.cs
                                  Diagnostic helpers
```

## Running as part of the test suite

The harness is also wired into the standard test run. `tests/Neo.Compiler.CSharp.UnitTests/Fuzzing/DifferentialFuzzTests.cs`
contains three tests:

- `EveryDifferentialOracleDetectsAPlantedDifference` — the oracle self-test, run as a
  unit test. This is the one that matters most: without it, a harness that stopped
  comparing anything would look exactly like a clean compiler.
- `EveryGeneratedFeatureFamilyReachesTheCrossLanguageOracle` — guards against a
  feature family silently dropping out of coverage.
- `ShortFuzzingCampaignFindsNoDefects` — a 60-iteration smoke test.

```bash
dotnet test tests/Neo.Compiler.CSharp.UnitTests --filter FullyQualifiedName~DifferentialFuzzTests
```

## Regenerating a testing artifact

The unit-test project compiles the generated artifacts in
`tests/Neo.Compiler.CSharp.UnitTests/TestingArtifacts/`, not the contract sources
directly. A contract added to `tests/Neo.Compiler.CSharp.TestContracts/` therefore
needs a matching artifact, and the usual regeneration path — running the tests, which
rewrites the artifacts — is blocked when the artifact is missing, because the test
project does not compile.

`--generate-artifact` breaks that deadlock by performing the same steps as
`TestCleanup.CreateArtifactAsync`, but through the pure Roslyn path so that no
MSBuild or NuGet restore is needed:

```bash
dotnet tests/Neo.Compiler.CSharp.Fuzzing/bin/Release/net10.0/neofuzz.dll \
    --generate-artifact Contract_ArithmeticOverflow --repo-root /path/to/neo-devpack-dotnet
```

The Neo core assemblies are deliberately excluded from the reference set for this
command: the test-contract project does not reference the Neo core package, because
the smart-contract framework defines its own `UInt160` and `ByteString`. Including the
core assemblies would make `using Neo;` resolve `Runtime` and `UInt160` to the wrong
types.

The project version is read from `src/Directory.Build.props` and injected into the
manifest extra, because the fast path does not go through MSBuild and therefore never
sees the `VersionPrefix` property. With that in place the generated artifact is
byte-identical to the one the test harness writes itself.

## Notes for maintainers

- **Calling convention.** `System.Contract_Call` expects the arguments *packed into an
  array*: push arguments in reverse order, then the count, then `PACK`. Pushing them
  individually makes the VM read the first argument as the array, which surfaces as
  `Integer cannot be converted to Array`. This cost real debugging time; the
  `--biginteger-repro` probe exists to keep it fixed.
- **Method names.** The compiler lower-cases the first character of public method names
  (`Helper.cs`), so the ABI name (`flow0`) differs from the C# name (`Flow0`). The
  cross-language oracle maps between them.
- **Storage isolation.** Each artifact set is deployed and executed inside its own
  `EngineCheckpoint`, so the two sides of a comparison cannot influence each other and
  repeated deployments do not collide.
- **Reference set.** The metadata references come from `TRUSTED_PLATFORM_ASSEMBLIES`
  plus the local `Neo.SmartContract.Framework` assembly. No MSBuild or NuGet restore is
  involved, which is what keeps the throughput high.
