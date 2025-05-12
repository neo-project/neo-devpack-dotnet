using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Detects division by zero vulnerabilities during symbolic execution.
    /// </summary>
    public class DivisionByZeroDetector : ISymbolicVulnerabilityDetector
    {
        /// <summary>
        /// Gets the name of the detector.
        /// </summary>
        public string Name => "Division by Zero Detector";

        /// <summary>
        /// Gets the description of the detector.
        /// </summary>
        public string Description => "Detects division by zero vulnerabilities during symbolic execution.";

        /// <summary>
        /// Detects division by zero vulnerabilities in a symbolic execution path.
        /// </summary>
        /// <param name="path">The symbolic execution path.</param>
        /// <returns>A list of vulnerabilities found.</returns>
        public List<SymbolicVulnerability> DetectVulnerabilities(ExecutionPath path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var vulnerabilities = new List<SymbolicVulnerability>();

            // Check for division by zero in binary expressions in path constraints
            if (path.PathConstraints != null)
            {
                foreach (var constraint in path.PathConstraints)
                {
                    if (constraint?.Expression == null) continue;

                    CheckExpressionForDivisionByZero(constraint.Expression, vulnerabilities, constraint.InstructionPointer);
                }
            }

            // Check for division by zero in execution steps
            if (path.Steps != null)
            {
                foreach (var step in path.Steps)
                {
                    if (step?.Instruction == null) continue;

                    // Check for DIV, MOD, or similar opcodes
                    if (IsDivisionOpcode(step.Instruction.OpCode))
                    {
                        // Check if the divisor could be zero
                        if (step.StackBefore != null && step.StackBefore.Count >= 2)
                        {
                            var divisor = step.StackBefore[step.StackBefore.Count - 1]; // Top of stack is divisor
                            if (divisor is SymbolicValue symValue && IsZeroOrPotentiallyZero(symValue))
                            {
                                vulnerabilities.Add(new SymbolicVulnerability
                                {
                                    Type = "Division by Zero",
                                    Severity = Types.SymbolicVulnerabilitySeverity.High,
                                    Description = $"Potential division by zero at instruction pointer {step.InstructionPointer}",
                                    Location = step.InstructionPointer.ToString(),
                                    Remediation = GetDivisionByZeroRemediation()
                                });
                            }
                        }
                    }
                }
            }

            // Check for division by zero in symbolic expressions in the final state
            if (path.FinalState != null)
            {
                // Check the evaluation stack in the final state
                if (path.FinalState.EvaluationStack != null)
                {
                    foreach (var item in path.FinalState.EvaluationStack)
                    {
                        if (item is SymbolicValue symValue)
                        {
                            CheckExpressionForDivisionByZero(symValue, vulnerabilities, null);
                        }
                    }
                }
            }

            return vulnerabilities;
        }

        /// <summary>
        /// Recursively checks an expression for division by zero vulnerabilities.
        /// </summary>
        private void CheckExpressionForDivisionByZero(SymbolicValue expr, List<SymbolicVulnerability> vulnerabilities, int? instructionPointer)
        {
            if (expr == null) return;

            // Check if this is a binary expression with division or modulo
            if (expr is SymbolicExpression binaryExpr)
            {
                if ((binaryExpr.Operator.Equals(Types.Operator.Divide) || binaryExpr.Operator.Equals(Types.Operator.Modulo)) &&
                    IsZeroOrPotentiallyZero(binaryExpr.Right))
                {
                    string location = instructionPointer.HasValue ? $" at instruction pointer {instructionPointer}" : "";

                    vulnerabilities.Add(new SymbolicVulnerability
                    {
                        Type = "Division by Zero",
                        Severity = Types.SymbolicVulnerabilitySeverity.High,
                        Description = $"Potential division by zero{location}: {binaryExpr}",
                        Location = instructionPointer?.ToString() ?? "Unknown",
                        Remediation = GetDivisionByZeroRemediation()
                    });
                }

                // Recursively check both sides of the expression
                CheckExpressionForDivisionByZero(binaryExpr.Left, vulnerabilities, instructionPointer);
                CheckExpressionForDivisionByZero(binaryExpr.Right, vulnerabilities, instructionPointer);
            }
        }

        /// <summary>
        /// Determines if a value is zero or could potentially be zero.
        /// </summary>
        private bool IsZeroOrPotentiallyZero(SymbolicValue expr)
        {
            if (expr == null) return true;

            // Check for concrete zero values
            if (expr is ConstantValue constant)
            {
                if (constant.Value is long longVal && longVal == 0)
                    return true;

                if (constant.Value is int intVal && intVal == 0)
                    return true;

                if (constant.Value is BigInteger bigIntVal && bigIntVal == BigInteger.Zero)
                    return true;

                if (constant.Value is byte byteVal && byteVal == 0)
                    return true;

                // If it's a non-zero constant, it's safe
                return false;
            }

            // Check for symbolic expressions that might be constrained to be non-zero
            if (expr is SymbolicVariable symVar)
            {
                // If we have path constraints that ensure this variable is non-zero, return false
                // This would require access to the path constraints and constraint solver
                // For now, conservatively assume it could be zero
                return true;
            }

            // Check for expressions that are guaranteed to be non-zero
            if (expr is SymbolicExpression symExpr)
            {
                // Check for expressions like (x + 1) which can't be zero if x is non-negative
                if (symExpr.Operator.Equals(Types.Operator.Add) &&
                    symExpr.Right is ConstantValue rightConst &&
                    rightConst.Value is long rightLong && rightLong > 0)
                {
                    return false;
                }

                // Check for expressions like (x * 2) which can only be zero if x is zero
                if (symExpr.Operator.Equals(Types.Operator.Multiply) &&
                    ((symExpr.Right is ConstantValue rightMultConst &&
                      rightMultConst.Value is long rightMultLong && rightMultLong != 0) ||
                     (symExpr.Left is ConstantValue leftMultConst &&
                      leftMultConst.Value is long leftMultLong && leftMultLong != 0)))
                {
                    return IsZeroOrPotentiallyZero(symExpr.Left) || IsZeroOrPotentiallyZero(symExpr.Right);
                }
            }

            // For other types of symbolic values, conservatively assume they could be zero
            return true;
        }

        /// <summary>
        /// Determines if an opcode is a division or modulo operation.
        /// </summary>
        private bool IsDivisionOpcode(VM.OpCode opcode)
        {
            return opcode == VM.OpCode.DIV ||
                   opcode == VM.OpCode.MOD;
        }

        /// <summary>
        /// Gets remediation advice for division by zero vulnerabilities.
        /// </summary>
        private string GetDivisionByZeroRemediation()
        {
            return "To prevent division by zero vulnerabilities:\n" +
                   "1. Always check that divisors are not zero before performing division operations.\n" +
                   "2. Use conditional logic to handle the case where a divisor might be zero.\n" +
                   "3. Consider using a default value or throwing an exception when a divisor is zero.\n" +
                   "4. For user inputs that might be used as divisors, validate them before performing calculations.\n" +
                   "5. In Neo smart contracts, use ExecutionEngine.Assert() to verify divisors are non-zero.";
        }
    }
}
