# Neo Compiler Fuzzer Troubleshooting

This document provides solutions to common issues that may arise when using the Neo Compiler Fuzzer.

## Common Issues

### Low Success Rate

**Issue**: The fuzzer has a low success rate, with many contracts failing to compile.

**Possible Causes**:
- Too many features per contract, leading to incompatible combinations
- Issues with specific features
- Bugs in the Neo N3 compiler

**Solutions**:
1. Reduce the number of features per contract using the `--features` option:
   ```bash
   ./scripts/fuzzer/run-compiler-fuzzer.sh --features 3
   ```
2. Analyze the feature success rates in the report to identify problematic features
3. Modify the feature generators in the source code to generate simpler code fragments
4. Filter out problematic features by modifying the `_featureGenerators` dictionary in the `FragmentGenerator` class

### Out of Memory Errors

**Issue**: The fuzzer crashes with out of memory errors during long-running sessions.

**Possible Causes**:
- Memory leaks in the fuzzer or the Neo N3 compiler
- Insufficient memory for the number of contracts being generated
- Large contracts consuming too much memory

**Solutions**:
1. Run the fuzzer with a smaller number of iterations or for a shorter duration
2. Increase the available memory for the .NET process:
   ```bash
   export DOTNET_GCHeapHardLimit=8000000000  # Set heap limit to 8GB
   ./scripts/fuzzer/run-compiler-fuzzer.sh
   ```
3. Modify the feature generators to produce smaller code fragments
4. Run the fuzzer on a machine with more memory

### Compilation Errors

**Issue**: Specific compilation errors occur frequently in the generated contracts.

**Possible Causes**:
- Issues with specific features or combinations of features
- Bugs in the Neo N3 compiler
- Invalid code generation in the feature generators

**Solutions**:
1. Analyze the logs to identify the specific compilation errors
2. Modify the feature generators to avoid generating code that triggers these errors
3. Filter out problematic features by modifying the `_featureGenerators` dictionary
4. Report the issues to the Neo N3 compiler team if they appear to be compiler bugs

### Execution Testing Failures

**Issue**: Contracts compile successfully but fail during execution testing.

**Possible Causes**:
- Issues with the execution environment
- Bugs in the Neo N3 virtual machine
- Invalid code generation that passes compilation but fails at runtime

**Solutions**:
1. Disable execution testing using the `--no-execution` option to focus on compilation:
   ```bash
   ./scripts/fuzzer/run-compiler-fuzzer.sh --no-execution
   ```
2. Analyze the logs to identify the specific execution errors
3. Modify the feature generators to avoid generating code that triggers these errors
4. Report the issues to the Neo N3 virtual machine team if they appear to be VM bugs

### Slow Performance

**Issue**: The fuzzer runs very slowly, taking a long time to generate and test contracts.

**Possible Causes**:
- Complex contracts with many features
- Inefficient code generation
- Resource-intensive compilation or execution testing
- Limited system resources

**Solutions**:
1. Reduce the number of features per contract using the `--features` option
2. Run the fuzzer on a more powerful machine
3. Modify the feature generators to produce simpler code fragments
4. Disable execution testing using the `--no-execution` option if it's not necessary

### Checkpointing Issues

**Issue**: Checkpoints are not being created or are incomplete during long-running sessions.

**Possible Causes**:
- Insufficient disk space
- Permission issues
- Bugs in the checkpointing system

**Solutions**:
1. Ensure there is sufficient disk space for checkpoints
2. Check file permissions for the output directory
3. Reduce the checkpoint interval to create smaller, more frequent checkpoints:
   ```bash
   ./scripts/fuzzer/run-compiler-fuzzer.sh --duration 24h --checkpoint-interval 15
   ```
4. Run the fuzzer with a higher log level to get more information:
   ```bash
   ./scripts/fuzzer/run-compiler-fuzzer.sh --log-level Debug
   ```

## Specific Error Messages

### "System.OutOfMemoryException: Array dimensions exceeded supported range."

**Issue**: The fuzzer crashes with this error when generating large arrays or collections.

