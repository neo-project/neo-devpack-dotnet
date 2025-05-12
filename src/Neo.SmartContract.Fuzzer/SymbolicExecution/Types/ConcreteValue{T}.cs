using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a concrete value with a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the concrete value.</typeparam>
    public class ConcreteValue<T> : SymbolicValue
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteValue{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ConcreteValue(T value)
        {
            Value = value != null ? value : throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type
        {
            get
            {
                if (Value is bool)
                    return StackItemType.Boolean;
                if (Value is BigInteger || Value is int || Value is long || Value is byte || Value is sbyte || Value is short || Value is ushort || Value is uint || Value is ulong)
                    return StackItemType.Integer;
                if (Value is byte[] || Value is string)
                    return StackItemType.ByteString;

                // Default for other types
                return StackItemType.Any;
            }
        }

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
                if (Value is BigInteger bi)
                {
                    value = bi;
                    return true;
                }
                else if (Value is int i)
                {
                    value = new BigInteger(i);
                    return true;
                }
                else if (Value is long l)
                {
                    value = new BigInteger(l);
                    return true;
                }
                else if (Value is bool b)
                {
                    value = b ? BigInteger.One : BigInteger.Zero;
                    return true;
                }
                else if (Value is byte[] bytes)
                {
                    value = new BigInteger(bytes);
                    return true;
                }
                else if (Value is string s && BigInteger.TryParse(s, out var result))
                {
                    value = result;
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
                if (Value is bool b)
                {
                    value = b;
                    return true;
                }
                else if (Value is BigInteger bi)
                {
                    value = !bi.IsZero;
                    return true;
                }
                else if (Value is int i)
                {
                    value = i != 0;
                    return true;
                }
                else if (Value is long l)
                {
                    value = l != 0;
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
                if (Value is byte[] bytes)
                {
                    value = bytes;
                    return true;
                }
                else if (Value is string s)
                {
                    value = System.Text.Encoding.UTF8.GetBytes(s);
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
            if (Value is bool b)
                return b ? VM.Types.Boolean.True : VM.Types.Boolean.False;
            if (Value is BigInteger bi)
                return new Integer(bi);
            if (Value is int i)
                return new Integer(i);
            if (Value is long l)
                return new Integer(l);
            if (Value is byte[] bytes)
                return new ByteString(bytes);
            if (Value is string s)
                return new ByteString(System.Text.Encoding.UTF8.GetBytes(s));

            throw new InvalidCastException($"Cannot convert type {typeof(T).Name} to StackItem");
        }
    }
}
