// Copyright (C) 2015-2025 The Neo Project.
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
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.Helpers
{
    internal static class ParseHelper
    {
        internal static int GetHexPrefixLength(byte[] valueBytes)
        {
            if (valueBytes.Length < 2) return 0;
            return valueBytes[0] == (byte)'0' && (valueBytes[1] == (byte)'x' || valueBytes[1] == (byte)'X') ? 2 : 0;
        }

        internal static byte[] ParseHex(byte[] valueBytes, int offset, int expectedLength)
        {
            int hexLength = valueBytes.Length - offset;
            if (hexLength != expectedLength * 2)
                throw new FormatException("Invalid hex length.");

            byte[] result = new byte[expectedLength];
            for (int i = 0; i < expectedLength; i++)
            {
                byte high = FromHexByte(valueBytes[offset + i * 2]);
                byte low = FromHexByte(valueBytes[offset + i * 2 + 1]);
                result[i] = (byte)((high << 4) | low);
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

        private static byte FromHexByte(byte value)
        {
            if (value >= (byte)'0' && value <= (byte)'9') return (byte)(value - (byte)'0');
            if (value >= (byte)'a' && value <= (byte)'f') return (byte)(value - (byte)'a' + 10);
            if (value >= (byte)'A' && value <= (byte)'F') return (byte)(value - (byte)'A' + 10);
            throw new FormatException("Invalid hex character.");
        }
    }
}
