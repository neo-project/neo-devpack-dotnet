# Token Security Guide

This guide provides comprehensive security considerations for NEO token development, covering both NEP-17 and NEP-11 standards.

## Security Fundamentals

### Core Security Principles

1. **Principle of Least Privilege** - Grant minimal necessary permissions
2. **Defense in Depth** - Multiple layers of security controls
3. **Fail-Safe Defaults** - Default to secure states
4. **Input Validation** - Validate all external inputs
5. **Authorization First** - Check permissions before state changes

## NEP-17 Token Security

### Transfer Security

```csharp
public static new bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
{
    // 1. Input validation
    if (!from.IsValid || !to.IsValid)
        throw new Exception("Invalid address");
    
    if (amount <= 0)
        throw new Exception("Invalid amount");
    
    // 2. Authorization check
    if (!Runtime.CheckWitness(from) && !IsApprovedSpender(from, Runtime.CallingScriptHash))
        throw new Exception("Unauthorized transfer");
    
    // 3. Balance validation
    if (BalanceOf(from) < amount)
        throw new Exception("Insufficient balance");
    
    // 4. Reentrancy protection
    if (_transferInProgress)
        throw new Exception("Reentrant call detected");
    
    _transferInProgress = true;
    
    try
    {
        // 5. Execute transfer
        return ExecuteTransfer(from, to, amount, data);
    }
    finally
    {
        _transferInProgress = false;
    }
}
```

### Approval Mechanism Security

```csharp
private static readonly Map<UInt160, Map<UInt160, BigInteger>> Allowances = new();

public static bool Approve(UInt160 spender, BigInteger amount)
{
    var owner = (Transaction)Runtime.ScriptContainer.Sender;
    
    // Prevent approval to self
    if (owner == spender)
        throw new Exception("Cannot approve self");
    
    // Store allowance
    if (!Allowances.HasKey(owner))
        Allowances[owner] = new Map<UInt160, BigInteger>();
    
    Allowances[owner][spender] = amount;
    
    OnApproval(owner, spender, amount);
    return true;
}

public static bool TransferFrom(UInt160 spender, UInt160 from, UInt160 to, BigInteger amount)
{
    // Check spender authorization
    if (!Runtime.CheckWitness(spender))
        throw new Exception("Invalid spender witness");
    
    // Check allowance
    var allowance = Allowance(from, spender);
    if (allowance < amount)
        throw new Exception("Insufficient allowance");
    
    // Update allowance before transfer (prevent reentrancy)
    Allowances[from][spender] = allowance - amount;
    
    // Execute transfer
    return Transfer(from, to, amount, null);
}
```

## NEP-11 NFT Security

### Minting Security

```csharp
public static void Mint(UInt160 to, ByteString tokenId, Map<string, object> properties)
{
    // 1. Authorization check
    if (!Runtime.CheckWitness(GetOwner()) && !HasRole(Runtime.CallingScriptHash, MINTER_ROLE))
        throw new Exception("Unauthorized minting");
    
    // 2. Input validation
    if (!to.IsValid)
        throw new Exception("Invalid recipient address");
    
    if (tokenId.Length == 0 || tokenId.Length > 64)
        throw new Exception("Invalid token ID length");
    
    // 3. Duplicate check
    if (TokenExists(tokenId))
        throw new Exception("Token already exists");
    
    // 4. Properties validation
    ValidateTokenProperties(properties);
    
    // 5. Execute mint
    var token = new TokenState { Owner = to, Properties = properties };
    Mint(tokenId, token);
}

private static void ValidateTokenProperties(Map<string, object> properties)
{
    if (properties.Count > MAX_PROPERTIES)
        throw new Exception("Too many properties");
    
    foreach (var pair in properties)
    {
        if (pair.Key.Length > MAX_PROPERTY_KEY_LENGTH)
            throw new Exception("Property key too long");
        
        // Validate property values
        if (pair.Value is string str && str.Length > MAX_PROPERTY_VALUE_LENGTH)
            throw new Exception("Property value too long");
    }
}
```

### Transfer Security

