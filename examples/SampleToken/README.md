# Sample Token - Complete Neo Contract Solution

This example demonstrates a complete Neo smart contract solution with:
- **Contract Project**: NEP-17 token implementation
- **Test Project**: Comprehensive unit tests
- **Deploy Project**: Deployment and management tools

## Project Structure

```
SampleToken/
├── SampleToken.sln                    # Solution file
├── src/
│   └── SampleToken.Contract/          # Smart contract project
│       ├── SampleToken.cs             # NEP-17 token implementation
│       └── SampleToken.Contract.csproj
├── tests/
│   └── SampleToken.Tests/             # Unit test project
│       ├── SampleTokenTests.cs        # Contract tests
│       └── SampleToken.Tests.csproj
└── deploy/
    └── SampleToken.Deploy/            # Deployment project
        ├── Program.cs                 # Deployment logic
        ├── appsettings.json          # Default configuration
        ├── appsettings.TestNet.json  # TestNet configuration
        ├── appsettings.MainNet.json  # MainNet configuration
        └── config/
            └── neo-express.json      # Neo Express configuration
```

## Features

### Smart Contract (NEP-17 Token)
- **Standard Compliance**: Full NEP-17 implementation
- **Minting**: Controlled token minting with max supply limit
- **Ownership**: Transferable ownership model
- **Minter Management**: Delegate minting permissions
- **Events**: Transfer, ownership, and minter change events

### Unit Tests
- Deploy and initialization tests
- Transfer functionality tests
- Minting and supply management tests
- Permission and ownership tests
- Edge case handling

### Deployment Tool
- Configuration-based deployment (JSON settings)
- Multi-network support (Private, TestNet, MainNet)
- Wallet integration
- Post-deployment verification
- Initial setup automation

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Neo.SmartContract.Framework
- Neo.SmartContract.Testing
- Neo.SmartContract.Deploy

### Build the Solution

```bash
cd SampleToken
dotnet build
```

### Run Tests

```bash
dotnet test tests/SampleToken.Tests
```

### Deploy the Contract

1. **Create a wallet** (if you don't have one):
   ```bash
   # Install Neo Express globally
   dotnet tool install -g Neo.Express
   
   # Create a new wallet
   neoxp wallet create wallet.json
   ```

2. **Configure deployment**:
   - Edit `deploy/SampleToken.Deploy/appsettings.json`
   - Set your wallet path and password
   - Configure network settings

3. **Run deployment**:
   ```bash
   cd deploy/SampleToken.Deploy
   
   # For local/private network
   dotnet run
   
   # For TestNet
   ASPNETCORE_ENVIRONMENT=TestNet dotnet run
   
   # For MainNet
   ASPNETCORE_ENVIRONMENT=MainNet dotnet run
   ```

## Token Details

- **Name**: Sample Token
- **Symbol**: SAMPLE
- **Decimals**: 8
- **Initial Supply**: 100,000,000 SAMPLE
- **Max Supply**: 1,000,000,000 SAMPLE

## Contract Methods

### NEP-17 Standard Methods
- `getName()` - Get token name
- `getSymbol()` - Get token symbol
- `getDecimals()` - Get decimal places
- `totalSupply()` - Get total token supply
- `balanceOf(account)` - Get account balance
- `transfer(from, to, amount, data)` - Transfer tokens

### Additional Methods
- `getOwner()` - Get contract owner
- `transferOwnership(newOwner)` - Transfer ownership
- `mint(to, amount)` - Mint new tokens
- `setMinter(account, isMinter)` - Set minting permission
- `isMinter(account)` - Check if account can mint
- `getMaxSupply()` - Get maximum supply limit
- `getRemainingSupply()` - Get remaining mintable tokens

## Configuration Options

### appsettings.json
```json
{
  "Network": {
    "RpcUrl": "http://localhost:50012",  // RPC endpoint
    "Network": "private"                  // Network type
  },
  "Wallet": {
    "Path": "wallet.json",               // Wallet file path
    "Password": "your-password"          // Wallet password
  },
  "Deployment": {
    "GasLimit": 100000000,              // Max GAS for deployment
    "WaitForConfirmation": true,        // Wait for tx confirmation
    "ConfirmationRetries": 10,          // Retry attempts
    "ConfirmationDelaySeconds": 5       // Delay between retries
  }
}
```

## Development Workflow

1. **Modify Contract**: Edit `src/SampleToken.Contract/SampleToken.cs`
2. **Test Changes**: Run `dotnet test` to verify functionality
3. **Deploy**: Use the deployment tool to deploy to desired network
4. **Verify**: The deployment tool automatically verifies the deployment

## Security Considerations

- Only the owner can mint tokens or assign minters
- Minting is limited by max supply cap
- All transfers require proper witness verification
- Invalid addresses and amounts are rejected

## License

This example is provided as-is for educational purposes.