// Copyright (C) 2015-2024 The Neo Project.
//
// TestNotary.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Testing.UnitTests
{
    /// <summary>
    /// Unit tests for the Notary native contract.
    /// Validates core functionality including contract availability, method signatures,
    /// hardfork activation, and integration with other native contracts.
    /// </summary>
    [TestClass]
    public class TestNotary
    {
        private TestEngine _engine = null!;

        /// <summary>
        /// Initializes the test engine with all hardforks enabled to activate the Notary contract.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            var settings = TestEngine.Default with
            {
                Hardforks = TestEngine.Default.Hardforks.ToImmutableDictionary(p => p.Key, p => 0u)
            };
            _engine = new TestEngine(settings, true);
        }

        /// <summary>
        /// Tests that the Notary contract is available and has a valid hash.
        /// </summary>
        [TestMethod]
        public void TestNotaryContractExists()
        {
            // Verify the Notary contract is accessible through the testing engine
            Assert.IsNotNull(_engine.Native.Notary, "Notary contract should be available");
            Assert.AreNotEqual(UInt160.Zero, _engine.Native.Notary.Hash, "Notary hash should not be zero");
        }

        /// <summary>
        /// Tests that the Notary contract hash matches between testing and native implementations.
        /// </summary>
        [TestMethod]
        public void TestNotaryContractHash()
        {
            var testingHash = _engine.Native.Notary.Hash;
            var nativeHash = Neo.SmartContract.Native.NativeContract.Notary.Hash;

            Assert.AreEqual(testingHash, nativeHash, "Hashes should match between testing and native contracts");
        }

        /// <summary>
        /// Tests that the Notary contract is properly registered in the native contracts list.
        /// </summary>
        [TestMethod]
        public void TestNotaryInNativeContractsList()
        {
            var nativeContracts = Neo.SmartContract.Native.NativeContract.Contracts;
            var notaryHash = _engine.Native.Notary.Hash;

            Assert.IsTrue(nativeContracts.Any(c => c.Hash == notaryHash),
                "Notary should be in native contracts list");
        }

        /// <summary>
        /// Tests the Notary contract manifest and verifies all required methods are present.
        /// </summary>
        [TestMethod]
        public void TestNotaryManifest()
        {
            var manifest = Neo.SmartContract.Native.NativeContract.Notary
                .GetContractState(_engine.ProtocolSettings, uint.MaxValue).Manifest;

            // Verify basic manifest properties
            Assert.IsNotNull(manifest, "Manifest should not be null");
            Assert.AreEqual("Notary", manifest.Name, "Contract name should be 'Notary'");

            // Verify all required methods are present in the manifest
            var methods = manifest.Abi.Methods;
            var requiredMethods = new[] { "balanceOf", "expirationOf", "lockDepositUntil", "getMaxNotValidBeforeDelta" };

            foreach (var methodName in requiredMethods)
            {
                Assert.IsTrue(methods.Any(m => m.Name == methodName),
                    $"Method '{methodName}' should be present in the manifest");
            }
        }

        /// <summary>
        /// Tests that the Notary contract is properly activated with the Echidna hardfork.
        /// </summary>
        [TestMethod]
        public void TestNotaryHardforkActivation()
        {
            // Verify the Notary contract is activated in the correct hardfork
            var activeIn = Neo.SmartContract.Native.NativeContract.Notary.ActiveIn;
            Assert.AreEqual(Hardfork.HF_Echidna, activeIn, "Notary should be active in Echidna hardfork");

            // Verify the contract is currently active based on protocol settings
            var isActive = Neo.SmartContract.Native.NativeContract.Notary.ActiveIn.HasValue &&
                          _engine.ProtocolSettings.Hardforks.ContainsKey(Neo.SmartContract.Native.NativeContract.Notary.ActiveIn.Value) &&
                          _engine.Native.Ledger.CurrentIndex >= _engine.ProtocolSettings.Hardforks[Neo.SmartContract.Native.NativeContract.Notary.ActiveIn.Value];

            Assert.IsTrue(isActive, "Notary contract should be active with current settings");
        }

        /// <summary>
        /// Tests the method signatures of key Notary contract methods.
        /// </summary>
        [TestMethod]
        public void TestNotaryMethodSignatures()
        {
            var manifest = Neo.SmartContract.Native.NativeContract.Notary
                .GetContractState(_engine.ProtocolSettings, uint.MaxValue).Manifest;
            var methods = manifest.Abi.Methods;

            // Test balanceOf method signature (account balance query)
            var balanceOfMethod = methods.FirstOrDefault(m => m.Name == "balanceOf");
            Assert.IsNotNull(balanceOfMethod, "balanceOf method should exist");
            Assert.AreEqual(1, balanceOfMethod.Parameters.Length, "balanceOf should have 1 parameter");
            Assert.AreEqual(ContractParameterType.Hash160, balanceOfMethod.Parameters[0].Type,
                "Parameter should be Hash160");
            Assert.AreEqual(ContractParameterType.Integer, balanceOfMethod.ReturnType,
                "Should return Integer");

            // Test getMaxNotValidBeforeDelta method signature (configuration query)
            var getMaxDeltaMethod = methods.FirstOrDefault(m => m.Name == "getMaxNotValidBeforeDelta");
            Assert.IsNotNull(getMaxDeltaMethod, "getMaxNotValidBeforeDelta method should exist");
            Assert.AreEqual(0, getMaxDeltaMethod.Parameters.Length,
                "getMaxNotValidBeforeDelta should have 0 parameters");
            Assert.AreEqual(ContractParameterType.Integer, getMaxDeltaMethod.ReturnType,
                "Should return Integer");
        }

        /// <summary>
        /// Tests integration between the Notary contract and other native contracts.
        /// </summary>
        [TestMethod]
        public void TestNotaryIntegrationWithOtherContracts()
        {
            // Verify other required native contracts are available for integration
            Assert.IsNotNull(_engine.Native.GAS, "GAS contract should be available");
            Assert.IsNotNull(_engine.Native.Policy, "Policy contract should be available");
            Assert.IsNotNull(_engine.Native.RoleManagement, "RoleManagement contract should be available");

            // Verify the Notary contract has a unique hash different from other contracts
            Assert.AreNotEqual(_engine.Native.Notary.Hash, _engine.Native.GAS.Hash,
                "Notary and GAS should have different hashes");
        }
    }
}
