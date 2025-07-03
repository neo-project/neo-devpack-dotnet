# Wallet Directory

Place your NEP-6 wallet files here for different environments:

- `development.json` - Local development wallet (Neo Express or private net)
- `testnet.json` - TestNet deployment wallet  
- `mainnet.json` - MainNet deployment wallet

## Security Notes

- **NEVER** commit wallet files to version control
- **NEVER** store MainNet passwords in configuration files
- Use environment variables for sensitive passwords:
  ```bash
  export NEO_WALLET_PASSWORD="your-secure-password"
  ```

## Wallet Creation

To create a new wallet for deployment:

```bash
# Using Neo CLI
neo-cli wallet create wallets/testnet.json

# Using Neo Express (for local development)
neoxp wallet create wallets/development.json
```

## .gitignore

This directory should be in your `.gitignore`:

```
wallets/*.json
!wallets/README.md
```