# Neo Smart Contract Update Guide

This guide provides comprehensive information on updating Neo smart contracts using the R3E.SmartContract.Deploy toolkit.

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Basic Update Workflow](#basic-update-workflow)
4. [Contract Update Requirements](#contract-update-requirements)
5. [API Reference](#api-reference)
6. [Examples](#examples)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)

## Overview

Contract updates in Neo N3 work by calling `ContractManagement.Update` directly. When a contract is updated, the blockchain automatically calls the contract's `_deploy` method with the `update` parameter set to `true`. This is where contracts implement authorization checks and state migration logic.

### Key Concepts

- **_deploy Method**: Every Neo contract has a `_deploy` method that is called during both deployment and updates
- **Update Flag**: The `_deploy` method receives a boolean `update` parameter to distinguish between deployment and updates
- **Authorization**: Authorization checks must be implemented in the `_deploy` method when `update` is true
- **State Preservation**: Contract storage is preserved during updates
- **Transaction Signing**: Updates require proper transaction signing with the authorized account

## Prerequisites

1. **R3E.SmartContract.Deploy toolkit** installed and configured
2. **Authorized Account**: Access to the account that can update the contract (typically the deployer/owner)
3. **Contract with _deploy Method**: The target contract must have proper authorization logic in its `_deploy` method
4. **Sufficient GAS**: Balance to cover update transaction costs

## Basic Update Workflow

```csharp
// 1. Create deployment toolkit
var toolkit = new DeploymentToolkit();

// 2. Configure authentication (WIF key recommended)
toolkit.SetWifKey("your-wif-key-here");
toolkit.SetNetwork("testnet");

// 3. Update the contract
var result = await toolkit.UpdateAsync(
    contractHash: "0x1234...abcd",
    path: "path/to/UpdatedContract.cs"
);

// 4. Check result
if (result.Success)
{
    Console.WriteLine($"✅ Update successful: {result.TransactionHash}");
}
else
{
    Console.WriteLine($"❌ Update failed: {result.ErrorMessage}");
}
```

## Contract Update Requirements

### 1. Implement _deploy Method with Update Logic

Every Neo contract has a `_deploy` method that handles both deployment and updates:

```csharp
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        // Check authorization - customize based on your needs
        if (!Runtime.CheckWitness(GetOwner()))
        {
            throw new Exception("Only owner can update contract");
        }
        
        // Optional: Perform state migration if needed
        MigrateState();
        
        return;
    }
    
    // Initial deployment logic
    Storage.Put(Storage.CurrentContext, "deployed", 1);
    // Initialize owner, etc.
}
```

### 2. Authorization Patterns

#### Owner-based Authorization
```csharp
if (!Runtime.CheckWitness(GetOwner()))
{
    throw new Exception("Only owner can update contract");
}
```

#### Multi-signature Authorization
```csharp
if (!Runtime.CheckWitness(GetMultiSigAccount()))
{
    throw new Exception("Multi-sig approval required");
}
```

#### Governance-based Authorization
```csharp
if (!CheckGovernanceApproval())
{
    throw new Exception("Governance approval required");
}
```

#### Time-locked Updates
```csharp
var updateProposal = GetUpdateProposal();
if (updateProposal.ProposedAt + UPDATE_DELAY > Runtime.Time)
{
    throw new Exception("Update delay period not met");
}
```

### 3. State Management

Contract state is automatically preserved during updates. State migration logic should be implemented in the `_deploy` method:

```csharp
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        // Authorization check
        if (!Runtime.CheckWitness(GetOwner()))
        {
            throw new Exception("Only owner can update contract");
        }
        
        // State migration logic
        MigrateState();
        
        // Post-update initialization if needed
        PostUpdateInitialization();
        
        return;
    }
    
    // Initial deployment
    Storage.Put(Storage.CurrentContext, "version", 1);
    Storage.Put(Storage.CurrentContext, "deployed", 1);
}

private static void MigrateState()
{
    // Example: Migrate data format if needed
    var oldVersion = Storage.Get(Storage.CurrentContext, "version");
    if (oldVersion == null || (int)oldVersion < 2)
    {
        // Perform migration
        Storage.Put(Storage.CurrentContext, "version", 2);
        // Migrate other data structures as needed
    }
}
```

## API Reference

### DeploymentToolkit.UpdateAsync

```csharp
public async Task<ContractDeploymentInfo> UpdateAsync(string contractHashOrAddress, string path)
```

**Parameters:**
- `contractHashOrAddress`: Contract hash (0x...) or Neo address to update
- `path`: Path to contract source file (.cs) or project file (.csproj)

**Returns:**
- `ContractDeploymentInfo`: Result containing success status, transaction hash, and error details

**Exceptions:**
- `ArgumentException`: Invalid parameters
- `FileNotFoundException`: Contract file not found
- `InvalidOperationException`: No authorized account configured

### Configuration Options

```csharp
// Network configuration
toolkit.SetNetwork("testnet");        // mainnet, testnet, or custom
toolkit.SetRpcUrl("https://...");     // Custom RPC endpoint

// Authentication
toolkit.SetWifKey("KzjaqM...");       // WIF private key (recommended)
await toolkit.LoadWalletAsync("path/to/wallet.json", "password");

// Gas and confirmation settings
// Configure via appsettings.json:
{
  "Deployment": {
    "GasLimit": 100000000,
    "WaitForConfirmation": true,
    "VerifyAfterDeploy": true
  }
}
```

## Examples

### Example 1: Basic Token Contract Update

```csharp
using R3E.SmartContract.Deploy;

class Program
{
    static async Task Main(string[] args)
    {
        var toolkit = new DeploymentToolkit();
        
        // Use WIF key for authentication
        toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
        toolkit.SetNetwork("testnet");
        
        try
        {
            // Update the contract
            var result = await toolkit.UpdateAsync(
                "0xaef772277517be7405e42da00177b87c5293f413",
                "contracts/TokenContract.cs"
            );
            
            if (result.Success)
            {
                Console.WriteLine($"✅ Contract updated successfully!");
                Console.WriteLine($"   Transaction: {result.TransactionHash}");
                Console.WriteLine($"   Gas consumed: {result.GasConsumed / 100_000_000m} GAS");
            }
            else
            {
                Console.WriteLine($"❌ Update failed: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
```

### Example 2: Update with Environment Variables

```csharp
// Set environment variables
Environment.SetEnvironmentVariable("NEO_WIF_KEY", "your-wif-key");
Environment.SetEnvironmentVariable("NEO_NETWORK", "testnet");

var toolkit = new DeploymentToolkit();

// Toolkit automatically loads from environment variables
var result = await toolkit.UpdateAsync(
    contractHash: args[0],
    path: args[1]
);
```

### Example 3: Batch Contract Updates

```csharp
var contracts = new[]
{
    new { Hash = "0x1234...", Path = "contracts/Token.cs" },
    new { Hash = "0x5678...", Path = "contracts/NFT.cs" },
    new { Hash = "0x9abc...", Path = "contracts/Governance.cs" }
};

var toolkit = new DeploymentToolkit();
toolkit.SetWifKey(Environment.GetEnvironmentVariable("NEO_WIF_KEY"));
toolkit.SetNetwork("testnet");

foreach (var contract in contracts)
{
    try
    {
        Console.WriteLine($"Updating {contract.Hash}...");
        
        var result = await toolkit.UpdateAsync(contract.Hash, contract.Path);
        
        if (result.Success)
        {
            Console.WriteLine($"✅ {contract.Hash} updated: {result.TransactionHash}");
        }
        else
        {
            Console.WriteLine($"❌ {contract.Hash} failed: {result.ErrorMessage}");
        }
        
        // Wait between updates to avoid rate limiting
        await Task.Delay(5000);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error updating {contract.Hash}: {ex.Message}");
    }
}
```

### Example 4: Update with Verification

```csharp
var toolkit = new DeploymentToolkit();
toolkit.SetWifKey("your-wif-key");
toolkit.SetNetwork("testnet");

// Update contract
var updateResult = await toolkit.UpdateAsync(contractHash, contractPath);

if (updateResult.Success)
{
    // Wait for confirmation
    await Task.Delay(15000);
    
    // Verify the update worked by calling a contract method
    try
    {
        var version = await toolkit.CallAsync<int>(contractHash, "getVersion");
        Console.WriteLine($"✅ Contract updated to version: {version}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Update succeeded but verification failed: {ex.Message}");
    }
}
```

## Best Practices

### 1. Version Management

```csharp
// Add version tracking to your contracts
private const int CONTRACT_VERSION = 2;

[DisplayName("getVersion")]
[Safe]
public static int GetVersion() => CONTRACT_VERSION;

[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        if (!Runtime.CheckWitness(GetOwner()))
            throw new Exception("Only owner can update");
        
        // Log the update
        var oldVersion = Storage.Get(Storage.CurrentContext, "version");
        Runtime.Log($"Updating from version {oldVersion ?? "1"} to {CONTRACT_VERSION}");
        
        // Update version in storage
        Storage.Put(Storage.CurrentContext, "version", CONTRACT_VERSION);
        return;
    }
    
    // Initial deployment
    Storage.Put(Storage.CurrentContext, "version", CONTRACT_VERSION);
    Storage.Put(Storage.CurrentContext, "deployed", 1);
}
```

### 2. Testing Updates

```csharp
// Test on testnet first
var toolkit = new DeploymentToolkit();
toolkit.SetNetwork("testnet");

// Use dry run mode for testing (if available)
var dryRunResult = await toolkit.UpdateAsync(contractHash, contractPath);

// Deploy to mainnet only after successful testnet testing
if (dryRunResult.Success)
{
    toolkit.SetNetwork("mainnet");
    var mainnetResult = await toolkit.UpdateAsync(contractHash, contractPath);
}
```

### 3. Error Handling

```csharp
try
{
    var result = await toolkit.UpdateAsync(contractHash, contractPath);
    
    if (!result.Success)
    {
        // Log specific error details
        _logger.LogError($"Update failed for {contractHash}: {result.ErrorMessage}");
        
        // Check common issues
        if (result.ErrorMessage?.Contains("unauthorized") == true)
        {
            throw new UnauthorizedAccessException("Update authorization failed in _deploy method");
        }
        
        if (result.ErrorMessage?.Contains("unauthorized") == true)
        {
            throw new UnauthorizedAccessException("Account not authorized to update contract");
        }
    }
}
catch (FileNotFoundException)
{
    _logger.LogError("Contract source file not found");
    throw;
}
catch (InvalidOperationException ex) when (ex.Message.Contains("Wallet not loaded"))
{
    _logger.LogError("No authorized account configured");
    throw;
}
```

### 4. Monitoring and Logging

```csharp
var toolkit = new DeploymentToolkit();

// Enable detailed logging
toolkit.ConfigureLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Log update attempts
_logger.LogInformation($"Attempting to update contract {contractHash}");

var result = await toolkit.UpdateAsync(contractHash, contractPath);

if (result.Success)
{
    _logger.LogInformation($"Contract {contractHash} updated successfully");
    _logger.LogInformation($"Transaction: {result.TransactionHash}");
    _logger.LogInformation($"Gas consumed: {result.GasConsumed}");
    
    // Optional: Send notification
    await SendUpdateNotification(contractHash, result.TransactionHash);
}
```

## Troubleshooting

### Common Issues

#### 1. "method not found: _deploy/2"
**Problem**: Contract doesn't have a proper `_deploy` method.
**Solution**: Ensure your contract has a `_deploy` method with the correct signature: `public static void _deploy(object data, bool update)`

#### 2. "Only owner can update contract"
**Problem**: Unauthorized account attempting update.
**Solution**: Use the same WIF key/account that deployed the contract.

#### 3. "Insufficient GAS balance"
**Problem**: Not enough GAS for the update transaction.
**Solution**: Fund the account with sufficient GAS (typically 10-50 GAS).

#### 4. "Contract doesn't exist"
**Problem**: Invalid contract hash or contract not deployed.
**Solution**: Verify the contract hash and network.

#### 5. "RPC connection failed"
**Problem**: Network connectivity issues.
**Solution**: Check RPC URL and network connectivity.

### Debugging Steps

1. **Verify Contract Exists**:
   ```csharp
   var exists = await toolkit.ContractExistsAsync(contractHash);
   if (!exists)
   {
       throw new InvalidOperationException("Contract not found");
   }
   ```

2. **Check Account Balance**:
   ```csharp
   var balance = await toolkit.GetGasBalanceAsync();
   if (balance < 50) // 50 GAS minimum recommended
   {
       throw new InvalidOperationException("Insufficient GAS balance");
   }
   ```

3. **Verify Contract Can Be Updated**:
   ```csharp
   try
   {
       // Check if contract has proper _deploy method
       var manifest = await rpcClient.GetContractAsync(contractHash);
       var deployMethod = manifest.Abi.Methods.FirstOrDefault(m => m.Name == "_deploy");
       if (deployMethod == null || deployMethod.Parameters.Length != 2)
       {
           throw new InvalidOperationException("Contract missing proper _deploy method");
       }
   }
   catch (Exception ex)
   {
       // Contract cannot be updated
   }
   ```

### Getting Help

- **GitHub Issues**: [neo-devpack-dotnet issues](https://github.com/neo-project/neo-devpack-dotnet/issues)
- **Neo Discord**: [Neo Community Discord](https://discord.gg/neo)
- **Documentation**: [Neo Developer Documentation](https://docs.neo.org/)

## Advanced Topics

### Custom Update Authorization

```csharp
[DisplayName("_deploy")]
public static void _deploy(object data, bool update)
{
    if (update)
    {
        // Complex authorization logic
        var isOwner = Runtime.CheckWitness(GetOwner());
        var isCouncilMember = IsCouncilMember(Runtime.CallingScriptHash);
        var hasGovernanceApproval = HasPendingUpdateApproval();
        
        if (!isOwner && !isCouncilMember && !hasGovernanceApproval)
        {
            throw new Exception("Unauthorized update attempt");
        }
        
        // Time-based restrictions
        var lastUpdate = GetLastUpdateTime();
        if (Runtime.Time - lastUpdate < MIN_UPDATE_INTERVAL)
        {
            throw new Exception("Update too soon after previous update");
        }
        
        // Log the update for audit trail
        Runtime.Log($"Contract updated by {Runtime.CallingScriptHash}");
        
        // Store update metadata
        Storage.Put(Storage.CurrentContext, "lastUpdateTime", Runtime.Time);
        Storage.Put(Storage.CurrentContext, "lastUpdatedBy", Runtime.CallingScriptHash);
        
        // Perform any state migration
        MigrateState();
        
        return;
    }
    
    // Initial deployment logic
    InitializeContract();
}
```

### Automated Update Workflows

```csharp
// Example CI/CD update script
public class ContractUpdatePipeline
{
    public async Task<bool> RunUpdatePipeline(string contractHash, string sourceDir)
    {
        var toolkit = new DeploymentToolkit();
        
        try
        {
            // 1. Build and test
            await BuildAndTest(sourceDir);
            
            // 2. Deploy to testnet
            toolkit.SetNetwork("testnet");
            var testResult = await toolkit.UpdateAsync(contractHash, sourceDir);
            
            if (!testResult.Success)
            {
                throw new Exception($"Testnet update failed: {testResult.ErrorMessage}");
            }
            
            // 3. Run integration tests
            await RunIntegrationTests(contractHash);
            
            // 4. Deploy to mainnet (requires manual approval)
            if (await GetMainnetApproval())
            {
                toolkit.SetNetwork("mainnet");
                var mainnetResult = await toolkit.UpdateAsync(contractHash, sourceDir);
                
                if (mainnetResult.Success)
                {
                    await NotifyStakeholders(contractHash, mainnetResult.TransactionHash);
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            await NotifyFailure(contractHash, ex.Message);
            return false;
        }
    }
}
```

This comprehensive guide covers all aspects of contract updates using the R3E.SmartContract.Deploy toolkit. For additional examples and advanced use cases, see the deployment examples in the repository.