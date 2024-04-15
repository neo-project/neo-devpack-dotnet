using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":4371,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":4386,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":87,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":283,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":4401,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":521,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":551,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":644,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1029,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1082,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1037,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1145,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":1153,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":4416,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4431,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4446,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4461,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4476,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4491,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4506,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4521,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4536,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":4551,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4566,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4581,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2046,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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IEFybW9yDBVTdHVkZGVkIExlYXRoZXIgQXJtb3IMEERyYWdvbnNraW4gQXJtb3IMCkRlbW9uIEh1c2sMBVNoaXJ0DARSb2JlDApMaW5lbiBSb2JlDAlTaWxrIFJvYmUMC0RpdmluZSBSb2JlH8BnCgwESG9vZAwKTGluZW4gSG9vZAwJU2lsayBIb29kDAtEaXZpbmUgSG9vZAwFQ3Jvd24MA0NhcAwLTGVhdGhlciBDYXAMB1dhciBDYXAMDkRyYWdvbidzIENyb3duDAtEZW1vbiBDcm93bgwESGVsbQwJRnVsbCBIZWxtDApHcmVhdCBIZWxtDAtPcm5hdGUgSGVsbQwMQW5jaWVudCBIZWxtH8BnCwwEU2FzaAwKTGluZW4gU2FzaAwJV29vbCBTYXNoDAlTaWxrIFNhc2gMD0JyaWdodHNpbGsgU2FzaAwMTGVhdGhlciBCZWx0DBFIYXJkIExlYXRoZXIgQmVsdAwUU3R1ZGRlZCBMZWF0aGVyIEJlbHQMD0RyYWdvbnNraW4gQmVsdAwORGVtb25oaWRlIEJlbHQMCkhlYXZ5IEJlbHQMCU1lc2ggQmVsdAwLUGxhdGVkIEJlbHQMCFdhciBCZWx0DAtPcm5hdGUgQmVsdB/AZwwMBVNob2VzDAtMaW5lbiBTaG9lcwwKV29vbCBTaG9lcwwNU2lsayBTbGlwcGVycwwPRGl2aW5lIFNsaXBwZXJzDA1MZWF0aGVyIEJvb3RzDBJIYXJkIExlYXRoZXIgQm9vdHMMFVN0dWRkZWQgTGVhdGhlciBCb290cwwQRHJhZ29uc2tpbiBCb290cwwPRGVtb25oaWRlIEJvb3RzDAtIZWF2eSBCb290cwwLQ2hhaW4gQm9vdHMMB0dyZWF2ZXMMDk9ybmF0ZSBHcmVhdmVzDAxIb2x5IEdyZWF2ZXMfwGcNDAZHbG92ZXMMDExpbmVuIEdsb3ZlcwwLV29vbCBHbG92ZXMMC1NpbGsgR2xvdmVzDA1EaXZpbmUgR2xvdmVzDA5MZWF0aGVyIEdsb3ZlcwwTSGFyZCBMZWF0aGVyIEdsb3ZlcwwWU3R1ZGRlZCBMZWF0aGVyIEdsb3ZlcwwRRHJhZ29uc2tpbiBHbG92ZXMMDURlbW9uJ3MgSGFuZHMMDEhlYXZ5IEdsb3ZlcwwMQ2hhaW4gR2xvdmVzDAlHYXVudGxldHMMEE9ybmF0ZSBHYXVudGxldHMMDkhvbHkgR2F1bnRsZXRzH8BnDgwHUGVuZGFudAwGQW11bGV0DAhOZWNrbGFjZRPAZw8MDVRpdGFuaXVtIFJpbmcMDVBsYXRpbnVtIFJpbmcMC0Jyb256ZSBSaW5nDAtTaWx2ZXIgUmluZwwJR29sZCBSaW5nFcBnEAwMb2YgdGhlIFR3aW5zDA1vZiBSZWZsZWN0aW9uDAxvZiBEZXRlY3Rpb24MCm9mIHRoZSBGb3gMCm9mIFZpdHJpb2wMB29mIEZ1cnkMB29mIFJhZ2UMCG9mIEFuZ2VyDA1vZiBQcm90ZWN0aW9uDBBvZiBFbmxpZ2h0ZW5tZW50DA1vZiBCcmlsbGlhbmNlDA1vZiBQZXJmZWN0aW9uDAhvZiBTa2lsbAwJb2YgVGl0YW5zDAlvZiBHaWFudHMMCG9mIFBvd2VyIMBmDApTaGltbWVyaW5nDAdMaWdodCdzDAVXcmF0aAwDV29lDAZWb3J0ZXgMBVZpcGVyDAdWaWN0b3J5DAlWZW5nZWFuY2UMB1Rvcm1lbnQMB1RlbXBlc3QMBVN0b3JtDAZTcGlyaXQMBlNvcnJvdwwEU291bAwDU29sDAVTa3VsbAwEUnVuZQwHUmFwdHVyZQwEUmFnZQwGUGxhZ3VlDAdQaG9lbml4DAtQYW5kZW1vbml1bQwEUGFpbgwJT25zbGF1Z2h0DAhPYmxpdmlvbgwGTW9yYmlkDAdNaXJhY2xlDARNaW5kDAlNYWVsc3Ryb20MBUxvYXRoDAZLcmFrZW4MCEh5cG5vdGljDAZIb3Jyb3IMBkhvbm91cgwFSGF2b2MMBEhhdGUMBEdyaW0MBUdvbGVtDAVHbHlwaAwFR2xvb20MBUdob3VsDARHYWxlDANGb2UMBEZhdGUMCEVtcHlyZWFuDAVFYWdsZQwERHVzawwERG9vbQwFRHJlYWQMBkRyYWdvbgwERGlyZQwFRGVtb24MBURlYXRoDAlEYW1uYXRpb24MCkNvcnJ1cHRpb24MBkNvcnBzZQwIQ2hpbWVyaWMMCUNhdGFjbHlzbQwHQ2FycmlvbgwFQnJvb2QMCUJyaW1zdG9uZQwHQnJhbWJsZQwFQmxvb2QMBkJsaWdodAwIQmVoZW1vdGgMBUJlYXN0DApBcm1hZ2VkZG9uDApBcG9jYWx5cHNlDAVBZ29ueQBFwGcHDARNb29uDANTdW4MBEZvcm0MBFBlYWsMBFRlYXIMBUdyb3dsDAVTaG91dAwHV2hpc3BlcgwGU2hhZG93DAZCZW5kZXIMBEdsb3cMCkluc3RydW1lbnQMBUdyYXNwDARSb2FyDARTb25nDARCaXRlDARSb290DARCYW5lABLAZwgKyvD//wot7///CgTv//8TwGAKAAAAAAob7///CxPAYUDCSljPSjXz7v//I+Pu///CSlnPSjXr7v//I/ju///CSljPSjXV7v//I4Hw///CSljPSjXG7v//Izvz///CSljPSjW37v//I17z///CSljPSjWo7v//Iz/0///CSljPSjWZ7v//I0L0///CSljPSjWK7v//I0X0///CSljPSjV77v//I0j0///CSljPSjVs7v//I0v0///CSljPSjVd7v//I070///CSljPSjVO7v//I1H0///CSljPSjU/7v//I1T0///CSljPSjUw7v//I3r0///CSljPSjUh7v//I7X1////vQKN"));

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

    #region Constructor for internal use only

    protected SampleLootNFT(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
