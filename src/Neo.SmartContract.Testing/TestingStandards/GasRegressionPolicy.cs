// Copyright (C) 2015-2026 The Neo Project.
//
// GasRegressionPolicy.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Testing.TestingStandards;

/// <summary>
/// Classification of a single gas measurement compared with its baseline.
/// </summary>
public enum GasChangeStatus
{
    Unchanged,
    WithinTolerance,
    Improvement,
    Regression,
    Missing,
    Unexpected
}

/// <summary>
/// One compared measurement.
/// </summary>
public sealed record GasChange(string Name, long? Baseline, long? Current, GasChangeStatus Status)
{
    /// <summary>
    /// Current minus baseline, or null when either side is absent.
    /// </summary>
    public Int128? Difference => Baseline is null || Current is null ? null : (Int128)Current.Value - Baseline.Value;

    /// <summary>
    /// True when this change must fail CI.
    /// </summary>
    public bool IsFailure => Status is GasChangeStatus.Regression or GasChangeStatus.Missing or GasChangeStatus.Unexpected;
}

/// <summary>
/// CI regression policy: gas decreases pass, increases beyond the tolerance fail,
/// and missing or unexpected measurements fail until the baseline is reviewed.
/// Unlike <see cref="GasSnapshot.AssertGasWithin"/>, this policy is not symmetric.
/// </summary>
public sealed class GasRegressionReport
{
    /// <summary>
    /// Environment variable naming the directory where reports are written.
    /// </summary>
    public const string ReportDirectoryVariable = "NEO_GAS_REPORT_DIR";
    private readonly GasSnapshot _baseline;
    private readonly GasSnapshot _current;

    private GasRegressionReport(GasSnapshot baseline, GasSnapshot current, GasChange[] changes, long tolerance)
    {
        _baseline = baseline;
        _current = current;
        Changes = Array.AsReadOnly(changes);
        Tolerance = tolerance;
    }

    public IReadOnlyList<GasChange> Changes { get; }

    public long Tolerance { get; }

    public bool HasFailures => Changes.Any(c => c.IsFailure);

