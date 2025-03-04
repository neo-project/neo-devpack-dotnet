// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Assignment.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Assignment : SmartContract.Framework.SmartContract
    {
        public static void TestAssignment()
        {
            int a = 1;
            ExecutionEngine.Assert(a == 1);
            int b;
            a = b = 2;
            ExecutionEngine.Assert(a == 2);
            ExecutionEngine.Assert(b == 2);
        }

        public static void TestCoalesceAssignment()
        {
            int? a = null;
            a ??= 1;
            ExecutionEngine.Assert(a == 1);
            a ??= 2;
            ExecutionEngine.Assert(a == 1);
        }
    }
}
