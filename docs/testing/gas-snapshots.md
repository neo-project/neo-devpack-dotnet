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
