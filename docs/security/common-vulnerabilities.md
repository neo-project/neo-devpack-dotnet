# Common Neo Smart Contract Vulnerabilities

This document outlines the most common security vulnerabilities found in Neo smart contracts and provides specific guidance on how to prevent them.

## Table of Contents

- [Reentrancy Attacks](#reentrancy-attacks)
- [Integer Overflow/Underflow](#integer-overflowunderflow)
- [Access Control Bypass](#access-control-bypass)
- [Unchecked External Calls](#unchecked-external-calls)
- [Storage Manipulation](#storage-manipulation)
- [Gas Limit Issues](#gas-limit-issues)
- [Timestamp Dependencies](#timestamp-dependencies)
- [Front-Running Attacks](#front-running-attacks)

## Reentrancy Attacks

Reentrancy occurs when an external contract calls back into your contract before the first function call is finished, potentially leading to unexpected behavior.

### ❌ Vulnerable Code

```csharp
public class VulnerableContract : SmartContract
{
    public static bool Withdraw(UInt160 user, BigInteger amount)
    {
        BigInteger balance = GetBalance(user);
        
        // Vulnerable: External call before state update
        if (NotifyExternalContract(user, amount))
        {
            // State change happens after external call
            SetBalance(user, balance - amount);
            return true;
        }
        return false;
    }
    
    private static bool NotifyExternalContract(UInt160 user, BigInteger amount)
    {
        // This could call back into Withdraw() before state is updated
        return Contract.Call(user, "onWithdraw", CallFlags.All, amount) != null;
    }
}
```

### ✅ Secure Code

```csharp
public class SecureContract : SmartContract
{
    private static readonly ByteString LOCK_KEY = "withdrawal_lock";
    
    public static bool Withdraw(UInt160 user, BigInteger amount)
    {
        // Reentrancy guard
        Assert(!IsLocked(), "Withdrawal in progress");
        SetLock(true);
        
        try
        {
            BigInteger balance = GetBalance(user);
            Assert(balance >= amount, "Insufficient balance");
            
            // Update state FIRST
            SetBalance(user, balance - amount);
            
            // External calls AFTER state changes
            NotifyExternalContract(user, amount);
            
            return true;
        }
        finally
        {
            SetLock(false);
        }
    }
    
    private static bool IsLocked()
    {
        return Storage.Get(Storage.CurrentContext, LOCK_KEY) != null;
    }
    
    private static void SetLock(bool locked)
    {
        if (locked)
            Storage.Put(Storage.CurrentContext, LOCK_KEY, 1);
        else
            Storage.Delete(Storage.CurrentContext, LOCK_KEY);
    }
}
```

### Prevention Strategies

1. **Checks-Effects-Interactions Pattern**: Always update state before external calls
2. **Reentrancy Guards**: Use locks to prevent recursive calls
3. **Pull Over Push**: Let users withdraw funds rather than automatically sending them

## Integer Overflow/Underflow

Neo smart contracts use `BigInteger` which can handle large numbers, but you still need to validate ranges and prevent unexpected arithmetic operations.

### ❌ Vulnerable Code

```csharp
public class VulnerableArithmetic : SmartContract
{
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        BigInteger fromBalance = GetBalance(from);
        BigInteger toBalance = GetBalance(to);
        
        // Vulnerable: No overflow check for addition
        SetBalance(from, fromBalance - amount);
        SetBalance(to, toBalance + amount); // Could overflow
        
        return true;
    }
    
    public static bool CalculateReward(BigInteger baseAmount, BigInteger multiplier)
    {
        // Vulnerable: No check for extremely large results
        return baseAmount * multiplier;
    }
}
```

### ✅ Secure Code

```csharp
public class SecureArithmetic : SmartContract
{
    private static readonly BigInteger MAX_SUPPLY = BigInteger.Parse("100000000000000000000000000"); // Example max
    
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        Assert(amount > 0 && amount <= MAX_SUPPLY, "Invalid amount");
        
        BigInteger fromBalance = GetBalance(from);
        BigInteger toBalance = GetBalance(to);
        
        Assert(fromBalance >= amount, "Insufficient balance");
        
        // Check for overflow before addition
        Assert(toBalance <= MAX_SUPPLY - amount, "Balance overflow");
        
        SetBalance(from, fromBalance - amount);
        SetBalance(to, toBalance + amount);
        
        return true;
    }
    
    public static BigInteger SafeAdd(BigInteger a, BigInteger b)
    {
        Assert(a >= 0 && b >= 0, "Negative values not allowed");
        Assert(a <= MAX_SUPPLY - b, "Addition overflow");
        return a + b;
    }
    
    public static BigInteger SafeMultiply(BigInteger a, BigInteger b)
    {
        if (a == 0 || b == 0) return 0;
        
        Assert(a <= MAX_SUPPLY / b, "Multiplication overflow");
        return a * b;
    }
}
```

## Access Control Bypass

Improper access control implementation can allow unauthorized users to perform privileged operations.

### ❌ Vulnerable Code

```csharp
public class VulnerableAccess : SmartContract
{
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    
    public static bool AdminFunction()
    {
        // Vulnerable: Only checks transaction sender, not witness
        if (Runtime.ScriptContainer.Sender == OWNER)
        {
            return PerformAdminAction();
        }
        return false;
    }
    
    public static bool ModeratorFunction(UInt160 user)
    {
        // Vulnerable: No role verification
        if (user != null)
        {
            return PerformModeratorAction();
        }
        return false;
    }
}
```

### ✅ Secure Code

```csharp
public class SecureAccess : SmartContract
{
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    
    public static bool AdminFunction()
    {
        // Secure: Check witness (cryptographic proof)
        Assert(Runtime.CheckWitness(OWNER), "Unauthorized: Owner only");
        return PerformAdminAction();
    }
    
    public static bool ModeratorFunction(UInt160 user)
    {
        // Secure: Verify both role and witness
        Assert(user != null && user.IsValid, "Invalid user");
        Assert(Runtime.CheckWitness(user), "Unauthorized: Invalid signature");
        Assert(HasRole(user, "moderator"), "Unauthorized: Moderator role required");
        
        return PerformModeratorAction();
    }
    
    private static bool HasRole(UInt160 user, string role)
    {
        StorageMap roles = new(Storage.CurrentContext, "roles");
        return roles.Get(user + role) != null;
    }
    
    // Multi-signature support for critical operations
    public static bool CriticalAdminFunction(UInt160[] signers)
    {
        const int REQUIRED_SIGS = 3;
        Assert(signers.Length >= REQUIRED_SIGS, $"Minimum {REQUIRED_SIGS} signatures required");
        
        foreach (var signer in signers)
        {
            Assert(IsAuthorizedAdmin(signer), "Unauthorized signer");
            Assert(Runtime.CheckWitness(signer), "Invalid signature");
        }
        
        return PerformCriticalAction();
    }
}
```

## Unchecked External Calls

External contract calls can fail or behave unexpectedly, and your contract should handle these scenarios gracefully.

### ❌ Vulnerable Code

```csharp
public class VulnerableExternalCalls : SmartContract
{
    public static bool ProcessPayment(UInt160 token, UInt160 recipient, BigInteger amount)
    {
        // Vulnerable: Assumes external call always succeeds
        Contract.Call(token, "transfer", CallFlags.All, Runtime.ExecutingScriptHash, recipient, amount);
        
        // Continues execution assuming transfer succeeded
        UpdatePaymentRecord(recipient, amount);
        return true;
    }
    
    public static bool BatchTransfer(UInt160[] recipients, BigInteger[] amounts)
    {
        for (int i = 0; i < recipients.Length; i++)
        {
            // Vulnerable: One failure doesn't stop execution
            Contract.Call(TokenContract, "transfer", CallFlags.All, recipients[i], amounts[i]);
        }
        return true; // Always returns true regardless of failures
    }
}
```

### ✅ Secure Code

```csharp
public class SecureExternalCalls : SmartContract
{
    public static bool ProcessPayment(UInt160 token, UInt160 recipient, BigInteger amount)
    {
        // Validate inputs
        Assert(token != null && recipient != null, "Invalid addresses");
        Assert(amount > 0, "Invalid amount");
        Assert(IsWhitelistedToken(token), "Token not whitelisted");
        
        try
        {
            // Check result of external call
            var result = Contract.Call(token, "transfer", CallFlags.All, 
                Runtime.ExecutingScriptHash, recipient, amount);
            
            bool success = result != null && (bool)result;
            
            if (success)
            {
                UpdatePaymentRecord(recipient, amount);
                OnPaymentProcessed(recipient, amount);
                return true;
            }
            else
            {
                OnPaymentFailed(recipient, amount, "Transfer failed");
                return false;
            }
        }
        catch (Exception ex)
        {
            OnPaymentFailed(recipient, amount, "External call exception");
            return false;
        }
    }
    
    public static bool BatchTransfer(UInt160[] recipients, BigInteger[] amounts)
    {
        Assert(recipients.Length == amounts.Length, "Array length mismatch");
        Assert(recipients.Length <= 50, "Batch too large"); // Prevent gas issues
        
        bool[] results = new bool[recipients.Length];
        int successCount = 0;
        
        for (int i = 0; i < recipients.Length; i++)
        {
            try
            {
                var result = Contract.Call(TokenContract, "transfer", CallFlags.All, 
                    recipients[i], amounts[i]);
                results[i] = result != null && (bool)result;
                if (results[i]) successCount++;
            }
            catch
            {
                results[i] = false;
            }
        }
        
        // Emit detailed results
        OnBatchTransferCompleted(successCount, recipients.Length, results);
        
        // Return true only if all transfers succeeded
        return successCount == recipients.Length;
    }
    
    private static bool IsWhitelistedToken(UInt160 token)
    {
        StorageMap whitelist = new(Storage.CurrentContext, "token_whitelist");
        return whitelist.Get(token) != null;
    }
    
    [DisplayName("PaymentProcessed")]
    public static event Action<UInt160, BigInteger> OnPaymentProcessed;
    
    [DisplayName("PaymentFailed")]
    public static event Action<UInt160, BigInteger, string> OnPaymentFailed;
    
    [DisplayName("BatchTransferCompleted")]
    public static event Action<int, int, bool[]> OnBatchTransferCompleted;
}
```

## Storage Manipulation

Improper storage handling can lead to data corruption or unauthorized access to sensitive information.

### ❌ Vulnerable Code

```csharp
public class VulnerableStorage : SmartContract
{
    public static bool SetUserData(UInt160 user, string key, ByteString data)
    {
        // Vulnerable: Direct key usage allows collision attacks
        Storage.Put(Storage.CurrentContext, key, data);
        return true;
    }
    
    public static ByteString GetSensitiveData(string identifier)
    {
        // Vulnerable: No access control on sensitive data
        return Storage.Get(Storage.CurrentContext, "sensitive_" + identifier);
    }
    
    public static bool DeleteUserAccount(UInt160 user)
    {
        // Vulnerable: Bulk deletion without proper checks
        Storage.Delete(Storage.CurrentContext, user);
        return true;
    }
}
```

### ✅ Secure Code

```csharp
public class SecureStorage : SmartContract
{
    private static readonly byte[] NAMESPACE_USER = new byte[] { 0x01 };
    private static readonly byte[] NAMESPACE_ADMIN = new byte[] { 0x02 };
    private static readonly byte[] NAMESPACE_SENSITIVE = new byte[] { 0x03 };
    
    public static bool SetUserData(UInt160 user, string key, ByteString data)
    {
        Assert(Runtime.CheckWitness(user), "Unauthorized");
        Assert(!string.IsNullOrEmpty(key) && key.Length <= 64, "Invalid key");
        Assert(data.Length <= 1024, "Data too large");
        
        // Secure: Use namespaced keys to prevent collisions
        ByteString storageKey = CreateUserKey(user, key);
        Storage.Put(Storage.CurrentContext, storageKey, data);
        
        OnUserDataUpdated(user, key);
        return true;
    }
    
    public static ByteString GetSensitiveData(UInt160 requester, string identifier)
    {
        // Secure: Access control for sensitive data
        Assert(HasAdminAccess(requester), "Unauthorized: Admin access required");
        Assert(Runtime.CheckWitness(requester), "Invalid signature");
        
        ByteString key = CreateSensitiveKey(identifier);
        ByteString data = Storage.Get(Storage.CurrentContext, key);
        
        OnSensitiveDataAccessed(requester, identifier);
        return data;
    }
    
    public static bool DeleteUserAccount(UInt160 user, UInt160 admin)
    {
        Assert(Runtime.CheckWitness(admin), "Admin signature required");
        Assert(HasAdminAccess(admin), "Unauthorized: Admin access required");
        Assert(Runtime.CheckWitness(user), "User consent required");
        
        // Secure: Selective deletion with audit trail
        string[] userKeys = GetUserKeys(user);
        foreach (string key in userKeys)
        {
            ByteString storageKey = CreateUserKey(user, key);
            Storage.Delete(Storage.CurrentContext, storageKey);
        }
        
        OnUserAccountDeleted(user, admin);
        return true;
    }
    
    private static ByteString CreateUserKey(UInt160 user, string key)
    {
        return NAMESPACE_USER.Concat(user).Concat(Encoding.UTF8.GetBytes(key));
    }
    
    private static ByteString CreateSensitiveKey(string identifier)
    {
        ByteString hash = CryptoLib.Sha256(Encoding.UTF8.GetBytes(identifier));
        return NAMESPACE_SENSITIVE.Concat(hash);
    }
    
    private static bool HasAdminAccess(UInt160 user)
    {
        StorageMap admins = new(Storage.CurrentContext, "admins");
        return admins.Get(user) != null;
    }
    
    [DisplayName("UserDataUpdated")]
    public static event Action<UInt160, string> OnUserDataUpdated;
    
    [DisplayName("SensitiveDataAccessed")]
    public static event Action<UInt160, string> OnSensitiveDataAccessed;
    
    [DisplayName("UserAccountDeleted")]
    public static event Action<UInt160, UInt160> OnUserAccountDeleted;
}
```

## Gas Limit Issues

Poorly designed contracts can consume excessive gas or hit gas limits, causing transactions to fail.

### ❌ Vulnerable Code

```csharp
public class GasVulnerable : SmartContract
{
    public static bool ProcessLargeArray(UInt160[] users)
    {
        // Vulnerable: No limit on array size
        for (int i = 0; i < users.Length; i++)
        {
            // Complex operation for each user
            ProcessComplexUserOperation(users[i]);
        }
        return true;
    }
    
    public static string[] GetAllUsers()
    {
        // Vulnerable: Could return massive arrays
        List<string> allUsers = new List<string>();
        
        // Iterating through potentially unlimited storage
        var iterator = Storage.Find(Storage.CurrentContext, "user_", FindOptions.None);
        while (iterator.Next())
        {
            allUsers.Add(iterator.Value);
        }
        
        return allUsers.ToArray();
    }
}
```

### ✅ Secure Code

```csharp
public class GasEfficient : SmartContract
{
    private const int MAX_BATCH_SIZE = 50;
    private const int MAX_RESULTS = 100;
    
    public static bool ProcessBatchedArray(UInt160[] users, int batchIndex)
    {
        Assert(users.Length <= MAX_BATCH_SIZE, $"Batch size cannot exceed {MAX_BATCH_SIZE}");
        Assert(batchIndex >= 0, "Invalid batch index");
        
        for (int i = 0; i < users.Length; i++)
        {
            if (!ProcessSingleUserEfficiently(users[i]))
            {
                OnUserProcessingFailed(users[i], batchIndex, i);
                return false;
            }
        }
        
        OnBatchProcessed(batchIndex, users.Length);
        return true;
    }
    
    public static string[] GetUsersPaginated(int offset, int limit)
    {
        Assert(offset >= 0, "Invalid offset");
        Assert(limit > 0 && limit <= MAX_RESULTS, $"Limit must be 1-{MAX_RESULTS}");
        
        List<string> users = new List<string>();
        int currentIndex = 0;
        int collected = 0;
        
        var iterator = Storage.Find(Storage.CurrentContext, "user_", FindOptions.None);
        while (iterator.Next() && collected < limit)
        {
            if (currentIndex >= offset)
            {
                users.Add(iterator.Value);
                collected++;
            }
            currentIndex++;
        }
        
        return users.ToArray();
    }
    
    // Gas-efficient single operations
    private static bool ProcessSingleUserEfficiently(UInt160 user)
    {
        // Simplified, gas-efficient processing
        ByteString userData = Storage.Get(Storage.CurrentContext, "user_" + user);
        if (userData != null)
        {
            // Minimal processing to save gas
            UpdateUserTimestamp(user);
            return true;
        }
        return false;
    }
    
    private static void UpdateUserTimestamp(UInt160 user)
    {
        // Efficient timestamp update
        Storage.Put(Storage.CurrentContext, "timestamp_" + user, Runtime.Time);
    }
    
    [DisplayName("BatchProcessed")]
    public static event Action<int, int> OnBatchProcessed;
    
    [DisplayName("UserProcessingFailed")]
    public static event Action<UInt160, int, int> OnUserProcessingFailed;
}
```

## Prevention Best Practices

### General Security Checklist

1. **Input Validation**: Always validate all external inputs
2. **Access Control**: Implement proper authorization checks
3. **State Management**: Update state before external calls
4. **Error Handling**: Handle failures gracefully
5. **Gas Optimization**: Design for efficient gas usage
6. **Testing**: Comprehensive security testing
7. **Auditing**: Regular security audits
8. **Monitoring**: Event logging for security monitoring

### Code Review Guidelines

- Review all external interactions
- Verify access control mechanisms
- Check for potential race conditions
- Validate all arithmetic operations
- Ensure proper error handling
- Test edge cases and failure scenarios
- Verify gas efficiency

Remember: Security is not a one-time implementation but an ongoing process that requires constant vigilance and updates as new threats emerge.