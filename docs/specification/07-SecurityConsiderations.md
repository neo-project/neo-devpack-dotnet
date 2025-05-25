# 8. Security Considerations

## 8.1 Reentrancy Protection

Prevent reentrancy attacks where external calls can re-enter your contract.

### 8.1.1 Reentrancy Vulnerability

```csharp
// VULNERABLE: External call before state update
public static bool Withdraw(BigInteger amount)
{
    UInt160 sender = Runtime.CallingScriptHash;
    StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");
    BigInteger balance = (BigInteger)balances.Get(sender);

    if (balance < amount) return false;

    // DANGER: External call first
    bool success = GAS.Transfer(Runtime.ExecutingScriptHash, sender, amount);

    // State update after external call - vulnerable to reentrancy
    if (success) balances.Put(sender, balance - amount);

    return success;
}
```

### 8.1.2 Protection Methods

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

// Protect entire contract
[NoReentrant]
public class MyContract : SmartContract { }

// Protect specific method
[NoReentrantMethod]
public static bool Withdraw(BigInteger amount) { }

// Secure pattern: Checks-Effects-Interactions
public static bool SecureWithdraw(BigInteger amount)
{
    // 1. Checks
    UInt160 sender = Runtime.CallingScriptHash;
    StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");
    BigInteger balance = (BigInteger)balances.Get(sender);
    if (balance < amount) return false;

    // 2. Effects (update state first)
    balances.Put(sender, balance - amount);

    // 3. Interactions (external calls last)
    bool success = GAS.Transfer(Runtime.ExecutingScriptHash, sender, amount);
    if (!success) balances.Put(sender, balance); // Revert on failure

    return success;
}
```

## 8.2 Authorization

Verify caller authorization using `Runtime.CheckWitness`.

### 8.2.1 Basic Authorization

```csharp
using Neo;
using Neo.SmartContract.Framework.Services;
using System;

// Check single signer
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    if (!Runtime.CheckWitness(from))
        throw new Exception("Unauthorized");

    // Proceed with transfer
    return true;
}

// Check multiple signers (both required)
public static bool MultiSigOperation(UInt160 signer1, UInt160 signer2)
{
    if (!Runtime.CheckWitness(signer1) || !Runtime.CheckWitness(signer2))
        return false;

    // Proceed with operation
    return true;
}
```

### 8.2.2 Contract Verification

```csharp
// Custom verification logic
public static bool Verify()
{
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");
    return Runtime.CheckWitness(owner);
}

// Owner-only operations
public static bool UpdateContract(ByteString nefFile, string manifest)
{
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");
    if (!Runtime.CheckWitness(owner))
        throw new Exception("Only owner can update");

    ContractManagement.Update(nefFile, manifest);
    return true;
}
```

## 8.3 Exception Handling

Handle errors gracefully with try-catch blocks.

### 8.3.1 Basic Exception Handling

```csharp
// Simple try-catch
public static bool SafeOperation()
{
    try
    {
        // Risky operation
        Contract.Call(scriptHash, "method", CallFlags.All, args);
        return true;
    }
    catch
    {
        // Handle any exception
        return false;
    }
}

// State reversion on exception
public static bool SafeStateChange()
{
    BigInteger originalValue = (BigInteger)Storage.Get(Storage.CurrentContext, "value");

    try
    {
        Storage.Put(Storage.CurrentContext, "value", originalValue + 1);
        bool success = (bool)Contract.Call(scriptHash, "method", CallFlags.All, args);

        if (!success)
        {
            Storage.Put(Storage.CurrentContext, "value", originalValue);
            return false;
        }

        return true;
    }
    catch
    {
        // Revert state on exception
        Storage.Put(Storage.CurrentContext, "value", originalValue);
        return false;
    }
}
```

### 8.3.2 Exception Limitations

- Only one catch block per try
- No exception type specification
- No access to exception object
- No finally block support

## 8.4 Input Validation

Always validate inputs to prevent unexpected behavior.

### 8.4.1 Basic Validation

```csharp
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // Validate addresses
    if (from is null || !from.IsValid)
        throw new Exception("Invalid from address");
    if (to is null || !to.IsValid)
        throw new Exception("Invalid to address");

    // Validate amount
    if (amount <= 0)
        throw new Exception("Amount must be positive");

    // Proceed with transfer
    return true;
}
```

### 8.4.2 Range Validation

```csharp
// Safe BigInteger to int conversion
public static int SafeConvert(BigInteger value)
{
    if (value > int.MaxValue || value < int.MinValue)
        throw new Exception("Value out of range");

    return (int)value;
}
```

## 8.5 Access Control

Control who can perform sensitive operations.

### 8.5.1 Owner Pattern

```csharp
private static readonly byte[] OwnerKey = new byte[] { 0x01 };

