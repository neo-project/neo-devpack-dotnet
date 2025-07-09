using System;
using System.IO;
using System.Threading.Tasks;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;

namespace MultiContractDeployment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Neo Multi-Contract Deployment Example");
            Console.WriteLine("=====================================\n");

            // Create deployment toolkit
            using var toolkit = new DeploymentToolkit();

            // Configure network and account
            var network = Environment.GetEnvironmentVariable("NEO_NETWORK") ?? "testnet";
            var wifKey = Environment.GetEnvironmentVariable("NEO_WIF_KEY");

            if (string.IsNullOrEmpty(wifKey))
            {
                Console.WriteLine("Please set NEO_WIF_KEY environment variable with your private key");
                return;
            }

            toolkit.SetNetwork(network).SetWifKey(wifKey);

            // Example 1: Deploy using manifest builder
            await DeployUsingManifestBuilder(toolkit);

            // Example 2: Deploy from manifest file
            await DeployFromManifestFile(toolkit);

            // Example 3: Deploy complex DeFi ecosystem
            await DeployDeFiEcosystem(toolkit);
        }

        static async Task DeployUsingManifestBuilder(DeploymentToolkit toolkit)
        {
            Console.WriteLine("\n1. Deploying contracts using manifest builder...");

            var manifest = toolkit.CreateManifestBuilder()
                .WithName("Simple Token Ecosystem")
                .WithDescription("Deploy a token and its governance contract")
                .WithSettings(settings =>
                {
                    settings.VerifyAfterDeploy = true;
                    settings.WaitForConfirmation = true;
                })
                .AddContract("token", "MyToken", "./contracts/MyToken/MyToken.csproj", contract =>
                {
                    contract
                        .WithDescription("NEP-17 Token Contract")
                        .WithTag("token")
                        .WithTag("nep17")
                        .WithInitParams(1000000000); // Initial supply
                })
                .AddContract("governance", "MyGovernance", "./contracts/MyGovernance/MyGovernance.csproj", contract =>
                {
                    contract
                        .WithDescription("Governance Contract")
                        .WithTag("governance")
                        .DependsOn("token")
                        .WithInitParams("@contract:token", 100); // Token contract hash and voting threshold
                })
                .AddInteraction("governance", "token", "approve", interaction =>
                {
                    interaction
                        .WithDescription("Allow governance to spend tokens")
                        .WithParams("@contract:governance", 1000000000)
                        .WithOrder(1);
                })
                .Build();

            try
            {
                var result = await toolkit.DeployMultipleAsync(manifest);

                Console.WriteLine($"\nDeployment Status: {result.Status}");
                Console.WriteLine($"Total Contracts: {result.Summary.TotalContracts}");
                Console.WriteLine($"Successful: {result.Summary.SuccessfulDeployments}");
                Console.WriteLine($"Failed: {result.Summary.FailedDeployments}");
                Console.WriteLine($"Duration: {result.Summary.Duration?.TotalSeconds:F2} seconds");

                foreach (var (contractId, info) in result.DeployedContracts)
                {
                    Console.WriteLine($"\n{contractId}:");
                    Console.WriteLine($"  Contract Hash: {info.ContractHash}");
                    Console.WriteLine($"  Transaction: {info.TransactionHash}");
                    Console.WriteLine($"  GAS Consumed: {info.GasConsumed:N0}");
                }

                if (result.InteractionResults.Count > 0)
                {
                    Console.WriteLine("\nContract Interactions:");
                    foreach (var interaction in result.InteractionResults)
                    {
                        Console.WriteLine($"  {interaction.Description}: {(interaction.Success ? "Success" : "Failed")}");
                        if (interaction.Success)
                        {
                            Console.WriteLine($"    Transaction: {interaction.TransactionHash}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deployment failed: {ex.Message}");
            }
        }

        static async Task DeployFromManifestFile(DeploymentToolkit toolkit)
        {
            Console.WriteLine("\n2. Deploying contracts from manifest file...");

            // Create a sample manifest file
            var manifestPath = "deployment-manifest.json";
            var manifest = new DeploymentManifest
            {
                Name = "NFT Marketplace",
                Description = "Deploy NFT contract and marketplace",
                Version = "1.0",
                Contracts = new System.Collections.Generic.List<ContractDefinition>
                {
                    new()
                    {
                        Id = "nft",
                        Name = "MyNFT",
                        Description = "NEP-11 NFT Contract",
                        ProjectPath = "./contracts/MyNFT/MyNFT.csproj",
                        Tags = { "nft", "nep11" }
                    },
                    new()
                    {
                        Id = "marketplace",
                        Name = "NFTMarketplace",
                        Description = "NFT Marketplace Contract",
                        ProjectPath = "./contracts/NFTMarketplace/NFTMarketplace.csproj",
                        Dependencies = { "nft" },
                        InitParams = new System.Collections.Generic.List<object> 
                        { 
                            "@contract:nft",  // NFT contract hash
                            250              // Marketplace fee (2.5%)
                        },
                        Tags = { "marketplace", "defi" }
                    }
                },
                Interactions = new System.Collections.Generic.List<ContractInteraction>
                {
                    new()
                    {
                        Source = "nft",
                        Target = "marketplace",
                        Method = "registerMarketplace",
                        Params = { "@contract:marketplace" },
                        Description = "Register marketplace as authorized seller",
                        Order = 1
                    }
                },
                Settings = new DeploymentSettings
                {
                    VerifyAfterDeploy = true,
                    WaitForConfirmation = true,
                    DefaultGasLimit = 100_000_000
                }
            };

            // Save manifest to file
            var json = System.Text.Json.JsonSerializer.Serialize(manifest, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
            await File.WriteAllTextAsync(manifestPath, json);

            try
            {
                var result = await toolkit.DeployFromManifestAsync(manifestPath);
                Console.WriteLine($"\nNFT Marketplace Deployment Status: {result.Status}");
                
                // Display deployment results
                DisplayDeploymentResults(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deployment failed: {ex.Message}");
            }
            finally
            {
                // Clean up manifest file
                if (File.Exists(manifestPath))
                    File.Delete(manifestPath);
            }
        }

        static async Task DeployDeFiEcosystem(DeploymentToolkit toolkit)
        {
            Console.WriteLine("\n3. Deploying complex DeFi ecosystem...");

            var manifest = toolkit.CreateManifestBuilder()
                .WithName("DeFi Ecosystem")
                .WithDescription("Complete DeFi ecosystem with tokens, DEX, lending, and staking")
                .EnableBatching(3) // Deploy up to 3 contracts in parallel where possible
                .WithSettings(s =>
                {
                    s.VerifyAfterDeploy = true;
                    s.WaitForConfirmation = true;
                    s.RollbackOnFailure = true; // Rollback all if any fails
                })
                // Core tokens
                .AddContract("usdt", "USDT", "./contracts/StableCoin/StableCoin.csproj", c => c
                    .WithDescription("Tether USD stablecoin")
                    .WithTag("stablecoin")
                    .WithInitParams("Tether USD", "USDT", 6, 1000000000))
                .AddContract("wbtc", "WBTC", "./contracts/WrappedToken/WrappedToken.csproj", c => c
                    .WithDescription("Wrapped Bitcoin")
                    .WithTag("wrapped")
                    .WithInitParams("Wrapped Bitcoin", "WBTC", 8, 21000000))
                .AddContract("governance", "GovernanceToken", "./contracts/GovernanceToken/GovernanceToken.csproj", c => c
                    .WithDescription("Platform governance token")
                    .WithTag("governance")
                    .WithInitParams("DeFi Governance", "DEFI", 8, 100000000))
                
                // Core infrastructure
                .AddContract("oracle", "PriceOracle", "./contracts/PriceOracle/PriceOracle.csproj", c => c
                    .WithDescription("Price oracle for asset valuation")
                    .WithTag("infrastructure"))
                
                // DeFi protocols
                .AddContract("dex", "DecentralizedExchange", "./contracts/DEX/DEX.csproj", c => c
                    .WithDescription("Automated Market Maker DEX")
                    .WithTag("defi")
                    .WithTag("dex")
                    .DependsOn("usdt", "wbtc", "governance", "oracle")
                    .WithInitParams("@contract:governance", "@contract:oracle", 30)) // 0.3% swap fee
                .AddContract("lending", "LendingProtocol", "./contracts/Lending/Lending.csproj", c => c
                    .WithDescription("Decentralized lending protocol")
                    .WithTag("defi")
                    .WithTag("lending")
                    .DependsOn("usdt", "wbtc", "oracle")
                    .WithInitParams("@contract:oracle"))
                .AddContract("staking", "StakingRewards", "./contracts/Staking/Staking.csproj", c => c
                    .WithDescription("Staking rewards distribution")
                    .WithTag("defi")
                    .WithTag("staking")
                    .DependsOn("governance")
                    .WithInitParams("@contract:governance", 100000)) // Rewards per block
                
                // Setup interactions
                .AddInteraction("dex", "usdt", "approve", i => i
                    .WithDescription("Approve DEX for USDT")
                    .WithParams("@contract:dex", "1000000000000")
                    .WithOrder(1))
                .AddInteraction("dex", "wbtc", "approve", i => i
                    .WithDescription("Approve DEX for WBTC")
                    .WithParams("@contract:dex", "1000000000000")
                    .WithOrder(2))
                .AddInteraction("lending", "usdt", "approve", i => i
                    .WithDescription("Approve lending for USDT")
                    .WithParams("@contract:lending", "1000000000000")
                    .WithOrder(3))
                .AddInteraction("lending", "wbtc", "approve", i => i
                    .WithDescription("Approve lending for WBTC")
                    .WithParams("@contract:lending", "1000000000000")
                    .WithOrder(4))
                .AddInteraction("oracle", "oracle", "addPriceFeed", i => i
                    .WithDescription("Add USDT price feed")
                    .WithParams("@contract:usdt", "USD", 100000000) // $1.00
                    .WithOrder(5))
                .AddInteraction("oracle", "oracle", "addPriceFeed", i => i
                    .WithDescription("Add WBTC price feed")
                    .WithParams("@contract:wbtc", "BTC", 4500000000000) // $45,000
                    .WithOrder(6))
                .Build();

            try
            {
                Console.WriteLine($"Deploying {manifest.Contracts.Count} contracts...");
                
                var result = await toolkit.DeployMultipleAsync(manifest);
                
                Console.WriteLine($"\nDeFi Ecosystem Deployment Completed!");
                Console.WriteLine($"Status: {result.Status}");
                Console.WriteLine($"Total GAS Spent: {result.TotalGasSpent:N2}");
                Console.WriteLine($"Total Network Fees: {result.TotalNetworkFees:N2}");
                
                // Display detailed results
                DisplayDeploymentResults(result);
                
                // Save deployment report
                await SaveDeploymentReport(result, "defi-deployment-report.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeFi ecosystem deployment failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        static void DisplayDeploymentResults(MultiContractDeploymentResult result)
        {
            Console.WriteLine("\nDeployed Contracts:");
            Console.WriteLine("==================");
            foreach (var (id, info) in result.DeployedContracts)
            {
                Console.WriteLine($"\n{id}:");
                Console.WriteLine($"  Name: {info.ContractName}");
                Console.WriteLine($"  Hash: {info.ContractHash}");
                Console.WriteLine($"  Transaction: {info.TransactionHash}");
                Console.WriteLine($"  GAS: {info.GasConsumed:N0}");
                Console.WriteLine($"  Network Fee: {info.NetworkFee:N0}");
            }

            if (result.FailedDeployments.Count > 0)
            {
                Console.WriteLine("\nFailed Deployments:");
                Console.WriteLine("==================");
                foreach (var (id, failure) in result.FailedDeployments)
                {
                    Console.WriteLine($"\n{id}:");
                    Console.WriteLine($"  Contract: {failure.ContractName}");
                    Console.WriteLine($"  Stage: {failure.FailureStage}");
                    Console.WriteLine($"  Error: {failure.ErrorMessage}");
                }
            }

            if (result.InteractionResults.Count > 0)
            {
                Console.WriteLine("\nContract Interactions:");
                Console.WriteLine("=====================");
                foreach (var interaction in result.InteractionResults)
                {
                    Console.WriteLine($"\n{interaction.Description}:");
                    Console.WriteLine($"  {interaction.SourceContractId} -> {interaction.TargetContractId}");
                    Console.WriteLine($"  Method: {interaction.Method}");
                    Console.WriteLine($"  Status: {(interaction.Success ? "Success" : "Failed")}");
                    if (interaction.Success)
                    {
                        Console.WriteLine($"  Transaction: {interaction.TransactionHash}");
                        Console.WriteLine($"  GAS: {interaction.GasConsumed:N0}");
                    }
                    else
                    {
                        Console.WriteLine($"  Error: {interaction.ErrorMessage}");
                    }
                }
            }

            if (result.RollbackResult != null)
            {
                Console.WriteLine("\nRollback Information:");
                Console.WriteLine("====================");
                Console.WriteLine($"Status: {result.RollbackResult.Status}");
                Console.WriteLine($"Rolled Back: {string.Join(", ", result.RollbackResult.RolledBackContracts)}");
                if (result.RollbackResult.FailedRollbacks.Count > 0)
                {
                    Console.WriteLine("Failed Rollbacks:");
                    foreach (var (contract, error) in result.RollbackResult.FailedRollbacks)
                    {
                        Console.WriteLine($"  {contract}: {error}");
                    }
                }
            }
        }

        static async Task SaveDeploymentReport(MultiContractDeploymentResult result, string filename)
        {
            var report = new
            {
                DeploymentId = result.DeploymentId,
                Timestamp = result.StartTime,
                Duration = result.Summary.Duration?.TotalSeconds,
                Status = result.Status.ToString(),
                Summary = result.Summary,
                Contracts = result.DeployedContracts.Select(kvp => new
                {
                    Id = kvp.Key,
                    kvp.Value.ContractName,
                    Hash = kvp.Value.ContractHash.ToString(),
                    Transaction = kvp.Value.TransactionHash.ToString(),
                    kvp.Value.GasConsumed,
                    kvp.Value.NetworkFee
                }),
                Interactions = result.InteractionResults.Select(i => new
                {
                    i.Description,
                    i.SourceContractId,
                    i.TargetContractId,
                    i.Method,
                    i.Success,
                    Transaction = i.TransactionHash?.ToString(),
                    i.ErrorMessage
                }),
                TotalCost = new
                {
                    GAS = result.TotalGasSpent,
                    NetworkFees = result.TotalNetworkFees
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filename, json);
            Console.WriteLine($"\nDeployment report saved to: {filename}");
        }
    }
}