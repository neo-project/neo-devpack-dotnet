using Neo.VM;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Fuzzer.Tests
{
    /// <summary>
    /// Helper methods for creating test objects.
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Creates a new SymbolicState with an empty script.
        /// </summary>
        /// <returns>A new SymbolicState.</returns>
        public static SymbolicState CreateSymbolicState()
        {
            return new SymbolicState(new ReadOnlyMemory<byte>(new byte[0]));
        }

        /// <summary>
        /// Creates a new TestExecutionStep for testing.
        /// </summary>
        /// <param name="opCode">The OpCode for the instruction.</param>
        /// <returns>A new TestExecutionStep.</returns>
        public static TestExecutionStep CreateExecutionStep(OpCode opCode)
        {
            // Create a script with the opcode and enough padding for any operands
            byte[] scriptBytes = new byte[21]; // Max instruction size is 21 bytes (PUSHDATA4 + 4 bytes length + 16 bytes data)
            scriptBytes[0] = (byte)opCode;

            // Fill the rest with zeros (for operands)
            for (int i = 1; i < scriptBytes.Length; i++)
            {
                scriptBytes[i] = 0;
            }

            var instruction = new VM.Script(scriptBytes).GetInstruction(0);

            var step = new TestExecutionStep(
                instruction,
                new List<object>(),
                new List<object>(),
                new List<PathConstraint>(),
                new List<int>(),
                0
            );

            return step;
        }

        /// <summary>
        /// Creates a new PathConstraint for testing.
        /// </summary>
        /// <param name="expression">The symbolic expression.</param>
        /// <returns>A new PathConstraint.</returns>
        public static PathConstraint CreatePathConstraint(SymbolicExpression expression)
        {
            return new PathConstraint(expression, 0);
        }
    }
}
