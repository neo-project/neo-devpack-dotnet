# NEO DevPack for .NET

<p align="center">
  <a href="https://neo.org/">
    <img src="https://neo3.azureedge.net/images/logo%20files-dark.svg" width="250px" alt="neo-logo">
  </a>
</p>

<p align="center">
  <a href='https://coveralls.io/github/neo-project/neo-devpack-dotnet'>
    <img src='https://coveralls.io/repos/github/neo-project/neo-devpack-dotnet/badge.svg' alt='Coverage Status' />
  </a>
  <a href="https://github.com/neo-project/neo-devpack-dotnet/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License">
  </a>
  <a href="https://www.nuget.org/packages/Neo.SmartContract.Framework">
    <img src="https://img.shields.io/nuget/v/Neo.SmartContract.Framework.svg" alt="NuGet Version">
  </a>
  <a href="https://www.nuget.org/packages/Neo.SmartContract.Framework">
    <img src="https://img.shields.io/nuget/dt/Neo.SmartContract.Framework.svg" alt="NuGet Downloads">
  </a>
  <a href="https://discord.gg/rvZFQ5382k">
    <img src="https://img.shields.io/discord/382937847893590016?logo=discord" alt="Discord">
  </a>
  <a href="https://twitter.com/neo_blockchain">
    <img src="https://img.shields.io/twitter/follow/neo_blockchain?style=social" alt="Twitter Follow">
  </a>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4" alt=".NET Version">
  <img src="https://img.shields.io/badge/C%23-14-512BD4?logo=csharp&logoColor=white" alt="C# 14 Support">
</p>

## Overview

NEO DevPack for .NET is a comprehensive suite of development tools for building smart contracts and decentralized applications (dApps) on the NEO blockchain platform using .NET. This toolkit enables developers to write, compile, test, and deploy smart contracts using C# and other .NET languages.

## Components

The NEO DevPack for .NET consists of several key components:

### Neo.SmartContract.Framework

The framework provides the necessary libraries and APIs for writing NEO smart contracts in C#. It includes:

- Base classes and interfaces for smart contract development
- NEO blockchain API wrappers
- Standard contract templates (NEP-17, NEP-11, etc.)
- Utilities for common blockchain operations

### Neo.Compiler.CSharp

A specialized compiler that translates C# code into NEO Virtual Machine (NeoVM) bytecode. Features include:

- Full C# language support for smart contract development
- Optimization for gas efficiency
- Debug information generation
- Source code generation for contract testing
- Contract interface generation

### Neo.SmartContract.Testing

A testing framework for NEO smart contracts that allows:

- Unit testing of contracts without deployment
- Storage simulation
- Mock native contracts
- Blockchain state simulation
- Gas consumption tracking
- Code coverage analysis

### Neo.Disassembler.CSharp

A tool for disassembling NeoVM bytecode back to readable C# code.

### Neo.SmartContract.Analyzer

Code analyzers and linting tools to help write secure and efficient contracts.

### Neo.SmartContract.Template

Project templates for creating new NEO smart contracts with the proper structure and configurations.

### Neo.SmartContract.Deploy

A streamlined deployment toolkit that provides a simplified API for Neo smart contract deployment. Features include:

- **Artifact Deployment**: Deploy precompiled `.nef` and manifest artifacts with a single call
- **Network Support**: Target mainnet, testnet, private nets, or custom RPC endpoints
- **WIF Key Integration**: Sign deployment and invocation transactions directly with WIF keys
- **Contract Interaction**: Perform read-only calls and on-chain invocations against deployed contracts
- **Balance Checking**: Query GAS balances for deployment accounts
- **Configurable Runtime**: Tune confirmation behaviour and network profiles through `DeploymentOptions`

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 10.0 or later
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (optional but recommended)

### Installation

Clone the repository with submodules:

```shell
git clone --recurse-submodules https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet
```

### Build

```shell
dotnet build
```

### Run Tests

```shell
dotnet test
```

## Usage

### Creating a New Smart Contract

1. Create a new class library project targeting .NET 10.0 or later
2. Add a reference to the Neo.SmartContract.Framework package
3. Create a class that inherits from `SmartContract`
4. Implement your contract logic
5. Compile using the Neo.Compiler.CSharp

Example:

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

