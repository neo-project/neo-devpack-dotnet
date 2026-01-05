// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Concat.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Concat : SmartContract.Framework.SmartContract
    {
        public static string TestStringAdd1(string a)
        {
            return a + "hello";
        }

        public static string TestStringAdd2(string a, string b)
        {
            return a + b + "hello";
        }

        public static string TestStringAdd3(string a, string b, string c)
        {
            return a + b + c + "hello";
        }

        public static string TestStringAdd4(string a, string b, string c, string d)
        {
            return a + b + c + d + "hello";
        }
    }
}
