// Copyright (C) 2015-2026 The Neo Project.
//
// GasBaselineStoreTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    [TestClass]
    public class GasBaselineStoreTests
    {
        private string _root = "";
        private string Baselines => Path.Combine(_root, "baselines");

        [TestInitialize]
        public void Init() => _root = Path.Combine(Path.GetTempPath(), "gas-store-" + Path.GetRandomFileName());

        [TestCleanup]
        public void Clean()
        {
            if (Directory.Exists(_root)) Directory.Delete(_root, true);
        }

        private static GasTestResult Pass(string test, params long[] gas) =>
            new(test, GasTestStatus.Passed, gas.Select((g, i) => (Key: test + "#" + (i + 1), Gas: g)).ToDictionary(e => e.Key, e => e.Gas));

        private static GasTestResult Status(string test, GasTestStatus status) =>
            new(test, status, new Dictionary<string, long>());

        private static GasRunInventory Inventory(params string[] methods) =>
            new(methods.ToDictionary(m => m, _ => 1), []);

        private GasBaselineSyncResult Run(GasBaselineMode mode, GasRunInventory inventory, params GasTestResult[] results) =>
            GasBaselineStore.Synchronize(Baselines, "Asm", results, inventory, mode, mode == GasBaselineMode.Verify, Path.Combine(_root, "out"));

        private string Read(string name) => File.ReadAllText(Path.Combine(Baselines, name), Encoding.UTF8);

        [TestMethod]
        public void RenderIsDeterministicLfWithoutBomOrFinalNewline()
        {
            var files = GasBaselineStore.Render(new Dictionary<string, long> { ["A.T.B#2"] = 2, ["A.T.B#1"] = 1, ["A.T.A[\"x:y\"]#1"] = 3 });
            Assert.AreEqual("{\n  \"A.T.A[\\\"x:y\\\"]#1\": 3,\n  \"A.T.B#1\": 1,\n  \"A.T.B#2\": 2\n}", files["A.T.json"]);
            Run(GasBaselineMode.Update, Inventory("A.T.B"), Pass("A.T.B", 1, 2));
            var bytes = File.ReadAllBytes(Path.Combine(Baselines, "A.T.json"));
            Assert.AreNotEqual(0xEF, bytes[0]);
            CollectionAssert.DoesNotContain(bytes, (byte)'\r');
        }

        [TestMethod]
        public void LargeClassesAreSplitInParts()
        {
            var data = Enumerable.Range(1, 5).ToDictionary(i => $"A.T.M#{i}", i => (long)i);
            var files = GasBaselineStore.Render(data, 2);
            CollectionAssert.AreEqual(new[] { "A.T.part01.json", "A.T.part02.json", "A.T.part03.json" }, files.Keys.ToArray());
            Assert.AreEqual(data.Count, files.Values.Sum(f => f.Split('\n').Length - 2));
        }

        [TestMethod]
        public void UpdateWritesNewTestsImprovementsAndIncreases()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50));
            StringAssert.Contains(Read("A.T.json"), "\"A.T.B#1\": 100");

            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 80), Pass("A.T.C", 70));
            var text = Read("A.T.json");
            StringAssert.Contains(text, "\"A.T.B#1\": 80");
            StringAssert.Contains(text, "\"A.T.C#1\": 70");
        }

        [TestMethod]
        public void SecondIdenticalRunDoesNotRewriteFiles()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B"), Pass("A.T.B", 100));
            var path = Path.Combine(Baselines, "A.T.json");
            File.SetLastWriteTimeUtc(path, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var second = Run(GasBaselineMode.Update, Inventory("A.T.B"), Pass("A.T.B", 100));
            Assert.IsEmpty(second.ChangedFiles);
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), File.GetLastWriteTimeUtc(path));
        }

        [TestMethod]
        public void PartialRunPreservesOtherBaselines()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50));
            var result = Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 90));

            Assert.IsFalse(result.Reconciliation.IsCompleteRun);
            var text = Read("A.T.json");
            StringAssert.Contains(text, "\"A.T.B#1\": 90");
            StringAssert.Contains(text, "\"A.T.C#1\": 50");
        }

        [TestMethod]
        public void CompleteRunRemovesDeletedTestsAndShrinkingExecutions()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100, 200), Pass("A.T.C", 50));
            var result = Run(GasBaselineMode.Update, Inventory("A.T.B"), Pass("A.T.B", 100));

            Assert.IsTrue(result.Reconciliation.IsCompleteRun);
            var text = Read("A.T.json");
            StringAssert.Contains(text, "\"A.T.B#1\": 100");
            Assert.DoesNotContain("A.T.B#2", text);
            Assert.DoesNotContain("A.T.C", text);
        }

        [TestMethod]
        public void FailedOrUnexecutedTestsNeverChangeBaselinesNorAllowPruning()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50));
            var result = Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Status("A.T.B", GasTestStatus.Failed), Pass("A.T.C", 40));

            Assert.IsFalse(result.Reconciliation.IsCompleteRun);
            var text = Read("A.T.json");
            StringAssert.Contains(text, "\"A.T.B#1\": 100");
            StringAssert.Contains(text, "\"A.T.C#1\": 40");
        }

        [TestMethod]
        public void IncompleteDataRowsKeepOtherRows()
        {
            var inventory = new GasRunInventory(new Dictionary<string, int> { ["A.T.B"] = 2 }, []);
            var rows = new[]
            {
                new GasTestResult("A.T.B[1]", GasTestStatus.Passed, new Dictionary<string, long> { ["A.T.B[1]#1"] = 1 }),
                new GasTestResult("A.T.B[2]", GasTestStatus.Passed, new Dictionary<string, long> { ["A.T.B[2]#1"] = 2 })
            };
            Run(GasBaselineMode.Update, inventory, rows);
            var result = Run(GasBaselineMode.Update, inventory, rows[0]);

            Assert.IsFalse(result.Reconciliation.IsCompleteRun);
            StringAssert.Contains(Read("A.T.json"), "\"A.T.B[2]#1\": 2");
        }

        [TestMethod]
        public void IgnoredMethodsAreNeverPruned()
        {
            var inventory = new GasRunInventory(new Dictionary<string, int> { ["A.T.B"] = 1 }, ["A.T.Dynamic"]);
            Run(GasBaselineMode.Update, inventory, Pass("A.T.B", 1), Pass("A.T.Dynamic[x]", 5));
            var result = Run(GasBaselineMode.Update, inventory, Pass("A.T.B", 1));

            Assert.IsTrue(result.Reconciliation.IsCompleteRun);
            StringAssert.Contains(Read("A.T.json"), "A.T.Dynamic[x]#1");
        }

        [TestMethod]
        public void UnstableAndInvalidMeasurementsAreErrorsAndPreserveBaselines()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50));
            var result = Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Status("A.T.B", GasTestStatus.Unstable), Status("A.T.C", GasTestStatus.Invalid));

            Assert.HasCount(2, result.Errors);
            var text = Read("A.T.json");
            StringAssert.Contains(text, "\"A.T.B#1\": 100");
            StringAssert.Contains(text, "\"A.T.C#1\": 50");
        }

        [TestMethod]
        public void VerifyPassesWhenUpToDateAndFailsOnIncreaseReductionNewAndStale()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50));
            var snapshot = Directory.GetFiles(Baselines).ToDictionary(Path.GetFileName, File.ReadAllBytes);

            Assert.IsEmpty(Run(GasBaselineMode.Verify, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50)).Errors);

            foreach (var changed in new[] { Pass("A.T.B", 101), Pass("A.T.B", 99), Pass("A.T.B", 100, 5), Pass("A.T.N", 1) })
            {
                var result = Run(GasBaselineMode.Verify, Inventory("A.T.B", "A.T.C"), changed, Pass("A.T.C", 50));
                Assert.IsNotEmpty(result.Errors, changed.TestId);
                Assert.IsNotEmpty(result.Differences);
            }

            var stale = Run(GasBaselineMode.Verify, Inventory("A.T.B"), Pass("A.T.B", 100));
            Assert.IsNotEmpty(stale.Errors);
            StringAssert.Contains(string.Join("\n", stale.Differences), "- A.T.C#1: 50");

            foreach (var kv in snapshot)
                CollectionAssert.AreEqual(kv.Value, File.ReadAllBytes(Path.Combine(Baselines, kv.Key!)));
            Assert.IsNotNull(Run(GasBaselineMode.Verify, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50)).OutputDirectory);
        }

        [TestMethod]
        public void VerifyRejectsPartialRuns()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100), Pass("A.T.C", 50));
            var result = Run(GasBaselineMode.Verify, Inventory("A.T.B", "A.T.C"), Pass("A.T.B", 100));
            Assert.IsNotEmpty(result.Errors);
        }

        [TestMethod]
        public void ConcurrentUpdatesNeverCorruptFiles()
        {
            Run(GasBaselineMode.Update, Inventory("A.T.B"), Pass("A.T.B", 1));
            Parallel.For(0, 16, _ => Run(GasBaselineMode.Update, Inventory("A.T.B"), Pass("A.T.B", 1)));
            Assert.AreEqual("{\n  \"A.T.B#1\": 1\n}", Read("A.T.json"));
            Assert.IsEmpty(Directory.GetFiles(Baselines, "*.tmp"));
        }

        [TestMethod]
        public void TestIdParsingIgnoresSeparatorsInsideBrackets()
        {
            Assert.AreEqual("A.T.M[\"a#b:c\"]", GasBaselineStore.TestIdOf("A.T.M[\"a#b:c\"]#3"));
            Assert.AreEqual("A.T.M", GasBaselineStore.TestIdOf("A.T.M:label"));
            Assert.ThrowsExactly<InvalidDataException>(() => GasBaselineStore.TestIdOf("nosuffix"));
        }
    }
}
