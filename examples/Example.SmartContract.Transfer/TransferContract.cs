// Copyright (C) 2015-2024 The Neo Project.
//
// Contract1.cs file belongs to the neo project and is free
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

using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework.Services;

namespace Transfer;

/// <summary>
/// This is a sample contract that can be used as a template for creating new contracts.
/// </summary>
[DisplayName("Contract1")]
[ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
[ContractDescription("<Description Here>")]
[ContractVersion("1.0.0.0")]
[ContractSourceCode("https://github.com/cschuchardt88/neo-templates")]
[ContractPermission(Permission.WildCard, Method.WildCard)]
public class TransferContract : SmartContract
{

    [Hash160("NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP")]
    private static readonly UInt160 Owner = default;

    [DisplayName("_deploy")]
    public static void OnDeployment(object data, bool update)
    {
        if (update)
        {
            // Add logic for fixing contract on update
            return;
        }
        // Add logic here for 1st time deployed
    }

    // TODO: Allow ONLY contract owner to call update
    public static bool Update(ByteString nefFile, string manifest)
    {
        ContractManagement.Update(nefFile, manifest);
        return true;
    }

    // TODO: Allow ONLY contract owner to call destroy
    public static bool Destroy()
    {
        ContractManagement.Destroy();
        return true;
    }

    /// <summary>
    /// Transfer method that demonstrate how to transfer NEO and GAS
    /// </summary>
    /// <param name="to">Target address to send Neo and GAS</param>
    /// <param name="amount">Amount of tokens to be sent</param>
    public static void Transfer(UInt160 to, BigInteger amount)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(Owner));
        ExecutionEngine.Assert(NEO.Transfer(Runtime.ExecutingScriptHash, to, amount));
        ExecutionEngine.Assert(GAS.Transfer(Runtime.ExecutingScriptHash, to, GAS.BalanceOf(Runtime.ExecutingScriptHash), true));
    }
}
