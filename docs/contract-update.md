# Contract Update Guide

This guide explains how to use the Neo Smart Contract Deployment Toolkit to update existing contracts on the Neo blockchain.

## Overview

The deployment toolkit provides comprehensive contract update capabilities that:
- Support full or partial contract updates (NEF only, manifest only, or both)
- Use Neo N3's `_deploy` pattern with `update=true`
- Validate authorization before updates
- Handle update transaction creation and submission
- Provide detailed update status and verification

## Prerequisites

Before updating a contract:
1. The contract must already be deployed on the network
2. The contract must have proper update permissions in its manifest
3. You must have the private key with update authorization
4. The contract should implement the `_deploy` method to handle updates

## Basic Usage

### Update from Source Code

```csharp
using Neo.SmartContract.Deploy;

// Initialize the toolkit
var toolkit = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("your-private-key-wif");

// Update the contract
var updateInfo = await toolkit.UpdateAsync(
    "0x1234...contract-hash", 
    "path/to/updated/Contract.csproj"
);

Console.WriteLine($"Update successful: {updateInfo.Success}");
Console.WriteLine($"Transaction: {updateInfo.TransactionHash}");
```

### Update from Compiled Artifacts

```csharp
// Update both NEF and manifest
var updateInfo = await toolkit.UpdateArtifactsAsync(
    "0x1234...contract-hash",
    "path/to/contract.nef",
    "path/to/contract.manifest.json"
);

// Update only the code (NEF)
var updateInfo = await toolkit.UpdateArtifactsAsync(
    "0x1234...contract-hash",
    "path/to/contract.nef",
    null  // Keep existing manifest
);

// Update only the manifest
var updateInfo = await toolkit.UpdateArtifactsAsync(
    "0x1234...contract-hash",
    null,  // Keep existing code
    "path/to/contract.manifest.json"
);
```

## Update with Parameters

You can pass parameters to the contract's `_deploy` method during updates:

```csharp
// Pass update parameters
var updateParams = new object[] { "v2.0.0", DateTime.UtcNow.Ticks };
var updateInfo = await toolkit.UpdateAsync(
    "0x1234...contract-hash",
    "path/to/Contract.csproj",
    updateParams
);
```

## Contract Implementation

Your contract should implement the `_deploy` method to handle updates:

```csharp
[ContractPermission("*", "*")]
public class MyContract : SmartContract
{
    public static void _deploy(object data, bool update)
    {
        if (update)
        {
            // Handle update logic
            Storage.Put(Storage.CurrentContext, "version", "v2");
            
            // Process update parameters if provided
            if (data is object[] args && args.Length > 0)
            {
                var version = (string)args[0];
                Storage.Put(Storage.CurrentContext, "updateVersion", version);
            }
        }
        else
        {
            // Initial deployment logic
            Storage.Put(Storage.CurrentContext, "version", "v1");
        }
    }
}
```

## Update Permissions

Ensure your contract manifest includes proper update permissions:

```json
{
    "permissions": [
        {
            "contract": "0xfffdc93764dbaddd97c48f252a53ea4643faa3fd",
            "methods": ["update"]
        }
    ]
}
```

Or use the attribute in your contract:

```csharp
[ContractPermission("0xfffdc93764dbaddd97c48f252a53ea4643faa3fd", "update")]
public class MyContract : SmartContract
{
    // Contract implementation
}
```

## Advanced Options

### Custom Update Options

```csharp
var options = new UpdateOptions
{
    WifKey = "your-private-key",
    RpcUrl = "http://localhost:10332",
    NetworkMagic = 894710606,
    WaitForConfirmation = true,
    VerifyAfterUpdate = true,
    GasLimit = 100_000_000,
    DryRun = false,
    UpdateNefOnly = false,
    UpdateManifestOnly = false
};

var updateInfo = await updater.UpdateAsync(
    contractHash, 
    compiledContract, 
    options, 
    updateParams
);
```

### Using IContractUpdateService Directly

```csharp
// Get the update service from DI
var updater = serviceProvider.GetRequiredService<IContractUpdateService>();

// Check if contract can be updated
var canUpdate = await updater.CanUpdateAsync(contractHash, rpcUrl);
if (!canUpdate)
{
    throw new Exception("Contract cannot be updated");
}

// Perform the update
var updateInfo = await updater.UpdateAsync(
    contractHash,
    compiledContract,
    updateOptions,
    updateParams
);
```

## Update Result Information

The `ContractUpdateInfo` object provides detailed update information:

