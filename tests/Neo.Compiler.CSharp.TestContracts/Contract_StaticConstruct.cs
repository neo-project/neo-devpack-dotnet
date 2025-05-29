// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_StaticConstruct.cs file belongs to the neo project and is free
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
    public class Contract_StaticConstruct : SmartContract.Framework.SmartContract
    {
        static int a;
        //define and staticvar and initit with a runtime code.
        static Contract_StaticConstruct()
        {
            int b = 3;
            a = b + 1;
        }

        public static int TestStatic()
        {
            return a;
        }
    }
}


