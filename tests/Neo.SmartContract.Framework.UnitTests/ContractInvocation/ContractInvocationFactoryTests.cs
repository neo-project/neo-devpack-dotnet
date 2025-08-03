// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInvocationFactoryTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.ContractInvocation;
using Neo.SmartContract.Framework.ContractInvocation.Attributes;
using System;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.ContractInvocation
{
    [TestClass]
    public class ContractInvocationFactoryTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // Clear registered contracts before each test
            ContractInvocationFactory.ClearRegisteredContracts();
        }

        [TestMethod]
        public void DefaultNetworkContext_ShouldReturnValidContext()
        {
            var context = ContractInvocationFactory.DefaultNetworkContext;
            
            Assert.IsNotNull(context);
            Assert.AreEqual("privnet", context.CurrentNetwork);
        }

        [TestMethod]
        public void RegisterDevelopmentContract_ShouldRegisterAndReturnReference()
        {
            var identifier = "TestContract";
            var projectPath = "./TestContract.csproj";
            
            var reference = ContractInvocationFactory.RegisterDevelopmentContract(identifier, projectPath);
            
            Assert.IsNotNull(reference);
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(projectPath, reference.ProjectPath);
            Assert.AreEqual(reference, ContractInvocationFactory.GetContractReference(identifier));
        }

        [TestMethod]
        public void RegisterDeployedContract_ShouldRegisterAndReturnReference()
        {
            var identifier = "NEP17Token";
            var networkContext = new NetworkContext("testnet");
            
            var reference = ContractInvocationFactory.RegisterDeployedContract(identifier, networkContext);
            
            Assert.IsNotNull(reference);
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(networkContext, reference.NetworkContext);
            Assert.AreEqual(reference, ContractInvocationFactory.GetContractReference(identifier));
        }

        [TestMethod]
        public void RegisterDeployedContract_WithSingleAddress_ShouldRegisterCorrectly()
        {
            var identifier = "TestContract";
            var address = UInt160.Zero;
            var network = "testnet";
            
            var reference = ContractInvocationFactory.RegisterDeployedContract(identifier, address, network);
            
            Assert.IsNotNull(reference);
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(network, reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(address, reference.NetworkContext.GetCurrentNetworkAddress());
        }

        [TestMethod]
        public void RegisterMultiNetworkContract_ShouldRegisterAllAddresses()
        {
            var identifier = "MultiNetworkContract";
            var privnetAddr = new UInt160(new byte[20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
            var testnetAddr = new UInt160(new byte[20] { 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
            var mainnetAddr = new UInt160(new byte[20] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            
            var reference = ContractInvocationFactory.RegisterMultiNetworkContract(
                identifier, privnetAddr, testnetAddr, mainnetAddr, "testnet");
            
            Assert.IsNotNull(reference);
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual("testnet", reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(privnetAddr, reference.NetworkContext.GetNetworkAddress("privnet"));
            Assert.AreEqual(testnetAddr, reference.NetworkContext.GetNetworkAddress("testnet"));
            Assert.AreEqual(mainnetAddr, reference.NetworkContext.GetNetworkAddress("mainnet"));
        }

        [TestMethod]
        public void GetContractReference_WithUnknownIdentifier_ShouldReturnNull()
        {
            var reference = ContractInvocationFactory.GetContractReference("UnknownContract");
            
            Assert.IsNull(reference);
        }

        [TestMethod]
        public void CreateFromAttribute_WithDevelopmentType_ShouldCreateDevelopmentReference()
        {
            var attribute = new ContractReferenceAttribute("TestContract")
            {
                ReferenceType = ContractReferenceType.Development,
                ProjectPath = "./TestContract.csproj"
            };
            
            var reference = ContractInvocationFactory.CreateFromAttribute(attribute);
            
            Assert.IsInstanceOfType(reference, typeof(DevelopmentContractReference));
            Assert.AreEqual("TestContract", reference.Identifier);
        }

        [TestMethod]
        public void CreateFromAttribute_WithDeployedType_ShouldCreateDeployedReference()
        {
            var attribute = new ContractReferenceAttribute("NEP17Token")
            {
                ReferenceType = ContractReferenceType.Deployed,
                TestnetAddress = "0x1234567890123456789012345678901234567890"
            };
            
            var reference = ContractInvocationFactory.CreateFromAttribute(attribute);
            
            Assert.IsInstanceOfType(reference, typeof(DeployedContractReference));
            Assert.AreEqual("NEP17Token", reference.Identifier);
        }

        [TestMethod]
        public void CreateFromAttribute_WithAutoType_ShouldDetectCorrectType()
        {
            // Test auto-detection for development contract (has project path)
            var devAttribute = new ContractReferenceAttribute("TestContract")
            {
                ReferenceType = ContractReferenceType.Auto,
                ProjectPath = "./TestContract.csproj"
            };
            
            var devReference = ContractInvocationFactory.CreateFromAttribute(devAttribute);
            Assert.IsInstanceOfType(devReference, typeof(DevelopmentContractReference));
            
            // Test auto-detection for deployed contract (has network addresses)
            var deployedAttribute = new ContractReferenceAttribute("NEP17Token")
            {
                ReferenceType = ContractReferenceType.Auto,
                TestnetAddress = "0x1234567890123456789012345678901234567890"
            };
            
            var deployedReference = ContractInvocationFactory.CreateFromAttribute(deployedAttribute);
            Assert.IsInstanceOfType(deployedReference, typeof(DeployedContractReference));
        }

        [TestMethod]
        public void CreateFromAttribute_WithNullAttribute_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => 
                ContractInvocationFactory.CreateFromAttribute(null!));
        }

        [TestMethod]
        public void GetAllRegisteredContracts_ShouldReturnAllContracts()
        {
            ContractInvocationFactory.RegisterDevelopmentContract("DevContract", "./dev.csproj");
            ContractInvocationFactory.RegisterDeployedContract("DeployedContract", UInt160.Zero);
            
            var contracts = ContractInvocationFactory.GetAllRegisteredContracts();
            
            Assert.AreEqual(2, contracts.Count);
            Assert.IsTrue(contracts.Any(c => c.Identifier == "DevContract"));
            Assert.IsTrue(contracts.Any(c => c.Identifier == "DeployedContract"));
        }

        [TestMethod]
        public void ClearRegisteredContracts_ShouldRemoveAllContracts()
        {
            ContractInvocationFactory.RegisterDevelopmentContract("TestContract1", "./test1.csproj");
            ContractInvocationFactory.RegisterDevelopmentContract("TestContract2", "./test2.csproj");
            
            Assert.AreEqual(2, ContractInvocationFactory.GetAllRegisteredContracts().Count);
            
            ContractInvocationFactory.ClearRegisteredContracts();
            
            Assert.AreEqual(0, ContractInvocationFactory.GetAllRegisteredContracts().Count);
        }

        [TestMethod]
        public void SwitchNetwork_ShouldSwitchAllRegisteredContracts()
        {
            var testnetAddr = UInt160.Zero;
            var mainnetAddr = new UInt160(new byte[20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
            
            var reference = ContractInvocationFactory.RegisterMultiNetworkContract(
                "TestContract", null, testnetAddr, mainnetAddr, "testnet");
            
            Assert.AreEqual("testnet", reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(testnetAddr, reference.NetworkContext.GetCurrentNetworkAddress());
            
            ContractInvocationFactory.SwitchNetwork("mainnet");
            
            Assert.AreEqual("mainnet", reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual("mainnet", ContractInvocationFactory.DefaultNetworkContext.CurrentNetwork);
            Assert.AreEqual(mainnetAddr, reference.NetworkContext.GetCurrentNetworkAddress());
        }
    }
}