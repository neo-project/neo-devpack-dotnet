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

⚠️ **Critical**: Never use randomness for access control or fund distribution.

```csharp
public class SecureRandomContract : SmartContract
{
    public static BigInteger GetSecureRandom(BigInteger min, BigInteger max)
    {
        Assert(max > min, "Invalid range");
        
        // Combine multiple entropy sources
        var nonce = GetAndIncrementNonce();
        var random = Runtime.GetRandom();
        var blockHash = Ledger.GetBlock(Ledger.CurrentIndex).Hash;
        var combined = CryptoLib.Sha256(random + nonce + blockHash);
        
        // Scale to range
        var value = new BigInteger(combined);
        return (value % (max - min)) + min;
    }
    
    // Simple commit-reveal pattern for fair randomness
    public static bool CommitChoice(ByteString hashedValue)
    {
        var caller = Runtime.CallingScriptHash;
        Storage.Put(Storage.CurrentContext, "commit_" + caller, hashedValue);
        return true;
    }
    
    public static bool RevealChoice(BigInteger value, BigInteger nonce)
    {
        var caller = Runtime.CallingScriptHash;
        var commitment = Storage.Get(Storage.CurrentContext, "commit_" + caller);
        var hash = CryptoLib.Sha256(value + nonce);
        
        Assert(hash == commitment, "Invalid reveal");
        Storage.Put(Storage.CurrentContext, "reveal_" + caller, value);
        return true;
    }
    
    private static BigInteger GetAndIncrementNonce()
    {
        var nonce = (BigInteger)(Storage.Get(Storage.CurrentContext, "nonce") ?? 0);
        Storage.Put(Storage.CurrentContext, "nonce", nonce + 1);
        return nonce;
    }
}
```

### Randomness Best Practices

- **Avoid for critical decisions** - Don't use for access control or fund distribution
- **Use commit-reveal** - For fair random selection with multiple participants
- **Combine entropy sources** - Block data + user input + nonces
- **Prefer deterministic logic** - When randomness isn't essential

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

## Security Audit Tools and Automation

### Automated Security Testing

Integrate security testing into your development workflow with these tools and practices:

```csharp
[TestClass]
public class AutomatedSecurityTests : TestBase<MyContract>
{
    [TestMethod]
    public void Security_TestReentrancyProtection()
    {
        var attacker = new ReentrancyAttacker(Contract.Hash);
        Engine.Deploy(attacker);
        
        // Attempt reentrancy attack
        Assert.ThrowsException<Exception>(() => 
        {
            attacker.Attack(TestUser, 100);
        }, "Reentrancy attack succeeded - contract is vulnerable!");
    }
    
    [TestMethod]
    public void Security_TestIntegerOverflow()
    {
        var maxValue = BigInteger.Pow(2, 256) - 1;
        
        // Test addition overflow
        Assert.ThrowsException<Exception>(() => 
        {
            Contract.Add(maxValue, 1);
        }, "Integer overflow not detected");
        
        // Test multiplication overflow
        Assert.ThrowsException<Exception>(() => 
        {
            Contract.Multiply(maxValue, 2);
        }, "Multiplication overflow not detected");
    }
    
    [TestMethod]
    public void Security_TestAccessControl()
    {
        var unauthorizedUser = GenerateRandomAddress();
        
        // Test all admin functions
        var adminMethods = new[] { "Pause", "Unpause", "UpdateOwner", "Withdraw" };
        
        foreach (var method in adminMethods)
        {
            Engine.SetCallingScriptHash(unauthorizedUser);
            Assert.ThrowsException<Exception>(() => 
            {
                Contract.Call(method);
            }, $"Unauthorized access to {method} was allowed");
        }
    }
}
```

### Security Scanning Tools

#### 1. NEO Security Analyzer
```bash
# Install NEO Security Analyzer
dotnet tool install -g Neo.Security.Analyzer

# Run security scan
neo-security-scan --project MyContract.csproj --output security-report.json

# Run with specific rules
neo-security-scan --project MyContract.csproj --rules high-severity --fix
```

#### 2. Static Analysis Configuration
```xml
<!-- .editorconfig for security rules -->
[*.cs]
# Security rules
dotnet_diagnostic.NEO001.severity = error  # No hardcoded keys
dotnet_diagnostic.NEO002.severity = error  # Check witness before state changes
dotnet_diagnostic.NEO003.severity = error  # Validate external inputs
dotnet_diagnostic.NEO004.severity = warning # Prefer safe math operations
dotnet_diagnostic.NEO005.severity = error  # No unchecked external calls
```

