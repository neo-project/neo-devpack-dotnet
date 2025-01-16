// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_ContractCall.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [Contract("0e26a6a9b6f37a54d5666aaa2efb71dc75abfdfa")]
    public class Contract_Call
    {
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        public static extern byte[] testArgs1(byte a);
        public static extern void testVoid();
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
    }

    public class Contract_ContractCall : SmartContract.Framework.SmartContract
    {
        public static byte[] testContractCall()
        {
            return Contract_Call.testArgs1(4);
        }

        public static void testContractCallVoid()
        {
            Contract_Call.testVoid();
        }
    }
}
