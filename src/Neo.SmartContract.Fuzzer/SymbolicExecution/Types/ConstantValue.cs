using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a constant value in symbolic execution.
    /// This class is an alias for SymbolicConstant to maintain compatibility with existing code.
    /// </summary>
    public class ConstantValue : SymbolicConstant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantValue"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        /// <param name="type">The type of the constant.</param>
        public ConstantValue(object value, StackItemType type) 
            : base(value, type)
        {
        }

        /// <summary>
        /// Creates a new integer constant.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>A new constant value.</returns>
        public static ConstantValue Integer(BigInteger value)
        {
            return new ConstantValue(value, StackItemType.Integer);
        }

        /// <summary>
        /// Creates a new integer constant.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>A new constant value.</returns>
        public static ConstantValue Integer(long value)
        {
            return new ConstantValue(value, StackItemType.Integer);
        }

        /// <summary>
        /// Creates a new integer constant.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>A new constant value.</returns>
        public static ConstantValue Integer(int value)
        {
            return new ConstantValue(value, StackItemType.Integer);
        }

        /// <summary>
        /// Creates a new boolean constant.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <returns>A new constant value.</returns>
        public static ConstantValue Boolean(bool value)
        {
            return new ConstantValue(value, StackItemType.Boolean);
        }

        /// <summary>
        /// Creates a new byte string constant.
        /// </summary>
        /// <param name="value">The byte array value.</param>
        /// <returns>A new constant value.</returns>
        public static ConstantValue ByteString(byte[] value)
        {
            return new ConstantValue(value, StackItemType.ByteString);
        }

        /// <summary>
        /// Creates a new string constant.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A new constant value.</returns>
        public static ConstantValue String(string value)
        {
            return new ConstantValue(value, StackItemType.ByteString);
        }
    }
}
