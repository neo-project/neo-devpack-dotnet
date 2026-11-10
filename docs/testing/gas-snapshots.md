# Gas snapshots

`GasSnapshot` provides deterministic gas measurements for testing compiler and contract changes. Measurements are stored by name, serialized in ordinal name order, and compared against a checked-in or generated baseline.

```csharp
var baseline = GasSnapshot.Load("baselines/transfer.json");
var current = GasSnapshot.Capture([
    ("transfer", MeasureTransfer()),
    ("mint", MeasureMint())
]);

current.AssertGasWithin(baseline, tolerance: 5);
```

`AssertGasWithin` fails when a measurement is missing, unexpectedly added, or outside the absolute tolerance. Use `Compare` when a test needs to inspect the individual differences instead of throwing.

Snapshots can be persisted with `Save` and restored with `Load`. Names should identify stable operations so that a baseline remains meaningful when the test suite evolves.

## Markdown differences

Write `current.ToMarkdown(baseline, tolerance: 100)` to a report file to compare
GAS measurements in CI. The table lists added, removed and out-of-tolerance
operations in ordinal order, with absolute changes in datoshi. Equal measurements
and changes within the tolerance are omitted. An empty comparison still includes
the table header. Numbers and line endings are independent of machine culture;
operation names are escaped for Markdown tables.

JSON baselines reject duplicate operation names, including escaped spellings of
the same name. `Measurements` is a live read-only view; use `Record` to add values
so nonnegative values and unique names remain enforced.

## CI regression reports

Baselines live in `tests/Neo.Compiler.CSharp.UnitTests/GasBaselines/*.json` under
version control. The project copies them to the test output directory, and the
representative test loads them relative to `AppContext.BaseDirectory`, not the
working directory. Missing or invalid baselines fail the test; tests never
rewrite them.

`GasRegressionReport.Evaluate(baseline, current, tolerance: 0)` applies a separate
CI policy:

| Measurement | Policy result |
| --- | --- |
| Equal GAS | Pass, unchanged |
| Lower GAS | Pass, improvement |
| Higher GAS within tolerance | Pass, within tolerance (not unchanged) |
| Higher GAS above tolerance | Fail, regression |
| Missing or unexpected operation | Fail, intentional baseline review required |

Tolerance is absolute datoshi and defaults to zero, not a percentage. Comparisons
use `Int128` to avoid overflow. The report captures private copies of both
snapshots at evaluation time and exposes read-only changes. Later calls to
`Record()` on the original snapshots cannot change the comparison or published
files. Add all measurements before evaluating.

`GasSnapshot.AssertGasWithin()` remains symmetric: a decrease beyond tolerance
also fails that API. **The regression policy does not override other tests.**
For the three representative StringBuilder operations, `UnitTest_String` checks
functional results while `UnitTest_GasSnapshotBaseline` measures GAS and applies
the regression policy. Their former duplicate exact-GAS assertions are removed,
so improvements to these operations no longer fail an exact-match check.
The checked-in JSON is their single GAS baseline. Other operations retain their
existing strict assertions; those may still fail if their own GAS changes.
Do not disable strict assertions globally.

### CI artifacts and context

When `NEO_GAS_REPORT_DIR` is set, `WriteReportIfConfiguredAndAssert()` writes
`baseline.json`, `current.json` and `gas-regression-report.md` **before** asserting
the policy. Without this variable, normal local execution creates no reports.
Publication uses the evaluated snapshots, not replacements supplied by a caller.

The `Test` job sets this directory for its existing full compiler test step;
it does not run the compiler suite again. Summary and upload steps use
`always()` with the existing full-scope condition, so a GAS assertion failure
does not prevent publication. Documentation-only selection remains unchanged.

Download the `gas-report` artifact from the workflow run and open its Markdown
file. Compare `baseline.json` with `current.json` to inspect the exact datoshi
values. An unavailable report is noted in the summary if tests never reached
report generation, for example because the baseline was missing.

