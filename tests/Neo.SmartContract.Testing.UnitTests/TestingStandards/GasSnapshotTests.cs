// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards;

[TestClass]
public class GasSnapshotTests
{
    [TestMethod]
    public void CaptureSerializesMeasurementsInStableOrder()
    {
        var snapshot = GasSnapshot.Capture([
            ("transfer", 120L),
            ("balanceOf", 80L)
        ]);

        Assert.AreEqual("{\n  \"balanceOf\": 80,\n  \"transfer\": 120\n}", snapshot.ToJson());
        var roundTrip = GasSnapshot.FromJson(snapshot.ToJson());
        CollectionAssert.AreEqual(snapshot.Measurements.ToArray(), roundTrip.Measurements.ToArray());
    }

    [TestMethod]
    public void CompareReportsOnlyRegressionsAndMissingMeasurements()
    {
        var baseline = GasSnapshot.Capture([
            ("same", 100L),
            ("regressed", 100L),
            ("removed", 100L)
        ]);
        var current = GasSnapshot.Capture([
            ("same", 102L),
            ("regressed", 130L),
            ("added", 1L)
        ]);

        var differences = current.Compare(baseline, tolerance: 5);

        CollectionAssert.AreEqual(
            new[] { "added", "regressed", "removed" },
            differences.Select(difference => difference.Name).ToArray());
        Assert.AreEqual(100L, differences[1].Expected);
        Assert.AreEqual(130L, differences[1].Actual);
    }

    [TestMethod]
    public void AssertGasWithinAcceptsMeasurementsInsideTolerance()
    {
        var baseline = GasSnapshot.Capture([
            ("transfer", 100L),
            ("mint", 250L)
        ]);
        var current = GasSnapshot.Capture([
            ("transfer", 105L),
            ("mint", 245L)
        ]);

        current.AssertGasWithin(baseline, tolerance: 5);
    }

    [TestMethod]
    public void AssertGasWithinReportsEveryDifference()
    {
        var baseline = new GasSnapshot().Record("transfer", 100);
        var current = new GasSnapshot()
            .Record("transfer", 130)
            .Record("mint", 20);

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => current.AssertGasWithin(baseline, tolerance: 5));

        StringAssert.Contains(exception.Message, "mint (expected missing, actual 20)");
        StringAssert.Contains(exception.Message, "transfer (expected 100, actual 130)");
    }

    [TestMethod]
    public void RecordRejectsDuplicateAndNegativeMeasurements()
    {
        var snapshot = new GasSnapshot().Record("transfer", 10);

        Assert.ThrowsExactly<ArgumentException>(() => snapshot.Record("transfer", 11));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => snapshot.Record("negative", -1));
    }

    [TestMethod]
    public void SaveAndLoadRoundTrip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"neo-gas-{Guid.NewGuid():N}.json");
        try
        {
            var snapshot = new GasSnapshot().Record("transfer", 123);
            snapshot.Save(path);
            Assert.AreEqual(123L, GasSnapshot.Load(path).Measurements["transfer"]);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [TestMethod]
    public void FromJsonRejectsNullSnapshot()
    {
        Assert.ThrowsExactly<FormatException>(() => GasSnapshot.FromJson("null"));
    }
}
