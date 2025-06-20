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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleRoyaltyNEP11Token"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11"",""NEP-24""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":18,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":56,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":318,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":499,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":550,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":588,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":684,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1478,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1541,""safe"":false},{""name"":""getMinter"",""parameters"":[],""returntype"":""Hash160"",""offset"":1670,""safe"":true},{""name"":""setMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1715,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":1827,""safe"":false},{""name"":""currentCount"",""parameters"":[],""returntype"":""Integer"",""offset"":1946,""safe"":true},{""name"":""setRoyaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyInfos"",""type"":""Array""}],""returntype"":""Void"",""offset"":1973,""safe"":false},{""name"":""royaltyInfo"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""royaltyToken"",""type"":""Hash160""},{""name"":""salePrice"",""type"":""Integer""}],""returntype"":""Array"",""offset"":2275,""safe"":true},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":2432,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":2438,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2482,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}]},{""name"":""SetMinter"",""parameters"":[{""name"":""newMinter"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample of NEP-11 Royalty Feature"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
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
