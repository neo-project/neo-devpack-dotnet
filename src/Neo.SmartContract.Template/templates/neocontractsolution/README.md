# MyContract Neo Smart Contract Solution

This solution contains a Neo smart contract project created with the Neo Contract Development Toolkit.

## Project Structure

```
MyContract/
├── src/
│   └── MyContract/          # Smart contract implementation
#if (enableTests)
├── tests/
│   └── MyContract.Tests/    # Unit tests for the contract
#endif
#if (enableDeployment)
├── deploy/
│   └── MyContract.Deploy/   # Deployment toolkit project
├── deployment.json          # Deployment manifest
#endif
├── README.md               # This file
└── MyContract.sln          # Solution file
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Neo blockchain development tools
- Neo Express (for local development)

### Building the Contract

```bash
dotnet build
```

### Running Tests

#if (enableTests)
```bash
dotnet test
```
#else
Tests are not included in this project. You can add tests by creating a test project.
#endif

### Deploying the Contract

#if (enableDeployment)
The project includes deployment support. You can deploy using:

1. **Using the deployment toolkit:**
   ```bash
   cd deploy/MyContract.Deploy
   dotnet run -- deploy
   ```

2. **Using the deployment manifest:**
   ```bash
   dotnet run -- deploy-manifest deployment.json
   ```

See the deployment documentation for more options.
#else
Deployment support is not included. You can add deployment support by installing the Neo.SmartContract.Deploy package.
#endif

## Contract Details

- **Contract Type**: #(contractType)
- **Name**: MyContract
- **Framework**: #(Framework)

#if (contractType == 'NEP17')
This is a NEP-17 fungible token contract. It implements the standard token interface for Neo blockchain.

### Token Details
- **Symbol**: MYT
- **Decimals**: 8
- **Total Supply**: 10,000,000

### Methods
- `symbol()` - Returns the token symbol
- `decimals()` - Returns the number of decimals
- `totalSupply()` - Returns the total supply
- `balanceOf(owner)` - Returns the balance of an account
- `transfer(from, to, amount, data)` - Transfers tokens
#endif

#if (contractType == 'NEP11')
This is a NEP-11 non-fungible token contract. It implements the standard NFT interface for Neo blockchain.

### NFT Details
- **Symbol**: MYNFT
- **Collection Name**: My NFT Collection

### Methods
- `symbol()` - Returns the NFT symbol
- `decimals()` - Returns 0 (NFTs are indivisible)
- `totalSupply()` - Returns the total number of tokens
- `balanceOf(owner)` - Returns the number of tokens owned
- `tokensOf(owner)` - Returns the list of token IDs owned
- `transfer(to, tokenId, data)` - Transfers an NFT
- `ownerOf(tokenId)` - Returns the owner of a token
- `properties(tokenId)` - Returns token properties
#endif

#if (contractType == 'Governance')
This is a governance contract that implements voting and proposal management.

### Features
- Create and manage proposals
- Vote on proposals
- Execute approved proposals
- Configurable voting parameters

### Methods
- `createProposal(description, actions)` - Creates a new proposal
- `vote(proposalId, support)` - Vote on a proposal
- `execute(proposalId)` - Execute an approved proposal
- `getProposal(proposalId)` - Get proposal details
#endif

#if (contractType == 'Basic')
This is a basic smart contract template. You can extend it with your custom logic.

### Methods
- `_deploy(data, update)` - Contract deployment handler
- `update(nefFile, manifest, data)` - Contract update handler
- `destroy()` - Contract destruction handler
#endif

#if (enableSecurityFeatures)
## Security Features

This contract includes security features:
- Multi-signature support
- Access control with roles
- Permission management
- Secure initialization patterns
#endif

## Development

### Adding New Methods

To add new methods to your contract:

1. Add the method to the contract class in `src/MyContract/`
2. Add appropriate attributes (`[DisplayName]`, `[Safe]`, etc.)
3. Write unit tests for the new method
4. Update the deployment manifest if needed

### Testing Best Practices

- Write unit tests for all public methods
- Test edge cases and error conditions
- Use the Neo Test Framework for integration tests
- Test contract upgrades and migrations

## Resources

- [Neo Documentation](https://docs.neo.org/)
- [Neo Smart Contract Development](https://docs.neo.org/docs/n3/develop/write/basics)
- [Neo Standards](https://github.com/neo-project/proposals)
- [Neo Developer Portal](https://developers.neo.org/)

## License

This project is licensed under the MIT License - see the LICENSE file for details.