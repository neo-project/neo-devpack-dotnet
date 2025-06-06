using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a single step in the symbolic execution.
    /// </summary>
    public class ExecutionStep
    {
        /// <summary>
        /// Gets the instruction pointer for this step.
        /// </summary>
        public int InstructionPointer { get; }

        /// <summary>
        /// Gets the opcode executed in this step.
        /// </summary>
        public OpCode Opcode => Instruction.OpCode;

        /// <summary>
        /// Gets the full instruction executed in this step.
        /// </summary>
        public Instruction Instruction { get; }

        /// <summary>
        /// Gets the stack before this execution step.
        /// </summary>
        public IReadOnlyList<object> StackBefore { get; }

        /// <summary>
        /// Gets the stack after this execution step.
        /// </summary>
        public IReadOnlyList<object> StackAfter { get; }

        /// <summary>
        /// Gets any path constraints added during this step.
        /// </summary>
        public IReadOnlyList<PathConstraint> AddedConstraints { get; }

        /// <summary>
        /// Gets any branches created during this step.
        /// </summary>
        public IReadOnlyList<int> CreatedBranches { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionStep"/> class.
        /// </summary>
        /// <param name="instruction">The instruction executed.</param>
        /// <param name="stackBefore">The stack before execution.</param>
        /// <param name="stackAfter">The stack after execution.</param>
        /// <param name="addedConstraints">Constraints added during this step.</param>
        /// <param name="createdBranches">Branches created during this step.</param>
        public ExecutionStep(
            Instruction instruction,
            IEnumerable<object> stackBefore,
            IEnumerable<object> stackAfter,
            IReadOnlyList<PathConstraint>? addedConstraints = null,
            IReadOnlyList<int>? createdBranches = null,
            int instructionPointer = 0)
        {
            Instruction = instruction;
            InstructionPointer = instructionPointer;
            StackBefore = stackBefore?.ToList().AsReadOnly() ?? new List<object>().AsReadOnly();
            StackAfter = stackAfter?.ToList().AsReadOnly() ?? new List<object>().AsReadOnly();
            AddedConstraints = addedConstraints ?? new List<PathConstraint>().AsReadOnly();
            CreatedBranches = createdBranches ?? new List<int>().AsReadOnly();
        }

        /// <summary>
        /// Returns a string representation of this execution step.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"IP: {InstructionPointer}, Op: {Instruction.OpCode}";
        }
    }
}
