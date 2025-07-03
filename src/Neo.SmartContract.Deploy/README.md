# Neo Smart Contract Deployment Toolkit

A comprehensive deployment toolkit for Neo N3 smart contracts, providing infrastructure for compiling, deploying, initializing, and managing smart contracts.

## Features

### Two Deployment Approaches

#### 1. Compile-and-Deploy Approach
- Specify compilation options for each contract
- Compile contracts from source code
- Deploy compiled contracts to the network
- Get deployment results with contract addresses
- Invoke deployed contracts for setup and initialization

#### 2. Artifact-based Deploy Approach
- Use pre-compiled NEF and manifest files
- Deploy artifacts directly to the network
- Skip compilation step for faster deployment
- Ideal for production deployments with CI/CD pipelines

### Core Capabilities

- **Contract Compilation**: Compile smart contracts from C# source code with customizable options
- **Contract Deployment**: Deploy contracts to Neo N3 networks with transaction management
- **Contract Updates**: Update existing contracts with new versions
- **Contract Invocation**: Invoke contract methods for setup, initialization, and data retrieval
- **Wallet Management**: Integrate with Neo wallets for transaction signing
- **Multi-contract Support**: Deploy multiple contracts with dependencies in sequence
- **Network Awareness**: Support for different Neo networks (MainNet, TestNet, Private)

## Installation

```bash
dotnet add package Neo.SmartContract.Deploy
```

## Quick Start

### Basic Usage

```csharp
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Contracts;

// Create toolkit instance
var toolkit = ServiceCollectionExtensions.CreateDefaultToolkit();

// Load wallet for signing transactions
await toolkit.LoadWalletAsync("path/to/wallet.json", "password");
var deployerAccount = toolkit.GetDeployerAccount();

// Approach 1: Compile and deploy from source
var compilationOptions = new CompilationOptions
{
    SourcePath = "MyContract.cs",
    OutputDirectory = "bin/contracts",
    ContractName = "MyContract"
};

var deploymentOptions = new DeploymentOptions
{
    RpcUrl = "http://localhost:10332",
    DeployerAccount = deployerAccount,
    GasLimit = 50_000_000 // 0.5 GAS
};

var result = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
Console.WriteLine($"Contract deployed: {result.ContractHash}");

// Initialize the deployed contract
var invocationOptions = new InvocationOptions
{
    RpcUrl = "http://localhost:10332",
    SignerAccount = deployerAccount,
    GasLimit = 20_000_000 // 0.2 GAS
};

await toolkit.InvokeContractAsync(
    result.ContractHash,
    "initialize",
    new object[] { "setup parameter" },
    invocationOptions
);
```

### Artifact-based Deployment

```csharp
// Approach 2: Deploy from pre-compiled artifacts
var result = await toolkit.DeployFromArtifactsAsync(
    "MyContract.nef",
    "MyContract.manifest.json",
    deploymentOptions
);
```

### Multiple Contracts with Dependencies

```csharp
// Deploy Token Contract first
var tokenResult = await toolkit.CompileAndDeployAsync(tokenOptions, deploymentOptions);

// Deploy Governance Contract
var govResult = await toolkit.CompileAndDeployAsync(govOptions, deploymentOptions);

// Initialize Governance with Token reference
await toolkit.InvokeContractAsync(
    govResult.ContractHash,
    "initialize",
    new object[] { tokenResult.ContractHash }, // Pass token hash as dependency
    invocationOptions
);

// Deploy Main Contract that depends on both
var mainResult = await toolkit.CompileAndDeployAsync(mainOptions, deploymentOptions);

// Initialize Main Contract with both dependencies
await toolkit.InvokeContractAsync(
    mainResult.ContractHash,
    "initialize",
    new object[] { 
        tokenResult.ContractHash,
        govResult.ContractHash 
    },
    invocationOptions
);
```

## Advanced Usage

### Multi-Contract Deployment with Dependencies

```csharp
public class TokenAndGovernanceDeploymentStep : BaseDeploymentStep
{
    private readonly IContractInvoker _invoker;

    public override string Name => "Deploy Token and Governance";
    public override int Order => 20;

    public override async Task<bool> ExecuteAsync(DeploymentContext context)
    {
        // Deploy token contract
        var tokenContract = await context.ContractLoader.LoadContractAsync("TokenContract");
        var tokenResult = await context.DeploymentService.DeployContractAsync(
            tokenContract.Name,
            tokenContract.NefBytes,
            tokenContract.Manifest
        );

        // Deploy governance contract
        var govContract = await context.ContractLoader.LoadContractAsync("GovernanceContract");
        var govResult = await context.DeploymentService.DeployContractAsync(
            govContract.Name,
            govContract.NefBytes,
            govContract.Manifest
        );

        // Configure governance to manage token
        await context.DeploymentService.InvokeContractAsync(
            govResult.Hash!,
            "setTokenContract",
            tokenResult.Hash!
        );

        return true;
    }
}
```

### Using Contract Invoker

```csharp
var invoker = serviceProvider.GetRequiredService<IContractInvoker>();

// Call without transaction (read-only)
var balance = await invoker.CallAsync<BigInteger>(
    contractHash, 
    "balanceOf", 
    accountHash
);

// Send transaction
var txHash = await invoker.SendAsync(
    contractHash,
    "transfer",
    fromAccount,
    toAccount,
    amount
);

// Wait for confirmation
await invoker.WaitForConfirmationAsync(txHash);
```

## Configuration Options

### DeploymentOptions
- `WaitForConfirmation`: Whether to wait for transaction confirmation
- `ConfirmationRetries`: Number of retries when waiting
- `ConfirmationDelaySeconds`: Delay between confirmation checks
- `ContractsPath`: Path to compiled contracts directory

