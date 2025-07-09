# Neo Smart Contract Development Best Practices

This guide provides best practices for developing secure, efficient, and maintainable smart contracts on the Neo blockchain.

## Table of Contents
1. [Code Organization](#code-organization)
2. [Security Best Practices](#security-best-practices)
3. [Gas Optimization](#gas-optimization)
4. [Storage Management](#storage-management)
5. [Error Handling](#error-handling)
6. [Testing Strategies](#testing-strategies)
7. [Documentation](#documentation)
8. [Deployment Practices](#deployment-practices)

## Code Organization

### Contract Structure

```csharp
[DisplayName("MyContract")]
[ManifestExtra("Author", "Your Name")]
[ManifestExtra("Email", "contact@example.com")]
[ManifestExtra("Description", "Contract description")]
[ContractSourceCode("https://github.com/yourrepo")]
public class MyContract : SmartContract
{
    // 1. Constants and configuration
    private const byte Prefix_Config = 0x01;
    private const byte Prefix_Data = 0x02;
    
    // 2. Contract owner/admin
    private static readonly UInt160 Owner = "NXX...".ToScriptHash();
    
    // 3. Helper/validation methods
    private static void RequireOwner() { ... }
    
    // 4. Deploy method
    public static void Deploy(object data, bool update) { ... }
    
    // 5. Public methods (state-changing)
    public static void Store(string key, object value) { ... }
    
    // 6. Public methods (read-only)
    [Safe]
    public static object Get(string key) { ... }
    
    // 7. Events
    public static event Action<string, object> OnStored;
    
    // 8. Contract management
    public static void Update(ByteString nef, string manifest) { ... }
}
```

### Method Organization

1. **Group related methods together**
2. **Place read-only methods after state-changing methods**
3. **Keep helper methods private**
4. **Use descriptive method names**

## Security Best Practices

### 1. Access Control

```csharp
// Always implement proper access control
private static void RequireOwner()
{
    if (!Runtime.CheckWitness(Owner))
        throw new Exception("Unauthorized");
}

// For multi-admin scenarios
private static readonly UInt160[] Admins = new[] 
{
    "NAdmin1...".ToScriptHash(),
    "NAdmin2...".ToScriptHash()
};

private static void RequireAdmin()
{
    foreach (var admin in Admins)
    {
        if (Runtime.CheckWitness(admin))
            return;
    }
    throw new Exception("Unauthorized");
}
```

### 2. Input Validation

```csharp
public static void Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // Validate addresses
    if (!from.IsValid || !to.IsValid)
        throw new Exception("Invalid address");
    
    // Validate amount
    if (amount <= 0)
        throw new Exception("Amount must be positive");
    
    // Check witness
    if (!Runtime.CheckWitness(from))
        throw new Exception("Unauthorized");
    
    // Proceed with transfer...
}
```

### 3. Reentrancy Protection

```csharp
// Use state changes before external calls
public static void Withdraw(UInt160 account, BigInteger amount)
{
    var balance = GetBalance(account);
    if (balance < amount)
        throw new Exception("Insufficient balance");
    
    // Update state BEFORE external call
    SetBalance(account, balance - amount);
    
    // Then make external call
    GAS.Transfer(Runtime.ExecutingScriptHash, account, amount);
}
```

### 4. Integer Overflow Protection

```csharp
// Check for overflows in arithmetic operations
public static void AddBalance(UInt160 account, BigInteger amount)
{
    var currentBalance = GetBalance(account);
    var newBalance = currentBalance + amount;
    
    // Check for overflow
    if (newBalance < currentBalance)
        throw new Exception("Integer overflow");
    
    SetBalance(account, newBalance);
}
```

## Gas Optimization

### 1. Minimize Storage Operations

```csharp
// Bad: Multiple storage reads
public static void BadTransfer(UInt160 from, UInt160 to, BigInteger amount)
{
    if (GetBalance(from) < amount) throw new Exception();
    SetBalance(from, GetBalance(from) - amount);  // Reads balance again!
    SetBalance(to, GetBalance(to) + amount);
}

// Good: Single storage read
public static void GoodTransfer(UInt160 from, UInt160 to, BigInteger amount)
{
    var fromBalance = GetBalance(from);
    if (fromBalance < amount) throw new Exception();
    
    var toBalance = GetBalance(to);
    SetBalance(from, fromBalance - amount);
    SetBalance(to, toBalance + amount);
}
```

### 2. Use Appropriate Data Types

```csharp
// Use ByteString for binary data (more efficient)
public static ByteString GetData(ByteString key)
{
    return Storage.Get(Storage.CurrentContext, key);
}

// Use specific types instead of object
public static BigInteger GetCount() // Not object GetCount()
{
    return (BigInteger)Storage.Get(Storage.CurrentContext, "count");
}
```

### 3. Batch Operations

```csharp
// Provide batch methods to save gas
public static void BatchTransfer(UInt160 from, UInt160[] recipients, BigInteger[] amounts)
{
    if (recipients.Length != amounts.Length)
        throw new Exception("Array length mismatch");
    
    if (!Runtime.CheckWitness(from))
        throw new Exception("Unauthorized");
    
    var totalAmount = BigInteger.Zero;
    for (int i = 0; i < amounts.Length; i++)
    {
        totalAmount += amounts[i];
    }
    
    var balance = GetBalance(from);
    if (balance < totalAmount)
        throw new Exception("Insufficient balance");
    
    SetBalance(from, balance - totalAmount);
    
    for (int i = 0; i < recipients.Length; i++)
    {
        var recipientBalance = GetBalance(recipients[i]);
        SetBalance(recipients[i], recipientBalance + amounts[i]);
        OnTransfer(from, recipients[i], amounts[i]);
    }
}
```

## Storage Management

### 1. Use Prefixes for Organization

```csharp
// Define storage prefixes
private const byte Prefix_Config = 0x01;
private const byte Prefix_Balance = 0x02;
private const byte Prefix_Allowance = 0x03;
private const byte Prefix_Metadata = 0x04;

// Use prefixes consistently
private static byte[] GetBalanceKey(UInt160 account)
{
    return new byte[] { Prefix_Balance }.Concat(account);
}
```

### 2. Clean Up Unused Storage

```csharp
public static void DeleteData(string key)
{
    var storageKey = GetStorageKey(key);
    Storage.Delete(Storage.CurrentContext, storageKey);
    // This refunds GAS!
}

// Delete zero balances
private static void SetBalance(UInt160 account, BigInteger balance)
{
    var key = GetBalanceKey(account);
    if (balance <= 0)
        Storage.Delete(Storage.CurrentContext, key);
    else
        Storage.Put(Storage.CurrentContext, key, balance);
}
```

### 3. Use Serialization Wisely

```csharp
// For complex data structures
public static void StoreData(string id, Map<string, object> data)
{
    var key = new byte[] { Prefix_Data } + id.ToByteArray();
    Storage.Put(Storage.CurrentContext, key, StdLib.Serialize(data));
}

[Safe]
public static Map<string, object> GetData(string id)
{
    var key = new byte[] { Prefix_Data } + id.ToByteArray();
    var data = Storage.Get(Storage.CurrentContext, key);
    
    if (data == null || data.Length == 0)
        return new Map<string, object>();
    
    return (Map<string, object>)StdLib.Deserialize(data);
}
```

## Error Handling

### 1. Use Descriptive Error Messages

```csharp
// Good: Specific error messages
public static void Transfer(UInt160 from, UInt160 to, BigInteger amount)
{
    if (!from.IsValid)
        throw new Exception("Invalid sender address");
    
    if (!to.IsValid)
        throw new Exception("Invalid recipient address");
    
    if (amount <= 0)
        throw new Exception("Transfer amount must be positive");
    
    if (!Runtime.CheckWitness(from))
        throw new Exception("Sender signature verification failed");
    
    var balance = GetBalance(from);
    if (balance < amount)
        throw new Exception($"Insufficient balance. Required: {amount}, Available: {balance}");
}
```

### 2. Validate Early

```csharp
public static void ComplexOperation(UInt160 account, string data, BigInteger amount)
{
    // Validate all inputs first
    if (!account.IsValid)
        throw new Exception("Invalid account");
    
    if (string.IsNullOrEmpty(data) || data.Length > 1024)
        throw new Exception("Invalid data");
    
    if (amount <= 0 || amount > MaxAmount)
        throw new Exception("Invalid amount");
    
    // Then proceed with operation
    // ...
}
```

### 3. Handle Edge Cases

```csharp
[Safe]
public static BigInteger Divide(BigInteger a, BigInteger b)
{
    if (b == 0)
        throw new Exception("Division by zero");
    
    return a / b;
}

public static void ProcessArray(ByteString[] items)
{
    if (items == null || items.Length == 0)
        throw new Exception("Empty array not allowed");
    
    if (items.Length > 100)
        throw new Exception("Array too large (max 100 items)");
    
    // Process items...
}
```

## Testing Strategies

### 1. Unit Test Structure

```csharp
[Test]
public void Transfer_ValidInput_Success()
{
    // Arrange
    var from = _testAccount1;
    var to = _testAccount2;
    var amount = 100;
    SetBalance(from, 1000);
    
    // Act
    var result = Contract.Transfer(from, to, amount);
    
    // Assert
    Assert.IsTrue(result);
    Assert.AreEqual(900, GetBalance(from));
    Assert.AreEqual(100, GetBalance(to));
}

[Test]
public void Transfer_InsufficientBalance_ThrowsException()
{
    // Arrange
    var from = _testAccount1;
    var to = _testAccount2;
    var amount = 1000;
    SetBalance(from, 100);
    
    // Act & Assert
    var ex = Assert.Throws<Exception>(() => 
        Contract.Transfer(from, to, amount));
    Assert.Contains("Insufficient balance", ex.Message);
}
```

### 2. Test Edge Cases

```csharp
[Test]
public void Transfer_ZeroAmount_ThrowsException() { }

[Test]
public void Transfer_ToSameAddress_Success() { }

[Test]
public void Transfer_MaxAmount_Success() { }

[Test]
public void Transfer_InvalidAddress_ThrowsException() { }
```

### 3. Integration Testing

```csharp
[Test]
public async Task FullScenario_TokenLifecycle()
{
    // Deploy
    var deployment = await _toolkit.DeployAsync("Token.csproj");
    
    // Mint
    await _toolkit.InvokeAsync(deployment.ContractHash, "mint", _owner, 1000000);
    
    // Transfer
    await _toolkit.InvokeAsync(deployment.ContractHash, "transfer", _owner, _user1, 100000);
    
    // Verify
    var balance = await _toolkit.CallAsync<BigInteger>(
        deployment.ContractHash, "balanceOf", _user1);
    Assert.AreEqual(100000, balance);
}
```

## Documentation

### 1. Contract Documentation

```csharp
/// <summary>
/// NEP-17 compliant token contract with additional features
/// </summary>
[DisplayName("AdvancedToken")]
[ManifestExtra("Author", "Neo Team")]
[ManifestExtra("Email", "contact@neo.org")]
[ManifestExtra("Description", "Advanced NEP-17 token with minting and burning")]
[ManifestExtra("Version", "1.0.0")]
[ContractSourceCode("https://github.com/neo-project/token")]
[SupportedStandards("NEP-17")]
public class TokenContract : SmartContract
{
    /// <summary>
    /// Transfers tokens from one account to another
    /// </summary>
    /// <param name="from">Source account</param>
    /// <param name="to">Destination account</param>
    /// <param name="amount">Amount to transfer</param>
    /// <param name="data">Additional data for notification</param>
    /// <returns>True if successful</returns>
    [DisplayName("transfer")]
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
    {
        // Implementation
    }
}
```

### 2. Inline Comments

```csharp
public static void ComplexAlgorithm(BigInteger input)
{
    // Step 1: Validate input range (must be between 1 and 1000000)
    if (input <= 0 || input > 1000000)
        throw new Exception("Input out of range");
    
    // Step 2: Calculate fee based on input size
    // Fee = base_fee + (input / 1000) * rate
    var baseFee = 100;
    var rate = 10;
    var fee = baseFee + (input / 1000) * rate;
    
    // Step 3: Process with calculated fee
    // ...
}
```

## Deployment Practices

### 1. Pre-deployment Checklist

- [ ] All tests pass
- [ ] Security audit completed
- [ ] Gas consumption optimized
- [ ] Contract size within limits
- [ ] Initialization parameters defined
- [ ] Deployment manifest created
- [ ] Testnet deployment successful

### 2. Deployment Configuration

```json
{
  "name": "ProductionDeployment",
  "contracts": [
    {
      "name": "TokenContract",
      "projectPath": "src/Token/Token.csproj",
      "initParams": ["ProductionToken", "PROD", 8, 1000000000],
      "verifyAfterDeploy": true,
      "saveArtifacts": true
    }
  ],
  "security": {
    "requireMultiSig": true,
    "minSignatures": 3,
    "auditReport": "audits/token-audit-v1.pdf"
  }
}
```

### 3. Post-deployment Verification

```csharp
// Verify deployment
var exists = await toolkit.ContractExistsAsync(contractHash);
Assert.IsTrue(exists);

// Verify initialization
var symbol = await toolkit.CallAsync<string>(contractHash, "symbol");
Assert.AreEqual("PROD", symbol);

// Verify permissions
var owner = await toolkit.CallAsync<UInt160>(contractHash, "getOwner");
Assert.AreEqual(expectedOwner, owner);
```

## Summary

Following these best practices will help you create:
- **Secure** contracts resistant to common vulnerabilities
- **Efficient** contracts that minimize gas consumption
- **Maintainable** contracts that are easy to update and extend
- **Reliable** contracts that handle edge cases properly
- **Well-documented** contracts that others can understand

Remember to:
1. Always test thoroughly
2. Get security audits for production contracts
3. Start with simple implementations and iterate
4. Learn from existing contracts and examples
5. Stay updated with Neo platform changes

## Related Resources

- [Security Guide](../security/best-practices.md)
- [Gas Optimization Guide](../advanced/gas-optimization.md)
- [Testing Guide](../testing/unit-testing.md)
- [Contract Examples](../../examples/DeploymentExample/README.md)