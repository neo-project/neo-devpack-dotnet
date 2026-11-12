// Copyright (C) 2015-2026 The Neo Project.
//
// GasRegressionPolicyTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.GasRegression;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards;

[TestClass]
[DoNotParallelize]
public class GasRegressionPolicyTests
{
    private static GasSnapshot Snap(long transfer) => new GasSnapshot().Record("transfer", transfer);

    [TestMethod]
    public void EqualPasses()
    {
        var report = GasRegressionReport.Evaluate(Snap(1000), Snap(1000));
        Assert.IsFalse(report.HasFailures);
        report.AssertNoRegression();
    }

    [TestMethod]
    public void ImprovementPassesAndIsReported()
    {
        var baseline = Snap(1000);
        var current = Snap(800);
        var report = GasRegressionReport.Evaluate(baseline, current);
        report.AssertNoRegression();
        Assert.AreEqual(GasChangeStatus.Improvement, report.Changes[0].Status);
        Assert.AreEqual((Int128)(-200), report.Changes[0].Difference);
        StringAssert.Contains(report.ToMarkdown(), "| transfer | 1000 | 800 | -200 | Improvement |");
        // The strict, symmetric API keeps its contract.
        Assert.ThrowsExactly<InvalidOperationException>(() => current.AssertGasWithin(baseline));
    }

    [TestMethod]
    public void RegressionFailsWithValues()
    {
        var report = GasRegressionReport.Evaluate(Snap(1000), Snap(1200));
        var ex = Assert.ThrowsExactly<InvalidOperationException>(report.AssertNoRegression);
        StringAssert.Contains(ex.Message, "expected 1000");
        StringAssert.Contains(ex.Message, "actual 1200");
    }

    [TestMethod]
    public void ToleranceIsApplied()
    {
        GasRegressionReport.Evaluate(Snap(1000), Snap(1005), 5).AssertNoRegression();
        Assert.ThrowsExactly<InvalidOperationException>(() => GasRegressionReport.Evaluate(Snap(1000), Snap(1005), 4).AssertNoRegression());
    }

    [TestMethod]
    public void MissingMeasurementFails()
    {
        var report = GasRegressionReport.Evaluate(Snap(1000), new GasSnapshot());
        Assert.AreEqual(GasChangeStatus.Missing, report.Changes[0].Status);
        var ex = Assert.ThrowsExactly<InvalidOperationException>(report.AssertNoRegression);
        StringAssert.Contains(ex.Message, "missing measurement");
    }

    [TestMethod]
    public void UnexpectedMeasurementFails()
    {
        var report = GasRegressionReport.Evaluate(new GasSnapshot(), Snap(1000));
        Assert.AreEqual(GasChangeStatus.Unexpected, report.Changes[0].Status);
        var ex = Assert.ThrowsExactly<InvalidOperationException>(report.AssertNoRegression);
        StringAssert.Contains(ex.Message, "baseline update");
    }

    [TestMethod]
    public void MarkdownIsDeterministic()
    {
        var baseline = GasSnapshot.Capture([("z-operation", 1000L), ("a-operation", 1000L)]);
        var current = GasSnapshot.Capture([("a-operation", 800L), ("z-operation", 1200L)]);
        var reorderedBaseline = GasSnapshot.Capture([("a-operation", 1000L), ("z-operation", 1000L)]);
        var reorderedCurrent = GasSnapshot.Capture([("z-operation", 1200L), ("a-operation", 800L)]);
        var a = GasRegressionReport.Evaluate(baseline, current).ToMarkdown([
            new("Optimization", "All"), new("Contract", "Contract_String")
        ]);
        var b = GasRegressionReport.Evaluate(reorderedBaseline, reorderedCurrent).ToMarkdown([
            new("Contract", "Contract_String"), new("Optimization", "All")
        ]);
        Assert.AreEqual(a, b);
        Assert.IsFalse(a.Contains('\r'));
        Assert.IsTrue(a.IndexOf("| a-operation", StringComparison.Ordinal) < a.IndexOf("| z-operation", StringComparison.Ordinal));
    }