**Solution**: Modify the feature generators to limit the size of arrays and collections:

```csharp
// Before
int[] array = new int[random.Next(1, 1000)];

// After
int[] array = new int[random.Next(1, 100)];
```

### "CS0246: The type or namespace name 'X' could not be found"

**Issue**: The fuzzer generates code that references types or namespaces that are not available in the Neo N3 framework.

**Solution**: Modify the feature generators to only use types and namespaces that are available in the Neo N3 framework:

```csharp
// Before
using System.Collections.Generic;
using System.Linq;

// After
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
```

### "CS0029: Cannot implicitly convert type 'X' to 'Y'"

**Issue**: The fuzzer generates code with incompatible type conversions.

**Solution**: Modify the feature generators to include explicit type conversions:

```csharp
// Before
long x = 42;
ulong y = x;

// After
long x = 42;
ulong y = (ulong)x;
```

### "CS0120: An object reference is required for the non-static field, method, or property 'X'"

**Issue**: The fuzzer generates code that tries to access instance members from a static context.

**Solution**: Modify the feature generators to ensure instance members are accessed correctly:

```csharp
// Before
public static void Main()
{
    int x = SomeInstanceMethod();
}

// After
public static void Main()
{
    MyClass instance = new MyClass();
    int x = instance.SomeInstanceMethod();
}
```

## Advanced Troubleshooting

### Debugging the Fuzzer

To debug the fuzzer, you can run it with the `--log-level Debug` option to get more detailed logs:

```bash
./scripts/fuzzer/run-compiler-fuzzer.sh --log-level Debug
```

You can also modify the source code to add more logging or debugging information:

```csharp
Logger.Debug($"Generating feature: {feature}");
Logger.Debug($"Generated code: {code}");
```

### Analyzing Compilation Errors

To analyze compilation errors in detail, you can modify the `CompileAndTestContract` method in the `DynamicContractFuzzer` class to log more information about the compilation process:

```csharp
private bool CompileAndTestContract(string contractPath)
{
    try
    {
        // Compile the contract
        var compilationEngine = GetCompilationEngine();
        var result = compilationEngine.Compile(contractPath);

        // Log compilation diagnostics
        foreach (var diagnostic in result.Diagnostics)
        {
            Logger.Debug($"Diagnostic: {diagnostic.Id} - {diagnostic.GetMessage()}");
        }

        // Check if compilation was successful
        if (result.Success)
        {
            // ...
        }
        else
        {
            // Log compilation errors
            foreach (var error in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            {
                Logger.Error($"Compilation error: {error.Id} - {error.GetMessage()}");
            }
        }

        // ...
    }
    catch (Exception ex)
    {
        // ...
    }
}
```

### Analyzing Feature Combinations

To analyze which combinations of features are problematic, you can modify the `GenerateAndTestSingleContract` method to log the feature combinations:

```csharp
public bool GenerateAndTestSingleContract(string contractName, int featuresPerContract)
{
    try
    {
        // Generate the contract
        string contractCode = GenerateDynamicContract(contractName, featuresPerContract);
        string filePath = Path.Combine(_outputDirectory, $"{contractName}.cs");

        // Extract the features used in this contract
        List<string> features = ExtractFeaturesFromContract(contractCode);
        _contractFeatures[contractName] = features;

        // Log the feature combination
        Logger.Debug($"Contract {contractName} features: {string.Join(", ", features)}");

        // ...
    }
    catch (Exception ex)
    {
        // ...
    }
}
```

## Getting Help

If you encounter issues that are not covered in this troubleshooting guide, you can:

1. Check the logs in the `Logs` directory for more information
2. Open an issue on the [Neo DevPack GitHub repository](https://github.com/neo-project/neo-devpack-dotnet)
3. Ask for help on the [Neo Discord server](https://discord.gg/neo)
4. Contact the Neo development team for assistance

## Conclusion

The Neo Compiler Fuzzer is a complex tool that may encounter various issues during operation. By understanding the common issues and their solutions, you can effectively troubleshoot and resolve problems that arise during fuzzing sessions.
