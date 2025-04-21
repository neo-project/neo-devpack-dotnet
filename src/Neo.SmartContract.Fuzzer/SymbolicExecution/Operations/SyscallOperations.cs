using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all system call operations in the symbolic virtual machine.
    /// This class is responsible for simulating interactions with the Neo blockchain
    /// and native contracts during symbolic execution.
    /// </summary>
    public class SyscallOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Dictionary mapping syscall names to handler functions.
        /// </summary>
        private readonly Dictionary<string, Func<Instruction, bool>> _syscallHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyscallOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public SyscallOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));

            // Initialize syscall handlers
            _syscallHandlers = new Dictionary<string, Func<Instruction, bool>>
            {
                { "System.Runtime.GetTrigger", HandleGetTrigger },
                { "System.Runtime.CheckWitness", HandleCheckWitness },
                { "System.Runtime.Notify", HandleNotify },
                { "System.Runtime.Log", HandleLog },
                { "System.Storage.Get", HandleStorageGet },
                { "System.Storage.Put", HandleStoragePut },
                { "System.Storage.Delete", HandleStorageDelete },
                { "Neo.Native.Call", HandleNativeCall },
                { "System.Contract.Call", HandleContractCall },
                { "System.Runtime.GasLeft", HandleGasLeft },
                { "System.Runtime.GetTime", HandleGetTime },
                { "System.Runtime.GetNetwork", HandleGetNetwork },
                { "System.Runtime.GetRandom", HandleGetRandom }
            };
        }

        /// <summary>
        /// Executes a syscall operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            if (opcode != OpCode.SYSCALL) return false;

            var instruction = engine.CurrentState.CurrentInstruction(_script);

            // Add null check for instruction
            if (instruction == null)
            {
                LogDebug("Cannot execute SYSCALL operation: null instruction");
                return false;
            }

            return ExecuteSyscall(instruction);
        }

        /// <summary>
        /// Executes a system call instruction.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool ExecuteSyscall(Instruction instruction)
        {
            // Convert ReadOnlyMemory<byte> to string using .Span.ToArray() on the Operand
            string syscallName = Encoding.UTF8.GetString(instruction.Operand.Span.ToArray());
            LogDebug($"SYSCALL: {syscallName}");

            // Look for a specific handler for this syscall
            if (_syscallHandlers.TryGetValue(syscallName, out var handler))
            {
                return handler(instruction);
            }

            // If no specific handler is found, use a generic handler
            return HandleGenericSyscall(syscallName, instruction);
        }

        /// <summary>
        /// Handles a generic system call.
        /// </summary>
        /// <param name="syscallName">The name of the system call.</param>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleGenericSyscall(string syscallName, Instruction instruction)
        {
            // Create a symbolic variable to represent the syscall result
            var symbolicResult = new SymbolicVariable($"Syscall_{syscallName.Replace(".", "_")}_{_engine.CurrentState.InstructionPointer}", StackItemType.Any);
            _engine.CurrentState.Push(symbolicResult);

            LogDebug($"Created generic symbolic result for syscall {syscallName}: {symbolicResult}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.GetTrigger syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleGetTrigger(Instruction instruction)
        {
            // In symbolic execution, we create a symbolic variable for the trigger
            // This allows exploration of all possible trigger types
            var triggerVar = new SymbolicVariable($"Trigger_{_engine.CurrentState.InstructionPointer}", StackItemType.Integer);
            _engine.CurrentState.Push(triggerVar);

            LogDebug($"Created symbolic trigger: {triggerVar}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.CheckWitness syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleCheckWitness(Instruction instruction)
        {
            // Pop the hash to check
            var hash = _engine.CurrentState.Pop();

            // In symbolic execution, we create a symbolic variable for the result
            // This allows exploration of both authentication success and failure paths
            var authResult = new SymbolicVariable($"CheckWitness_{_engine.CurrentState.InstructionPointer}", StackItemType.Boolean);
            _engine.CurrentState.Push(authResult);

            LogDebug($"Created symbolic CheckWitness result for {hash}: {authResult}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.Notify syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleNotify(Instruction instruction)
        {
            // Pop the notification data
            var state = _engine.CurrentState.Pop();
            var name = _engine.CurrentState.Pop();

            LogDebug($"Notify: {name} with state {state}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.Log syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleLog(Instruction instruction)
        {
            // Pop the log message
            var message = _engine.CurrentState.Pop();

            LogDebug($"Log: {message}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Storage.Get syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleStorageGet(Instruction instruction)
        {
            // Pop the key
            var key = _engine.CurrentState.Pop();

            // Check if this key exists in our symbolic storage
            // Storage is a Dictionary<object, object> but we need to ensure we work with SymbolicValue
            if (_engine.CurrentState.Storage.TryGetValue(key, out var value) && value is SymbolicValue symbolicValue)
            {
                // Use the properly typed symbolicValue
                _engine.CurrentState.Push(symbolicValue);
                LogDebug($"Storage.Get: {key} -> {symbolicValue}");
            }
            else
            {
                // If key doesn't exist in storage, create a symbolic variable
                var storageVar = new SymbolicVariable($"Storage_{key}_{_engine.CurrentState.InstructionPointer}", StackItemType.ByteString);
                _engine.CurrentState.Push(storageVar);
                LogDebug($"Storage.Get: {key} -> {storageVar} (symbolic)");
            }

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Storage.Put syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleStoragePut(Instruction instruction)
        {
            // Pop the value and key
            var value = _engine.CurrentState.Pop();
            var key = _engine.CurrentState.Pop();

            // Store in our symbolic storage
            _engine.CurrentState.Storage[key] = value;

            LogDebug($"Storage.Put: {key} = {value}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Storage.Delete syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleStorageDelete(Instruction instruction)
        {
            // Pop the key
            var key = _engine.CurrentState.Pop();

            // Remove from our symbolic storage
            _engine.CurrentState.Storage.Remove(key);

            LogDebug($"Storage.Delete: {key}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the Neo.Native.Call syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleNativeCall(Instruction instruction)
        {
            // Pop the arguments
            var args = _engine.CurrentState.Pop();
            var method = _engine.CurrentState.Pop();
            var contract = _engine.CurrentState.Pop();

            // Create a symbolic variable to represent the native call result
            var symbolicResult = new SymbolicVariable($"Native_Call_{contract}_{method}_{_engine.CurrentState.InstructionPointer}", StackItemType.Any);
            _engine.CurrentState.Push(symbolicResult);

            LogDebug($"Created symbolic result for Native.Call: {contract}.{method} -> {symbolicResult}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Contract.Call syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleContractCall(Instruction instruction)
        {
            // Pop the arguments
            var args = _engine.CurrentState.Pop();
            var method = _engine.CurrentState.Pop();
            var contract = _engine.CurrentState.Pop();

            // Create a symbolic variable to represent the contract call result
            var symbolicResult = new SymbolicVariable($"Contract_Call_{contract}_{method}_{_engine.CurrentState.InstructionPointer}", StackItemType.Any);
            _engine.CurrentState.Push(symbolicResult);

            LogDebug($"Created symbolic result for Contract.Call: {contract}.{method} -> {symbolicResult}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.GasLeft syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleGasLeft(Instruction instruction)
        {
            // Create a symbolic variable for the gas left
            var gasVar = new SymbolicVariable($"GasLeft_{_engine.CurrentState.InstructionPointer}", StackItemType.Integer);
            _engine.CurrentState.Push(gasVar);

            LogDebug($"Created symbolic gas left: {gasVar}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.GetTime syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleGetTime(Instruction instruction)
        {
            // Create a symbolic variable for the time
            var timeVar = new SymbolicVariable($"Time_{_engine.CurrentState.InstructionPointer}", StackItemType.Integer);
            _engine.CurrentState.Push(timeVar);

            LogDebug($"Created symbolic time: {timeVar}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.GetNetwork syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleGetNetwork(Instruction instruction)
        {
            // Create a symbolic variable for the network
            var networkVar = new SymbolicVariable($"Network_{_engine.CurrentState.InstructionPointer}", StackItemType.Integer);
            _engine.CurrentState.Push(networkVar);

            LogDebug($"Created symbolic network: {networkVar}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }

        /// <summary>
        /// Handles the System.Runtime.GetRandom syscall.
        /// </summary>
        /// <param name="instruction">The instruction to execute.</param>
        /// <returns>True if the syscall was handled, false otherwise.</returns>
        private bool HandleGetRandom(Instruction instruction)
        {
            // Create a symbolic variable for the random value
            var randomVar = new SymbolicVariable($"Random_{_engine.CurrentState.InstructionPointer}", StackItemType.Integer);
            _engine.CurrentState.Push(randomVar);

            LogDebug($"Created symbolic random value: {randomVar}");

            // Advance instruction pointer
            _engine.CurrentState.InstructionPointer += instruction.Size;

            return true;
        }
    }
}
