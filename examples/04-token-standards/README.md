# Token Standards Examples

This directory contains implementations of NEO Enhancement Proposals (NEPs) for token standards. These are essential patterns for creating fungible and non-fungible tokens on NEO.

## 🎯 Learning Objectives

After completing these examples, you will understand:

- NEP-17 fungible token standard
- NEP-11 non-fungible token (NFT) standard
- Token metadata and properties
- Transfer mechanics and events
- Advanced token features (royalties, etc.)

## 📚 Examples

### 1. NEP-17 (Fungible Token)
**Difficulty**: ⭐⭐⭐☆☆

Complete implementation of the NEP-17 standard for fungible tokens (like ERC-20 on Ethereum).

**Key Features**:
- Token minting and burning
- Transfer operations
- Balance tracking
- Allowance mechanism
- Standard events

**Use Cases**:
- Utility tokens
- Stablecoins
- Governance tokens
- Reward tokens

### 2. NEP-11 (Non-Fungible Token)
**Difficulty**: ⭐⭐⭐⭐☆

Full implementation of the NEP-11 standard for non-fungible tokens (NFTs).

**Key Features**:
- Unique token minting
- Ownership tracking
- Token properties/metadata
- Enumeration support
- Transfer mechanics

**Use Cases**:
- Digital art
- Gaming items
- Real estate tokens
- Identity tokens

### 3. Royalty NFT
**Difficulty**: ⭐⭐⭐⭐☆

Extended NEP-11 implementation with built-in royalty support.

**Key Features**:
- Royalty calculation
- Automatic royalty distribution
- Secondary market support
- Creator attribution
- Flexible royalty rates

**Use Cases**:
- Art marketplaces
- Music NFTs
- Content licensing
- Creator economies

## 📋 Standards Compliance

All implementations strictly follow the respective NEP standards:

- [NEP-17 Specification](https://github.com/neo-project/proposals/blob/master/nep-17.mediawiki)
- [NEP-11 Specification](https://github.com/neo-project/proposals/blob/master/nep-11.mediawiki)

## 🚀 Implementation Guide

### Creating a Fungible Token (NEP-17)

1. Inherit from `Nep17Token` base class
2. Define token properties (name, symbol, decimals)
3. Implement initial distribution
4. Add custom features as needed

### Creating an NFT (NEP-11)

1. Inherit from `Nep11Token<T>` base class
2. Define token properties structure
3. Implement minting logic
4. Add metadata management

## 💡 Best Practices

### Token Design
- Choose appropriate decimal places
- Plan initial distribution carefully
- Consider upgrade mechanisms
- Implement proper access controls

### NFT Considerations
- Design efficient metadata storage
- Plan for enumeration queries
- Consider on-chain vs off-chain storage
- Implement proper royalty mechanisms

## 🔒 Security Considerations

### Common Vulnerabilities
- Integer overflow in transfers
- Reentrancy in complex operations
- Improper access control
- Missing event emissions

### Mitigation Strategies
- Use safe math operations
- Implement checks-effects-interactions
- Validate all inputs
- Follow standard interfaces exactly

## 📊 Gas Optimization

### NEP-17 Tips
- Batch operations when possible
- Optimize storage layout
- Consider lazy initialization

### NEP-11 Tips
- Efficient property storage
- Optimize enumeration
- Consider pagination

## 🧪 Testing Strategies

Each token implementation includes tests for:
- Standard compliance
- Edge cases
- Gas consumption
- Security scenarios
- Upgrade paths

## 🔗 Integration

### Wallet Integration
- Follow standard interfaces
- Emit required events
- Provide proper metadata

### Exchange Integration
- Implement standard methods
- Ensure decimal handling
- Support balance queries

## 📚 Additional Resources

- [NEO Token Standards](https://docs.neo.org/docs/n3/nep)
- [Token Design Patterns](../../docs/token-patterns.md)
- [Security Checklist](../../docs/security/token-security.md)

## 🎓 Learning Path

1. Start with NEP-17 for fungible tokens
2. Progress to NEP-11 for NFTs
3. Explore Royalty NFT for advanced features
4. Combine patterns for complex use cases

## 🔗 Next Steps

After mastering token standards:
- Build DeFi protocols using NEP-17
- Create NFT marketplaces with NEP-11
- Design custom token economics
- Explore [Specialized Examples](../05-specialized/README.md)