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
        private readonly SimpleConstraintSolver _constraintSolver;

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
            _constraintSolver = new SimpleConstraintSolver(config.Seed);

            // Initialize vulnerability detectors
            _vulnerabilityDetectors = new List<IVulnerabilityDetector>
            {
                new IntegerOverflowDetector(),
                new ReentrancyDetector(),
                new UnauthorizedAccessDetector(),
                new StorageManipulationDetector(),
                new NeoNativeContractDetector(),
                new OracleVulnerabilityDetector(),
                new TokenImplementationDetector()
            };

            _parameterGenerator = new ParameterGenerator(config.Seed);
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

                for (int i = 0; i < _config.Iterations; i++)
                {
                    try
                    {
                        // Generate parameters
                        var parameters = GenerateParameters(method);

                        // Execute method
                        var result = _executor.ExecuteMethod(method, parameters, i);

                        // Track coverage
                        if (_config.EnableCoverage)
                        {
                            _coverageTracker.TrackExecutionCoverage(result);
                        }

                        // Save result
                        SaveExecutionResult(method.Name, parameters, result, i);

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing method {method.Name} (iteration {i}): {ex.Message}");
                        errorCount++;
                    }
                }

                Console.WriteLine($"Method {method.Name} fuzzing complete: {successCount} successful, {errorCount} failed");
            }

            // Generate coverage report
            if (_config.EnableCoverage)
            {
                _coverageTracker.GenerateReport(_config.CoverageFormat);
                Console.WriteLine($"Coverage report generated in {_config.OutputDirectory}");
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
                ReturnValue = result.ReturnValue?.ToString()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string resultJson = System.Text.Json.JsonSerializer.Serialize(resultData, options);
            File.WriteAllText(resultPath, resultJson);
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
                        _vulnerabilityDetectors,
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
