using Neo;
using Neo.SmartContract.Deploy;
using Microsoft.Extensions.Configuration;

namespace DeploymentExample.Deploy;

/// <summary>
/// Neo Smart Contract Deployment Example
/// 
/// This example demonstrates how to deploy and interact with the ExampleContract
/// using the simplified deployment toolkit API.
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            // Create the deployment toolkit with simplified API
            // Configuration is loaded automatically from appsettings.json
            var toolkit = new DeploymentToolkit();

            // Select network based on command line argument or default
            if (args.Length > 0)
            {
                toolkit.SetNetwork(args[0]); // e.g., "mainnet", "testnet", "local"
            }

            Console.WriteLine("=== NEO Smart Contract Deployment Example ===");
            Console.WriteLine();
            
            // Deploy the example contract
            await DeployExampleContract(toolkit);
            
            Console.WriteLine();
            Console.WriteLine("Deployment completed successfully!");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Deployment failed: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            return 1;
        }
    }

    /// <summary>
    /// Deploy and test the example contract
    /// </summary>
    static async Task DeployExampleContract(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying ExampleContract ===");
        
        // Get deployer account for initialization
        var deployerAddress = await toolkit.GetDeployerAccount();
        Console.WriteLine($"Deployer: {deployerAddress}");
        
        // Check GAS balance
        var gasBalance = await toolkit.GetGasBalance();
        Console.WriteLine($"GAS Balance: {gasBalance}");
        
        if (gasBalance == 0)
        {
            throw new InvalidOperationException(
                "Insufficient GAS balance. Please fund the deployer account."
            );
        }
        
        // Deploy the contract with the deployer as initial owner
        var deploymentResult = await toolkit.Deploy(
            "../../src/DeploymentExample.Contract/DeploymentExample.Contract.csproj",
            deployerAddress // Pass deployer as owner during deployment
        );
        
        Console.WriteLine($"Contract deployed!");
        Console.WriteLine($"  Contract Hash: {deploymentResult.ContractHash}");
        Console.WriteLine($"  Transaction: {deploymentResult.TransactionHash}");
        Console.WriteLine($"  Block: {deploymentResult.BlockIndex}");
        Console.WriteLine($"  GAS Consumed: {deploymentResult.GasConsumed / 100_000_000m} GAS");
        
        // Test contract methods
        await TestContractMethods(toolkit, deploymentResult.ContractHash);
    }

    /// <summary>
    /// Test the deployed contract's methods
    /// </summary>
    static async Task TestContractMethods(DeploymentToolkit toolkit, UInt160 contractHash)
    {
        Console.WriteLine();
        Console.WriteLine("=== Testing Contract Methods ===");
        
        // 1. Get contract info (read-only call)
        Console.WriteLine("\n1. Getting contract info...");
        var info = await toolkit.Call<string>(contractHash, "getInfo");
        Console.WriteLine($"   Contract Info: {info}");
        
        // 2. Get current counter value
        Console.WriteLine("\n2. Getting counter value...");
        var counter = await toolkit.Call<System.Numerics.BigInteger>(
            contractHash, 
            "getCounter"
        );
        Console.WriteLine($"   Current Counter: {counter}");
        
        // 3. Increment counter (state-changing transaction)
        Console.WriteLine("\n3. Incrementing counter...");
        var txHash = await toolkit.Invoke(contractHash, "increment");
        Console.WriteLine($"   Transaction: {txHash}");
        
        // Wait a moment for the transaction to be processed
        Console.WriteLine("   Waiting for transaction confirmation...");
        await Task.Delay(5000);
        
        // Get updated counter value
        var newCounter = await toolkit.Call<System.Numerics.BigInteger>(
            contractHash,
            "getCounter"
        );
        Console.WriteLine($"   New Counter: {newCounter}");
        
        // 4. Test multiply function
        Console.WriteLine("\n4. Testing multiply function...");
        var result = await toolkit.Call<System.Numerics.BigInteger>(
            contractHash,
            "multiply",
            7, 6
        );
        Console.WriteLine($"   7 × 6 = {result}");
        
        // 5. Get owner
        Console.WriteLine("\n5. Getting contract owner...");
        var owner = await toolkit.Call<string>(contractHash, "getOwner");
        Console.WriteLine($"   Owner: {owner}");
        
        Console.WriteLine("\n=== All tests completed successfully! ===");
    }
}