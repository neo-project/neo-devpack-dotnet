using System;
using System.Threading.Tasks;
using Neo;
using Neo.SmartContract.Deploy;

namespace DeploymentExample.Deploy
{
    class CheckDeployment
    {
        public static async Task CheckContractStatus()
        {
            var toolkit = new DeploymentToolkit();
            toolkit.SetNetwork("testnet");
            
            // Our deployment transaction
            var txHash = "0x9797f5e48ce40a52faf0feb42b8ddfa837643e7c87f14e6fde7ec67c92985758";
            var expectedContractHash = "0x3f4e8cdd19a9f07e6eab4e3f3ba387e4faa16a77";
            
            Console.WriteLine("=== Checking Deployment Status ===");
            Console.WriteLine($"Transaction: {txHash}");
            Console.WriteLine($"Expected Contract: {expectedContractHash}");
            Console.WriteLine();
            
            // Check if contract exists
            Console.WriteLine("Checking if contract exists...");
            try
            {
                var exists = await toolkit.ContractExistsAsync(expectedContractHash);
                if (exists)
                {
                    Console.WriteLine("✅ Contract found on blockchain!");
                }
                else
                {
                    Console.WriteLine("❌ Contract not found yet. Transaction may still be processing.");
                    Console.WriteLine("   Please wait a few more blocks and try again.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking contract: {ex.Message}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Transaction Explorer Links:");
            Console.WriteLine($"- NeoTube: https://testnet.neotube.io/transaction/{txHash}");
            Console.WriteLine($"- NeoTracker: https://testnet.neotracker.io/tx/{txHash}");
        }
    }
}