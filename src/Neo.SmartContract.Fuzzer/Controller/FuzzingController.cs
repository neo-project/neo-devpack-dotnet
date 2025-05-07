using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Fuzzer.InputGeneration;
using Neo.SmartContract.Fuzzer.Logging;
using Neo.SmartContract.Fuzzer.Minimization;
using Neo.SmartContract.Fuzzer.Reporting;
using Neo.SmartContract.Fuzzer.StaticAnalysis;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Fuzzer.Controller
{
    /// <summary>
    /// Controls the fuzzing process.
    /// </summary>
    public class FuzzingController
    {
        private readonly FuzzerConfiguration _config;
        private readonly ContractManifest _manifest;
        private readonly byte[] _nefBytes;
        private readonly InputGenerator _inputGenerator;
        private readonly ContractExecutor _executor;
        private readonly FeedbackAggregator _feedbackAggregator;
        private readonly CoverageTracker _coverageTracker;
        private readonly CoverageFeedback _coverageFeedback;
        private readonly List<IStaticAnalyzer> _staticAnalyzers = new List<IStaticAnalyzer>();
        private readonly List<ExecutionVulnerabilityDetector> _vulnerabilityDetectors = new List<ExecutionVulnerabilityDetector>();
        private readonly ReportGenerator _reportGenerator;
        private readonly SymbolicExecution.SymbolicExecutionIntegrator? _symbolicExecutionIntegrator;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private FuzzingStatus _status = new FuzzingStatus();
        private Task? _fuzzingTask;
        private readonly List<IssueReport> _issues = new List<IssueReport>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzingController"/> class.
        /// </summary>
        /// <param name="config">The fuzzer configuration.</param>
        public FuzzingController(FuzzerConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Load NEF and manifest
            _nefBytes = File.ReadAllBytes(config.NefPath);
            string manifestJson = File.ReadAllText(config.ManifestPath);
            _manifest = ContractManifest.Parse(manifestJson);

            // Create output directory if it doesn't exist
            Directory.CreateDirectory(config.OutputDirectory);
            string corpusDirectory = Path.Combine(config.OutputDirectory, "corpus");
            Directory.CreateDirectory(corpusDirectory);

            // Initialize components
            _feedbackAggregator = new FeedbackAggregator(config.Seed);
            _executor = new ContractExecutor(_nefBytes, _manifest, config);

            // Initialize coverage tracking
            _coverageTracker = new CoverageTracker(_nefBytes, _manifest, config.OutputDirectory);
            _coverageFeedback = new CoverageFeedback(_coverageTracker, config.Seed);

            // Configure coverage feedback based on configuration
            if (!config.EnableCoverageGuidedFuzzing)
            {
                Logger.Info("Coverage-guided fuzzing is disabled in configuration");
            }
            else
            {
                Logger.Info("Coverage-guided fuzzing is enabled");
                if (config.PrioritizeBranchCoverage)
                {
                    Logger.Info("Branch coverage prioritization is enabled");
                }
                if (config.PrioritizePathCoverage)
                {
                    Logger.Info("Path coverage prioritization is enabled");
                }
            }

            _inputGenerator = new InputGenerator(
                _manifest,
                _feedbackAggregator,
                config.Seed,
                corpusDirectory,
                config.MaxCorpusSize);

            // Initialize report generator
            _reportGenerator = new ReportGenerator(
                config.OutputDirectory,
                config.ReportFormats,
                config);

            // Initialize static analyzers
            if (config.EnableStaticAnalysis)
            {
                // Add NEF analyzer
                _staticAnalyzers.Add(new NefStaticAnalyzer(_nefBytes, _manifest));

                // Add C# analyzer if source path is provided
                if (!string.IsNullOrEmpty(config.SourcePath) && File.Exists(config.SourcePath))
                {
                    _staticAnalyzers.Add(new CSharpStaticAnalyzer(config.SourcePath));
                }
            }

            // Initialize vulnerability detectors
            _vulnerabilityDetectors.Add(new CrashDetector());
            _vulnerabilityDetectors.Add(new GasConsumptionDetector(config.GasLimit / 2)); // Alert if gas usage is more than half the limit
            _vulnerabilityDetectors.Add(new StorageVulnerabilityDetector());
            _vulnerabilityDetectors.Add(new IntegerVulnerabilityDetector());

            // Add our new vulnerability detectors
            _vulnerabilityDetectors.Add(new TimeoutDetector(10000, 1000)); // 10,000 instructions or 1 second
            _vulnerabilityDetectors.Add(new StateChangeAfterExternalCallDetector());
            _vulnerabilityDetectors.Add(new AccessControlDetector());

            // Initialize symbolic execution integrator if enabled
            if (config.EnableSymbolicExecution)
            {
                _symbolicExecutionIntegrator = new SymbolicExecution.SymbolicExecutionIntegrator(
                    config,
                    _manifest,
                    _nefBytes,
                    _feedbackAggregator);
            }
        }

        /// <summary>
        /// Starts the fuzzing process.
        /// </summary>
        public void Start()
        {
            if (_fuzzingTask != null && !_fuzzingTask.IsCompleted)
            {
                throw new InvalidOperationException("Fuzzing is already running");
            }

            _cancellationTokenSource.TryReset();
            _stopwatch.Restart();
            _status = new FuzzingStatus();

            _fuzzingTask = Task.Run(() => FuzzingLoopAsync(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stops the fuzzing process.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _stopwatch.Stop();
        }

        /// <summary>
        /// Gets the current status of the fuzzing process.
        /// </summary>
        /// <returns>The current fuzzing status.</returns>
        public FuzzingStatus GetStatus()
        {
            _status.ElapsedTime = _stopwatch.Elapsed;
            return _status;
        }

        /// <summary>
        /// Waits for the fuzzing process to complete.
        /// </summary>
        /// <returns>A task that completes when the fuzzing process completes.</returns>
        public Task WaitForCompletionAsync()
        {
            return _fuzzingTask ?? Task.CompletedTask;
        }

        /// <summary>
        /// Gets an awaiter for the fuzzing process.
        /// </summary>
        /// <returns>An awaiter for the fuzzing process.</returns>
        public System.Runtime.CompilerServices.TaskAwaiter GetAwaiter()
        {
            return WaitForCompletionAsync().GetAwaiter();
        }

        private async Task FuzzingLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Run static analysis
                if (_config.EnableStaticAnalysis)
                {
                    await RunStaticAnalysisAsync();
                }

                // Get methods to fuzz
                var methodsToFuzz = GetMethodsToFuzz();
                _status.TotalMethods = methodsToFuzz.Length;

                // Run symbolic execution if enabled
                if (_config.EnableSymbolicExecution && _symbolicExecutionIntegrator != null)
                {
                    await RunSymbolicExecutionAsync(methodsToFuzz);
                }

                Logger.Info($"Starting fuzzing for {_manifest.Name}");
                Logger.Info($"Methods to fuzz: {string.Join(", ", methodsToFuzz.Select(m => m.Name))}");

                // Main fuzzing loop
                for (int i = 0; i < _config.IterationsPerMethod; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // Determine which methods to prioritize based on coverage
                    List<ContractMethodDescriptor> prioritizedMethods = PrioritizeMethodsBasedOnCoverage(methodsToFuzz, i);

                    foreach (var method in prioritizedMethods)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        try
                        {
                            // Generate test case
                            var testCase = _inputGenerator.GenerateTestCase(method.Name);

                            // Execute test case
                            var result = _executor.ExecuteMethod(method, testCase.Parameters, i);

                            // Track coverage if enabled
                            if (_config.EnableCoverage)
                            {
                                _coverageTracker.TrackExecutionCoverage(result);
                            }

                            // Process result for general feedback
                            bool isInteresting = _feedbackAggregator.AddExecutionFeedback(testCase, result);

                            // Process result for coverage feedback if enabled
                            if (_config.EnableCoverageGuidedFuzzing)
                            {
                                var coverageFeedback = _coverageFeedback.ProcessExecutionResult(testCase, result);
                                if (coverageFeedback != null)
                                {
                                    isInteresting = true;
                                    Logger.Verbose($"New coverage: {coverageFeedback.Description}");

                                    // Enhanced energy adjustment based on coverage type
                                    if (coverageFeedback.Description.Contains("branch") && _config.PrioritizeBranchCoverage)
                                    {
                                        testCase.Energy += 0.8; // Increased from 0.5 to 0.8 for branch coverage

                                        // Log more detailed information about the branch coverage
                                        Logger.Info($"New branch coverage in method {method.Name}: {coverageFeedback.Description}");
                                    }
                                    else if (coverageFeedback.Description.Contains("path") && _config.PrioritizePathCoverage)
                                    {
                                        testCase.Energy += 0.6; // Increased from 0.3 to 0.6 for path coverage

                                        // Log more detailed information about the path coverage
                                        Logger.Info($"New execution path in method {method.Name}");
                                    }
                                    else if (coverageFeedback.Description.Contains("instruction"))
                                    {
                                        testCase.Energy += 0.4; // Increased from 0.1 to 0.4 for instruction coverage

                                        // Log more detailed information about the instruction coverage
                                        Logger.Info($"New instruction coverage in method {method.Name}");
                                    }
                                    else
                                    {
                                        testCase.Energy += 0.2; // Increased from 0.1 to 0.2 for other coverage types
                                    }

                                    // Additional energy boost for methods with low coverage
                                    if (_coverageFeedback.GetLowCoverageMethods().Contains(method.Name))
                                    {
                                        testCase.Energy += 0.3; // Extra boost for methods with low coverage
                                        Logger.Info($"Boosting energy for low-coverage method: {method.Name}");
                                    }
                                }
                            }

                            // Add to corpus if interesting
                            _inputGenerator.AddToCorpus(testCase, isInteresting);

                            // Update status
                            _status.TotalExecutions++;
                            if (result.Success)
                            {
                                _status.SuccessfulExecutions++;
                            }
                            else
                            {
                                _status.FailedExecutions++;
                            }

                            // Update method statistics in the input generator
                            _inputGenerator.UpdateMethodStatistics(method.Name, result.Success);

                            // If the execution was successful, add the parameters as known good values
                            if (result.Success)
                            {
                                for (int j = 0; j < testCase.Parameters.Length; j++)
                                {
                                    if (j < method.Parameters.Length)
                                    {
                                        _inputGenerator.AddKnownGoodValue(
                                            method.Parameters[j].Type.ToString(),
                                            testCase.Parameters[j]
                                        );
                                    }
                                }
                            }

                            // Detect vulnerabilities
                            var newIssues = DetectVulnerabilities(testCase, result, method);
                            if (newIssues.Count > 0)
                            {
                                _status.IssuesFound += newIssues.Count;
                                _issues.AddRange(newIssues);

                                // Increase energy for test cases that find issues
                                testCase.Energy += 1.0; // Significant boost for finding issues
                                Logger.Info($"Found {newIssues.Count} issues in method {method.Name}");

                                // Minimize test cases if enabled
                                if (_config.EnableTestCaseMinimization)
                                {
                                    MinimizeTestCases(newIssues, method);
                                }

                                // Save the issue-triggering test case to corpus regardless of other criteria
                                _inputGenerator.AddToCorpus(testCase, true);
                            }

                            // Save result
                            SaveExecutionResult(method.Name, testCase, result, i);

                            // Periodically log coverage progress
                            if (i % 100 == 0 && _config.EnableCoverage)
                            {
                                var coverageStats = _coverageFeedback.GetCoverageStatistics();
                                Logger.Info($"Current coverage after {i} iterations:");
                                Logger.Info($"Method coverage: {_status.CodeCoverage:P2}");

                                if (coverageStats.ContainsKey("BranchCoveragePercent"))
                                {
                                    double branchCoverage = (double)coverageStats["BranchCoveragePercent"] / 100.0;
                                    Logger.Info($"Branch coverage: {branchCoverage:P2}");
                                }

                                if (coverageStats.ContainsKey("InstructionCoveragePercent"))
                                {
                                    double instructionCoverage = (double)coverageStats["InstructionCoveragePercent"] / 100.0;
                                    Logger.Info($"Instruction coverage: {instructionCoverage:P2}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Error executing method {method.Name}: {ex.Message}");
                            Logger.Verbose(ex.StackTrace);
                            _status.Errors++;
                        }
                    }

                    // Update coverage statistics if coverage is enabled
                    if (_config.EnableCoverage)
                    {
                        var coverageStats = _coverageFeedback.GetCoverageStatistics();

                        // Calculate method coverage
                        _status.CodeCoverage = coverageStats.ContainsKey("MethodsCovered")
                            ? (double)(int)coverageStats["MethodsCovered"] / _status.TotalMethods
                            : 0;

                        // Update branch coverage in status
                        if (coverageStats.ContainsKey("BranchCoveragePercent"))
                        {
                            _status.BranchCoverage = (double)coverageStats["BranchCoveragePercent"] / 100.0;
                        }

                        // Update instruction coverage in status
                        if (coverageStats.ContainsKey("InstructionsCovered") && coverageStats.ContainsKey("TotalInstructions"))
                        {
                            int covered = (int)coverageStats["InstructionsCovered"];
                            int total = (int)coverageStats["TotalInstructions"];
                            _status.InstructionCoverage = total > 0 ? (double)covered / total : 0;
                        }

                        // Generate coverage report periodically
                        if (i % 1000 == 0 || i == _config.IterationsPerMethod - 1)
                        {
                            try
                            {
                                _coverageTracker.GenerateReport();
                            }
                            catch (Exception ex)
                            {
                                // Log but don't fail the fuzzing process if report generation fails
                                Logger.Error($"Failed to generate coverage report: {ex.Message}");
                                Logger.Verbose(ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        // If coverage is disabled, use method execution counts from feedback aggregator
                        var feedbackStats = _feedbackAggregator.GetCoverageStatistics();
                        _status.CodeCoverage = feedbackStats.ContainsKey("MethodsCovered")
                            ? (double)(int)feedbackStats["MethodsCovered"] / _status.TotalMethods
                            : 0;
                    }
                }

                // Calculate actual coverage metrics from the coverage tracker
                if (_coverageTracker != null)
                {
                    var coverageStats = _coverageTracker.GetCoverageStatistics();
                    _status.BranchCoverage = coverageStats.BranchCoverage;
                    _status.InstructionCoverage = coverageStats.InstructionCoverage;
                    _status.CodeCoverage = coverageStats.MethodCoverage;
                }

                Logger.Info("Fuzzing completed");
                Logger.Info($"Total executions: {_status.TotalExecutions}");
                Logger.Info($"Successful executions: {_status.SuccessfulExecutions}");
                Logger.Info($"Failed executions: {_status.FailedExecutions}");
                Logger.Info($"Issues found: {_status.IssuesFound}");
                Logger.Info($"Method coverage: {_status.CodeCoverage:P2}");
                Logger.Info($"Branch coverage: {_status.BranchCoverage:P2}");
                Logger.Info($"Instruction coverage: {_status.InstructionCoverage:P2}");

                // Generate final coverage report
                if (_config.EnableCoverage)
                {
                    try
                    {
                        _coverageTracker.GenerateReport();
                        Logger.Info($"Coverage report generated in {Path.Combine(_config.OutputDirectory, "coverage")}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to generate final coverage report: {ex.Message}");
                        Logger.Verbose(ex.StackTrace);
                    }
                }

                // Generate reports
                _reportGenerator.GenerateReports(_issues, _status);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error in fuzzing loop");
                _status.Errors++;
            }
            finally
            {
                _stopwatch.Stop();
            }
        }

        private async Task RunStaticAnalysisAsync()
        {
            Logger.Info("Running static analysis...");

            foreach (var analyzer in _staticAnalyzers)
            {
                try
                {
                    var hints = analyzer.Analyze();
                    foreach (var hint in hints)
                    {
                        _feedbackAggregator.AddStaticAnalysisHint(hint);
                        Logger.Info($"Static analysis hint: {hint.RiskType} - {hint.Description}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in static analysis: {ex.Message}");
                    Logger.Verbose(ex.StackTrace);
                }
            }

            await Task.CompletedTask; // For future async implementation
        }

        private async Task RunSymbolicExecutionAsync(ContractMethodDescriptor[] methods)
        {
            Logger.Info("Running symbolic execution...");

            if (_symbolicExecutionIntegrator == null)
            {
                Logger.Warning("Symbolic execution integrator is not initialized.");
                return;
            }

            foreach (var method in methods)
            {
                try
                {
                    Logger.Info($"Running symbolic execution on method: {method.Name}");
                    var issues = _symbolicExecutionIntegrator.AnalyzeMethod(method);

                    if (issues.Count > 0)
                    {
                        Logger.Info($"Found {issues.Count} issues in method {method.Name} using symbolic execution.");
                        _issues.AddRange(issues);
                        _status.IssuesFound += issues.Count;
                    }
                    else
                    {
                        Logger.Info($"No issues found in method {method.Name} using symbolic execution.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in symbolic execution for method {method.Name}: {ex.Message}");
                    Logger.Verbose(ex.StackTrace);
                }
            }

            await Task.CompletedTask; // For future async implementation
        }

        private ContractMethodDescriptor[] GetMethodsToFuzz()
        {
            var methods = _manifest.Abi.Methods;

            // Filter methods based on configuration
            if (_config.MethodsToFuzz?.Any() == true)
            {
                methods = methods.Where(m => _config.MethodsToFuzz.Contains(m.Name)).ToArray();
            }

            if (_config.MethodsToExclude?.Any() == true)
            {
                methods = methods.Where(m => !_config.MethodsToExclude.Contains(m.Name)).ToArray();
            }

            return methods;
        }

        private List<IssueReport> DetectVulnerabilities(TestCase testCase, ExecutionResult result, ContractMethodDescriptor method)
        {
            var issues = new List<IssueReport>();

            // Run all vulnerability detectors
            foreach (var detector in _vulnerabilityDetectors)
            {
                try
                {
                    var detectedIssues = detector.DetectVulnerabilities(testCase, result);
                    issues.AddRange(detectedIssues);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in vulnerability detector {detector.Name}: {ex.Message}");
                    Logger.Verbose(ex.StackTrace);
                }
            }

            return issues;
        }

        private void MinimizeTestCases(List<IssueReport> issues, ContractMethodDescriptor method)
        {
            foreach (var issue in issues)
            {
                if (issue.TestCase == null)
                    continue;

                try
                {
                    Logger.Info($"Minimizing test case for issue: {issue.IssueType}");

                    // Create a predicate based on the issue type
                    Predicate<ExecutionResult> predicate;
                    switch (issue.IssueType)
                    {
                        case "Crash":
                            predicate = MinimizationPredicates.FailsExecution();
                            break;
                        case "High Gas Consumption":
                            predicate = MinimizationPredicates.ConsumesMoreGasThan(issue.GasConsumed / 2);
                            break;
                        case "Large Storage Key":
                        case "Large Storage Value":
                        case "Many Storage Operations":
                            predicate = result => result.StorageChanges?.Count > 0;
                            break;
                        case "Division by Zero":
                            predicate = MinimizationPredicates.FailsWithExceptionMessage("division by zero|DivideByZeroException");
                            break;
                        case "Integer Overflow":
                            predicate = MinimizationPredicates.FailsWithExceptionMessage("overflow|OverflowException");
                            break;
                        default:
                            // Default predicate: just check if the execution fails
                            predicate = MinimizationPredicates.FailsExecution();
                            break;
                    }

                    // Create a minimizer
                    var minimizer = new TestCaseMinimizer(_executor, method, predicate, _config.Seed);

                    // Minimize the test case
                    var minimizedTestCase = minimizer.Minimize(issue.TestCase);

                    // Update the issue with the minimized test case
                    issue.MinimizedTestCase = minimizedTestCase;

                    Logger.Info($"Test case minimized: {issue.TestCase.Parameters.Length} parameters -> {minimizedTestCase.Parameters.Length} parameters");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error minimizing test case: {ex.Message}");
                    Logger.Verbose(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Prioritizes methods based on coverage feedback to focus fuzzing efforts.
        /// </summary>
        /// <param name="methods">The methods to prioritize.</param>
        /// <param name="iteration">The current iteration number.</param>
        /// <returns>A prioritized list of methods.</returns>
        private List<ContractMethodDescriptor> PrioritizeMethodsBasedOnCoverage(ContractMethodDescriptor[] methods, int iteration)
        {
            // In early iterations, use the original order to establish baseline coverage
            if (iteration < 10 || !_config.EnableCoverageGuidedFuzzing)
            {
                return methods.ToList();
            }

            // Get methods with low coverage
            var lowCoverageMethods = _coverageFeedback?.GetLowCoverageMethods(methods.Length) ?? new List<string>();

            // Get uncovered branches
            var uncoveredBranches = _coverageFeedback?.GetUncoveredBranches(10) ?? new List<int>();

            // Create a dictionary to store method priorities
            var methodPriorities = new Dictionary<string, double>();

            // Initialize all methods with a base priority
            foreach (var method in methods)
            {
                methodPriorities[method.Name] = 1.0;
            }

            // Boost priority for methods with low coverage
            foreach (var methodName in lowCoverageMethods)
            {
                if (methodPriorities.ContainsKey(methodName))
                {
                    // Higher boost for methods with very low coverage (earlier in the list)
                    int index = lowCoverageMethods.IndexOf(methodName);
                    double boost = 3.0 - (index * 0.2); // 3.0, 2.8, 2.6, etc.
                    methodPriorities[methodName] *= boost;

                    Logger.Verbose($"Boosting priority for low-coverage method {methodName} by {boost:F1}x");
                }
            }

            // Every 10th iteration, randomize the order to avoid getting stuck
            if (iteration % 10 == 0)
            {
                Logger.Info("Randomizing method order to avoid local optima");
                var random = new Random(_config.Seed + iteration); // Create a deterministic random generator
                return methods.OrderBy(_ => random.Next()).ToList();
            }

            // Every 20th iteration, focus exclusively on methods with the lowest coverage
            if (iteration % 20 == 0 && lowCoverageMethods.Count > 0)
            {
                Logger.Info("Focusing exclusively on methods with lowest coverage");
                return methods
                    .Where(m => lowCoverageMethods.Contains(m.Name))
                    .OrderBy(m => lowCoverageMethods.IndexOf(m.Name))
                    .ToList();
            }

            // Sort methods by priority (higher priority first)
            return methods
                .OrderByDescending(m => methodPriorities.ContainsKey(m.Name) ? methodPriorities[m.Name] : 1.0)
                .ToList();
        }

        private void SaveExecutionResult(string methodName, TestCase testCase, ExecutionResult result, int iteration)
        {
            // Skip saving if configured to save only failing inputs and the execution succeeded
            if (_config.SaveFailingInputsOnly && result.Success)
                return;

            // Create directories
            string methodDir = Path.Combine(_config.OutputDirectory, "methods", methodName);
            string inputsDir = Path.Combine(methodDir, "inputs");
            string resultsDir = Path.Combine(methodDir, "results");
            Directory.CreateDirectory(inputsDir);
            Directory.CreateDirectory(resultsDir);

            // Save input
            string inputPath = Path.Combine(inputsDir, $"input_{iteration}.json");
            File.WriteAllText(inputPath, testCase.ToJson());

            // Save result
            string resultPath = Path.Combine(resultsDir, $"result_{iteration}.json");
            var resultData = new
            {
                Method = methodName,
                Parameters = testCase.SerializedParameters,
                GasConsumed = result.FeeConsumed,
                State = result.Success ? "Success" : "Failed",
                Exception = result.Exception?.ToString(),
                ReturnValue = result.ReturnValue?.ToString(),
                Timestamp = DateTime.Now,
                Energy = testCase.Energy, // Add energy to the saved result for analysis
                Coverage = _config.EnableCoverage ? GetCoverageSummary() : null // Add coverage summary if enabled
            };

            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };

            string resultJson = System.Text.Json.JsonSerializer.Serialize(resultData, options);
            File.WriteAllText(resultPath, resultJson);

            // If the execution failed, save to issues directory
            if (!result.Success)
            {
                string issuesDir = Path.Combine(_config.OutputDirectory, "issues");
                Directory.CreateDirectory(issuesDir);
                string issuePath = Path.Combine(issuesDir, $"{methodName}_{iteration}.json");
                File.WriteAllText(issuePath, resultJson);
            }
        }

        /// <summary>
        /// Gets a summary of the current coverage statistics.
        /// </summary>
        /// <returns>A dictionary with coverage statistics.</returns>
        private Dictionary<string, object> GetCoverageSummary()
        {
            var summary = new Dictionary<string, object>
            {
                ["MethodCoverage"] = _status.CodeCoverage,
                ["BranchCoverage"] = _status.BranchCoverage,
                ["InstructionCoverage"] = _status.InstructionCoverage
            };

            // Add detailed statistics if available
            if (_config.EnableCoverageGuidedFuzzing && _coverageFeedback != null)
            {
                var detailedStats = _coverageFeedback.GetCoverageStatistics();
                foreach (var stat in detailedStats)
                {
                    summary[$"Detailed_{stat.Key}"] = stat.Value;
                }
            }

            return summary;
        }
    }
}
