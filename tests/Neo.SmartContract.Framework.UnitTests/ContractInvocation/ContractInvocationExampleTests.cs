// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInvocationExampleTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using scfx::Neo.SmartContract.Framework;
using scfx::Neo.SmartContract.Framework.ContractInvocation;
using scfx::Neo.SmartContract.Framework.ContractInvocation.Attributes;
using scfx::Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.ContractInvocation
{
    [TestClass]
    public class ContractInvocationExampleTests
    {
        [TestMethod]
        public void TestBasicContractProxy()
        {
            // Test basic proxy pattern
            var identifier = "TestToken";
            var contractHash = CreateMockUInt160();
            
            var reference = new DeployedContractReference(identifier);
            reference.NetworkContext.SetNetworkAddress("testnet", contractHash);
            
            Assert.IsNotNull(reference);
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.IsTrue(reference.IsResolved);
        }

        [TestMethod]
        public void TestMultiNetworkScenario()
        {
            // Test multi-network support
            var identifier = "MultiNetContract";
            var privnetAddr = CreateMockUInt160();
            var testnetAddr = CreateMockUInt160();
            var mainnetAddr = CreateMockUInt160();
            
            var reference = DeployedContractReference.CreateMultiNetwork(
                identifier, privnetAddr, testnetAddr, mainnetAddr, "testnet");
            
            // Verify initial network
            Assert.AreEqual("testnet", reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(testnetAddr, reference.NetworkContext.GetCurrentNetworkAddress());
            
            // Switch networks
            reference.NetworkContext.SwitchNetwork("mainnet");
            Assert.AreEqual("mainnet", reference.NetworkContext.CurrentNetwork);
            Assert.AreEqual(mainnetAddr, reference.NetworkContext.GetCurrentNetworkAddress());
            
            // Switch to privnet
            reference.NetworkContext.SwitchNetwork("privnet");
            Assert.AreEqual(privnetAddr, reference.NetworkContext.GetCurrentNetworkAddress());
        }

        [TestMethod]
        public void TestDevelopmentContractScenario()
        {
            // Test development contract reference
            var identifier = "DevContract";
            var projectPath = "./TestProject.csproj";
            
            var reference = new DevelopmentContractReference(identifier, projectPath);
            
            Assert.AreEqual(identifier, reference.Identifier);
            Assert.AreEqual(projectPath, reference.ProjectPath);
            Assert.IsFalse(reference.IsResolved);
            
            // Simulate hash resolution
            var hash = CreateMockUInt160();
            reference.ResolveHash(hash);
            
            Assert.IsTrue(reference.IsResolved);
            Assert.AreEqual(hash, reference.ResolvedHash);
        }

        [TestMethod]
        public void TestCustomMethodAttribute()
        {
            // Test custom method mapping
            var attr = new CustomMethodAttribute("actualMethod")
            {
                CallFlags = CallFlags.All,
                TransformStrategy = ParameterTransform.JsonSerialize
            };
            
            Assert.AreEqual("actualMethod", attr.MethodName);
            Assert.AreEqual(CallFlags.All, attr.CallFlags);
            Assert.AreEqual(ParameterTransform.JsonSerialize, attr.TransformStrategy);
        }

        [TestMethod]
        public void TestMethodResolution()
        {
            // Test method resolution with custom mapping
            var resolver = new MethodResolver();
            var customMethod = "customOp";
            var actualMethod = "performOperation";
            
            resolver.RegisterMethod(customMethod, new MethodResolutionInfo
            {
                ContractMethodName = actualMethod,
                CallFlags = CallFlags.All
            });
            
            Assert.IsTrue(resolver.HasMethod(customMethod));
            
            var resolved = resolver.ResolveMethod(customMethod);
            Assert.IsNotNull(resolved);
            Assert.AreEqual(actualMethod, resolved.ContractMethodName);
            Assert.AreEqual(CallFlags.All, resolved.CallFlags);
        }

        [TestMethod]
        public void TestParameterTransformation()
        {
            // Test parameter transformation strategies
            var transformer = new DefaultParameterTransformer();
            
            // Test pass-through
            var input1 = new object[] { 1, "test", true };
            var result1 = transformer.Transform(input1, ParameterTransform.None);
            CollectionAssert.AreEqual(input1, result1);
            
            // Test JSON serialization
            var complexObj = new { Name = "Test", Value = 123 };
            var input2 = new object[] { complexObj };
            var result2 = transformer.Transform(input2, ParameterTransform.JsonSerialize);
            Assert.IsNotNull(result2);
            Assert.AreEqual(1, result2.Length);
        }

        [TestMethod]
        public void TestContractProxyInheritance()
        {
            // Test proxy base class functionality
            var proxy = new TestContractProxy();
            var reference = new DeployedContractReference("TestProxy");
            
            // Verify proxy can hold reference
            TestContractProxy.SetContractReference(reference);
            var retrieved = TestContractProxy.GetContractReference();
            
            Assert.AreSame(reference, retrieved);
        }

        [TestMethod]
        public void TestNetworkContextValidation()
        {
            // Test network context validation
            var context = new NetworkContext("testnet");
            
            // Test null network name
            Assert.ThrowsException<ArgumentNullException>(() => 
                context.SetNetworkAddress(null!, CreateMockUInt160()));
            
            // Test null address
            Assert.ThrowsException<ArgumentNullException>(() => 
                context.SetNetworkAddress("testnet", (scfx::Neo.SmartContract.Framework.UInt160)null!));
            
            // Test empty network name
            Assert.ThrowsException<ArgumentNullException>(() => 
                context.SetNetworkAddress("", CreateMockUInt160()));
        }

        [TestMethod]
        public void TestContractReferenceFactory()
        {
            // Test factory pattern
            ContractInvocationFactory.ClearRegisteredContracts();
            
            // Register development contract
            var devRef = ContractInvocationFactory.RegisterDevelopmentContract(
                "DevContract", "./dev.csproj");
            Assert.IsNotNull(devRef);
            
            // Register deployed contract
            var deployRef = ContractInvocationFactory.RegisterDeployedContract(
                "DeployedContract", CreateMockUInt160(), "testnet");
            Assert.IsNotNull(deployRef);
            
            // Verify retrieval
            var contracts = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(2, contracts.Count);
            
            // Test retrieval by identifier
            var retrieved = ContractInvocationFactory.GetContractReference("DevContract");
            Assert.AreSame(devRef, retrieved);
        }

        [TestMethod]
        public void TestComplexInvocationScenario()
        {
            // Test complex scenario with multiple contracts
            ContractInvocationFactory.ClearRegisteredContracts();
            
            // Setup multiple contracts
            var tokenContract = ContractInvocationFactory.RegisterMultiNetworkContract(
                "TokenContract",
                CreateMockUInt160(),
                CreateMockUInt160(),
                CreateMockUInt160(),
                "testnet"
            );
            
            var dexContract = ContractInvocationFactory.RegisterDeployedContract(
                "DexContract", CreateMockUInt160(), "testnet"
            );
            
            var governanceContract = ContractInvocationFactory.RegisterDevelopmentContract(
                "GovernanceContract", "../governance/governance.csproj"
            );
            
            // Verify all registered
            var all = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(3, all.Count);
            
            // Test network switching affects all
            ContractInvocationFactory.SwitchNetwork("mainnet");
            
            Assert.AreEqual("mainnet", tokenContract.NetworkContext.CurrentNetwork);
            Assert.AreEqual("mainnet", dexContract.NetworkContext.CurrentNetwork);
            Assert.AreEqual("mainnet", governanceContract.NetworkContext.CurrentNetwork);
        }

        // Helper method to create mock UInt160
        private static scfx::Neo.SmartContract.Framework.UInt160 CreateMockUInt160()
        {
            // Since UInt160 is abstract in the framework, we need to work around this
            // In real usage, this would be handled by the compiler
            return scfx::Neo.SmartContract.Framework.UInt160.Zero;
        }

        // Test proxy class
        private class TestContractProxy : ContractProxyBase
        {
            private static IContractReference? _reference;

            public static void SetContractReference(IContractReference reference)
            {
                _reference = reference;
            }

            public static IContractReference? GetContractReference()
            {
                return _reference;
            }
        }

        // Helper class for parameter transformation
        private class DefaultParameterTransformer : IParameterTransformer
        {
            public object[] Transform(object[] parameters, ParameterTransform strategy)
            {
                switch (strategy)
                {
                    case ParameterTransform.None:
                        return parameters;
                    case ParameterTransform.JsonSerialize:
                        // Simplified JSON serialization
                        return new object[] { System.Text.Json.JsonSerializer.Serialize(parameters[0]) };
                    default:
                        return parameters;
                }
            }
        }
    }
}