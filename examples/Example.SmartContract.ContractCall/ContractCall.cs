// Copyright (C) 2015-2024 The Neo Project.
//
// ContractCall.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.ComponentModel;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace ContractCall;

[DisplayName("SampleContractCall")]
[ContractAuthor("core-dev")]
[ContractEmail("core@neo.org")]
[ContractVersion("0.0.1")]
[ContractDescription("A sample contract to demonstrate how to call a contract")]
[ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/examples/Example.SmartContract.ContractCall")]
public class SampleContractCall : SmartContract
{
    [Hash160("0x13a83e059c2eedd5157b766d3357bc826810905e")]
    private static readonly UInt160 DummyTarget;

    public static void onNEP17Payment(UInt160 from, BigInteger amount, BigInteger data)
    {
        UInt160 tokenHash = Runtime.CallingScriptHash;
        if (!data.Equals(123)) return;
        UInt160 @this = Runtime.ExecutingScriptHash;
        BigInteger balanceOf = (BigInteger)Contract.Call(tokenHash, "balanceOf", CallFlags.All, @this);
        Contract.Call(DummyTarget, "dummyMethod", CallFlags.All, @this, tokenHash, balanceOf);
    }
}
