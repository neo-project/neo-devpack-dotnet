# NEO Smart Contract Deployment Example

This example demonstrates a complete workflow for developing, testing, and deploying NEO smart contracts using the Neo.SmartContract.Deploy toolkit.

## Project Structure

```
DeploymentExample/
├── src/
│   └── DeploymentExample.Contract/     # Smart contract project
│       ├── ExampleContract.cs          # Contract implementation
│       └── DeploymentExample.Contract.csproj
├── tests/
│   └── DeploymentExample.Tests/        # Unit tests
│       ├── ExampleContract.cs          # Generated contract wrapper
│       ├── ExampleContractTests.cs     # Test cases
│       └── DeploymentExample.Tests.csproj
├── deploy/
│   └── DeploymentExample.Deploy/       # Deployment application
│       ├── Program.cs                  # Deployment logic
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
dotnet run
```

### 4. TestNet/MainNet Deployment

Set up your environment:

```bash
# For TestNet
export NEO_NETWORK=testnet

# For MainNet
export NEO_NETWORK=mainnet

# Run deployment
dotnet run
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

## Contract Features

The example contract demonstrates:

1. **Owner Management**: Deploy with owner, transfer ownership
2. **State Management**: Counter that can be incremented
3. **Events**: Emit events for important state changes
4. **Safe Methods**: Read-only methods that don't modify state
5. **Access Control**: Owner-only operations
6. **Contract Lifecycle**: Update and destroy capabilities

## Testing

The test project uses Neo.SmartContract.Testing framework to:

- Test contract deployment
- Verify state changes
- Check event emissions
- Ensure access control
- Measure code coverage

Run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment Workflow

1. **Compile**: The toolkit automatically compiles the contract project
2. **Deploy**: Send deployment transaction with initialization parameters
3. **Verify**: Check deployment status and contract state
4. **Invoke**: Test contract methods

## Troubleshooting

### Common Issues

1. **"Deployer account has no GAS"**
   - Fund your wallet address with GAS
   - For Neo Express: `neo-express transfer GAS 100 deployer-wallet`

2. **"Cannot connect to RPC server"**
   - Ensure Neo Express is running for local deployment
   - Check network connectivity for testnet/mainnet
   - Verify RPC URL in configuration

3. **"Invalid wallet password"**
   - Check password in appsettings.json or environment variable
   - Ensure wallet file exists and is accessible

## Security Best Practices

1. **Never commit real wallets** to version control
2. **Use environment variables** for sensitive configuration
3. **Test thoroughly** on testnet before mainnet deployment
4. **Audit your contracts** before production deployment
5. **Keep private keys secure** and use hardware wallets when possible

## Next Steps

- Modify the contract to add your own functionality
- Integrate with your application using Neo .NET SDK
- Set up CI/CD pipeline for automated deployment
- Monitor contract usage and events

## Resources

- [NEO Documentation](https://docs.neo.org/)
- [NEO Developer Portal](https://developers.neo.org/)
- [Neo.SmartContract.Deploy Documentation](https://github.com/neo-project/neo-devpack-dotnet)