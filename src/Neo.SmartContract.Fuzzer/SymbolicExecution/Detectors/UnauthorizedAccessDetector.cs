using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Detects unauthorized access vulnerabilities during symbolic execution.
    /// </summary>
    public class UnauthorizedAccessDetector : ISymbolicVulnerabilityDetector
    {
        /// <summary>
        /// Gets the name of the detector.
        /// </summary>
        public string Name => "Unauthorized Access Detector";

        /// <summary>
        /// Gets the description of the detector.
        /// </summary>
        public string Description => "Detects unauthorized access vulnerabilities during symbolic execution.";

        /// <summary>
        /// Detects unauthorized access vulnerabilities in a symbolic execution path.
        /// </summary>
        /// <param name="path">The symbolic execution path.</param>
        /// <returns>A list of vulnerabilities found.</returns>
        public List<SymbolicVulnerability> DetectVulnerabilities(ExecutionPath path)
        {
            var vulnerabilities = new List<SymbolicVulnerability>();

            // Check if the path has storage changes but no witness checks
            if (path.StorageChanges.Count > 0 && !HasWitnessCheck(path))
            {
                vulnerabilities.Add(new SymbolicVulnerability
                {
                    Type = "Missing Witness Check",
                    Severity = SymbolicVulnerabilitySeverity.High,
                    Description = "Storage is modified without a witness check, which could allow unauthorized access.",
                    Remediation = "Add Runtime.CheckWitness() calls to verify the identity of the caller before modifying storage."
                });
            }

            return vulnerabilities;
        }

        private bool HasWitnessCheck(ExecutionPath path)
        {
            // Check if any of the events indicate a witness check
            return path.Events.Any(e => e.Contains("CheckWitness") || e.Contains("GetCallingScriptHash"));
        }
    }
}
