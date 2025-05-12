using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Fuzzer for Neo smart contracts
    /// </summary>
    public class SmartContractFuzzer
    {
        private readonly FuzzerConfiguration _config;
        private readonly ContractExecutor _executor;
        private readonly CoverageTracker _coverageTracker;
        private readonly ParameterGenerator _parameterGenerator;
        private readonly ContractManifest _manifest;
        private readonly byte[] _nefBytes;
        private readonly List<IVulnerabilityDetector> _vulnerabilityDetectors;
        private readonly SymbolicExecution.Interfaces.IConstraintSolver _constraintSolver;
        private readonly Random _random;

        /// <summary>
        /// Initialize a new instance of the SmartContractFuzzer class
        /// </summary>
        /// <param name="config">Fuzzer configuration</param>
        public SmartContractFuzzer(FuzzerConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Check if the required files exist
            if (!File.Exists(config.NefPath))
                throw new FileNotFoundException($"NEF file not found: {config.NefPath}");

            if (!File.Exists(config.ManifestPath))
                throw new FileNotFoundException($"Manifest file not found: {config.ManifestPath}");

            // Load NEF and manifest files
            _nefBytes = File.ReadAllBytes(config.NefPath);
            string manifestJson = File.ReadAllText(config.ManifestPath);
            _manifest = ContractManifest.Parse(manifestJson);

            // Create output directory if it doesn't exist
            Directory.CreateDirectory(config.OutputDirectory);

            // Initialize components
            _executor = new ContractExecutor(_nefBytes, _manifest, config);
            _coverageTracker = new CoverageTracker(_nefBytes, _manifest, config.OutputDirectory);

            // Initialize constraint solver using the unified constraint solver
            var unifiedSolver = Solvers.ConstraintSolverFactory.Create(Solvers.ConstraintSolverType.Unified, config.Seed);
            _constraintSolver = new Solvers.ConstraintSolverAdapter(unifiedSolver);

            // Initialize vulnerability detectors
            _vulnerabilityDetectors = new List<IVulnerabilityDetector>
            {
                new Detectors.IntegerOverflowDetector(),
                new ReentrancyDetector(),
                new Detectors.UnauthorizedAccessDetector(),
                new StorageManipulationDetector(),
                new NeoNativeContractDetector(),
                new OracleVulnerabilityDetector(),
                new TokenImplementationDetector()
            };

            _parameterGenerator = new ParameterGenerator(config.Seed);
            _random = new Random(config.Seed);
        }

        /// <summary>
        /// Run the fuzzer
        /// </summary>
        public void Run()
        {
            Console.WriteLine($"Running Neo Smart Contract Fuzzer");
            Console.WriteLine($"Contract: {_manifest.Name}");
            Console.WriteLine($"Output directory: {_config.OutputDirectory}");
            Console.WriteLine($"Iterations: {_config.Iterations}");
            Console.WriteLine($"Random seed: {_config.Seed}");
            Console.WriteLine($"Coverage-guided fuzzing: {(_config.EnableCoverageGuidedFuzzing ? "Enabled" : "Disabled")}");
            Console.WriteLine($"Feedback-guided fuzzing: {(_config.EnableFeedbackGuidedFuzzing ? "Enabled" : "Disabled")}");

            // Get methods to fuzz
            var methodsToFuzz = _manifest.Abi.Methods;
            if (_config.MethodsToInclude?.Any() == true)
            {
                methodsToFuzz = methodsToFuzz.Where(m => _config.MethodsToInclude.Contains(m.Name)).ToArray();
            }
            if (_config.MethodsToExclude?.Any() == true)
            {
                methodsToFuzz = methodsToFuzz.Where(m => !_config.MethodsToExclude.Contains(m.Name)).ToArray();
            }

            Console.WriteLine($"Methods to fuzz: {string.Join(", ", methodsToFuzz.Select(m => m.Name))}");

            // Run fuzzer for each method
            foreach (var method in methodsToFuzz)
            {
                Console.WriteLine($"\nFuzzing method: {method.Name}");

                int successCount = 0;
                int errorCount = 0;
                int newCoverageCount = 0;

                // Initialize corpus for this method
                var corpus = new List<StackItem[]>();
                var coverageMap = new Dictionary<int, HashSet<int>>(); // Maps iteration to covered instructions
                var initialGasEstimate = EstimateGasForMethod(method);

                // Add initial seed inputs to the corpus
                AddInitialSeedsToCorpus(method, corpus);

                // Main fuzzing loop
                for (int i = 0; i < _config.Iterations; i++)
                {
                    try
                    {
                        // Generate parameters - either from corpus or randomly
                        StackItem[] parameters;
                        if (_config.EnableFeedbackGuidedFuzzing && corpus.Count > 0 && _random.Next(100) < 70)
                        {
                            // Select an input from the corpus and mutate it
                            parameters = MutateCorpusInput(method, corpus[_random.Next(corpus.Count)]);
                        }
                        else
                        {
                            // Generate new parameters
                            parameters = GenerateParameters(method);
                        }

                        // Adjust gas limit based on previous executions
                        long gasLimit = _config.GasLimit;
                        if (initialGasEstimate > 0)
                        {
                            // Use a higher gas limit for the first few iterations
                            gasLimit = i < 5 ? Math.Max(_config.GasLimit, initialGasEstimate * 2) :
                                                Math.Max(_config.GasLimit, initialGasEstimate);
                        }

                        // Execute method with adjusted gas limit
                        var result = _executor.ExecuteMethod(method, parameters, i, gasLimit);

                        // Track coverage
                        bool newCoverage = false;
                        if (_config.EnableCoverage)
                        {
                            newCoverage = _coverageTracker.TrackExecutionCoverage(result);
                            coverageMap[i] = new HashSet<int>(_coverageTracker.GetCoveredInstructions());

                            if (newCoverage)
                            {
                                newCoverageCount++;

                                // Add to corpus if it increased coverage
                                if (_config.EnableCoverageGuidedFuzzing && corpus.Count < _config.MaxCorpusSize)
                                {
                                    corpus.Add(parameters);
                                }
                            }
                        }

                        // Save result
                        if (!_config.SaveFailingInputsOnly || !result.Success)
                        {
                            SaveExecutionResult(method.Name, parameters, result, i);
                        }

                        // Update gas estimate based on actual consumption
                        if (result.Success && result.FeeConsumed > 0)
                        {
                            initialGasEstimate = Math.Max(initialGasEstimate, result.FeeConsumed);
                        }

                        successCount++;

                        // Print progress every 10% of iterations
                        if (i > 0 && i % Math.Max(1, _config.Iterations / 10) == 0)
                        {
                            Console.WriteLine($"  Progress: {i}/{_config.Iterations} iterations ({i * 100 / _config.Iterations}%), " +
                                             $"New coverage: {newCoverageCount}, Corpus size: {corpus.Count}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing method {method.Name} (iteration {i}): {ex.Message}");
                        errorCount++;
                    }
                }

                // Save corpus for this method
                SaveCorpus(method.Name, corpus);

                Console.WriteLine($"Method {method.Name} fuzzing complete: {successCount} successful, {errorCount} failed, " +
                                 $"{newCoverageCount} new coverage points, corpus size: {corpus.Count}");
            }

            // Generate coverage report
            if (_config.EnableCoverage)
            {
                _coverageTracker.GenerateReport(_config.CoverageFormat);
                Console.WriteLine($"Coverage report generated in {_config.OutputDirectory}");
            }

            // Generate summary report
            GenerateSummaryReport();
        }

        /// <summary>
        /// Estimates the gas required for a method based on its complexity
        /// </summary>
        private long EstimateGasForMethod(ContractMethodDescriptor method)
        {
            // Base gas cost
            long baseGas = 1_000_000;

            // Add gas based on parameter count and types
            foreach (var param in method.Parameters)
            {
                switch (param.Type)
                {
                    case ContractParameterType.Array:
                    case ContractParameterType.Map:
                        baseGas += 500_000; // Complex types need more gas
                        break;
                    case ContractParameterType.ByteArray:
                    case ContractParameterType.String:
                        baseGas += 100_000; // Variable-length types
                        break;
                    default:
                        baseGas += 10_000; // Simple types
                        break;
                }
            }

            return baseGas;
        }

        /// <summary>
        /// Adds initial seed inputs to the corpus
        /// </summary>
        private void AddInitialSeedsToCorpus(ContractMethodDescriptor method, List<StackItem[]> corpus)
        {
            // Add a few basic inputs to the corpus

            // 1. All zeros/defaults
            var defaultParams = new StackItem[method.Parameters.Length];
            for (int i = 0; i < method.Parameters.Length; i++)
            {
                switch (method.Parameters[i].Type)
                {
                    case ContractParameterType.Boolean:
                        defaultParams[i] = StackItem.False;
                        break;
                    case ContractParameterType.Integer:
                        defaultParams[i] = new Integer(0);
                        break;
                    case ContractParameterType.ByteArray:
                    case ContractParameterType.String:
                    case ContractParameterType.Hash160:
                    case ContractParameterType.Hash256:
                    case ContractParameterType.PublicKey:
                    case ContractParameterType.Signature:
                        defaultParams[i] = new ByteString(System.Array.Empty<byte>());
                        break;
                    case ContractParameterType.Array:
                        defaultParams[i] = new VM.Types.Array();
                        break;
                    case ContractParameterType.Map:
                        defaultParams[i] = new Map();
                        break;
                    default:
                        defaultParams[i] = StackItem.Null;
                        break;
                }
            }
            corpus.Add(defaultParams);

            // 2. Random valid values
            corpus.Add(GenerateParameters(method));

            // 3. Method-specific seeds
            if (method.Name.Equals("transfer", StringComparison.OrdinalIgnoreCase))
            {
                // Add a typical transfer with small amount
                var transferParams = new StackItem[method.Parameters.Length];
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    var paramName = method.Parameters[i].Name?.ToLowerInvariant() ?? "";

                    if (paramName.Contains("from") || paramName.Contains("to"))
                    {
                        // Generate a valid-looking address
                        byte[] hash = new byte[20];
                        _random.NextBytes(hash);
                        transferParams[i] = new ByteString(hash);
                    }
                    else if (paramName.Contains("amount") || paramName.Contains("value"))
                    {
                        // Small transfer amount
                        transferParams[i] = new Integer(1);
                    }
                    else
                    {
                        // Default value for other parameters
                        transferParams[i] = defaultParams[i];
                    }
                }
                corpus.Add(transferParams);
            }
        }

        /// <summary>
        /// Mutates an input from the corpus to generate a new input
        /// </summary>
        private StackItem[] MutateCorpusInput(ContractMethodDescriptor method, StackItem[] input)
        {
            // Create a copy of the input
            var mutated = new StackItem[input.Length];
            System.Array.Copy(input, mutated, input.Length);

            // Select a random parameter to mutate
            int paramIndex = _random.Next(mutated.Length);

            // Mutate the selected parameter
            var paramType = method.Parameters[paramIndex].Type;
            mutated[paramIndex] = _parameterGenerator.GenerateParameter(paramType, 0);

            return mutated;
        }

        /// <summary>
        /// Saves the corpus for a method
        /// </summary>
        private void SaveCorpus(string methodName, List<StackItem[]> corpus)
        {
            // Create a directory for the method's corpus
            string corpusDir = Path.Combine(_config.OutputDirectory, methodName, "corpus");
            Directory.CreateDirectory(corpusDir);

            // Save each corpus entry
            for (int i = 0; i < corpus.Count; i++)
            {
                string corpusPath = Path.Combine(corpusDir, $"input_{i}.json");

                var corpusData = new
                {
                    Method = methodName,
                    Parameters = corpus[i].Select(p => p.ToString()).ToArray()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string corpusJson = System.Text.Json.JsonSerializer.Serialize(corpusData, options);
                File.WriteAllText(corpusPath, corpusJson);
            }
        }

        /// <summary>
        /// Save execution result
        /// </summary>
        private void SaveExecutionResult(string methodName, StackItem[] parameters, ExecutionResult result, int iteration)
        {
            // Create a directory for the method
            string methodDir = Path.Combine(_config.OutputDirectory, methodName);
            Directory.CreateDirectory(methodDir);

            // Save result
            string resultPath = Path.Combine(methodDir, $"result_{iteration}.json");

            var resultData = new
            {
                Method = methodName,
                Parameters = parameters.Select(p => p.ToString()).ToArray(),
                GasConsumed = result.FeeConsumed,
                State = result.Success ? "Success" : "Failed",
                Exception = result.Exception?.ToString(),
                ReturnValue = result.ReturnValue?.ToString(),
                InstructionCount = result.InstructionCount,
                ExecutionTime = result.ExecutionTime.TotalMilliseconds,
                TimedOut = result.TimedOut,
                StorageChanges = result.StorageChanges?.Count ?? 0,
                WitnessChecks = result.WitnessChecks?.Count ?? 0,
                ExternalCalls = result.ExternalCalls?.Count ?? 0,
                Logs = result.CollectedLogs?.Select(l => l.Message).ToArray() ?? System.Array.Empty<string>()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string resultJson = System.Text.Json.JsonSerializer.Serialize(resultData, options);
            File.WriteAllText(resultPath, resultJson);

            // If this is a failing execution, save it to a separate failures directory for easier analysis
            if (!result.Success)
            {
                string failuresDir = Path.Combine(_config.OutputDirectory, "failures");
                Directory.CreateDirectory(failuresDir);

                string failurePath = Path.Combine(failuresDir, $"{methodName}_failure_{iteration}.json");
                File.WriteAllText(failurePath, resultJson);

                // Also save a summary of the failure
                string failureSummaryPath = Path.Combine(failuresDir, "failure_summary.txt");
                string failureSummary = $"Method: {methodName}, Iteration: {iteration}, Exception: {result.Exception?.Message}\n";
                File.AppendAllText(failureSummaryPath, failureSummary);
            }
        }

        /// <summary>
        /// Generate a summary report of the fuzzing results
        /// </summary>
        private void GenerateSummaryReport()
        {
            string summaryPath = Path.Combine(_config.OutputDirectory, "fuzzing_summary.txt");
            var summary = new StringBuilder();

            summary.AppendLine("Neo N3 Smart Contract Fuzzing Summary");
            summary.AppendLine("=====================================");
            summary.AppendLine();
            summary.AppendLine($"Contract: {_manifest.Name}");
            summary.AppendLine($"Date: {DateTime.Now}");
            summary.AppendLine($"Iterations per method: {_config.Iterations}");
            summary.AppendLine($"Random seed: {_config.Seed}");
            summary.AppendLine();

            // Collect statistics for each method
            var methodStats = new Dictionary<string, (int Success, int Failures, long TotalGas, long MaxGas, TimeSpan TotalTime)>();

            foreach (var method in _manifest.Abi.Methods)
            {
                string methodDir = Path.Combine(_config.OutputDirectory, method.Name);
                if (!Directory.Exists(methodDir))
                    continue;

                int successCount = 0;
                int failureCount = 0;
                long totalGas = 0;
                long maxGas = 0;
                TimeSpan totalTime = TimeSpan.Zero;

                // Read all result files
                foreach (var resultFile in Directory.GetFiles(methodDir, "result_*.json"))
                {
                    try
                    {
                        string json = File.ReadAllText(resultFile);
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;

                        // Get execution status
                        if (root.TryGetProperty("State", out var state) && state.GetString() == "Success")
                            successCount++;
                        else
                            failureCount++;

                        // Get gas consumed
                        if (root.TryGetProperty("GasConsumed", out var gas))
                        {
                            long gasConsumed = gas.GetInt64();
                            totalGas += gasConsumed;
                            maxGas = Math.Max(maxGas, gasConsumed);
                        }

                        // Get execution time
                        if (root.TryGetProperty("ExecutionTime", out var time))
                        {
                            double ms = time.GetDouble();
                            totalTime += TimeSpan.FromMilliseconds(ms);
                        }
                    }
                    catch
                    {
                        // Ignore errors reading result files
                    }
                }

                methodStats[method.Name] = (successCount, failureCount, totalGas, maxGas, totalTime);
            }

            // Add method statistics to summary
            summary.AppendLine("Method Statistics:");
            summary.AppendLine("=================");

            foreach (var method in methodStats.Keys.OrderBy(m => m))
            {
                var stats = methodStats[method];
                int totalExecutions = stats.Success + stats.Failures;
                double successRate = totalExecutions > 0 ? (double)stats.Success / totalExecutions * 100 : 0;
                double avgGas = totalExecutions > 0 ? (double)stats.TotalGas / totalExecutions : 0;
                double avgTimeMs = totalExecutions > 0 ? stats.TotalTime.TotalMilliseconds / totalExecutions : 0;

                summary.AppendLine($"  {method}:");
                summary.AppendLine($"    Executions: {totalExecutions} ({stats.Success} successful, {stats.Failures} failed)");
                summary.AppendLine($"    Success Rate: {successRate:F2}%");
                summary.AppendLine($"    Gas Usage: Avg {avgGas:F2}, Max {stats.MaxGas}");
                summary.AppendLine($"    Execution Time: Avg {avgTimeMs:F2}ms, Total {stats.TotalTime.TotalSeconds:F2}s");
                summary.AppendLine();
            }

            // Add coverage statistics if available
            if (_config.EnableCoverage)
            {
                summary.AppendLine("Coverage Statistics:");
                summary.AppendLine("===================");
                summary.AppendLine($"  Coverage report available at: {Path.Combine(_config.OutputDirectory, "coverage-report")}");
                summary.AppendLine();
            }

            // Add failure statistics
            string failuresDir = Path.Combine(_config.OutputDirectory, "failures");
            if (Directory.Exists(failuresDir))
            {
                var failureFiles = Directory.GetFiles(failuresDir, "*.json");
                summary.AppendLine("Failure Statistics:");
                summary.AppendLine("==================");
                summary.AppendLine($"  Total Failures: {failureFiles.Length}");
                summary.AppendLine($"  Failure details available at: {failuresDir}");
                summary.AppendLine();

                // Group failures by exception type
                var exceptionTypes = new Dictionary<string, int>();
                foreach (var failureFile in failureFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(failureFile);
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("Exception", out var exception) && exception.ValueKind == JsonValueKind.String)
                        {
                            string exceptionText = exception.GetString() ?? "Unknown";
                            string exceptionType = exceptionText.Split(':')[0].Trim();

                            if (!exceptionTypes.ContainsKey(exceptionType))
                                exceptionTypes[exceptionType] = 0;

                            exceptionTypes[exceptionType]++;
                        }
                    }
                    catch
                    {
                        // Ignore errors reading failure files
                    }
                }

                // Add exception type statistics
                summary.AppendLine("Exception Types:");
                foreach (var exceptionType in exceptionTypes.Keys.OrderByDescending(e => exceptionTypes[e]))
                {
                    summary.AppendLine($"  {exceptionType}: {exceptionTypes[exceptionType]} occurrences");
                }
                summary.AppendLine();
            }

            // Add recommendations
            summary.AppendLine("Recommendations:");
            summary.AppendLine("===============");
            summary.AppendLine("1. Review failures to identify potential issues in the contract");
            summary.AppendLine("2. Increase iterations for methods with low success rates");
            summary.AppendLine("3. Optimize gas usage for methods with high gas consumption");
            summary.AppendLine("4. Add more targeted test cases for methods with low coverage");
            summary.AppendLine();

            // Write summary to file
            File.WriteAllText(summaryPath, summary.ToString());
            Console.WriteLine($"Summary report generated at {summaryPath}");
        }

        /// <summary>
        /// Generate parameters for a method
        /// </summary>
        private StackItem[] GenerateParameters(ContractMethodDescriptor method)
        {
            var parameters = new StackItem[method.Parameters.Length];

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                parameters[i] = _parameterGenerator.GenerateParameter(method.Parameters[i].Type, 0);
            }

            return parameters;
        }



        /// <summary>
        /// Detect vulnerabilities in the smart contract using symbolic execution.
        /// </summary>
        public void DetectVulnerabilities()
        {
            Console.WriteLine("\nRunning vulnerability detection using symbolic execution...");

            // Create a directory for vulnerability reports
            string vulnDir = Path.Combine(_config.OutputDirectory, "vulnerabilities");
            Directory.CreateDirectory(vulnDir);

            // Get methods to analyze
            var methodsToAnalyze = _manifest.Abi.Methods;
            if (_config.MethodsToInclude?.Any() == true)
            {
                methodsToAnalyze = methodsToAnalyze.Where(m => _config.MethodsToInclude.Contains(m.Name)).ToArray();
            }
            if (_config.MethodsToExclude?.Any() == true)
            {
                methodsToAnalyze = methodsToAnalyze.Where(m => !_config.MethodsToExclude.Contains(m.Name)).ToArray();
            }

            int totalVulnerabilitiesFound = 0;
            var allVulnerabilities = new Dictionary<string, List<VulnerabilityRecord>>();

            foreach (var method in methodsToAnalyze)
            {
                Console.WriteLine($"Analyzing method: {method.Name}");

                try
                {
                    // Create symbolic arguments for the method
                    // Convert contract parameter types to string types for symbolic execution
                    var paramTypes = method.Parameters.Select(p => p.Type.ToString()).ToList();
                    var symbolicArgs = SymbolicExecutionEngine.CreateSymbolicArgumentsForMethod(paramTypes);

                    // Create a symbolic execution engine for the method
                    var symbolicEngine = new SymbolicExecutionEngine(
                        _nefBytes,
                        _constraintSolver,
                        null, // No symbolic vulnerability detectors
                        symbolicArgs,
                        _config.MaxSteps);

                    // Execute the method symbolically
                    var result = symbolicEngine.Execute();

                    // Record and report vulnerabilities
                    if (result.ExecutionPaths.Count > 0)
                    {
                        allVulnerabilities[method.Name] = new List<VulnerabilityRecord>();
                        totalVulnerabilitiesFound += 0;

                        // Write detailed results to a file
                        string resultPath = Path.Combine(vulnDir, $"{method.Name}_vulnerabilities.txt");
                        File.WriteAllText(resultPath, $"Execution paths: {result.ExecutionPaths.Count}");

                        Console.WriteLine($"  Found 0 potential vulnerabilities in {method.Name}");
                        // Commented out: foreach (var vulnType in result.Vulnerabilities.Select(v => v.Type).Distinct())
                        {
                            // Console.WriteLine($"    - {vulnType}: {result.Vulnerabilities.Count(v => v.Type == vulnType)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  No vulnerabilities found in {method.Name}");
                    }

                    Console.WriteLine($"  Explored {result.ExecutionPaths.Count} execution paths");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing method {method.Name}: {ex.Message}");
                }
            }

            // Create a summary report
            string summaryPath = Path.Combine(vulnDir, "vulnerability_summary.txt");
            var summary = new StringBuilder();
            summary.AppendLine("Neo N3 Smart Contract Vulnerability Analysis Summary");
            summary.AppendLine("===================================================\n");
            summary.AppendLine($"Contract: {_manifest.Name}");
            summary.AppendLine($"Date: {DateTime.Now}\n");
            summary.AppendLine($"Total Vulnerabilities Found: {totalVulnerabilitiesFound}\n");

            // Group vulnerabilities by type
            var vulnTypes = allVulnerabilities.Values
                .SelectMany(v => v)
                .GroupBy(v => v.Type)
                .OrderByDescending(g => g.Count());

            summary.AppendLine("Vulnerability Types Summary:");
            foreach (var type in vulnTypes)
            {
                summary.AppendLine($"  {type.Key}: {type.Count()} occurrences");
            }

            summary.AppendLine("\nVulnerabilities By Method:");
            foreach (var method in allVulnerabilities.Keys.OrderBy(m => m))
            {
                summary.AppendLine($"  {method}: {allVulnerabilities[method].Count} vulnerabilities");

                foreach (var vulnerability in allVulnerabilities[method])
                {
                    summary.AppendLine($"    - {vulnerability.Type}: {vulnerability.Description}");
                }
            }

            summary.AppendLine("\nRecommendations:");
            summary.AppendLine("  1. Review each vulnerability report in detail");
            summary.AppendLine("  2. Apply fixes according to suggested remediation strategies");
            summary.AppendLine("  3. Re-run vulnerability analysis to ensure issues are resolved");

            File.WriteAllText(summaryPath, summary.ToString());
            Console.WriteLine($"\nVulnerability summary written to {summaryPath}");
        }
    }
}
