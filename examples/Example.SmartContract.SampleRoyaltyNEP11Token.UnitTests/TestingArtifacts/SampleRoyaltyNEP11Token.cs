using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleRoyaltyNEP11Token : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleRoyaltyNEP11Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11"",""NEP-24""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":2009,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":2024,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":49,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":95,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":281,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":2039,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":482,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":510,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":599,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":974,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1029,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":1142,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1185,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1290,""safe"":false},{""name"":""currentCount"",""parameters"":[],""returntype"":""Integer"",""offset"":1415,""safe"":true},{""name"":""setRoyaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyInfos"",""type"":""Array""}],""returntype"":""Void"",""offset"":1438,""safe"":false},{""name"":""royaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyToken"",""type"":""Hash160""},{""name"":""salePrice"",""type"":""Integer""}],""returntype"":""Array"",""offset"":1722,""safe"":true},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1859,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1865,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1901,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample of NEP-11 Royalty Feature"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Events

    public delegate void delSetMinter(UInt160? newMinter);

    [DisplayName("SetMinter")]
    public event delSetMinter? OnSetMinter;

    public delegate void delSetOwner(UInt160? newOwner);

    [DisplayName("SetOwner")]
    public event delSetOwner? OnSetOwner;

    public delegate void delTransfer(UInt160? from, UInt160? to, BigInteger? amount, byte[]? tokenId);

    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? CurrentCount { [DisplayName("currentCount")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Minter { [DisplayName("getMinter")] get; [DisplayName("setMinter")] set; }

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
    public abstract object? Tokens { [DisplayName("tokens")] get; }

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

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("royaltyInfo")]
    public abstract IList<object>? RoyaltyInfo(byte[]? tokenId, UInt160? royaltyToken, BigInteger? salePrice);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("setRoyaltyInfo")]
    public abstract void SetRoyaltyInfo(byte[]? tokenId, IList<object>? royaltyInfos);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract bool? Update(byte[]? nefFile, byte[]? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected SampleRoyaltyNEP11Token(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
