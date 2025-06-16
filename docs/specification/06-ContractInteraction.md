# 7. Contract Interaction

## 7.1 Contract Calls

Contracts can call methods in other contracts using `Contract.Call`.

### 7.1.1 Basic Call Syntax

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

// Basic contract call
object result = Contract.Call(scriptHash, "methodName", CallFlags.ReadStates, args);

// Example: Get NEO balance
BigInteger balance = (BigInteger)Contract.Call(NEO.Hash, "balanceOf",
    CallFlags.ReadStates, new object[] { account });

// Example: Transfer tokens
bool success = (bool)Contract.Call(tokenHash, "transfer",
    CallFlags.States, new object[] { from, to, amount });
```

### 7.1.2 Call Flags

| Flag | Description | Use Case |
|:-----|:------------|:---------|
| `None` | No permissions | Restricted calls |
| `ReadStates` | Read contract storage | Query operations |
| `WriteStates` | Write contract storage | State changes |
| `AllowCall` | Call other contracts | Nested calls |
| `AllowNotify` | Emit notifications | Event emission |
| `States` | Read + Write | Most operations |
| `All` | All permissions | Complex operations |

### 7.1.3 Call Examples

```csharp
// Read-only call
string symbol = (string)Contract.Call(tokenHash, "symbol", CallFlags.ReadStates);

// State-changing call
bool result = (bool)Contract.Call(tokenHash, "transfer", CallFlags.States,
    new object[] { from, to, amount });

// Complex call with all permissions
object data = Contract.Call(contractHash, "complexMethod", CallFlags.All, args);
```

## 7.2 Native Contracts

Neo N3 provides built-in native contracts for core functionality.

### 7.2.1 Common Native Contracts

| Contract | Description |
|:---------|:------------|
| `NEO` | NEO token management |
| `GAS` | GAS token management |
| `ContractManagement` | Contract deployment/updates |
| `StdLib` | Standard library functions |
| `CryptoLib` | Cryptographic functions |
| `Oracle` | External data access |

### 7.2.2 Native Contract Usage

```csharp
using Neo;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

// NEO and GAS tokens
BigInteger neoBalance = NEO.BalanceOf(account);
bool gasTransfer = GAS.Transfer(from, to, amount);

// Contract management
Contract contract = ContractManagement.GetContract(scriptHash);
ContractManagement.Deploy(nefFile, manifest, data);

// Standard library
ByteString serialized = StdLib.Serialize(obj);
object deserialized = StdLib.Deserialize(serialized);

// Cryptography
byte[] hash = CryptoLib.Sha256(data);
bool valid = CryptoLib.VerifyWithECDsa(message, pubKey, signature, curve);
```

## 7.3 Contract Permissions

Define which contracts can call your contract methods.

### 7.3.1 Permission Attributes

```csharp
using Neo;
using Neo.SmartContract.Framework.Attributes;

// Allow any contract to call any method
[ContractPermission(Permission.Any, Method.Any)]

// Allow specific contract to call any method
[ContractPermission("0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5", Method.Any)]

// Allow any contract to call specific method
[ContractPermission(Permission.Any, "transfer")]

// Allow specific contract to call specific method
[ContractPermission("0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5", "transfer")]
public class MyContract : SmartContract
{
    // Contract implementation
}
```

## 7.4 Contract Callbacks

Special methods called automatically by the system.

### 7.4.1 Token Payment Callbacks

```csharp
// NEP-17 token received
public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
{
    // Handle fungible token payment
    if (Runtime.CallingScriptHash == GAS.Hash)
    {
        // Handle GAS payment
    }
}

// NEP-11 token received
public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
{
    // Handle NFT payment (amount is always 1)
}
```

### 7.4.2 Lifecycle Callbacks

```csharp
// Called on deployment/update
public static void _deploy(object data, bool update)
{
    if (update) return; // Skip on updates

    // Initialize contract state
    Storage.Put(Storage.CurrentContext, "owner", (UInt160)data);
}

// Called for transaction verification
public static bool Verify()
{
    // Custom verification logic
    return Runtime.CheckWitness(GetOwner());
}
```

## 7.5 Common Patterns

### 7.5.1 Token Operations

```csharp
// Transfer tokens
bool success = NEO.Transfer(from, to, amount);
bool success = (bool)Contract.Call(tokenHash, "transfer", CallFlags.States,
    new object[] { from, to, amount });

// Approve and transfer from
Contract.Call(tokenHash, "approve", CallFlags.States, new object[] { owner, spender, amount });
Contract.Call(tokenHash, "transferFrom", CallFlags.States, new object[] { spender, from, to, amount });
```

### 7.5.2 Oracle Data Access

```csharp
// Request external data
Oracle.Request(url, filter, "onOracleResponse", userData, gasForResponse);

// Handle oracle response
public static void OnOracleResponse(string url, byte[] userData, int code, byte[] result)
{
    if (Runtime.CallingScriptHash != Oracle.Hash) return;
    if (code != 0) return; // Request failed

    // Process result
    BigInteger price = (BigInteger)StdLib.Deserialize(result);
    Storage.Put(Storage.CurrentContext, "price", price);
}
```

## 7.6 Security Best Practices

### 7.6.1 Authorization and Validation

```csharp
// Check authorization
if (!Runtime.CheckWitness(owner))
    throw new Exception("Not authorized");

// Validate inputs
if (account is null || !account.IsValid)
    throw new Exception("Invalid account");
if (amount <= 0)
    throw new Exception("Amount must be positive");
```

### 7.6.2 Reentrancy Protection

```csharp
// Protect entire contract
[NoReentrant]
public class MyContract : SmartContract { }

// Protect specific method
[NoReentrantMethod]
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount) { }
```

## 7.7 Simple Interaction Example

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.Numerics;

[DisplayName("SimpleInteraction")]
[ContractPermission(Permission.Any, Method.Any)]
public class SimpleInteraction : SmartContract
{
    public static event Action<UInt160, BigInteger> TokenReceived;

    // Call another contract
    public static BigInteger GetTokenBalance(UInt160 tokenHash, UInt160 account)
    {
        return (BigInteger)Contract.Call(tokenHash, "balanceOf",
            CallFlags.ReadStates, new object[] { account });
    }

    // Transfer tokens via contract call
    public static bool TransferTokens(UInt160 tokenHash, UInt160 to, BigInteger amount)
    {
        UInt160 from = Runtime.CallingScriptHash;
        return (bool)Contract.Call(tokenHash, "transfer",
            CallFlags.States, new object[] { from, to, amount });
    }

    // Handle token payments
    public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        UInt160 token = Runtime.CallingScriptHash;

        // Only accept GAS
        if (token != GAS.Hash)
            throw new Exception("Only GAS accepted");

        // Emit event
        TokenReceived(from, amount);
    }

    // Oracle data request
    public static void RequestData(string url)
    {
        Oracle.Request(url, "$.price", "onOracleResponse", null, 10000000);
    }

    public static void OnOracleResponse(string url, byte[] userData, int code, byte[] result)
    {
        if (Runtime.CallingScriptHash != Oracle.Hash) return;
        if (code == 0) // Success
        {
            Storage.Put(Storage.CurrentContext, "price", result);
        }
    }
}
```

This example demonstrates:
- Contract-to-contract calls
- Token balance queries
- Token transfer operations
- NEP-17 payment handling
- Oracle data requests
- Event emission
