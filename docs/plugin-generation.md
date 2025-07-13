# Neo N3 Smart Contract Plugin Generation

The Neo smart contract compiler (r3e neo contract compiler - `rncc`) now supports automatic generation of Neo N3 plugins that provide CLI commands for interacting with compiled smart contracts through neo-cli.

## Overview

When you compile a smart contract with the `--generate-plugin` option, the compiler will generate a complete Neo N3 plugin project that:

- Provides CLI commands for each public contract method
- Handles parameter parsing and type conversion
- Integrates with neo-cli console
- Includes a contract wrapper for direct invocation

## Usage

To generate a plugin when compiling your contract:

```bash
dotnet rncc MyContract.cs --generate-plugin -o ./output
```

This will create:
- `MyContract.nef` - The compiled contract
- `MyContract.manifest.json` - The contract manifest
- `MyContractPlugin/` - A complete plugin project directory

## Generated Plugin Structure

The generated plugin consists of several components:

### 1. Main Plugin Class (`MyContractPlugin.cs`)
- Inherits from `Neo.Plugins.Plugin`
- Initializes the contract wrapper and RPC methods
- Handles plugin lifecycle (configuration, system loading, disposal)

### 2. CLI Commands Class (`MyContractCommands.cs`)
- Defines CLI command handlers for each contract method
- Handles parameter parsing and validation
- Displays results in the console
- Provides help and error handling

### 3. Contract Wrapper Class (`MyContractWrapper.cs`)
- Provides strongly-typed async methods for contract invocation
- Handles script building and execution
- Converts between Neo VM types and .NET types

### 4. Plugin Configuration (`MyContractPlugin.json`)
- Contains plugin configuration including:
  - Contract hash
  - Network ID
  - Maximum gas per transaction
  - Dependencies (e.g., RpcServer)

### 5. Project File (`MyContractPlugin.csproj`)
- References required Neo libraries
- Configured for .NET 9.0
- Includes necessary build settings

## Example

For a simple HelloWorld contract:

```csharp
[DisplayName("HelloWorld")]
public class HelloWorld : SmartContract
{
    [Safe]
    public static string SayHello()
    {
        return "Hello, World!";
    }
    
    public static bool SetMessage(string message)
    {
        Storage.Put("message", message);
        return true;
    }
}
```

The generated plugin will provide CLI commands:
- `helloworld sayhello` - Returns "Hello, World!"
- `helloworld setmessage <message>` - Sets a message in storage

## CLI Command Structure

Commands follow the pattern: `{contractname} {methodname} [parameters]`
- Contract name is converted to lowercase
- Method name is converted to lowercase
- Parameters are passed as space-separated arguments
- Example: `helloworld setmessage "Hello, Neo!"`

## Using the Generated Plugin

1. Build the plugin project:
   ```bash
   cd MyContractPlugin
   dotnet build
   ```

2. Copy the built plugin to your Neo node's Plugins directory

3. Configure the plugin by editing `MyContractPlugin.json`:
   - Set the correct network ID
   - Adjust gas limits if needed
   - Configure any specific settings

4. Restart your Neo node to load the plugin

5. Use the CLI commands in neo-cli:
   ```
   neo> helloworld sayhello
   neo> helloworld setmessage "Hello, Neo!"
   neo> helloworld help
   ```

## Type Mapping

The plugin generator automatically maps Neo smart contract types to .NET types:

| Contract Type | .NET Type | CLI Format |
|--------------|-----------|------------|
| Integer | BigInteger | Decimal number |
| String | string | Quoted string |
| ByteArray | byte[] | Base64 string |
| Hash160 | UInt160 | Hex string (0x...) |
| Hash256 | UInt256 | Hex string (0x...) |
| PublicKey | ECPoint | Hex string |
| Boolean | bool | true/false |
| Array | object[] | Space-separated values |

## Advanced Features

### Safe Methods
Methods marked with the `[Safe]` attribute are read-only and don't modify blockchain state. The plugin displays this information in the help output with a `[SAFE]` indicator.

### Parameter Validation
The generated CLI handlers include parameter validation and provide detailed error messages for invalid inputs, including usage instructions.

### Interactive Help
Each generated plugin includes comprehensive help commands that show:
- Available methods with their parameters
- Parameter types and requirements
- Contract hash information
- Safe method indicators

### Error Handling
The plugin provides comprehensive error handling, including:
- Contract execution failures
- Parameter parsing errors
- Type conversion errors
- Usage validation

## Limitations

- Generated plugins currently support basic types; complex nested structures may require manual adjustments
- Transaction signing must be handled by the wallet integration in neo-cli
- Array parameters need to be handled carefully with proper escaping

## Future Enhancements

Planned improvements include:
- Support for complex parameter types
- Integration with wallet commands for automatic signing
- Support for batch operations
- Enhanced parameter input validation