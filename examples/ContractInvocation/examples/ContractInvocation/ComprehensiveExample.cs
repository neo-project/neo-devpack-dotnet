// Comprehensive example demonstrating various contract invocation scenarios
// This includes calling deployed contracts, development contracts, multi-network support,
// and contracts that are not yet deployed

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
    [DisplayName("ComprehensiveExample")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Description", "Comprehensive contract invocation examples")]
    public class ComprehensiveExample : SmartContract
    {
        #region Contract References

        // 1. Reference to deployed system contracts (NEO and GAS)
        [ContractReference("NEO",
            ReferenceType = ContractReferenceType.Deployed,
            PrivnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
            TestnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
            MainnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
        private static IContractReference? NeoContract;

        [ContractReference("GAS",
            ReferenceType = ContractReferenceType.Deployed,
            PrivnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
            TestnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
            MainnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf")]
        private static IContractReference? GasContract;

        // 2. Reference to a custom deployed token (multi-network)
        [ContractReference("CustomToken",
            ReferenceType = ContractReferenceType.Deployed,
            PrivnetAddress = "0x1234567890abcdef1234567890abcdef12345678",
            TestnetAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
            MainnetAddress = "0x567890abcdef1234567890abcdef123456789012")]
        private static IContractReference? CustomTokenContract;

        // 3. Reference to a contract in development (not yet deployed)
        [ContractReference("MyDevelopmentContract",
            ReferenceType = ContractReferenceType.Development,
            ProjectPath = "../MyToken/MyToken.csproj")]
        private static IContractReference? DevelopmentContract;

        // 4. Reference to another development contract that may or may not be deployed
        [ContractReference("MyGovernanceContract",
            ReferenceType = ContractReferenceType.Auto,
            ProjectPath = "../Governance/Governance.csproj",
            PrivnetAddress = "0x9876543210fedcba9876543210fedcba98765432")]
        private static IContractReference? GovernanceContract;

        // 5. Reference to an oracle contract (only deployed on specific networks)
        [ContractReference("PriceOracle",
            ReferenceType = ContractReferenceType.Deployed,
            TestnetAddress = "0xfedcba9876543210fedcba9876543210fedcba98",
            MainnetAddress = "0x3210fedcba9876543210fedcba9876543210fedc")]
        private static IContractReference? OracleContract;

        #endregion

        #region Basic Contract Calls

        [DisplayName("getTokenBalance")]
        public static BigInteger GetTokenBalance(UInt160 account, string tokenId)
        {
            // Validate input
            if (account == null || account == UInt160.Zero)
                throw new ArgumentException("Invalid account address");

            // Select contract based on token ID
            var contract = SelectContract(tokenId);
            
            // Handle contracts that might not be resolved yet
            if (!contract.IsResolved)
            {
                Runtime.Log($"Contract {contract.Identifier} is not resolved on current network");
                return 0;
            }

            // Make the contract call
            var result = Contract.Call(contract.ResolvedHash!, "balanceOf", CallFlags.ReadOnly, account);
            return result != null ? (BigInteger)result : 0;
        }

        #endregion

        #region Development Contract Scenarios

        [DisplayName("callDevelopmentContract")]
        public static object CallDevelopmentContract(string method, object[] args)
        {
            // Check if development contract is available
            if (DevelopmentContract == null)
                throw new ContractNotResolvedException("MyDevelopmentContract", "Development contract not configured");

            // Development contracts might not be deployed yet
            if (!DevelopmentContract.IsResolved)
            {
                Runtime.Log("Development contract not yet deployed - attempting to resolve");
                
                // In a real scenario, you might trigger deployment here
                // For this example, we'll return a default value
                return "Contract not deployed";
            }

            // Call the method on the development contract
            return Contract.Call(DevelopmentContract.ResolvedHash!, method, CallFlags.All, args);
        }

        [DisplayName("deployAndCall")]
        public static object DeployAndCall(byte[] nefFile, string manifest)
        {
            // This demonstrates deploying a contract and then calling it
            // Note: This requires appropriate permissions

            try
            {
                // Deploy the contract
                var deployedHash = (UInt160)ContractManagement.Deploy(nefFile, manifest);
                
                if (deployedHash == null || deployedHash == UInt160.Zero)
                    throw new Exception("Failed to deploy contract");

                // Log deployment for tracking
                Runtime.Log($"Contract deployed at: {deployedHash}");

                // Now call a method on the newly deployed contract
                return Contract.Call(deployedHash, "initialize", CallFlags.All);
            }
            catch (Exception ex)
            {
                Runtime.Log($"Deploy and call failed: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Multi-Network Scenarios

        [DisplayName("getNetworkSpecificInfo")]
        public static object[] GetNetworkSpecificInfo()
        {
            var results = new object[6];
            
            // Get current network for all contracts
            results[0] = NeoContract?.NetworkContext?.CurrentNetwork ?? "unknown";
            results[1] = CustomTokenContract?.NetworkContext?.CurrentNetwork ?? "unknown";
            results[2] = OracleContract?.NetworkContext?.CurrentNetwork ?? "unknown";
            
            // Check which contracts are available on current network
            results[3] = NeoContract?.IsResolved ?? false;
            results[4] = CustomTokenContract?.IsResolved ?? false;
            results[5] = OracleContract?.IsResolved ?? false;
            
            return results;
        }

        [DisplayName("switchNetwork")]
        public static bool SwitchNetwork(string newNetwork)
        {
            try
            {
                // Validate network name
                if (newNetwork != "privnet" && newNetwork != "testnet" && newNetwork != "mainnet")
                    throw new ArgumentException("Invalid network name");

                // Switch all contracts to new network
                ContractInvocationFactory.SwitchNetwork(newNetwork);
                
                // Log which contracts are available on new network
                Runtime.Log($"Switched to {newNetwork}");
                Runtime.Log($"NEO available: {NeoContract?.IsResolved ?? false}");
                Runtime.Log($"CustomToken available: {CustomTokenContract?.IsResolved ?? false}");
                Runtime.Log($"Oracle available: {OracleContract?.IsResolved ?? false}");
                
                return true;
            }
            catch (Exception ex)
            {
                Runtime.Log($"Network switch failed: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Contract Proxy Pattern

        [DisplayName("transferToken")]
        public static bool TransferToken(string tokenId, UInt160 from, UInt160 to, BigInteger amount, object? data)
        {
            // Create a proxy for the token contract
            var tokenContract = SelectContract(tokenId);
            
            if (!tokenContract.IsResolved)
                throw new ContractNotResolvedException(tokenId, "Token contract not available");

            // Use a generic NEP-17 proxy pattern
            var proxy = new Nep17Proxy(tokenContract);
            return proxy.Transfer(from, to, amount, data);
        }

        [DisplayName("getTokenInfo")]
        public static object[] GetTokenInfo(string tokenId)
        {
            var contract = SelectContract(tokenId);
            
            if (!contract.IsResolved)
                return new object[] { tokenId, "Not deployed", 0, "" };

            var proxy = new Nep17Proxy(contract);
            return new object[]
            {
                tokenId,
                proxy.Symbol(),
                proxy.Decimals(),
                proxy.TotalSupply()
            };
        }

        #endregion

        #region Fallback and Error Handling

        [DisplayName("safeContractCall")]
        public static object SafeContractCall(string contractId, string method, object[] args)
        {
            try
            {
                var contract = SelectContract(contractId);
                
                // Handle unresolved contracts gracefully
                if (!contract.IsResolved)
                {
                    Runtime.Log($"Contract {contractId} not resolved, checking alternatives");
                    
                    // Try to find an alternative or fallback
                    if (TryFindAlternativeContract(contractId, out var alternative))
                    {
                        contract = alternative;
                    }
                    else
                    {
                        return HandleUnresolvedContract(contractId, method);
                    }
                }

                // Make the call with comprehensive error handling
                return Contract.Call(contract.ResolvedHash!, method, CallFlags.All, args);
            }
            catch (Exception ex)
            {
                Runtime.Log($"Unexpected error calling {contractId}.{method}: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private static IContractReference SelectContract(string contractId)
        {
            return contractId switch
            {
                "NEO" => NeoContract ?? throw new ContractNotResolvedException("NEO"),
                "GAS" => GasContract ?? throw new ContractNotResolvedException("GAS"),
                "CUSTOM" => CustomTokenContract ?? throw new ContractNotResolvedException("CustomToken"),
                "DEV" => DevelopmentContract ?? throw new ContractNotResolvedException("MyDevelopmentContract"),
                "GOV" => GovernanceContract ?? throw new ContractNotResolvedException("MyGovernanceContract"),
                "ORACLE" => OracleContract ?? throw new ContractNotResolvedException("PriceOracle"),
                _ => throw new ArgumentException($"Unknown contract ID: {contractId}")
            };
        }

        private static bool TryFindAlternativeContract(string contractId, out IContractReference alternative)
        {
            alternative = null!;
            
            // Example: If oracle is not available on current network, try a fallback
            if (contractId == "ORACLE" && OracleContract != null && !OracleContract.IsResolved)
            {
                // Check if we're on privnet and can use a development oracle
                if (OracleContract.NetworkContext?.CurrentNetwork == "privnet")
                {
                    // Could return a development oracle contract here
                    return false;
                }
            }
            
            return false;
        }

        private static object HandleUnresolvedContract(string contractId, string method)
        {
            // Return appropriate default values based on contract and method
            if (method == "balanceOf") return 0;
            if (method == "symbol") return "N/A";
            if (method == "decimals") return 0;
            if (method == "totalSupply") return 0;
            
            return null!;
        }

        private static object HandleMethodNotFound(string contractId, string method)
        {
            // Log and return appropriate default
            Runtime.Log($"Method {method} not supported by {contractId}");
            return null!;
        }

        #endregion

        #region Contract Proxy Implementation

        // Simple NEP-17 proxy for demonstration
        private class Nep17Proxy : ContractProxyBase
        {
            public Nep17Proxy(IContractReference contractRef) : base(contractRef)
            {
            }

            [ContractMethod("symbol", CallFlags.ReadOnly)]
            public string Symbol()
            {
                return (string)InvokeReadOnly("symbol");
            }

            [ContractMethod("decimals", CallFlags.ReadOnly)]
            public byte Decimals()
            {
                return (byte)InvokeReadOnly("decimals");
            }

            [ContractMethod("totalSupply", CallFlags.ReadOnly)]
            public BigInteger TotalSupply()
            {
                return (BigInteger)InvokeReadOnly("totalSupply");
            }

            [ContractMethod("balanceOf", CallFlags.ReadOnly)]
            public BigInteger BalanceOf(UInt160 account)
            {
                return (BigInteger)InvokeReadOnly("balanceOf", account);
            }

            [ContractMethod("transfer", CallFlags.All)]
            public bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data)
            {
                return (bool)Contract.Call(ContractReference.ResolvedHash!, "transfer", CallFlags.All, from, to, amount, data);
            }
        }

        #endregion
    }
}