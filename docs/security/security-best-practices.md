# Neo Smart Contract Security Best Practices

This guide outlines essential security practices for developing secure smart contracts on the Neo N3 blockchain using C# and the Neo DevPack.

## Table of Contents

- [Core Security Principles](#core-security-principles)
- [Input Validation](#input-validation)
- [Access Control](#access-control)
- [State Management](#state-management)
- [External Interactions](#external-interactions)
- [Error Handling](#error-handling)
- [Gas Considerations](#gas-considerations)
- [Testing and Auditing](#testing-and-auditing)

## Core Security Principles

### 1. Defense in Depth

Implement multiple layers of security controls rather than relying on a single mechanism.

```csharp
public class SecureContract : SmartContract
{
    // Multiple validation layers
    public static bool TransferTokens(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Layer 1: Parameter validation
        Assert(from != null && to != null, "Invalid addresses");
        Assert(amount > 0, "Amount must be positive");
        
        // Layer 2: Authorization check
        Assert(Runtime.CheckWitness(from), "Unauthorized transfer");
        
        // Layer 3: Business logic validation
        Assert(GetBalance(from) >= amount, "Insufficient balance");
        
        // Layer 4: State consistency check
        return ExecuteTransfer(from, to, amount);
    }
}
```

### 2. Fail-Safe Defaults

Design systems to fail securely when unexpected conditions occur.

```csharp
public static bool WithdrawFunds(UInt160 user, BigInteger amount)
{
    // Fail-safe: Default to rejection
    if (!IsValidUser(user) || amount <= 0)
        return false;
    
    // Explicit checks before proceeding
    if (GetBalance(user) < amount)
        return false;
        
    return ProcessWithdrawal(user, amount);
}
```

### 3. Principle of Least Privilege

Grant only the minimum permissions necessary for functionality.

```csharp
[DisplayName("SecureVault")]
public class SecureVault : SmartContract
{
    // Separate roles with minimal required permissions
    private const byte ADMIN_ROLE = 1;
    private const byte OPERATOR_ROLE = 2;
    private const byte USER_ROLE = 3;
    
    [Safe]
    public static bool CanPerformAction(UInt160 user, byte requiredRole)
    {
        byte userRole = GetUserRole(user);
        return userRole <= requiredRole; // Lower numbers = higher privileges
    }
}
```

## Input Validation

### Always Validate External Inputs

Never trust external data without thorough validation.

```csharp
public static bool SetUserData(UInt160 user, string data)
{
    // Null and empty checks
    Assert(user != null && user.IsValid, "Invalid user address");
    Assert(!string.IsNullOrEmpty(data), "Data cannot be empty");
    
    // Length validation
    Assert(data.Length <= 1024, "Data too long");
    
    // Content validation
    Assert(IsValidDataFormat(data), "Invalid data format");
    
    // Authorization
    Assert(Runtime.CheckWitness(user), "Unauthorized");
    
    Storage.Put(Storage.CurrentContext, user, data);
    return true;
}

private static bool IsValidDataFormat(string data)
{
    // Implement specific validation logic
    return data.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
}
```

### Numeric Input Validation

Prevent integer overflow/underflow and validate ranges.

```csharp
public static bool SafeAdd(BigInteger a, BigInteger b)
{
    // Check for overflow before operation
    if (a > 0 && b > BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935") - a)
        throw new Exception("Integer overflow");
    
    if (a < 0 && b < BigInteger.Parse("-115792089237316195423570985008687907853269984665640564039457584007913129639935") - a)
        throw new Exception("Integer underflow");
        
    return a + b;
}

public static bool ValidateAmount(BigInteger amount)
{
    const BigInteger MAX_SUPPLY = 100_000_000_00000000; // Example: 100M tokens with 8 decimals
    return amount > 0 && amount <= MAX_SUPPLY;
}
```

## Access Control

### Implement Role-Based Access Control

```csharp
[DisplayName("RoleBasedContract")]
public class RoleBasedContract : SmartContract
{
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    
    // Role definitions
    private const string ADMIN_ROLE = "admin";
    private const string MODERATOR_ROLE = "moderator";
    private const string USER_ROLE = "user";
    
    // Role hierarchy (admins can do everything, moderators have subset, etc.)
    private static readonly Dictionary<string, string[]> RoleHierarchy = new()
    {
        [ADMIN_ROLE] = new[] { ADMIN_ROLE, MODERATOR_ROLE, USER_ROLE },
        [MODERATOR_ROLE] = new[] { MODERATOR_ROLE, USER_ROLE },
        [USER_ROLE] = new[] { USER_ROLE }
    };
    
    public static bool GrantRole(UInt160 user, string role)
    {
        Assert(HasRole(Runtime.CallingScriptHash, ADMIN_ROLE), "Only admins can grant roles");
        Assert(IsValidRole(role), "Invalid role");
        
        StorageMap roles = new(Storage.CurrentContext, "roles");
        roles.Put(user, role);
        
        OnRoleGranted(user, role);
        return true;
    }
    
    public static bool HasRole(UInt160 user, string role)
    {
        StorageMap roles = new(Storage.CurrentContext, "roles");
        string userRole = roles.Get(user);
        
        if (user == OWNER) return true; // Owner has all roles
        if (string.IsNullOrEmpty(userRole)) return false;
        
        return RoleHierarchy.ContainsKey(userRole) && 
               RoleHierarchy[userRole].Contains(role);
    }
    
    private static bool IsValidRole(string role)
    {
        return role == ADMIN_ROLE || role == MODERATOR_ROLE || role == USER_ROLE;
    }
    
    [DisplayName("RoleGranted")]
    public static event Action<UInt160, string> OnRoleGranted;
}
```

### Multi-Signature Security

```csharp
public class MultiSigContract : SmartContract
{
    private const int REQUIRED_SIGNATURES = 3;
    private const int MAX_SIGNERS = 5;
    
    public static bool ExecuteMultiSigOperation(UInt160[] signers, ByteString operation)
    {
        Assert(signers != null && signers.Length >= REQUIRED_SIGNATURES, 
               $"Minimum {REQUIRED_SIGNATURES} signatures required");
        Assert(signers.Length <= MAX_SIGNERS, "Too many signers");
        
        // Verify all signers are authorized
        foreach (var signer in signers)
        {
            Assert(IsAuthorizedSigner(signer), "Unauthorized signer");
            Assert(Runtime.CheckWitness(signer), "Invalid signature");
        }
        
        // Check for duplicate signers
        var uniqueSigners = signers.Distinct().ToArray();
        Assert(uniqueSigners.Length == signers.Length, "Duplicate signers not allowed");
        
        return ExecuteOperation(operation);
    }
    
    private static bool IsAuthorizedSigner(UInt160 signer)
    {
        StorageMap authorizedSigners = new(Storage.CurrentContext, "signers");
        return authorizedSigners.Get(signer) != null;
    }
}
```

## State Management

### Secure Storage Patterns

```csharp
public class SecureStorage : SmartContract
{
    // Use separate storage contexts for different data types
    private static StorageContext UserContext => new StorageContext()
    {
        Prefix = (byte)StoragePrefix.User
    };
    
    private static StorageContext BalanceContext => new StorageContext()
    {
        Prefix = (byte)StoragePrefix.Balance
    };
    
    private enum StoragePrefix : byte
    {
        User = 0x01,
        Balance = 0x02,
        Config = 0x03,
        Temp = 0x04
    }
    
    public static bool SecurelyStoreUserData(UInt160 user, ByteString data)
    {
        Assert(Runtime.CheckWitness(user), "Unauthorized");
        Assert(data.Length <= 1024, "Data too large");
        
        // Use user-specific key with namespace isolation
        ByteString key = CryptoLib.Sha256(user + data[..32]); // Use hash for privacy
        Storage.Put(UserContext, key, data);
        
        return true;
    }
    
    // Implement atomic operations for critical state changes
    public static bool AtomicTransfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Get current balances
        BigInteger fromBalance = GetBalance(from);
        BigInteger toBalance = GetBalance(to);
        
        // Validate operation
        Assert(fromBalance >= amount, "Insufficient balance");
        
        // Perform atomic update
        try
        {
            SetBalance(from, fromBalance - amount);
            SetBalance(to, toBalance + amount);
            
            // Verify final state
            Assert(GetBalance(from) == fromBalance - amount, "From balance mismatch");
            Assert(GetBalance(to) == toBalance + amount, "To balance mismatch");
            
            return true;
        }
        catch
        {
            // Rollback is automatic due to transaction failure
            throw new Exception("Transfer failed");
        }
    }
}
```

### Prevent Race Conditions

```csharp
public class RaceConditionSafe : SmartContract
{
    // Use locks for critical sections
    private static readonly ByteString GLOBAL_LOCK = "global_lock";
    
    public static bool SafeCriticalOperation(UInt160 user, BigInteger amount)
    {
        // Acquire lock
        Assert(!IsLocked(), "Operation in progress, try again");
        SetLock(true);
        
        try
        {
            // Critical section
            BigInteger balance = GetBalance(user);
            Assert(balance >= amount, "Insufficient balance");
            
            // Simulate complex operation that might be interrupted
            SetBalance(user, balance - amount);
            ProcessComplexLogic(amount);
            
            return true;
        }
        finally
        {
            // Always release lock
            SetLock(false);
        }
    }
    
    private static bool IsLocked()
    {
        return Storage.Get(Storage.CurrentContext, GLOBAL_LOCK) != null;
    }
    
    private static void SetLock(bool locked)
    {
        if (locked)
            Storage.Put(Storage.CurrentContext, GLOBAL_LOCK, 1);
        else
            Storage.Delete(Storage.CurrentContext, GLOBAL_LOCK);
    }
}
```

## External Interactions

### Safe Contract Calls

```csharp
public class SafeInteractions : SmartContract
{
    public static bool SafeExternalCall(UInt160 targetContract, string method, object[] args)
    {
        // Validate target contract
        Assert(IsWhitelistedContract(targetContract), "Contract not whitelisted");
        Assert(!string.IsNullOrEmpty(method), "Invalid method");
        
        // Set call limits
        const int MAX_GAS = 1_000_000;
        
        try
        {
            // Make the call with gas limit
            var result = Contract.Call(targetContract, method, CallFlags.ReadOnly, args);
            return result != null;
        }
        catch (Exception ex)
        {
            // Log the error but don't expose internal details
            OnExternalCallFailed(targetContract, method);
            return false;
        }
    }
    
    private static bool IsWhitelistedContract(UInt160 contract)
    {
        StorageMap whitelist = new(Storage.CurrentContext, "whitelist");
        return whitelist.Get(contract) != null;
    }
    
    [DisplayName("ExternalCallFailed")]
    public static event Action<UInt160, string> OnExternalCallFailed;
}
```

### Reentrancy Protection

```csharp
public class ReentrancyGuard : SmartContract
{
    private const byte REENTRANCY_GUARD = 1;
    
    public static bool NonReentrant(UInt160 user, BigInteger amount)
    {
        // Check if already executing
        Assert(!IsExecuting(), "Reentrant call detected");
        
        // Set guard
        SetExecuting(true);
        
        try
        {
            // External interactions after state changes
            BigInteger balance = GetBalance(user);
            Assert(balance >= amount, "Insufficient balance");
            
            // Update state BEFORE external calls
            SetBalance(user, balance - amount);
            
            // Now safe to make external calls
            bool success = NotifyExternalContract(user, amount);
            
            if (!success)
            {
                // Revert state change
                SetBalance(user, balance);
                return false;
            }
            
            return true;
        }
        finally
        {
            // Always clear guard
            SetExecuting(false);
        }
    }
    
    private static bool IsExecuting()
    {
        return Storage.Get(Storage.CurrentContext, "executing") != null;
    }
    
    private static void SetExecuting(bool executing)
    {
        if (executing)
            Storage.Put(Storage.CurrentContext, "executing", REENTRANCY_GUARD);
        else
            Storage.Delete(Storage.CurrentContext, "executing");
    }
    
    private static bool NotifyExternalContract(UInt160 user, BigInteger amount)
    {
        // Simulate external contract notification
        return true;
    }
}
```

## Error Handling

### Secure Error Handling

```csharp
public class SecureErrorHandling : SmartContract
{
    public static bool SecureOperation(UInt160 user, string data)
    {
        try
        {
            // Validate inputs first
            ValidateInputs(user, data);
            
            // Perform operation
            return ProcessUserData(user, data);
        }
        catch (Exception ex) when (ex.Message.Contains("Invalid input"))
        {
            // Handle specific known errors
            OnInputValidationFailed(user);
            return false;
        }
        catch (Exception)
        {
            // Generic error handling - don't expose internal details
            OnOperationFailed(user);
            return false;
        }
    }
    
    private static void ValidateInputs(UInt160 user, string data)
    {
        if (user == null || !user.IsValid)
            throw new Exception("Invalid input: user address");
            
        if (string.IsNullOrEmpty(data) || data.Length > 1024)
            throw new Exception("Invalid input: data format");
            
        if (!Runtime.CheckWitness(user))
            throw new Exception("Invalid input: unauthorized");
    }
    
    // Events for monitoring (don't expose sensitive details)
    [DisplayName("InputValidationFailed")]
    public static event Action<UInt160> OnInputValidationFailed;
    
    [DisplayName("OperationFailed")]
    public static event Action<UInt160> OnOperationFailed;
}
```

## Gas Considerations

### Gas-Efficient Security

```csharp
public class GasEfficientSecurity : SmartContract
{
    // Cache frequently accessed data
    private static readonly Dictionary<UInt160, bool> _adminCache = new();
    
    public static bool IsAdmin(UInt160 user)
    {
        // Check cache first to save gas
        if (_adminCache.ContainsKey(user))
            return _adminCache[user];
        
        // Load from storage only if not cached
        StorageMap admins = new(Storage.CurrentContext, "admins");
        bool isAdmin = admins.Get(user) != null;
        
        // Cache the result
        _adminCache[user] = isAdmin;
        
        return isAdmin;
    }
    
    // Batch operations to reduce gas costs
    public static bool BatchProcess(UInt160[] users, BigInteger[] amounts)
    {
        Assert(users.Length == amounts.Length, "Array length mismatch");
        Assert(users.Length <= 100, "Batch too large"); // Prevent gas limit issues
        
        // Validate all inputs first (fail fast)
        for (int i = 0; i < users.Length; i++)
        {
            Assert(users[i] != null && amounts[i] > 0, $"Invalid input at index {i}");
        }
        
        // Process batch
        for (int i = 0; i < users.Length; i++)
        {
            ProcessSingleOperation(users[i], amounts[i]);
        }
        
        return true;
    }
}
```

## Testing and Auditing

### Security Testing Framework

```csharp
[TestClass]
public class SecurityTests : TestBase<MyContract>
{
    [TestMethod]
    public void TestAccessControl()
    {
        // Test unauthorized access
        Assert.ThrowsException<Exception>(() => 
        {
            Contract.AdminFunction();
        });
        
        // Test with proper authorization
        Engine.SetCallingScriptHash(AdminAddress);
        Assert.IsTrue(Contract.AdminFunction());
    }
    
    [TestMethod]
    public void TestInputValidation()
    {
        // Test null inputs
        Assert.IsFalse(Contract.ProcessData(null, "data"));
        
        // Test oversized inputs
        string largeData = new string('x', 10000);
        Assert.IsFalse(Contract.ProcessData(ValidUser, largeData));
        
        // Test valid inputs
        Assert.IsTrue(Contract.ProcessData(ValidUser, "valid data"));
    }
    
    [TestMethod]
    public void TestReentrancyProtection()
    {
        // Simulate reentrancy attack
        bool firstCall = true;
        Contract.OnExternalCall += () =>
        {
            if (firstCall)
            {
                firstCall = false;
                Assert.ThrowsException<Exception>(() => Contract.WithdrawFunds(TestUser, 100));
            }
        };
        
        Assert.IsTrue(Contract.WithdrawFunds(TestUser, 100));
    }
}
```

This comprehensive security guide provides the foundation for developing secure Neo smart contracts. Remember that security is an ongoing process, and you should regularly review and update your security practices as the ecosystem evolves.