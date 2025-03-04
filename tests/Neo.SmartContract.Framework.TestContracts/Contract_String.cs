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

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
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
