using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic byte array value.
    /// </summary>
    public class SymbolicByteArray : SymbolicValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public ReadOnlyMemory<byte> Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicByteArray"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SymbolicByteArray(ReadOnlySpan<byte> value)
        {
            Value = new ReadOnlyMemory<byte>(value.ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicByteArray"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SymbolicByteArray(byte[] value)
        {
            Value = new ReadOnlyMemory<byte>(value ?? throw new ArgumentNullException(nameof(value)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicByteArray"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SymbolicByteArray(ReadOnlyMemory<byte> value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => StackItemType.ByteString;

        /// <summary>
        /// Indicates whether the value is concrete (always true for SymbolicByteArray).
        /// </summary>
        public override bool IsConcrete => true;

        /// <summary>
        /// Attempts to get the concrete value as a byte array.
        /// </summary>
        public override bool TryGetBytes(out byte[] value)
        {
            value = Value.ToArray();
            return true;
        }

        /// <summary>
        /// Converts this symbolic value to a concrete stack item.
        /// </summary>
        public override StackItem ToStackItem()
        {
            return new ByteString(Value);
        }
    }
}
