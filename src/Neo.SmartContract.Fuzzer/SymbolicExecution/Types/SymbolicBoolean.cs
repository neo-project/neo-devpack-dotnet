using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic boolean value.
    /// </summary>
    public class SymbolicBoolean : SymbolicValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicBoolean"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SymbolicBoolean(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => StackItemType.Boolean;

        /// <summary>
        /// Indicates whether the value is concrete (always true for SymbolicBoolean).
        /// </summary>
        public override bool IsConcrete => true;

        /// <summary>
        /// Converts this symbolic value to a concrete stack item.
        /// </summary>
        public override StackItem ToStackItem()
        {
            return Value ? VM.Types.Boolean.True : VM.Types.Boolean.False;
        }
    }
}
