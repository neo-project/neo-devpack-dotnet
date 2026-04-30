# Security Testing Guide for Neo Smart Contracts

Comprehensive testing frameworks and methodologies for validating security implementations in Neo smart contracts, including automated testing, vulnerability assessment, and continuous security validation.

> **Foundation**: Ensure you've implemented security patterns from other guides before testing them with these frameworks.

## Table of Contents

- [Security Testing Fundamentals](#security-testing-fundamentals)
- [Test Planning and Strategy](#test-planning-and-strategy)
- [Unit Testing for Security](#unit-testing-for-security)
- [Vulnerability Testing](#vulnerability-testing)
- [Access Control Testing](#access-control-testing)

## Security Testing Fundamentals

Use validated parsing helpers for any fixed inputs to align tests with production safety:

```csharp
private static readonly UInt160 Owner = UInt160.Parse("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB");
private static readonly UInt256 AssetId = UInt256.Parse("0x000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f");
private static readonly ECPoint AdminKey = ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
```

### Testing Pyramid for Smart Contract Security

```
    ┌─────────────────┐
    │   Manual Tests  │  Security Audits, Penetration Testing
    ├─────────────────┤
    │ Integration     │  Multi-contract interactions, E2E scenarios
    │ Security Tests  │
    ├─────────────────┤
    │ Component Tests │  Individual function security validation
    ├─────────────────┤
    │ Unit Security   │  Input validation, access control, state checks
    │ Tests           │
    └─────────────────┘
```

### Advanced Security Testing Frameworks

This guide focuses on comprehensive testing implementations that validate all security patterns and provide thorough coverage of vulnerability scenarios.

## Test Planning and Strategy

### Security Test Plan Template

```csharp
/// <summary>
/// Comprehensive security test plan for smart contracts
/// </summary>
[TestClass]
public class SecurityTestPlan : TestBase<YourContract>
{
    // Test Categories:
    // 1. Input Validation Tests
    // 2. Access Control Tests  
    // 3. State Management Tests
    // 4. Reentrancy Tests
    // 5. Integer Overflow Tests
    // 6. Gas Limit Tests
    // 7. External Call Tests
    // 8. Storage Security Tests
    
    [TestInitialize]
    public void SecurityTestSetup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
        
        // Initialize test accounts
        InitializeTestAccounts();
        
        // Set up test data
        SetupTestData();
        
        // Configure security monitoring
        EnableSecurityMonitoring();
    }
    
    [TestCleanup]
    public void SecurityTestCleanup()
    {
        // Analyze security events
        AnalyzeSecurityEvents();
        
        // Generate security report
        GenerateSecurityReport();
        
        // Clean up test state
        CleanupTestState();
    }
}
```

### Test Account Setup

```csharp
public class SecurityTestAccounts
{
    // Standard test accounts
    public static readonly UInt160 Owner = UInt160.Parse("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB");
    public static readonly UInt160 ValidUser = UInt160.Parse("NXpRXq8e9gRaH5vVAEUkHQeXNHLZsUfz1G");
    public static readonly UInt160 Attacker = UInt160.Parse("NVqXCjKHHi9xetyDhpEP6KtqCq8fHaXprC");
    public static readonly UInt160 UnauthorizedUser = UInt160.Parse("Nb2CHYY4wTqPQv7hPYnKr6CjN4fEeX2vks");
    
    // Multi-sig test accounts
    public static readonly UInt160[] MultiSigSigners = new[]
    {
        UInt160.Parse("NZNovmGqaNZF6P4qFUCcHzPE6GFU1F8ueT"),
        UInt160.Parse("NeJSJ4YsH89g9XrEqwWM4yVMm8jYM4jHG9"),
        UInt160.Parse("NbMKdHpJJ5T6K7gLhW2V8R9Q3wX5M8nTpN")
    };
    
    // Specialized test accounts
    public static readonly UInt160 ContractCaller = UInt160.Parse("NYzKR3qP8BV5w2j8Q9VH6tXz5M7nXsY8Tp");
    public static readonly UInt160 TokenHolder = UInt160.Parse("NQ5gR8pT9VX3j2m7W6hJ8kY5z4nP9qX7Bp");
}
```

## Unit Testing for Security

### Input Validation Testing

```csharp
[TestClass]
public class InputValidationSecurityTests : TestBase<YourContract>
{
    [TestMethod]
    public void TestNullInputRejection()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test null UInt160 parameters
        Assert.ThrowsException<Exception>(() =>
            Contract.ProcessUserData(null, "valid data"));
        
        // Test null string parameters
        Assert.ThrowsException<Exception>(() =>
            Contract.ProcessUserData(SecurityTestAccounts.ValidUser, null));
        
        // Test null byte array parameters
        Assert.ThrowsException<Exception>(() =>
            Contract.StoreData(SecurityTestAccounts.ValidUser, null));
    }
    
    [TestMethod]
    public void TestInputSizeValidation()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test oversized string input
        string largeString = new string('x', 10000);
        Assert.ThrowsException<Exception>(() =>
            Contract.ProcessUserData(SecurityTestAccounts.ValidUser, largeString));
        
        // Test oversized byte array
        byte[] largeArray = new byte[100000];
        Assert.ThrowsException<Exception>(() =>
            Contract.StoreData(SecurityTestAccounts.ValidUser, largeArray));
        
        // Test valid sizes work
        string validString = new string('x', 100);
        Assert.IsTrue(Contract.ProcessUserData(SecurityTestAccounts.ValidUser, validString));
    }
    
    [TestMethod]
    public void TestNumericBoundaryValidation()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test negative amounts where not allowed
        Assert.ThrowsException<Exception>(() =>
            Contract.Transfer(SecurityTestAccounts.ValidUser, SecurityTestAccounts.TokenHolder, -100));
        
        // Test zero amounts where not allowed
        Assert.ThrowsException<Exception>(() =>
            Contract.Transfer(SecurityTestAccounts.ValidUser, SecurityTestAccounts.TokenHolder, 0));
        
        // Test maximum value handling
        BigInteger maxValue = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935");
        Assert.ThrowsException<Exception>(() =>
            Contract.Transfer(SecurityTestAccounts.ValidUser, SecurityTestAccounts.TokenHolder, maxValue));
        
        // Test valid amounts work
        Assert.IsTrue(Contract.Transfer(SecurityTestAccounts.ValidUser, SecurityTestAccounts.TokenHolder, 100));
    }
    
    [TestMethod]
    public void TestStringFormatValidation()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test invalid characters
        Assert.ThrowsException<Exception>(() =>
            Contract.SetUserName(SecurityTestAccounts.ValidUser, "user<script>"));
        
        // Test SQL injection patterns
        Assert.ThrowsException<Exception>(() =>
            Contract.SetUserName(SecurityTestAccounts.ValidUser, "'; DROP TABLE--"));
        
        // Test Unicode attacks
        Assert.ThrowsException<Exception>(() =>
            Contract.SetUserName(SecurityTestAccounts.ValidUser, "user\u202e"));
        
        // Test valid format works
        Assert.IsTrue(Contract.SetUserName(SecurityTestAccounts.ValidUser, "validuser123"));
    }
    
    [TestMethod]
    public void TestArrayParameterValidation()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test empty arrays where not allowed
        Assert.ThrowsException<Exception>(() =>
            Contract.BatchProcess(new UInt160[0], new BigInteger[0]));
        
        // Test mismatched array lengths
        var users = new[] { SecurityTestAccounts.ValidUser, SecurityTestAccounts.TokenHolder };
        var amounts = new[] { BigInteger.One };
        Assert.ThrowsException<Exception>(() =>
            Contract.BatchProcess(users, amounts));
        
        // Test oversized arrays
        var largeUserArray = new UInt160[1000];
        var largeAmountArray = new BigInteger[1000];
        Assert.ThrowsException<Exception>(() =>
            Contract.BatchProcess(largeUserArray, largeAmountArray));
        
        // Test valid arrays work
        var validUsers = new[] { SecurityTestAccounts.ValidUser };
        var validAmounts = new[] { BigInteger.One };
        Assert.IsTrue(Contract.BatchProcess(validUsers, validAmounts));
    }
}
```

### Access Control Testing

```csharp
[TestClass]
public class AccessControlSecurityTests : TestBase<YourContract>
{
    [TestMethod]
    public void TestOwnerOnlyFunctions()
    {
        // Test unauthorized access fails
        Engine.SetCallingScriptHash(SecurityTestAccounts.UnauthorizedUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.AdminFunction("test"));
        
        // Test owner access succeeds
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Assert.IsTrue(Contract.AdminFunction("test"));
    }
    
    [TestMethod]
    public void TestWitnessVerification()
    {
        // Test that calling without proper witness fails
        Engine.SetCallingScriptHash(SecurityTestAccounts.Attacker);
        
        // Even if attacker sets themselves as parameter, witness check should fail
        Assert.ThrowsException<Exception>(() =>
            Contract.UserOnlyFunction(SecurityTestAccounts.ValidUser, "malicious data"));
        
        // Test proper witness verification works
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.IsTrue(Contract.UserOnlyFunction(SecurityTestAccounts.ValidUser, "valid data"));
    }
    
    [TestMethod]
    public void TestRoleBasedAccess()
    {
        // Setup roles
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Contract.GrantRole(SecurityTestAccounts.ValidUser, "moderator");
        
        // Test role-based function access
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.IsTrue(Contract.ModeratorFunction("test"));
        
        // Test unauthorized role access
        Engine.SetCallingScriptHash(SecurityTestAccounts.UnauthorizedUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.ModeratorFunction("test"));
        
        // Test role revocation
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Contract.RevokeRole(SecurityTestAccounts.ValidUser, "moderator");
        
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.ModeratorFunction("test"));
    }
    
    [TestMethod]
    public void TestMultiSigAccess()
    {
        var signers = SecurityTestAccounts.MultiSigSigners.Take(3).ToArray();
        
        // Test insufficient signatures
        Engine.SetCallingScriptHash(signers[0]);
        Assert.IsTrue(Contract.ProposeOperation("op1", "test", "data", signers));
        
        Engine.SetCallingScriptHash(signers[1]);
        Assert.IsTrue(Contract.SignOperation("op1"));
        
        // Should not execute with only 2 signatures (requires 3)
        var notifications = Notifications.Where(n => n.EventName == "OperationExecuted");
        Assert.IsFalse(notifications.Any());
        
        // Test sufficient signatures
        Engine.SetCallingScriptHash(signers[2]);
        Assert.IsTrue(Contract.SignOperation("op1"));
        
        // Should now execute
        notifications = Notifications.Where(n => n.EventName == "OperationExecuted");
        Assert.IsTrue(notifications.Any());
    }
    
    [TestMethod]
    public void TestPrivilegeEscalation()
    {
        // Test that users cannot escalate their own privileges
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        Assert.ThrowsException<Exception>(() =>
            Contract.GrantRole(SecurityTestAccounts.ValidUser, "admin"));
        
        // Test that users cannot grant privileges to others
        Assert.ThrowsException<Exception>(() =>
            Contract.GrantRole(SecurityTestAccounts.UnauthorizedUser, "moderator"));
        
        // Test that moderators cannot grant admin privileges
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Contract.GrantRole(SecurityTestAccounts.ValidUser, "moderator");
        
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.GrantRole(SecurityTestAccounts.UnauthorizedUser, "admin"));
    }
}
```

## Vulnerability Testing

### Reentrancy Attack Testing

```csharp
[TestClass]
public class ReentrancySecurityTests : TestBase<YourContract>
{
    [TestMethod]
    public void TestReentrancyProtection()
    {
        // Setup: User has some balance
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Contract.Mint(SecurityTestAccounts.ValidUser, 1000);
        
        // Create malicious contract that attempts reentrancy
        var maliciousContract = CreateMaliciousReentrantContract();
        
        // Test that reentrancy is prevented
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // First withdrawal should work
        Assert.IsTrue(Contract.Withdraw(SecurityTestAccounts.ValidUser, 100));
        
        // Simulate reentrancy attack
        bool reentrancyAttempted = false;
        Contract.OnExternalCall += () =>
        {
            if (!reentrancyAttempted)
            {
                reentrancyAttempted = true;
                // This should fail due to reentrancy guard
                Assert.ThrowsException<Exception>(() =>
                    Contract.Withdraw(SecurityTestAccounts.ValidUser, 100));
            }
        };
        
        // Trigger withdrawal that would attempt reentrancy
        Assert.IsTrue(Contract.WithdrawWithCallback(SecurityTestAccounts.ValidUser, 100));
        Assert.IsTrue(reentrancyAttempted, "Reentrancy attack should have been attempted and blocked");
    }
    
    [TestMethod]
    public void TestCrossContractReentrancy()
    {
        // Setup multiple contracts that could be used for cross-contract reentrancy
        var contractA = Contract;
        var contractB = CreateSecondContract();
        
        // Test that cross-contract reentrancy is prevented
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Attempt to use contract B to call back into contract A
        Assert.ThrowsException<Exception>(() =>
            contractA.ComplexOperation(contractB.Hash, "reentrant_call"));
    }
    
    [TestMethod]
    public void TestReadOnlyReentrancy()
    {
        // Test that read-only reentrancy doesn't affect state
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        var initialBalance = Contract.GetBalance(SecurityTestAccounts.ValidUser);
        
        // Simulate read-only reentrancy during state change
        Contract.OnExternalCall += () =>
        {
            // This read should see the old state, not the new state
            var currentBalance = Contract.GetBalance(SecurityTestAccounts.ValidUser);
            Assert.AreEqual(initialBalance, currentBalance);
        };
        
        Contract.WithdrawWithReadOnlyCallback(SecurityTestAccounts.ValidUser, 100);
    }
}
```

### Integer Overflow Testing

```csharp
[TestClass]
public class IntegerOverflowSecurityTests : TestBase<YourContract>
{
    [TestMethod]
    public void TestAdditionOverflow()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        
        // Test addition that would overflow
        BigInteger maxValue = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935");
        BigInteger largeValue = maxValue - 100;
        
        // Set user balance to near-maximum
        Contract.SetBalance(SecurityTestAccounts.ValidUser, largeValue);
        
        // Attempt to add amount that would cause overflow
        Assert.ThrowsException<Exception>(() =>
            Contract.Mint(SecurityTestAccounts.ValidUser, 200));
        
        // Verify balance unchanged
        Assert.AreEqual(largeValue, Contract.GetBalance(SecurityTestAccounts.ValidUser));
    }
    
    [TestMethod]
    public void TestSubtractionUnderflow()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Set small balance
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Contract.SetBalance(SecurityTestAccounts.ValidUser, 50);
        
        // Attempt to withdraw more than balance
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.Withdraw(SecurityTestAccounts.ValidUser, 100));
        
        // Verify balance unchanged
        Assert.AreEqual(50, Contract.GetBalance(SecurityTestAccounts.ValidUser));
    }
    
    [TestMethod]
    public void TestMultiplicationOverflow()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        
        BigInteger largeBase = BigInteger.Parse("340282366920938463463374607431768211456"); // 2^128
        BigInteger multiplier = 1000;
        
        // Test multiplication that would overflow
        Assert.ThrowsException<Exception>(() =>
            Contract.CalculateReward(largeBase, multiplier));
    }
    
    [TestMethod]
    public void TestSafeArithmeticOperations()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        
        // Test that safe operations work correctly
        BigInteger a = 1000000;
        BigInteger b = 2000000;
        
        Assert.AreEqual(3000000, Contract.SafeAdd(a, b));
        Assert.AreEqual(1000000, Contract.SafeSubtract(b, a));
        Assert.AreEqual(2000000000000, Contract.SafeMultiply(a, b));
    }
}
```

### Gas Limit and DoS Testing

```csharp
[TestClass]
public class GasLimitSecurityTests : TestBase<YourContract>
{
    [TestMethod]
    public void TestGasBombPrevention()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.Attacker);
        
        // Test that operations with unbounded loops are prevented
        var largeArray = new UInt160[10000];
        for (int i = 0; i < largeArray.Length; i++)
        {
            largeArray[i] = SecurityTestAccounts.ValidUser;
        }
        
        Assert.ThrowsException<Exception>(() =>
            Contract.ProcessLargeArray(largeArray));
    }
    
    [TestMethod]
    public void TestBatchSizeLimits()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test that batch operations have reasonable limits
        var users = new UInt160[1000];
        var amounts = new BigInteger[1000];
        
        Assert.ThrowsException<Exception>(() =>
            Contract.BatchTransfer(users, amounts));
        
        // Test that reasonable batch sizes work
        var smallUsers = new UInt160[10];
        var smallAmounts = new BigInteger[10];
        Array.Fill(smallUsers, SecurityTestAccounts.ValidUser);
        Array.Fill(smallAmounts, BigInteger.One);
        
        Assert.IsTrue(Contract.BatchTransfer(smallUsers, smallAmounts));
    }
    
    [TestMethod]
    public void TestStorageGasCosts()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test that storage operations have gas limits
        byte[] largeData = new byte[1048576]; // 1MB
        
        Assert.ThrowsException<Exception>(() =>
            Contract.StoreData(SecurityTestAccounts.ValidUser, largeData));
        
        // Test that reasonable data sizes work
        byte[] smallData = new byte[1024]; // 1KB
        Assert.IsTrue(Contract.StoreData(SecurityTestAccounts.ValidUser, smallData));
    }
    
    [TestMethod]
    public void TestComputationalComplexity()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test that computationally expensive operations are bounded
        Assert.ThrowsException<Exception>(() =>
            Contract.ExpensiveCalculation(1000000));
        
        // Test that reasonable computations work
        Assert.IsTrue(Contract.ExpensiveCalculation(100));
    }
}
```
