using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Contracts;
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
    public static async Task DeployDeFiProtocol(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("DeFi");
        
        var deploymentOptions = new DeploymentOptions
        {
            RpcUrl = "http://localhost:10332",
            DeployerAccount = deployerAccount,
            GasLimit = 100_000_000 // 1 GAS per contract
        };

        var invocationOptions = new InvocationOptions
        {
            RpcUrl = deploymentOptions.RpcUrl,
            SignerAccount = deployerAccount,
            GasLimit = 20_000_000 // 0.2 GAS
        };

        logger.LogInformation("=== Deploying DeFi Protocol ===");

        // 1. Deploy Oracle Contract
        logger.LogInformation("Deploying Oracle Contract...");
        var oracleOptions = new CompilationOptions
        {
            SourcePath = "../../src/Oracle/OracleContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "Oracle"
        };
        var oracleResult = await toolkit.CompileAndDeployAsync(oracleOptions, deploymentOptions);
        logger.LogInformation("Oracle deployed: {Hash}", oracleResult.ContractHash);

        // 2. Deploy Token Contract
        logger.LogInformation("Deploying Token Contract...");
        var tokenOptions = new CompilationOptions
        {
            SourcePath = "../../src/Token/TokenContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "DeFiToken"
        };
        var tokenResult = await toolkit.CompileAndDeployAsync(tokenOptions, deploymentOptions);
        
        // Initialize token with name, symbol, decimals, and total supply
        await toolkit.InvokeContractAsync(
            tokenResult.ContractHash,
            "initialize",
            new object[] { "DeFi Token", "DFT", 8, 1000000_00000000L },
            invocationOptions
        );
        logger.LogInformation("Token deployed and initialized: {Hash}", tokenResult.ContractHash);

        // 3. Deploy Lending Pool Contract
        logger.LogInformation("Deploying Lending Pool Contract...");
        var lendingOptions = new CompilationOptions
        {
            SourcePath = "../../src/LendingPool/LendingPoolContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "LendingPool"
        };
        var lendingResult = await toolkit.CompileAndDeployAsync(lendingOptions, deploymentOptions);
        
        // Initialize lending pool with token and oracle dependencies
        await toolkit.InvokeContractAsync(
            lendingResult.ContractHash,
            "initialize",
            new object[] { 
                tokenResult.ContractHash,     // Collateral token
                oracleResult.ContractHash,     // Price oracle
                80,                            // 80% collateralization ratio
                500                            // 5% annual interest rate (in basis points)
            },
            invocationOptions
        );
        logger.LogInformation("Lending Pool deployed and initialized: {Hash}", lendingResult.ContractHash);

        // 4. Deploy Governance Contract
        logger.LogInformation("Deploying Governance Contract...");
        var govOptions = new CompilationOptions
        {
            SourcePath = "../../src/Governance/GovernanceContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "Governance"
        };
        var govResult = await toolkit.CompileAndDeployAsync(govOptions, deploymentOptions);
        
        // Initialize governance with all protocol contracts
        await toolkit.InvokeContractAsync(
            govResult.ContractHash,
            "initialize",
            new object[] { 
                tokenResult.ContractHash,      // Governance token
                lendingResult.ContractHash,    // Lending pool to govern
                oracleResult.ContractHash,     // Oracle to govern
                259200                         // 3 day voting period (in seconds)
            },
            invocationOptions
        );
        logger.LogInformation("Governance deployed and initialized: {Hash}", govResult.ContractHash);

        // 5. Configure cross-contract permissions
        logger.LogInformation("Configuring contract permissions...");
        
        // Allow lending pool to transfer tokens
        await toolkit.InvokeContractAsync(
            tokenResult.ContractHash,
            "addAuthorizedContract",
            new object[] { lendingResult.ContractHash },
            invocationOptions
        );
        
        // Allow governance to update lending pool parameters
        await toolkit.InvokeContractAsync(
            lendingResult.ContractHash,
            "setGovernance",
            new object[] { govResult.ContractHash },
            invocationOptions
        );

        logger.LogInformation("=== DeFi Protocol Deployment Complete ===");
        logger.LogInformation("Oracle Contract: {Hash}", oracleResult.ContractHash);
        logger.LogInformation("Token Contract: {Hash}", tokenResult.ContractHash);
        logger.LogInformation("Lending Pool: {Hash}", lendingResult.ContractHash);
        logger.LogInformation("Governance: {Hash}", govResult.ContractHash);
    }

    /// <summary>
    /// Deploy an NFT marketplace with royalties
    /// </summary>
    public static async Task DeployNFTMarketplace(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("NFT");
        
        var deploymentOptions = new DeploymentOptions
        {
            RpcUrl = "http://localhost:10332",
            DeployerAccount = deployerAccount,
            GasLimit = 80_000_000 // 0.8 GAS
        };

        var invocationOptions = new InvocationOptions
        {
            RpcUrl = deploymentOptions.RpcUrl,
            SignerAccount = deployerAccount,
            GasLimit = 20_000_000 // 0.2 GAS
        };

        logger.LogInformation("=== Deploying NFT Marketplace ===");

        // 1. Deploy NFT Contract
        logger.LogInformation("Deploying NFT Contract...");
        var nftOptions = new CompilationOptions
        {
            SourcePath = "../../src/NFT/NFTContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "NFTCollection"
        };
        var nftResult = await toolkit.CompileAndDeployAsync(nftOptions, deploymentOptions);
        
        await toolkit.InvokeContractAsync(
            nftResult.ContractHash,
            "initialize",
            new object[] { 
                "Neo Art Collection", 
                "NAC",
                "https://api.neoart.io/metadata/",
                deployerAccount // Initial minter
            },
            invocationOptions
        );

        // 2. Deploy Marketplace Contract
        logger.LogInformation("Deploying Marketplace Contract...");
        var marketOptions = new CompilationOptions
        {
            SourcePath = "../../src/Marketplace/MarketplaceContract.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "NFTMarketplace"
        };
        var marketResult = await toolkit.CompileAndDeployAsync(marketOptions, deploymentOptions);
        
        await toolkit.InvokeContractAsync(
            marketResult.ContractHash,
            "initialize",
            new object[] { 
                250,  // 2.5% marketplace fee
                1000  // 10% max royalty rate
            },
            invocationOptions
        );

        // 3. Deploy Royalty Registry
        logger.LogInformation("Deploying Royalty Registry...");
        var royaltyOptions = new CompilationOptions
        {
            SourcePath = "../../src/Royalty/RoyaltyRegistry.cs",
            OutputDirectory = "bin/contracts",
            ContractName = "RoyaltyRegistry"
        };
        var royaltyResult = await toolkit.CompileAndDeployAsync(royaltyOptions, deploymentOptions);

        // 4. Configure marketplace to use royalty registry
        await toolkit.InvokeContractAsync(
            marketResult.ContractHash,
            "setRoyaltyRegistry",
            new object[] { royaltyResult.ContractHash },
            invocationOptions
        );

        // 5. Whitelist NFT collection on marketplace
        await toolkit.InvokeContractAsync(
            marketResult.ContractHash,
            "whitelistCollection",
            new object[] { nftResult.ContractHash },
            invocationOptions
        );

        logger.LogInformation("=== NFT Marketplace Deployment Complete ===");
        logger.LogInformation("NFT Collection: {Hash}", nftResult.ContractHash);
        logger.LogInformation("Marketplace: {Hash}", marketResult.ContractHash);
        logger.LogInformation("Royalty Registry: {Hash}", royaltyResult.ContractHash);
    }

    /// <summary>
    /// Deploy contracts from pre-compiled artifacts (CI/CD scenario)
    /// </summary>
    public static async Task DeployFromArtifacts(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Artifacts");
        
        var deploymentOptions = new DeploymentOptions
        {
            RpcUrl = Environment.GetEnvironmentVariable("NEO_RPC_URL") ?? "http://localhost:10332",
            DeployerAccount = deployerAccount,
            GasLimit = 50_000_000 // 0.5 GAS
        };

        var invocationOptions = new InvocationOptions
        {
            RpcUrl = deploymentOptions.RpcUrl,
            SignerAccount = deployerAccount,
            GasLimit = 20_000_000 // 0.2 GAS
        };

        logger.LogInformation("=== Deploying from Artifacts ===");

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
            logger.LogInformation("Deploying {Name} from artifacts...", name);
            
            var nefPath = Path.Combine(artifactPath, $"{artifact}.nef");
            var manifestPath = Path.Combine(artifactPath, $"{artifact}.manifest.json");
            
            if (!File.Exists(nefPath) || !File.Exists(manifestPath))
            {
                logger.LogError("Artifact files not found for {Name}", name);
                continue;
            }

            var result = await toolkit.DeployFromArtifactsAsync(nefPath, manifestPath, deploymentOptions);
            deployedContracts[name] = result.ContractHash;
            
            // Resolve contract references in init params
            var resolvedParams = ResolveContractReferences(initParams, deployedContracts);
            
            // Initialize if params provided
            if (resolvedParams.Length > 0)
            {
                await toolkit.InvokeContractAsync(
                    result.ContractHash,
                    "initialize",
                    resolvedParams,
                    invocationOptions
                );
            }
            
            logger.LogInformation("{Name} deployed: {Hash}", name, result.ContractHash);
        }

        logger.LogInformation("=== Artifact Deployment Complete ===");
    }

    /// <summary>
    /// Update existing contracts scenario
    /// </summary>
    public static async Task UpdateContracts(NeoContractToolkit toolkit, UInt160 deployerAccount)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Update");
        
        var deploymentOptions = new DeploymentOptions
        {
            RpcUrl = "http://localhost:10332",
            DeployerAccount = deployerAccount,
            GasLimit = 60_000_000 // 0.6 GAS for updates
        };

        logger.LogInformation("=== Updating Contracts ===");

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
            if (await toolkit.ContractExistsAsync(contractHash, deploymentOptions.RpcUrl))
            {
                logger.LogInformation("Updating {Name} at {Hash}...", name, contractHash);
                
                var compilationOptions = new CompilationOptions
                {
                    SourcePath = $"../../src/{name}/{name}.cs",
                    OutputDirectory = "bin/contracts",
                    ContractName = name
                };

                try
                {
                    var updateResult = await toolkit.UpdateContractAsync(
                        contractHash,
                        compilationOptions,
                        deploymentOptions
                    );
                    
                    logger.LogInformation("{Name} updated successfully. Tx: {TxHash}", 
                        name, updateResult.TransactionHash);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to update {Name}", name);
                }
            }
            else
            {
                logger.LogWarning("Contract {Name} not found at {Hash}", name, contractHash);
            }
        }

        logger.LogInformation("=== Contract Updates Complete ===");
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
}