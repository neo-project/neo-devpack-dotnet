using Neo.VM;
using System.Collections.Generic;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic operation in the execution trace.
    /// </summary>
    public class SymbolicOperation
    {
        /// <summary>
        /// Gets or sets the OpCode of the operation.
        /// </summary>
        public OpCode OpCode { get; set; }

        /// <summary>
        /// Gets or sets the method name if this is a method call.
        /// </summary>
        public string? MethodName { get; set; }

        /// <summary>
        /// Gets or sets the syscall hash if this is a syscall.
        /// </summary>
        public uint Syscall { get; set; }

        /// <summary>
        /// Gets or sets the arguments for this operation.
        /// </summary>
        public List<SymbolicValue>? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the context for this operation.
        /// </summary>
        public string? Context { get; set; }
    }
}
