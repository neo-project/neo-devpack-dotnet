// Copyright (C) 2015-2025 The Neo Project.
//
// Contract1.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract1 : SmartContract.Framework.SmartContract
    {
        private static string privateMethod()
        {
            return "NEO3";
        }

        public static byte[] unitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

        public static void testVoid()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
        }

        public static byte[] testArgs1(byte a)
        {
            var nb = new byte[] { 1, 2, 3, 3 };
            nb[3] = a;
            return nb;
        }

        public static byte[] testArgs2(byte[] a)
        {
            return a;
        }

        public static int testArgs3(int a, int b)
        {
            a = a + 2;
            return a;
        }

        public static int testArgs4(int a, int b)
        {
            a = a + 2;
            return a + b;
        }
    }
}
