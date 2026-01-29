# Neo.Disassembler.CSharp

A disassembler for NeoVM bytecode that converts compiled Neo smart contracts back into readable C#-like instructions.

## Features

- **NeoVM Disassembly**: Convert NeoVM bytecode to human-readable instructions
- **Debug Info Parsing**: Parse and utilize debug information for source mapping
- **Method Extraction**: Extract and display individual contract methods
- **Instruction Analysis**: Detailed opcode analysis with parameter decoding

## Installation

```bash
dotnet add package Neo.Disassembler.CSharp
```

## Usage

### Basic Disassembly

```csharp
using Neo.Disassembler.CSharp;
using Neo.SmartContract;

// Load a NEF file
var nef = Neo.IO.Helper.AsSerializable<NefFile>(File.ReadAllBytes("contract.nef"));

// Disassemble the entire script
var instructions = Disassembler.ConvertScriptToInstructions(nef.Script);

foreach (var (address, instruction) in instructions)
{
    Console.WriteLine($"{address:X4}: {instruction}");
}
```

### Method-Specific Disassembly

```csharp
// Disassemble a specific method using debug info
var debugInfo = JObject.Parse(File.ReadAllText("contract.debug.json"));
var method = Disassembler.GetMethod(methodDescriptor, debugInfo);
var (start, end) = Disassembler.GetMethodStartEndAddress(method);
var methodInstructions = Disassembler.ConvertMethodToInstructions(nef, start, end);
```

## API Reference

### Main Classes

- `Disassembler` - Core disassembly functionality
- `Instruction` - Represents a single NeoVM instruction
- `DebugInfo` - Debug information parser

## Integration

This package is used by:
- Neo.SmartContract.Testing - For test coverage analysis
- Neo.Compiler.CSharp - For debugging support

## License

MIT - See [LICENSE](../../LICENSE) for details.
