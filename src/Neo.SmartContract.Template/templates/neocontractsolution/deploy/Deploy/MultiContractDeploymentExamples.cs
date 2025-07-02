using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeoContractSolution.Deploy;

/// <summary>
/// Examples of multi-contract deployment scenarios
/// </summary>
public static class MultiContractDeploymentExamples
{
    /// <summary>
    /// Example 1: Deploy multiple contracts with dependencies
    /// </summary>
    public static async Task DeployMultipleContractsWithDependencies(NeoContractToolkit toolkit)
    {
        // Define deployment options
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = toolkit.GetDeployerAccount(),
            GasLimit = 50_000_000,
            WaitForConfirmation = true
        };

        // Define contracts to deploy
        var deploymentRequests = new List<ContractDeploymentRequest>
        {
            // Base registry contract (no dependencies)
            new ContractDeploymentRequest
            {
                Name = "Registry",
                ProjectPath = "../src/Registry/Registry.csproj",
                OutputDirectory = "./artifacts/registry",
                Dependencies = new List<string>(),
                GenerateDebugInfo = true
            },
            // Token contract (depends on Registry)
            new ContractDeploymentRequest
            {
                Name = "MyToken",
                ProjectPath = "../src/MyToken/MyToken.csproj",
                OutputDirectory = "./artifacts/token",
                Dependencies = new List<string> { "Registry" },
                GenerateDebugInfo = true,
                InitialParameters = new List<object> { "MyToken", "MTK", 8, 1000000 },
                InjectDependencies = true
            },
            // Exchange contract (depends on Registry and MyToken)
            new ContractDeploymentRequest
            {
                Name = "Exchange",
                ProjectPath = "../src/Exchange/Exchange.csproj",
                OutputDirectory = "./artifacts/exchange",
                Dependencies = new List<string> { "Registry", "MyToken" },
                GenerateDebugInfo = true,
                InjectDependencies = true,
                PostDeploymentActions = new List<PostDeploymentAction>
                {
                    new PostDeploymentAction
                    {
                        Method = "initialize",
                        Parameters = new List<object> { 100 }, // 1% fee
                        Required = true
                    }
                }
            }
        };

        // Deploy all contracts
        var result = await toolkit.DeployMultipleContractsAsync(deploymentRequests, deploymentOptions);

        Console.WriteLine($"Deployment complete:");
        Console.WriteLine($"Total contracts: {result.TotalContracts}");
        Console.WriteLine($"Successful: {result.SuccessfulDeployments.Count}");
        Console.WriteLine($"Failed: {result.FailedDeployments.Count}");

        foreach (var success in result.SuccessfulDeployments)
        {
            Console.WriteLine($"  ✓ {success.ContractName}: {success.ContractHash}");
        }

