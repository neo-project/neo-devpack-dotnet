// Copyright (C) 2015-2026 The Neo Project.
//
// GasExecutionTracker.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Neo.SmartContract.Testing.TestingStandards
{
    /// <summary>
    /// A single observed contract execution.
    /// </summary>
    public sealed record GasMeasurement(string Id, long Gas, bool Halted);

    /// <summary>
    /// Tracks the GAS of each TestEngine execution inside an AsyncLocal per-test scope.
    /// </summary>
    public static class GasExecutionTracker
    {
        private static readonly AsyncLocal<GasExecutionScope?> _current = new();
        private static readonly AsyncLocal<ExecutionModifier?> _modifier = new();

        /// <summary>
        /// Active scope of the current async flow, or null when tracking is disabled.
        /// </summary>
        public static GasExecutionScope? Current => _current.Value;

        /// <summary>
        /// Starts tracking in the current async flow.
        /// </summary>
        public static GasExecutionScope Begin(string testId)
        {
            if (string.IsNullOrWhiteSpace(testId)) throw new ArgumentException("Test id is required.", nameof(testId));
            var scope = new GasExecutionScope(testId);
            _current.Value = scope;
            return scope;
        }

        /// <summary>
        /// Deterministic test identifier.
        /// </summary>
        public static string CreateTestId(string className, string methodName, string? caseIdentity)
        {
            var id = className + "." + methodName;
            return string.IsNullOrEmpty(caseIdentity) ? id : id + "[" + caseIdentity + "]";
        }

        /// <summary>
        /// Excludes executions made inside the returned scope (e.g. nondeterministic ones).
        /// </summary>
        public static IDisposable Exclude() => Push(new ExecutionModifier(true, null));

        /// <summary>
        /// Uses an explicit label instead of the invocation index for executions made inside the returned scope.
        /// </summary>
        public static IDisposable Label(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Label is required.", nameof(label));
            return Push(new ExecutionModifier(false, label));
        }

        private static IDisposable Push(ExecutionModifier modifier)
        {
            var previous = _modifier.Value;
            _modifier.Value = modifier;
            return new Restore(previous);
        }

        internal static void Record(long gas, bool halted)
        {
            var scope = _current.Value;
            if (scope is null) return;
            var modifier = _modifier.Value;
            if (modifier?.Excluded == true) return;
            scope.RecordExecution(gas, halted, modifier?.Label);
        }

        internal static void End(GasExecutionScope scope)
        {
            if (ReferenceEquals(_current.Value, scope)) _current.Value = null;
        }

        private sealed record ExecutionModifier(bool Excluded, string? Label);

        private sealed class Restore : IDisposable
        {
            private readonly ExecutionModifier? _previous;
            public Restore(ExecutionModifier? previous) => _previous = previous;
            public void Dispose() => _modifier.Value = _previous;
        }
    }

    /// <summary>
    /// Measurements of one test.
    /// </summary>
    public sealed class GasExecutionScope : IDisposable
    {
        private readonly object _lock = new();
        private readonly List<GasMeasurement> _measurements = new();
        private int _index;

        public string TestId { get; }

        internal GasExecutionScope(string testId) => TestId = testId;

        /// <summary>
        /// Snapshot of the measurements in recording order.
        /// </summary>
        public IReadOnlyList<GasMeasurement> Measurements
        {
            get { lock (_lock) return _measurements.ToArray(); }
        }

        internal void RecordExecution(long gas, bool halted, string? label)
        {
            lock (_lock)
            {
                _index++;
                Add(label is null ? TestId + "#" + _index : TestId + ":" + label, gas, halted);
            }
        }

        public void Add(string id, long gas, bool halted)
        {
            lock (_lock) _measurements.Add(new GasMeasurement(id, gas, halted));
        }

        public GasSnapshot ToSnapshot()
        {
            var snapshot = new GasSnapshot();
            foreach (var m in Measurements) snapshot.Record(m.Id, m.Gas);
            return snapshot;
        }

        /// <summary>
        /// Compares against the expected baseline. When the test did not complete, missing executions are not reported.
        /// </summary>
        public GasRegressionReport Evaluate(GasSnapshot baseline, bool testCompleted, long tolerance = 0)
        {
            var current = ToSnapshot();
            if (testCompleted) return GasRegressionReport.Evaluate(baseline, current, tolerance);

            var seen = new GasSnapshot();
            foreach (var kv in baseline.Measurements)
                if (current.Measurements.ContainsKey(kv.Key)) seen.Record(kv.Key, kv.Value);
            return GasRegressionReport.Evaluate(seen, current, tolerance);
        }

        public void Dispose() => GasExecutionTracker.End(this);
    }
}
