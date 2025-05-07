using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all extended stack operations in the symbolic virtual machine.
    /// This class is responsible for additional stack manipulation operations.
    /// </summary>
    public class ExtendedStackOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedStackOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public ExtendedStackOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes an extended stack operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.NIP:
                    return HandleNip();
                case OpCode.XDROP:
                    return HandleXDrop();
                case OpCode.CLEAR:
                    return HandleClear();
                case OpCode.REVERSE3:
                    return HandleReverse3();
                case OpCode.REVERSE4:
                    return HandleReverse4();
                case OpCode.REVERSEN:
                    return HandleReverseN();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the NIP operation, which removes the second item from the top of the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNip()
        {
            // Check if there are at least 2 items on the stack
            if (_engine.CurrentState.EvaluationStack.Count < 2)
            {
                LogDebug("NIP: Not enough items on stack");
                return false;
            }

            // Pop the top item
            var top = _engine.CurrentState.Pop();

            // Pop the second item (and discard it)
            _engine.CurrentState.Pop();

            // Push the top item back
            _engine.CurrentState.Push(top);

            LogDebug("Removed second item from top of stack");
            return true;
        }

        /// <summary>
        /// Handles the XDROP operation, which removes the item n back from the top of the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleXDrop()
        {
            // Pop the index from the stack
            var index = _engine.CurrentState.Pop();

            if (index == null)
            {
                LogDebug("XDROP: Stack underflow");
                return false;
            }

            // If index is a concrete value, remove the item at that index
            if (index is ConcreteValue<int> intIndex)
            {
                // Check if there are enough items on the stack
                if (_engine.CurrentState.EvaluationStack.Count <= intIndex.Value)
                {
                    LogDebug($"XDROP: Not enough items on stack for index {intIndex.Value}");
                    return false;
                }

                // Pop all items up to the index
                var items = new List<SymbolicValue>();
                for (int i = 0; i < intIndex.Value; i++)
                {
                    items.Add(_engine.CurrentState.Pop());
                }

                // Pop and discard the item at the index
                _engine.CurrentState.Pop();

                // Push the items back in reverse order
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    _engine.CurrentState.Push(items[i]);
                }

                LogDebug($"Removed item at index {intIndex.Value} from stack");
            }
            else
            {
                LogDebug("XDROP: Index is not a concrete value");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the CLEAR operation, which removes all items from the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleClear()
        {
            // Clear the evaluation stack
            while (_engine.CurrentState.EvaluationStack.Count > 0)
            {
                _engine.CurrentState.Pop();
            }

            LogDebug("Cleared evaluation stack");
            return true;
        }

        /// <summary>
        /// Handles the REVERSE3 operation, which reverses the order of the top 3 items on the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleReverse3()
        {
            // Check if there are at least 3 items on the stack
            if (_engine.CurrentState.EvaluationStack.Count < 3)
            {
                LogDebug("REVERSE3: Not enough items on stack");
                return false;
            }

            // Pop the top 3 items
            var item1 = _engine.CurrentState.Pop();
            var item2 = _engine.CurrentState.Pop();
            var item3 = _engine.CurrentState.Pop();

            // Push them back in reverse order
            _engine.CurrentState.Push(item1);
            _engine.CurrentState.Push(item2);
            _engine.CurrentState.Push(item3);

            LogDebug("Reversed top 3 items on stack");
            return true;
        }

        /// <summary>
        /// Handles the REVERSE4 operation, which reverses the order of the top 4 items on the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleReverse4()
        {
            // Check if there are at least 4 items on the stack
            if (_engine.CurrentState.EvaluationStack.Count < 4)
            {
                LogDebug("REVERSE4: Not enough items on stack");
                return false;
            }

            // Pop the top 4 items
            var item1 = _engine.CurrentState.Pop();
            var item2 = _engine.CurrentState.Pop();
            var item3 = _engine.CurrentState.Pop();
            var item4 = _engine.CurrentState.Pop();

            // Push them back in reverse order
            _engine.CurrentState.Push(item1);
            _engine.CurrentState.Push(item2);
            _engine.CurrentState.Push(item3);
            _engine.CurrentState.Push(item4);

            LogDebug("Reversed top 4 items on stack");
            return true;
        }

        /// <summary>
        /// Handles the REVERSEN operation, which reverses the order of the top n items on the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleReverseN()
        {
            // Pop the count from the stack
            var count = _engine.CurrentState.Pop();

            if (count == null)
            {
                LogDebug("REVERSEN: Stack underflow");
                return false;
            }

            // If count is a concrete value, reverse the top count items
            if (count is ConcreteValue<int> intCount)
            {
                // Check if there are enough items on the stack
                if (_engine.CurrentState.EvaluationStack.Count < intCount.Value)
                {
                    LogDebug($"REVERSEN: Not enough items on stack for count {intCount.Value}");
                    return false;
                }

                // Pop the top count items
                var items = new List<SymbolicValue>();
                for (int i = 0; i < intCount.Value; i++)
                {
                    items.Add(_engine.CurrentState.Pop());
                }

                // Push them back in reverse order
                foreach (var item in items)
                {
                    _engine.CurrentState.Push(item);
                }

                LogDebug($"Reversed top {intCount.Value} items on stack");
            }
            else
            {
                LogDebug("REVERSEN: Count is not a concrete value");
                return false;
            }

            return true;
        }
    }
}