    [TestMethod]
    public void MarkdownIsCultureIndependent()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var expected = GasRegressionReport.Evaluate(Snap(1000), Snap(1234567)).ToMarkdown();
            foreach (var name in new[] { "de-DE", "fr-FR", "ar-SA" })
            {
                CultureInfo.CurrentCulture = new CultureInfo(name);
                Assert.AreEqual(expected, GasRegressionReport.Evaluate(Snap(1000), Snap(1234567)).ToMarkdown());
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [TestMethod]
    public void LargeValuesDoNotOverflow()
    {
        var report = GasRegressionReport.Evaluate(Snap(0), Snap(long.MaxValue), long.MaxValue);
        report.AssertNoRegression();
        Assert.AreEqual((Int128)long.MaxValue, report.Changes[0].Difference);

        var down = GasRegressionReport.Evaluate(Snap(long.MaxValue), Snap(0));
        Assert.AreEqual(GasChangeStatus.Improvement, down.Changes[0].Status);
        Assert.AreEqual(GasChangeStatus.Regression, GasRegressionReport.Evaluate(Snap(long.MaxValue - 1), Snap(long.MaxValue)).Changes[0].Status);
    }

    [TestMethod]
    public void ReportIsWrittenBeforeRegressionFailure()
    {
        var dir = Path.Combine(Path.GetTempPath(), "gas-report-" + Guid.NewGuid().ToString("N"));
        var previous = Environment.GetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable);
        try
        {
            Environment.SetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable, dir);
            var baseline = Snap(1000);
            var current = Snap(1200);
            var report = GasRegressionReport.Evaluate(baseline, current);
            Assert.ThrowsExactly<InvalidOperationException>(() => report.WriteReportIfConfiguredAndAssert());

            StringAssert.Contains(File.ReadAllText(Path.Combine(dir, "gas-regression-report.md")), "| transfer | 1000 | 1200 | +200 | Regression |");
            Assert.AreEqual(baseline.ToJson(), File.ReadAllText(Path.Combine(dir, "baseline.json")));
            Assert.AreEqual(current.ToJson(), File.ReadAllText(Path.Combine(dir, "current.json")));
        }
        finally
        {
            Environment.SetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable, previous);
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
    }

    [TestMethod]
    public void NoFilesWithoutEnvironmentVariable()
    {
        var previous = Environment.GetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable);
        try
        {
            Environment.SetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable, null);
            var report = GasRegressionReport.Evaluate(Snap(1), Snap(1));
            report.WriteReportIfConfiguredAndAssert();
        }
        finally
        {
            Environment.SetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable, previous);
        }
    }

    [TestMethod]
    public void ChangesCannotBeModifiedToBypassRegression()
    {
        var report = GasRegressionReport.Evaluate(Snap(1000), Snap(1200));
        Assert.IsFalse(report.Changes is GasChange[]);
        var changes = (IList<GasChange>)report.Changes;
        var replacement = new GasChange("transfer", 1000, 1200, GasChangeStatus.Improvement);
        Assert.ThrowsExactly<NotSupportedException>(() => changes[0] = replacement);
        Assert.ThrowsExactly<NotSupportedException>(() => changes.Add(replacement));
        Assert.ThrowsExactly<InvalidOperationException>(report.AssertNoRegression);
    }

    [TestMethod]
    public void PublishedFilesRetainEvaluatedMeasurements()
    {
        var dir = Path.Combine(Path.GetTempPath(), "gas-report-" + Guid.NewGuid().ToString("N"));
        var baseline = Snap(1000);
        var current = Snap(1000);
        var baselineJson = baseline.ToJson();
        var currentJson = current.ToJson();
        var report = GasRegressionReport.Evaluate(baseline, current);
        var markdown = report.ToMarkdown();
        baseline.Record("added-later-to-baseline", 10);
        current.Record("added-later-to-current", 20);
        try
        {
            report.WriteTo(dir);
            report.AssertNoRegression();
            Assert.AreEqual(baselineJson, File.ReadAllText(Path.Combine(dir, "baseline.json")));
            Assert.AreEqual(currentJson, File.ReadAllText(Path.Combine(dir, "current.json")));
            Assert.AreEqual(markdown, File.ReadAllText(Path.Combine(dir, "gas-regression-report.md")));
            Assert.AreEqual(1, report.Changes.Count);
            Assert.AreEqual(2, baseline.Measurements.Count);
            Assert.AreEqual(2, current.Measurements.Count);
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
    }

    [TestMethod]
    public void MarkdownEscapesOperationNamesAndMetadata()
    {
        const string name = "cost|<x>\r\n`call`";
        var baseline = new GasSnapshot().Record(name, 50);
        var current = new GasSnapshot().Record(name, 75);
        var markdown = GasRegressionReport.Evaluate(baseline, current).ToMarkdown([
            new("Info", "<x>\r\n`call` &")
        ]);
        StringAssert.Contains(markdown, "| cost&#124;&lt;x&gt;<br>\\`call\\` | 50 | 75 | +25 | Regression |");
        StringAssert.Contains(markdown, "Info: &lt;x&gt;<br>\\`call\\` &amp;\n");
        Assert.IsFalse(markdown.Contains('\r'));
    }

    [TestMethod]
    public void ToleratedIncreaseIsNotReportedAsUnchanged()
    {
        var report = GasRegressionReport.Evaluate(Snap(1000), Snap(1005), 5);
        report.AssertNoRegression();
        Assert.AreEqual(GasChangeStatus.WithinTolerance, report.Changes[0].Status);
        var markdown = report.ToMarkdown();
        StringAssert.Contains(markdown, "- Unchanged: 0\n");
        StringAssert.Contains(markdown, "- Within tolerance: 1\n");
        StringAssert.Contains(markdown, "| transfer | 1000 | 1005 | +5 | Within tolerance |");
    }

    [TestMethod]
    public void EvaluateRejectsInvalidArguments()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => GasRegressionReport.Evaluate(null!, Snap(1000)));
        Assert.ThrowsExactly<ArgumentNullException>(() => GasRegressionReport.Evaluate(Snap(1000), null!));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => GasRegressionReport.Evaluate(Snap(1000), Snap(1000), -1));
    }
}
