# Neo N3 Smart Contract Fuzzer

*Updated: April 20, 2025*

A comprehensive fuzzing and vulnerability detection tool for Neo N3 smart contracts that tests contract methods with randomly generated inputs and performs symbolic execution for deep analysis.

## Overview

The Neo N3 Smart Contract Fuzzer is designed to help developers test Neo N3 smart contracts by automatically generating random inputs for contract methods and executing them in a controlled environment. This helps identify potential issues, vulnerabilities, or unexpected behaviors in the contract before deployment to the mainnet. The fuzzer has been enhanced with specialized capabilities for Neo N3-specific features, types, and vulnerabilities.

The fuzzer works by:
1. Loading a contract from its NEF and manifest files
2. Identifying the methods available in the contract
3. Generating random but valid inputs for each method
4. Executing the methods with these inputs
5. Performing symbolic execution to explore execution paths
6. Detecting potential vulnerabilities and security issues
7. Recording and analyzing the results

## Features

- **Contract Analysis**: Automatically analyzes contract methods and their parameter types
- **Smart Input Generation**: Generates random but valid inputs for various Neo N3 parameter types, including specialized types like Hash160, Storage Keys, and cryptographic parameters
- **Execution Environment**: Runs the contract in a controlled Neo N3 VM environment with full Neo N3 syscall support
- **Symbolic Execution**: Explores multiple execution paths to find hard-to-reach vulnerabilities
  - Path exploration with constraint tracking
  - Concrete input generation from symbolic paths
  - Neo N3 syscall modeling with symbolic values
  - Deep vulnerability detection with Neo N3-specific analysis
  - Multiple search strategies (DFS, BFS, Random, Coverage-guided)
- **Vulnerability Detection**: Identifies common smart contract vulnerabilities and security issues
  - Integer overflow/underflow
  - Reentrancy vulnerabilities
  - Unauthorized access
  - Improper exception handling
  - Gas-related vulnerabilities
  - Logic vulnerabilities
  - Neo N3 native contract interaction issues
  - Storage manipulation vulnerabilities
  - Oracle service vulnerabilities
  - NEP-11/NEP-17 implementation issues
- **Result Analysis**: Identifies exceptions, state changes, and unexpected behaviors
- **Comprehensive Reporting**: Generates detailed reports of the fuzzing and symbolic execution results
  - Multiple formats (Markdown, JSON, HTML, CSV)
  - Detailed execution path analysis
  - Vulnerability summaries with recommendations
  - Code coverage statistics
- **Reproducible Testing**: Supports fixed random seeds for reproducible tests
- **Code Coverage Analysis**: Provides comprehensive code coverage analysis with multiple report formats

## Prerequisites

- .NET 9.0 SDK or later
- Neo N3 contract NEF and manifest files

## Quick Start

1. Build the fuzzer:
   ```bash
   dotnet build
   ```

2. Run the fuzzer on a contract:
   ```bash
   dotnet run -- --nef <path-to-nef-file> --manifest <path-to-manifest-file>
   ```

3. Run the fuzzer with symbolic execution enabled:
   ```bash
   dotnet run -- --nef <path-to-nef-file> --manifest <path-to-manifest-file> --enable-symbolic-execution
   ```

4. Run the fuzzer with advanced configuration:
   ```bash
   dotnet run -- --nef <path-to-nef-file> --manifest <path-to-manifest-file> --enable-symbolic-execution --symbolic-execution-depth 50 --symbolic-execution-max-paths 20 --report-formats Markdown,Html,Json
   ```

5. Check the results in the `FuzzingResults` directory.

## Documentation

For detailed usage instructions, see the [USAGE.md](Documentation/USAGE.md) file.

To learn about the code coverage feature, see [CODE_COVERAGE.md](Documentation/CODE_COVERAGE.md).

For information about the symbolic execution engine, see [SymbolicExecution.md](Documentation/SymbolicExecution.md).

To understand the vulnerability detection system, see [VulnerabilityDetection.md](Documentation/VulnerabilityDetection.md).

## Architecture

The Neo Smart Contract Fuzzer consists of several components:

1. **Contract Loader**: Loads and parses NEF and manifest files
2. **Method Analyzer**: Identifies methods and their parameters
3. **Parameter Generator**: Generates random inputs for different parameter types
   - **NeoParameterGenerator**: Specialized generator for Neo N3 parameter types
4. **Execution Engine**: Executes contract methods in a Neo VM environment
5. **Symbolic Execution Engine**: Explores multiple execution paths symbolically
   - **SymbolicVirtualMachine**: Executes Neo N3 VM instructions symbolically
   - **SymbolicValue**: Represents symbolic values and operations
   - **SymbolicConstraint**: Tracks path constraints
   - **ConstraintSolver**: Generates concrete inputs from constraints
   - **SymbolicState**: Manages execution state during symbolic execution
   - **SyscallHandler**: Models Neo N3 syscalls symbolically
   - **NativeContractSimulator**: Simulates Neo N3 native contracts
   - **SymbolicExecutionResult**: Captures results of symbolic execution
6. **Vulnerability Detector**: Identifies potential security vulnerabilities
7. **Result Analyzer**: Analyzes execution results and identifies issues
8. **Report Generator**: Generates detailed reports of the fuzzing and symbolic execution results

## Supported Parameter Types

The fuzzer supports generating random inputs for the following Neo N3 parameter types:

- Boolean
- Integer (various sizes)
- String
- ByteArray
- Hash160 (with Neo N3 contract addresses)
- Hash256 (with transaction and block hashes)
- PublicKey (with valid EC points)
- Signatures (with valid ECDSA signatures)
- Storage Keys (with Neo N3-specific prefixes)
- Array (with nested Neo N3 types)
- Map (with complex key-value pairs)
- Struct (with combined Neo N3 types)
- InteropInterface (for native contract interaction)

## Extending the Fuzzer

The fuzzer is designed to be extensible. You can:

1. Add support for new parameter types
2. Implement custom input generation strategies
3. Add new analysis techniques
4. Create custom report formats

See the [CONTRIBUTING.md](../../CONTRIBUTING.md) file for more information.

## License

The Neo Smart Contract Fuzzer is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.