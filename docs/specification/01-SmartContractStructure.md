# 2. Smart Contract Structure

## 2.1 Contract Definition

A Neo N3 smart contract is defined as a class that inherits from the `SmartContract` base class or one of its specialized subclasses in the `Neo.SmartContract.Framework` namespace. The contract class must be decorated with appropriate attributes to define its metadata.

### 2.1.1 Basic Contract

For general-purpose contracts, inherit directly from `SmartContract`:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

[DisplayName("MyContract")]
[ContractDescription("A description of the contract")]
[ContractEmail("contact@example.com")]
[ContractVersion("1.0.0")]
[ContractSourceCode("https://github.com/example/repository")]
[ContractPermission(Permission.Any, Method.Any)]
public class MyContract : SmartContract
{
    // Contract implementation
}
```

### 2.1.2 SmartContract Base Class

The `SmartContract` base class is the foundation for all Neo N3 smart contracts:

```csharp
public abstract class SmartContract
{
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern static void _initialize();
}
```

The base class provides:
- Basic contract functionality
- Access to the `_initialize()` method for static field initialization
- Integration with the NeoVM

### 2.1.3 Contract Inheritance Hierarchy

The Neo N3 framework provides a hierarchy of base classes for different types of contracts:

```
SmartContract (base class)
├── TokenContract (abstract base for all tokens)
    ├── Nep17Token (abstract base for NEP-17 fungible tokens)
    └── Nep11Token<TokenState> (abstract base for NEP-11 NFTs)
```

### 2.1.4 Token Contracts

For token contracts, use the appropriate specialized base class:

**NEP-17 Fungible Token:**
```csharp
[DisplayName("MyToken")]
[SupportedStandards(NepStandard.Nep17)]
public class MyToken : Nep17Token
{
    public override string Symbol { [Safe] get => "MTK"; }
    public override byte Decimals { [Safe] get => 8; }
}
```

**NEP-11 Non-Fungible Token:**
```csharp
[DisplayName("MyNFT")]
[SupportedStandards(NepStandard.Nep11)]
public class MyNFT : Nep11Token<MyTokenState>
{
    public override string Symbol { [Safe] get => "MNFT"; }
}
```

### 2.1.5 Contract Class Requirements

A Neo N3 smart contract must be a public class that inherits from `SmartContract` or one of its subclasses. Private, internal, or protected classes cannot be deployed as contracts.

## 2.2 Specialized Base Classes

### 2.2.1 TokenContract

The `TokenContract` abstract class provides common functionality for all token contracts:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.Numerics;

public abstract class TokenContract : SmartContract
{
    public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount);

    [DisplayName("Transfer")]
    public static event OnTransferDelegate OnTransfer;

    public abstract string Symbol { [Safe] get; }
    public abstract byte Decimals { [Safe] get; }

    [Stored(0x00)]
    public static BigInteger TotalSupply { [Safe] get; protected set; }

    protected const byte Prefix_Balance = 0x01;

    [Safe]
    public static BigInteger BalanceOf(UInt160 owner)
    {
        if (owner is null || !owner.IsValid)
            throw new Exception("Invalid owner");
        StorageMap balanceMap = new(Storage.CurrentContext, Prefix_Balance);
        return (BigInteger)balanceMap[owner];
    }

    protected static void UpdateBalance(UInt160 owner, BigInteger increment)
    {
        StorageMap balanceMap = new(Storage.CurrentContext, Prefix_Balance);
        BigInteger balance = (BigInteger)balanceMap[owner];
        balance += increment;
        if (balance.IsZero)
            balanceMap.Delete(owner);
        else
            balanceMap[owner] = balance;
    }

    protected static void PostTransfer(UInt160 from, UInt160 to, BigInteger amount, object data)
    {
        OnTransfer(from, to, amount);
        if (to is not null && ContractManagement.GetContract(to) is not null)
            Contract.Call(to, "onNEP17Payment", CallFlags.All, new object[] { from, amount, data });
    }
}
```

