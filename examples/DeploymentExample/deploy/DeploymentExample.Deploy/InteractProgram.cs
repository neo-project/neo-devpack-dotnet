using System;
using System.Numerics;
using System.Threading.Tasks;
using Neo;
using Neo.SmartContract.Deploy;

namespace DeploymentExample.Deploy
{
    class InteractProgram
    {
        public static async Task RunInteraction()
        {
            // The deployed contract hash from our recent deployment
            var contractHash = "0x82eebea79d89df1e5b856d60a349a2beb678a4c9"; // SimpleContract deployed to testnet
            var wifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";

            // Create toolkit
            var toolkit = new DeploymentToolkit();
            toolkit.SetNetwork("testnet");
            toolkit.SetWifKey(wifKey);

            Console.WriteLine($"=== Interacting with Deployed Contract ===");
            Console.WriteLine($"Contract: {contractHash}");
            Console.WriteLine($"Account: NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX");
            Console.WriteLine();

            try
            {
                // 1. Get current counter
                Console.WriteLine("1. Getting current counter value...");
                var counter = await toolkit.CallAsync<BigInteger>(contractHash, "getCounter");
                Console.WriteLine($"   Current Counter: {counter}");

                // 2. Test multiply function
                Console.WriteLine("\n2. Testing multiply function...");
                var result = await toolkit.CallAsync<BigInteger>(contractHash, "multiply", 9, 8);
                Console.WriteLine($"   9 × 8 = {result}");

                // 3. Increment counter
                Console.WriteLine("\n3. Incrementing counter...");
                Console.WriteLine("   Sending transaction...");
                var txHash = await toolkit.InvokeAsync(contractHash, "increment");
                Console.WriteLine($"   Transaction sent: {txHash}");
                Console.WriteLine($"   View on explorer: https://testnet.neotube.io/transaction/{txHash}");
                
                // 4. Wait and check new value
                Console.WriteLine("\n4. Waiting 20 seconds for confirmation...");
                await Task.Delay(20000);
                
                var newCounter = await toolkit.CallAsync<BigInteger>(contractHash, "getCounter");
                Console.WriteLine($"   New Counter Value: {newCounter}");
                
                if (newCounter > counter)
                {
                    Console.WriteLine($"\n✅ Success! Counter incremented from {counter} to {newCounter}!");
                }
                else
                {
                    Console.WriteLine($"\n⏳ Counter not yet updated, transaction may need more time");
                }
                
                Console.WriteLine("\n=== Contract Information ===");
                Console.WriteLine($"Contract Hash: {contractHash}");
                Console.WriteLine($"TestNet Explorer: https://testnet.neotube.io/contract/{contractHash}");
                Console.WriteLine($"RPC Endpoint: https://testnet1.neo.coz.io:443");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}