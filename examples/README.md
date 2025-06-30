# NEO Smart Contract Examples

This directory contains a comprehensive collection of smart contract examples organized by difficulty level and use case. Each example includes complete source code, unit tests, and documentation.

## 📚 Organization

Examples are organized into the following categories:

### 🟢 01-beginner
Perfect for developers new to NEO smart contracts. These examples cover fundamental concepts.

- **HelloWorld** - Your first smart contract
- **Storage** - Working with persistent storage
- **Events** - Emitting and handling events
- **Exception** - Error handling patterns

### 🟡 02-intermediate
For developers familiar with basics, ready to explore more complex patterns.

- **Modifier** - Using method modifiers and attributes
- **ContractCall** - Inter-contract communication
- **Transfer** - Asset transfer operations

### 🔴 03-advanced
Advanced patterns and complex implementations.

- **Oracle** - External data integration
- **ZKP** - Zero-knowledge proof implementations
- **Inscription** - On-chain inscription patterns

### 💰 04-token-standards
Standard token implementations following NEO Enhancement Proposals (NEPs).

- **NEP-17** - Fungible token standard
- **NEP-11** - Non-fungible token (NFT) standard
- **RoyaltyNFT** - NFT with royalty features

### 🔧 05-specialized
Specialized use cases and domain-specific examples.

- **DeFi** - Decentralized finance patterns
- **Gaming** - Game-specific contracts
- **DAO** - Decentralized autonomous organizations

## 🚀 Getting Started

### Prerequisites

- .NET SDK 9.0 or later
- NEO DevPack for .NET
- Basic understanding of C# and blockchain concepts

### Running an Example

1. Navigate to the example directory:
   ```bash
   cd 01-beginner/HelloWorld
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run tests:
   ```bash
   cd ../HelloWorld.UnitTests
   dotnet test
   ```

4. Compile to NEO bytecode:
   ```bash
   dotnet run --project ../../../src/Neo.Compiler.CSharp -- HelloWorld.csproj
   ```

## 📖 Learning Path

We recommend following this learning path:

1. **Start with Beginner Examples**
   - HelloWorld → Storage → Events → Exception

2. **Progress to Intermediate**
   - Modifier → ContractCall → Transfer

3. **Explore Token Standards**
   - NEP-17 (if interested in fungible tokens)
   - NEP-11 (if interested in NFTs)

4. **Dive into Advanced Topics**
   - Oracle (for external data)
   - ZKP (for privacy features)

5. **Study Specialized Examples**
   - Based on your specific use case

## 🧪 Testing

Every example includes comprehensive unit tests demonstrating:

- Contract deployment
- Method invocation
- State verification
- Error scenarios
- Gas consumption analysis

Run all example tests:
```bash
dotnet test neo-contract-examples.sln
```

## 📝 Example Structure

Each example follows a consistent structure:

```
ExampleName/
├── ExampleName.cs          # Contract source code
├── ExampleName.csproj      # Project file
├── README.md              # Detailed documentation
└── ExampleName.UnitTests/ # Unit tests
    ├── ExampleNameTests.cs
    └── TestingArtifacts/  # Compiled artifacts
```

## 🔍 Finding the Right Example

### By Feature

- **Storage Operations**: Storage, NEP-17, NEP-11
- **Events & Notifications**: Events, Transfer, NEP-17
- **Access Control**: Modifier, Oracle, NEP-11
- **Token Operations**: NEP-17, NEP-11, RoyaltyNFT
- **External Data**: Oracle
- **Advanced Cryptography**: ZKP

### By Use Case

- **Creating a Token**: NEP-17
- **Creating NFTs**: NEP-11, RoyaltyNFT
- **Building a DApp**: ContractCall, Oracle
- **Managing Permissions**: Modifier
- **Handling Errors**: Exception

## 💡 Best Practices

When studying these examples:

1. **Read the README** - Each example has detailed documentation
2. **Study the Tests** - Tests demonstrate proper usage
3. **Check Gas Usage** - Understand the cost implications
4. **Review Security** - Note security patterns used
5. **Experiment** - Modify and extend examples

## 🤝 Contributing

Have an example to contribute? Please ensure it:

1. Follows the existing structure
2. Includes comprehensive tests
3. Has clear documentation
4. Demonstrates best practices
5. Adds educational value

## 📚 Additional Resources

- [NEO Documentation](https://docs.neo.org/)
- [Getting Started Guide](../docs/getting-started.md)
- [Security Best Practices](../docs/security/)
- [NEO Discord Community](https://discord.gg/rvZFQ5382k)

## 🔒 Security Notice

These examples are for educational purposes. Always:

- Audit contracts before mainnet deployment
- Follow security best practices
- Test thoroughly on testnet first
- Consider edge cases and attack vectors

---

Happy learning! If you have questions, join our [Discord community](https://discord.gg/rvZFQ5382k).