using Neo.SmartContract.Deploy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeploymentExample.Deploy
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== NEO Smart Contract Deployment Example ===");
            Console.WriteLine();

            try
            {
                // Create deployment toolkit instance
                // This automatically loads configuration from appsettings.json
                var toolkit = new DeploymentToolkit();

                // Option 1: Use environment variable for network selection
                var network = Environment.GetEnvironmentVariable("NEO_NETWORK") ?? "local";
                Console.WriteLine($"Target network: {network}");

                // Set the network (mainnet, testnet, local, or custom RPC URL)
                toolkit.SetNetwork(network);

                // Option 2: Interactive network selection
                if (args.Length == 0)
                {
                    Console.WriteLine("\nSelect deployment network:");
                    Console.WriteLine("1. Local (Neo Express)");
                    Console.WriteLine("2. TestNet");
                    Console.WriteLine("3. MainNet");
                    Console.Write("\nEnter your choice (1-3): ");
                    
                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            toolkit.SetNetwork("local");
                            break;
                        case "2":
                            toolkit.SetNetwork("testnet");
                            break;
                        case "3":
                            toolkit.SetNetwork("mainnet");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Using local network.");
                            toolkit.SetNetwork("local");
                            break;
                    }
                }

                // Get deployer account information
                Console.WriteLine("\nDeployer Account Information:");
                var deployerAddress = await toolkit.GetDeployerAccount();
                Console.WriteLine($"Address: {deployerAddress}");
                
                var gasBalance = await toolkit.GetGasBalance();
                Console.WriteLine($"GAS Balance: {gasBalance} GAS");

                if (gasBalance == 0)
                {
                    Console.WriteLine("\nWARNING: Deployer account has no GAS!");
                    Console.WriteLine("Please fund the account before deployment.");
                    return;
                }

                // Deploy the contract
                Console.WriteLine("\n=== Deploying Contract ===");
                Console.WriteLine("Contract: ExampleContract");
                Console.WriteLine("Initializing with deployer as owner...");

                // Deploy with initialization parameters
                // The contract's _deploy method expects the owner address
                var deploymentInfo = await toolkit.Deploy(
                    "../../src/DeploymentExample.Contract/DeploymentExample.Contract.csproj",
                    new object[] { deployerAddress } // Pass deployer as initial owner
                );

                Console.WriteLine("\n=== Deployment Successful! ===");
                Console.WriteLine($"Contract Hash: {deploymentInfo.ContractHash}");
                Console.WriteLine($"Transaction Hash: {deploymentInfo.TransactionHash}");
                Console.WriteLine($"GAS Consumed: {deploymentInfo.GasConsumed} GAS");
                Console.WriteLine($"Block Index: {deploymentInfo.BlockIndex}");

                // Demonstrate contract invocation
                Console.WriteLine("\n=== Testing Contract Methods ===");
                
                // Get contract info
                Console.WriteLine("\n1. Getting contract info...");
                var info = await toolkit.Call<System.Collections.Generic.Dictionary<string, object>>(
                    deploymentInfo.ContractHash.ToString(),
                    "getInfo"
                );
                Console.WriteLine($"Contract Info: {string.Join(", ", info.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");

                // Get current counter value
                Console.WriteLine("\n2. Getting counter value...");
                var counter = await toolkit.Call<System.Numerics.BigInteger>(
                    deploymentInfo.ContractHash.ToString(),
                    "getCounter"
                );
                Console.WriteLine($"Current Counter: {counter}");

                // Increment counter (this will consume GAS)
                Console.WriteLine("\n3. Incrementing counter...");
                var incrementTx = await toolkit.Invoke(
                    deploymentInfo.ContractHash.ToString(),
                    "increment"
                );
                Console.WriteLine($"Transaction sent: {incrementTx}");
                
                // Get updated counter value
                await Task.Delay(2000); // Wait for transaction to process
                var newCounter = await toolkit.Call<System.Numerics.BigInteger>(
                    deploymentInfo.ContractHash.ToString(),
                    "getCounter"
                );
                Console.WriteLine($"New Counter Value: {newCounter}");

                // Multiply example
                Console.WriteLine("\n4. Testing multiply function...");
                var multiplyResult = await toolkit.Call<System.Numerics.BigInteger>(
                    deploymentInfo.ContractHash.ToString(),
                    "multiply",
                    7, 6
                );
                Console.WriteLine($"7 * 6 = {multiplyResult}");

                Console.WriteLine("\n=== Deployment Example Complete! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine("\nTroubleshooting tips:");
                Console.WriteLine("1. Ensure Neo Express is running (for local deployment)");
                Console.WriteLine("2. Check that wallet.json exists and password is correct");
                Console.WriteLine("3. Verify the deployer account has sufficient GAS");
                Console.WriteLine("4. Check network connectivity for testnet/mainnet");
            }
        }
    }
}