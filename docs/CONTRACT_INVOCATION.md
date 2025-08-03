# Contract Invocation System

The Neo N3 C# DevPack now includes a high-level contract invocation system that allows developers to call other contracts as if they were regular C# classes. This system supports both contracts under development (without deployed addresses) and deployed contracts (with network-specific addresses).

## Overview

The contract invocation system provides:

1. **Type-safe contract method calls** - Call contract methods with IntelliSense support
2. **Development contract references** - Reference contracts that are still under development
3. **Multi-network deployment support** - Different addresses for privnet/testnet/mainnet
4. **Automatic address resolution** - Contract addresses resolved at compilation time
5. **Seamless integration** - Works with existing Neo N3 contract development workflow

## Key Components

### Contract References

Contract references represent connections to other contracts and come in two types:

- **DevelopmentContractReference**: For contracts under development (no deployed address yet)
- **DeployedContractReference**: For contracts deployed on blockchain networks

### Network Context

The `NetworkContext` class manages contract addresses across different networks:

```csharp
var networkContext = new NetworkContext("testnet");
networkContext.SetNetworkAddress("privnet", "0x1234...");
networkContext.SetNetworkAddress("testnet", "0x5678...");
networkContext.SetNetworkAddress("mainnet", "0x9abc...");
```

### Contract Proxies

Contract proxies provide type-safe method invocation that compiles to `Contract.Call` instructions:

```csharp
public class Nep17ContractProxy : ContractProxyBase
{
    public Nep17ContractProxy(IContractReference contractReference) : base(contractReference) { }

    [ContractMethod(ReadOnly = true)]
    public BigInteger BalanceOf(UInt160 account)
    {
        return (BigInteger)InvokeReadOnly("balanceOf", account);
    }
}
```

## Usage Examples

### 1. Referencing a Development Contract

When developing multiple contracts simultaneously, you can reference other contracts in your solution:

```csharp
[ContractReference("TokenContract", 
    ReferenceType = ContractReferenceType.Development,
    ProjectPath = "../MyToken/MyToken.csproj")]
private static readonly DevelopmentContractReference TokenContract;

public static BigInteger GetTokenBalance(UInt160 account)
{
    var proxy = new Nep17ContractProxy(TokenContract);
    return proxy.BalanceOf(account);
}
```

### 2. Referencing a Deployed Contract

For contracts already deployed on networks:

```csharp
[ContractReference("GAS",
    ReferenceType = ContractReferenceType.Deployed,
    PrivnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
    TestnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
    MainnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf")]
private static readonly DeployedContractReference GasContract;

public static BigInteger GetGasBalance(UInt160 account)
{
    var proxy = new Nep17ContractProxy(GasContract);
    return proxy.BalanceOf(account);
}
```

### 3. Auto-Detection

The system can automatically detect the reference type:

```csharp
// Will be detected as deployed contract (has addresses)
[ContractReference("NEO",
    PrivnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
    TestnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
private static readonly IContractReference NeoContract;

// Will be detected as development contract (has project path)
[ContractReference("MyContract", ProjectPath = "./MyContract.csproj")]
private static readonly IContractReference MyContract;
```

## Contract Attributes

### ContractReferenceAttribute

Specifies contract reference metadata:

```csharp
[ContractReference(identifier,
    ReferenceType = ContractReferenceType.Auto,  // Auto, Development, or Deployed
    ProjectPath = "path/to/project.csproj",      // For development contracts
    PrivnetAddress = "0x...",                    // Privnet address
    TestnetAddress = "0x...",                    // Testnet address  
    MainnetAddress = "0x...",                    // Mainnet address
    FetchManifest = true,                        // Fetch manifest from blockchain
    CompileAsDependency = true)]                 // Compile as dependency
```

### ContractMethodAttribute

Customizes method invocation behavior:

```csharp
[ContractMethod("actualMethodName",              // Override method name
    ReadOnly = true,                             // Read-only method
    CallFlags = CallFlags.ReadOnly)]             // Custom call flags
public BigInteger GetValue()
{
    return (BigInteger)InvokeReadOnly("getValue");
}
```

## Compilation Process

### Development Contract Resolution

1. **Dependency Discovery**: The compiler scans for `ContractReference` attributes
2. **Project Resolution**: Project paths are resolved relative to the current project
3. **Dependency Compilation**: Referenced projects are compiled first to get their hashes
4. **Address Binding**: Development contract hashes are bound to the references
5. **Code Generation**: `Contract.Call` instructions are generated with resolved addresses

### Deployed Contract Resolution

1. **Network Configuration**: Addresses are loaded for the current target network
2. **Manifest Fetching**: Contract manifests are optionally fetched from the blockchain
3. **Interface Generation**: Type-safe interfaces are generated from manifests
4. **Address Binding**: Network-specific addresses are bound to the references

## Network Management

### Switching Networks

