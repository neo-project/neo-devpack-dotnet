# Neo Smart Contract Deployment Project

This project is where you write explicit C# code to deploy your Neo N3 smart contracts using the Neo Smart Contract Deployment Toolkit.

## Overview

This is NOT a command-line tool. This is YOUR deployment program where you write C# code to:

- Compile contracts from source code
- Deploy contracts in a specific order
- Initialize contracts with parameters
- Set up dependencies between contracts
- Update existing contracts
- Handle complex deployment scenarios

You control everything through explicit C# code in `Program.cs`.

## Getting Started

### 1. Prepare Your Wallet

Create a wallet directory and place your NEP-6 wallet file there:

```bash
mkdir wallets
cp /path/to/your/wallet.json wallets/development.json
```

### 2. Configure Your Deployment

Edit `Program.cs` to configure your deployment:

```csharp
// Load your wallet
await toolkit.LoadWalletAsync("wallets/development.json", "your-password");

// Configure compilation options
var compilationOptions = new CompilationOptions
{
    SourcePath = "../../src/MyContract/MyContract.cs",
    OutputDirectory = "bin/contracts",
    ContractName = "MyContract"
};

// Configure deployment options
var deploymentOptions = new DeploymentOptions
{
    RpcUrl = "http://localhost:10332", // Your Neo node RPC
    DeployerAccount = deployerAccount,
    GasLimit = 50_000_000 // 0.5 GAS
};
```

### 3. Run Your Deployment

```bash
# Run your deployment program
dotnet run
```

That's it! Your C# code in `Program.cs` controls everything.

## Deployment Scenarios

### Simple Contract Deployment

```csharp
// Compile and deploy
var result = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
Console.WriteLine($"Contract deployed: {result.ContractHash}");

// Initialize the contract
await toolkit.InvokeContractAsync(
    result.ContractHash,
    "initialize",
    new object[] { "parameter1", 123 },
    invocationOptions
);
```

### Multiple Contracts with Dependencies

```csharp
// Deploy Token Contract
var tokenResult = await toolkit.CompileAndDeployAsync(tokenOptions, deploymentOptions);

// Deploy Governance Contract
var govResult = await toolkit.CompileAndDeployAsync(govOptions, deploymentOptions);

// Initialize Governance with Token dependency
await toolkit.InvokeContractAsync(
    govResult.ContractHash,
    "setTokenContract",
    new object[] { tokenResult.ContractHash },
    invocationOptions
);
```

### Deploy from Artifacts

```csharp
// Deploy from pre-compiled files
var result = await toolkit.DeployFromArtifactsAsync(
    "bin/contracts/MyContract.nef",
    "bin/contracts/MyContract.manifest.json",
    deploymentOptions
);
```

## Contract Updates

To update an existing contract:

```csharp
// Check if contract exists
var contractHash = UInt160.Parse("0x...");
if (await toolkit.ContractExistsAsync(contractHash, rpcUrl))
{
    // Update the contract
    var updateResult = await toolkit.UpdateContractAsync(
        contractHash,
        compilationOptions,
        deploymentOptions
    );
}
```

## Writing Your Deployment Logic

### Modify Program.cs

The `Program.cs` file contains example deployment methods. Modify the `Main` method to call your deployment logic:

```csharp
static async Task<int> Main(string[] args)
{
    // ... initialization code ...
    
    // Call YOUR deployment methods here:
    await DeployMyProtocol(toolkit, deployerAccount, rpcUrl);
    
    // Or deploy from artifacts:
    await DeployFromCompiledArtifacts(toolkit, deployerAccount, rpcUrl);
    
    // Or update contracts:
    await UpdateMyContracts(toolkit, deployerAccount, rpcUrl);
}
```

### Custom Deployment Example

```csharp
static async Task DeployMyProtocol(NeoContractToolkit toolkit, UInt160 deployerAccount, string rpcUrl)
{
    var deployOptions = new DeploymentOptions
    {
        RpcUrl = rpcUrl,
        DeployerAccount = deployerAccount,
        GasLimit = 100_000_000
    };

    // Deploy contracts in order
    var oracleHash = await DeployContract(toolkit, "Oracle", deployOptions);
    var tokenHash = await DeployContract(toolkit, "Token", deployOptions);
    var vaultHash = await DeployContract(toolkit, "Vault", deployOptions);
    
    // Initialize with dependencies
    await InitializeVault(toolkit, vaultHash, tokenHash, oracleHash, deployOptions.RpcUrl);
    
    // Save deployment info
    SaveDeploymentInfo(oracleHash, tokenHash, vaultHash);
}
```

## Advanced Usage

### Using the Builder Pattern

```csharp
var toolkit = NeoContractToolkitBuilder.Create()
    .ConfigureLogging(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Debug);
    })
    .UseCompiler<MyCustomCompiler>()  // Optional: custom implementations
    .UseDeployer<MyCustomDeployer>()
    .Build();
```

### Environment-Specific Deployments

```csharp
var environment = Environment.GetEnvironmentVariable("DEPLOY_ENV") ?? "development";

var (rpcUrl, walletPath) = environment switch
{
    "production" => ("https://rpc.n3.neotube.io:8080", "wallets/mainnet.json"),
    "staging" => ("https://rpc.t5.n3.neotube.io:8080", "wallets/testnet.json"),
    _ => ("http://localhost:10332", "wallets/development.json")
};
```

### Deployment Records

Keep track of your deployments:

```csharp
public class DeploymentRecord
{
    public string Network { get; set; }
    public DateTime DeployedAt { get; set; }
    public Dictionary<string, UInt160> Contracts { get; set; }
}

static void SaveDeploymentRecord(string network, Dictionary<string, UInt160> contracts)
{
    var record = new DeploymentRecord
    {
        Network = network,
        DeployedAt = DateTime.UtcNow,
        Contracts = contracts
    };
    
    var json = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText($"deployments/{network}.json", json);
}
```

## Security Notes

- **Never commit wallet files** to version control
- **Use environment variables** for sensitive data like passwords
- **Test on TestNet** before deploying to MainNet
- **Verify contract hashes** after deployment

## Next Steps

1. Customize the deployment logic in `Program.cs`
2. Add your contract source files to the `src/` directory
3. Configure network endpoints for your target environment
4. Run the deployment and monitor the results

For more information, see the [Neo Smart Contract Deployment Toolkit documentation](https://github.com/neo-project/neo-devpack-dotnet).