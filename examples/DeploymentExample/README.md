# NEO Smart Contract Deployment Example

This example demonstrates a complete workflow for developing, testing, and deploying NEO smart contracts using the Neo.SmartContract.Deploy toolkit. It includes both single contract and multi-contract deployment scenarios.

## Project Structure

```
DeploymentExample/
├── src/
│   └── DeploymentExample.Contract/     # Smart contract projects
│       ├── ExampleContract.cs          # Simple example contract
│       ├── TokenContract.cs            # NEP-17 fungible token
│       ├── NFTContract.cs              # NEP-11 non-fungible token  
│       ├── GovernanceContract.cs       # DAO governance system
│       └── DeploymentExample.Contract.csproj
├── tests/
│   └── DeploymentExample.Tests/        # Unit tests
│       ├── ExampleContract.cs          # Generated contract wrapper
│       ├── ExampleContractTests.cs     # Test cases
│       └── DeploymentExample.Tests.csproj
├── deploy/
│   └── DeploymentExample.Deploy/       # Deployment application
│       ├── Program.cs                  # Main deployment program
│       ├── MultiContractDeployer.cs    # Multi-contract deployment
│       ├── MultiContractTester.cs      # Integration tests
│       ├── deployment-manifest.json    # Batch deployment config
│       ├── appsettings.json           # Configuration
│       ├── wallet.json                # Sample wallet (DO NOT USE IN PRODUCTION)
│       └── DeploymentExample.Deploy.csproj
└── DeploymentExample.sln              # Solution file
```

## Prerequisites

1. .NET 9.0 SDK or later
2. Neo Express (for local testing)
3. Neo.SmartContract.Deploy toolkit

## Quick Start

### 1. Build the Solution

```bash
cd examples/DeploymentExample
dotnet build
```

### 2. Run Tests

```bash
dotnet test
```

### 3. Local Deployment with Neo Express

First, start Neo Express:

```bash
# Initialize Neo Express (if not already done)
neo-express create -f default.neo-express

# Start Neo Express
neo-express run
```

In another terminal, deploy the contract:

```bash
cd deploy/DeploymentExample.Deploy

# Deploy single contract
dotnet run

# Deploy multiple contracts
dotnet run multi

# Deploy from manifest
dotnet run manifest
```

## Deployment Modes

### Single Contract Deployment

Deploy the simple example contract:

```bash
dotnet run single local
```

This deploys a basic contract with:
- Owner management
- Counter functionality
- Event emissions

### Multi-Contract Deployment

Deploy an entire ecosystem of interrelated contracts:

```bash
dotnet run multi local
```

This deploys:
1. **Token Contract (NEP-17)**: Fungible token with governance integration
2. **NFT Contract (NEP-11)**: NFTs with token-based minting fees
3. **Governance Contract**: DAO system managing other contracts

The deployment automatically:
- Handles dependencies between contracts
- Configures cross-contract permissions
- Initializes governance relationships
- Tests all integrations

### Manifest-Based Deployment

Deploy using a configuration file:

```bash
dotnet run manifest local
```

The `deployment-manifest.json` defines:
- Contract deployment order
- Dependencies and references
- Initialization parameters
- Post-deployment actions

### Testing Deployed Contracts

Test existing contracts:

```bash
dotnet run test <token_hash> <nft_hash> <governance_hash>
```

## Multi-Contract Architecture

### Token Contract (NEP-17)
- Standard fungible token implementation
- Minting controlled by governance
- Pausable transfers
- Burn functionality

### NFT Contract (NEP-11)
- Non-fungible token implementation
- Minting requires token payment
- Metadata and properties support
- URI-based token information

### Governance Contract
- Proposal and voting system
- Voting power based on token holdings
- Manages other contracts
- Configurable voting parameters

### Contract Interactions

```
┌─────────────────┐
│   Governance    │
│    Contract     │
└────────┬────────┘
         │ Manages
    ┌────┴────┐
    │         │
    v         v
┌──────┐  ┌──────┐
│Token │  │ NFT  │
│ NEP17│  │ NEP11│
└──┬───┘  └───┬──┘
   │          │
   └──────────┘
    Payment for
     Minting
```

## Configuration

### Network Settings

The deployment toolkit supports multiple networks:

- **local**: Neo Express local blockchain (default)
- **testnet**: NEO TestNet
- **mainnet**: NEO MainNet
- **Custom RPC**: Any custom RPC endpoint URL

### Wallet Configuration

**IMPORTANT**: The included `wallet.json` is for demonstration only. For production:

1. Generate a new wallet:
   ```bash
   neo-express wallet create deployer-wallet
   ```

2. Update `appsettings.json` with your wallet details:
   ```json
   {
     "Wallet": {
       "Path": "path/to/your/wallet.json",
       "Password": "your-secure-password"
     }
   }
   ```

3. Fund your wallet with GAS for deployment

### Environment Variables

You can override configuration using environment variables:

```bash
# Network configuration
export NEO_NETWORK=testnet
export Network__RpcUrl=https://custom.rpc.url

# Wallet configuration
export NEO_WALLET_PATH=/path/to/wallet.json
export NEO_WALLET_PASSWORD=your-password
```

