// Simple example demonstrating contract invocation system
// This shows the basic concept of referencing other contracts

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.ContractInvocation;
using Neo.SmartContract.Framework.ContractInvocation.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Examples.ContractInvocation
{
    [DisplayName("SimpleExample")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Description", "Simple contract invocation example")]
    public class SimpleExample : SmartContract
    {
        #region Contract References

        // Reference to NEO token contract (deployed)
        [ContractReference("NEO",
            PrivnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
            TestnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
            MainnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
        private static IContractReference? NeoContract;

        // Reference to GAS token contract (deployed)
        [ContractReference("GAS",
            PrivnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
            TestnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
            MainnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf")]
        private static IContractReference? GasContract;

        #endregion

        [DisplayName("getTokenInfo")]
        public static object[] GetTokenInfo()
        {
            // This demonstrates the basic concept - in practice, the compiler
            // would resolve these contract references to proper Contract.Call instructions

            // Get NEO token information
            var neoHash = NeoContract?.ResolvedHash;
            var gasHash = GasContract?.ResolvedHash;

            // Return basic information about the referenced contracts
            return new object[] 
            { 
                NeoContract?.Identifier ?? "NEO", 
                neoHash ?? UInt160.Zero,
                GasContract?.Identifier ?? "GAS",
                gasHash ?? UInt160.Zero
            };
        }

        [DisplayName("checkContracts")]
        public static bool CheckContracts()
        {
            // Check if contracts are resolved
            return (NeoContract?.IsResolved ?? false) && (GasContract?.IsResolved ?? false);
        }
    }
}