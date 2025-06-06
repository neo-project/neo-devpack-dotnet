using Neo.VM;
using System;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Event arguments for a VM step.
    /// </summary>
    public class StepEventArgs : EventArgs
    {
        /// <summary>
        /// The VM opcode that was executed.
        /// </summary>
        public OpCode OpCode { get; }

        /// <summary>
        /// The current instruction pointer.
        /// </summary>
        public int InstructionPointer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepEventArgs"/> class.
        /// </summary>
        /// <param name="opCode">The VM opcode that was executed.</param>
        /// <param name="instructionPointer">The current instruction pointer.</param>
        public StepEventArgs(OpCode opCode, int instructionPointer)
        {
            OpCode = opCode;
            InstructionPointer = instructionPointer;
        }
    }
}
