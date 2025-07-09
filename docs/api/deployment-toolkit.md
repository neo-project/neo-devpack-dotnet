# DeploymentToolkit API Reference

## Class: DeploymentToolkit

The main class for deploying and managing Neo smart contracts.

### Constructor

```csharp
public DeploymentToolkit(string? configPath = null)
```

Creates a new instance of the DeploymentToolkit.

**Parameters:**
- `configPath` (optional): Path to configuration file. Defaults to `appsettings.json` in the current directory.

**Example:**
```csharp
// Use default configuration
var toolkit = new DeploymentToolkit();

// Use custom configuration
var toolkit = new DeploymentToolkit("config/production.json");
```

### Configuration Methods

#### SetNetwork

```csharp
public DeploymentToolkit SetNetwork(string network)
```

Sets the Neo network to use for operations.

**Parameters:**
- `network`: Network name (`"mainnet"`, `"testnet"`, `"local"`) or custom RPC URL

**Returns:** The toolkit instance for method chaining

**Example:**
```csharp
toolkit.SetNetwork("testnet");
toolkit.SetNetwork("http://localhost:10332");
```

#### SetWifKey

```csharp
public DeploymentToolkit SetWifKey(string wifKey)
```

Sets the WIF (Wallet Import Format) private key for signing transactions.

**Parameters:**
- `wifKey`: The WIF-encoded private key

**Returns:** The toolkit instance for method chaining

**Throws:**
- `ArgumentException`: If the WIF key is invalid

**Example:**
```csharp
toolkit.SetWifKey("L1234567890...");
```

### Deployment Methods

#### DeployAsync

```csharp
public async Task<ContractDeploymentInfo> DeployAsync(
    string path, 
    object[]? initParams = null)
```

Deploys a smart contract from source code or project file.

**Parameters:**
- `path`: Path to contract project (.csproj) or source file (.cs)
- `initParams` (optional): Initialization parameters for the contract

**Returns:** `ContractDeploymentInfo` containing deployment details

**Throws:**
- `ArgumentException`: If path is invalid
- `DeploymentException`: If deployment fails

**Example:**
```csharp
// Deploy without initialization parameters
var result = await toolkit.DeployAsync("src/MyContract/MyContract.csproj");

// Deploy with initialization parameters
var result = await toolkit.DeployAsync(
    "src/Token/Token.csproj",
    new object[] { "MyToken", "MTK", 8, 1000000 }
);
```

#### DeployArtifactsAsync

```csharp
public async Task<ContractDeploymentInfo> DeployArtifactsAsync(
    string nefPath, 
    string manifestPath, 
    object[]? initParams = null)
```

Deploys a pre-compiled contract from NEF and manifest files.

**Parameters:**
- `nefPath`: Path to the NEF file
- `manifestPath`: Path to the manifest file
- `initParams` (optional): Initialization parameters

**Returns:** `ContractDeploymentInfo` containing deployment details

**Example:**
```csharp
var result = await toolkit.DeployArtifactsAsync(
    "artifacts/contract.nef",
    "artifacts/contract.manifest.json"
);
```

#### DeployFromManifestAsync

```csharp
public async Task<MultiContractDeploymentResult> DeployFromManifestAsync(
    string manifestPath)
```

Deploys multiple contracts from a deployment manifest file.

**Parameters:**
- `manifestPath`: Path to the deployment manifest JSON file

**Returns:** `MultiContractDeploymentResult` with all deployment information

**Example:**
```csharp
var result = await toolkit.DeployFromManifestAsync("deployment-manifest.json");
foreach (var contract in result.DeployedContracts)
{
    Console.WriteLine($"{contract.Key}: {contract.Value.ContractHash}");
}
```

### Contract Interaction Methods

#### CallAsync

```csharp
public async Task<T> CallAsync<T>(
    string contractHashOrAddress, 
    string method, 
    params object[] args)
```

Calls a contract method without creating a transaction (read-only).

**Type Parameters:**
- `T`: The expected return type

**Parameters:**
- `contractHashOrAddress`: Contract hash or address
- `method`: Method name to call
- `args`: Method arguments

