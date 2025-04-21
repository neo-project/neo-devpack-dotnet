using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.VM;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Base class for operation handlers.
    /// </summary>
    public abstract class BaseOperations : IOperationHandler
    {
        /// <summary>
        /// The symbolic execution engine.
        /// </summary>
        protected readonly ISymbolicExecutionEngine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        protected BaseOperations(ISymbolicExecutionEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        /// <summary>
        /// Executes an operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public abstract bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode);

        /// <summary>
        /// Logs debug information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected void LogDebug(string message)
        {
            _engine.LogDebug(message);
        }

        /// <summary>
        /// Advances the instruction pointer by the size of the instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        protected void AdvanceInstructionPointer(Instruction instruction)
        {
            _engine.CurrentState.InstructionPointer += instruction.Size;
        }
    }
}