### 2.2.2 Nep17Token

The `Nep17Token` abstract class provides NEP-17 fungible token functionality:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[SupportedStandards(NepStandard.Nep17)]
public abstract class Nep17Token : TokenContract
{
    public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount);

    [DisplayName("Transfer")]
    public static event OnTransferDelegate OnTransfer;

    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
    {
        if (from is null || !from.IsValid)
            throw new Exception("Invalid 'from' address");
        if (to is null || !to.IsValid)
            throw new Exception("Invalid 'to' address");
        if (amount < 0)
            throw new Exception("Invalid amount");
        if (!Runtime.CheckWitness(from))
            return false;

        if (from != to && amount != 0)
        {
            UpdateBalance(from, -amount);
            UpdateBalance(to, amount);
        }
        PostTransfer(from, to, amount, data);
        return true;
    }

    protected static void Mint(UInt160 account, BigInteger amount)
    {
        if (account is null || !account.IsValid)
            throw new Exception("Invalid account");
        if (amount < 0)
            throw new Exception("Invalid amount");

        UpdateBalance(account, amount);
        TotalSupply += amount;
        PostTransfer(null, account, amount, null);
    }

    protected static void Burn(UInt160 account, BigInteger amount)
    {
        if (account is null || !account.IsValid)
            throw new Exception("Invalid account");
        if (amount < 0)
            throw new Exception("Invalid amount");

        UpdateBalance(account, -amount);
        TotalSupply -= amount;
        PostTransfer(account, null, amount, null);
    }
}
```

### 2.2.3 Nep11Token&lt;TokenState&gt;

The `Nep11Token<TokenState>` abstract class provides NEP-11 NFT functionality:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.Numerics;

[SupportedStandards(NepStandard.Nep11)]
public abstract class Nep11Token<TokenState> : TokenContract
    where TokenState : Nep11TokenState
{
    public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount, ByteString tokenId);

    [DisplayName("Transfer")]
    public static event OnTransferDelegate OnTransfer;

    protected const byte Prefix_TokenId = 0x02;
    protected const byte Prefix_Token = 0x03;
    protected const byte Prefix_AccountToken = 0x04;

    public sealed override byte Decimals { [Safe] get => 0; }

    [Safe]
    public static UInt160 OwnerOf(ByteString tokenId)
    {
        if (tokenId.Length > 64)
            throw new Exception("Token ID too long");

        var tokenMap = new StorageMap(Storage.CurrentContext, Prefix_Token);
        var tokenData = tokenMap[tokenId] ?? throw new Exception("Token not found");
        TokenState token = (TokenState)StdLib.Deserialize(tokenData);
        return token.Owner;
    }

    [Safe]
    public virtual Map<string, object> Properties(ByteString tokenId)
    {
        var tokenMap = new StorageMap(Storage.CurrentContext, Prefix_Token);
        TokenState token = (TokenState)StdLib.Deserialize(tokenMap[tokenId]);
        return new Map<string, object>() { ["name"] = token.Name };
    }

    public static bool Transfer(UInt160 to, ByteString tokenId, object data)
    {
        if (to is null || !to.IsValid)
            throw new Exception("Invalid recipient");

        var tokenMap = new StorageMap(Storage.CurrentContext, Prefix_Token);
        TokenState token = (TokenState)StdLib.Deserialize(tokenMap[tokenId]);
        UInt160 from = token.Owner;

        if (!Runtime.CheckWitness(from))
            return false;

        if (from != to)
        {
            token.Owner = to;
            tokenMap[tokenId] = StdLib.Serialize(token);
            UpdateBalance(from, -1);
            UpdateBalance(to, +1);
        }
        PostTransfer(from, to, 1, data);
        return true;
    }

    protected static ByteString NewTokenId()
    {
        return NewTokenId(Runtime.ExecutingScriptHash);
    }

    protected static ByteString NewTokenId(ByteString salt)
    {
        StorageContext context = Storage.CurrentContext;
        byte[] key = new byte[] { Prefix_TokenId };
        ByteString id = Storage.Get(context, key);
        Storage.Put(context, key, (BigInteger)id + 1);
        if (id is not null) salt += id;
        return CryptoLib.Sha256(salt);
    }

    protected static void Mint(ByteString tokenId, TokenState token)
    {
        StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
        tokenMap[tokenId] = StdLib.Serialize(token);
        UpdateBalance(token.Owner, +1);
        TotalSupply++;
        PostTransfer(null, token.Owner, 1, null);
    }

    protected static void Burn(ByteString tokenId)
    {
        StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
        TokenState token = (TokenState)StdLib.Deserialize(tokenMap[tokenId]);
        tokenMap.Delete(tokenId);
        UpdateBalance(token.Owner, -1);
        TotalSupply--;
        PostTransfer(token.Owner, null, 1, null);
    }
}
```

