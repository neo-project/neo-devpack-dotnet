using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Detects storage exhaustion vulnerabilities during symbolic execution.
    /// </summary>
    public class StorageExhaustionDetector : ISymbolicVulnerabilityDetector
    {
        /// <summary>
        /// Gets the name of the detector.
        /// </summary>
        public string Name => "Storage Exhaustion Detector";

        /// <summary>
        /// Gets the description of the detector.
        /// </summary>
        public string Description => "Detects storage exhaustion vulnerabilities during symbolic execution.";

        /// <summary>
        /// Detects storage exhaustion vulnerabilities in a symbolic execution path.
        /// </summary>
        /// <param name="path">The symbolic execution path.</param>
        /// <returns>A list of vulnerabilities found.</returns>
        public List<SymbolicVulnerability> DetectVulnerabilities(ExecutionPath path)
        {
            var vulnerabilities = new List<SymbolicVulnerability>();

            // Check for large storage keys
            foreach (var key in path.StorageChanges.Keys)
            {
                if (key.Length > 64)
                {
                    vulnerabilities.Add(new SymbolicVulnerability
                    {
                        Type = "Large Storage Key",
                        Severity = SymbolicVulnerabilitySeverity.Medium,
                        Description = $"Large storage key detected ({key.Length} bytes), which could lead to high gas consumption.",
                        Remediation = "Use smaller storage keys to reduce gas consumption. Consider using a hash of the data instead of the full data as a key."
                    });
                }
            }

            // Check for large storage values
            foreach (var value in path.StorageChanges.Values)
            {
                if (value.Length > 1024)
                {
                    vulnerabilities.Add(new SymbolicVulnerability
                    {
                        Type = "Large Storage Value",
                        Severity = SymbolicVulnerabilitySeverity.Medium,
                        Description = $"Large storage value detected ({value.Length} bytes), which could lead to high gas consumption.",
                        Remediation = "Use smaller storage values to reduce gas consumption. Consider splitting large data into multiple storage entries or using a different storage strategy."
                    });
                }
            }

            // Check for many storage operations
            if (path.StorageChanges.Count > 10)
            {
                vulnerabilities.Add(new SymbolicVulnerability
                {
                    Type = "Many Storage Operations",
                    Severity = SymbolicVulnerabilitySeverity.Low,
                    Description = $"Many storage operations detected ({path.StorageChanges.Count}), which could lead to high gas consumption.",
                    Remediation = "Reduce the number of storage operations to improve performance and reduce gas consumption."
                });
            }

            return vulnerabilities;
        }
    }
}