    /// <summary>
    /// Compares private copies of a current snapshot and baseline so later changes
    /// to the input snapshots cannot alter the report or its published files.
    /// </summary>
    public static GasRegressionReport Evaluate(GasSnapshot baseline, GasSnapshot current, long tolerance = 0)
    {
        ArgumentNullException.ThrowIfNull(baseline);
        ArgumentNullException.ThrowIfNull(current);
        ArgumentOutOfRangeException.ThrowIfNegative(tolerance);

        baseline = GasSnapshot.Capture(baseline.Measurements.Select(measurement => (measurement.Key, measurement.Value)));
        current = GasSnapshot.Capture(current.Measurements.Select(measurement => (measurement.Key, measurement.Value)));

        var changes = baseline.Measurements.Keys
            .Union(current.Measurements.Keys, StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .Select(name =>
            {
                var hasBaseline = baseline.Measurements.TryGetValue(name, out var expected);
                var hasCurrent = current.Measurements.TryGetValue(name, out var actual);
                if (!hasCurrent) return new GasChange(name, expected, null, GasChangeStatus.Missing);
                if (!hasBaseline) return new GasChange(name, null, actual, GasChangeStatus.Unexpected);
                var difference = (Int128)actual - expected;
                var status = difference == 0 ? GasChangeStatus.Unchanged
                    : difference < 0 ? GasChangeStatus.Improvement
                    : difference <= tolerance ? GasChangeStatus.WithinTolerance
                    : GasChangeStatus.Regression;
                return new GasChange(name, expected, actual, status);
            })
            .ToArray();
        return new GasRegressionReport(baseline, current, changes, tolerance);
    }

    /// <summary>
    /// Renders a deterministic Markdown report (invariant culture, LF line endings).
    /// </summary>
    /// <param name="metadata">Optional stable key/value lines (for example optimization level).</param>
    public string ToMarkdown(IEnumerable<KeyValuePair<string, string>>? metadata = null)
    {
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.Append("# Neo DevPack GAS Regression Report\n\n");
        foreach (var (key, value) in (metadata ?? [])
            .OrderBy(item => item.Key, StringComparer.Ordinal)
            .ThenBy(item => item.Value, StringComparer.Ordinal))
            sb.Append(GasSnapshot.EscapeMarkdown(key)).Append(": ").Append(GasSnapshot.EscapeMarkdown(value)).Append('\n');
        sb.Append("Policy: Fail on GAS regression greater than ").Append(Tolerance.ToString(inv)).Append(" datoshi\n");
        sb.Append("Result: ").Append(HasFailures ? "FAILED" : "PASSED").Append("\n\n");
        sb.Append("## Summary\n\n");
        sb.Append("- Benchmarks: ").Append(Changes.Count.ToString(inv)).Append('\n');
        sb.Append("- Improvements: ").Append(Count(GasChangeStatus.Improvement).ToString(inv)).Append('\n');
        sb.Append("- Unchanged: ").Append(Count(GasChangeStatus.Unchanged).ToString(inv)).Append('\n');
        sb.Append("- Within tolerance: ").Append(Count(GasChangeStatus.WithinTolerance).ToString(inv)).Append('\n');
        sb.Append("- Regressions: ").Append(Count(GasChangeStatus.Regression).ToString(inv)).Append('\n');
        sb.Append("- Missing: ").Append(Count(GasChangeStatus.Missing).ToString(inv)).Append('\n');
        sb.Append("- Unexpected: ").Append(Count(GasChangeStatus.Unexpected).ToString(inv)).Append("\n\n");
        sb.Append("## Changes\n\n");
        sb.Append("| Operation | Baseline | Current | Difference | Status |\n| --- | ---: | ---: | ---: | --- |\n");
        foreach (var change in Changes)
        {
            sb.Append("| ").Append(GasSnapshot.EscapeMarkdown(change.Name))
                .Append(" | ").Append(Format(change.Baseline))
                .Append(" | ").Append(Format(change.Current))
                .Append(" | ").Append(change.Difference is { } d ? d.ToString("+0;-0;0", inv) : "n/a")
                .Append(" | ").Append(Describe(change.Status)).Append(" |\n");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Writes the evaluated snapshots and report to a directory as baseline.json,
    /// current.json and gas-regression-report.md.
    /// </summary>
    public void WriteTo(string directory, IEnumerable<KeyValuePair<string, string>>? metadata = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        Directory.CreateDirectory(directory);
        _baseline.Save(Path.Combine(directory, "baseline.json"));
        _current.Save(Path.Combine(directory, "current.json"));
        File.WriteAllText(Path.Combine(directory, "gas-regression-report.md"), ToMarkdown(metadata));
    }

    /// <summary>
    /// Throws when any measurement is a regression, missing or unexpected.
    /// </summary>
    public void AssertNoRegression()
    {
        if (!HasFailures) return;
        var details = string.Join(", ", Changes.Where(c => c.IsFailure).Select(c =>
            $"{c.Name} ({Describe(c.Status).ToLowerInvariant()}: expected {Format(c.Baseline)}, actual {Format(c.Current)})"));
        throw new InvalidOperationException(
            $"GAS regression policy failed (tolerance {Tolerance.ToString(CultureInfo.InvariantCulture)}): {details}. " +
            "Missing or unexpected measurements require an intentional, reviewed baseline update.");
    }

    /// <summary>
    /// Writes reports when <see cref="ReportDirectoryVariable"/> is set, then asserts the policy.
    /// </summary>
    public void WriteReportIfConfiguredAndAssert(IEnumerable<KeyValuePair<string, string>>? metadata = null)
    {
        var directory = Environment.GetEnvironmentVariable(ReportDirectoryVariable);
        if (!string.IsNullOrWhiteSpace(directory))
            WriteTo(directory, metadata);
        AssertNoRegression();
    }

    private int Count(GasChangeStatus status) => Changes.Count(c => c.Status == status);

    private static string Format(long? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "missing";

    private static string Describe(GasChangeStatus status) => status switch
    {
        GasChangeStatus.Unchanged => "Unchanged",
        GasChangeStatus.WithinTolerance => "Within tolerance",
        GasChangeStatus.Improvement => "Improvement",
        GasChangeStatus.Regression => "Regression",
        GasChangeStatus.Missing => "Missing measurement",
        _ => "Unexpected measurement"
    };

}
