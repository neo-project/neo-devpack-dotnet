using Neo.VM;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a complete execution path through a smart contract.
    /// </summary>
    public class ExecutionPath
    {
        /// <summary>
        /// Gets the execution steps in this path.
        /// </summary>
        public IReadOnlyList<ExecutionStep> Steps { get; }

        /// <summary>
        /// Gets the path constraints that define this execution path.
        /// </summary>
        public IReadOnlyList<PathConstraint> PathConstraints { get; }

        /// <summary>
        /// The reason the execution halted (e.g., HALT, FAULT).
        /// </summary>
        public VMState HaltReason { get; private set; } = VMState.NONE; // Default to NONE initially

        /// <summary>
        /// Gets the final instruction pointer for this execution path.
        /// </summary>
        public int FinalInstructionPointer { get; }

        /// <summary>
        /// Gets the final stack for this execution path.
        /// </summary>
        public IReadOnlyList<object> FinalStack { get; }

        /// <summary>
        /// Gets the final symbolic state for this execution path.
        /// </summary>
        public SymbolicState FinalState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this path terminated normally.
        /// </summary>
        public bool TerminatedNormally => HaltReason == VMState.HALT;

        /// <summary>
        /// Gets a dictionary of concrete variable assignments that satisfy the path constraints.
        /// </summary>
        public IReadOnlyDictionary<string, object> SatisfyingInputs { get; }

        /// <summary>
        /// Gets or sets the storage changes made during this execution path.
        /// </summary>
        public Dictionary<byte[], byte[]> StorageChanges { get; set; } = new Dictionary<byte[], byte[]>();

        /// <summary>
        /// Gets or sets the events emitted during this execution path.
        /// </summary>
        public List<string> Events { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the vulnerabilities found on this path.
        /// </summary>
        public List<SymbolicVulnerability> Vulnerabilities { get; set; } = new List<SymbolicVulnerability>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionPath"/> class.
        /// </summary>
        /// <param name="steps">The execution steps.</param>
        /// <param name="pathConstraints">The path constraints.</param>
        /// <param name="haltReason">The halt reason.</param>
        /// <param name="finalInstructionPointer">The final instruction pointer.</param>
        /// <param name="finalStack">The final stack.</param>
        /// <param name="finalState">The final symbolic state.</param>
        /// <param name="satisfyingInputs">Concrete variable assignments that satisfy the path constraints.</param>
        public ExecutionPath(
            IEnumerable<ExecutionStep> steps,
            IEnumerable<PathConstraint> pathConstraints,
            VMState haltReason,
            int finalInstructionPointer,
            IEnumerable<object> finalStack,
            SymbolicState? finalState = null,
            IReadOnlyDictionary<string, object>? satisfyingInputs = null)
        {
            Steps = steps?.ToList() ?? new List<ExecutionStep>();
            PathConstraints = pathConstraints?.ToList() ?? new List<PathConstraint>();
            HaltReason = haltReason;
            FinalInstructionPointer = finalInstructionPointer;
            FinalStack = finalStack?.ToList() ?? new List<object>();
            FinalState = finalState ?? new SymbolicState(new ReadOnlyMemory<byte>(new byte[0]));
            SatisfyingInputs = satisfyingInputs ?? new Dictionary<string, object>();
            StorageChanges = new Dictionary<byte[], byte[]>();
            Events = new List<string>();
            Vulnerabilities = new List<SymbolicVulnerability>();
        }

        /// <summary>
        /// Creates an execution path from a symbolic state.
        /// </summary>
        /// <param name="state">The symbolic state.</param>
        /// <param name="executionTrace">The execution trace.</param>
        /// <param name="satisfyingInputs">Concrete variable assignments that satisfy the path constraints.</param>
        /// <returns>A new execution path.</returns>
        public static ExecutionPath FromSymbolicState(
            ISymbolicState state,
            IEnumerable<ExecutionStep> executionTrace,
            IReadOnlyDictionary<string, object>? satisfyingInputs = null)
        {
            SymbolicState? symbolicState = state as SymbolicState;
            return new ExecutionPath(
                executionTrace,
                state.PathConstraints,
                state.HaltReason,
                state.InstructionPointer,
                symbolicState?.EvaluationStack.Select(item => (object)item).ToList() ?? new List<object>(),
                symbolicState,
                satisfyingInputs);
        }

        /// <summary>
        /// Finalizes the execution path, setting the halt reason.
        /// </summary>
        /// <param name="reason">The reason execution halted.</param>
        public void FinalizePath(VMState reason)
        {
            if (HaltReason == VMState.NONE) // Only set if not already set
            {
                HaltReason = reason;
            }
        }

        /// <summary>
        /// Returns a string representation of this execution path.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"Path with {Steps.Count} steps, halted with {HaltReason}";
        }
    }
}

