// Copyright (C) 2015-2026 The Neo Project.
//
// TestBaseGasTrackingOptInTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.TestingStandards;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards;

[TestClass]
public class TestBaseGasTrackingOptInTests
{
    private abstract class Dummy : SmartContract, IContractInfo
    {
        protected Dummy(SmartContractInitialize initialize) : base(initialize) { }
        public static Neo.SmartContract.Manifest.ContractManifest Manifest => null!;
        public static Neo.SmartContract.NefFile Nef => null!;
    }

    [TestMethod]
    public void TestBaseHasNoGasTrackingLifecycle()
    {
        var methods = typeof(TestBase<Dummy>).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsFalse(methods.Any(m => m.GetCustomAttribute<TestInitializeAttribute>() is not null));
        Assert.IsNull(typeof(TestBase<Dummy>).GetField("_gasScope", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [TestMethod]
    public void TrackingBaseOptsInWithSingleCleanupPath()
    {
        var type = typeof(TestBaseWithGasTracking<Dummy>);
        Assert.IsTrue(type.BaseType == typeof(TestBase<Dummy>));

        var init = type.GetMethod("OnGasTrackingInitialize")!;
        Assert.IsNotNull(init.GetCustomAttribute<TestInitializeAttribute>());

        var cleanups = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<TestCleanupAttribute>() is not null)
            .ToArray();
        Assert.HasCount(1, cleanups, "The override is the single cleanup path (MSTest skips overridden base cleanups).");
        Assert.AreEqual("OnCleanup", cleanups[0].Name);
        Assert.IsTrue(cleanups[0].GetBaseDefinition().DeclaringType == typeof(TestBase<Dummy>));
    }

    [TestMethod]
    public void NoAutomaticScopeWithoutOptIn()
    {
        Assert.IsNull(GasExecutionTracker.Current);
    }
}
