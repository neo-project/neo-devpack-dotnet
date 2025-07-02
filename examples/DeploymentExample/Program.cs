using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DeploymentExample
{
    /// <summary>
    /// Example program demonstrating complete deployment toolkit functionality
    /// </summary>
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== Neo Smart Contract Deployment Example ===");
                Console.WriteLine();

                // Step 1: Create toolkit with configuration
                var toolkit = NeoContractToolkitBuilder.Create()
                    .ConfigureLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                    })
                    .ConfigureServices(services =>
                    {
                        // Load configuration from appsettings.json
                        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true)
                            .Build();
                        
                        services.AddSingleton<IConfiguration>(configuration);
                    })
                    .Build();

                Console.WriteLine("✓ Toolkit created successfully");

                // Step 2: Load wallet from configuration or create new
                var walletPath = "example.wallet.json";
                if (!File.Exists(walletPath))
                {
                    Console.WriteLine("Creating new wallet...");
                    // In a real scenario, you would create a wallet here
                    // For this example, we'll skip wallet creation
                    Console.WriteLine("! Wallet creation skipped for demo");
                    return 0;
                }

                await toolkit.LoadWalletAsync(walletPath, "password");
                var deployerAccount = toolkit.GetDeployerAccount();
                Console.WriteLine($"✓ Wallet loaded. Deployer: {deployerAccount}");

                // Step 3: Compile a smart contract from project
                Console.WriteLine("\nCompiling smart contract from project...");
                var compilationOptions = new CompilationOptions
                {
                    ProjectPath = "../MyContract/MyContract.csproj",  // Use project-based compilation
                    OutputDirectory = "output",
                    ContractName = "MyContract",
                    GenerateDebugInfo = true,
                    Optimize = true
                };

                var compiler = toolkit.GetService<IContractCompiler>();
                var compilationResult = await compiler.CompileAsync(compilationOptions);
                Console.WriteLine($"✓ Contract compiled successfully");
                Console.WriteLine($"  - NEF size: {compilationResult.NefBytes.Length} bytes");
                Console.WriteLine($"  - Name: {compilationResult.Manifest.Name}");
                Console.WriteLine($"  - Methods: {compilationResult.Manifest.Abi.Methods.Length}");

                // Step 4: Deploy the contract
                Console.WriteLine("\nDeploying contract...");
                var deploymentOptions = new DeploymentOptions
                {
                    DeployerAccount = deployerAccount,
                    GasLimit = 50_000_000,  // 0.5 GAS
                    WaitForConfirmation = true
                };

                var deployer = toolkit.GetService<IContractDeployer>();
                var deploymentResult = await deployer.DeployAsync(compilationResult, deploymentOptions);
                Console.WriteLine($"✓ Contract deployed successfully!");
                Console.WriteLine($"  - Contract Hash: {deploymentResult.ContractHash}");
                Console.WriteLine($"  - Transaction: {deploymentResult.TransactionHash}");
                Console.WriteLine($"  - Gas Consumed: {deploymentResult.GasConsumed}");

                // Step 5: Test invoke contract method (read-only)
                Console.WriteLine("\nTest invoking contract method...");
                var testResult = await toolkit.CallContractAsync<string>(
                    deploymentResult.ContractHash,
                    "testMethod",
                    "World"
                );

                Console.WriteLine($"✓ Method test invoked successfully!");
                Console.WriteLine($"  - Result: {testResult}");

                // Step 6: Invoke contract method (actual transaction)
                Console.WriteLine("\nInvoking contract method (transaction)...");
                var txHash = await toolkit.InvokeContractAsync(
                    deploymentResult.ContractHash,
                    "testMethod",
                    "World"
                );

                Console.WriteLine($"✓ Method invoked successfully!");
                Console.WriteLine($"  - Transaction Hash: {txHash}");

                // Step 7: Multi-contract deployment example
                Console.WriteLine("\n=== Multi-Contract Deployment Example ===");
                var deploymentRequests = new List<ContractDeploymentRequest>
                {
                    new ContractDeploymentRequest
                    {
                        Name = "TokenContract",
                        SourcePath = "../TokenContract/TokenContract.csproj",
                        OutputDirectory = "output/token",
                        Dependencies = new List<string>(),
                        InitialParameters = new List<object> { "MyToken", "MTK", 8, 1000000 }
                    },
                    new ContractDeploymentRequest
                    {
                        Name = "VaultContract",
                        SourcePath = "../VaultContract/VaultContract.csproj",
                        OutputDirectory = "output/vault",
                        Dependencies = new List<string> { "TokenContract" },
                        InjectDependencies = true
                    }
                };

                var multiDeployResult = await toolkit.DeployMultipleContractsAsync(
                    deploymentRequests,
                    deploymentOptions
                );

                Console.WriteLine($"✓ Multi-contract deployment complete!");
                Console.WriteLine($"  - Total: {multiDeployResult.TotalContracts}");
                Console.WriteLine($"  - Successful: {multiDeployResult.SuccessfulDeployments.Count}");
                Console.WriteLine($"  - Failed: {multiDeployResult.FailedDeployments.Count}");

                foreach (var success in multiDeployResult.SuccessfulDeployments)
                {
                    Console.WriteLine($"  ✓ {success.ContractName}: {success.ContractHash}");
                }

                Console.WriteLine("\n=== Deployment Example Complete ===");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\n✗ Error: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return 1;
            }
        }
    }
}