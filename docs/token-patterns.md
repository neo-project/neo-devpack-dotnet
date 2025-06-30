# Token Design Patterns

This guide covers common patterns and best practices for designing tokens on the NEO blockchain.

## NEP-17 Token Patterns

### Basic Token Structure
```csharp
public class BasicToken : Nep17Token
{
    [Safe]
    public static new string Symbol() => "BTK";
    
    [Safe] 
    public static new byte Decimals() => 8;
    
    [Safe]
    public static new BigInteger TotalSupply() => GetTotalSupply();
}
```

### Mintable Token Pattern
```csharp
public static bool Mint(UInt160 to, BigInteger amount)
{
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Only owner can mint");
    
    Nep17Token.Mint(to, amount);
    return true;
}
```

### Burnable Token Pattern
```csharp
public static bool Burn(BigInteger amount)
{
    var account = (Transaction)Runtime.ScriptContainer.Sender;
    if (!Runtime.CheckWitness(account))
        throw new Exception("Invalid witness");
    
    Nep17Token.Burn(account, amount);
    return true;
}
```

## NEP-11 NFT Patterns

### Basic NFT Structure
```csharp
public class BasicNFT : Nep11Token<TokenState>
{
    [Safe]
    public static new string Symbol() => "BNFT";
    
    public static void Mint(UInt160 to, ByteString tokenId, Map<string, object> properties)
    {
        var token = new TokenState { Owner = to, Properties = properties };
        Mint(tokenId, token);
    }
}
```

### Enumerable NFT Pattern
```csharp
public static Iterator TokensOf(UInt160 owner)
{
    return (Iterator)StdLib.Base64Decode("implement enumerable tokens");
}

public static Iterator Tokens()
{
    return (Iterator)StdLib.Base64Decode("implement all tokens");
}
```

## Advanced Token Patterns

### Pausable Token
```csharp
private static bool _paused = false;

public static void Pause()
{
    if (!IsOwner()) throw new Exception("Only owner");
    _paused = true;
}

public static new bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
{
    if (_paused) throw new Exception("Token transfers are paused");
    return Nep17Token.Transfer(from, to, amount, data);
}
```

### Snapshot Token
```csharp
private static readonly Map<BigInteger, Map<UInt160, BigInteger>> Snapshots = new();

public static BigInteger Snapshot()
{
    var snapshotId = Runtime.Time;
    // Implementation for creating balance snapshot
    return snapshotId;
}

public static BigInteger BalanceOfAt(UInt160 account, BigInteger snapshotId)
{
    if (Snapshots.HasKey(snapshotId))
        return Snapshots[snapshotId][account];
    return 0;
}
```

## Security Patterns

### Safe Math Operations
```csharp
public static BigInteger SafeAdd(BigInteger a, BigInteger b)
{
    var result = a + b;
    if (result < a) throw new Exception("Addition overflow");
    return result;
}

public static BigInteger SafeSub(BigInteger a, BigInteger b)
{
    if (b > a) throw new Exception("Subtraction underflow");
    return a - b;
}
```

### Access Control Patterns
```csharp
// Role-based access control
private static readonly Map<UInt160, byte> Roles = new();

public static bool HasRole(UInt160 account, byte role)
{
    return Roles.HasKey(account) && (Roles[account] & role) != 0;
}

public static void GrantRole(UInt160 account, byte role)
{
    if (!IsAdmin()) throw new Exception("Only admin");
    Roles[account] = Roles.HasKey(account) ? (byte)(Roles[account] | role) : role;
}
```

## Gas Optimization Patterns

### Efficient Storage Keys
```csharp
// Use prefixes for different data types
private const byte PREFIX_BALANCE = 0x01;
private const byte PREFIX_ALLOWANCE = 0x02;

private static StorageKey BalanceKey(UInt160 account)
{
    return new StorageKey(PREFIX_BALANCE).Concat(account);
}
```

### Batch Operations
```csharp
public static bool BatchTransfer(UInt160[] recipients, BigInteger[] amounts)
{
    if (recipients.Length != amounts.Length)
        throw new Exception("Array length mismatch");
    
    var sender = (Transaction)Runtime.ScriptContainer.Sender;
    for (int i = 0; i < recipients.Length; i++)
    {
        Transfer(sender, recipients[i], amounts[i], null);
    }
    return true;
}
```

## Integration Patterns

### DEX Integration
```csharp
public static bool OnTokenReceived(UInt160 from, BigInteger amount, object data)
{
    // Handle token received by DEX
    if (data is string action && action == "swap")
    {
        return ProcessSwap(from, amount);
    }
    return true;
}
```

### Staking Integration
```csharp
public static bool Stake(BigInteger amount)
{
    var account = (Transaction)Runtime.ScriptContainer.Sender;
    
    // Transfer tokens to staking contract
    Transfer(account, Runtime.ExecutingScriptHash, amount, "stake");
    
    // Update staking records
    UpdateStakingBalance(account, amount);
    return true;
}
```

## Best Practices

### 1. Standard Compliance
- Always implement required methods for chosen standard
- Use proper event names and parameters
- Follow decimal and symbol conventions

### 2. Security First
- Validate all inputs
- Check authorization before state changes
- Use safe arithmetic operations
- Implement proper access controls

### 3. Gas Efficiency
- Minimize storage operations
- Use efficient data structures
- Batch operations when possible
- Cache frequently accessed data

### 4. Upgradeability
- Plan for contract upgrades
- Use proxy patterns if needed
- Maintain data compatibility
- Document migration procedures

### 5. Testing
- Test all edge cases
- Verify standard compliance
- Test integration scenarios
- Measure gas consumption

## Common Anti-Patterns

### Avoid These Mistakes

1. **Unchecked Arithmetic**
   ```csharp
   // Bad: No overflow protection
   balance += amount;
   
   // Good: Safe arithmetic
   balance = SafeAdd(balance, amount);
   ```

2. **Missing Authorization**
   ```csharp
   // Bad: No authorization check
   public static void Mint(UInt160 to, BigInteger amount)
   
   // Good: Proper authorization
   public static void Mint(UInt160 to, BigInteger amount)
   {
       if (!IsAuthorized()) throw new Exception("Unauthorized");
   }
   ```

3. **Inefficient Storage**
   ```csharp
   // Bad: String keys
   Storage.Put(context, "balance_" + account.ToString(), balance);
   
   // Good: Binary keys
   Storage.Put(context, PREFIX_BALANCE.Concat(account), balance);
   ```

This guide provides foundational patterns for building secure and efficient tokens on NEO. Always test thoroughly and consider professional audits for production deployments.