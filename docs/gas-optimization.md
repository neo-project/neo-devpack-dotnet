# Gas Optimization Guide

This guide provides techniques and best practices for optimizing gas consumption in NEO smart contracts.

## Overview

Gas optimization is crucial for creating efficient and cost-effective smart contracts on NEO. This guide covers various optimization strategies from basic patterns to advanced techniques.

## Understanding Gas Costs

### Gas Consumption Factors

1. **OpCode Complexity**: Different operations have different gas costs
2. **Storage Operations**: Reading/writing to storage is expensive
3. **Contract Size**: Larger contracts cost more to deploy
4. **Method Complexity**: Complex algorithms consume more gas

### Measuring Gas Usage

```csharp
[TestMethod]
public void TestGasConsumption()
{
    var initialGas = Engine.GasConsumed;
    
    // Execute contract method
    Contract.MyMethod();
    
    var gasUsed = Engine.GasConsumed - initialGas;
    Console.WriteLine($"Gas consumed: {gasUsed}");
}
```

## Storage Optimization

### Efficient Key Design

```csharp
// Good: Short, efficient keys
private const byte Prefix_Balance = 0x01;
private const byte Prefix_Owner = 0x02;

// Bad: Long string keys
private const string KEY_USER_BALANCE = "user_balance_";
```

### Storage Patterns

```csharp
// Pack related data together
public struct UserData
{
    public BigInteger Balance;
    public uint LastUpdate;
    public bool IsActive;
}

// Store as single serialized object
Storage.Put(Storage.CurrentContext, userKey, StdLib.Serialize(userData));
```

### Lazy Initialization

```csharp
// Initialize storage only when needed
public static BigInteger GetBalance(UInt160 account)
{
    var key = Prefix_Balance.ToByteArray().Concat(account);
    var balance = Storage.Get(Storage.CurrentContext, key);
    return balance != null ? (BigInteger)balance : 0;
}
```

## Computation Optimization

### Avoid Expensive Operations

```csharp
// Good: Simple arithmetic
var result = value * 100 / rate;

// Bad: Complex floating point simulation
var result = (value * 10000) / (rate * 100);
```

### Cache Frequently Used Values

```csharp
private static UInt160 _owner;

public static UInt160 GetOwner()
{
    if (_owner == null)
    {
        _owner = (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
    }
    return _owner;
}
```

### Batch Operations

```csharp
// Good: Batch multiple operations
public static void BatchTransfer(UInt160[] recipients, BigInteger[] amounts)
{
    for (int i = 0; i < recipients.Length; i++)
    {
        Transfer(Runtime.ExecutingScriptHash, recipients[i], amounts[i], null);
    }
}

// Bad: Individual transaction for each transfer
```

## Method Optimization

### Use [Safe] Attribute

```csharp
// Mark read-only methods as Safe to reduce gas
[Safe]
public static BigInteger BalanceOf(UInt160 account)
{
    return GetBalance(account);
}
```

### Minimize External Calls

```csharp
// Good: Single call with multiple parameters
var result = ExternalContract.ProcessBatch(data1, data2, data3);

// Bad: Multiple external calls
var result1 = ExternalContract.Process(data1);
var result2 = ExternalContract.Process(data2);
var result3 = ExternalContract.Process(data3);
```

### Early Returns

```csharp
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // Early validation to save gas on failures
    if (!from.IsValid || !to.IsValid)
        return false;
    
    if (amount <= 0)
        return false;
    
    if (GetBalance(from) < amount)
        return false;
    
    // Expensive operations only after validation
    return ExecuteTransfer(from, to, amount);
}
```

## Data Structure Optimization

### Use Appropriate Types

```csharp
// Good: Use smallest sufficient type
public byte Status { get; set; }  // For values 0-255

// Bad: Oversized type
public BigInteger Status { get; set; }  // For values 0-255
```

### Bit Packing

```csharp
// Pack multiple boolean flags into single byte
public static class Flags
{
    public const byte Active = 0x01;
    public const byte Verified = 0x02;
    public const byte Premium = 0x04;
    public const byte Locked = 0x08;
}

// Check flags efficiently
public static bool IsActive(byte flags)
{
    return (flags & Flags.Active) != 0;
}
```