```csharp
public static new bool Transfer(UInt160 to, ByteString tokenId, object data)
{
    var token = GetToken(tokenId);
    var from = token.Owner;
    
    // 1. Authorization check
    if (!Runtime.CheckWitness(from) && !IsApproved(tokenId, Runtime.CallingScriptHash))
        throw new Exception("Unauthorized transfer");
    
    // 2. Input validation
    if (!to.IsValid)
        throw new Exception("Invalid recipient");
    
    if (from == to)
        throw new Exception("Self-transfer not allowed");
    
    // 3. Token existence check
    if (token == null)
        throw new Exception("Token does not exist");
    
    // 4. Execute transfer
    return ExecuteNFTTransfer(from, to, tokenId, data);
}
```

## Common Vulnerabilities and Mitigations

### 1. Integer Overflow/Underflow

```csharp
// Vulnerable code
public static bool UnsafeTransfer(UInt160 from, UInt160 to, BigInteger amount)
{
    var fromBalance = BalanceOf(from);
    var toBalance = BalanceOf(to);
    
    // Potential overflow
    SetBalance(from, fromBalance - amount);
    SetBalance(to, toBalance + amount);
    
    return true;
}

// Secure code
public static bool SafeTransfer(UInt160 from, UInt160 to, BigInteger amount)
{
    var fromBalance = BalanceOf(from);
    var toBalance = BalanceOf(to);
    
    // Check underflow
    if (fromBalance < amount)
        throw new Exception("Insufficient balance");
    
    // Check overflow
    if (toBalance > BigInteger.MaxValue - amount)
        throw new Exception("Balance overflow");
    
    SetBalance(from, fromBalance - amount);
    SetBalance(to, toBalance + amount);
    
    return true;
}
```

### 2. Reentrancy Attacks

```csharp
private static bool _locked = false;

public static bool ReentrancyGuard(Func<bool> operation)
{
    if (_locked)
        throw new Exception("Reentrant call detected");
    
    _locked = true;
    try
    {
        return operation();
    }
    finally
    {
        _locked = false;
    }
}

// Usage
public static bool SecureTransfer(UInt160 from, UInt160 to, BigInteger amount)
{
    return ReentrancyGuard(() => ExecuteTransfer(from, to, amount));
}
```

### 3. Front-Running Protection

```csharp
// Use commit-reveal scheme for sensitive operations
private static readonly Map<UInt160, ByteString> PendingCommits = new();

public static bool CommitApproval(UInt160 spender, ByteString commitment)
{
    var owner = (Transaction)Runtime.ScriptContainer.Sender;
    PendingCommits[owner] = commitment;
    
    // Store block height for timeout
    Storage.Put(Storage.CurrentContext, 
               CommitKey(owner), Runtime.GetCurrentBlock().Index);
    return true;
}

public static bool RevealApproval(UInt160 spender, BigInteger amount, BigInteger nonce)
{
    var owner = (Transaction)Runtime.ScriptContainer.Sender;
    var commitment = PendingCommits[owner];
    var expectedCommit = CryptoLib.Sha256(spender.Concat(amount.ToByteArray()).Concat(nonce.ToByteArray()));
    
    if (commitment != expectedCommit)
        throw new Exception("Invalid reveal");
    
    return Approve(spender, amount);
}
```

### 4. Access Control Vulnerabilities

```csharp
// Multi-level access control
public enum Roles : byte
{
    Admin = 1,
    Minter = 2,
    Burner = 4,
    Pauser = 8
}

private static readonly Map<UInt160, byte> UserRoles = new();

public static bool HasRole(UInt160 account, Roles role)
{
    return UserRoles.HasKey(account) && (UserRoles[account] & (byte)role) != 0;
}

public static void GrantRole(UInt160 account, Roles role)
{
    // Only admin can grant roles
    if (!HasRole((Transaction)Runtime.ScriptContainer.Sender, Roles.Admin))
        throw new Exception("Only admin can grant roles");
    
    if (!UserRoles.HasKey(account))
        UserRoles[account] = 0;
    
    UserRoles[account] |= (byte)role;
    OnRoleGranted(account, role);
}
```

## Security Testing

### Unit Test Examples

