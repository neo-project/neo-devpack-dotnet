# Neo.SmartContract.Template

.NET project templates for creating Neo smart contracts. Quickly scaffold new contract projects with the correct structure and dependencies.

## Features

- **NEP-17 Token Template**: Fungible token contract template
- **NEP-11 Token Template**: Non-fungible token contract template  
- **Oracle Template**: Contract with oracle functionality
- **Ownable Template**: Contract with ownership pattern
- **Pre-configured**: Includes proper framework references and build settings

## Installation

### Install Templates

```bash
dotnet new install Neo.SmartContract.Template
```

### Update Templates

```bash
dotnet new update Neo.SmartContract.Template
```

### Uninstall Templates

```bash
dotnet new uninstall Neo.SmartContract.Template
```

## Available Templates

### NEP-17 Token Contract

```bash
dotnet new neocontractnep17 -n MyToken -o ./MyToken/
```

Creates a NEP-17 compliant fungible token contract.

### NEP-11 Token Contract

```bash
dotnet new neocontractnep11 -n MyNFT -o ./MyNFT/
```

Creates a NEP-11 compliant non-fungible token contract.

### Oracle Contract

```bash
dotnet new neocontractoracle -n MyOracle -o ./MyOracle/
```

Creates a contract with oracle request functionality.

### Ownable Contract

```bash
dotnet new neocontractowner -n MyContract -o ./MyContract/
```

Creates a contract with ownership transfer capabilities.

## Template Structure

Each template generates:

```
MyContract/
├── MyContract.cs          # Main contract file
├── MyContract.csproj      # Project file with proper references
└── README.md              # Template-specific documentation
```

## Building Generated Contracts

```bash
cd MyContract
dotnet restore
dotnet build
```

The compiled `.nef` and `.manifest.json` files will be in the output directory.

## Customizing Templates

After generating, modify the contract file to add your custom logic while keeping the standard structure.

## License

MIT - See [LICENSE](../../LICENSE) for details.
