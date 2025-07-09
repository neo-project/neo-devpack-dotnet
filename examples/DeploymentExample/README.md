# Neo Smart Contract Deployment Example

This comprehensive example demonstrates how to use the Neo Contract Deployment Toolkit to deploy and manage smart contracts on the Neo blockchain.

## Overview

This solution contains multiple example contracts showing different aspects of Neo smart contract development and deployment:

1. **SimpleContract** - Basic smart contract with storage operations
2. **TokenContract** - NEP-17 fungible token implementation
3. **NFTContract** - NEP-11 non-fungible token implementation
4. **GovernanceContract** - Voting and governance functionality

## Project Structure

```
DeploymentExample/
├── src/                         # Smart contract implementations
│   ├── SimpleContract/          # Basic contract example
│   ├── TokenContract/           # NEP-17 token example
│   ├── NFTContract/             # NEP-11 NFT example
│   └── GovernanceContract/      # Governance example
├── tests/                       # Unit tests for each contract
│   ├── SimpleContract.Tests/
│   ├── TokenContract.Tests/
│   ├── NFTContract.Tests/
│   └── GovernanceContract.Tests/
├── deploy/                      # Deployment tools and scripts
│   └── DeploymentExample.Deploy/
├── scripts/                     # Automation scripts
│   ├── deploy-all.sh            # Deploy all contracts
│   ├── deploy-all.ps1           # Deploy all contracts (PowerShell)
│   ├── test-all.sh              # Run all tests
│   └── test-all.ps1             # Run all tests (PowerShell)
├── deployment-manifest.json     # Multi-contract deployment configuration
└── README.md                    # This file
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Neo blockchain development tools
- Neo Express (for local testing)
- A Neo wallet with testnet GAS

### Installation

1. Clone the repository
2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Build all contracts:
   ```bash
   dotnet build
   ```

### Running Tests

Run all unit tests:
```bash
dotnet test
```

Or use the test script:
```bash
./scripts/test-all.sh
```

## Deployment Examples

### 1. Deploy a Single Contract

Deploy the SimpleContract to testnet:

```bash
cd deploy/DeploymentExample.Deploy
dotnet run -- deploy -n testnet -w YOUR_WIF_KEY -c ../../src/SimpleContract/SimpleContract.csproj
```

### 2. Deploy Multiple Contracts

Use the deployment manifest to deploy all contracts in order:

```bash
cd deploy/DeploymentExample.Deploy
dotnet run -- deploy-manifest -n testnet -w YOUR_WIF_KEY -m ../../deployment-manifest.json
```

### 3. Using the Deployment Toolkit Programmatically

```csharp
using Neo.SmartContract.Deploy;

// Create toolkit instance
using var toolkit = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("YOUR_WIF_KEY");

// Deploy a contract
var result = await toolkit.DeployAsync("src/SimpleContract/SimpleContract.csproj");
Console.WriteLine($"Contract deployed at: {result.ContractAddress}");

// Invoke a method
await toolkit.InvokeAsync(result.ContractHash, "storeData", "key", "value");

// Call a read-only method
var value = await toolkit.CallAsync<string>(result.ContractHash, "getData", "key");
```

### 4. Deploy with Initialization Parameters

For contracts that require initialization:

```bash
dotnet run -- deploy -n testnet -w YOUR_WIF_KEY \
    -c ../../src/TokenContract/TokenContract.csproj \
    --init-params "MyToken" "MYT" 8 10000000
```

### 5. Update an Existing Contract

```bash
dotnet run -- update -n testnet -w YOUR_WIF_KEY \
    -h 0x1234567890abcdef \
    -c ../../src/SimpleContract/SimpleContract.csproj
```

## Contract Examples

### SimpleContract

Basic contract demonstrating:
- Storage operations
- Events
- Access control
- Contract updates

```csharp
// Store data
await toolkit.InvokeAsync(contractHash, "storeData", "name", "Neo");