#### Nep11TokenState Base Class

```csharp
using Neo;
using Neo.SmartContract.Framework;

public class Nep11TokenState
{
    public UInt160 Owner;
    public string Name;
}
```

## 2.3 Contract Metadata

Contract metadata is defined using attributes that provide information about the contract. These attributes are used to generate the contract manifest, which is stored on the blockchain along with the contract code.

### 2.3.1 Contract-Level Attributes

| Attribute | Description | Required |
|:----------|:------------|:--------:|
| `DisplayName` | Human-readable name for the contract | **Yes** |
| `ContractDescription` | Brief description of the contract's purpose | No |
| `ContractEmail` | Contact information for the contract author | No |
| `ContractVersion` | Version information for the contract | No |
| `ContractSourceCode` | Link to the source code repository | No |
| `ContractAuthor` | Author of the contract with optional URL | No |
| `ContractPermission` | Permission settings for contract interactions | No |
| `ContractTrust` | Specifies which contracts this contract trusts | No |
| `SupportedStandards` | NEP standards implemented by the contract | No |
| `NoReentrant` | Provides global reentrancy protection | No |
| `ManifestExtra` | Adds custom metadata to the contract manifest | No |

### 2.3.2 Field and Property Attributes

| Attribute | Description | Example Usage |
|:----------|:------------|:--------------|
| `ContractHash` | Marks a property to return the contract's hash | `[ContractHash] public static extern UInt160 Hash { get; }` |
| `Hash160` | Initializes a field with a Hash160 value | `[Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]` |
| `PublicKey` | Initializes a field with a public key | `[PublicKey("02486fd15702c4490a26703112a5cc1d...")]` |
| `ByteArray` | Initializes a field with byte array data | `[ByteArray("0a0b0c0d")]` |
| `Integer` | Initializes a field with an integer value | `[Integer(42)]` |
| `String` | Initializes a field with a string value | `[String("Hello")]` |
| `InitialValue` | Initializes a field with a custom value | `[InitialValue("custom")]` |
| `Stored` | Marks a property as storage-backed | `[Stored(0x01)] public static BigInteger Balance { get; set; }` |

Example of a contract with complete metadata:

```csharp
[DisplayName("TokenContract")]
[ContractDescription("A token contract implementing the NEP-17 standard")]
[ContractEmail("dev@example.com")]
[ContractVersion("1.0.0")]
[ContractSourceCode("https://github.com/example/token-contract")]
[ContractAuthor("Example Developer", "https://example.com")]
[SupportedStandards(NepStandard.Nep17)]
[ContractPermission(Permission.Any, Method.Any)]
public class TokenContract : SmartContract
{
    // Contract implementation
}
```

## 2.4 Interface Generation

The Neo compiler supports automatic interface generation for smart contracts, enabling type-safe interaction with deployed contracts. This feature generates C# interfaces that match the contract's public methods and events.

### 2.4.1 Enabling Interface Generation

Interface generation is enabled using the `--generate-interface` compiler flag:

```bash
nccs --generate-interface MyContract.cs
```

This generates an interface file (e.g., `IMyContract.cs`) that can be used for type-safe contract interaction.

