# Advanced Contract Invocation: Development Contracts & Non-Standard Methods

This document covers advanced scenarios for the Neo N3 contract invocation system, specifically addressing:
1. Calling methods on contracts under development
2. Handling non-standard contract methods

## 1. Development Contract Invocation

### Problem: Calling Methods on Developing Contracts

When developing multiple contracts simultaneously, you often need to invoke methods on contracts that:
- Don't have deployed addresses yet
- Are still under development and may change
- Need compilation order dependencies
- Require method validation at compile time

### Solution: Development Contract Proxies

#### Basic Development Contract Proxy

```csharp
// The contract under development
public class MyTokenContract : SmartContract
{
    public static string Symbol() => "MTK";
    public static byte Decimals() => 8;
    public static BigInteger TotalSupply() => 1000000;
    public static BigInteger BalanceOf(UInt160 account) => /* implementation */;
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data) => /* implementation */;
}

// Development contract proxy
public class MyTokenContractProxy : DevelopmentContractProxy
{
    protected override Type SourceContractType => typeof(MyTokenContract);

    public MyTokenContractProxy(DevelopmentContractReference contractReference) 
        : base(contractReference) { }

    // Type-safe method calls with compile-time validation
    public string Symbol() => (string)InvokeReadOnly(nameof(MyTokenContract.Symbol));
    public byte Decimals() => (byte)InvokeReadOnly(nameof(MyTokenContract.Decimals));
    public BigInteger TotalSupply() => (BigInteger)InvokeReadOnly(nameof(MyTokenContract.TotalSupply));
    public BigInteger BalanceOf(UInt160 account) => (BigInteger)InvokeReadOnly(nameof(MyTokenContract.BalanceOf), account);
    public bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data) => 
        (bool)InvokeWithStates(nameof(MyTokenContract.Transfer), from, to, amount, data);
}
```

#### Usage in Calling Contract

```csharp
public class MarketplaceContract : SmartContract
{
    // Reference to development contract
    [ContractReference("MyTokenContract", 
        ReferenceType = ContractReferenceType.Development,
        ProjectPath = "../MyTokenContract/MyTokenContract.csproj")]
    private static readonly DevelopmentContractReference TokenContract;

    public static bool PurchaseItem(UInt160 buyer, BigInteger itemPrice)
    {
        // Use development contract proxy
        var tokenProxy = new MyTokenContractProxy(TokenContract);
        
        // Compile-time method validation - will fail if method doesn't exist
        var buyerBalance = tokenProxy.BalanceOf(buyer);
        
        if (buyerBalance < itemPrice)
            return false;

        // This call will be resolved at compilation time
        return tokenProxy.Transfer(buyer, Runtime.ExecutingScriptHash, itemPrice, null);
    }
}
```

### Development-Time Behaviors

#### Option 1: Throw Exception (Default)
```csharp
public class StrictDevelopmentProxy : DevelopmentContractProxy
{
    protected override object HandleDevelopmentContractInvocation(MethodResolutionInfo resolution)
    {
        throw new InvalidOperationException(
            $"Contract '{ContractReference.Identifier}' must be deployed before calling '{resolution.OriginalMethodName}'");
    }
}
```

#### Option 2: Return Mock Values for Testing
```csharp
public class MockingDevelopmentProxy : DevelopmentContractProxy
{
    protected override object HandleDevelopmentContractInvocation(MethodResolutionInfo resolution)
    {
        // Return mock values for unit testing
        return resolution.OriginalMethodName switch
        {
            "balanceOf" => BigInteger.Parse("1000000"), // Mock balance
            "symbol" => "MOCK",
            "decimals" => (byte)8,
            "totalSupply" => BigInteger.Parse("1000000000"),
            "transfer" => true, // Mock successful transfer
            _ => GetDefaultReturnValue(resolution.SourceMethod?.ReturnType ?? typeof(object))
        };
    }
}
```

