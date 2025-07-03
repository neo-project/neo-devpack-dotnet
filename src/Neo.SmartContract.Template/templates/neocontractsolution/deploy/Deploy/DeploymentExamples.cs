using Neo;
using Neo.SmartContract.Deploy;
using Microsoft.Extensions.Logging;

namespace NeoContractSolution.Deploy;

/// <summary>
/// Additional deployment examples for reference.
/// Copy and modify these methods in your Program.cs as needed.
/// </summary>
public static class DeploymentExamples
{
    /// <summary>
    /// Deploy a DeFi protocol with multiple interdependent contracts
    /// </summary>
    public static async Task DeployDeFiProtocol(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying DeFi Protocol ===");

        // 1. Deploy Oracle Contract
        Console.WriteLine("Deploying Oracle Contract...");
        var oracleResult = await toolkit.Deploy("../../src/Oracle/Oracle.csproj");
        Console.WriteLine($"Oracle deployed: {oracleResult.ContractHash}");

        // 2. Deploy Token Contract
        Console.WriteLine("Deploying Token Contract...");
        var tokenResult = await toolkit.Deploy("../../src/Token/Token.csproj");
        
        // Initialize token with name, symbol, decimals, and total supply
        await toolkit.Invoke(
            tokenResult.ContractHash,
            "initialize",
            "DeFi Token", "DFT", 8, 1000000_00000000L
        );
        Console.WriteLine($"Token deployed and initialized: {tokenResult.ContractHash}");

        // 3. Deploy Lending Pool Contract
        Console.WriteLine("Deploying Lending Pool Contract...");
        var lendingResult = await toolkit.Deploy("../../src/LendingPool/LendingPool.csproj");
        
        // Initialize lending pool with token and oracle dependencies
        await toolkit.Invoke(
            lendingResult.ContractHash,
            "initialize",
            tokenResult.ContractHash,     // Collateral token
            oracleResult.ContractHash,     // Price oracle
            80,                            // 80% collateralization ratio
            500                            // 5% annual interest rate (in basis points)
        );
        Console.WriteLine($"Lending Pool deployed and initialized: {lendingResult.ContractHash}");

        // 4. Deploy Governance Contract
        Console.WriteLine("Deploying Governance Contract...");
        var govResult = await toolkit.Deploy("../../src/Governance/Governance.csproj");
        
        // Initialize governance with all protocol contracts
        await toolkit.Invoke(
            govResult.ContractHash,
            "initialize",
            tokenResult.ContractHash,      // Governance token
            lendingResult.ContractHash,    // Lending pool to govern
            oracleResult.ContractHash,     // Oracle to govern
            259200                         // 3 day voting period (in seconds)
        );
        Console.WriteLine($"Governance deployed and initialized: {govResult.ContractHash}");

        // 5. Configure cross-contract permissions
        Console.WriteLine("Configuring contract permissions...");
        
        // Allow lending pool to transfer tokens
        await toolkit.Invoke(
            tokenResult.ContractHash,
            "addAuthorizedContract",
            lendingResult.ContractHash
        );
        
        // Allow governance to update lending pool parameters
        await toolkit.Invoke(
            lendingResult.ContractHash,
            "setGovernance",
            govResult.ContractHash
        );

        Console.WriteLine("=== DeFi Protocol Deployment Complete ===");
        Console.WriteLine($"Oracle Contract: {oracleResult.ContractHash}");
        Console.WriteLine($"Token Contract: {tokenResult.ContractHash}");
        Console.WriteLine($"Lending Pool: {lendingResult.ContractHash}");
        Console.WriteLine($"Governance: {govResult.ContractHash}");
    }

