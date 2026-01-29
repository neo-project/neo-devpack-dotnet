# Neo.Compiler.CSharp

The official C# compiler for Neo smart contracts. This compiler transforms C# code into NeoVM-compatible bytecode (.nef files) and generates contract manifests.

## Features

- **C# to NeoVM Compilation**: Compile standard C# code to NeoVM bytecode
- **Smart Contract Support**: Full support for Neo smart contract development
- **Optimization**: Multiple optimization levels for bytecode efficiency
- **Debug Information**: Generate debug info for testing and debugging
- **Artifact Generation**: Auto-generate contract artifacts for testing

## Installation

### As a .NET Tool (Recommended)

```bash
dotnet tool install --global Neo.Compiler.CSharp
```

### As a Package Reference

```bash
dotnet add package Neo.Compiler.CSharp
```

## Usage

### Command Line

```bash
# Compile a contract
nccs MyContract.csproj

# Compile with specific output
nccs MyContract.csproj -o ./output/

# Compile in debug mode
nccs MyContract.csproj --debug

# Generate artifacts
nccs MyContract.csproj --generate-artifacts source
```

### MSBuild Integration

The compiler automatically integrates with MSBuild. Simply build your project:

```bash
dotnet build
```

## Project Configuration

Add to your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <NeoContract>true</NeoContract>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.9.1" />
  </ItemGroup>
</Project>
```

## Documentation

For detailed documentation, visit: https://github.com/neo-project/neo-devpack-dotnet

## License

MIT - See [LICENSE](../../LICENSE) for details.
