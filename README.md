# Neo N3 Smart Contract DevPack for .NET (R3E Community Edition)

**Version 0.0.1**

<p align="center">
  <a href="https://github.com/r3e-network/">
    <img src="https://avatars.githubusercontent.com/u/187460041?s=200&v=4" width="250px" alt="r3e-logo">
  </a>
</p>

<p align="center">
  <a href="https://github.com/r3e-network/neo-devpack-dotnet/blob/r3e/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License">
  </a>
  <a href="https://www.nuget.org/packages/R3E.SmartContract.Framework">
    <img src="https://img.shields.io/nuget/v/R3E.SmartContract.Framework.svg" alt="NuGet Version">
  </a>
  <a href="https://www.nuget.org/packages/R3E.SmartContract.Framework">
    <img src="https://img.shields.io/nuget/dt/R3E.SmartContract.Framework.svg" alt="NuGet Downloads">
  </a>
  <a href="https://github.com/r3e-network/neo-devpack-dotnet/releases">
    <img src="https://img.shields.io/github/v/release/r3e-network/neo-devpack-dotnet?include_prereleases" alt="GitHub Release">
  </a>
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4" alt=".NET Version">
</p>

## Overview

This is the R3E Community Edition of the Neo N3 Smart Contract DevPack for .NET - a comprehensive suite of development tools for building smart contracts and decentralized applications (dApps) on the Neo blockchain platform using .NET. This toolkit enables developers to write, compile, test, and deploy Neo smart contracts using C# and other .NET languages.

The R3E Community maintains this fork of the official Neo DevPack with the goal of providing enhanced features and tools for the Neo developer community. All contracts compiled with this DevPack run on the Neo blockchain.

## Available Packages