// Retrieve data
var name = await toolkit.CallAsync<string>(contractHash, "getData", "name");
```

### TokenContract (NEP-17)

Fungible token with:
- Standard NEP-17 methods
- Minting and burning
- Pausable functionality
- Transfer events

```csharp
// Check balance
var balance = await toolkit.CallAsync<BigInteger>(contractHash, "balanceOf", address);

// Transfer tokens
await toolkit.InvokeAsync(contractHash, "transfer", fromAddress, toAddress, amount, null);
```

### NFTContract (NEP-11)

Non-fungible token with:
- Minting with properties
- Token enumeration
- Ownership tracking
- Metadata storage

```csharp
// Mint NFT
var properties = new Dictionary<string, object>
{
    ["name"] = "My NFT",
    ["image"] = "https://example.com/nft.png"
};
await toolkit.InvokeAsync(contractHash, "mint", toAddress, properties);

// Get NFT owner
var owner = await toolkit.CallAsync<string>(contractHash, "ownerOf", tokenId);
```

### GovernanceContract

Governance system with:
- Proposal creation
- Voting mechanism
- Execution of passed proposals
- Configurable parameters

```csharp
// Create proposal
await toolkit.InvokeAsync(contractHash, "createProposal", 
    "Upgrade Protocol", 
    "Proposal to upgrade the protocol version",
    "QmIPFSHash123",
    actions);

// Vote on proposal
await toolkit.InvokeAsync(contractHash, "vote", proposalId, true);
```

## Security Features

All example contracts include optional security features:

1. **Access Control** - Owner and role-based permissions
2. **Multi-signature Support** - For critical operations
3. **Pausable Functionality** - Emergency pause mechanism
4. **Input Validation** - Parameter checking and sanitization
5. **Reentrancy Protection** - Safe external calls

## Deployment Manifest

The `deployment-manifest.json` file defines:
- Contract deployment order
- Initialization parameters
- Dependencies between contracts
- Post-deployment verification

Example:
```json
{
  "contracts": [
    {
      "name": "TokenContract",
      "projectPath": "src/TokenContract/TokenContract.csproj",
      "initParams": ["MyToken", "MYT", 8, 10000000],
      "deploymentOrder": 1
    },
    {
      "name": "GovernanceContract",
      "projectPath": "src/GovernanceContract/GovernanceContract.csproj",
      "dependencies": ["TokenContract"],
      "deploymentOrder": 2
    }
  ]
}
```

## Advanced Scenarios

### 1. Multi-Environment Deployment

Use different configuration files for each environment:

```bash
# Development
dotnet run -- deploy --config appsettings.Development.json

# Staging
dotnet run -- deploy --config appsettings.Staging.json

# Production
dotnet run -- deploy --config appsettings.Production.json
```

### 2. Automated Testing and Deployment

Use the provided GitHub Actions workflow:
```yaml
- name: Deploy Contracts
  run: |
    dotnet run --project deploy/DeploymentExample.Deploy -- \
      deploy-manifest -n testnet -m deployment-manifest.json
```

### 3. Contract Verification

After deployment, verify the contract:
```bash
dotnet run -- verify -n testnet -h CONTRACT_HASH
```

## Troubleshooting

### Common Issues

1. **Insufficient GAS**: Ensure your wallet has enough GAS for deployment
2. **Network Connection**: Check RPC endpoint availability
3. **Contract Size**: Optimize contract code if too large
4. **Compilation Errors**: Verify all dependencies are installed

### Debug Mode

Enable detailed logging:
```bash
dotnet run -- deploy -v --log-level Debug
```

## Resources

- [Neo Documentation](https://docs.neo.org/)
- [NEP Standards](https://github.com/neo-project/proposals)
- [Neo Developer Portal](https://developers.neo.org/)
- [Deployment Toolkit Documentation](../docs/deployment-toolkit.md)

## License

This example is provided under the MIT License.