# NEO Smart Contract - Multi-Signature Wallet

This template provides a complete multi-signature wallet implementation that requires multiple confirmations for transaction execution.

## Features

- **Multi-Signature Security**: Require multiple owners to approve transactions
- **Flexible Ownership**: Add/remove owners dynamically
- **Configurable Threshold**: Set required confirmation count
- **Transaction Management**: Submit, confirm, and execute transactions
- **Asset Support**: Handle NEO, GAS, and other tokens
- **Event Notifications**: Track all wallet activities

## Getting Started

### Building the Contract

```bash
dotnet build
```

### Compiling to NEO Bytecode

```bash
dotnet run --project path/to/Neo.Compiler.CSharp -- YourContract.csproj
```

## Contract Methods

### Owner Management

- `IsOwner(address)` - Check if address is an owner
- `AddOwner(newOwner)` - Add a new owner (requires multi-sig)
- `RemoveOwner(owner)` - Remove an owner (requires multi-sig)
- `GetRequiredConfirmations()` - Get confirmation threshold
- `ChangeRequirement(required)` - Change confirmation threshold

### Transaction Management

- `SubmitTransaction(to, value, data)` - Submit new transaction
- `ConfirmTransaction(transactionId)` - Confirm a transaction
- `GetTransaction(transactionId)` - Get transaction details
- `IsConfirmed(transactionId, owner)` - Check if owner confirmed
- `GetTransactionCount()` - Get total transaction count

### Administrative

- `Update(nefFile, manifest)` - Upgrade contract (requires multi-sig)
- `Destroy()` - Destroy contract (requires multi-sig)

## Usage Examples

### Setting Up the Wallet

```csharp
// During deployment, initialize with owners and threshold
UInt160[] owners = { owner1, owner2, owner3 };
BigInteger requiredConfirmations = 2; // 2 out of 3 signatures required
```

### Submitting a Transaction

```csharp
// Owner submits a transaction to transfer 100 GAS
var txId = SubmitTransaction(
    recipientAddress,
    100_00000000, // 100 GAS (8 decimals)
    ByteString.Empty
);
```

### Confirming a Transaction

```csharp
// Other owners confirm the transaction
ConfirmTransaction(txId);
// Transaction executes automatically when threshold is reached
```

### Contract Calls with Data

```csharp
// Submit transaction to call another contract
var callData = StdLib.Serialize(new object[] { "methodName", param1, param2 });
SubmitTransaction(targetContract, 0, callData);
```

## Security Features

### Access Control
- Only owners can submit and confirm transactions
- Prevents replay attacks with unique transaction IDs
- Double-confirmation protection

### Transaction Safety
- Atomic execution prevents partial failures
- Gas estimation and limits
- Validation of all parameters

### Upgrade Safety
- Contract upgrades require multi-signature approval
- Emergency stop functionality
- Owner management requires consensus

## Events

The contract emits the following events:

- `TransactionSubmitted` - New transaction created
- `TransactionConfirmed` - Transaction confirmed by owner
- `TransactionExecuted` - Transaction successfully executed
- `OwnerAdded` - New owner added
- `OwnerRemoved` - Owner removed
- `RequirementChanged` - Confirmation threshold changed

## Best Practices

### Owner Management
- Start with trusted owners
- Use time delays for sensitive operations
- Regularly review owner list
- Plan for owner key rotation

### Transaction Patterns
- Batch similar operations
- Use descriptive transaction data
- Monitor gas consumption
- Implement spending limits

### Security
- Test thoroughly on testnet
- Audit before mainnet deployment
- Monitor all transactions
- Have emergency procedures

## Deployment Checklist

- [ ] Review owner addresses
- [ ] Set appropriate confirmation threshold
- [ ] Test all transaction types
- [ ] Verify upgrade mechanisms
- [ ] Document emergency procedures
- [ ] Deploy to testnet first
- [ ] Conduct security audit

## License

MIT License - see LICENSE file for details