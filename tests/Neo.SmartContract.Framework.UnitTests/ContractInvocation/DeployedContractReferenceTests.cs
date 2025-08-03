// Copyright (C) 2015-2025 The Neo Project.
//
// DeployedContractReferenceTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.ContractInvocation;
using System;

namespace Neo.SmartContract.Framework.UnitTests.ContractInvocation
{
    [TestClass]
    public class DeployedContractReferenceTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var identifier = "NEP17Token";
            var networkContext = new NetworkContext("testnet");
            
            var reference = new DeployedContractReference(identifier, networkContext);
            
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(networkContext, reference.NetworkContext);
            Assert.IsTrue(reference.FetchManifest);
            Assert.IsNull(reference.Manifest);
        }

        [TestMethod]
        public void Constructor_WithNullIdentifier_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                new DeployedContractReference(null!));
        }

        [TestMethod]
        public void Constructor_WithNullNetworkContext_ShouldCreateDefaultNetworkContext()
        {
            var reference = new DeployedContractReference("TestContract");
            
            Assert.IsNotNull(reference.NetworkContext);
            Assert.AreEqual("privnet", reference.NetworkContext.CurrentNetwork);
        }

        [TestMethod]
        public void Create_ShouldCreateReferenceWithSingleNetworkAddress()
        {
            var identifier = "TestContract";
            var address = UInt160.Zero;
            var network = "testnet";
            
            var reference = DeployedContractReference.Create(identifier, address, network);
            
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(network, reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(address, reference.NetworkContext.GetCurrentNetworkAddress());
        }

        [TestMethod]
        public void CreateMultiNetwork_ShouldCreateReferenceWithMultipleAddresses()
        {
            var identifier = "TestContract";
            var privnetAddr = new UInt160(new byte[20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
            var testnetAddr = new UInt160(new byte[20] { 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            var mainnetAddr = new UInt160(new byte[20] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var reference = DeployedContractReference.CreateMultiNetwork(
                identifier, privnetAddr, testnetAddr, mainnetAddr, "testnet");
            
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual("testnet", reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(testnetAddr, reference.NetworkContext.GetCurrentNetworkAddress());
            Assert.AreEqual(privnetAddr, reference.NetworkContext.GetNetworkAddress("privnet"));
            Assert.AreEqual(testnetAddr, reference.NetworkContext.GetNetworkAddress("testnet"));
            Assert.AreEqual(mainnetAddr, reference.NetworkContext.GetNetworkAddress("mainnet"));
        }

        [TestMethod]
        public void ResolvedHash_ShouldReturnCurrentNetworkAddress()
        {
            var reference = new DeployedContractReference("TestContract");
            var address = UInt160.Zero;
            
            Assert.IsNull(reference.ResolvedHash);
            Assert.IsFalse(reference.IsResolved);
            
            reference.NetworkContext.SetNetworkAddress("privnet", address);
            
            Assert.AreEqual(address, reference.ResolvedHash);
            Assert.IsTrue(reference.IsResolved);
        }

        [TestMethod]
        public void AddNetworkAddress_ShouldAddAddressToNetworkContext()
        {
            var reference = new DeployedContractReference("TestContract");
            var address = UInt160.Zero;
            
            reference.AddNetworkAddress("testnet", address);
            
            Assert.AreEqual(address, reference.NetworkContext.GetNetworkAddress("testnet"));
        }

        [TestMethod]
        public void AddNetworkAddress_WithStringAddress_ShouldCallNetworkContextMethod()
        {
            var reference = new DeployedContractReference("TestContract");
            
            // This test verifies the method exists and doesn't throw
            // The actual string parsing is handled by NetworkContext
            reference.AddNetworkAddress("testnet", "0x1234567890123456789012345678901234567890");
        }

        [TestMethod]
        public void SetManifest_ShouldSetManifestProperty()
        {
            var reference = new DeployedContractReference("TestContract");
            var manifest = new ContractManifest();
            
            reference.SetManifest(manifest);
            
            Assert.AreEqual(manifest, reference.Manifest);
        }

        [TestMethod]
        public void FetchManifest_ShouldBeConfigurable()
        {
            var reference = new DeployedContractReference("TestContract");
            
            Assert.IsTrue(reference.FetchManifest);
            
            reference.FetchManifest = false;
            Assert.IsFalse(reference.FetchManifest);
        }

        [TestMethod]
        public void IsResolved_WithMultipleNetworks_ShouldDependOnCurrentNetwork()
        {
            var reference = new DeployedContractReference("TestContract", new NetworkContext("testnet"));
            var testnetAddr = UInt160.Zero;
            
            // Not resolved initially
            Assert.IsFalse(reference.IsResolved);
            
            // Add address for a different network
            reference.AddNetworkAddress("privnet", testnetAddr);
            Assert.IsFalse(reference.IsResolved); // Still not resolved for current network
            
            // Add address for current network
            reference.AddNetworkAddress("testnet", testnetAddr);
            Assert.IsTrue(reference.IsResolved); // Now resolved
        }
    }
}