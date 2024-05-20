using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleRoyaltyNEP11Token : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleRoyaltyNEP11Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":2068,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":2083,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":49,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":95,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":291,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":2098,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":496,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":526,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":619,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1004,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Any""}],""returntype"":""Void"",""offset"":1061,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":1178,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Any""}],""returntype"":""Void"",""offset"":1223,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1332,""safe"":false},{""name"":""currentCount"",""parameters"":[],""returntype"":""Integer"",""offset"":1462,""safe"":true},{""name"":""setRoyaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyInfos"",""type"":""Array""}],""returntype"":""Void"",""offset"":1487,""safe"":false},{""name"":""royaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyToken"",""type"":""Hash160""},{""name"":""salePrice"",""type"":""Integer""}],""returntype"":""Array"",""offset"":1777,""safe"":true},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1914,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1920,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1960,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample of NEP-11 Royalty Feature"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

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
    public abstract UInt160? Minter { [DisplayName("getMinter")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; }

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
    [DisplayName("setMinter")]
    public abstract void SetMinter(object? newMinter = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("setOwner")]
    public abstract void SetOwner(object? newOwner = null);

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
