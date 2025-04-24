// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_IntegerParse.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Compiler Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.Compiler.CSharp.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_IntegerParse : SmartContract.Framework.SmartContract
    {
        public static sbyte testSbyteparse(string s)
        {
            return sbyte.Parse(s);
        }

        public static byte testByteparse(string s)
        {
            return byte.Parse(s);
        }

        public static ushort testUshortparse(string s)
        {
            return ushort.Parse(s);
        }

        public static short testShortparse(string s)
        {
            return short.Parse(s);
        }

        public static ulong testUlongparse(string s)
        {
            return ulong.Parse(s);
        }

        public static long testLongparse(string s)
        {
            return long.Parse(s);
        }

        public static uint testUintparse(string s)
        {
            return uint.Parse(s);
        }

        public static int testIntparse(string s)
        {
            return int.Parse(s);
        }
    }
}


