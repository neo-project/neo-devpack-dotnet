# Neo Smart Contract Deploy - Quick Start Examples

## One-Line Deployment

```csharp
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Extensions;

// Deploy to TestNet in one line
var contractHash = await Deploy.ToTestNet.QuickDeploy("MyContract.csproj");

// Deploy to MainNet
var contractHash = await Deploy.ToMainNet.QuickDeploy("MyContract.csproj");

// Deploy to local network
var contractHash = await Deploy.ToLocal.QuickDeploy("MyContract.csproj");
```

## Basic Usage (3 lines)

```csharp
var toolkit = new DeploymentToolkit();
toolkit.SetNetwork("testnet");
var result = await toolkit.Deploy("MyContract.csproj");
```

## With Initialization Parameters

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("testnet");
var owner = await toolkit.GetDeployerAccount();
var result = await toolkit.Deploy("Token.csproj", new object[] { owner, 1000000 });
```

## Deploy from Source File

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("local");
var result = await toolkit.Deploy("MyContract.cs");
```

## Deploy Pre-compiled Contract

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("testnet");
var result = await toolkit.DeployArtifacts("contract.nef", "contract.manifest.json");
```

## Call Contract Methods

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("mainnet");

// Read-only call
var name = await toolkit.Call<string>("0x1234...abcd", "getName");

// State-changing transaction
var txHash = await toolkit.Invoke("0x1234...abcd", "transfer", from, to, amount);
```

## Update Contract

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("testnet");
var result = await toolkit.Update("0x1234...abcd", "UpdatedContract.csproj");
```

## Check Balance

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("mainnet");
var balance = await toolkit.GetGasBalance(); // Get deployer's GAS balance
var balance2 = await toolkit.GetGasBalance("NXXXxxxXXXxxx..."); // Get specific address balance
```

## Environment Variables

Set these to avoid any configuration:

```bash
export NEO_WALLET_PATH="wallet.json"
export NEO_WALLET_PASSWORD="your-password"
```

Then just:

```csharp
var result = await new DeploymentToolkit().SetNetwork("testnet").Deploy("Contract.csproj");
```

## Minimal appsettings.json

```json
{
  "Wallet": {
    "Path": "wallet.json",
    "Password": "your-password"
  }
}
```

That's it! The toolkit handles everything else automatically.