# Common Neo Smart Contract Vulnerabilities

This document outlines the most common security vulnerabilities found in Neo smart contracts and provides specific guidance on how to prevent them.

## Table of Contents

- [Reentrancy Attacks](#reentrancy-attacks)
- [Integer Overflow/Underflow](#integer-overflowunderflow)
- [Access Control Bypass](#access-control-bypass)
- [Unchecked External Calls](#unchecked-external-calls)
- [Storage Manipulation](#storage-manipulation)
- [Gas Limit Issues](#gas-limit-issues)
- [Timestamp Dependencies](#timestamp-dependencies)
- [Front-Running Attacks](#front-running-attacks)

## Reentrancy Attacks

Reentrancy occurs when external contracts call back into your contract before state updates complete.

### Key Vulnerability
- External calls before state changes
- Recursive function calls
- State inconsistency

### Prevention Strategies
1. **Checks-Effects-Interactions Pattern**: Update state before external calls
2. **Reentrancy Guards**: Use locks to prevent recursive calls
3. **Pull Over Push**: Let users withdraw rather than automatically sending

> **Implementation**: See [Security Best Practices](security-best-practices.md#reentrancy-protection) for complete code examples.

## Integer Overflow/Underflow

While Neo uses `BigInteger`, you must still validate ranges and prevent unexpected arithmetic operations.

### Key Vulnerabilities
- Unchecked arithmetic operations
- Missing range validation
- Business logic bypass through overflow

### Prevention Strategies
1. **Range Validation**: Set maximum values for business logic
2. **Safe Arithmetic**: Check for overflow before operations
3. **Input Validation**: Validate all numeric inputs

> **Implementation**: See [Safe Arithmetic Operations](safe-arithmetic.md) for comprehensive safe math patterns.

## Access Control Bypass

Improper access control allows unauthorized users to perform privileged operations.

### Key Vulnerabilities
- Using `Runtime.ScriptContainer.Sender` instead of `Runtime.CheckWitness()`
- Missing role validation
- Insufficient multi-signature verification
- Weak permission hierarchies

### Prevention Strategies
1. **Always use `Runtime.CheckWitness()`** for cryptographic verification
2. **Implement proper role-based access control**
3. **Use multi-signature for critical operations**
4. **Validate all authorization parameters**

> **Implementation**: See [Access Control Patterns](access-control-patterns.md) for comprehensive RBAC and multi-sig implementations.

## Unchecked External Calls

External contract calls can fail or behave unexpectedly without proper error handling.

### Key Vulnerabilities
- Assuming external calls always succeed
- Not validating call results
- Missing exception handling
- Continuing execution after failures

### Prevention Strategies
1. **Always check return values** from external calls
2. **Implement proper exception handling**
3. **Use contract whitelisting** for trusted interactions
4. **Validate all inputs** before external calls

> **Implementation**: See [Security Best Practices](security-best-practices.md#safe-contract-calls) for complete external call patterns.

## Storage Manipulation

Improper storage handling can lead to data corruption or unauthorized access.

### Key Vulnerabilities
- Direct key usage enabling collision attacks
- Missing access control on sensitive data
- Bulk operations without proper validation
- Lack of data isolation and namespacing

### Prevention Strategies
1. **Use namespaced storage keys** to prevent collisions
2. **Implement access control** for sensitive data
3. **Validate all storage operations** with proper authorization
4. **Use storage contexts** for data isolation

> **Implementation**: See [Storage Security](storage-security.md) for comprehensive secure storage patterns.

## Gas Limit Issues

Poorly designed contracts can consume excessive gas or hit limits, causing DoS.

### Key Vulnerabilities
- Unbounded loops and iterations
- Unlimited array/storage operations
- Complex operations without batching
- Missing gas consumption limits

### Prevention Strategies
1. **Implement batching** for large operations
2. **Set maximum limits** on array sizes and iterations
3. **Use pagination** for data retrieval
4. **Optimize algorithm complexity**

> **Implementation**: See [Gas Security](gas-security.md) for detailed gas optimization and DoS prevention patterns.

## Timestamp Dependencies

Using block timestamps for critical logic can introduce vulnerabilities due to miner manipulation.

### Key Vulnerabilities
- Direct timestamp comparisons for critical logic
- Weak randomness using timestamps
- Time-sensitive operations without tolerance

### Prevention Strategies
1. **Use time windows** instead of exact timestamps
2. **Implement tolerance ranges** for time-sensitive operations
3. **Avoid timestamps for randomness**
4. **Use block numbers** for rough time estimation

## Front-Running Attacks

Transaction ordering manipulation can allow attackers to profit from observing pending transactions.

### Key Vulnerabilities
- Predictable transaction outcomes
- Price-sensitive operations without protection
- Public transaction data exploitation

### Prevention Strategies
1. **Use commit-reveal schemes** for sensitive operations
2. **Implement transaction ordering protection**
3. **Add randomization** to reduce predictability
4. **Use batch processing** to reduce front-running opportunities

## Prevention Summary

### Security Checklist
- ✅ Validate all inputs and access controls
- ✅ Use proper state management patterns
- ✅ Implement comprehensive error handling
- ✅ Apply gas optimization with security in mind
- ✅ Test all vulnerability scenarios
- ✅ Conduct regular security audits

> **Next Steps**: Review specific implementation patterns in the dedicated security guides linked above.