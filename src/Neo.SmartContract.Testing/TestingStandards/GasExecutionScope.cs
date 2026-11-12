// Copyright (C) 2015-2026 The Neo Project.
//
// GasExecutionScope.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using Neo.SmartContract.Testing.GasRegression;

namespace Neo.SmartContract.Testing.TestingStandards
{
    /// <summary>
    /// Measurements of one test.
    /// </summary>
    public sealed class GasExecutionScope : IDisposable
    {
        private readonly object _lock = new();
        private readonly List<GasMeasurement> _measurements = new();
        private int _index;

        public string TestId { get; }

        internal GasExecutionScope? Parent { get; }

        internal GasExecutionScope(string testId, GasExecutionScope? parent = null)
        {
            TestId = testId;
            Parent = parent;
        }

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
