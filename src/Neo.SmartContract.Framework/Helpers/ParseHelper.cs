// Copyright (C) 2015-2026 The Neo Project.
//
// ParseHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Numerics;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.Helpers
{
    internal static class ParseHelper
    {
        private static int GetHexPrefixLength(ByteString value)
        {
            if (value.Length < 2) return 0;
            if (value[0] != (byte)'0') return 0;

            byte second = value[1];
            return (second == (byte)'x' || second == (byte)'X') ? 2 : 0;
        }

        internal static byte[] ParseHex(ByteString value)
        {
            var offset = GetHexPrefixLength(value);
            var rawLength = (BigInteger)value.Length - offset;
            if (rawLength % 2 != 0)
                throw new FormatException("Invalid hex string length.");

            var expectedLength = rawLength / 2;
            var result = new byte[(int)expectedLength];
            BigInteger index = offset;
            for (int i = 0; i < expectedLength; i++)
            {
                var high = FromHexByte(value[index]);
                var low = FromHexByte(value[index + 1]);
                result[i] = (byte)((high << 4) | low);
                index += 2;
            }
            return result;
        }

        internal static byte[] ParseAddress(string value, byte addressVersion)
        {
            var decoded = (byte[])StdLib.Base58CheckDecode(value);
            if (decoded.Length != 21)
                throw new FormatException("Invalid address length.");

            if (decoded[0] != addressVersion)
                throw new FormatException("Invalid address version.");

            return Helper.Range(decoded, 1, decoded.Length - 1);
        }

        private static BigInteger FromHexByte(BigInteger value)
        {
            if (value >= '0' && value <= '9') return value - '0';
            if (value >= 'a' && value <= 'f') return value - 'a' + 10;
            if (value >= 'A' && value <= 'F') return value - 'A' + 10;
            throw new FormatException("Invalid hex character.");
        }
    }
}