### NetworkOptions
- `RpcUrl`: Neo RPC endpoint URL
- `Network`: Network name (private, testnet, mainnet)

### WalletOptions
- `WalletPath`: Path to NEP-6 wallet file
- `Password`: Wallet password (or use WALLET_PASSWORD env var)
- `DefaultAccount`: Default account address

## Neo Express Integration (Optional)

The deployment toolkit has optional support for Neo Express, making local development and debugging easier. Neo Express is NOT required - you can use any Neo node (local or remote).

### When to Use Neo Express

Neo Express is recommended for:
- Local development and testing
- Quick prototyping
- Debugging smart contracts
- CI/CD pipelines

For production deployments, use:
- Neo TestNet
- Neo MainNet
- Private Neo networks

### Setup Neo Express (Optional)

1. Install Neo Express (if desired):
   ```bash
   dotnet tool install -g Neo.Express
   ```

2. Enable in your code:
   ```csharp
   // In your Program.cs or Startup.cs
   services.AddNeoDeploymentServices(configuration);
   
   // Optionally add Neo Express support
   if (configuration.GetValue<bool>("Deployment:UseNeoExpress"))
   {
       services.AddNeoExpressSupport(configuration);
   }
   ```

3. Configure in appsettings.json:
   ```json
   {
     "Deployment": {
       "UseNeoExpress": true  // Set to true only if you want to use Neo Express
     },
     "NeoExpress": {
       "ConfigFile": "neo-express.json",
       "AutoStart": true,
       "AutoCreate": true,
       "EnableCheckpoints": true,
       "SecondsPerBlock": 1
     }
   }
   ```

4. When enabled, the deployment toolkit will:
   - Create a Neo Express instance if needed
   - Start Neo Express
   - Create deployment wallets
   - Transfer GAS for deployments

### Debugging Features

1. **Transaction Details**: Enable debug logging to see detailed transaction information
   ```bash
   dotnet run --Logging:LogLevel:Default=Debug
   ```

2. **Invoke Results**: All contract invocations show detailed results in debug mode

3. **Storage Inspection**: Use DebugHelper to inspect contract storage
   ```csharp
   await DebugHelper.PrintStorageAsync(logger, blockchain, contractHash, storageKeys);
   ```

4. **Checkpoints**: Create checkpoints at any point during deployment
   ```csharp
   await DebugHelper.CreateDebugCheckpointAsync(logger, neoExpress, "checkpoint_name", "description");
   ```

### Neo Express Commands

Useful Neo Express commands for debugging:

```bash
# Show blockchain info
neoxp show

# List wallets
neoxp wallet list

# Show transaction
neoxp show transaction <txid>

# Create checkpoint
neoxp checkpoint create <name>

# Restore checkpoint
neoxp checkpoint restore <name>

# Reset blockchain
neoxp reset -f
```

## Contract Updates

The deployment toolkit tracks all deployments and supports updating existing contracts:

### Automatic Deploy/Update

Use `DeployOrUpdateContractStep` to automatically deploy new contracts or update existing ones:

```csharp
services.AddTransient<IDeploymentStep>(sp => 
    new DeployOrUpdateContractStep(
        sp.GetRequiredService<ILogger<DeployOrUpdateContractStep>>(),
        sp.GetRequiredService<IDeploymentService>(),
        sp.GetRequiredService<IContractUpdateService>(),
        sp.GetRequiredService<IDeploymentRecordService>(),
        sp.GetRequiredService<IContractLoader>(),
        sp.GetRequiredService<IOptions<NetworkOptions>>(),
        "MyContract",
        "2.0.0" // Optional version
    ));
```

### Manual Update

```csharp
var updateService = serviceProvider.GetRequiredService<IContractUpdateService>();

// Check if update is possible
var checkResult = await updateService.CheckUpdateAsync("MyContract", "testnet");
if (checkResult.CanUpdate)
{
    // Update the contract
    var result = await updateService.UpdateContractAsync(
        "MyContract",
        "testnet",
        nefBytes,
        manifest,
        updateData: null,
        version: "2.0.0"
    );
}
```

### Deployment Records

All deployments are tracked in `.deployments/` directory with network-specific records:

```json
{
  "testnet": {
    "contractHash": "0x1234...",
    "transactionHash": "0x5678...",
    "deployedAt": "2024-01-01T00:00:00Z",
    "version": "1.0.0",
    "updateHistory": [
      {
        "transactionHash": "0x9abc...",
        "updatedAt": "2024-01-02T00:00:00Z",
        "previousVersion": "1.0.0",
        "newVersion": "1.1.0"
      }
    ]
  }
}
```

### Managing Deployments

Use `DeploymentManager` utility:

```csharp
var manager = serviceProvider.GetRequiredService<DeploymentManager>();

// List all deployments on a network
await manager.ListDeploymentsAsync("testnet");

// Show detailed deployment info
await manager.ShowDeploymentAsync("MyContract", "testnet");

// Export deployment records
var json = await manager.ExportDeploymentsAsync();

// Create deployment report
var report = await manager.CreateDeploymentReportAsync();
```

### Important Notes

- Deployment records contain contract addresses - keep them secure
- Contract must have an `update` method to be updatable
- Version numbers are automatically incremented if not specified
- Update history is maintained for audit purposes

## Security Considerations

1. **Never commit wallet files or passwords** to version control
2. **Never commit deployment records** to public repositories
3. Use environment variables for sensitive configuration
4. Ensure proper access controls on wallet files
5. Use separate wallets for different environments
6. Monitor deployment transactions and contract state
7. Neo Express is for development only - never use it in production
8. Verify contract hashes after deployment/update

## License

MIT License - see LICENSE file for details