public static void _deploy(object data, bool update)
{
    if (update) return;
    Storage.Put(Storage.CurrentContext, OwnerKey, (UInt160)data);
}

[Safe]
public static UInt160 GetOwner()
{
    return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
}

public static bool OnlyOwner()
{
    return Runtime.CheckWitness(GetOwner());
}

public static bool UpdateOwner(UInt160 newOwner)
{
    if (!OnlyOwner()) return false;
    Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
    return true;
}
```

### 8.5.2 Role-Based Access

```csharp
private static readonly byte[] PrefixRole = new byte[] { 0x01 };

public static bool HasRole(UInt160 account, string role)
{
    StorageMap roles = new StorageMap(Storage.CurrentContext, PrefixRole);
    return roles.Get(account.Concat(role)) != null;
}

public static bool GrantRole(UInt160 account, string role)
{
    if (!OnlyOwner()) return false;

    StorageMap roles = new StorageMap(Storage.CurrentContext, PrefixRole);
    roles.Put(account.Concat(role), 1);
    return true;
}

public static bool AdminOperation()
{
    UInt160 sender = Runtime.CallingScriptHash;
    if (!HasRole(sender, "admin"))
        throw new Exception("Admin role required");

    // Proceed with admin operation
    return true;
}
```

## 8.6 Best Practices

### 8.6.1 Gas Optimization

```csharp
// Monitor gas consumption
long gasLeft = Runtime.GasLeft;

// Optimize storage operations (most expensive)
// Batch operations when possible
// Use appropriate data structures
```

### 8.6.2 Defensive Programming

```csharp
public static bool SafeOperation()
{
    try
    {
        bool success = (bool)Contract.Call(scriptHash, "method", CallFlags.All, args);
        return success;
    }
    catch
    {
        return false; // Handle gracefully
    }
}
```

### 8.6.3 Development Checklist

- ✅ Validate all inputs
- ✅ Use `Runtime.CheckWitness` for authorization
- ✅ Apply reentrancy protection
- ✅ Handle exceptions gracefully
- ✅ Test thoroughly
- ✅ Code review
- ✅ Security audit before mainnet

## 8.7 Simple Security Example

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.Numerics;

[DisplayName("SecureToken")]
[ContractPermission(Permission.Any, Method.Any)]
[NoReentrant] // Prevent reentrancy attacks
public class SecureToken : SmartContract
{
    private static readonly byte[] OwnerKey = new byte[] { 0x01 };
    private static readonly byte[] PrefixBalance = new byte[] { 0x02 };

    public static event Action<UInt160, UInt160, BigInteger> Transfer;

    public static void _deploy(object data, bool update)
    {
        if (update) return;

        // Validate input
        UInt160 owner = (UInt160)data;
        if (owner is null || !owner.IsValid)
            throw new Exception("Invalid owner");

        Storage.Put(Storage.CurrentContext, OwnerKey, owner);
    }

    [Safe]
    public static UInt160 GetOwner()
    {
        return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
    }

    [Safe]
    public static BigInteger BalanceOf(UInt160 account)
    {
        // Input validation
        if (account is null || !account.IsValid)
            throw new Exception("Invalid account");

        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        return (BigInteger)balances.Get(account);
    }

    [NoReentrantMethod] // Additional protection for critical method
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Input validation
        if (from is null || !from.IsValid) return false;
        if (to is null || !to.IsValid) return false;
        if (amount <= 0) return false;

        // Authorization check
        if (!Runtime.CheckWitness(from)) return false;

        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        BigInteger fromBalance = (BigInteger)balances.Get(from);
        if (fromBalance < amount) return false;

        // Checks-Effects-Interactions pattern
        // 1. Effects: Update state first
        balances.Put(from, fromBalance - amount);
        balances.Put(to, (BigInteger)balances.Get(to) + amount);

        // 2. Interactions: External calls last
        Transfer(from, to, amount);

        return true;
    }

    public static bool Mint(UInt160 to, BigInteger amount)
    {
        // Owner-only operation
        if (!Runtime.CheckWitness(GetOwner()))
            throw new Exception("Only owner can mint");

        // Input validation
        if (to is null || !to.IsValid) return false;
        if (amount <= 0) return false;

        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        balances.Put(to, (BigInteger)balances.Get(to) + amount);

        Transfer(null, to, amount);
        return true;
    }
}
```

This example demonstrates:
- **Reentrancy protection** with `[NoReentrant]` and `[NoReentrantMethod]`
- **Input validation** for all parameters
- **Authorization checks** with `Runtime.CheckWitness`
- **Checks-Effects-Interactions** pattern
- **Owner-only operations** for sensitive functions
- **Safe method** attributes for read-only operations
