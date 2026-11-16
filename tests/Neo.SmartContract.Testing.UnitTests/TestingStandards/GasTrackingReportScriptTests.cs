// Copyright (C) 2015-2026 The Neo Project.
//
// GasTrackingReportScriptTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards;

/// <summary>
/// Executes the real .github/scripts/gas/aggregate-report.ps1 against synthetic TSV input.
/// </summary>
[TestClass]
public class GasTrackingReportScriptTests
{
    private static string FindScript()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var path = Path.Combine(dir.FullName, ".github", "scripts", "gas", "aggregate-report.ps1");
            if (File.Exists(path)) return path;
            dir = dir.Parent;
        }
        Assert.Inconclusive("aggregate-report.ps1 not found.");
        return string.Empty;
    }

    private static string? FindShell()
    {
        foreach (var shell in new[] { "pwsh", "powershell" })
        {
            try
            {
                using var p = Process.Start(new ProcessStartInfo(shell, "-NoProfile -Command exit 0") { UseShellExecute = false, CreateNoWindow = true });
                p!.WaitForExit();
                if (p.ExitCode == 0) return shell;
            }
            catch (System.ComponentModel.Win32Exception) { }
        }
        return null;
    }

    private static (string Markdown, JsonDocument Json) Run(Action<string> arrange)
    {
        var shell = FindShell();
        if (shell == null) Assert.Inconclusive("PowerShell is not available.");
        var dir = Path.Combine(Path.GetTempPath(), "gas-report-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            arrange(dir);
            var psi = new ProcessStartInfo(shell!) { UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true };
            foreach (var a in new[] { "-NoProfile", "-File", FindScript(), "-ReportDirectory", dir, "-NeoCoreVersion", "test" }) psi.ArgumentList.Add(a);
            using var proc = Process.Start(psi)!;
            var err = proc.StandardError.ReadToEnd();
            proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            Assert.AreEqual(0, proc.ExitCode, err);
            return (File.ReadAllText(Path.Combine(dir, "gas-tracking-report.md")),
                JsonDocument.Parse(File.ReadAllText(Path.Combine(dir, "gas-tracking-report.json"))));
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    private static void Tsv(string dir, string assembly, string file, params string[] lines)
    {
        var tests = Path.Combine(dir, "tracked-" + assembly, "tests");
        Directory.CreateDirectory(tests);
        File.WriteAllText(Path.Combine(tests, file + ".tsv"), string.Join("\n", lines) + "\n");
    }

    [TestMethod]
    public void NoImprovements()
    {
        var (md, json) = Run(d => Tsv(d, "A", "t1", "T.A#1\tUnchanged\t100\t100", "T.B#1\tRegression\t100\t120"));
        StringAssert.Contains(md, "No GAS improvements detected.");
        StringAssert.Contains(md, "| T.B#1 | Regression | 100 | 120 |");
        var a = json.RootElement.GetProperty("A");
        Assert.AreEqual(0, a.GetProperty("improvements").GetInt32());
        Assert.AreEqual(1, a.GetProperty("regressions").GetInt32());
        Assert.AreEqual(0, a.GetProperty("totalGasSaved").GetDecimal());
        Assert.AreEqual(0, a.GetProperty("improvementDetails").GetArrayLength());
    }

    [TestMethod]
    public void ImprovementsSortedWithTotalsAcrossAssemblies()
    {
        var (md, json) = Run(d =>
        {
            Tsv(d, "A", "t1", "T.Z#1\tImprovement\t1000000\t950000", "T.B#1\tImprovement\t200\t100");
            Tsv(d, "A", "t2", "T.A#1\tImprovement\t200\t100", "T.U#1\tUnchanged\t5\t5", "T.M#1\tMissing\t5\t");
            Tsv(d, "B", "t1", "T.C#1\tImprovement\t300\t299");
        });
        StringAssert.Contains(md, "| Execution | Baseline | Current | GAS saved | Reduction |");
        var z = md.IndexOf("| T.Z#1 | 1000000 | 950000 | 50000 | 5.00% |", StringComparison.Ordinal);
        var a = md.IndexOf("| T.A#1 | 200 | 100 | 100 | 50.00% |", StringComparison.Ordinal);
        var b = md.IndexOf("| T.B#1 | 200 | 100 | 100 | 50.00% |", StringComparison.Ordinal);
        Assert.IsTrue(z >= 0 && z < a && a < b, md);
        StringAssert.Contains(md, "| T.C#1 | 300 | 299 | 1 | 0.33% |");
        StringAssert.Contains(md, "| T.M#1 | Missing | 5 |");

        var root = json.RootElement;
        var ja = root.GetProperty("A");
        Assert.AreEqual(3, ja.GetProperty("improvements").GetInt32());
        Assert.AreEqual(1, ja.GetProperty("missing").GetInt32());
        Assert.AreEqual(50200, ja.GetProperty("totalGasSaved").GetDecimal());
        Assert.AreEqual(50000, ja.GetProperty("largestGasSaved").GetDecimal());
        var first = ja.GetProperty("improvementDetails")[0];
        Assert.AreEqual("T.Z#1", first.GetProperty("execution").GetString());
        Assert.AreEqual(1000000, first.GetProperty("baseline").GetDecimal());
        Assert.AreEqual(950000, first.GetProperty("current").GetDecimal());
        Assert.AreEqual(50000, first.GetProperty("gasSaved").GetDecimal());
        Assert.AreEqual(5m, first.GetProperty("reductionPercent").GetDecimal());
        Assert.AreEqual(50201, root.GetProperty("_totals").GetProperty("totalGasSaved").GetDecimal());
    }

    [TestMethod]
    public void ZeroBaselineAndLargeValues()
    {
        var (md, json) = Run(d => Tsv(d, "A", "t1",
            "T.Zero#1\tImprovement\t0\t0",
            "T.Big#1\tImprovement\t9223372036854775807\t0",
            "T.Neg#1\tImprovement\t9223372036854775807\t-9223372036854775808"));
        StringAssert.Contains(md, "| T.Zero#1 | 0 | 0 | 0 | 0.00% |");
        StringAssert.Contains(md, "| T.Big#1 | 9223372036854775807 | 0 | 9223372036854775807 | 100.00% |");
        Assert.AreEqual(decimal.Parse("27670116110564327422"), json.RootElement.GetProperty("A").GetProperty("totalGasSaved").GetDecimal());
    }

    [TestMethod]
    public void EscapesIdentifiers()
    {
        // GasTrackingSession writes \t, \n, \r and \\ escaped inside the TSV field.
        var (md, json) = Run(d => Tsv(d, "A", "t1", "T.a\\tb|c\\\\d\\ne#1\tImprovement\t10\t5"));
        StringAssert.Contains(md, "| T.a\\tb\\|c\\\\d\\ne#1 | 10 | 5 | 5 | 50.00% |");
        Assert.AreEqual("T.a\tb|c\\d\ne#1", json.RootElement.GetProperty("A").GetProperty("improvementDetails")[0].GetProperty("execution").GetString());
    }

    [TestMethod]
    public void EmptyAndMissingInputs()
    {
        var (md, json) = Run(d =>
        {
            Directory.CreateDirectory(Path.Combine(d, "tracked-Empty", "tests"));
            File.WriteAllText(Path.Combine(d, "tracked-Empty", "tests", "e.tsv"), "");
            Directory.CreateDirectory(Path.Combine(d, "tracked-NoTests"));
        });
        StringAssert.Contains(md, "- Executions: 0");
        Assert.AreEqual(2, json.RootElement.EnumerateObject().Count(p => !p.Name.StartsWith('_')));
        Assert.AreEqual(0, json.RootElement.GetProperty("_totals").GetProperty("totalGasSaved").GetDecimal());
    }
}
