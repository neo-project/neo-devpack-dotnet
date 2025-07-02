# Neo Smart Contract Deployment Toolkit - Complete Documentation

## Overview

The Neo Smart Contract Deployment Toolkit is a comprehensive library that provides everything needed to compile, deploy, and manage Neo N3 smart contracts programmatically. It's designed as a library (not a CLI) for developers who want to write explicit C# deployment logic.

## Core Components

### 1. NeoContractToolkit (Main Entry Point)
- Central facade for all deployment operations
- Provides high-level APIs for compilation, deployment, and invocation
- Built using the builder pattern for easy configuration

### 2. Services

#### IContractCompiler
- Compiles smart contracts from project files (.csproj) or source files (.cs)
- Supports Neo.Compiler.CSharp for compilation
- Generates NEF files, manifest, and debug information

#### IContractDeployer
- Deploys compiled contracts to the blockchain
- Handles transaction creation and signing
- Supports contract updates

#### IContractInvoker
- Invokes contract methods (read-only and state-changing)
- Handles parameter serialization
- Manages transaction fees

#### IWalletManager
- Manages NEP-6 wallets
- Handles account operations
- Signs transactions

#### IMultiContractDeploymentService
- Deploys multiple contracts with dependency resolution
- Supports parallel deployment
- Handles rollback on failure

### 3. Configuration

The toolkit supports configuration through JSON files:

```json
{
  "Network": {
    "RpcUrl": "http://localhost:10332",
    "Network": "private",
    "Wallet": {
      "WalletPath": "wallet.json",
      "Password": "password"
    }
  },
  "Deployment": {
    "DefaultNetworkFee": 1000000,
    "DefaultGasLimit": 50000000,
    "WaitForConfirmation": true,
    "ConfirmationRetries": 10,
    "ConfirmationDelaySeconds": 1,
    "ValidUntilBlockOffset": 100
  }
}
```

## Key Features

### 1. Project-Based Compilation (NEW)
The toolkit now prefers project-based compilation, which provides:
- Better dependency management
- NuGet package resolution
- Consistent build configuration
- Support for project references

```csharp
var options = new CompilationOptions
{
    ProjectPath = "MyContract/MyContract.csproj",  // Preferred
    OutputDirectory = "output",
    ContractName = "MyContract",
    GenerateDebugInfo = true,
    Optimize = true
};
```

### 2. Multi-Contract Deployment
Deploy multiple contracts with automatic dependency resolution:

```csharp
var requests = new[]
{
    new ContractDeploymentRequest
    {
        Name = "Registry",
        ProjectPath = "Registry/Registry.csproj",
        Dependencies = new List<string>()
    },
    new ContractDeploymentRequest
    {
        Name = "Token",
        ProjectPath = "Token/Token.csproj",
        Dependencies = new List<string> { "Registry" },
        InjectDependencies = true
    }
};

var result = await toolkit.DeployMultipleContractsAsync(requests, options);
```

### 3. Configuration Integration
All services use configuration for network settings:
- RPC URLs are loaded from configuration
- Network type (TestNet, MainNet, Private) is configurable
- Wallet paths and passwords can be configured

### 4. Builder Pattern
Fluent API for toolkit configuration:

```csharp
var toolkit = NeoContractToolkitBuilder.Create()
    .ConfigureLogging(builder => builder.AddConsole())
    .ConfigureServices(services => services.AddSingleton(config))
    .Build();
```

## Deployment Workflow

1. **Setup Toolkit**
   ```csharp
   var toolkit = NeoContractToolkitBuilder.Create().Build();
   ```

2. **Load Wallet**
   ```csharp
   await toolkit.LoadWalletAsync("wallet.json", "password");
   ```

3. **Compile Contract**
   ```csharp
   var compiled = await toolkit.CompileAsync(compilationOptions);
   ```

4. **Deploy Contract**
   ```csharp
   var result = await toolkit.DeployAsync(compiled, deploymentOptions);
   ```

5. **Invoke Methods**
   ```csharp
   var response = await toolkit.InvokeContractAsync(hash, method, params);
   ```

## Models

### CompilationOptions
- `ProjectPath` - Path to .csproj file (preferred)
- `SourcePath` - Path to .cs file (legacy)
- `OutputDirectory` - Where to save compiled artifacts
- `ContractName` - Name for the contract
- `GenerateDebugInfo` - Include debug information
- `Optimize` - Enable optimizations

### DeploymentOptions
- `DeployerAccount` - Account to deploy from
- `GasLimit` - Maximum GAS to spend
- `NetworkFee` - Network fee for transaction
- `WaitForConfirmation` - Wait for block confirmation
- `InitialParameters` - Constructor parameters

### ContractDeploymentInfo
- `Success` - Whether deployment succeeded
- `ContractHash` - Deployed contract hash
- `TransactionHash` - Deployment transaction hash
- `GasConsumed` - Actual GAS consumed
- `BlockIndex` - Block containing transaction
- `ErrorMessage` - Error details if failed

## Template Integration

The toolkit includes project templates for easy setup:
- Deployment project template with examples
- Multi-contract deployment examples
- Configuration templates
- CI/CD integration examples

## Error Handling

The toolkit provides specific exceptions:
- `CompilationException` - With detailed compilation errors
- `DeploymentException` - With deployment failure reasons
- `WalletException` - For wallet-related issues
- `InvocationException` - For contract invocation failures

## Testing

The toolkit includes comprehensive tests:
- Unit tests for all services
- Integration tests with Neo.SmartContract.Testing
- Multi-contract deployment tests
- Configuration tests

## Complete Example

```csharp
using Neo.SmartContract.Deploy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// Setup
var toolkit = NeoContractToolkitBuilder.Create()
    .ConfigureLogging(builder => builder.AddConsole())
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        services.AddSingleton<IConfiguration>(config);
    })
    .Build();

// Load wallet from configuration
await toolkit.LoadWalletFromConfigurationAsync();

// Compile from project
var compilationOptions = new CompilationOptions
{
    ProjectPath = "MyContract/MyContract.csproj",
    OutputDirectory = "output",
    ContractName = "MyContract"
};

var compiled = await toolkit.CompileAsync(compilationOptions);

// Deploy
var deploymentOptions = new DeploymentOptions
{
    DeployerAccount = toolkit.GetDeployerAccount(),
    GasLimit = 50_000_000
};

var result = await toolkit.DeployAsync(compiled, deploymentOptions);

// Invoke
var value = await toolkit.CallContractAsync<string>(
    result.ContractHash,
    "getName"
);

Console.WriteLine($"Contract deployed at {result.ContractHash}");
Console.WriteLine($"Contract name: {value}");
```

## Summary

The Neo Smart Contract Deployment Toolkit is now complete with:
- ✅ Full configuration support from JSON
- ✅ Project-based compilation as the primary method
- ✅ Multi-contract deployment with dependencies
- ✅ Comprehensive error handling
- ✅ Builder pattern for easy setup
- ✅ Integration with Neo SDK
- ✅ Template projects for quick start
- ✅ Extensive documentation and examples

The toolkit provides a production-ready solution for deploying Neo smart contracts programmatically with explicit C# code.