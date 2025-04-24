// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_NativeContracts.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.TestContracts
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Compiler Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.Compiler.CSharp.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_NativeContracts : SmartContract.Framework.SmartContract
    {
        public static uint OracleMinimumResponseFee()
        {
            return Oracle.MinimumResponseFee;
        }

        public static string NEOSymbol()
        {
            return NEO.Symbol;
        }

        public static string GASSymbol()
        {
            return GAS.Symbol;
        }

        public static ECPoint[] getOracleNodes()
        {
            return RoleManagement.GetDesignatedByRole(Role.Oracle, 0);
        }

        public static UInt160 NEOHash()
        {
            return NEO.Hash;
        }


        public static UInt160 LedgerHash()
        {
            return Ledger.Hash;
        }


        public static UInt256 LedgerCurrentHash()
        {
            return Ledger.CurrentHash;
        }

        public static uint LedgerCurrentIndex()
        {
            return Ledger.CurrentIndex;
        }

    }
}