**Returns:** The method result cast to type `T`

**Example:**
```csharp
// Get token balance
var balance = await toolkit.CallAsync<BigInteger>(
    "0x1234567890abcdef",
    "balanceOf",
    "NXXxXXxXXxXXxXXxXXxXXxXXxXX"
);

// Get token symbol
var symbol = await toolkit.CallAsync<string>(contractHash, "symbol");
```

#### InvokeAsync

```csharp
public async Task<UInt256> InvokeAsync(
    string contractHashOrAddress, 
    string method, 
    params object[] args)
```

Invokes a contract method by creating a transaction.

**Parameters:**
- `contractHashOrAddress`: Contract hash or address
- `method`: Method name to invoke
- `args`: Method arguments

**Returns:** Transaction hash

**Throws:**
- `InvalidOperationException`: If WIF key is not set

**Example:**
```csharp
// Transfer tokens
var txHash = await toolkit.InvokeAsync(
    tokenContract,
    "transfer",
    fromAddress,
    toAddress,
    amount,
    null
);
```

### Update Methods

#### UpdateAsync

```csharp
public async Task<ContractUpdateInfo> UpdateAsync(
    string contractHashOrAddress, 
    string path, 
    object[]? updateParams = null)
```

Updates an existing contract from source.

**Parameters:**
- `contractHashOrAddress`: Contract to update
- `path`: Path to new contract source
- `updateParams` (optional): Update parameters

**Returns:** `ContractUpdateInfo` with update details

**Example:**
```csharp
var result = await toolkit.UpdateAsync(
    "0x1234567890abcdef",
    "src/MyContract/MyContract.csproj",
    new object[] { "v2.0" }
);
```

#### UpdateArtifactsAsync

```csharp
public async Task<ContractUpdateInfo> UpdateArtifactsAsync(
    string contractHashOrAddress, 
    string? nefPath, 
    string? manifestPath, 
    object[]? updateParams = null)
```

Updates a contract from NEF and/or manifest files.

**Parameters:**
- `contractHashOrAddress`: Contract to update
- `nefPath` (optional): New NEF file (null to keep existing)
- `manifestPath` (optional): New manifest file (null to keep existing)
- `updateParams` (optional): Update parameters

**Example:**
```csharp
// Update only manifest
var result = await toolkit.UpdateArtifactsAsync(
    contractHash,
    null,
    "new-manifest.json"
);
```

### Query Methods

#### GetDeployerAccountAsync

```csharp
public async Task<UInt160> GetDeployerAccountAsync()
```

Gets the deployer account script hash.

**Returns:** The deployer's account script hash

**Throws:**
- `InvalidOperationException`: If no WIF key is set

#### GetGasBalanceAsync

```csharp
public async Task<decimal> GetGasBalanceAsync(string? address = null)
```

Gets the GAS balance of an account.

**Parameters:**
- `address` (optional): Account address (null for deployer account)

**Returns:** GAS balance as decimal

**Example:**
```csharp
// Get deployer balance
var balance = await toolkit.GetGasBalanceAsync();

// Get specific address balance
var balance = await toolkit.GetGasBalanceAsync("NXXxXXxXXxXXxXXxXXxXXxXXxXX");
```

#### ContractExistsAsync

```csharp
public async Task<bool> ContractExistsAsync(string contractHashOrAddress)
```

Checks if a contract exists at the given address.

**Parameters:**
- `contractHashOrAddress`: Contract hash or address to check

**Returns:** `true` if contract exists, `false` otherwise

### Builder Methods

#### CreateManifestBuilder

```csharp
public DeploymentManifestBuilder CreateManifestBuilder()
```

Creates a deployment manifest builder for constructing deployment configurations.

**Returns:** A new `DeploymentManifestBuilder` instance

**Example:**
```csharp
var manifest = toolkit.CreateManifestBuilder()
    .SetName("My DApp Deployment")
    .AddContract("Token", "src/Token/Token.csproj", 1)
    .AddContract("Governance", "src/Gov/Gov.csproj", 2)
    .WithDependency("Governance", "Token")
    .Build();
```

### Utility Methods

