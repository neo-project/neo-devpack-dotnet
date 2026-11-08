using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":8,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":48,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":284,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":445,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":562,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":595,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":684,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1750,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1812,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1758,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1931,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":1980,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2026,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2087,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2289,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2306,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2323,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2340,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2357,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2374,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":2391,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":2408,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":3368,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":3732,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":3862,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""isContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""Version"":""3.10.1"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"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UGxhdGUgTWFpbAwRT3JuYXRlIENoZXN0cGxhdGUMD0hvbHkgQ2hlc3RwbGF0ZQwNTGVhdGhlciBBcm1vcgwSSGFyZCBMZWF0aGVyIEFybW9yDBVTdHVkZGVkIExlYXRoZXIgQXJtb3IMEERyYWdvbnNraW4gQXJtb3IMCkRlbW9uIEh1c2sMBVNoaXJ0DARSb2JlDApMaW5lbiBSb2JlDAlTaWxrIFJvYmUMC0RpdmluZSBSb2JlH8BnCAwESG9vZAwKTGluZW4gSG9vZAwJU2lsayBIb29kDAtEaXZpbmUgSG9vZAwFQ3Jvd24MA0NhcAwLTGVhdGhlciBDYXAMB1dhciBDYXAMDkRyYWdvbidzIENyb3duDAtEZW1vbiBDcm93bgwESGVsbQwJRnVsbCBIZWxtDApHcmVhdCBIZWxtDAtPcm5hdGUgSGVsbQwMQW5jaWVudCBIZWxtH8BnCQwEU2FzaAwKTGluZW4gU2FzaAwJV29vbCBTYXNoDAlTaWxrIFNhc2gMD0JyaWdodHNpbGsgU2FzaAwMTGVhdGhlciBCZWx0DBFIYXJkIExlYXRoZXIgQmVsdAwUU3R1ZGRlZCBMZWF0aGVyIEJlbHQMD0RyYWdvbnNraW4gQmVsdAwORGVtb25oaWRlIEJlbHQMCkhlYXZ5IEJlbHQMCU1lc2ggQmVsdAwLUGxhdGVkIEJlbHQMCFdhciBCZWx0DAtPcm5hdGUgQmVsdB/AZwoMBVNob2VzDAtMaW5lbiBTaG9lcwwKV29vbCBTaG9lcwwNU2lsayBTbGlwcGVycwwPRGl2aW5lIFNsaXBwZXJzDA1MZWF0aGVyIEJvb3RzDBJIYXJkIExlYXRoZXIgQm9vdHMMFVN0dWRkZWQgTGVhdGhlciBCb290cwwQRHJhZ29uc2tpbiBCb290cwwPRGVtb25oaWRlIEJvb3RzDAtIZWF2eSBCb290cwwLQ2hhaW4gQm9vdHMMB0dyZWF2ZXMMDk9ybmF0ZSBHcmVhdmVzDAxIb2x5IEdyZWF2ZXMfwGcLDAZHbG92ZXMMDExpbmVuIEdsb3ZlcwwLV29vbCBHbG92ZXMMC1NpbGsgR2xvdmVzDA1EaXZpbmUgR2xvdmVzDA5MZWF0aGVyIEdsb3ZlcwwTSGFyZCBMZWF0aGVyIEdsb3ZlcwwWU3R1ZGRlZCBMZWF0aGVyIEdsb3ZlcwwRRHJhZ29uc2tpbiBHbG92ZXMMDURlbW9uJ3MgSGFuZHMMDEhlYXZ5IEdsb3ZlcwwMQ2hhaW4gR2xvdmVzDAlHYXVudGxldHMMEE9ybmF0ZSBHYXVudGxldHMMDkhvbHkgR2F1bnRsZXRzH8BnDAwHUGVuZGFudAwGQW11bGV0DAhOZWNrbGFjZRPAZw0MDVRpdGFuaXVtIFJpbmcMDVBsYXRpbnVtIFJpbmcMC0Jyb256ZSBSaW5nDAtTaWx2ZXIgUmluZwwJR29sZCBSaW5nFcBnDgwMb2YgdGhlIFR3aW5zDA1vZiBSZWZsZWN0aW9uDAxvZiBEZXRlY3Rpb24MCm9mIHRoZSBGb3gMCm9mIFZpdHJpb2wMB29mIEZ1cnkMB29mIFJhZ2UMCG9mIEFuZ2VyDA1vZiBQcm90ZWN0aW9uDBBvZiBFbmxpZ2h0ZW5tZW50DA1vZiBCcmlsbGlhbmNlDA1vZiBQZXJmZWN0aW9uDAhvZiBTa2lsbAwJb2YgVGl0YW5zDAlvZiBHaWFudHMMCG9mIFBvd2VyIMBkDApTaGltbWVyaW5nDAdMaWdodCdzDAVXcmF0aAwDV29lDAZWb3J0ZXgMBVZpcGVyDAdWaWN0b3J5DAlWZW5nZWFuY2UMB1Rvcm1lbnQMB1RlbXBlc3QMBVN0b3JtDAZTcGlyaXQMBlNvcnJvdwwEU291bAwDU29sDAVTa3VsbAwEUnVuZQwHUmFwdHVyZQwEUmFnZQwGUGxhZ3VlDAdQaG9lbml4DAtQYW5kZW1vbml1bQwEUGFpbgwJT25zbGF1Z2h0DAhPYmxpdmlvbgwGTW9yYmlkDAdNaXJhY2xlDARNaW5kDAlNYWVsc3Ryb20MBUxvYXRoDAZLcmFrZW4MCEh5cG5vdGljDAZIb3Jyb3IMBkhvbm91cgwFSGF2b2MMBEhhdGUMBEdyaW0MBUdvbGVtDAVHbHlwaAwFR2xvb20MBUdob3VsDARHYWxlDANGb2UMBEZhdGUMCEVtcHlyZWFuDAVFYWdsZQwERHVzawwERG9vbQwFRHJlYWQMBkRyYWdvbgwERGlyZQwFRGVtb24MBURlYXRoDAlEYW1uYXRpb24MCkNvcnJ1cHRpb24MBkNvcnBzZQwIQ2hpbWVyaWMMCUNhdGFjbHlzbQwHQ2FycmlvbgwFQnJvb2QMCUJyaW1zdG9uZQwHQnJhbWJsZQwFQmxvb2QMBkJsaWdodAwIQmVoZW1vdGgMBUJlYXN0DApBcm1hZ2VkZG9uDApBcG9jYWx5cHNlDAVBZ29ueQBFwGUMBE1vb24MA1N1bgwERm9ybQwEUGVhawwEVGVhcgwFR3Jvd2wMBVNob3V0DAdXaGlzcGVyDAZTaGFkb3cMBkJlbmRlcgwER2xvdwwKSW5zdHJ1bWVudAwFR3Jhc3AMBFJvYXIMBFNvbmcMBEJpdGUMBFJvb3QMBEJhbmUAEsBmQNBzAMA=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Events

    public delegate void delEventMsg(string? obj);

    [DisplayName("EventMsg")]
    public event delEventMsg? OnEventMsg;

    public delegate void delTransfer(UInt160? from, UInt160? to, BigInteger? amount, byte[]? tokenId);

    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

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
    [DisplayName("getChest")]
    public abstract string? GetChest(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getCredential")]
    public abstract BigInteger? GetCredential(BigInteger? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getFoot")]
    public abstract string? GetFoot(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getHand")]
    public abstract string? GetHand(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getHead")]
    public abstract string? GetHead(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getNeck")]
    public abstract string? GetNeck(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getRing")]
    public abstract string? GetRing(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getWaist")]
    public abstract string? GetWaist(BigInteger? credential);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getWeapon")]
    public abstract string? GetWeapon(BigInteger? credential);

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
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("tokenURI")]
    public abstract string? TokenURI(BigInteger? tokenId);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("claim")]
    public abstract void Claim(BigInteger? tokenId);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ownerClaim")]
    public abstract void OwnerClaim(BigInteger? tokenId);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("setOwner")]
    public abstract UInt160? SetOwner(UInt160? newOwner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("verify")]
    public abstract bool? Verify();

    #endregion
}