### 2.4.2 Generated Interface Structure

For a contract like this:

```csharp
[DisplayName("HelloWorldContract")]
public class HelloWorldContract : SmartContract
{
    [ContractHash]
    public static extern UInt160 Hash { get; }

    [Safe]
    public static string SayHello(string name)
    {
        return $"Hello, {name}!";
    }

    [Safe]
    public static int Add(int a, int b)
    {
        return a + b;
    }
}
```

The compiler generates an interface:

```csharp
public interface IHelloWorldContract
{
    UInt160 Hash { get; }
    string SayHello(string name);
    int Add(int a, int b);
}
```

### 2.4.3 Using Generated Interfaces

Generated interfaces can be used with the testing framework for type-safe contract interaction:

```csharp
using Neo.SmartContract.Testing;

// Create a test instance
var engine = new TestEngine();
var contract = engine.Deploy<IHelloWorldContract>(nef, manifest);

// Call methods in a type-safe way
var result = contract.SayHello("World");
var sum = contract.Add(40, 2);
```

## 2.5 Contract Lifecycle Methods

### 2.5.1 Contract Initialization

Neo N3 smart contracts support two special initialization methods:

#### _deploy Method

Called automatically when a contract is deployed or updated:

```csharp
public static void _deploy(object data, bool update)
{
    if (update) return; // Skip initialization on updates

    // Initialize contract state
    UInt160 owner = (UInt160)data;
    Storage.Put(Storage.CurrentContext, "owner", owner);
}
```

#### _initialize Method

Called automatically to initialize static fields:

```csharp
public static void _initialize()
{
    // This method is called automatically by the framework
    // to initialize static fields with attributes
}
```

## 2.6 Contract Methods

Methods in a Neo N3 smart contract can be:

1. **Public Methods**: Accessible from outside the contract, can be invoked directly.
2. **Private Methods**: Only accessible from within the contract.
3. **Safe Methods**: Read-only methods that don't modify state (marked with `[Safe]` attribute).

### 2.6.1 Method Attributes

Methods can be decorated with various attributes to control their behavior:

| Attribute | Description | Example Usage |
|:----------|:------------|:--------------|
| `Safe` | Marks method as read-only, no state changes allowed | `[Safe] public static string GetName()` |
| `NoReentrant` | Provides global reentrancy protection | `[NoReentrant] public static void Transfer()` |
| `NoReentrantMethod` | Provides method-specific reentrancy protection | `[NoReentrantMethod] public static void Update()` |
| `DisplayName` | Specifies custom name for methods/events | `[DisplayName("Transfer")] public static event...` |
| `CallingConvention` | Sets calling convention for the method | `[CallingConvention(CallingConvention.Cdecl)]` |
| `OpCode` | Specifies VM opcode for method implementation | `[OpCode(OpCode.SHA256)]` |
| `Syscall` | Marks method as system call | `[Syscall("System.Runtime.CheckWitness")]` |

#### Custom Modifier Attributes

You can create custom modifier attributes by inheriting from `ModifierAttribute`:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

public class OnlyOwnerAttribute : ModifierAttribute
{
    private readonly UInt160 _owner;

    public OnlyOwnerAttribute(string ownerAddress)
    {
        _owner = (UInt160)ownerAddress.ToScriptHash();
    }

    public override void Enter()
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(_owner), "Unauthorized");
    }

    public override void Exit() { }
}

// Usage example
[OnlyOwner("NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP")]
public static void AdminFunction()
{
    // Only owner can call this method
}
```

### 2.6.2 Public Methods

Public methods are accessible from outside the contract and can be invoked directly by transactions or other contracts.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    if (from is null || !from.IsValid)
        throw new Exception("Invalid 'from' address");
    if (to is null || !to.IsValid)
        throw new Exception("Invalid 'to' address");
    if (!Runtime.CheckWitness(from))
        return false;

    // Method implementation
    return true;
}
```

### 2.6.3 Private Methods

