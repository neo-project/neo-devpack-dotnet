# NEO Smart Contract Init CLI

A command-line tool for quickly scaffolding NEO smart contract projects with best practices and modern tooling.

## Installation

### As a .NET Tool

```bash
dotnet tool install -g Neo.SmartContract.Init
```

### From Source

```bash
cd src/Neo.SmartContract.Init
dotnet build
dotnet pack
dotnet tool install --global --add-source ./nupkg Neo.SmartContract.Init
```

## Usage

### Create a New Project

```bash
# Basic usage
neo-init new MyContract

# With specific template
neo-init new MyToken --template nep17

# Interactive mode
neo-init new --interactive

# Custom output directory
neo-init new MyContract --output ./contracts
```

### List Available Templates

```bash
neo-init list
```

Available templates:
- `basic` - Basic smart contract with minimal setup
- `nep17` - NEP-17 fungible token standard
- `nep11` - NEP-11 non-fungible token (NFT) standard
- `oracle` - Contract with Oracle service integration
- `multisig` - Multi-signature wallet contract
- `dao` - Decentralized Autonomous Organization

### Add Components to Existing Project

```bash
# Add unit tests
neo-init add tests

# Add Docker support
neo-init add docker

# Add GitHub Actions CI/CD
neo-init add ci

# Add security analyzer
neo-init add analyzer
```

## Features

### Interactive Mode

The interactive mode guides you through project creation with prompts for:
- Project name
- Template selection
- Author information
- Additional features (tests, CI/CD, Docker, etc.)

### Project Structure

Generated projects include:
- Smart contract source code
- Project file with dependencies
- README with build instructions
- Optional unit tests
- Optional CI/CD pipeline
- Optional Docker configuration
- Optional VS Code settings

### Templates

Each template provides:
- Complete working contract code
- Proper security practices
- Standard compliance (NEP-17, NEP-11, etc.)
- Owner management
- Update/destroy capabilities
- Event definitions

## Examples

### Create a Token Project

```bash
neo-init new MyToken --template nep17 --author "John Doe" --email "john@example.com"
cd MyToken
dotnet build
dotnet test
```

### Create an NFT Project with All Features

```bash
neo-init new MyNFT --template nep11 --interactive
# Select: Unit Tests, GitHub Actions, Docker Support, VS Code Configuration
```

### Add Testing to Existing Project

```bash
cd MyExistingContract
neo-init add tests
dotnet test
```

## Development Workflow

1. Create project: `neo-init new MyContract`
2. Navigate to project: `cd MyContract`
3. Develop your contract
4. Build: `dotnet build`
5. Test: `dotnet test`
6. Compile to NEF: Use Neo.Compiler.CSharp
7. Deploy to testnet/mainnet

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

MIT License