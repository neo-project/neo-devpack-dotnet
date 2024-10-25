using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":37,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":182,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":341,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":388,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":416,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":499,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":787,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":789,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/RkDQBBAVwEADAEANA1waErYJgRFENshQFcAAXhB9rRr4kGSXegxQFcBAXhK2SgkBkUJIgbKABSzqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAQF4i9soNLNwaErYJgRFENshQFcEAkGb9mfOcAwBAXiL2yhxaWhBkl3oMXJqStgmBEUQ2yFza3mec2sQtSYECUBrELMmC2loQS9Yxe0iCmtpaEHmPxiECEBXAwF4ygBAtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDC5UaGUgdG9rZW4gd2l0aCBnaXZlbiAidG9rZW5JZCIgZG9lcyBub3QgZXhpc3QuOnFpNwAAcmoQzkBXAgETEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DE3AABxyEoMBG5hbWVpEc7QQFcBABMRiE4QUdBBm/ZnzhLAcBNowUVB3zC4mkBXAQF4StkoJAZFCSIGygAUs6omJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQFcDA3hK2SgkBkUJIgbKABSzqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjKomBAlAaniYJiV4SmkQUdBFaTcBAEp5aMFFU4tQQeY/GIRFD3lqNA8ReXg0Cnp5eGo0RQhAVwIDeng15v3//0VBm/ZnzhQRiE4QUdBQEsBweHmL2yhxehC3JhAQaWjBRVOLUEHmPxiEQGlowUVTi1BBL1jF7UBXAQR6EXl4FMAMCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AgBwaAuXqiYge3oReBTAHwwOb25ORVAxMVBheW1lbnR5QWJ9W1JFQAhAVwAEQD9QfIg="));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLOqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAF4i9soNLNwaErYJgRFENshQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.NOT
    /// 12 : OpCode.JMPIFNOT 25
    /// 14 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 36 : OpCode.THROW
    /// 37 : OpCode.PUSHDATA1 01
    /// 3A : OpCode.LDARG0
    /// 3B : OpCode.CAT
    /// 3C : OpCode.CONVERT 28
    /// 3E : OpCode.CALL B3
    /// 40 : OpCode.STLOC0
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.DUP
    /// 43 : OpCode.ISNULL
    /// 44 : OpCode.JMPIFNOT 04
    /// 46 : OpCode.DROP
    /// 47 : OpCode.PUSH0
    /// 48 : OpCode.CONVERT 21
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMoAQLcmPAxUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDFRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByahDOQA==
    /// 00 : OpCode.INITSLOT 0301
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SIZE
    /// 05 : OpCode.PUSHINT8 40
    /// 07 : OpCode.GT
    /// 08 : OpCode.JMPIFNOT 3C
    /// 0A : OpCode.PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203634206F72206C657373206279746573206C6F6E672E
    /// 43 : OpCode.THROW
    /// 44 : OpCode.PUSH3
    /// 45 : OpCode.PUSH1
    /// 46 : OpCode.NEWBUFFER
    /// 47 : OpCode.TUCK
    /// 48 : OpCode.PUSH0
    /// 49 : OpCode.ROT
    /// 4A : OpCode.SETITEM
    /// 4B : OpCode.SYSCALL 9BF667CE
    /// 50 : OpCode.PUSH2
    /// 51 : OpCode.PACK
    /// 52 : OpCode.STLOC0
    /// 53 : OpCode.LDARG0
    /// 54 : OpCode.LDLOC0
    /// 55 : OpCode.UNPACK
    /// 56 : OpCode.DROP
    /// 57 : OpCode.REVERSE3
    /// 58 : OpCode.CAT
    /// 59 : OpCode.SWAP
    /// 5A : OpCode.SYSCALL 925DE831
    /// 5F : OpCode.DUP
    /// 60 : OpCode.ISNULL
    /// 61 : OpCode.JMPIFNOT 34
    /// 63 : OpCode.DROP
    /// 64 : OpCode.PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E
    /// 94 : OpCode.THROW
    /// 95 : OpCode.STLOC1
    /// 96 : OpCode.LDLOC1
    /// 97 : OpCode.CALLT 0000
    /// 9A : OpCode.STLOC2
    /// 9B : OpCode.LDLOC2
    /// 9C : OpCode.PUSH0
    /// 9D : OpCode.PICKITEM
    /// 9E : OpCode.RET
    /// </remarks>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDG5hbWVpEc7QQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.PUSH1
    /// 05 : OpCode.NEWBUFFER
    /// 06 : OpCode.TUCK
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.ROT
    /// 09 : OpCode.SETITEM
    /// 0A : OpCode.SYSCALL 9BF667CE
    /// 0F : OpCode.PUSH2
    /// 10 : OpCode.PACK
    /// 11 : OpCode.STLOC0
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.UNPACK
    /// 15 : OpCode.DROP
    /// 16 : OpCode.REVERSE3
    /// 17 : OpCode.CAT
    /// 18 : OpCode.SWAP
    /// 19 : OpCode.SYSCALL 925DE831
    /// 1E : OpCode.CALLT 0000
    /// 21 : OpCode.STLOC1
    /// 22 : OpCode.NEWMAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.PUSHDATA1 6E616D65
    /// 2A : OpCode.LDLOC1
    /// 2B : OpCode.PUSH1
    /// 2C : OpCode.PICKITEM
    /// 2D : OpCode.SETITEM
    /// 2E : OpCode.RET
    /// </remarks>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErZKCQGRQkiBsoAFLOqJiQMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.NOT
    /// 12 : OpCode.JMPIFNOT 24
    /// 14 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964
    /// 35 : OpCode.THROW
    /// 36 : OpCode.PUSH4
    /// 37 : OpCode.PUSH1
    /// 38 : OpCode.NEWBUFFER
    /// 39 : OpCode.TUCK
    /// 3A : OpCode.PUSH0
    /// 3B : OpCode.ROT
    /// 3C : OpCode.SETITEM
    /// 3D : OpCode.SYSCALL 9BF667CE
    /// 42 : OpCode.PUSH2
    /// 43 : OpCode.PACK
    /// 44 : OpCode.STLOC0
    /// 45 : OpCode.PUSH3
    /// 46 : OpCode.LDARG0
    /// 47 : OpCode.LDLOC0
    /// 48 : OpCode.UNPACK
    /// 49 : OpCode.DROP
    /// 4A : OpCode.REVERSE3
    /// 4B : OpCode.CAT
    /// 4C : OpCode.SWAP
    /// 4D : OpCode.SYSCALL DF30B89A
    /// 52 : OpCode.RET
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
    /// 00 : OpCode.INITSLOT 0004
    /// 03 : OpCode.RET
    /// </remarks>
    [DisplayName("onNEP11Payment")]
    public abstract void OnNEP11Payment(UInt160? from, BigInteger? amount, string? tokenId, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CEA=
    /// 00 : OpCode.PUSHT
    /// 01 : OpCode.RET
    /// </remarks>
    [DisplayName("testStandard")]
    public abstract bool? TestStandard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDeErZKCQGRQkiBsoAFLOqJiIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcWkQznJqQfgn7IyqJgQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQA==
    /// 00 : OpCode.INITSLOT 0303
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.NOT
    /// 12 : OpCode.JMPIFNOT 22
    /// 14 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 33 : OpCode.THROW
    /// 34 : OpCode.PUSH3
    /// 35 : OpCode.PUSH1
    /// 36 : OpCode.NEWBUFFER
    /// 37 : OpCode.TUCK
    /// 38 : OpCode.PUSH0
    /// 39 : OpCode.ROT
    /// 3A : OpCode.SETITEM
    /// 3B : OpCode.SYSCALL 9BF667CE
    /// 40 : OpCode.PUSH2
    /// 41 : OpCode.PACK
    /// 42 : OpCode.STLOC0
    /// 43 : OpCode.LDARG1
    /// 44 : OpCode.LDLOC0
    /// 45 : OpCode.UNPACK
    /// 46 : OpCode.DROP
    /// 47 : OpCode.REVERSE3
    /// 48 : OpCode.CAT
    /// 49 : OpCode.SWAP
    /// 4A : OpCode.SYSCALL 925DE831
    /// 4F : OpCode.CALLT 0000
    /// 52 : OpCode.STLOC1
    /// 53 : OpCode.LDLOC1
    /// 54 : OpCode.PUSH0
    /// 55 : OpCode.PICKITEM
    /// 56 : OpCode.STLOC2
    /// 57 : OpCode.LDLOC2
    /// 58 : OpCode.SYSCALL F827EC8C
    /// 5D : OpCode.NOT
    /// 5E : OpCode.JMPIFNOT 04
    /// 60 : OpCode.PUSHF
    /// 61 : OpCode.RET
    /// 62 : OpCode.LDLOC2
    /// 63 : OpCode.LDARG0
    /// 64 : OpCode.NOTEQUAL
    /// 65 : OpCode.JMPIFNOT 25
    /// 67 : OpCode.LDARG0
    /// 68 : OpCode.DUP
    /// 69 : OpCode.LDLOC1
    /// 6A : OpCode.PUSH0
    /// 6B : OpCode.ROT
    /// 6C : OpCode.SETITEM
    /// 6D : OpCode.DROP
    /// 6E : OpCode.LDLOC1
    /// 6F : OpCode.CALLT 0100
    /// 72 : OpCode.DUP
    /// 73 : OpCode.LDARG1
    /// 74 : OpCode.LDLOC0
    /// 75 : OpCode.UNPACK
    /// 76 : OpCode.DROP
    /// 77 : OpCode.REVERSE3
    /// 78 : OpCode.CAT
    /// 79 : OpCode.SWAP
    /// 7A : OpCode.SYSCALL E63F1884
    /// 7F : OpCode.DROP
    /// 80 : OpCode.PUSHM1
    /// 81 : OpCode.LDARG1
    /// 82 : OpCode.LDLOC2
    /// 83 : OpCode.CALL 0F
    /// 85 : OpCode.PUSH1
    /// 86 : OpCode.LDARG1
    /// 87 : OpCode.LDARG0
    /// 88 : OpCode.CALL 0A
    /// 8A : OpCode.LDARG2
    /// 8B : OpCode.LDARG1
    /// 8C : OpCode.LDARG0
    /// 8D : OpCode.LDLOC2
    /// 8E : OpCode.CALL 45
    /// 90 : OpCode.PUSHT
    /// 91 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
