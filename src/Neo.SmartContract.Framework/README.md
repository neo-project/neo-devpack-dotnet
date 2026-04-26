# Neo.SmartContract.Framework

The foundational framework for developing Neo smart contracts in C#. Provides essential types, attributes, and APIs for blockchain development.

## Features

- **Smart Contract Base**: `SmartContract` base class with core functionality
- **Native Contracts**: Access to Neo native contracts (NeoToken, GasToken, etc.)
- **Cryptographic APIs**: Hashing, encryption, and signature verification
- **Storage APIs**: Persistent storage operations
- **Blockchain Access**: Block and transaction information
- **Supported Standards**: NEP-11, NEP-17, and other standard implementations

## Installation

```bash
dotnet add package Neo.SmartContract.Framework
```

## Quick Start

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace MyContract
{
    [DisplayName("MyToken")]
    [ContractAuthor("Your Name", "you@example.com")]
    [ContractDescription("My NEP-17 Token")]
    [ContractVersion("0.0.1")]
    [SupportedStandards(NepStandard.Nep17)]
    public class MyToken : Nep17Token
    {
        [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
        private static readonly UInt160 Owner = default!;

        public override string Symbol { [Safe] get => "MYT"; }
        public override byte Decimals { [Safe] get => 8; }

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object? data = null)
        {
            // Handle incoming payments
        }

        [Safe]
        public static bool IsOwner() => Runtime.CheckWitness(Owner);
    }
}
```

## Core Components

### Attributes

- `[DisplayName]` - Set contract/method display name
- `[ContractAuthor]`, `[ContractDescription]`, `[ContractVersion]` - Add metadata to contract manifest
- `[SupportedStandards]` - Declare supported NEP standards
- `[Hash160]`, `[InitialValue]` - Initialize static fields
- `[Safe]` - Mark methods as safe (no state changes)

### Native Contracts

- `NeoToken` - NEO token operations
- `GasToken` - GAS token operations
- `ContractManagement` - Contract deployment/management
- `CryptoLib` - Cryptographic operations
- `StdLib` - Standard library functions

### Services

- `Storage` - Key-value persistent storage
- `Runtime` - Contract execution context
- `Blockchain` - Block and transaction access
- `ExecutionEngine` - Script execution utilities

## Documentation

For comprehensive documentation and examples:
https://github.com/neo-project/neo-devpack-dotnet/tree/master-n3/docs

## License

MIT - See [LICENSE](../../LICENSE) for details.