#### ResolveDependencyOrder

```csharp
public List<ContractDefinition> ResolveDependencyOrder(
    IList<ContractDefinition> contracts)
```

Resolves the deployment order for contracts based on dependencies.

**Parameters:**
- `contracts`: List of contract definitions

**Returns:** Ordered list of contracts to deploy

## Data Types

### ContractDeploymentInfo

```csharp
public class ContractDeploymentInfo
{
    public string ContractName { get; set; }
    public UInt160 ContractHash { get; set; }
    public string ContractAddress { get; set; }
    public UInt256 TransactionHash { get; set; }
    public long GasConsumed { get; set; }
    public DateTime DeploymentTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### MultiContractDeploymentResult

```csharp
public class MultiContractDeploymentResult
{
    public Dictionary<string, ContractDeploymentInfo> DeployedContracts { get; set; }
    public TimeSpan DeploymentDuration { get; set; }
    public long TotalGasConsumed { get; set; }
    public List<string> Errors { get; set; }
    public bool Success { get; set; }
}
```

### ContractUpdateInfo

```csharp
public class ContractUpdateInfo
{
    public UInt160 ContractHash { get; set; }
    public UInt256 TransactionHash { get; set; }
    public long GasConsumed { get; set; }
    public DateTime UpdateTime { get; set; }
    public string PreviousVersion { get; set; }
    public string NewVersion { get; set; }
}
```

### DeploymentOptions

```csharp
public class DeploymentOptions
{
    public string WifKey { get; set; }
    public string RpcUrl { get; set; }
    public uint NetworkMagic { get; set; }
    public bool WaitForConfirmation { get; set; }
    public bool VerifyAfterDeploy { get; set; }
    public long GasLimit { get; set; }
}
```

## Error Handling

### DeploymentException

Thrown when deployment operations fail.

```csharp
public class DeploymentException : Exception
{
    public string ErrorCode { get; set; }
    public UInt256? TransactionHash { get; set; }
    public Dictionary<string, object> Details { get; set; }
}
```

Common error codes:
- `INSUFFICIENT_GAS`: Not enough GAS for deployment
- `CONTRACT_TOO_LARGE`: Contract exceeds size limit
- `COMPILATION_ERROR`: Contract compilation failed
- `NETWORK_ERROR`: RPC communication failed
- `INVALID_CONTRACT`: Contract validation failed

## Complete Example

```csharp
using Neo.SmartContract.Deploy;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Create and configure toolkit
        using var toolkit = new DeploymentToolkit()
            .SetNetwork("testnet")
            .SetWifKey(Environment.GetEnvironmentVariable("NEO_WIF_KEY"));

        try
        {
            // Deploy a single contract
            Console.WriteLine("Deploying contract...");
            var deployment = await toolkit.DeployAsync(
                "src/MyContract/MyContract.csproj",
                new object[] { "initial", "params" }
            );
            
            Console.WriteLine($"Contract deployed at: {deployment.ContractAddress}");
            Console.WriteLine($"Transaction: {deployment.TransactionHash}");
            
            // Interact with the contract
            await toolkit.InvokeAsync(
                deployment.ContractHash,
                "storeData",
                "key1",
                "value1"
            );
            
            var value = await toolkit.CallAsync<string>(
                deployment.ContractHash,
                "getData",
                "key1"
            );
            
            Console.WriteLine($"Stored value: {value}");
            
            // Deploy multiple contracts
            var multiResult = await toolkit.DeployFromManifestAsync(
                "deployment-manifest.json"
            );
            
            Console.WriteLine($"Deployed {multiResult.DeployedContracts.Count} contracts");
            Console.WriteLine($"Total gas: {multiResult.TotalGasConsumed / 100000000m} GAS");
        }
        catch (DeploymentException ex)
        {
            Console.WriteLine($"Deployment failed: {ex.Message}");
            Console.WriteLine($"Error code: {ex.ErrorCode}");
        }
    }
}
```

## See Also

- [Deployment Overview](../deployment-toolkit/overview.md)
- [Examples](../../examples/DeploymentExample/README.md)
- [Testing Framework API](testing-framework.md)