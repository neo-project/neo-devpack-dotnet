// Copyright (C) 2015-2025 The Neo Project.
//
// NetworkContextTests.cs file belongs to the neo project and is free
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
    public class NetworkContextTests
    {
        [TestMethod]
        public void Constructor_ShouldSetCurrentNetwork()
        {
            var context = new NetworkContext("testnet");
            Assert.AreEqual("testnet", context.CurrentNetwork);
        }

        [TestMethod]
        public void Constructor_WithNullNetwork_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NetworkContext(null!));
        }

        [TestMethod]
        public void SetNetworkAddress_ShouldAddAddressForNetwork()
        {
            var context = new NetworkContext();
            var address = UInt160.Zero;
            
            context.SetNetworkAddress("testnet", address);
            
            Assert.IsTrue(context.HasNetworkAddress("testnet"));
            Assert.AreEqual(address, context.GetNetworkAddress("testnet"));
        }

        [TestMethod]
        public void SetNetworkAddress_WithNullNetwork_ShouldThrowArgumentNullException()
        {
            var context = new NetworkContext();
            var address = UInt160.Zero;
            
            Assert.ThrowsException<ArgumentNullException>(() => context.SetNetworkAddress(null!, address));
        }

        [TestMethod]
        public void SetNetworkAddress_WithNullAddress_ShouldThrowArgumentNullException()
        {
            var context = new NetworkContext();
            
            Assert.ThrowsException<ArgumentNullException>(() => context.SetNetworkAddress("testnet", (UInt160)null!));
        }

        [TestMethod]
        public void GetCurrentNetworkAddress_ShouldReturnAddressForCurrentNetwork()
        {
            var context = new NetworkContext("testnet");
            var address = UInt160.Zero;
            
            context.SetNetworkAddress("testnet", address);
            
            Assert.AreEqual(address, context.GetCurrentNetworkAddress());
        }

        [TestMethod]
        public void GetCurrentNetworkAddress_WhenNotConfigured_ShouldReturnNull()
        {
            var context = new NetworkContext("mainnet");
            
            Assert.IsNull(context.GetCurrentNetworkAddress());
        }

        [TestMethod]
        public void SwitchNetwork_ShouldChangeCurrentNetwork()
        {
            var context = new NetworkContext("privnet");
            
            context.SwitchNetwork("testnet");
            
            Assert.AreEqual("testnet", context.CurrentNetwork);
        }

        [TestMethod]
        public void SwitchNetwork_WithNullNetwork_ShouldThrowArgumentNullException()
        {
            var context = new NetworkContext();
            
            Assert.ThrowsException<ArgumentNullException>(() => context.SwitchNetwork(null!));
        }

        [TestMethod]
        public void HasNetworkAddress_WithConfiguredNetwork_ShouldReturnTrue()
        {
            var context = new NetworkContext();
            context.SetNetworkAddress("testnet", UInt160.Zero);
            
            Assert.IsTrue(context.HasNetworkAddress("testnet"));
        }

        [TestMethod]
        public void HasNetworkAddress_WithUnconfiguredNetwork_ShouldReturnFalse()
        {
            var context = new NetworkContext();
            
            Assert.IsFalse(context.HasNetworkAddress("testnet"));
        }

        [TestMethod]
        public void ConfiguredNetworks_ShouldReturnAllConfiguredNetworks()
        {
            var context = new NetworkContext();
            context.SetNetworkAddress("privnet", UInt160.Zero);
            context.SetNetworkAddress("testnet", UInt160.Zero);
            context.SetNetworkAddress("mainnet", UInt160.Zero);
            
            var networks = context.ConfiguredNetworks;
            
            Assert.AreEqual(3, networks.Count);
            Assert.IsTrue(networks.Contains("privnet"));
            Assert.IsTrue(networks.Contains("testnet"));
            Assert.IsTrue(networks.Contains("mainnet"));
        }

        [TestMethod]
        public void MultipleNetworks_ShouldHandleCorrectly()
        {
            var context = new NetworkContext("privnet");
            var privnetAddress = new UInt160(new byte[20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
            var testnetAddress = new UInt160(new byte[20] { 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            
            context.SetNetworkAddress("privnet", privnetAddress);
            context.SetNetworkAddress("testnet", testnetAddress);
            
            // Current network is privnet
            Assert.AreEqual(privnetAddress, context.GetCurrentNetworkAddress());
            
            // Switch to testnet
            context.SwitchNetwork("testnet");
            Assert.AreEqual(testnetAddress, context.GetCurrentNetworkAddress());
            
            // Both addresses should still be available
            Assert.AreEqual(privnetAddress, context.GetNetworkAddress("privnet"));
            Assert.AreEqual(testnetAddress, context.GetNetworkAddress("testnet"));
        }
    }
}