## Event Optimization

### Minimize Event Data

```csharp
// Good: Essential data only
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

// Bad: Excessive event data
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger, string, uint, byte[]> OnTransfer;
```

### Conditional Events

```csharp
// Only emit events when necessary
public static void Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // Perform transfer logic
    ExecuteTransfer(from, to, amount);
    
    // Emit event only for significant amounts
    if (amount >= MinEventThreshold)
    {
        OnTransfer(from, to, amount);
    }
}
```

## Advanced Optimization Techniques

### Loop Optimization

```csharp
// Good: Minimize loop overhead
var length = array.Length;
for (int i = 0; i < length; i++)
{
    ProcessItem(array[i]);
}

// Bad: Repeated property access
for (int i = 0; i < array.Length; i++)
{
    ProcessItem(array[i]);
}
```

### Precomputed Values

```csharp
// Precompute common calculations
private static readonly BigInteger[] PowersOfTen = {
    1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000
};

public static BigInteger GetDecimals(int decimals)
{
    return decimals < PowersOfTen.Length ? PowersOfTen[decimals] : BigInteger.Pow(10, decimals);
}
```

### String Optimization

```csharp
// Good: Use ByteString for fixed strings
private static readonly ByteString TokenSymbol = "MYT";

// Bad: String concatenation in methods
public static string GetTokenInfo()
{
    return "Token: " + GetSymbol();
}
```

## Testing Gas Optimization

### Gas Benchmarking

```csharp
[TestMethod]
public void BenchmarkOperations()
{
    // Test different approaches
    var gas1 = TestApproach1();
    var gas2 = TestApproach2();
    
    Console.WriteLine($"Approach 1: {gas1} gas");
    Console.WriteLine($"Approach 2: {gas2} gas");
    
    Assert.IsTrue(gas2 < gas1, "Optimization should reduce gas consumption");
}
```

### Regression Testing

```csharp
[TestMethod]
public void GasRegressionTest()
{
    var gasUsed = MeasureGasUsage(() => Contract.ComplexOperation());
    
    // Ensure gas usage doesn't exceed baseline
    Assert.IsTrue(gasUsed <= ExpectedGasLimit, 
        $"Gas usage {gasUsed} exceeds limit {ExpectedGasLimit}");
}
```

## Best Practices Checklist

### Storage
- [ ] Use short, efficient storage keys
- [ ] Pack related data together
- [ ] Implement lazy initialization
- [ ] Minimize storage reads/writes

### Computation
- [ ] Avoid expensive operations in loops
- [ ] Cache frequently used values
- [ ] Use early returns for validation
- [ ] Batch similar operations

### Methods
- [ ] Mark read-only methods as [Safe]
- [ ] Minimize external contract calls
- [ ] Optimize parameter validation
- [ ] Use appropriate data types

### Testing
- [ ] Measure gas consumption in tests
- [ ] Compare optimization alternatives
- [ ] Set gas consumption limits
- [ ] Monitor for regressions

## Common Anti-Patterns

### Avoid These Patterns

1. **Excessive Storage Access**
   ```csharp
   // Bad: Multiple storage reads
   var name = Storage.Get(key + "_name");
   var age = Storage.Get(key + "_age");
   var email = Storage.Get(key + "_email");
   ```

2. **String Concatenation in Loops**
   ```csharp
   // Bad: String building in loop
   string result = "";
   for (int i = 0; i < items.Length; i++)
   {
       result += items[i].ToString();
   }
   ```

3. **Unnecessary External Calls**
   ```csharp
   // Bad: Repeated contract calls
   for (int i = 0; i < users.Length; i++)
   {
       ExternalContract.Process(users[i]);
   }
   ```

## Tools and Resources

### Gas Analysis Tools
- NEO Smart Contract Testing Framework
- Gas consumption unit tests
- Performance profilers

### Reference Materials
- [NEO OpCode Gas Costs](https://docs.neo.org/docs/n3/reference/opcodes)
- [Storage Best Practices](storage-security.md)
- [Performance Testing Guide](../examples/03-advanced/README.md)

Remember: Always measure actual gas consumption rather than assuming optimizations work. Profile before and after optimization to ensure improvements.