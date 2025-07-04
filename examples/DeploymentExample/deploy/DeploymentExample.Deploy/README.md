# ExampleContract Deployment

This project demonstrates how to deploy and interact with Neo smart contracts using the simplified deployment toolkit.

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
   # Deploy to default network (from appsettings.json)
   dotnet run
   
   # Deploy to specific network
   dotnet run local
   dotnet run testnet
   dotnet run mainnet
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

## What This Example Does

1. **Deploys the ExampleContract** with the deployer as initial owner
2. **Tests contract methods**:
   - Gets contract info
   - Reads and increments a counter
   - Tests the multiply function
   - Verifies ownership

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

1. Ensure your wallet has sufficient GAS for deployment (typically 10-20 GAS)
2. Verify network connectivity to RPC nodes
3. Check that wallet password is correctly set

## Security Notes

- Never commit wallet files to version control
- Use environment variables for sensitive data
- Test thoroughly on TestNet before MainNet deployment