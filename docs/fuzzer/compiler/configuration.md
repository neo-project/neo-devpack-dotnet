# Neo Compiler Fuzzer Configuration

This document describes the configuration options for the Neo Compiler Fuzzer, including command-line options and advanced configuration through source code modification.

## Command-Line Options

The Neo Compiler Fuzzer supports the following command-line options:

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

### Examples

```bash
# Run with default settings (5 iterations, 3 features per contract)
./scripts/fuzzer/run-compiler-fuzzer.sh

# Run with 10 iterations and 5 features per contract
./scripts/fuzzer/run-compiler-fuzzer.sh --iterations 10 --features 5

# Run with custom output directory and debug logging
./scripts/fuzzer/run-compiler-fuzzer.sh --output CustomOutput --log-level Debug

# Run for 24 hours with checkpoints every hour
./scripts/fuzzer/run-compiler-fuzzer.sh --duration 24h --checkpoint-interval 60

# Run indefinitely with debug logging
./scripts/fuzzer/run-compiler-fuzzer.sh --duration indefinite --log-level Debug
```

## Log Levels

The Neo Compiler Fuzzer supports the following log levels:

| Level | Description |
|-------|-------------|
| Debug | Detailed information for debugging purposes |
| Info | General information about the fuzzing process |
| Warning | Potential issues that don't prevent the fuzzer from running |
| Error | Errors that prevent a specific operation from completing |
| Fatal | Critical errors that prevent the fuzzer from running |

## Duration Format

The `--duration` option supports the following formats:

| Format | Description | Example |
|--------|-------------|---------|
| `<n>m` | Minutes | `30m` for 30 minutes |
| `<n>h` | Hours | `24h` for 24 hours |
| `<n>d` | Days | `7d` for 7 days |
| `<n>w` | Weeks | `2w` for 2 weeks |
| `indefinite` | Run indefinitely until manually stopped | `indefinite` |

If only a number is provided without a suffix, it is interpreted as minutes.

## Checkpointing

The Neo Compiler Fuzzer supports checkpointing for long-running fuzzing sessions. Checkpoints are saved at regular intervals specified by the `--checkpoint-interval` option.

Each checkpoint includes:
- A summary report of the fuzzing results
- Statistics in JSON format
- Logs of the fuzzing process

Checkpoints are saved in the `Checkpoints` directory within the output directory.

## Output Directory Structure

The Neo Compiler Fuzzer creates the following directory structure in the output directory:

```
OutputDirectory/
├── Checkpoints/
│   ├── DynamicContractFuzzerReport_Checkpoint_YYYYMMDD_HHMMSS.md
│   └── Statistics_YYYYMMDD_HHMMSS.json
├── Logs/
│   └── DynamicContractFuzzer_YYYYMMDD_HHMMSS.log
├── DynamicContract1.cs
├── DynamicContract2.cs
├── ...
├── DynamicContractFuzzerReport.md
└── FinalStatistics.json
```

## Advanced Configuration

For advanced configuration, you may need to modify the source code of the Neo Compiler Fuzzer.

### Feature Generators

The `FragmentGenerator` class contains a dictionary of feature generators that generate code fragments for various features. You can modify these generators or add new ones to test additional features.

```csharp
private readonly Dictionary<string, Func<string>> _featureGenerators = new Dictionary<string, Func<string>>
{
    // Data Types
    { "PrimitiveTypes", GeneratePrimitiveTypes },
    { "ComplexTypes", GenerateComplexTypes },
    // ...
};
```

### Feature Weights

You can modify the weights of features to control their probability of being selected. Features with higher weights are more likely to be selected.

```csharp
private readonly Dictionary<string, int> _featureWeights = new Dictionary<string, int>
{
    { "PrimitiveTypes", 10 },
    { "ComplexTypes", 5 },
    // ...
};
```

### Feature Dependencies

You can define dependencies between features to ensure that dependent features are included when a feature is selected.

```csharp
private readonly Dictionary<string, List<string>> _featureDependencies = new Dictionary<string, List<string>>
{
    { "NFTOperations", new List<string> { "RuntimeOperation" } },
    { "NameServiceOperations", new List<string> { "RuntimeOperation" } },
    // ...
};
```

### Feature Incompatibilities

You can define incompatibilities between features to ensure that incompatible features are not selected together.

```csharp
private readonly Dictionary<string, List<string>> _featureIncompatibilities = new Dictionary<string, List<string>>
{
    { "ContractAttributes", new List<string> { "AttributeUsage" } },
    // ...
};
```

### Compilation Options

You can modify the compilation options used by the `ContractCompiler` class to control how contracts are compiled.

```csharp
private CompilationOptions GetCompilationOptions()
{
    return new CompilationOptions
    {
        Debug = true,
        Optimize = false,
        // ...
    };
}
```

## Environment Variables

The Neo Compiler Fuzzer does not currently use environment variables for configuration. All configuration is done through command-line options or source code modification.

## Configuration Files

The Neo Compiler Fuzzer does not currently use configuration files. All configuration is done through command-line options or source code modification.

## Conclusion

The Neo Compiler Fuzzer provides a flexible configuration system through command-line options and source code modification. This allows you to customize the fuzzing process to suit your needs, whether you're running a quick test or an extended fuzzing session.
