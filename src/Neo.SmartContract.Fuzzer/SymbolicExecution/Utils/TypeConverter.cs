using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Utils
{
    /// <summary>
    /// Provides utility methods for converting between different type systems.
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Converts a Neo VM StackItemType to a SymbolicType.
        /// </summary>
        /// <param name="type">The StackItemType to convert.</param>
        /// <returns>The corresponding SymbolicType.</returns>
        public static SymbolicType ToSymbolicType(this StackItemType type)
        {
            return type switch
            {
                StackItemType.Any => SymbolicType.Any,
                StackItemType.Boolean => SymbolicType.Boolean,
                StackItemType.Integer => SymbolicType.Integer,
                StackItemType.ByteString => SymbolicType.ByteString,
                StackItemType.Buffer => SymbolicType.Buffer,
                StackItemType.Array => SymbolicType.Array,
                StackItemType.Struct => SymbolicType.Struct,
                StackItemType.Map => SymbolicType.Map,
                StackItemType.InteropInterface => SymbolicType.InteropInterface,
                StackItemType.Pointer => SymbolicType.Pointer,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown StackItemType")
            };
        }

        /// <summary>
        /// Converts a SymbolicType to a Neo VM StackItemType.
        /// </summary>
        /// <param name="type">The SymbolicType to convert.</param>
        /// <returns>The corresponding StackItemType.</returns>
        public static StackItemType ToStackItemType(this SymbolicType type)
        {
            return type switch
            {
                SymbolicType.Any => StackItemType.Any,
                SymbolicType.Boolean => StackItemType.Boolean,
                SymbolicType.Integer => StackItemType.Integer,
                SymbolicType.String => StackItemType.ByteString,
                SymbolicType.ByteString => StackItemType.ByteString,
                SymbolicType.ByteArray => StackItemType.Buffer,
                SymbolicType.Buffer => StackItemType.Buffer,
                SymbolicType.Array => StackItemType.Array,
                SymbolicType.Struct => StackItemType.Struct,
                SymbolicType.Map => StackItemType.Map,
                SymbolicType.InteropInterface => StackItemType.InteropInterface,
                SymbolicType.Pointer => StackItemType.Pointer,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown SymbolicType")
            };
        }
    }
}
