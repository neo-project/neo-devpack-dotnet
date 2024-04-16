using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleLootNFT : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

<<<<<<< HEAD
    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":4453,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":4480,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":2271,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":2317,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":2513,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":4495,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":2751,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":2781,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":2874,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":3259,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":3312,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":3267,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":3375,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":3383,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":4522,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4549,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4576,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4603,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4630,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4657,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4684,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4711,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4738,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":4765,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4792,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4819,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":4347,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAfA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABD/2j+kNG6lMqJY/El92t22Q3yf3/BnVwZGF0ZQMAAA/9o/pDRupTKiWPxJfdrdtkN8n9/wdkZXN0cm95AAAAD8DvOc7g5OklxsKgannhRA3Yb86sBGl0b2EBAAEPAAD97hJXAAEMBXNMb290QFcAAXgQDARCb29rDARUb21lDAlDaHJvbmljbGUMCEdyaW1vaXJlDARXYW5kDAlCb25lIFdhbmQMCkdyYXZlIFdhbmQMCkdob3N0IFdhbmQMC1Nob3J0IFN3b3JkDApMb25nIFN3b3JkDAhTY2ltaXRhcgwIRmFsY2hpb24MBkthdGFuYQwEQ2x1YgwETWFjZQwETWF1bAwMUXVhcnRlcnN0YWZmDAlXYXJoYW1tZXIAEsDQeBEMCVJpbmcgTWFpbAwKQ2hhaW4gTWFpbAwKUGxhdGUgTWFpbAwRT3JuYXRlIENoZXN0cGxhdGUMD0hvbHkgQ2hlc3RwbGF0ZQwNTGVhdGhlciBBcm1vcgwSSGFyZCBMZWF0aGVyIEFybW9yDBVTdHVkZGVkIExlYXRoZXIgQXJtb3IMEERyYWdvbnNraW4gQXJtb3IMCkRlbW9uIEh1c2sMBVNoaXJ0DARSb2JlDApMaW5lbiBSb2JlDAlTaWxrIFJvYmUMC0RpdmluZSBSb2JlH8DQeBIMBEhvb2QMCkxpbmVuIEhvb2QMCVNpbGsgSG9vZAwLRGl2aW5lIEhvb2QMBUNyb3duDANDYXAMC0xlYXRoZXIgQ2FwDAdXYXIgQ2FwDA5EcmFnb24ncyBDcm93bgwLRGVtb24gQ3Jvd24MBEhlbG0MCUZ1bGwgSGVsbQwKR3JlYXQgSGVsbQwLT3JuYXRlIEhlbG0MDEFuY2llbnQgSGVsbR/A0HgTDARTYXNoDApMaW5lbiBTYXNoDAlXb29sIFNhc2gMCVNpbGsgU2FzaAwPQnJpZ2h0c2lsayBTYXNoDAxMZWF0aGVyIEJlbHQMEUhhcmQgTGVhdGhlciBCZWx0DBRTdHVkZGVkIExlYXRoZXIgQmVsdAwPRHJhZ29uc2tpbiBCZWx0DA5EZW1vbmhpZGUgQmVsdAwKSGVhdnkgQmVsdAwJTWVzaCBCZWx0DAtQbGF0ZWQgQmVsdAwIV2FyIEJlbHQMC09ybmF0ZSBCZWx0H8DQeBQMBVNob2VzDAtMaW5lbiBTaG9lcwwKV29vbCBTaG9lcwwNU2lsayBTbGlwcGVycwwPRGl2aW5lIFNsaXBwZXJzDA1MZWF0aGVyIEJvb3RzDBJIYXJkIExlYXRoZXIgQm9vdHMMFVN0dWRkZWQgTGVhdGhlciBCb290cwwQRHJhZ29uc2tpbiBCb290cwwPRGVtb25oaWRlIEJvb3RzDAtIZWF2eSBCb290cwwLQ2hhaW4gQm9vdHMMB0dyZWF2ZXMMDk9ybmF0ZSBHcmVhdmVzDAxIb2x5IEdyZWF2ZXMfwNB4FQwGR2xvdmVzDAxMaW5lbiBHbG92ZXMMC1dvb2wgR2xvdmVzDAtTaWxrIEdsb3ZlcwwNRGl2aW5lIEdsb3ZlcwwOTGVhdGhlciBHbG92ZXMME0hhcmQgTGVhdGhlciBHbG92ZXMMFlN0dWRkZWQgTGVhdGhlciBHbG92ZXMMEURyYWdvbnNraW4gR2xvdmVzDA1EZW1vbidzIEhhbmRzDAxIZWF2eSBHbG92ZXMMDENoYWluIEdsb3ZlcwwJR2F1bnRsZXRzDBBPcm5hdGUgR2F1bnRsZXRzDA5Ib2x5IEdhdW50bGV0cx/A0HgWDAdQZW5kYW50DAZBbXVsZXQMCE5lY2tsYWNlE8DQeBcMDVRpdGFuaXVtIFJpbmcMDVBsYXRpbnVtIFJpbmcMC0Jyb256ZSBSaW5nDAtTaWx2ZXIgUmluZwwJR29sZCBSaW5nFcDQeBgMDG9mIHRoZSBUd2lucwwNb2YgUmVmbGVjdGlvbgwMb2YgRGV0ZWN0aW9uDApvZiB0aGUgRm94DApvZiBWaXRyaW9sDAdvZiBGdXJ5DAdvZiBSYWdlDAhvZiBBbmdlcgwNb2YgUHJvdGVjdGlvbgwQb2YgRW5saWdodGVubWVudAwNb2YgQnJpbGxpYW5jZQwNb2YgUGVyZmVjdGlvbgwIb2YgU2tpbGwMCW9mIFRpdGFucwwJb2YgR2lhbnRzDAhvZiBQb3dlciDA0HgZDApTaGltbWVyaW5nDAdMaWdodCdzDAVXcmF0aAwDV29lDAZWb3J0ZXgMBVZpcGVyDAdWaWN0b3J5DAlWZW5nZWFuY2UMB1Rvcm1lbnQMB1RlbXBlc3QMBVN0b3JtDAZTcGlyaXQMBlNvcnJvdwwEU291bAwDU29sDAVTa3VsbAwEUnVuZQwHUmFwdHVyZQwEUmFnZQwGUGxhZ3VlDAdQaG9lbml4DAtQYW5kZW1vbml1bQwEUGFpbgwJT25zbGF1Z2h0DAhPYmxpdmlvbgwGTW9yYmlkDAdNaXJhY2xlDARNaW5kDAlNYWVsc3Ryb20MBUxvYXRoDAZLcmFrZW4MCEh5cG5vdGljDAZIb3Jyb3IMBkhvbm91cgwFSGF2b2MMBEhhdGUMBEdyaW0MBUdvbGVtDAVHbHlwaAwFR2xvb20MBUdob3VsDARHYWxlDANGb2UMBEZhdGUMCEVtcHlyZWFuDAVFYWdsZQwERHVzawwERG9vbQwFRHJlYWQMBkRyYWdvbgwERGlyZQwFRGVtb24MBURlYXRoDAlEYW1uYXRpb24MCkNvcnJ1cHRpb24MBkNvcnBzZQwIQ2hpbWVyaWMMCUNhdGFjbHlzbQwHQ2FycmlvbgwFQnJvb2QMCUJyaW1zdG9uZQwHQnJhbWJsZQwFQmxvb2QMBkJsaWdodAwIQmVoZW1vdGgMBUJlYXN0DApBcm1hZ2VkZG9uDApBcG9jYWx5cHNlDAVBZ29ueQBFwNB4GgwETW9vbgwDU3VuDARGb3JtDARQZWFrDARUZWFyDAVHcm93bAwFU2hvdXQMB1doaXNwZXIMBlNoYWRvdwwGQmVuZGVyDARHbG93DApJbnN0cnVtZW50DAVHcmFzcAwEUm9hcgwEU29uZwwEQml0ZQwEUm9vdAwEQmFuZQASwNB4NANAVwABeDQDQFcAAXg0A0BXAAFAVwABEEBa2CYXDAEAQfa0a+JBkl3oMUrYJgRFEEpiQFcAAXhieAwBAEGb9mfOQeY/GIRAVwEBeHBoC5cmBxHbICINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLA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"));
