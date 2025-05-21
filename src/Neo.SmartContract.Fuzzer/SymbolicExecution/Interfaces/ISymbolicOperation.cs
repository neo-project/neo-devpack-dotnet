using Neo.VM;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    /// <summary>
    /// Interface for operation handlers in the symbolic virtual machine.
    /// Each implementation handles a specific set of NeoVM opcodes.
    /// </summary>
    public interface ISymbolicOperation
    {
        /// <summary>
        /// Attempts to execute an operation based on the provided opcode.
        /// </summary>
        /// <param name="vm">The symbolic virtual machine instance.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        bool ExecuteOperation(SymbolicVirtualMachine vm, OpCode opcode);
    }
}
