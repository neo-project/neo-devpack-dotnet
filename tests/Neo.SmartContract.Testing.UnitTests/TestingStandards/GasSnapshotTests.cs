// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.Json;
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
    [TestMethod]
    [DataRow("{\"transfer\":10,\"transfer\":20}")]
    [DataRow("{\"transfer\":10,\"\\u0074ransfer\":20}")]
    public void FromJsonRejectsDuplicateNames(string json)
    {
        Assert.ThrowsExactly<ArgumentException>(() => GasSnapshot.FromJson(json));
    }

    [TestMethod]
    public void MeasurementsCannotBypassRecordValidation()
    {
        var snapshot = new GasSnapshot().Record("transfer", 10);
        var dictionary = (IDictionary<string, long>)snapshot.Measurements;
        Assert.ThrowsExactly<NotSupportedException>(() => dictionary.Add("negative", -1));
        Assert.AreEqual(10L, snapshot.Measurements["transfer"]);
        snapshot.Record("mint", 20);
        Assert.AreEqual(20L, dictionary["mint"]);
    }

    [TestMethod]
    public void MarkdownReportIsDeterministicAndEscapesOperationNames()
    {
        var baseline = GasSnapshot.Capture([("removed", 1L), ("same", 10L), ("cost|<x>\r\n`call`", 50L), ("lower", long.MaxValue)]);
        var current = GasSnapshot.Capture([("same", 11L), ("cost|<x>\r\n`call`", 75L), ("added", 0L), ("lower", 0L)]);
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            Assert.AreEqual(
                "| Operation | Baseline (datoshi) | Current (datoshi) | Change (datoshi) |\n| --- | ---: | ---: | ---: |\n"
                + "| added | missing | 0 | added |\n"
                + "| cost&#124;&lt;x&gt;<br>\\`call\\` | 50 | 75 | +25 |\n"
                + "| lower | 9223372036854775807 | 0 | -9223372036854775807 |\n"
                + "| removed | 1 | missing | removed |\n",
                current.ToMarkdown(baseline, tolerance: 1));
        }
        finally { CultureInfo.CurrentCulture = culture; }
    }

    [TestMethod]
    public void EmptyReportHasHeadersAndChecksComparisonArguments()
    {
        var snapshot = new GasSnapshot();
        Assert.AreEqual("| Operation | Baseline (datoshi) | Current (datoshi) | Change (datoshi) |\n| --- | ---: | ---: | ---: |\n", snapshot.ToMarkdown(snapshot));
        Assert.ThrowsExactly<ArgumentNullException>(() => snapshot.ToMarkdown(null!));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => snapshot.ToMarkdown(snapshot, -1));
    }

    [TestMethod]
    public void JsonStillRejectsInvalidValuesAndPreservesDistinctNames()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => GasSnapshot.FromJson("{\"negative\":-1}"));
        Assert.ThrowsExactly<JsonException>(() => GasSnapshot.FromJson("[]"));
        Assert.ThrowsExactly<JsonException>(() => GasSnapshot.FromJson("{\"text\":\"1\"}"));
        var snapshot = GasSnapshot.FromJson("{\"a\":0,\"A\":9223372036854775807}");
        Assert.AreEqual(2, snapshot.Measurements.Count);
        Assert.AreEqual(long.MaxValue, snapshot.Measurements["A"]);
    }

}
