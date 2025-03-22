using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleRoyaltyNEP11Token(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleRoyaltyNEP11Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11"",""NEP-24""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":18,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":56,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":234,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":393,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":440,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":468,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":550,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":915,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":989,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":1106,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1168,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1277,""safe"":false},{""name"":""currentCount"",""parameters"":[],""returntype"":""Integer"",""offset"":1395,""safe"":true},{""name"":""setRoyaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyInfos"",""type"":""Array""}],""returntype"":""Void"",""offset"":1415,""safe"":false},{""name"":""royaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyToken"",""type"":""Hash160""},{""name"":""salePrice"",""type"":""Integer""}],""returntype"":""Array"",""offset"":1709,""safe"":true},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1868,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1874,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1912,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample of NEP-11 Royalty Feature"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"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").AsSerializable<Neo.SmartContract.NefFile>();

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
