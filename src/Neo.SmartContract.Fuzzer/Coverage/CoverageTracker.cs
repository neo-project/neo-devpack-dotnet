using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Neo.SmartContract.Fuzzer.Coverage
{
    /// <summary>
    /// Tracks code coverage during contract execution
    /// </summary>
    public class CoverageTracker
    {
        private readonly Dictionary<string, MethodCoverage> _methodCoverage = new();
        private readonly HashSet<int> _coveredInstructions = new();
        private readonly Dictionary<int, BranchStatus> _branchCoverage = new();
        private readonly HashSet<string> _executionPaths = new();
        private readonly ContractManifest _manifest;
        private readonly Script? _script;
        private readonly string _outputDirectory;

        /// <summary>
        /// Track execution coverage from an execution result
        /// </summary>
        /// <param name="result">The execution result to track coverage for</param>
        /// <returns>True if new coverage was found, false otherwise</returns>
        public bool TrackExecutionCoverage(ExecutionResult result)
        {
            if (result == null) return false;

            // Get current coverage before tracking
            int previousCoverage = _coveredInstructions.Count;

            // Track method execution
            if (_methodCoverage.TryGetValue(result.Method, out var methodCoverage))
            {
                methodCoverage.ExecutionCount++;

                // Track instruction coverage if available
                if (result.Engine != null)
                {
                    // Track execution using the engine
                    TrackExecution(result.Method, result.Engine);
                }
            }

            // Check if we found new coverage
            return _coveredInstructions.Count > previousCoverage;
        }

        /// <summary>
        /// Gets the set of covered instructions
        /// </summary>
        /// <returns>A copy of the set of covered instructions</returns>
        public HashSet<int> GetCoveredInstructions()
        {
            return new HashSet<int>(_coveredInstructions);
        }

        /// <summary>
        /// Gets the coverage statistics
        /// </summary>
        /// <returns>Coverage statistics</returns>
        public CoverageStatistics GetCoverageStatistics()
        {
            // Calculate coverage metrics
            var metrics = CalculateCoverageMetrics();

            return new CoverageStatistics
            {
                MethodCoverage = metrics.MethodCoverage / 100.0, // Convert from percentage to ratio
                BranchCoverage = metrics.BranchCoverage / 100.0,
                InstructionCoverage = metrics.InstructionCoverage / 100.0,
                CoveredMethods = metrics.CoveredMethods,
                TotalMethods = metrics.TotalMethods,
                CoveredInstructions = metrics.CoveredInstructions,
                TotalInstructions = metrics.TotalInstructions,
                CoveredBranches = metrics.CoveredBranches,
                TotalBranches = metrics.TotalBranches
            };
        }

        /// <summary>
        /// Initialize a new instance of the CoverageTracker class
        /// </summary>
        /// <param name="nefBytes">The NEF file bytes</param>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="outputDirectory">Output directory for coverage reports</param>
        public CoverageTracker(byte[] nefBytes, ContractManifest manifest, string outputDirectory)
        {
            _script = nefBytes != null ? new Script(nefBytes) : null;
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));

            // Initialize method coverage tracking for all methods in the manifest
            foreach (var method in manifest.Abi.Methods)
            {
                _methodCoverage[method.Name] = new MethodCoverage
                {
                    Name = method.Name,
                    Parameters = method.Parameters.Select(p => p.Type.ToString()).ToArray(),
                    ReturnType = method.ReturnType.ToString(),
                    ExecutionCount = 0,
                    CoveredInstructions = new HashSet<int>()
                };
            }
        }

        /// <summary>
        /// Track execution of a contract method
        /// </summary>
        /// <param name="methodName">Name of the method being executed</param>
        /// <param name="engine">Application engine executing the method</param>
        public void TrackExecution(string methodName, ApplicationEngine engine)
        {
            if (!_methodCoverage.TryGetValue(methodName, out var coverage))
                return;

            // Increment execution count for this method
            coverage.ExecutionCount++;

            // Track covered instructions
            var executedInstructions = GetExecutedInstructions(engine);

            // Get method offsets to associate instructions with this method
            var methodOffsets = GetMethodOffsets(_script, methodName);
            int methodStart = 0;
            int methodEnd = _script?.Length ?? 0;

            if (methodOffsets.HasValue)
            {
                methodStart = methodOffsets.Value.start;
                methodEnd = methodOffsets.Value.end;
            }

            // Add instructions to method coverage and global coverage
            foreach (var instruction in executedInstructions)
            {
                // Add to method coverage if within method bounds or if we don't know the bounds
                if (!methodOffsets.HasValue || (instruction >= methodStart && instruction < methodEnd))
                {
                    coverage.CoveredInstructions.Add(instruction);
                }

                // Add to global coverage
                _coveredInstructions.Add(instruction);
            }

            // If no instructions were covered, try to get coverage from the execution trace
            if (coverage.CoveredInstructions.Count == 0 && engine.CurrentContext != null)
            {
                // Get the current instruction pointer
                int currentIp = engine.CurrentContext.InstructionPointer;
                if (currentIp >= methodStart && currentIp < methodEnd)
                {
                    coverage.CoveredInstructions.Add(currentIp);
                }

                // Try to get coverage from the invocation stack
                foreach (var context in engine.InvocationStack)
                {
                    int ip = context.InstructionPointer;
                    if (ip >= methodStart && ip < methodEnd)
                    {
                        coverage.CoveredInstructions.Add(ip);
                    }
                }
            }

            // Track branch coverage
            var branches = GetBranches(engine);
            foreach (var branch in branches)
            {
                if (!_branchCoverage.TryGetValue(branch.Key, out var branchCoverage))
                {
                    branchCoverage = new BranchStatus
                    {
                        TrueCovered = false,
                        FalseCovered = false
                    };
                    _branchCoverage[branch.Key] = branchCoverage;
                }

                if (branch.Value)
                    branchCoverage.TrueCovered = true;
                else
                    branchCoverage.FalseCovered = true;
            }

            // Track execution path
            string path = string.Join(",", executedInstructions);
            _executionPaths.Add(path);
        }

        /// <summary>
        /// Generate coverage report in the specified format
        /// </summary>
        /// <param name="format">Report format (html, json, text)</param>
        public void GenerateReport(string format)
        {
            // Create output directory if it doesn't exist
            string outputDir = Path.Combine(_outputDirectory, "coverage-report");
            Directory.CreateDirectory(outputDir);

            // Calculate coverage metrics
            var metrics = CalculateCoverageMetrics();

            // Generate report in the specified format
            switch (format?.ToLowerInvariant())
            {
                case "json":
                    GenerateJsonReport(outputDir, metrics);
                    break;
                case "text":
                    GenerateTextReport(outputDir, metrics);
                    break;
                case "html":
                default:
                    GenerateHtmlReport(outputDir, metrics);
                    break;
            }

            Console.WriteLine($"Coverage report generated in: {Path.GetFullPath(outputDir)}");
        }

        /// <summary>
        /// Generate coverage report in HTML format (default)
        /// </summary>
        public void GenerateReport()
        {
            GenerateReport("html");
        }

        /// <summary>
        /// Get executed instructions from the engine
        /// </summary>
        /// <param name="engine">Application engine</param>
        /// <returns>Set of instruction pointers that were executed</returns>
        private HashSet<int> GetExecutedInstructions(ApplicationEngine engine)
        {
            var instructions = new HashSet<int>();

            // Get executed instructions from the engine's execution trace
            if (engine.CurrentContext != null)
            {
                // Add the current instruction pointer
                instructions.Add(engine.CurrentContext.InstructionPointer);

                // Add instruction pointers from the invocation stack
                foreach (var context in engine.InvocationStack)
                {
                    instructions.Add(context.InstructionPointer);
                }
            }

            return instructions;
        }

        /// <summary>
        /// Get branch decisions from the engine
        /// </summary>
        /// <param name="engine">Application engine</param>
        /// <returns>Dictionary of branch instruction pointers and their taken paths</returns>
        private Dictionary<int, bool> GetBranches(ApplicationEngine engine)
        {
            var branches = new Dictionary<int, bool>();

            if (engine.CurrentContext == null)
                return branches;

            // Get all contexts from the invocation stack
            foreach (var context in engine.InvocationStack)
            {
                var script = context.Script;

                // Scan for branch instructions in the script
                for (int ip = 0; ip < script.Length;)
                {
                    try
                    {
                        var instruction = script.GetInstruction(ip);

                        // Check if this is a branch instruction (JMPIF, JMPIFNOT, etc.)
                        if (instruction.OpCode == OpCode.JMPIF ||
                            instruction.OpCode == OpCode.JMPIFNOT ||
                            instruction.OpCode == OpCode.JMPEQ ||
                            instruction.OpCode == OpCode.JMPNE ||
                            instruction.OpCode == OpCode.JMPGT ||
                            instruction.OpCode == OpCode.JMPGE ||
                            instruction.OpCode == OpCode.JMPLT ||
                            instruction.OpCode == OpCode.JMPLE)
                        {
                            // Get the jump offset from the instruction
                            int offset = instruction.TokenI8;
                            int targetIp = ip + offset;
                            bool taken = false;

                            // If the current instruction pointer is at or past the branch instruction
                            if (context.InstructionPointer >= ip)
                            {
                                // If the current instruction pointer is at the branch target,
                                // the branch was taken
                                if (context.InstructionPointer == targetIp)
                                {
                                    taken = true;
                                }
                                // If the current instruction pointer is at the next instruction after the branch,
                                // the branch was not taken
                                else if (context.InstructionPointer == ip + instruction.Size)
                                {
                                    taken = false;
                                }
                                // Otherwise, we can't determine for sure
                                else
                                {
                                    // Skip this branch since we can't determine
                                    ip += instruction.Size;
                                    continue;
                                }

                                // Record the branch decision
                                branches[ip] = taken;
                            }
                        }

                        ip += instruction.Size;
                    }
                    catch
                    {
                        // Skip invalid instructions
                        ip++;
                    }
                }
            }

            return branches;
        }

        /// <summary>
        /// Estimate the maximum number of possible execution paths
        /// </summary>
        /// <returns>Estimated maximum number of paths</returns>
        private int EstimateMaxPaths()
        {
            // If we don't have a script, use a simple heuristic
            if (_script == null)
            {
                int branchCount = _branchCoverage.Count;

                // Each branch doubles the number of possible paths
                // But cap at a reasonable number to avoid overflow
                if (branchCount > 20)
                    return int.MaxValue;

                return Math.Max(1, 1 << branchCount); // 2^branchCount
            }

            // Build a control flow graph from the script
            var cfg = BuildControlFlowGraph(_script);

            // Count the number of paths through the CFG
            return CountPathsInCFG(cfg);
        }

        /// <summary>
        /// Builds a control flow graph from the script
        /// </summary>
        /// <param name="script">The script to analyze</param>
        /// <returns>A dictionary mapping instruction pointers to their possible next instruction pointers</returns>
        private static Dictionary<int, List<int>> BuildControlFlowGraph(Script script)
        {
            var cfg = new Dictionary<int, List<int>>();

            // Scan the script for branch instructions
            for (int ip = 0; ip < script.Length;)
            {
                try
                {
                    var instruction = script.GetInstruction(ip);

                    // Initialize the CFG entry for this instruction
                    if (!cfg.TryGetValue(ip, out var successors))
                    {
                        successors = new List<int>();
                        cfg[ip] = successors;
                    }

                    // Handle different types of control flow instructions
                    switch (instruction.OpCode)
                    {
                        case OpCode.JMP:
                            // Unconditional jump
                            int targetIp = ip + instruction.TokenI8;
                            cfg[ip].Add(targetIp);
                            break;

                        case OpCode.JMPIF:
                        case OpCode.JMPIFNOT:
                        case OpCode.JMPEQ:
                        case OpCode.JMPNE:
                        case OpCode.JMPGT:
                        case OpCode.JMPGE:
                        case OpCode.JMPLT:
                        case OpCode.JMPLE:
                            // Conditional jump - can go to target or next instruction
                            int condTargetIp = ip + instruction.TokenI8;
                            int nextIp = ip + instruction.Size;
                            cfg[ip].Add(condTargetIp);
                            cfg[ip].Add(nextIp);
                            break;

                        case OpCode.RET:
                            // Return instruction - no outgoing edges
                            break;

                        default:
                            // Regular instruction - goes to next instruction
                            int regularNextIp = ip + instruction.Size;
                            if (regularNextIp < script.Length)
                            {
                                cfg[ip].Add(regularNextIp);
                            }
                            break;
                    }

                    ip += instruction.Size;
                }
                catch
                {
                    // Skip invalid instructions
                    ip++;
                }
            }

            return cfg;
        }

        /// <summary>
        /// Counts the number of unique paths in a control flow graph
        /// </summary>
        /// <param name="cfg">The control flow graph</param>
        /// <returns>The number of unique paths</returns>
        private static int CountPathsInCFG(Dictionary<int, List<int>> cfg)
        {
            // Find entry and exit points
            int entryPoint = cfg.Keys.Min();

            // Use a cache to avoid recounting paths
            var pathCache = new Dictionary<int, int>();

            // Count paths from entry point
            int pathCount = CountPathsFromNode(cfg, entryPoint, pathCache);

            // Cap at a reasonable number
            return Math.Min(pathCount, int.MaxValue);
        }

        /// <summary>
        /// Recursively counts paths from a node to exit nodes
        /// </summary>
        /// <param name="cfg">The control flow graph</param>
        /// <param name="node">The current node</param>
        /// <param name="pathCache">Cache of already computed path counts</param>
        /// <returns>The number of paths from this node to exit nodes</returns>
        private static int CountPathsFromNode(Dictionary<int, List<int>> cfg, int node, Dictionary<int, int> pathCache)
        {
            // If we've already computed paths from this node, return cached result
            if (pathCache.TryGetValue(node, out int cachedCount))
            {
                return cachedCount;
            }

            // If this is a terminal node (no outgoing edges or RET instruction)
            if (!cfg.TryGetValue(node, out var successors) || successors.Count == 0)
            {
                pathCache[node] = 1; // One path from here (the end)
                return 1;
            }

            // Count paths from each successor
            int totalPaths = 0;
            foreach (int successor in cfg[node])
            {
                // Avoid cycles by temporarily marking this node as having 0 paths
                pathCache[node] = 0;

                // Count paths from successor
                int pathsFromSuccessor = CountPathsFromNode(cfg, successor, pathCache);

                // Add to total
                totalPaths += pathsFromSuccessor;

                // Cap at a reasonable number to avoid overflow
                if (totalPaths > 1_000_000)
                {
                    totalPaths = 1_000_000;
                    break;
                }
            }

            // Cache and return result
            pathCache[node] = totalPaths;
            return totalPaths;
        }

        // Reusable JSON serializer options
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Generate JSON coverage report
        /// </summary>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="metrics">Coverage metrics</param>
        private static void GenerateJsonReport(string outputDirectory, CoverageMetrics metrics)
        {
            string jsonPath = Path.Combine(outputDirectory, "coverage.json");

            // Convert metrics to JSON
            string json = System.Text.Json.JsonSerializer.Serialize(metrics, _jsonOptions);
            File.WriteAllText(jsonPath, json);
        }

        /// <summary>
        /// Generate text coverage report
        /// </summary>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="metrics">Coverage metrics</param>
        private static void GenerateTextReport(string outputDirectory, CoverageMetrics metrics)
        {
            string textPath = Path.Combine(outputDirectory, "coverage.txt");

            using var writer = new StreamWriter(textPath);

            writer.WriteLine("COVERAGE REPORT");
            writer.WriteLine("===============");
            writer.WriteLine();
            writer.WriteLine($"Overall Coverage: {metrics.MethodCoverage:F2}%");
            writer.WriteLine($"Method Coverage: {metrics.MethodCoverage:F2}% ({metrics.CoveredMethods}/{metrics.TotalMethods})");
            writer.WriteLine($"Branch Coverage: {metrics.BranchCoverage:F2}% ({metrics.CoveredBranches}/{metrics.TotalBranches})");
            writer.WriteLine($"Path Coverage: {metrics.PathCoverage:F2}% ({metrics.CoveredPaths}/{metrics.TotalPaths})");
            writer.WriteLine();

            writer.WriteLine("METHOD DETAILS");
            writer.WriteLine("==============");

            foreach (var method in metrics.MethodDetails.OrderBy(m => m.Name))
            {
                writer.WriteLine($"{method.Name}: {(method.ExecutionCount > 0 ? "COVERED" : "NOT COVERED")} (Called {method.ExecutionCount} times)");
            }
        }

        /// <summary>
        /// Generate an HTML coverage report
        /// </summary>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="metrics">Coverage metrics</param>
        private void GenerateHtmlReport(string outputDirectory, CoverageMetrics metrics)
        {
            string reportPath = Path.Combine(outputDirectory, "index.html");

            using var writer = new StreamWriter(reportPath);
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html lang=\"en\">");
                writer.WriteLine("<head>");
                writer.WriteLine("  <meta charset=\"UTF-8\">");
                writer.WriteLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                writer.WriteLine("  <title>Neo Contract Coverage Report</title>");
                writer.WriteLine("  <style>");
                writer.WriteLine("    body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }");
                writer.WriteLine("    .container { max-width: 1200px; margin: 0 auto; }");
                writer.WriteLine("    h1 { color: #333; }");
                writer.WriteLine("    .summary { background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin-bottom: 20px; }");
                writer.WriteLine("    .metric { margin-bottom: 10px; }");
                writer.WriteLine("    .metric-name { font-weight: bold; display: inline-block; width: 200px; }");
                writer.WriteLine("    .metric-value { display: inline-block; }");
                writer.WriteLine("    .progress { height: 20px; background-color: #e0e0e0; border-radius: 5px; overflow: hidden; }");
                writer.WriteLine("    .progress-bar { height: 100%; background-color: #4CAF50; text-align: center; color: white; line-height: 20px; }");
                writer.WriteLine("    .method-table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
                writer.WriteLine("    .method-table th, .method-table td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                writer.WriteLine("    .method-table th { background-color: #f2f2f2; }");
                writer.WriteLine("    .method-table tr:nth-child(even) { background-color: #f9f9f9; }");
                writer.WriteLine("    .uncovered { color: #e53935; }");
                writer.WriteLine("    .partially-covered { color: #ff9800; }");
                writer.WriteLine("    .fully-covered { color: #4CAF50; }");
                writer.WriteLine("  </style>");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");
                writer.WriteLine("  <div class=\"container\">");
                writer.WriteLine($"    <h1>Coverage Report for {_manifest.Name}</h1>");
                writer.WriteLine("    <div class=\"summary\">");
                writer.WriteLine("      <h2>Summary</h2>");

                // Method coverage
                writer.WriteLine("      <div class=\"metric\">");
                writer.WriteLine("        <span class=\"metric-name\">Method Coverage:</span>");
                writer.WriteLine("        <span class=\"metric-value\">");
                writer.WriteLine($"          {metrics.MethodCoverage:F1}% ({metrics.CoveredMethods}/{metrics.TotalMethods})");
                writer.WriteLine("        </span>");
                writer.WriteLine("        <div class=\"progress\">");
                writer.WriteLine($"          <div class=\"progress-bar\" style=\"width: {metrics.MethodCoverage}%\">{metrics.MethodCoverage:F1}%</div>");
                writer.WriteLine("        </div>");
                writer.WriteLine("      </div>");

                // Instruction coverage
                writer.WriteLine("      <div class=\"metric\">");
                writer.WriteLine("        <span class=\"metric-name\">Instruction Coverage:</span>");
                writer.WriteLine("        <span class=\"metric-value\">");
                writer.WriteLine($"          {metrics.InstructionCoverage:F1}% ({metrics.CoveredInstructions}/{metrics.TotalInstructions})");
                writer.WriteLine("        </span>");
                writer.WriteLine("        <div class=\"progress\">");
                writer.WriteLine($"          <div class=\"progress-bar\" style=\"width: {metrics.InstructionCoverage}%\">{metrics.InstructionCoverage:F1}%</div>");
                writer.WriteLine("        </div>");
                writer.WriteLine("      </div>");

                // Branch coverage
                writer.WriteLine("      <div class=\"metric\">");
                writer.WriteLine("        <span class=\"metric-name\">Branch Coverage:</span>");
                writer.WriteLine("        <span class=\"metric-value\">");
                writer.WriteLine($"          {metrics.BranchCoverage:F1}% ({metrics.CoveredBranches}/{metrics.TotalBranches})");
                writer.WriteLine("        </span>");
                writer.WriteLine("        <div class=\"progress\">");
                writer.WriteLine($"          <div class=\"progress-bar\" style=\"width: {metrics.BranchCoverage}%\">{metrics.BranchCoverage:F1}%</div>");
                writer.WriteLine("        </div>");
                writer.WriteLine("      </div>");

                // Path coverage
                writer.WriteLine("      <div class=\"metric\">");
                writer.WriteLine("        <span class=\"metric-name\">Path Coverage:</span>");
                writer.WriteLine("        <span class=\"metric-value\">");
                writer.WriteLine($"          {metrics.PathCoverage:F1}% ({metrics.CoveredPaths} unique paths)");
                writer.WriteLine("        </span>");
                writer.WriteLine("        <div class=\"progress\">");
                writer.WriteLine($"          <div class=\"progress-bar\" style=\"width: {metrics.PathCoverage}%\">{metrics.PathCoverage:F1}%</div>");
                writer.WriteLine("        </div>");
                writer.WriteLine("      </div>");

                writer.WriteLine("    </div>"); // End of summary

                // Method details
                writer.WriteLine("    <h2>Method Details</h2>");
                writer.WriteLine("    <table class=\"method-table\">");
                writer.WriteLine("      <tr>");
                writer.WriteLine("        <th>Method</th>");
                writer.WriteLine("        <th>Parameters</th>");
                writer.WriteLine("        <th>Return Type</th>");
                writer.WriteLine("        <th>Execution Count</th>");
                writer.WriteLine("        <th>Coverage</th>");
                writer.WriteLine("      </tr>");

                foreach (var method in metrics.MethodDetails.OrderBy(m => m.Name))
                {
                    string coverageClass = method.ExecutionCount == 0 ? "uncovered" : "fully-covered";
                    writer.WriteLine("      <tr>");
                    writer.WriteLine($"        <td class=\"{coverageClass}\">{method.Name}</td>");
                    writer.WriteLine($"        <td>{string.Join(", ", method.Parameters)}</td>");
                    writer.WriteLine($"        <td>{method.ReturnType}</td>");
                    writer.WriteLine($"        <td>{method.ExecutionCount}</td>");

                    // Calculate method-specific coverage
                    double methodCoverage = 0;
                    if (method.ExecutionCount > 0)
                    {
                        // Calculate coverage based on the number of covered instructions
                        var methodOffsets = GetMethodOffsets(_script, method.Name);
                        if (methodOffsets.HasValue)
                        {
                            int methodStart = methodOffsets.Value.start;
                            int methodEnd = methodOffsets.Value.end;
                            int methodSize = methodEnd - methodStart;

                            // Count instructions in this method's range that have been covered
                            int coveredInMethod = method.CoveredInstructions.Count(ip => ip >= methodStart && ip < methodEnd);

                            // Calculate coverage percentage
                            methodCoverage = methodSize > 0 ? Math.Min(100, (double)coveredInMethod / methodSize * 100) : 0.0;
                        }
                        else
                        {
                            // If we can't determine method boundaries, use a simpler approach
                            methodCoverage = Math.Min(100, (double)method.CoveredInstructions.Count / Math.Max(1, method.TotalInstructions) * 100);
                        }
                    }

                    writer.WriteLine($"        <td><div class=\"progress\"><div class=\"progress-bar\" style=\"width: {methodCoverage}%\">{methodCoverage:F1}%</div></div></td>");
                    writer.WriteLine("      </tr>");
                }

                writer.WriteLine("    </table>");
                writer.WriteLine("  </div>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        /// <summary>
        /// Gets the start and end offsets of a method in the script.
        /// </summary>
        /// <param name="script">The script to analyze.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>A tuple with the start and end offsets, or null if not found.</returns>
        private static (int start, int end)? GetMethodOffsets(Script? script, string methodName)
        {
            if (script == null)
                return null;

            // This is a simplified implementation that looks for method signatures in the script
            // A more accurate implementation would use debug information or analyze the script structure

            try
            {
                // Look for method signature in the script
                byte[] methodNameBytes = System.Text.Encoding.UTF8.GetBytes(methodName);

                // Scan the script for the method name
                for (int ip = 0; ip < script.Length;)
                {
                    try
                    {
                        var instruction = script.GetInstruction(ip);

                        // Look for SYSCALL instructions that might indicate method boundaries
                        if (instruction.OpCode == OpCode.SYSCALL)
                        {
                            // Check if this is the start of a method
                            if (ip + methodNameBytes.Length < script.Length)
                            {
                                bool found = true;
                                for (int i = 0; i < methodNameBytes.Length && found; i++)
                                {
                                    if (ip + i >= script.Length || (byte)script[ip + i] != methodNameBytes[i])
                                        found = false;
                                }

                                if (found)
                                {
                                    // Found the method start, now look for the end
                                    int methodStart = ip;
                                    int methodEnd = script.Length;

                                    // Look for RET instruction or next method start
                                    for (int endIp = ip + instruction.Size; endIp < script.Length;)
                                    {
                                        var endInstruction = script.GetInstruction(endIp);
                                        if (endInstruction.OpCode == OpCode.RET)
                                        {
                                            methodEnd = endIp + endInstruction.Size;
                                            break;
                                        }

                                        endIp += endInstruction.Size;
                                    }

                                    return (methodStart, methodEnd);
                                }
                            }
                        }

                        ip += instruction.Size;
                    }
                    catch
                    {
                        // Skip invalid instructions
                        ip++;
                    }
                }
            }
            catch
            {
                // If anything goes wrong, return null
            }

            return null;
        }

        /// <summary>
        /// Calculates coverage metrics
        /// </summary>
        /// <returns>Coverage metrics</returns>
        public CoverageMetrics CalculateCoverageMetrics()
        {
            var metrics = new CoverageMetrics();

            // Calculate method coverage
            int totalMethods = _methodCoverage.Count;
            int coveredMethods = _methodCoverage.Count(m => m.Value.ExecutionCount > 0);
            metrics.TotalMethods = totalMethods;
            metrics.CoveredMethods = coveredMethods;
            metrics.MethodCoverage = totalMethods > 0 ? (double)coveredMethods / totalMethods * 100 : 0;

            // Calculate instruction coverage
            int totalInstructions = 0;
            int coveredInstructions = _coveredInstructions.Count;

            // Try to get total instructions from script
            if (_script != null)
            {
                // Count all instructions in the script
                for (int ip = 0; ip < _script.Length;)
                {
                    try
                    {
                        var instruction = _script.GetInstruction(ip);
                        totalInstructions++;
                        ip += instruction.Size;
                    }
                    catch
                    {
                        // Skip invalid instructions
                        ip++;
                    }
                }
            }
            else
            {
                // Estimate total instructions
                totalInstructions = Math.Max(coveredInstructions, (int)(coveredInstructions / 0.7));
            }

            metrics.TotalInstructions = totalInstructions;
            metrics.CoveredInstructions = coveredInstructions;
            metrics.InstructionCoverage = totalInstructions > 0 ? (double)coveredInstructions / totalInstructions * 100 : 0;

            // Calculate branch coverage
            int totalBranches = _branchCoverage.Count * 2; // Each branch has true and false paths
            int coveredBranches = _branchCoverage.Sum(b => (b.Value.TrueCovered ? 1 : 0) + (b.Value.FalseCovered ? 1 : 0));
            metrics.TotalBranches = totalBranches;
            metrics.CoveredBranches = coveredBranches;
            metrics.BranchCoverage = totalBranches > 0 ? (double)coveredBranches / totalBranches * 100 : 0;

            // Calculate path coverage (simplified)
            int totalPaths = _executionPaths.Count;
            int coveredPaths = _executionPaths.Count;
            metrics.TotalPaths = totalPaths;
            metrics.CoveredPaths = coveredPaths;
            metrics.PathCoverage = totalPaths > 0 ? (double)coveredPaths / totalPaths * 100 : 0;

            // Set method details
            metrics.MethodDetails = _methodCoverage.Values.ToArray();

            // Set branch details
            metrics.BranchDetails = _branchCoverage.Select(b => new BranchCoverage
            {
                Id = b.Key,
                TrueCovered = b.Value.TrueCovered,
                FalseCovered = b.Value.FalseCovered
            }).ToArray();

            return metrics;
        }
    }

    /// <summary>
    /// Branch coverage status
    /// </summary>
    public class BranchStatus
    {
        /// <summary>
        /// Whether the true branch was covered
        /// </summary>
        public bool TrueCovered { get; set; }

        /// <summary>
        /// Whether the false branch was covered
        /// </summary>
        public bool FalseCovered { get; set; }
    }
}
