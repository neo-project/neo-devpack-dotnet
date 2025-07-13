# Release Notes - NEO DevPack for .NET v0.0.1 (R3E Edition)

## Overview

We are excited to announce the first release of the R3E Edition of NEO DevPack for .NET. This release introduces significant enhancements to the Neo smart contract development experience, including a rebranded compiler and new code generation capabilities.

## Major Features

### 🚀 R3E Neo Contract Compiler (rncc)

The Neo C# compiler has been rebranded as the **R3E Neo Contract Compiler** with the command-line tool renamed from `nccs` to `rncc`. This change reflects the enhanced capabilities and improved developer experience.

### 🌐 Web GUI Generation

New `--generate-webgui` option automatically generates interactive web interfaces for your compiled contracts:

```bash
rncc MyContract.cs --generate-webgui -o ./output
```

Features:
- Interactive method invocation interface
- Real-time transaction monitoring
- Contract state visualization
- Wallet connection support
- Responsive design for all devices

### 🔌 Enhanced Plugin Generation

Improved `--generate-plugin` option for Neo N3 plugin creation:

```bash
rncc MyContract.cs --generate-plugin -o ./output
```

Generates:
- Complete Neo N3 plugin project structure
- CLI commands for all contract methods
- Contract wrapper classes
- Type-safe parameter handling
- Comprehensive error handling

### 📦 Production-Ready Deployment Toolkit

The deployment toolkit has been enhanced for production use:

```csharp
var toolkit = new DeploymentToolkit()
    .SetNetwork("testnet")
    .SetWifKey("your-wif-key");

var result = await toolkit.DeployAsync("MyContract.cs");
```

Features:
- Simplified API for deployment
- Multi-network support (mainnet, testnet, local)
- Automatic configuration management
- Contract update capabilities
- Comprehensive error handling

## Installation

### CLI Tool

```bash
dotnet tool install -g R3E.Compiler.CSharp.Tool --version 0.0.1
```

### NuGet Packages

```xml
<PackageReference Include="R3E.SmartContract.Framework" Version="0.0.1" />
<PackageReference Include="R3E.Compiler.CSharp" Version="0.0.1" />
<PackageReference Include="R3E.SmartContract.Testing" Version="0.0.1" />
<PackageReference Include="R3E.SmartContract.Deploy" Version="0.0.1" />
```

## Breaking Changes

- The compiler CLI tool has been renamed from `nccs` to `rncc`
- All package names have been changed from `Neo.*` to `R3E.*` to distinguish from official Neo packages
- All build scripts and automation using `nccs` must be updated to use `rncc`
- Package references in project files must be updated to use R3E package names

## Migration Guide

### Update CLI Commands

Replace all instances of `nccs` with `rncc` in your scripts:

```bash
# Old
nccs MyContract.cs -o ./output

# New
rncc MyContract.cs -o ./output
```

### Update Project Files

In your `.csproj` files, update any references:

```xml
<!-- Old -->
<Exec Command="nccs $(ProjectPath)" />

<!-- New -->
<Exec Command="rncc $(ProjectPath)" />
```

## Known Issues

- Web GUI generation requires manual hosting for production use
- Plugin generation currently targets Neo CLI v3.x

## Future Enhancements

- Web GUI hosting service integration
- Enhanced plugin templates for common use cases
- Visual Studio and VS Code extensions
- Improved debugging capabilities

## Support

For issues and feature requests, please visit:
- GitHub: https://github.com/neo-project/neo-devpack-dotnet/issues
- Discord: https://discord.gg/rvZFQ5382k

## Contributors

Special thanks to all contributors who made this release possible.

---

**Note**: This is a major version reset to 0.0.1 marking the beginning of the R3E edition of NEO DevPack for .NET.