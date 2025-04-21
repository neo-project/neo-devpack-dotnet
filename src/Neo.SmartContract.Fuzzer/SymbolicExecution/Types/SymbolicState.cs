using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents the state of a symbolic execution at a specific point.
    /// </summary>
    public class SymbolicState : ISymbolicState
    {
        private readonly Stack<SymbolicValue> _evaluationStack = new Stack<SymbolicValue>();
        private readonly Stack<SymbolicValue> _altStack = new Stack<SymbolicValue>();
        private readonly List<PathConstraint> _pathConstraints = new List<PathConstraint>();
        private readonly List<ExecutionStep> _executionTrace = new List<ExecutionStep>();
        private readonly ReadOnlyMemory<byte> _script;
        private SymbolicSlot _slot;

        /// <summary>
        /// Gets or sets the instruction pointer.
        /// </summary>
        public int InstructionPointer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the state is halted.
        /// </summary>
        public bool IsHalted { get; set; }

        /// <summary>
        /// Gets or sets the reason why execution halted, if applicable.
        /// </summary>
        public VMState HaltReason { get; set; }

        /// <summary>
        /// Gets the path constraints for this state.
        /// </summary>
        public IList<PathConstraint> PathConstraints => _pathConstraints;

        /// <summary>
        /// Gets or sets the current execution depth.
        /// </summary>
        public int ExecutionDepth { get; set; }

        /// <summary>
        /// Gets the execution trace.
        /// </summary>
        public IList<ExecutionStep> ExecutionTrace => _executionTrace;

        /// <summary>
        /// Gets the evaluation stack.
        /// </summary>
        public Stack<SymbolicValue> EvaluationStack => _evaluationStack;

        /// <summary>
        /// Gets the alt stack.
        /// </summary>
        public Stack<SymbolicValue> AltStack => _altStack;

        /// <summary>
        /// Gets or sets the symbolic storage.
        /// Note: ISymbolicState requires Dictionary<object, object>. This requires careful handling.
        /// For now, returning a new dictionary potentially losing symbolic info.
        /// TODO: Revisit storage representation to properly handle ISymbolicState requirement.
        /// </summary>
        public Dictionary<object, object> Storage
        {
            get
            {
                // This is a potential loss of symbolic information and needs review.
                // Converting SymbolicValue to object might not be the correct approach.
                return _storage.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
            }
        }
        private readonly Dictionary<object, SymbolicValue> _storage = new Dictionary<object, SymbolicValue>();

        /// <summary>
        /// Gets the script being executed.
        /// </summary>
        public ReadOnlyMemory<byte> Script => _script;

        /// <summary>
        /// Gets the slot for local variables, arguments, and static fields.
        /// </summary>
        public SymbolicSlot Slot => _slot;

        /// <summary>
        /// Gets a unique identifier for this state, useful for debugging.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicState"/> class.
        /// </summary>
        /// <param name="script">The script byte memory.</param>
        /// <param name="instructionPointer">The initial instruction pointer.</param>
        /// <param name="executionDepth">The initial execution depth.</param>
        /// <param name="staticFieldCount">The number of static fields.</param>
        /// <param name="localVariableCount">The number of local variables.</param>
        /// <param name="argumentCount">The number of arguments.</param>
        public SymbolicState(ReadOnlyMemory<byte> script, int instructionPointer = 0, int executionDepth = 0, int staticFieldCount = 0, int localVariableCount = 0, int argumentCount = 0)
        {
            _script = script;
            InstructionPointer = instructionPointer;
            ExecutionDepth = executionDepth;
            IsHalted = false;
            HaltReason = VMState.NONE;
            _slot = new SymbolicSlot(staticFieldCount, localVariableCount, argumentCount);
        }

        // Private constructor for cloning
        private SymbolicState(SymbolicState source)
        {
            _script = source._script;
            InstructionPointer = source.InstructionPointer;
            IsHalted = source.IsHalted;
            HaltReason = source.HaltReason;
            ExecutionDepth = source.ExecutionDepth;
            Id = source.Id;

            // Deep copy value types and immutable types
            _evaluationStack = new Stack<SymbolicValue>(source._evaluationStack.Reverse());
            _altStack = new Stack<SymbolicValue>(source._altStack.Reverse());
            _pathConstraints = new List<PathConstraint>(source._pathConstraints.Select(c => c));
            _storage = new Dictionary<object, SymbolicValue>(source._storage);
            _executionTrace = new List<ExecutionStep>(source._executionTrace);
            _slot = source._slot?.Clone();
        }

        /// <summary>
        /// Pushes a value onto the evaluation stack.
        /// </summary>
        /// <param name="value">The value to push.</param>
        public void Push(SymbolicValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            _evaluationStack.Push(value);
        }

        /// <summary>
        /// Pops a value from the evaluation stack.
        /// </summary>
        /// <returns>The popped value.</returns>
        public SymbolicValue Pop()
        {
            if (_evaluationStack.Count == 0)
                throw new InvalidOperationException("Stack underflow");
            return _evaluationStack.Pop();
        }

        /// <summary>
        /// Peeks at the top value on the evaluation stack.
        /// </summary>
        /// <param name="index">The distance from the top.</param>
        /// <returns>The top value.</returns>
        public SymbolicValue Peek(int index = 0)
        {
            if (index < 0 || index >= _evaluationStack.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Peek index is out of bounds.");
            if (index == 0)
            {
                if (_evaluationStack.Count == 0)
                    throw new InvalidOperationException("Evaluation stack is empty.");
                return _evaluationStack.Peek();
            }
            else
            {
                return _evaluationStack.ElementAt(index);
            }
        }

        /// <summary>
        /// Gets the current instruction from the script.
        /// </summary>
        /// <param name="script">The script object.</param>
        /// <returns>The current instruction, or null if the pointer is invalid or an error occurs.</returns>
        public Instruction? CurrentInstruction(Script script)
        {
            if (script == null)
            {
                Halt(VMState.FAULT); // Halt if script is null
                return null;
            }
            // Check bounds using the script Length property
            if (InstructionPointer < 0 || InstructionPointer >= script.Length)
            {
                // Halt the state if IP is out of bounds
                Halt(VMState.HALT); // Use HALT instead of FAULT for out-of-bounds IP
                return null; // Return null as no valid instruction can be fetched
            }

            // Use the Script object's GetInstruction method
            try
            {
                return script.GetInstruction(InstructionPointer);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Instruction pointer is out of bounds
                Halt(VMState.HALT); // Use HALT instead of FAULT for out-of-bounds IP
                return null;
            }
            catch (Exception ex)
            {
                // Other unexpected errors during instruction fetch
                // Log the exception details here
                Console.WriteLine($"Error fetching instruction at IP {InstructionPointer}: {ex.Message}");
                Halt(VMState.HALT); // Use HALT instead of FAULT for instruction fetch errors
                return null;
            }
        }

        /// <summary>
        /// Halts the execution with the specified reason.
        /// </summary>
        /// <param name="reason">The reason for halting.</param>
        public void Halt(VMState reason)
        {
            if (!IsHalted)
            {
                IsHalted = true;
                HaltReason = reason;
            }
        }

        /// <summary>
        /// Adds a path constraint to this state.
        /// </summary>
        /// <param name="constraint">The constraint to add.</param>
        public void AddConstraint(PathConstraint constraint)
        {
            _pathConstraints.Add(constraint ?? throw new ArgumentNullException(nameof(constraint)));
        }

        /// <summary>
        /// Adds a record of an executed step to the trace.
        /// </summary>
        /// <param name="step">The execution step to add.</param>
        public void AddExecutionStep(ExecutionStep step)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            _executionTrace.Add(step);

            // Check execution depth limit
            if (ExecutionDepth > 1000) // Example limit, configure appropriately
            {
                Halt(VMState.FAULT);
                Console.WriteLine("Execution depth limit exceeded.");
            }
        }

        /// <summary>
        /// Creates a deep clone of this state.
        /// </summary>
        /// <returns>A new state that is a deep clone of this one.</returns>
        public ISymbolicState Clone()
        {
            return new SymbolicState(this);
        }

        /// <summary>
        /// Gets a value indicating whether the execution has halted due to an integer overflow.
        /// TODO: This needs a more reliable mechanism, perhaps a specific HaltReason or flag set during the operation.
        /// </summary>
        public bool IsFaultedByIntegerOverflow => HaltReason == VMState.FAULT; // Simplistic check, needs improvement
    }
}
