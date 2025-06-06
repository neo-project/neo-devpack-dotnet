# Neo Compiler Fuzzer

A dynamic fuzzing system for Neo N3 smart contracts that generates contracts with various supported C# syntax features and Neo N3 features to ensure they can be compiled and executed correctly.

## Features

- **Dynamic Contract Generation**: Generates contract fragments on-the-fly for testing various situations
- **Comprehensive Coverage**: Tests all Neo N3 supported syntax features and system calls
- **Automatic Fragment Sizing**: Breaks large inputs into smaller chunks for better testing
- **Detailed Reporting**: Generates comprehensive reports of test results
- **Production-Ready Compilation**: Uses the Neo.Compiler.CSharp CompilationEngine directly
- **Basic Execution Validation**: Validates the compiled contracts
- **Long-Running Mode**: Supports running for extended periods (hours, days, weeks, or indefinitely)
- **Checkpointing**: Regular checkpoints to save progress and statistics
- **Robust Error Handling**: Comprehensive error handling and logging

## Getting Started

### Prerequisites

- .NET 9.0 SDK

### Usage

#### Using the Shell Script

The easiest way to run the fuzzer is using the provided shell script:

```bash
./scripts/fuzzer/run-compiler-fuzzer.sh
```

Run with custom options:
```bash
./scripts/fuzzer/run-compiler-fuzzer.sh --iterations 10 --features 3 --output CustomOutput
```

#### Using dotnet run

Alternatively, you can run the fuzzer directly with dotnet:

```bash
dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic
```

Run with custom options:
```bash
dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --iterations 10 --features 3 --output CustomOutput
```

### Available Options

- `--iterations <n>`: Number of iterations (default: 5)
- `--features <n>`: Number of features per contract (default: 3)
- `--output <dir>`: Output directory for generated contracts (default: GeneratedContracts)
- `--no-execution`: Disable contract execution testing
- `--log-level <level>`: Set log level: Debug, Info, Warning, Error, Fatal (default: Info)
- `--duration <time>`: Run for a specified duration instead of fixed iterations
  - Format: `<n>m` (minutes), `<n>h` (hours), `<n>d` (days), `<n>w` (weeks), or 'indefinite'
- `--checkpoint-interval <n>`: Interval in minutes between checkpoints in long-running mode (default: 30)
- `--help`: Show help message

### Examples

```bash
# Run for 10 iterations with 3 features per contract
./scripts/fuzzer/run-compiler-fuzzer.sh --iterations 10 --features 3

# Run with custom output directory and debug logging
./scripts/fuzzer/run-compiler-fuzzer.sh --output CustomOutput --log-level Debug

# Run for 24 hours with checkpoints every 30 minutes
./scripts/fuzzer/run-compiler-fuzzer.sh --duration 24h --features 3

# Run for 7 days with checkpoints every hour
./scripts/fuzzer/run-compiler-fuzzer.sh --duration 7d --checkpoint-interval 60

# Run indefinitely with debug logging
./scripts/fuzzer/run-compiler-fuzzer.sh --duration indefinite --log-level Debug
```

## Architecture

The fuzzer is composed of several key components:

1. **FragmentGenerator**: Generates random code fragments for different Neo N3 features
2. **ContractCompiler**: Compiles the generated contracts using the Neo compiler
3. **LongRunningFuzzer**: Manages long-running fuzzing sessions with checkpointing
4. **Templates**: Contains templates for generating contracts

## Supported Features

The Neo Compiler Fuzzer supports the following Neo N3 features:

### Data Types
- Primitive types (bool, byte, sbyte, short, ushort, int, uint, long, ulong, string)
- Complex types (BigInteger, UInt160, UInt256, ECPoint, ByteString, Map, List, Dictionary)
- Arrays and collections

### Control Flow
- If statements
- Switch statements
- For loops
- While loops
- Do-while loops
- Foreach loops
- Try-catch-finally blocks
- Using statements
- Nested control flow
- Jump statements (break, continue, return)

### Operators
- Arithmetic operations (+, -, *, /, %)
- Logical operations (&&, ||, !)
- Bitwise operations (&, |, ^, ~, <<, >>)
- Comparison operations (==, !=, <, >, <=, >=)
- Assignment operations (=, +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=)
- Conditional operator (? :)
- Null-coalescing operator (??)
- Null-conditional operator (?.)
- Null-coalescing assignment operator (??=)

### String and Math Operations
- String operations (Length, Substring, ToUpper, ToLower, etc.)
- String concatenation
- String interpolation
- String splitting
- String joining
- Character operations
- Math operations (Abs, Max, Min, Sign, etc.)
- Type conversion operations
- ByteString operations

### Storage Operations
- Basic storage (Put/Get/Delete)
- StorageMap operations
- Storage.Find with various options
- Storage contexts (CurrentContext, ReadOnlyContext)

### Runtime Operations
- Basic properties (Platform, Trigger, ExecutingScriptHash, etc.)
- Notifications (GetNotifications)
- Logging (Runtime.Log)
- CheckWitness
- Gas operations (BurnGas)
- Random number generation (GetRandom)

### Native Contract Calls
- NEO token (Symbol, Decimals, TotalSupply, BalanceOf, etc.)
- GAS token (Symbol, Decimals, TotalSupply, BalanceOf)
- ContractManagement (GetContract, HasMethod, etc.)
- CryptoLib (Sha256, Ripemd160, VerifyWithECDsa, etc.)
- Ledger (CurrentHash, CurrentIndex, GetBlock, etc.)
- Oracle (MinimumResponseFee, GetPrice)
- Policy (GetFeePerByte, GetExecFeeFactor, etc.)
- RoleManagement (GetDesignatedByRole)
- StdLib (JsonSerialize, Base64Encode, Itoa, etc.)

### Contract Features
- Contract attributes (DisplayName, ContractDescription, etc.)
- Contract calls with various flags and parameters
- Stored properties
- Contract methods with attributes
- Events (declaration and emission)

## Extending the Fuzzer

To add support for additional features:

1. Add new methods to the `FragmentGenerator` class
2. Register the new features in the `_featureGenerators` dictionary in the `DynamicContractFuzzer` constructor

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Neo Global Development for the Neo N3 platform
- The Neo community for their support and feedback
