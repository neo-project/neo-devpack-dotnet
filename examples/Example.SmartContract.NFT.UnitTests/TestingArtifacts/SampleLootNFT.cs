using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":4367,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":4382,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":87,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":283,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":4397,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":521,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":551,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":644,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":1029,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1082,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":1037,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1145,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":1153,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":4412,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4427,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4442,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4457,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4472,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4487,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4502,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4517,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4532,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":4547,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4562,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4577,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2042,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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b3IMFVN0dWRkZWQgTGVhdGhlciBBcm1vcgwQRHJhZ29uc2tpbiBBcm1vcgwKRGVtb24gSHVzawwFU2hpcnQMBFJvYmUMCkxpbmVuIFJvYmUMCVNpbGsgUm9iZQwLRGl2aW5lIFJvYmUfwGcKDARIb29kDApMaW5lbiBIb29kDAlTaWxrIEhvb2QMC0RpdmluZSBIb29kDAVDcm93bgwDQ2FwDAtMZWF0aGVyIENhcAwHV2FyIENhcAwORHJhZ29uJ3MgQ3Jvd24MC0RlbW9uIENyb3duDARIZWxtDAlGdWxsIEhlbG0MCkdyZWF0IEhlbG0MC09ybmF0ZSBIZWxtDAxBbmNpZW50IEhlbG0fwGcLDARTYXNoDApMaW5lbiBTYXNoDAlXb29sIFNhc2gMCVNpbGsgU2FzaAwPQnJpZ2h0c2lsayBTYXNoDAxMZWF0aGVyIEJlbHQMEUhhcmQgTGVhdGhlciBCZWx0DBRTdHVkZGVkIExlYXRoZXIgQmVsdAwPRHJhZ29uc2tpbiBCZWx0DA5EZW1vbmhpZGUgQmVsdAwKSGVhdnkgQmVsdAwJTWVzaCBCZWx0DAtQbGF0ZWQgQmVsdAwIV2FyIEJlbHQMC09ybmF0ZSBCZWx0H8BnDAwFU2hvZXMMC0xpbmVuIFNob2VzDApXb29sIFNob2VzDA1TaWxrIFNsaXBwZXJzDA9EaXZpbmUgU2xpcHBlcnMMDUxlYXRoZXIgQm9vdHMMEkhhcmQgTGVhdGhlciBCb290cwwVU3R1ZGRlZCBMZWF0aGVyIEJvb3RzDBBEcmFnb25za2luIEJvb3RzDA9EZW1vbmhpZGUgQm9vdHMMC0hlYXZ5IEJvb3RzDAtDaGFpbiBCb290cwwHR3JlYXZlcwwOT3JuYXRlIEdyZWF2ZXMMDEhvbHkgR3JlYXZlcx/AZw0MBkdsb3ZlcwwMTGluZW4gR2xvdmVzDAtXb29sIEdsb3ZlcwwLU2lsayBHbG92ZXMMDURpdmluZSBHbG92ZXMMDkxlYXRoZXIgR2xvdmVzDBNIYXJkIExlYXRoZXIgR2xvdmVzDBZTdHVkZGVkIExlYXRoZXIgR2xvdmVzDBFEcmFnb25za2luIEdsb3ZlcwwNRGVtb24ncyBIYW5kcwwMSGVhdnkgR2xvdmVzDAxDaGFpbiBHbG92ZXMMCUdhdW50bGV0cwwQT3JuYXRlIEdhdW50bGV0cwwOSG9seSBHYXVudGxldHMfwGcODAdQZW5kYW50DAZBbXVsZXQMCE5lY2tsYWNlE8BnDwwNVGl0YW5pdW0gUmluZwwNUGxhdGludW0gUmluZwwLQnJvbnplIFJpbmcMC1NpbHZlciBSaW5nDAlHb2xkIFJpbmcVwGcQDAxvZiB0aGUgVHdpbnMMDW9mIFJlZmxlY3Rpb24MDG9mIERldGVjdGlvbgwKb2YgdGhlIEZveAwKb2YgVml0cmlvbAwHb2YgRnVyeQwHb2YgUmFnZQwIb2YgQW5nZXIMDW9mIFByb3RlY3Rpb24MEG9mIEVubGlnaHRlbm1lbnQMDW9mIEJyaWxsaWFuY2UMDW9mIFBlcmZlY3Rpb24MCG9mIFNraWxsDAlvZiBUaXRhbnMMCW9mIEdpYW50cwwIb2YgUG93ZXIgwGYMClNoaW1tZXJpbmcMB0xpZ2h0J3MMBVdyYXRoDANXb2UMBlZvcnRleAwFVmlwZXIMB1ZpY3RvcnkMCVZlbmdlYW5jZQwHVG9ybWVudAwHVGVtcGVzdAwFU3Rvcm0MBlNwaXJpdAwGU29ycm93DARTb3VsDANTb2wMBVNrdWxsDARSdW5lDAdSYXB0dXJlDARSYWdlDAZQbGFndWUMB1Bob2VuaXgMC1BhbmRlbW9uaXVtDARQYWluDAlPbnNsYXVnaHQMCE9ibGl2aW9uDAZNb3JiaWQMB01pcmFjbGUMBE1pbmQMCU1hZWxzdHJvbQwFTG9hdGgMBktyYWtlbgwISHlwbm90aWMMBkhvcnJvcgwGSG9ub3VyDAVIYXZvYwwESGF0ZQwER3JpbQwFR29sZW0MBUdseXBoDAVHbG9vbQwFR2hvdWwMBEdhbGUMA0ZvZQwERmF0ZQwIRW1weXJlYW4MBUVhZ2xlDAREdXNrDAREb29tDAVEcmVhZAwGRHJhZ29uDAREaXJlDAVEZW1vbgwFRGVhdGgMCURhbW5hdGlvbgwKQ29ycnVwdGlvbgwGQ29ycHNlDAhDaGltZXJpYwwJQ2F0YWNseXNtDAdDYXJyaW9uDAVCcm9vZAwJQnJpbXN0b25lDAdCcmFtYmxlDAVCbG9vZAwGQmxpZ2h0DAhCZWhlbW90aAwFQmVhc3QMCkFybWFnZWRkb24MCkFwb2NhbHlwc2UMBUFnb255AEXAZwcMBE1vb24MA1N1bgwERm9ybQwEUGVhawwEVGVhcgwFR3Jvd2wMBVNob3V0DAdXaGlzcGVyDAZTaGFkb3cMBkJlbmRlcgwER2xvdwwKSW5zdHJ1bWVudAwFR3Jhc3AMBFJvYXIMBFNvbmcMBEJpdGUMBFJvb3QMBEJhbmUAEsBnCArO8P//CjHv//8KCO///xPAYAoAAAAACh/v//8LE8BhQMJKWM9KNffu//8j5+7//8JKWc9KNe/u//8j/O7//8JKWM9KNdnu//8jhfD//8JKWM9KNcru//8jP/P//8JKWM9KNbvu//8jYvP//8JKWM9KNazu//8jQ/T//8JKWM9KNZ3u//8jRvT//8JKWM9KNY7u//8jSfT//8JKWM9KNX/u//8jTPT//8JKWM9KNXDu//8jT/T//8JKWM9KNWHu//8jUvT//8JKWM9KNVLu//8jVfT//8JKWM9KNUPu//8jWPT//8JKWM9KNTTu//8jfvT//8JKWM9KNSXu//8jtfX//8iMu2w="));

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
