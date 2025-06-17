using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.SmartContract.Fuzzer.Tests
{
    /// <summary>
    /// A test-specific implementation of ExecutionStep that allows setting properties for testing.
    /// </summary>
    public class TestExecutionStep : ExecutionStep
    {
        /// <summary>
        /// Gets or sets the OpCode for testing.
        /// </summary>
        public new OpCode Opcode { get; set; }

        /// <summary>
        /// Gets or sets the stack before the operation for testing.
        /// </summary>
        public new List<object> StackBefore { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets the stack after the operation for testing.
        /// </summary>
        public new List<object> StackAfter { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets the instruction pointer for testing.
        /// </summary>
        public new int InstructionPointer { get; set; }

        /// <summary>
        /// Gets or sets the instruction for testing.
        /// </summary>
        public TestInstruction TestInstruction { get; set; }

        /// <summary>
        /// Creates a new TestExecutionStep.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <param name="stackBefore">The stack before the operation.</param>
        /// <param name="stackAfter">The stack after the operation.</param>
        /// <param name="constraints">The constraints.</param>
        /// <param name="path">The path.</param>
        /// <param name="instructionPointer">The instruction pointer.</param>
        public TestExecutionStep(VM.Instruction instruction, IReadOnlyList<object> stackBefore, IReadOnlyList<object> stackAfter, IReadOnlyList<PathConstraint> constraints, IReadOnlyList<int> path, int instructionPointer)
            : base(instruction, stackBefore, stackAfter, constraints, path, instructionPointer)
        {
            Opcode = instruction.OpCode;
            StackBefore = new List<object>(stackBefore);
            StackAfter = new List<object>(stackAfter);
            InstructionPointer = instructionPointer;
            TestInstruction = new TestInstruction(instruction);
        }
    }

    /// <summary>
    /// A test-specific implementation of Instruction that allows setting properties for testing.
    /// </summary>
    public class TestInstruction
    {
        private readonly VM.Instruction _instruction;

        /// <summary>
        /// Gets or sets the TokenI32 for testing.
        /// </summary>
        public int TokenI32 { get; set; }

        /// <summary>
        /// Gets or sets the TokenU32 for testing.
        /// </summary>
        public uint TokenU32 { get; set; }

        /// <summary>
        /// Creates a new TestInstruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        public TestInstruction(VM.Instruction instruction)
        {
            _instruction = instruction;
            // Only set TokenI32 and TokenU32 for instructions that have them
            if (instruction.OpCode == OpCode.SYSCALL || instruction.OpCode == OpCode.CALL)
            {
                TokenI32 = 0; // Default value
                TokenU32 = 0; // Default value
            }
        }
    }
}
