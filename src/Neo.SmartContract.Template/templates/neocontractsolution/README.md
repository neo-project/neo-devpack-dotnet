# NeoContractSolution

A Neo N3 smart contract solution with:
- Smart contract projects (in `src/`)
- Unit tests (in `tests/`)
- Simple deployment (in `deploy/`)

## Quick Start

### 1. Build and Test
```bash
dotnet build
dotnet test
```

### 2. Configure Wallets
Create wallet files for each network:
```bash
cd deploy/Deploy/wallets

# Create development wallet (for local testing)
neo-cli create wallet development.json

# Create TestNet wallet (for TestNet deployment)  
neo-cli create wallet testnet.json

# Create MainNet wallet (for production)
neo-cli create wallet mainnet.json
```

### 3. Deploy
```bash
cd deploy/Deploy

# Development (uses wallets/development.json)
dotnet run

# TestNet (uses wallets/testnet.json)
ENVIRONMENT=TestNet dotnet run

# MainNet (uses wallets/mainnet.json)
ENVIRONMENT=MainNet dotnet run
```

## Configuration

Edit `deploy/Deploy/appsettings.json`:

```json
{
  "Network": {
    "RpcUrl": "http://localhost:10332",
    "Network": "private",
    "Wallet": {
      "WalletPath": "wallets/development.json",
      "Password": "your-development-password"
    }
  },
  "Contracts": [
    {
      "Name": "MyContract",
      "InitParams": []
    }
  ]
}
```

**Network-Specific Configuration:**
- `appsettings.Development.json` → `wallets/development.json`
- `appsettings.TestNet.json` → `wallets/testnet.json` 
- `appsettings.MainNet.json` → `wallets/mainnet.json`

## Features

- ✅ **Automatic compilation** - Builds contracts before deployment
- ✅ **Deploy/Update detection** - Automatically deploys new or updates existing contracts
- ✅ **Multi-network support** - Same contracts, different networks
- ✅ **Deployment tracking** - Remembers contract addresses per network
- ✅ **Neo Express support** - Optional for local development

## Adding Contracts

### Single Contract (No Initialization)
```json
"Contracts": [
  {
    "Name": "SimpleStorage",
    "InitParams": []
  }
]
```

### Multiple Contracts with Dependencies
```json
"Contracts": [
  {
    "Name": "TokenContract",
    "InitParams": ["MyToken", "MTK", 8, 1000000]
  },
  {
    "Name": "GovernanceContract", 
    "InitParams": ["{{TokenContract}}", "0x1234567890abcdef1234567890abcdef12345678"]
  },
  {
    "Name": "StakingContract",
    "InitParams": ["{{TokenContract}}", "{{GovernanceContract}}", 86400]
  }
]
```

**Contract References:**
- Use `{{ContractName}}` to reference previously deployed contracts
- The toolkit automatically replaces these with actual contract addresses
- Contracts are deployed in the order listed (dependencies first)

### Contract with Complex Initialization
```json
"Contracts": [
  {
    "Name": "NFTContract",
    "InitParams": [
      "My NFT Collection",
      "MNC", 
      "https://api.example.com/metadata/",
      "0x1234567890abcdef1234567890abcdef12345678"
    ]
  }
]
```

**Multiple Contract Deployment:**
- Contract `Name` must match the project folder in `src/`
- `InitParams` are passed to the contract's initialization method
- **Deployment Order**: List contracts in dependency order (dependencies first)
- **Contract References**: Use `{{ContractName}}` to reference other contracts
- **Automatic Resolution**: The toolkit replaces `{{ContractName}}` with actual contract addresses
- **Update Support**: When updating, the toolkit maintains contract references automatically
- **Network Isolation**: Contract addresses are tracked separately per network

**Common Patterns:**
- **Token + Governance**: Deploy token first, then governance with token reference
- **DeFi Ecosystem**: Oracle → Token → DEX → Farming (in dependency order)
- **NFT Marketplace**: NFT → Marketplace → Auction (each referencing the previous)
- **Multi-Contract Systems**: Any contract can reference multiple previously deployed contracts

## Security

### Wallet Security
- ⚠️ **NEVER commit wallet files** - they contain private keys
- Development: Safe to store passwords in config (test accounts only)
- TestNet: Use separate wallets from development  
- MainNet: **NEVER store passwords in config** - use environment variables

### Best Practices
- Different wallet for each network (dev/testnet/mainnet)
- Use `WALLET_PASSWORD` environment variable for production
- Deployment records stored locally in `deploy/.deployments/`
- The `wallets/` directory has `.gitignore` to protect wallet files