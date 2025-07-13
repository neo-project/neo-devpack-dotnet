# R3E Package Naming Guide

## Overview

The R3E Edition of NEO DevPack for .NET uses the `R3E.*` namespace for all packages to distinguish from the official Neo packages. This document outlines the package naming conventions and mappings.

## Package Mappings

| Original Package Name | R3E Package Name | Description |
|----------------------|------------------|-------------|
| Neo.SmartContract.Framework | R3E.SmartContract.Framework | Core framework for smart contract development |
| Neo.Compiler.CSharp | R3E.Compiler.CSharp | C# to NeoVM compiler library |
| Neo.Compiler.CSharp.Tool | R3E.Compiler.CSharp.Tool | CLI tool for contract compilation (rncc) |
| Neo.SmartContract.Testing | R3E.SmartContract.Testing | Testing framework for smart contracts |
| Neo.SmartContract.Analyzer | R3E.SmartContract.Analyzer | Code analyzers for smart contracts |
| Neo.SmartContract.Template | R3E.SmartContract.Template | Project templates for new contracts |
| Neo.Disassembler.CSharp | R3E.Disassembler.CSharp | NeoVM bytecode disassembler |
| Neo.SmartContract.Deploy | R3E.SmartContract.Deploy | Deployment toolkit for contracts |

## Installation

### Package Manager Console

```powershell
Install-Package R3E.SmartContract.Framework -Version 0.0.1
Install-Package R3E.SmartContract.Testing -Version 0.0.1
Install-Package R3E.Compiler.CSharp -Version 0.0.1
Install-Package R3E.SmartContract.Deploy -Version 0.0.1
```

### .NET CLI

```bash
dotnet add package R3E.SmartContract.Framework --version 0.0.1
dotnet add package R3E.SmartContract.Testing --version 0.0.1
dotnet add package R3E.Compiler.CSharp --version 0.0.1
dotnet add package R3E.SmartContract.Deploy --version 0.0.1
```

### PackageReference

```xml
<ItemGroup>
  <PackageReference Include="R3E.SmartContract.Framework" Version="0.0.1" />
  <PackageReference Include="R3E.SmartContract.Testing" Version="0.0.1" />
  <PackageReference Include="R3E.Compiler.CSharp" Version="0.0.1" />
  <PackageReference Include="R3E.SmartContract.Deploy" Version="0.0.1" />
</ItemGroup>
```

## CLI Tool

The command-line compiler tool is installed as:

```bash
dotnet tool install -g R3E.Compiler.CSharp.Tool --version 0.0.1
```

And is invoked using the `rncc` command (R3E Neo Contract Compiler).

## Namespace Usage

While package names use the `R3E.*` prefix, the namespaces in code remain compatible with the original Neo namespaces to ensure code compatibility:

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
```

This allows existing smart contract code to work without modification.

## Publishing

All R3E packages are published to NuGet.org under the R3E organization prefix to ensure clear differentiation from official Neo packages.

## Version Policy

R3E packages follow independent versioning starting from 0.0.1, separate from the official Neo package versions.