Private methods are only accessible from within the contract and cannot be invoked directly by transactions or other contracts.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

private static bool ValidateOwner()
{
    UInt160 owner = GetOwner();
    return Runtime.CheckWitness(owner);
}
```

### 2.6.4 Safe Methods

Safe methods are read-only methods that don't modify state. They are marked with the `[Safe]` attribute and can be invoked without incurring GAS fees.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

[Safe]
public static string GetValue(string key)
{
    return Storage.Get(Storage.CurrentContext, key);
}
```

### 2.6.5 Method Parameters and Return Types

Neo N3 smart contracts support the following parameter and return types:

- `bool`: Boolean value
- `byte[]`: Byte array
- `string`: String
- `BigInteger`: Arbitrary-precision integer
- `UInt160`: 160-bit unsigned integer (typically used for script hashes)
- `UInt256`: 256-bit unsigned integer (typically used for transaction hashes or block hashes)
- `ECPoint`: Elliptic curve point (typically used for public keys)
- Custom types: Classes and structs defined in the contract

Example of a method with parameters and return type:

```csharp
public static BigInteger BalanceOf(UInt160 account)
{
    if (account is null || !account.IsValid)
        throw new Exception("Invalid account");

    StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");
    return (BigInteger)balances.Get(account);
}
```

### 2.3.5 Method Attributes

Methods in Neo N3 smart contracts can be decorated with attributes that provide additional information or behavior:

| Attribute | Description |
|-----------|-------------|
| `Safe` | Indicates that the method is read-only and doesn't modify state |
| `DisplayName` | Specifies a custom name for the method in the contract manifest |
| `NoReentrantMethod` | Prevents reentrancy attacks on the method |

Example of a method with attributes:

```csharp
[Safe]
[DisplayName("balance_of")]
public static BigInteger BalanceOf(UInt160 account)
{
    // Method implementation
}
```

## 2.4 Contract Events

Events in Neo N3 smart contracts are defined using the `event` keyword with an `Action` delegate. Events can be optionally named using the `[DisplayName]` attribute.

```csharp
[DisplayName("transfer")]
public static event Action<UInt160, UInt160, BigInteger> Transfer;

// Event emission
Transfer(from, to, amount);
```

Events are used to notify external systems about state changes or important occurrences in the contract. They are stored in the transaction log and can be queried by applications.

Events can have up to 16 parameters of the following types:
- `byte[]`
- `string`
- `BigInteger`
- `bool`
- `UInt160`
- `UInt256`
- `ECPoint`

Example of a contract with events:

```csharp
public class TokenContract : SmartContract
{
    [DisplayName("transfer")]
    public static event Action<UInt160, UInt160, BigInteger> Transfer;

    public static bool TransferToken(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Method implementation

        // Emit event
        Transfer(from, to, amount);

        return true;
    }
}
```

## 2.5 Contract Storage

Storage in Neo N3 smart contracts is accessed through the `Storage` class, which provides methods to read from and write to the blockchain's storage.

### 2.5.1 Storage Operations

The `Storage` class provides the following methods for storage operations:

- `Get`: Retrieve a value from storage
- `Put`: Store a value in storage
- `Delete`: Remove a value from storage
- `Find`: Search for values in storage with a prefix

```csharp
// Read from storage
ByteString value = Storage.Get(Storage.CurrentContext, key);

// Write to storage
Storage.Put(Storage.CurrentContext, key, value);

// Delete from storage
Storage.Delete(Storage.CurrentContext, key);

// Find values with a prefix
Iterator<KeyValue> pairs = Storage.Find(Storage.CurrentContext, prefix);
```

### 2.5.2 Storage Context

Storage operations in Neo N3 smart contracts are performed within a storage context, which defines the scope of the storage operations. There are two types of storage contexts:

1. **Current Context**: Allows read and write operations
2. **Read-Only Context**: Allows only read operations

```csharp
// Get current context (read/write)
StorageContext context = Storage.CurrentContext;

// Get read-only context
StorageContext readOnlyContext = Storage.CurrentReadOnlyContext;
```

