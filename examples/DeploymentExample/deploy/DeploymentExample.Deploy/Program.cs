using Neo;
using Neo.SmartContract.Deploy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
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
///   dotnet run test token_hash nft_hash gov_hash -- Test deployed contracts
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
            var network = args.Length > 1 ? args[1] : "testnet";
            
            // Set network and WIF key for testnet deployment
            toolkit.SetNetwork(network);
            
            // Use WIF key for signing if provided
            var wifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
            toolkit.SetWifKey(wifKey);

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
                    await tester.TestDeployedContracts(results);
                    break;
                    
                case "manifest":
                    // Deploy from manifest file
                    var manifestResults = await toolkit.DeployFromManifestAsync("deployment-manifest.json");
                    Console.WriteLine($"Deployed {manifestResults.Count} contracts from manifest");
                    foreach (var kvp in manifestResults)
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value.ContractHash}");
                    }
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
                        Success = true,
                        TokenContract = UInt160.Parse(args[1]),
                        NFTContract = UInt160.Parse(args[2]),
                        GovernanceContract = UInt160.Parse(args[3])
                    };
                    
                    var contractTester = new MultiContractTester(toolkit);
                    await contractTester.TestDeployedContracts(testResults);
                    break;
                    
                case "interact":
                    // Interact with the deployed contract
                    await InteractProgram.RunInteraction();
                    break;
                    
                case "check":
                    // Check deployment status
                    await CheckDeployment.CheckContractStatus();
                    break;
                    
                case "debug":
                    // Debug deployment with minimal contract
                    await DebugDeploy.DebugDeployment();
                    break;
                    
                case "update":
                    // Update deployed contracts
                    using (var updateLoggerFactory = LoggerFactory.Create(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                    }))
                    {
                        var updateConfiguration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false)
                            .AddJsonFile($"appsettings.{network}.json", optional: true)
                            .AddEnvironmentVariables()
                            .Build();
                            
                        var updateLogger = updateLoggerFactory.CreateLogger<UpdateContracts>();
                        var updater = new UpdateContracts(updateConfiguration, updateLogger);
                        await updater.RunAsync();
                    }
                    break;
                    
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Available commands: single, multi, manifest, test, interact, check, debug, update");
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
        Console.WriteLine("=== Deploying SimpleContract ===");
        
        // Get deployer account for initialization
        var deployerAddress = await toolkit.GetDeployerAccountAsync();
        Console.WriteLine($"Deployer: {deployerAddress}");
        
        // Check GAS balance
        try
        {
            var gasBalance = await toolkit.GetGasBalanceAsync();
            Console.WriteLine($"GAS Balance: {gasBalance}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not check GAS balance: {ex.Message}");
            Console.WriteLine("Proceeding with deployment anyway...");
        }
        
        // Deploy the simple contract first to test deployment
        var contractSourcePath = "../../src/DeploymentExample.Contract/SimpleContract.cs";
        Console.WriteLine($"Deploying from: {contractSourcePath}");
        
        var deploymentResult = await toolkit.DeployAsync(contractSourcePath);
        
        Console.WriteLine($"Contract deployed!");
        Console.WriteLine($"  Contract Hash: {deploymentResult.ContractHash}");
        Console.WriteLine($"  Transaction: {deploymentResult.TransactionHash}");
        Console.WriteLine($"  Block: {deploymentResult.BlockIndex}");
        Console.WriteLine($"  GAS Consumed: {deploymentResult.GasConsumed / 100_000_000m} GAS");
        
        if (deploymentResult.Success)
        {
            // Test simple contract methods
            await TestSimpleContractMethods(toolkit, deploymentResult.ContractHash);
        }
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
        var info = await toolkit.CallAsync<object>(contractHash.ToString(), "getInfo");
        Console.WriteLine($"   Contract Info: {info}");
        
        // 2. Get current counter value
        Console.WriteLine("\n2. Getting counter value...");
        var counter = await toolkit.CallAsync<BigInteger>(
            contractHash.ToString(), 
            "getCounter"
        );
        Console.WriteLine($"   Current Counter: {counter}");
        
        // 3. Increment counter (state-changing transaction)
        Console.WriteLine("\n3. Incrementing counter...");
        var txHash = await toolkit.InvokeAsync(contractHash.ToString(), "increment");
        Console.WriteLine($"   Transaction: {txHash}");
        
        // Wait a moment for the transaction to be processed
        Console.WriteLine("   Waiting for transaction confirmation...");
        await Task.Delay(5000);
        
        // Get updated counter value
        var newCounter = await toolkit.CallAsync<BigInteger>(
            contractHash.ToString(),
            "getCounter"
        );
        Console.WriteLine($"   New Counter: {newCounter}");
        
        // 4. Test multiply function
        Console.WriteLine("\n4. Testing multiply function...");
        var result = await toolkit.CallAsync<BigInteger>(
            contractHash.ToString(),
            "multiply",
            7, 6
        );
        Console.WriteLine($"   7 × 6 = {result}");
        
        // 5. Get owner
        Console.WriteLine("\n5. Getting contract owner...");
        var owner = await toolkit.CallAsync<string>(contractHash.ToString(), "getOwner");
        Console.WriteLine($"   Owner: {owner}");
        
        Console.WriteLine("\n=== All tests completed successfully! ===");
    }

    /// <summary>
    /// Test the simple contract's methods
    /// </summary>
    static async Task TestSimpleContractMethods(DeploymentToolkit toolkit, UInt160 contractHash)
    {
        Console.WriteLine();
        Console.WriteLine("=== Testing Simple Contract Methods ===");
        
        // 1. Get current counter value
        Console.WriteLine("\n1. Getting counter value...");
        var counter = await toolkit.CallAsync<BigInteger>(
            contractHash.ToString(), 
            "getCounter"
        );
        Console.WriteLine($"   Current Counter: {counter}");
        
        // 2. Increment counter
        Console.WriteLine("\n2. Incrementing counter...");
        var txHash = await toolkit.InvokeAsync(contractHash.ToString(), "increment");
        Console.WriteLine($"   Transaction: {txHash}");
        
        // Wait for confirmation
        Console.WriteLine("   Waiting for transaction confirmation...");
        await Task.Delay(15000); // Wait 15 seconds for block
        
        // Get updated counter value
        var newCounter = await toolkit.CallAsync<BigInteger>(
            contractHash.ToString(),
            "getCounter"
        );
        Console.WriteLine($"   New Counter: {newCounter}");
        
        // 3. Test multiply function
        Console.WriteLine("\n3. Testing multiply function...");
        var result = await toolkit.CallAsync<BigInteger>(
            contractHash.ToString(),
            "multiply",
            7, 6
        );
        Console.WriteLine($"   7 × 6 = {result}");
        
        Console.WriteLine("\n=== Simple contract tests completed! ===");
    }
}