using System.Collections.Generic;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.VM;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    /// <summary>
    /// Interface for a symbolic execution engine.
    /// </summary>
    public interface ISymbolicExecutionEngine
    {
        /// <summary>
        /// Gets the current state of the symbolic execution.
        /// </summary>
        ISymbolicState CurrentState { get; }

        /// <summary>
        /// Gets the execution engine limits.
        /// </summary>
        ExecutionEngineLimits Limits { get; }

        /// <summary>
        /// Gets the pending states queue.
        /// </summary>
        Queue<ISymbolicState> PendingStates { get; }

        /// <summary>
        /// Executes the symbolic engine and returns the result.
        /// </summary>
        /// <returns>The symbolic execution result.</returns>
        SymbolicExecutionResult Execute();

        /// <summary>
        /// Executes a single step in the current state.
        /// </summary>
        void ExecuteStep();

        /// <summary>
        /// Forks the current state with additional constraints.
        /// </summary>
        /// <param name="constraints">The additional constraints.</param>
        /// <returns>A new symbolic state.</returns>
        ISymbolicState ForkState(IEnumerable<PathConstraint> constraints);

        /// <summary>
        /// Adds a state to the pending queue.
        /// </summary>
        /// <param name="state">The state to add.</param>
        void AddPendingState(ISymbolicState state);

        /// <summary>
        /// Logs debug information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogDebug(string message);
    }
}
