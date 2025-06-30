# Intermediate Examples

This directory contains examples for developers who have mastered the basics and are ready to explore more complex smart contract patterns.

## 🎯 Learning Objectives

After completing these examples, you will understand:

- Advanced method modifiers and attributes
- Inter-contract communication patterns
- Asset transfer operations
- Complex state management
- Security considerations

## 📚 Examples

### 1. Modifier
**Difficulty**: ⭐⭐⭐☆☆

Learn how to use method modifiers and attributes to control contract behavior.

**Key Concepts**:
- Method attributes ([Safe], [DisplayName], etc.)
- Access control modifiers
- Manifest customization
- Permission management

### 2. ContractCall
**Difficulty**: ⭐⭐⭐☆☆

Master inter-contract communication and building composable smart contracts.

**Key Concepts**:
- Calling other contracts
- Contract interfaces
- Return value handling
- Gas management across calls
- Security considerations

### 3. Transfer
**Difficulty**: ⭐⭐⭐☆☆

Understand how to handle asset transfers safely and efficiently.

**Key Concepts**:
- NEO and GAS transfers
- Transfer notifications
- Balance verification
- Atomic operations
- Security patterns

## 🚀 Prerequisites

Before starting these examples, you should have:

- Completed all beginner examples
- Good understanding of C# and OOP
- Basic knowledge of NEO's economic model
- Familiarity with contract storage and events

## 💡 Best Practices

- Always validate external contract calls
- Consider gas costs for complex operations
- Implement proper access controls
- Test inter-contract scenarios thoroughly
- Handle edge cases and failures

## 🔒 Security Considerations

These examples introduce patterns that require careful security consideration:

- **ContractCall**: Validate return values and handle failures
- **Transfer**: Prevent reentrancy and validate amounts
- **Modifier**: Ensure proper access control implementation

## 📖 Advanced Topics Covered

- Contract composition patterns
- State machine implementations
- Multi-signature scenarios
- Upgrade patterns
- Gas optimization techniques

## 🔗 Next Steps

After completing these examples:
- Explore [Advanced Examples](../03-advanced/README.md)
- Study [Token Standards](../04-token-standards/README.md)
- Review [Security Best Practices](../../docs/security/)