## Testing

The test project uses Neo.SmartContract.Testing framework to:

- Test contract deployment
- Verify state changes
- Check event emissions
- Ensure access control
- Test cross-contract calls
- Measure code coverage

Run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Multi-Contract Deployment Workflow

1. **Deploy Token Contract**
   - Initialize with deployer as owner
   - Set initial supply

2. **Deploy NFT Contract**
   - Link to token contract for payments
   - Set minting price

3. **Deploy Governance Contract**
   - Link to token for voting power
   - Initialize voting parameters

4. **Configure Relationships**
   - Set governance on token contract
   - Add contracts to governance management

5. **Initialize Governance**
   - Create initial proposals
   - Set up management structure

## Troubleshooting

### Common Issues

1. **"Insufficient GAS balance"**
   - Multi-contract deployment requires ~300 GAS
   - Fund your wallet address with sufficient GAS
   - For Neo Express: `neo-express transfer GAS 300 deployer-wallet`

2. **"Contract dependency not found"**
   - Ensure contracts are deployed in correct order
   - Check that dependency references are correct
   - Verify contract hashes in manifest

3. **"Cross-contract call failed"**
   - Verify contract permissions in manifest
   - Ensure contracts are properly initialized
   - Check that relationships are configured

## Security Best Practices

1. **Never commit real wallets** to version control
2. **Use environment variables** for sensitive configuration
3. **Test thoroughly** on testnet before mainnet deployment
4. **Audit all contracts** before production deployment
5. **Verify cross-contract permissions** carefully
6. **Implement proper access controls** in governance
7. **Use time delays** for critical governance actions

## Example Usage Scenarios

### Token Distribution
```csharp
// Mint tokens to multiple addresses
await governance.CreateProposal(
    ProposalType.MintTokens,
    new[] { address1, address2 },
    "Initial token distribution"
);
```

### NFT Collection Launch
```csharp
// Set NFT minting parameters
await nft.SetMintPrice(20_00000000); // 20 tokens
await nft.SetMaxSupply(10000);
```

### Governance Actions
```csharp
// Pause token transfers
await governance.CreateProposal(
    ProposalType.ExecuteAction,
    new object[] { tokenContract, "setPaused", true },
    "Emergency pause"
);
```

## Deployed Testnet Contracts

The example has been successfully deployed to NEO TestNet for demonstration purposes:

### Latest Deployment (TestNet)

| Contract | Address | Transaction Hash | Network |
|----------|---------|------------------|---------|
| **TokenContract** | `0x2db2dce76b4a7f8116ecfae0d819e7099cb3a256` | `0x29397173e0e5ad93010f721ff6f786171e32e84f9bc44067072df332853e9554` | TestNet |
| **NFTContract** | `0x8699c5d074fc27cdbd7caec486387c1a29300536` | `0xc0b8c4b7dd68525da2adc39455252f32ac884947aeaba387c6ce20fa017952b7` | TestNet |
| **GovernanceContract** | `0xa3db58df3764610e43f3fda0c7b8633636c6c147` | `0x43d4a849396ddaf6b9917a3aeb30e49929a15fa32ebf51b1ab4b2d9e46dcf5a8` | TestNet |

**Deployment Details:**
- **Deployer Address**: `NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX` (`0x0c3146e78efc42bfb7d4cc2e06e3efd063c01c56`)
- **Network**: NEO TestNet
- **RPC Endpoint**: `https://testnet1.neo.coz.io:443`
- **Total GAS Consumed**: ~30.00233163 GAS
- **Deployment Mode**: Multi-contract with dependencies

### Contract Features Verified

#### TokenContract (NEP-17)
- **Symbol**: `EXT`
- **Decimals**: `8`
- **Total Supply**: `100,000,000 EXT`
- **Status**: ✅ Deployed and initialized

#### NFTContract (NEP-11)
- **Symbol**: `EXNFT`
- **Decimals**: `0`
- **Total Supply**: `0 NFTs`
- **Status**: ✅ Deployed and initialized

#### GovernanceContract
- **Type**: DAO Governance System
- **Status**: ✅ Deployed and initialized

### Test the Deployed Contracts

You can test the deployed contracts using the deploy project:

```bash
cd deploy/DeploymentExample.Deploy

# Test existing deployed contracts
dotnet run test 0x2db2dce76b4a7f8116ecfae0d819e7099cb3a256 0x8699c5d074fc27cdbd7caec486387c1a29300536 0xa3db58df3764610e43f3fda0c7b8633636c6c147 testnet
```

**Note**: These are demonstration contracts deployed on TestNet. Do not use them for production purposes.

## Next Steps

- Customize contracts for your use case
- Add additional contract types (DEX, Staking, etc.)
- Implement frontend for user interaction
- Set up monitoring and analytics
- Create automated deployment pipelines

## Resources

- [NEO Documentation](https://docs.neo.org/)
- [NEO Developer Portal](https://developers.neo.org/)
- [Neo.SmartContract.Deploy Documentation](https://github.com/neo-project/neo-devpack-dotnet)
- [NEP Standards](https://github.com/neo-project/proposals)