#### Option 3: Direct Source Method Invocation (for Unit Tests)
```csharp
public class DirectInvocationProxy : DevelopmentContractProxy
{
    protected override object HandleDevelopmentContractInvocation(MethodResolutionInfo resolution)
    {
        // Directly invoke the source method for unit testing
        if (resolution.SourceMethod != null)
        {
            try
            {
                return resolution.SourceMethod.Invoke(null, resolution.ResolvedParameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to invoke source method '{resolution.SourceMethod.Name}': {ex.Message}", ex);
            }
        }

        return GetDefaultReturnValue(typeof(object));
    }
}
```

## 2. Non-Standard Method Handling

### Problem: Custom Contract Methods

Some contracts have non-standard methods that:
- Use custom parameter formats
- Require parameter transformation
- Have non-standard naming conventions
- Need special call flags

### Solution: Custom Method Attributes

#### Basic Custom Method

```csharp
public class CustomContractProxy : ContractProxyBase
{
    public CustomContractProxy(IContractReference contractReference) : base(contractReference) { }

    // Map C# method to different contract method name
    [CustomMethod("get_user_data")]
    public UserData GetUserData(UInt160 userHash)
    {
        return (UserData)InvokeReadOnly("get_user_data", userHash);
    }

    // Method with parameter validation
    [CustomMethod("batch_transfer", MinParameters = 2, MaxParameters = 100)]
    public bool BatchTransfer(TransferRequest[] transfers)
    {
        return (bool)InvokeWithStates("batch_transfer", transfers);
    }
}
```

#### Advanced Custom Method with Transformation

```csharp
public class AdvancedContractProxy : ContractProxyBase
{
    public AdvancedContractProxy(IContractReference contractReference) : base(contractReference) { }

    // Method that serializes complex parameters
    [CustomMethod("execute_complex_operation", 
        ParameterTransform = ParameterTransformStrategy.SerializeToByteArray,
        CallFlags = CallFlags.States | CallFlags.AllowCall)]
    public object ExecuteComplexOperation(ComplexRequest request, Dictionary<string, object> options)
    {
        // Parameters will be automatically serialized to byte array before Contract.Call
        return InvokeMethod("execute_complex_operation", CallFlags.States | CallFlags.AllowCall, request, options);
    }

    // Method that flattens array parameters
    [CustomMethod("multi_mint", 
        ParameterTransform = ParameterTransformStrategy.FlattenArrays)]
    public bool MultiMint(UInt160[] recipients, BigInteger[] amounts)
    {
        // Arrays will be flattened: MultiMint([addr1, addr2], [100, 200]) 
        // becomes Contract.Call("multi_mint", addr1, addr2, 100, 200)
        return (bool)InvokeWithStates("multi_mint", recipients, amounts);
    }
}
```

#### Flexible Method Resolution

```csharp
public class FlexibleProxy : ContractProxyBase
{
    public FlexibleProxy(IContractReference contractReference) : base(contractReference) { }

    // Dynamic method invocation with automatic resolution
    public T InvokeCustomMethod<T>(string methodName, params object[] parameters)
    {
        var resolution = MethodResolver.ResolveMethod(ContractReference, methodName, parameters);
        
        if (!resolution.IsResolved)
        {
            throw new InvalidOperationException($"Cannot resolve method '{methodName}': {resolution.ErrorMessage}");
        }

        var result = Contract.Call(
            ContractReference.ResolvedHash!, 
            resolution.ResolvedMethodName, 
            resolution.CallFlags, 
            resolution.ResolvedParameters);

        return (T)result;
    }

    // Example usage of dynamic invocation
    public BigInteger GetCustomBalance(UInt160 account, string tokenType)
    {
        return InvokeCustomMethod<BigInteger>("getBalanceForType", account, tokenType);
    }
}
```

## 3. Real-World Examples

### Game Contract with Development Dependencies

