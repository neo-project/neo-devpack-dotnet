// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_UnboundedOperation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_UnboundedOperation : SmartContract.Framework.SmartContract
    {
        // Contains a loop that will produce backward jumps
        public static int Sum(int n)
        {
            int total = 0;
            for (int i = 0; i < n; i++)
            {
                total += i;
            }
            return total;
        }
    }
}
