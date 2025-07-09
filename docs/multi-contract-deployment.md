# Multi-Contract Deployment

The Neo Smart Contract Deployment Toolkit provides comprehensive support for deploying multiple interconnected contracts with dependency resolution, batch deployment, and automatic interaction setup.

## Features

- **Dependency Resolution**: Automatically determines the correct deployment order based on contract dependencies
- **Batch Deployment**: Deploy multiple independent contracts in parallel for faster deployment
- **Contract Interactions**: Automatically setup contract interactions after deployment
- **Rollback Support**: Rollback deployed contracts if deployment fails
- **Manifest-Based Deployment**: Define your entire deployment in a JSON manifest file
- **Fluent Builder API**: Programmatically build deployment manifests

## Quick Start

### Using the Fluent Builder

```csharp
using var toolkit = new DeploymentToolkit();
toolkit.SetNetwork("testnet").SetWifKey(yourWifKey);

var manifest = toolkit.CreateManifestBuilder()
    .WithName("My DApp Deployment")
    .WithDescription("Deploy token and governance contracts")
    .AddContract("token", "MyToken", "./contracts/MyToken.csproj", c => c
        .WithInitParams(1000000000) // Initial supply
        .WithTag("nep17"))
    .AddContract("governance", "MyGovernance", "./contracts/MyGovernance.csproj", c => c
        .DependsOn("token")
        .WithInitParams("@contract:token") // Reference to token contract
        .WithTag("governance"))
    .AddInteraction("governance", "token", "approve", i => i
        .WithParams("@contract:governance", 1000000000)
        .WithDescription("Allow governance to manage tokens"))
    .Build();

var result = await toolkit.DeployMultipleAsync(manifest);
```

### Using a Manifest File

Create a `deployment-manifest.json`:

```json
{
  "name": "My DApp Deployment",
  "version": "1.0",
  "description": "Deploy token and governance contracts",
  "contracts": [
    {
      "id": "token",
      "name": "MyToken",
      "projectPath": "./contracts/MyToken.csproj",
      "initParams": [1000000000],
      "tags": ["nep17", "token"]
    },
    {
      "id": "governance",
      "name": "MyGovernance",
      "projectPath": "./contracts/MyGovernance.csproj",
      "dependencies": ["token"],
      "initParams": ["@contract:token"],
      "tags": ["governance"]
    }
  ],
  "interactions": [
    {
      "source": "governance",
      "target": "token",
      "method": "approve",
      "params": ["@contract:governance", 1000000000],
      "description": "Allow governance to manage tokens",
      "order": 1
    }
  ],
  "settings": {
    "verifyAfterDeploy": true,
    "waitForConfirmation": true,
    "rollbackOnFailure": false
  }
}
```

Deploy from the manifest:

```csharp
var result = await toolkit.DeployFromManifestAsync("deployment-manifest.json");
```

## Deployment Manifest Structure

### Contracts

Each contract in the manifest requires:

- `id`: Unique identifier for the contract
- `name`: Contract name
- `projectPath` or `nefPath`/`manifestPath`: Source or compiled artifacts

Optional fields:

- `description`: Human-readable description
- `dependencies`: List of contract IDs that must be deployed first
- `initParams`: Initialization parameters for `_deploy` method
- `tags`: Categorization tags
- `expectedHash`: Expected contract hash for verification
- `options`: Contract-specific deployment options

### Contract References

Use special prefixes in parameters to reference other contracts:

- `@contract:contractId`: Will be replaced with the deployed contract hash
- `@deployer`: Will be replaced with the deployer account address

### Interactions

Define post-deployment contract interactions:

- `source`: Contract ID that will invoke the method
- `target`: Contract ID or hash to invoke
- `method`: Method name to call
- `params`: Method parameters (supports contract references)
- `order`: Execution order (lower numbers execute first)
- `optional`: If true, deployment continues even if interaction fails

## Advanced Features

### Batch Deployment

Enable parallel deployment of independent contracts:

```csharp
var manifest = toolkit.CreateManifestBuilder()
    .WithName("Batch Deployment")
    .EnableBatching(5) // Deploy up to 5 contracts in parallel
    // ... add contracts
    .Build();
```

### Dependency Resolution

The deployment service automatically:

1. Analyzes contract dependencies
2. Detects circular dependencies
3. Determines optimal deployment order
4. Ensures dependencies are deployed before dependent contracts

### Error Handling

Configure error handling behavior:

```csharp
var manifest = toolkit.CreateManifestBuilder()
    .WithName("Deployment")
    .ContinueOnError(true) // Continue deploying even if some contracts fail
    .WithSettings(s =>
    {
        s.RollbackOnFailure = true; // Rollback all contracts if any fail
    })
    .Build();
```

### Deployment Results

The deployment returns a comprehensive result object:

```csharp
var result = await toolkit.DeployMultipleAsync(manifest);

// Check overall status
Console.WriteLine($"Status: {result.Status}");
Console.WriteLine($"Deployed: {result.DeployedContracts.Count}");
Console.WriteLine($"Failed: {result.FailedDeployments.Count}");

// Access deployed contract information
foreach (var (contractId, info) in result.DeployedContracts)
{
    Console.WriteLine($"{contractId}: {info.ContractHash}");
}

// Check interaction results
foreach (var interaction in result.InteractionResults)
{
    Console.WriteLine($"{interaction.Description}: {interaction.Success}");
}
```

## Best Practices

1. **Use Descriptive IDs**: Contract IDs should be meaningful and unique
2. **Specify Dependencies**: Always declare dependencies to ensure correct deployment order
3. **Verify Expected Hashes**: Use `expectedHash` for critical contracts to ensure deployment integrity
4. **Handle Failures Gracefully**: Use `continueOnError` for non-critical contracts
5. **Test Locally First**: Always test multi-contract deployments on a local network
6. **Use Tags**: Tag contracts for easier filtering and organization
7. **Document Interactions**: Provide clear descriptions for all contract interactions

## Example Scenarios

### DeFi Protocol Deployment

```csharp
var manifest = toolkit.CreateManifestBuilder()
    .WithName("DeFi Protocol")
    .AddContract("token", "GovernanceToken", "./Token.csproj")
    .AddContract("oracle", "PriceOracle", "./Oracle.csproj")
    .AddContract("lending", "LendingPool", "./Lending.csproj", c => c
        .DependsOn("token", "oracle")
        .WithInitParams("@contract:token", "@contract:oracle"))
    .AddContract("staking", "StakingRewards", "./Staking.csproj", c => c
        .DependsOn("token")
        .WithInitParams("@contract:token", 100)) // 100 tokens per block
    .Build();
```

### NFT Marketplace Deployment

```csharp
var manifest = toolkit.CreateManifestBuilder()
    .WithName("NFT Marketplace")
    .AddContract("nft", "ArtworkNFT", "./NFT.csproj")
    .AddContract("marketplace", "NFTMarketplace", "./Marketplace.csproj", c => c
        .DependsOn("nft")
        .WithInitParams("@contract:nft", 250)) // 2.5% fee
    .AddContract("auction", "NFTAuction", "./Auction.csproj", c => c
        .DependsOn("nft", "marketplace")
        .WithInitParams("@contract:nft", "@contract:marketplace"))
    .AddInteraction("nft", "marketplace", "authorize", i => i
        .WithParams("@contract:marketplace", true))
    .AddInteraction("nft", "auction", "authorize", i => i
        .WithParams("@contract:auction", true))
    .Build();
```

## Troubleshooting

### Common Issues

1. **Circular Dependencies**: Check your dependency graph for cycles
2. **Missing Dependencies**: Ensure all referenced contracts are included in the manifest
3. **Parameter Resolution**: Verify contract references use the correct format (@contract:id)
4. **Gas Limits**: Increase gas limits for complex deployments
5. **Network Issues**: Ensure stable RPC connection for batch deployments

### Debugging Tips

- Enable verbose logging to see detailed deployment progress
- Use dry run mode to validate deployment without spending GAS
- Deploy contracts individually first to isolate issues
- Check contract compilation errors in the failure details
- Verify initialization parameters match contract expectations

## API Reference

### IMultiContractDeploymentService

The core service interface for multi-contract deployment:

```csharp
public interface IMultiContractDeploymentService
{
    Task<MultiContractDeploymentResult> DeployMultipleAsync(
        DeploymentManifest manifest, 
        DeploymentOptions options);
    
    Task<MultiContractDeploymentResult> DeployFromManifestAsync(
        string manifestPath, 
        DeploymentOptions options);
    
    List<ContractDefinition> ResolveDependencyOrder(
        IList<ContractDefinition> contracts);
    
    Task<ContractInteractionSetupResult> SetupContractInteractionsAsync(
        MultiContractDeploymentResult deploymentResult,
        IList<ContractInteraction> interactions,
        InvocationOptions options);
    
    Task<RollbackResult> RollbackDeploymentAsync(
        IList<ContractDeploymentInfo> deployedContracts,
        DeploymentOptions options);
}
```

### DeploymentManifestBuilder

Fluent API for building deployment manifests:

```csharp
public class DeploymentManifestBuilder
{
    DeploymentManifestBuilder WithName(string name);
    DeploymentManifestBuilder WithDescription(string description);
    DeploymentManifestBuilder ContinueOnError(bool continueOnError = true);
    DeploymentManifestBuilder EnableBatching(int batchSize = 5);
    DeploymentManifestBuilder WithSettings(Action<DeploymentSettings> configure);
    
    DeploymentManifestBuilder AddContract(
        string id, 
        string name, 
        string projectPath,
        Action<ContractDefinitionBuilder>? configure = null);
    
    DeploymentManifestBuilder AddCompiledContract(
        string id,
        string name,
        string nefPath,
        string manifestPath,
        Action<ContractDefinitionBuilder>? configure = null);
    
    DeploymentManifestBuilder AddInteraction(
        string source,
        string target,
        string method,
        Action<ContractInteractionBuilder>? configure = null);
    
    DeploymentManifest Build();
    Task SaveAsync(string path);
}
```