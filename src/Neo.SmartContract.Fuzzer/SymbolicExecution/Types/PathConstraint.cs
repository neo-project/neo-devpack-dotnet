using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a constraint on an execution path in symbolic execution.
    /// </summary>
    public class PathConstraint
    {
        /// <summary>
        /// Gets the symbolic expression representing the constraint.
        /// </summary>
        public SymbolicExpression Expression { get; }

        /// <summary>
        /// Gets the instruction pointer where the constraint was added.
        /// </summary>
        public int InstructionPointer { get; }

        /// <summary>
        /// Gets or sets whether this constraint is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the PathConstraint class.
        /// </summary>
        /// <param name="expression">The symbolic expression representing the constraint.</param>
        /// <param name="instructionPointer">The instruction pointer where the constraint was added.</param>
        public PathConstraint(SymbolicExpression expression, int instructionPointer)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            InstructionPointer = instructionPointer;
        }

        /// <summary>
        /// Returns a string representation of the constraint.
        /// </summary>
        /// <returns>A string representation of the constraint.</returns>
        public override string ToString()
        {
            return $"{Expression} @ IP={InstructionPointer}";
        }
    }
}
