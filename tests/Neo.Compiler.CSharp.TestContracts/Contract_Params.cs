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

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Compiler Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.Compiler.CSharp.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
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


