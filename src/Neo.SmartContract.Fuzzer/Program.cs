using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;           // Added
using Neo.VM.Types;               // Added
using Newtonsoft.Json; // Add this
using Neo.SmartContract.Manifest; // Added

namespace Neo.SmartContract.Fuzzer
{
    class Program
    {
        // --- Instance Fields for Options (Removed static) ---
        readonly Option<string> nefOption = new Option<string>("--nef", "Path to the NEF file") { IsRequired = true };
        readonly Option<string> manifestOption = new Option<string>("--manifest", "Path to the contract manifest file") { IsRequired = true };
        readonly Option<string> outputOption = new Option<string>("--output", () => "fuzzer-output", "Directory to save execution results and logs");
        readonly Option<int> iterationsOption = new Option<int>("--iterations", () => 1000, "Number of fuzzing iterations");
        readonly Option<long> gasLimitOption = new Option<long>("--gas-limit", () => 20_00000000L, "Gas limit per execution"); // Default 20 GAS
        readonly Option<int> seedOption = new Option<int>("--seed", () => Environment.TickCount, "Random seed for reproducibility");
        readonly Option<bool> coverageOption = new Option<bool>("--coverage", () => false, "Enable code coverage measurement");
        readonly Option<string> coverageFormatOption = new Option<string>("--coverage-format", () => "coz", "Coverage report format (e.g., coz, json)");
        readonly Option<string> methodsOption = new Option<string>("--methods", "Comma-separated list of methods to include in fuzzing (default: all public methods)");
        readonly Option<string> excludeOption = new Option<string>("--exclude", "Comma-separated list of methods to exclude from fuzzing");
        readonly Option<string> configOption = new Option<string>("--config", "Path to fuzzer configuration file");
        readonly Option<string?> replayOption = new Option<string?>("--replay", "Path to a specific input file to replay"); // Nullable string

        static async Task<int> Main(string[] args)
        {
            // Refactor: Create instance of Program
            var programInstance = new Program();

            var rootCommand = new RootCommand("Neo Smart Contract Fuzzer")
            {
                // Access options via instance
                programInstance.nefOption, programInstance.manifestOption, programInstance.outputOption, programInstance.iterationsOption,
                programInstance.gasLimitOption, programInstance.seedOption, programInstance.coverageOption, programInstance.coverageFormatOption,
                programInstance.methodsOption, programInstance.excludeOption, programInstance.configOption, programInstance.replayOption
            };

            rootCommand.Description = "A tool to perform fuzz testing on Neo smart contracts.";

            // Refactor: Call instance Execute method
            rootCommand.SetHandler((InvocationContext context) =>
            {
                programInstance.Execute(context);
            });

            return await rootCommand.InvokeAsync(args);
        }

