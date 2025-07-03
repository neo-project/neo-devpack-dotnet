using Neo;
using Neo.SmartContract.Deploy;
using System.Text.Json;

namespace NeoContractSolution.Deploy;

/// <summary>
/// Examples of multi-contract deployment scenarios using the simplified toolkit
/// </summary>
public static class MultiContractDeploymentExamples
{
    /// <summary>
    /// Example 1: Deploy multiple contracts with dependencies using manifest
    /// </summary>
    public static async Task DeployWithManifest(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying Multi-Contract System from Manifest ===");
        
        // Deploy all contracts defined in the manifest
        var results = await toolkit.DeployFromManifest("deployment-manifest.json");
        
        Console.WriteLine($"\nDeployed {results.Count} contracts:");
        foreach (var (name, info) in results)
        {
            Console.WriteLine($"  {name}: {info.ContractHash}");
        }
        
        // Save deployment results for future reference
        await SaveDeploymentResults(results, "deployment-results.json");
    }

    /// <summary>
    /// Example 2: Deploy contracts in specific order with cross-references
    /// </summary>
    public static async Task DeployWithCrossReferences(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying Contracts with Cross-References ===");
        
        var deployedContracts = new Dictionary<string, UInt160>();
        
        // 1. Deploy Registry (base contract)
        Console.WriteLine("\n1. Deploying Registry...");
        var registryResult = await toolkit.Deploy("../../src/Registry/Registry.csproj");
        deployedContracts["Registry"] = registryResult.ContractHash;
        Console.WriteLine($"   Registry: {registryResult.ContractHash}");
        
        // 2. Deploy Token and register it
        Console.WriteLine("\n2. Deploying Token...");
        var tokenResult = await toolkit.Deploy("../../src/Token/Token.csproj");
        deployedContracts["Token"] = tokenResult.ContractHash;
        
        // Initialize token
        await toolkit.Invoke(
            tokenResult.ContractHash,
            "initialize",
            "MyToken", "MTK", 8, 1000000_00000000L
        );
        
        // Register token in registry
        await toolkit.Invoke(
            registryResult.ContractHash,
            "registerContract",
            "Token",
            tokenResult.ContractHash
        );
        Console.WriteLine($"   Token: {tokenResult.ContractHash} (registered)");
        
        // 3. Deploy Exchange with token reference
        Console.WriteLine("\n3. Deploying Exchange...");
        var exchangeResult = await toolkit.Deploy("../../src/Exchange/Exchange.csproj");
        deployedContracts["Exchange"] = exchangeResult.ContractHash;
        
        // Initialize exchange with token reference
        await toolkit.Invoke(
            exchangeResult.ContractHash,
            "initialize",
            tokenResult.ContractHash,
            300 // 3% fee
        );
        
        // Register exchange in registry
        await toolkit.Invoke(
            registryResult.ContractHash,
            "registerContract",
            "Exchange",
            exchangeResult.ContractHash
        );
        Console.WriteLine($"   Exchange: {exchangeResult.ContractHash} (initialized with token)");
        
        // 4. Configure permissions
        Console.WriteLine("\n4. Configuring permissions...");
        await toolkit.Invoke(
            tokenResult.ContractHash,
            "addTrustedContract",
            exchangeResult.ContractHash
        );
        Console.WriteLine("   Token trusts Exchange");
        
        Console.WriteLine("\n=== Deployment Complete ===");
        Console.WriteLine("All contracts deployed and configured!");
        
        // Save the deployment info
        await SaveDeploymentMap(deployedContracts, "contract-addresses.json");
    }

    /// <summary>
    /// Example 3: Deploy contracts for different environments
    /// </summary>
    public static async Task DeployForEnvironment(DeploymentToolkit toolkit, string environment)
    {
        Console.WriteLine($"=== Deploying to {environment} ===");
        
        // Set network based on environment
        switch (environment.ToLower())
        {
            case "production":
                toolkit.SetNetwork("mainnet");
                break;
            case "staging":
                toolkit.SetNetwork("testnet");
                break;
            default:
                toolkit.SetNetwork("local");
                break;
        }
        
        // Deploy contracts
        var contracts = new Dictionary<string, ContractDeploymentInfo>();
        
        // Deploy core contracts
        Console.WriteLine("\nDeploying core contracts...");
        contracts["Oracle"] = await toolkit.Deploy("../../src/Oracle/Oracle.csproj");
        contracts["Token"] = await toolkit.Deploy("../../src/Token/Token.csproj");
        contracts["Governance"] = await toolkit.Deploy("../../src/Governance/Governance.csproj");
        
        // Initialize contracts based on environment
        if (environment.ToLower() == "production")
        {
            // Production initialization
            await toolkit.Invoke(
                contracts["Token"].ContractHash,
                "initialize",
                "Production Token", "PROD", 8, 100000000_00000000L
            );
        }
        else
        {
            // Test initialization with smaller values
            await toolkit.Invoke(
                contracts["Token"].ContractHash,
                "initialize",
                "Test Token", "TEST", 8, 1000000_00000000L
            );
        }
        
        // Save environment-specific deployment
        var filename = $"deployment-{environment.ToLower()}.json";
        await SaveDeploymentResults(
            contracts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            filename
        );
        
        Console.WriteLine($"\nDeployment saved to: {filename}");
    }

