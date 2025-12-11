// Copyright (C) 2015-2025 The Neo Project.
//
// UInt256.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Helpers;

namespace Neo.SmartContract.Framework
{
    public abstract class UInt256 : ByteString
    {
        public static extern UInt256 Zero { [OpCode(OpCode.PUSHDATA1, "200000000000000000000000000000000000000000000000000000000000000000")] get; }

        public static int Size => 32;

        public extern bool IsZero
        {
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISTYPE, "0x28")] //ByteString
            [OpCode(OpCode.JMPIF, "06")]  // to SIZE
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.PUSHF)]
            [OpCode(OpCode.JMP, "06")]    // to the end
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        public bool IsValidAndNotZero => IsValid && !IsZero;

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIF, "09")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator UInt256(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](UInt256 value);

        /// <summary>
        /// Implicitly converts a hexadecimal string to a UInt256 object.
        /// Assumes the string is a valid hexadecimal representation.
        /// <example>
        /// 32 bytes (64 characters) hexadecimal string: "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f" (no prefix)
        /// </example>
        /// <remarks>The input `MUST` be a valid hex string of a 32 bytes data.</remarks>
        /// <remarks>
        /// This is a compile time conversion, only work with constant string.
        /// If you want to convert a runtime string, convert it to byte[] first.
        /// </remarks>
        /// </summary>
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        public static extern implicit operator UInt256(string value);
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it

        /// <summary>
        /// Parses a 32-byte hex string (optionally prefixed with <c>0x</c>) into a <see cref="UInt256"/>.
        /// </summary>
        /// <param name="value">Hex string representing the 32-byte value.</param>
        /// <returns>The parsed <see cref="UInt256"/>.</returns>
        /// <exception cref="FormatException">Thrown when the input is not a 32-byte hex string.</exception>
        public static UInt256 Parse(string value)
        {
            if (value is null) throw new FormatException("Value cannot be null.");

            if (!ParseHelper.TryParseHex(value.ToByteArray(), out var data))
                throw new FormatException("Invalid hex value.");

            if (data.Length != UInt256.Size)
                throw new FormatException("UInt256 must be 32 bytes long.");

            return (UInt256)data.Reverse();
        }
    }
}
