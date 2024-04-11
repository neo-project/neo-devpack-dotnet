using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":4382,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":4409,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":2271,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":2317,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":2513,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":4424,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":2751,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":2781,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":2874,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":3259,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":3312,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":3267,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":3375,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":3383,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":4451,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4478,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4505,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4532,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4559,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4586,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4613,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4640,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4667,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":4694,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4721,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4748,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":4276,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABHaHR0cHM6Ly9naXRodWIuY29tL25lby1wcm9qZWN0L25lby1kZXZwYWNrLWRvdG5ldC90cmVlL21hc3Rlci9leGFtcGxlcy8AB8DvOc7g5OklxsKgannhRA3Yb86sC2Rlc2VyaWFsaXplAQABD8DvOc7g5OklxsKgannhRA3Yb86sCXNlcmlhbGl6ZQEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8b9XWrEYlohBNhCjWhKIbN4LZscgZzaGEyNTYBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8GdXBkYXRlAwAAD/2j+kNG6lMqJY/El92t22Q3yf3/B2Rlc3Ryb3kAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ8AAP2nElcAAQwFc0xvb3RAVwABeBAMBEJvb2sMBFRvbWUMCUNocm9uaWNsZQwIR3JpbW9pcmUMBFdhbmQMCUJvbmUgV2FuZAwKR3JhdmUgV2FuZAwKR2hvc3QgV2FuZAwLU2hvcnQgU3dvcmQMCkxvbmcgU3dvcmQMCFNjaW1pdGFyDAhGYWxjaGlvbgwGS2F0YW5hDARDbHViDARNYWNlDARNYXVsDAxRdWFydGVyc3RhZmYMCVdhcmhhbW1lcgASwNB4EQwJUmluZyBNYWlsDApDaGFpbiBNYWlsDApQbGF0ZSBNYWlsDBFPcm5hdGUgQ2hlc3RwbGF0ZQwPSG9seSBDaGVzdHBsYXRlDA1MZWF0aGVyIEFybW9yDBJIYXJkIExlYXRoZXIgQXJtb3IMFVN0dWRkZWQgTGVhdGhlciBBcm1vcgwQRHJhZ29uc2tpbiBBcm1vcgwKRGVtb24gSHVzawwFU2hpcnQMBFJvYmUMCkxpbmVuIFJvYmUMCVNpbGsgUm9iZQwLRGl2aW5lIFJvYmUfwNB4EgwESG9vZAwKTGluZW4gSG9vZAwJU2lsayBIb29kDAtEaXZpbmUgSG9vZAwFQ3Jvd24MA0NhcAwLTGVhdGhlciBDYXAMB1dhciBDYXAMDkRyYWdvbidzIENyb3duDAtEZW1vbiBDcm93bgwESGVsbQwJRnVsbCBIZWxtDApHcmVhdCBIZWxtDAtPcm5hdGUgSGVsbQwMQW5jaWVudCBIZWxtH8DQeBMMBFNhc2gMCkxpbmVuIFNhc2gMCVdvb2wgU2FzaAwJU2lsayBTYXNoDA9CcmlnaHRzaWxrIFNhc2gMDExlYXRoZXIgQmVsdAwRSGFyZCBMZWF0aGVyIEJlbHQMFFN0dWRkZWQgTGVhdGhlciBCZWx0DA9EcmFnb25za2luIEJlbHQMDkRlbW9uaGlkZSBCZWx0DApIZWF2eSBCZWx0DAlNZXNoIEJlbHQMC1BsYXRlZCBCZWx0DAhXYXIgQmVsdAwLT3JuYXRlIEJlbHQfwNB4FAwFU2hvZXMMC0xpbmVuIFNob2VzDApXb29sIFNob2VzDA1TaWxrIFNsaXBwZXJzDA9EaXZpbmUgU2xpcHBlcnMMDUxlYXRoZXIgQm9vdHMMEkhhcmQgTGVhdGhlciBCb290cwwVU3R1ZGRlZCBMZWF0aGVyIEJvb3RzDBBEcmFnb25za2luIEJvb3RzDA9EZW1vbmhpZGUgQm9vdHMMC0hlYXZ5IEJvb3RzDAtDaGFpbiBCb290cwwHR3JlYXZlcwwOT3JuYXRlIEdyZWF2ZXMMDEhvbHkgR3JlYXZlcx/A0HgVDAZHbG92ZXMMDExpbmVuIEdsb3ZlcwwLV29vbCBHbG92ZXMMC1NpbGsgR2xvdmVzDA1EaXZpbmUgR2xvdmVzDA5MZWF0aGVyIEdsb3ZlcwwTSGFyZCBMZWF0aGVyIEdsb3ZlcwwWU3R1ZGRlZCBMZWF0aGVyIEdsb3ZlcwwRRHJhZ29uc2tpbiBHbG92ZXMMDURlbW9uJ3MgSGFuZHMMDEhlYXZ5IEdsb3ZlcwwMQ2hhaW4gR2xvdmVzDAlHYXVudGxldHMMEE9ybmF0ZSBHYXVudGxldHMMDkhvbHkgR2F1bnRsZXRzH8DQeBYMB1BlbmRhbnQMBkFtdWxldAwITmVja2xhY2UTwNB4FwwNVGl0YW5pdW0gUmluZwwNUGxhdGludW0gUmluZwwLQnJvbnplIFJpbmcMC1NpbHZlciBSaW5nDAlHb2xkIFJpbmcVwNB4GAwMb2YgdGhlIFR3aW5zDA1vZiBSZWZsZWN0aW9uDAxvZiBEZXRlY3Rpb24MCm9mIHRoZSBGb3gMCm9mIFZpdHJpb2wMB29mIEZ1cnkMB29mIFJhZ2UMCG9mIEFuZ2VyDA1vZiBQcm90ZWN0aW9uDBBvZiBFbmxpZ2h0ZW5tZW50DA1vZiBCcmlsbGlhbmNlDA1vZiBQZXJmZWN0aW9uDAhvZiBTa2lsbAwJb2YgVGl0YW5zDAlvZiBHaWFudHMMCG9mIFBvd2VyIMDQeBkMClNoaW1tZXJpbmcMB0xpZ2h0J3MMBVdyYXRoDANXb2UMBlZvcnRleAwFVmlwZXIMB1ZpY3RvcnkMCVZlbmdlYW5jZQwHVG9ybWVudAwHVGVtcGVzdAwFU3Rvcm0MBlNwaXJpdAwGU29ycm93DARTb3VsDANTb2wMBVNrdWxsDARSdW5lDAdSYXB0dXJlDARSYWdlDAZQbGFndWUMB1Bob2VuaXgMC1BhbmRlbW9uaXVtDARQYWluDAlPbnNsYXVnaHQMCE9ibGl2aW9uDAZNb3JiaWQMB01pcmFjbGUMBE1pbmQMCU1hZWxzdHJvbQwFTG9hdGgMBktyYWtlbgwISHlwbm90aWMMBkhvcnJvcgwGSG9ub3VyDAVIYXZvYwwESGF0ZQwER3JpbQwFR29sZW0MBUdseXBoDAVHbG9vbQwFR2hvdWwMBEdhbGUMA0ZvZQwERmF0ZQwIRW1weXJlYW4MBUVhZ2xlDAREdXNrDAREb29tDAVEcmVhZAwGRHJhZ29uDAREaXJlDAVEZW1vbgwFRGVhdGgMCURhbW5hdGlvbgwKQ29ycnVwdGlvbgwGQ29ycHNlDAhDaGltZXJpYwwJQ2F0YWNseXNtDAdDYXJyaW9uDAVCcm9vZAwJQnJpbXN0b25lDAdCcmFtYmxlDAVCbG9vZAwGQmxpZ2h0DAhCZWhlbW90aAwFQmVhc3QMCkFybWFnZWRkb24MCkFwb2NhbHlwc2UMBUFnb255AEXA0HgaDARNb29uDANTdW4MBEZvcm0MBFBlYWsMBFRlYXIMBUdyb3dsDAVTaG91dAwHV2hpc3BlcgwGU2hhZG93DAZCZW5kZXIMBEdsb3cMCkluc3RydW1lbnQMBUdyYXNwDARSb2FyDARTb25nDARCaXRlDARSb290DARCYW5lABLA0Hg0A0BXAAF4NANAVwABeDQDQFcAAUBXAAEQQFrYJhcMAQBB9rRr4kGSXegxStgmBEUQSmJAVwABeGJ4DAEAQZv2Z85B5j8YhEBXAQF4cGgL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"));

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
