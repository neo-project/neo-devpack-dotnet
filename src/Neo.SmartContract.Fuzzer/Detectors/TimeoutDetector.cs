using Neo.SmartContract.Fuzzer.Extensions;
using Neo.SmartContract.Fuzzer.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Detectors
{
    /// <summary>
    /// Detects potential infinite loops or excessive computation that could lead to timeouts.
    /// </summary>
    public class TimeoutDetector : ExecutionVulnerabilityDetector
    {
        private readonly long _instructionThreshold;
        private readonly long _timeThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutDetector"/> class.
        /// </summary>
        /// <param name="instructionThreshold">The instruction count threshold above which to report an issue.</param>
        /// <param name="timeThreshold">The execution time threshold (in milliseconds) above which to report an issue.</param>
        public TimeoutDetector(long instructionThreshold = 10000, long timeThreshold = 1000)
        {
            _instructionThreshold = instructionThreshold;
            _timeThreshold = timeThreshold;
        }

        /// <inheritdoc/>
        public override string Name => "Timeout Detector";

        /// <inheritdoc/>
        public override string Description => $"Detects potential infinite loops or excessive computation that could lead to timeouts.";

        /// <inheritdoc/>
        public override List<IssueReport> DetectVulnerabilities(TestCase testCase, ExecutionResult result)
        {
            var issues = new List<IssueReport>();

            // Check for high instruction count
            if (result.InstructionCount > _instructionThreshold)
            {
                var issue = IssueReport.FromExecutionResult(
                    testCase,
                    result,
                    "High Instruction Count",
                    IssueSeverity.Medium,
                    $"The contract executed {result.InstructionCount} instructions during execution of method {testCase.MethodName}, which is above the threshold of {_instructionThreshold}.");

                issue.Remediation = "Review the code for potential infinite loops or excessive computation. Consider adding early exit conditions or optimizing algorithms.";
                issue.AdditionalInfo["InstructionCount"] = result.InstructionCount.ToString();

                issues.Add(issue);
            }

            // Check for long execution time
            if (result.ExecutionTime.TotalMilliseconds > _timeThreshold)
            {
                var issue = IssueReport.FromExecutionResult(
                    testCase,
                    result,
                    "Long Execution Time",
                    IssueSeverity.Medium,
                    $"The contract took {result.ExecutionTime.TotalMilliseconds:F2} ms to execute method {testCase.MethodName}, which is above the threshold of {_timeThreshold} ms.");

                issue.Remediation = "Review the code for potential infinite loops or excessive computation. Consider adding early exit conditions or optimizing algorithms.";
                issue.AdditionalInfo["ExecutionTime"] = result.ExecutionTime.ToString();

                issues.Add(issue);
            }

            // Check for timeout
            if (result.TimedOut)
            {
                var issue = IssueReport.FromExecutionResult(
                    testCase,
                    result,
                    "Execution Timeout",
                    IssueSeverity.High,
                    $"The contract execution timed out during execution of method {testCase.MethodName}.");

                issue.Remediation = "Review the code for potential infinite loops or excessive computation. Consider adding early exit conditions or optimizing algorithms.";

                issues.Add(issue);
            }

            return issues;
        }
    }
}
