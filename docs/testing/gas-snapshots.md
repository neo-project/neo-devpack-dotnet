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

The `Test` job sets this directory for its full compiler and framework test
steps; it does not run those suites again. The summary step runs after the last
tracked suite and uses `always()` with the existing full-scope condition, so a
GAS assertion failure does not prevent publication. Documentation-only
selection remains unchanged.

The report is published in the job summary of the workflow run. No artifact is
uploaded. To inspect exact datoshi values, run the tests locally with
`NEO_GAS_REPORT_DIR` set and compare `baseline.json` with `current.json`. An
unavailable report is noted in the summary if tests never reached report
generation, for example because the baseline was missing.

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
Baseline JSON is never edited by hand: CI generates and synchronizes it (see
below). Never accept an increased measurement merely to make CI pass.

### Automatic baseline synchronization

1. `main.yml` (read-only, also for forks) runs the GAS-tracked suites with
   `NEO_GAS_BASELINE_CANDIDATE_DIR`. Tests write candidate JSON only to that
   temporary directory (never to `GasBaselines`), atomically and deterministically.
   Failed tests leave `.failed` markers and unstable repeated values leave
   `.unstable` markers. `NEO_GAS_BASELINE_GENERATE_DIR` remains refused in CI.
   The candidates and a run manifest (SHA, suite outcomes, `filtered=false`) are
   uploaded as the `gas-candidates` artifact.
2. After a successful push run on `master-n3`, `gas-baseline-sync.yml`
   (`UpdateGasBaselines`) downloads the artifact as data only and runs the
   trusted `scripts/gas/sync-baseline.ps1 -Mode Plan`. It checks the manifest SHA
   and that `master-n3` has not advanced, then stages replacement files.
3. The publish job (protected environment, GitHub App token) re-validates the
   staged files (`-Mode Apply`), commits only baseline JSON to
   `bot/gas-baseline-sync` with `chore: sync GAS baselines after successful CI`,
   and creates or updates a single PR. Auto-merge is requested only when the
   change is improvements only; it still honours branch protection.

| Condition | Result |
|---|---|
| GAS unchanged | no change |
| GAS lower | baseline updated |
| GAS higher, measurement missing, unexpected execution in an existing test | rejected |
| Functional failure, failed/unstable/incomplete/filtered run | rejected, JSON untouched |
| Previously unseen test identity | candidate staged, protected environment approval required; never auto-merged |

Legitimate increases: run `gas-baseline-approve.yml` (`workflow_dispatch`) with the
PR number, exact head SHA, exact `master-n3` base SHA and justification. Rebase
the reviewed PR on that base first; it must not contain baseline JSON changes.
It needs approval in the protected `gas-baseline-approval` environment, and opens
an **integration PR containing the reviewed code and generated baselines together**.
Merge that integration PR through the normal checks/reviews instead of merging
the original PR separately. It is never auto-merged. If base or head advances,
dispatch again and obtain fresh authorization. Only trusted base scripts execute
in the privileged job; the reviewed tree is handled as data, not built or tested there.
Labels or contributor input alone never authorize an increase.
An unseen identity is not proof of a new test; it may represent an existing test
newly tracked or renamed. The protected approval explicitly reviews that ambiguity.

Forks: PRs from forks only run the read-only workflow; the GAS report shows the
result in the job summary. The bot PR is opened against `master-n3` after merge,
so no credentials ever reach fork code and no commits are pushed to contributor
branches.

`scripts/gas/merge-baseline.ps1` (maintainers, local) now preserves entries not
in the input, rejects duplicates and invalid or negative values, and validates
staged files before replacing anything (`-Replace` for a full replacement).

Required repository configuration:

- Install a dedicated GitHub App on this repository only, with Contents and Pull
  requests read/write (Metadata read is implicit). Do not grant bypass rights.
- Set Actions repository variable `GAS_SYNC_APP_ID` to its App ID. Store the PEM
  private key as environment secret `GAS_SYNC_APP_PRIVATE_KEY` in **both**
  `gas-baseline-sync` and `gas-baseline-approval`.
