# Contract Update Guide

This guide explains how to make Neo smart contracts updatable and use the deployment toolkit's update functionality.

## Key Concepts

### 1. Contract Update Mechanism in Neo

- **ContractManagement.Update**: A native contract method that updates a deployed contract
- **Important**: Only the contract itself can call `ContractManagement.Update`
- External accounts cannot directly update contracts - they must call the contract's update method

### 2. Making Contracts Updatable

To make a contract updatable, you must implement an `update` method in your contract:

```csharp
[DisplayName("update")]
public static bool Update(ByteString nefFile, string manifest, object data)
{
    // Check authorization (e.g., only owner can update)
    if (!Runtime.CheckWitness(GetOwner()))
    {
        throw new Exception("Only owner can update contract");
    }
    
    // Call ContractManagement.Update
    ContractManagement.Update(nefFile, manifest, data);
    return true;
}
```

### 3. Authorization Patterns

Common patterns for controlling who can update:

#### Owner-based:
```csharp
if (!Runtime.CheckWitness(GetOwner()))
{
    throw new Exception("Only owner can update");
}
```

#### Multi-sig or Council:
```csharp
var isCouncil = IsCouncilMember(Runtime.CallingScriptHash);
if (!isCouncil && !Runtime.CheckWitness(GetOwner()))
{
    throw new Exception("Only council or owner can update");
}
```

#### Governance-based:
```csharp
if (!CheckGovernance())
{
    throw new Exception("Only governance can update");
}
```

## Using the Deployment Toolkit

### Deploy a Contract with Update Capability

```csharp
// Deploy contract (must have update method)
var toolkit = new DeploymentToolkit();
toolkit.SetWifKey("your-wif-key");
toolkit.SetNetwork("testnet");

var deployResult = await toolkit.DeployAsync("path/to/Contract.cs");
Console.WriteLine($"Contract deployed: {deployResult.ContractHash}");
```

### Update a Deployed Contract

```csharp
// Update the contract
var updateResult = await toolkit.UpdateAsync(
    contractHash: deployResult.ContractHash.ToString(),
    sourcePath: "path/to/UpdatedContract.cs"
);

if (updateResult.Success)
{
    Console.WriteLine($"Contract updated: {updateResult.TransactionHash}");
}
```

## Complete Example

See `UpdateDemo.cs` for a complete example that:
1. Deploys a contract with update functionality
2. Initializes the contract
3. Tests the contract works
4. Updates the contract
5. Verifies the update succeeded

Run the demo:
```bash
export NEO_WIF_KEY="your-wif-key"
dotnet run demo
```

## Common Issues and Solutions

### "method not found: update/3"
**Problem**: The contract doesn't have an update method.
**Solution**: Add an update method to your contract before deployment.

### "Only owner can update contract"
**Problem**: The WIF key doesn't match the contract owner.
**Solution**: Use the same WIF key that deployed the contract.

### "contract doesn't exist"
**Problem**: Trying to update a non-existent contract.
**Solution**: Verify the contract hash and that it's deployed on the correct network.

## Testing Contract Updates

The deployment toolkit includes comprehensive update testing:

1. **UpdateContracts.cs**: Tests updating multiple contracts
2. **UpdateDemo.cs**: Demonstrates the full deployment and update cycle
3. **Unit tests**: Test the update script generation and API

## Technical Details

### Update Script Generation

The toolkit generates an update script that:
1. Pushes the new NEF file bytes
2. Pushes the new manifest JSON
3. Pushes optional update data
4. Calls the contract's update method

See `ScriptBuilderHelper.BuildUpdateScript` for implementation details.

### Transaction Signing

Updates require proper transaction signing:
- Use the same account that owns the contract
- The toolkit supports both wallet files and WIF keys
- Network magic must match (testnet: 894710606, mainnet: 860833102)

## Best Practices

1. **Always test updates on testnet first**
2. **Keep the update method simple** - avoid complex logic
3. **Log update events** for audit trails
4. **Consider time delays** for critical updates
5. **Implement proper access control**
6. **Test state preservation** after updates
7. **Version your contracts** in manifest metadata

## Example Contracts with Update Support

All example contracts now include update methods:
- **TokenContract**: Owner-based update
- **NFTContract**: Owner-based update
- **GovernanceContract**: Council or owner-based update

## Deployment Record

Successfully deployed updatable contract:
- **Contract**: TokenContract
- **Hash**: `0xaef772277517be7405e42da00177b87c5293f413`
- **Network**: TestNet
- **Features**: NEP-17 token with update capability
- **Deploy TX**: `0x0f9cc29768af0380f5b5fca7cb32b0f1cfe9c2b7bb23216f7d9335feead07003`

This contract can be updated by the owner using the deployment toolkit's update functionality.