using Neo;
using Neo.SmartContract.Deploy;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Numerics;

namespace DeploymentExample.Deploy;

/// <summary>
/// Neo Smart Contract Deployment Example
/// 
/// This example demonstrates how to deploy and interact with contracts
/// using the simplified deployment toolkit API.
/// 
/// Usage:
///   dotnet run                    -- Deploy single example contract
///   dotnet run multi              -- Deploy multiple interrelated contracts
///   dotnet run test <addresses>   -- Test deployed contracts
///   dotnet run manifest           -- Deploy from manifest file
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

            // Parse command line arguments
            var command = args.Length > 0 ? args[0].ToLower() : "single";
            var network = args.Length > 1 ? args[1] : "local";
            
            // Set network
            toolkit.SetNetwork(network);

            Console.WriteLine("=== NEO Smart Contract Deployment Example ===");
            Console.WriteLine($"Network: {network}");
            Console.WriteLine($"Mode: {command}");
            Console.WriteLine();

            switch (command)
            {
                case "single":
                    // Deploy single example contract
                    await DeployExampleContract(toolkit);
                    break;
                    
                case "multi":
                    // Deploy multiple contracts with dependencies
                    var multiDeployer = new MultiContractDeployer(toolkit);
                    var results = await multiDeployer.DeployAllContracts();
                    
                    // Test the deployed contracts
                    var tester = new MultiContractTester(toolkit);
                    await tester.RunAllTests(results);
                    break;
                    
                case "manifest":
                    // Deploy from manifest file
                    var manifestDeployer = new MultiContractDeployer(toolkit);
                    await manifestDeployer.DeployFromManifest("deployment-manifest.json");
                    break;
                    
                case "test":
                    // Test existing contracts
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Usage: dotnet run test <token_hash> <nft_hash> <gov_hash>");
                        return 1;
                    }
                    
                    var testResults = new DeploymentResults
                    {
                        TokenContract = new Neo.SmartContract.Deploy.Models.ContractDeploymentInfo 
                        { 
                            ContractHash = UInt160.Parse(args[1]) 
                        },
                        NFTContract = new Neo.SmartContract.Deploy.Models.ContractDeploymentInfo 
                        { 
                            ContractHash = UInt160.Parse(args[2]) 
                        },
                        GovernanceContract = new Neo.SmartContract.Deploy.Models.ContractDeploymentInfo 
                        { 
                            ContractHash = UInt160.Parse(args[3]) 
                        }
                    };
                    
                    var contractTester = new MultiContractTester(toolkit);
                    await contractTester.RunAllTests(testResults);
                    break;
                    
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Available commands: single, multi, manifest, test");
                    return 1;
            }
            
            Console.WriteLine();
            Console.WriteLine("Operation completed successfully!");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Operation failed: {ex.Message}");
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
            new object[] { deployerAddress } // Pass deployer as owner during deployment
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
        var info = await toolkit.Call<object>(contractHash.ToString(), "getInfo");
        Console.WriteLine($"   Contract Info: {info}");
        
        // 2. Get current counter value
        Console.WriteLine("\n2. Getting counter value...");
        var counter = await toolkit.Call<BigInteger>(
            contractHash.ToString(), 
            "getCounter"
        );
        Console.WriteLine($"   Current Counter: {counter}");
        
        // 3. Increment counter (state-changing transaction)
        Console.WriteLine("\n3. Incrementing counter...");
        var txHash = await toolkit.Invoke(contractHash.ToString(), "increment");
        Console.WriteLine($"   Transaction: {txHash}");
        
        // Wait a moment for the transaction to be processed
        Console.WriteLine("   Waiting for transaction confirmation...");
        await Task.Delay(5000);
        
        // Get updated counter value
        var newCounter = await toolkit.Call<BigInteger>(
            contractHash.ToString(),
            "getCounter"
        );
        Console.WriteLine($"   New Counter: {newCounter}");
        
        // 4. Test multiply function
        Console.WriteLine("\n4. Testing multiply function...");
        var result = await toolkit.Call<BigInteger>(
            contractHash.ToString(),
            "multiply",
            7, 6
        );
        Console.WriteLine($"   7 × 6 = {result}");
        
        // 5. Get owner
        Console.WriteLine("\n5. Getting contract owner...");
        var owner = await toolkit.Call<string>(contractHash.ToString(), "getOwner");
        Console.WriteLine($"   Owner: {owner}");
        
        Console.WriteLine("\n=== All tests completed successfully! ===");
    }
}