using System;
using Neo.VM;
using Neo.VM.Types;
using System.Collections.Generic;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    /// <summary>
    /// Interface for a symbolic execution state.
    /// </summary>
    public interface ISymbolicState
    {
        /// <summary>
        /// Gets the instruction pointer.
        /// </summary>
        int InstructionPointer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the state is halted.
        /// </summary>
        bool IsHalted { get; set; }

        /// <summary>
        /// Gets the reason for halting the execution.
        /// </summary>
        VMState HaltReason { get; set; }

        /// <summary>
        /// Gets the path constraints for this state.
        /// </summary>
        IList<PathConstraint> PathConstraints { get; }

        /// <summary>
        /// Gets or sets the current execution depth.
        /// </summary>
        int ExecutionDepth { get; set; }

        /// <summary>
        /// Gets the execution trace.
        /// </summary>
        IList<ExecutionStep> ExecutionTrace { get; }

        /// <summary>
        /// Gets or sets symbolic storage.
        /// </summary>
        Dictionary<object, object> Storage { get; }

        /// <summary>
        /// Gets the evaluation stack.
        /// </summary>
        Stack<SymbolicValue> EvaluationStack { get; }

        /// <summary>
        /// Gets the alternative stack.
        /// </summary>
        Stack<SymbolicValue> AltStack { get; }

        /// <summary>
        /// Gets the script associated with this state.
        /// </summary>
        ReadOnlyMemory<byte> Script { get; }

        // --- Evaluation Stack Operations ---

        /// <summary>
        /// Pushes a value onto the evaluation stack.
        /// </summary>
        /// <param name="value">The value to push.</param>
        void Push(SymbolicValue value);

        /// <summary>
        /// Pops a value from the evaluation stack.
        /// </summary>
        /// <returns>The popped value.</returns>
        SymbolicValue Pop();

        /// <summary>
        /// Peeks at a value on the evaluation stack without removing it.
        /// </summary>
        /// <param name="index">The zero-based index from the top of the stack (0 is the top item).</param>
        /// <returns>The value at the specified index.</returns>
        SymbolicValue Peek(int index = 0);

        // --- State Management ---

        /// <summary>
        /// Gets the current instruction from the script.
        /// </summary>
        /// <param name="script">The script object.</param>
        /// <returns>The current instruction, or null if the pointer is invalid or an error occurs.</returns>
        Instruction? CurrentInstruction(Script script);

        /// <summary>
        /// Adds a path constraint to this state.
        /// </summary>
        /// <param name="constraint">The constraint to add.</param>
        void AddConstraint(PathConstraint constraint);

        /// <summary>
        /// Creates a deep clone of the current state.
        /// </summary>
        /// <returns>A new <see cref="ISymbolicState"/> instance that is a clone of the current state.</returns>
        ISymbolicState Clone();
    }
}