The representative report records the loaded Neo Core assembly version, actual
compiler optimization, benchmark contract, and the engine's hardfork activation
heights. Currently these tests use Neo Core 3.10.2, optimization `All`,
`Contract_String`, and `TestEngine` protocol settings. Branch and checkout
revision are included in CI (shown as `local` outside CI); baseline history
remains in Git. Measurements and metadata are ordered ordinally and rendered
with invariant numbers, escaped Markdown and LF line endings, without timestamps.

For example, the existing representative measurements produce these rows:

| Operation | Baseline | Current | Difference | Status |
| --- | ---: | ---: | ---: | --- |
| string-builder-basic | 2600550 | 2600550 | 0 | Unchanged |
| string-builder-clear | 1916460 | 1916460 | 0 | Unchanged |
| string-builder-length | 2665110 | 2665110 | 0 | Unchanged |

The report also includes the overall result, tolerance and classification counts.

### Reproduce locally

PowerShell (reports are outside the source tree):

```powershell
$previous = $env:NEO_GAS_REPORT_DIR
try {
    $env:NEO_GAS_REPORT_DIR = Join-Path $env:TEMP "neo-gas-report"
    dotnet test tests/Neo.Compiler.CSharp.UnitTests --filter "FullyQualifiedName~UnitTest_GasSnapshotBaseline"
} finally {
    $env:NEO_GAS_REPORT_DIR = $previous
}
```

Bash:

```sh
NEO_GAS_REPORT_DIR="${TMPDIR:-/tmp}/neo-gas-report" dotnet test tests/Neo.Compiler.CSharp.UnitTests --filter "FullyQualifiedName~UnitTest_GasSnapshotBaseline"
```

### Intentional baseline updates

Investigate any change using the report and its configuration. Compiler
optimizations, Neo Core/NeoVM dependencies and protocol/hardfork updates can
change GAS; elapsed time, CPU use and allocations are not regression inputs.
Edit the checked-in JSON explicitly in a reviewed change, using `current.json`
as evidence rather than automatically accepting it. Explain every changed value,
added operation or removed operation. For other operations with strict GAS
assertions, review those assertions too if affected by a legitimate change.
Never copy an increased measurement into the baseline merely to make CI pass.

This integration adds baseline comparison and automated reporting only.
Persistent historical trend storage across runs and automated baseline governance
are not implemented, so Issue #8 is not fully complete.

## Automatic tracking in unit tests

`TestEngine.Execute` records the GAS of every execution (`ApplicationEngine.FeeConsumed`,
HALT and FAULT) into a per-test `AsyncLocal` scope (`GasExecutionTracker`). `TestBase<T>`
starts the scope in `[TestInitialize]`, after the constructor has deployed the contract, so
deployment is never measured. In `[TestCleanup]` the measurements are compared with
`GasBaselines/<test assembly>/*.json` (one file per test class; large classes are split into `<Class>.partNN.json`, and all files are merged on load):

- An increase is a regression, a decrease is an accepted improvement.
- A missing or unexpected execution fails the test.
- Comparison only happens for tests that passed, so functional failures keep their own diagnostics.
- Tests excluded by a focused run or skipped never execute, so they are never reported as missing.
- Tracking is disabled (no overhead beyond a null check) when the assembly has no baseline.

Execution identifiers are `<Class>.<Method>[<case>]#<n>`, or `:<label>` when
`GasExecutionTracker.Label(...)` is used. Use `GasExecutionTracker.Exclude()` around
nondeterministic executions.

### Generating baselines (maintainers only)

```powershell
$env:NEO_GAS_BASELINE_GENERATE_DIR = "$PWD\gen-tmp"
dotnet test tests/<Project>
-Output tests/<Project>/GasBaselines/<Assembly>
```

Generation writes one file per test (parallel-safe) and is refused when `CI` or
`GITHUB_ACTIONS` is set. Committed baselines are never rewritten automatically; review every
changed value. Run generation twice and compare the merged files to confirm stability.

### Reports

With `NEO_GAS_REPORT_DIR` set, each tracked test writes its own `tracked-<assembly>/tests/*.tsv`.
`scripts/gas/aggregate-report.ps1` merges them into `gas-tracking-report.md/json`; CI runs it
even when tests fail.
