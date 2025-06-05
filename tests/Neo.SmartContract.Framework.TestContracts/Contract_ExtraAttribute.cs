// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_ExtraAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("E-mail", "dev@neo.org")]
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Neo Framework Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.SmartContract.Framework.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_ExtraAttribute : SmartContract
    {
        public static object Main2(string method, object[] args)
        {
            return true;
        }
    }
}

