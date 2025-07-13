# R3E.Compiler.CSharp

Neo Smart Contract Compiler for C# - Compiles C# code to Neo VM bytecode.

## Overview

R3E.Compiler.CSharp is the official C# compiler for Neo N3 smart contracts. It compiles C# source code into Neo VM bytecode, generating NEF (Neo Executable Format) files and contract manifests that can be deployed to the Neo blockchain.

## Installation

### As a NuGet Package (Library)

```bash
dotnet add package R3E.Compiler.CSharp
```

### As a Global Tool

```bash
dotnet tool install -g R3E.Compiler.CSharp.Tool
```

## Usage

### As a Library

```csharp
using Neo.Compiler;
using Microsoft.CodeAnalysis;

// Create compilation options
var options = new CompilationOptions
{
    Debug = CompilationOptions.DebugType.Extended,
    Optimize = CompilationOptions.OptimizationType.Basic,
    Nullable = NullableContextOptions.Enable
};

// Create compilation engine
var engine = new CompilationEngine(options);

// Compile from project file
var contexts = engine.CompileProject("MyContract.csproj");

// Or compile from source files
var contexts = engine.CompileSources("Contract1.cs", "Contract2.cs");

// Process results
foreach (var context in contexts)
{
    if (context.Success)
    {
        // Generate NEF, manifest, and debug info
        context.CreateResults("./output");
        
        Console.WriteLine($"Compiled {context.ContractName} successfully!");
    }
    else
    {
        // Handle compilation errors
        foreach (var diagnostic in context.Diagnostics)
        {
            Console.WriteLine(diagnostic);
        }
    }
}
```

### Advanced Usage

```csharp
// Compile with custom references
var references = new CompilationSourceReferences
{
    Packages = new[] 
    { 
        ("R3E.SmartContract.Framework", "3.9.0"),
        ("MyCustomPackage", "1.0.0")
    },
    Projects = new[] { "../SharedContracts/Shared.csproj" }
};

var contexts = engine.CompileSources(references, "MyContract.cs");

// Security analysis
foreach (var context in contexts.Where(c => c.Success))
{
    var nef = context.CreateExecutable();
    var manifest = context.CreateManifest();
    var debugInfo = context.CreateDebugInformation();
    
    SecurityAnalyzer.AnalyzeWithPrint(nef, manifest, debugInfo);
}

// Generate contract interface
var interfaceCode = ContractInterfaceGenerator.GenerateInterface(
    "MyContract", 
    manifest, 
    contractHash
);
```

## Features

- **Full C# Language Support**: Supports modern C# features compatible with Neo VM
- **Project and Source Compilation**: Compile from .csproj files or individual source files
- **Optimization**: Multiple optimization levels (None, Basic, Experimental, All)
- **Debug Information**: Generate comprehensive debug information for debugging
- **Security Analysis**: Built-in security analyzer to detect common vulnerabilities
- **Contract Interface Generation**: Automatically generate C# interfaces for compiled contracts
- **Plugin Generation**: Generate Neo node plugins from smart contracts
- **Artifact Generation**: Flexible artifact generation options

## Compilation Options

| Option | Description | Default |
|--------|-------------|---------|
| `Debug` | Debug information level (None, Strict, Extended) | None |
| `Optimize` | Optimization level (None, Basic, Experimental, All) | None |
| `Nullable` | Nullable context options | Annotations |
| `Checked` | Enable overflow/underflow checking | false |
| `NoInline` | Disable method inlining | false |
| `AddressVersion` | Neo address version byte | 53 (0x35) |

## Output Files

The compiler generates the following files:

- **`.nef`** - Neo Executable Format file containing the compiled bytecode
- **`.manifest.json`** - Contract manifest with ABI, permissions, and metadata
- **`.nefdbgnfo`** - Debug information for debugging tools
- **`.abi.json`** - Application Binary Interface definition

## Requirements

- .NET 9.0 or higher
- R3E.SmartContract.Framework reference in your smart contract project

## Examples

See the [examples directory](https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples) for sample smart contracts and compilation scripts.

## License

MIT License - see [LICENSE](https://github.com/neo-project/neo-devpack-dotnet/blob/master/LICENSE) file for details.