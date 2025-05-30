# 5. Storage and State

## 5.1 Storage Context

Smart contracts access storage through a storage context that ensures data isolation between contracts.

### 5.1.1 Storage Context Types

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

// Read-write context (default)
StorageContext context = Storage.CurrentContext;

// Read-only context
StorageContext readOnlyContext = Storage.CurrentReadOnlyContext;

// Convert to read-only
StorageContext readOnly = Storage.CurrentContext.AsReadOnly();
```

| Context Type | Read | Write | Use Case |
|:-------------|:----:|:-----:|:---------|
| `CurrentContext` | ✅ | ✅ | Normal operations |
| `CurrentReadOnlyContext` | ✅ | ❌ | Safe methods |
| `AsReadOnly()` | ✅ | ❌ | Converted context |

## 5.2 Storage Operations

### 5.2.1 Basic Operations

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

// Get value (returns null if not found)
ByteString value = Storage.Get(Storage.CurrentContext, "key");

// Put value (overwrites existing)
Storage.Put(Storage.CurrentContext, "key", "value");
Storage.Put(Storage.CurrentContext, "balance", 1000);

// Delete value (no effect if not found)
Storage.Delete(Storage.CurrentContext, "key");

// Find values with prefix
Iterator iterator = Storage.Find(Storage.CurrentContext, "prefix");
```

### 5.2.2 Find Options

| Option | Description |
|:-------|:------------|
| `None` | Default behavior |
| `KeysOnly` | Return only keys |
| `ValuesOnly` | Return only values |
| `RemovePrefix` | Remove prefix from keys |
| `DeserializeValues` | Auto-deserialize values |

```csharp
// Find with options
Iterator keys = Storage.Find(Storage.CurrentContext, "prefix", FindOptions.KeysOnly);
Iterator clean = Storage.Find(Storage.CurrentContext, "prefix", FindOptions.RemovePrefix);
```

### 5.2.3 Constraints

| Constraint | Limit |
|:-----------|:------|
| Key size | 64 bytes |
| Value size | 65,536 bytes |
| Access scope | Own contract only |

## 5.3 Storage Maps

Storage maps organize data with prefixes to prevent key collisions.

### 5.3.1 Basic Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

// Create storage map
StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");

// Operations
balances.Put(account, 1000);
BigInteger balance = (BigInteger)balances.Get(account);
balances.Delete(account);

// Find with prefix
Iterator iterator = balances.Find("prefix");
```

### 5.3.2 Key Composition

Storage maps automatically combine prefix and key:
```
ActualKey = Prefix + Key
```

### 5.3.3 Token Balance Example

```csharp
private static readonly byte[] PrefixBalance = new byte[] { 0x01 };

[Safe]
public static BigInteger BalanceOf(UInt160 account)
{
    StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
    return (BigInteger)balances.Get(account);
}

public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);

    BigInteger fromBalance = (BigInteger)balances.Get(from);
    if (fromBalance < amount) return false;

    balances.Put(from, fromBalance - amount);
    balances.Put(to, (BigInteger)balances.Get(to) + amount);

    return true;
}
```

## 5.4 Serialization

### 5.4.1 Basic Serialization

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;

// Serialize and deserialize
ByteString serialized = StdLib.Serialize(obj);
MyClass obj = (MyClass)StdLib.Deserialize(serialized);
```

### 5.4.2 Supported Types

| Type | Example |
|:-----|:--------|
| `bool` | `true`, `false` |
| `byte[]` | `new byte[] { 1, 2, 3 }` |
| `string` | `"Hello"` |
| `BigInteger` | `1000` |
| `UInt160` | Address hash |
| `UInt256` | Transaction hash |
| `ECPoint` | Public key |
| Custom classes | User-defined types |

### 5.4.3 Custom Type Example

```csharp
public class TokenInfo
{
    public UInt160 Owner;
    public string Name;
    public BigInteger Supply;

    public TokenInfo() { } // Required parameterless constructor
}

// Usage
TokenInfo info = new TokenInfo { Owner = owner, Name = "Token", Supply = 1000 };
Storage.Put(Storage.CurrentContext, "info", StdLib.Serialize(info));

// Retrieve
ByteString data = Storage.Get(Storage.CurrentContext, "info");
TokenInfo retrieved = (TokenInfo)StdLib.Deserialize(data);
```

**Note**: Serialization format is binary and internal to Neo. Use only for contract storage.

## 5.5 Storage Patterns

