# NEO Smart Contract Debugging Guide

This guide provides comprehensive debugging techniques and tools for NEO smart contract development.

## Table of Contents

1. [Debugging Overview](#debugging-overview)
2. [Development-Time Debugging](#development-time-debugging)
3. [Testing and Debugging](#testing-and-debugging)
4. [Common Error Messages](#common-error-messages)
5. [Debugging Tools](#debugging-tools)
6. [Best Practices](#best-practices)
7. [Troubleshooting Checklist](#troubleshooting-checklist)

## Debugging Overview

NEO smart contracts run in a deterministic environment, which means traditional debugging approaches need adaptation. This guide covers techniques for each stage of development.

### Debugging Stages

- **Compile-time** - Syntax and type errors
- **Test-time** - Logic and behavior errors  
- **Deployment-time** - Configuration errors
- **Runtime** - Production monitoring

## Development-Time Debugging

### Using Runtime.Log for Debugging

```csharp
public class DebugContract : SmartContract
{
    public static bool ProcessData(string input)
    {
        // Log entry point
        Runtime.Log("ProcessData called");
        Runtime.Log($"Input: {input}");
        
        if (string.IsNullOrEmpty(input))
        {
            Runtime.Log("ERROR: Empty input");
            return false;
        }
        
        // Log intermediate values
        var processed = input.ToUpper();
        Runtime.Log($"Processed: {processed}");
        
        // Log success
        Runtime.Log("ProcessData completed successfully");
        return true;
    }
}
```

**Note**: Remove all `Runtime.Log` calls before mainnet deployment as they consume GAS.

### Debugging Compilation Errors

Common compilation issues and solutions:

```csharp
// ❌ Float not supported
public static double CalculateInterest(double principal) { }

// ✅ Use BigInteger for decimals
public static BigInteger CalculateInterest(BigInteger principal)
{
    // Multiply by 100 for 2 decimal precision
    return principal * 105 / 100; // 5% interest
}

// ❌ LINQ not fully supported
var filtered = items.Where(x => x > 10).ToList();

// ✅ Use manual iteration
List<int> filtered = new();
foreach (var item in items)
{
    if (item > 10) filtered.Add(item);
}

// ❌ Try-catch with specific exceptions
try { } catch (ArgumentException ex) { }

// ✅ Use generic exception
try { } catch (Exception) { }
```

### IDE Debugging Setup

#### Visual Studio Code

1. **Install Required Extensions**:
   - C# Extension
   - NEO Blockchain Toolkit

2. **Configure launch.json**:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Debug NEO Contract",
            "type": "neo-contract",
            "request": "launch",
            "program": "${workspaceFolder}/bin/sc/Contract.nef",
            "checkpoint": "${workspaceFolder}/checkpoints/checkpoint.neo",
            "invocation": {
                "operation": "methodName",
                "args": ["arg1", 123]
            },
            "storage": [
                {
                    "key": "0x01",
                    "value": "0x1234"
                }
            ]
        }
    ]
}
```

3. **Set Breakpoints**:
   - Click in the gutter next to line numbers
   - Use conditional breakpoints for specific conditions
   - Watch variables in the debug panel

## Testing and Debugging

### Unit Test Debugging

```csharp
[TestClass]
public class ContractDebugTests : TestBase<MyContract>
{
    [TestMethod]
    public void Debug_ComplexOperation()
    {
        // Enable verbose output
        Engine.DebugMode = true;
        
        // Log initial state
        Console.WriteLine($"Initial balance: {Contract.BalanceOf(TestAddress)}");
        
        // Execute with logging
        try
        {
            var result = Contract.ComplexOperation(TestAddress, 100);
            Console.WriteLine($"Result: {result}");
            
            // Log final state
            Console.WriteLine($"Final balance: {Contract.BalanceOf(TestAddress)}");
            
            // Check events
            var events = Engine.Notifications;
            foreach (var evt in events)
            {
                Console.WriteLine($"Event: {evt.EventName}");
                Console.WriteLine($"Data: {evt.State}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }
    
    [TestMethod]
    public void Debug_GasConsumption()
    {
        // Track gas usage
        var initialGas = Engine.GasConsumed;
        
        Contract.ExpensiveOperation();
        
        var gasUsed = Engine.GasConsumed - initialGas;
        Console.WriteLine($"Gas consumed: {gasUsed}");
        
        // Assert reasonable gas usage
        Assert.IsTrue(gasUsed < 10_000_000, $"Too much gas: {gasUsed}");
    }
}
```

### Storage Debugging

```csharp
public class StorageDebugger : SmartContract
{
    // Debug helper to dump all storage
    [Safe]
    public static Map<string, object> DumpStorage(string prefix)
    {
        var result = new Map<string, object>();
        var context = Storage.CurrentContext;
        
        // Use storage find to iterate
        var iterator = Storage.Find(context, prefix, FindOptions.None);
        
        while (iterator.Next())
        {
            var key = (ByteString)iterator.Value[0];
            var value = iterator.Value[1];
            result[key] = value;
        }
        
        return result;
    }
    
    // Debug helper to check specific key
    [Safe]
    public static object DebugGetStorage(ByteString key)
    {
        var value = Storage.Get(Storage.CurrentContext, key);
        Runtime.Log($"Storage[{key}] = {value}");
        return value;
    }
}
```

## Common Error Messages

### Compilation Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `Unsupported opcode` | Using unsupported C# feature | Check supported features list |
| `Method not found` | Missing using statement | Add appropriate using directive |
| `Cannot convert type` | Type mismatch | Use explicit casting |
| `Stack overflow` | Recursive call too deep | Reduce recursion depth |

### Runtime Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `ASSERT` | Failed assertion | Check assertion conditions |
| `Insufficient GAS` | Operation too expensive | Optimize code or increase GAS |
| `Invalid operation` | Unauthorized action | Check witness verification |
| `Key not found` | Missing storage entry | Add null checks |

### Example Error Diagnosis

```csharp
// Common error: ASSERT
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // Debug: Log parameters
    Runtime.Log($"Transfer: {from} -> {to}, amount: {amount}");
    
    // This might fail with ASSERT
    Assert(Runtime.CheckWitness(from), "Witness check failed");
    
    // Debug: Witness passed
    Runtime.Log("Witness verified");
    
    var balance = GetBalance(from);
    Runtime.Log($"Balance: {balance}");
    
    // This might fail with ASSERT
    Assert(balance >= amount, $"Insufficient balance: {balance} < {amount}");
    
    // Continue with transfer...
}
```

## Debugging Tools

### Neo Express Debugging

```bash
# Create checkpoint before testing
neoxp checkpoint create before-test

# Run contract and see logs
neoxp contract invoke MyContract myMethod --account alice --verbose

# Restore if something goes wrong
neoxp checkpoint restore before-test

# View transaction details
neoxp transaction show <txid>

# Inspect storage
neoxp contract storage MyContract
```

### Neo CLI Debugging

```bash
# Enable debug logging
neo-cli --debug

# In Neo CLI console:
# View contract state
neo> getcontractstate <contract-hash>

# View storage
neo> getstorage <contract-hash> <key>

# Test invoke (doesn't consume GAS)
neo> invokefunction <contract-hash> <method> <params>

# Get transaction details
neo> gettransaction <txid>

# View application log (events)
neo> getapplicationlog <txid>
```

### Custom Test Helpers

```csharp
public static class DebugHelpers
{
    public static void PrintContractState(TestEngine engine, UInt160 contract)
    {
        Console.WriteLine($"=== Contract State: {contract} ===");
        Console.WriteLine($"GAS Consumed: {engine.GasConsumed}");
        Console.WriteLine($"Notifications: {engine.Notifications.Count}");
        
        foreach (var notification in engine.Notifications)
        {
            Console.WriteLine($"  Event: {notification.EventName}");
            Console.WriteLine($"  Data: {notification.State}");
        }
    }
    
    public static void AssertNoErrors(TestEngine engine)
    {
        Assert.IsFalse(engine.FaultException != null, 
            $"Engine fault: {engine.FaultException?.Message}");
    }
    
    public static T InvokeWithDebug<T>(TestEngine engine, 
        Expression<Func<T>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodName = methodCall.Method.Name;
        
        Console.WriteLine($"Invoking: {methodName}");
        var gasStart = engine.GasConsumed;
        
        try
        {
            var result = expression.Compile()();
            var gasUsed = engine.GasConsumed - gasStart;
            Console.WriteLine($"Success: {methodName} used {gasUsed} GAS");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {methodName}");
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
```

## Best Practices

### 1. Defensive Programming

```csharp
public static bool SafeOperation(UInt160 user, BigInteger amount)
{
    // Validate all inputs first
    if (user == null || !user.IsValid)
    {
        Runtime.Log("Invalid user address");
        return false;
    }
    
    if (amount <= 0)
    {
        Runtime.Log("Invalid amount");
        return false;
    }
    
    // Check state before modification
    var currentBalance = GetBalance(user);
    Runtime.Log($"Current balance: {currentBalance}");
    
    if (currentBalance < amount)
    {
        Runtime.Log("Insufficient balance");
        return false;
    }
    
    // Perform operation
    return ExecuteOperation(user, amount);
}
```

### 2. Event-Based Debugging

```csharp
public class EventDebugContract : SmartContract
{
    // Debug events (remove for production)
    [DisplayName("Debug")]
    public static event Action<string, object> OnDebug;
    
    public static bool ComplexOperation(UInt160 user)
    {
        OnDebug("Start", user);
        
        var state = LoadUserState(user);
        OnDebug("StateLoaded", state);
        
        var result = ProcessState(state);
        OnDebug("ProcessComplete", result);
        
        return SaveResult(user, result);
    }
}
```

### 3. Test Data Generation

```csharp
public static class TestDataGenerator
{
    public static UInt160 GenerateAddress(int seed)
    {
        var bytes = new byte[20];
        for (int i = 0; i < 20; i++)
        {
            bytes[i] = (byte)((seed + i) % 256);
        }
        return new UInt160(bytes);
    }
    
    public static List<(UInt160, BigInteger)> GenerateBalances(int count)
    {
        var result = new List<(UInt160, BigInteger)>();
        for (int i = 0; i < count; i++)
        {
            var address = GenerateAddress(i);
            var balance = new BigInteger(1000 * (i + 1));
            result.Add((address, balance));
        }
        return result;
    }
}
```

## Troubleshooting Checklist

### Contract Won't Compile

- [ ] Check for unsupported C# features
- [ ] Verify all using statements
- [ ] Ensure no floating-point operations
- [ ] Check LINQ usage limitations
- [ ] Verify attribute usage

### Tests Failing

- [ ] Check witness requirements
- [ ] Verify initial state setup
- [ ] Confirm parameter types match
- [ ] Check for gas limitations
- [ ] Verify event expectations

### Deployment Issues

- [ ] Sufficient GAS in wallet
- [ ] Correct contract path
- [ ] Valid manifest JSON
- [ ] Network connectivity
- [ ] Correct network selection

### Runtime Errors

- [ ] Check all assertions
- [ ] Verify witness checks
- [ ] Confirm storage keys exist
- [ ] Check arithmetic operations
- [ ] Verify external contract calls

### Performance Problems

- [ ] Profile gas consumption
- [ ] Optimize storage access
- [ ] Reduce external calls
- [ ] Batch operations
- [ ] Cache frequently used data

## Advanced Debugging Techniques

### Memory Dumps

```csharp
[TestMethod]
public void Debug_MemoryUsage()
{
    var snapshot = Engine.Snapshot;
    
    // Take memory snapshot
    var before = GC.GetTotalMemory(false);
    
    // Execute operation
    Contract.MemoryIntensiveOperation();
    
    // Check memory growth
    var after = GC.GetTotalMemory(false);
    var growth = after - before;
    
    Console.WriteLine($"Memory growth: {growth} bytes");
}
```

### Call Stack Tracing

```csharp
public static bool TraceableOperation()
{
    var stack = new Stack<string>();
    stack.Push("TraceableOperation:Start");
    
    try
    {
        stack.Push("ValidationPhase");
        ValidateInputs();
        stack.Pop();
        
        stack.Push("ProcessingPhase");
        ProcessData();
        stack.Pop();
        
        return true;
    }
    catch (Exception)
    {
        // Log call stack on error
        while (stack.Count > 0)
        {
            Runtime.Log($"Stack: {stack.Pop()}");
        }
        throw;
    }
}
```

Remember: Always remove debug code and logs before deploying to mainnet to save GAS and improve security.