using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.VM; // For VMState
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents the result of a symbolic execution.
    /// </summary>
    public class SymbolicExecutionResult
    {
        /// <summary>
        /// Gets the execution paths explored during symbolic execution.
        /// </summary>
        public IReadOnlyList<ExecutionPath> ExecutionPaths { get; }

        /// <summary>
        /// Gets the paths that terminated normally.
        /// </summary>
        public IEnumerable<ExecutionPath> NormalTerminationPaths =>
            ExecutionPaths.Where(p => p.HaltReason == VMState.HALT);

        /// <summary>
        /// Gets the paths that terminated with an abort.
        /// </summary>
        public IEnumerable<ExecutionPath> AbortTerminationPaths =>
            ExecutionPaths.Where(p => p.HaltReason == VMState.FAULT);

        /// <summary>
        /// Gets the paths that terminated with a throw.
        /// </summary>
        public IEnumerable<ExecutionPath> ThrowTerminationPaths =>
            ExecutionPaths.Where(p => p.HaltReason == VMState.FAULT);

        /// <summary>
        /// Gets the paths that terminated with a fault.
        /// </summary>
        public IEnumerable<ExecutionPath> FaultTerminationPaths =>
            ExecutionPaths.Where(p => p.HaltReason == VMState.FAULT);

        /// <summary>
        /// Gets the paths that reached the maximum execution depth.
        /// </summary>
        public IEnumerable<ExecutionPath> MaxDepthTerminationPaths =>
            ExecutionPaths.Where(p => p.HaltReason == VMState.BREAK);

        /// <summary>
        /// Gets a value indicating whether any path terminated normally.
        /// </summary>
        public bool HasNormalTermination => NormalTerminationPaths.Any();

        /// <summary>
        /// Gets the total number of paths explored.
        /// </summary>
        public int TotalPaths => ExecutionPaths.Count;

        /// <summary>
        /// Gets the timestamp when the execution was completed.
        /// </summary>
        public DateTime CompletionTime { get; }

        /// <summary>
        /// Gets the vulnerabilities detected during symbolic execution.
        /// </summary>
        public List<Vulnerability> Vulnerabilities { get; } = new List<Vulnerability>();

        /// <summary>
        /// Gets a value indicating whether any vulnerabilities were detected.
        /// </summary>
        public bool HasVulnerabilities => Vulnerabilities.Count > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicExecutionResult"/> class.
        /// </summary>
        /// <param name="executionPaths">The execution paths.</param>
        /// <param name="vulnerabilities">The vulnerabilities detected during execution.</param>
        public SymbolicExecutionResult(IEnumerable<ExecutionPath> executionPaths, IEnumerable<Vulnerability>? vulnerabilities = null)
        {
            ExecutionPaths = executionPaths?.ToList() ?? throw new ArgumentNullException(nameof(executionPaths));
            if (vulnerabilities != null)
            {
                Vulnerabilities.AddRange(vulnerabilities);
            }
            CompletionTime = DateTime.Now;
        }

        /// <summary>
        /// Gets statistics about the execution.
        /// </summary>
        /// <returns>A dictionary of statistics.</returns>
        public Dictionary<string, object> GetStatistics()
        {
            return new Dictionary<string, object>
            {
                ["TotalPaths"] = TotalPaths,
                ["NormalTerminations"] = NormalTerminationPaths.Count(),
                ["AbortTerminations"] = AbortTerminationPaths.Count(),
                ["ThrowTerminations"] = ThrowTerminationPaths.Count(),
                ["FaultTerminations"] = FaultTerminationPaths.Count(),
                ["MaxDepthTerminations"] = MaxDepthTerminationPaths.Count(),
                ["AveragePathLength"] = ExecutionPaths.Count > 0
                    ? ExecutionPaths.Average(p => p.Steps.Count)
                    : 0,
                ["MaxPathLength"] = ExecutionPaths.Count > 0
                    ? ExecutionPaths.Max(p => p.Steps.Count)
                    : 0,
                ["MinPathLength"] = ExecutionPaths.Count > 0
                    ? ExecutionPaths.Min(p => p.Steps.Count)
                    : 0,
                ["CompletionTime"] = CompletionTime
            };
        }

        /// <summary>
        /// Gets a summary of the execution result.
        /// </summary>
        /// <returns>A summary string.</returns>
        public string GetSummary()
        {
            var stats = GetStatistics();
            return $"Symbolic Execution completed at {CompletionTime}\n" +
                   $"Total Paths: {stats["TotalPaths"]}\n" +
                   $"Normal Terminations: {stats["NormalTerminations"]}\n" +
                   $"Abort Terminations: {stats["AbortTerminations"]}\n" +
                   $"Throw Terminations: {stats["ThrowTerminations"]}\n" +
                   $"Fault Terminations: {stats["FaultTerminations"]}\n" +
                   $"Max Depth Terminations: {stats["MaxDepthTerminations"]}\n" +
                   $"Average Path Length: {stats["AveragePathLength"]}\n" +
                   $"Max Path Length: {stats["MaxPathLength"]}\n" +
                   $"Min Path Length: {stats["MinPathLength"]}";
        }

        /// <summary>
        /// Creates a summary of the execution result including vulnerabilities.
        /// </summary>
        /// <returns>A summary string.</returns>
        public string CreateSummary()
        {
            var summary = GetSummary();

            if (HasVulnerabilities)
            {
                summary += "\n\nVulnerabilities Detected:\n";
                var vulnerabilitiesByType = Vulnerabilities
                    .GroupBy(v => v.Type)
                    .OrderBy(g => g.Key);

                foreach (var group in vulnerabilitiesByType)
                {
                    summary += $"  {group.Key}: {group.Count()} instance(s)\n";
                }
            }
            else
            {
                summary += "\n\nNo vulnerabilities detected.";
            }

            return summary;
        }

        /// <summary>
        /// Gets all satisfying inputs across all feasible paths.
        /// </summary>
        /// <returns>A dictionary of path index to satisfying inputs.</returns>
        public Dictionary<int, Dictionary<string, object>> GetAllSatisfyingInputs()
        {
            var result = new Dictionary<int, Dictionary<string, object>>();

            for (int i = 0; i < ExecutionPaths.Count; i++)
            {
                var path = ExecutionPaths[i];
                if (path.SatisfyingInputs.Count > 0)
                {
                    result[i] = new Dictionary<string, object>(path.SatisfyingInputs);
                }
            }

            return result;
        }
    }
}
