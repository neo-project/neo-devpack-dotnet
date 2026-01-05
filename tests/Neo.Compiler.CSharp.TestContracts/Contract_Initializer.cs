// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Initializer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Initializer : SmartContract.Framework.SmartContract
    {
        public class Data
        {
            public int A = 1;
            public int B = 2;
        }

        public int sum()
        {
            var x = new Data();
            return x.A + x.B;
        }

        public int sum1(int a, int b)
        {
            var x = new Data
            {
                A = a,
                B = b
            };
            return x.A + x.B;
        }

        public int sum2(int a, int b)
        {
            var x = new Data();
            x.A = a;
            x.B = b;
            return x.A + x.B;
        }

        public void anonymousObjectCreation()
        {
            var v = new { Amount = 108, Message = "Hello" };
            Runtime.Log(v.Message);
            var anonArray = new[] { new { name = "apple", diam = 4 }, new { name = "grape", diam = 1 } };
            Runtime.Log(anonArray[0].name);
        }
    }
}