```csharp
// Game items contract under development
public class GameItemsContract : SmartContract
{
    public static bool TransferItem(UInt160 from, UInt160 to, BigInteger itemId) => true;
    public static UInt160 GetItemOwner(BigInteger itemId) => UInt160.Zero;
    public static GameItem GetItemData(BigInteger itemId) => new GameItem();
}

// Game marketplace contract
public class GameMarketplaceContract : SmartContract
{
    [ContractReference("GameItemsContract", 
        ReferenceType = ContractReferenceType.Development,
        ProjectPath = "../GameItems/GameItemsContract.csproj")]
    private static readonly DevelopmentContractReference ItemsContract;

    public static bool SellItem(UInt160 seller, BigInteger itemId, BigInteger price)
    {
        var itemsProxy = new GameItemsProxy(ItemsContract);
        
        // Verify ownership - will validate method exists at compile time
        var owner = itemsProxy.GetItemOwner(itemId);
        if (owner != seller)
            return false;

        // Create listing logic here...
        return true;
    }
}

public class GameItemsProxy : DevelopmentContractProxy
{
    protected override Type SourceContractType => typeof(GameItemsContract);

    public GameItemsProxy(DevelopmentContractReference contractReference) : base(contractReference) { }

    public UInt160 GetItemOwner(BigInteger itemId) => 
        (UInt160)InvokeReadOnly(nameof(GameItemsContract.GetItemOwner), itemId);

    public GameItem GetItemData(BigInteger itemId) => 
        (GameItem)InvokeReadOnly(nameof(GameItemsContract.GetItemData), itemId);

    public bool TransferItem(UInt160 from, UInt160 to, BigInteger itemId) => 
        (bool)InvokeWithStates(nameof(GameItemsContract.TransferItem), from, to, itemId);
}
```

### DeFi Protocol with Custom Methods

```csharp
public class DeFiProtocolProxy : ContractProxyBase
{
    public DeFiProtocolProxy(IContractReference contractReference) : base(contractReference) { }

    // Standard methods
    [ContractMethod(ReadOnly = true)]
    public BigInteger GetLiquidityPool(UInt160 token1, UInt160 token2)
    {
        return (BigInteger)InvokeReadOnly("getLiquidityPool", token1, token2);
    }

    // Custom method with complex parameters
    [CustomMethod("execute_arbitrage", 
        ParameterTransform = ParameterTransformStrategy.WrapInArray,
        MinParameters = 3)]
    public ArbitrageResult ExecuteArbitrage(
        ArbitrageRequest request, 
        SwapPath[] paths, 
        SlippageSettings slippage)
    {
        // All parameters wrapped in single array
        return (ArbitrageResult)InvokeWithStates("execute_arbitrage", request, paths, slippage);
    }

    // Method with custom parameter format
    [CustomMethod("bulk_operations", 
        CustomParameterFormat = "compressed_batch",
        ParameterTransform = ParameterTransformStrategy.Custom)]
    public bool BulkOperations(Operation[] operations)
    {
        // Custom transformation applied based on CustomParameterFormat
        return (bool)InvokeWithStates("bulk_operations", operations);
    }
}
```

## 4. Best Practices

### For Development Contracts

1. **Use Compile-Time Validation**: Always inherit from `DevelopmentContractProxy` to get method validation
2. **Handle Development State**: Implement appropriate `HandleDevelopmentContractInvocation` behavior
3. **Use Nameof**: Use `nameof()` for method names to catch refactoring changes
4. **Mock for Testing**: Provide mock implementations for unit testing scenarios

### For Non-Standard Methods

1. **Document Custom Methods**: Always document why custom attributes are needed
2. **Validate Parameters**: Use `MinParameters`/`MaxParameters` for parameter validation
3. **Choose Appropriate Transformations**: Select the right `ParameterTransformStrategy`
4. **Test Thoroughly**: Test custom method mappings with real contract calls

### For Both Scenarios

1. **Error Handling**: Provide clear error messages for resolution failures
2. **Performance**: Cache method resolutions for repeated calls
3. **Versioning**: Consider contract version compatibility
4. **Documentation**: Document all custom behaviors and transformations

This advanced contract invocation system provides robust solutions for complex multi-contract development scenarios while maintaining type safety and developer productivity.