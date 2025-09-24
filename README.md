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
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4" alt=".NET Version">
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

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 9.0 or later
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

1. Create a new class library project targeting .NET 9.0 or later
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

namespace Example.SmartContract.MyContract.UnitTests
{
    [TestClass]
    public class MyContractTests : ContractProjectTestBase
    {
        public MyContractTests()
            : base("../Example.SmartContract.MyContract/Example.SmartContract.MyContract.csproj")
        {
        }

        [TestInitialize]
        public void SetUp()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void MyMethod_ReturnsExpectedValue()
        {
            var result = Contract.MyMethod("parameter");
            Assert.AreEqual("Expected result", result);
        }
    }
}
``` 

#### Key Testing Features

1. **ContractProjectTestBase**: Compiles the referenced contract project at test runtime and deploys it into an isolated `TestEngine` automatically.

2. **No Manual Artifacts**: The runtime compiler keeps NEF/manifest/debug assets in memory—no more checking generated files into source control.

3. **Direct Contract Interaction**: Interact with the contract through the dynamic `Contract` property, or project it onto helper interfaces for strongly-typed calls.

4. **Storage Simulation**: Exercise persistent storage via the `Engine` and `Storage` helpers without deploying to a real network.

5. **Event Verification**: Capture runtime notifications and logs to verify that the contract emits the expected events.

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

1. Create a new MSTest project for your contract tests.
2. Reference your contract project and add a project reference to `Neo.SmartContract.Testing.RuntimeCompilation`.
3. Create a test class that inherits from `ContractProjectTestBase`, supplying the path to the contract `.csproj` (and optional contract name).
4. Call `EnsureContractDeployed()` from `[TestInitialize]` to compile and deploy the contract before each test.
5. Write test methods that interact with the dynamic `Contract` property, asserting storage, notifications, and GAS as needed.

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
