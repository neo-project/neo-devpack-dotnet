using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all compound type operations in the symbolic virtual machine.
    /// This class is responsible for operations on arrays, maps, and structs.
    /// </summary>
    public class CompoundTypeOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundTypeOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public CompoundTypeOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes a compound type operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                // Array operations
                case OpCode.NEWARRAY0:
                    return HandleNewArray0();
                case OpCode.NEWARRAY:
                    return HandleNewArray();
                case OpCode.NEWARRAY_T:
                    return HandleNewArrayT();

                // Struct operations
                case OpCode.NEWSTRUCT0:
                    return HandleNewStruct0();
                case OpCode.NEWSTRUCT:
                    return HandleNewStruct();

                // Map operations
                case OpCode.NEWMAP:
                    return HandleNewMap();
                case OpCode.PACKMAP:
                    return HandlePackMap();

                // Pack/Unpack operations
                case OpCode.PACK:
                    return HandlePack();
                case OpCode.UNPACK:
                    return HandleUnpack();
                case OpCode.PACKSTRUCT:
                    return HandlePackStruct();

                // Collection operations
                case OpCode.SIZE:
                    return HandleSize();
                case OpCode.HASKEY:
                    return HandleHasKey();
                case OpCode.KEYS:
                    return HandleKeys();
                case OpCode.VALUES:
                    return HandleValues();
                case OpCode.PICKITEM:
                    return HandlePickItem();
                case OpCode.APPEND:
                    return HandleAppend();
                case OpCode.SETITEM:
                    return HandleSetItem();
                case OpCode.REVERSEITEMS:
                    return HandleReverseItems();
                case OpCode.REMOVE:
                    return HandleRemove();
                case OpCode.CLEARITEMS:
                    return HandleClearItems();
                case OpCode.POPITEM:
                    return HandlePopItem();

                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the NEWARRAY0 operation, which creates an empty array.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewArray0()
        {
            // Create a new symbolic array with size 0
            var symbolicArray = new SymbolicArray(0);
            _engine.CurrentState.Push(symbolicArray);

            LogDebug("Created empty array");
            return true;
        }

        /// <summary>
        /// Handles the NEWARRAY operation, which creates an array of the specified size.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewArray()
        {
            // Pop the size from the stack
            var size = _engine.CurrentState.Pop();

            if (size == null)
            {
                LogDebug("NEWARRAY: Stack underflow");
                return false;
            }

            // If size is a concrete value, create an array of that size
            if (size is ConcreteValue<int> intSize)
            {
                var symbolicArray = new SymbolicArray(intSize.Value);
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array of size {intSize.Value}");
            }
            else if (size is ConcreteValue<BigInteger> bigIntSize)
            {
                // Ensure the size is within reasonable bounds
                if (bigIntSize.Value > int.MaxValue || bigIntSize.Value < 0)
                {
                    LogDebug($"NEWARRAY: Invalid size {bigIntSize.Value}");
                    return false;
                }

                var symbolicArray = new SymbolicArray((int)bigIntSize.Value);
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array of size {bigIntSize.Value}");
            }
            else
            {
                // If size is symbolic, create a symbolic array with unknown size
                var symbolicArray = new SymbolicArray();
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array of symbolic size {size}");
            }

            return true;
        }

        /// <summary>
        /// Handles the NEWARRAY_T operation, which creates an array of the specified type and size.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewArrayT()
        {
            // Get the type from the instruction operand
            var instruction = _engine.CurrentState.CurrentInstruction(_script);
            if (instruction == null || instruction.Operand.IsEmpty)
            {
                LogDebug("NEWARRAY_T: Invalid instruction or operand");
                return false;
            }

            // The operand is a single byte representing the type
            var type = (VM.Types.StackItemType)instruction.Operand.Span[0];

            // Pop the size from the stack
            var size = _engine.CurrentState.Pop();

            if (size == null)
            {
                LogDebug("NEWARRAY_T: Stack underflow");
                return false;
            }

            // If size is a concrete value, create an array of that size and type
            if (size is ConcreteValue<int> intSize)
            {
                var symbolicArray = new SymbolicArray(intSize.Value, type);
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array of type {type} and size {intSize.Value}");
            }
            else if (size is ConcreteValue<BigInteger> bigIntSize)
            {
                // Ensure the size is within reasonable bounds
                if (bigIntSize.Value > int.MaxValue || bigIntSize.Value < 0)
                {
                    LogDebug($"NEWARRAY_T: Invalid size {bigIntSize.Value}");
                    return false;
                }

                var symbolicArray = new SymbolicArray((int)bigIntSize.Value, type);
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array of type {type} and size {bigIntSize.Value}");
            }
            else
            {
                // If size is symbolic, create a symbolic array with unknown size but known type
                var symbolicArray = new SymbolicArray(type);
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array of type {type} and symbolic size {size}");
            }

            return true;
        }

        /// <summary>
        /// Handles the NEWSTRUCT0 operation, which creates an empty struct.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewStruct0()
        {
            // Create a new symbolic struct with size 0
            var symbolicStruct = new SymbolicStruct(0);
            _engine.CurrentState.Push(symbolicStruct);

            LogDebug("Created empty struct");
            return true;
        }

        /// <summary>
        /// Handles the NEWSTRUCT operation, which creates a struct of the specified size.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewStruct()
        {
            // Pop the size from the stack
            var size = _engine.CurrentState.Pop();

            if (size == null)
            {
                LogDebug("NEWSTRUCT: Stack underflow");
                return false;
            }

            // If size is a concrete value, create a struct of that size
            if (size is ConcreteValue<int> intSize)
            {
                var symbolicStruct = new SymbolicStruct(intSize.Value);
                _engine.CurrentState.Push(symbolicStruct);
                LogDebug($"Created struct of size {intSize.Value}");
            }
            else if (size is ConcreteValue<BigInteger> bigIntSize)
            {
                // Ensure the size is within reasonable bounds
                if (bigIntSize.Value > int.MaxValue || bigIntSize.Value < 0)
                {
                    LogDebug($"NEWSTRUCT: Invalid size {bigIntSize.Value}");
                    return false;
                }

                var symbolicStruct = new SymbolicStruct((int)bigIntSize.Value);
                _engine.CurrentState.Push(symbolicStruct);
                LogDebug($"Created struct of size {bigIntSize.Value}");
            }
            else
            {
                // If size is symbolic, create a symbolic struct with unknown size
                var symbolicStruct = new SymbolicStruct();
                _engine.CurrentState.Push(symbolicStruct);
                LogDebug($"Created struct of symbolic size {size}");
            }

            return true;
        }

        /// <summary>
        /// Handles the NEWMAP operation, which creates a new map.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleNewMap()
        {
            // Create a new symbolic map
            var symbolicMap = new SymbolicMap();
            _engine.CurrentState.Push(symbolicMap);

            LogDebug("Created new map");
            return true;
        }

        /// <summary>
        /// Handles the PACKMAP operation, which creates a map from key-value pairs on the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandlePackMap()
        {
            // Pop the number of key-value pairs from the stack
            var count = _engine.CurrentState.Pop();

            if (count == null)
            {
                LogDebug("PACKMAP: Stack underflow");
                return false;
            }

            // If count is a concrete value, create a map with that many key-value pairs
            if (count is ConcreteValue<int> intCount || count is ConcreteValue<BigInteger> bigIntCount)
            {
                int n = count is ConcreteValue<int> ? ((ConcreteValue<int>)count).Value : (int)((ConcreteValue<BigInteger>)count).Value;

                // Ensure we have enough items on the stack
                if (_engine.CurrentState.EvaluationStack.Count < n * 2)
                {
                    LogDebug($"PACKMAP: Not enough items on stack for {n} key-value pairs");
                    return false;
                }

                var symbolicMap = new SymbolicMap();

                // Pop key-value pairs from the stack and add them to the map
                for (int i = 0; i < n; i++)
                {
                    var value = _engine.CurrentState.Pop();
                    var key = _engine.CurrentState.Pop();

                    bool success = symbolicMap.Add(key, value);
                    if (!success)
                    {
                        LogDebug($"Failed to add key-value pair to map: key size may exceed MaxKeySize");
                    }
                }

                _engine.CurrentState.Push(symbolicMap);
                LogDebug($"Created map with {n} key-value pairs");
            }
            else
            {
                // If count is symbolic, create an empty symbolic map
                var symbolicMap = new SymbolicMap();
                _engine.CurrentState.Push(symbolicMap);
                LogDebug($"Created map with symbolic count {count}");
            }

            return true;
        }

        /// <summary>
        /// Handles the PACK operation, which creates an array from items on the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandlePack()
        {
            // Pop the number of items from the stack
            var count = _engine.CurrentState.Pop();

            if (count == null)
            {
                LogDebug("PACK: Stack underflow");
                return false;
            }

            // If count is a concrete value, create an array with that many items
            if (count is ConcreteValue<int> intCount || count is ConcreteValue<BigInteger> bigIntCount)
            {
                int n = count is ConcreteValue<int> ? ((ConcreteValue<int>)count).Value : (int)((ConcreteValue<BigInteger>)count).Value;

                // Ensure we have enough items on the stack
                if (_engine.CurrentState.EvaluationStack.Count < n)
                {
                    LogDebug($"PACK: Not enough items on stack for array of size {n}");
                    return false;
                }

                var items = new List<SymbolicValue>();

                // Pop items from the stack and add them to the array
                for (int i = 0; i < n; i++)
                {
                    var item = _engine.CurrentState.Pop();
                    items.Add(item);
                }

                // Reverse the items since they were popped in reverse order
                items.Reverse();

                var symbolicArray = new SymbolicArray(items);
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array with {n} items");
            }
            else
            {
                // If count is symbolic, create an empty symbolic array
                var symbolicArray = new SymbolicArray();
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created array with symbolic count {count}");
            }

            return true;
        }

        /// <summary>
        /// Handles the UNPACK operation, which unpacks an array onto the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleUnpack()
        {
            // Pop the array from the stack
            var array = _engine.CurrentState.Pop();

            if (array == null)
            {
                LogDebug("UNPACK: Stack underflow");
                return false;
            }

            // If the array is a symbolic array, unpack its items onto the stack
            if (array is SymbolicArray symbolicArray)
            {
                var items = symbolicArray.GetItems();

                // Push the items onto the stack
                foreach (var item in items)
                {
                    _engine.CurrentState.Push(item);
                }

                // Push the count onto the stack
                _engine.CurrentState.Push(new ConcreteValue<int>(items.Count));
                LogDebug($"Unpacked array with {items.Count} items");
            }
            else if (array is SymbolicMap symbolicMap)
            {
                var entries = symbolicMap.GetEntries();

                // For a map, we push key-value pairs onto the stack
                foreach (var entry in entries)
                {
                    _engine.CurrentState.Push(entry.Key);
                    _engine.CurrentState.Push(entry.Value);
                }

                // Push the count onto the stack (number of key-value pairs)
                _engine.CurrentState.Push(new ConcreteValue<int>(entries.Count));
                LogDebug($"Unpacked map with {entries.Count} entries");
            }
            else if (array is SymbolicStruct symbolicStruct)
            {
                var fields = symbolicStruct.GetFields();

                // Push the fields onto the stack
                foreach (var field in fields)
                {
                    _engine.CurrentState.Push(field);
                }

                // Push the count onto the stack
                _engine.CurrentState.Push(new ConcreteValue<int>(fields.Count));
                LogDebug($"Unpacked struct with {fields.Count} fields");
            }
            else
            {
                // If it's not a collection type, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"Unpack_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for UNPACK of non-collection type: {array}");
            }

            return true;
        }

        /// <summary>
        /// Handles the PACKSTRUCT operation, which creates a struct from items on the stack.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandlePackStruct()
        {
            // Pop the number of items from the stack
            var count = _engine.CurrentState.Pop();

            if (count == null)
            {
                LogDebug("PACKSTRUCT: Stack underflow");
                return false;
            }

            // If count is a concrete value, create a struct with that many items
            if (count is ConcreteValue<int> intCount || count is ConcreteValue<BigInteger> bigIntCount)
            {
                int n = count is ConcreteValue<int> ? ((ConcreteValue<int>)count).Value : (int)((ConcreteValue<BigInteger>)count).Value;

                // Ensure we have enough items on the stack
                if (_engine.CurrentState.EvaluationStack.Count < n)
                {
                    LogDebug($"PACKSTRUCT: Not enough items on stack for struct of size {n}");
                    return false;
                }

                var fields = new List<SymbolicValue>();

                // Pop items from the stack and add them to the struct
                for (int i = 0; i < n; i++)
                {
                    var field = _engine.CurrentState.Pop();
                    fields.Add(field);
                }

                // Reverse the fields since they were popped in reverse order
                fields.Reverse();

                var symbolicStruct = new SymbolicStruct(fields);
                _engine.CurrentState.Push(symbolicStruct);
                LogDebug($"Created struct with {n} fields");
            }
            else
            {
                // If count is symbolic, create an empty symbolic struct
                var symbolicStruct = new SymbolicStruct();
                _engine.CurrentState.Push(symbolicStruct);
                LogDebug($"Created struct with symbolic count {count}");
            }

            return true;
        }

        /// <summary>
        /// Handles the SIZE operation, which gets the size of a collection.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleSize()
        {
            // Pop the collection from the stack
            var collection = _engine.CurrentState.Pop();

            if (collection == null)
            {
                LogDebug("SIZE: Stack underflow");
                return false;
            }

            // If the collection is a symbolic array, get its size
            if (collection is SymbolicArray symbolicArray)
            {
                var size = symbolicArray.GetSize();
                if (size.HasValue)
                {
                    _engine.CurrentState.Push(new ConcreteValue<int>(size.Value));
                    LogDebug($"Size of array: {size.Value}");
                }
                else
                {
                    var symbolicSize = new SymbolicVariable($"Array_Size_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                    _engine.CurrentState.Push(symbolicSize);
                    LogDebug($"Created symbolic size for array: {symbolicSize}");
                }
            }
            else if (collection is SymbolicMap symbolicMap)
            {
                var size = symbolicMap.GetSize();
                if (size.HasValue)
                {
                    _engine.CurrentState.Push(new ConcreteValue<int>(size.Value));
                    LogDebug($"Size of map: {size.Value}");
                }
                else
                {
                    var symbolicSize = new SymbolicVariable($"Map_Size_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                    _engine.CurrentState.Push(symbolicSize);
                    LogDebug($"Created symbolic size for map: {symbolicSize}");
                }
            }
            else if (collection is SymbolicStruct symbolicStruct)
            {
                var size = symbolicStruct.GetSize();
                if (size.HasValue)
                {
                    _engine.CurrentState.Push(new ConcreteValue<int>(size.Value));
                    LogDebug($"Size of struct: {size.Value}");
                }
                else
                {
                    var symbolicSize = new SymbolicVariable($"Struct_Size_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                    _engine.CurrentState.Push(symbolicSize);
                    LogDebug($"Created symbolic size for struct: {symbolicSize}");
                }
            }
            else
            {
                // If it's not a collection type, create a symbolic variable to represent the result
                var symbolicSize = new SymbolicVariable($"Size_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                _engine.CurrentState.Push(symbolicSize);
                LogDebug($"Created symbolic size for non-collection type: {collection}");
            }

            return true;
        }

        /// <summary>
        /// Handles the HASKEY operation, which checks if a key exists in a collection.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleHasKey()
        {
            // Pop the key and collection from the stack
            var key = _engine.CurrentState.Pop();
            var collection = _engine.CurrentState.Pop();

            if (key == null || collection == null)
            {
                LogDebug("HASKEY: Stack underflow");
                return false;
            }

            // If the collection is a symbolic array and the key is a concrete integer, check if the index is valid
            if (collection is SymbolicArray symbolicArray && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                var size = symbolicArray.GetSize();

                if (size.HasValue)
                {
                    bool hasKey = index >= 0 && index < size.Value;
                    _engine.CurrentState.Push(new ConcreteValue<bool>(hasKey));
                    LogDebug($"Array has key {index}: {hasKey}");
                }
                else
                {
                    var symbolicResult = new SymbolicVariable($"Array_HasKey_{index}_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Boolean);
                    _engine.CurrentState.Push(symbolicResult);
                    LogDebug($"Created symbolic result for array has key {index}: {symbolicResult}");
                }
            }
            else if (collection is SymbolicMap symbolicMap)
            {
                var hasKey = symbolicMap.HasKey(key);
                if (hasKey.HasValue)
                {
                    _engine.CurrentState.Push(new ConcreteValue<bool>(hasKey.Value));
                    LogDebug($"Map has key {key}: {hasKey.Value}");
                }
                else
                {
                    var symbolicResult = new SymbolicVariable($"Map_HasKey_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Boolean);
                    _engine.CurrentState.Push(symbolicResult);
                    LogDebug($"Created symbolic result for map has key {key}: {symbolicResult}");
                }
            }
            else
            {
                // If it's not a collection type or the key is symbolic, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"HasKey_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Boolean);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for has key: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the KEYS operation, which gets the keys of a map.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleKeys()
        {
            // Pop the map from the stack
            var map = _engine.CurrentState.Pop();

            if (map == null)
            {
                LogDebug("KEYS: Stack underflow");
                return false;
            }

            // If the map is a symbolic map, get its keys
            if (map is SymbolicMap symbolicMap)
            {
                var keys = symbolicMap.GetKeys();
                var keysArray = new SymbolicArray(keys);
                _engine.CurrentState.Push(keysArray);
                LogDebug($"Got keys from map: {keys.Count} keys");
            }
            else
            {
                // If it's not a map, create a symbolic array to represent the result
                var symbolicArray = new SymbolicArray();
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created symbolic array for keys of non-map type: {map}");
            }

            return true;
        }

        /// <summary>
        /// Handles the VALUES operation, which gets the values of a map.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleValues()
        {
            // Pop the map from the stack
            var map = _engine.CurrentState.Pop();

            if (map == null)
            {
                LogDebug("VALUES: Stack underflow");
                return false;
            }

            // If the map is a symbolic map, get its values
            if (map is SymbolicMap symbolicMap)
            {
                var values = symbolicMap.GetValues();
                var valuesArray = new SymbolicArray(values);
                _engine.CurrentState.Push(valuesArray);
                LogDebug($"Got values from map: {values.Count} values");
            }
            else
            {
                // If it's not a map, create a symbolic array to represent the result
                var symbolicArray = new SymbolicArray();
                _engine.CurrentState.Push(symbolicArray);
                LogDebug($"Created symbolic array for values of non-map type: {map}");
            }

            return true;
        }

        /// <summary>
        /// Handles the PICKITEM operation, which gets an item from a collection.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandlePickItem()
        {
            // Pop the key and collection from the stack
            var key = _engine.CurrentState.Pop();
            var collection = _engine.CurrentState.Pop();

            if (key == null || collection == null)
            {
                LogDebug("PICKITEM: Stack underflow");
                return false;
            }

            // If the collection is a symbolic array and the key is a concrete integer, get the item at that index
            if (collection is SymbolicArray symbolicArray && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                var item = symbolicArray.GetItem(index);
                _engine.CurrentState.Push(item);
                LogDebug($"Picked item at index {index} from array");
            }
            else if (collection is SymbolicMap symbolicMap)
            {
                var item = symbolicMap.GetItem(key);
                _engine.CurrentState.Push(item);
                LogDebug($"Picked item with key {key} from map");
            }
            else if (collection is SymbolicStruct symbolicStruct && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                var field = symbolicStruct.GetField(index);
                _engine.CurrentState.Push(field);
                LogDebug($"Picked field at index {index} from struct");
            }
            else
            {
                // If it's not a collection type or the key is symbolic, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"PickItem_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Any);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for pick item: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the APPEND operation, which appends an item to an array.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleAppend()
        {
            // Pop the item and array from the stack
            var item = _engine.CurrentState.Pop();
            var array = _engine.CurrentState.Pop();

            if (item == null || array == null)
            {
                LogDebug("APPEND: Stack underflow");
                return false;
            }

            // If the array is a symbolic array, append the item to it
            if (array is SymbolicArray symbolicArray)
            {
                symbolicArray.Append(item);
                LogDebug($"Appended item to array");
            }
            else if (array is SymbolicStruct symbolicStruct)
            {
                symbolicStruct.Append(item);
                LogDebug($"Appended field to struct");
            }
            else
            {
                LogDebug($"Cannot append to non-array/struct type: {array}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the SETITEM operation, which sets an item in a collection.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleSetItem()
        {
            // Pop the value, key, and collection from the stack
            var value = _engine.CurrentState.Pop();
            var key = _engine.CurrentState.Pop();
            var collection = _engine.CurrentState.Pop();

            if (value == null || key == null || collection == null)
            {
                LogDebug("SETITEM: Stack underflow");
                return false;
            }

            // If the collection is a symbolic array and the key is a concrete integer, set the item at that index
            if (collection is SymbolicArray symbolicArray && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                bool success = symbolicArray.SetItem(index, value);
                LogDebug($"Set item at index {index} in array: {(success ? "success" : "failed")}");
            }
            else if (collection is SymbolicMap symbolicMap)
            {
                bool success = symbolicMap.SetItem(key, value);
                LogDebug($"Set item with key {key} in map: {(success ? "success" : "failed - key may exceed MaxKeySize")}");
            }
            else if (collection is SymbolicStruct symbolicStruct && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                bool success = symbolicStruct.SetField(index, value);
                LogDebug($"Set field at index {index} in struct: {(success ? "success" : "failed")}");
            }
            else
            {
                LogDebug($"Cannot set item in non-collection type: {collection}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the REVERSEITEMS operation, which reverses the items in an array.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleReverseItems()
        {
            // Pop the array from the stack
            var array = _engine.CurrentState.Pop();

            if (array == null)
            {
                LogDebug("REVERSEITEMS: Stack underflow");
                return false;
            }

            // If the array is a symbolic array, reverse its items
            if (array is SymbolicArray symbolicArray)
            {
                symbolicArray.Reverse();
                LogDebug($"Reversed items in array");
            }
            else if (array is SymbolicStruct symbolicStruct)
            {
                symbolicStruct.Reverse();
                LogDebug($"Reversed fields in struct");
            }
            else
            {
                LogDebug($"Cannot reverse items in non-array/struct type: {array}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the REMOVE operation, which removes an item from a collection.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleRemove()
        {
            // Pop the key and collection from the stack
            var key = _engine.CurrentState.Pop();
            var collection = _engine.CurrentState.Pop();

            if (key == null || collection == null)
            {
                LogDebug("REMOVE: Stack underflow");
                return false;
            }

            // If the collection is a symbolic array and the key is a concrete integer, remove the item at that index
            if (collection is SymbolicArray symbolicArray && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                bool success = symbolicArray.RemoveAt(index);
                LogDebug($"Removed item at index {index} from array: {(success ? "success" : "failed")}");
            }
            else if (collection is SymbolicMap symbolicMap)
            {
                bool success = symbolicMap.Remove(key);
                LogDebug($"Removed item with key {key} from map: {(success ? "success" : "failed")}");
            }
            else if (collection is SymbolicStruct symbolicStruct && (key is ConcreteValue<int> || key is ConcreteValue<BigInteger>))
            {
                int index = key is ConcreteValue<int> ? ((ConcreteValue<int>)key).Value : (int)((ConcreteValue<BigInteger>)key).Value;
                bool success = symbolicStruct.RemoveAt(index);
                LogDebug($"Removed field at index {index} from struct: {(success ? "success" : "failed")}");
            }
            else
            {
                LogDebug($"Cannot remove item from non-collection type: {collection}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the CLEARITEMS operation, which clears all items from a collection.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleClearItems()
        {
            // Pop the collection from the stack
            var collection = _engine.CurrentState.Pop();

            if (collection == null)
            {
                LogDebug("CLEARITEMS: Stack underflow");
                return false;
            }

            // If the collection is a symbolic array, clear its items
            if (collection is SymbolicArray symbolicArray)
            {
                symbolicArray.Clear();
                LogDebug($"Cleared items from array");
            }
            else if (collection is SymbolicMap symbolicMap)
            {
                symbolicMap.Clear();
                LogDebug($"Cleared items from map");
            }
            else if (collection is SymbolicStruct symbolicStruct)
            {
                symbolicStruct.Clear();
                LogDebug($"Cleared fields from struct");
            }
            else
            {
                LogDebug($"Cannot clear items from non-collection type: {collection}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the POPITEM operation, which pops the last item from an array.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandlePopItem()
        {
            // Pop the array from the stack
            var array = _engine.CurrentState.Pop();

            if (array == null)
            {
                LogDebug("POPITEM: Stack underflow");
                return false;
            }

            // If the array is a symbolic array, pop its last item
            if (array is SymbolicArray symbolicArray)
            {
                var item = symbolicArray.Pop();
                _engine.CurrentState.Push(item);
                LogDebug($"Popped last item from array");
            }
            else
            {
                // If it's not an array, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"PopItem_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Any);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for pop item from non-array type: {array}");
            }

            return true;
        }
    }
}
