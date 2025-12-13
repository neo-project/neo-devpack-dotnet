# Gas Optimization Security Considerations

Advanced patterns for optimizing gas consumption while maintaining security, including DoS prevention, economic attack mitigation, and performance optimization techniques.

> **Foundation**: Review [Common Vulnerabilities](common-vulnerabilities.md#gas-limit-issues) for basic gas security concepts.

## Table of Contents

- [Gas Security Fundamentals](#gas-security-fundamentals)
- [Common Gas-Related Security Issues](#common-gas-related-security-issues)
- [Secure Gas Optimization Patterns](#secure-gas-optimization-patterns)
- [Gas Attack Vectors](#gas-attack-vectors)
- [Monitoring and Prevention](#monitoring-and-prevention)

## Gas Security Fundamentals

### Understanding Gas in Security Context

Gas optimization is crucial for user experience and cost efficiency, but it must never come at the expense of security. Poor gas optimization can lead to:

- **Denial of Service attacks** through gas limit exhaustion
- **Economic attacks** making contract interactions too expensive
- **State inconsistency** from partial execution due to gas limits
- **Front-running opportunities** in gas-sensitive operations

### Advanced Gas Security Patterns

This guide provides production-ready implementations for gas-efficient secure operations, focusing on complex scenarios and optimization techniques.

## Common Gas-Related Security Issues

### 1. Unbounded Loops (Gas Bomb)

#### ❌ Vulnerable Code

```csharp
public class GasBombVulnerable : SmartContract
{
    public static bool ProcessAllUsers()
    {
        // Dangerous: Unbounded loop that can exceed gas limit
        var iterator = (Iterator<ByteString>)Storage.Find(Storage.CurrentContext, "user_", FindOptions.ValuesOnly);
        while (iterator.Next())
        {
            ProcessUser(iterator.Value);
        }
        return true;
    }
    
    public static bool BatchTransfer(UInt160[] recipients, BigInteger[] amounts)
    {
        // Dangerous: No limit on array size
        for (int i = 0; i < recipients.Length; i++)
        {
            Transfer(recipients[i], amounts[i]);
        }
        return true;
    }
}
```

#### ✅ Secure Code

```csharp
public class GasBombSecure : SmartContract
{
    private const int MAX_BATCH_SIZE = 50;
    private const int MAX_ITERATION_COUNT = 100;
    
    public static bool ProcessUsersPaginated(int startIndex, int count)
    {
        ExecutionEngine.Assert(count > 0 && count <= MAX_ITERATION_COUNT, "Invalid count range");
        ExecutionEngine.Assert(startIndex >= 0, "Invalid start index");
        
        int processed = 0;
        int currentIndex = 0;
        
        var iterator = (Iterator<ByteString>)Storage.Find(Storage.CurrentContext, "user_", FindOptions.ValuesOnly);
        while (iterator.Next() && processed < count)
        {
            if (currentIndex >= startIndex)
            {
                if (!ProcessUserSafely(iterator.Value))
                {
                    OnUserProcessingFailed(iterator.Value);
                    break; // Stop on first failure to prevent cascading issues
                }
                processed++;
            }
            currentIndex++;
        }
        
        OnBatchProcessed(startIndex, processed);
        return processed > 0;
    }
    
    public static bool BatchTransferSecure(UInt160[] recipients, BigInteger[] amounts)
    {
        ExecutionEngine.Assert(recipients.Length == amounts.Length, "Array length mismatch");
        ExecutionEngine.Assert(recipients.Length <= MAX_BATCH_SIZE, $"Batch size exceeds limit of {MAX_BATCH_SIZE}");
        
        // Pre-validate all inputs to fail fast
        for (int i = 0; i < recipients.Length; i++)
        {
            ExecutionEngine.Assert(recipients[i] != null && amounts[i] > 0, $"Invalid input at index {i}");
        }
        
        // Track gas usage per operation
        int successCount = 0;
        for (int i = 0; i < recipients.Length; i++)
        {
            if (TransferSafely(recipients[i], amounts[i]))
            {
                successCount++;
            }
            else
            {
                OnTransferFailed(recipients[i], amounts[i], i);
            }
        }
        
        return successCount == recipients.Length;
    }
    
    private static bool ProcessUserSafely(ByteString userData)
    {
        // Implement efficient, bounded processing
        try
        {
            // Minimal, gas-efficient operations only
            UpdateUserLastSeen(userData);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

### 2. Gas Griefing Attacks

#### ❌ Vulnerable Code

```csharp
public class GasGriefingVulnerable : SmartContract
{
    public static bool ProcessUserRequests(UInt160[] users)
    {
        foreach (var user in users)
        {
            // Vulnerable: External call that can consume arbitrary gas
            try
            {
                Contract.Call(user, "processCallback", CallFlags.All);
            }
            catch
            {
                // Continues processing even if one user griefs
            }
        }
        return true;
    }
    
    public static bool ComplexCalculation(BigInteger input)
    {
        // Vulnerable: Computation complexity depends on input
        BigInteger result = 1;
        for (BigInteger i = 0; i < input; i++)
        {
            result = result * i + (BigInteger)CryptoLib.Sha256((ByteString)result);
        }
        return true;
    }
}
```

#### ✅ Secure Code

```csharp
public class GasGriefingSecure : SmartContract
{
    private const int MAX_EXTERNAL_CALL_GAS = 500_000;
    private const int MAX_COMPUTATION_ITERATIONS = 1000;
    
    public static bool ProcessUserRequestsSafely(UInt160[] users)
    {
        ExecutionEngine.Assert(users.Length <= 20, "Too many users in batch");
        
        foreach (var user in users)
        {
            if (IsWhitelistedUser(user))
            {
                // Secure: Limited gas for external calls
                try
                {
                    var result = Contract.Call(user, "processCallback", 
                        CallFlags.ReadOnly, // Limit to read-only calls
                        new object[0]);
                    
                    // Set reasonable timeout for external operations
                    OnUserProcessed(user, result != null);
                }
                catch (Exception ex)
                {
                    OnUserProcessingFailed(user, "External call failed");
                }
            }
        }
        return true;
    }
    
    public static bool BoundedCalculation(BigInteger input)
    {
        // Secure: Bound computation complexity
        ExecutionEngine.Assert(input >= 0 && input <= MAX_COMPUTATION_ITERATIONS, 
               $"Input must be between 0 and {MAX_COMPUTATION_ITERATIONS}");
        
        BigInteger result = 1;
        for (BigInteger i = 1; i <= input; i++)
        {
            // Use simpler operations to prevent gas griefing
            result = (result * i) % 1000000007; // Use modulo to keep numbers manageable
        }
        
        Storage.Put(Storage.CurrentContext, "last_calculation", result);
        return true;
    }
    
    private static bool IsWhitelistedUser(UInt160 user)
    {
        StorageMap whitelist = new(Storage.CurrentContext, "user_whitelist");
        return whitelist.Get(user) != null;
    }
}
```

## Secure Gas Optimization Patterns

### 1. Storage Access Optimization

```csharp
public class OptimizedStorage : SmartContract
{
    public static BigInteger GetBalanceOptimized(UInt160 user)
    {
        // Load from storage
        StorageMap balances = new(Storage.CurrentContext, "balances");
        return (BigInteger)balances.Get(user);
    }
    
    public static void SetBalanceOptimized(UInt160 user, BigInteger amount)
    {
        StorageMap balances = new(Storage.CurrentContext, "balances");
        balances.Put(user, amount);
    }
    
    // Batch storage operations to reduce gas costs
    public static bool BatchUpdateBalances(UInt160[] users, BigInteger[] amounts)
    {
        ExecutionEngine.Assert(users.Length == amounts.Length, "Array length mismatch");
        ExecutionEngine.Assert(users.Length <= 50, "Batch too large");
        
        StorageMap balances = new(Storage.CurrentContext, "balances");
        
        // Batch all storage operations
        for (int i = 0; i < users.Length; i++)
        {
            balances.Put(users[i], amounts[i]);
        }
        
        OnBatchBalanceUpdate(users.Length);
        return true;
    }
}
```

### 2. Computation Optimization

```csharp
public class OptimizedComputation : SmartContract
{
    // Pre-computed lookup tables for expensive operations
    private static readonly BigInteger[] PowersOfTwo = new BigInteger[]
    {
        1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192
    };
    
    public static BigInteger EfficientPowerOfTwo(int exponent)
    {
        // Use lookup table for small exponents
        if (exponent >= 0 && exponent < PowersOfTwo.Length)
        {
            return PowersOfTwo[exponent];
        }
        
        // Fallback to calculation for larger exponents with bounds
        ExecutionEngine.Assert(exponent <= 64, "Exponent too large");
        
        BigInteger result = 1;
        BigInteger baseValue = 2;
        
        // Use efficient exponentiation by squaring
        while (exponent > 0)
        {
            if (exponent % 2 == 1)
            {
                result *= baseValue;
            }
            baseValue *= baseValue;
            exponent /= 2;
        }
        
        return result;
    }
    
    // Optimize string operations
    public static bool EfficientStringValidation(string input)
    {
        // Early exits for common cases
        if (string.IsNullOrEmpty(input))
            return false;
            
        if (input.Length > 256)
            return false;
        
        // Use efficient character checking
        foreach (char c in input)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                return false;
        }
        
        return true;
    }
}
```

### 3. Event Optimization

```csharp
public class OptimizedEvents : SmartContract
{
    // Use efficient event parameter types
    [DisplayName("Transfer")]
    public static event Action<UInt160, UInt160, BigInteger> OnTransfer;
    
    [DisplayName("BatchOperation")]
    public static event Action<int, bool> OnBatchOperation; // Count and success flag
    
    public static bool EfficientTransfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Validate inputs
        ExecutionEngine.Assert(from != null && to != null, "Invalid addresses");
        ExecutionEngine.Assert(amount > 0, "Invalid amount");
        
        // Perform transfer logic
        BigInteger fromBalance = GetBalance(from);
        ExecutionEngine.Assert(fromBalance >= amount, "Insufficient balance");
        
        SetBalance(from, fromBalance - amount);
        SetBalance(to, GetBalance(to) + amount);
        
        // Emit single, efficient event
        OnTransfer(from, to, amount);
        
        return true;
    }
    
    public static bool BatchTransferEfficient(TransferData[] transfers)
    {
        ExecutionEngine.Assert(transfers.Length <= 50, "Batch too large");
        
        bool allSuccessful = true;
        
        foreach (var transfer in transfers)
        {
            if (!EfficientTransfer(transfer.From, transfer.To, transfer.Amount))
            {
                allSuccessful = false;
                break; // Stop on first failure
            }
        }
        
        // Single event for entire batch
        OnBatchOperation(transfers.Length, allSuccessful);
        
        return allSuccessful;
    }
    
    public struct TransferData
    {
        public UInt160 From;
        public UInt160 To;
        public BigInteger Amount;
    }
}
```

## Gas Attack Vectors

### 1. Gas Limit DoS

**Attack**: Submitting transactions that consume just enough gas to reach the block limit, preventing other transactions.

**Prevention**:
```csharp
public class GasLimitProtection : SmartContract
{
    private const int MAX_OPERATIONS_PER_CALL = 100;
    
    public static bool RateLimitedOperation(UInt160 user, int operationCount)
    {
        ExecutionEngine.Assert(operationCount <= MAX_OPERATIONS_PER_CALL, "Too many operations");
        
        // Check user's recent activity
        BigInteger lastCallTime = GetLastCallTime(user);
        BigInteger currentTime = Runtime.Time;
        
        // Rate limiting: minimum 1 second between calls
        ExecutionEngine.Assert(currentTime >= lastCallTime + 1000, "Rate limit exceeded");
        
        SetLastCallTime(user, currentTime);
        
        // Perform bounded operations
        for (int i = 0; i < operationCount; i++)
        {
            PerformSingleOperation(user, i);
        }
        
        return true;
    }
}
```

### 2. Economic Denial of Service

**Attack**: Making contract interactions so expensive that users can't afford to use them.

**Prevention**:
```csharp
public class EconomicDoSProtection : SmartContract
{
    public static bool CostEfficientOperation(UInt160 user, byte[] data)
    {
        // Validate input size to prevent excessive gas usage
        ExecutionEngine.Assert(data.Length <= 1024, "Data too large");
        
        // Use efficient algorithms
        ByteString hash = CryptoLib.Sha256(data); // Single hash operation
        
        // Store efficiently
        Storage.Put(Storage.CurrentContext, user.Concat((ByteString)((byte[])hash).Take(8)), data); // Use partial hash as key
        
        return true;
    }
    
    // Provide alternative low-cost operations
    public static bool LowCostQuery(UInt160 user)
    {
        // Read-only operation with minimal gas cost
        return Storage.Get(Storage.CurrentContext, user) != null;
    }
}
```

## Monitoring and Prevention

### Gas Usage Monitoring

```csharp
public class GasMonitoring : SmartContract
{
    [DisplayName("GasUsage")]
    public static event Action<string, int> OnGasUsage;
    
    public static bool MonitoredOperation(UInt160 user, string operation)
    {
        int initialGas = (int)Runtime.GasLeft;
        
        try
        {
            // Perform the operation
            bool result = PerformOperation(user, operation);
            
            // Calculate gas used
            int gasUsed = initialGas - (int)Runtime.GasLeft;
            OnGasUsage(operation, gasUsed);
            
            return result;
        }
        catch (Exception ex)
        {
            int gasUsed = initialGas - (int)Runtime.GasLeft;
            OnGasUsage($"{operation}_failed", gasUsed);
            throw;
        }
    }
    
    // Gas budget validation
    public static bool ValidateGasBudget(string operation, int expectedMaxGas)
    {
        int initialGas = (int)Runtime.GasLeft;
        ExecutionEngine.Assert(initialGas >= expectedMaxGas, "Insufficient gas for operation");
        
        return true;
    }
}
```

### Best Practices Summary

1. **Always bound loops and iterations**
2. **Validate input sizes and complexity**
3. **Use efficient data structures and algorithms**
4. **Cache frequently accessed storage data**
5. **Batch operations when possible**
6. **Implement gas usage monitoring**
7. **Set reasonable limits on user inputs**
8. **Fail fast on invalid conditions**
9. **Use read-only calls when possible**
10. **Regular gas cost analysis and optimization**

Remember: Gas optimization should enhance security, not compromise it. Always prioritize security over minor gas savings.
