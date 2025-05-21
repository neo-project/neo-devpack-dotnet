using Neo.SmartContract.Fuzzer.Extensions;
using Neo.SmartContract.Fuzzer.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Detectors
{
    /// <summary>
    /// Detects potential access control vulnerabilities in smart contracts.
    /// </summary>
    public class AccessControlDetector : ExecutionVulnerabilityDetector
    {
        private readonly HashSet<string> _sensitiveMethodPrefixes = new HashSet<string>
        {
            "admin", "owner", "set", "update", "upgrade", "change", "modify", "delete", "remove", "withdraw", "transfer"
        };

        /// <inheritdoc/>
        public override string Name => "Access Control Detector";

        /// <inheritdoc/>
        public override string Description => "Detects potential access control vulnerabilities in smart contracts.";

        /// <inheritdoc/>
        public override List<IssueReport> DetectVulnerabilities(TestCase testCase, ExecutionResult result)
        {
            var issues = new List<IssueReport>();

            // Check if the method name suggests it should be restricted
            bool isSensitiveMethod = _sensitiveMethodPrefixes.Any(prefix => 
                testCase.MethodName.ToLowerInvariant().StartsWith(prefix));

            if (isSensitiveMethod)
            {
                // Check if there was a witness check before making changes
                bool hasWitnessCheck = result.WitnessChecks != null && result.WitnessChecks.Count > 0;
                
                // Check if there were storage changes
                bool hasStorageChanges = result.StorageChanges != null && result.StorageChanges.Count > 0;

                if (hasStorageChanges && !hasWitnessCheck && result.Success)
                {
                    var issue = IssueReport.FromExecutionResult(
                        testCase,
                        result,
                        "Missing Access Control",
                        IssueSeverity.High,
                        $"The sensitive method {testCase.MethodName} made storage changes without performing witness checks.");

                    issue.Remediation = "Add appropriate access control checks using Runtime.CheckWitness() to restrict access to sensitive methods.";
                    issue.AdditionalInfo["StorageChanges"] = result.StorageChanges.Count.ToString();

                    issues.Add(issue);
                }
            }

            return issues;
        }
    }
}
