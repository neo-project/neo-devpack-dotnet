using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a concrete symbolic value with a known type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class SymbolicValue<T> : SymbolicValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicValue{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SymbolicValue(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type
        {
            get
            {
                return Value switch
                {
                    bool _ => StackItemType.Boolean,
                    int _ => StackItemType.Integer,
                    long _ => StackItemType.Integer,
                    BigInteger _ => StackItemType.Integer,
                    byte[] _ => StackItemType.ByteString,
                    string _ => StackItemType.ByteString,
                    _ => StackItemType.Any
                };
            }
        }

        /// <summary>
        /// Indicates whether the value is concrete (always true for SymbolicValue{T}).
        /// </summary>
        public override bool IsConcrete => true;

        /// <summary>
        /// Attempts to get the concrete value as a BigInteger.
        /// </summary>
        public override bool TryGetInteger(out BigInteger value)
        {
            if (Value is int intValue)
            {
                value = new BigInteger(intValue);
                return true;
            }
            else if (Value is long longValue)
            {
                value = new BigInteger(longValue);
                return true;
            }
            else if (Value is BigInteger bigIntValue)
            {
                value = bigIntValue;
                return true;
            }
            else if (Value is bool boolValue)
            {
                value = boolValue ? BigInteger.One : BigInteger.Zero;
                return true;
            }

            value = default;
            return false;
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
            else if (Value is int intValue)
            {
                value = intValue != 0;
                return true;
            }
            else if (Value is long longValue)
            {
                value = longValue != 0;
                return true;
            }
            else if (Value is BigInteger bigIntValue)
            {
                value = bigIntValue != BigInteger.Zero;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the concrete value as a byte array.
        /// </summary>
        public override bool TryGetBytes(out byte[] value)
        {
            if (Value is byte[] byteArray)
            {
                value = byteArray;
                return true;
            }
            else if (Value is string stringValue)
            {
                value = System.Text.Encoding.UTF8.GetBytes(stringValue);
                return true;
            }

            value = System.Array.Empty<byte>();
            return false;
        }

        /// <summary>
        /// Converts this symbolic value to a concrete stack item.
        /// </summary>
        public override StackItem ToStackItem()
        {
            return Value switch
            {
                bool boolValue => boolValue ? Neo.VM.Types.Boolean.True : Neo.VM.Types.Boolean.False,
                int intValue => new Integer(intValue),
                long longValue => new Integer(longValue),
                BigInteger bigIntValue => new Integer(bigIntValue),
                byte[] byteArray => new ByteString(new ReadOnlyMemory<byte>(byteArray)),
                string stringValue => new ByteString(System.Text.Encoding.UTF8.GetBytes(stringValue)),
                _ => throw new InvalidOperationException($"Cannot convert {Value?.GetType().Name} to StackItem")
            };
        }

        /// <summary>
        /// Returns a string representation of this symbolic value.
        /// </summary>
        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }
    }
}
