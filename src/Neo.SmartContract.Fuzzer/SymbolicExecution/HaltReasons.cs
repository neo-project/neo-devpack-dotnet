using Neo.VM;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Provides extension methods and constants for handling halt reasons in symbolic execution.
    /// This addresses the gap between Neo.VM.VMState enum and the extended halt reason semantics 
    /// needed for symbolic execution.
    /// </summary>
    public static class HaltReasons
    {
        /// <summary>
        /// Represents a normal (successful) termination. Maps to VMState.HALT.
        /// </summary>
        public static readonly VMState Normal = VMState.HALT;

        /// <summary>
        /// Represents a fault termination. Maps to VMState.FAULT.
        /// </summary>
        public static readonly VMState Fault = VMState.FAULT;

        /// <summary>
        /// Represents an abort termination caused by the ABORT opcode.
        /// </summary>
        public static readonly VMState Abort = (VMState)4;

        /// <summary>
        /// Represents a throw termination caused by the THROW opcode.
        /// </summary>
        public static readonly VMState Throw = (VMState)5;

        /// <summary>
        /// Represents a termination due to reaching the maximum allowed steps.
        /// </summary>
        public static readonly VMState MaxStepsReached = (VMState)6;

        /// <summary>
        /// Determines if a VMState represents an abort termination.
        /// </summary>
        /// <param name="state">The VMState to check.</param>
        /// <returns>True if the state represents an abort, false otherwise.</returns>
        public static bool IsAbort(this VMState state) => state == Abort;

        /// <summary>
        /// Determines if a VMState represents a throw termination.
        /// </summary>
        /// <param name="state">The VMState to check.</param>
        /// <returns>True if the state represents a throw, false otherwise.</returns>
        public static bool IsThrow(this VMState state) => state == Throw;

        /// <summary>
        /// Determines if a VMState represents a normal termination.
        /// </summary>
        /// <param name="state">The VMState to check.</param>
        /// <returns>True if the state represents a normal termination, false otherwise.</returns>
        public static bool IsNormal(this VMState state) => state == Normal;

        /// <summary>
        /// Determines if a VMState represents a fault termination.
        /// </summary>
        /// <param name="state">The VMState to check.</param>
        /// <returns>True if the state represents a fault, false otherwise.</returns>
        public static bool IsFault(this VMState state) => state == Fault;

        /// <summary>
        /// Determines if a VMState represents a max steps reached termination.
        /// </summary>
        /// <param name="state">The VMState to check.</param>
        /// <returns>True if the state represents a max steps reached termination, false otherwise.</returns>
        public static bool IsMaxStepsReached(this VMState state) => state == MaxStepsReached;
    }
}
