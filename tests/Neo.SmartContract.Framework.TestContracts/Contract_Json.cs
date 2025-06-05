// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Json.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Neo Framework Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.SmartContract.Framework.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_Json : SmartContract
    {
        public static string Serialize(object obj)
        {
            return StdLib.JsonSerialize(obj);
        }

        public static object Deserialize(string json)
        {
            return StdLib.JsonDeserialize(json);
        }
    }
}

