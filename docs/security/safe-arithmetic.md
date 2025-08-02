# Safe Arithmetic Operations in Neo Smart Contracts

Arithmetic operations in smart contracts require special attention to prevent integer overflow, underflow, and other mathematical vulnerabilities. This guide provides comprehensive patterns for safe arithmetic in Neo N3 smart contracts.

## Table of Contents

- [Arithmetic Security Fundamentals](#arithmetic-security-fundamentals)
- [BigInteger in Neo Contracts](#biginteger-in-neo-contracts)
- [Safe Addition Operations](#safe-addition-operations)
- [Safe Subtraction Operations](#safe-subtraction-operations)
- [Safe Multiplication Operations](#safe-multiplication-operations)
- [Safe Division Operations](#safe-division-operations)
- [Percentage and Ratio Calculations](#percentage-and-ratio-calculations)
- [Fixed-Point Arithmetic](#fixed-point-arithmetic)
- [Rounding and Precision](#rounding-and-precision)
- [Testing Arithmetic Security](#testing-arithmetic-security)

## Arithmetic Security Fundamentals

This guide focuses on implementing safe arithmetic operations in Neo smart contracts using practical patterns and comprehensive validation techniques.

> **Foundation**: Review [Common Vulnerabilities](common-vulnerabilities.md#integer-overflowunderflow) for basic overflow concepts before implementing these patterns.

### Neo N3 BigInteger Characteristics

> **Important**: As noted by the Neo team, BigInteger in Neo VM has a maximum length of 256 bits (32 bytes). 
> The VM will throw an exception if a value exceeds this limit, providing automatic overflow protection.
> However, we still recommend implementing explicit bounds checking for business logic validation.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

[DisplayName("ArithmeticSafetyDemo")]
public class ArithmeticSafetyDemo : SmartContract
{
    // BigInteger in Neo VM has a maximum of 256 bits (32 bytes)
    // The VM automatically prevents overflow beyond this limit
    // We still define practical bounds for business logic validation
    
    /// <summary>
    /// Maximum safe value for token amounts (example: 21M tokens with 8 decimals)
    /// </summary>
    private static readonly BigInteger MAX_TOKEN_AMOUNT = 2_100_000_000_000_000; // 21M * 10^8
    
    /// <summary>
    /// Maximum safe value for general calculations
    /// </summary>
    private static readonly BigInteger MAX_SAFE_VALUE = BigInteger.Parse("79228162514264337593543950335"); // Decimal.MaxValue
    
    /// <summary>
    /// Demonstrate BigInteger characteristics in Neo
    /// </summary>
    [Safe]
    public static void DemonstrateBigIntegerBehavior()
    {
        // BigInteger in Neo VM has automatic overflow protection at 256 bits
        BigInteger large1 = BigInteger.Parse("999999999999999999999999999999");
        BigInteger large2 = BigInteger.Parse("111111111111111111111111111111");
        
        // The VM will automatically throw if result exceeds 256 bits
        // We still validate for business logic constraints
        BigInteger result = large1 + large2;
        
        // Always validate results against business logic constraints
        Assert(result <= MAX_SAFE_VALUE, "Result exceeds safe limits");
    }
}
```

## Safe Addition Operations

### Basic Safe Addition

```csharp
public class SafeAddition : SmartContract
{
    /// <summary>
    /// Safe addition with business logic validation
    /// </summary>
    public static BigInteger SafeAdd(BigInteger a, BigInteger b)
    {
        // Input validation
        Assert(a >= 0, "First operand must be non-negative");
        Assert(b >= 0, "Second operand must be non-negative");
        
        // Check against business logic limits (VM handles 256-bit overflow automatically)
        Assert(a <= MAX_SAFE_VALUE - b, "Addition would exceed business logic limits");
        
        BigInteger result = a + b;
        
        // Post-condition check
        Assert(result >= a && result >= b, "Addition result validation failed");
        
        return result;
    }
    
    /// <summary>
    /// Safe addition for token amounts with specific limits
    /// </summary>
    public static BigInteger SafeAddTokens(BigInteger currentBalance, BigInteger amount)
    {
        Assert(currentBalance >= 0, "Current balance cannot be negative");
        Assert(amount > 0, "Amount must be positive");
        Assert(amount <= MAX_TOKEN_AMOUNT, "Amount exceeds maximum token limit");
        
        // Check total won't exceed maximum supply
        Assert(currentBalance <= MAX_TOKEN_AMOUNT - amount, "Addition would exceed maximum token supply");
        
        return currentBalance + amount;
    }
    
    /// <summary>
    /// Batch safe addition with cumulative overflow protection
    /// </summary>
    public static BigInteger SafeAddBatch(BigInteger[] values)
    {
        Assert(values != null && values.Length > 0, "Values array required");
        Assert(values.Length <= 100, "Too many values in batch");
        
        BigInteger total = 0;
        
        foreach (BigInteger value in values)
        {
            Assert(value >= 0, "All values must be non-negative");
            total = SafeAdd(total, value);
        }
        
        return total;
    }
    
    /// <summary>
    /// Safe addition with percentage bounds checking
    /// </summary>
    public static BigInteger SafeAddWithPercentageLimit(BigInteger base_, BigInteger addition, 
                                                       int maxPercentageIncrease)
    {
        Assert(base_ > 0, "Base value must be positive");
        Assert(addition >= 0, "Addition must be non-negative");
        Assert(maxPercentageIncrease > 0 && maxPercentageIncrease <= 10000, "Invalid percentage limit"); // 100.00%
        
        // Calculate maximum allowed addition (as percentage of base)
        BigInteger maxAddition = (base_ * maxPercentageIncrease) / 10000;
        Assert(addition <= maxAddition, $"Addition exceeds {maxPercentageIncrease / 100}% limit");
        
        return SafeAdd(base_, addition);
    }
}
```

## Safe Subtraction Operations

### Underflow Protection

```csharp
public class SafeSubtraction : SmartContract
{
    /// <summary>
    /// Safe subtraction with underflow protection
    /// </summary>
    public static BigInteger SafeSubtract(BigInteger a, BigInteger b)
    {
        Assert(a >= 0, "Minuend must be non-negative");
        Assert(b >= 0, "Subtrahend must be non-negative");
        Assert(a >= b, "Subtraction would cause underflow");
        
        BigInteger result = a - b;
        
        // Post-condition validation
        Assert(result >= 0, "Result must be non-negative");
        Assert(result <= a, "Result must not exceed original value");
        
        return result;
    }
    
    /// <summary>
    /// Safe subtraction for balance operations
    /// </summary>
    public static BigInteger SafeSubtractBalance(BigInteger currentBalance, BigInteger amount)
    {
        Assert(currentBalance >= 0, "Current balance cannot be negative");
        Assert(amount > 0, "Amount must be positive");
        Assert(currentBalance >= amount, "Insufficient balance");
        
        BigInteger newBalance = currentBalance - amount;
        
        // Ensure balance integrity
        Assert(newBalance >= 0, "New balance cannot be negative");
        Assert(newBalance == currentBalance - amount, "Balance calculation error");
        
        return newBalance;
    }
    
    /// <summary>
    /// Safe subtraction with minimum threshold
    /// </summary>
    public static BigInteger SafeSubtractWithMinimum(BigInteger current, BigInteger amount, 
                                                   BigInteger minimumRequired)
    {
        Assert(current >= minimumRequired, "Current value below minimum");
        Assert(amount > 0, "Amount must be positive");
        
        BigInteger remaining = SafeSubtract(current, amount);
        Assert(remaining >= minimumRequired, "Operation would leave value below minimum threshold");
        
        return remaining;
    }
    
    /// <summary>
    /// Batch subtraction with atomic operation guarantee
    /// </summary>
    public static BigInteger SafeSubtractBatch(BigInteger initial, BigInteger[] amounts)
    {
        Assert(amounts != null && amounts.Length > 0, "Amounts array required");
        Assert(amounts.Length <= 50, "Too many amounts in batch");
        
        // Pre-validate that total subtraction is possible
        BigInteger totalToSubtract = SafeAddBatch(amounts);
        Assert(initial >= totalToSubtract, "Insufficient value for batch subtraction");
        
        BigInteger current = initial;
        foreach (BigInteger amount in amounts)
        {
            current = SafeSubtract(current, amount);
        }
        
        // Verify final result matches expected calculation
        Assert(current == initial - totalToSubtract, "Batch subtraction result mismatch");
        
        return current;
    }
    
    /// <summary>
    /// Safe percentage deduction
    /// </summary>
    public static BigInteger SafeSubtractPercentage(BigInteger amount, int percentageToDeduct)
    {
        Assert(amount >= 0, "Amount must be non-negative");
        Assert(percentageToDeduct >= 0 && percentageToDeduct <= 10000, "Invalid percentage"); // 0-100.00%
        
        BigInteger deduction = (amount * percentageToDeduct) / 10000;
        return SafeSubtract(amount, deduction);
    }
}
```

## Safe Multiplication Operations

### Overflow Prevention in Multiplication

```csharp
public class SafeMultiplication : SmartContract
{
    /// <summary>
    /// Safe multiplication with business logic validation
    /// </summary>
    public static BigInteger SafeMultiply(BigInteger a, BigInteger b)
    {
        Assert(a >= 0 && b >= 0, "Both operands must be non-negative");
        
        // Special cases
        if (a == 0 || b == 0) return 0;
        if (a == 1) return b;
        if (b == 1) return a;
        
        // Check against business logic limits (VM handles 256-bit overflow automatically)
        Assert(a <= MAX_SAFE_VALUE / b, "Multiplication would exceed business logic limits");
        
        BigInteger result = a * b;
        
        // Verify result integrity
        Assert(result / a == b, "Multiplication result verification failed");
        
        return result;
    }
    
    /// <summary>
    /// Safe multiplication for financial calculations
    /// </summary>
    public static BigInteger SafeMultiplyFinancial(BigInteger principal, BigInteger rate, 
                                                 BigInteger timePeriods)
    {
        Assert(principal > 0, "Principal must be positive");
        Assert(rate >= 0, "Rate must be non-negative");
        Assert(timePeriods >= 0, "Time periods must be non-negative");
        
        // Perform multiplication in safe order (smallest first)
        BigInteger temp = SafeMultiply(rate, timePeriods);
        return SafeMultiply(principal, temp);
    }
    
    /// <summary>
    /// Safe power operation (exponentiation)
    /// </summary>
    public static BigInteger SafePower(BigInteger base_, int exponent)
    {
        Assert(base_ >= 0, "Base must be non-negative");
        Assert(exponent >= 0, "Exponent must be non-negative");
        Assert(exponent <= 64, "Exponent too large"); // Reasonable limit
        
        if (exponent == 0) return 1;
        if (exponent == 1) return base_;
        if (base_ == 0) return 0;
        if (base_ == 1) return 1;
        
        BigInteger result = 1;
        BigInteger currentBase = base_;
        int currentExponent = exponent;
        
        // Exponentiation by squaring to prevent overflow
        while (currentExponent > 0)
        {
            if (currentExponent % 2 == 1)
            {
                result = SafeMultiply(result, currentBase);
            }
            
            if (currentExponent > 1)
            {
                currentBase = SafeMultiply(currentBase, currentBase);
            }
            
            currentExponent /= 2;
        }
        
        return result;
    }
    
    /// <summary>
    /// Safe scaling multiplication for decimal-like operations
    /// </summary>
    public static BigInteger SafeScale(BigInteger amount, BigInteger numerator, BigInteger denominator)
    {
        Assert(amount >= 0, "Amount must be non-negative");
        Assert(numerator >= 0, "Numerator must be non-negative");
        Assert(denominator > 0, "Denominator must be positive");
        
        // Handle zero cases
        if (amount == 0 || numerator == 0) return 0;
        
        // Check for potential overflow in multiplication
        Assert(amount <= MAX_SAFE_VALUE / numerator, "Scaling would cause overflow");
        
        BigInteger scaledAmount = SafeMultiply(amount, numerator);
        return SafeDivide(scaledAmount, denominator);
    }
}
```

## Safe Division Operations

### Division by Zero and Precision Handling

```csharp
public class SafeDivision : SmartContract
{
    /// <summary>
    /// Safe division with zero protection
    /// </summary>
    public static BigInteger SafeDivide(BigInteger dividend, BigInteger divisor)
    {
        Assert(dividend >= 0, "Dividend must be non-negative");
        Assert(divisor > 0, "Divisor must be positive");
        
        BigInteger result = dividend / divisor;
        
        // Verify division integrity
        Assert(result * divisor <= dividend, "Division result verification failed");
        Assert((result + 1) * divisor > dividend, "Division precision verification failed");
        
        return result;
    }
    
    /// <summary>
    /// Safe division with remainder information
    /// </summary>
    public static (BigInteger quotient, BigInteger remainder) SafeDivideWithRemainder(
        BigInteger dividend, BigInteger divisor)
    {
        Assert(dividend >= 0, "Dividend must be non-negative");
        Assert(divisor > 0, "Divisor must be positive");
        
        BigInteger quotient = dividend / divisor;
        BigInteger remainder = dividend % divisor;
        
        // Verify division identity: dividend = quotient * divisor + remainder
        Assert(dividend == quotient * divisor + remainder, "Division identity verification failed");
        Assert(remainder < divisor, "Remainder must be less than divisor");
        
        return (quotient, remainder);
    }
    
    /// <summary>
    /// Safe division with rounding mode specification
    /// </summary>
    public static BigInteger SafeDivideRounded(BigInteger dividend, BigInteger divisor, 
                                             RoundingMode mode = RoundingMode.Down)
    {
        var (quotient, remainder) = SafeDivideWithRemainder(dividend, divisor);
        
        switch (mode)
        {
            case RoundingMode.Down:
                return quotient;
                
            case RoundingMode.Up:
                return remainder > 0 ? quotient + 1 : quotient;
                
            case RoundingMode.Nearest:
                BigInteger halfDivisor = divisor / 2;
                return remainder >= halfDivisor ? quotient + 1 : quotient;
                
            default:
                throw new ArgumentException("Invalid rounding mode");
        }
    }
    
    /// <summary>
    /// Safe percentage calculation
    /// </summary>
    public static BigInteger SafePercentage(BigInteger amount, int percentage)
    {
        Assert(amount >= 0, "Amount must be non-negative");
        Assert(percentage >= 0 && percentage <= 10000, "Percentage must be 0-10000 (0-100.00%)");
        
        if (percentage == 0) return 0;
        if (percentage == 10000) return amount;
        
        BigInteger result = SafeMultiply(amount, percentage);
        return SafeDivide(result, 10000);
    }
    
    /// <summary>
    /// Safe division for token distribution
    /// </summary>
    public static BigInteger[] SafeDistribute(BigInteger totalAmount, BigInteger[] weights)
    {
        Assert(totalAmount >= 0, "Total amount must be non-negative");
        Assert(weights != null && weights.Length > 0, "Weights array required");
        Assert(weights.Length <= 100, "Too many distribution targets");
        
        // Validate all weights are positive
        BigInteger totalWeight = 0;
        foreach (BigInteger weight in weights)
        {
            Assert(weight > 0, "All weights must be positive");
            totalWeight = SafeAdd(totalWeight, weight);
        }
        
        Assert(totalWeight > 0, "Total weight must be positive");
        
        BigInteger[] distributions = new BigInteger[weights.Length];
        BigInteger distributed = 0;
        
        // Distribute proportionally
        for (int i = 0; i < weights.Length - 1; i++)
        {
            distributions[i] = SafeDivide(SafeMultiply(totalAmount, weights[i]), totalWeight);
            distributed = SafeAdd(distributed, distributions[i]);
        }
        
        // Give remainder to last recipient to ensure exact total
        distributions[weights.Length - 1] = SafeSubtract(totalAmount, distributed);
        
        // Verify total distribution equals input
        BigInteger verifyTotal = SafeAddBatch(distributions);
        Assert(verifyTotal == totalAmount, "Distribution total mismatch");
        
        return distributions;
    }
}

public enum RoundingMode
{
    Down,
    Up,
    Nearest
}
```

## Percentage and Ratio Calculations

### Precise Percentage Operations

```csharp
public class SafePercentageOperations : SmartContract
{
    // Use basis points for precise percentage calculations (1 basis point = 0.01%)
    private const int BASIS_POINTS_SCALE = 10000; // 100.00%
    private const int PERCENTAGE_SCALE = 100;     // 100%
    
    /// <summary>
    /// Calculate percentage with basis points precision
    /// </summary>
    public static BigInteger CalculatePercentageBasisPoints(BigInteger amount, int basisPoints)
    {
        Assert(amount >= 0, "Amount must be non-negative");
        Assert(basisPoints >= 0 && basisPoints <= BASIS_POINTS_SCALE, 
               "Basis points must be 0-10000");
        
        if (basisPoints == 0) return 0;
        if (basisPoints == BASIS_POINTS_SCALE) return amount;
        
        BigInteger result = SafeMultiply(amount, basisPoints);
        return SafeDivide(result, BASIS_POINTS_SCALE);
    }
    
    /// <summary>
    /// Apply compound percentage with safety checks
    /// </summary>
    public static BigInteger ApplyCompoundPercentage(BigInteger principal, int annualBasisPoints, 
                                                   int periods)
    {
        Assert(principal > 0, "Principal must be positive");
        Assert(annualBasisPoints >= 0, "Interest rate must be non-negative");
        Assert(periods >= 0 && periods <= 100, "Invalid number of periods");
        
        if (periods == 0 || annualBasisPoints == 0) return principal;
        
        BigInteger current = principal;
        
        for (int i = 0; i < periods; i++)
        {
            BigInteger interest = CalculatePercentageBasisPoints(current, annualBasisPoints);
            current = SafeAdd(current, interest);
            
            // Prevent runaway growth
            Assert(current <= principal * 1000, "Compound growth exceeds safety limits");
        }
        
        return current;
    }
    
    /// <summary>
    /// Calculate weighted average with precision
    /// </summary>
    public static BigInteger CalculateWeightedAverage(BigInteger[] values, BigInteger[] weights)
    {
        Assert(values != null && weights != null, "Arrays cannot be null");
        Assert(values.Length == weights.Length, "Arrays must have equal length");
        Assert(values.Length > 0, "Arrays cannot be empty");
        
        BigInteger weightedSum = 0;
        BigInteger totalWeight = 0;
        
        for (int i = 0; i < values.Length; i++)
        {
            Assert(values[i] >= 0, $"Value at index {i} must be non-negative");
            Assert(weights[i] > 0, $"Weight at index {i} must be positive");
            
            BigInteger weightedValue = SafeMultiply(values[i], weights[i]);
            weightedSum = SafeAdd(weightedSum, weightedValue);
            totalWeight = SafeAdd(totalWeight, weights[i]);
        }
        
        return SafeDivide(weightedSum, totalWeight);
    }
    
    /// <summary>
    /// Calculate pro-rata distribution
    /// </summary>
    public static BigInteger CalculateProRataShare(BigInteger totalPool, BigInteger userShare, 
                                                 BigInteger totalShares)
    {
        Assert(totalPool >= 0, "Total pool must be non-negative");
        Assert(userShare >= 0, "User share must be non-negative");
        Assert(totalShares > 0, "Total shares must be positive");
        Assert(userShare <= totalShares, "User share cannot exceed total shares");
        
        if (userShare == 0) return 0;
        if (userShare == totalShares) return totalPool;
        
        BigInteger userPool = SafeMultiply(totalPool, userShare);
        return SafeDivide(userPool, totalShares);
    }
}
```

## Fixed-Point Arithmetic

### Decimal-Like Operations with Integer Math

```csharp
public class FixedPointArithmetic : SmartContract
{
    // Fixed-point scaling factors
    private const int DECIMALS_8 = 8;
    private const int DECIMALS_18 = 18;
    private static readonly BigInteger SCALE_8 = 100_000_000;      // 10^8
    private static readonly BigInteger SCALE_18 = BigInteger.Parse("1000000000000000000"); // 10^18
    
    /// <summary>
    /// Convert integer to fixed-point representation
    /// </summary>
    public static BigInteger ToFixedPoint(BigInteger value, int decimals = DECIMALS_8)
    {
        Assert(value >= 0, "Value must be non-negative");
        Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");
        
        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        return SafeMultiply(value, scale);
    }
    
    /// <summary>
    /// Convert fixed-point to integer (truncating)
    /// </summary>
    public static BigInteger FromFixedPoint(BigInteger fixedValue, int decimals = DECIMALS_8)
    {
        Assert(fixedValue >= 0, "Fixed value must be non-negative");
        Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");
        
        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        return SafeDivide(fixedValue, scale);
    }
    
    /// <summary>
    /// Add two fixed-point numbers
    /// </summary>
    public static BigInteger AddFixedPoint(BigInteger a, BigInteger b, int decimals = DECIMALS_8)
    {
        Assert(a >= 0 && b >= 0, "Fixed-point values must be non-negative");
        return SafeAdd(a, b);
    }
    
    /// <summary>
    /// Multiply two fixed-point numbers
    /// </summary>
    public static BigInteger MultiplyFixedPoint(BigInteger a, BigInteger b, int decimals = DECIMALS_8)
    {
        Assert(a >= 0 && b >= 0, "Fixed-point values must be non-negative");
        Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");
        
        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        BigInteger product = SafeMultiply(a, b);
        return SafeDivide(product, scale);
    }
    
    /// <summary>
    /// Divide two fixed-point numbers
    /// </summary>
    public static BigInteger DivideFixedPoint(BigInteger a, BigInteger b, int decimals = DECIMALS_8)
    {
        Assert(a >= 0, "Dividend must be non-negative");
        Assert(b > 0, "Divisor must be positive");
        Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");
        
        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        BigInteger scaledDividend = SafeMultiply(a, scale);
        return SafeDivide(scaledDividend, b);
    }
    
    /// <summary>
    /// Calculate square root of fixed-point number using Newton's method
    /// </summary>
    public static BigInteger SqrtFixedPoint(BigInteger fixedValue, int decimals = DECIMALS_8)
    {
        Assert(fixedValue >= 0, "Cannot calculate square root of negative number");
        
        if (fixedValue == 0) return 0;
        
        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        
        // Newton's method for square root
        BigInteger x = fixedValue;
        BigInteger y = SafeAdd(x, 1) / 2;
        
        // Limit iterations to prevent infinite loops
        int maxIterations = 50;
        int iterations = 0;
        
        while (y < x && iterations < maxIterations)
        {
            x = y;
            y = SafeAdd(SafeDivide(fixedValue, x), x) / 2;
            iterations++;
        }
        
        return x;
    }
}
```

## Testing Arithmetic Security

### Comprehensive Arithmetic Security Tests

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

[TestClass]
public class ArithmeticSecurityTests : TestBase<SafeArithmeticContract>
{
    [TestInitialize]
    public void Setup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
    }
    
    [TestMethod]
    public void TestAdditionOverflowPrevention()
    {
        // Test that addition overflow is properly prevented
        BigInteger maxSafe = BigInteger.Parse("79228162514264337593543950335");
        BigInteger largeValue = maxSafe - 100;
        
        // This should work
        Assert.AreEqual(maxSafe - 99, Contract.SafeAdd(largeValue, 1));
        
        // This should fail (overflow)
        Assert.ThrowsException<Exception>(() => 
            Contract.SafeAdd(largeValue, 200));
    }
    
    [TestMethod]
    public void TestSubtractionUnderflowPrevention()
    {
        // Test that subtraction underflow is properly prevented
        Assert.AreEqual(50, Contract.SafeSubtract(100, 50));
        
        // This should fail (underflow)
        Assert.ThrowsException<Exception>(() =>
            Contract.SafeSubtract(50, 100));
    }
    
    [TestMethod]
    public void TestMultiplicationOverflowPrevention()
    {
        // Test safe multiplication bounds
        BigInteger base1 = 1000000;
        BigInteger base2 = 2000000;
        
        // This should work
        Assert.AreEqual(2000000000000, Contract.SafeMultiply(base1, base2));
        
        // Test overflow prevention
        BigInteger largeBase1 = BigInteger.Parse("100000000000000000");
        BigInteger largeBase2 = BigInteger.Parse("100000000000000000");
        
        Assert.ThrowsException<Exception>(() =>
            Contract.SafeMultiply(largeBase1, largeBase2));
    }
    
    [TestMethod]
    public void TestDivisionByZeroPrevention()
    {
        // Test division by zero prevention
        Assert.ThrowsException<Exception>(() =>
            Contract.SafeDivide(100, 0));
        
        // Test valid division
        Assert.AreEqual(50, Contract.SafeDivide(100, 2));
    }
    
    [TestMethod]
    public void TestPercentageCalculations()
    {
        // Test basis points calculations
        BigInteger amount = 1000000; // 10.00000 tokens (8 decimals)
        
        // 5% = 500 basis points
        BigInteger fivePercent = Contract.CalculatePercentageBasisPoints(amount, 500);
        Assert.AreEqual(50000, fivePercent); // 0.50000 tokens
        
        // 100% = 10000 basis points
        BigInteger oneHundredPercent = Contract.CalculatePercentageBasisPoints(amount, 10000);
        Assert.AreEqual(amount, oneHundredPercent);
        
        // Test invalid percentage
        Assert.ThrowsException<Exception>(() =>
            Contract.CalculatePercentageBasisPoints(amount, 15000)); // > 100%
    }
    
    [TestMethod]
    public void TestFixedPointArithmetic()
    {
        // Test fixed-point conversion
        BigInteger integer = 5;
        BigInteger fixed8 = Contract.ToFixedPoint(integer, 8);
        Assert.AreEqual(500000000, fixed8); // 5.00000000
        
        // Test fixed-point multiplication
        BigInteger fixed1 = Contract.ToFixedPoint(2, 8); // 2.00000000
        BigInteger fixed2 = Contract.ToFixedPoint(3, 8); // 3.00000000
        BigInteger product = Contract.MultiplyFixedPoint(fixed1, fixed2, 8);
        BigInteger expected = Contract.ToFixedPoint(6, 8); // 6.00000000
        Assert.AreEqual(expected, product);
        
        // Test conversion back to integer
        Assert.AreEqual(6, Contract.FromFixedPoint(product, 8));
    }
    
    [TestMethod]
    public void TestDistributionCalculations()
    {
        // Test pro-rata distribution
        BigInteger totalPool = 1000000;
        BigInteger userShare = 25;
        BigInteger totalShares = 100;
        
        BigInteger userAmount = Contract.CalculateProRataShare(totalPool, userShare, totalShares);
        Assert.AreEqual(250000, userAmount); // 25% of pool
        
        // Test edge cases
        Assert.AreEqual(0, Contract.CalculateProRataShare(totalPool, 0, totalShares));
        Assert.AreEqual(totalPool, Contract.CalculateProRataShare(totalPool, totalShares, totalShares));
        
        // Test invalid cases
        Assert.ThrowsException<Exception>(() =>
            Contract.CalculateProRataShare(totalPool, 150, totalShares)); // Share > total
    }
    
    [TestMethod]
    public void TestArithmeticEdgeCases()
    {
        // Test zero operands
        Assert.AreEqual(0, Contract.SafeMultiply(0, 1000));
        Assert.AreEqual(0, Contract.SafeMultiply(1000, 0));
        
        // Test identity operations
        Assert.AreEqual(1000, Contract.SafeMultiply(1000, 1));
        Assert.AreEqual(1000, Contract.SafeDivide(1000, 1));
        
        // Test boundary values
        Assert.AreEqual(1, Contract.SafeSubtract(1, 0));
        Assert.AreEqual(1, Contract.SafeAdd(0, 1));
    }
    
    [TestMethod]
    public void TestCompoundCalculations()
    {
        // Test compound interest calculations
        BigInteger principal = 1000000; // 10.00000 tokens
        int annualRate = 500; // 5% in basis points
        int periods = 2;
        
        BigInteger result = Contract.ApplyCompoundPercentage(principal, annualRate, periods);
        
        // Expected: 10 * 1.05 * 1.05 = 11.025
        BigInteger expected = Contract.ToFixedPoint(1102500, 6); // 11.025000 with 8 decimals
        
        // Allow small rounding differences
        BigInteger difference = result > expected ? result - expected : expected - result;
        Assert.IsTrue(difference <= 100, $"Compound calculation difference too large: {difference}");
    }
}
```

Safe arithmetic operations are fundamental to secure smart contract development. Always validate inputs, check for overflow/underflow conditions, and test edge cases thoroughly to ensure your contracts handle mathematical operations securely and predictably.