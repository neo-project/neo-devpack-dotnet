// Copyright (C) 2015-2026 The Neo Project.
//
// GasExecutionTrackerTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.GasRegression;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    [TestClass]
    public class GasExecutionTrackerTests
    {
        private static Script Push(int count)
        {
            var sb = new ScriptBuilder();
            for (var i = 0; i < count; i++) sb.Emit(OpCode.NOP);
            sb.Emit(OpCode.RET);
            return sb.ToArray();
        }

        private static Script Fault()
        {
            var sb = new ScriptBuilder();
            sb.Emit(OpCode.NOP);
            sb.Emit(OpCode.THROW);
            return sb.ToArray();
        }

        [TestMethod]
        public void NestedScopeRestoresOuterScopeOnDispose()
        {
            using var outer = GasExecutionTracker.Begin("Outer.Test");
            var engine = new TestEngine(true);
            engine.Execute(Push(1));

            using (var inner = GasExecutionTracker.Begin("Inner.Test"))
            {
                Assert.AreSame(inner, GasExecutionTracker.Current);
                engine.Execute(Push(2));
                Assert.AreEqual(1, inner.Measurements.Count);
            }

            Assert.AreSame(outer, GasExecutionTracker.Current);
            engine.Execute(Push(3));
            CollectionAssert.AreEqual(new[] { "Outer.Test#1", "Outer.Test#2" }, outer.Measurements.Select(m => m.Id).ToArray());
        }

        [TestMethod]
        public void NoTrackingOutsideScope()
        {
            Assert.IsNull(GasExecutionTracker.Current);
            new TestEngine(true).Execute(Push(1));
            Assert.IsNull(GasExecutionTracker.Current);
        }

        [TestMethod]
        public void RecordsMultipleExecutionsWithDeterministicIds()
        {
            using var scope = GasExecutionTracker.Begin("A.B");
            var engine = new TestEngine(true);
            engine.Execute(Push(1));
            engine.Execute(Push(1));
            engine.Execute(Push(5));

            var ids = scope.Measurements.Select(m => m.Id).ToArray();
            CollectionAssert.AreEqual(new[] { "A.B#1", "A.B#2", "A.B#3" }, ids);
            Assert.AreEqual(scope.Measurements[0].Gas, scope.Measurements[1].Gas);
            Assert.IsTrue(scope.Measurements[2].Gas > scope.Measurements[1].Gas);
        }

        [TestMethod]
        public void RecordsPerExecutionGasNotAccumulated()
        {
            using var scope = GasExecutionTracker.Begin("A.C");
            var engine = new TestEngine(true) { ResetFeeConsumed = false };
            engine.Execute(Push(3));
            engine.Execute(Push(3));
            Assert.AreEqual(scope.Measurements[0].Gas, scope.Measurements[1].Gas);
        }

        [TestMethod]
        public void RecordsFaultExecutions()
        {
            using var scope = GasExecutionTracker.Begin("A.F");
            Assert.ThrowsExactly<TestException>(() => new TestEngine(true).Execute(Fault()));
            Assert.HasCount(1, scope.Measurements);
            Assert.IsFalse(scope.Measurements[0].Halted);
            Assert.IsTrue(scope.Measurements[0].Gas > 0);
        }

        [TestMethod]
        public void MultipleEnginesShareSequence()
        {
            using var scope = GasExecutionTracker.Begin("A.M");
            new TestEngine(true).Execute(Push(1));
            new TestEngine(true).Execute(Push(1));
            Assert.AreEqual("A.M#2", scope.Measurements[1].Id);
        }

        [TestMethod]
        public void ExcludedExecutionsAreNotRecorded()
        {
            using var scope = GasExecutionTracker.Begin("A.E");
            var engine = new TestEngine(true);
            using (GasExecutionTracker.Exclude()) engine.Execute(Push(1));
            engine.Execute(Push(1));
            Assert.HasCount(1, scope.Measurements);
            Assert.AreEqual("A.E#1", scope.Measurements[0].Id);
        }

        [TestMethod]
        public void LabelOverridesIndex()
        {
            using var scope = GasExecutionTracker.Begin("A.L");
            using (GasExecutionTracker.Label("deploy")) new TestEngine(true).Execute(Push(1));
            Assert.AreEqual("A.L:deploy", scope.Measurements[0].Id);
        }

        [TestMethod]
        public async Task ParallelScopesAreIsolated()
        {
            var tasks = Enumerable.Range(1, 8).Select(i => Task.Run(() =>
            {
                using var scope = GasExecutionTracker.Begin("P." + i);
                var engine = new TestEngine(true);
                for (var n = 0; n < i; n++) engine.Execute(Push(1));
                return scope.Measurements.Count;
            })).ToArray();
            var counts = await Task.WhenAll(tasks);
            CollectionAssert.AreEqual(Enumerable.Range(1, 8).ToArray(), counts);
        }

        [TestMethod]
        public void CompleteDetectsRegressionImprovementMissingUnexpected()
        {
            var baseline = new GasSnapshot();
            baseline.Record("T#1", 100);
            baseline.Record("T#2", 100);
            baseline.Record("T#3", 100);
            baseline.Record("T#4", 100);

            using var scope = GasExecutionTracker.Begin("T");
            scope.Add("T#1", 150, true);
            scope.Add("T#2", 50, true);
            scope.Add("T#3", 100, true);
            scope.Add("T#5", 10, true);

            var report = scope.Evaluate(baseline, testCompleted: true);
            var status = report.Changes.ToDictionary(c => c.Name, c => c.Status);
            Assert.AreEqual(GasChangeStatus.Regression, status["T#1"]);
            Assert.AreEqual(GasChangeStatus.Improvement, status["T#2"]);
            Assert.AreEqual(GasChangeStatus.Unchanged, status["T#3"]);
            Assert.AreEqual(GasChangeStatus.Missing, status["T#4"]);
            Assert.AreEqual(GasChangeStatus.Unexpected, status["T#5"]);
        }

        [TestMethod]
        public void IncompleteTestDoesNotReportMissing()
        {
            var baseline = new GasSnapshot();
            baseline.Record("T#1", 100);
            baseline.Record("T#2", 100);
            using var scope = GasExecutionTracker.Begin("T");
            scope.Add("T#1", 100, true);
            var report = scope.Evaluate(baseline, testCompleted: false);
            Assert.IsFalse(report.Changes.Any(c => c.Status == GasChangeStatus.Missing));
        }

        [TestMethod]
        public void ParameterizedIdentityIsStable()
        {
            Assert.AreEqual("N.C.M[1, a]", GasExecutionTracker.CreateTestId("N.C", "M", "1, a"));
            Assert.AreEqual("N.C.M", GasExecutionTracker.CreateTestId("N.C", "M", null));
        }
    }
}
