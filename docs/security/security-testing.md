# Security Testing Guide for Neo Smart Contracts

Comprehensive testing frameworks and methodologies for validating security implementations in Neo smart contracts, including automated testing, vulnerability assessment, and continuous security validation.

> **Foundation**: Ensure you've implemented security patterns from other guides before testing them with these frameworks.

## Table of Contents

- [Security Testing Fundamentals](#security-testing-fundamentals)
- [Test Planning and Strategy](#test-planning-and-strategy)
- [Unit Testing for Security](#unit-testing-for-security)
- [Integration Security Testing](#integration-security-testing)
- [Vulnerability Testing](#vulnerability-testing)
- [Performance and DoS Testing](#performance-and-dos-testing)
- [Access Control Testing](#access-control-testing)
- [State Management Testing](#state-management-testing)
- [Automated Security Testing](#automated-security-testing)
- [Security Test Reporting](#security-test-reporting)

## Security Testing Fundamentals

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.RuntimeCompilation;

/// <summary>
/// Comprehensive security test plan for smart contracts
/// </summary>
[TestClass]
public class SecurityTestPlan : ContractProjectTestBase
{
    public SecurityTestPlan()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

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
        EnsureContractDeployed();

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
    public static readonly UInt160 Owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    public static readonly UInt160 ValidUser = "NXpRXq8e9gRaH5vVAEUkHQeXNHLZsUfz1G".ToScriptHash();
    public static readonly UInt160 Attacker = "NVqXCjKHHi9xetyDhpEP6KtqCq8fHaXprC".ToScriptHash();
    public static readonly UInt160 UnauthorizedUser = "Nb2CHYY4wTqPQv7hPYnKr6CjN4fEeX2vks".ToScriptHash();
    
    // Multi-sig test accounts
    public static readonly UInt160[] MultiSigSigners = new[]
    {
        "NZNovmGqaNZF6P4qFUCcHzPE6GFU1F8ueT".ToScriptHash(),
        "NeJSJ4YsH89g9XrEqwWM4yVMm8jYM4jHG9".ToScriptHash(),
        "NbMKdHpJJ5T6K7gLhW2V8R9Q3wX5M8nTpN".ToScriptHash()
    };
    
    // Specialized test accounts
    public static readonly UInt160 ContractCaller = "NYzKR3qP8BV5w2j8Q9VH6tXz5M7nXsY8Tp".ToScriptHash();
    public static readonly UInt160 TokenHolder = "NQ5gR8pT9VX3j2m7W6hJ8kY5z4nP9qX7Bp".ToScriptHash();
}
```

## Unit Testing for Security

### Input Validation Testing

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class InputValidationSecurityTests : ContractProjectTestBase
{
    private static readonly Signer ValidUser = TestEngine.GetNewSigner();
    private static readonly Signer TokenHolder = TestEngine.GetNewSigner();

    public InputValidationSecurityTests()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
        Engine.SetTransactionSigners(ValidUser);
    }

    [TestMethod]
    public void TestNullInputRejection()
    {
        Assert.ThrowsException<Exception>(() => Contract.ProcessUserData(null, "valid data"));
        Assert.ThrowsException<Exception>(() => Contract.ProcessUserData(ValidUser.Account, null));
        Assert.ThrowsException<Exception>(() => Contract.StoreData(ValidUser.Account, null));
    }

    [TestMethod]
    public void TestInputSizeValidation()
    {
        string largeString = new string('x', 10_000);
        Assert.ThrowsException<Exception>(() => Contract.ProcessUserData(ValidUser.Account, largeString));

        byte[] largeArray = new byte[100_000];
        Assert.ThrowsException<Exception>(() => Contract.StoreData(ValidUser.Account, largeArray));

        string validString = new string('x', 100);
        Assert.IsTrue(Contract.ProcessUserData(ValidUser.Account, validString));
    }

    [TestMethod]
    public void TestNumericBoundaryValidation()
    {
        Assert.ThrowsException<Exception>(() => Contract.Transfer(ValidUser.Account, TokenHolder.Account, -100));
        Assert.ThrowsException<Exception>(() => Contract.Transfer(ValidUser.Account, TokenHolder.Account, 0));

        var maxValue = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935");
        Assert.ThrowsException<Exception>(() => Contract.Transfer(ValidUser.Account, TokenHolder.Account, maxValue));

        Assert.IsTrue(Contract.Transfer(ValidUser.Account, TokenHolder.Account, 100));
    }

    [TestMethod]
    public void TestStringFormatValidation()
    {
        Assert.ThrowsException<Exception>(() => Contract.SetUserName(ValidUser.Account, "user<script>"));
        Assert.ThrowsException<Exception>(() => Contract.SetUserName(ValidUser.Account, "'; DROP TABLE--"));
        Assert.ThrowsException<Exception>(() => Contract.SetUserName(ValidUser.Account, "user‮"));

        Assert.IsTrue(Contract.SetUserName(ValidUser.Account, "validuser123"));
    }

    [TestMethod]
    public void TestArrayParameterValidation()
    {
        Assert.ThrowsException<Exception>(() => Contract.BatchProcess(Array.Empty<UInt160>(), Array.Empty<BigInteger>()));

        var users = new[] { ValidUser.Account, TokenHolder.Account };
        var amounts = new[] { new BigInteger(10), new BigInteger(20) };
        Assert.IsTrue(Contract.BatchProcess(users, amounts));
    }
}
```


    }
}
```

### Access Control Testing

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class AccessControlSecurityTests : ContractProjectTestBase
{
    public AccessControlSecurityTests()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
    }

    [TestMethod]
    public void TestOwnershipTransfer()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.UnauthorizedUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.TransferOwnership(SecurityTestAccounts.UnauthorizedUser));

        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Assert.IsTrue(Contract.TransferOwnership(SecurityTestAccounts.ValidUser));
    }

    [TestMethod]
    public void TestRoleBasedAccess()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.Owner);
        Contract.GrantRole(SecurityTestAccounts.ValidUser, "moderator");

        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.IsTrue(Contract.ModeratorFunction("test"));

        Engine.SetCallingScriptHash(SecurityTestAccounts.UnauthorizedUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.ModeratorFunction("test"));

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

        Engine.SetCallingScriptHash(signers[0]);
        Assert.IsTrue(Contract.ProposeOperation("op1", "test", "data", signers));

        Engine.SetCallingScriptHash(signers[1]);
        Assert.IsTrue(Contract.SignOperation("op1"));

        var notifications = Notifications.Where(n => n.EventName == "OperationExecuted");
        Assert.IsFalse(notifications.Any());

        Engine.SetCallingScriptHash(signers[2]);
        Assert.IsTrue(Contract.SignOperation("op1"));

        notifications = Notifications.Where(n => n.EventName == "OperationExecuted");
        Assert.IsTrue(notifications.Any());
    }

    [TestMethod]
    public void TestPrivilegeEscalation()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        Assert.ThrowsException<Exception>(() =>
            Contract.GrantRole(SecurityTestAccounts.ValidUser, "admin"));

        Assert.ThrowsException<Exception>(() =>
            Contract.GrantRole(SecurityTestAccounts.UnauthorizedUser, "moderator"));

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

```

## Vulnerability Testing

### Reentrancy Attack Testing

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class ReentrancySecurityTests : ContractProjectTestBase
{
    private static readonly Signer Owner = TestEngine.GetNewSigner();
    private static readonly Signer ValidUser = TestEngine.GetNewSigner();

    public ReentrancySecurityTests()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
    }

    [TestMethod]
    public void TestReentrancyProtection()
    {
        Engine.SetCallingScriptHash(Owner.Account);
        Contract.Mint(ValidUser.Account, 1000);

        var maliciousContract = CreateMaliciousReentrantContract();

        Engine.SetCallingScriptHash(ValidUser.Account);
        Assert.IsTrue(Contract.Withdraw(ValidUser.Account, 100));

        bool reentrancyAttempted = false;
        Contract.OnExternalCall += () =>
        {
            if (!reentrancyAttempted)
            {
                reentrancyAttempted = true;
                Assert.ThrowsException<Exception>(() =>
                    Contract.Withdraw(ValidUser.Account, 100));
            }
        };

        Assert.IsTrue(Contract.WithdrawWithCallback(ValidUser.Account, 100));
        Assert.IsTrue(reentrancyAttempted, "Reentrancy attack should have been attempted and blocked");
    }
}
```

### Integer Overflow Testing


```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class IntegerOverflowSecurityTests : ContractProjectTestBase
{
    public IntegerOverflowSecurityTests()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
    }

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class GasLimitSecurityTests : ContractProjectTestBase
{
    public GasLimitSecurityTests()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
    }

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

## Performance and Load Testing

### Stress Testing Framework


```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class PerformanceSecurityTests : ContractProjectTestBase
{
    public PerformanceSecurityTests()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
    }

    [TestMethod]
    public void TestConcurrentAccess()
    {
        // Simulate concurrent access patterns
        var tasks = new List<Task>();
        var results = new ConcurrentBag<bool>();
        
        for (int i = 0; i < 100; i++)
        {
            var userId = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
                    var result = Contract.ProcessConcurrentOperation(userId, "data" + userId);
                    results.Add(result);
                }
                catch (Exception)
                {
                    results.Add(false);
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        
        // Verify all operations completed successfully
        Assert.AreEqual(100, results.Count);
        Assert.IsTrue(results.All(r => r), "All concurrent operations should succeed");
    }
    
    [TestMethod]
    public void TestMemoryConsumption()
    {
        // Test that contract doesn't consume excessive memory
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        long initialMemory = GC.GetTotalMemory(true);
        
        // Perform memory-intensive operations
        for (int i = 0; i < 1000; i++)
        {
            Contract.ProcessData(SecurityTestAccounts.ValidUser, $"data{i}");
        }
        
        long finalMemory = GC.GetTotalMemory(true);
        long memoryIncrease = finalMemory - initialMemory;
        
        // Memory increase should be reasonable (less than 100MB for this test)
        Assert.IsTrue(memoryIncrease < 100 * 1024 * 1024, 
            $"Memory increase too large: {memoryIncrease} bytes");
    }
    
    [TestMethod]
    public void TestDatabaseGrowthLimits()
    {
        Engine.SetCallingScriptHash(SecurityTestAccounts.ValidUser);
        
        // Test that storage growth is bounded
        int maxOperations = 10000;
        
        for (int i = 0; i < maxOperations; i++)
        {
            try
            {
                Contract.StoreData(SecurityTestAccounts.ValidUser, $"key{i}", $"data{i}");
            }
            catch (Exception ex) when (ex.Message.Contains("storage limit"))
            {
                // Expected when storage limits are reached
                Assert.IsTrue(i > 100, "Storage limit should allow reasonable amount of data");
                break;
            }
        }
    }
}
```

## Automated Security Testing

### Security Test Automation Framework


```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.RuntimeCompilation;

[TestClass]
public class AutomatedSecurityScanner : ContractProjectTestBase
{
    public AutomatedSecurityScanner()
        : base("../path/to/YourContract/YourContract.csproj", contractName: "YourContract")
    {
    }

    [TestInitialize]
    public void Setup()
    {
        EnsureContractDeployed();
    }

    [TestMethod]
    public void RunComprehensiveSecurityScan()
    {
        var scanner = new ContractSecurityScanner(Contract);
        var results = scanner.RunFullScan();
        
        // Assert no critical vulnerabilities found
        Assert.IsFalse(results.HasCriticalVulnerabilities, 
            $"Critical vulnerabilities found: {string.Join(", ", results.CriticalIssues)}");
        
        // Assert high-severity issues are within acceptable limits
        Assert.IsTrue(results.HighSeverityIssues.Count <= 2, 
            $"Too many high-severity issues: {results.HighSeverityIssues.Count}");
        
        // Log all findings for review
        LogSecurityFindings(results);
    }
    
    private void LogSecurityFindings(SecurityScanResults results)
    {
        Console.WriteLine("=== Security Scan Results ===");
        Console.WriteLine($"Critical Issues: {results.CriticalIssues.Count}");
        Console.WriteLine($"High Severity: {results.HighSeverityIssues.Count}");
        Console.WriteLine($"Medium Severity: {results.MediumSeverityIssues.Count}");
        Console.WriteLine($"Low Severity: {results.LowSeverityIssues.Count}");
        
        foreach (var issue in results.AllIssues)
        {
            Console.WriteLine($"[{issue.Severity}] {issue.Title}: {issue.Description}");
        }
    }
}

public class ContractSecurityScanner
{
    private readonly object _contract;
    
    public ContractSecurityScanner(object contract)
    {
        _contract = contract;
    }
    
    public SecurityScanResults RunFullScan()
    {
        var results = new SecurityScanResults();
        
        // Run various security checks
        results.AddResults(CheckAccessControls());
        results.AddResults(CheckInputValidation());
        results.AddResults(CheckIntegerOverflows());
        results.AddResults(CheckReentrancyProtection());
        results.AddResults(CheckGasLimits());
        results.AddResults(CheckStorageSecurity());
        
        return results;
    }
    
    private List<SecurityIssue> CheckAccessControls()
    {
        var issues = new List<SecurityIssue>();
        
        // Implement access control checks
        // This would use reflection to analyze contract methods
        // and verify proper access control implementation
        
        return issues;
    }
    
    private List<SecurityIssue> CheckInputValidation()
    {
        var issues = new List<SecurityIssue>();
        
        // Implement input validation checks
        // Test various invalid inputs and verify proper rejection
        
        return issues;
    }
    
    // Additional security check methods...
}

public class SecurityScanResults
{
    public List<SecurityIssue> CriticalIssues { get; } = new();
    public List<SecurityIssue> HighSeverityIssues { get; } = new();
    public List<SecurityIssue> MediumSeverityIssues { get; } = new();
    public List<SecurityIssue> LowSeverityIssues { get; } = new();
    
    public bool HasCriticalVulnerabilities => CriticalIssues.Any();
    
    public IEnumerable<SecurityIssue> AllIssues => 
        CriticalIssues.Concat(HighSeverityIssues)
                     .Concat(MediumSeverityIssues)
                     .Concat(LowSeverityIssues);
    
    public void AddResults(List<SecurityIssue> issues)
    {
        foreach (var issue in issues)
        {
            switch (issue.Severity)
            {
                case SecuritySeverity.Critical:
                    CriticalIssues.Add(issue);
                    break;
                case SecuritySeverity.High:
                    HighSeverityIssues.Add(issue);
                    break;
                case SecuritySeverity.Medium:
                    MediumSeverityIssues.Add(issue);
                    break;
                case SecuritySeverity.Low:
                    LowSeverityIssues.Add(issue);
                    break;
            }
        }
    }
}

public class SecurityIssue
{
    public string Title { get; set; }
    public string Description { get; set; }
    public SecuritySeverity Severity { get; set; }
    public string Recommendation { get; set; }
    public string Location { get; set; }
}

public enum SecuritySeverity
{
    Low,
    Medium,
    High,
    Critical
}
```

## Security Test Reporting

### Comprehensive Security Test Report

```csharp
[TestClass]
public class SecurityTestReporter
{
    [TestMethod]
    public void GenerateSecurityTestReport()
    {
        var report = new SecurityTestReport();
        
        // Run all security test suites and collect results
        report.AddTestSuite("Input Validation", RunInputValidationTests());
        report.AddTestSuite("Access Control", RunAccessControlTests());
        report.AddTestSuite("Reentrancy Protection", RunReentrancyTests());
        report.AddTestSuite("Integer Overflow", RunOverflowTests());
        report.AddTestSuite("Gas Limits", RunGasLimitTests());
        report.AddTestSuite("Storage Security", RunStorageSecurityTests());
        
        // Generate comprehensive report
        string reportContent = report.GenerateReport();
        
        // Save report to file
        File.WriteAllText("SecurityTestReport.md", reportContent);
        
        // Assert overall security posture
        Assert.IsTrue(report.OverallSecurityScore >= 85, 
            $"Security score too low: {report.OverallSecurityScore}%");
    }
    
    private TestSuiteResults RunInputValidationTests()
    {
        // Implementation would run all input validation tests
        // and return aggregated results
        return new TestSuiteResults
        {
            TotalTests = 25,
            PassedTests = 24,
            FailedTests = 1,
            CriticalFailures = 0
        };
    }
    
    // Additional test suite runner methods...
}

public class SecurityTestReport
{
    private readonly List<TestSuiteResult> _testSuites = new();
    
    public void AddTestSuite(string name, TestSuiteResults results)
    {
        _testSuites.Add(new TestSuiteResult
        {
            Name = name,
            Results = results,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public string GenerateReport()
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("# Smart Contract Security Test Report");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        
        sb.AppendLine("## Executive Summary");
        sb.AppendLine($"Overall Security Score: {OverallSecurityScore}%");
        sb.AppendLine($"Total Tests: {TotalTests}");
        sb.AppendLine($"Passed: {PassedTests}");
        sb.AppendLine($"Failed: {FailedTests}");
        sb.AppendLine($"Critical Failures: {CriticalFailures}");
        sb.AppendLine();
        
        sb.AppendLine("## Test Suite Results");
        foreach (var suite in _testSuites)
        {
            sb.AppendLine($"### {suite.Name}");
            sb.AppendLine($"- Total Tests: {suite.Results.TotalTests}");
            sb.AppendLine($"- Passed: {suite.Results.PassedTests}");
            sb.AppendLine($"- Failed: {suite.Results.FailedTests}");
            sb.AppendLine($"- Critical Failures: {suite.Results.CriticalFailures}");
            sb.AppendLine($"- Success Rate: {suite.SuccessRate:F1}%");
            sb.AppendLine();
        }
        
        sb.AppendLine("## Recommendations");
        GenerateRecommendations(sb);
        
        return sb.ToString();
    }
    
    public int OverallSecurityScore => 
        _testSuites.Any() ? (int)_testSuites.Average(s => s.SuccessRate) : 0;
    
    public int TotalTests => _testSuites.Sum(s => s.Results.TotalTests);
    public int PassedTests => _testSuites.Sum(s => s.Results.PassedTests);
    public int FailedTests => _testSuites.Sum(s => s.Results.FailedTests);
    public int CriticalFailures => _testSuites.Sum(s => s.Results.CriticalFailures);
    
    private void GenerateRecommendations(StringBuilder sb)
    {
        if (CriticalFailures > 0)
        {
            sb.AppendLine("⚠️ **CRITICAL**: Address critical security failures immediately before deployment.");
        }
        
        if (OverallSecurityScore < 90)
        {
            sb.AppendLine("🔍 **REVIEW**: Consider additional security measures and testing.");
        }
        
        if (OverallSecurityScore >= 95)
        {
            sb.AppendLine("✅ **EXCELLENT**: Security posture meets high standards.");
        }
        
        sb.AppendLine();
        sb.AppendLine("### Next Steps");
        sb.AppendLine("1. Address all critical and high-severity issues");
        sb.AppendLine("2. Consider professional security audit");
        sb.AppendLine("3. Implement continuous security monitoring");
        sb.AppendLine("4. Regular security test updates");
    }
}
```

Security testing is an ongoing process that should be integrated throughout the development lifecycle. Regular testing, automated scans, and comprehensive reporting ensure that your Neo smart contracts maintain high security standards and protect user assets effectively.