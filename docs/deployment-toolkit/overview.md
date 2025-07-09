# Neo Smart Contract Deployment Toolkit Overview

The Neo Smart Contract Deployment Toolkit is a comprehensive solution for deploying and managing smart contracts on the Neo blockchain. It provides a simplified API, command-line tools, and automation features to streamline the deployment process.

## Features

### Core Features
- **Single Contract Deployment**: Deploy individual contracts with ease
- **Multi-Contract Deployment**: Deploy multiple interdependent contracts
- **Contract Updates**: Update existing contracts safely
- **Contract Verification**: Verify deployed contracts
- **Transaction Management**: Handle transaction signing and submission
- **Network Support**: Works with MainNet, TestNet, and private networks

### Advanced Features
- **Dependency Resolution**: Automatically handles contract dependencies
- **Initialization Support**: Pass parameters during deployment
- **Gas Optimization**: Estimates and optimizes gas usage
- **Security Features**: Multi-signature support and access control
- **Deployment Manifests**: JSON-based deployment configuration
- **Automation Support**: CI/CD integration ready

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                  DeploymentToolkit                   │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌──────────────┐  ┌───────────────┐  ┌──────────┐│
│  │   Compiler   │  │   Deployer    │  │ Invoker  ││
│  │   Service    │  │   Service     │  │ Service  ││
│  └──────────────┘  └───────────────┘  └──────────┘│
│                                                     │
│  ┌──────────────┐  ┌───────────────┐  ┌──────────┐│
│  │   Wallet     │  │   Contract    │  │  Multi   ││
│  │   Manager    │  │   Updater     │  │ Deployer ││
│  └──────────────┘  └───────────────┘  └──────────┘│
│                                                     │
└─────────────────────────────────────────────────────┘
                           │
                           ▼
                    ┌─────────────┐
                    │ Neo Network │
                    └─────────────┘
```

## Getting Started

### Installation

```bash
# Install as a global tool
dotnet tool install -g Neo.SmartContract.Deploy.CLI

# Or add to your project
dotnet add package Neo.SmartContract.Deploy
```

### Basic Usage

#### Using the Toolkit Programmatically

```csharp
using Neo.SmartContract.Deploy;

// Create and configure toolkit
using var toolkit = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("YOUR_WIF_KEY");

// Deploy a contract
var result = await toolkit.DeployAsync("path/to/contract.csproj");
Console.WriteLine($"Contract deployed at: {result.ContractAddress}");
```

#### Using the CLI

```bash
# Deploy a single contract
neo-deploy deploy -n testnet -w YOUR_WIF_KEY -c MyContract.csproj

# Deploy from manifest
neo-deploy deploy-manifest -n testnet -w YOUR_WIF_KEY -m deployment.json

# Update a contract
neo-deploy update -n testnet -w YOUR_WIF_KEY -h CONTRACT_HASH -c MyContract.csproj
```

## Key Components

### 1. DeploymentToolkit

The main entry point for all deployment operations:

```csharp
public class DeploymentToolkit : IDisposable
{
    // Network configuration
    public DeploymentToolkit SetNetwork(string network);
    public DeploymentToolkit SetWifKey(string wifKey);
    
    // Deployment operations
    public Task<ContractDeploymentInfo> DeployAsync(string path, object[]? initParams = null);
    public Task<MultiContractDeploymentResult> DeployFromManifestAsync(string manifestPath);
    
    // Contract operations
    public Task<T> CallAsync<T>(string contractHash, string method, params object[] args);
    public Task<UInt256> InvokeAsync(string contractHash, string method, params object[] args);
    
    // Update operations
    public Task<ContractUpdateInfo> UpdateAsync(string contractHash, string path, object[]? updateParams = null);
}
```

### 2. Deployment Manifest

JSON configuration for multi-contract deployments:

```json
{
  "name": "My DApp Deployment",
  "contracts": [
    {
      "name": "TokenContract",
      "projectPath": "src/Token/Token.csproj",
      "initParams": ["MyToken", "MTK", 8, 1000000],
      "deploymentOrder": 1
    },
    {
      "name": "GovernanceContract",
      "projectPath": "src/Governance/Governance.csproj",
      "dependencies": ["TokenContract"],
      "deploymentOrder": 2
    }
  ]
}
```

### 3. Deployment Options

Configure deployment behavior:

```csharp
public class DeploymentOptions
{
    public string WifKey { get; set; }
    public string RpcUrl { get; set; }
    public uint NetworkMagic { get; set; }
    public bool WaitForConfirmation { get; set; }
    public bool VerifyAfterDeploy { get; set; }
    public long GasLimit { get; set; }
}
```

## Deployment Workflow

1. **Compilation**: Contracts are compiled to NEF and manifest files
2. **Validation**: Contract size and structure are validated
3. **Gas Estimation**: Required GAS is calculated
4. **Transaction Creation**: Deployment transaction is constructed
5. **Signing**: Transaction is signed with the provided key
6. **Submission**: Transaction is submitted to the network
7. **Confirmation**: Wait for transaction confirmation
8. **Verification**: Verify contract is deployed correctly

## Security Considerations

### Best Practices
- Always verify contracts after deployment
- Use multi-signature wallets for production deployments
- Test thoroughly on testnet before mainnet deployment
- Keep deployment keys secure and never commit them to source control

### Multi-Signature Support

```csharp
var options = new DeploymentOptions
{
    RequireMultiSig = true,
    MinSignatures = 2,
    Signers = new[] { signer1, signer2, signer3 }
};

var result = await toolkit.DeployAsync("contract.csproj", options);
```

## Error Handling

The toolkit provides detailed error information:

```csharp
try
{
    var result = await toolkit.DeployAsync("contract.csproj");
}
catch (DeploymentException ex)
{
    Console.WriteLine($"Deployment failed: {ex.Message}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
    Console.WriteLine($"Transaction: {ex.TransactionHash}");
}
```

## Performance Optimization

### Gas Optimization
- The toolkit automatically estimates gas requirements
- Provides warnings for high gas consumption
- Suggests optimizations when possible

### Batch Operations
- Deploy multiple contracts in a single session
- Reuse RPC connections for efficiency
- Parallel compilation support

## Integration

### CI/CD Integration

```yaml
# GitHub Actions example
- name: Deploy Contracts
  run: |
    neo-deploy deploy-manifest \
      -n ${{ vars.NETWORK }} \
      -w ${{ secrets.DEPLOY_WIF }} \
      -m deployment-manifest.json
```

### Docker Support

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
RUN dotnet tool install -g Neo.SmartContract.Deploy.CLI
ENTRYPOINT ["neo-deploy"]
```

## Next Steps

- [Single Contract Deployment Guide](single-deployment.md)
- [Multi-Contract Deployment Guide](../multi-contract-deployment.md)
- [Contract Update Guide](../contract-update.md)
- [API Reference](../api/deployment-toolkit.md)
- [Examples](../../examples/DeploymentExample/README.md)