// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Extensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public static class Ext
    {
        public static int sum(this int a, int b)
        {
            return a + b;
        }
    }

    public class Contract_Extensions : SmartContract.Framework.SmartContract
    {
        public static int TestSum(int a, int b)
        {
            return a.sum(b);
        }
    }
}