### 2.5.3 Storage Maps

Storage maps provide a higher-level abstraction for working with structured data in storage.

```csharp
// Create a storage map
StorageMap map = new StorageMap(context, prefix);

// Get a value from the map
ByteString value = map.Get(key);

// Put a value in the map
map.Put(key, value);

// Delete a value from the map
map.Delete(key);
```

### 2.5.4 Storage Prefixes

Storage prefixes are used to organize data in storage and prevent key collisions. It is a common practice to define storage prefixes as constants in the contract.

```csharp
private static readonly byte[] PrefixBalance = new byte[] { 0x01 };
private static readonly byte[] PrefixSupply = new byte[] { 0x02 };
private static readonly byte[] PrefixOwner = new byte[] { 0x03 };

public static BigInteger BalanceOf(UInt160 account)
{
    StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
    return (BigInteger)balances.Get(account);
}
```

## 2.6 Contract Fields

Fields in Neo N3 smart contracts can be used to store constant values or to initialize values at compile time using attributes.

### 2.6.1 Constant Fields

Constant fields are defined using the `const` keyword and must be initialized with a constant expression.

```csharp
private const string TokenName = "MyToken";
private const byte Decimals = 8;
private const ulong InitialSupply = 100_000_000;
```

### 2.6.2 Static Fields with Initialization Attributes

Static fields can be initialized at compile time using attributes such as `ByteArray`, `Hash160`, `String`, and `Integer`.

```csharp
[ByteArray("0a0b0c0d0E0F")]
private static readonly byte[] Data = default!;

[Hash160("NL1JGjDe22U44R57ZXVSeTYFBavEkVmkgF")]
private static readonly UInt160 Owner = default!;

[String("MyToken")]
private static readonly string Name = default!;

[Integer("100000000")]
private static readonly BigInteger TotalSupply = default!;
```

### 2.6.3 Stored Properties

The `[Stored]` attribute can be used to automatically persist property values to storage:

```csharp
public abstract class TokenContract : SmartContract
{
    [Stored(0x00)] // Storage prefix
    public static BigInteger TotalSupply { [Safe] get; protected set; }
}
```

Properties marked with `[Stored]` automatically:
- Read values from storage when accessed
- Write values to storage when modified
- Use the specified prefix for storage keys

### 2.6.4 Additional Initialization Attributes

The framework provides several specialized attributes for initializing static fields:

**PublicKey Attribute:**
```csharp
[PublicKey("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c")]
private static readonly ECPoint ValidatorKey = default!;
```

**Complete Attribute Examples:**
```csharp
public class MyContract : SmartContract
{
    // String initialization
    [String("MyToken")]
    private static readonly string TokenName = default!;

    // Integer initialization
    [Integer("100000000")]
    private static readonly BigInteger InitialSupply = default!;

    // Byte array initialization
    [ByteArray("0123456789ABCDEF")]
    private static readonly byte[] ContractData = default!;

    // Hash160 initialization (NEO address)
    [Hash160("NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP")]
    private static readonly UInt160 OwnerAddress = default!;

    // Public key initialization
    [PublicKey("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c")]
    private static readonly ECPoint PublicKey = default!;
}

## 2.7 Contract Properties

Properties in Neo N3 smart contracts can be used to provide a more convenient interface for accessing contract state.

```csharp
private static readonly byte[] TotalSupplyKey = new byte[] { 0x00 };

[Safe]
public static BigInteger TotalSupply
{
    get
    {
        ByteString value = Storage.Get(Storage.CurrentContext, TotalSupplyKey);
        return value is null ? 0 : (BigInteger)value;
    }
}
```

## 2.8 Contract Namespaces

Neo N3 smart contracts can be organized into namespaces to improve code organization and readability.

```csharp
namespace MyProject.Contracts
{
    [DisplayName("MyContract")]
    public class MyContract : SmartContract
    {
        // Contract implementation
    }
}
```

## 2.9 Contract Using Directives

Neo N3 smart contracts can use the `using` directive to import namespaces, making it easier to reference types and members.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("MyContract")]
public class MyContract : SmartContract
{
    // Contract implementation
}
```