```csharp
[TestMethod]
public void Test_PreventDoubleSpending()
{
    // Setup
    var alice = TestUsers.Alice;
    var bob = TestUsers.Bob;
    var charlie = TestUsers.Charlie;
    
    Contract.Transfer(alice, bob, 100);
    
    // Attempt double spending
    Engine.SetTransactionSigners(bob);
    
    // First transfer should succeed
    Assert.IsTrue(Contract.Transfer(bob, charlie, 50));
    
    // Second transfer should fail (insufficient balance)
    Assert.ThrowsException<Exception>(() => 
        Contract.Transfer(bob, charlie, 60));
}

[TestMethod]
public void Test_PreventUnauthorizedMinting()
{
    var attacker = TestUsers.Attacker;
    Engine.SetTransactionSigners(attacker);
    
    // Should fail - attacker is not owner/minter
    Assert.ThrowsException<Exception>(() => 
        Contract.Mint(attacker, 1000));
}

[TestMethod]
public void Test_ReentrancyProtection()
{
    // Setup malicious contract that tries reentrancy
    var maliciousContract = CreateMaliciousContract();
    
    // Transfer to malicious contract should not allow reentrancy
    Assert.ThrowsException<Exception>(() =>
        Contract.Transfer(TestUsers.Alice, maliciousContract, 100));
}
```

### Security Audit Checklist

#### Pre-Deployment Security Review

- [ ] **Input Validation**
  - [ ] All public methods validate inputs
  - [ ] Address validation for all UInt160 parameters
  - [ ] Amount validation (positive values, overflow checks)
  - [ ] String length validation for metadata

- [ ] **Authorization**
  - [ ] All state-changing methods check authorization
  - [ ] Multi-signature requirements implemented correctly
  - [ ] Role-based access control properly configured
  - [ ] Emergency stop mechanisms in place

- [ ] **Arithmetic Safety**
  - [ ] All arithmetic operations protected against overflow/underflow
  - [ ] Safe math library used consistently
  - [ ] Division by zero checks implemented

- [ ] **External Call Security**
  - [ ] External contract calls properly validated
  - [ ] Return values checked and handled
  - [ ] Reentrancy protection implemented
  - [ ] Gas limits considered for external calls

- [ ] **Storage Security**
  - [ ] Storage keys properly namespaced
  - [ ] Sensitive data encrypted if needed
  - [ ] Storage cleanup implemented where appropriate

## Emergency Response

### Pause Mechanism

```csharp
private static bool _paused = false;

public static void Pause()
{
    if (!HasRole((Transaction)Runtime.ScriptContainer.Sender, Roles.Pauser))
        throw new Exception("Only pauser can pause");
    
    _paused = true;
    OnPaused();
}

public static void Unpause()
{
    if (!HasRole((Transaction)Runtime.ScriptContainer.Sender, Roles.Admin))
        throw new Exception("Only admin can unpause");
    
    _paused = false;
    OnUnpaused();
}

private static void RequireNotPaused()
{
    if (_paused)
        throw new Exception("Contract is paused");
}
```

### Emergency Withdrawal

```csharp
public static bool EmergencyWithdraw()
{
    // Only admin in emergency situations
    if (!HasRole((Transaction)Runtime.ScriptContainer.Sender, Roles.Admin))
        throw new Exception("Only admin");
    
    // Transfer all contract funds to safe address
    var contractBalance = GAS.BalanceOf(Runtime.ExecutingScriptHash);
    return GAS.Transfer(Runtime.ExecutingScriptHash, GetEmergencyAddress(), contractBalance, null);
}
```

## Best Practices Summary

1. **Always validate inputs** before processing
2. **Check authorization** before state changes
3. **Use safe arithmetic** to prevent overflow/underflow
4. **Implement reentrancy protection** for critical functions
5. **Follow principle of least privilege** for access control
6. **Test extensively** including edge cases and attack scenarios
7. **Plan for emergencies** with pause and recovery mechanisms
8. **Keep contracts simple** to reduce attack surface
9. **Use established patterns** rather than inventing new ones
10. **Get professional security audits** before mainnet deployment

Remember: Security is not a feature to be added later - it must be built into the contract from the beginning.