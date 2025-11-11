// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_UInt.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_UInt : SmartContract
    {
        public static bool IsValidAndNotZeroUInt256(UInt256 value) => value.IsValidAndNotZero;
        public static bool IsValidAndNotZeroUInt160(UInt160 value) => value.IsValidAndNotZero;
        public static bool IsZeroUInt256(UInt256 value)
        {
            return value.IsZero;
        }

        public static bool IsZeroUInt160(UInt160 value)
        {
            return value.IsZero;
        }

        public static string ToAddress(UInt160 value)
        {
            return value.ToAddress();
        }

        public static byte[] ParseUInt160Bytes(byte[] value)
        {
            return (byte[])UInt160.Parse((string)(ByteString)value);
        }

        public static byte[] ParseUInt256Bytes(byte[] value)
        {
            return (byte[])UInt256.Parse((string)(ByteString)value);
        }

        public static byte[] ParseECPointBytes(byte[] value)
        {
            return (byte[])ECPoint.Parse((string)(ByteString)value);
        }
    }
}