public class HelloWorldContract : SmartContract
{
    [Safe]
    public static string SayHello(string name)
    {
        return $"Hello, {name}!";
    }
}
```

### Compiling a Smart Contract

The NEO C# compiler (nccs) translates your C# smart contract into NeoVM bytecode, which can then be deployed to the NEO blockchain. There are several ways to compile your contract:

#### Basic Compilation

```shell
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- path/to/your/contract.csproj
```

This command will compile your contract and generate the following files in the `bin/sc` directory of your project:
- `YourContract.nef`: The compiled bytecode file that is deployed to the blockchain
- `YourContract.manifest.json`: Contract manifest containing metadata and ABI information

#### Compilation Options

You can customize the compilation process with various options:

```shell
# For bash/zsh (macOS/Linux)
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- \
    path/to/your/contract.csproj \
    -o output/directory \
    --base-name MyContract \
    --debug \
    --assembly \
    --optimize=All \
    --generate-interface

# For Windows Command Prompt
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- ^
    path/to/your/contract.csproj ^
    -o output/directory ^
    --base-name MyContract ^
    --debug ^
    --assembly ^
    --optimize=All ^
    --generate-interface
```

#### Working with Single Files or Directories

The compiler can also process individual `.cs` files or entire directories:

```shell
# Compile a single file
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- path/to/Contract.cs

# Compile all contracts in a directory
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- path/to/contract/directory
```

#### Compiler Command Reference

The NEO C# compiler supports the following options:

| Option | Description |
|--------|-------------|
| `-o, --output` | Specifies the output directory for compiled files |
| `--base-name` | Specifies the base name of the output files (overrides contract name) |
| `--debug` | Generates debug information (default is `Extended`) |
| `--assembly` | Generates a readable assembly (.asm) file |
| `--generate-artifacts` | Generates additional artifacts for contract interaction (Source, Library, or All) |
| `--generate-interface` | Generates a C# interface file for the contract (useful for type-safe interaction) |
| `--security-analysis` | Performs security analysis on the compiled contract |
| `--optimize` | Specifies the optimization level (None, Basic, All, Experimental) |
| `--no-inline` | Disables method inlining during compilation |
| `--nullable` | Sets the nullable context options (Disable, Enable, Annotations, Warnings) |
| `--checked` | Enables overflow checking for arithmetic operations |
| `--address-version` | Sets the address version for script hash calculations |

### Testing a Smart Contract

The NEO DevPack includes a comprehensive testing framework specifically designed for smart contracts. Here's how to create unit tests for your contracts:

```csharp
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.SmartContract.MyContract.UnitTests
{
    [TestClass]
    public class MyContractTests : TestBase<ArtifactMyContract>
    {
        [TestInitialize]
        public void TestSetup()
        {
            // Compile and generate testing artifacts
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();

            // Setup the test base with the compiled contract
            TestBaseSetup(nef, manifest);
        }

        [TestMethod]
        public void TestMethod()
        {
            // Access contract properties and methods through the Contract property
            var result = Contract.MyMethod("parameter");
            Assert.AreEqual("Expected result", result);

            // Test storage changes after operations
            var storedValue = Storage.Get(Contract.Hash, "myKey");
            Assert.AreEqual("Expected storage value", storedValue);

            // Verify emitted events
            var notifications = Notifications;
            Assert.AreEqual(1, notifications.Count);
            Assert.AreEqual("ExpectedEvent", notifications[0].EventName);
        }
    }
}
```

#### Key Testing Features

1. **TestBase<T> Class**: Provides a base class for contract testing with access to the contract, storage, and notifications.

2. **Automatic Artifact Generation**: The testing framework automatically compiles your contracts and generates testing artifacts.

3. **Direct Contract Interaction**: Access contract properties and methods directly through the strongly-typed `Contract` property.

4. **Storage Simulation**: Test persistent storage operations without deploying to a blockchain.

5. **Event Verification**: Validate that your contract emits the expected events.

6. **Gas Consumption Analysis**: Track and analyze GAS costs of operations:

```csharp
[TestMethod]
public void TestGasConsumption()
{
    // Record initial gas
    var initialGas = Engine.GasConsumed;

    // Execute contract operation
    Contract.ExpensiveOperation();

    // Check gas consumption
    var gasUsed = Engine.GasConsumed - initialGas;
    Console.WriteLine($"Gas used: {gasUsed}");
    Assert.IsTrue(gasUsed < 100_000_000, "Operation used too much gas");
}
```

#### Setting Up the Test Project

1. Create a new test project for your contract
2. Add references to the Neo.SmartContract.Testing package and your contract project
3. Create a test class that inherits from TestBase<T>
4. Implement the TestSetup method to compile and initialize the contract
5. Write test methods for each contract feature or scenario

## Examples

The repository includes various example contracts that demonstrate different features and capabilities in the `examples` directory:

| Example | Description |
|---------|-------------|
| [HelloWorld](examples/Example.SmartContract.HelloWorld/) | Basic contract example |
| [NEP-17](examples/Example.SmartContract.NEP17/) | Token standard implementation |
| [NEP-11](examples/Example.SmartContract.NFT/) | NFT standard implementation |
| [Storage](examples/Example.SmartContract.Storage/) | Persistent storage example |
| [Events](examples/Example.SmartContract.Event/) | Event notification example |
| [Contract Calls](examples/Example.SmartContract.ContractCall/) | Inter-contract calls |
| [Exception](examples/Example.SmartContract.Exception/) | Error handling examples |
| [ZKP](examples/Example.SmartContract.ZKP/) | Zero-knowledge proof implementation examples |
| [Inscription](examples/Example.SmartContract.Inscription/) | Blockchain inscription examples |
| [Oracle](examples/Example.SmartContract.Oracle/) | Oracle service interaction examples |
| [Modifier](examples/Example.SmartContract.Modifier/) | Method modifier usage examples |
| [Transfer](examples/Example.SmartContract.Transfer/) | Asset transfer examples |
| [SampleRoyaltyNEP11Token](examples/Example.SmartContract.SampleRoyaltyNEP11Token/) | NFT with royalty feature implementation |

Each example comes with corresponding unit tests that demonstrate how to properly test the contract functionality.

## Contract Deployment

This package supports deploying compiled artifacts (`.nef` + manifest), compiling and deploying from source projects, orchestrating multi-contract deployments via manifests, and basic RPC interactions (call, invoke, GAS balance, contract existence).

The `Neo.SmartContract.Deploy` package provides a streamlined way to deploy contracts to the NEO blockchain using compiled artifacts.

### Installation

```shell
dotnet add package Neo.SmartContract.Deploy
```

### Basic Usage

```csharp
using Neo.SmartContract.Deploy;

