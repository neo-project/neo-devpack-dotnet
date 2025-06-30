# Getting Started with NEO Smart Contract Development

Welcome to NEO smart contract development! This guide will walk you through everything you need to know to start building decentralized applications on the NEO blockchain using C# and .NET.

## Table of Contents

1. [Introduction](#introduction)
2. [Prerequisites](#prerequisites)
3. [Development Environment Setup](#development-environment-setup)
4. [Your First Smart Contract](#your-first-smart-contract)
5. [Understanding Contract Basics](#understanding-contract-basics)
6. [Testing Your Contract](#testing-your-contract)
7. [Deploying to Testnet](#deploying-to-testnet)
8. [Next Steps](#next-steps)
9. [Troubleshooting](#troubleshooting)

## Introduction

NEO smart contracts are programs that run on the NEO blockchain. They are written in high-level languages like C# and compiled to NeoVM bytecode. This guide focuses on C# development using the NEO DevPack for .NET.

### What You'll Learn

- Setting up your development environment
- Writing and compiling smart contracts
- Testing contracts locally
- Deploying to NEO testnet
- Best practices and security considerations

### Time Required

This guide takes approximately 30-45 minutes to complete.

## Prerequisites

Before starting, ensure you have:

### Knowledge Requirements
1. **Basic C# Knowledge**: Familiarity with C# syntax and object-oriented programming
2. **Command Line Experience**: Basic terminal/command prompt usage
3. **Blockchain Basics**: Understanding of blockchain concepts (optional but helpful)

### Hardware Requirements
- **Minimum Requirements**:
  - CPU: Dual-core 2.0 GHz or higher
  - RAM: 4 GB (8 GB recommended)
  - Storage: 10 GB free space
  - Network: Stable internet connection

- **Recommended for Development**:
  - CPU: Quad-core 2.5 GHz or higher
  - RAM: 8 GB or more
  - Storage: 20 GB free space (SSD preferred)
  - OS: Windows 10/11, Ubuntu 20.04+, or macOS 10.15+

### Software Requirements
1. **Git**: For version control ([Download](https://git-scm.com/))
2. **.NET SDK 9.0+**: Required for compilation ([Download](https://dotnet.microsoft.com/download))
3. **Text Editor or IDE**: 
   - Visual Studio 2022+ (Windows/Mac)
   - Visual Studio Code (All platforms)
   - JetBrains Rider (All platforms)

## Development Environment Setup
⏱️ **Estimated Time: 10-15 minutes**

### Step 1: Install .NET SDK

Download and install .NET SDK 9.0 or later from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).

Verify installation:
```bash
dotnet --version
```

### Step 2: Clone NEO DevPack

```bash
git clone --recurse-submodules https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet
```

### Step 3: Build the DevPack

```bash
dotnet build
```

### Step 4: Install Development Tools (Optional)

For the best development experience, install:

- **Visual Studio Code** with C# extension
- **NEO Blockchain Toolkit** VS Code extension
- **Docker** (for local blockchain testing)

## Your First Smart Contract
⏱️ **Estimated Time: 15-20 minutes**

Let's create a simple "Hello World" smart contract.

### Step 1: Create a New Project

```bash
# Create a new directory for your contract
mkdir HelloWorldContract
cd HelloWorldContract

# Create a new class library project
dotnet new classlib
```

### Step 2: Add NEO Framework Reference

Edit `HelloWorldContract.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.8.1" />
  </ItemGroup>
</Project>
```

### Step 3: Write Your Contract

Replace the contents of `Class1.cs` with:

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace HelloWorld
{
    [DisplayName("HelloWorld")]
    [ManifestExtra("Author", "{{YOUR_NAME}}")]
    [ManifestExtra("Email", "{{YOUR_EMAIL}}")]
    [ManifestExtra("Description", "My first NEO smart contract")]
    public class HelloWorld : SmartContract
    {
        // Contract owner - ⚠️ REPLACE "NiHURyS..." WITH YOUR WALLET ADDRESS ⚠️
        [InitialValue("{{YOUR_WALLET_ADDRESS}}", ContractParameterType.Hash160)]
        private static readonly UInt160 Owner = default;

        // Simple greeting method
        [Safe]
        public static string Greeting(string name)
        {
            return $"Hello, {name}! Welcome to NEO.";
        }

        // Get contract owner
        [Safe]
        public static UInt160 GetOwner()
        {
            return Owner;
        }

        // Store a message (requires owner permission)
        private const string MESSAGE_PREFIX = "Message";
        
        public static bool StoreMessage(string message)
        {
            // Check if caller is the owner
            if (!Runtime.CheckWitness(Owner))
            {
                throw new Exception("Only owner can store messages");
            }

            // Store the message
            Storage.Put(Storage.CurrentContext, MESSAGE_PREFIX, message);
            return true;
        }

        // Retrieve stored message
        [Safe]
        public static string GetMessage()
        {
            return Storage.Get(Storage.CurrentContext, MESSAGE_PREFIX);
        }

        // Contract update method
        public static bool Update(ByteString nefFile, string manifest)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                throw new Exception("Only owner can update");
            }
            ContractManagement.Update(nefFile, manifest);
            return true;
        }
    }
}
```

### Step 4: Compile Your Contract

From your project directory, compile the contract:

```bash
# Build the project first
dotnet build

# Compile to NEO bytecode (from the neo-devpack-dotnet root directory)
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- HelloWorldContract/HelloWorldContract.csproj
```

**Verify compilation:**
```bash
# Check that the artifacts were generated
ls bin/sc/
```

This should generate:
- `HelloWorldContract.nef` - The compiled bytecode
- `HelloWorldContract.manifest.json` - Contract metadata

## Understanding Contract Basics

### Key Concepts

1. **SmartContract Base Class**: All contracts inherit from `SmartContract`
2. **Attributes**: Control contract behavior and metadata
   - `[Safe]`: Method doesn't modify storage
   - `[DisplayName]`: Contract's display name
   - `[ManifestExtra]`: Additional metadata
3. **Storage**: Persistent key-value storage
4. **Runtime.CheckWitness**: Verify caller identity
5. **Native Contracts**: Built-in contracts like `ContractManagement`

### Contract Structure

```csharp
public class MyContract : SmartContract
{
    // State variables (stored on blockchain)
    private const string STORAGE_KEY = "myKey";
    
    // Read-only methods (marked with [Safe])
    [Safe]
    public static string GetData() { }
    
    // State-changing methods
    public static void SetData(string value) { }
    
    // Events
    [DisplayName("DataChanged")]
    public static event Action<string> OnDataChanged;
}
```

## Local Development Network
⏱️ **Estimated Time: 10-15 minutes**

Before deploying to testnet, it's recommended to test your contracts on a local blockchain. Here are two approaches:

### Using Neo Express (Recommended)

Neo Express provides a lightweight, developer-friendly private blockchain:

```bash
# Install Neo Express globally
dotnet tool install -g Neo.Express

# Create a new private blockchain
neoxp create mynet

# Start the blockchain (single-node)
neoxp run -s 1

# Create a wallet for testing
neoxp wallet create alice

# Give alice some GAS
neoxp transfer 100 GAS genesis alice

# Deploy your contract
neoxp contract deploy HelloWorld.nef HelloWorld.manifest.json alice
```

### Using Docker

For a more production-like environment:

```bash
# Run Neo node with default configuration
docker run -d \
  --name neo-node \
  -p 20332:20332 \
  -p 20333:20333 \
  -p 30333:30333 \
  neo:latest

# Run with custom configuration
docker run -d \
  --name neo-node \
  -v /path/to/config:/neo-cli/config \
  -v /path/to/wallet:/neo-cli/wallet \
  -p 20332:20332 \
  -p 20333:20333 \
  neo:latest
```

### Neo CLI for Local Testing

```bash
# Download Neo CLI
wget https://github.com/neo-project/neo-cli/releases/latest/download/neo-cli-linux-x64.zip
unzip neo-cli-linux-x64.zip

# Start private net
./neo-cli --privnet

# Create and open wallet
create wallet test.json
open wallet test.json

# Deploy contract
deploy HelloWorld.nef HelloWorld.manifest.json
```

### Testing Tools Comparison

| Tool | Best For | Setup Time | Features |
|------|----------|------------|----------|
| Neo Express | Quick development | < 1 min | Simple commands, fast reset |
| Docker | CI/CD pipelines | 2-3 min | Consistent environment |
| Neo CLI | Production-like testing | 5 min | Full node features |

## Testing Your Contract

### Step 1: Create a Test Project

```bash
# From the HelloWorldContract directory
dotnet new mstest -n HelloWorldContract.Tests
cd HelloWorldContract.Tests
```

### Step 2: Add References

Edit `HelloWorldContract.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.1.1" />
    <PackageReference Include="MSTest.TestFramework" Version="3.1.1" />
    <PackageReference Include="Neo.SmartContract.Testing" Version="3.8.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../HelloWorldContract.csproj" />
  </ItemGroup>
</Project>
```

### Step 3: Write Tests

Create `HelloWorldTests.cs`:

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System;

namespace HelloWorld.Tests
{
    [TestClass]
    public class HelloWorldTests : TestBase<HelloWorld>
    {
        [TestInitialize]
        public void TestSetup()
        {
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(nef, manifest);
        }

        [TestMethod]
        public void Test_Greeting()
        {
            // Act
            var result = Contract.Greeting("NEO Developer");
            
            // Assert
            Assert.AreEqual("Hello, NEO Developer! Welcome to NEO.", result);
        }

        [TestMethod]
        public void Test_StoreAndRetrieveMessage()
        {
            // Arrange
            var testMessage = "Test message";
            var owner = Contract.GetOwner();
            
            // Act - Store message as owner
            Engine.SetTransactionSigners(owner);
            var storeResult = Contract.StoreMessage(testMessage);
            
            // Assert
            Assert.IsTrue(storeResult);
            Assert.AreEqual(testMessage, Contract.GetMessage());
        }

        [TestMethod]
        public void Test_OnlyOwnerCanStore()
        {
            // Arrange
            var nonOwner = Neo.UInt160.Zero;
            
            // Act & Assert
            Engine.SetTransactionSigners(nonOwner);
            Assert.ThrowsException<Exception>(() => 
                Contract.StoreMessage("Unauthorized"));
        }

        [TestMethod]
        public void Test_GasConsumption()
        {
            // Arrange
            var initialGas = Engine.GasConsumed;
            
            // Act
            Contract.Greeting("Test");
            
            // Assert
            var gasUsed = Engine.GasConsumed - initialGas;
            Console.WriteLine($"Greeting method used {gasUsed} GAS");
            Assert.IsTrue(gasUsed < 1_000_000); // Less than 0.01 GAS
        }
    }
}
```

### Step 4: Run Tests

```bash
dotnet test
```

## Deploying to Testnet

### Step 1: Get a NEO Wallet

1. Download [NEO CLI](https://github.com/neo-project/neo-cli/releases)
2. Create a new wallet:
   ```bash
   neo-cli
   neo> create wallet testwallet.json
   ```

### Step 2: Get Testnet GAS

Visit the [NEO Testnet Faucet](https://neowish.ngd.network/) and request GAS for your wallet address.

### Step 3: Deploy Contract

Using NEO CLI:
```bash
neo> open wallet testwallet.json
neo> deploy HelloWorld.nef HelloWorld.manifest.json
```

### Step 4: Invoke Your Contract

```bash
neo> invoke [contract-hash] greeting "World"
```

## IDE Configuration

### Visual Studio Code

1. **Install Extensions**
   ```bash
   code --install-extension ms-dotnettools.csharp
   code --install-extension neo-blockchain.neo-blockchain-toolkit
   ```

2. **Configure launch.json for Debugging**
   ```json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": "Neo Contract Debug",
         "type": "neo-contract",
         "request": "launch",
         "program": "${workspaceFolder}/bin/sc/HelloWorld.nef",
         "invocation": {
           "operation": "greeting",
           "args": ["World"]
         }
       }
     ]
   }
   ```

3. **Configure tasks.json for Compilation**
   ```json
   {
     "version": "2.0.0",
     "tasks": [
       {
         "label": "build contract",
         "command": "dotnet",
         "type": "process",
         "args": [
           "run",
           "--project",
           "${workspaceFolder}/../neo-devpack-dotnet/src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj",
           "--",
           "${workspaceFolder}/HelloWorld.csproj"
         ]
       }
     ]
   }
   ```

### Visual Studio

1. **Install NEO DevPack Extension**
   - Open Visual Studio
   - Go to Extensions → Manage Extensions
   - Search for "NEO Smart Contract"
   - Install and restart

2. **Configure Project Properties**
   - Right-click project → Properties
   - Set output path for compiled contracts
   - Configure debugging options

### JetBrains Rider

1. **Install Plugin**
   - File → Settings → Plugins
   - Search for "NEO Blockchain"
   - Install and restart

2. **Configure Run Configurations**
   - Run → Edit Configurations
   - Add Neo Smart Contract configuration
   - Set contract path and test parameters

## Common Development Patterns

### Factory Pattern

Create contracts dynamically:

```csharp
public class ContractFactory : SmartContract
{
    private const string CONTRACT_MAP = "contracts";
    
    public static UInt160 CreateContract(string name, ByteString nefFile, string manifest)
    {
        Assert(Runtime.CheckWitness(GetOwner()), "Unauthorized");
        Assert(!ContractExists(name), "Contract already exists");
        
        // Deploy new contract
        UInt160 contractHash = ContractManagement.Deploy(nefFile, manifest);
        
        // Store contract reference
        StorageMap contractMap = new(Storage.CurrentContext, CONTRACT_MAP);
        contractMap.Put(name, contractHash);
        
        OnContractCreated(name, contractHash);
        return contractHash;
    }
    
    [DisplayName("ContractCreated")]
    public static event Action<string, UInt160> OnContractCreated;
}
```

### Upgradeable Contracts

Implement safe upgrade mechanisms:

```csharp
public class UpgradeableContract : SmartContract
{
    private const string VERSION_KEY = "version";
    
    public static string GetVersion()
    {
        return Storage.Get(Storage.CurrentContext, VERSION_KEY) ?? "1.0.0";
    }
    
    public static bool Upgrade(ByteString nefFile, string manifest, object data)
    {
        Assert(Runtime.CheckWitness(GetOwner()), "Only owner can upgrade");
        
        // Save current state if needed
        var currentVersion = GetVersion();
        
        // Perform upgrade
        ContractManagement.Update(nefFile, manifest, data);
        
        OnUpgraded(currentVersion, "2.0.0");
        return true;
    }
    
    public static void _deploy(object data, bool update)
    {
        if (update)
        {
            // Migration logic here
            MigrateData();
        }
        else
        {
            // Initial deployment
            Storage.Put(Storage.CurrentContext, VERSION_KEY, "1.0.0");
        }
    }
    
    [DisplayName("Upgraded")]
    public static event Action<string, string> OnUpgraded;
}
```

### Proxy Pattern

For upgradeable logic with stable address:

```csharp
public class ProxyContract : SmartContract
{
    private const string IMPLEMENTATION_KEY = "impl";
    
    public static object Main(string operation, object[] args)
    {
        var implementation = GetImplementation();
        Assert(implementation != null, "Implementation not set");
        
        return Contract.Call(implementation, operation, CallFlags.All, args);
    }
    
    public static bool SetImplementation(UInt160 newImplementation)
    {
        Assert(Runtime.CheckWitness(GetOwner()), "Unauthorized");
        Storage.Put(Storage.CurrentContext, IMPLEMENTATION_KEY, newImplementation);
        return true;
    }
    
    private static UInt160 GetImplementation()
    {
        return (UInt160)Storage.Get(Storage.CurrentContext, IMPLEMENTATION_KEY);
    }
}
```

## Next Steps

### Learn More

1. **Token Standards**
   - [NEP-17 Token Tutorial](../examples/04-token-standards/README.md#nep-17-fungible-token)
   - [NEP-11 NFT Tutorial](../examples/04-token-standards/README.md#nep-11-non-fungible-token)

2. **Advanced Topics**
   - [Oracle Integration](../examples/03-advanced/README.md#oracle)
   - [Security Best Practices](../docs/security/security-best-practices.md)

3. **Tools and Resources**
   - [NEO Docs](https://docs.neo.org/)
   - [NEO Developer Discord](https://discord.gg/rvZFQ5382k)
   - [NEO GitHub](https://github.com/neo-project)

### Practice Projects

1. **Simple Voting Contract**: Create a contract for proposal voting
2. **Escrow Service**: Build a trustless escrow system
3. **Token Swap**: Implement a simple DEX for token swapping

## Troubleshooting

### Common Issues

**Issue: "Could not find Neo.SmartContract.Framework"**
- Solution: Ensure you've added the correct NuGet package reference

**Issue: "Contract compilation failed"**
- Solution: Check that your contract follows NEO limitations (no float, limited LINQ, etc.)
- Run with `--verbose` flag for detailed errors

**Issue: "Insufficient GAS"**
- Solution: Request more GAS from testnet faucet
- Optimize your contract to use less GAS

**Issue: "Method not found in contract"**
- Solution: Ensure methods are public static
- Check the manifest.json includes your method

### Getting Help

- **Discord**: Join the [NEO Discord](https://discord.gg/rvZFQ5382k)
- **GitHub Issues**: Report bugs in the [devpack repository](https://github.com/neo-project/neo-devpack-dotnet/issues)
- **Stack Overflow**: Tag questions with `neo-blockchain`

### Debug Tips

1. Use extensive unit testing before deployment
2. Test on private net before testnet
3. Monitor GAS consumption in tests
4. Use `Runtime.Log` for debugging (testnet only)

---

Congratulations! You've completed the Getting Started guide. You now have the foundation to build powerful smart contracts on NEO. Happy coding!