// Copyright (C) 2015-2026 The Neo Project.
//
// GasCandidateTrackingTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.GasRegression;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    /// <summary>
    /// Candidate generation (NEO_GAS_BASELINE_CANDIDATE_DIR) is allowed in CI, never writes baselines,
    /// ignores failed tests and is safe under parallel execution. The Neo.SmartContract.Testing assembly is used
    /// as the "tracked" assembly because it is not a test assembly and has no other tracking users.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class GasCandidateTrackingTests
    {
        private static readonly Assembly Tracked = typeof(TestEngine).Assembly;
        private string? _ci, _actions, _generate, _candidate, _report;
        private string _dir = string.Empty;

        [ClassInitialize]
        public static void CreateBaseline(TestContext _)
        {
            var directory = Path.Combine(AppContext.BaseDirectory, "GasBaselines", Tracked.GetName().Name!);
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "candidate-tests.json"), "{\n  \"Cand.Existing[1]#1\": 100\n}");
        }

        [ClassCleanup]
        public static void RemoveBaseline()
        {
            File.Delete(Path.Combine(AppContext.BaseDirectory, "GasBaselines", Tracked.GetName().Name!, "candidate-tests.json"));
        }

        [TestInitialize]
        public void Save()
        {
            _ci = Environment.GetEnvironmentVariable("CI");
            _actions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
            _generate = Environment.GetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable);
            _candidate = Environment.GetEnvironmentVariable(GasTrackingSession.CandidateDirectoryVariable);
            _report = Environment.GetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable);
            _dir = Path.Combine(Path.GetTempPath(), "gas-cand-" + Path.GetRandomFileName());
            Environment.SetEnvironmentVariable("CI", "true");
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, null);
            Environment.SetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable, null);
            Environment.SetEnvironmentVariable(GasTrackingSession.CandidateDirectoryVariable, _dir);
        }

        [TestCleanup]
        public void Restore()
        {
            Environment.SetEnvironmentVariable("CI", _ci);
            Environment.SetEnvironmentVariable("GITHUB_ACTIONS", _actions);
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, _generate);
            Environment.SetEnvironmentVariable(GasTrackingSession.CandidateDirectoryVariable, _candidate);
            Environment.SetEnvironmentVariable(GasRegressionReport.ReportDirectoryVariable, _report);
            if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
        }

        private string AssemblyDirectory => Path.Combine(_dir, Tracked.GetName().Name!);

        private static GasExecutionScope Begin(string testId, long gas)
        {
            var scope = GasTrackingSession.Begin(Tracked, testId);
            Assert.IsNotNull(scope);
            scope.Add(testId + "#1", gas, true);
            return scope;
        }

        [TestMethod]
        public void CandidatesAreWrittenInCiForPassingTests()
        {
            var report = GasTrackingSession.Complete(Tracked, Begin("Cand.Existing[1]", 90), true);

            var files = Directory.GetFiles(AssemblyDirectory, "*.json");
            Assert.HasCount(1, files);
            Assert.AreEqual("{\n  \"Cand.Existing[1]#1\": 90\n}", File.ReadAllText(files[0]));
            Assert.IsNotNull(report);
            Assert.IsFalse(report.HasFailures);
            Assert.AreEqual(GasChangeStatus.Improvement, report.Changes.Single().Status);
        }

        [TestMethod]
        public void GenerationStaysRefusedInCi()
        {
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, _dir);
            Assert.ThrowsExactly<InvalidOperationException>(() => _ = GasTrackingSession.GenerateDirectory);
        }

        [TestMethod]
        public void FailedTestsLeaveAMarkerAndNoMeasurements()
        {
            GasTrackingSession.Complete(Tracked, Begin("Cand.Existing[1]", 90), false);

            Assert.IsEmpty(Directory.GetFiles(AssemblyDirectory, "*.json"));
            Assert.HasCount(1, Directory.GetFiles(AssemblyDirectory, "*.failed"));
        }

        [TestMethod]
        public void DifferentValuesForTheSameTestAreMarkedUnstable()
        {
            GasTrackingSession.Complete(Tracked, Begin("Cand.Existing[1]", 90), true);
            GasTrackingSession.Complete(Tracked, Begin("Cand.Existing[1]", 95), true);

            Assert.HasCount(1, Directory.GetFiles(AssemblyDirectory, "*.unstable"));
        }

        [TestMethod]
        public void IdenticalRepeatedValuesAreIdempotent()
        {
            GasTrackingSession.Complete(Tracked, Begin("Cand.Existing[1]", 90), true);
            GasTrackingSession.Complete(Tracked, Begin("Cand.Existing[1]", 90), true);

            Assert.IsEmpty(Directory.GetFiles(AssemblyDirectory, "*.unstable"));
            Assert.HasCount(1, Directory.GetFiles(AssemblyDirectory, "*.json"));
        }

        [TestMethod]
        public void CandidateDirectoryInsideABaselineFolderIsRejected()
        {
            Environment.SetEnvironmentVariable(GasTrackingSession.CandidateDirectoryVariable, Path.Combine(_dir, "GasBaselines", "x"));
            var scope = Begin("Cand.Existing[1]", 90);
            Assert.ThrowsExactly<InvalidOperationException>(() => GasTrackingSession.Complete(Tracked, scope, true));
        }

        [TestMethod]
        public void NewTestsDoNotFailInCandidateModeButExistingOnesStayStrict()
        {
            var newTest = GasTrackingSession.Complete(Tracked, Begin("Cand.Brand[1]", 50), true);
            Assert.IsNull(newTest);
            Assert.HasCount(1, Directory.GetFiles(AssemblyDirectory, "*.json"));

            var extra = GasTrackingSession.Begin(Tracked, "Cand.Existing[1]")!;
            extra.Add("Cand.Existing[1]#1", 100, true);
            extra.Add("Cand.Existing[1]#2", 5, true);
            var report = GasTrackingSession.Complete(Tracked, extra, true);
            Assert.IsNotNull(report);
            Assert.IsTrue(report.HasFailures);
        }

        [TestMethod]
        public void NewTestsStillFailWithoutCandidateMode()
        {
            Environment.SetEnvironmentVariable(GasTrackingSession.CandidateDirectoryVariable, null);
            var report = GasTrackingSession.Complete(Tracked, Begin("Cand.Brand[1]", 50), true);
            Assert.IsNotNull(report);
            Assert.IsTrue(report.HasFailures);
        }

        [TestMethod]
        public void ParallelTestsDoNotLoseOrCorruptCandidates()
        {
            Parallel.For(0, 64, i => GasTrackingSession.Complete(Tracked, Begin($"Cand.Parallel[{i}]", i), true));

            var files = Directory.GetFiles(AssemblyDirectory, "*.json");
            Assert.HasCount(64, files);
            Assert.IsEmpty(Directory.GetFiles(AssemblyDirectory, "*.tmp"));
            foreach (var file in files) GasSnapshot.Load(file);
        }

        [TestMethod]
        public void ConcurrentDifferentValuesForSameIdAreRejected()
        {
            Parallel.For(0, 32, i => GasTrackingSession.Complete(Tracked, Begin("Cand.Same[1]", i), true));
            Assert.HasCount(1, Directory.GetFiles(AssemblyDirectory, "*.unstable"));
            Assert.HasCount(1, Directory.GetFiles(AssemblyDirectory, "*.json"));
        }

        [TestMethod]
        public void CandidateModeInitializesAssemblyWithoutBaseline()
        {
            var assembly = typeof(string).Assembly;
            var scope = GasTrackingSession.Begin(assembly, "Initial.Test");
            Assert.IsNotNull(scope);
            scope.Add("Initial.Test#1", 10, true);
            GasTrackingSession.Complete(assembly, scope, true);
            Assert.HasCount(1, Directory.GetFiles(Path.Combine(_dir, assembly.GetName().Name!), "*.json"));
        }
    }
}
