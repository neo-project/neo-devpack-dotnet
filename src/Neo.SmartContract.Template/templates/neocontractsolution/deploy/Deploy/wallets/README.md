# Wallet Files

This directory contains network-specific wallet files for deployment.

## Structure

```
wallets/
├── development.json    # Local development wallet
├── testnet.json       # TestNet wallet  
├── mainnet.json       # MainNet wallet (PRODUCTION)
└── README.md          # This file
```

## Security Guidelines

⚠️ **CRITICAL**: Wallet files contain private keys and should NEVER be committed to version control.

### Development Wallets
- Safe to store passwords in config for local development
- Use test accounts with no real value
- Can be shared among team members for local testing

### TestNet Wallets  
- Use separate wallet from development
- Contains TestNet GAS for testing
- Passwords can be in config or environment variables
- Still no real monetary value

### MainNet Wallets
- **NEVER store passwords in config files**
- Use secure password management
- Consider hardware wallets for high-value deployments
- Implement multi-signature for production contracts

## Creating Wallets

### Using Neo CLI
```bash
# Create new wallet
neo-cli create wallet development.json

# Import existing account
neo-cli import key development.json
```

### Using Neo GUI
1. File → New Wallet
2. Save in appropriate wallets/ subdirectory
3. Configure passwords according to network security requirements

## Environment Variables

For production deployments, use environment variables instead of config files:

```bash
export WALLET_PASSWORD="your-secure-password"
```

The deployment toolkit supports both config-based and environment variable passwords.