### 5.5.1 Simple Key-Value

```csharp
// Basic storage
Storage.Put(Storage.CurrentContext, "name", "MyToken");
string name = (string)Storage.Get(Storage.CurrentContext, "name");
```

### 5.5.2 Prefix Organization

```csharp
private static readonly byte[] PrefixBalance = new byte[] { 0x01 };
private static readonly byte[] PrefixSupply = new byte[] { 0x02 };

// Organized storage
Storage.Put(Storage.CurrentContext, PrefixBalance.Concat(account), balance);
Storage.Put(Storage.CurrentContext, PrefixSupply, totalSupply);
```

### 5.5.3 Iteration Pattern

```csharp
// Store multiple items
StorageMap tokens = new StorageMap(Storage.CurrentContext, "tokens");
tokens.Put(tokenId, StdLib.Serialize(tokenData));

// Iterate through items
Iterator iterator = tokens.Find(FindOptions.RemovePrefix);
while (iterator.Next())
{
    ByteString tokenId = iterator.Key;
    TokenData data = (TokenData)StdLib.Deserialize(iterator.Value);
}
```

### 5.5.4 Composite Keys

```csharp
// Relationship storage: owner -> tokens
StorageMap ownerTokens = new StorageMap(Storage.CurrentContext, "ownerTokens");
ownerTokens.Put(owner.Concat(tokenId), 1);

// Check ownership
bool owns = ownerTokens.Get(owner.Concat(tokenId)) != null;

// Find owner's tokens
Iterator iterator = ownerTokens.Find(owner);
```

## 5.6 State Management

### 5.6.1 State Components

Contract state consists of:
- **Code**: Contract bytecode
- **Manifest**: Contract metadata
- **Storage**: Key-value data

### 5.6.2 Atomic Transactions

All storage changes in a transaction are atomic - either all succeed or all fail.

```csharp
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    if (!Runtime.CheckWitness(from)) return false;

    StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");
    BigInteger fromBalance = (BigInteger)balances.Get(from);
    if (fromBalance < amount) return false;

    // Atomic state update
    balances.Put(from, fromBalance - amount);
    balances.Put(to, (BigInteger)balances.Get(to) + amount);

    return true;
}
```

### 5.6.3 State Guarantees

- **Consistency**: Blockchain consensus ensures state consistency
- **Rollback**: Failed transactions automatically rollback all changes

## 5.7 Storage Costs

### 5.7.1 Cost Calculation

```csharp
// Get current storage price
long storagePrice = Policy.GetStoragePrice();

// Cost formula
StorageFee = StoragePrice × (KeySize + ValueSize)
```

### 5.7.2 Cost Example

```csharp
// Storing "balance" -> 1000 costs:
// StoragePrice × (7 bytes + 4 bytes) = StoragePrice × 11 bytes
Storage.Put(Storage.CurrentContext, "balance", 1000);
```

### 5.7.3 Refunds

Deleting storage items refunds a portion of the original storage fee.

## 5.8 Simple Storage Example

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

[DisplayName("SimpleStorage")]
public class SimpleStorage : SmartContract
{
    private static readonly byte[] PrefixBalance = new byte[] { 0x01 };
    private static readonly byte[] PrefixConfig = new byte[] { 0x02 };

    // Simple key-value storage
    [Safe]
    public static string GetName()
    {
        return (string)Storage.Get(Storage.CurrentContext, "name");
    }

    public static void SetName(string name)
    {
        Storage.Put(Storage.CurrentContext, "name", name);
    }

    // Storage map usage
    [Safe]
    public static BigInteger BalanceOf(UInt160 account)
    {
        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        return (BigInteger)balances.Get(account);
    }

    public static void SetBalance(UInt160 account, BigInteger amount)
    {
        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        balances.Put(account, amount);
    }

    // Serialization example
    public class Config
    {
        public string Name;
        public BigInteger Version;
        public Config() { }
    }

    public static void SaveConfig(string name, BigInteger version)
    {
        Config config = new Config { Name = name, Version = version };
        Storage.Put(Storage.CurrentContext, "config", StdLib.Serialize(config));
    }

    [Safe]
    public static Config GetConfig()
    {
        ByteString data = Storage.Get(Storage.CurrentContext, "config");
        return data is null ? null : (Config)StdLib.Deserialize(data);
    }
}
```

This example demonstrates:
- Basic key-value storage
- Storage maps with prefixes
- Object serialization
- Safe method attributes