| Package | Version | Description |
|---------|---------|-------------|
| [R3E.SmartContract.Framework](https://www.nuget.org/packages/R3E.SmartContract.Framework/) | ![NuGet](https://img.shields.io/nuget/v/R3E.SmartContract.Framework.svg) | Core framework for Neo smart contracts (R3E edition) |
| [R3E.SmartContract.Testing](https://www.nuget.org/packages/R3E.SmartContract.Testing/) | ![NuGet](https://img.shields.io/nuget/v/R3E.SmartContract.Testing.svg) | Testing framework for Neo contracts |
| [R3E.Compiler.CSharp](https://www.nuget.org/packages/R3E.Compiler.CSharp/) | ![NuGet](https://img.shields.io/nuget/v/R3E.Compiler.CSharp.svg) | C# to Neo VM bytecode compiler |
| [R3E.Compiler.CSharp.Tool](https://www.nuget.org/packages/R3E.Compiler.CSharp.Tool/) | ![NuGet](https://img.shields.io/nuget/v/R3E.Compiler.CSharp.Tool.svg) | Global CLI tool for compiling Neo contracts |
| [R3E.SmartContract.Analyzer](https://www.nuget.org/packages/R3E.SmartContract.Analyzer/) | ![NuGet](https://img.shields.io/nuget/v/R3E.SmartContract.Analyzer.svg) | Roslyn analyzers for Neo contract development |
| [R3E.SmartContract.Template](https://www.nuget.org/packages/R3E.SmartContract.Template/) | ![NuGet](https://img.shields.io/nuget/v/R3E.SmartContract.Template.svg) | Project templates for Neo contracts |
| [R3E.Disassembler.CSharp](https://www.nuget.org/packages/R3E.Disassembler.CSharp/) | ![NuGet](https://img.shields.io/nuget/v/R3E.Disassembler.CSharp.svg) | Neo VM bytecode disassembler |

## Components

The R3E Community Edition Neo DevPack consists of several key components:

### R3E.SmartContract.Framework

The framework provides the necessary libraries and APIs for writing Neo smart contracts in C#. It includes:

- Base classes and interfaces for smart contract development
- Neo blockchain API wrappers
- Standard contract templates (NEP-17, NEP-11, etc.)
- Utilities for common blockchain operations

### R3E.Compiler.CSharp (Neo Contract Compiler - R3E Edition)

A specialized compiler that translates C# code into Neo Virtual Machine (NeoVM) bytecode. Features include:

- Full C# language support for smart contract development
- Optimization for gas efficiency
- Debug information generation
- Source code generation for contract testing
- Contract interface generation
- **Web GUI generation for interactive contract interfaces**
- **Neo N3 plugin generation for CLI integration**
- **Available as both a CLI tool and a NuGet library for programmatic compilation**

[![NuGet](https://img.shields.io/nuget/v/R3E.Compiler.CSharp.svg)](https://www.nuget.org/packages/R3E.Compiler.CSharp/)

### R3E.SmartContract.Testing

A testing framework for Neo smart contracts that allows:

- Unit testing of contracts without deployment
- Storage simulation
- Mock native contracts
- Blockchain state simulation
- Gas consumption tracking
- Code coverage analysis

### R3E.Disassembler.CSharp

A tool for disassembling Neo VM bytecode back to readable C# code.

### R3E.SmartContract.Analyzer

Code analyzers and linting tools to help write secure and efficient Neo contracts.

### R3E.SmartContract.Template

Project templates for creating new Neo smart contracts with the proper structure and configurations.

### R3E.SmartContract.Deploy

A comprehensive deployment toolkit that simplifies the process of deploying and updating smart contracts on Neo networks. Features include:

- Simplified API for contract deployment and updates
- Automatic wallet and configuration management
- Support for multiple Neo networks (MainNet, TestNet, local)
- Contract compilation and deployment in one step
- Multi-contract deployment from manifest files
- **Contract update capabilities using Neo N3's _deploy method**
- WIF key support for direct transaction signing
- Integration with Neo Express for local development
- Comprehensive error handling and retry mechanisms

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 9.0 or later
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (optional but recommended)

### Installation

#### Install from NuGet

```shell
# Install the core framework
dotnet add package R3E.SmartContract.Framework --version 0.0.1

# Install the testing framework
dotnet add package R3E.SmartContract.Testing --version 0.0.1

# Install the compiler library
dotnet add package R3E.Compiler.CSharp --version 0.0.1

# Install the compiler global tool
dotnet tool install -g R3E.Compiler.CSharp.Tool --version 0.0.1

# Install project templates
dotnet new install R3E.SmartContract.Template::0.0.1
```

#### Build from Source

Clone the repository with submodules:

```shell
git clone --recurse-submodules https://github.com/r3e-network/neo-devpack-dotnet.git
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

#### Using the Template (Recommended)

```shell
# Create a new Neo smart contract project
dotnet new r3e-contract -n MyContract

# Navigate to the project
cd MyContract

# Build the contract
dotnet build
```

#### Manual Creation

1. Create a new class library project targeting .NET 9.0 or later
2. Add a reference to the R3E.SmartContract.Framework package
3. Create a class that inherits from `SmartContract`
4. Implement your contract logic
5. Compile using the R3E.Compiler.CSharp

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

The R3E Community Edition compiler translates your C# smart contract into Neo VM bytecode, which can then be deployed to the Neo blockchain. There are several ways to compile your contract:

#### Using the Global Tool (Recommended)

Install the compiler as a global tool:

```shell
dotnet tool install -g R3E.Compiler.CSharp.Tool
```

Then compile your contract:

```shell
# The R3E compiler still compiles to Neo VM bytecode
r3e-compiler path/to/your/contract.csproj
```

#### Using the Library Package

Add the compiler library to your project:

```shell
dotnet add package R3E.Compiler.CSharp
```

Then compile programmatically in your code. See [compiler library usage documentation](docs/compiler-library-usage.md) for details.

#### Basic Compilation (From Source)

```shell
dotnet run --project src/R3E.Compiler.CSharp/R3E.Compiler.CSharp.csproj -- path/to/your/contract.csproj
```

This command will compile your contract and generate the following files in the `bin/sc` directory of your project:
- `YourContract.nef`: The compiled bytecode file that is deployed to the blockchain
- `YourContract.manifest.json`: Contract manifest containing metadata and ABI information

#### Compilation Options

You can customize the compilation process with various options:

```shell
# For bash/zsh (macOS/Linux)
dotnet run --project src/R3E.Compiler.CSharp/R3E.Compiler.CSharp.csproj -- \
    path/to/your/contract.csproj \
    -o output/directory \
    --base-name MyContract \
    --debug \
    --assembly \
    --optimize=All \
    --generate-interface

# For Windows Command Prompt
dotnet run --project src/R3E.Compiler.CSharp/R3E.Compiler.CSharp.csproj -- ^
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
dotnet run --project src/R3E.Compiler.CSharp/R3E.Compiler.CSharp.csproj -- path/to/Contract.cs

# Compile all contracts in a directory
dotnet run --project src/R3E.Compiler.CSharp/R3E.Compiler.CSharp.csproj -- path/to/contract/directory
```

#### Compiler Command Reference

The R3E C# compiler supports the following options:

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

The R3E Community Edition includes a comprehensive testing framework specifically designed for Neo smart contracts. Here's how to create unit tests for your contracts:

```csharp
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.SmartContract.MyContract.UnitTests
{
    [TestClass]
    public class MyContractTests : TestBase<MyContract>
    {
        [TestInitialize]
        public void TestSetup()
        {
            // The testing framework automatically compiles and loads your contract
            TestInitialize();
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
2. Add references to the R3E.SmartContract.Testing package and your contract project
3. Create a test class that inherits from TestBase<T>
4. Implement the TestSetup method to compile and initialize the contract
5. Write test methods for each contract feature or scenario

### Updating Smart Contracts

The R3E Community Edition provides comprehensive support for updating deployed Neo smart contracts. Here's how to implement and execute contract updates:

#### Making Your Contract Updatable

Add an update method to your contract:

```csharp
[DisplayName("update")]
public static bool Update(ByteString nefFile, string manifest, object data)
{
    // Check authorization - only owner can update
    if (!Runtime.CheckWitness(GetOwner()))
    {
        throw new Exception("Only owner can update contract");
    }
    
    // Call ContractManagement.Update to perform the update
    ContractManagement.Update(nefFile, manifest, data);
    return true;
}
```

#### Updating a Contract Using the Deployment Toolkit

```csharp
using Neo.SmartContract.Deploy;

// Create and configure the toolkit
var toolkit = new DeploymentToolkit();
toolkit.SetWifKey("your-wif-key"); // Use the same key that deployed the contract
toolkit.SetNetwork("testnet");

// Update the contract
var result = await toolkit.UpdateAsync(
    contractHash: "0x1234567890abcdef1234567890abcdef12345678",
    path: "path/to/UpdatedContract.cs"
);

if (result.Success)
{
    Console.WriteLine($"Contract updated successfully!");
    Console.WriteLine($"Transaction: {result.TransactionHash}");
}
```

#### Update Best Practices

1. **Test Updates Thoroughly**: Always test on testnet before mainnet
2. **Maintain State Compatibility**: Ensure storage layout remains compatible
3. **Version Your Contracts**: Track contract versions for easier management
4. **Implement Authorization**: Use proper access controls for updates
5. **Document Changes**: Keep a changelog of contract updates

For detailed update documentation, see:
- [Contract Update Guide](docs/UPDATE_GUIDE.md)
- [Update Troubleshooting](docs/CONTRACT_UPDATE_TROUBLESHOOTING.md)
- [Deployment Example with Updates](examples/DeploymentExample/)

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

## Documentation

For detailed documentation on Neo smart contract development with .NET:

- [R3E Community Edition Repository](https://github.com/r3e-network/neo-devpack-dotnet)
- [Neo Official Documentation](https://docs.neo.org/)
- [Neo Smart Contract Development Guide](https://docs.neo.org/docs/en-us/develop/write/basics.html)
- [Official Neo DevPack](https://github.com/neo-project/neo-devpack-dotnet) (upstream project)

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

- [R3E Community](https://github.com/r3e-network/) - A Neo blockchain community
- [NEO Project](https://neo.org/) - Original developers of the Neo DevPack
- [NEO Community](https://neo.org/community)

## About R3E

R3E is a community within the Neo ecosystem dedicated to advancing Neo blockchain development. This DevPack edition represents our contribution to making Neo smart contract development more accessible and feature-rich for developers.
