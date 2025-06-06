using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a constant value in symbolic execution.
    /// </summary>
    public class SymbolicConstant : SymbolicValue
    {
        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the type of the constant.
        /// </summary>
        private readonly StackItemType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicConstant"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        /// <param name="type">The type of the constant.</param>
        public SymbolicConstant(object value, StackItemType type)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            _type = type;
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => _type;

        /// <summary>
        /// Indicates whether the value is concrete.
        /// </summary>
        public override bool IsConcrete => true;

        /// <summary>
        /// Attempts to get the concrete value as a BigInteger.
        /// </summary>
        public override bool TryGetInteger(out BigInteger value)
        {
            if (Value is BigInteger bigInt)
            {
                value = bigInt;
                return true;
            }
            if (Value is int intValue)
            {
                value = new BigInteger(intValue);
                return true;
            }
            if (Value is long longValue)
            {
                value = new BigInteger(longValue);
                return true;
            }
            return base.TryGetInteger(out value);
        }

        /// <summary>
        /// Attempts to get the concrete value as a boolean.
        /// </summary>
        public override bool TryGetBoolean(out bool value)
        {
            if (Value is bool boolValue)
            {
                value = boolValue;
                return true;
            }
            return base.TryGetBoolean(out value);
        }

        /// <summary>
        /// Attempts to get the concrete value as a byte array.
        /// </summary>
        public override bool TryGetBytes(out byte[] value)
        {
            if (Value is byte[] bytes)
            {
                value = bytes;
                return true;
            }
            return base.TryGetBytes(out value);
        }

        /// <summary>
        /// Converts this symbolic constant to a concrete stack item.
        /// </summary>
        public override StackItem ToStackItem()
        {
            return Value switch
            {
                BigInteger bigInt => new VM.Types.Integer(bigInt),
                int intValue => new VM.Types.Integer(intValue),
                long longValue => new VM.Types.Integer(longValue),
                bool boolValue => boolValue ? VM.Types.StackItem.True : VM.Types.StackItem.False,
                byte[] bytes => new VM.Types.ByteString(bytes),
                string str => new VM.Types.ByteString(System.Text.Encoding.UTF8.GetBytes(str)),
                _ => throw new InvalidOperationException($"Cannot convert value of type {Value.GetType()} to StackItem")
            };
        }

        /// <summary>
        /// Returns a string representation of this constant.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}


