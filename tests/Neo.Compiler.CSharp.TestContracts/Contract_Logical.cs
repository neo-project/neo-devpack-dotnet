// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Logical.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Logical : SmartContract.Framework.SmartContract
    {
        public static bool TestConditionalLogicalAnd(bool x, bool y)
        {
            return x && y;
        }

        public static bool TestConditionalLogicalOr(bool x, bool y)
        {
            return x || y;
        }

        public static bool TestLogicalExclusiveOr(bool x, bool y)
        {
            return x ^ y;
        }

        public static int TestLogicalAnd(byte x, byte y)
        {
            return x & y;
        }

        public static int TestLogicalOr(byte x, byte y)
        {
            return x | y;
        }

        public static bool TestLogicalNegation(bool x)
        {
            return !x;
        }
    }
}