#### 3. Automated Security Checklist
```csharp
public class SecurityAuditor
{
    public static SecurityAuditReport AuditContract(string contractPath)
    {
        var report = new SecurityAuditReport();
        
        // Check for common vulnerabilities
        report.AddCheck("Reentrancy Protection", CheckReentrancyProtection(contractPath));
        report.AddCheck("Integer Overflow", CheckIntegerOverflow(contractPath));
        report.AddCheck("Access Control", CheckAccessControl(contractPath));
        report.AddCheck("Input Validation", CheckInputValidation(contractPath));
        report.AddCheck("External Calls", CheckExternalCalls(contractPath));
        report.AddCheck("Randomness Usage", CheckRandomnessUsage(contractPath));
        report.AddCheck("Storage Security", CheckStorageSecurity(contractPath));
        
        return report;
    }
    
    private static bool CheckReentrancyProtection(string path)
    {
        // Analyze code for reentrancy guards
        var code = File.ReadAllText(path);
        return code.Contains("ReentrancyGuard") || 
               code.Contains("nonReentrant modifier");
    }
}
```

### Security Incident Response Templates

#### Incident Classification
```csharp
public enum SecurityIncidentLevel
{
    Low = 1,      // No immediate risk, monitor
    Medium = 2,   // Potential risk, investigate
    High = 3,     // Active threat, respond immediately
    Critical = 4  // System compromise, emergency response
}

public class SecurityIncident
{
    public string IncidentId { get; set; }
    public DateTime DetectedAt { get; set; }
    public SecurityIncidentLevel Level { get; set; }
    public string Description { get; set; }
    public string AffectedContract { get; set; }
    public BigInteger PotentialLoss { get; set; }
    public string ResponsePlan { get; set; }
}
```

#### Incident Response Procedure
```markdown
## Security Incident Response Template

### 1. Initial Assessment (0-15 minutes)
- [ ] Identify incident type and severity
- [ ] Assess immediate risk to funds
- [ ] Determine affected contracts/users
- [ ] Notify incident response team

### 2. Containment (15-30 minutes)
- [ ] Execute emergency pause if available
- [ ] Block malicious addresses if identified
- [ ] Prevent further damage
- [ ] Document all actions taken

### 3. Investigation (30-120 minutes)
- [ ] Analyze attack vector
- [ ] Review transaction history
- [ ] Identify root cause
- [ ] Assess total impact

### 4. Recovery (2-24 hours)
- [ ] Develop fix for vulnerability
- [ ] Test fix thoroughly
- [ ] Plan deployment strategy
- [ ] Prepare user communications

### 5. Post-Incident (24-72 hours)
- [ ] Deploy fixed contract
- [ ] Restore normal operations
- [ ] Compensate affected users if needed
- [ ] Publish incident report

### 6. Lessons Learned (Within 1 week)
- [ ] Conduct post-mortem analysis
- [ ] Update security procedures
- [ ] Implement additional safeguards
- [ ] Share findings with community
```

### Continuous Security Monitoring

```csharp
public class SecurityMonitor : SmartContract
{
    private const int MONITORING_INTERVAL = 300; // 5 minutes
    
    [DisplayName("SecurityAlert")]
    public static event Action<string, SecurityIncidentLevel> OnSecurityAlert;
    
    public static void MonitorSecurity()
    {
        // Check unusual activity patterns
        var recentTransactions = GetRecentTransactionCount();
        if (recentTransactions > NORMAL_TRANSACTION_THRESHOLD)
        {
            OnSecurityAlert("Unusual transaction volume", SecurityIncidentLevel.Medium);
        }
        
        // Check large transfers
        var largeTransfers = GetLargeTransfers();
        if (largeTransfers.Any())
        {
            OnSecurityAlert("Large transfer detected", SecurityIncidentLevel.High);
        }
        
        // Check failed transaction patterns
        var failureRate = CalculateFailureRate();
        if (failureRate > ACCEPTABLE_FAILURE_RATE)
        {
            OnSecurityAlert("High failure rate", SecurityIncidentLevel.Medium);
        }
    }
}
```

This comprehensive security guide provides the foundation for developing secure Neo smart contracts. Remember that security is an ongoing process, and you should regularly review and update your security practices as the ecosystem evolves.