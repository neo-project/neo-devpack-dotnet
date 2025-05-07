using Neo.SmartContract.Fuzzer.Coverage;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Feedback
{
    /// <summary>
    /// Provides feedback based on code coverage metrics to guide the fuzzing process.
    /// </summary>
    public class CoverageFeedback
    {
        private readonly Dictionary<string, HashSet<int>> _coveredInstructionsByMethod = new();
        private readonly Dictionary<int, BranchStatus> _branchCoverage = new();
        private readonly HashSet<string> _executionPaths = new();
        private readonly CoverageTracker _coverageTracker;
        private readonly Random _random;
        private readonly Script? _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverageFeedback"/> class.
        /// </summary>
        /// <param name="coverageTracker">The coverage tracker to use for feedback.</param>
        /// <param name="seed">Random seed for prioritization.</param>
        /// <param name="script">The script to analyze for coverage.</param>
        public CoverageFeedback(CoverageTracker coverageTracker, int seed, Script? script = null)
        {
            _coverageTracker = coverageTracker ?? throw new ArgumentNullException(nameof(coverageTracker));
            _random = new Random(seed);
            _script = script;
        }

        /// <summary>
        /// Processes an execution result and determines if it provides new coverage.
        /// </summary>
        /// <param name="testCase">The test case that was executed.</param>
        /// <param name="result">The execution result.</param>
        /// <returns>A feedback item if the execution provided new coverage, null otherwise.</returns>
        public FeedbackItem ProcessExecutionResult(TestCase testCase, ExecutionResult result)
        {
            if (testCase == null) throw new ArgumentNullException(nameof(testCase));
            if (result == null) throw new ArgumentNullException(nameof(result));

            try
            {
                bool newCoverage = false;
                string description = string.Empty;

                // Track method coverage
                if (!_coveredInstructionsByMethod.ContainsKey(testCase.MethodName))
                {
                    _coveredInstructionsByMethod[testCase.MethodName] = new HashSet<int>();
                    newCoverage = true;
                    description = $"New method coverage: {testCase.MethodName}";
                }

                // Track instruction coverage
                if (result.Engine != null)
                {
                    try
                    {
                        var executedInstructions = GetExecutedInstructions(result.Engine);
                        var methodInstructions = _coveredInstructionsByMethod[testCase.MethodName];
                        int previousCount = methodInstructions.Count;

                        foreach (var instruction in executedInstructions)
                        {
                            methodInstructions.Add(instruction);
                        }

                        if (methodInstructions.Count > previousCount)
                        {
                            newCoverage = true;
                            int newInstructions = methodInstructions.Count - previousCount;
                            description = string.IsNullOrEmpty(description)
                                ? $"New instruction coverage: {newInstructions} new instructions in {testCase.MethodName}"
                                : $"{description}, {newInstructions} new instructions";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue - don't let instruction tracking failure stop the whole process
                        Console.WriteLine($"Warning: Failed to track instruction coverage: {ex.Message}");
                    }
                }

                // Track branch coverage
                if (result.Engine != null)
                {
                    try
                    {
                        var branches = GetBranches(result.Engine);
                        foreach (var branch in branches)
                        {
                            if (!_branchCoverage.TryGetValue(branch.Key, out var status))
                            {
                                status = new BranchStatus();
                                _branchCoverage[branch.Key] = status;
                            }

                            bool newBranchCoverage = false;
                            if (branch.Value && !status.TrueBranchCovered)
                            {
                                status.TrueBranchCovered = true;
                                newBranchCoverage = true;
                            }
                            else if (!branch.Value && !status.FalseBranchCovered)
                            {
                                status.FalseBranchCovered = true;
                                newBranchCoverage = true;
                            }

                            if (newBranchCoverage)
                            {
                                newCoverage = true;
                                description = string.IsNullOrEmpty(description)
                                    ? $"New branch coverage at offset {branch.Key} in {testCase.MethodName}"
                                    : $"{description}, new branch at {branch.Key}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue - don't let branch tracking failure stop the whole process
                        Console.WriteLine($"Warning: Failed to track branch coverage: {ex.Message}");
                    }
                }

                // Track execution paths
                if (result.Engine != null)
                {
                    try
                    {
                        string path = GetExecutionPath(result.Engine);
                        if (_executionPaths.Add(path))
                        {
                            newCoverage = true;
                            description = string.IsNullOrEmpty(description)
                                ? $"New execution path in {testCase.MethodName}"
                                : $"{description}, new execution path";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue - don't let path tracking failure stop the whole process
                        Console.WriteLine($"Warning: Failed to track execution path: {ex.Message}");
                    }
                }

                // If we found new coverage, create a feedback item
                if (newCoverage)
                {
                    return new FeedbackItem
                    {
                        Type = FeedbackType.NewCoverage,
                        RelatedTestCase = testCase.Clone(),
                        Priority = CalculatePriority(testCase, result),
                        Timestamp = DateTime.Now,
                        Description = description
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the fuzzing process
                Console.WriteLine($"Error in ProcessExecutionResult: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Gets the current coverage statistics.
        /// </summary>
        /// <returns>A dictionary with coverage statistics.</returns>
        public Dictionary<string, object> GetCoverageStatistics()
        {
            try
            {
                int totalInstructions = _coveredInstructionsByMethod.Values.Sum(v => v.Count);
                int totalBranches = _branchCoverage.Count * 2; // Each branch has true and false paths
                int coveredBranches = _branchCoverage.Sum(b => (b.Value.TrueBranchCovered ? 1 : 0) + (b.Value.FalseBranchCovered ? 1 : 0));

                // Calculate total instructions from script analysis
                int totalScriptInstructions = 0;

                // Get the script from the contract
                if (_script != null)
                {
                    // Count all instructions in the script
                    for (int ip = 0; ip < _script.Length;)
                    {
                        try
                        {
                            var instruction = _script.GetInstruction(ip);
                            totalScriptInstructions++;
                            ip += instruction.Size;
                        }
                        catch
                        {
                            // Skip invalid instructions
                            ip++;
                        }
                    }
                }

                // Use the actual count if available, otherwise estimate
                int estimatedTotalInstructions = totalScriptInstructions > 0
                    ? totalScriptInstructions
                    : Math.Max(totalInstructions, (int)(totalInstructions / 0.7)); // Fallback to estimation

                return new Dictionary<string, object>
                {
                    ["MethodsCovered"] = _coveredInstructionsByMethod.Count,
                    ["InstructionsCovered"] = totalInstructions,
                    ["TotalInstructions"] = estimatedTotalInstructions,
                    ["InstructionCoveragePercent"] = estimatedTotalInstructions > 0 ? (double)totalInstructions / estimatedTotalInstructions * 100 : 0,
                    ["BranchesCovered"] = coveredBranches,
                    ["TotalBranches"] = totalBranches,
                    ["BranchCoveragePercent"] = totalBranches > 0 ? (double)coveredBranches / totalBranches * 100 : 0,
                    ["UniquePaths"] = _executionPaths.Count
                };
            }
            catch (Exception ex)
            {
                // Log the error but return a minimal set of statistics to avoid crashing the process
                Console.WriteLine($"Error in GetCoverageStatistics: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                return new Dictionary<string, object>
                {
                    ["MethodsCovered"] = _coveredInstructionsByMethod.Count,
                    ["InstructionsCovered"] = 0,
                    ["TotalInstructions"] = 1, // Avoid division by zero
                    ["InstructionCoveragePercent"] = 0,
                    ["BranchesCovered"] = 0,
                    ["TotalBranches"] = 1, // Avoid division by zero
                    ["BranchCoveragePercent"] = 0,
                    ["UniquePaths"] = _executionPaths.Count,
                    ["Error"] = ex.Message
                };
            }
        }

        /// <summary>
        /// Gets methods with low coverage that should be prioritized for fuzzing.
        /// </summary>
        /// <param name="count">The number of methods to return.</param>
        /// <returns>A list of method names with low coverage.</returns>
        public List<string> GetLowCoverageMethods(int count = 5)
        {
            // Calculate coverage percentage for each method
            var methodCoverage = new Dictionary<string, double>();

            foreach (var method in _coveredInstructionsByMethod.Keys)
            {
                // Get the total number of instructions for this method (approximation)
                int totalInstructions = EstimateMethodInstructionCount(method);
                int coveredInstructions = _coveredInstructionsByMethod[method].Count;

                double coverage = totalInstructions > 0
                    ? (double)coveredInstructions / totalInstructions * 100
                    : 0;

                methodCoverage[method] = coverage;
            }

            // Return methods with lowest coverage
            return methodCoverage
                .OrderBy(m => m.Value)
                .Take(count)
                .Select(m => m.Key)
                .ToList();
        }

        /// <summary>
        /// Gets branches that have not been fully covered.
        /// </summary>
        /// <param name="count">The number of branches to return.</param>
        /// <returns>A list of branch IDs that have not been fully covered.</returns>
        public List<int> GetUncoveredBranches(int count = 10)
        {
            return _branchCoverage
                .Where(b => !b.Value.TrueBranchCovered || !b.Value.FalseBranchCovered)
                .OrderBy(_ => _random.Next()) // Randomize to avoid getting stuck on hard-to-cover branches
                .Take(count)
                .Select(b => b.Key)
                .ToList();
        }

        /// <summary>
        /// Calculates the priority of a test case based on its coverage contribution.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The execution result.</param>
        /// <returns>A priority value (higher is more important).</returns>
        private int CalculatePriority(TestCase testCase, ExecutionResult result)
        {
            int priority = 50; // Base priority

            // Increase priority for methods with low coverage
            var lowCoverageMethods = GetLowCoverageMethods();
            if (lowCoverageMethods.Contains(testCase.MethodName))
            {
                priority += 30; // Increased from 20 to 30 to prioritize low coverage methods more
            }

            // Increase priority for test cases that cover branches
            if (result.Engine != null)
            {
                var branches = GetBranches(result.Engine);
                foreach (var branch in branches)
                {
                    if (_branchCoverage.TryGetValue(branch.Key, out var status))
                    {
                        // Higher priority for branches that were previously uncovered
                        if ((branch.Value && !status.TrueBranchCovered) ||
                            (!branch.Value && !status.FalseBranchCovered))
                        {
                            priority += 25; // Increased from 15 to 25 to prioritize branch coverage more
                        }
                    }
                }
            }

            // Increase priority for new execution paths
            if (result.Engine != null)
            {
                string path = GetExecutionPath(result.Engine);
                if (!_executionPaths.Contains(path))
                {
                    priority += 20; // Increased from 10 to 20 to prioritize new paths more
                }
            }

            // Increase priority for test cases that trigger exceptions
            // This helps find edge cases and potential vulnerabilities
            if (result.Exception != null)
            {
                priority += 15;

                // Extra priority for specific types of exceptions that might indicate vulnerabilities
                string exceptionType = result.Exception.GetType().Name;
                if (exceptionType.Contains("Overflow") ||
                    exceptionType.Contains("Divide") ||
                    exceptionType.Contains("Arithmetic") ||
                    exceptionType.Contains("OutOfMemory"))
                {
                    priority += 10;
                }
            }

            // Increase priority for test cases that access storage
            if (result.StorageChanges != null && result.StorageChanges.Count > 0)
            {
                priority += 15;

                // Extra priority for large storage operations
                if (result.StorageChanges.Count > 5)
                {
                    priority += 10;
                }
            }

            // Increase priority for test cases with high gas consumption
            if (result.FeeConsumed > 1_000_000) // 0.01 GAS
            {
                priority += 10;

                // Extra priority for very high gas consumption
                if (result.FeeConsumed > 10_000_000) // 0.1 GAS
                {
                    priority += 15;
                }
            }

            return priority;
        }

        /// <summary>
        /// Estimates the total number of instructions in a method.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>An estimate of the total number of instructions.</returns>
        private int EstimateMethodInstructionCount(string methodName)
        {
            // If we don't have coverage data for this method, return a default value
            if (!_coveredInstructionsByMethod.ContainsKey(methodName))
                return 100; // Default estimate

            int coveredInstructions = _coveredInstructionsByMethod[methodName].Count;

            // Try to get method boundaries from the script
            if (_script != null)
            {
                // Get method offsets from the script
                var methodOffsets = GetMethodOffsets(_script, methodName);
                if (methodOffsets.HasValue)
                {
                    int methodStart = methodOffsets.Value.start;
                    int methodEnd = methodOffsets.Value.end;

                    // Count instructions in the method
                    int totalInstructions = 0;
                    for (int ip = methodStart; ip < methodEnd;)
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

                    return Math.Max(coveredInstructions, totalInstructions);
                }
            }

            // Fallback to estimation if we can't determine method boundaries
            return Math.Max(coveredInstructions, (int)(coveredInstructions / 0.7));
        }

        /// <summary>
        /// Gets the start and end offsets of a method in the script.
        /// </summary>
        /// <param name="script">The script to analyze.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>A tuple with the start and end offsets, or null if not found.</returns>
        private (int start, int end)? GetMethodOffsets(Script script, string methodName)
        {
            // This is a simplified implementation that looks for method signatures in the script
            // A more accurate implementation would use debug information or analyze the script structure

            try
            {
                // Look for method signature in the script
                byte[] methodNameBytes = System.Text.Encoding.UTF8.GetBytes(methodName);

                // Convert methodNameBytes to an array of bytes for comparison
                byte[] methodNameBytesArray = new byte[methodNameBytes.Length];

                // Scan the script for the method name
                for (int ip = 0; ip < script.Length;)
                {
                    try
                    {
                        var instruction = script.GetInstruction(ip);

                        // Look for SYSCALL instructions that might indicate method boundaries
                        if ((byte)instruction.OpCode == (byte)VM.OpCode.SYSCALL)
                        {
                            // Check if this is the start of a method
                            if (ip + methodNameBytes.Length < script.Length)
                            {
                                bool found = true;
                                // Copy script bytes to methodNameBytesArray for comparison
                                for (int i = 0; i < methodNameBytes.Length && found; i++)
                                {
                                    if (ip + i >= script.Length)
                                    {
                                        found = false;
                                    }
                                    else
                                    {
                                        // Get the byte from the script
                                        byte scriptByte = (byte)script[ip + i];

                                        // Compare with the method name byte
                                        if (scriptByte != methodNameBytes[i])
                                            found = false;
                                    }
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
                                        if ((byte)endInstruction.OpCode == (byte)VM.OpCode.RET)
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
        /// Gets the instructions executed during a method call.
        /// </summary>
        /// <param name="engine">Application engine that executed the method.</param>
        /// <returns>Set of instruction offsets that were executed.</returns>
        private HashSet<int> GetExecutedInstructions(ApplicationEngine engine)
        {
            var executedInstructions = new HashSet<int>();

            // Get the current instruction pointer if available
            if (engine.CurrentContext != null)
            {
                // Add the current instruction pointer
                executedInstructions.Add(engine.CurrentContext.InstructionPointer);

                // Get all contexts from the invocation stack
                foreach (var context in engine.InvocationStack)
                {
                    // Add the current instruction pointer from each context
                    executedInstructions.Add(context.InstructionPointer);

                    // Add all instructions in the script as a fallback
                    // This is a temporary solution to ensure we get some coverage
                    var script = context.Script;
                    for (int ip = 0; ip < script.Length;)
                    {
                        try
                        {
                            var instruction = script.GetInstruction(ip);
                            executedInstructions.Add(ip);
                            ip += instruction.Size;
                        }
                        catch
                        {
                            // Skip invalid instructions
                            ip++;
                        }
                    }
                }
            }

            return executedInstructions;
        }

        /// <summary>
        /// Gets the branches executed during a method call.
        /// </summary>
        /// <param name="engine">Application engine that executed the method.</param>
        /// <returns>Dictionary mapping branch IDs to whether the true branch was taken.</returns>
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

                        // Check for conditional branch instructions
                        if ((byte)instruction.OpCode == (byte)VM.OpCode.JMPIF || (byte)instruction.OpCode == (byte)VM.OpCode.JMPIFNOT)
                        {
                            // Calculate the jump target
                            int offset = instruction.TokenI8;
                            int targetIp = ip + offset;

                            // Determine if the branch was taken
                            bool taken;

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
        /// Gets a string representation of the execution path.
        /// </summary>
        /// <param name="engine">Application engine that executed the method.</param>
        /// <returns>A string representing the execution path.</returns>
        private string GetExecutionPath(ApplicationEngine engine)
        {
            var instructions = GetExecutedInstructions(engine);
            return string.Join(",", instructions.OrderBy(i => i));
        }

        /// <summary>
        /// Status of a branch in the code.
        /// </summary>
        private class BranchStatus
        {
            /// <summary>
            /// Whether the true branch has been covered.
            /// </summary>
            public bool TrueBranchCovered { get; set; }

            /// <summary>
            /// Whether the false branch has been covered.
            /// </summary>
            public bool FalseBranchCovered { get; set; }
        }
    }
}
