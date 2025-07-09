# Neo Smart Contract Templates

This package provides project templates for Neo smart contract development.

## Installation

```bash
dotnet new install Neo.SmartContract.Template
```

## Available Templates

### 1. Neo Smart Contract Solution (neocontractsolution)

A complete solution template with contract, tests, and deployment support.

```bash
dotnet new neocontractsolution -n MyProject
```

**Options:**
- `--contractName` - Name of the smart contract (default: MyContract)
- `--enableDeployment` - Include deployment toolkit support (default: true)
- `--enableTests` - Include unit test project (default: true)
- `--enableSecurityFeatures` - Include security features like multi-sig (default: true)
- `--contractType` - Type of contract to create:
  - `Basic` - Basic smart contract template (default)
  - `NEP17` - NEP-17 fungible token standard
  - `NEP11` - NEP-11 non-fungible token standard
  - `Governance` - Governance contract with voting

### 2. NEP-17 Token Contract (neocontractnep17)

A NEP-17 compliant fungible token contract.

```bash
dotnet new neocontractnep17 -n MyToken
```

### 3. Oracle Contract (neocontractoracle)

A contract that integrates with Neo Oracle service.

```bash
dotnet new neocontractoracle -n MyOracle
```

### 4. Owner Contract (neocontractowner)

A contract with owner-based access control.

```bash
dotnet new neocontractowner -n MyOwnable
```

## Template Features

### Complete Solution Structure

The `neocontractsolution` template creates:

```
MyProject/
├── src/
│   └── MyContract/          # Smart contract implementation
├── tests/
│   └── MyContract.Tests/    # Unit tests
├── deploy/
│   └── MyContract.Deploy/   # Deployment toolkit
├── deployment.json          # Deployment manifest
├── README.md                # Project documentation
└── MyProject.sln            # Solution file
```

### Built-in Features

- **Smart Contract Development**
  - Pre-configured contract structure
  - Standard attributes and metadata
  - Event declarations
  - Storage management helpers

- **Testing Support**
  - Unit test project setup
  - Neo.SmartContract.Testing framework
  - Example test cases
  - Test utilities

- **Deployment Support**
  - Deployment CLI tool
  - Configuration management
  - Multi-environment support
  - Deployment manifest

- **Security Features**
  - Owner-based access control
  - Multi-signature support
  - Input validation patterns
  - Reentrancy protection

## Usage Examples

### Create a Basic Contract

```bash
dotnet new neocontractsolution -n SimpleStorage --contractType Basic
cd SimpleStorage
dotnet build
dotnet test
```

### Create a Token Contract

```bash
dotnet new neocontractsolution -n MyToken --contractType NEP17
cd MyToken
dotnet build

# Deploy to testnet
cd deploy/MyToken.Deploy
dotnet run -- deploy -n testnet -w YOUR_WIF_KEY
```

### Create an NFT Contract

```bash
dotnet new neocontractsolution -n MyNFT --contractType NEP11
cd MyNFT
dotnet build
```

### Create a Governance Contract

```bash
dotnet new neocontractsolution -n MyDAO --contractType Governance
cd MyDAO
dotnet build
```

## Customization

After creating a project from the template, you can:

1. **Modify the Contract**: Edit the contract in `src/MyContract/`
2. **Add Methods**: Implement your business logic
3. **Update Tests**: Add test cases in `tests/MyContract.Tests/`
4. **Configure Deployment**: Edit `deployment.json` for your needs
5. **Extend Features**: Add additional contracts or libraries

## Template Development

To modify or create new templates:

1. Clone the repository
2. Edit templates in `src/Neo.SmartContract.Template/templates/`
3. Test locally: `dotnet new install ./src/Neo.SmartContract.Template/`
4. Create new templates following the structure

## Resources

- [Neo Documentation](https://docs.neo.org/)
- [Smart Contract Development Guide](https://docs.neo.org/docs/n3/develop/write/basics)
- [NEP Standards](https://github.com/neo-project/proposals)
- [Deployment Toolkit Documentation](../../docs/deployment-toolkit/overview.md)

## Support

For issues or questions:
- [GitHub Issues](https://github.com/neo-project/neo-devpack-dotnet/issues)
- [Neo Discord](https://discord.gg/neo)
- [Neo Forum](https://www.reddit.com/r/NEO/)