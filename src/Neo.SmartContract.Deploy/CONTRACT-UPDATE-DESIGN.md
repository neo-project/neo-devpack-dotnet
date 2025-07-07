# Contract Update Design

## Overview

This document describes how contract updates are handled in the Neo Smart Contract Deployment Toolkit after resolving the overlapping update interfaces.

## Previous Issues

1. **Overlapping Interfaces**: Both `IContractDeployer` and `IContractUpdateService` provided update functionality
2. **Incomplete Implementation**: `ContractUpdateService` returned "not implemented" error
3. **Confusion**: Unclear which interface to use for updates

## Resolution

### Architecture

```
┌─────────────────────┐
│  DeploymentToolkit  │
│   UpdateAsync()     │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────────┐      ┌────────────────────────┐
│  IContractUpdateService │      │ IDeploymentRecordService│
│  UpdateContractAsync()  │─────▶│  RecordUpdateAsync()   │
└───────────┬─────────────┘      └────────────────────────┘
            │
            ▼
┌──────────────────────┐
│  IContractDeployer   │
│    UpdateAsync()     │
└──────────────────────┘
```

### Key Changes

1. **ContractUpdateService** now properly delegates to `IContractDeployer.UpdateAsync()`
2. **Added deployment record tracking** for update history
3. **Unified update flow** through a single code path

### Update Flow

1. User calls `DeploymentToolkit.UpdateAsync()` or `NeoContractToolkit.UpdateContractAsync()`
2. Contract is compiled (if needed)
3. `ContractUpdateService` checks deployment records
4. Delegates to `ContractDeployerService.UpdateAsync()` for actual update
5. Records update history in deployment records

### Benefits

- **Single source of truth**: `ContractDeployerService` handles all contract operations
- **Proper tracking**: Update history is maintained in deployment records
- **Consistent API**: Both toolkit APIs use the same underlying implementation
- **Production ready**: Supports dry-run, verification, and rollback features

## Usage Examples

### Using DeploymentToolkit

```csharp
var toolkit = new DeploymentToolkit().SetNetwork("testnet");
var result = await toolkit.UpdateAsync("0x123...", "MyContract.csproj");
```

### Using NeoContractToolkit

```csharp
var toolkit = NeoContractToolkitBuilder.Create()
    .ConfigureNetwork(options => options.Network = "testnet")
    .Build();

var result = await toolkit.UpdateContractAsync(
    contractHash,
    compiledContract,
    deploymentOptions);
```

### Using ContractUpdateService

```csharp
var updateService = serviceProvider.GetRequiredService<IContractUpdateService>();
var result = await updateService.UpdateContractAsync(
    "MyContract",
    "testnet",
    nefBytes,
    manifest,
    null,
    "2.0.0");
```

## Implementation Details

### ContractUpdateService

- Validates contract exists in deployment records
- Creates `CompiledContract` from provided data
- Builds `DeploymentOptions` from configuration
- Delegates to `IContractDeployer.UpdateAsync()`
- Updates deployment records with version and history

### DeploymentRecord Model

```csharp
public class DeploymentRecord
{
    public string ContractName { get; set; }
    public string? ContractHash { get; set; }
    public string? TransactionHash { get; set; }
    public DateTime DeployedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Version { get; set; }
    public UpdateHistoryEntry[]? UpdateHistory { get; set; }
}
```

### UpdateHistoryEntry Model

```csharp
public class UpdateHistoryEntry
{
    public string? TransactionHash { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? PreviousVersion { get; set; }
    public string? NewVersion { get; set; }
}
```

## Future Improvements

1. **Version management**: Automatic version bumping
2. **Rollback support**: Ability to revert to previous versions
3. **Update validation**: Pre-update checks for compatibility
4. **Migration support**: Data migration during updates