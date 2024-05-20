// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework
{
    public abstract class ECPoint : ByteString
    {
        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISTYPE, "0x28")] //ByteString
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "21")] // 0x21 == 33 bytes expected array size
            [OpCode(OpCode.NUMEQUAL)]
            [OpCode(OpCode.BOOLAND)]
            get;
        }

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIF, "09")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "21")] // 0x21 == 33 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator ECPoint(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](ECPoint value);

        /// <summary>
        /// Implicitly converts a hexadecimal string to a PublicKey object.
        /// Assumes the string is a valid hexadecimal representation.
        /// </summary>
        /// <example>
        /// PublicKey from a 33 bytes (66 characters) hexadecimal string:
        ///     "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9"
        /// </example>
        /// <remarks>
        /// This is a compile time conversion, only work with constant string.
        /// If you want to convert a runtime string, convert it to byte[] first.
        /// </remarks>
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        public static extern implicit operator ECPoint(string value);
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
    }
}
