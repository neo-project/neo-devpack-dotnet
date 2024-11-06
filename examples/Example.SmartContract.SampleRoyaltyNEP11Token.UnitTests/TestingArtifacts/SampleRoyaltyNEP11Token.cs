using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleRoyaltyNEP11Token(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleRoyaltyNEP11Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11"",""NEP-24""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":18,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":64,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":245,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":404,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":451,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":479,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":567,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":928,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1002,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":1115,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1177,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1282,""safe"":false},{""name"":""currentCount"",""parameters"":[],""returntype"":""Integer"",""offset"":1398,""safe"":true},{""name"":""setRoyaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyInfos"",""type"":""Array""}],""returntype"":""Void"",""offset"":1417,""safe"":false},{""name"":""royaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyToken"",""type"":""Hash160""},{""name"":""salePrice"",""type"":""Integer""}],""returntype"":""Array"",""offset"":1707,""safe"":true},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1866,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1872,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1910,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample of NEP-11 Royalty Feature"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

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
}
