using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Fuzzer.Reporting;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Integrates the symbolic execution engine with the feedback-guided fuzzing process.
    /// </summary>
    public class SymbolicExecutionIntegrator
    {
        private readonly FuzzerConfiguration _config;
        private readonly ContractManifest _manifest;
        private readonly byte[] _nefBytes;
        private readonly FeedbackAggregator _feedbackAggregator;
        private readonly List<ISymbolicVulnerabilityDetector> _vulnerabilityDetectors = new List<ISymbolicVulnerabilityDetector>();
        private readonly IConstraintSolver _solver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicExecutionIntegrator"/> class.
        /// </summary>
        /// <param name="config">The fuzzer configuration.</param>
        /// <param name="manifest">The contract manifest.</param>
        /// <param name="nefBytes">The NEF file bytes.</param>
        /// <param name="feedbackAggregator">The feedback aggregator.</param>
        public SymbolicExecutionIntegrator(
            FuzzerConfiguration config,
            ContractManifest manifest,
            byte[] nefBytes,
            FeedbackAggregator feedbackAggregator)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _nefBytes = nefBytes ?? throw new ArgumentNullException(nameof(nefBytes));
            _feedbackAggregator = feedbackAggregator ?? throw new ArgumentNullException(nameof(feedbackAggregator));

            // Initialize constraint solver using the unified constraint solver
            var unifiedSolver = Solvers.ConstraintSolverFactory.Create(Solvers.ConstraintSolverType.Unified, config.Seed);
            _solver = new Solvers.ConstraintSolverAdapter(unifiedSolver);

            // Initialize vulnerability detectors
            _vulnerabilityDetectors.Add(new IntegerOverflowDetector());
            _vulnerabilityDetectors.Add(new DivisionByZeroDetector());
            _vulnerabilityDetectors.Add(new UnauthorizedAccessDetector());
            _vulnerabilityDetectors.Add(new StorageExhaustionDetector());
        }

        /// <summary>
        /// Executes symbolic analysis on a method and returns the results.
        /// </summary>
        /// <param name="method">The method to analyze.</param>
        /// <returns>A list of issue reports for any vulnerabilities found.</returns>
        public List<IssueReport> AnalyzeMethod(ContractMethodDescriptor method)
        {
            Console.WriteLine($"Starting symbolic analysis of method: {method.Name}");

            var issues = new List<IssueReport>();

            try
            {
                // Create symbolic arguments for the method
                var parameterTypes = method.Parameters.Select(p => p.Type.ToString()).ToList();
                var symbolicArguments = SymbolicExecutionEngine.CreateSymbolicArgumentsForMethod(parameterTypes);

                // Create the symbolic execution engine
                var script = new Script(_nefBytes);
                var evaluationService = new SimpleEvaluationService();
                var limits = new ExecutionEngineLimits
                {
                    MaxStackSize = 2048,
                    MaxInvocationStackSize = 1024
                };

                var engine = new SymbolicExecutionEngine(
                    _nefBytes,
                    _solver,
                    _vulnerabilityDetectors,
                    symbolicArguments,
                    _config.SymbolicExecutionDepth);

                // Execute the symbolic engine
                var result = engine.Execute();

                // Process the results
                foreach (var path in result.ExecutionPaths)
                {
                    // Check if the path ended in a fault
                    if (path.HaltReason == VMState.FAULT)
                    {
                        // Create an issue report
                        var issue = new IssueReport
                        {
                            IssueType = "Symbolic Execution Fault",
                            Severity = IssueSeverity.High,
                            Description = $"Symbolic execution of method {method.Name} resulted in a fault.",
                            MethodName = method.Name,
                            VMState = path.HaltReason.ToString()
                        };

                        // Add the issue to the list
                        issues.Add(issue);
                    }

                    // Check for vulnerabilities detected during symbolic execution
                    foreach (var vulnerability in path.Vulnerabilities)
                    {
                        // Create an issue report
                        var issue = new IssueReport
                        {
                            IssueType = vulnerability.Type,
                            Severity = ConvertSeverity(vulnerability.Severity),
                            Description = vulnerability.Description,
                            MethodName = method.Name,
                            VMState = path.HaltReason.ToString(),
                            Remediation = vulnerability.Remediation
                        };

                        // Add the issue to the list
                        issues.Add(issue);
                    }

                    // Generate concrete test cases from the symbolic path
                    if (_solver.TrySolve(path.PathConstraints, out var solution))
                    {
                        // Create a test case from the solution
                        var testCase = CreateTestCaseFromSolution(method, solution);

                        // Add the test case to the feedback aggregator
                        _feedbackAggregator.AddStaticAnalysisHint(new StaticAnalysis.StaticAnalysisHint
                        {
                            FilePath = "Symbolic Execution",
                            LineNumber = 0,
                            RiskType = "SymbolicPath",
                            Description = $"Symbolic execution found a path in method {method.Name}",
                            Priority = 70,
                            MethodName = method.Name
                        });

                        // Save the test case to the corpus directory
                        SaveTestCase(method.Name, testCase);
                    }
                }

                Console.WriteLine($"Symbolic analysis of method {method.Name} completed. Found {issues.Count} issues.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during symbolic analysis of method {method.Name}: {ex.Message}");
            }

            return issues;
        }

        private TestCase CreateTestCaseFromSolution(ContractMethodDescriptor method, Dictionary<string, object> solution)
        {
            // Create a test case from the solution
            var parameters = new VM.Types.StackItem[method.Parameters.Length];

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                string varName = $"arg{i}";
                if (solution.TryGetValue(varName, out var value))
                {
                    parameters[i] = ConvertToStackItem(value, method.Parameters[i].Type.ToString());
                }
                else
                {
                    // If the solution doesn't contain a value for this parameter, use a default value
                    parameters[i] = CreateDefaultStackItem(method.Parameters[i].Type.ToString());
                }
            }

            return new TestCase
            {
                MethodName = method.Name,
                Parameters = parameters,
                Energy = 2.0, // Higher energy for symbolic execution test cases
                Iteration = 0,
                Timestamp = DateTime.Now
            };
        }

        private VM.Types.StackItem ConvertToStackItem(object value, string type)
        {
            switch (value)
            {
                case bool boolValue:
                    return boolValue ? VM.Types.StackItem.True : VM.Types.StackItem.False;
                case int intValue:
                    return new VM.Types.Integer(intValue);
                case long longValue:
                    return new VM.Types.Integer(longValue);
                case System.Numerics.BigInteger bigIntValue:
                    return new VM.Types.Integer(bigIntValue);
                case byte[] byteArrayValue:
                    return new VM.Types.ByteString(byteArrayValue);
                case string stringValue:
                    return new VM.Types.ByteString(System.Text.Encoding.UTF8.GetBytes(stringValue));
                default:
                    return CreateDefaultStackItem(type);
            }
        }

        private VM.Types.StackItem CreateDefaultStackItem(string type)
        {
            switch (type)
            {
                case "Boolean":
                    return VM.Types.StackItem.False;
                case "Integer":
                    return new VM.Types.Integer(0);
                case "ByteArray":
                case "String":
                    return new VM.Types.ByteString(System.Array.Empty<byte>());
                case "Hash160":
                    return new VM.Types.ByteString(new byte[20]);
                case "Hash256":
                    return new VM.Types.ByteString(new byte[32]);
                case "PublicKey":
                    return new VM.Types.ByteString(new byte[33]);
                case "Array":
                    return new VM.Types.Array();
                case "Map":
                    return new VM.Types.Map();
                default:
                    return VM.Types.StackItem.Null;
            }
        }

        private IssueSeverity ConvertSeverity(SymbolicVulnerabilitySeverity severity)
        {
            switch (severity)
            {
                case SymbolicVulnerabilitySeverity.Critical:
                    return IssueSeverity.Critical;
                case SymbolicVulnerabilitySeverity.High:
                    return IssueSeverity.High;
                case SymbolicVulnerabilitySeverity.Medium:
                    return IssueSeverity.Medium;
                case SymbolicVulnerabilitySeverity.Low:
                    return IssueSeverity.Low;
                default:
                    return IssueSeverity.Medium;
            }
        }

        private void SaveTestCase(string methodName, TestCase testCase)
        {
            try
            {
                string corpusDirectory = Path.Combine(_config.OutputDirectory, "corpus");
                Directory.CreateDirectory(corpusDirectory);

                string filePath = Path.Combine(corpusDirectory, $"{methodName}_symbolic_{Guid.NewGuid()}.json");
                File.WriteAllText(filePath, testCase.ToJson());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving symbolic test case: {ex.Message}");
            }
        }
    }
}


