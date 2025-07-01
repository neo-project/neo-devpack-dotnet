# Neo.SmartContract.Deploy

A comprehensive deployment toolkit for Neo N3 smart contracts, providing infrastructure for deploying, initializing, and managing smart contracts.

## Features

- **Modular Deployment System**: Step-based deployment process for complex scenarios
- **Wallet Management**: Secure wallet handling with environment variable support
- **Transaction Management**: Automatic gas estimation and transaction confirmation
- **Multi-Contract Support**: Deploy multiple contracts with dependencies
- **Configuration**: Environment-based configuration (Development, TestNet, MainNet)
- **Utilities**: Contract invocation helpers and deployment utilities

## Installation

```bash
dotnet add package Neo.SmartContract.Deploy
```

## Quick Start

### 1. Configure Services

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Neo.SmartContract.Deploy;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();

// Add deployment services
services.AddNeoDeploymentServices(configuration);

// Add deployment steps
services.AddDeploymentStep<MyContractDeploymentStep>();

var serviceProvider = services.BuildServiceProvider();
```

### 2. Create a Deployment Step

```csharp
using Neo.SmartContract.Deploy.Steps;

public class MyContractDeploymentStep : BaseDeploymentStep
{
    public MyContractDeploymentStep(ILogger<MyContractDeploymentStep> logger) 
        : base(logger) { }

    public override string Name => "Deploy MyContract";
    public override int Order => 10;

    public override async Task<bool> ExecuteAsync(DeploymentContext context)
    {
        // Load contract
        var contract = await context.ContractLoader.LoadContractAsync("MyContract");
        
        // Deploy contract
        var result = await context.DeploymentService.DeployContractAsync(
            contract.Name,
            contract.NefBytes,
            contract.Manifest
        );

        if (!result.Success)
        {
            Logger.LogError("Failed to deploy contract: {Error}", result.ErrorMessage);
            return false;
        }

        // Store deployed contract hash
        context.DeployedContracts[contract.Name] = result.Hash!;
        
        // Initialize contract
        await context.DeploymentService.InvokeContractAsync(
            result.Hash!,
            "initialize"
        );

        return true;
    }
}
```

### 3. Configure appsettings.json

```json
{
  "Network": {
    "RpcUrl": "http://localhost:10332",
    "Network": "private"
  },
  "Wallet": {
    "WalletPath": "wallet.json",
    "DefaultAccount": null
  },
  "Deployment": {
    "WaitForConfirmation": true,
    "ConfirmationRetries": 30,
    "ConfirmationDelaySeconds": 5,
    "ContractsPath": "contracts"
  }
}
```

### 4. Deploy Contracts

```csharp
var deploymentService = serviceProvider.GetRequiredService<IDeploymentService>();
var result = await deploymentService.DeployAllAsync();

if (result.Success)
{
    Console.WriteLine("Deployment successful!");
    foreach (var contract in result.DeployedContracts)
    {
        Console.WriteLine($"{contract.Name}: {contract.Hash}");
    }
}
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
       "ConfigFile": "default.neo-express",
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