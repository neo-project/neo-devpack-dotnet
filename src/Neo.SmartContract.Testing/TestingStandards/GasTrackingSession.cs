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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.GasRegression;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Testing.TestingStandards
{
    /// <summary>
    /// Process-wide collector of per-test GAS measurements.
    /// <para>
    /// Default (external consumers): measurements of passing tests are compared with the baselines copied to
    /// <c>GasBaselines/{assembly}</c> next to the test assembly and baselines are never written.
    /// </para>
    /// <para>
    /// Managed mode (this repository's test projects, like <c>dotnet format</c>): <see cref="GasBaselineMode.Update"/>
    /// rewrites the version-controlled baselines when the assembly cleanup runs, <see cref="GasBaselineMode.Verify"/>
    /// (the default in CI) fails when they are not up to date. A test project opts in with assembly metadata
    /// (<see cref="DirectoryMetadataKey"/> and <see cref="ModeMetadataKey"/>) and calls <see cref="Finish"/> from its
    /// <c>[AssemblyCleanup]</c>.
    /// </para>
    /// </summary>
    public static class GasTrackingSession
    {
        public const string ModeVariable = "NEO_GAS_BASELINE_MODE";
        public const string DirectoryVariable = "NEO_GAS_BASELINE_DIR";
        public const string OutputDirectoryVariable = "NEO_GAS_BASELINE_OUTPUT_DIR";
        public const string DirectoryMetadataKey = "Neo.GasBaselines.Directory";
        public const string ModeMetadataKey = "Neo.GasBaselines.Mode";

        private sealed class AssemblyState
        {
            public GasBaselineMode Mode;
            public string? Directory;
            public string? OutputRoot;
            public GasSnapshot? Baseline;
            public readonly object Lock = new();
            public readonly Dictionary<string, GasTestResult> Results = new(StringComparer.Ordinal);
        }

        private static readonly ConcurrentDictionary<string, AssemblyState> _states = new(StringComparer.Ordinal);

        internal static void Reset() => _states.Clear();

        private static bool IsCi() =>
            string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

        private static string? Metadata(Assembly assembly, string key) => assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == key)?.Value;

        private static AssemblyState GetState(Assembly assembly)
        {
            var name = assembly.GetName().Name!;
            return _states.GetOrAdd(name, n =>
            {
                var state = new AssemblyState();
                var explicitMode = Environment.GetEnvironmentVariable(ModeVariable);
                var text = string.IsNullOrWhiteSpace(explicitMode) ? Metadata(assembly, ModeMetadataKey) : explicitMode;
                var mode = GasBaselineMode.Off;
                if (!string.IsNullOrWhiteSpace(text) && !Enum.TryParse(text.Trim(), true, out mode))
                    throw new InvalidOperationException($"Unknown GAS baseline mode '{text}'. Use off, update or verify.");
                if (string.IsNullOrWhiteSpace(explicitMode) && mode == GasBaselineMode.Update && IsCi())
                    mode = GasBaselineMode.Verify;

                var directory = Environment.GetEnvironmentVariable(DirectoryVariable);
                if (string.IsNullOrWhiteSpace(directory)) directory = Metadata(assembly, DirectoryMetadataKey);

                if (mode != GasBaselineMode.Off)
                {
                    if (string.IsNullOrWhiteSpace(directory))
                    {
                        if (!string.IsNullOrWhiteSpace(explicitMode))
                            throw new InvalidOperationException($"GAS baseline mode '{mode}' requires {DirectoryVariable} or the {DirectoryMetadataKey} assembly metadata.");
                        mode = GasBaselineMode.Off;
                    }
                    else if (!System.IO.Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(directory))))
                    {
                        // A build made in another checkout: never create baselines at a stale location.
                        Console.Error.WriteLine($"GAS baselines are not managed: '{directory}' does not belong to this checkout.");
                        mode = GasBaselineMode.Off;
                    }
                }

                state.Mode = mode;
                if (mode != GasBaselineMode.Off)
                {
                    state.Directory = Path.GetFullPath(directory!);
                    var output = Environment.GetEnvironmentVariable(OutputDirectoryVariable);
                    state.OutputRoot = string.IsNullOrWhiteSpace(output) ? null : Path.GetFullPath(output);
                    state.Baseline = ToSnapshot(GasBaselineStore.Load(state.Directory));
                }
                else
                {
                    var dir = Path.Combine(AppContext.BaseDirectory, "GasBaselines", n);
                    if (System.IO.Directory.Exists(dir) && System.IO.Directory.GetFiles(dir, "*.json").Length > 0)
                        state.Baseline = ToSnapshot(GasBaselineStore.Load(dir));
                }
                return state;
            });
        }

        private static GasSnapshot ToSnapshot(IReadOnlyDictionary<string, long> measurements)
        {
            var snapshot = new GasSnapshot();
            foreach (var kv in measurements) snapshot.Record(kv.Key, kv.Value);
            return snapshot;
        }

        /// <summary>
        /// Starts tracking when baselines are managed or exist; otherwise returns null (no overhead).
        /// </summary>
        public static GasExecutionScope? Begin(Assembly assembly, string testId)
        {
            var state = GetState(assembly);
            if (state.Mode == GasBaselineMode.Off && state.Baseline is null) return null;
            return GasExecutionTracker.Begin(testId);
        }

        /// <summary>
        /// Completes a scope. Returns the report only when comparison applies immediately (unmanaged mode and the
        /// test completed); in managed modes the comparison happens in <see cref="Finish"/> for the whole run.
        /// </summary>
        public static GasRegressionReport? Complete(Assembly assembly, GasExecutionScope scope, bool testCompleted)
        {
            var state = GetState(assembly);
            scope.Dispose();

            GasSnapshot expected = new();
            if (state.Baseline is not null)
                foreach (var kv in state.Baseline.Measurements)
                    if (BelongsTo(kv.Key, scope.TestId)) expected.Record(kv.Key, kv.Value);

            if (state.Mode != GasBaselineMode.Off)
            {
                Record(state, scope, testCompleted);
                WriteTestReport(assembly, scope.TestId, scope.Evaluate(expected, testCompleted));
                return null;
            }

            if (state.Baseline is null) return null;
            var report = scope.Evaluate(expected, testCompleted);
            WriteTestReport(assembly, scope.TestId, report);
            return report;
        }

        private static void Record(AssemblyState state, GasExecutionScope scope, bool testCompleted)
        {
            GasTestResult result;
            if (!testCompleted) result = new GasTestResult(scope.TestId, GasTestStatus.Failed, new Dictionary<string, long>());
            else
            {
                try { result = new GasTestResult(scope.TestId, GasTestStatus.Passed, scope.ToSnapshot().Measurements.ToDictionary(m => m.Key, m => m.Value, StringComparer.Ordinal)); }
                catch (ArgumentException) { result = new GasTestResult(scope.TestId, GasTestStatus.Invalid, new Dictionary<string, long>()); }
            }

            lock (state.Lock)
            {
                if (!state.Results.TryGetValue(scope.TestId, out var previous)) state.Results[scope.TestId] = result;
                else if (previous.Status == GasTestStatus.Passed && result.Status == GasTestStatus.Passed)
                {
                    var same = previous.Measurements.Count == result.Measurements.Count &&
                        previous.Measurements.All(kv => result.Measurements.TryGetValue(kv.Key, out var gas) && gas == kv.Value);
                    if (!same) state.Results[scope.TestId] = new GasTestResult(scope.TestId, GasTestStatus.Unstable, new Dictionary<string, long>());
                }
                else if (result.Status > previous.Status) state.Results[scope.TestId] = result;
            }
        }

        /// <summary>
        /// Reconciles (update) or verifies (verify) the version-controlled baselines with the whole run.
        /// Call it from the test project's <c>[AssemblyCleanup]</c>. Does nothing when baselines are not managed
        /// or no tracked test ran.
        /// </summary>
        public static void Finish(Assembly assembly)
        {
            var state = GetState(assembly);
            if (state.Mode == GasBaselineMode.Off) return;

            GasTestResult[] results;
            lock (state.Lock) results = state.Results.Values.ToArray();
            if (results.Length == 0) return;

            var name = assembly.GetName().Name!;
            var sync = GasBaselineStore.Synchronize(state.Directory!, name, results, BuildInventory(assembly), state.Mode,
                requireCompleteRun: state.Mode == GasBaselineMode.Verify, state.OutputRoot);

            foreach (var note in sync.Reconciliation.Notes) Console.WriteLine($"GAS baselines: {note}");
            if (state.Mode == GasBaselineMode.Update && sync.ChangedFiles.Count > 0)
                Console.WriteLine($"GAS baselines updated in {state.Directory}; commit them with your change:{Environment.NewLine}  " + string.Join(Environment.NewLine + "  ", sync.ChangedFiles));

            if (state.OutputRoot is not null)
            {
                Directory.CreateDirectory(state.OutputRoot);
                File.WriteAllLines(Path.Combine(state.OutputRoot, name + ".diff.txt"), sync.Differences.Concat(sync.ChangedFiles.Select(f => "file " + f)));
            }

            if (sync.Errors.Count == 0) return;

            var lines = new List<string>(sync.Errors);
            lines.AddRange(sync.Differences.Take(50));
            if (sync.Differences.Count > 50) lines.Add($"... and {sync.Differences.Count - 50} more differences");
            lines.AddRange(sync.ChangedFiles.Select(f => "file " + f));
            if (state.Mode == GasBaselineMode.Verify)
                lines.Add($"Regenerate them locally with 'dotnet test' on this test project (full run, no filter) and commit the changes under {state.Directory}.");
            Assert.Fail("GAS baselines check failed:" + Environment.NewLine + string.Join(Environment.NewLine, lines));
        }

        /// <summary>
        /// Methods that a complete run of the assembly executes, found by reflection.
        /// </summary>
        internal static GasRunInventory BuildInventory(Assembly assembly)
        {
            var expected = new Dictionary<string, int>(StringComparer.Ordinal);
            var ignored = new HashSet<string>(StringComparer.Ordinal);

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsGenericTypeDefinition || type.GetCustomAttribute<TestClassAttribute>() is null || !DerivesFromTestBase(type))
                    continue;

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (method.GetCustomAttribute<TestMethodAttribute>() is null) continue;
                    var key = type.FullName + "." + method.Name;
                    if (method.GetCustomAttribute<IgnoreAttribute>() is not null || type.GetCustomAttribute<IgnoreAttribute>() is not null) continue;

                    var sources = method.GetCustomAttributes().OfType<ITestDataSource>().ToArray();
                    var conditional = method.GetCustomAttributes().Concat(type.GetCustomAttributes()).Any(a => a is ConditionBaseAttribute);
                    if (conditional || sources.Any(s => s is not DataRowAttribute)) ignored.Add(key);
                    else expected[key] = Math.Max(1, sources.Length);
                }
            }
            return new GasRunInventory(expected, ignored);
        }

        private static bool DerivesFromTestBase(Type type)
        {
            for (var current = type.BaseType; current is not null; current = current.BaseType)
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(TestBase<>)) return true;
            return false;
        }

        private static void WriteTestReport(Assembly assembly, string testId, GasRegressionReport report)
        {
            var reportDir = Environment.GetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable);
            if (string.IsNullOrWhiteSpace(reportDir)) return;

            var directory = Path.Combine(reportDir, "tracked-" + assembly.GetName().Name, "tests");
            Directory.CreateDirectory(directory);
            var lines = report.Changes.Select(c => string.Join("\t", EscapeField(c.Name), c.Status,
                c.Baseline?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "",
                c.Current?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? ""));
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

        internal static bool BelongsTo(string id, string testId) =>
            id.StartsWith(testId + "#", StringComparison.Ordinal) || id.StartsWith(testId + ":", StringComparison.Ordinal);
    }
}
