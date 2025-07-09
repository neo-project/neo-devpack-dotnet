# Quick Start Guide

Get started with Neo smart contract development in minutes!

## Prerequisites

- .NET 8.0 SDK or later
- Basic knowledge of C#
- A code editor (Visual Studio, VS Code, or Rider recommended)

## Installation

### 1. Install Neo Development Tools

```bash
# Install Neo contract compiler
dotnet tool install -g Neo.Compiler.CSharp

# Install Neo Express for local blockchain
dotnet tool install -g Neo.Express

# Install deployment toolkit CLI
dotnet tool install -g Neo.SmartContract.Deploy.CLI
```

### 2. Create Your First Contract

Use the project template to create a new smart contract solution:

```bash
# Install the templates
dotnet new install Neo.SmartContract.Template

# Create a new contract solution
dotnet new neocontractsolution -n MyFirstContract

# Navigate to the project
cd MyFirstContract
```

### 3. Build the Contract

```bash
# Build all projects
dotnet build

# The contract will be compiled to NEF and manifest files
```

### 4. Test Your Contract

```bash
# Run unit tests
dotnet test
```

### 5. Deploy to Local Network

First, start a local Neo Express instance:

```bash
# Create a new Neo Express instance
neoxp create

# Start the blockchain
neoxp run
```

In another terminal, deploy your contract:

```bash
cd deploy/MyFirstContract.Deploy
dotnet run -- deploy -n local -w YOUR_WIF_KEY
```

## What's Next?

Now that you have your first contract deployed, you can:

1. **Modify the Contract**: Edit the contract code in `src/MyFirstContract/`
2. **Add Methods**: Implement your business logic
3. **Write Tests**: Add unit tests in `tests/MyFirstContract.Tests/`
4. **Deploy to Testnet**: Use the deployment toolkit to deploy to Neo testnet

## Example: Simple Storage Contract

Here's a minimal contract to get you started:

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Attributes;

[DisplayName("SimpleStorage")]
[ManifestExtra("Author", "Your Name")]
[ManifestExtra("Email", "your.email@example.com")]
public class SimpleStorage : SmartContract
{
    // Store a value
    public static void Store(string key, string value)
    {
        Storage.Put(Storage.CurrentContext, key, value);
    }

    // Retrieve a value
    [Safe]
    public static string Get(string key)
    {
        return Storage.Get(Storage.CurrentContext, key) ?? "";
    }
}
```

## Using the Deployment Toolkit

The deployment toolkit simplifies contract deployment:

```csharp
using Neo.SmartContract.Deploy;

// Create toolkit instance
using var toolkit = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("YOUR_WIF_KEY");

// Deploy contract
var result = await toolkit.DeployAsync("path/to/contract.csproj");
Console.WriteLine($"Contract deployed at: {result.ContractAddress}");

// Invoke methods
await toolkit.InvokeAsync(result.ContractHash, "store", "name", "Neo");
var value = await toolkit.CallAsync<string>(result.ContractHash, "get", "name");
```

## Common Commands

### Building Contracts
```bash
# Build a specific contract
dotnet build src/MyContract/MyContract.csproj

# Build all contracts
dotnet build
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Deployment Commands
```bash
# Deploy to testnet
dotnet run -- deploy -n testnet -w YOUR_WIF_KEY

# Deploy from manifest
dotnet run -- deploy-manifest -n testnet -m deployment.json

# Check deployment status
dotnet run -- verify -n testnet -h CONTRACT_HASH
```

## Troubleshooting

### Common Issues

1. **"Neo.Compiler.CSharp not found"**
   - Make sure you've installed the compiler: `dotnet tool install -g Neo.Compiler.CSharp`

2. **"Insufficient GAS"**
   - Ensure your wallet has enough GAS for deployment
   - For testnet, get free GAS from a faucet

3. **"Contract too large"**
   - Optimize your contract code
   - Remove unnecessary methods or logic

## Resources

- [Full Documentation](../README.md)
- [Contract Examples](../../examples/DeploymentExample/README.md)
- [Neo Documentation](https://docs.neo.org/)
- [Community Support](https://discord.gg/neo)