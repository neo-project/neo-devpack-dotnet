# Neo Compiler Fuzzer Documentation

The Neo Compiler Fuzzer is a tool designed to test the Neo N3 smart contract compiler by generating random, valid smart contracts with various combinations of C# syntax features and Neo-specific functionality. This documentation provides a comprehensive guide to using and understanding the Neo Compiler Fuzzer.

## Table of Contents

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Usage](#usage)
4. [Features](#features)
5. [Architecture](#architecture)
6. [Configuration](#configuration)
7. [Reports and Statistics](#reports-and-statistics)
8. [Troubleshooting](#troubleshooting)
9. [Contributing](#contributing)

## Introduction

The Neo Compiler Fuzzer is designed to ensure the reliability and correctness of the Neo N3 compiler by testing its ability to compile a wide range of valid C# smart contracts. It dynamically generates contracts with random combinations of features, compiles them, and optionally tests their execution.

The fuzzer can be run for a specified number of iterations or for a specified duration (minutes, hours, days, or weeks), making it suitable for both quick tests and extended fuzzing sessions.

## Installation

The Neo Compiler Fuzzer is part of the Neo DevPack and can be built from source:

```bash
# Clone the repository
git clone https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet

# Build the project
dotnet build src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj
```

## Usage

The Neo Compiler Fuzzer can be run using the provided shell script or directly with `dotnet run`.

### Using the Shell Script

```bash
# Run with default settings (5 iterations, 3 features per contract)
./scripts/fuzzer/run-compiler-fuzzer.sh

# Run with custom settings
./scripts/fuzzer/run-compiler-fuzzer.sh --iterations 10 --features 3 --output CustomOutput

# Run for a specified duration
./scripts/fuzzer/run-compiler-fuzzer.sh --duration 24h --features 3 --checkpoint-interval 60
```

### Using dotnet run

```bash
# Run with default settings
dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic

# Run with custom settings
dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --iterations 10 --features 3 --output CustomOutput

# Run for a specified duration
dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --duration 24h --features 3 --checkpoint-interval 60
```

### Command-Line Options

| Option | Description | Default |
|--------|-------------|---------|
| `--iterations <n>` | Number of contracts to generate | 5 |
| `--features <n>` | Number of features per contract | 3 |
| `--output <dir>` | Output directory for generated contracts | GeneratedContracts |
| `--no-execution` | Skip execution testing of generated contracts | (execution enabled) |
| `--log-level <level>` | Set log level: Debug, Info, Warning, Error, Fatal | Info |
| `--duration <time>` | Run for a specified duration instead of fixed iterations<br>Format: `<n>m` (minutes), `<n>h` (hours), `<n>d` (days), `<n>w` (weeks), or 'indefinite' | (not set) |
| `--checkpoint-interval <n>` | Interval in minutes between checkpoints in long-running mode | 30 |
| `--help` | Show help message | |

## Features

The Neo Compiler Fuzzer tests a wide range of C# syntax features and Neo-specific functionality, including:

- **Data Types**: Primitive types, complex types, arrays, structs, tuples, etc.
- **Control Flow**: If statements, loops, switch statements, ternary operators, etc.
- **Storage Operations**: Basic storage operations, storage maps, storage contexts, etc.
- **Runtime Operations**: Runtime properties, notifications, logging, etc.
- **Native Contract Calls**: NEO, GAS, ContractManagement, etc.
- **Neo N3 Specific Features**: NFT operations, name service operations, etc.
- **Exception Handling**: Try-catch blocks, throw statements, etc.
- **Operators and Expressions**: Arithmetic, comparison, logical operators, etc.
- **String and Math Operations**: String manipulation, math functions, etc.
- **Contract Features**: Contract attributes, contract calls, events, etc.

For a complete list of supported features, see the [Features](features.md) documentation.

## Architecture

The Neo Compiler Fuzzer consists of several components:

- **DynamicContractFuzzer**: The main class that orchestrates the fuzzing process.
- **FragmentGenerator**: Generates code fragments for various C# syntax features.
- **ContractCompiler**: Compiles the generated contracts using the Neo.Compiler.CSharp CompilationEngine.
- **Logger**: Provides logging functionality for the fuzzer.

For more details on the architecture, see the [Architecture](architecture.md) documentation.

## Configuration

The Neo Compiler Fuzzer can be configured through command-line options. For more advanced configuration, you may need to modify the source code.

For more details on configuration, see the [Configuration](configuration.md) documentation.

## Reports and Statistics

The Neo Compiler Fuzzer generates detailed reports and statistics about the fuzzing process, including:

- **Success Rate**: The percentage of contracts that compiled successfully.
- **Feature Success Rates**: The success rate for each feature.
- **Detailed Results**: A list of all contracts with their status and features.
- **Execution Environment**: Information about the environment in which the fuzzer was run.

For more details on reports and statistics, see the [Reports and Statistics](reports.md) documentation.

## Troubleshooting

If you encounter issues with the Neo Compiler Fuzzer, check the following:

- Ensure you have the correct version of .NET installed.
- Check the log files in the `Logs` directory for error messages.
- Try reducing the number of features per contract to increase the success rate.

For more troubleshooting tips, see the [Troubleshooting](troubleshooting.md) documentation.

## Contributing

Contributions to the Neo Compiler Fuzzer are welcome! If you find a bug or have a feature request, please open an issue on the [Neo DevPack GitHub repository](https://github.com/neo-project/neo-devpack-dotnet).

For more information on contributing, see the [Contributing](contributing.md) documentation.
