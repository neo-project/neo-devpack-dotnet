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

### Role-based pausing

Use `PausableOwnable` for a single owner's pause authority, or `PausableAccessControl`
when multiple accounts need permission to pause and unpause. These are alternative
authorization models; role-based pausing does not replace the owner-based model.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public class EmergencyContract : PausableAccessControl
{
    public static void _deploy(object data, bool update)
    {
        InitializePausableAccessControl((UInt160?)data, update);
    }

    [WhenNotPaused]
    public static bool DoWork() => true;

    [WhenPaused]
    public static bool InspectWhilePaused() => true;
}
```

On initial deployment, pass an administrator's script hash as deployment data, or
null to use the transaction sender. The initializer grants that account both
`DEFAULT_ADMIN_ROLE()` (0) and `PAUSER_ROLE()` (1). Other roles must use distinct
identifiers: reusing 1 also grants pause authority.

The inherited `GrantRole(PAUSER_ROLE(), admin, pauser)` and
`RevokeRole(PAUSER_ROLE(), admin, pauser)` manage pauser assignments. The acting
administrator must hold the role's admin role and witness the invocation.
`Pause(pauser)` and `Unpause(pauser)` likewise require both the pauser role and a
valid witness for the supplied account; knowing a pauser's address is insufficient.
Successful transitions emit `Paused` or `Unpaused` with the acting account.

The pause flag uses storage key `0xFE`, matching the default pause modifiers, while
access-control storage uses `0xFB`. Do not reuse those prefixes for application storage.
The modifiers only protect annotated methods; apply other authorization checks to
sensitive application operations as appropriate.

Always forward the deployment callback's `update` argument to the initializer.
When true, initialization is a no-op: it preserves the administrator, role assignments
and pause state without regranting revoked roles. Contracts migrating from a different
base class need their own authorized migration policy; the update path does not
bootstrap missing roles.

With the default role-admin policy, if every pauser has been revoked, a default
administrator can still grant `PAUSER_ROLE()` to itself or another account, then
unpause with that account's witness. If a derived contract changes the pauser role's
admin role, it must preserve an authorized recovery path.

## Documentation

For comprehensive documentation and examples:
https://github.com/neo-project/neo-devpack-dotnet/tree/master-n3/docs

## License

MIT - See [LICENSE](../../LICENSE) for details.
