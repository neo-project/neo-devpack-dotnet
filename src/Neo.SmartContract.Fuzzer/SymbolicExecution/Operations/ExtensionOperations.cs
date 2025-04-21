using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all extension operations in the symbolic virtual machine.
    /// This class is responsible for operations like ABORTMSG and ASSERTMSG.
    /// </summary>
    public class ExtensionOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public ExtensionOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes an extension operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.ABORTMSG:
                    return HandleAbortMsg();
                case OpCode.ASSERTMSG:
                    return HandleAssertMsg();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the ABORTMSG operation, which aborts execution with a message.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleAbortMsg()
        {
            // Pop the message from the stack
            var message = _engine.CurrentState.Pop();

            if (message == null)
            {
                LogDebug("ABORTMSG: Stack underflow");
                return false;
            }

            // In symbolic execution, we don't actually abort execution
            // Instead, we create a fork for the abort path
            var abortState = _engine.CurrentState.Clone();
            ((SymbolicState)abortState).Halt(VMState.FAULT);
            _engine.AddPendingState(abortState);

            // We also create a fork for the non-abort path
            var nonAbortState = _engine.CurrentState.Clone();
            _engine.AddPendingState(nonAbortState);

            LogDebug($"Created forks for abort and non-abort paths with message: {message}");
            return true;
        }

        /// <summary>
        /// Handles the ASSERTMSG operation, which asserts a condition with a message.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleAssertMsg()
        {
            // Pop the message and condition from the stack
            var message = _engine.CurrentState.Pop();
            var condition = _engine.CurrentState.Pop();

            if (message == null || condition == null)
            {
                LogDebug("ASSERTMSG: Stack underflow");
                return false;
            }

            // In symbolic execution, we don't actually assert the condition
            // Instead, we create a fork for the assert success path and another for the assert failure path

            // First, create a fork for the assert success path
            var successState = _engine.CurrentState.Clone();
            _engine.AddPendingState(successState);

            // Then, create a fork for the assert failure path
            var failureState = _engine.CurrentState.Clone();
            ((SymbolicState)failureState).Halt(VMState.FAULT);
            _engine.AddPendingState(failureState);

            LogDebug($"Created forks for assert success and failure paths with message: {message}");
            return true;
        }
    }
}
