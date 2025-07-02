using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Contracts;
using Microsoft.Extensions.Logging;

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
            // Create the deployment toolkit
            var toolkit = NeoContractToolkitBuilder.Create()
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            // Load wallet from configuration
            await toolkit.LoadWalletFromConfigurationAsync();
            var deployerAccount = toolkit.GetDeployerAccount();

            Console.WriteLine("Starting Neo Smart Contract Deployment");
            Console.WriteLine($"Deployer Account: {deployerAccount}");
            Console.WriteLine();
            
            // ========================================
            // MODIFY BELOW: Your deployment logic here
            // ========================================
            
            // Example 1: Deploy a single contract
            await DeploySingleContract(toolkit, deployerAccount);
            
            // Example 2: Deploy multiple contracts with dependencies
            // await DeployMultipleContracts(toolkit, deployerAccount);
            
            // Example 3: Update existing contract
            // await UpdateExistingContract(toolkit, deployerAccount);

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
    static async Task DeploySingleContract(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        Console.WriteLine("=== Deploying MyContract ===");
        
        // Step 1: Define compilation options
        var compilationOptions = new CompilationOptions
        {
            SourcePath = "../../src/MyContract/MyContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "MyContract",
            GenerateDebugInfo = true,
            Optimize = true
        };

        // Step 2: Define deployment options (network settings from configuration)
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = deployerAccount,
            GasLimit = 50_000_000 // 0.5 GAS
        };

        // Step 3: Compile and deploy
        var deploymentResult = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
        
        Console.WriteLine($"Contract deployed!");
        Console.WriteLine($"  Contract Hash: {deploymentResult.ContractHash}");
        Console.WriteLine($"  Transaction: {deploymentResult.TransactionHash}");
        Console.WriteLine($"  Block: {deploymentResult.BlockIndex}");
        
        // Step 4: Initialize the contract (network settings from configuration)
        var invocationOptions = new InvocationOptions
        {
            SignerAccount = deployerAccount,
            GasLimit = 20_000_000 // 0.2 GAS
        };

        var initResult = await toolkit.InvokeContractAsync(
            deploymentResult.ContractHash,
            "initialize",
            new object[] { "Production v1.0" },
            invocationOptions
        );

        if (initResult.State == Neo.VM.VMState.HALT)
        {
            Console.WriteLine("Contract initialized successfully!");
        }
        else
        {
            throw new Exception($"Contract initialization failed: {initResult.Exception}");
        }
    }

    /// <summary>
    /// Example: Deploy multiple contracts with dependencies
    /// </summary>
    static async Task DeployMultipleContracts(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = deployerAccount,
            GasLimit = 100_000_000 // 1 GAS per contract
        };

        var invocationOptions = new InvocationOptions
        {
            SignerAccount = deployerAccount,
            GasLimit = 20_000_000 // 0.2 GAS
        };

        // Deploy Token Contract first
        Console.WriteLine("=== Deploying Token Contract ===");
        var tokenCompilation = new CompilationOptions
        {
            SourcePath = "../../src/TokenContract/TokenContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "TokenContract"
        };
        
        var tokenResult = await toolkit.CompileAndDeployAsync(tokenCompilation, deploymentOptions);
        Console.WriteLine($"Token Contract: {tokenResult.ContractHash}");
        
        // Initialize token
        await toolkit.InvokeContractAsync(
            tokenResult.ContractHash,
            "initialize",
            new object[] { "MyToken", "MTK", 8, 1000000_00000000L },
            invocationOptions
        );

        // Deploy Vault Contract that depends on Token
        Console.WriteLine("=== Deploying Vault Contract ===");
        var vaultCompilation = new CompilationOptions
        {
            SourcePath = "../../src/VaultContract/VaultContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "VaultContract"
        };
        
        var vaultResult = await toolkit.CompileAndDeployAsync(vaultCompilation, deploymentOptions);
        Console.WriteLine($"Vault Contract: {vaultResult.ContractHash}");
        
        // Initialize vault with token dependency
        await toolkit.InvokeContractAsync(
            vaultResult.ContractHash,
            "initialize",
            new object[] { 
                tokenResult.ContractHash,  // Pass token contract hash
                1000,                      // Min deposit amount
                86400                      // Lock period (1 day)
            },
            invocationOptions
        );

        // Configure token to trust vault
        await toolkit.InvokeContractAsync(
            tokenResult.ContractHash,
            "addTrustedContract",
            new object[] { vaultResult.ContractHash },
            invocationOptions
        );

        Console.WriteLine();
        Console.WriteLine("Multi-contract deployment complete!");
        Console.WriteLine($"Token Contract: {tokenResult.ContractHash}");
        Console.WriteLine($"Vault Contract: {vaultResult.ContractHash}");
    }

    /// <summary>
    /// Example: Update an existing contract
    /// </summary>
    static async Task UpdateExistingContract(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        // The contract hash of the deployed contract you want to update
        var existingContractHash = UInt160.Parse("0x1234567890abcdef1234567890abcdef12345678");
        
        Console.WriteLine($"=== Updating Contract {existingContractHash} ===");
        
        // Check if contract exists
        if (!await toolkit.ContractExistsAsync(existingContractHash))
        {
            throw new Exception("Contract not found on network");
        }

        // New version compilation options
        var compilationOptions = new CompilationOptions
        {
            SourcePath = "../../src/MyContract/MyContract.cs", // Updated source
            OutputDirectory = "bin/contracts",
            ContractName = "MyContract",
            GenerateDebugInfo = true,
            Optimize = true
        };

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = deployerAccount,
            GasLimit = 60_000_000 // 0.6 GAS for updates
        };

        // Update the contract
        var updateResult = await toolkit.UpdateContractAsync(
            existingContractHash,
            compilationOptions,
            deploymentOptions
        );

        Console.WriteLine($"Contract updated!");
        Console.WriteLine($"  Transaction: {updateResult.TransactionHash}");
        Console.WriteLine($"  Block: {updateResult.BlockIndex}");
        
        // Run any post-update migration if needed
        var invocationOptions = new InvocationOptions
        {
            SignerAccount = deployerAccount,
            GasLimit = 20_000_000
        };

        await toolkit.InvokeContractAsync(
            existingContractHash,
            "migrate",
            new object[] { "v2.0" },
            invocationOptions
        );
        
        Console.WriteLine("Migration completed!");
    }
}