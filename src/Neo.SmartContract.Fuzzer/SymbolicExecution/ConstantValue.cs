using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents a constant value in symbolic execution.
    /// </summary>
    public class ConstantValue : SymbolicValue
    {
        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the symbolic type of the constant.
        /// </summary>
        public SymbolicType SymbolicType { get; }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => ConvertType(SymbolicType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantValue"/> class.
        /// </summary>
        /// <param name="value">The value of the constant.</param>
        /// <param name="type">The type of the constant.</param>
        public ConstantValue(object value, SymbolicType type)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            SymbolicType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantValue"/> class with auto-detected type.
        /// </summary>
        /// <param name="value">The value of the constant.</param>
        public ConstantValue(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            SymbolicType = DetermineType(value);
        }

        /// <summary>
        /// Determines the symbolic type based on the value.
        /// </summary>
        /// <param name="value">The value to determine the type for.</param>
        /// <returns>The determined symbolic type.</returns>
        private static SymbolicType DetermineType(object value)
        {
            if (value is int || value is long || value is BigInteger)
                return SymbolicType.Integer;
            if (value is bool)
                return SymbolicType.Boolean;
            if (value is string)
                return SymbolicType.String;
            if (value is byte[])
                return SymbolicType.ByteArray;
            if (value is System.Collections.IList)
                return SymbolicType.Array;
            if (value is System.Collections.IDictionary)
                return SymbolicType.Map;

            return SymbolicType.Any;
        }

        /// <summary>
        /// Converts the constant value to a Neo VM StackItem.
        /// </summary>
        /// <returns>A StackItem representation of the constant.</returns>
        public override StackItem ToStackItem()
        {
            switch (Value)
            {
                case int intValue:
                    return new VM.Types.Integer(intValue);
                case long longValue:
                    return new VM.Types.Integer(longValue);
                case BigInteger bigIntValue:
                    return new VM.Types.Integer(bigIntValue);
                case bool boolValue:
                    return boolValue ? VM.Types.Boolean.True : VM.Types.Boolean.False;
                case string strValue:
                    return new VM.Types.ByteString(System.Text.Encoding.UTF8.GetBytes(strValue));
                case byte[] bytesValue:
                    return new VM.Types.Buffer(bytesValue);
                default:
                    return VM.Types.Null.Null;
            }
        }

        /// <summary>
        /// Attempts to get the concrete value as a BigInteger.
        /// </summary>
        /// <param name="value">The output BigInteger value.</param>
        /// <returns>True if the value could be converted to a BigInteger, false otherwise.</returns>
        public override bool TryGetInteger(out BigInteger value)
        {
            switch (Value)
            {
                case int intValue:
                    value = new BigInteger(intValue);
                    return true;
                case long longValue:
                    value = new BigInteger(longValue);
                    return true;
                case BigInteger bigIntValue:
                    value = bigIntValue;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        /// <summary>
        /// Attempts to get the concrete value as a boolean.
        /// </summary>
        /// <param name="value">The output boolean value.</param>
        /// <returns>True if the value could be converted to a boolean, false otherwise.</returns>
        public override bool TryGetBoolean(out bool value)
        {
            if (Value is bool boolValue)
            {
                value = boolValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the concrete value as a byte array.
        /// </summary>
        /// <param name="value">The output byte array value.</param>
        /// <returns>True if the value could be converted to a byte array, false otherwise.</returns>
        public override bool TryGetBytes(out byte[] value)
        {
            if (Value is byte[] bytesValue)
            {
                value = bytesValue;
                return true;
            }

            if (Value is string strValue)
            {
                value = System.Text.Encoding.UTF8.GetBytes(strValue);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Converts a SymbolicType to a StackItemType.
        /// </summary>
        /// <param name="type">The symbolic type to convert.</param>
        /// <returns>The corresponding StackItemType.</returns>
        private static StackItemType ConvertType(SymbolicType type)
        {
            switch (type)
            {
                case SymbolicType.Integer:
                    return StackItemType.Integer;
                case SymbolicType.Boolean:
                    return StackItemType.Boolean;
                case SymbolicType.String:
                    return StackItemType.ByteString;
                case SymbolicType.ByteArray:
                    return StackItemType.Buffer;
                case SymbolicType.Array:
                    return StackItemType.Array;
                case SymbolicType.Map:
                    return StackItemType.Map;
                default:
                    return StackItemType.Any;
            }
        }

        /// <summary>
        /// Returns a string representation of the constant.
        /// </summary>
        /// <returns>A string representation of the constant.</returns>
        public override string ToString()
        {
            return $"Const({Value}, {Type})";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is ConstantValue other)
            {
                return Value.Equals(other.Value) && Type == other.Type;
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Type);
        }
    }
}
