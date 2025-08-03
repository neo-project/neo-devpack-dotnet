# Contract Invocation Examples

This directory contains example contracts that demonstrate the contract invocation system for the Neo N3 C# devpack.

## Examples

### 1. MyToken.cs
A simple NEP-17 token contract that demonstrates basic contract structure. This serves as a target contract that could be invoked by other contracts.

**Features:**
- NEP-17 compliant token
- Mint and burn functionality
- Approval system for delegated transfers
- Owner-based access control

### 2. SimpleExample.cs
A basic example demonstrating the contract invocation system concepts.

**Features:**
- References to NEO and GAS token contracts
- Shows how to use `[ContractReference]` attributes
- Basic contract resolution checking
- Demonstrates multi-network address configuration

## Key Concepts Demonstrated

### Contract References
The examples show how to reference other contracts using attributes:

```csharp
[ContractReference("NEO",
    PrivnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
    TestnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
    MainnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
private static IContractReference? NeoContract;
```

### Contract Invocation System Benefits

1. **Type Safety**: Contract references provide compile-time checking
2. **Multi-Network Support**: Different addresses for privnet/testnet/mainnet
3. **Development Integration**: Support for referencing contracts under development
4. **Compiler Integration**: The compiler resolves references and generates appropriate Contract.Call instructions

## Build Instructions

To build these examples:

```bash
dotnet build --configuration Release
```

Note: The build may show warnings related to unsupported interfaces and methods, as the contract invocation system is still under development. These warnings don't prevent successful compilation.

## Usage Notes

- The contract invocation system is designed for compile-time resolution
- IContractReference fields are populated by the compiler during compilation
- The examples demonstrate the intended API, though full functionality depends on compiler implementation
- The SimpleExample contract shows basic usage patterns that can be extended for more complex scenarios