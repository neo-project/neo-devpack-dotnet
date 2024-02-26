using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17TemplateContract : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":1308,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1323,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":89,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":353,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":799,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":855,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":993,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1035,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1077,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":1083,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":1109,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1247,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1292,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    [DisplayName("Transfer")]
    public event Neo.SmartContract.Testing.TestingStandards.INep17Standard.delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName("verify")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected Nep17TemplateContract(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
