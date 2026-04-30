# Safe Arithmetic Operations in Neo Smart Contracts

Arithmetic operations in smart contracts require special attention to prevent integer overflow, underflow, and other mathematical vulnerabilities.
This guide provides comprehensive patterns for safe arithmetic in Neo N3 smart contracts.

## Table of Contents

- [Arithmetic Security Fundamentals](#arithmetic-security-fundamentals)
- [Neo N3 BigInteger Characteristics](#neo-n3-biginteger-characteristics)
- [Safe Addition Operations](#safe-addition-operations)
- [Safe Subtraction Operations](#safe-subtraction-operations)
- [Safe Multiplication Operations](#safe-multiplication-operations)
- [Safe Division Operations](#safe-division-operations)
- [Percentage and Ratio Calculations](#percentage-and-ratio-calculations)
- [Fixed-Point Arithmetic](#fixed-point-arithmetic)

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
    private static readonly BigInteger MAX_SAFE_VALUE = BigInteger.Parse("79228162514264337593543950335"); // An example
    
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
        ExecutionEngine.Assert(result <= MAX_SAFE_VALUE, "Result exceeds safe limits");
    }
}
```

## Safe Addition Operations

### Basic Safe Addition

```csharp
public class SafeAddition : SmartContract
{
    /// <summary>
    /// Safe addition with input validation and business logic validation.
    /// 
    /// See also SafeMath.UnsignedAdd(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static BigInteger SafeAdd(BigInteger a, BigInteger b)
    {
        // Input validation
        ExecutionEngine.Assert(a >= 0, "First operand must be non-negative");
        ExecutionEngine.Assert(b >= 0, "Second operand must be non-negative");
        
        // Check against business logic limits (VM handles 256-bit overflow automatically)
        ExecutionEngine.Assert(a <= MAX_SAFE_VALUE - b, "Addition would exceed business logic limits");
        
        BigInteger result = a + b;
        
        // Post-condition check
        ExecutionEngine.Assert(result >= a && result >= b, "Addition result validation failed");
        
        return result;
    }
    
    /// <summary>
    /// Safe addition for token amounts with specific limits
    /// 
    /// See also SafeMath.UnsignedAdd(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static BigInteger SafeAddTokens(BigInteger currentBalance, BigInteger amount)
    {
        ExecutionEngine.Assert(currentBalance >= 0, "Current balance cannot be negative");
        ExecutionEngine.Assert(amount > 0, "Amount must be positive");
        ExecutionEngine.Assert(amount <= MAX_TOKEN_AMOUNT, "Amount exceeds maximum token limit");
        
        // Check total won't exceed maximum supply
        ExecutionEngine.Assert(currentBalance <= MAX_TOKEN_AMOUNT - amount, "Addition would exceed maximum token supply");
        
        return currentBalance + amount;
    }
    
    /// <summary>
    /// Batch safe addition with cumulative overflow protection
    /// </summary>
    public static BigInteger SafeAddBatch(BigInteger[] values)
    {
        // Reasonable limit according to business logic
        ExecutionEngine.Assert(values != null && values.Length > 0, "Values array required");
        ExecutionEngine.Assert(values.Length <= 100, "Too many values in batch");

        BigInteger total = 0;
        foreach (BigInteger value in values)
        {
            ExecutionEngine.Assert(value >= 0, "All values must be non-negative");
            total = SafeAdd(total, value);
        }
        return total;
    }
    
    /// <summary>
    /// Safe addition with percentage bounds checking
    /// </summary>
    public static BigInteger SafeAddWithPercentageLimit(BigInteger base_, BigInteger addition, int maxPercentageIncrease)
    {
        ExecutionEngine.Assert(base_ > 0, "Base value must be positive");
        ExecutionEngine.Assert(addition >= 0, "Addition must be non-negative");
        ExecutionEngine.Assert(maxPercentageIncrease > 0 && maxPercentageIncrease <= 10000, "Invalid percentage limit"); // 100.00%

        // Calculate maximum allowed addition (as percentage of base)
        BigInteger maxAddition = (base_ * maxPercentageIncrease) / 10000;
        ExecutionEngine.Assert(addition <= maxAddition, $"Addition exceeds {maxPercentageIncrease / 100}% limit");

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
    /// Safe subtraction with underflow protection.
    /// See also SafeMath.UnsignedSub(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static BigInteger SafeSubtract(BigInteger a, BigInteger b)
    {
        ExecutionEngine.Assert(a >= 0, "Minuend must be non-negative");
        ExecutionEngine.Assert(b >= 0, "Subtrahend must be non-negative");
        ExecutionEngine.Assert(a >= b, "Subtraction would cause underflow");
        
        BigInteger result = a - b;
        
        // Post-condition validation
        ExecutionEngine.Assert(result >= 0, "Result must be non-negative");
        ExecutionEngine.Assert(result <= a, "Result must not exceed original value");
        
        return result;
    }
    
    /// <summary>
    /// Safe subtraction for balance operations
    /// 
    /// See also SafeMath.UnsignedSub(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static BigInteger SafeSubtractBalance(BigInteger currentBalance, BigInteger amount)
    {
        ExecutionEngine.Assert(currentBalance >= 0, "Current balance cannot be negative");
        ExecutionEngine.Assert(amount > 0, "Amount must be positive");
        ExecutionEngine.Assert(currentBalance >= amount, "Insufficient balance");
        
        BigInteger newBalance = SafeMath.UnsignedSub(currentBalance, amount);

        // Ensure balance integrity
        ExecutionEngine.Assert(newBalance >= 0, "New balance cannot be negative");

        return newBalance;
    }
    
    /// <summary>
    /// Safe subtraction with minimum threshold
    /// </summary>
    public static BigInteger SafeSubtractWithMinimum(BigInteger current, BigInteger amount, BigInteger minimumRequired)
    {
        ExecutionEngine.Assert(current >= minimumRequired, "Current value below minimum");
        ExecutionEngine.Assert(amount > 0, "Amount must be positive");

        BigInteger remaining = SafeSubtract(current, amount);
        ExecutionEngine.Assert(remaining >= minimumRequired, "Operation would leave value below minimum threshold");

        return remaining;
    }
    
    /// <summary>
    /// Batch subtraction with atomic operation guarantee
    /// </summary>
    public static BigInteger SafeSubtractBatch(BigInteger initial, BigInteger[] amounts)
    {
        ExecutionEngine.Assert(amounts != null && amounts.Length > 0, "Amounts array required");
        ExecutionEngine.Assert(amounts.Length <= 50, "Too many amounts in batch");
        
        // Pre-validate that total subtraction is possible
        BigInteger totalToSubtract = SafeAddBatch(amounts);
        ExecutionEngine.Assert(initial >= totalToSubtract, "Insufficient value for batch subtraction");

        return SafeSubtract(initial, totalToSubtract);
    }
    
    /// <summary>
    /// Safe percentage deduction
    /// </summary>
    public static BigInteger SafeSubtractPercentage(BigInteger amount, int percentageToDeduct)
    {
        ExecutionEngine.Assert(amount >= 0, "Amount must be non-negative");
        ExecutionEngine.Assert(percentageToDeduct >= 0 && percentageToDeduct <= 10000, "Invalid percentage"); // 0-100.00%

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
    /// The Neo-VM will automatically abort the execution if the result overflows beyond the 256-bit limit,
    /// 
    /// See also SafeMath.UnsignedMul(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static BigInteger SafeMultiply(BigInteger a, BigInteger b)
    {
        ExecutionEngine.Assert(a >= 0 && b >= 0, "Both operands must be non-negative");
        
        // Special cases
        if (a == 0 || b == 0) return 0;
        if (a == 1) return b;
        if (b == 1) return a;

        // Check against business logic limits (VM handles 256-bit overflow automatically)
        ExecutionEngine.Assert(a <= MAX_SAFE_VALUE / b, "Multiplication would exceed business logic limits");

        BigInteger result = a * b;

        // Verify result integrity
        ExecutionEngine.Assert((result / a) == b, "Multiplication result verification failed");
        
        return result;
    }
    
    /// <summary>
    /// Safe multiplication for financial calculations
    /// </summary>
    public static BigInteger SafeMultiplyFinancial(BigInteger principal, BigInteger rate, BigInteger timePeriods)
    {
        ExecutionEngine.Assert(principal > 0, "Principal must be positive");
        ExecutionEngine.Assert(rate >= 0, "Rate must be non-negative");
        ExecutionEngine.Assert(timePeriods >= 0, "Time periods must be non-negative");

        // Perform multiplication in safe order (smallest first)
        BigInteger temp = SafeMultiply(rate, timePeriods);
        return SafeMultiply(principal, temp);
    }
    
    /// <summary>
    /// Safe power operation (exponentiation)
    /// </summary>
    public static BigInteger SafePower(BigInteger base_, int exponent)
    {
        // Some reasonable limits according to business logic
        ExecutionEngine.Assert(base_ >= 0, "Base must be non-negative");
        ExecutionEngine.Assert(exponent >= 0, "Exponent must be non-negative");
        ExecutionEngine.Assert(exponent <= 64, "Exponent too large");
        
        // The Neo-VM POW instruction will automatically abort the execution 
        // if the result overflows beyond the 256-bit limit or the exponent is less than -1.
        // Note: The exponent value range is different from C# standard library.
        return BigInteger.Pow(base_, exponent);
    }
    
    /// <summary>
    /// Safe scaling multiplication for decimal-like operations
    /// </summary>
    public static BigInteger SafeScale(BigInteger amount, BigInteger numerator, BigInteger denominator)
    {
        ExecutionEngine.Assert(amount >= 0, "Amount must be non-negative");
        ExecutionEngine.Assert(numerator >= 0, "Numerator must be non-negative");
        ExecutionEngine.Assert(denominator > 0, "Denominator must be positive");

        // Handle zero cases
        if (amount == 0 || numerator == 0) return 0;

        // Check for potential overflow in multiplication
        ExecutionEngine.Assert(amount <= MAX_SAFE_VALUE / numerator, "Scaling would cause overflow");
        
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
    /// 
    /// See also SafeMath.UnsignedDiv(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static BigInteger SafeDivide(BigInteger dividend, BigInteger divisor)
    {
        BigInteger result = SafeMath.UnsignedDiv(dividend, divisor);
        
        // Verify division integrity
        ExecutionEngine.Assert(result * divisor <= dividend, "Division result verification failed");
        ExecutionEngine.Assert((result + 1) * divisor > dividend, "Division precision verification failed");
        
        return result;
    }
    
    /// <summary>
    /// Safe division with remainder information
    /// 
    /// See also SafeMath.UnsignedDiv(a, b), SafeMath.UnsignedMod(a, b) in Neo.SmartContract.Framework
    /// </summary>
    public static (BigInteger quotient, BigInteger remainder) SafeDivideWithRemainder(
        BigInteger dividend, BigInteger divisor)
    {
        ExecutionEngine.Assert(dividend >= 0, "Dividend must be non-negative");
        ExecutionEngine.Assert(divisor > 0, "Divisor must be positive");
        
        BigInteger quotient = dividend / divisor;
        BigInteger remainder = dividend % divisor;
        
        // Verify division identity: dividend = quotient * divisor + remainder
        ExecutionEngine.Assert(dividend == quotient * divisor + remainder, "Division identity verification failed");
        ExecutionEngine.Assert(remainder < divisor, "Remainder must be less than divisor");
        
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
        ExecutionEngine.Assert(amount >= 0, "Amount must be non-negative");
        ExecutionEngine.Assert(percentage >= 0 && percentage <= 10000, "Percentage must be 0-10000 (0-100.00%)");
        
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
        ExecutionEngine.Assert(totalAmount >= 0, "Total amount must be non-negative");
        ExecutionEngine.Assert(weights != null && weights.Length > 0, "Weights array required");
        ExecutionEngine.Assert(weights.Length <= 100, "Too many distribution targets");
        
        // Validate all weights are positive
        BigInteger totalWeight = 0;
        foreach (BigInteger weight in weights)
        {
            ExecutionEngine.Assert(weight > 0, "All weights must be positive");
            totalWeight = SafeAdd(totalWeight, weight);
        }
        
        ExecutionEngine.Assert(totalWeight > 0, "Total weight must be positive");
        
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
        ExecutionEngine.Assert(verifyTotal == totalAmount, "Distribution total mismatch");
        
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
        ExecutionEngine.Assert(amount >= 0, "Amount must be non-negative");
        ExecutionEngine.Assert(basisPoints >= 0 && basisPoints <= BASIS_POINTS_SCALE, "Basis points must be 0-10000");
        
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
        ExecutionEngine.Assert(principal > 0, "Principal must be positive");
        ExecutionEngine.Assert(annualBasisPoints >= 0, "Interest rate must be non-negative");
        ExecutionEngine.Assert(periods >= 0 && periods <= 100, "Invalid number of periods");

        if (periods == 0 || annualBasisPoints == 0) return principal;

        BigInteger current = principal;
        for (int i = 0; i < periods; i++)
        {
            BigInteger interest = CalculatePercentageBasisPoints(current, annualBasisPoints);
            current = SafeAdd(current, interest);

            // Prevent runaway growth
            ExecutionEngine.Assert(current <= principal * 1000, "Compound growth exceeds safety limits");
        }
        
        return current;
    }
    
    /// <summary>
    /// Calculate weighted average with precision
    /// </summary>
    public static BigInteger CalculateWeightedAverage(BigInteger[] values, BigInteger[] weights)
    {
        ExecutionEngine.Assert(values != null && weights != null, "Arrays cannot be null");
        ExecutionEngine.Assert(values.Length == weights.Length, "Arrays must have equal length");
        ExecutionEngine.Assert(values.Length > 0, "Arrays cannot be empty");
        
        BigInteger weightedSum = 0;
        BigInteger totalWeight = 0;
        for (int i = 0; i < values.Length; i++)
        {
            ExecutionEngine.Assert(values[i] >= 0, $"Value at index {i} must be non-negative");
            ExecutionEngine.Assert(weights[i] > 0, $"Weight at index {i} must be positive");
            
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
        ExecutionEngine.Assert(totalPool >= 0, "Total pool must be non-negative");
        ExecutionEngine.Assert(userShare >= 0, "User share must be non-negative");
        ExecutionEngine.Assert(totalShares > 0, "Total shares must be positive");
        ExecutionEngine.Assert(userShare <= totalShares, "User share cannot exceed total shares");

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
        ExecutionEngine.Assert(value >= 0, "Value must be non-negative");
        ExecutionEngine.Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");

        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        return SafeMultiply(value, scale);
    }
    
    /// <summary>
    /// Convert fixed-point to integer (truncating)
    /// </summary>
    public static BigInteger FromFixedPoint(BigInteger fixedValue, int decimals = DECIMALS_8)
    {
        ExecutionEngine.Assert(fixedValue >= 0, "Fixed value must be non-negative");
        ExecutionEngine.Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");

        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        return SafeDivide(fixedValue, scale);
    }
    
    /// <summary>
    /// Add two fixed-point numbers
    /// </summary>
    public static BigInteger AddFixedPoint(BigInteger a, BigInteger b, int decimals = DECIMALS_8)
    {
        ExecutionEngine.Assert(a >= 0 && b >= 0, "Fixed-point values must be non-negative");
        return SafeAdd(a, b);
    }
    
    /// <summary>
    /// Multiply two fixed-point numbers
    /// </summary>
    public static BigInteger MultiplyFixedPoint(BigInteger a, BigInteger b, int decimals = DECIMALS_8)
    {
        ExecutionEngine.Assert(a >= 0 && b >= 0, "Fixed-point values must be non-negative");
        ExecutionEngine.Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");
        
        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        BigInteger product = SafeMultiply(a, b);
        return SafeDivide(product, scale);
    }
    
    /// <summary>
    /// Divide two fixed-point numbers
    /// </summary>
    public static BigInteger DivideFixedPoint(BigInteger a, BigInteger b, int decimals = DECIMALS_8)
    {
        ExecutionEngine.Assert(a >= 0, "Dividend must be non-negative");
        ExecutionEngine.Assert(b > 0, "Divisor must be positive");
        ExecutionEngine.Assert(decimals == DECIMALS_8 || decimals == DECIMALS_18, "Unsupported decimal precision");

        BigInteger scale = decimals == DECIMALS_8 ? SCALE_8 : SCALE_18;
        BigInteger scaledDividend = SafeMultiply(a, scale);
        return SafeDivide(scaledDividend, b);
    }
    

    public static BigInteger SqrtFixedPoint(BigInteger fixedValue)
    {
        // Check the `fixedValue` if necessary.
        // The Neo-VM built-in `SQRT` instruction will automatically abort the execution if the `fixedValue` is negative.
        return fixedValue.Sqrt();
    }
}
```
