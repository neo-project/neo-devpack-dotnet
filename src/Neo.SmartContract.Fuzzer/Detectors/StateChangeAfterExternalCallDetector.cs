using Neo.SmartContract.Fuzzer.Extensions;
using Neo.SmartContract.Fuzzer.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Detectors
{
    /// <summary>
    /// Detects state changes after external calls, which could indicate reentrancy vulnerabilities.
    /// </summary>
    public class StateChangeAfterExternalCallDetector : ExecutionVulnerabilityDetector
    {
        /// <inheritdoc/>
        public override string Name => "State Change After External Call Detector";

        /// <inheritdoc/>
        public override string Description => "Detects state changes after external calls, which could indicate reentrancy vulnerabilities.";

        /// <inheritdoc/>
        public override List<IssueReport> DetectVulnerabilities(TestCase testCase, ExecutionResult result)
        {
            var issues = new List<IssueReport>();

            // Check if there were external calls
            if (result.ExternalCalls != null && result.ExternalCalls.Count > 0)
            {
                // Check if there were storage changes after external calls
                bool stateChangeAfterExternalCall = false;
                long lastExternalCallTime = 0;

                foreach (var call in result.ExternalCalls)
                {
                    lastExternalCallTime = Math.Max(lastExternalCallTime, call.Timestamp);
                }

                if (result.StorageChanges != null && result.StorageChanges.Count > 0)
                {
                    foreach (var change in result.StorageChanges)
                    {
                        if (change.Value.Timestamp > lastExternalCallTime)
                        {
                            stateChangeAfterExternalCall = true;
                            break;
                        }
                    }
                }

                if (stateChangeAfterExternalCall)
                {
                    var issue = IssueReport.FromExecutionResult(
                        testCase,
                        result,
                        "State Change After External Call",
                        IssueSeverity.High,
                        $"The contract changed state after making external calls during execution of method {testCase.MethodName}, which could indicate a reentrancy vulnerability.");

                    issue.Remediation = "Follow the checks-effects-interactions pattern: perform all state changes before making external calls, or use reentrancy guards.";
                    issue.AdditionalInfo["ExternalCalls"] = string.Join(", ", result.ExternalCalls.Select(c => c.Target));

                    issues.Add(issue);
                }
            }

            return issues;
        }
    }
}