```csharp
// Switch all contracts to testnet
ContractInvocationFactory.SwitchNetwork("testnet");

// Or switch individual contract
myContract.NetworkContext.SwitchNetwork("mainnet");
```

### Multi-Network Deployment

```csharp
var contract = DeployedContractReference.CreateMultiNetwork(
    "MyContract",
    privnetAddress: UInt160.Parse("0x1234..."),
    testnetAddress: UInt160.Parse("0x5678..."),
    mainnetAddress: UInt160.Parse("0x9abc..."));
```

## Advanced Features

### Custom Contract Proxies

Create custom proxy classes for better type safety:

```csharp
public class MyTokenProxy : ContractProxyBase
{
    public MyTokenProxy(IContractReference reference) : base(reference) { }

    [ContractMethod(ReadOnly = true)]
    public string Name() => (string)InvokeReadOnly("name");

    [ContractMethod(ReadOnly = true)]
    public string Symbol() => (string)InvokeReadOnly("symbol");

    [ContractMethod(ReadOnly = true)]
    public byte Decimals() => (byte)InvokeReadOnly("decimals");

    public bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
        => (bool)InvokeWithStates("transfer", from, to, amount, null);
}
```

### Interface Generation

Automatically generate contract interfaces from manifests:

```csharp
var manifest = blockchainInterface.GetContractManifest(contractHash);
var interfaceCode = ContractInterfaceGenerator.GenerateInterface(
    manifest, "MyContract", "Generated.Contracts");
```

### Blockchain Integration

Configure blockchain connectivity for manifest fetching:

```csharp
var context = new CompilationContext
{
    RpcEndpoint = "http://localhost:10332",
    FetchManifests = true,
    NetworkContext = new NetworkContext("testnet")
};
```

## Best Practices

### 1. Organize Contract References

Group related contract references in a separate file:

```csharp
public static class ContractReferences
{
    [ContractReference("NEO", /* addresses */)]
    public static readonly DeployedContractReference Neo;

    [ContractReference("GAS", /* addresses */)]
    public static readonly DeployedContractReference Gas;

    [ContractReference("MyToken", ProjectPath = "../MyToken/MyToken.csproj")]
    public static readonly DevelopmentContractReference MyToken;
}
```

### 2. Use Descriptive Names

```csharp
[ContractReference("UserTokenContract", 
    ProjectPath = "../UserToken/UserToken.csproj")]
private static readonly DevelopmentContractReference UserTokenContract;
```

### 3. Validate Contract Existence

```csharp
public static BigInteger GetBalance(UInt160 account)
{
    if (!TokenContract.IsResolved)
        throw new InvalidOperationException("Token contract not resolved");
    
    var proxy = new Nep17ContractProxy(TokenContract);
    return proxy.BalanceOf(account);
}
```

### 4. Handle Network Differences

```csharp
public static UInt160 GetCurrentTokenAddress()
{
    var network = TokenContract.NetworkContext.CurrentNetwork;
    return network switch
    {
        "mainnet" => TokenContract.NetworkContext.GetNetworkAddress("mainnet")!,
        "testnet" => TokenContract.NetworkContext.GetNetworkAddress("testnet")!,
        _ => TokenContract.NetworkContext.GetNetworkAddress("privnet")!
    };
}
```

## Migration from Direct Contract.Call

### Before (Old Way)

```csharp
[Hash160("0xd2a4cff31913016155e38e474a2c06d08be276cf")]
private static readonly UInt160 GasTokenHash;

public static BigInteger GetGasBalance(UInt160 account)
{
    return (BigInteger)Contract.Call(GasTokenHash, "balanceOf", CallFlags.ReadOnly, account);
}
```

### After (New Way)

```csharp
[ContractReference("GAS",
    PrivnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
    TestnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
    MainnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf")]
private static readonly DeployedContractReference GasContract;

public static BigInteger GetGasBalance(UInt160 account)
{
    var proxy = new Nep17ContractProxy(GasContract);
    return proxy.BalanceOf(account);
}
```

## Troubleshooting

### Common Issues

1. **Contract reference not resolved**: Check project paths and ensure dependencies compile successfully
2. **Network address not found**: Verify addresses are configured for the current network
3. **Method not found**: Ensure contract method names match exactly (case-sensitive)
4. **Compilation errors**: Check that referenced projects have no compilation errors

### Debug Information

Enable verbose logging to see contract resolution details:

```csharp
var context = new CompilationContext();
context.ReportInfo("Resolving contract references...");
```

## Limitations

1. **Compile-time resolution**: Contract references must be resolvable at compilation time
2. **Static references**: Contract references must be declared as static fields
3. **Network consistency**: All referenced contracts must be available on the target network
4. **Manifest compatibility**: Auto-generated interfaces depend on accurate contract manifests

## Future Enhancements

- **Dynamic contract loading**: Support for runtime contract discovery
- **Automatic interface generation**: Generate proxy classes from contract manifests
- **IDE integration**: IntelliSense support for contract methods
- **Version management**: Handle contract updates and version compatibility