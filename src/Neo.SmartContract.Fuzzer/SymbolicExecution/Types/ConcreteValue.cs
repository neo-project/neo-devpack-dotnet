using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a concrete value.
    /// </summary>
    public class ConcreteValue : SymbolicValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public StackItem Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ConcreteValue(StackItem value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => Value.Type;

        /// <summary>
        /// Indicates whether the value is concrete (always true for ConcreteValue).
        /// </summary>
        public override bool IsConcrete => true;

        /// <summary>
        /// Attempts to get the concrete value as a BigInteger.
        /// </summary>
        public override bool TryGetInteger(out BigInteger value)
        {
            try
            {
                if (Value is Integer integer)
                {
                    value = integer.GetInteger();
                    return true;
                }
                else if (Value is VM.Types.Boolean boolean)
                {
                    value = boolean.GetBoolean() ? BigInteger.One : BigInteger.Zero;
                    return true;
                }
                else if (Value is ByteString byteString)
                {
                    value = new BigInteger(byteString.GetSpan());
                    return true;
                }
            }
            catch
            {
                // Ignore exceptions
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the concrete value as a boolean.
        /// </summary>
        public override bool TryGetBoolean(out bool value)
        {
            try
            {
                if (Value is VM.Types.Boolean boolean)
                {
                    value = boolean.GetBoolean();
                    return true;
                }
                else if (Value is Integer integer)
                {
                    value = !integer.GetInteger().IsZero;
                    return true;
                }
            }
            catch
            {
                // Ignore exceptions
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the concrete value as a byte array.
        /// </summary>
        public override bool TryGetBytes(out byte[] value)
        {
            try
            {
                if (Value is ByteString byteString)
                {
                    value = byteString.GetSpan().ToArray();
                    return true;
                }
            }
            catch
            {
                // Ignore exceptions
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Converts this symbolic value to a concrete stack item.
        /// </summary>
        public override StackItem ToStackItem()
        {
            return Value;
        }
    }
}
