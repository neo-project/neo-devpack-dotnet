// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;

namespace Neo
{
    public abstract class UInt160 : ByteString
    {
        public static extern UInt160 Zero { [OpCode(OpCode.PUSHDATA1, "140000000000000000000000000000000000000000")] get; }

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
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "14")] // 0x14 == 20 bytes expected array size
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
        [OpCode(OpCode.PUSHINT8, "14")] // 0x14 == 20 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator UInt160(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](UInt160 value);

        /// <summary>
        /// Converts the specified script hash to an address.
        /// </summary>
        /// <param name="version">The address version.</param>
        /// <returns>The converted address.</returns>
        public string ToAddress(byte version)
        {
            byte[] data = { version };
            data = Helper.Concat(data, this);
            return StdLib.Base58CheckEncode((ByteString)data);
        }
    }
}
