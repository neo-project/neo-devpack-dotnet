// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_ExecutionEngine.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Neo Framework Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.SmartContract.Framework.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_ExecutionEngine : SmartContract
    {
        public static byte[] CallingScriptHash()
        {
            return (byte[])Runtime.CallingScriptHash;
        }

        public static byte[] EntryScriptHash()
        {
            return (byte[])Runtime.EntryScriptHash;
        }

        public static byte[] ExecutingScriptHash()
        {
            return (byte[])Runtime.ExecutingScriptHash;
        }

#pragma warning disable CS0618
        public static object ScriptContainer()
        {
            return Runtime.ScriptContainer;
        }
#pragma warning restore CS0618

        public static object Transaction()
        {
            return Runtime.Transaction;
        }
    }
}

