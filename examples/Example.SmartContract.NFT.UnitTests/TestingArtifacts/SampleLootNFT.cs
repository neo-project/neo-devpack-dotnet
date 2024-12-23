using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":8,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":56,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":237,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":396,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":501,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":529,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":617,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":978,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1048,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":986,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1111,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":1119,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1123,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1171,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1390,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1407,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1424,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1441,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1458,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1475,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":1492,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":1509,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1557,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":1873,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1961,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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NwAAcWlyatgmFQwQVG9rZW4gbm90IGV4aXN0c+BpQFcBAXixJAUJIgd4AWIetSQVDBBUb2tlbiBJRCBpbnZhbGlk4EH5tOI4QTlTbjyXJCMMHkNvbnRyYWN0IGNhbGxzIGFyZSBub3QgYWxsb3dlZOBBLVEIMHBoE854NCoMFFBsYXllciBtaW50cyBzdWNjZXNzEcAMCEV2ZW50TXNnQZUBb2FAVwICeDQrcGh4eTRhcWl4NwUANd38//8MBXRha2VueDcFAF8PwUVTi1BB5j8YhEBXAAF4NwUAXw/BRVOLUEGSXegxDAV0YWtlbpgkGwwWVG9rZW4gYWxyZWFkeSBjbGFpbWVkLuBBa96pKEBXAAMQEAsLFMB6eXgTTTQDQFcABHlKeBBR0EV6SngSUdBFe0p4E1HQRQwQTjMgU2VjdXJlIExvb3QgI3gSzjcFAIvbKEp4EVHQRUBXAQEBYR63JAUJIgYBQR+1JBUMEFRva2VuIElEIGludmFsaWTgNWT8//9waDUe////DBNPd25lciBtaW50cyBzdWNjZXNzEcAMCEV2ZW50TXNnQZUBb2FAVhBBm/ZnzgAVEYhOEFHQUBLAYQAWEYhOEFHQQZv2Z84SwGcPExGIThBR0EGb9mfOEsBjDARCb29rDARUb21lDAlDaHJvbmljbGUMCEdyaW1vaXJlDARXYW5kDAlCb25lIFdhbmQMCkdyYXZlIFdhbmQMCkdob3N0IFdhbmQMC1Nob3J0IFN3b3JkDApMb25nIFN3b3JkDAhTY2ltaXRhcgwIRmFsY2hpb24MBkthdGFuYQwEQ2x1YgwETWFjZQwETWF1bAwMUXVhcnRlcnN0YWZmDAlXYXJoYW1tZXIAEsBnBwwJUmluZyBNYWlsDApDaGFpbiBNYWlsDApQbGF0ZSBNYWlsDBFPcm5hdGUgQ2hlc3RwbGF0ZQwPSG9seSBDaGVzdHBsYXRlDA1MZWF0aGVyIEFybW9yDBJIYXJkIExlYXRoZXIgQXJtb3IMFVN0dWRkZWQgTGVhdGhlciBBcm1vcgwQRHJhZ29uc2tpbiBBcm1vcgwKRGVtb24gSHVzawwFU2hpcnQMBFJvYmUMCkxpbmVuIFJvYmUMCVNpbGsgUm9iZQwLRGl2aW5lIFJvYmUfwGcIDARIb29kDApMaW5lbiBIb29kDAlTaWxrIEhvb2QMC0RpdmluZSBIb29kDAVDcm93bgwDQ2FwDAtMZWF0aGVyIENhcAwHV2FyIENhcAwORHJhZ29uJ3MgQ3Jvd24MC0RlbW9uIENyb3duDARIZWxtDAlGdWxsIEhlbG0MCkdyZWF0IEhlbG0MC09ybmF0ZSBIZWxtDAxBbmNpZW50IEhlbG0fwGcJDARTYXNoDApMaW5lbiBTYXNoDAlXb29sIFNhc2gMCVNpbGsgU2FzaAwPQnJpZ2h0c2lsayBTYXNoDAxMZWF0aGVyIEJlbHQMEUhhcmQgTGVhdGhlciBCZWx0DBRTdHVkZGVkIExlYXRoZXIgQmVsdAwPRHJhZ29uc2tpbiBCZWx0DA5EZW1vbmhpZGUgQmVsdAwKSGVhdnkgQmVsdAwJTWVzaCBCZWx0DAtQbGF0ZWQgQmVsdAwIV2FyIEJlbHQMC09ybmF0ZSBCZWx0H8BnCgwFU2hvZXMMC0xpbmVuIFNob2VzDApXb29sIFNob2VzDA1TaWxrIFNsaXBwZXJzDA9EaXZpbmUgU2xpcHBlcnMMDUxlYXRoZXIgQm9vdHMMEkhhcmQgTGVhdGhlciBCb290cwwVU3R1ZGRlZCBMZWF0aGVyIEJvb3RzDBBEcmFnb25za2luIEJvb3RzDA9EZW1vbmhpZGUgQm9vdHMMC0hlYXZ5IEJvb3RzDAtDaGFpbiBCb290cwwHR3JlYXZlcwwOT3JuYXRlIEdyZWF2ZXMMDEhvbHkgR3JlYXZlcx/AZwsMBkdsb3ZlcwwMTGluZW4gR2xvdmVzDAtXb29sIEdsb3ZlcwwLU2lsayBHbG92ZXMMDURpdmluZSBHbG92ZXMMDkxlYXRoZXIgR2xvdmVzDBNIYXJkIExlYXRoZXIgR2xvdmVzDBZTdHVkZGVkIExlYXRoZXIgR2xvdmVzDBFEcmFnb25za2luIEdsb3ZlcwwNRGVtb24ncyBIYW5kcwwMSGVhdnkgR2xvdmVzDAxDaGFpbiBHbG92ZXMMCUdhdW50bGV0cwwQT3JuYXRlIEdhdW50bGV0cwwOSG9seSBHYXVudGxldHMfwGcMDAdQZW5kYW50DAZBbXVsZXQMCE5lY2tsYWNlE8BnDQwNVGl0YW5pdW0gUmluZwwNUGxhdGludW0gUmluZwwLQnJvbnplIFJpbmcMC1NpbHZlciBSaW5nDAlHb2xkIFJpbmcVwGcODAxvZiB0aGUgVHdpbnMMDW9mIFJlZmxlY3Rpb24MDG9mIERldGVjdGlvbgwKb2YgdGhlIEZveAwKb2YgVml0cmlvbAwHb2YgRnVyeQwHb2YgUmFnZQwIb2YgQW5nZXIMDW9mIFByb3RlY3Rpb24MEG9mIEVubGlnaHRlbm1lbnQMDW9mIEJyaWxsaWFuY2UMDW9mIFBlcmZlY3Rpb24MCG9mIFNraWxsDAlvZiBUaXRhbnMMCW9mIEdpYW50cwwIb2YgUG93ZXIgwGQMClNoaW1tZXJpbmcMB0xpZ2h0J3MMBVdyYXRoDANXb2UMBlZvcnRleAwFVmlwZXIMB1ZpY3RvcnkMCVZlbmdlYW5jZQwHVG9ybWVudAwHVGVtcGVzdAwFU3Rvcm0MBlNwaXJpdAwGU29ycm93DARTb3VsDANTb2wMBVNrdWxsDARSdW5lDAdSYXB0dXJlDARSYWdlDAZQbGFndWUMB1Bob2VuaXgMC1BhbmRlbW9uaXVtDARQYWluDAlPbnNsYXVnaHQMCE9ibGl2aW9uDAZNb3JiaWQMB01pcmFjbGUMBE1pbmQMCU1hZWxzdHJvbQwFTG9hdGgMBktyYWtlbgwISHlwbm90aWMMBkhvcnJvcgwGSG9ub3VyDAVIYXZvYwwESGF0ZQwER3JpbQwFR29sZW0MBUdseXBoDAVHbG9vbQwFR2hvdWwMBEdhbGUMA0ZvZQwERmF0ZQwIRW1weXJlYW4MBUVhZ2xlDAREdXNrDAREb29tDAVEcmVhZAwGRHJhZ29uDAREaXJlDAVEZW1vbgwFRGVhdGgMCURhbW5hdGlvbgwKQ29ycnVwdGlvbgwGQ29ycHNlDAhDaGltZXJpYwwJQ2F0YWNseXNtDAdDYXJyaW9uDAVCcm9vZAwJQnJpbXN0b25lDAdCcmFtYmxlDAVCbG9vZAwGQmxpZ2h0DAhCZWhlbW90aAwFQmVhc3QMCkFybWFnZWRkb24MCkFwb2NhbHlwc2UMBUFnb255AEXAZQwETW9vbgwDU3VuDARGb3JtDARQZWFrDARUZWFyDAVHcm93bAwFU2hvdXQMB1doaXNwZXIMBlNoYWRvdwwGQmVuZGVyDARHbG93DApJbnN0cnVtZW50DAVHcmFzcAwEUm9hcgwEU29uZwwEQml0ZQwEUm9vdAwEQmFuZQASwGZA0Mi1Pw=="));

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
