# Neo Smart Contract Deployment Project

This project uses the Neo Smart Contract Deployment Toolkit to deploy your Neo N3 smart contracts with a simplified API.

## Overview

Deploy your Neo smart contracts with just a few lines of code:

```csharp
var toolkit = new DeploymentToolkit();
var result = await toolkit.Deploy("path/to/contract.csproj");
```

## Getting Started

### 1. Configure Your Environment

Edit `appsettings.json` to configure your deployment environment:

```json
{
  "Network": {
    "DefaultNetwork": "testnet",
    "Networks": {
      "mainnet": {
        "RpcUrl": "http://seed1.neo.org:10332"
      },
      "testnet": {
        "RpcUrl": "http://seed1.testnet.neo.org:20332"
      },
      "local": {
        "RpcUrl": "http://localhost:50012"
      }
    },
    "Wallet": {
      "Path": "wallets/deployment.json",
      "Password": "your-password"
    }
  }
}
```

### 2. Prepare Your Wallet

Create a wallet directory and place your NEP-6 wallet file there:

```bash
mkdir wallets
cp /path/to/your/wallet.json wallets/deployment.json
```

### 3. Run Your Deployment

```bash
# Deploy to default network (from appsettings.json)
dotnet run

# Deploy to specific network
dotnet run mainnet
dotnet run testnet
dotnet run local
```

## Deployment Examples

### Simple Contract Deployment

```csharp
// Deploy a contract
var result = await toolkit.Deploy("../../src/MyContract/MyContract.csproj");

// Initialize the contract
await toolkit.Invoke(result.ContractHash, "initialize", "param1", 123);
```

### Multiple Contracts with Dependencies

```csharp
// Deploy Token Contract
var token = await toolkit.Deploy("../../src/Token/Token.csproj");

// Deploy Vault Contract
var vault = await toolkit.Deploy("../../src/Vault/Vault.csproj");

// Set up dependencies
await toolkit.Invoke(vault.ContractHash, "setToken", token.ContractHash);
```

### Deploy from Manifest

Create a `deployment-manifest.json` file:

```json
{
  "contracts": [
    {
      "name": "Token",
      "projectPath": "../../src/Token/Token.csproj",
      "deploymentParameters": ["MyToken", "MTK", 8, 1000000]
    },
    {
      "name": "Vault",
      "projectPath": "../../src/Vault/Vault.csproj",
      "dependencies": ["Token"],
      "postDeploymentActions": [
        {
          "method": "initialize",
          "parameters": []
        }
      ]
    }
  ]
}
```

Then deploy all contracts:

```csharp
var results = await toolkit.DeployFromManifest("deployment-manifest.json");
```

### Contract Updates

```csharp
// Update an existing contract
var contractHash = UInt160.Parse("0x...");
var result = await toolkit.Update(contractHash, "../../src/MyContract/MyContract.csproj");

// Run migration
await toolkit.Invoke(contractHash, "migrate", "v2.0");
```

## Advanced Features

### Custom Network Configuration

```csharp
// Use custom RPC endpoint
toolkit.SetNetwork("https://custom-rpc.example.com:10332");
```

### Check Contract Existence

```csharp
if (await toolkit.ContractExists(contractHash))
{
    // Contract exists, update it
    await toolkit.Update(contractHash, projectPath);
}
else
{
    // Deploy new contract
    await toolkit.Deploy(projectPath);
}
```

### Get Gas Balance

```csharp
var balance = await toolkit.GetGasBalance();
Console.WriteLine($"GAS Balance: {balance}");
```

### Call Read-Only Methods

```csharp
// Call contract method without creating transaction
var result = await toolkit.Call<string>(contractHash, "getName");
Console.WriteLine($"Contract name: {result}");
```

## Security Best Practices

- **Never commit wallet files** or passwords to version control
- **Use environment variables** for sensitive configuration:
  ```bash
  export NEO_WALLET_PASSWORD="your-password"
  ```
- **Test on TestNet** before deploying to MainNet
- **Verify contract hashes** after deployment

## Deployment Workflow

1. **Development**: Deploy to local Neo Express
2. **Testing**: Deploy to TestNet
3. **Production**: Deploy to MainNet

Example environment-specific deployment:

```csharp
var environment = args.Length > 0 ? args[0] : "local";
toolkit.SetNetwork(environment);

Console.WriteLine($"Deploying to {environment}...");
var result = await toolkit.Deploy("../../src/MyContract/MyContract.csproj");
```

## Troubleshooting

### Common Issues

1. **Insufficient GAS**: Ensure your wallet has enough GAS for deployment
2. **Contract Already Exists**: Use `Update` instead of `Deploy`
3. **Network Connection**: Verify RPC endpoint is accessible

### Debug Mode

Enable detailed logging in `Program.cs`:

```csharp
var toolkit = new DeploymentToolkit();
// Logging is configured in appsettings.json
```

## Next Steps

1. Modify `Program.cs` to implement your deployment logic
2. Update `appsettings.json` with your network configuration
3. Run the deployment and monitor the results

For more information, see the [Neo Smart Contract Deployment Toolkit documentation](https://github.com/neo-project/neo-devpack-dotnet).