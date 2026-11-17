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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    [TestClass]
    [DoNotParallelize]
    public class GasTrackingSessionTests
    {
        [TestMethod]
        public void ExternalConsumersDoNotTrackNorWriteWithoutOptIn()
        {
            Assert.IsNull(Environment.GetEnvironmentVariable(GasTrackingSession.ModeVariable));
            var assembly = typeof(GasTrackingSessionTests).Assembly;
            Assert.IsNull(GasTrackingSession.Begin(assembly, "X.Y"));
            Assert.IsNull(GasExecutionTracker.Current);
            GasTrackingSession.Finish(assembly);
        }

        [TestMethod]
        public void InventoryOnlyListsTestBaseClasses()
        {
            var inventory = GasTrackingSession.BuildInventory(typeof(GasTrackingSessionTests).Assembly);
            Assert.IsFalse(inventory.ExpectedMethods.ContainsKey(typeof(GasTrackingSessionTests).FullName + "." + nameof(ExternalConsumersDoNotTrackNorWriteWithoutOptIn)));
        }

        [TestMethod]
        public void IgnoredTestsRemainInInventoryAndKeepTheirBaselines()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("GasInventoryFixture"), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("Main");
            var testMethod = typeof(GasTrackingSessionTests).GetMethod(nameof(IgnoredTestsRemainInInventoryAndKeepTheirBaselines))!
                .CustomAttributes.Single(a => a.AttributeType == typeof(TestMethodAttribute));
            foreach (var classIgnored in new[] { false, true })
            {
                var type = module.DefineType(classIgnored ? "IgnoredClass" : "TrackedClass", TypeAttributes.Public,
                    typeof(TestBaseWithGasTracking<InventoryContract>));
                type.SetCustomAttribute(new CustomAttributeBuilder(typeof(TestClassAttribute).GetConstructor(Type.EmptyTypes)!, []));
                if (classIgnored)
                    type.SetCustomAttribute(new CustomAttributeBuilder(typeof(IgnoreAttribute).GetConstructor(Type.EmptyTypes)!, []));
                foreach (var name in new[] { "Active", "Ignored" })
                {
                    var method = type.DefineMethod(name, MethodAttributes.Public, typeof(void), Type.EmptyTypes);
                    method.SetCustomAttribute(new CustomAttributeBuilder(testMethod.Constructor, testMethod.ConstructorArguments.Select(a => a.Value).ToArray()));
                    if (name == "Ignored")
                        method.SetCustomAttribute(new CustomAttributeBuilder(typeof(IgnoreAttribute).GetConstructor(Type.EmptyTypes)!, []));
                    method.GetILGenerator().Emit(OpCodes.Ret);
                }
                type.CreateType();
            }
            var inventory = GasTrackingSession.BuildInventory(assembly);
            Assert.HasCount(1, inventory.ExpectedMethods);
            Assert.HasCount(3, inventory.IgnoredMethods);
            var baseline = new Dictionary<string, long>
            {
                ["TrackedClass.Active#1"] = 1,
                ["TrackedClass.Ignored#1"] = 2,
                ["IgnoredClass.Active#1"] = 3,
                ["IgnoredClass.Ignored#1"] = 4,
                ["RemovedClass.Removed#1"] = 5
            };
            var reconciled = GasBaselineStore.Reconcile(baseline,
                [new GasTestResult("TrackedClass.Active", GasTestStatus.Passed, new Dictionary<string, long> { ["TrackedClass.Active#1"] = 1 })], inventory);
            Assert.IsTrue(reconciled.IsCompleteRun);
            Assert.HasCount(4, reconciled.Measurements);
            Assert.AreEqual(2L, reconciled.Measurements["TrackedClass.Ignored#1"]);
            Assert.AreEqual(3L, reconciled.Measurements["IgnoredClass.Active#1"]);
            Assert.AreEqual(4L, reconciled.Measurements["IgnoredClass.Ignored#1"]);
        }

        public abstract class InventoryContract(SmartContractInitialize initialize) : SmartContract(initialize), IContractInfo
        {
            public static Neo.SmartContract.Manifest.ContractManifest Manifest => null!;
            public static Neo.SmartContract.NefFile Nef => null!;
        }

        [TestMethod]
        public void VerifyDetectsStaleBaselinesWhenNoTrackedTestProducedResults()
        {
            var dir = Path.Combine(Path.GetTempPath(), "gas-verify-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "Stale.Class.json"), "{\n  \"Stale.Class.Method#1\": 1\n}");
            var previousMode = Environment.GetEnvironmentVariable(GasTrackingSession.ModeVariable);
            var previousDir = Environment.GetEnvironmentVariable(GasTrackingSession.DirectoryVariable);
            Environment.SetEnvironmentVariable(GasTrackingSession.ModeVariable, "verify");
            Environment.SetEnvironmentVariable(GasTrackingSession.DirectoryVariable, dir);
            GasTrackingSession.Reset();
            try
            {
                Assert.ThrowsExactly<AssertFailedException>(() => GasTrackingSession.Finish(typeof(GasTrackingSessionTests).Assembly));
            }
            finally
            {
                Environment.SetEnvironmentVariable(GasTrackingSession.ModeVariable, previousMode);
                Environment.SetEnvironmentVariable(GasTrackingSession.DirectoryVariable, previousDir);
                GasTrackingSession.Reset();
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void ModeWithoutDirectoryIsRejected()
        {
            var previous = Environment.GetEnvironmentVariable(GasTrackingSession.ModeVariable);
            Environment.SetEnvironmentVariable(GasTrackingSession.ModeVariable, "update");
            GasTrackingSession.Reset();
            try
            {
                Assert.ThrowsExactly<InvalidOperationException>(() => GasTrackingSession.Begin(typeof(GasTrackingSessionTests).Assembly, "X.Y"));
            }
            finally
            {
                Environment.SetEnvironmentVariable(GasTrackingSession.ModeVariable, previous);
                GasTrackingSession.Reset();
            }
        }
    }
}
