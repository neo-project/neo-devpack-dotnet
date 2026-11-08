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
