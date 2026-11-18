// Copyright (C) 2015-2026 The Neo Project.
//
// GasBaselineStore.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Neo.SmartContract.Testing.TestingStandards
{
    /// <summary>
    /// What <see cref="GasBaselineStore.Synchronize"/> does with the version-controlled baselines.
    /// </summary>
    public enum GasBaselineMode
    {
        /// <summary>Baselines are not managed.</summary>
        Off,
        /// <summary>Baselines are rewritten in the source directory (like <c>dotnet format</c>).</summary>
        Update,
        /// <summary>Baselines are only compared; a mismatch is an error.</summary>
        Verify
    }

    public enum GasTestStatus
    {
        Passed,
        Failed,
        Invalid,
        Unstable
    }

    /// <summary>
    /// Measurements of one executed test.
    /// </summary>
    public sealed record GasTestResult(string TestId, GasTestStatus Status, IReadOnlyDictionary<string, long> Measurements);

    /// <summary>
    /// Tests that a complete run of the assembly must execute (<c>Class.Method</c>), and the ones that are
    /// ignored on purpose, whose baselines are always preserved.
    /// </summary>
    public sealed record GasRunInventory(IReadOnlyDictionary<string, int> ExpectedMethods, IReadOnlyCollection<string> IgnoredMethods);

    public sealed class GasBaselineReconciliation
    {
        public GasBaselineReconciliation(IReadOnlyDictionary<string, long> measurements, bool isCompleteRun,
            IReadOnlyList<string> notes, IReadOnlyList<string> errors)
        {
            Measurements = measurements;
            IsCompleteRun = isCompleteRun;
            Notes = notes;
            Errors = errors;
        }

        /// <summary>Resulting baseline entries.</summary>
        public IReadOnlyDictionary<string, long> Measurements { get; }

        /// <summary>True only when every expected test ran and passed, so stale entries may be removed.</summary>
        public bool IsCompleteRun { get; }

        public IReadOnlyList<string> Notes { get; }

        /// <summary>Invalid or unstable measurements.</summary>
        public IReadOnlyList<string> Errors { get; }
    }

    public sealed class GasBaselineSyncResult
    {
        public GasBaselineSyncResult(GasBaselineReconciliation reconciliation, IReadOnlyList<string> changedFiles,
            IReadOnlyList<string> differences, IReadOnlyList<string> errors, string? outputDirectory)
        {
            Reconciliation = reconciliation;
            ChangedFiles = changedFiles;
            Differences = differences;
            Errors = errors;
            OutputDirectory = outputDirectory;
        }

        public GasBaselineReconciliation Reconciliation { get; }

        /// <summary>Files that were (update) or would be (verify) added "+", changed "~" or removed "-".</summary>
        public IReadOnlyList<string> ChangedFiles { get; }

        /// <summary>Entry-level differences between the committed baseline and the measured one.</summary>
        public IReadOnlyList<string> Differences { get; }

        /// <summary>Problems that must fail the run.</summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>Isolated directory with the generated files (verify mode only).</summary>
        public string? OutputDirectory { get; }
    }

    /// <summary>
    /// Deterministic reading, rendering and reconciliation of per-assembly GAS baseline files.
    /// Layout: one JSON object per file, entries in test order, two-space indent, LF, no trailing newline,
    /// UTF-8 without BOM; one file per test class, split into <c>Class.partNN.json</c> past 2000 entries.
    /// </summary>
    public static class GasBaselineStore
    {
        public const int MaxEntriesPerFile = 2000;

        private static readonly UTF8Encoding Utf8 = new(false);

        /// <summary>
        /// Test identifier of an execution identifier (<c>Class.Method[case]#n</c> or <c>Class.Method:label</c>).
        /// </summary>
        public static string TestIdOf(string executionId)
        {
            var depth = 0;
            for (var i = 0; i < executionId.Length; i++)
            {
                var c = executionId[i];
                if (c == '[') depth++;
                else if (c == ']') depth--;
                else if (depth == 0 && (c == '#' || c == ':')) return executionId.Substring(0, i);
                if (depth < 0) break;
            }
            throw new InvalidDataException($"Invalid GAS execution identifier '{executionId}'.");
        }

        /// <summary>
        /// <c>Class.Method</c> of a test identifier.
        /// </summary>
        public static string MethodOf(string testId)
        {
            var index = testId.IndexOf('[');
            return index < 0 ? testId : testId.Substring(0, index);
        }

        private static string ClassOf(string testId)
        {
            var method = MethodOf(testId);
            var index = method.LastIndexOf('.');
            return index < 0 ? method : method.Substring(0, index);
        }

        /// <summary>
        /// Reads every <c>*.json</c> file of a directory (ordinal order). A missing directory is empty.
        /// </summary>
        public static SortedDictionary<string, long> Load(string directory)
        {
            var result = new SortedDictionary<string, long>(StringComparer.Ordinal);
            if (!Directory.Exists(directory)) return result;

            var files = Directory.GetFiles(directory, "*.json");
            Array.Sort(files, StringComparer.Ordinal);
            foreach (var file in files)
                foreach (var kv in GasSnapshot.Load(file).Measurements)
                    if (!result.TryAdd(kv.Key, kv.Value))
                        throw new InvalidDataException($"Duplicate GAS measurement '{kv.Key}' in '{file}'.");
            return result;
        }

        /// <summary>
        /// Renders measurements as baseline files (file name to content).
        /// </summary>
        public static IReadOnlyDictionary<string, string> Render(IReadOnlyDictionary<string, long> measurements, int maxEntriesPerFile = MaxEntriesPerFile)
        {
            var classes = new SortedDictionary<string, SortedDictionary<string, List<KeyValuePair<string, long>>>>(StringComparer.Ordinal);
            foreach (var kv in measurements.OrderBy(m => m.Key, StringComparer.Ordinal))
            {
                var test = TestIdOf(kv.Key);
                var cls = ClassOf(test);
                if (!classes.TryGetValue(cls, out var tests))
                    classes[cls] = tests = new SortedDictionary<string, List<KeyValuePair<string, long>>>(StringComparer.Ordinal);
                if (!tests.TryGetValue(test, out var entries)) tests[test] = entries = new List<KeyValuePair<string, long>>();
                entries.Add(kv);
            }

            var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var (cls, tests) in classes)
            {
                var chunks = new List<List<KeyValuePair<string, long>>>();
                var current = new List<KeyValuePair<string, long>>();
                foreach (var entry in tests.Values.SelectMany(e => e))
                {
                    if (current.Count >= maxEntriesPerFile)
                    {
                        chunks.Add(current);
                        current = new List<KeyValuePair<string, long>>();
                    }
                    current.Add(entry);
                }
                if (current.Count > 0) chunks.Add(current);

                for (var i = 0; i < chunks.Count; i++)
                {
                    var name = chunks.Count == 1 ? cls + ".json" : string.Format(CultureInfo.InvariantCulture, "{0}.part{1:D2}.json", cls, i + 1);
                    files[name] = "{\n" + string.Join(",\n", chunks[i].Select(e =>
                        "  " + Quote(e.Key) + ": " + e.Value.ToString(CultureInfo.InvariantCulture))) + "\n}";
                }
            }
            return files;
        }

        private static string Quote(string text)
        {
            var sb = new System.Text.StringBuilder(text.Length + 2).Append('"');
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        var pair = char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]);
                        if (pair) { sb.Append(c).Append(text[++i]); }
                        else if (c < 0x20 || c == '\'' || c == '<' || c == '>' || c == '&' || char.IsSurrogate(c))
                            sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else sb.Append(c);
                        break;
                }
            }
            return sb.Append('"').ToString();
        }

        /// <summary>
        /// Applies the measurements of the executed tests to the committed baseline.
        /// Only tests that passed are updated. Entries of other tests are removed only when the run is complete.
        /// </summary>
        public static GasBaselineReconciliation Reconcile(IReadOnlyDictionary<string, long> committed,
            IReadOnlyCollection<GasTestResult> results, GasRunInventory inventory)
        {
            var notes = new List<string>();
            var errors = new List<string>();
            var final = new SortedDictionary<string, long>(StringComparer.Ordinal);
            foreach (var kv in committed) final[kv.Key] = kv.Value;

            var byTest = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var key in committed.Keys)
            {
                var test = TestIdOf(key);
                if (!byTest.TryGetValue(test, out var keys)) byTest[test] = keys = new List<string>();
                keys.Add(key);
            }

            var passed = new HashSet<string>(StringComparer.Ordinal);
            foreach (var result in results.OrderBy(r => r.TestId, StringComparer.Ordinal))
            {
                switch (result.Status)
                {
                    case GasTestStatus.Passed:
                        passed.Add(result.TestId);
                        if (byTest.TryGetValue(result.TestId, out var old))
                            foreach (var key in old) final.Remove(key);
                        foreach (var kv in result.Measurements) final[kv.Key] = kv.Value;
                        break;
                    case GasTestStatus.Failed:
                        notes.Add($"{result.TestId} did not pass; its baseline was preserved.");
                        break;
                    case GasTestStatus.Invalid:
                        errors.Add($"{result.TestId} reported duplicate or invalid GAS measurements; its baseline was preserved.");
                        break;
                    default:
                        errors.Add($"{result.TestId} reported different GAS values in the same run (unstable); its baseline was preserved.");
                        break;
                }
            }

            var reasons = new List<string>();
            if (results.Any(r => r.Status != GasTestStatus.Passed)) reasons.Add("some tests failed, were skipped or reported invalid measurements");
            if (inventory.ExpectedMethods.Count == 0) reasons.Add("no tests were discovered");
            var passedByMethod = passed.GroupBy(MethodOf, StringComparer.Ordinal).ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
            foreach (var (method, cases) in inventory.ExpectedMethods.OrderBy(m => m.Key, StringComparer.Ordinal))
            {
                passedByMethod.TryGetValue(method, out var count);
                if (count < cases) reasons.Add($"{method} passed {count} of {cases} cases");
            }

            var complete = reasons.Count == 0;
            if (complete)
            {
                var ignored = new HashSet<string>(inventory.IgnoredMethods, StringComparer.Ordinal);
                foreach (var key in final.Keys.ToArray())
                {
                    var test = TestIdOf(key);
                    if (!passed.Contains(test) && !ignored.Contains(MethodOf(test))) final.Remove(key);
                }
            }
            else
            {
                notes.Add("Partial run: baselines of tests that were not executed successfully were preserved.");
                foreach (var reason in reasons.Take(10)) notes.Add("  " + reason);
                if (reasons.Count > 10) notes.Add($"  ... and {reasons.Count - 10} more");
            }

            return new GasBaselineReconciliation(final, complete, notes, errors);
        }

        /// <summary>
        /// Reconciles the run with the baselines in <paramref name="directory"/> and either updates the files
        /// or verifies that they are already up to date. Safe against concurrent processes.
        /// </summary>
        public static GasBaselineSyncResult Synchronize(string directory, string assemblyName, IReadOnlyCollection<GasTestResult> results,
            GasRunInventory inventory, GasBaselineMode mode, bool requireCompleteRun, string? outputRoot = null)
        {
            if (mode == GasBaselineMode.Off) throw new ArgumentException("Baselines are not managed.", nameof(mode));

            var identity = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (OperatingSystem.IsWindows()) identity = identity.ToUpperInvariant();
            using var mutex = new Mutex(false, "Neo.GasBaselines." + Convert.ToHexString(SHA256.HashData(Utf8.GetBytes(identity))));
            var acquired = false;
            try
            {
                try { acquired = mutex.WaitOne(TimeSpan.FromSeconds(60)); }
                catch (AbandonedMutexException) { acquired = true; }
                if (!acquired) throw new IOException("Timed out waiting for another process updating the GAS baselines.");
                return SynchronizeLocked(directory, assemblyName, results, inventory, mode, requireCompleteRun, outputRoot);
            }
            finally
            {
                if (acquired) mutex.ReleaseMutex();
            }
        }

        private static GasBaselineSyncResult SynchronizeLocked(string directory, string assemblyName, IReadOnlyCollection<GasTestResult> results,
            GasRunInventory inventory, GasBaselineMode mode, bool requireCompleteRun, string? outputRoot)
        {
            var committed = Load(directory);
            var reconciliation = Reconcile(committed, results, inventory);
            var files = Render(reconciliation.Measurements);
            var changes = Plan(directory, files);
            var errors = new List<string>(reconciliation.Errors);
            string? output = null;

            if (mode == GasBaselineMode.Update)
            {
                if (changes.Count > 0) Apply(directory, files, changes);
            }
            else
            {
                output = Path.Combine(outputRoot ?? Path.Combine(Path.GetTempPath(), "neo-gas-baselines"), assemblyName, Guid.NewGuid().ToString("N"));
                Apply(output, files, Plan(output, files));
                if (requireCompleteRun && !reconciliation.IsCompleteRun)
                    errors.Add("The GAS baselines could not be verified because the run was not complete.");
                if (changes.Count > 0)
                    errors.Add("The committed GAS baselines are out of date.");
            }

            return new GasBaselineSyncResult(reconciliation, changes.Select(c => c.Marker + " " + c.Name).ToArray(),
                Describe(committed, reconciliation.Measurements), errors, output);
        }

        private readonly record struct FileChange(char Marker, string Name);

        private static List<FileChange> Plan(string directory, IReadOnlyDictionary<string, string> files)
        {
            var changes = new List<FileChange>();
            foreach (var (name, content) in files)
            {
                var path = Path.Combine(directory, name);
                if (!File.Exists(path)) changes.Add(new FileChange('+', name));
                else if (!File.ReadAllBytes(path).AsSpan().SequenceEqual(Utf8.GetBytes(content))) changes.Add(new FileChange('~', name));
            }
            if (Directory.Exists(directory))
                foreach (var path in Directory.GetFiles(directory, "*.json").OrderBy(p => p, StringComparer.Ordinal))
                    if (!files.ContainsKey(Path.GetFileName(path))) changes.Add(new FileChange('-', Path.GetFileName(path)));
            return changes;
        }

        private static void Apply(string directory, IReadOnlyDictionary<string, string> files, List<FileChange> changes)
        {
            Directory.CreateDirectory(directory);
            foreach (var change in changes.Where(c => c.Marker != '-'))
            {
                var path = Path.Combine(directory, change.Name);
                var temporary = Path.Combine(directory, "." + change.Name + "." + Guid.NewGuid().ToString("N") + ".tmp");
                File.WriteAllBytes(temporary, Utf8.GetBytes(files[change.Name]));
                File.Move(temporary, path, true);
            }
            foreach (var change in changes.Where(c => c.Marker == '-'))
                File.Delete(Path.Combine(directory, change.Name));
        }

        /// <summary>
        /// Entry-level differences ("+" added, "-" removed, "~" changed), in ordinal order.
        /// </summary>
        public static IReadOnlyList<string> Describe(IReadOnlyDictionary<string, long> before, IReadOnlyDictionary<string, long> after)
        {
            var lines = new List<string>();
            foreach (var key in before.Keys.Union(after.Keys).OrderBy(k => k, StringComparer.Ordinal))
            {
                var hasBefore = before.TryGetValue(key, out var old);
                var hasAfter = after.TryGetValue(key, out var current);
                if (hasBefore && hasAfter) { if (old != current) lines.Add($"~ {key}: {old} -> {current}"); }
                else if (hasAfter) lines.Add($"+ {key}: {current}");
                else lines.Add($"- {key}: {old}");
            }
            return lines;
        }
    }
}
