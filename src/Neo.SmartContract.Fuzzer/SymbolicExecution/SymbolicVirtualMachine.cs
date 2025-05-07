using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
// using Neo.SmartContract.Framework; // Keep for potential attribute usage, but qualify OpCode
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Operations;
// Use specific using for Types to avoid conflict with SymbolicState in root
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types; // For SymbolicValue, PathConstraint, SymbolicExpression, SymbolicState (impl), ExecutionPath, ConstraintType
using Neo.VM; // For ExecutionEngineLimits, Script, TriggerType, HaltReason, Instruction, OpCode
using Neo.VM.Types; // For StackItem, BooleanStackItem, Integer
using System;
using System.Collections.Generic; // Use specific using for List<>
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    public class SymbolicVirtualMachine : ISymbolicExecutionEngine
    {
        private const int DefaultMaxExecutionDepth = 1000;
        private const int DefaultMaxPaths = 100;

        private readonly Script _script; // Reintroduce Script field
        private readonly TriggerType _trigger;
        private readonly IConstraintSolver _solver;
        private readonly IEvaluationService _evaluationService;
        private readonly int _maxExecutionDepth;
        private readonly int _maxPaths;
        private readonly ArithmeticOperations _arithmeticOps;
        private readonly BitwiseOperations _bitwiseOps;
        private readonly ComparisonOperations _comparisonOps;
        private readonly StackOperations _stackOps;
        private readonly FlowControlOperations _flowControlOps;
        private readonly SyscallOperations _syscallOps;
        private readonly CompoundTypeOperations _compoundTypeOps;
        private readonly SpliceOperations _spliceOps;
        private readonly TypeOperations _typeOps;
        private readonly SlotOperations _slotOps;
        private readonly AdvancedFlowControlOperations _advancedFlowControlOps;
        private readonly ExtendedStackOperations _extendedStackOps;
        private readonly ExtensionOperations _extensionOps;
        private readonly AdvancedArithmeticOperations _advancedArithmeticOps;

        // Dictionary to store previous state constraints for tracking changes
        private readonly Dictionary<int, HashSet<PathConstraint>> _previousStateConstraints = new Dictionary<int, HashSet<PathConstraint>>();

        // --- Interface Properties ---
        public ISymbolicState CurrentState { get; private set; }
        public ExecutionEngineLimits Limits { get; }
        public Queue<ISymbolicState> PendingStates { get; } = new Queue<ISymbolicState>();
        public System.Collections.Generic.List<ISymbolicState> CompletedPaths { get; } = new System.Collections.Generic.List<ISymbolicState>();

        // Revert constructor to accept Script script
        public SymbolicVirtualMachine(Script script, IConstraintSolver solver, IEvaluationService evaluationService, ExecutionEngineLimits limits, int maxExecutionDepth = DefaultMaxExecutionDepth, int maxPaths = DefaultMaxPaths)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script)); // Store Script object
            _trigger = TriggerType.Application;
            Limits = limits;
            _solver = solver ?? throw new ArgumentNullException(nameof(solver));
            _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
            _maxExecutionDepth = maxExecutionDepth;
            _maxPaths = maxPaths;

            // Pass required arguments to operation constructors
            _arithmeticOps = new ArithmeticOperations(this, _evaluationService);
            _bitwiseOps = new BitwiseOperations(this, _evaluationService);
            _comparisonOps = new ComparisonOperations(this);
            // Convert Script to byte array using implicit conversion to ReadOnlyMemory<byte>
            ReadOnlyMemory<byte> scriptMemory = _script;
            byte[] scriptBytes = scriptMemory.ToArray();
            _stackOps = new StackOperations(this, scriptBytes);
            _flowControlOps = new FlowControlOperations(this, scriptBytes);
            _syscallOps = new SyscallOperations(this, scriptBytes);
            _compoundTypeOps = new CompoundTypeOperations(this, scriptBytes);
            _spliceOps = new SpliceOperations(this, scriptBytes);
            _typeOps = new TypeOperations(this, scriptBytes);
            _slotOps = new SlotOperations(this, scriptBytes);
            _advancedFlowControlOps = new AdvancedFlowControlOperations(this, scriptBytes);
            _extendedStackOps = new ExtendedStackOperations(this, scriptBytes);
            _extensionOps = new ExtensionOperations(this, scriptBytes);
            _advancedArithmeticOps = new AdvancedArithmeticOperations(this, scriptBytes);

            // Pass _script directly, implicit conversion to ReadOnlyMemory<byte> should work
            var initialState = new Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicState(_script, 0, 0);
            CurrentState = initialState; // Initialize CurrentState
            PendingStates.Enqueue(initialState);
        }

        // --- Interface Methods ---
        public SymbolicExecutionResult Execute()
        {
            while (PendingStates.Count > 0)
            {
                ISymbolicState stateToProcess = PendingStates.Dequeue();
                CurrentState = stateToProcess;

                // Execute until the state halts or reaches max depth
                // Check for both HALT and FAULT (or any other halt reason)
                while (!((SymbolicState)CurrentState).IsHalted)
                {
                    if (CurrentState.ExecutionDepth > _maxExecutionDepth)
                    {
                        ((SymbolicState)CurrentState).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
                        CompletedPaths.Add(CurrentState);
                        break;
                    }

                    ExecuteStep();

                    if (((SymbolicState)CurrentState).IsHalted)
                    {
                        CompletedPaths.Add(CurrentState);
                    }
                }
            }
            // Convert ISymbolicState to ExecutionPath before returning
            // Map properties from the completed state to the ExecutionPath constructor
            var executionPaths = CompletedPaths.Select(state =>
            {
                // Cast to concrete type to access properties not on the interface
                var concreteState = (SymbolicState)state;
                return new ExecutionPath(
                    concreteState.ExecutionTrace,  // IEnumerable<ExecutionStep>
                    concreteState.PathConstraints, // IEnumerable<PathConstraint>
                    concreteState.HaltReason, // Use HaltReason property
                    concreteState.InstructionPointer, // int finalInstructionPointer
                    concreteState.EvaluationStack.Cast<object>(), // IEnumerable<object> finalStack (Use EvaluationStack and cast)
                    concreteState, // SymbolicState finalState
                    new Dictionary<string, object>() // IReadOnlyDictionary<string, object> satisfyingInputs - Using empty for now
                );
            }).ToList();
            return new SymbolicExecutionResult(executionPaths);
        }

        public void ExecuteStep()
        {
            if (CurrentState == null || ((SymbolicState)CurrentState).IsHalted) // Check IsHalted instead of HaltReason
            {
                LogDebug("Attempted to execute step with no active state or on a halted state.");
                return;
            }
            ExecuteStepInternal(CurrentState);
        }

        public ISymbolicState ForkState(IEnumerable<PathConstraint> constraints)
        {
            if (CurrentState == null) throw new InvalidOperationException("Cannot fork without a current state.");

            ISymbolicState newState = CurrentState.Clone();
            foreach (var constraint in constraints)
            {
                newState.AddConstraint(constraint);
            }

            if (_solver.IsSatisfiable(newState.PathConstraints))
            {
                AddPendingState(newState);
                LogDebug($"Forked new state at IP: {CurrentState.InstructionPointer} with constraints.");
            }
            else
            {
                LogDebug($"Forked state at IP: {CurrentState.InstructionPointer} discarded (unsatisfiable).");
            }
            return newState;
        }

        public void AddPendingState(ISymbolicState state)
        {
            PendingStates.Enqueue(state);
        }

        public void LogDebug(string message)
        {
            Console.WriteLine($"[DEBUG] {message}");
        }

        // --- Private Helper Methods ---
        private void ExecuteStepInternal(ISymbolicState state)
        {
            // Ensure state is not already halted
            if (((SymbolicState)state).IsHalted) return; // Check IsHalted instead of HaltReason

            // Get the current instruction using the Script object
            Instruction? instructionNullable = state.CurrentInstruction(_script);
            int currentInstructionPointer = state.InstructionPointer;

            // Capture stack state *before* execution
            List<object> stackBefore = new List<object>();
            if (state is SymbolicState concreteStateBefore) // Need concrete state for stack
            {
                stackBefore = concreteStateBefore.EvaluationStack.Cast<object>().ToList();
            }
            else
            {
                // Handle case where state is not the expected concrete type, maybe log or throw?
                LogDebug("Warning: ExecuteStepInternal received non-SymbolicState type. Cannot capture stack before.");
                // Optionally halt: state.Halt(VMState.FAULT);
            }

            // Check if getting the instruction caused the state to halt (e.g., invalid IP)
            // Note: CurrentInstruction likely sets IsHalted internally if IP is invalid
            if (((SymbolicState)state).IsHalted || instructionNullable is null) // Check IsHalted instead of HaltReason
            {
                return;
            }
            Instruction instruction = instructionNullable; // Revert direct assignment

            // Increment IP only if instruction is valid
            state.InstructionPointer = currentInstructionPointer + instruction.Size;

            // Increment Execution Depth
            ((SymbolicState)state).ExecutionDepth++;

            // --- Execute Instruction ---
            try
            {
                // Qualify ALL OpCode usages
                switch (instruction.OpCode)
                {
                    case Neo.VM.OpCode.PUSHINT8:
                    case Neo.VM.OpCode.PUSHINT16:
                    case Neo.VM.OpCode.PUSHINT32:
                    case Neo.VM.OpCode.PUSHINT64:
                    case Neo.VM.OpCode.PUSHINT128:
                    case Neo.VM.OpCode.PUSHINT256:
                    case Neo.VM.OpCode.PUSHT:
                    case Neo.VM.OpCode.PUSHF:
                    case Neo.VM.OpCode.PUSHA:
                    case Neo.VM.OpCode.PUSHNULL:
                    case Neo.VM.OpCode.PUSHDATA1:
                    case Neo.VM.OpCode.PUSHDATA2:
                    case Neo.VM.OpCode.PUSHDATA4:
                    case Neo.VM.OpCode.PUSHM1:
                    case Neo.VM.OpCode.PUSH0:
                    case Neo.VM.OpCode.PUSH1:
                    case Neo.VM.OpCode.PUSH2:
                    case Neo.VM.OpCode.PUSH3:
                    case Neo.VM.OpCode.PUSH4:
                    case Neo.VM.OpCode.PUSH5:
                    case Neo.VM.OpCode.PUSH6:
                    case Neo.VM.OpCode.PUSH7:
                    case Neo.VM.OpCode.PUSH8:
                    case Neo.VM.OpCode.PUSH9:
                    case Neo.VM.OpCode.PUSH10:
                    case Neo.VM.OpCode.PUSH11:
                    case Neo.VM.OpCode.PUSH12:
                    case Neo.VM.OpCode.PUSH13:
                    case Neo.VM.OpCode.PUSH14:
                    case Neo.VM.OpCode.PUSH15:
                    case Neo.VM.OpCode.PUSH16:
                        if (!_stackOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Stack) not handled by StackOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;
                    case Neo.VM.OpCode.NOP:
                    case Neo.VM.OpCode.DUP:
                    case Neo.VM.OpCode.SWAP:
                    case Neo.VM.OpCode.TUCK:
                    case Neo.VM.OpCode.OVER:
                    case Neo.VM.OpCode.ROT:
                    case Neo.VM.OpCode.DEPTH:
                    case Neo.VM.OpCode.DROP:
                    case Neo.VM.OpCode.PICK:
                    case Neo.VM.OpCode.ROLL: // Ensure NIP is NOT here if handled elsewhere
                        if (!_stackOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Stack) not handled by StackOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    case Neo.VM.OpCode.INC:
                    case Neo.VM.OpCode.DEC:
                    case Neo.VM.OpCode.NEGATE:
                    case Neo.VM.OpCode.ABS:
                    case Neo.VM.OpCode.ADD:
                    case Neo.VM.OpCode.SUB:
                    case Neo.VM.OpCode.MUL:
                    case Neo.VM.OpCode.DIV:
                    case Neo.VM.OpCode.MOD:
                    case Neo.VM.OpCode.SHL:
                    case Neo.VM.OpCode.SHR:
                    case Neo.VM.OpCode.SIGN:
                    case Neo.VM.OpCode.MIN:
                    case Neo.VM.OpCode.MAX:
                        if (!_arithmeticOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Arithmetic) not handled by ArithmeticOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    case Neo.VM.OpCode.INVERT:
                    case Neo.VM.OpCode.AND:
                    case Neo.VM.OpCode.OR:
                    case Neo.VM.OpCode.XOR:
                        if (!_bitwiseOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Bitwise) not handled by BitwiseOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;
                    case Neo.VM.OpCode.EQUAL:
                    case Neo.VM.OpCode.NOTEQUAL:
                    case Neo.VM.OpCode.LT:
                    case Neo.VM.OpCode.LE:
                    case Neo.VM.OpCode.GT:
                    case Neo.VM.OpCode.GE:
                    case Neo.VM.OpCode.BOOLAND:
                    case Neo.VM.OpCode.BOOLOR:
                    case Neo.VM.OpCode.NZ:
                    case Neo.VM.OpCode.WITHIN:
                    case Neo.VM.OpCode.NUMNOTEQUAL:
                        if (!_comparisonOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Comparison) not handled by ComparisonOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    case Neo.VM.OpCode.JMP:
                    case Neo.VM.OpCode.JMP_L:
                    case Neo.VM.OpCode.JMPIF:
                    case Neo.VM.OpCode.JMPIF_L:
                    case Neo.VM.OpCode.JMPIFNOT:
                    case Neo.VM.OpCode.JMPIFNOT_L:
                    case Neo.VM.OpCode.JMPEQ:
                    case Neo.VM.OpCode.JMPEQ_L:
                    case Neo.VM.OpCode.JMPNE:
                    case Neo.VM.OpCode.JMPNE_L:
                    case Neo.VM.OpCode.JMPGT:
                    case Neo.VM.OpCode.JMPGT_L:
                    case Neo.VM.OpCode.JMPGE:
                    case Neo.VM.OpCode.JMPGE_L:
                    case Neo.VM.OpCode.JMPLT:
                    case Neo.VM.OpCode.JMPLT_L:
                    case Neo.VM.OpCode.JMPLE:
                    case Neo.VM.OpCode.JMPLE_L:
                    case Neo.VM.OpCode.CALL:
                    case Neo.VM.OpCode.CALL_L:
                    case Neo.VM.OpCode.RET:
                        if (!_flowControlOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Flow Control) not handled by FlowControlOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;
                    case Neo.VM.OpCode.SYSCALL:
                        if (!_syscallOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Syscall) not handled by SyscallOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Compound Type Operations
                    case Neo.VM.OpCode.NEWARRAY0:
                    case Neo.VM.OpCode.NEWARRAY:
                    case Neo.VM.OpCode.NEWARRAY_T:
                    case Neo.VM.OpCode.NEWSTRUCT0:
                    case Neo.VM.OpCode.NEWSTRUCT:
                    case Neo.VM.OpCode.NEWMAP:
                    case Neo.VM.OpCode.PACKMAP:
                    case Neo.VM.OpCode.PACK:
                    case Neo.VM.OpCode.UNPACK:
                    case Neo.VM.OpCode.PACKSTRUCT:
                    case Neo.VM.OpCode.SIZE:
                    case Neo.VM.OpCode.HASKEY:
                    case Neo.VM.OpCode.KEYS:
                    case Neo.VM.OpCode.VALUES:
                    case Neo.VM.OpCode.PICKITEM:
                    case Neo.VM.OpCode.APPEND:
                    case Neo.VM.OpCode.SETITEM:
                    case Neo.VM.OpCode.REVERSEITEMS:
                    case Neo.VM.OpCode.REMOVE:
                    case Neo.VM.OpCode.CLEARITEMS:
                    case Neo.VM.OpCode.POPITEM:
                        if (!_compoundTypeOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Compound Type) not handled by CompoundTypeOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Splice Operations
                    case Neo.VM.OpCode.NEWBUFFER:
                    case Neo.VM.OpCode.MEMCPY:
                    case Neo.VM.OpCode.CAT:
                    case Neo.VM.OpCode.SUBSTR:
                    case Neo.VM.OpCode.LEFT:
                    case Neo.VM.OpCode.RIGHT:
                        if (!_spliceOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Splice) not handled by SpliceOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Type Operations
                    case Neo.VM.OpCode.ISNULL:
                    case Neo.VM.OpCode.ISTYPE:
                    case Neo.VM.OpCode.CONVERT:
                        if (!_typeOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Type) not handled by TypeOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Slot Operations
                    case Neo.VM.OpCode.INITSSLOT:
                    case Neo.VM.OpCode.INITSLOT:
                    case Neo.VM.OpCode.LDSFLD0:
                    case Neo.VM.OpCode.LDSFLD1:
                    case Neo.VM.OpCode.LDSFLD2:
                    case Neo.VM.OpCode.LDSFLD3:
                    case Neo.VM.OpCode.LDSFLD4:
                    case Neo.VM.OpCode.LDSFLD5:
                    case Neo.VM.OpCode.LDSFLD6:
                    case Neo.VM.OpCode.LDSFLD:
                    case Neo.VM.OpCode.STSFLD0:
                    case Neo.VM.OpCode.STSFLD1:
                    case Neo.VM.OpCode.STSFLD2:
                    case Neo.VM.OpCode.STSFLD3:
                    case Neo.VM.OpCode.STSFLD4:
                    case Neo.VM.OpCode.STSFLD5:
                    case Neo.VM.OpCode.STSFLD6:
                    case Neo.VM.OpCode.STSFLD:
                    case Neo.VM.OpCode.LDLOC0:
                    case Neo.VM.OpCode.LDLOC1:
                    case Neo.VM.OpCode.LDLOC2:
                    case Neo.VM.OpCode.LDLOC3:
                    case Neo.VM.OpCode.LDLOC4:
                    case Neo.VM.OpCode.LDLOC5:
                    case Neo.VM.OpCode.LDLOC6:
                    case Neo.VM.OpCode.LDLOC:
                    case Neo.VM.OpCode.STLOC0:
                    case Neo.VM.OpCode.STLOC1:
                    case Neo.VM.OpCode.STLOC2:
                    case Neo.VM.OpCode.STLOC3:
                    case Neo.VM.OpCode.STLOC4:
                    case Neo.VM.OpCode.STLOC5:
                    case Neo.VM.OpCode.STLOC6:
                    case Neo.VM.OpCode.STLOC:
                    case Neo.VM.OpCode.LDARG0:
                    case Neo.VM.OpCode.LDARG1:
                    case Neo.VM.OpCode.LDARG2:
                    case Neo.VM.OpCode.LDARG3:
                    case Neo.VM.OpCode.LDARG4:
                    case Neo.VM.OpCode.LDARG5:
                    case Neo.VM.OpCode.LDARG6:
                    case Neo.VM.OpCode.LDARG:
                    case Neo.VM.OpCode.STARG0:
                    case Neo.VM.OpCode.STARG1:
                    case Neo.VM.OpCode.STARG2:
                    case Neo.VM.OpCode.STARG3:
                    case Neo.VM.OpCode.STARG4:
                    case Neo.VM.OpCode.STARG5:
                    case Neo.VM.OpCode.STARG6:
                    case Neo.VM.OpCode.STARG:
                        if (!_slotOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Slot) not handled by SlotOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Advanced Flow Control Operations
                    case Neo.VM.OpCode.CALLA:
                    case Neo.VM.OpCode.CALLT:
                    case Neo.VM.OpCode.TRY:
                    case Neo.VM.OpCode.TRY_L:
                    case Neo.VM.OpCode.ENDTRY:
                    case Neo.VM.OpCode.ENDTRY_L:
                    case Neo.VM.OpCode.ENDFINALLY:
                        if (!_advancedFlowControlOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Advanced Flow Control) not handled by AdvancedFlowControlOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Extended Stack Operations
                    case Neo.VM.OpCode.NIP:
                    case Neo.VM.OpCode.XDROP:
                    case Neo.VM.OpCode.CLEAR:
                    case Neo.VM.OpCode.REVERSE3:
                    case Neo.VM.OpCode.REVERSE4:
                    case Neo.VM.OpCode.REVERSEN:
                        if (!_extendedStackOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Extended Stack) not handled by ExtendedStackOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Extension Operations
                    case Neo.VM.OpCode.ABORTMSG:
                    case Neo.VM.OpCode.ASSERTMSG:
                        if (!_extensionOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Extension) not handled by ExtensionOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Advanced Arithmetic Operations
                    case Neo.VM.OpCode.POW:
                    case Neo.VM.OpCode.SQRT:
                    case Neo.VM.OpCode.MODMUL:
                    case Neo.VM.OpCode.MODPOW:
                        if (!_advancedArithmeticOps.ExecuteOperation(this, instruction.OpCode))
                        {
                            LogDebug($"OpCode {instruction.OpCode} (Advanced Arithmetic) not handled by AdvancedArithmeticOperations.");
                            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                        }
                        break;

                    // Alt Stack Operations (Placeholders - Commented out if undefined)
                    // case Neo.VM.OpCode.TOALTSTACK:
                    // case Neo.VM.OpCode.TOALTSTACK: // Seems undefined in current OpCode enum
                    //     LogDebug($"Alt Stack OpCode {instruction.OpCode} not yet implemented. Halting.");
                    //     ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                    //     break;
                    // case Neo.VM.OpCode.FROMALTSTACK:
                    // case Neo.VM.OpCode.FROMALTSTACK: // Seems undefined
                    //     LogDebug($"Alt Stack OpCode {instruction.OpCode} not yet implemented. Halting.");
                    //     ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                    //     break;
                    // case Neo.VM.OpCode.DUPFROMALTSTACK:
                    // case Neo.VM.OpCode.DUPFROMALTSTACK: // Seems undefined
                    //     LogDebug($"Alt Stack OpCode {instruction.OpCode} not yet implemented. Halting.");
                    //     ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
                    //     break;

                    // Exception Handling
                    case Neo.VM.OpCode.THROW:
                        LogDebug("Execution explicitly THROWn.");
                        ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
                        break;
                    case Neo.VM.OpCode.ABORT:
                        LogDebug("Execution ABORTed.");
                        // Mark execution as halted with appropriate VMState
                        // Use the standardized VMState.FAULT value for consistency with the test
                        var symbolicState = (SymbolicState)state;
                        symbolicState.Halt(VMState.FAULT);

                        // Add the state to completed paths
                        CompletedPaths.Add(state);
                        break;
                    case Neo.VM.OpCode.ASSERT:
                        ExecuteAssert(state, currentInstructionPointer);
                        break;

                    default:
                        LogDebug($"OpCode {instruction.OpCode} not implemented.");
                        ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
                        break;
                }
            }
            catch (Exception ex)
            {
                LogDebug($"Exception during execution at IP {currentInstructionPointer}: {ex.Message}\n{ex.StackTrace}");
                ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
            }
            // --- End Instruction Execution ---

            // Capture stack state *after* execution
            List<object> stackAfter = new List<object>();

            // Capture added constraints and created branches for ExecutionStep
            List<PathConstraint> addedConstraints = new List<PathConstraint>();
            List<int> createdBranches = new List<int>();

            // If the state is a SymbolicState, get the constraints added in this step
            if (state is SymbolicState concreteStateForConstraints)
            {
                // Get constraints added since the last step
                int constraintCount = concreteStateForConstraints.PathConstraints.Count;

                // If we have a previous state snapshot, compare constraints
                if (_previousStateConstraints.TryGetValue(state.GetHashCode(), out var prevConstraints))
                {
                    // Find constraints that were added in this step
                    for (int i = 0; i < constraintCount; i++)
                    {
                        var constraint = concreteStateForConstraints.PathConstraints[i];
                        if (!prevConstraints.Contains(constraint))
                        {
                            addedConstraints.Add(constraint);
                        }
                    }
                }
                else if (constraintCount > 0)
                {
                    // If no previous state, assume the last constraint was added in this step
                    addedConstraints.Add(concreteStateForConstraints.PathConstraints[constraintCount - 1]);
                }

                // Update previous state constraints for next step
                _previousStateConstraints[state.GetHashCode()] = new HashSet<PathConstraint>(
                    concreteStateForConstraints.PathConstraints);

                // For branches, we would need to track which instruction pointers were created
                // during branch operations. This is a placeholder implementation.
                foreach (var pendingState in PendingStates)
                {
                    // If a state was added to the pending queue during this step,
                    // its instruction pointer would be a branch target
                    createdBranches.Add(pendingState.InstructionPointer);
                }
            }

            if (state is SymbolicState concreteStateAfter) // Need concrete state for stack
            {
                stackAfter = concreteStateAfter.EvaluationStack.Cast<object>().ToList();
            }
            else
            {
                LogDebug("Warning: ExecuteStepInternal received non-SymbolicState type. Cannot capture stack after.");
            }

            // Create and record the execution step
            var execStep = new ExecutionStep(
                instruction,
                stackBefore,
                stackAfter,
                addedConstraints,
                createdBranches,
                currentInstructionPointer
            );
            if (state is SymbolicState concreteStateFinal)
            {
                concreteStateFinal.AddExecutionStep(execStep); // Corrected: Cast state to SymbolicState
            }
            else
            {
                LogDebug("Error: State is not SymbolicState, cannot add execution step.");
                // Optionally halt if this is considered a critical error
                // state.Halt(VMState.FAULT);
            }
        }

        private void ExecuteAssert(ISymbolicState state, int instructionPointer)
        {
            var condition = state.Pop(); // Pop directly from state
            if (condition == null)
            {
                LogDebug("ASSERT failed: Stack empty.");
                ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
                return;
            }

            // Check if the condition is a concrete boolean 'true'
            if (condition is ConcreteValue<bool> concreteBool && concreteBool.Value)
            {
                LogDebug("Concrete ASSERT passed.");
                return; // Assertion passed, continue execution on this path
            }

            // Check if the condition is a concrete boolean 'false'
            if (condition is ConcreteValue<bool> concreteFalse && !concreteFalse.Value)
            {
                LogDebug($"ASSERT failed at IP {instructionPointer}: Concrete value is false.");
                ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
                CompletedPaths.Add(state); // Add this failing path
                return;
            }

            // Check if the condition is a symbolic expression
            if (condition is SymbolicExpression expression)
            {
                LogDebug($"ASSERT condition is symbolic ({expression}), but constraint creation is currently blocked by type mismatch. Halting path.");
                ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Halt due to unhandled symbolic assert
                CompletedPaths.Add(state);
            }
            else
            {
                // Condition is neither concrete true/false boolean nor a symbolic expression
                // Could be ConcreteValue<int>, SymbolicVariable, etc. - Treat as error.
                LogDebug($"ASSERT error at IP {instructionPointer}: Unexpected condition type '{condition?.GetType().Name}'.");
                ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT); // Cast and Halt
                CompletedPaths.Add(state);
            }
        }

        private void ExecuteUnconditionalJump(ISymbolicState state, Instruction instruction, int currentInstructionPointer)
        {
            LogDebug($"Unconditional jump logic missing at IP {currentInstructionPointer}. Halting.");
            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
            CompletedPaths.Add(state);
        }

        private void ExecuteConditionalJump(ISymbolicState state, Instruction instruction, int currentInstructionPointer)
        {
            LogDebug($"Conditional jump logic missing at IP {currentInstructionPointer}. Halting.");
            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
            CompletedPaths.Add(state);
        }

        private void ExecuteComparisonJump(ISymbolicState state, Instruction instruction, int currentInstructionPointer)
        {
            LogDebug($"Comparison jump logic missing at IP {currentInstructionPointer}. Halting.");
            ((SymbolicState)state).Halt(Neo.VM.VMState.FAULT);
            CompletedPaths.Add(state);
        }
    }
}
