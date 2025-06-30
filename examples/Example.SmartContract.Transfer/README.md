# NEO Smart Contract - Transfer Example

This example demonstrates secure asset transfer patterns and operations within NEO smart contracts.

## Overview

The Transfer smart contract showcases how to safely handle asset transfers, including NEO, GAS, and NEP-17 tokens. It demonstrates best practices for transfer validation, security checks, and event handling.

## Features

- **Multi-Asset Support**: Handle NEO, GAS, and NEP-17 token transfers
- **Security Validation**: Comprehensive checks for transfer safety
- **Event Notifications**: Proper event emission for transfer tracking
- **Error Handling**: Robust error handling for failed transfers
- **Atomic Operations**: Ensure transfer operations are atomic

## Key Concepts Demonstrated

- Asset transfer mechanics
- Transfer validation and security
- Event-driven notifications
- Error handling patterns
- Atomic transaction design

## Contract Methods

### Transfer Operations

- `TransferNEO(from, to, amount)` - Transfer NEO tokens
- `TransferGAS(from, to, amount)` - Transfer GAS tokens
- `TransferNEP17(token, from, to, amount)` - Transfer NEP-17 tokens
- `BatchTransfer(transfers)` - Execute multiple transfers atomically

### Validation Methods

- `ValidateTransfer(from, to, amount)` - Validate transfer parameters
- `CheckBalance(account, asset)` - Check account balance
- `VerifySignature(account)` - Verify transaction signature

### Query Methods

- `GetTransferHistory(account)` - Get transfer history for account
- `GetTotalTransferred(asset)` - Get total amount transferred for asset

## Usage Examples

### Basic NEO Transfer

```csharp
// Transfer 100 NEO from sender to recipient
var success = TransferNEO(senderAddress, recipientAddress, 100_00000000);
if (success)
{
    Runtime.Log("NEO transfer completed successfully");
}
```

### NEP-17 Token Transfer

```csharp
// Transfer custom tokens
var tokenContract = UInt160.Parse("0x1234567890abcdef1234567890abcdef12345678");
var amount = 1000_00000000; // 1000 tokens with 8 decimals

var result = TransferNEP17(tokenContract, from, to, amount);
```

### Batch Transfer Operations

```csharp
// Execute multiple transfers atomically
var transfers = new List<TransferInfo>
{
    new TransferInfo { Asset = NEO.Hash, From = sender1, To = recipient1, Amount = 50 },
    new TransferInfo { Asset = GAS.Hash, From = sender2, To = recipient2, Amount = 100 }
};

var allSuccessful = BatchTransfer(transfers);
```

## Security Features

### Transfer Validation
- **Amount Validation**: Ensure positive, non-zero amounts
- **Address Validation**: Verify valid sender and recipient addresses
- **Balance Checks**: Confirm sufficient balance before transfer
- **Authorization**: Verify transaction signatures and permissions

### Atomic Operations
```csharp
public static bool SafeTransfer(UInt160 from, UInt160 to, BigInteger amount)
{
    // Validate inputs
    if (!ValidateTransfer(from, to, amount))
        return false;
    
    // Check balance
    if (GetBalance(from) < amount)
        return false;
    
    // Execute transfer atomically
    try
    {
        UpdateBalance(from, -amount);
        UpdateBalance(to, amount);
        EmitTransferEvent(from, to, amount);
        return true;
    }
    catch
    {
        // Revert changes on failure
        return false;
    }
}
```

### Reentrancy Protection
- **State checks**: Verify state before and after operations
- **Mutex patterns**: Prevent concurrent access to critical sections
- **Event ordering**: Ensure proper event emission sequence

## Event Handling

The contract emits comprehensive events for transfer tracking:

```csharp
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

[DisplayName("TransferBatch")]
public static event Action<ByteString> OnTransferBatch;

[DisplayName("TransferFailed")]
public static event Action<UInt160, UInt160, BigInteger, string> OnTransferFailed;
```

## Error Handling Patterns

### Graceful Failure Handling
```csharp
public static bool TryTransfer(UInt160 from, UInt160 to, BigInteger amount, out string error)
{
    error = "";
    
    if (!from.IsValid)
    {
        error = "Invalid sender address";
        return false;
    }
    
    if (!to.IsValid)
    {
        error = "Invalid recipient address";
        return false;
    }
    
    if (amount <= 0)
    {
        error = "Amount must be positive";
        return false;
    }
    
    // Execute transfer
    return ExecuteTransfer(from, to, amount);
}
```

## Gas Optimization

### Efficient Transfer Patterns
- **Batch Operations**: Group multiple transfers to reduce gas costs
- **Storage Optimization**: Minimize storage reads and writes
- **Event Optimization**: Emit only necessary events
- **Computation Reduction**: Optimize validation logic

## Testing Scenarios

The example includes tests for:

- **Happy Path**: Successful transfers with valid parameters
- **Error Cases**: Invalid addresses, insufficient balances, unauthorized access
- **Edge Cases**: Zero amounts, self-transfers, maximum values
- **Security Tests**: Reentrancy attacks, signature validation
- **Gas Analysis**: Transfer cost optimization

Run tests with:
```bash
cd Example.SmartContract.Transfer.UnitTests
dotnet test
```

## Integration Patterns

### With Wallets
```csharp
// Integration with multi-signature wallets
public static bool MultiSigTransfer(UInt160[] signers, UInt160 to, BigInteger amount)
{
    // Verify required signatures
    if (!VerifyMultiSig(signers))
        return false;
    
    return TransferFromMultiSig(signers, to, amount);
}
```

### With DEX/AMM
```csharp
// Integration with decentralized exchanges
public static bool SwapTransfer(UInt160 tokenA, UInt160 tokenB, BigInteger amountA)
{
    // Calculate swap amount
    var amountB = CalculateSwapAmount(tokenA, tokenB, amountA);
    
    // Execute atomic swap
    return AtomicSwap(tokenA, tokenB, amountA, amountB);
}
```

## Best Practices Demonstrated

1. **Always validate inputs** before executing transfers
2. **Check balances** before attempting transfers
3. **Use atomic operations** to ensure consistency
4. **Emit events** for external monitoring
5. **Handle errors gracefully** with clear error messages
6. **Optimize for gas efficiency** where possible
7. **Test extensively** including edge cases and attacks

## Related Examples

- [Storage Example](Example.SmartContract.Storage/) - State management patterns
- [Events Example](Example.SmartContract.Event/) - Event handling
- [NEP-17 Token](Example.SmartContract.NEP17/) - Token standard implementation
- [Multi-sig Example](../src/Neo.SmartContract.Template/templates/neocontractmultisig/) - Multi-signature patterns

This example provides a solid foundation for implementing secure asset transfer functionality in NEO smart contracts.