# Neo Smart Contract Deployment Example

This project demonstrates how to deploy and interact with Neo smart contracts using the simplified deployment toolkit. It supports both single contract and multi-contract deployment scenarios.

## Quick Start

1. **Configure your environment**

   Select the appropriate configuration file:
   - `appsettings.Development.json` - Local Neo Express
   - `appsettings.TestNet.json` - Neo TestNet  
   - `appsettings.MainNet.json` - Neo MainNet

2. **Set up your wallet**

   Place your NEP-6 wallet in the `wallets/` directory:
   ```bash
   cp /path/to/your/wallet.json wallets/deployment.json
   ```

3. **Run the deployment**

   ```bash
   # Deploy single contract to default network
   dotnet run
   
   # Deploy multiple contracts with dependencies
   dotnet run multi
   
   # Deploy to specific networks
   dotnet run single local
   dotnet run multi testnet
   dotnet run manifest mainnet
   
   # Test existing deployed contracts
   dotnet run test <token_hash> <nft_hash> <governance_hash>
   ```

## Configuration

The deployment uses a layered configuration approach:

1. `appsettings.json` - Base configuration
2. `appsettings.{Environment}.json` - Environment-specific overrides
3. Environment variables - Runtime overrides

### Environment Variables

- `NEO_WALLET_PASSWORD` - Wallet password (recommended for production)
- `ASPNETCORE_ENVIRONMENT` - Select configuration file (Development, TestNet, MainNet)

Example:
```bash
export NEO_WALLET_PASSWORD="your-secure-password"
export ASPNETCORE_ENVIRONMENT="TestNet"
dotnet run
```

## Deployment Manifest

The `deployment-manifest.json` file allows batch deployment of multiple contracts:

```bash
# Deploy using manifest
dotnet run -- --manifest deployment-manifest.json
```

## Deployment Modes

### Single Contract Deployment
1. **Deploys SimpleContract** with the deployer as initial owner
2. **Tests contract methods**:
   - Gets counter value and increments it
   - Tests the multiply function
   - Verifies basic functionality

### Multi-Contract Deployment
1. **Deploys TokenContract (NEP-17)** - Fungible token with governance
2. **Deploys NFTContract (NEP-11)** - Non-fungible tokens with token-based minting
3. **Deploys GovernanceContract** - DAO system managing other contracts
4. **Configures cross-contract relationships** and dependencies
5. **Tests all integrations** to ensure ecosystem works correctly

## Deployed TestNet Example

For testing purposes, the multi-contract system has been deployed to NEO TestNet:

| Contract | Address | Network |
|----------|---------|---------|
| **TokenContract** | `0x2db2dce76b4a7f8116ecfae0d819e7099cb3a256` | TestNet |
| **NFTContract** | `0x8699c5d074fc27cdbd7caec486387c1a29300536` | TestNet |
| **GovernanceContract** | `0xa3db58df3764610e43f3fda0c7b8633636c6c147` | TestNet |

### Test the Deployed Contracts

```bash
# Test the deployed TestNet contracts
dotnet run test 0x2db2dce76b4a7f8116ecfae0d819e7099cb3a256 0x8699c5d074fc27cdbd7caec486387c1a29300536 0xa3db58df3764610e43f3fda0c7b8633636c6c147 testnet
```

## Troubleshooting

### Local Development

1. Start Neo Express:
   ```bash
   neoxp run
   ```

2. Create and fund a wallet:
   ```bash
   neoxp wallet create wallets/development.json
   neoxp transfer 100 GAS genesis wallets/development.json
   ```

### TestNet/MainNet

#### Single Contract Deployment
1. Ensure your wallet has sufficient GAS for deployment (typically 10-20 GAS)
2. Verify network connectivity to RPC nodes
3. Check that wallet password is correctly set

#### Multi-Contract Deployment
1. Ensure your wallet has sufficient GAS for deployment (typically 150+ GAS)
2. The deployment will deploy contracts in order: Token → NFT → Governance
3. Each contract requires ~10 GAS for deployment
4. Additional GAS needed for initialization and configuration transactions

## Security Notes

- Never commit wallet files to version control
- Use environment variables for sensitive data
- Test thoroughly on TestNet before MainNet deployment