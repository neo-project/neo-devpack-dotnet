// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_CheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
#pragma warning disable CS8625
    public class Contract_CheckWitness : SmartContract.Framework.SmartContract
    {
        public static void Main(UInt160 u)
        {
            Runtime.CheckWitness(u);
            ExecutionEngine.Assert(Runtime.CheckWitness(u));
        }
#pragma warning restore CS8625
    }
}
