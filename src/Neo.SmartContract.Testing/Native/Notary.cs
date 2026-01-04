// Copyright (C) 2015-2026 The Neo Project.
//
// Notary.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class Notary(SmartContractInitialize initialize) : SmartContract(initialize)
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Notary"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-27""],""abi"":{""methods"":[{""name"":""balanceOf"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""expirationOf"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""getMaxNotValidBeforeDelta"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":true},{""name"":""lockDepositUntil"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""till"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":21,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":28,""safe"":false},{""name"":""setMaxNotValidBeforeDelta"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":35,""safe"":false},{""name"":""verify"",""parameters"":[{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":42,""safe"":true},{""name"":""withdraw"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":49,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? MaxNotValidBeforeDelta { [DisplayName("getMaxNotValidBeforeDelta")] get; [DisplayName("setMaxNotValidBeforeDelta")] set; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("expirationOf")]
    public abstract BigInteger? ExpirationOf(UInt160? account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("verify")]
    public abstract bool? Verify(byte[]? signature);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("lockDepositUntil")]
    public abstract bool? LockDepositUntil(UInt160? account, BigInteger? till);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("withdraw")]
    public abstract bool? Withdraw(UInt160? from, UInt160? to);

    #endregion
}