        // Refactor: Remove static from Execute method
        private void Execute(InvocationContext context)
        {
            // Refactor: Retrieve values using instance options
            var nef = context.ParseResult.GetValueForOption(nefOption)!;
            var manifest = context.ParseResult.GetValueForOption(manifestOption)!;
            var output = context.ParseResult.GetValueForOption(outputOption)!;
            var iterations = context.ParseResult.GetValueForOption(iterationsOption);
            var gasLimit = context.ParseResult.GetValueForOption(gasLimitOption);
            var seed = context.ParseResult.GetValueForOption(seedOption);
            var coverage = context.ParseResult.GetValueForOption(coverageOption);
            var coverageFormat = context.ParseResult.GetValueForOption(coverageFormatOption)!;
            var methods = context.ParseResult.GetValueForOption(methodsOption)!;
            var exclude = context.ParseResult.GetValueForOption(excludeOption)!;
            var config = context.ParseResult.GetValueForOption(configOption)!;
            var replay = context.ParseResult.GetValueForOption(replayOption); // Nullable string

            try
            {
                // --- Replay Logic ---
                if (!string.IsNullOrEmpty(replay))
                {
                    Console.WriteLine($"--- Replay Mode: Executing input file {replay} ---");

                    if (!File.Exists(replay))
                    {
                        Console.WriteLine($"Error: Replay input file not found: {replay}");
                        return; // Or throw
                    }

                    // Load config (needed for ContractExecutor) - prioritize --config if provided
                    FuzzerConfiguration fuzzerConfig;
                    if (!string.IsNullOrEmpty(config) && File.Exists(config))
                    {
                        Console.WriteLine($"Loading configuration from {config} for replay context...");
                        fuzzerConfig = FuzzerConfiguration.LoadFromFile(config);
                        // Ensure NefPath and ManifestPath are set if they were overridden by CLI args originally
                        fuzzerConfig.NefPath = !string.IsNullOrEmpty(nef) ? nef : fuzzerConfig.NefPath;
                        fuzzerConfig.ManifestPath = !string.IsNullOrEmpty(manifest) ? manifest : fuzzerConfig.ManifestPath;
                    }
                    else // Use default/other CLI args if no config file specified
                    {
                        Console.WriteLine("Using default/command-line config for replay context...");
                        if (string.IsNullOrEmpty(nef) || string.IsNullOrEmpty(manifest))
                        {
                            Console.WriteLine("Error: --nef and --manifest options are required for replay mode if --config is not used.");
                            return;
                        }
                        fuzzerConfig = new FuzzerConfiguration
                        {
                            NefPath = nef,
                            ManifestPath = manifest,
                            GasLimit = gasLimit, // Use provided gas limit
                            OutputDirectory = output, // Use output dir if needed
                            PersistStateBetweenCalls = false // Replay should be isolated
                        };
                    }


                    // Load the contract manifest to find the method descriptor
                    if (!File.Exists(fuzzerConfig.NefPath) || !File.Exists(fuzzerConfig.ManifestPath))
                    {
                        Console.WriteLine($"Error: NEF ({fuzzerConfig.NefPath}) or Manifest ({fuzzerConfig.ManifestPath}) not found for replay.");
                        return;
                    }
                    byte[] nefBytes = File.ReadAllBytes(fuzzerConfig.NefPath);
                    string manifestJson = File.ReadAllText(fuzzerConfig.ManifestPath);
                    ContractManifest contractManifest = ContractManifest.Parse(manifestJson);

                    // Extract method name from replay file path
                    string methodName = ExtractMethodNameFromReplayPath(replay);
                    if (string.IsNullOrEmpty(methodName))
                    {
                        Console.WriteLine($"Error: Could not determine method name from replay path: {replay}");
                        return;
                    }
                    ContractMethodDescriptor? methodDescriptor = contractManifest.Abi.Methods.FirstOrDefault(m => m.Name == methodName);
                    if (methodDescriptor == null)
                    {
                        Console.WriteLine($"Error: Method '{methodName}' not found in contract manifest.");
                        return;
                    }

                    // Load input JSON and deserialize parameters using the method descriptor
                    StackItem[]? parameters = LoadAndDeserializeParameters(replay, methodDescriptor); // Pass descriptor
                    if (parameters == null)
                    {
                        // Error handled in LoadAndDeserializeParameters
                        return;
                    }

                    // Create executor and execute
                    // IMPORTANT: Creates a new executor ensures clean state for replay
                    var executor = new ContractExecutor(nefBytes, contractManifest, fuzzerConfig);

                    Console.WriteLine($"Replaying method: {methodName} with loaded parameters...");
                    var result = executor.ExecuteMethod(methodDescriptor, parameters, 0); // Use iteration 0 for replay

                    // Print detailed replay result
                    Console.WriteLine("--- Replay Result ---");
                    Console.WriteLine($"Success: {result.Success}");
                    Console.WriteLine($"VM State: {result.Engine?.State}");
                    Console.WriteLine($"Gas Consumed: {result.FeeConsumed}");
                    if (!result.Success)
                    {
                        Console.WriteLine($"Exception: {result.Exception}");
                    }
                    else
                    {
                        // Refactor: Separate conversion and serialization
                        if (result.ReturnValue != null)
                        {
                            StackItem returnValueStackItem = result.ReturnValue; // Assign after null check
                            var convertedValue = ContractExecutor.ConvertStackItemToJson(returnValueStackItem);
                            string returnValueJson = JsonConvert.SerializeObject(convertedValue);
                            Console.WriteLine($"Return Value: {returnValueJson}");
                        }
                        else
                        {
                            Console.WriteLine("Return Value: null");
                        }
                        if (result.Engine?.Notifications?.Count > 0)
                        {
                            Console.WriteLine("Notifications:");
                            foreach (var n in result.Engine.Notifications)
                            {
                                // Refactor: Separate cast, conversion, and serialization
                                StackItem stateStackItem = (StackItem)n.State; // Cast first
                                var convertedState = ContractExecutor.ConvertStackItemToJson(stateStackItem); // Convert
                                string stateJson = JsonConvert.SerializeObject(convertedState);
                                Console.WriteLine($"  {n.ScriptHash}: {n.EventName} = {stateJson}"); // Use temp var
                            }
                        }
                        if (result.CollectedLogs?.Count > 0)
                        {
                            Console.WriteLine("Logs:");
                            foreach (var log in result.CollectedLogs)
                                Console.WriteLine($"  Container: {log.ScriptHash}, Message: {log.Message}");
                        }
                    }
                    Console.WriteLine("--- Replay Complete ---");

                    return; // Exit after replay
                }

                // --- Normal Fuzzing Logic ---
                Console.WriteLine("--- Fuzzing Mode ---");
                // Create output directory if it doesn't exist
                Directory.CreateDirectory(output);

                // Load configuration from file or CLI options
                FuzzerConfiguration mainFuzzerConfig;
                if (!string.IsNullOrEmpty(config) && File.Exists(config))
                {
                    Console.WriteLine($"Loading configuration from {config}");
                    mainFuzzerConfig = FuzzerConfiguration.LoadFromFile(config);
                    // Apply CLI overrides if necessary (optional, depends on desired behavior)
                    mainFuzzerConfig.NefPath = !string.IsNullOrEmpty(nef) ? nef : mainFuzzerConfig.NefPath;
                    mainFuzzerConfig.ManifestPath = !string.IsNullOrEmpty(manifest) ? manifest : mainFuzzerConfig.ManifestPath;
                    mainFuzzerConfig.OutputDirectory = !string.IsNullOrEmpty(output) && output != "fuzzer-output" ? output : mainFuzzerConfig.OutputDirectory; // Check default
                    // Handle other overrides similarly if needed (Iterations, GasLimit, Seed, Coverage, etc.)
                }
                else
                {
                    // Create configuration from command line options
                    mainFuzzerConfig = new FuzzerConfiguration
                    {
                        NefPath = nef,
                        ManifestPath = manifest,
                        OutputDirectory = output,
                        IterationsPerMethod = iterations,
                        GasLimit = gasLimit,
                        Seed = seed,
                        EnableCoverage = coverage,
                        CoverageFormat = coverageFormat
                        // PersistStateBetweenCalls & SaveFailingInputsOnly will use their defaults unless set in a config file
                    };

                    // Parse methods to fuzz
                    if (!string.IsNullOrEmpty(methods))
                    {
                        foreach (var method in methods.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            mainFuzzerConfig.MethodsToFuzz.Add(method.Trim());
                        }
                    }

                    // Parse methods to exclude
                    if (!string.IsNullOrEmpty(exclude))
                    {
                        foreach (var method in exclude.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            mainFuzzerConfig.MethodsToExclude!.Add(method.Trim());
                        }
                    }
                }

                // Validate paths before starting fuzzer
                if (string.IsNullOrEmpty(mainFuzzerConfig.NefPath) || !File.Exists(mainFuzzerConfig.NefPath))
                {
                    Console.WriteLine($"Error: NEF file not found or not specified: {mainFuzzerConfig.NefPath}");
                    return;
                }
                if (string.IsNullOrEmpty(mainFuzzerConfig.ManifestPath) || !File.Exists(mainFuzzerConfig.ManifestPath))
                {
                    Console.WriteLine($"Error: Manifest file not found or not specified: {mainFuzzerConfig.ManifestPath}");
                    return;
                }

                // Create and run the fuzzer
                Console.WriteLine($"Fuzzing contract: {mainFuzzerConfig.NefPath}");
                Console.WriteLine($"Output directory: {mainFuzzerConfig.OutputDirectory}");
                Console.WriteLine($"Iterations per method: {mainFuzzerConfig.IterationsPerMethod}");
                Console.WriteLine($"Gas limit: {mainFuzzerConfig.GasLimit}");
                Console.WriteLine($"Seed: {mainFuzzerConfig.Seed}");
                Console.WriteLine($"Coverage enabled: {mainFuzzerConfig.EnableCoverage}");
                Console.WriteLine($"Persist State: {mainFuzzerConfig.PersistStateBetweenCalls}");
                Console.WriteLine($"Save Failing Only: {mainFuzzerConfig.SaveFailingInputsOnly}");

                // Create and run the main fuzzer implementation
                var fuzzer = new SmartContractFuzzer(mainFuzzerConfig);
                fuzzer.Run();

                Console.WriteLine("Fuzzing completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        // --- Helper methods for Replay ---

        private string ExtractMethodNameFromReplayPath(string replayPath)
        {
            try
            {
                // Assumes path like: .../Failures/MethodName/input_123.json
                // Or .../Results/MethodName/input_123.json
                var directoryName = Path.GetDirectoryName(replayPath);
                if (directoryName != null)
                {
                    return Path.GetFileName(directoryName);
                }
            }
            catch { }
            return string.Empty;
        }

        // Updated signature and implemented body
        private StackItem[]? LoadAndDeserializeParameters(string replayPath, ContractMethodDescriptor methodDescriptor)
        {
            try
            {
                string jsonContent = File.ReadAllText(replayPath);
                using JsonDocument document = JsonDocument.Parse(jsonContent);

                if (!document.RootElement.TryGetProperty("Parameters", out JsonElement parametersElement) || parametersElement.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine($"Error: Could not find 'Parameters' array in JSON file: {replayPath}");
                    return null;
                }

                var jsonParams = parametersElement.EnumerateArray().ToList();
                var methodParams = methodDescriptor.Parameters;

                // Explicitly store counts in int variables
                int jsonCount = jsonParams.Count;
                // Cast to resolve ambiguity with Linq Count() method, using correct type
                int methodCount = ((IReadOnlyList<ContractParameterDefinition>)methodParams).Count;

                if (jsonCount != methodCount)
                {
                    string errorMessage = $"Error: Parameter count mismatch in {replayPath}. JSON has {jsonCount}, Manifest expects {methodCount} for method {methodDescriptor.Name}.";
                    Console.WriteLine(errorMessage);
                    return null;
                }

                var stackItems = new StackItem[jsonCount]; // Use jsonCount here too
                for (int i = 0; i < jsonCount; i++)       // And here
                {
                    var jsonParam = jsonParams[i];
                    var expectedType = methodParams[i].Type;
                    try
                    {
                        // Call the now public static method in ContractExecutor
                        stackItems[i] = ContractExecutor.ConvertJsonElementToStackItem(jsonParam, expectedType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing parameter {i} ('{methodParams[i].Name}') for method {methodDescriptor.Name} from {replayPath}: {ex.Message}");
                        // Optionally log stack trace ex.ToString()
                        return null; // Fail fast on parameter deserialization error
                    }
                }
                Console.WriteLine($"Successfully loaded and deserialized {stackItems.Length} parameters from {replayPath}");
                return stackItems;
            }
            catch (Newtonsoft.Json.JsonException jsonEx)
            {
                Console.WriteLine($"Error parsing JSON file {replayPath}: {jsonEx.Message}");
                return null;
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Error reading file {replayPath}: {ioEx.Message}");
                return null;
            }
            catch (Exception ex) // Catch unexpected errors
            {
                Console.WriteLine($"An unexpected error occurred while loading parameters from {replayPath}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
    }
}
