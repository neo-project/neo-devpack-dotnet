# Neo N3 Smart Contract Plugin Generation

The Neo smart contract compiler (`nccs`) now supports automatic generation of Neo N3 plugins that provide RPC interfaces for interacting with compiled smart contracts.

## Overview

When you compile a smart contract with the `--generate-plugin` option, the compiler will generate a complete Neo N3 plugin project that:

- Provides RPC methods for each public contract method
- Handles parameter parsing and type conversion
- Integrates with the Neo node's RPC server
- Includes a contract wrapper for easy invocation

## Usage

To generate a plugin when compiling your contract:

```bash
dotnet nccs MyContract.cs --generate-plugin -o ./output
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

### 2. RPC Methods Class (`MyContractRpcMethods.cs`)
- Defines RPC method handlers for each contract method
- Handles parameter parsing and validation
- Formats results for RPC responses
- Provides error handling

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

The generated plugin will provide RPC methods:
- `helloworld_sayhello` - Returns "Hello, World!"
- `helloworld_setmessage` - Sets a message in storage

## RPC Method Naming Convention

RPC methods are named using the pattern: `{contractname}_{methodname}`
- Contract name is converted to lowercase
- Method name is converted to lowercase
- Example: `SampleStorage.TestPutByte` → `samplestorage_testputbyte`

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

5. Call the RPC methods:
   ```bash
   curl -X POST http://localhost:20332 \
     -H "Content-Type: application/json" \
     -d '{"jsonrpc":"2.0","method":"helloworld_sayhello","params":[],"id":1}'
   ```

## Type Mapping

The plugin generator automatically maps Neo smart contract types to .NET types:

| Contract Type | .NET Type | RPC Format |
|--------------|-----------|------------|
| Integer | BigInteger | String (decimal) |
| String | string | String |
| ByteArray | byte[] | Base64 string |
| Hash160 | UInt160 | Hex string |
| Hash256 | UInt256 | Hex string |
| PublicKey | ECPoint | Hex string |
| Boolean | bool | Boolean |
| Array | object[] | JSON array |

## Advanced Features

### Safe Methods
Methods marked with the `[Safe]` attribute are read-only and don't modify blockchain state. The plugin correctly identifies these methods and sets the appropriate RPC metadata.

### Parameter Validation
The generated RPC handlers include parameter validation and provide detailed error messages for invalid inputs.

### Async Execution
All contract invocations are performed asynchronously to avoid blocking the RPC server.

### Error Handling
The plugin provides comprehensive error handling, including:
- Contract execution failures
- Parameter parsing errors
- Type conversion errors

## Limitations

- The plugin requires the RpcServer plugin to be installed and running
- Generated plugins currently support basic types; complex nested structures may require manual adjustments
- Transaction signing and fee calculation must be handled by the RPC client

## Future Enhancements

Planned improvements include:
- Support for event subscriptions
- Automatic transaction building for state-changing methods
- Integration with wallet plugins for signing
- Support for batch operations