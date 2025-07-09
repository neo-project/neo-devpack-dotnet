# Neo Smart Contract Deployment Toolkit

A comprehensive toolkit for deploying and managing Neo smart contracts with minimal configuration.

## Features

- **Simple Deployment**: Deploy contracts with just a few lines of code
- **Multi-Contract Deployment**: Deploy multiple interconnected contracts with dependency resolution
- **Contract Updates**: Update existing contracts with full or partial updates
- **Multiple Networks**: Support for MainNet, TestNet, and custom networks
- **Contract Compilation**: Built-in contract compilation support
- **Contract Invocation**: Easy contract method calls and invocations
- **Wallet Management**: Integrated wallet and account management
- **Batch Operations**: Deploy multiple contracts in parallel for faster deployment
- **Dependency Resolution**: Automatically determine correct deployment order
- **Contract Interactions**: Setup contract interactions after deployment
- **Rollback Support**: Rollback deployed contracts on failure
- **Manifest-Based Deployment**: Define complex deployments in JSON manifests
- **Extensibility**: Pluggable architecture with dependency injection

## Installation

```bash
dotnet add package Neo.SmartContract.Deploy
```

## Quick Start

### Deploy a Contract

```csharp
using Neo.SmartContract.Deploy;

// Initialize the toolkit
var toolkit = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("your-private-key-wif");

// Deploy from source
var deployment = await toolkit.DeployAsync("path/to/Contract.csproj");
Console.WriteLine($"Contract deployed at: {deployment.ContractHash}");
```

### Update a Contract

```csharp
// Update an existing contract
var updateInfo = await toolkit.UpdateAsync(
    "0x1234...contract-hash", 
    "path/to/UpdatedContract.csproj"
);

Console.WriteLine($"Update successful: {updateInfo.Success}");
Console.WriteLine($"Transaction: {updateInfo.TransactionHash}");
```

### Call Contract Methods

```csharp
// Read-only call
var result = await toolkit.CallAsync<string>(
    "0x1234...contract-hash",
    "getName"
);

// State-changing invocation
var txHash = await toolkit.InvokeAsync(
    "0x1234...contract-hash",
    "transfer",
    fromAddress, toAddress, amount
);
```

### Multi-Contract Deployment

Deploy multiple interconnected contracts with dependency resolution:

```csharp
// Using fluent builder
var manifest = toolkit.CreateManifestBuilder()
    .WithName("DeFi Protocol")
    .AddContract("token", "GovernanceToken", "./Token.csproj")
    .AddContract("dex", "DEX", "./DEX.csproj", c => c
        .DependsOn("token")
        .WithInitParams("@contract:token", 1000000))
    .AddInteraction("dex", "token", "approve", i => i
        .WithParams("@contract:dex", 1000000000))
    .Build();

var result = await toolkit.DeployMultipleAsync(manifest);

// Or from a manifest file
var result = await toolkit.DeployFromManifestAsync("deployment-manifest.json");
```

Key features:
- Automatic dependency resolution
- Parallel deployment of independent contracts
- Contract reference resolution (`@contract:id`)
- Post-deployment interaction setup
- Rollback on failure support
- Comprehensive deployment reporting

See [Multi-Contract Deployment Guide](../../docs/multi-contract-deployment.md) for detailed documentation.

## Core Components

### DeploymentToolkit

The main entry point providing a fluent API for common operations:
- `DeployAsync()` - Deploy new contracts
- `UpdateAsync()` - Update existing contracts
- `CallAsync()` - Make read-only contract calls
- `InvokeAsync()` - Execute state-changing transactions
- `ContractExistsAsync()` - Check if a contract exists

### Services

- **IContractCompiler** - Compiles C# source code to NEF and manifest
- **IContractDeployer** - Handles contract deployment transactions
- **IContractUpdateService** - Manages contract updates
- **IContractInvoker** - Executes contract method calls
- **IWalletManager** - Manages accounts and signatures
- **IMultiContractDeploymentService** - Orchestrates multi-contract deployments with dependencies

### Update Functionality

The toolkit provides comprehensive contract update capabilities:

#### Update Modes
- **Full Update**: Update both NEF (code) and manifest
- **NEF Only**: Update only the contract code
- **Manifest Only**: Update only the contract manifest

#### Update Process
1. Validates contract exists and has update permissions
2. Builds update script using `ContractManagement.Update`
3. Calls contract's `_deploy` method with `update=true`
4. Optionally verifies the update was applied

#### Example Contract with Update Support

```csharp
[ContractPermission("*", "*")]
public class MyContract : SmartContract
{
    public static void _deploy(object data, bool update)
    {
        if (update)
        {
            // Handle update logic
            Storage.Put(Storage.CurrentContext, "updated", true);
            
            // Process update parameters
            if (data is object[] args)
            {
                var version = (string)args[0];
                Storage.Put(Storage.CurrentContext, "version", version);
            }
        }
        else
        {
            // Initial deployment
            Storage.Put(Storage.CurrentContext, "deployed", true);
        }
    }
}
```

## Configuration

### Using appsettings.json

```json
{
  "Network": {
    "RpcUrl": "http://localhost:10332",
    "NetworkMagic": 894710606
  },
  "Deployment": {
    "GasLimit": 100000000,
    "WaitForConfirmation": true
  }
}
```

### Programmatic Configuration

```csharp
var services = new ServiceCollection();
services.AddNeoContractDeploy(configuration);
services.UseContractCompiler<CustomCompiler>();
services.UseContractUpdateService<CustomUpdateService>();

var serviceProvider = services.BuildServiceProvider();
var toolkit = serviceProvider.GetRequiredService<NeoContractToolkit>();
```

## Advanced Usage

### Custom Update Options

```csharp
var updateInfo = await toolkit.UpdateArtifactsAsync(
    contractHash,
    "path/to/contract.nef",     // null to keep existing
    "path/to/manifest.json",    // null to keep existing
    new object[] { "v2.0" }     // update parameters
);
```

### Batch Operations

```csharp
// Deploy multiple contracts from manifest
var deployments = await toolkit.DeployFromManifestAsync("deployment-manifest.json");
```

### Direct Service Usage

```csharp
// Use services directly for more control
var updater = serviceProvider.GetRequiredService<IContractUpdateService>();

// Check if update is allowed
var canUpdate = await updater.CanUpdateAsync(contractHash, rpcUrl);

// Perform update with custom options
var options = new UpdateOptions
{
    WifKey = wifKey,
    DryRun = true,  // Simulate only
    UpdateNefOnly = true
};

var result = await updater.UpdateAsync(contractHash, contract, options);
```

## Testing

The toolkit includes comprehensive unit and integration tests:

```bash
# Run all tests
dotnet test

# Run only update tests
dotnet test --filter "DisplayName~Update"

# Run integration tests (requires local Neo node)
dotnet test --filter "Category=Integration"
```

## Contributing

Contributions are welcome! Please read our contributing guidelines and submit pull requests to our repository.

## License

This project is licensed under the MIT License - see the LICENSE file for details.