# 11. Testing Framework

Neo N3 provides a testing framework for smart contracts through the `Neo.SmartContract.Testing` namespace.

## 11.1 Basic Testing Setup

### 11.1.1 Simple Test Structure

```csharp
using Neo.SmartContract.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MyContractTests
{
    private TestEngine engine;
    private MyContract contract;

    [TestInitialize]
    public void Setup()
    {
        engine = new TestEngine(true); // Initialize with native contracts

        // Deploy contract
        var nef = NefFile.FromFile("MyContract.nef");
        var manifest = ContractManifest.FromJson(File.ReadAllText("MyContract.manifest.json"));
        contract = engine.Deploy<MyContract>(nef, manifest);
    }

    [TestMethod]
    public void TestMethod()
    {
        var result = contract.MyMethod(arg1, arg2);
        Assert.AreEqual(expectedResult, result);
    }
}
```

### 11.1.2 TestEngine Methods

| Method | Description |
|:-------|:------------|
| `Deploy<T>` | Deploy contract and return instance |
| `FromHash<T>` | Create contract instance from hash |
| `SetTransactionSigners` | Set transaction signers |
| `GetNewSigner` | Get new random signer |

### 11.1.3 Native Contract Access

```csharp
// Access native contracts
var neoBalance = engine.Native.NEO.BalanceOf(account);
var gasBalance = engine.Native.GAS.BalanceOf(account);
var contractInfo = engine.Native.ContractManagement.GetContract(hash);
```

## 11.2 Contract Deployment

### 11.2.1 Basic Deployment

```csharp
// Deploy contract
var nef = NefFile.FromFile("MyContract.nef");
var manifest = ContractManifest.FromJson(File.ReadAllText("MyContract.manifest.json"));
var contract = engine.Deploy<MyContract>(nef, manifest);

// Deploy with initialization data
var owner = engine.ValidatorsAddress;
var contract = engine.Deploy<MyContract>(nef, manifest, owner);

// Create instance from existing contract
var contract = engine.FromHash<MyContract>(hash);
```

### 11.2.2 Transaction Management

```csharp
// Set transaction signers
var signer = engine.GetNewSigner();
engine.SetTransactionSigners(new[] { signer });

// Access current transaction
var transaction = engine.Transaction;
```

## 11.3 Testing Features

### 11.3.1 Event Testing

```csharp
[TestMethod]
public void TestEvents()
{
    bool eventRaised = false;
    contract.OnTransfer += (from, to, amount) => {
        eventRaised = true;
        Assert.AreEqual(sender, from);
        Assert.AreEqual(recipient, to);
        Assert.AreEqual(100, amount);
    };

    contract.Transfer(sender, recipient, 100);
    Assert.IsTrue(eventRaised);
}
```

### 11.3.2 Storage Testing

```csharp
// Access contract storage
var storage = contract.Storage;

// Check storage operations
bool exists = storage.Contains(key);
var value = storage.Get(key);
storage.Put(key, value);
storage.Remove(key);
```

### 11.3.3 Fee Testing

```csharp
// Monitor gas consumption
engine.FeeConsumed.Reset();
contract.MyMethod();
var fee = engine.FeeConsumed.Value;

// Use fee watcher
var watcher = engine.CreateFeeWatcher();
contract.MyMethod();
var methodFee = watcher.Value;
```

## 11.4 Advanced Testing

### 11.4.1 Coverage Analysis

```csharp
// Enable coverage tracking
engine.EnableCoverageCapture = true;

// Run tests
contract.MyMethod();

// Get coverage information
var coverage = engine.GetCoverage(contract);
Console.WriteLine($"Coverage: {coverage.CoveredPercentage:P2}");

// Export coverage report
File.WriteAllText("coverage.html", coverage.Dump(DumpFormat.Html));
```

### 11.4.2 Testing Base Classes

```csharp
// Use base class for standard contracts
public class MyTokenTests : NEP17TestBase<MyToken>
{
    [TestInitialize]
    public void Setup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
    }

    [TestMethod]
    public void TestTransfer()
    {
        // Test token transfer functionality
    }
}

// Use base class for NFT contracts
public class MyNFTTests : NEP11TestBase<MyNFT>
{
    [TestInitialize]
    public void Setup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
    }
}
```

## 11.5 Simple Testing Example

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

[TestClass]
public class SimpleTokenTests : NEP17TestBase<SimpleToken>
{
    private UInt160 owner;
    private UInt160 user;

    [TestInitialize]
    public void Setup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);

        owner = Engine.ValidatorsAddress;
        user = Engine.GetNewSigner().Sender;
        Engine.SetTransactionSigners(new[] { Engine.GetNewSigner(owner) });
    }

    [TestMethod]
    public void TestMetadata()
    {
        Assert.AreEqual("STK", Contract.Symbol);
        Assert.AreEqual(8, Contract.Decimals);
        Assert.AreEqual(1000000, Contract.TotalSupply);
    }

    [TestMethod]
    public void TestTransfer()
    {
        // Test event emission
        bool eventRaised = false;
        Contract.OnTransfer += (from, to, amount) => {
            eventRaised = true;
            Assert.AreEqual(owner, from);
            Assert.AreEqual(user, to);
            Assert.AreEqual(100, amount);
        };

        // Perform transfer
        Assert.IsTrue(Contract.Transfer(owner, user, 100));
        Assert.IsTrue(eventRaised);

        // Verify balances
        Assert.AreEqual(999900, Contract.BalanceOf(owner));
        Assert.AreEqual(100, Contract.BalanceOf(user));
    }

    [TestMethod]
    public void TestUnauthorizedTransfer()
    {
        Engine.SetTransactionSigners(new[] { Engine.GetNewSigner(user) });
        Assert.IsFalse(Contract.Transfer(owner, user, 100));
    }

    [TestMethod]
    public void TestCoverage()
    {
        Engine.EnableCoverageCapture = true;

        Contract.Symbol;
        Contract.Transfer(owner, user, 100);

        var coverage = Engine.GetCoverage(Contract);
        Console.WriteLine($"Coverage: {coverage.CoveredPercentage:P2}");
    }
}
```

This example demonstrates:
- **Basic test setup** with TestEngine and contract deployment
- **Metadata testing** for token properties
- **Transfer testing** with event verification
- **Authorization testing** for security
- **Coverage analysis** for code quality
