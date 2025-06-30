# Neo N3 Plugin Generation Examples

This document summarizes the plugins generated for the example contracts with the `--generate-plugin` option.

## Generated Plugins

The following example contracts have been compiled with plugin generation enabled:

### 1. SampleHelloWorld Plugin
- **Location**: `Example.SmartContract.HelloWorld/bin/sc/SampleHelloWorldPlugin/`
- **Contract Hash**: `0xef061fe2c2f02e63f00159e99dfd90cbc54ae0d2`
- **RPC Methods**:
  - `samplehelloworld_sayhello` - Returns "Hello, World!" (Safe method)

### 2. SampleStorage Plugin
- **Location**: `Example.SmartContract.Storage/bin/sc/SampleStoragePlugin/`
- **RPC Methods Include**:
  - `samplestorage_testputbyte` - Store byte array in storage
  - `samplestorage_testdeletebyte` - Delete byte array from storage
  - `samplestorage_testgetbyte` - Retrieve byte array from storage
  - `samplestorage_testputstring` - Store string in storage
  - `samplestorage_testdeletestring` - Delete string from storage
  - `samplestorage_testgetstring` - Retrieve string from storage
  - Additional methods for various data types (BigInteger, ByteString, etc.)

### 3. SampleNep17Token Plugin
- **Location**: `Example.SmartContract.NEP17/bin/sc/SampleNep17TokenPlugin/`
- **RPC Methods Include**:
  - `samplenep17token_symbol` - Get token symbol (Safe)
  - `samplenep17token_decimals` - Get token decimals (Safe)
  - `samplenep17token_totalsupply` - Get total supply (Safe)
  - `samplenep17token_balanceof` - Get balance of account (Safe)
  - `samplenep17token_transfer` - Transfer tokens
  - `samplenep17token_getowner` - Get contract owner (Safe)
  - `samplenep17token_setowner` - Set contract owner
  - `samplenep17token_getminter` - Get minter address (Safe)
  - `samplenep17token_setminter` - Set minter address
  - `samplenep17token_mint` - Mint new tokens
  - `samplenep17token_burn` - Burn tokens
  - `samplenep17token_verify` - Verify contract (Safe)
  - `samplenep17token_update` - Update contract

### 4. SampleOracle Plugin
- **Location**: `Example.SmartContract.Oracle/bin/sc/SampleOraclePlugin/`
- **RPC Methods Include**:
  - `sampleoracle_requesturl` - Request data from URL
  - `sampleoracle_onorracleresponse` - Handle Oracle response
  - `sampleoracle_getrequest` - Get request data

### 5. SampleTransferContract Plugin
- **Location**: `Example.SmartContract.Transfer/bin/sc/SampleTransferContractPlugin/`
- **RPC Methods Include**:
  - `sampletransfercontract_onpayment` - Handle payment events
  - `sampletransfercontract_transfer` - Transfer tokens
  - `sampletransfercontract_transfermulti` - Multi-transfer

## Plugin Structure

Each generated plugin contains:

1. **Main Plugin Class** (`{ContractName}Plugin.cs`)
   - Inherits from `Neo.Plugins.Plugin`
   - Manages plugin lifecycle
   - Initializes contract wrapper and RPC methods

2. **RPC Methods Class** (`{ContractName}RpcMethods.cs`)
   - Provides RPC endpoint handlers for each contract method
   - Handles parameter parsing and validation
   - Formats responses and error handling

3. **Contract Wrapper Class** (`{ContractName}Wrapper.cs`)
   - Provides strongly-typed async methods for contract invocation
   - Handles VM execution and result conversion
   - Maps between Neo VM types and .NET types

4. **Plugin Configuration** (`{ContractName}Plugin.json`)
   - Contains contract hash and plugin settings
   - Dependencies on RpcServer plugin
   - Network and gas configuration

5. **Project File** (`{ContractName}Plugin.csproj`)
   - .NET 9.0 project with Neo dependencies
   - Proper build configuration for plugin deployment

## Usage Examples

### Call HelloWorld Contract
```bash
curl -X POST http://localhost:20332 \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"samplehelloworld_sayhello","params":[],"id":1}'
```

### Get NEP17 Token Balance
```bash
curl -X POST http://localhost:20332 \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"samplenep17token_balanceof","params":["0x1234567890abcdef1234567890abcdef12345678"],"id":1}'
```

### Store Data in Storage Contract
```bash
curl -X POST http://localhost:20332 \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"samplestorage_testputstring","params":["mykey","myvalue"],"id":1}'
```

## Deployment

To deploy a generated plugin:

1. Build the plugin project:
   ```bash
   cd {ContractName}Plugin
   dotnet build -c Release
   ```

2. Copy the built plugin to your Neo node's Plugins directory

3. Configure the plugin settings in the `.json` configuration file

4. Restart your Neo node

5. The RPC methods will be available through the node's RPC interface

## Features Demonstrated

The generated plugins showcase:

- **Type Safety**: Strongly-typed method parameters and return values
- **Async Operations**: All contract calls are asynchronous
- **Error Handling**: Comprehensive error responses for failed operations
- **Parameter Validation**: Automatic parsing and validation of RPC parameters
- **Safe Method Detection**: Proper identification of read-only methods
- **Neo Standards Compliance**: Following Neo plugin development standards

This plugin generation feature greatly simplifies the development of applications that need to interact with Neo smart contracts by providing ready-to-use RPC interfaces.