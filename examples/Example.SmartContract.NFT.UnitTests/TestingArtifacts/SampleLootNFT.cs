using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":5969,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":5984,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":87,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":345,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":5999,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":621,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":659,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":761,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1561,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1625,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1569,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1701,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":1713,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":6014,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6029,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6044,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6059,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6074,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6089,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6104,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6119,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":6134,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":6149,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":6164,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":6179,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":3600,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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QZUBb2FAQS1RCDBAVwIDeXg0MXBoeXo0cHFpeUoSzhLONjXo9///DAV0YWtlbnlKEs4SzjZfEcFFU4tQQeY/GIRAVwACeUoSzhLONl8RwUVTi1BBkl3oMQwFdGFrZW6YDBZUb2tlbiBhbHJlYWR5IGNsYWltZWQu4UFr3qkoIgJAQWveqShAVwADEBALCxTAenl4E000A0BXAAR4NDl5SngQUdBFekp4ElHQRXtKeBNR0EUMEE4zIFNlY3VyZSBMb290ICN4Es43BgCL2yhKeBFR0EVAVwABQFcBAgFhHrckBQkiBgFBH7UMEFRva2VuIElEIGludmFsaWThNUb4//9waHg1CP///8JKDBNPd25lciBtaW50cyBzdWNjZXNzzwwIRXZlbnRNc2dBlQFvYUBWEgwUnD0XfAe1ZXMhF5tlcu2fdsJz9opkQZv2Z84AFRGIThBR0FASwGMAFhGIThBR0EGb9mfOEsBnERMRiE4QUdBBm/ZnzhLAZQwEQm9vawwEVG9tZQwJQ2hyb25pY2xlDAhHcmltb2lyZQwEV2FuZAwJQm9uZSBXYW5kDApHcmF2ZSBXYW5kDApHaG9zdCBXYW5kDAtTaG9ydCBTd29yZAwKTG9uZyBTd29yZAwIU2NpbWl0YXIMCEZhbGNoaW9uDAZLYXRhbmEMBENsdWIMBE1hY2UMBE1hdWwMDFF1YXJ0ZXJzdGFmZgwJV2FyaGFtbWVyABLAZwkMCVJpbmcgTWFpbAwKQ2hhaW4gTWFpbAwKUGxhdGUgTWFpbAwRT3JuYXRlIENoZXN0cGxhdGUMD0hvbHkgQ2hlc3RwbGF0ZQwNTGVhdGhlciBBcm1vcgwSSGFyZCBMZWF0aGVyIEFybW9yDBVTdHVkZGVkIExlYXRoZXIgQXJtb3IMEERyYWdvbnNraW4gQXJtb3IMCkRlbW9uIEh1c2sMBVNoaXJ0DARSb2JlDApMaW5lbiBSb2JlDAlTaWxrIFJvYmUMC0RpdmluZSBSb2JlH8BnCgwESG9vZAwKTGluZW4gSG9vZAwJU2lsayBIb29kDAtEaXZpbmUgSG9vZAwFQ3Jvd24MA0NhcAwLTGVhdGhlciBDYXAMB1dhciBDYXAMDkRyYWdvbidzIENyb3duDAtEZW1vbiBDcm93bgwESGVsbQwJRnVsbCBIZWxtDApHcmVhdCBIZWxtDAtPcm5hdGUgSGVsbQwMQW5jaWVudCBIZWxtH8BnCwwEU2FzaAwKTGluZW4gU2FzaAwJV29vbCBTYXNoDAlTaWxrIFNhc2gMD0JyaWdodHNpbGsgU2FzaAwMTGVhdGhlciBCZWx0DBFIYXJkIExlYXRoZXIgQmVsdAwUU3R1ZGRlZCBMZWF0aGVyIEJlbHQMD0RyYWdvbnNraW4gQmVsdAwORGVtb25oaWRlIEJlbHQMCkhlYXZ5IEJlbHQMCU1lc2ggQmVsdAwLUGxhdGVkIEJlbHQMCFdhciBCZWx0DAtPcm5hdGUgQmVsdB/AZwwMBVNob2VzDAtMaW5lbiBTaG9lcwwKV29vbCBTaG9lcwwNU2lsayBTbGlwcGVycwwPRGl2aW5lIFNsaXBwZXJzDA1MZWF0aGVyIEJvb3RzDBJIYXJkIExlYXRoZXIgQm9vdHMMFVN0dWRkZWQgTGVhdGhlciBCb290cwwQRHJhZ29uc2tpbiBCb290cwwPRGVtb25oaWRlIEJvb3RzDAtIZWF2eSBCb290cwwLQ2hhaW4gQm9vdHMMB0dyZWF2ZXMMDk9ybmF0ZSBHcmVhdmVzDAxIb2x5IEdyZWF2ZXMfwGcNDAZHbG92ZXMMDExpbmVuIEdsb3ZlcwwLV29vbCBHbG92ZXMMC1NpbGsgR2xvdmVzDA1EaXZpbmUgR2xvdmVzDA5MZWF0aGVyIEdsb3ZlcwwTSGFyZCBMZWF0aGVyIEdsb3ZlcwwWU3R1ZGRlZCBMZWF0aGVyIEdsb3ZlcwwRRHJhZ29uc2tpbiBHbG92ZXMMDURlbW9uJ3MgSGFuZHMMDEhlYXZ5IEdsb3ZlcwwMQ2hhaW4gR2xvdmVzDAlHYXVudGxldHMMEE9ybmF0ZSBHYXVudGxldHMMDkhvbHkgR2F1bnRsZXRzH8BnDgwHUGVuZGFudAwGQW11bGV0DAhOZWNrbGFjZRPAZw8MDVRpdGFuaXVtIFJpbmcMDVBsYXRpbnVtIFJpbmcMC0Jyb256ZSBSaW5nDAtTaWx2ZXIgUmluZwwJR29sZCBSaW5nFcBnEAwMb2YgdGhlIFR3aW5zDA1vZiBSZWZsZWN0aW9uDAxvZiBEZXRlY3Rpb24MCm9mIHRoZSBGb3gMCm9mIFZpdHJpb2wMB29mIEZ1cnkMB29mIFJhZ2UMCG9mIEFuZ2VyDA1vZiBQcm90ZWN0aW9uDBBvZiBFbmxpZ2h0ZW5tZW50DA1vZiBCcmlsbGlhbmNlDA1vZiBQZXJmZWN0aW9uDAhvZiBTa2lsbAwJb2YgVGl0YW5zDAlvZiBHaWFudHMMCG9mIFBvd2VyIMBmDApTaGltbWVyaW5nDAdMaWdodCdzDAVXcmF0aAwDV29lDAZWb3J0ZXgMBVZpcGVyDAdWaWN0b3J5DAlWZW5nZWFuY2UMB1Rvcm1lbnQMB1RlbXBlc3QMBVN0b3JtDAZTcGlyaXQMBlNvcnJvdwwEU291bAwDU29sDAVTa3VsbAwEUnVuZQwHUmFwdHVyZQwEUmFnZQwGUGxhZ3VlDAdQaG9lbml4DAtQYW5kZW1vbml1bQwEUGFpbgwJT25zbGF1Z2h0DAhPYmxpdmlvbgwGTW9yYmlkDAdNaXJhY2xlDARNaW5kDAlNYWVsc3Ryb20MBUxvYXRoDAZLcmFrZW4MCEh5cG5vdGljDAZIb3Jyb3IMBkhvbm91cgwFSGF2b2MMBEhhdGUMBEdyaW0MBUdvbGVtDAVHbHlwaAwFR2xvb20MBUdob3VsDARHYWxlDANGb2UMBEZhdGUMCEVtcHlyZWFuDAVFYWdsZQwERHVzawwERG9vbQwFRHJlYWQMBkRyYWdvbgwERGlyZQwFRGVtb24MBURlYXRoDAlEYW1uYXRpb24MCkNvcnJ1cHRpb24MBkNvcnBzZQwIQ2hpbWVyaWMMCUNhdGFjbHlzbQwHQ2FycmlvbgwFQnJvb2QMCUJyaW1zdG9uZQwHQnJhbWJsZQwFQmxvb2QMBkJsaWdodAwIQmVoZW1vdGgMBUJlYXN0DApBcm1hZ2VkZG9uDApBcG9jYWx5cHNlDAVBZ29ueQBFwGcHDARNb29uDANTdW4MBEZvcm0MBFBlYWsMBFRlYXIMBUdyb3dsDAVTaG91dAwHV2hpc3BlcgwGU2hhZG93DAZCZW5kZXIMBEdsb3cMCkluc3RydW1lbnQMBUdyYXNwDARSb2FyDARTb25nDARCaXRlDARSb290DARCYW5lABLAZwgKCuv//wob6f//CvLo//8TwGAKDwAAAAoJ6f//CxPAYUBXAgITEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxyGkRzktT0CICQMJKWM9KNbXo//8jpej//8JKWc9KNa3o//8juuj//8JKWM9KNZfo//8jler//8JKWM9KNYjo//8jMe///8JKWM9KNXno//8jWe///8JKWM9KNWro//8jOvD//8JKWM9KNVvo//8jPfD//8JKWM9KNUzo//8jQPD//8JKWM9KNT3o//8jQ/D//8JKWM9KNS7o//8jRvD//8JKWM9KNR/o//8jSfD//8JKWM9KNRDo//8jTPD//8JKWM9KNQHo//8jT/D//8JKWM9KNfLn//8jQfT//8JKWM9KNePn//8ji/X//xW3460="));

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
