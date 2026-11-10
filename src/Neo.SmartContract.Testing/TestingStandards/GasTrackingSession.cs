// Copyright (C) 2015-2026 The Neo Project.
//
// GasTrackingSession.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Testing.TestingStandards
{
    /// <summary>
    /// Process-wide collector that validates per-test measurements against version-controlled baselines
    /// (GasBaselines/{assembly}.executions.json) and writes one report file per test.
    /// Baselines are never modified unless a maintainer sets <see cref="GenerateDirectoryVariable"/> (refused in CI).
    /// </summary>
    public static class GasTrackingSession
    {
        public const string GenerateDirectoryVariable = "NEO_GAS_BASELINE_GENERATE_DIR";

        private sealed class AssemblyState
        {
            public GasSnapshot? Baseline;
            public readonly ConcurrentDictionary<string, long> Observed = new(StringComparer.Ordinal);
            public readonly ConcurrentDictionary<string, bool> Tests = new(StringComparer.Ordinal);
        }

        private static readonly ConcurrentDictionary<string, AssemblyState> _states = new(StringComparer.Ordinal);

        public static string? GenerateDirectory
        {
            get
            {
                var dir = Environment.GetEnvironmentVariable(GenerateDirectoryVariable);
                if (string.IsNullOrWhiteSpace(dir)) return null;
                if (IsCi()) throw new InvalidOperationException("GAS baselines must not be generated in CI.");
                return dir;
            }
        }

        private static bool IsCi() =>
            string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

        private static AssemblyState GetState(Assembly assembly)
        {
            var name = assembly.GetName().Name!;
            return _states.GetOrAdd(name, n =>
            {
                var state = new AssemblyState();
                var path = Path.Combine(AppContext.BaseDirectory, "GasBaselines", n + ".executions.json");
                if (File.Exists(path)) state.Baseline = GasSnapshot.Load(path);
                return state;
            });
        }

        /// <summary>
        /// Starts tracking when a baseline exists or generation is requested; otherwise returns null (no overhead).
        /// </summary>
        public static GasExecutionScope? Begin(Assembly assembly, string testId)
        {
            var state = GetState(assembly);
            if (state.Baseline is null && GenerateDirectory is null) return null;
            state.Tests[testId] = true;
            return GasExecutionTracker.Begin(testId);
        }

        /// <summary>
        /// Completes a scope. Returns the report only when the test completed successfully and comparison applies.
        /// </summary>
        public static GasRegressionReport? Complete(Assembly assembly, GasExecutionScope scope, bool testCompleted)
        {
            var state = GetState(assembly);
            foreach (var m in scope.Measurements) state.Observed[m.Id] = m.Gas;
            scope.Dispose();
            WriteGenerated(assembly, scope);
            if (state.Baseline is null) return null;

            var expected = new GasSnapshot();
            foreach (var kv in state.Baseline.Measurements)
                if (BelongsTo(kv.Key, scope.TestId)) expected.Record(kv.Key, kv.Value);
            var report = scope.Evaluate(expected, testCompleted);
            WriteTestReport(assembly, scope.TestId, report);
            return report;
        }

        private static void WriteTestReport(Assembly assembly, string testId, GasRegressionReport report)
        {
            var reportDir = Environment.GetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable);
            if (string.IsNullOrWhiteSpace(reportDir)) return;

            var directory = Path.Combine(reportDir, "tracked-" + assembly.GetName().Name, "tests");
            Directory.CreateDirectory(directory);
            var lines = report.Changes.Select(c => string.Join("\t", c.Name, c.Status,
                c.Baseline?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "",
                c.Current?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? ""));
            File.WriteAllText(Path.Combine(directory, TestFileName(testId) + ".tsv"), string.Join("\n", lines) + "\n", new System.Text.UTF8Encoding(false));
        }

        private static string TestFileName(string testId)
        {
            var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(testId));
            var safe = new string(testId.Select(c => char.IsLetterOrDigit(c) || c == '.' ? c : '_').ToArray());
            if (safe.Length > 120) safe = safe.Substring(safe.Length - 120);
            return safe + "." + Convert.ToHexString(hash, 0, 4);
        }

        private static void WriteGenerated(Assembly assembly, GasExecutionScope scope)
        {
            var generate = GenerateDirectory;
            var measurements = scope.Measurements;
            if (generate is null || measurements.Count == 0) return;

            var directory = Path.Combine(generate, assembly.GetName().Name!);
            Directory.CreateDirectory(directory);
            var snapshot = new GasSnapshot();
            foreach (var m in measurements) snapshot.Record(m.Id, m.Gas);

            var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(scope.TestId));
            snapshot.Save(Path.Combine(directory, TestFileName(scope.TestId) + ".json"));
        }

        internal static bool BelongsTo(string id, string testId) =>
            id.StartsWith(testId + "#", StringComparison.Ordinal) || id.StartsWith(testId + ":", StringComparison.Ordinal);
    }
}
