using Neo;
using Neo.SmartContract.Deploy;
using Microsoft.Extensions.Configuration;

namespace NeoContractSolution.Deploy;

/// <summary>
/// Neo Smart Contract Deployment Program
/// 
/// This is YOUR deployment program where you write explicit C# code to:
/// - Compile contracts from source
/// - Deploy contracts in specific order
/// - Initialize contracts with parameters
/// - Set up contract dependencies
/// - Update existing contracts
/// 
/// Modify this file to implement your specific deployment logic.
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

            // Select network based on configuration or override
            if (args.Length > 0)
            {
                toolkit.SetNetwork(args[0]); // e.g., "mainnet", "testnet", "local"
            }

            Console.WriteLine("Starting Neo Smart Contract Deployment");
            Console.WriteLine();
            
            // ========================================
            // MODIFY BELOW: Your deployment logic here
            // ========================================
            
            // Example 1: Deploy a single contract
            await DeploySingleContract(toolkit);
            
            // Example 2: Deploy multiple contracts with dependencies
            // await DeployMultipleContracts(toolkit);
            
            // Example 3: Update existing contract
            // await UpdateExistingContract(toolkit);
            
            // Example 4: Deploy from manifest file
            // await DeployFromManifest(toolkit);

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
    /// Example: Deploy a single contract
    /// </summary>
    static async Task DeploySingleContract(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying MyContract ===");
        
        // Deploy the contract - simple one-liner!
        var deploymentResult = await toolkit.Deploy("../../src/MyContract/MyContract.csproj");
        
        Console.WriteLine($"Contract deployed!");
        Console.WriteLine($"  Contract Hash: {deploymentResult.ContractHash}");
        Console.WriteLine($"  Transaction: {deploymentResult.TransactionHash}");
        Console.WriteLine($"  Block: {deploymentResult.BlockIndex}");
        
        // Initialize the contract with parameters
        var txHash = await toolkit.Invoke(
            deploymentResult.ContractHash,
            "initialize",
            "Production v1.0"
        );

        Console.WriteLine($"Contract initialized! Transaction: {txHash}");
    }

    /// <summary>
    /// Example: Deploy multiple contracts with dependencies
    /// </summary>
    static async Task DeployMultipleContracts(DeploymentToolkit toolkit)
    {
        // Deploy Token Contract first
        Console.WriteLine("=== Deploying Token Contract ===");
        var tokenResult = await toolkit.Deploy("../../src/TokenContract/TokenContract.csproj");
        Console.WriteLine($"Token Contract: {tokenResult.ContractHash}");
        
        // Initialize token with parameters
        await toolkit.Invoke(
            tokenResult.ContractHash,
            "initialize",
            "MyToken", "MTK", 8, 1000000_00000000L
        );

        // Deploy Vault Contract that depends on Token
        Console.WriteLine("=== Deploying Vault Contract ===");
        var vaultResult = await toolkit.Deploy("../../src/VaultContract/VaultContract.csproj");
        Console.WriteLine($"Vault Contract: {vaultResult.ContractHash}");
        
        // Initialize vault with token dependency
        await toolkit.Invoke(
            vaultResult.ContractHash,
            "initialize",
            tokenResult.ContractHash,  // Pass token contract hash
            1000,                      // Min deposit amount
            86400                      // Lock period (1 day)
        );

        // Configure token to trust vault
        await toolkit.Invoke(
            tokenResult.ContractHash,
            "addTrustedContract",
            vaultResult.ContractHash
        );

        Console.WriteLine();
        Console.WriteLine("Multi-contract deployment complete!");
        Console.WriteLine($"Token Contract: {tokenResult.ContractHash}");
        Console.WriteLine($"Vault Contract: {vaultResult.ContractHash}");
    }

    /// <summary>
    /// Example: Update an existing contract
    /// </summary>
    static async Task UpdateExistingContract(DeploymentToolkit toolkit)
    {
        // The contract hash of the deployed contract you want to update
        var existingContractHash = UInt160.Parse("0x1234567890abcdef1234567890abcdef12345678");
        
        Console.WriteLine($"=== Updating Contract {existingContractHash} ===");
        
        // Check if contract exists
        if (!await toolkit.ContractExists(existingContractHash))
        {
            throw new Exception("Contract not found on network");
        }

        // Update the contract with new version
        var updateResult = await toolkit.Update(
            existingContractHash,
            "../../src/MyContract/MyContract.csproj" // Updated project
        );

        Console.WriteLine($"Contract updated!");
        Console.WriteLine($"  Transaction: {updateResult.TransactionHash}");
        Console.WriteLine($"  Block: {updateResult.BlockIndex}");
        
        // Run any post-update migration if needed
        await toolkit.Invoke(
            existingContractHash,
            "migrate",
            "v2.0"
        );
        
        Console.WriteLine("Migration completed!");
    }

    /// <summary>
    /// Example: Deploy from manifest file
    /// </summary>
    static async Task DeployFromManifest(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying from Manifest ===");
        
        // Deploy all contracts defined in the manifest
        var results = await toolkit.DeployFromManifest("deployment-manifest.json");
        
        Console.WriteLine();
        Console.WriteLine("Deployment Summary:");
        foreach (var result in results)
        {
            Console.WriteLine($"  {result.Key}: {result.Value.ContractHash}");
        }
    }
}