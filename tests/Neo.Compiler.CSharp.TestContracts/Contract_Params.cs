// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Params.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public sealed class Contract_Params : SmartContract.Framework.SmartContract
    {
        private static int Sum(params int[] args)
        {
            int sum = 0;
            foreach (int x in args)
                sum += x;
            return sum;
        }

        public static int Test()
        {
            return Sum() + Sum(1) + Sum(2, 3) + Sum(new int[] { 4, 5 });
        }
    }
}
