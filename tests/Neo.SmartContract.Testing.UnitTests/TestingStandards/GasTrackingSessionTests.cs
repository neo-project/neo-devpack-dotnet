// Copyright (C) 2015-2026 The Neo Project.
//
// GasTrackingSessionTests.cs file belongs to the neo project and is free
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
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    [TestClass]
    [DoNotParallelize]
    public class GasTrackingSessionTests
    {
        private string? _ci, _actions, _generate;

        [TestInitialize]
        public void Save()
        {
            _ci = Environment.GetEnvironmentVariable("CI");
            _actions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
            _generate = Environment.GetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable);
        }

        [TestCleanup]
        public void Restore()
        {
            Environment.SetEnvironmentVariable("CI", _ci);
            Environment.SetEnvironmentVariable("GITHUB_ACTIONS", _actions);
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, _generate);
        }

        [TestMethod]
        public void NoTrackingWithoutBaselineOrGeneration()
        {
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, null);
            Assert.IsNull(GasTrackingSession.Begin(typeof(GasTrackingSessionTests).Assembly, "X.Y"));
            Assert.IsNull(GasExecutionTracker.Current);
        }

        [TestMethod]
        public void GenerationIsRefusedInCi()
        {
            Environment.SetEnvironmentVariable("CI", "true");
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, Path.GetTempPath());
            Assert.ThrowsExactly<InvalidOperationException>(() => GasTrackingSession.Begin(typeof(GasTrackingSessionTests).Assembly, "X.Y"));
        }

        [TestMethod]
        public void GenerationWritesOneFilePerTestAndNeverTouchesBaselines()
        {
            Environment.SetEnvironmentVariable("CI", null);
            Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);
            var dir = Path.Combine(Path.GetTempPath(), "gas-gen-" + Path.GetRandomFileName());
            Environment.SetEnvironmentVariable(GasTrackingSession.GenerateDirectoryVariable, dir);
            try
            {
                var assembly = typeof(GasTrackingSessionTests).Assembly;
                var scope = GasTrackingSession.Begin(assembly, "G.T[1]");
                Assert.IsNotNull(scope);
                scope.Add("G.T[1]#1", 10, true);
                GasTrackingSession.Complete(assembly, scope, true);

                var files = Directory.GetFiles(Path.Combine(dir, assembly.GetName().Name!), "*.json");
                Assert.HasCount(1, files);
                StringAssert.Contains(File.ReadAllText(files[0]), "\"G.T[1]#1\": 10");
                Assert.IsNull(GasExecutionTracker.Current);
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
        }
    }
}