    /// <summary>
    /// Deploy an NFT marketplace with royalties
    /// </summary>
    public static async Task DeployNFTMarketplace(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying NFT Marketplace ===");

        // 1. Deploy NFT Contract
        Console.WriteLine("Deploying NFT Contract...");
        var nftResult = await toolkit.Deploy("../../src/NFT/NFT.csproj");
        
        // Get deployer account for initial minter
        var deployerAccount = await toolkit.GetDeployerAccount();
        
        await toolkit.Invoke(
            nftResult.ContractHash,
            "initialize",
            "Neo Art Collection", 
            "NAC",
            "https://api.neoart.io/metadata/",
            deployerAccount // Initial minter
        );

        // 2. Deploy Marketplace Contract
        Console.WriteLine("Deploying Marketplace Contract...");
        var marketResult = await toolkit.Deploy("../../src/Marketplace/Marketplace.csproj");
        
        await toolkit.Invoke(
            marketResult.ContractHash,
            "initialize",
            250,  // 2.5% marketplace fee
            1000  // 10% max royalty rate
        );

        // 3. Deploy Royalty Registry
        Console.WriteLine("Deploying Royalty Registry...");
        var royaltyResult = await toolkit.Deploy("../../src/Royalty/Royalty.csproj");

        // 4. Configure marketplace to use royalty registry
        await toolkit.Invoke(
            marketResult.ContractHash,
            "setRoyaltyRegistry",
            royaltyResult.ContractHash
        );

        // 5. Whitelist NFT collection on marketplace
        await toolkit.Invoke(
            marketResult.ContractHash,
            "whitelistCollection",
            nftResult.ContractHash
        );

        Console.WriteLine("=== NFT Marketplace Deployment Complete ===");
        Console.WriteLine($"NFT Collection: {nftResult.ContractHash}");
        Console.WriteLine($"Marketplace: {marketResult.ContractHash}");
        Console.WriteLine($"Royalty Registry: {royaltyResult.ContractHash}");
    }

    /// <summary>
    /// Deploy contracts from pre-compiled artifacts (CI/CD scenario)
    /// </summary>
    public static async Task DeployFromArtifacts(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying from Artifacts ===");

        // Load deployment manifest (could be from CI/CD pipeline)
        var artifactPath = Environment.GetEnvironmentVariable("ARTIFACT_PATH") ?? "artifacts";
        
        // Deploy contracts listed in manifest
        var contracts = new[]
        {
            ("CoreContract", "core", new object[] { "v2.0.0" }),
            ("APIContract", "api", new object[] { /* api will reference core */ }),
            ("AdminContract", "admin", new object[] { /* admin will reference both */ })
        };

        var deployedContracts = new Dictionary<string, UInt160>();

        foreach (var (name, artifact, initParams) in contracts)
        {
            Console.WriteLine($"Deploying {name} from artifacts...");
            
            var nefPath = Path.Combine(artifactPath, $"{artifact}.nef");
            var manifestPath = Path.Combine(artifactPath, $"{artifact}.manifest.json");
            
            if (!File.Exists(nefPath) || !File.Exists(manifestPath))
            {
                Console.WriteLine($"Artifact files not found for {name}");
                continue;
            }

            var result = await toolkit.DeployFromArtifacts(nefPath, manifestPath);
            deployedContracts[name] = result.ContractHash;
            
            // Resolve contract references in init params
            var resolvedParams = ResolveContractReferences(initParams, deployedContracts);
            
            // Initialize if params provided
            if (resolvedParams.Length > 0)
            {
                await toolkit.Invoke(
                    result.ContractHash,
                    "initialize",
                    resolvedParams
                );
            }
            
            Console.WriteLine($"{name} deployed: {result.ContractHash}");
        }

        Console.WriteLine("=== Artifact Deployment Complete ===");
    }

