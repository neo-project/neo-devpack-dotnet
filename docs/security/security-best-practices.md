# Neo Smart Contract Security Implementation Guide

Practical implementation patterns for the core security principles outlined in the [Security Overview](security-overview.md).

## Table of Contents

- [Input Validation Patterns](#input-validation-patterns)
- [Secure State Management](#secure-state-management)
- [External Interaction Safety](#external-interaction-safety)
- [Error Handling Strategies](#error-handling-strategies)
- [Security Implementation Examples](#security-implementation-examples)

## Input Validation Patterns

### Comprehensive Input Validation Framework

```csharp
public static class InputValidator
{
    public static void ValidateAddress(UInt160 address, string paramName = "address")
    {
        ExecutionEngine.Assert(address != null && address.IsValid, $"Invalid {paramName}");
    }
    
    public static void ValidateAmount(BigInteger amount, BigInteger max = 0, string paramName = "amount")
    {
        ExecutionEngine.Assert(amount > 0, $"{paramName} must be positive");
        if (max > 0) ExecutionEngine.Assert(amount <= max, $"{paramName} exceeds maximum");
    }
    
    public static void ValidateString(string input, int maxLength = 1024, bool allowEmpty = false)
    {
        if (!allowEmpty) ExecutionEngine.Assert(!string.IsNullOrEmpty(input), "Input cannot be empty");
        if (input != null) ExecutionEngine.Assert(input.Length <= maxLength, "Input too long");
    }
    
    public static void ValidateArray<T>(T[] array, int maxLength = 100)
    {
        ExecutionEngine.Assert(array != null && array.Length > 0, "Array required");
        ExecutionEngine.Assert(array.Length <= maxLength, "Array too large");
    }
}

// Usage example
public static bool ProcessUserData(UInt160 user, string data, BigInteger amount)
{
    InputValidator.ValidateAddress(user, "user");
    InputValidator.ValidateString(data, 512);
    InputValidator.ValidateAmount(amount, MAX_TOKEN_AMOUNT);
    ExecutionEngine.Assert(Runtime.CheckWitness(user), "Unauthorized");
    
    return ProcessData(user, data, amount);
}
```

## Access Control

### Implement Role-Based Access Control

Use explicit parsing APIs for any hard-coded script hashes or keys to enforce validation:

```csharp
private static readonly UInt160 OWNER = UInt160.Parse("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB");
private static readonly ECPoint ADMIN_KEY = ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
```

```csharp
[DisplayName("RoleBasedContract")]
public class RoleBasedContract : SmartContract
{
    private static readonly UInt160 OWNER = UInt160.Parse("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB");
    
    // Role definitions
    private const string ADMIN_ROLE = "admin";
    private const string MODERATOR_ROLE = "moderator";
    private const string USER_ROLE = "user";
    
    // Role hierarchy (admins can do everything, moderators have subset, etc.)
    private static bool RoleImplies(string userRole, string requiredRole) => userRole switch
    {
        ADMIN_ROLE => true,
        MODERATOR_ROLE => requiredRole == MODERATOR_ROLE || requiredRole == USER_ROLE,
        USER_ROLE => requiredRole == USER_ROLE,
        _ => false
    };
    
    public static bool GrantRole(UInt160 user, string role)
    {
        UInt160 caller = Runtime.Transaction.Sender;
        ExecutionEngine.Assert(Runtime.CheckWitness(caller), "Access denied: Invalid signature");
        ExecutionEngine.Assert(HasRole(caller, ADMIN_ROLE), "Only admins can grant roles");
        ExecutionEngine.Assert(IsValidRole(role), "Invalid role");
        
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
        
        return RoleImplies(userRole, role);
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
        ExecutionEngine.Assert(signers != null && signers.Length >= REQUIRED_SIGNATURES, 
               $"Minimum {REQUIRED_SIGNATURES} signatures required");
        ExecutionEngine.Assert(signers.Length <= MAX_SIGNERS, "Too many signers");
        
        // Verify all signers are authorized
        foreach (var signer in signers)
        {
            ExecutionEngine.Assert(IsAuthorizedSigner(signer), "Unauthorized signer");
            ExecutionEngine.Assert(Runtime.CheckWitness(signer), "Invalid signature");
        }
        
        // Check for duplicate signers (avoid LINQ/HashSet)
        for (int i = 0; i < signers.Length; i++)
        {
            for (int j = i + 1; j < signers.Length; j++)
            {
                ExecutionEngine.Assert(signers[i] != signers[j], "Duplicate signers not allowed");
            }
        }
        
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
    // Use StorageMap/explicit prefixes to separate data under one context
    private static StorageContext UserContext => Storage.CurrentContext;
    private static StorageContext BalanceContext => Storage.CurrentContext;
    
    private enum StoragePrefix : byte
    {
        User = 0x01,
        Balance = 0x02,
        Config = 0x03,
        Temp = 0x04
    }
    
    public static bool SecurelyStoreUserData(UInt160 user, ByteString data)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Unauthorized");
        ExecutionEngine.Assert(data.Length <= 1024, "Data too large");
        
        // Use user-specific key with namespace isolation
        ByteString key = CryptoLib.Sha256(user.Concat((ByteString)((byte[])data).Take(32))); // Use hash for privacy
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
        ExecutionEngine.Assert(fromBalance >= amount, "Insufficient balance");
        
        // Perform atomic update
        try
        {
            SetBalance(from, fromBalance - amount);
            SetBalance(to, toBalance + amount);
            
            // Verify final state
            ExecutionEngine.Assert(GetBalance(from) == fromBalance - amount, "From balance mismatch");
            ExecutionEngine.Assert(GetBalance(to) == toBalance + amount, "To balance mismatch");
            
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
        ExecutionEngine.Assert(!IsLocked(), "Operation in progress, try again");
        SetLock(true);
        
        try
        {
            // Critical section
            BigInteger balance = GetBalance(user);
            ExecutionEngine.Assert(balance >= amount, "Insufficient balance");
            
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
        ExecutionEngine.Assert(IsWhitelistedContract(targetContract), "Contract not whitelisted");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(method), "Invalid method");
        
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
        ExecutionEngine.Assert(!IsExecuting(), "Reentrant call detected");
        
        // Set guard
        SetExecuting(true);
        
        try
        {
            // External interactions after state changes
            BigInteger balance = GetBalance(user);
            ExecutionEngine.Assert(balance >= amount, "Insufficient balance");
            
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
        catch (Exception ex)
        {
            // Neo contracts do not support exception filters; branch manually.
            if (ex.Message.Contains("Invalid input"))
            {
                OnInputValidationFailed(user);
            }
            else
            {
                OnOperationFailed(user);
            }
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
    public static bool IsAdmin(UInt160 user)
    {
        StorageMap admins = new(Storage.CurrentContext, "admins");
        return admins.Get(user) != null;
    }
    
    // Batch operations to reduce gas costs
    public static bool BatchProcess(UInt160[] users, BigInteger[] amounts)
    {
        ExecutionEngine.Assert(users.Length == amounts.Length, "Array length mismatch");
        ExecutionEngine.Assert(users.Length <= 100, "Batch too large"); // Prevent gas limit issues
        
        // Validate all inputs first (fail fast)
        for (int i = 0; i < users.Length; i++)
        {
            ExecutionEngine.Assert(users[i] != null && amounts[i] > 0, $"Invalid input at index {i}");
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
