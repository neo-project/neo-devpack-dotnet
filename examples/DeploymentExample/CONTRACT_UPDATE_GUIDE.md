# Contract Update Guide

This guide explains how to make Neo smart contracts updatable and use the deployment toolkit's update functionality.

## Key Concepts

### 1. Contract Update Mechanism in Neo N3

- **ContractManagement.Update**: A native contract method that updates a deployed contract
- **_deploy Method**: When a contract is updated, Neo automatically calls the contract's `_deploy` method with `update=true`
- **Authorization**: Must be implemented in the `_deploy` method when handling updates

### 2. Making Contracts Updatable

In Neo N3, contracts handle updates through the `_deploy` method:

```csharp
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        // Check authorization (e.g., only owner can update)
        if (!Runtime.CheckWitness(GetOwner()))
        {
            throw new Exception("Only owner can update contract");
        }
        
        // Optional: Perform state migration
        // MigrateState(data);
        
        return;
    }
    
    // Initial deployment logic
    Storage.Put(Storage.CurrentContext, "deployed", 1);
    // Initialize owner, etc.
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
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        var isCouncil = IsCouncilMember(Runtime.CallingScriptHash);
        if (!isCouncil && !Runtime.CheckWitness(GetOwner()))
        {
            throw new Exception("Only council or owner can update");
        }
        return;
    }
    // Initial deployment...
}
```

#### Governance-based:
```csharp
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        if (!CheckGovernance())
        {
            throw new Exception("Only governance can update");
        }
        return;
    }
    // Initial deployment...
}
```

## Using the Deployment Toolkit

### Deploy a Contract with Update Capability

```csharp
// Deploy contract (with proper _deploy method)
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

### "Unauthorized update attempt"
**Problem**: The authorization check in `_deploy` method failed.
**Solution**: Use the same WIF key that deployed the contract, or ensure proper authorization is granted.

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
1. Pushes the contract hash to update
2. Pushes optional update data
3. Pushes the new manifest JSON
4. Pushes the new NEF file bytes
5. Calls `ContractManagement.Update` directly

This triggers the contract's `_deploy` method with `update=true`.

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

All example contracts implement the `_deploy` method with update authorization:
- **TokenContract**: Owner-based update authorization in `_deploy`
- **NFTContract**: Owner-based update authorization in `_deploy`
- **GovernanceContract**: Council or owner-based update authorization in `_deploy`

## Deployment Record

Successfully deployed updatable contract:
- **Contract**: TokenContract
- **Hash**: `0xaef772277517be7405e42da00177b87c5293f413`
- **Network**: TestNet
- **Features**: NEP-17 token with update capability
- **Deploy TX**: `0x0f9cc29768af0380f5b5fca7cb32b0f1cfe9c2b7bb23216f7d9335feead07003`

This contract can be updated by the owner using the deployment toolkit's update functionality.