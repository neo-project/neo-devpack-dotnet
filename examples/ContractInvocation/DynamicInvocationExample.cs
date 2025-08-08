// Example demonstrating dynamic contract invocation scenarios
// This includes runtime contract discovery, factory patterns, and upgrade scenarios

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.ContractInvocation;
using Neo.SmartContract.Framework.ContractInvocation.Attributes;
using Neo.SmartContract.Framework.ContractInvocation.Exceptions;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Examples.ContractInvocation
{
    [DisplayName("DynamicInvocationExample")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Description", "Dynamic contract invocation patterns")]
    public class DynamicInvocationExample : SmartContract
    {
        #region Storage Keys
        
        private static readonly ByteString REGISTRY_PREFIX = "registry";
        private static readonly ByteString CONTRACT_VERSION_PREFIX = "version";
        private static readonly ByteString FACTORY_CONTRACTS_PREFIX = "factory";
        
        #endregion

        #region Dynamic Contract Registration

        [DisplayName("registerContract")]
        public static bool RegisterContract(string identifier, UInt160 contractHash, string network)
        {
            // Only contract owner can register contracts
            if (!IsOwner()) return false;

            // Validate inputs
            if (string.IsNullOrEmpty(identifier) || contractHash == null || contractHash == UInt160.Zero)
                throw new ArgumentException("Invalid contract registration parameters");

            // Store contract in registry
            var key = CreateRegistryKey(identifier, network);
            Storage.Put(Storage.CurrentContext, key, contractHash);

            // Register with factory for runtime access
            var registeredContract = ContractInvocationFactory.RegisterDeployedContract(
                identifier, contractHash, network);

            Runtime.Log($"Registered contract {identifier} on {network}: {contractHash}");
            return true;
        }

        [DisplayName("getRegisteredContract")]
        public static UInt160? GetRegisteredContract(string identifier, string network)
        {
            var key = CreateRegistryKey(identifier, network);
            var stored = Storage.Get(Storage.CurrentContext, key);
            
            if (stored == null || stored.Length == 0)
                return null;

            return (UInt160)stored;
        }

        [DisplayName("dynamicContractCall")]
        public static object DynamicContractCall(string identifier, string network, string method, object[] args)
        {
            // Look up contract from registry
            var contractHash = GetRegisteredContract(identifier, network);
            if (contractHash == null)
                throw new ContractNotResolvedException(identifier, $"Contract not found in registry for {network}");

            // Try to get from factory first (may already be registered)
            var contractRef = ContractInvocationFactory.GetContractReference(identifier);
            
            if (contractRef == null)
            {
                // Register dynamically if not found
                contractRef = ContractInvocationFactory.RegisterDeployedContract(
                    identifier, contractHash, network);
            }

            // Ensure we're on the correct network
            if (contractRef.NetworkContext?.CurrentNetwork != network)
            {
                contractRef.NetworkContext?.SwitchNetwork(network);
            }

            // Make the call
            return Contract.Call(contractHash, method, CallFlags.All, args);
        }

        #endregion

        #region Contract Factory Pattern

        [DisplayName("deployFromFactory")]
        public static UInt160 DeployFromFactory(string contractType, object[] constructorArgs)
        {
            // Only authorized deployers
            if (!IsAuthorizedDeployer()) 
                throw new Exception("Not authorized to deploy contracts");

            // Get factory contract for the requested type
            var factoryContract = GetFactoryContract(contractType);
            if (factoryContract == null)
                throw new Exception($"No factory found for contract type: {contractType}");

            // Call factory to deploy new instance
            var deployedHash = (UInt160)Contract.Call(
                factoryContract, 
                "deployInstance", 
                CallFlags.All, 
                constructorArgs);

            // Register the new contract
            var instanceId = $"{contractType}_{Runtime.Time}";
            RegisterContract(instanceId, deployedHash, GetCurrentNetwork());

            // Store version information
            StoreContractVersion(deployedHash, 1);

            Runtime.Log($"Deployed new {contractType} instance: {deployedHash}");
            return deployedHash;
        }

        [DisplayName("createTokenPair")]
        public static object[] CreateTokenPair(string token1Symbol, string token2Symbol)
        {
            // Deploy two token contracts and a DEX pair contract
            var token1Args = new object[] { token1Symbol, 8, 1000000 };
            var token2Args = new object[] { token2Symbol, 8, 1000000 };

            // Deploy tokens
            var token1Hash = DeployFromFactory("NEP17Token", token1Args);
            var token2Hash = DeployFromFactory("NEP17Token", token2Args);

            // Deploy DEX pair
            var pairArgs = new object[] { token1Hash, token2Hash };
            var pairHash = DeployFromFactory("DEXPair", pairArgs);

            // Register as a token pair
            RegisterTokenPair(token1Symbol, token2Symbol, pairHash);

            return new object[] { token1Hash, token2Hash, pairHash };
        }

        #endregion

        #region Contract Upgrade Scenarios

        [DisplayName("upgradeContract")]
        public static bool UpgradeContract(string identifier, byte[] newNef, string newManifest, object[] migrationData)
        {
            // Only owner can upgrade
            if (!IsOwner()) return false;

            // Get current contract
            var currentHash = GetRegisteredContract(identifier, GetCurrentNetwork());
            if (currentHash == null)
                throw new Exception($"Contract {identifier} not found");

            // Get current version
            var currentVersion = GetContractVersion(currentHash);

            // Call update on the contract (it must support updates)
            try
            {
                Contract.Call(currentHash, "update", CallFlags.All, newNef, newManifest, migrationData);
                
                // Update version
                StoreContractVersion(currentHash, currentVersion + 1);
                
                Runtime.Log($"Upgraded {identifier} to version {currentVersion + 1}");
                return true;
            }
            catch (Exception ex)
            {
                Runtime.Log($"Upgrade failed: {ex.Message}");
                return false;
            }
        }

        [DisplayName("migrateToNewContract")]
        public static UInt160 MigrateToNewContract(string oldIdentifier, string newIdentifier, byte[] newNef, string newManifest)
        {
            // This demonstrates migrating from an old contract to a completely new one
            if (!IsOwner()) throw new Exception("Not authorized");

            // Get old contract
            var oldContract = GetRegisteredContract(oldIdentifier, GetCurrentNetwork());
            if (oldContract == null)
                throw new Exception($"Old contract {oldIdentifier} not found");

            // Deploy new contract
            var newContract = (UInt160)ContractManagement.Deploy(newNef, newManifest);

            // Migrate data (example: transfer ownership/funds)
            try
            {
                // Call migration method on old contract
                Contract.Call(oldContract, "migrate", CallFlags.All, newContract);
                
                // Update registry - replace old with new
                RegisterContract(newIdentifier, newContract, GetCurrentNetwork());
                
                // Mark old contract as deprecated
                MarkContractDeprecated(oldIdentifier);
                
                Runtime.Log($"Migrated {oldIdentifier} to {newIdentifier}: {newContract}");
                return newContract;
            }
            catch (Exception ex)
            {
                Runtime.Log($"Migration failed: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Discovery and Enumeration

        [DisplayName("discoverContracts")]
        public static object[] DiscoverContracts(string pattern)
        {
            // This demonstrates discovering contracts based on a pattern
            var results = new object[10]; // Max 10 results
            var count = 0;

            // Iterate through registry (simplified - in practice use proper iteration)
            var iterator = Storage.Find(Storage.CurrentContext, REGISTRY_PREFIX, FindOptions.KeysOnly);
            
            foreach (var item in iterator)
            {
                if (count >= 10) break;
                
                var key = (byte[])item;
                var keyStr = key.ToString();
                
                if (keyStr.Contains(pattern))
                {
                    var contractHash = Storage.Get(Storage.CurrentContext, key);
                    results[count++] = new object[] { keyStr, contractHash };
                }
            }

            // Trim results array
            var finalResults = new object[count];
            for (int i = 0; i < count; i++)
            {
                finalResults[i] = results[i];
            }

            return finalResults;
        }

        [DisplayName("getAllVersions")]
        public static object[] GetAllVersions()
        {
            // Get version information for all registered contracts
            var results = new Map<string, int>();
            
            var iterator = Storage.Find(Storage.CurrentContext, CONTRACT_VERSION_PREFIX);
            foreach (var item in iterator)
            {
                var contractHash = ((ByteString)item).ToString();
                var version = 1; // Simplified for example
                results[contractHash.ToString()] = version;
            }

            return new object[] { results };
        }

        #endregion

        #region Helper Methods

        private static byte[] CreateRegistryKey(string identifier, string network)
        {
            return REGISTRY_PREFIX + identifier + network;
        }

        private static bool IsOwner()
        {
            // Simplified ownership check
            return Runtime.CheckWitness(GetOwner());
        }

        private static bool IsAuthorizedDeployer()
        {
            // Check if caller is authorized to deploy contracts
            return IsOwner() || Runtime.CheckWitness(GetDeployerAddress());
        }

        private static UInt160 GetOwner()
        {
            // In practice, this would be stored in storage
            return (UInt160)Storage.Get(Storage.CurrentContext, "owner");
        }

        private static UInt160 GetDeployerAddress()
        {
            // In practice, this would be stored in storage
            return (UInt160)Storage.Get(Storage.CurrentContext, "deployer");
        }

        private static string GetCurrentNetwork()
        {
            // In practice, this might be determined by configuration
            return ContractInvocationFactory.DefaultNetworkContext.CurrentNetwork;
        }

        private static UInt160? GetFactoryContract(string contractType)
        {
            var key = FACTORY_CONTRACTS_PREFIX + contractType;
            var stored = Storage.Get(Storage.CurrentContext, key);
            return stored?.Length > 0 ? (UInt160)stored : null;
        }

        private static void RegisterTokenPair(string token1, string token2, UInt160 pairContract)
        {
            var pairId = $"{token1}-{token2}";
            RegisterContract(pairId, pairContract, GetCurrentNetwork());
        }

        private static void StoreContractVersion(UInt160 contractHash, int version)
        {
            var key = CONTRACT_VERSION_PREFIX.Concat(contractHash);
            Storage.Put(Storage.CurrentContext, key, version);
        }

        private static int GetContractVersion(UInt160 contractHash)
        {
            var key = CONTRACT_VERSION_PREFIX.Concat(contractHash);
            var stored = Storage.Get(Storage.CurrentContext, key);
            if (stored == null || stored.Length == 0) return 0;
            return stored.ToInteger();
        }

        private static void MarkContractDeprecated(string identifier)
        {
            var key = CreateRegistryKey(identifier + "_deprecated", GetCurrentNetwork());
            Storage.Put(Storage.CurrentContext, key, Runtime.Time);
        }

        #endregion
    }
}