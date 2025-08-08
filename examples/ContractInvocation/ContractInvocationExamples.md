# Contract Invocation Examples

This directory contains comprehensive examples demonstrating the Neo Smart Contract Framework's contract invocation system.

## Examples Overview

### 1. SimpleExample.cs
Basic contract invocation patterns including:
- Referencing deployed system contracts (NEO, GAS)
- Calling contract methods with proper error handling
- Handling contracts that might not be deployed yet
- Batch operations and validation

**Key Features:**
- Safe contract calls with comprehensive error handling
- Default value handling for undeployed contracts
- Input validation and contract health checks

### 2. ComprehensiveExample.cs
Advanced contract invocation scenarios including:
- Multi-network contract references
- Development vs deployed contracts
- Contract proxy pattern implementation
- Dynamic contract selection
- Network switching capabilities
- Fallback strategies for unresolved contracts

**Key Features:**
- Support for contracts across privnet, testnet, and mainnet
- Handling contracts in development (not yet deployed)
- NEP-17 token proxy implementation
- Safe contract calls with fallback mechanisms

### 3. DynamicInvocationExample.cs
Dynamic and runtime contract invocation patterns:
- Runtime contract registration and discovery
- Contract factory patterns
- Contract upgrade scenarios
- Migration strategies
- Contract versioning

**Key Features:**
- Dynamic contract registry
- Factory pattern for deploying contract instances
- Contract upgrade and migration support
- Contract discovery based on patterns

### 4. MyToken.cs
Example token contract that can be referenced by other contracts.

## Usage Patterns

### Basic Contract Reference
```csharp
[ContractReference("NEO",
    PrivnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
    TestnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
    MainnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
private static IContractReference? NeoContract;
```

### Development Contract Reference
```csharp
[ContractReference("MyDevelopmentContract",
    ReferenceType = ContractReferenceType.Development,
    ProjectPath = "../MyToken/MyToken.csproj")]
private static IContractReference? DevelopmentContract;
```

### Safe Contract Calls
```csharp
// Check if contract is deployed
if (!contract.IsResolved)
{
    return GetDefaultValue();
}

// Make the call
var result = Contract.Call(contract.ResolvedHash!, "method", CallFlags.ReadOnly, args);
```

### Multi-Network Support
```csharp
// Switch all contracts to a different network
ContractInvocationFactory.SwitchNetwork("testnet");

// Check which contracts are available on current network
var isAvailable = contract.IsResolved;
```

## Best Practices

1. **Always Check IsResolved**: Before calling a contract, verify it's resolved on the current network
2. **Handle Errors Gracefully**: Provide appropriate defaults or fallbacks for unresolved contracts
3. **Use Contract Proxies**: For complex contracts, implement proxy classes for type safety
4. **Validate Inputs**: Always validate addresses and parameters before making contract calls
5. **Log Important Events**: Use Runtime.Log for debugging and monitoring
6. **Consider Network Differences**: Contracts may have different addresses or availability across networks

## Common Scenarios

### Calling a Contract That May Not Be Deployed
```csharp
if (!contract.IsResolved)
{
    Runtime.Log($"Contract not deployed on {contract.NetworkContext.CurrentNetwork}");
    return defaultValue;
}
```

### Dynamic Contract Discovery
```csharp
var contractHash = GetRegisteredContract(identifier, network);
var contract = ContractInvocationFactory.RegisterDeployedContract(
    identifier, contractHash, network);
```

### Contract Migration
```csharp
// Deploy new version
var newContract = ContractManagement.Deploy(newNef, newManifest);

// Migrate data from old contract
Contract.Call(oldContract, "migrate", CallFlags.All, newContract);

// Update registry
RegisterContract(identifier, newContract, network);
```

## Testing

When testing contracts that use the invocation system:

1. Mock contract references in unit tests
2. Test behavior when contracts are not resolved
3. Verify error handling for failed contract calls
4. Test multi-network scenarios
5. Validate contract upgrade paths

## Security Considerations

1. **Validate Contract Hashes**: Always verify contract addresses before registration
2. **Access Control**: Implement proper authorization for contract registration/updates
3. **Input Validation**: Validate all inputs to prevent injection attacks
4. **Error Information**: Don't expose sensitive information in error messages
5. **Upgrade Safety**: Ensure contract upgrades maintain data integrity

## Additional Resources

- [Neo Smart Contract Framework Documentation](https://docs.neo.org/docs/n3/develop/write/neocontractsystem)
- [NEP-17 Token Standard](https://github.com/neo-project/proposals/blob/master/nep-17.mediawiki)
- [Contract Invocation System Design](../../src/Neo.SmartContract.Framework/ContractInvocation/README.md)