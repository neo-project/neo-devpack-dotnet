# Neo N3 Modifier Smart Contract Example

## Overview
This example demonstrates how to implement and use custom modifiers in Neo N3 smart contracts. Modifiers provide a powerful way to enhance method functionality with pre-conditions and post-conditions, improving code reusability, readability, and security by centralizing common validation logic.

## Key Features
- Custom modifier attribute creation
- Access control implementation
- Method decoration with modifiers
- Pre-condition and post-condition handling
- Reusable authorization logic

## Technical Implementation
The `SampleModifier` contract demonstrates two key aspects of Neo N3 modifiers:

### Modifier Definition
The example shows how to:
- Create a custom `OnlyOwnerAttribute` that inherits from `ModifierAttribute`
- Define constructor parameters for configurable modifiers
- Implement `Enter()` method for pre-execution validation
- Implement `Exit()` method for post-execution operations

### Modifier Usage
The contract demonstrates:
- Applying the custom modifier to a contract method
- Passing configuration parameters to the modifier
- Automatic validation before method execution

## How Neo N3 Modifiers Work
Modifiers in Neo N3 follow this execution flow:
1. When a method with a modifier is called, the modifier's `Enter()` method is executed first
2. If the `Enter()` method completes without exceptions, the decorated method executes
3. After the method execution (even if it fails), the modifier's `Exit()` method is called
4. If the `Enter()` method throws an exception, the decorated method is not executed

## Common Modifier Patterns
The example demonstrates the most common modifier pattern:
- **Access Control**: Restricting method access to specific addresses
- **Ownership Validation**: Ensuring the caller has appropriate permissions

Other common patterns that can be implemented with modifiers:
- **Reentrancy Guards**: Preventing recursive calls
- **State Validation**: Ensuring contract is in a valid state for the operation
- **Pause Mechanism**: Enabling/disabling contract functionality

## Security Benefits
Modifiers improve contract security by:
- Centralizing validation logic to avoid repetition
- Enforcing consistent access control
- Preventing execution when conditions aren't met
- Making authorization requirements explicit

## Customization Guide
To adapt this example for your own modifiers:
1. Create a new class that inherits from `ModifierAttribute`
2. Implement the `Enter()` and `Exit()` methods with your logic
3. Add any configuration parameters to the constructor
4. Apply your new modifier to methods using the attribute syntax

## Educational Value
This example teaches:
- How to create and use custom modifiers
- Implementing access control patterns
- C# attribute-based programming
- Pre-condition and post-condition handling
- Code organization for reusable validation logic

Modifiers are a powerful tool for Neo N3 smart contract development, enabling more maintainable, secure, and expressive code by separating cross-cutting concerns from business logic.