=======
    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleLootNFT"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":4114,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":4129,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":67,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":263,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":4144,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":501,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":531,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":624,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":937,""safe"":false},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":990,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":945,""safe"":true},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":1053,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":1061,""safe"":false},{""name"":""getCredential"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":4159,""safe"":true},{""name"":""getWeapon"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4174,""safe"":true},{""name"":""getChest"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4189,""safe"":true},{""name"":""getHead"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4204,""safe"":true},{""name"":""getWaist"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4219,""safe"":true},{""name"":""getFoot"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4234,""safe"":true},{""name"":""getHand"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4249,""safe"":true},{""name"":""getNeck"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4264,""safe"":true},{""name"":""getRing"",""parameters"":[{""name"":""credential"",""type"":""Integer""}],""returntype"":""String"",""offset"":4279,""safe"":true},{""name"":""tokenURI"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""String"",""offset"":4294,""safe"":true},{""name"":""claim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4309,""safe"":false},{""name"":""ownerClaim"",""parameters"":[{""name"":""tokenId"",""type"":""Integer""}],""returntype"":""Void"",""offset"":4324,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1789,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]},{""name"":""EventMsg"",""parameters"":[{""name"":""obj"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""itoa"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""getContract"",""update""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""This is a text Example.SmartContract.NFT"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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XegxNwAAcWlyaguXqgwQVG9rZW4gbm90IGV4aXN0c+FpIwUAAABAVwMCeRCzqiQHENsgIgd5AWIetQwQVG9rZW4gSUQgaW52YWxpZOFB+bTiOEE5U248lwweQ29udHJhY3QgY2FsbHMgYXJlIG5vdCBhbGxvd2Vk4UEtUQgwcGgTzjcGAF8RwUVTi1BBkl3oMQwFdGFrZW6YDBZUb2tlbiBhbHJlYWR5IGNsYWltZWQu4QPHGUaWAgAAACICQFcDAgFhHrckBxDbICIGAUEftQwQVG9rZW4gSUQgaW52YWxpZOE19/z//3BoNwYAXxHBRVOLUEGSXegxDAV0YWtlbpgMFlRva2VuIGFscmVhZHkgY2xhaW1lZC7hA8cZRpYCAAAAIgJAVhIMFJw9F3wHtWVzIRebZXLtn3bCc/aKZEGb9mfOABURiE4QUdBQEsBjABYRiE4QUdBBm/ZnzhLAZxETEYhOEFHQQZv2Z84SwGUMBEJvb2sMBFRvbWUMCUNocm9uaWNsZQwIR3JpbW9pcmUMBFdhbmQMCUJvbmUgV2FuZAwKR3JhdmUgV2FuZAwKR2hvc3QgV2FuZAwLU2hvcnQgU3dvcmQMCkxvbmcgU3dvcmQMCFNjaW1pdGFyDAhGYWxjaGlvbgwGS2F0YW5hDARDbHViDARNYWNlDARNYXVsDAxRdWFydGVyc3RhZmYMCVdhcmhhbW1lcgASwGcJDAlSaW5nIE1haWwMCkNoYWluIE1haWwMClBsYXRlIE1haWwMEU9ybmF0ZSBDaGVzdHBsYXRlDA9Ib2x5IENoZXN0cGxhdGUMDUxlYXRoZXIgQXJtb3IMEkhhcmQgTGVhdGhlciBBcm1vcgwVU3R1ZGRlZCBMZWF0aGVyIEFybW9yDBBEcmFnb25za2luIEFybW9yDApEZW1vbiBIdXNrDAVTaGlydAwEUm9iZQwKTGluZW4gUm9iZQwJU2lsayBSb2JlDAtEaXZpbmUgUm9iZR/AZwoMBEhvb2QMCkxpbmVuIEhvb2QMCVNpbGsgSG9vZAwLRGl2aW5lIEhvb2QMBUNyb3duDANDYXAMC0xlYXRoZXIgQ2FwDAdXYXIgQ2FwDA5EcmFnb24ncyBDcm93bgwLRGVtb24gQ3Jvd24MBEhlbG0MCUZ1bGwgSGVsbQwKR3JlYXQgSGVsbQwLT3JuYXRlIEhlbG0MDEFuY2llbnQgSGVsbR/AZwsMBFNhc2gMCkxpbmVuIFNhc2gMCVdvb2wgU2FzaAwJU2lsayBTYXNoDA9CcmlnaHRzaWxrIFNhc2gMDExlYXRoZXIgQmVsdAwRSGFyZCBMZWF0aGVyIEJlbHQMFFN0dWRkZWQgTGVhdGhlciBCZWx0DA9EcmFnb25za2luIEJlbHQMDkRlbW9uaGlkZSBCZWx0DApIZWF2eSBCZWx0DAlNZXNoIEJlbHQMC1BsYXRlZCBCZWx0DAhXYXIgQmVsdAwLT3JuYXRlIEJlbHQfwGcMDAVTaG9lcwwLTGluZW4gU2hvZXMMCldvb2wgU2hvZXMMDVNpbGsgU2xpcHBlcnMMD0RpdmluZSBTbGlwcGVycwwNTGVhdGhlciBCb290cwwSSGFyZCBMZWF0aGVyIEJvb3RzDBVTdHVkZGVkIExlYXRoZXIgQm9vdHMMEERyYWdvbnNraW4gQm9vdHMMD0RlbW9uaGlkZSBCb290cwwLSGVhdnkgQm9vdHMMC0NoYWluIEJvb3RzDAdHcmVhdmVzDA5Pcm5hdGUgR3JlYXZlcwwMSG9seSBHcmVhdmVzH8BnDQwGR2xvdmVzDAxMaW5lbiBHbG92ZXMMC1dvb2wgR2xvdmVzDAtTaWxrIEdsb3ZlcwwNRGl2aW5lIEdsb3ZlcwwOTGVhdGhlciBHbG92ZXMME0hhcmQgTGVhdGhlciBHbG92ZXMMFlN0dWRkZWQgTGVhdGhlciBHbG92ZXMMEURyYWdvbnNraW4gR2xvdmVzDA1EZW1vbidzIEhhbmRzDAxIZWF2eSBHbG92ZXMMDENoYWluIEdsb3ZlcwwJR2F1bnRsZXRzDBBPcm5hdGUgR2F1bnRsZXRzDA5Ib2x5IEdhdW50bGV0cx/AZw4MB1BlbmRhbnQMBkFtdWxldAwITmVja2xhY2UTwGcPDA1UaXRhbml1bSBSaW5nDA1QbGF0aW51bSBSaW5nDAtCcm9uemUgUmluZwwLU2lsdmVyIFJpbmcMCUdvbGQgUmluZxXAZxAMDG9mIHRoZSBUd2lucwwNb2YgUmVmbGVjdGlvbgwMb2YgRGV0ZWN0aW9uDApvZiB0aGUgRm94DApvZiBWaXRyaW9sDAdvZiBGdXJ5DAdvZiBSYWdlDAhvZiBBbmdlcgwNb2YgUHJvdGVjdGlvbgwQb2YgRW5saWdodGVubWVudAwNb2YgQnJpbGxpYW5jZQwNb2YgUGVyZmVjdGlvbgwIb2YgU2tpbGwMCW9mIFRpdGFucwwJb2YgR2lhbnRzDAhvZiBQb3dlciDAZgwKU2hpbW1lcmluZwwHTGlnaHQncwwFV3JhdGgMA1dvZQwGVm9ydGV4DAVWaXBlcgwHVmljdG9yeQwJVmVuZ2VhbmNlDAdUb3JtZW50DAdUZW1wZXN0DAVTdG9ybQwGU3Bpcml0DAZTb3Jyb3cMBFNvdWwMA1NvbAwFU2t1bGwMBFJ1bmUMB1JhcHR1cmUMBFJhZ2UMBlBsYWd1ZQwHUGhvZW5peAwLUGFuZGVtb25pdW0MBFBhaW4MCU9uc2xhdWdodAwIT2JsaXZpb24MBk1vcmJpZAwHTWlyYWNsZQwETWluZAwJTWFlbHN0cm9tDAVMb2F0aAwGS3Jha2VuDAhIeXBub3RpYwwGSG9ycm9yDAZIb25vdXIMBUhhdm9jDARIYXRlDARHcmltDAVHb2xlbQwFR2x5cGgMBUdsb29tDAVHaG91bAwER2FsZQwDRm9lDARGYXRlDAhFbXB5cmVhbgwFRWFnbGUMBER1c2sMBERvb20MBURyZWFkDAZEcmFnb24MBERpcmUMBURlbW9uDAVEZWF0aAwJRGFtbmF0aW9uDApDb3JydXB0aW9uDAZDb3Jwc2UMCENoaW1lcmljDAlDYXRhY2x5c20MB0NhcnJpb24MBUJyb29kDAlCcmltc3RvbmUMB0JyYW1ibGUMBUJsb29kDAZCbGlnaHQMCEJlaGVtb3RoDAVCZWFzdAwKQXJtYWdlZGRvbgwKQXBvY2FseXBzZQwFQWdvbnkARcBnBwwETW9vbgwDU3VuDARGb3JtDARQZWFrDARUZWFyDAVHcm93bAwFU2hvdXQMB1doaXNwZXIMBlNoYWRvdwwGQmVuZGVyDARHbG93DApJbnN0cnVtZW50DAVHcmFzcAwEUm9hcgwEU29uZwwEQml0ZQwEUm9vdAwEQmFuZQASwGcICrfx//8KLvD//woF8P//E8BgCgAAAAAKHPD//wsTwGFAwkpYz0o19O///yPk7///wkpZz0o17O///yP57///wkpYz0o11u///yNu8f//wkpYz0o1x+///yPg8///wkpYz0o1uO///yMD9P//wkpYz0o1qe///yPk9P//wkpYz0o1mu///yPn9P//wkpYz0o1i+///yPq9P//wkpYz0o1fO///yPt9P//wkpYz0o1be///yPw9P//wkpYz0o1Xu///yPz9P//wkpYz0o1T+///yP29P//wkpYz0o1QO///yP59P//wkpYz0o1Me///yMf9f//wkpYz0o1Iu///yOn9f//+bqeew=="));
>>>>>>> master

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
