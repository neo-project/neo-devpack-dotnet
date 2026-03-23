// Copyright (C) 2015-2026 The Neo Project.
//
// TestTreasury.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestTreasury
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
        public void TestTreasuryContractExists()
        {
            Assert.IsNotNull(_engine.Native.Treasury, "Treasury contract should be available");
            Assert.AreNotEqual(UInt160.Zero, _engine.Native.Treasury.Hash, "Treasury hash should not be zero");
        }

        [TestMethod]
        public void TestTreasuryContractHashAndId()
        {
            Assert.AreEqual(_engine.Native.Treasury.Hash, Neo.SmartContract.Native.NativeContract.Treasury.Hash);
        }

        [TestMethod]
        public void TestTreasuryInNativeContractsList()
        {
            var nativeContracts = Neo.SmartContract.Native.NativeContract.Contracts;
            Assert.IsTrue(nativeContracts.Any(c => c.Hash == _engine.Native.Treasury.Hash),
                "Treasury should be in native contracts list");
        }

        [TestMethod]
        public void TestTreasuryManifestAndMethodSignatures()
        {
            var manifest = Neo.SmartContract.Native.NativeContract.Treasury
                .GetContractState(_engine.ProtocolSettings, uint.MaxValue).Manifest;

            Assert.IsNotNull(manifest);
            Assert.AreEqual("Treasury", manifest.Name);

            var verify = manifest.Abi.GetMethod("verify", 0);
            Assert.IsNotNull(verify);
            Assert.IsTrue(verify!.Safe);
            Assert.AreEqual(ContractParameterType.Boolean, verify.ReturnType);

            var onNep17Payment = manifest.Abi.GetMethod("onNEP17Payment", 3);
            Assert.IsNotNull(onNep17Payment);
            Assert.IsTrue(onNep17Payment!.Safe);
            Assert.AreEqual(ContractParameterType.Void, onNep17Payment.ReturnType);

            var onNep11Payment = manifest.Abi.GetMethod("onNEP11Payment", 4);
            Assert.IsNotNull(onNep11Payment);
            Assert.IsTrue(onNep11Payment!.Safe);
            Assert.AreEqual(ContractParameterType.Void, onNep11Payment.ReturnType);
        }

        [TestMethod]
        public void TestTreasuryVerifyDependsOnCommitteeSignature()
        {
            _engine.SetTransactionSigners(new Network.P2P.Payloads.Signer
            {
                Account = _engine.CommitteeAddress,
                Scopes = Network.P2P.Payloads.WitnessScope.Global
            });

            Assert.IsTrue(_engine.Native.Treasury.Verify()!.Value);

            _engine.SetTransactionSigners(TestEngine.GetNewSigner());

            Assert.IsFalse(_engine.Native.Treasury.Verify()!.Value);
        }

        [TestMethod]
        public void TestTreasuryPaymentCallbacksAreCallable()
        {
            _engine.Native.Treasury.OnNEP17Payment(_engine.CommitteeAddress, BigInteger.One, null);
            _engine.Native.Treasury.OnNEP11Payment(_engine.CommitteeAddress, BigInteger.One, new byte[] { 0x01 }, null);
        }
    }
}