- Create those environments before enabling publication. `gas-baseline-approval`
  must require maintainer reviewers, prevent self-review and disallow bypass.
  Ordinary improvements use `gas-baseline-sync`; ambiguous new identities use
  `gas-baseline-approval`. The YAML alone does not configure environment protection.
- Restrict environment deployment refs to trusted workflow branches. The manual
  approval workflow additionally requires dispatch from `master-n3`.
- Protect `master-n3`: PR-only changes, required functional/GAS checks, appropriate
  reviews, no force pushes, no bypass for the App. Enable squash and auto-merge
  only if desired; auto-merge never substitutes required reviews.
- The receiver of `workflow_run` and the manually dispatched workflow must exist
  on the default branch. If it differs from `master-n3`, deploy these trusted
  workflow definitions there too and restrict environments accordingly.
- Default Actions token permissions should be read-only. Keep existing package/
  release permissions scoped to their publishing jobs. App-authored commits
  trigger normal PR checks, unlike relying on `GITHUB_TOKEN` pushes.

Plan/Apply bind staging to the original baseline file hashes and serialize
writers with a shared lock. Apply restores all participating assemblies if a
publication fails. This protects handled errors, not machine/power failure.
The bot rechecks the branch tip before publishing and uses force-with-lease;
required up-to-date checks or a merge queue remain necessary for races after that
check. Synchronization currently waits for the entire main workflow, including
package/release jobs, to succeed.

Troubleshooting: read `summary.md` in the `gas-baseline-staged` artifact; each
rejection lists the reason. Rerun CI on the full suite if the run was filtered or
the branch moved.

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
- Tracking is disabled when the assembly has no baseline unless local generation or candidate collection is explicitly enabled.

Execution identifiers are `<Class>.<Method>[<case>]#<n>`, or `:<label>` when
`GasExecutionTracker.Label(...)` is used. Use `GasExecutionTracker.Exclude()` around
nondeterministic executions.

### Generating baselines (maintainers only)

```powershell
$env:NEO_GAS_BASELINE_GENERATE_DIR = "$PWD\gen-tmp"
dotnet test tests/<Project>
./scripts/gas/merge-baseline.ps1 -InputDirectory gen-tmp/<Assembly> -Output tests/<Project>/GasBaselines/<Assembly>
```

Generation writes one file per test (parallel-safe) and is refused when `CI` or
`GITHUB_ACTIONS` is set. This local maintainer path remains available, but normal
contributors need not generate or edit JSON. Review intentional changes and run
local generation twice when investigating stability.

### Reports

With `NEO_GAS_REPORT_DIR` set, each tracked test writes its own `tracked-<assembly>/tests/*.tsv`.
Run `.github/scripts/gas/aggregate-report.ps1` to aggregate them; CI publishes
the summary even when tests fail.

#### GAS improvements

The aggregated report (`gas-tracking-report.md` / `.json`) lists every `Improvement` per assembly:

| Execution | Baseline | Current | GAS saved | Reduction |
|---|---:|---:|---:|---:|

`GAS saved = Baseline - Current` (datoshi) and `Reduction = (Baseline - Current) / Baseline * 100`
(two decimals, `0.00%` for a zero baseline). Rows are sorted by GAS saved (descending), then by
execution id. The JSON keeps `executions`, `regressions`, `improvements`, `missing` and `unexpected`,
and adds `totalGasSaved`, `largestGasSaved` and `improvementDetails` (`execution`, `baseline`,
`current`, `gasSaved`, `reductionPercent`) per assembly, plus `_totals.totalGasSaved`.

The totals are the sum of savings over measured test executions; they are not a transaction fee
estimate nor a percentage of real contract costs. Improvements pass because only a GAS increase
beyond tolerance is a regression and fails the test.

In GitHub Actions the report appears in the workflow run **Summary**. Locally, set
`NEO_GAS_REPORT_DIR`, run the tests and execute the script above, then open
`gas-tracking-report.md`.
