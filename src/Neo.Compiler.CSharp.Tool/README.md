# Neo.Compiler.CSharp.Tool

Neo Smart Contract Compiler CLI Tool - Command-line interface for compiling Neo smart contracts.

## Installation

Install as a global .NET tool:

```bash
dotnet tool install -g Neo.Compiler.CSharp.Tool
```

Update to the latest version:

```bash
dotnet tool update -g Neo.Compiler.CSharp.Tool
```

## Usage

Once installed, the `nccs` command will be available globally.

### Basic Usage

Compile a C# project:
```bash
nccs MyContract.csproj
```

Compile source files:
```bash
nccs Contract1.cs Contract2.cs
```

### Options

```
nccs [paths] [options]

Arguments:
  paths                           The path of the solution file, project file, project directory or source files.

Options:
  -o, --output <output>           Specifies the output directory.
  --base-name <base-name>         Specifies the base name of the output files.
  --nullable <nullable>           Represents the default state of nullable analysis. [default: Annotations]
  --checked                       Indicates whether to check for overflow and underflow.
  --assembly                      Indicates whether to generate assembly.
  --generate-artifacts <kind>     Instruct the compiler how to generate artifacts.
  --security-analysis             Whether to perform security analysis.
  --generate-interface            Generate interface file for contracts.
  --generate-plugin               Generate plugin from contract.
  --inline                        Inline methods. [default: True]
  --verbose                       Outputs additional information.
  -d, --debug <None|Portable>     Debug type. [default: None]
  --optimize <optimize>           Optimize output. [default: Basic]
  --version                       Show version information.
  -?, -h, --help                  Show help and usage information.
```

### Examples

Compile with debug information:
```bash
nccs MyContract.csproj -d Portable
```

Compile with optimization:
```bash
nccs MyContract.csproj --optimize All
```

Generate contract interface:
```bash
nccs MyContract.csproj --generate-interface
```

Generate plugin from contract:
```bash
nccs MyContract.csproj --generate-plugin
```

Run security analysis:
```bash
nccs MyContract.csproj --security-analysis
```

Compile to specific output directory:
```bash
nccs MyContract.csproj -o ./build
```

## Output Files

The compiler generates:
- `.nef` - Neo Executable Format file
- `.manifest.json` - Contract manifest
- `.nefdbgnfo` - Debug information (when -d is used)
- `.abi.json` - ABI definition
- Interface files (when --generate-interface is used)
- Plugin files (when --generate-plugin is used)

## For Library Usage

If you need to integrate the compiler into your own tools or applications, use the `Neo.Compiler.CSharp` NuGet package instead.

## License

MIT License - see [LICENSE](https://github.com/neo-project/neo-devpack-dotnet/blob/master/LICENSE) file for details.