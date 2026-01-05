// Copyright (C) 2015-2026 The Neo Project.
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

        public static UInt160 ParseUInt160(string value)
        {
            return UInt160.Parse(value);
        }

        public static UInt256 ParseUInt256(string value)
        {
            return UInt256.Parse(value);
        }

        public static ECPoint ParseECPoint(string value)
        {
            return ECPoint.Parse(value);
        }

        public static bool TryParseUInt160(string value)
        {
            try
            {
                _ = UInt160.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryParseUInt256(string value)
        {
            try
            {
                _ = UInt256.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryParseECPoint(string value)
        {
            try
            {
                _ = ECPoint.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
