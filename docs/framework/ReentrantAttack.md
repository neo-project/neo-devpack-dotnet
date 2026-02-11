# Neo C# Smart Contract Reentrancy Protection

## Overview

Neo smart contracts written in C# provide a built-in mechanism to prevent reentrancy attacks. This protection is implemented through custom attributes that can be applied to methods within smart contracts. There are two main attributes available:

1. `NoReentrantAttribute`
2. `NoReentrantMethodAttribute`

Both of these attributes help ensure that a method cannot be re-entered while it's still executing, preventing potential vulnerabilities that could be exploited in reentrancy attacks.

## How It Works

### The Concept

Reentrancy protection works by using a flag in contract storage to indicate whether a protected method is currently executing. When a protected method is called, it checks this flag. If the flag is not set, it sets the flag and proceeds with execution. If the flag is already set, the method call is rejected.

### Implementation Details

1. **Storage Context**: Both attributes use a `StorageMap` to store the reentrancy flag. This map is associated with the current storage context of the contract.

2. **Unique Keys**: Each protected method (or group of methods) uses a unique key in the storage map. This allows for fine-grained control over which methods can or cannot be reentered.

3. **Enter and Exit Logic**: The protection is implemented in two parts:
    - `Enter()`: Checks if the method is already executing and throws an exception if it is. Otherwise, it sets the flag to indicate the method is now executing.
    - `Exit()`: Clears the flag after the method finishes executing.

4. **Automatic Application**: The Neo smart contract framework automatically calls `Enter()` before the method execution and `Exit()` after the method completes (or if an exception is thrown).

## Usage

### NoReentrantAttribute

This attribute provides global reentrancy protection. It uses a single key for all methods it's applied to.

```csharp
[NoReentrant]
public static void ProtectedMethod()
{
    // Method implementation
}
```

### NoReentrantMethodAttribute

This attribute provides method-specific reentrancy protection. By default, it uses the method name as the key, allowing for more granular control.

```csharp
[NoReentrantMethod]
public static void SpecificProtectedMethod()
{
    // Method implementation
}
```

## Customization

Both attributes allow for customization:

- You can specify a custom storage prefix (default is 0xFF).
- For `NoReentrantAttribute`, you can specify a custom key.
- For `NoReentrantMethodAttribute`, the key defaults to the method name, but you can override it if needed.

## How It Takes Effect

1. When a protected method is called, the Neo execution engine automatically invokes the `Enter()` method of the attribute.
2. `Enter()` checks the storage map for the presence of the key.
3. If the key is not present, it's added, and the method proceeds.
4. If the key is present, an exception is thrown with the message "Already entered".
5. After the method completes (or if an exception is thrown), the `Exit()` method is automatically called, which removes the key from storage.

- Be mindful of gas costs, as each storage operation consumes GAS.
- Consider the trade-offs between using global (`NoReentrantAttribute`) and method-specific (`NoReentrantMethodAttribute`) protection based on your contract's needs.

By using these attributes, Neo C# smart contract developers can easily add reentrancy protection to their contracts, significantly reducing the risk of reentrancy attacks and improving overall contract security.

## Limitations and Known Risks

### Cross-Contract Reentrancy Is Not Detected

The `[NoReentrant]` and `[NoReentrantMethod]` attributes only protect against reentrancy within a single contract. If Contract A calls Contract B, and Contract B calls back into Contract A through a different entry point that is not protected, the reentrancy will not be detected. The analyzer does not perform cross-contract reentrancy analysis.

Developers must manually audit cross-contract call chains and apply reentrancy guards on all externally callable methods that modify state, not just the methods that initiate outbound calls.

### Unhandled Exceptions May Leave the Lock Permanently Set

The reentrancy lock is stored in contract storage. The `Exit()` method is responsible for clearing this lock after method execution. If an unhandled exception occurs in a code path where `Exit()` is not reached, the lock key remains in storage permanently. This effectively bricks the protected method (or all `[NoReentrant]` methods), as every subsequent call will see the lock and throw "Already entered".

To mitigate this risk:

- Ensure all code paths within protected methods are properly handled.
- Consider implementing an administrative method to manually clear the lock key in emergency scenarios.

### Shared Lock Key for `[NoReentrant]`

All methods decorated with `[NoReentrant]` share a single lock key (`"noReentrant"` by default). This means that if Method A is executing, any call to Method B (also decorated with `[NoReentrant]`) will be rejected, even if Method B is a completely unrelated operation.

This is a global mutex across all `[NoReentrant]` methods in the contract. If you need independent locking for different methods, use `[NoReentrantMethod]` instead.

### `[NoReentrant]` vs `[NoReentrantMethod]`

| Attribute             | Lock Scope                     | Key Used                            | Use Case                                                                    |
| --------------------- | ------------------------------ | ----------------------------------- | --------------------------------------------------------------------------- |
| `[NoReentrant]`       | Global (all decorated methods) | Single shared key (`"noReentrant"`) | When you want to prevent any concurrent execution across multiple methods   |
| `[NoReentrantMethod]` | Per-method                     | Method name (or custom key)         | When methods are independent and should only block re-entry into themselves |

Choose `[NoReentrantMethod]` when your contract has multiple protected methods that do not share state and should be allowed to execute independently. Choose `[NoReentrant]` when all protected methods access shared state and no two should ever run concurrently.