    /// <summary>
    /// Update existing contracts scenario
    /// </summary>
    public static async Task UpdateContracts(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Updating Contracts ===");

        // Load existing contract addresses (from deployment records)
        var existingContracts = new Dictionary<string, string>
        {
            ["TokenContract"] = "0x1234567890abcdef1234567890abcdef12345678",
            ["GovernanceContract"] = "0xabcdef1234567890abcdef1234567890abcdef12"
        };

        foreach (var (name, hashString) in existingContracts)
        {
            var contractHash = UInt160.Parse(hashString);
            
            // Check if contract exists and is updatable
            if (await toolkit.ContractExists(contractHash))
            {
                Console.WriteLine($"Updating {name} at {contractHash}...");
                
                try
                {
                    var updateResult = await toolkit.Update(
                        contractHash,
                        $"../../src/{name}/{name}.csproj"
                    );
                    
                    Console.WriteLine($"{name} updated successfully. Tx: {updateResult.TransactionHash}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update {name}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Contract {name} not found at {contractHash}");
            }
        }

        Console.WriteLine("=== Contract Updates Complete ===");
    }

    /// <summary>
    /// Deploy using environment-specific configurations
    /// </summary>
    public static async Task DeployWithEnvironments(DeploymentToolkit toolkit)
    {
        // Get environment from args or environment variable
        var environment = Environment.GetEnvironmentVariable("DEPLOY_ENV") ?? "local";
        
        Console.WriteLine($"=== Deploying to {environment} environment ===");
        
        // Switch network based on environment
        switch (environment.ToLower())
        {
            case "production":
            case "mainnet":
                toolkit.SetNetwork("mainnet");
                break;
            case "staging":
            case "testnet":
                toolkit.SetNetwork("testnet");
                break;
            default:
                toolkit.SetNetwork("local");
                break;
        }
        
        // Deploy contracts
        var contractResult = await toolkit.Deploy("../../src/MyContract/MyContract.csproj");
        Console.WriteLine($"Contract deployed to {environment}: {contractResult.ContractHash}");
        
        // Save deployment record
        SaveDeploymentRecord(environment, contractResult);
    }

    /// <summary>
    /// Deploy from manifest with complex dependencies
    /// </summary>
    public static async Task DeployComplexManifest(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying from Complex Manifest ===");
        
        // Create a custom manifest for a complex deployment
        var manifest = new
        {
            version = "1.0",
            description = "Complex multi-contract deployment",
            contracts = new[]
            {
                new
                {
                    name = "Registry",
                    projectPath = "../../src/Registry/Registry.csproj",
                    deploymentParameters = Array.Empty<object>()
                },
                new
                {
                    name = "Token",
                    projectPath = "../../src/Token/Token.csproj",
                    dependencies = new[] { "Registry" },
                    deploymentParameters = new object[] { "MyToken", "MTK", 8, 1000000 }
                },
                new
                {
                    name = "Exchange",
                    projectPath = "../../src/Exchange/Exchange.csproj",
                    dependencies = new[] { "Registry", "Token" },
                    postDeploymentActions = new[]
                    {
                        new { method = "registerToken", parameters = new object[] { "{{Token}}" } },
                        new { method = "setFeeRate", parameters = new object[] { 300 } }
                    }
                }
            }
        };
        
        // Save manifest to file
        var manifestPath = "deployment-manifest-complex.json";
        await File.WriteAllTextAsync(manifestPath, System.Text.Json.JsonSerializer.Serialize(manifest));
        
        // Deploy from manifest
        var results = await toolkit.DeployFromManifest(manifestPath);
        
        Console.WriteLine("=== Complex Deployment Complete ===");
        foreach (var (name, info) in results)
        {
            Console.WriteLine($"{name}: {info.ContractHash}");
        }
    }

    /// <summary>
    /// Helper method to resolve contract references in parameters
    /// </summary>
    private static object[] ResolveContractReferences(object[] parameters, Dictionary<string, UInt160> deployedContracts)
    {
        var resolved = new object[parameters.Length];
        
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i] is string str && str.StartsWith("{{") && str.EndsWith("}}"))
            {
                var contractName = str.Substring(2, str.Length - 4);
                if (deployedContracts.TryGetValue(contractName, out var hash))
                {
                    resolved[i] = hash;
                }
                else
                {
                    throw new InvalidOperationException($"Contract reference not found: {contractName}");
                }
            }
            else
            {
                resolved[i] = parameters[i];
            }
        }
        
        return resolved;
    }

    /// <summary>
    /// Save deployment record for tracking
    /// </summary>
    private static void SaveDeploymentRecord(string environment, ContractDeploymentInfo deploymentInfo)
    {
        var record = new
        {
            environment,
            deployedAt = DateTime.UtcNow,
            contractHash = deploymentInfo.ContractHash.ToString(),
            transactionHash = deploymentInfo.TransactionHash.ToString(),
            blockIndex = deploymentInfo.BlockIndex,
            gasConsumed = deploymentInfo.GasConsumed
        };
        
        var recordPath = $"deployments/{environment}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
        Directory.CreateDirectory("deployments");
        File.WriteAllText(recordPath, System.Text.Json.JsonSerializer.Serialize(record, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        
        Console.WriteLine($"Deployment record saved to: {recordPath}");
    }
}