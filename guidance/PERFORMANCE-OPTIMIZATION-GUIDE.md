# NEO Smart Contract Performance Optimization Guide

This guide provides comprehensive techniques and patterns for optimizing NEO smart contract performance, focusing on gas efficiency, storage optimization, and computational efficiency.

## Table of Contents

1. [Performance Overview](#performance-overview)
2. [Gas Optimization Techniques](#gas-optimization-techniques)
3. [Storage Optimization](#storage-optimization)
4. [Computational Efficiency](#computational-efficiency)
5. [Batch Operations](#batch-operations)
6. [Performance Testing](#performance-testing)
7. [Benchmarks and Metrics](#benchmarks-and-metrics)
8. [Anti-Patterns to Avoid](#anti-patterns-to-avoid)

## Performance Overview

### Understanding NEO Gas Costs

Every operation in NEO has a gas cost. Understanding these costs is crucial for optimization:

| Operation | Gas Cost | Notes |
|-----------|----------|-------|
| Storage.Put (per byte) | 1 GAS | Most expensive operation |
| Storage.Get | 0.3 GAS | Cache frequently accessed data |
| Contract.Call | 0.01 GAS | Minimize external calls |
| Runtime.CheckWitness | 0.2 GAS | Cache results when possible |
| Crypto operations | 0.01-1 GAS | Varies by operation |

### Core Principles

- **Minimize storage operations** - Most expensive resource
- **Cache computed values** - Avoid recalculation  
- **Batch operations** - Group similar operations
- **Early exit patterns** - Fail fast validation
- **Optimize data structures** - Efficient representations

## Gas Optimization Techniques

### 1. Storage Access Optimization

```csharp
public class StorageOptimized : SmartContract
{
    // ❌ BAD: Multiple storage reads
    public static BigInteger GetTotalBad()
    {
        var balance1 = Storage.Get(Storage.CurrentContext, "balance1");
        var balance2 = Storage.Get(Storage.CurrentContext, "balance2");
        var balance3 = Storage.Get(Storage.CurrentContext, "balance3");
        return (BigInteger)balance1 + (BigInteger)balance2 + (BigInteger)balance3;
    }
    
    // ✅ GOOD: Single storage read with packed data
    public static BigInteger GetTotalGood()
    {
        var packedData = Storage.Get(Storage.CurrentContext, "packedBalances");
        return UnpackTotal(packedData);
    }
    
    // ✅ BEST: Use storage context for batch operations
    private static readonly StorageContext BalanceContext = Storage.CurrentContext;
    
    public static void BatchUpdateBalances(UInt160[] users, BigInteger[] amounts)
    {
        Assert(users.Length == amounts.Length, "Array length mismatch");
        
        using var snapshot = Storage.CurrentSnapshot;
        for (int i = 0; i < users.Length; i++)
        {
            var key = CreateKey("balance", users[i]);
            var currentBalance = (BigInteger)(snapshot.Get(key) ?? 0);
            snapshot.Put(key, currentBalance + amounts[i]);
        }
        snapshot.Commit();
    }
}
```

### 2. Caching Pattern

```csharp
public class CachingContract : SmartContract
{
    // Cache expensive computations
    private static readonly Dictionary<ByteString, object> _cache = new();
    
    public static BigInteger GetExpensiveCalculation(ByteString input)
    {
        // Check cache first
        if (_cache.TryGetValue(input, out var cached))
        {
            return (BigInteger)cached;
        }
        
        // Perform expensive calculation
        var result = PerformExpensiveCalculation(input);
        
        // Cache for future use (within transaction)
        _cache[input] = result;
        
        return result;
    }
    
    // Cache witness checks
    private static readonly Dictionary<UInt160, bool> _witnessCache = new();
    
    public static bool IsAuthorized(UInt160 user)
    {
        if (_witnessCache.TryGetValue(user, out var isAuthorized))
        {
            return isAuthorized;
        }
        
        isAuthorized = Runtime.CheckWitness(user);
        _witnessCache[user] = isAuthorized;
        
        return isAuthorized;
    }
}
```

### 3. Early Exit Optimization

```csharp
public class EarlyExitOptimized : SmartContract
{
    // ❌ BAD: Checks all conditions even if first fails
    public static bool ProcessTransactionBad(UInt160 from, UInt160 to, BigInteger amount)
    {
        var isFromValid = from != null && from.IsValid;
        var isToValid = to != null && to.IsValid;
        var isAmountValid = amount > 0 && amount <= MAX_AMOUNT;
        var hasBalance = GetBalance(from) >= amount;
        var isAuthorized = Runtime.CheckWitness(from);
        
        if (isFromValid && isToValid && isAmountValid && hasBalance && isAuthorized)
        {
            return ExecuteTransfer(from, to, amount);
        }
        return false;
    }
    
    // ✅ GOOD: Early exit on first failure
    public static bool ProcessTransactionGood(UInt160 from, UInt160 to, BigInteger amount)
    {
        // Cheapest checks first
        if (from == null || !from.IsValid) return false;
        if (to == null || !to.IsValid) return false;
        if (amount <= 0 || amount > MAX_AMOUNT) return false;
        
        // More expensive checks later
        if (!Runtime.CheckWitness(from)) return false;
        if (GetBalance(from) < amount) return false;
        
        return ExecuteTransfer(from, to, amount);
    }
}
```

## Storage Optimization

### 1. Data Packing

```csharp
public class DataPacking : SmartContract
{
    // ❌ BAD: Separate storage entries (3 storage operations)
    public static void StoreUserDataBad(UInt160 user, BigInteger balance, uint lastUpdate, bool isActive)
    {
        Storage.Put(Storage.CurrentContext, user + "_balance", balance);
        Storage.Put(Storage.CurrentContext, user + "_lastUpdate", lastUpdate);
        Storage.Put(Storage.CurrentContext, user + "_isActive", isActive);
    }
    
    // ✅ GOOD: Packed data (1 storage operation)
    public struct UserData
    {
        public BigInteger Balance;
        public uint LastUpdate;
        public bool IsActive;
    }
    
    public static void StoreUserDataGood(UInt160 user, UserData data)
    {
        Storage.Put(Storage.CurrentContext, user, StdLib.Serialize(data));
    }
    
    // ✅ BEST: Bit-packed data for maximum efficiency
    public static void StoreUserDataBest(UInt160 user, BigInteger balance, uint lastUpdate, bool isActive)
    {
        // Pack all data into single BigInteger
        // Balance: bits 0-255
        // LastUpdate: bits 256-287 (32 bits)
        // IsActive: bit 288
        var packed = balance | (new BigInteger(lastUpdate) << 256) | (isActive ? BigInteger.One << 288 : 0);
        Storage.Put(Storage.CurrentContext, user, packed);
    }
}
```

### 2. Storage Key Optimization

```csharp
public class KeyOptimization : SmartContract
{
    // ❌ BAD: Long string keys
    private const string USER_BALANCE_PREFIX = "user_balance_for_address_";
    
    public static void SetBalanceBad(UInt160 user, BigInteger balance)
    {
        Storage.Put(Storage.CurrentContext, USER_BALANCE_PREFIX + user, balance);
    }
    
    // ✅ GOOD: Short byte keys
    private const byte BALANCE_PREFIX = 0x01;
    
    public static void SetBalanceGood(UInt160 user, BigInteger balance)
    {
        var key = new byte[21];
        key[0] = BALANCE_PREFIX;
        user.ToArray().CopyTo(key, 1);
        Storage.Put(Storage.CurrentContext, key, balance);
    }
    
    // ✅ BEST: Composite keys for efficient querying
    public static ByteString CreateCompositeKey(byte prefix, UInt160 address, BigInteger tokenId)
    {
        return prefix + address + tokenId.ToByteArray();
    }
}
```

### 3. Lazy Deletion Pattern

```csharp
public class LazyDeletion : SmartContract
{
    // Instead of deleting many entries, mark them as deleted
    private const byte DELETED_FLAG = 0xFF;
    
    public static void BulkDelete(ByteString[] keys)
    {
        // ❌ BAD: Delete each key (expensive)
        // foreach (var key in keys)
        // {
        //     Storage.Delete(Storage.CurrentContext, key);
        // }
        
        // ✅ GOOD: Mark as deleted, clean up later
        var deletionTime = Runtime.Time;
        foreach (var key in keys)
        {
            Storage.Put(Storage.CurrentContext, key, DELETED_FLAG + deletionTime);
        }
    }
    
    public static bool IsDeleted(ByteString key)
    {
        var value = Storage.Get(Storage.CurrentContext, key);
        return value != null && value[0] == DELETED_FLAG;
    }
}
```

## Computational Efficiency

### 1. Loop Optimization

```csharp
public class LoopOptimization : SmartContract
{
    // ❌ BAD: Nested loops with storage access
    public static BigInteger CalculateTotalBad(UInt160[] users)
    {
        BigInteger total = 0;
        foreach (var user in users)
        {
            var userTokens = GetUserTokens(user); // Storage read
            foreach (var tokenId in userTokens)
            {
                var tokenValue = GetTokenValue(tokenId); // Another storage read
                total += tokenValue;
            }
        }
        return total;
    }
    
    // ✅ GOOD: Optimized with batch loading
    public static BigInteger CalculateTotalGood(UInt160[] users)
    {
        // Load all data in one pass
        var userData = BatchLoadUserData(users);
        
        // Process in memory
        BigInteger total = 0;
        foreach (var data in userData)
        {
            total += data.TotalValue; // Pre-calculated value
        }
        return total;
    }
    
    // ✅ BEST: Use accumulator pattern
    public static BigInteger CalculateTotalBest()
    {
        // Maintain running total in storage
        return (BigInteger)Storage.Get(Storage.CurrentContext, "totalValue");
    }
}
```

### 2. Algorithm Optimization

```csharp
public class AlgorithmOptimization : SmartContract
{
    // ❌ BAD: O(n²) algorithm
    public static bool HasDuplicatesBad(BigInteger[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            for (int j = i + 1; j < values.Length; j++)
            {
                if (values[i] == values[j]) return true;
            }
        }
        return false;
    }
    
    // ✅ GOOD: O(n) with hash set
    public static bool HasDuplicatesGood(BigInteger[] values)
    {
        var seen = new HashSet<BigInteger>();
        foreach (var value in values)
        {
            if (!seen.Add(value)) return true;
        }
        return false;
    }
    
    // Binary search instead of linear search
    public static int BinarySearch(BigInteger[] sortedArray, BigInteger target)
    {
        int left = 0, right = sortedArray.Length - 1;
        
        while (left <= right)
        {
            int mid = (left + right) / 2;
            if (sortedArray[mid] == target) return mid;
            if (sortedArray[mid] < target) left = mid + 1;
            else right = mid - 1;
        }
        
        return -1;
    }
}
```

## Batch Operations

### 1. Batch Transfer Pattern

```csharp
public class BatchOperations : SmartContract
{
    // ❌ BAD: Individual transfers
    public static void TransferToMultipleBad(UInt160 from, UInt160[] recipients, BigInteger amount)
    {
        foreach (var recipient in recipients)
        {
            Transfer(from, recipient, amount); // Multiple storage updates
        }
    }
    
    // ✅ GOOD: Batch transfer with single balance update
    public static void TransferToMultipleGood(UInt160 from, UInt160[] recipients, BigInteger amountEach)
    {
        var totalAmount = amountEach * recipients.Length;
        var fromBalance = GetBalance(from);
        
        Assert(fromBalance >= totalAmount, "Insufficient balance");
        
        // Update sender balance once
        SetBalance(from, fromBalance - totalAmount);
        
        // Update recipients
        foreach (var recipient in recipients)
        {
            var recipientBalance = GetBalance(recipient);
            SetBalance(recipient, recipientBalance + amountEach);
        }
        
        OnBatchTransfer(from, recipients, amountEach);
    }
    
    [DisplayName("BatchTransfer")]
    public static event Action<UInt160, UInt160[], BigInteger> OnBatchTransfer;
}
```

### 2. Batch State Updates

```csharp
public class BatchStateUpdate : SmartContract
{
    public struct StateUpdate
    {
        public ByteString Key;
        public ByteString Value;
        public bool IsDelete;
    }
    
    public static void BatchUpdate(StateUpdate[] updates)
    {
        // Sort updates to minimize storage context switches
        var sortedUpdates = updates.OrderBy(u => u.Key).ToArray();
        
        // Apply updates in batch
        using var snapshot = Storage.CurrentSnapshot;
        foreach (var update in sortedUpdates)
        {
            if (update.IsDelete)
                snapshot.Delete(update.Key);
            else
                snapshot.Put(update.Key, update.Value);
        }
        snapshot.Commit();
    }
}
```

## Performance Testing

### 1. Gas Measurement Framework

```csharp
[TestClass]
public class PerformanceTests : TestBase<MyContract>
{
    [TestMethod]
    public void MeasureGasConsumption()
    {
        var operations = new[]
        {
            ("Storage Write", () => Contract.StoreData("key", "value")),
            ("Storage Read", () => Contract.GetData("key")),
            ("Transfer", () => Contract.Transfer(User1, User2, 100)),
            ("Complex Calculation", () => Contract.CalculateRewards(User1))
        };
        
        foreach (var (name, operation) in operations)
        {
            var gasBefore = Engine.GasConsumed;
            operation();
            var gasUsed = Engine.GasConsumed - gasBefore;
            
            Console.WriteLine($"{name}: {gasUsed} GAS");
            Assert.IsTrue(gasUsed < MAX_ACCEPTABLE_GAS, $"{name} uses too much gas");
        }
    }
    
    [TestMethod]
    public void BenchmarkBatchVsIndividual()
    {
        var users = GenerateUsers(100);
        
        // Measure individual operations
        var individualGas = MeasureGas(() =>
        {
            foreach (var user in users)
            {
                Contract.UpdateBalance(user, 100);
            }
        });
        
        // Measure batch operation
        var batchGas = MeasureGas(() =>
        {
            Contract.BatchUpdateBalances(users, Enumerable.Repeat(100, users.Length).ToArray());
        });
        
        Console.WriteLine($"Individual: {individualGas} GAS");
        Console.WriteLine($"Batch: {batchGas} GAS");
        Console.WriteLine($"Savings: {(1 - batchGas / individualGas) * 100:F2}%");
        
        Assert.IsTrue(batchGas < individualGas * 0.5, "Batch should be at least 50% more efficient");
    }
}
```

### 2. Load Testing

```csharp
public class LoadTests : TestBase<MyContract>
{
    [TestMethod]
    public void TestScalability()
    {
        var testCases = new[] { 10, 50, 100, 500, 1000 };
        var results = new List<(int Count, long GasUsed, double TimeMs)>();
        
        foreach (var count in testCases)
        {
            var users = GenerateUsers(count);
            var stopwatch = Stopwatch.StartNew();
            var gasBefore = Engine.GasConsumed;
            
            // Perform operation
            Contract.ProcessBulkOperation(users);
            
            var gasUsed = Engine.GasConsumed - gasBefore;
            stopwatch.Stop();
            
            results.Add((count, gasUsed, stopwatch.ElapsedMilliseconds));
        }
        
        // Analyze scaling behavior
        foreach (var result in results)
        {
            Console.WriteLine($"Count: {result.Count}, Gas: {result.GasUsed}, Time: {result.TimeMs}ms");
        }
        
        // Verify linear or better scaling
        var scalingFactor = (double)results.Last().GasUsed / results.First().GasUsed;
        var expectedFactor = (double)results.Last().Count / results.First().Count;
        
        Assert.IsTrue(scalingFactor <= expectedFactor * 1.2, "Poor scaling detected");
    }
}
```

## Benchmarks and Metrics

### Reference Performance Metrics

| Operation | Target Gas Cost | Maximum Acceptable |
|-----------|----------------|-------------------|
| Simple Transfer | < 0.1 GAS | 0.5 GAS |
| Token Mint | < 0.2 GAS | 1 GAS |
| Complex Calculation | < 0.5 GAS | 2 GAS |
| Batch Transfer (100) | < 5 GAS | 20 GAS |
| Storage Update | < 0.01 GAS/byte | 0.05 GAS/byte |

### Performance Monitoring

```csharp
public class PerformanceMonitor : SmartContract
{
    private const string METRICS_PREFIX = "metrics_";
    
    public static void TrackPerformance(string operation, BigInteger gasUsed)
    {
        var key = METRICS_PREFIX + operation;
        var current = Storage.Get(Storage.CurrentContext, key);
        
        var metrics = current != null ? 
            (PerformanceMetrics)StdLib.Deserialize(current) : 
            new PerformanceMetrics();
        
        metrics.TotalCalls++;
        metrics.TotalGas += gasUsed;
        metrics.AverageGas = metrics.TotalGas / metrics.TotalCalls;
        metrics.LastGas = gasUsed;
        metrics.LastUpdate = Runtime.Time;
        
        if (gasUsed > metrics.MaxGas) metrics.MaxGas = gasUsed;
        if (gasUsed < metrics.MinGas || metrics.MinGas == 0) metrics.MinGas = gasUsed;
        
        Storage.Put(Storage.CurrentContext, key, StdLib.Serialize(metrics));
    }
    
    public struct PerformanceMetrics
    {
        public BigInteger TotalCalls;
        public BigInteger TotalGas;
        public BigInteger AverageGas;
        public BigInteger MaxGas;
        public BigInteger MinGas;
        public BigInteger LastGas;
        public uint LastUpdate;
    }
}
```

## Anti-Patterns to Avoid

### 1. Unbounded Loops

```csharp
// ❌ NEVER: Unbounded loop
public static void ProcessAllUsers()
{
    var users = GetAllUsers(); // Could be thousands
    foreach (var user in users)
    {
        ProcessUser(user); // Will hit gas limit
    }
}

// ✅ ALWAYS: Paginated processing
public static void ProcessUsersPaginated(int page, int pageSize = 100)
{
    var users = GetUsersPaginated(page, pageSize);
    foreach (var user in users)
    {
        ProcessUser(user);
    }
}
```

### 2. Recursive Storage Access

```csharp
// ❌ AVOID: Recursive storage reads
public static BigInteger GetTotalValue(UInt160 node)
{
    var value = GetNodeValue(node); // Storage read
    var children = GetNodeChildren(node); // Storage read
    
    foreach (var child in children)
    {
        value += GetTotalValue(child); // Recursive call with more storage reads
    }
    
    return value;
}

// ✅ PREFER: Maintain aggregated values
public static BigInteger GetTotalValue(UInt160 node)
{
    return GetCachedTotal(node); // Single storage read
}
```

### 3. String Concatenation in Loops

```csharp
// ❌ BAD: String concatenation
public static string BuildMessage(string[] parts)
{
    string result = "";
    foreach (var part in parts)
    {
        result += part + ","; // Creates new string each time
    }
    return result;
}

// ✅ GOOD: Use StringBuilder pattern or avoid strings
public static ByteString BuildMessage(ByteString[] parts)
{
    var result = new ByteString();
    foreach (var part in parts)
    {
        result = result + part + ",";
    }
    return result;
}
```

## Optimization Checklist

Before deploying to mainnet, ensure:

- [ ] All loops have bounded iterations
- [ ] Storage operations are minimized
- [ ] Data is packed efficiently
- [ ] Frequently accessed data is cached
- [ ] Early exit conditions are implemented
- [ ] Batch operations are used where possible
- [ ] Gas consumption is measured and within limits
- [ ] No unbounded recursive calls
- [ ] Storage keys are optimized for size
- [ ] Computational complexity is analyzed
- [ ] Performance tests pass with margin
- [ ] Scalability has been validated

Remember: Profile first, optimize second. Always measure the impact of your optimizations to ensure they actually improve performance.