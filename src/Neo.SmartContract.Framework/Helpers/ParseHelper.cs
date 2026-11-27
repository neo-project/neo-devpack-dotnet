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
        internal static byte[] ParseHex(ByteString value)
        {
            var offset = BigInteger.Zero;
            BigInteger valueLength = value.Length;
            if (valueLength > 2 && value[0] == (byte)'0')
            {
                byte x = value[1];
                offset = (x == (byte)'x' || x == (byte)'X') ? 2 : 0;
            }

            var rawLength = valueLength - offset;
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

        internal static ByteString ParseAddress(string value, byte addressVersion)
        {
            var decoded = StdLib.Base58CheckDecode(value);
            if (decoded.Length != 21)
                throw new FormatException("Invalid address length.");

            if (decoded[0] != addressVersion)
                throw new FormatException("Invalid address version.");

            return decoded.Substring(1, 20); // decoded.Length - 1 = 20
        }

        private static BigInteger FromHexByte(BigInteger value)
        {
            if (value.Within('0', ':')) return value - '0'; // ':' is the next char of '9'
            if (value.Within('a', 'g')) return value - 87; // 'g' is the next char of 'f'
            if (value.Within('A', 'G')) return value - 55;
            throw new FormatException("Invalid hex character.");
        }
    }
}
