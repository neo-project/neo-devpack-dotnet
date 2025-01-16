// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_GoTo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_GoTo : SmartContract.Framework.SmartContract
    {
        public static int test()
        {
            int a = 1;
        sum:
            a++;
            if (a == 3) return a;

            goto sum;
        }

        public static int testTry()
        {
            int a = 1;
        sum:
            try
            {
                a++;
                if (a == 3) return a;
            }
            catch { }
            goto sum;
        }
    }
}
