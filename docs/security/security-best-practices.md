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
        Assert(address != null && address.IsValid, $"Invalid {paramName}");
    }
    
    public static void ValidateAmount(BigInteger amount, BigInteger max = 0, string paramName = "amount")
    {
        Assert(amount > 0, $"{paramName} must be positive");
        if (max > 0) Assert(amount <= max, $"{paramName} exceeds maximum");
    }
    
    public static void ValidateString(string input, int maxLength = 1024, bool allowEmpty = false)
    {
        if (!allowEmpty) Assert(!string.IsNullOrEmpty(input), "Input cannot be empty");
        if (input != null) Assert(input.Length <= maxLength, "Input too long");
    }
    
    public static void ValidateArray<T>(T[] array, int maxLength = 100)
    {
        Assert(array != null && array.Length > 0, "Array required");
        Assert(array.Length <= maxLength, "Array too large");
    }
}

// Usage example
public static bool ProcessUserData(UInt160 user, string data, BigInteger amount)
{
    InputValidator.ValidateAddress(user, "user");
    InputValidator.ValidateString(data, 512);
    InputValidator.ValidateAmount(amount, MAX_TOKEN_AMOUNT);
    Assert(Runtime.CheckWitness(user), "Unauthorized");
    
    return ProcessData(user, data, amount);
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

## Oracle Security

### Secure Oracle Integration

Oracle services provide external data to smart contracts but introduce security risks. Here's how to use them safely:

```csharp
public class SecureOracleContract : SmartContract
{
    // Whitelist trusted oracle sources
    private static readonly string[] TRUSTED_SOURCES = new[]
    {
        "https://api.trusted-provider.com",
        "https://secure-oracle.example.com"
    };
    
    // Track pending oracle requests
    private const string ORACLE_REQUEST_PREFIX = "oracle_request_";
    
    public static bool RequestOracleData(string url, string filter, BigInteger requestId)
    {
        // Validate oracle source
        Assert(IsWhitelistedSource(url), "Untrusted oracle source");
        
        // Validate request parameters
        Assert(url.Length <= 256, "URL too long");
        Assert(!string.IsNullOrEmpty(filter), "Filter required");
        Assert(requestId > 0, "Invalid request ID");
        
        // Check for duplicate requests
        var requestKey = ORACLE_REQUEST_PREFIX + requestId;
        Assert(Storage.Get(Storage.CurrentContext, requestKey) == null, 
               "Duplicate request");
        
        // Store request details for validation
        var request = new OracleRequest
        {
            Url = url,
            Filter = filter,
            Timestamp = Runtime.Time,
            Requester = Runtime.CallingScriptHash
        };
        
        Storage.Put(Storage.CurrentContext, requestKey, StdLib.Serialize(request));
        
        // Make oracle request with callback
        Oracle.Request(url, filter, "oracleCallback", requestId, Oracle.MinimumResponseGas);
        
        OnOracleRequested(requestId, url);
        return true;
    }
    
    public static void OracleCallback(string url, BigInteger requestId, 
                                     OracleResponseCode code, string result)
    {
        // Verify callback is from Oracle contract
        Assert(Runtime.CallingScriptHash == Oracle.Hash, "Invalid oracle callback");
        
        // Retrieve and validate request
        var requestKey = ORACLE_REQUEST_PREFIX + requestId;
        var requestData = Storage.Get(Storage.CurrentContext, requestKey);
        Assert(requestData != null, "Unknown oracle request");
        
        var request = (OracleRequest)StdLib.Deserialize(requestData);
        Assert(request.Url == url, "URL mismatch");
        
        // Check response code
        if (code != OracleResponseCode.Success)
        {
            OnOracleFailed(requestId, code);
            Storage.Delete(Storage.CurrentContext, requestKey);
            return;
        }
        
        // Validate response format
        Assert(IsValidResponse(result), "Invalid oracle response format");
        
        // Process the data with additional validation
        ProcessOracleData(requestId, result);
        
        // Clean up
        Storage.Delete(Storage.CurrentContext, requestKey);
        OnOracleSuccess(requestId, result);
    }
    
    private static bool IsWhitelistedSource(string url)
    {
        foreach (var trusted in TRUSTED_SOURCES)
        {
            if (url.StartsWith(trusted))
                return true;
        }
        return false;
    }
    
    private static bool IsValidResponse(string result)
    {
        // Implement response format validation
        // Example: Check JSON structure, data types, ranges
        try
        {
            var data = StdLib.JsonDeserialize(result);
            return data != null;
        }
        catch
        {
            return false;
        }
    }
    
    [DisplayName("OracleRequested")]
    public static event Action<BigInteger, string> OnOracleRequested;
    
    [DisplayName("OracleSuccess")]
    public static event Action<BigInteger, string> OnOracleSuccess;
    
    [DisplayName("OracleFailed")]
    public static event Action<BigInteger, OracleResponseCode> OnOracleFailed;
    
    private struct OracleRequest
    {
        public string Url;
        public string Filter;
        public uint Timestamp;
        public UInt160 Requester;
    }
}
```

### Oracle Security Best Practices

1. **Always whitelist oracle sources** - Never accept arbitrary URLs
2. **Validate all responses** - Check format, ranges, and data types
3. **Implement timeouts** - Clean up stale requests
4. **Rate limit requests** - Prevent spam and DoS
5. **Handle failures gracefully** - Don't let oracle failures break your contract

## Secure Randomness

### Using Runtime.GetRandom Safely

NEO provides `Runtime.GetRandom()` for randomness, but it must be used carefully:

```csharp
public class SecureRandomContract : SmartContract
{
    // IMPORTANT: Never use randomness for critical security decisions
    // like access control or fund distribution
    
    public static BigInteger GetSecureRandom(BigInteger min, BigInteger max)
    {
        Assert(max > min, "Invalid range");
        Assert(max - min <= BigInteger.Pow(10, 18), "Range too large");
        
        // Add nonce for better distribution
        var nonce = GetAndIncrementNonce();
        
        // Get random value
        var random = Runtime.GetRandom();
        
        // Combine with block information for additional entropy
        var blockHash = Ledger.GetBlock(Ledger.CurrentIndex).Hash;
        var combined = CryptoLib.Sha256(random + nonce + blockHash);
        
        // Convert to BigInteger and scale to range
        var value = new BigInteger(combined);
        var scaled = (value % (max - min)) + min;
        
        return scaled < 0 ? -scaled : scaled;
    }
    
    // Lottery with commit-reveal pattern
    public static bool StartLottery()
    {
        var lotteryId = GetNextLotteryId();
        
        // Set commit phase deadline
        var commitDeadline = Runtime.Time + 3600; // 1 hour
        var revealDeadline = commitDeadline + 3600; // 2 hours total
        
        SaveLottery(lotteryId, commitDeadline, revealDeadline);
        OnLotteryStarted(lotteryId, commitDeadline);
        
        return true;
    }
    
    public static bool CommitEntry(BigInteger lotteryId, ByteString commitment)
    {
        var lottery = GetLottery(lotteryId);
        Assert(Runtime.Time < lottery.CommitDeadline, "Commit phase ended");
        
        var participant = Runtime.CallingScriptHash;
        Assert(!HasCommitted(lotteryId, participant), "Already committed");
        
        // Store commitment
        SaveCommitment(lotteryId, participant, commitment);
        
        return true;
    }
    
    public static bool RevealEntry(BigInteger lotteryId, BigInteger number, BigInteger nonce)
    {
        var lottery = GetLottery(lotteryId);
        Assert(Runtime.Time >= lottery.CommitDeadline, "Still in commit phase");
        Assert(Runtime.Time < lottery.RevealDeadline, "Reveal phase ended");
        
        var participant = Runtime.CallingScriptHash;
        var commitment = GetCommitment(lotteryId, participant);
        Assert(commitment != null, "No commitment found");
        
        // Verify commitment matches
        var hash = CryptoLib.Sha256(number + nonce);
        Assert(hash == commitment, "Invalid reveal");
        
        // Store revealed number
        SaveReveal(lotteryId, participant, number);
        
        return true;
    }
    
    public static UInt160 DrawWinner(BigInteger lotteryId)
    {
        var lottery = GetLottery(lotteryId);
        Assert(Runtime.Time >= lottery.RevealDeadline, "Reveal phase not ended");
        Assert(!lottery.WinnerDrawn, "Winner already drawn");
        
        // Collect all revealed entries
        var entries = GetRevealedEntries(lotteryId);
        Assert(entries.Length > 0, "No valid entries");
        
        // Combine all revealed numbers with block data
        BigInteger combined = 0;
        foreach (var entry in entries)
        {
            combined += entry.Number;
        }
        
        // Add block randomness
        var blockRandom = Runtime.GetRandom();
        var finalRandom = (combined + new BigInteger(blockRandom)) % entries.Length;
        
        // Select winner
        var winner = entries[(int)finalRandom].Participant;
        lottery.WinnerDrawn = true;
        lottery.Winner = winner;
        SaveLottery(lotteryId, lottery);
        
        OnWinnerDrawn(lotteryId, winner);
        return winner;
    }
    
    private static BigInteger GetAndIncrementNonce()
    {
        const string NONCE_KEY = "random_nonce";
        var nonce = Storage.Get(Storage.CurrentContext, NONCE_KEY) ?? 0;
        Storage.Put(Storage.CurrentContext, NONCE_KEY, nonce + 1);
        return (BigInteger)nonce;
    }
    
    [DisplayName("LotteryStarted")]
    public static event Action<BigInteger, uint> OnLotteryStarted;
    
    [DisplayName("WinnerDrawn")]
    public static event Action<BigInteger, UInt160> OnWinnerDrawn;
}
```

### Randomness Best Practices

1. **Never use for security-critical decisions** - Randomness can be influenced by validators
2. **Implement commit-reveal schemes** - For fair random selection
3. **Combine multiple entropy sources** - Block data, user input, nonces
4. **Add time delays** - Prevent manipulation of block production
5. **Use deterministic alternatives** - When possible, avoid randomness entirely

## Denial of Service Protection

### Rate Limiting and DoS Prevention

```csharp
public class DoSProtectedContract : SmartContract
{
    private const int MAX_CALLS_PER_BLOCK = 10;
    private const int MAX_CALLS_PER_USER = 5;
    private const BigInteger MAX_ARRAY_SIZE = 100;
    private const BigInteger MAX_COMPUTATION_COST = 1000;
    
    // Rate limiting per user
    public static bool RateLimitedOperation(UInt160 user, object[] params)
    {
        Assert(Runtime.CheckWitness(user), "Unauthorized");
        
        // Check array size limits
        Assert(params.Length <= MAX_ARRAY_SIZE, "Input too large");
        
        // Check per-block rate limit
        var blockCalls = GetBlockCallCount();
        Assert(blockCalls < MAX_CALLS_PER_BLOCK, "Block rate limit exceeded");
        
        // Check per-user rate limit
        var userCalls = GetUserCallCount(user, Ledger.CurrentIndex);
        Assert(userCalls < MAX_CALLS_PER_USER, "User rate limit exceeded");
        
        // Increment counters
        IncrementBlockCallCount();
        IncrementUserCallCount(user);
        
        // Check computational cost
        var cost = EstimateComputationCost(params);
        Assert(cost <= MAX_COMPUTATION_COST, "Operation too expensive");
        
        return ProcessOperation(user, params);
    }
    
    // Prevent storage exhaustion
    public static bool StoreUserData(UInt160 user, ByteString data)
    {
        Assert(Runtime.CheckWitness(user), "Unauthorized");
        
        // Limit data size
        Assert(data.Length <= 1024, "Data too large");
        
        // Check user storage quota
        var currentUsage = GetUserStorageUsage(user);
        var newUsage = currentUsage + data.Length;
        Assert(newUsage <= MAX_USER_STORAGE, "Storage quota exceeded");
        
        // Charge for storage
        var storageFee = CalculateStorageFee(data.Length);
        Assert(ChargeUser(user, storageFee), "Insufficient balance for storage");
        
        // Store data
        Storage.Put(Storage.CurrentContext, GetUserDataKey(user), data);
        UpdateUserStorageUsage(user, newUsage);
        
        return true;
    }
    
    // Circuit breaker pattern
    private static bool CircuitBreaker(string operation)
    {
        var errorCount = GetRecentErrorCount(operation);
        
        if (errorCount > ERROR_THRESHOLD)
        {
            var lastReset = GetLastCircuitReset(operation);
            var timeSinceReset = Runtime.Time - lastReset;
            
            // Keep circuit open for cooldown period
            if (timeSinceReset < CIRCUIT_COOLDOWN)
            {
                OnCircuitOpen(operation);
                return false;
            }
            
            // Reset circuit
            ResetCircuit(operation);
        }
        
        return true;
    }
    
    [DisplayName("CircuitOpen")]
    public static event Action<string> OnCircuitOpen;
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