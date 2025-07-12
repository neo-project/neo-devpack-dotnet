# Using Neo.Compiler.CSharp as a Library

Starting with version 3.9.0, the Neo C# compiler is available as a NuGet library package that can be referenced in your own projects. This allows developers to integrate Neo smart contract compilation directly into their applications.

## Installation

Add the Neo.Compiler.CSharp package to your project:

```bash
dotnet add package Neo.Compiler.CSharp --version 3.9.0
```

## Basic Usage

Here's a simple example of how to compile a Neo smart contract programmatically:

```csharp
using Neo.Compiler;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        // Configure compilation options
        var options = new CompilationOptions
        {
            Debug = CompilationOptions.DebugType.Extended,
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
        };

        // Create compilation engine
        var engine = new CompilationEngine(options);

        // Compile a single source file
        string sourcePath = "MyContract.cs";
        var contexts = engine.CompileSources(new[] { sourcePath });

        // Or compile a project
        // var contexts = engine.CompileProject("MyContract.csproj");

        foreach (var context in contexts)
        {
            if (context.Success)
            {
                // Generate NEF, manifest, and debug info
                var (nef, manifest, debugInfo) = context.CreateResults(".");
                
                // Save the compiled contract
                string baseName = context.ContractName;
                File.WriteAllBytes($"{baseName}.nef", nef.ToArray());
                File.WriteAllBytes($"{baseName}.manifest.json", manifest.ToJson().ToByteArray(false));
                
                Console.WriteLine($"Successfully compiled {baseName}");
            }
            else
            {
                // Handle compilation errors
                foreach (var diagnostic in context.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
        }
    }
}
```

## Advanced Features

### Custom Compilation Options

```csharp
var options = new CompilationOptions
{
    Debug = CompilationOptions.DebugType.Extended,
    Optimize = CompilationOptions.OptimizationType.All,
    Checked = true,                    // Enable overflow checking
    NoInline = false,                  // Allow inlining
    AddressVersion = 0x35,             // Neo N3 address version
    BaseName = "MyCustomContract"      // Override output name
};
```

### Compiling Multiple Files

```csharp
var sourceFiles = new[]
{
    "Contract1.cs",
    "Contract2.cs",
    "SharedLibrary.cs"
};

var contexts = engine.CompileSources(sourceFiles);
```

### Accessing Compilation Diagnostics

```csharp
foreach (var context in contexts)
{
    foreach (var diagnostic in context.Diagnostics)
    {
        var severity = diagnostic.Severity;
        var message = diagnostic.GetMessage();
        var location = diagnostic.Location.GetLineSpan();
        
        Console.WriteLine($"[{severity}] {location}: {message}");
    }
}
```

### Generating Contract Artifacts

```csharp
if (context.Success)
{
    var (nef, manifest, debugInfo) = context.CreateResults(".");
    
    // Generate contract interface source code
    var interfaceSource = manifest.GetArtifactsSource(
        baseName: context.ContractName,
        nef: nef,
        debugInfo: debugInfo
    );
    
    File.WriteAllText($"{context.ContractName}.artifacts.cs", interfaceSource);
}
```

## Integration with Build Tools

You can integrate the compiler library into MSBuild or other build systems:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="Neo.Compiler.CSharp" Version="3.9.0" />
  </ItemGroup>

  <Target Name="CompileNeoContracts" BeforeTargets="Build">
    <!-- Custom MSBuild task using the compiler library -->
  </Target>
</Project>
```

## Tool Package

If you need the command-line interface, install the tool package globally:

```bash
dotnet tool install -g Neo.Compiler.CSharp.Tool --version 3.9.0
```

Then use it from the command line:

```bash
nccs MyContract.cs --debug --optimize=All
```

## API Reference

Key classes and interfaces:

- `CompilationEngine`: Main entry point for compilation
- `CompilationOptions`: Configuration for the compilation process
- `CompilationContext`: Results and diagnostics from compilation
- `DumpNef`: Utilities for NEF file analysis
- `CompilationException`: Exceptions thrown during compilation

## Migration from CLI Tool

If you were previously using the `nccs` command-line tool in your build scripts, you can now integrate compilation directly into your application for better control and error handling.

Before (using CLI):
```bash
nccs MyContract.cs -o ./output --debug
```

After (using library):
```csharp
var engine = new CompilationEngine(new CompilationOptions { Debug = DebugType.Extended });
var results = engine.CompileSources(new[] { "MyContract.cs" });
// Process results programmatically
```

## Requirements

- .NET 9.0 or later
- Neo.SmartContract.Framework (automatically included as a dependency)

## See Also

- [Neo Smart Contract Documentation](https://docs.neo.org/docs/n3/develop/write/basics)
- [Neo.SmartContract.Framework Documentation](https://github.com/neo-project/neo-devpack-dotnet)