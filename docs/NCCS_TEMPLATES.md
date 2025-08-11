# NCCS Template Generation

## Overview

The Neo C# Compiler Service (NCCS) now includes a powerful template generation feature that allows developers to quickly scaffold new Neo smart contracts from pre-built templates. This feature streamlines the development process by providing production-ready contract templates for common use cases.

## Usage

### Basic Command Structure

```bash
nccs new <contract-name> [options]
```

### Available Options

- `-t, --template <template>`: Specifies the template to use (default: basic)
- `-o, --output <path>`: Sets the output directory for the new contract
- `-f, --force`: Overwrites existing files if the directory already exists

### Available Templates

1. **basic** - A simple smart contract with basic functionality
2. **nep17** - NEP-17 compliant fungible token contract
3. **nft** / **nep11** - NEP-11 compliant NFT contract
4. **oracle** - Oracle-enabled contract for external data integration
5. **ownable** - Access-controlled contract with owner and admin management

## Template Descriptions

### Basic Template
The basic template provides a minimal smart contract structure with:
- Owner management
- Deploy/Update/Destroy methods
- Simple storage operations
- Example methods (HelloWorld, Add, SetValue/GetValue)

```bash
nccs new MyContract --template basic
```

### NEP-17 Token Template
Creates a fungible token contract compliant with the NEP-17 standard:
- Full NEP-17 implementation (transfer, balanceOf, etc.)
- Mint and burn functionality (owner-restricted)
- Symbol and decimals configuration
- Owner management with transfer capabilities

```bash
nccs new MyToken --template nep17
```

### NEP-11 NFT Template
Generates a non-fungible token contract following NEP-11 standard:
- Complete NEP-11 implementation
- Token minting with metadata (name, description, image)
- Automatic token ID generation
- Owner-controlled minting

```bash
nccs new MyNFT --template nft
```

### Oracle Template
Creates a contract that can interact with external data sources:
- Oracle request functionality
- Response handling and processing
- Data storage and retrieval
- JSON deserialization support

```bash
nccs new MyOracleContract --template oracle
```

### Ownable Template
Provides an access-controlled contract with role management:
- Owner and admin role separation
- Permission-based method access
- Admin management (add/remove)
- Protected data operations

```bash
nccs new MySecureContract --template ownable
```

## Examples

### Creating a New NEP-17 Token

```bash
# Create a new token contract
nccs new MyAwesomeToken --template nep17 --output ./contracts/MyToken

# Navigate to the contract directory
cd ./contracts/MyToken

# Compile the contract
nccs compile
```

### Creating a Contract with Custom Output Directory

```bash
# Create contract in a specific directory
nccs new GameItems --template nft --output /path/to/my/project/contracts

# Compile from project root
nccs compile /path/to/my/project/contracts/GameItems.csproj
```

### Overwriting Existing Contract

```bash
# Force overwrite if directory exists
nccs new MyContract --template basic --force
```

## Post-Generation Steps

After generating a contract from a template:

1. **Review and Customize**: Open the generated `.cs` file and customize:
   - Update contract metadata (author, description, version)
   - Modify the symbol and decimals (for token contracts)
   - Add your custom business logic

2. **Update Dependencies**: Check the `.csproj` file and update package versions if needed

3. **Compile**: Use `nccs compile` to build your contract

4. **Test**: Write and run tests for your contract functionality

5. **Deploy**: Deploy to Neo TestNet or MainNet using your preferred deployment tool

## Template Customization

Each generated contract includes placeholder values that should be updated:

- `<Your Name>` - Replace with your name or organization
- `<Your Email>` - Your contact email
- `<Description of your contract>` - Meaningful contract description
- `https://github.com/your-repo/{contractName}` - Your actual repository URL
- `EXAMPLE` - Token symbol (for NEP-17/NEP-11 contracts)

## Best Practices

1. **Always Review Generated Code**: Templates provide a starting point; review and understand the code before deployment

2. **Update Metadata**: Ensure all contract metadata is accurate and up-to-date

3. **Security Considerations**: 
   - Review and test owner/admin functionality
   - Implement proper access controls
   - Add input validation where needed

4. **Version Control**: Commit your contracts to version control immediately after generation

5. **Testing**: Write comprehensive tests for all contract functionality

## Troubleshooting

### Directory Already Exists
If you see "Directory already exists", either:
- Choose a different name/location
- Use the `--force` flag to overwrite
- Manually delete the existing directory

### Compilation Errors
After generation, if compilation fails:
- Ensure Neo.SmartContract.Framework is properly installed
- Check that the target framework matches your environment
- Verify all using statements are correct

### Invalid Contract Name
Contract names must be valid C# identifiers:
- Start with a letter or underscore
- Contain only letters, numbers, and underscores
- No spaces or special characters

## Integration with Existing Projects

The template generation feature integrates seamlessly with existing Neo projects:

1. Generate contracts directly into your project structure
2. Use the `--output` flag to specify your contracts directory
3. Compile multiple contracts together using solution files
4. Share common code between generated and custom contracts

## Future Enhancements

Planned improvements for the template system:
- Custom template support
- Interactive template configuration
- Multi-contract project scaffolding
- Template versioning and updates
- Community-contributed templates

## Contributing

To contribute new templates or improvements:
1. Fork the neo-devpack-dotnet repository
2. Add your template generation method in Program.cs
3. Update documentation
4. Submit a pull request

## Support

For issues or questions about the template generation feature:
- Open an issue on the neo-devpack-dotnet GitHub repository
- Ask in the Neo Discord development channel
- Check the Neo documentation for updates