// Create deployment toolkit
var deployment = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("your-wif-key-here");

// Deploy from compiled artifacts
var result = await deployment.DeployArtifactsAsync(
    "MyContract.nef",
    "MyContract.manifest.json",
    waitForConfirmation: true);

Console.WriteLine($"Contract deployed: {result.ContractHash}");
Console.WriteLine($"Transaction: {result.TransactionHash}");
```

### Artifact Deployment

```csharp
// Deploy with initialization parameters
var initParams = new object[] { "param1", 42, true };
var artifactsResult = await deployment.DeployArtifactsAsync(
    "MyContract.nef",
    "MyContract.manifest.json",
    initParams,
    waitForConfirmation: true);

Console.WriteLine($"Tx: {artifactsResult.TransactionHash}");
Console.WriteLine($"Expected Contract Hash: {artifactsResult.ContractHash}");

Example app: See `examples/DeploymentArtifactsDemo` for a minimal console that deploys from NEF + manifest and performs read-only calls.
```

```csharp
var request = new DeploymentArtifactsRequest("MyContract.nef", "MyContract.manifest.json")
    .WithInitParams("owner", 100)
    .WithConfirmationPolicy(waitForConfirmation: true, retries: 20, delaySeconds: 2);

await deployment.DeployArtifactsAsync(request, cancellationToken);

var customSignerOptions = new DeploymentOptions
{
    SignerProvider = _ => new[]
    {
        new Signer
        {
            Account = UInt160.Parse("0x0123456789abcdef0123456789abcdef01234567"),
            Scopes = WitnessScope.CalledByEntry
        }
    },
    TransactionSignerAsync = (manager, ct) =>
    {
        // Attach additional witnesses or delegate to an external signer.
        return manager.SignAsync();
    }
};

var customSignerToolkit = new DeploymentToolkit(options: customSignerOptions);
```

> **Note:** Smart-contract parameters must be serialisable to Neo stack items. When providing initialization arguments (directly or via JSON manifests), numeric values must be whole numbers. Encode fractional values as strings or scaled integers if required.

### Source Deployment

```csharp
var compileAndDeploy = await deployment.DeployAsync(
    "contracts/MyContract/MyContract.csproj",
    initParams: new object?[] { "owner", 1000 });

