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
using Neo.SmartContract.Testing.GasRegression;

namespace Neo.SmartContract.Testing.TestingStandards
{
    /// <summary>
    /// Process-wide collector that validates per-test measurements against version-controlled baselines
    /// (one JSON file per test class under GasBaselines/{assembly}) and writes one report file per test.
    /// Baselines are never modified unless a maintainer sets <see cref="GenerateDirectoryVariable"/> (refused in CI).
    /// <see cref="CandidateDirectoryVariable"/> is the CI-safe alternative: it only writes candidate measurements of
    /// passing tests to a separate directory and never touches baselines.
    /// </summary>
    public static class GasTrackingSession
    {
        public const string GenerateDirectoryVariable = "NEO_GAS_BASELINE_GENERATE_DIR";
        public const string CandidateDirectoryVariable = "NEO_GAS_BASELINE_CANDIDATE_DIR";

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

        public static string? CandidateDirectory
        {
            get
            {
                var dir = Environment.GetEnvironmentVariable(CandidateDirectoryVariable);
                if (string.IsNullOrWhiteSpace(dir)) return null;
                var normalized = Path.GetFullPath(dir).Replace('\\', '/');
                if (normalized.Split('/').Contains("GasBaselines", StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException("GAS candidates must not be written to a baseline directory.");
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
                var dir = Path.Combine(AppContext.BaseDirectory, "GasBaselines", n);
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.json");
                    Array.Sort(files, StringComparer.Ordinal);
                    if (files.Length > 0)
                    {
                        var baseline = new GasSnapshot();
                        foreach (var file in files)
                            foreach (var kv in GasSnapshot.Load(file).Measurements)
                                baseline.Record(kv.Key, kv.Value);
                        state.Baseline = baseline;
                    }
                }
                return state;
            });
        }

        /// <summary>
        /// Starts tracking when a baseline exists or generation is requested; otherwise returns null (no overhead).
        /// </summary>
        public static GasExecutionScope? Begin(Assembly assembly, string testId)
        {
            var state = GetState(assembly);
            if (state.Baseline is null && GenerateDirectory is null && CandidateDirectory is null) return null;
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
            WriteCandidate(assembly, scope, testCompleted);
            if (state.Baseline is null) return null;

            var expected = new GasSnapshot();
            foreach (var kv in state.Baseline.Measurements)
                if (BelongsTo(kv.Key, scope.TestId)) expected.Record(kv.Key, kv.Value);

            // A test without any baseline entry is a new test: in candidate mode it is onboarded through the
            // synchronization workflow instead of failing; existing tests keep the strict policy.
            if (testCompleted && expected.Measurements.Count == 0 && scope.Measurements.Count > 0 && CandidateDirectory is not null)
            {
                WriteNewTestReport(assembly, scope);
                return null;
            }
            var report = scope.Evaluate(expected, testCompleted);
            WriteTestReport(assembly, scope.TestId, report);
            return report;
        }

        private static void WriteTestReport(Assembly assembly, string testId, GasRegressionReport report)
        {
            WriteTestReportLines(assembly, testId, report.Changes.Select(c => string.Join("\t", EscapeField(c.Name), c.Status,
                c.Baseline?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "",
                c.Current?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "")));
        }

        private static void WriteNewTestReport(Assembly assembly, GasExecutionScope scope)
        {
            WriteTestReportLines(assembly, scope.TestId, scope.Measurements.Select(m => string.Join("\t", EscapeField(m.Id), "New", "",
                m.Gas.ToString(System.Globalization.CultureInfo.InvariantCulture))));
        }

        private static void WriteTestReportLines(Assembly assembly, string testId, IEnumerable<string> lines)
        {
            var reportDir = Environment.GetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable);
            if (string.IsNullOrWhiteSpace(reportDir)) return;

            var directory = Path.Combine(reportDir, "tracked-" + assembly.GetName().Name, "tests");
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, TestFileName(testId) + ".tsv"), string.Join("\n", lines) + "\n", new System.Text.UTF8Encoding(false));
        }

        private static string EscapeField(string value) => value
            .Replace("\\", "\\\\")
            .Replace("\t", "\\t")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");

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

            snapshot.Save(Path.Combine(directory, TestFileName(scope.TestId) + ".json"));
        }

        private static void WriteCandidate(Assembly assembly, GasExecutionScope scope, bool testCompleted)
        {
            var candidates = CandidateDirectory;
            if (candidates is null) return;

            var directory = Path.Combine(candidates, assembly.GetName().Name!);
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, TestFileName(scope.TestId));
            var identity = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(Path.GetFullPath(path)));
            using var mutex = new System.Threading.Mutex(false, "NeoGasCandidate_" + Convert.ToHexString(identity));
            var acquired = false;
            try
            {
                try { acquired = mutex.WaitOne(TimeSpan.FromSeconds(30)); }
                catch (System.Threading.AbandonedMutexException)
                {
                    acquired = true;
                    WriteAtomic(path + ".unstable", scope.TestId + "\n");
                }
                if (!acquired) throw new IOException("Timed out waiting for the GAS candidate writer.");
                WriteCandidateLocked(path, scope, testCompleted);
            }
            finally
            {
                if (acquired) mutex.ReleaseMutex();
            }
        }

        private static void WriteCandidateLocked(string path, GasExecutionScope scope, bool testCompleted)
        {

            // Failed tests never contribute measurements; a marker makes the run ineligible for synchronization.
            if (!testCompleted)
            {
                WriteAtomic(path + ".failed", scope.TestId + "\n");
                return;
            }
            if (scope.Measurements.Count == 0) return;

            string json;
            try { json = scope.ToSnapshot().ToJson(); }
            catch (ArgumentException)
            {
                WriteAtomic(path + ".unstable", scope.TestId + "\n");
                return;
            }

            // The same test reporting different values (for example on retry) is not deterministic.
            if (File.Exists(path + ".json") && !string.Equals(File.ReadAllText(path + ".json"), json, StringComparison.Ordinal))
            {
                WriteAtomic(path + ".unstable", scope.TestId + "\n");
                return;
            }
            WriteAtomic(path + ".json", json);
        }

        private static void WriteAtomic(string path, string content)
        {
            var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(temporary, content, new System.Text.UTF8Encoding(false));
            File.Move(temporary, path, true);
        }

        internal static bool BelongsTo(string id, string testId) =>
            id.StartsWith(testId + "#", StringComparison.Ordinal) || id.StartsWith(testId + ":", StringComparison.Ordinal);
    }
}