```csharp
public class ContractUpdateInfo
{
    public string ContractName { get; set; }
    public UInt160? ContractHash { get; set; }
    public UInt256? TransactionHash { get; set; }
    public uint BlockIndex { get; set; }
    public long GasConsumed { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool VerificationFailed { get; set; }
    public string? PreviousVersion { get; set; }
    public string? NewVersion { get; set; }
    // ... additional properties
}
```

## Error Handling

Common update errors and their solutions:

### Contract Not Found
```csharp
try
{
    var updateInfo = await toolkit.UpdateAsync(contractHash, path);
}
catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
{
    Console.WriteLine("Contract does not exist on the network");
}
```

### Insufficient Permissions
```csharp
try
{
    var updateInfo = await toolkit.UpdateAsync(contractHash, path);
}
catch (InvalidOperationException ex) when (ex.Message.Contains("cannot be updated"))
{
    Console.WriteLine("Contract lacks update permissions");
}
```

### Update Validation Failed
```csharp
if (updateInfo.Success && updateInfo.VerificationFailed)
{
    Console.WriteLine("Update transaction succeeded but verification failed");
    Console.WriteLine("The contract state may not have been updated as expected");
}
```

## Best Practices

1. **Always test updates on testnet first**
   - Deploy and update contracts on testnet before mainnet
   - Verify the update process works as expected

2. **Implement proper _deploy logic**
   - Handle both deployment and update scenarios
   - Validate update parameters
   - Maintain backward compatibility when needed

3. **Use version tracking**
   - Store version information in contract storage
   - Track update history for auditing

4. **Verify updates**
   - Use `VerifyAfterUpdate = true` in production
   - Check that the contract state was updated correctly

5. **Handle partial updates carefully**
   - When updating only NEF or manifest, ensure compatibility
   - Test partial updates thoroughly

## Example: Complete Update Flow

```csharp
public async Task UpdateContractExample()
{
    // Initialize toolkit
    var toolkit = new DeploymentToolkit()
        .SetNetwork("testnet")
        .SetWifKey("your-wif-key");

    var contractHash = "0x1234...your-contract-hash";
    
    // Check if contract exists
    if (!await toolkit.ContractExistsAsync(contractHash))
    {
        Console.WriteLine("Contract not found");
        return;
    }

    // Check current version (optional)
    var currentVersion = await toolkit.CallAsync<string>(
        contractHash, 
        "getVersion"
    );
    Console.WriteLine($"Current version: {currentVersion}");

    // Perform update
    try
    {
        var updateInfo = await toolkit.UpdateAsync(
            contractHash,
            "path/to/UpdatedContract.csproj",
            new object[] { "v2.0.0", DateTime.UtcNow }
        );

        if (updateInfo.Success)
        {
            Console.WriteLine($"Update successful!");
            Console.WriteLine($"Transaction: {updateInfo.TransactionHash}");
            Console.WriteLine($"Gas consumed: {updateInfo.GasConsumed}");
            
            // Verify new version
            var newVersion = await toolkit.CallAsync<string>(
                contractHash, 
                "getVersion"
            );
            Console.WriteLine($"New version: {newVersion}");
        }
        else
        {
            Console.WriteLine($"Update failed: {updateInfo.ErrorMessage}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating contract: {ex.Message}");
    }
}
```

## Testing Contract Updates

The deployment toolkit includes comprehensive tests for update functionality:

- Unit tests: Test update logic and script generation
- Integration tests: Test full update flow with a local Neo node

Run the tests:
```bash
dotnet test --filter "Category=Update"
```

## Security Considerations

1. **Private key security**: Never commit private keys to source control
2. **Update authorization**: Limit update permissions to authorized accounts
3. **Validation**: Always validate update parameters in the `_deploy` method
4. **Atomic updates**: Ensure update operations are atomic and reversible

## Troubleshooting

### Update transaction fails immediately
- Check that the contract has update permissions
- Verify the WIF key has sufficient GAS
- Ensure the contract exists on the network

### Update succeeds but changes aren't reflected
- Verify the `_deploy` method handles updates correctly
- Check that storage changes are being persisted
- Ensure the updated NEF/manifest are correct

### Verification fails after update
- The contract state may be inconsistent
- Check the `_deploy` method for errors
- Review the update parameters

## Next Steps

- Learn about [batch updates](./batch-operations.md)
- Explore [contract migration strategies](./migration.md)
- Read about [security best practices](./security.md)