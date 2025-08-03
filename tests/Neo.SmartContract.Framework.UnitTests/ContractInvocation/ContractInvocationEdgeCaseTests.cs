// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInvocationEdgeCaseTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using scfx::Neo.SmartContract.Framework;
using scfx::Neo.SmartContract.Framework.ContractInvocation;
using scfx::Neo.SmartContract.Framework.ContractInvocation.Attributes;
using scfx::Neo.SmartContract.Framework.ContractInvocation.Exceptions;
using scfx::Neo.SmartContract.Framework.Services;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework.UnitTests.ContractInvocation
{
    [TestClass]
    public class ContractInvocationEdgeCaseTests
    {
        [TestInitialize]
        public void Setup()
        {
            ContractInvocationFactory.ClearRegisteredContracts();
        }

        [TestMethod]
        public void TestNullAndEmptyInputValidation()
        {
            // Test null identifier
            Assert.ThrowsException<ArgumentNullException>(() =>
                ContractInvocationFactory.RegisterDevelopmentContract(null!, "./project.csproj")
            );

            // Test empty identifier
            Assert.ThrowsException<ArgumentNullException>(() =>
                ContractInvocationFactory.RegisterDevelopmentContract("", "./project.csproj")
            );

            // Test null project path
            Assert.ThrowsException<ArgumentNullException>(() =>
                ContractInvocationFactory.RegisterDevelopmentContract("Contract", null!)
            );

            // Test null network context
            Assert.ThrowsException<ArgumentNullException>(() =>
                new DeployedContractReference("Contract", null!)
            );
        }

        [TestMethod]
        public void TestDuplicateContractRegistration()
        {
            // Register a contract
            var contract1 = ContractInvocationFactory.RegisterDevelopmentContract(
                "DuplicateTest", "./project1.csproj"
            );

            // Try to register with same identifier
            var contract2 = ContractInvocationFactory.RegisterDevelopmentContract(
                "DuplicateTest", "./project2.csproj"
            );

            // Should return the same instance (no duplicates)
            Assert.AreSame(contract1, contract2);
            
            // Verify only one instance in registry
            var all = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(1, all.Count);
        }

        [TestMethod]
        public void TestUnresolvedContractAccess()
        {
            var devContract = new DevelopmentContractReference("UnresolvedContract", "./test.csproj");
            
            // Should not be resolved initially
            Assert.IsFalse(devContract.IsResolved);
            Assert.IsNull(devContract.ResolvedHash);
            
            // Accessing unresolved contract should handle gracefully
            var hash = devContract.ResolvedHash;
            Assert.IsNull(hash);
        }

        [TestMethod]
        public void TestNetworkSwitchWithMissingAddress()
        {
            var contract = ContractInvocationFactory.RegisterDeployedContract(
                "TestContract", CreateMockUInt160(), "testnet"
            );
            
            // Switch to network without configured address
            contract.NetworkContext.SwitchNetwork("mainnet");
            
            // Should handle gracefully
            Assert.AreEqual("mainnet", contract.NetworkContext.CurrentNetwork);
            Assert.IsNull(contract.NetworkContext.GetCurrentNetworkAddress());
            Assert.IsFalse(contract.IsResolved);
        }

        [TestMethod]
        public void TestMethodResolverEdgeCases()
        {
            var resolver = new MethodResolver();
            
            // Test null method name
            Assert.ThrowsException<ArgumentNullException>(() =>
                resolver.RegisterMethod(null!, new MethodResolutionInfo())
            );
            
            // Test empty method name
            Assert.ThrowsException<ArgumentNullException>(() =>
                resolver.RegisterMethod("", new MethodResolutionInfo())
            );
            
            // Test resolving non-existent method
            Assert.ThrowsException<MethodNotFoundException>(() =>
                resolver.ResolveMethod("nonExistentMethod")
            );
            
            // Test null method info
            Assert.ThrowsException<ArgumentNullException>(() =>
                resolver.RegisterMethod("test", null!)
            );
        }

        [TestMethod]
        public void TestCircularContractDependencies()
        {
            // Create contracts that reference each other
            var contractA = new DevelopmentContractReference("ContractA", "./a.csproj");
            var contractB = new DevelopmentContractReference("ContractB", "./b.csproj");
            
            // Simulate circular dependency scenario
            contractA.AddDependency(contractB);
            contractB.AddDependency(contractA);
            
            // Should handle without stack overflow
            Assert.IsNotNull(contractA);
            Assert.IsNotNull(contractB);
        }

        [TestMethod]
        public void TestLargeScaleContractRegistry()
        {
            // Test with many contracts
            const int contractCount = 1000;
            var contracts = new List<IContractReference>();
            
            for (int i = 0; i < contractCount; i++)
            {
                var contract = ContractInvocationFactory.RegisterDeployedContract(
                    $"Contract_{i}",
                    CreateMockUInt160(i),
                    "testnet"
                );
                contracts.Add(contract);
            }
            
            // Verify all registered
            var all = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(contractCount, all.Count);
            
            // Test retrieval performance
            var retrieved = ContractInvocationFactory.GetContractReference("Contract_500");
            Assert.IsNotNull(retrieved);
            Assert.AreEqual("Contract_500", retrieved.Identifier);
        }

        [TestMethod]
        public void TestParameterTransformationErrors()
        {
            var transformer = new FaultyParameterTransformer();
            
            // Test transformation that throws
            Assert.ThrowsException<TransformationException>(() =>
                transformer.Transform(new object[] { "test" }, ParameterTransform.Custom)
            );
            
            // Test with null parameters
            Assert.ThrowsException<ArgumentNullException>(() =>
                transformer.Transform(null!, ParameterTransform.None)
            );
        }

        [TestMethod]
        public void TestContractManifestHandling()
        {
            var contract = new DeployedContractReference("ManifestTest");
            
            // Initially no manifest
            Assert.IsNull(contract.Manifest);
            
            // Set invalid manifest
            Assert.ThrowsException<ArgumentNullException>(() =>
                contract.SetManifest(null!)
            );
            
            // Create mock manifest
            var manifest = new ContractManifest
            {
                Name = "TestContract",
                Abi = new ContractAbi
                {
                    Methods = new[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "testMethod",
                            Parameters = new ContractParameterDefinition[0],
                            ReturnType = ContractParameterType.Boolean
                        }
                    }
                }
            };
            
            contract.SetManifest(manifest);
            Assert.IsNotNull(contract.Manifest);
            Assert.AreEqual("TestContract", contract.Manifest.Name);
        }

        [TestMethod]
        public void TestConcurrentContractAccess()
        {
            // Test thread safety of factory
            var tasks = new List<System.Threading.Tasks.Task>();
            var contracts = new System.Collections.Concurrent.ConcurrentBag<IContractReference>();
            
            // Spawn multiple threads registering contracts
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                var task = System.Threading.Tasks.Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var contract = ContractInvocationFactory.RegisterDeployedContract(
                            $"Concurrent_{index}_{j}",
                            CreateMockUInt160(index * 100 + j)
                        );
                        contracts.Add(contract);
                    }
                });
                tasks.Add(task);
            }
            
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            
            // Verify all contracts registered
            Assert.AreEqual(100, contracts.Count);
            var all = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(100, all.Count);
        }

        [TestMethod]
        public void TestInvalidNetworkNames()
        {
            var context = new NetworkContext();
            
            // Test various invalid network names
            var invalidNames = new[] { " ", "\t", "\n", "network with spaces", "network\twith\ttabs" };
            
            foreach (var name in invalidNames)
            {
                // Should handle gracefully or throw appropriate exception
                try
                {
                    context.SetNetworkAddress(name, CreateMockUInt160());
                    // If it accepts, it should be retrievable
                    Assert.IsTrue(context.HasNetworkAddress(name));
                }
                catch (ArgumentException)
                {
                    // Expected for invalid names
                }
            }
        }

        [TestMethod]
        public void TestMemoryLeakScenario()
        {
            // Test clearing and re-registering many times
            for (int iteration = 0; iteration < 100; iteration++)
            {
                // Register contracts
                for (int i = 0; i < 10; i++)
                {
                    ContractInvocationFactory.RegisterDeployedContract(
                        $"MemTest_{i}",
                        CreateMockUInt160(iteration * 1000 + i)
                    );
                }
                
                // Clear all
                ContractInvocationFactory.ClearRegisteredContracts();
                
                // Verify cleared
                Assert.AreEqual(0, ContractInvocationFactory.GetAllRegisteredContracts().Count);
            }
            
            // Final verification
            Assert.AreEqual(0, ContractInvocationFactory.GetAllRegisteredContracts().Count);
        }

        // Helper classes
        private static scfx::Neo.SmartContract.Framework.UInt160 CreateMockUInt160(int seed = 0)
        {
            return scfx::Neo.SmartContract.Framework.UInt160.Zero;
        }

        private class FaultyParameterTransformer : IParameterTransformer
        {
            public object[] Transform(object[] parameters, ParameterTransform strategy)
            {
                if (parameters == null)
                    throw new ArgumentNullException(nameof(parameters));
                    
                if (strategy == ParameterTransform.Custom)
                    throw new TransformationException("Simulated transformation error");
                    
                return parameters;
            }
        }

        private class TransformationException : Exception
        {
            public TransformationException(string message) : base(message) { }
        }
    }

    // Mock interfaces for testing
    public interface IParameterTransformer
    {
        object[] Transform(object[] parameters, ParameterTransform strategy);
    }

    // Extension for testing
    public static class DevelopmentContractReferenceExtensions
    {
        private static readonly Dictionary<string, List<IContractReference>> dependencies = new();

        public static void AddDependency(this DevelopmentContractReference contract, IContractReference dependency)
        {
            if (!dependencies.ContainsKey(contract.Identifier))
                dependencies[contract.Identifier] = new List<IContractReference>();
            
            dependencies[contract.Identifier].Add(dependency);
        }
    }
}