        foreach (var failure in result.FailedDeployments)
        {
            Console.WriteLine($"  ✗ {failure.ContractName}: {failure.Reason}");
        }
    }

    /// <summary>
    /// Example 2: Deploy contracts from manifest file
    /// </summary>
    public static async Task DeployFromManifest(NeoContractToolkit toolkit)
    {
        // Create a manifest template first (one-time setup)
        await toolkit.CreateDeploymentManifestTemplateAsync("deployment-manifest.json");
        
        // Create a custom manifest for DeFi protocol
        var manifestContent = @"{
  ""version"": ""1.0"",
  ""description"": ""DeFi Protocol Deployment"",
  ""contracts"": [
    {
      ""name"": ""PriceOracle"",
      ""projectPath"": ""../src/Oracle/PriceOracle.csproj"",
      ""dependencies"": [],
      ""generateDebugInfo"": true,
      ""gasLimit"": 30000000,
      ""postDeploymentActions"": [
        {
          ""method"": ""setInitialPrices"",
          ""parameters"": [
            [""NEO"", 10000],
            [""GAS"", 2000]
          ],
          ""required"": true
        }
      ]
    },
    {
      ""name"": ""GovernanceToken"",
      ""projectPath"": ""../src/Token/GovernanceToken.csproj"",
      ""dependencies"": [],
      ""generateDebugInfo"": true,
      ""gasLimit"": 50000000,
      ""initialParameters"": [""DeFi Gov"", ""DGOV"", 8, 10000000]
    },
    {
      ""name"": ""LendingPool"",
      ""projectPath"": ""../src/Lending/LendingPool.csproj"",
      ""dependencies"": [""PriceOracle"", ""GovernanceToken""],
      ""generateDebugInfo"": true,
      ""gasLimit"": 80000000,
      ""injectDependencies"": true
    },
    {
      ""name"": ""StakingRewards"",
      ""projectPath"": ""../src/Staking/StakingRewards.csproj"",
      ""dependencies"": [""GovernanceToken"", ""LendingPool""],
      ""generateDebugInfo"": true,
      ""gasLimit"": 60000000,
      ""injectDependencies"": true,
      ""postDeploymentActions"": [
        {
          ""method"": ""setRewardRate"",
          ""parameters"": [1000],
          ""required"": true
        }
      ]
    }
  ]
}";

        await File.WriteAllTextAsync("defi-deployment-manifest.json", manifestContent);

        // Load and deploy from manifest
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = toolkit.GetDeployerAccount(),
            GasLimit = 50_000_000,
            WaitForConfirmation = true
        };

        var result = await toolkit.DeployFromManifestAsync("defi-deployment-manifest.json", deploymentOptions);
        
        Console.WriteLine($"Manifest deployment complete: {result.AllSuccessful}");
        
        // Save deployment results for future reference
        await SaveDeploymentResults(result, "defi-deployment-results.json");
    }

    /// <summary>
    /// Example 3: Parallel deployment of independent contracts
    /// </summary>
    public static async Task DeployContractsInParallel(NeoContractToolkit toolkit)
    {
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = toolkit.GetDeployerAccount(),
            GasLimit = 30_000_000,
            WaitForConfirmation = false // Don't wait for each individually
        };

        // Define independent contracts (no cross-dependencies)
        var deploymentRequests = new List<ContractDeploymentRequest>
        {
            new ContractDeploymentRequest
            {
                Name = "Oracle",
                ProjectPath = "../src/Oracle/Oracle.csproj",
                OutputDirectory = "./artifacts/oracle"
            },
            new ContractDeploymentRequest
            {
                Name = "Random",
                ProjectPath = "../src/Random/Random.csproj",
                OutputDirectory = "./artifacts/random"
            },
            new ContractDeploymentRequest
            {
                Name = "Storage",
                ProjectPath = "../src/Storage/Storage.csproj",
                OutputDirectory = "./artifacts/storage"
            },
            new ContractDeploymentRequest
            {
                Name = "Events",
                ProjectPath = "../src/Events/Events.csproj",
                OutputDirectory = "./artifacts/events"
            },
            new ContractDeploymentRequest
            {
                Name = "Timer",
                ProjectPath = "../src/Timer/Timer.csproj",
                OutputDirectory = "./artifacts/timer"
            }
        };

        // Deploy in parallel with max 3 concurrent deployments
        var result = await toolkit.DeployMultipleContractsParallelAsync(
            deploymentRequests, 
            deploymentOptions, 
            maxParallelism: 3);

        Console.WriteLine($"Parallel deployment completed!");
        Console.WriteLine($"Deployed {result.SuccessfulDeployments.Count} contracts in parallel");
    }

    /// <summary>
    /// Example 4: Deploy with rollback on failure
    /// </summary>
    public static async Task DeployWithRollback(NeoContractToolkit toolkit)
    {
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = toolkit.GetDeployerAccount(),
            GasLimit = 50_000_000,
            WaitForConfirmation = true
        };

        // Track deployed contracts for potential rollback
        var deployedContracts = new List<ContractDeploymentInfo>();

        var deploymentRequests = new List<ContractDeploymentRequest>
        {
            new ContractDeploymentRequest
            {
                Name = "TokenA",
                ProjectPath = "../src/TokenA/TokenA.csproj",
                OutputDirectory = "./artifacts/tokenA",
                FailureMode = DeploymentFailureMode.StopOnError
            },
            new ContractDeploymentRequest
            {
                Name = "TokenB",
                ProjectPath = "../src/TokenB/TokenB.csproj",
                OutputDirectory = "./artifacts/tokenB",
                FailureMode = DeploymentFailureMode.StopOnError
            },
            new ContractDeploymentRequest
            {
                Name = "AMM", // This might fail if TokenA or TokenB deployment fails
                ProjectPath = "../src/AMM/AMM.csproj",
                OutputDirectory = "./artifacts/amm",
                Dependencies = new List<string> { "TokenA", "TokenB" },
                InjectDependencies = true,
                FailureMode = DeploymentFailureMode.RollbackOnError
            }
        };

        try
        {
            var result = await toolkit.DeployMultipleContractsAsync(deploymentRequests, deploymentOptions);
            
            if (!result.AllSuccessful)
            {
                Console.WriteLine("Deployment failed. Rolling back...");
                
                // In a real scenario, you would call destroy methods on deployed contracts
                foreach (var deployed in result.SuccessfulDeployments.Reverse())
                {
                    Console.WriteLine($"Would rollback: {deployed.ContractName}");
                    // await toolkit.InvokeContractAsync(deployed.ContractHash, "destroy", new object[0]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical failure: {ex.Message}");
            // Perform rollback logic
        }
    }

    /// <summary>
    /// Example 5: Update multiple contracts
    /// </summary>
    public static async Task UpdateMultipleContracts(NeoContractToolkit toolkit)
    {
        // Load existing deployment results
        var previousDeployment = await LoadDeploymentResults("defi-deployment-results.json");
        
        var updateRequests = new List<ContractUpdateRequest>();
        
        foreach (var contract in previousDeployment.SuccessfulDeployments)
        {
            // Check if update project exists
            var updateProjectPath = $"../src/{contract.ContractName}/{contract.ContractName}.csproj";
            if (File.Exists(updateProjectPath))
            {
                updateRequests.Add(new ContractUpdateRequest
                {
                    Name = contract.ContractName,
                    ContractHash = contract.ContractHash,
                    ProjectPath = updateProjectPath,
                    GenerateDebugInfo = true,
                    Optimize = true
                });
            }
        }

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = toolkit.GetDeployerAccount(),
            GasLimit = 60_000_000,
            WaitForConfirmation = true
        };

        var result = await toolkit.UpdateMultipleContractsAsync(updateRequests, deploymentOptions);
        
        Console.WriteLine($"Updated {result.SuccessfulDeployments.Count} contracts");
        foreach (var failure in result.FailedDeployments)
        {
            Console.WriteLine($"Failed to update {failure.ContractName}: {failure.Reason}");
        }
    }

    /// <summary>
    /// Example 6: Deploy contracts for different networks
    /// </summary>
    public static async Task DeployToMultipleNetworks(NeoContractToolkit toolkit)
    {
        var networks = new[]
        {
            ("TestNet", "https://testnet1.neo.org:20331"),
            ("PrivateNet", "http://localhost:10332")
        };

        var deploymentRequests = new List<ContractDeploymentRequest>
        {
            new ContractDeploymentRequest
            {
                Name = "SimpleToken",
                ProjectPath = "../src/SimpleToken/SimpleToken.csproj",
                OutputDirectory = "./artifacts/token",
                GenerateDebugInfo = true
            }
        };

        foreach (var (networkName, rpcUrl) in networks)
        {
            Console.WriteLine($"Deploying to {networkName}...");
            
            // Load network-specific wallet
            await toolkit.LoadWalletAsync($"wallets/{networkName.ToLower()}.wallet.json", "password");
            
            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = toolkit.GetDeployerAccount(),
                GasLimit = 50_000_000,
                WaitForConfirmation = true
            };

            try
            {
                var result = await toolkit.DeployMultipleContractsAsync(deploymentRequests, deploymentOptions);
                
                // Save network-specific deployment results
                await SaveDeploymentResults(result, $"deployment-{networkName.ToLower()}.json");
                
                Console.WriteLine($"Deployed to {networkName}: {result.AllSuccessful}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deploy to {networkName}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Helper method to save deployment results
    /// </summary>
    private static async Task SaveDeploymentResults(MultiContractDeploymentResult result, string filename)
    {
        var deploymentRecord = new
        {
            Timestamp = DateTime.UtcNow,
            TotalContracts = result.TotalContracts,
            Successful = result.SuccessfulDeployments.Select(d => new
            {
                d.ContractName,
                ContractHash = d.ContractHash.ToString(),
                TransactionHash = d.TransactionHash.ToString(),
                d.GasConsumed
            }),
            Failed = result.FailedDeployments.Select(f => new
            {
                f.ContractName,
                f.Reason
            })
        };

        var json = System.Text.Json.JsonSerializer.Serialize(deploymentRecord, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(filename, json);
    }

    /// <summary>
    /// Helper method to load previous deployment results
    /// </summary>
    private static async Task<MultiContractDeploymentResult> LoadDeploymentResults(string filename)
    {
        var json = await File.ReadAllTextAsync(filename);
        var record = System.Text.Json.JsonSerializer.Deserialize<DeploymentRecord>(json);
        
        var result = new MultiContractDeploymentResult
        {
            TotalContracts = record.TotalContracts
        };

        foreach (var success in record.Successful)
        {
            result.SuccessfulDeployments.Add(new ContractDeploymentInfo
            {
                Success = true,
                ContractName = success.ContractName,
                ContractHash = UInt160.Parse(success.ContractHash),
                TransactionHash = UInt256.Parse(success.TransactionHash),
                GasConsumed = success.GasConsumed
            });
        }

        foreach (var failed in record.Failed)
        {
            result.FailedDeployments.Add(new FailedDeployment
            {
                ContractName = failed.ContractName,
                Reason = failed.Reason
            });
        }

        return result;
    }

    private class DeploymentRecord
    {
        public DateTime Timestamp { get; set; }
        public int TotalContracts { get; set; }
        public List<SuccessRecord> Successful { get; set; } = new();
        public List<FailedRecord> Failed { get; set; } = new();
    }

    private class SuccessRecord
    {
        public string ContractName { get; set; } = "";
        public string ContractHash { get; set; } = "";
        public string TransactionHash { get; set; } = "";
        public long GasConsumed { get; set; }
    }

    private class FailedRecord
    {
        public string ContractName { get; set; } = "";
        public string Reason { get; set; } = "";
    }
}