# Neo.SmartContract.Template

.NET project templates for creating Neo smart contracts. Quickly scaffold new contract projects with the correct structure and dependencies.

## Features

- **Solution Template**: Contract project plus a wired unit-test project
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

### Smart Contract Solution

```bash
dotnet new neocontract -n MyContract -o ./MyContract/
```

Creates a solution with:

- a smart contract project
- an MSTest project wired to `Neo.SmartContract.Testing`
- generated contract artifacts consumed automatically by the test project
- a local `nccs` tool manifest used during the contract build

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

Project templates generate:

```
MyContract/
├── MyContract.cs          # Main contract file
├── MyContract.csproj      # Project file with proper references
└── README.md              # Template-specific documentation
```

The solution template generates:

```
MyContract/
├── .config/dotnet-tools.json
├── MyContract.sln
├── MyContract/
│   ├── MyContract.csproj
│   └── SmartContract.cs
└── MyContract.UnitTests/
    ├── MyContract.UnitTests.csproj
    └── SmartContractTests.cs
```

## Building Generated Contracts

```bash
cd MyContract
dotnet restore
dotnet build
```

The compiled `.nef` and `.manifest.json` files will be in the output directory.

For the solution template, run the generated tests with:

```bash
cd MyContract
dotnet test
```

The contract build restores the local `nccs` tool, generates `.nef`, `.manifest.json`, and `.artifacts.cs`, and the unit-test project compiles the generated artifacts automatically.

## Customizing Templates

After generating, modify the contract file to add your custom logic while keeping the standard structure.

## License

MIT - See [LICENSE](../../LICENSE) for details.