## 2.10 Complete Contract Examples

### 2.10.1 NEP-17 Token Example

Here's a complete example of a NEP-17 token using the framework base class:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("SimpleToken")]
[ContractDescription("A simple NEP-17 token")]
[ContractEmail("dev@example.com")]
[ContractVersion("1.0.0")]
[SupportedStandards(NepStandard.Nep17)]
[ContractPermission(Permission.Any, Method.Any)]
public class SimpleToken : Nep17Token
{
    // Token metadata
    public override string Symbol { [Safe] get => "SPT"; }
    public override byte Decimals { [Safe] get => 8; }

    // Owner management
    private const byte PrefixOwner = 0xff;
    private static readonly UInt160 InitialOwner = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

    [Safe]
    public static UInt160 GetOwner()
    {
        var currentOwner = Storage.Get(new[] { PrefixOwner });
        return currentOwner == null ? InitialOwner : (UInt160)currentOwner;
    }

    private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

    // Contract initialization
    public static void _deploy(object data, bool update)
    {
        if (update) return;

        UInt160 owner = (UInt160)data ?? InitialOwner;
        Storage.Put(new[] { PrefixOwner }, owner);

        // Mint initial supply
        Mint(owner, 100_000_000 * 100_000_000); // 100M tokens with 8 decimals
    }

    // Administrative functions
    public static void Mint(UInt160 to, BigInteger amount)
    {
        if (!IsOwner())
            throw new InvalidOperationException("No Authorization!");
        Nep17Token.Mint(to, amount);
    }

    public static void Burn(UInt160 account, BigInteger amount)
    {
        if (!IsOwner())
            throw new InvalidOperationException("No Authorization!");
        Nep17Token.Burn(account, amount);
    }

    public static bool Update(ByteString nefFile, string manifest)
    {
        if (!IsOwner())
            throw new InvalidOperationException("No Authorization!");
        ContractManagement.Update(nefFile, manifest);
        return true;
    }
}
```

### 2.10.2 NEP-11 NFT Example

Here's a complete example of a NEP-11 NFT using the framework base class:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

public class MyTokenState : Nep11TokenState
{
    public BigInteger TokenId;
    public string Description;

    public MyTokenState(UInt160 owner, BigInteger tokenId, string description)
    {
        Owner = owner;
        TokenId = tokenId;
        Description = description;
        Name = "MyNFT #" + TokenId;
    }
}

[DisplayName("SimpleNFT")]
[ContractDescription("A simple NEP-11 NFT")]
[SupportedStandards(NepStandard.Nep11)]
[ContractPermission(Permission.Any, Method.Any)]
public class SimpleNFT : Nep11Token<MyTokenState>
{
    public override string Symbol { [Safe] get => "SNFT"; }

    private const byte PrefixOwner = 0xff;
    private static readonly UInt160 InitialOwner = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

    [Safe]
    public static UInt160 GetOwner()
    {
        var currentOwner = Storage.Get(new[] { PrefixOwner });
        return currentOwner == null ? InitialOwner : (UInt160)currentOwner;
    }

    private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

    public static void _deploy(object data, bool update)
    {
        if (update) return;
        UInt160 owner = (UInt160)data ?? InitialOwner;
        Storage.Put(new[] { PrefixOwner }, owner);
    }

    public static ByteString MintToken(UInt160 to, string description)
    {
        if (!IsOwner())
            throw new InvalidOperationException("No Authorization!");

        ByteString tokenId = NewTokenId();
        MyTokenState token = new MyTokenState(to, TotalSupply + 1, description);
        Mint(tokenId, token);
        return tokenId;
    }
}
```

These examples demonstrate:
- Proper use of framework base classes
- Token metadata implementation
- Owner management patterns
- Administrative functions
- Contract initialization
- Framework-provided functionality
