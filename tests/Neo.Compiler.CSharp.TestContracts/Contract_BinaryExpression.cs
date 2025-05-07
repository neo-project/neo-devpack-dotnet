// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_BinaryExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Compiler Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.Compiler.CSharp.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_BinaryExpression : SmartContract.Framework.SmartContract
    {
        public static void BinaryIs()
        {
            ByteString a = "a";
            ExecutionEngine.Assert(a is ByteString);
            string b = $"";
#pragma warning disable CS0184
            ExecutionEngine.Assert(b is ByteString);
#pragma warning restore CS0184
        }

        public static void BinaryAs()
        {
            UInt160 a = UInt160.Zero;
            ExecutionEngine.Assert(a as ByteString == UInt160.Zero);
        }
    }
}


