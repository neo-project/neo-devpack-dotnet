using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System.Collections.Generic;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Detects integer overflow vulnerabilities during symbolic execution.
    /// </summary>
    public class IntegerOverflowDetector : ISymbolicVulnerabilityDetector
    {
        /// <summary>
        /// Gets the name of the detector.
        /// </summary>
        public string Name => "Integer Overflow Detector";

        /// <summary>
        /// Gets the description of the detector.
        /// </summary>
        public string Description => "Detects integer overflow vulnerabilities during symbolic execution.";

        /// <summary>
        /// Detects integer overflow vulnerabilities in a symbolic execution path.
        /// </summary>
        /// <param name="path">The symbolic execution path.</param>
        /// <returns>A list of vulnerabilities found.</returns>
        public List<SymbolicVulnerability> DetectVulnerabilities(ExecutionPath path)
        {
            var vulnerabilities = new List<SymbolicVulnerability>();

            // Check for integer overflow in binary expressions
            foreach (var constraint in path.PathConstraints)
            {
                if (constraint.Expression is SymbolicBinaryExpression binaryExpr)
                {
                    if (IsArithmeticOperator(binaryExpr.Operator) && binaryExpr.Type == Neo.VM.Types.StackItemType.Integer)
                    {
                        vulnerabilities.Add(new SymbolicVulnerability
                        {
                            Type = "Integer Overflow",
                            Severity = Types.SymbolicVulnerabilitySeverity.High,
                            Description = $"Potential integer overflow in arithmetic operation: {binaryExpr}",
                            Remediation = "Use checked arithmetic or add bounds checks to prevent integer overflow."
                        });
                    }
                }
            }

            return vulnerabilities;
        }

        private bool IsArithmeticOperator(Types.Operator op)
        {
            return op.Equals(Types.Operator.Add) || op.Equals(Types.Operator.Subtract) || op.Equals(Types.Operator.Multiply) || op.Equals(Types.Operator.Divide);
        }
    }
}
