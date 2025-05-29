// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_String.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Neo Framework Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.SmartContract.Framework.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_String : SmartContract
    {
        public static int TestStringAdd(string s1, string s2)
        {
            int a = 3;
            string c = s1 + s2;
            if (c == "hello")
            {
                a = 4;
            }
            else if (c == "world")
            {
                a = 5;
            }
            return a;
        }

        public static string TestStringAddInt(string s, int i)
        {
            return s + i;
        }
    }
}

