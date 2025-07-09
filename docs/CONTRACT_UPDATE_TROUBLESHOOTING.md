# Neo Smart Contract Update Troubleshooting Guide

This guide helps diagnose and resolve common issues when updating Neo smart contracts using the deployment toolkit.

## Table of Contents

1. [Common Error Messages](#common-error-messages)
2. [Authorization Issues](#authorization-issues)
3. [Technical Issues](#technical-issues)
4. [Network and Connection Issues](#network-and-connection-issues)
5. [Contract-Specific Issues](#contract-specific-issues)
6. [Debugging Steps](#debugging-steps)
7. [Prevention Best Practices](#prevention-best-practices)

## Common Error Messages

### "Unauthorized update attempt"

**Problem**: The authorization check in the contract's `_deploy` method failed.

**Solution**:
```csharp
// Ensure your _deploy method has proper authorization checks
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        // Check authorization
        if (!Runtime.CheckWitness(GetOwner()))
        {
            throw new Exception("Only owner can update contract");
        }
        
        // Optional: Perform state migration
        return;
    }
    
    // Initial deployment logic
}
```

### "Only owner can update contract"

**Problem**: The account attempting the update is not authorized.

**Solutions**:
1. Use the same account that deployed the contract:
   ```csharp
   toolkit.SetWifKey("original-deployer-wif-key");
   ```

2. Transfer ownership to a new account first:
   ```csharp
   await toolkit.InvokeAsync(contractHash, "transferOwnership", newOwnerAddress);
   ```

3. If using multi-sig or governance, ensure proper authorization:
   ```csharp
   // For governance-based contracts
   await toolkit.InvokeAsync(governanceHash, "proposeUpdate", contractHash, newNef, newManifest);
   await toolkit.InvokeAsync(governanceHash, "approveUpdate", proposalId);
   ```

### "Contract doesn't exist"

**Problem**: The specified contract hash is not found on the network.

**Solutions**:
1. Verify the contract hash:
   ```csharp
   var exists = await toolkit.ContractExistsAsync(contractHash);
   if (!exists)
   {
       Console.WriteLine("Contract not found on network");
   }
   ```

2. Check you're on the correct network:
   ```csharp
   toolkit.SetNetwork("testnet"); // or "mainnet"
   ```

3. Verify the hash format:
   ```csharp
   // Correct: 0x followed by 40 hex characters
   var validHash = "0x1234567890abcdef1234567890abcdef12345678";
   ```

### "Insufficient GAS balance"

**Problem**: Not enough GAS to pay for the update transaction.

**Solutions**:
1. Check your balance:
   ```csharp
   var balance = await toolkit.GetGasBalanceAsync();
   Console.WriteLine($"Current balance: {balance} GAS");
   ```

2. Typical update costs:
   - Small contract: 10-20 GAS
   - Medium contract: 20-50 GAS
   - Large contract: 50-100 GAS

3. Fund your account:
   ```bash
   # Using neo-cli
   neo> send gas <amount> <your-address>
   ```

### "Contract size exceeds maximum allowed"

**Problem**: The new contract is too large (>2MB).

**Solutions**:
1. Optimize your contract:
   ```csharp
   var compilationOptions = new CompilationOptions
   {
       Optimize = true,
       GenerateDebugInfo = false
   };
   ```

2. Reduce contract size:
   - Remove unnecessary methods
   - Simplify logic
   - Move data off-chain
   - Use storage more efficiently

3. Check size before update:
   ```bash
   ls -lh compiled-contracts/*.nef
   ```

## Authorization Issues

### Multi-Signature Authorization

If your contract requires multi-sig authorization:

```csharp
// Setup multi-sig
var multiSigAccount = Contract.CreateMultiSigContract(2, publicKeys);

// Sign with multiple keys
var transaction = await CreateUpdateTransaction(contractHash, newNef, newManifest);
transaction.AddWitness(key1);
transaction.AddWitness(key2);
await SendTransaction(transaction);
```

### Time-Locked Updates

For contracts with time restrictions:

```csharp
// Check if update is allowed
var lastUpdate = await toolkit.CallAsync<BigInteger>(contractHash, "getLastUpdateTime");
var minInterval = 7 * 24 * 60 * 60 * 1000; // 7 days
var canUpdate = (Runtime.Time - lastUpdate) >= minInterval;

if (!canUpdate)
{
    var timeRemaining = minInterval - (Runtime.Time - lastUpdate);
    Console.WriteLine($"Must wait {timeRemaining / 86400000} days before next update");
}
```

### Governance-Based Updates

For DAO/governance contracts:

```csharp
// 1. Create update proposal
var proposalId = await toolkit.InvokeAsync(
    governanceContract,
    "createUpdateProposal",
    contractHash,
    newNefBytes,
    newManifest,
    "Update to fix bug #123"
);

// 2. Wait for voting period
await WaitForVotingPeriod(proposalId);

// 3. Execute if approved
await toolkit.InvokeAsync(governanceContract, "executeProposal", proposalId);
```

## Technical Issues

### Contract State Migration

When updating contracts with state changes:

```csharp
[DisplayName("update")]
public static bool Update(ByteString nefFile, string manifest, object data)
{
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized");
    
    // Migrate state before update
    var version = Storage.Get(Storage.CurrentContext, "version");
    if (version == null || (int)version < 2)
    {
        MigrateToV2();
    }
    
    ContractManagement.Update(nefFile, manifest, data);
    return true;
}

private static void MigrateToV2()
{
    // Example: Change storage key format
    var oldKey = new byte[] { 0x01 };
    var newKey = new byte[] { 0x02 };
    
    var value = Storage.Get(Storage.CurrentContext, oldKey);
    if (value != null)
    {
        Storage.Put(Storage.CurrentContext, newKey, value);
        Storage.Delete(Storage.CurrentContext, oldKey);
    }
    
    Storage.Put(Storage.CurrentContext, "version", 2);
}
```

### Method Signature Changes

When changing method signatures:

```csharp
// Old contract
public static string GetData(int id)

// New contract - maintain backward compatibility
public static string GetData(int id)
{
    return GetDataV2(id, "default");
}

public static string GetDataV2(int id, string format)
{
    // New implementation
}
```

### Storage Layout Changes

Avoid breaking storage compatibility:

```csharp
// ❌ Bad: Changing storage key structure
// Old: Storage.Put(key, value)
// New: Storage.Put(Hash256(key), value)

// ✅ Good: Add new keys, keep old ones
// Old keys remain: Storage.Get(oldKey)
// New keys added: Storage.Put(newKey, newValue)
```

## Network and Connection Issues

### RPC Connection Failures

```csharp
// Use retry logic
var retryCount = 3;
for (int i = 0; i < retryCount; i++)
{
    try
    {
        var result = await toolkit.UpdateAsync(contractHash, contractPath);
        if (result.Success) break;
    }
    catch (Exception ex) when (ex.Message.Contains("connection"))
    {
        if (i == retryCount - 1) throw;
        await Task.Delay(5000); // Wait 5 seconds
    }
}
```

### Network Congestion

During high network usage:

```csharp
var deploymentOptions = new DeploymentOptions
{
    // Increase network fee
    DefaultNetworkFee = 2_000_000, // 0.02 GAS
    
    // Increase valid period
    ValidUntilBlockOffset = 500,
    
    // Set higher gas limit
    GasLimit = 200_000_000
};
```

### SSL/TLS Issues

For SSL certificate problems:

```csharp
// Use different RPC endpoint
toolkit.SetNetwork("https://testnet1.neo.coz.io:443");

// Or configure custom RPC
var customRpc = "http://your-node:10332"; // HTTP instead of HTTPS
toolkit.SetNetwork(customRpc);
```

## Contract-Specific Issues

### NEP-17 Token Updates

Special considerations for token contracts:

```csharp
[DisplayName("update")]
public static bool Update(ByteString nefFile, string manifest, object data)
{
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized");
    
    // Pause transfers during update
    Storage.Put(Storage.CurrentContext, "paused", 1);
    
    // Ensure no pending operations
    var pendingOps = Storage.Get(Storage.CurrentContext, "pendingOps");
    if (pendingOps != null && (int)pendingOps > 0)
        throw new Exception("Pending operations must complete first");
    
    ContractManagement.Update(nefFile, manifest, data);
    
    // Resume in _deploy method
    return true;
}
```

### NFT Contract Updates

For NFT contracts:

```csharp
// Ensure token URIs remain valid
private static void ValidateNFTUpdate()
{
    // Check that base URI hasn't changed incompatibly
    var oldBaseUri = Storage.Get(Storage.CurrentContext, "baseUri");
    var newBaseUri = "https://new-api.example.com/";
    
    if (oldBaseUri != null && !newBaseUri.StartsWith((string)oldBaseUri))
    {
        throw new Exception("Base URI change would break existing tokens");
    }
}
```

### Oracle Contract Updates

For oracle-dependent contracts:

```csharp
// Maintain oracle request compatibility
public static void ValidateOracleUpdate()
{
    // Ensure oracle URLs remain accessible
    var oracleUrl = Storage.Get(Storage.CurrentContext, "oracleUrl");
    if (!IsUrlAccessible((string)oracleUrl))
    {
        throw new Exception("Oracle URL no longer accessible");
    }
}
```

## Debugging Steps

### 1. Enable Detailed Logging

```csharp
var toolkit = new DeploymentToolkit();
toolkit.ConfigureLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
```

### 2. Test Update Locally

```bash
# Using Neo Express
neoxp contract update <hash> <nef-file> <manifest> <account>
```

### 3. Verify Contract State

```csharp
// Before update
var stateBefore = await CaptureContractState(contractHash);

// After update
var stateAfter = await CaptureContractState(contractHash);

// Compare states
CompareStates(stateBefore, stateAfter);
```

### 4. Transaction Debugging

```csharp
// Get detailed transaction info
var tx = await rpcClient.GetTransactionAsync(txHash);
Console.WriteLine($"VM State: {tx.VMState}");
Console.WriteLine($"Exception: {tx.Exception}");

// Get application log
var appLog = await rpcClient.GetApplicationLogAsync(txHash);
foreach (var notification in appLog.Executions[0].Notifications)
{
    Console.WriteLine($"Event: {notification.EventName}");
}
```

## Prevention Best Practices

### 1. Always Test Updates

```csharp
// Test on testnet first
toolkit.SetNetwork("testnet");
var testResult = await toolkit.UpdateAsync(testnetHash, contractPath);

// Only proceed to mainnet if successful
if (testResult.Success)
{
    toolkit.SetNetwork("mainnet");
    var mainnetResult = await toolkit.UpdateAsync(mainnetHash, contractPath);
}
```

### 2. Implement Update Safeguards

```csharp
[DisplayName("update")]
public static bool Update(ByteString nefFile, string manifest, object data)
{
    // Multiple safety checks
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized");
    
    if (!IsUpdateAllowed())
        throw new Exception("Update conditions not met");
    
    if (!ValidateNewContract(nefFile, manifest))
        throw new Exception("Invalid contract");
    
    // Create backup point
    CreateStateBackup();
    
    try
    {
        ContractManagement.Update(nefFile, manifest, data);
        return true;
    }
    catch
    {
        RestoreStateBackup();
        throw;
    }
}
```

### 3. Version Your Contracts

```csharp
public class ContractInfo
{
    public const int Version = 2;
    public const string BuildDate = "2024-01-15";
    public const string CommitHash = "abc123def";
}

[DisplayName("getVersion")]
public static Map<string, object> GetVersion()
{
    return new Map<string, object>
    {
        ["version"] = ContractInfo.Version,
        ["buildDate"] = ContractInfo.BuildDate,
        ["commitHash"] = ContractInfo.CommitHash
    };
}
```

### 4. Document Update Procedures

Create a `CONTRACT_UPDATE_PROCEDURE.md` for your team:

```markdown
## Update Checklist

- [ ] Test update on local Neo Express
- [ ] Test update on testnet
- [ ] Verify state migration works
- [ ] Check storage compatibility
- [ ] Ensure method signatures are compatible
- [ ] Update documentation
- [ ] Notify users of planned update
- [ ] Have rollback plan ready
- [ ] Monitor after update
```

### 5. Monitor After Updates

```csharp
// Set up monitoring
public async Task MonitorContractHealth(string contractHash)
{
    while (true)
    {
        try
        {
            // Check basic functionality
            var symbol = await toolkit.CallAsync<string>(contractHash, "symbol");
            
            // Check for errors
            var errors = await toolkit.CallAsync<int>(contractHash, "getErrorCount");
            if (errors > threshold)
            {
                await SendAlert($"High error count: {errors}");
            }
            
            await Task.Delay(60000); // Check every minute
        }
        catch (Exception ex)
        {
            await SendAlert($"Contract health check failed: {ex.Message}");
        }
    }
}
```

## Getting Help

If you continue to experience issues:

1. **Check the logs**: Enable debug logging for detailed error information
2. **Community Support**: 
   - Neo Discord: https://discord.gg/neo
   - GitHub Issues: https://github.com/neo-project/neo-devpack-dotnet/issues
3. **Documentation**: 
   - Neo Docs: https://docs.neo.org/
   - Update Guide: [UPDATE_GUIDE.md](./UPDATE_GUIDE.md)

Remember: Always test updates thoroughly on testnet before applying to mainnet!