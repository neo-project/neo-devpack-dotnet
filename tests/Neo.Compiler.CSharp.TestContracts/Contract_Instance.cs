// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Instance.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Instance : SmartContract.Framework.SmartContract
    {
        public int init = 0;

        public Contract_Instance()
        {
            init++;
        }

        public int sum(int a)
        {
            return a + init;
        }

        public int sum2(int a)
        {
            return sum(a) + sum(a);
        }
    }
}
