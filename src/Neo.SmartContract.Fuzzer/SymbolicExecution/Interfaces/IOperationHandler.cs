using Neo.VM;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    /// <summary>
    /// Interface for a handler that executes VM operations.
    /// </summary>
    public interface IOperationHandler
    {
        /// <summary>
        /// Executes an operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode);
    }
}
