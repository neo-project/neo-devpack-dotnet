# 6. Events and Notifications

## 6.1 Events

Events communicate contract state changes to external applications.

### 6.1.1 Event Declaration

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System;
using System.Numerics;

// Basic event
public static event Action<UInt160, UInt160, BigInteger> Transfer;

// Named event
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

// Event with no parameters
public static event Action ContractInitialized;
```

### 6.1.2 Supported Parameter Types

| Type | Description | Example |
|:-----|:------------|:--------|
| `byte[]` | Byte array | Transaction data |
| `string` | Text string | Token name |
| `BigInteger` | Large integer | Token amount |
| `bool` | Boolean | Success flag |
| `UInt160` | Address hash | Account address |
| `UInt256` | Transaction hash | Block hash |
| `ECPoint` | Public key | Signature key |

### 6.1.3 Event Constraints

- Maximum 16 parameters per event
- Must use `Action` delegate type
- Parameters must be serializable types

## 6.2 Event Emission

### 6.2.1 Emitting Events

```csharp
// Emit events by calling them like methods
Transfer(from, to, amount);
ContractInitialized();

// Example in a transfer method
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // ... transfer logic ...

    // Emit event
    Transfer(from, to, amount);
    return true;
}
```

### 6.2.2 Event Rules

- Events can only be emitted by the contract that defines them
- Events are included in transaction execution results
- Emitting events consumes GAS
- Contract must have `AllowNotify` permission

## 6.3 Notifications

### 6.3.1 Runtime Notifications

```csharp
using Neo;
using Neo.SmartContract.Framework.Services;

// Simple notification
Runtime.Notify("event_name");

// Notification with parameters
Runtime.Notify("transfer", from, to, amount);

// Dynamic notification name
string eventName = "user_action_" + actionType;
Runtime.Notify(eventName, user, data);
```

### 6.3.2 Events vs Notifications

| Feature | Events | Notifications |
|:--------|:-------|:--------------|
| Definition | Declared in contract | Dynamic |
| Manifest | Included | Not included |
| Naming | Fixed or DisplayName | Runtime |
| Use Case | Standard interface | Debugging, dynamic |

### 6.3.3 Logging

```csharp
// Simple log messages
Runtime.Log("Contract initialized");
Runtime.Log($"Transfer completed: {amount}");
```

## 6.4 Standard Events

### 6.4.1 NEP-17 (Fungible Tokens)

```csharp
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

// Usage: minting (from = null), burning (to = null), transfer
OnTransfer(null, account, amount);        // Mint
OnTransfer(account, null, amount);        // Burn
OnTransfer(from, to, amount);             // Transfer
```

### 6.4.2 NEP-11 (Non-Fungible Tokens)

```csharp
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger, ByteString> OnTransfer;

// Usage: amount is always 1 for NFTs
OnTransfer(from, to, 1, tokenId);
```

## 6.5 Event Subscription

External applications can subscribe to events using the Neo RPC API.

### 6.5.1 RPC Methods

- `getapplicationlog`: Get transaction events and logs
- `subscribe`: Real-time event subscription

### 6.5.2 Application Log Structure

Each log entry contains:
- **Contract**: Contract script hash
- **EventName**: Event name
- **State**: Event parameters array
- **Type**: Entry type (event/notification/log)

## 6.6 Simple Events Example

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("SimpleEvents")]
public class SimpleEvents : SmartContract
{
    // Event declarations
    [DisplayName("Transfer")]
    public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

    public static event Action<UInt160, string> UserAction;
    public static event Action ContractInitialized;

    // Simple transfer with event
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        if (!Runtime.CheckWitness(from)) return false;

        // ... transfer logic ...

        // Emit event
        OnTransfer(from, to, amount);

        // Emit notification
        Runtime.Notify("transfer_completed", from, to, amount);

        // Log message
        Runtime.Log($"Transferred {amount} from {from} to {to}");

        return true;
    }

    public static void DoAction(string action)
    {
        UInt160 user = Runtime.CallingScriptHash;

        // Emit custom event
        UserAction(user, action);

        // Dynamic notification
        Runtime.Notify($"action_{action}", user, Runtime.Time);
    }

    public static void _deploy(object data, bool update)
    {
        if (update) return;

        // Emit initialization event
        ContractInitialized();
        Runtime.Log("Contract deployed successfully");
    }
}
```

This example demonstrates:
- Standard Transfer event
- Custom events with different parameter types
- Runtime notifications
- Log messages
- Event emission in different contexts