    /// <summary>
    /// Example 4: Update multiple existing contracts
    /// </summary>
    public static async Task UpdateMultipleContracts(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Updating Multiple Contracts ===");
        
        // Load previous deployment
        var previousDeployment = await LoadDeploymentMap("contract-addresses.json");
        
        var updateCount = 0;
        var failedUpdates = new List<string>();
        
        foreach (var (name, hashString) in previousDeployment)
        {
            var contractHash = UInt160.Parse(hashString);
            Console.WriteLine($"\nChecking {name} at {contractHash}...");
            
            if (await toolkit.ContractExists(contractHash))
            {
                try
                {
                    Console.WriteLine($"  Updating {name}...");
                    var projectPath = $"../../src/{name}/{name}.csproj";
                    
                    if (File.Exists(projectPath))
                    {
                        var updateResult = await toolkit.Update(contractHash, projectPath);
                        Console.WriteLine($"  ✓ {name} updated successfully!");
                        updateCount++;
                        
                        // Run migration if needed
                        try
                        {
                            await toolkit.Invoke(contractHash, "migrate", "v2.0");
                            Console.WriteLine($"  ✓ Migration completed");
                        }
                        catch
                        {
                            // Migration method might not exist
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  ⚠ Project file not found: {projectPath}");
                        failedUpdates.Add($"{name}: Project file not found");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Failed to update {name}: {ex.Message}");
                    failedUpdates.Add($"{name}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"  ⚠ Contract not found on network");
                failedUpdates.Add($"{name}: Not found on network");
            }
        }
        
        Console.WriteLine($"\n=== Update Summary ===");
        Console.WriteLine($"Successfully updated: {updateCount}");
        Console.WriteLine($"Failed: {failedUpdates.Count}");
        
        if (failedUpdates.Count > 0)
        {
            Console.WriteLine("\nFailed updates:");
            foreach (var failure in failedUpdates)
            {
                Console.WriteLine($"  - {failure}");
            }
        }
    }

    /// <summary>
    /// Example 5: Deploy contracts from CI/CD artifacts
    /// </summary>
    public static async Task DeployFromArtifacts(DeploymentToolkit toolkit)
    {
        Console.WriteLine("=== Deploying from Build Artifacts ===");
        
        var artifactPath = Environment.GetEnvironmentVariable("BUILD_ARTIFACTS_PATH") ?? "./artifacts";
        var deployedContracts = new Dictionary<string, ContractDeploymentInfo>();
        
        // Define contracts to deploy from artifacts
        var contracts = new[]
        {
            ("Core", "core-contract"),
            ("API", "api-contract"),
            ("Admin", "admin-contract")
        };
        
        foreach (var (name, artifactName) in contracts)
        {
            var nefPath = Path.Combine(artifactPath, $"{artifactName}.nef");
            var manifestPath = Path.Combine(artifactPath, $"{artifactName}.manifest.json");
            
            if (File.Exists(nefPath) && File.Exists(manifestPath))
            {
                Console.WriteLine($"\nDeploying {name} from artifacts...");
                var result = await toolkit.DeployFromArtifacts(nefPath, manifestPath);
                deployedContracts[name] = result;
                Console.WriteLine($"  ✓ {name}: {result.ContractHash}");
            }
            else
            {
                Console.WriteLine($"\n⚠ Artifacts not found for {name}");
                Console.WriteLine($"  Expected: {nefPath}");
                Console.WriteLine($"  Expected: {manifestPath}");
            }
        }
        
        // Initialize contracts after deployment
        if (deployedContracts.ContainsKey("Core"))
        {
            await toolkit.Invoke(
                deployedContracts["Core"].ContractHash,
                "initialize",
                Environment.GetEnvironmentVariable("BUILD_VERSION") ?? "1.0.0"
            );
        }
        
        Console.WriteLine($"\n=== Artifact Deployment Complete ===");
        Console.WriteLine($"Deployed {deployedContracts.Count} contracts from artifacts");
        
        // Save deployment for release notes
        await SaveDeploymentResults(deployedContracts, $"release-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json");
    }

    /// <summary>
    /// Helper: Save deployment results to JSON file
    /// </summary>
    private static async Task SaveDeploymentResults(Dictionary<string, ContractDeploymentInfo> contracts, string filename)
    {
        var record = new
        {
            timestamp = DateTime.UtcNow,
            network = "current",
            contracts = contracts.Select(kvp => new
            {
                name = kvp.Key,
                contractHash = kvp.Value.ContractHash.ToString(),
                transactionHash = kvp.Value.TransactionHash.ToString(),
                blockIndex = kvp.Value.BlockIndex,
                gasConsumed = kvp.Value.GasConsumed
            })
        };
        
        var json = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filename, json);
        Console.WriteLine($"Deployment results saved to: {filename}");
    }

    /// <summary>
    /// Helper: Save simple contract address map
    /// </summary>
    private static async Task SaveDeploymentMap(Dictionary<string, UInt160> contracts, string filename)
    {
        var map = contracts.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToString()
        );
        
        var json = JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filename, json);
    }

    /// <summary>
    /// Helper: Load contract address map
    /// </summary>
    private static async Task<Dictionary<string, string>> LoadDeploymentMap(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"Deployment file not found: {filename}");
        }
        
        var json = await File.ReadAllTextAsync(filename);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
            ?? new Dictionary<string, string>();
    }
}