Console.WriteLine($"Contract hash: {compileAndDeploy.ContractHash}");
```

### Manifest Deployment

You can orchestrate multiple deployments via a JSON manifest:

```json
{
  "network": "mainnet",
  "wif": "Kx...",
  "waitForConfirmation": true,
  "confirmationRetries": 40,
  "confirmationDelaySeconds": 2,
  "contracts": [
    {
      "name": "Token",
      "nef": "artifacts/Token.nef",
      "manifest": "artifacts/Token.manifest.json",
      "initParams": ["admin", 1000000]
    },
    {
      "name": "Treasury",
      "nef": "artifacts/Treasury.nef",
      "manifest": "artifacts/Treasury.manifest.json",
      "waitForConfirmation": false
    }
  ]
}
```

```csharp
var deployments = await deployment.DeployFromManifestAsync("deployment.json");
foreach (var (name, info) in deployments)
{
    Console.WriteLine($"{name}: {info.ContractHash} ({info.TransactionHash})");
}
```

### Network Configuration

```csharp
// Use predefined networks
deployment.SetNetwork("mainnet");
deployment.SetNetwork("testnet"); 
deployment.SetNetwork("local");

// Or use custom RPC URL
deployment.SetNetwork("https://my-custom-rpc.com:10332");

// Alternatively configure at construction time
var options = new DeploymentOptions { Network = NetworkProfile.TestNet };
var toolkit = new DeploymentToolkit(options: options);

// The same API exposes cancellation and confirmation tuning
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
var requestResult = await toolkit.DeployArtifactsAsync(
    "MyContract.nef",
    "MyContract.manifest.json",
    waitForConfirmation: true,
    confirmationRetries: 60,
    confirmationDelaySeconds: 2,
    cancellationToken: cts.Token);
```

### Contract Interaction

```csharp
// Call contract method (read-only)
var balance = await deployment.CallAsync<int>("contract-hash", "balanceOf", "account-address");

// Invoke contract method (state-changing)
var txHash = await deployment.InvokeAsync("contract-hash", "transfer", fromAccount, toAccount, amount);

// Check contract existence
var exists = await deployment.ContractExistsAsync("contract-hash");

// Get account balance
var gasBalance = await deployment.GetGasBalanceAsync();
```

### Deployment Project Template

Scaffold a ready-to-run deployment helper alongside your solution. You can generate it directly when creating a contract via `nccs` or by using the standalone template:

```bash
# Contract + deployment helper in one go
nccs new MyContract --template Basic --deploy MyContract.Deploy

# Standalone deploy helper
dotnet new neodeploy --contract-project MyContract -n MyContract.Deploy
cd MyContract.Deploy
dotnet run -- deploy --network express --wif <your-wif>
```

Pass `--deploy-contract <ExistingProjectName>` if the helper should point at a different contract project inside your solution.

The generated project keeps deployment settings inside `deploysettings.json`, which points at the compiled `.nef`/`.manifest.json` artifacts (defaulting to `../MyContract/bin/sc`). It exposes three commands:

- `deploy` – Load the compiled artifacts file, deploy to Neo-Express, mainnet, testnet, or any custom RPC URL, and wait for confirmation (optional).
- `invoke` – Send state-changing transactions to the deployed contract.
- `call` – Perform read-only invocations and print the result (choose the return type via `--return`).

Override networks on the fly via `--network mainnet` / `--network testnet` / `--network express` or provide any custom RPC URL with `--rpc http://host:port`. You can also register bespoke aliases under `customNetworks` within `deploysettings.json`.

### Configuration File Support

Create an `appsettings.json` file for configuration:

```json
{
  "Network": {
    "Network": "mainnet",
    "Networks": {
      "mainnet": {
        "RpcUrl": "https://mainnet1.neo.coz.io:443",
        "NetworkMagic": 860833102,
        "Wif": "<optional-mainnet-wif>"
      },
      "devnet": {
        "RpcUrl": "http://localhost:50012",
        "NetworkMagic": 123456789,
        "AddressVersion": 53,
        "Wif": ""
      }
    }
  },
  "Deployment": {
    "GasLimit": 100000000,
    "WaitForConfirmation": true
  }
}
```

Entries under `Network:Networks` let you preconfigure RPC URLs, network magic numbers, address versions, and per-network signing keys. When a `Wif` is provided the toolkit automatically loads it as soon as you switch to that network, while still allowing runtime overrides via `SetWifKey`, environment variables, or CLI prompts.

## Documentation

For detailed documentation on NEO smart contract development with .NET:

- [NEO Official Documentation](https://docs.neo.org/)
- [NEO Smart Contract Development Guide](https://docs.neo.org/docs/en-us/develop/write/basics.html)

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -am 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Create a new Pull Request

Please ensure that your code follows the existing coding style and includes appropriate tests and documentation.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [NEO Project](https://neo.org/)
- [NEO Community](https://neo.org/community)
