// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInvocationIntegrationTests.cs file belongs to the neo project and is free
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
using scfx::Neo.SmartContract.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.ContractInvocation
{
    [TestClass]
    public class ContractInvocationIntegrationTests
    {
        [TestInitialize]
        public void Setup()
        {
            ContractInvocationFactory.ClearRegisteredContracts();
        }

        [TestMethod]
        public void TestEndToEndTokenSwapScenario()
        {
            // Simulate a DEX scenario with multiple token contracts
            
            // Register token contracts
            var token1 = ContractInvocationFactory.RegisterMultiNetworkContract(
                "Token1",
                CreateMockUInt160(1),
                CreateMockUInt160(2),
                CreateMockUInt160(3),
                "testnet"
            );
            
            var token2 = ContractInvocationFactory.RegisterMultiNetworkContract(
                "Token2",
                CreateMockUInt160(4),
                CreateMockUInt160(5),
                CreateMockUInt160(6),
                "testnet"
            );
            
            // Register DEX contract
            var dex = ContractInvocationFactory.RegisterDeployedContract(
                "DexContract",
                CreateMockUInt160(7),
                "testnet"
            );
            
            // Verify all contracts are registered
            var contracts = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(3, contracts.Count);
            
            // Simulate network switch
            ContractInvocationFactory.SwitchNetwork("mainnet");
            
            // Verify all contracts switched
            Assert.AreEqual("mainnet", token1.NetworkContext.CurrentNetwork);
            Assert.AreEqual("mainnet", token2.NetworkContext.CurrentNetwork);
            Assert.AreEqual("mainnet", dex.NetworkContext.CurrentNetwork);
        }

        [TestMethod]
        public void TestGovernanceVotingScenario()
        {
            // Simulate a governance system with multiple contracts
            
            // Register governance contracts
            var votingContract = ContractInvocationFactory.RegisterDevelopmentContract(
                "VotingContract", "./voting/VotingContract.csproj"
            );
            
            var treasuryContract = ContractInvocationFactory.RegisterDeployedContract(
                "TreasuryContract", CreateMockUInt160(8)
            );
            
            var stakingContract = ContractInvocationFactory.RegisterMultiNetworkContract(
                "StakingContract",
                CreateMockUInt160(9),
                CreateMockUInt160(10),
                CreateMockUInt160(11)
            );
            
            // Create method resolver for complex governance operations
            var resolver = new MethodResolver();
            
            resolver.RegisterMethod("createProposal", new MethodResolutionInfo
            {
                ContractMethodName = "submitProposal",
                CallFlags = CallFlags.All,
                RequiredParameters = new[] { typeof(string), typeof(uint), typeof(byte[]) }
            });
            
            resolver.RegisterMethod("executeProposal", new MethodResolutionInfo
            {
                ContractMethodName = "execute",
                CallFlags = CallFlags.All,
                TransformParameters = (args) => {
                    // Transform proposal ID to proper format
                    return new object[] { ConvertToBytes((int)args[0]) };
                }
            });
            
            // Verify resolver
            Assert.IsTrue(resolver.HasMethod("createProposal"));
            Assert.IsTrue(resolver.HasMethod("executeProposal"));
            
            var createMethod = resolver.ResolveMethod("createProposal");
            Assert.AreEqual("submitProposal", createMethod.ContractMethodName);
        }

        [TestMethod]
        public void TestDeFiProtocolIntegration()
        {
            // Test complex DeFi protocol with multiple interacting contracts
            
            var contracts = new Dictionary<string, IContractReference>();
            
            // Core protocol contracts
            contracts["LendingPool"] = ContractInvocationFactory.RegisterDeployedContract(
                "LendingPool", CreateMockUInt160(12)
            );
            
            contracts["Oracle"] = ContractInvocationFactory.RegisterMultiNetworkContract(
                "PriceOracle",
                CreateMockUInt160(13),
                CreateMockUInt160(14),
                CreateMockUInt160(15)
            );
            
            contracts["Collateral"] = ContractInvocationFactory.RegisterDevelopmentContract(
                "CollateralManager", "../defi/CollateralManager.csproj"
            );
            
            // Asset contracts
            var assets = new[] { "USDT", "USDC", "DAI", "WBTC", "WETH" };
            foreach (var asset in assets)
            {
                contracts[asset] = ContractInvocationFactory.RegisterDeployedContract(
                    asset, CreateMockUInt160(asset.GetHashCode())
                );
            }
            
            // Verify all contracts registered
            var all = ContractInvocationFactory.GetAllRegisteredContracts();
            Assert.AreEqual(8, all.Count); // 3 core + 5 assets
            
            // Test batch operations
            var assetContracts = all.Where(c => assets.Contains(c.Identifier)).ToList();
            Assert.AreEqual(5, assetContracts.Count);
        }

        [TestMethod]
        public void TestContractUpgradeScenario()
        {
            // Test contract upgrade scenario with address changes
            
            var contractId = "UpgradableContract";
            
            // Initial deployment
            var v1Address = CreateMockUInt160(20);
            var contract = ContractInvocationFactory.RegisterDeployedContract(
                contractId, v1Address, "mainnet"
            );
            
            Assert.AreEqual(v1Address, contract.NetworkContext.GetCurrentNetworkAddress());
            
            // Simulate upgrade by updating address
            var v2Address = CreateMockUInt160(21);
            contract.NetworkContext.SetNetworkAddress("mainnet", v2Address);
            
            Assert.AreEqual(v2Address, contract.NetworkContext.GetCurrentNetworkAddress());
            
            // Add new network after upgrade
            contract.NetworkContext.SetNetworkAddress("testnet", CreateMockUInt160(22));
            Assert.IsTrue(contract.NetworkContext.HasNetworkAddress("testnet"));
        }

        [TestMethod]
        public void TestCrossContractCallChain()
        {
            // Test scenario where contracts call each other in a chain
            
            // A -> B -> C -> D
            var chainContracts = new List<IContractReference>();
            
            for (int i = 0; i < 4; i++)
            {
                var contract = ContractInvocationFactory.RegisterDeployedContract(
                    $"Contract{(char)('A' + i)}", 
                    CreateMockUInt160(30 + i)
                );
                chainContracts.Add(contract);
            }
            
            // Verify chain
            Assert.AreEqual(4, chainContracts.Count);
            foreach (var contract in chainContracts)
            {
                Assert.IsTrue(contract.IsResolved);
            }
        }

        [TestMethod]
        public void TestContractFactoryWithAttributes()
        {
            // Test using attributes for contract configuration
            var attr = new ContractReferenceAttribute("AttributeContract")
            {
                ContractHash = "0x" + new string('a', 40),
                Network = "testnet"
            };
            
            var reference = ContractInvocationFactory.CreateFromAttribute(attr);
            
            Assert.IsNotNull(reference);
            Assert.AreEqual("AttributeContract", reference.Identifier);
            Assert.IsTrue(reference is DeployedContractReference);
        }

        [TestMethod]
        public void TestMethodResolverWithComplexTypes()
        {
            var resolver = new MethodResolver();
            
            // Register methods with different parameter requirements
            resolver.RegisterMethod("transfer", new MethodResolutionInfo
            {
                ContractMethodName = "transfer",
                CallFlags = CallFlags.All,
                RequiredParameters = new[] { typeof(UInt160), typeof(UInt160), typeof(BigInteger) }
            });
            
            resolver.RegisterMethod("mint", new MethodResolutionInfo
            {
                ContractMethodName = "mintTokens",
                CallFlags = CallFlags.All,
                RequiredParameters = new[] { typeof(UInt160), typeof(BigInteger) },
                TransformParameters = (args) => {
                    // Add default data parameter
                    return new object[] { args[0], args[1], new byte[0] };
                }
            });
            
            resolver.RegisterMethod("burn", new MethodResolutionInfo
            {
                ContractMethodName = "burnTokens",
                CallFlags = CallFlags.States | CallFlags.AllowNotify,
                OptionalParameters = new Dictionary<string, object>
                {
                    ["reason"] = "user_request",
                    ["timestamp"] = 0
                }
            });
            
            // Test resolution
            var transferMethod = resolver.ResolveMethod("transfer");
            Assert.AreEqual(3, transferMethod.RequiredParameters?.Length);
            
            var mintMethod = resolver.ResolveMethod("mint");
            Assert.IsNotNull(mintMethod.TransformParameters);
            
            var burnMethod = resolver.ResolveMethod("burn");
            Assert.IsTrue(burnMethod.OptionalParameters?.ContainsKey("reason"));
        }

        [TestMethod]
        public void TestNetworkMigrationScenario()
        {
            // Test migrating contracts from testnet to mainnet
            
            var contracts = new[] { "Contract1", "Contract2", "Contract3" };
            var references = new List<IContractReference>();
            
            // Deploy on testnet first
            foreach (var name in contracts)
            {
                var testnetAddr = CreateMockUInt160(name.GetHashCode());
                var reference = ContractInvocationFactory.RegisterDeployedContract(
                    name, testnetAddr, "testnet"
                );
                references.Add(reference);
            }
            
            // Prepare mainnet addresses
            foreach (var reference in references)
            {
                var mainnetAddr = CreateMockUInt160(reference.Identifier.GetHashCode() + 1000);
                reference.NetworkContext.SetNetworkAddress("mainnet", mainnetAddr);
            }
            
            // Switch to mainnet
            ContractInvocationFactory.SwitchNetwork("mainnet");
            
            // Verify all contracts have mainnet addresses
            foreach (var reference in references)
            {
                Assert.AreEqual("mainnet", reference.NetworkContext.CurrentNetwork);
                Assert.IsTrue(reference.NetworkContext.HasNetworkAddress("mainnet"));
                Assert.IsNotNull(reference.NetworkContext.GetCurrentNetworkAddress());
            }
        }

        // Helper methods
        private static scfx::Neo.SmartContract.Framework.UInt160 CreateMockUInt160(int seed = 0)
        {
            return scfx::Neo.SmartContract.Framework.UInt160.Zero;
        }

        private static byte[] ConvertToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}