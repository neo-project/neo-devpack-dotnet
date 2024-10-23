using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":29,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":212,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":371,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":418,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":446,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":535,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":836,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":838,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":842,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/U0DQBBAWNgmFwwBAEH2tGviQZJd6DFK2CYERRBKYEBXAQF4cGgLlyYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5xaRC1JgQJQGkQsyYQeGjBRVOLUEEvWMXtIg9peGjBRVOLUEHmPxiECEBXAwF4ygBAtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDC5UaGUgdG9rZW4gd2l0aCBnaXZlbiAidG9rZW5JZCIgZG9lcyBub3QgZXhpc3QuOnFpNwAAcmoQzkBXAgETEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DE3AABxyEoMBG5hbWVpEc7QQFcBABMRiE4QUdBBm/ZnzhLAcBNowUVB3zC4mkBXAQF4cGgLlyYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQFcDA3hwaAuXJgUIIg14StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjKomBAlAaniYJiV4SmkQUdBFaTcBAEp5aMFFU4tQQeY/GIRFD3lqNA8ReXg0Cnp5eGo0RQhAVwIDeng1y/3//0VBm/ZnzhQRiE4QUdBQEsBweHmL2yhxehC3JhAQaWjBRVOLUEHmPxiEQGlowUVTi1BBL1jF7UBXAQTCSnjPSnnPShHPSnrPDAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwIAcGgLl6omIHt6EXgUwB8MDm9uTkVQMTFQYXltZW50eUFifVtSRUAIQFcABEBWAUDXxctQ"));

    #endregion

    #region Events

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
    /// <remarks>
    /// Script: VwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSHNULL	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 0A : OpCode.PUSHT	[1 datoshi]
    /// 0B : OpCode.JMP 0D	[2 datoshi]
    /// 0D : OpCode.LDARG0	[2 datoshi]
    /// 0E : OpCode.DUP	[2 datoshi]
    /// 0F : OpCode.ISTYPE 28	[2 datoshi]
    /// 11 : OpCode.SWAP	[2 datoshi]
    /// 12 : OpCode.SIZE	[4 datoshi]
    /// 13 : OpCode.PUSHINT8 14	[1 datoshi]
    /// 15 : OpCode.NUMEQUAL	[8 datoshi]
    /// 16 : OpCode.BOOLAND	[8 datoshi]
    /// 17 : OpCode.NOT	[4 datoshi]
    /// 18 : OpCode.JMPIFNOT 25	[2 datoshi]
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E	[8 datoshi]
    /// 3C : OpCode.THROW	[512 datoshi]
    /// 3D : OpCode.SYSCALL 9BF667CE	[0 datoshi]
    /// 42 : OpCode.PUSH1	[1 datoshi]
    /// 43 : OpCode.PUSH1	[1 datoshi]
    /// 44 : OpCode.NEWBUFFER	[256 datoshi]
    /// 45 : OpCode.TUCK	[2 datoshi]
    /// 46 : OpCode.PUSH0	[1 datoshi]
    /// 47 : OpCode.ROT	[2 datoshi]
    /// 48 : OpCode.SETITEM	[8192 datoshi]
    /// 49 : OpCode.SWAP	[2 datoshi]
    /// 4A : OpCode.PUSH2	[1 datoshi]
    /// 4B : OpCode.PACK	[2048 datoshi]
    /// 4C : OpCode.STLOC0	[2 datoshi]
    /// 4D : OpCode.LDARG0	[2 datoshi]
    /// 4E : OpCode.LDLOC0	[2 datoshi]
    /// 4F : OpCode.UNPACK	[2048 datoshi]
    /// 50 : OpCode.DROP	[2 datoshi]
    /// 51 : OpCode.REVERSE3	[2 datoshi]
    /// 52 : OpCode.CAT	[2048 datoshi]
    /// 53 : OpCode.SWAP	[2 datoshi]
    /// 54 : OpCode.SYSCALL 925DE831	[0 datoshi]
    /// 59 : OpCode.DUP	[2 datoshi]
    /// 5A : OpCode.ISNULL	[2 datoshi]
    /// 5B : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 5D : OpCode.DROP	[2 datoshi]
    /// 5E : OpCode.PUSH0	[1 datoshi]
    /// 5F : OpCode.CONVERT 21	[8192 datoshi]
    /// 61 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMoAQLcmPAxUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDFRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByahDOQA==
    /// 00 : OpCode.INITSLOT 0301	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.PUSHINT8 40	[1 datoshi]
    /// 07 : OpCode.GT	[8 datoshi]
    /// 08 : OpCode.JMPIFNOT 3C	[2 datoshi]
    /// 0A : OpCode.PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203634206F72206C657373206279746573206C6F6E672E	[8 datoshi]
    /// 43 : OpCode.THROW	[512 datoshi]
    /// 44 : OpCode.PUSH3	[1 datoshi]
    /// 45 : OpCode.PUSH1	[1 datoshi]
    /// 46 : OpCode.NEWBUFFER	[256 datoshi]
    /// 47 : OpCode.TUCK	[2 datoshi]
    /// 48 : OpCode.PUSH0	[1 datoshi]
    /// 49 : OpCode.ROT	[2 datoshi]
    /// 4A : OpCode.SETITEM	[8192 datoshi]
    /// 4B : OpCode.SYSCALL 9BF667CE	[0 datoshi]
    /// 50 : OpCode.PUSH2	[1 datoshi]
    /// 51 : OpCode.PACK	[2048 datoshi]
    /// 52 : OpCode.STLOC0	[2 datoshi]
    /// 53 : OpCode.LDARG0	[2 datoshi]
    /// 54 : OpCode.LDLOC0	[2 datoshi]
    /// 55 : OpCode.UNPACK	[2048 datoshi]
    /// 56 : OpCode.DROP	[2 datoshi]
    /// 57 : OpCode.REVERSE3	[2 datoshi]
    /// 58 : OpCode.CAT	[2048 datoshi]
    /// 59 : OpCode.SWAP	[2 datoshi]
    /// 5A : OpCode.SYSCALL 925DE831	[0 datoshi]
    /// 5F : OpCode.DUP	[2 datoshi]
    /// 60 : OpCode.ISNULL	[2 datoshi]
    /// 61 : OpCode.JMPIFNOT 34	[2 datoshi]
    /// 63 : OpCode.DROP	[2 datoshi]
    /// 64 : OpCode.PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E	[8 datoshi]
    /// 94 : OpCode.THROW	[512 datoshi]
    /// 95 : OpCode.STLOC1	[2 datoshi]
    /// 96 : OpCode.LDLOC1	[2 datoshi]
    /// 97 : OpCode.CALLT 0000	[32768 datoshi]
    /// 9A : OpCode.STLOC2	[2 datoshi]
    /// 9B : OpCode.LDLOC2	[2 datoshi]
    /// 9C : OpCode.PUSH0	[1 datoshi]
    /// 9D : OpCode.PICKITEM	[64 datoshi]
    /// 9E : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDG5hbWVpEc7QQA==
    /// 00 : OpCode.INITSLOT 0201	[64 datoshi]
    /// 03 : OpCode.PUSH3	[1 datoshi]
    /// 04 : OpCode.PUSH1	[1 datoshi]
    /// 05 : OpCode.NEWBUFFER	[256 datoshi]
    /// 06 : OpCode.TUCK	[2 datoshi]
    /// 07 : OpCode.PUSH0	[1 datoshi]
    /// 08 : OpCode.ROT	[2 datoshi]
    /// 09 : OpCode.SETITEM	[8192 datoshi]
    /// 0A : OpCode.SYSCALL 9BF667CE	[0 datoshi]
    /// 0F : OpCode.PUSH2	[1 datoshi]
    /// 10 : OpCode.PACK	[2048 datoshi]
    /// 11 : OpCode.STLOC0	[2 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.LDLOC0	[2 datoshi]
    /// 14 : OpCode.UNPACK	[2048 datoshi]
    /// 15 : OpCode.DROP	[2 datoshi]
    /// 16 : OpCode.REVERSE3	[2 datoshi]
    /// 17 : OpCode.CAT	[2048 datoshi]
    /// 18 : OpCode.SWAP	[2 datoshi]
    /// 19 : OpCode.SYSCALL 925DE831	[0 datoshi]
    /// 1E : OpCode.CALLT 0000	[32768 datoshi]
    /// 21 : OpCode.STLOC1	[2 datoshi]
    /// 22 : OpCode.NEWMAP	[8 datoshi]
    /// 23 : OpCode.DUP	[2 datoshi]
    /// 24 : OpCode.PUSHDATA1 6E616D65	[8 datoshi]
    /// 2A : OpCode.LDLOC1	[2 datoshi]
    /// 2B : OpCode.PUSH1	[1 datoshi]
    /// 2C : OpCode.PICKITEM	[64 datoshi]
    /// 2D : OpCode.SETITEM	[8192 datoshi]
    /// 2E : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSHNULL	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 0A : OpCode.PUSHT	[1 datoshi]
    /// 0B : OpCode.JMP 0D	[2 datoshi]
    /// 0D : OpCode.LDARG0	[2 datoshi]
    /// 0E : OpCode.DUP	[2 datoshi]
    /// 0F : OpCode.ISTYPE 28	[2 datoshi]
    /// 11 : OpCode.SWAP	[2 datoshi]
    /// 12 : OpCode.SIZE	[4 datoshi]
    /// 13 : OpCode.PUSHINT8 14	[1 datoshi]
    /// 15 : OpCode.NUMEQUAL	[8 datoshi]
    /// 16 : OpCode.BOOLAND	[8 datoshi]
    /// 17 : OpCode.NOT	[4 datoshi]
    /// 18 : OpCode.JMPIFNOT 24	[2 datoshi]
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964	[8 datoshi]
    /// 3B : OpCode.THROW	[512 datoshi]
    /// 3C : OpCode.PUSH4	[1 datoshi]
    /// 3D : OpCode.PUSH1	[1 datoshi]
    /// 3E : OpCode.NEWBUFFER	[256 datoshi]
    /// 3F : OpCode.TUCK	[2 datoshi]
    /// 40 : OpCode.PUSH0	[1 datoshi]
    /// 41 : OpCode.ROT	[2 datoshi]
    /// 42 : OpCode.SETITEM	[8192 datoshi]
    /// 43 : OpCode.SYSCALL 9BF667CE	[0 datoshi]
    /// 48 : OpCode.PUSH2	[1 datoshi]
    /// 49 : OpCode.PACK	[2048 datoshi]
    /// 4A : OpCode.STLOC0	[2 datoshi]
    /// 4B : OpCode.PUSH3	[1 datoshi]
    /// 4C : OpCode.LDARG0	[2 datoshi]
    /// 4D : OpCode.LDLOC0	[2 datoshi]
    /// 4E : OpCode.UNPACK	[2048 datoshi]
    /// 4F : OpCode.DROP	[2 datoshi]
    /// 50 : OpCode.REVERSE3	[2 datoshi]
    /// 51 : OpCode.CAT	[2048 datoshi]
    /// 52 : OpCode.SWAP	[2 datoshi]
    /// 53 : OpCode.SYSCALL DF30B89A	[0 datoshi]
    /// 58 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEQA==
    /// 00 : OpCode.INITSLOT 0004	[64 datoshi]
    /// 03 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("onNEP11Payment")]
    public abstract void OnNEP11Payment(UInt160? from, BigInteger? amount, string? tokenId, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CEA=
    /// 00 : OpCode.PUSHT	[1 datoshi]
    /// 01 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStandard")]
    public abstract bool? TestStandard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDeHBoC5cmBQgiDXhK2ShQygAUs6uqJiIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcWkQznJqQfgn7IyqJgQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQA==
    /// 00 : OpCode.INITSLOT 0303	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSHNULL	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 0A : OpCode.PUSHT	[1 datoshi]
    /// 0B : OpCode.JMP 0D	[2 datoshi]
    /// 0D : OpCode.LDARG0	[2 datoshi]
    /// 0E : OpCode.DUP	[2 datoshi]
    /// 0F : OpCode.ISTYPE 28	[2 datoshi]
    /// 11 : OpCode.SWAP	[2 datoshi]
    /// 12 : OpCode.SIZE	[4 datoshi]
    /// 13 : OpCode.PUSHINT8 14	[1 datoshi]
    /// 15 : OpCode.NUMEQUAL	[8 datoshi]
    /// 16 : OpCode.BOOLAND	[8 datoshi]
    /// 17 : OpCode.NOT	[4 datoshi]
    /// 18 : OpCode.JMPIFNOT 22	[2 datoshi]
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E	[8 datoshi]
    /// 39 : OpCode.THROW	[512 datoshi]
    /// 3A : OpCode.PUSH3	[1 datoshi]
    /// 3B : OpCode.PUSH1	[1 datoshi]
    /// 3C : OpCode.NEWBUFFER	[256 datoshi]
    /// 3D : OpCode.TUCK	[2 datoshi]
    /// 3E : OpCode.PUSH0	[1 datoshi]
    /// 3F : OpCode.ROT	[2 datoshi]
    /// 40 : OpCode.SETITEM	[8192 datoshi]
    /// 41 : OpCode.SYSCALL 9BF667CE	[0 datoshi]
    /// 46 : OpCode.PUSH2	[1 datoshi]
    /// 47 : OpCode.PACK	[2048 datoshi]
    /// 48 : OpCode.STLOC0	[2 datoshi]
    /// 49 : OpCode.LDARG1	[2 datoshi]
    /// 4A : OpCode.LDLOC0	[2 datoshi]
    /// 4B : OpCode.UNPACK	[2048 datoshi]
    /// 4C : OpCode.DROP	[2 datoshi]
    /// 4D : OpCode.REVERSE3	[2 datoshi]
    /// 4E : OpCode.CAT	[2048 datoshi]
    /// 4F : OpCode.SWAP	[2 datoshi]
    /// 50 : OpCode.SYSCALL 925DE831	[0 datoshi]
    /// 55 : OpCode.CALLT 0000	[32768 datoshi]
    /// 58 : OpCode.STLOC1	[2 datoshi]
    /// 59 : OpCode.LDLOC1	[2 datoshi]
    /// 5A : OpCode.PUSH0	[1 datoshi]
    /// 5B : OpCode.PICKITEM	[64 datoshi]
    /// 5C : OpCode.STLOC2	[2 datoshi]
    /// 5D : OpCode.LDLOC2	[2 datoshi]
    /// 5E : OpCode.SYSCALL F827EC8C	[0 datoshi]
    /// 63 : OpCode.NOT	[4 datoshi]
    /// 64 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 66 : OpCode.PUSHF	[1 datoshi]
    /// 67 : OpCode.RET	[0 datoshi]
    /// 68 : OpCode.LDLOC2	[2 datoshi]
    /// 69 : OpCode.LDARG0	[2 datoshi]
    /// 6A : OpCode.NOTEQUAL	[32 datoshi]
    /// 6B : OpCode.JMPIFNOT 25	[2 datoshi]
    /// 6D : OpCode.LDARG0	[2 datoshi]
    /// 6E : OpCode.DUP	[2 datoshi]
    /// 6F : OpCode.LDLOC1	[2 datoshi]
    /// 70 : OpCode.PUSH0	[1 datoshi]
    /// 71 : OpCode.ROT	[2 datoshi]
    /// 72 : OpCode.SETITEM	[8192 datoshi]
    /// 73 : OpCode.DROP	[2 datoshi]
    /// 74 : OpCode.LDLOC1	[2 datoshi]
    /// 75 : OpCode.CALLT 0100	[32768 datoshi]
    /// 78 : OpCode.DUP	[2 datoshi]
    /// 79 : OpCode.LDARG1	[2 datoshi]
    /// 7A : OpCode.LDLOC0	[2 datoshi]
    /// 7B : OpCode.UNPACK	[2048 datoshi]
    /// 7C : OpCode.DROP	[2 datoshi]
    /// 7D : OpCode.REVERSE3	[2 datoshi]
    /// 7E : OpCode.CAT	[2048 datoshi]
    /// 7F : OpCode.SWAP	[2 datoshi]
    /// 80 : OpCode.SYSCALL E63F1884	[0 datoshi]
    /// 85 : OpCode.DROP	[2 datoshi]
    /// 86 : OpCode.PUSHM1	[1 datoshi]
    /// 87 : OpCode.LDARG1	[2 datoshi]
    /// 88 : OpCode.LDLOC2	[2 datoshi]
    /// 89 : OpCode.CALL 0F	[512 datoshi]
    /// 8B : OpCode.PUSH1	[1 datoshi]
    /// 8C : OpCode.LDARG1	[2 datoshi]
    /// 8D : OpCode.LDARG0	[2 datoshi]
    /// 8E : OpCode.CALL 0A	[512 datoshi]
    /// 90 : OpCode.LDARG2	[2 datoshi]
    /// 91 : OpCode.LDARG1	[2 datoshi]
    /// 92 : OpCode.LDARG0	[2 datoshi]
    /// 93 : OpCode.LDLOC2	[2 datoshi]
    /// 94 : OpCode.CALL 45	[512 datoshi]
    /// 96 : OpCode.PUSHT	[1 datoshi]
    /// 97 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
