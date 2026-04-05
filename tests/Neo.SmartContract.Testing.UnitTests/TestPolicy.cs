// Copyright (C) 2015-2026 The Neo Project.
//
// TestPolicy.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Native;
using System.Collections.Immutable;
using System.Linq;
using FrameworkPolicy = Neo.SmartContract.Framework.Native.Policy;
using TestingPolicy = Neo.SmartContract.Testing.Native.Policy;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestPolicy
    {
        private TestEngine _engine = null!;

        [TestInitialize]
        public void Setup()
        {
            var settings = TestEngine.Default with
            {
                Hardforks = TestEngine.Default.Hardforks.ToImmutableDictionary(p => p.Key, p => 0u)
            };
            _engine = new TestEngine(settings, true);
        }

        [TestMethod]
        public void TestPolicyManifestIncludesLatestMembers()
        {
            var manifest = NativeContract.Policy.GetContractState(_engine.ProtocolSettings, uint.MaxValue).Manifest;

            Assert.IsNotNull(manifest.Abi.GetMethod("getMillisecondsPerBlock", 0));
            Assert.IsNotNull(manifest.Abi.GetMethod("getMaxValidUntilBlockIncrement", 0));
            Assert.IsNotNull(manifest.Abi.GetMethod("getMaxTraceableBlocks", 0));
            Assert.IsNotNull(manifest.Abi.GetMethod("setMillisecondsPerBlock", 1));
            Assert.IsNotNull(manifest.Abi.GetMethod("setMaxValidUntilBlockIncrement", 1));
            Assert.IsNotNull(manifest.Abi.GetMethod("setMaxTraceableBlocks", 1));
            Assert.IsNotNull(manifest.Abi.GetMethod("recoverFund", 2));
            Assert.IsNotNull(manifest.Abi.GetMethod("setWhitelistFeeContract", 4));
            Assert.IsNotNull(manifest.Abi.GetMethod("removeWhitelistFeeContract", 3));

            CollectionAssert.AreEquivalent(
                new[] { "MillisecondsPerBlockChanged", "WhitelistFeeChanged", "RecoveredFund" },
                manifest.Abi.Events.Select(static e => e.Name).ToArray());
        }

        [TestMethod]
        public void TestPolicyWrappersExposeLatestMembers()
        {
            Assert.IsNotNull(typeof(TestingPolicy).GetProperty(nameof(TestingPolicy.MillisecondsPerBlock)));
            Assert.IsNotNull(typeof(TestingPolicy).GetProperty(nameof(TestingPolicy.MaxValidUntilBlockIncrement)));
            Assert.IsNotNull(typeof(TestingPolicy).GetProperty(nameof(TestingPolicy.MaxTraceableBlocks)));
            Assert.IsNotNull(typeof(TestingPolicy).GetEvent(nameof(TestingPolicy.OnMillisecondsPerBlockChanged)));
            Assert.IsNotNull(typeof(TestingPolicy).GetEvent(nameof(TestingPolicy.OnWhitelistFeeChanged)));
            Assert.IsNotNull(typeof(TestingPolicy).GetEvent(nameof(TestingPolicy.OnRecoveredFund)));

            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.GetMillisecondsPerBlock)));
            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.GetMaxValidUntilBlockIncrement)));
            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.GetMaxTraceableBlocks)));
            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.BlockAccount)));
            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.RecoverFund)));
            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.SetWhitelistFeeContract)));
            Assert.IsNotNull(typeof(FrameworkPolicy).GetMethod(nameof(FrameworkPolicy.RemoveWhitelistFeeContract)));
        }

        [TestMethod]
        public void TestPolicySettersUpdateStateAndRaiseMillisecondsPerBlockChanged()
        {
            _engine.SetTransactionSigners(new Signer
            {
                Account = _engine.CommitteeAddress,
                Scopes = WitnessScope.Global
            });

            uint originalMillisecondsPerBlock = _engine.Native.Policy.MillisecondsPerBlock;
            uint updatedMillisecondsPerBlock = originalMillisecondsPerBlock + 1;
            uint originalMaxValidUntilBlockIncrement = _engine.Native.Policy.MaxValidUntilBlockIncrement;
            uint updatedMaxValidUntilBlockIncrement = originalMaxValidUntilBlockIncrement > 1 ? originalMaxValidUntilBlockIncrement - 1 : originalMaxValidUntilBlockIncrement + 1;
            uint originalMaxTraceableBlocks = _engine.Native.Policy.MaxTraceableBlocks;
            uint updatedMaxTraceableBlocks = originalMaxTraceableBlocks > 1 ? originalMaxTraceableBlocks - 1 : originalMaxTraceableBlocks + 1;

            bool eventRaised = false;
            uint observedOldMillisecondsPerBlock = 0;
            uint observedNewMillisecondsPerBlock = 0;

            _engine.Native.Policy.OnMillisecondsPerBlockChanged += (oldValue, newValue) =>
            {
                eventRaised = true;
                observedOldMillisecondsPerBlock = oldValue;
                observedNewMillisecondsPerBlock = newValue;
            };

            _engine.Native.Policy.MillisecondsPerBlock = updatedMillisecondsPerBlock;
            _engine.Native.Policy.MaxValidUntilBlockIncrement = updatedMaxValidUntilBlockIncrement;
            _engine.Native.Policy.MaxTraceableBlocks = updatedMaxTraceableBlocks;

            Assert.IsTrue(eventRaised);
            Assert.AreEqual(originalMillisecondsPerBlock, observedOldMillisecondsPerBlock);
            Assert.AreEqual(updatedMillisecondsPerBlock, observedNewMillisecondsPerBlock);
            Assert.AreEqual(updatedMillisecondsPerBlock, _engine.Native.Policy.MillisecondsPerBlock);
            Assert.AreEqual(updatedMaxValidUntilBlockIncrement, _engine.Native.Policy.MaxValidUntilBlockIncrement);
            Assert.AreEqual(updatedMaxTraceableBlocks, _engine.Native.Policy.MaxTraceableBlocks);
        }
    }
}
