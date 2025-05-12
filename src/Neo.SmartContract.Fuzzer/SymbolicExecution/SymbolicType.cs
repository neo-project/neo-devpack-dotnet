using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents the type of a symbolic value.
    /// </summary>
    public enum SymbolicType
    {
        /// <summary>
        /// Any type.
        /// </summary>
        Any,

        /// <summary>
        /// Integer type.
        /// </summary>
        Integer,

        /// <summary>
        /// Boolean type.
        /// </summary>
        Boolean,

        /// <summary>
        /// String type.
        /// </summary>
        String,

        /// <summary>
        /// Byte array type.
        /// </summary>
        ByteArray,

        /// <summary>
        /// Byte string type.
        /// </summary>
        ByteString,

        /// <summary>
        /// Buffer type.
        /// </summary>
        Buffer,

        /// <summary>
        /// Array type.
        /// </summary>
        Array,

        /// <summary>
        /// Map type.
        /// </summary>
        Map,

        /// <summary>
        /// Struct type.
        /// </summary>
        Struct,

        /// <summary>
        /// Pointer type.
        /// </summary>
        Pointer,

        /// <summary>
        /// Interop interface type.
        /// </summary>
        InteropInterface
    }
}
