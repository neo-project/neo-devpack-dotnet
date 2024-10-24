using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":35,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":185,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":344,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":391,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":419,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":508,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":805,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":807,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":811,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/TYDQBBAVwEAWDQNcGhK2CYERRDbIUBXAAF4Qfa0a+JBkl3oMUBXAQF4cGgLlyYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46WXiL2yg1r////3BoStgmBEUQ2yFAVwQCQZv2Z85wWXiL2yhxaWhBkl3oMXJqStgmBEUQ2yFza3mec2sQtSYECUBrEJcmC2loQS9Yxe0iCmtpaEHmPxiECEBXAwF4ygBAtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDC5UaGUgdG9rZW4gd2l0aCBnaXZlbiAidG9rZW5JZCIgZG9lcyBub3QgZXhpc3QuOnFpNwAAcmoQzkBXAgETEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DE3AABxyEoMBG5hbWVpEc7QQFcBABMRiE4QUdBBm/ZnzhLAcBNowUVB3zC4mkBXAQF4cGgLlyYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQFcDA3hwaAuXJgUIIg14StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjKomBAlAaniYJiV4SmkQUdBFaTcBAEp5aMFFU4tQQeY/GIRFD3lqNBIReXg0DXp5eGo1SAAAAAhAVwIDeng12f3//0VBm/ZnzhQRiE4QUdBQEsBweHmL2yhxehC3JhAQaWjBRVOLUEHmPxiEQGlowUVTi1BBL1jF7UBXAQR6EXl4FMAMCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AgBwaAuXqiYge3oReBTAHwwOb25ORVAxMVBheW1lbnR5QWJ9W1JFQAhAVwAEQFYCDAEAYAwBAWFAA0ZAMA=="));

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
    /// Script: VwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46WXiL2yg1r////3BoStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 25
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 3C : OpCode.THROW
    /// 3D : OpCode.LDSFLD1
    /// 3E : OpCode.LDARG0
    /// 3F : OpCode.CAT
    /// 40 : OpCode.CONVERT 28
    /// 42 : OpCode.CALL_L AFFFFFFF
    /// 47 : OpCode.STLOC0
    /// 48 : OpCode.LDLOC0
    /// 49 : OpCode.DUP
    /// 4A : OpCode.ISNULL
    /// 4B : OpCode.JMPIFNOT 04
    /// 4D : OpCode.DROP
    /// 4E : OpCode.PUSH0
    /// 4F : OpCode.CONVERT 21
    /// 51 : OpCode.RET
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
    /// Script: VwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 24
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964
    /// 3B : OpCode.THROW
    /// 3C : OpCode.PUSH4
    /// 3D : OpCode.PUSH1
    /// 3E : OpCode.NEWBUFFER
    /// 3F : OpCode.TUCK
    /// 40 : OpCode.PUSH0
    /// 41 : OpCode.ROT
    /// 42 : OpCode.SETITEM
    /// 43 : OpCode.SYSCALL 9BF667CE
    /// 48 : OpCode.PUSH2
    /// 49 : OpCode.PACK
    /// 4A : OpCode.STLOC0
    /// 4B : OpCode.PUSH3
    /// 4C : OpCode.LDARG0
    /// 4D : OpCode.LDLOC0
    /// 4E : OpCode.UNPACK
    /// 4F : OpCode.DROP
    /// 50 : OpCode.REVERSE3
    /// 51 : OpCode.CAT
    /// 52 : OpCode.SWAP
    /// 53 : OpCode.SYSCALL DF30B89A
    /// 58 : OpCode.RET
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
    /// Script: VwMDeHBoC5cmBQgiDXhK2ShQygAUs6uqJiIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcWkQznJqQfgn7IyqJgQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQSEXl4NA16eXhqNUgAAAAIQA==
    /// 00 : OpCode.INITSLOT 0303
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 22
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 39 : OpCode.THROW
    /// 3A : OpCode.PUSH3
    /// 3B : OpCode.PUSH1
    /// 3C : OpCode.NEWBUFFER
    /// 3D : OpCode.TUCK
    /// 3E : OpCode.PUSH0
    /// 3F : OpCode.ROT
    /// 40 : OpCode.SETITEM
    /// 41 : OpCode.SYSCALL 9BF667CE
    /// 46 : OpCode.PUSH2
    /// 47 : OpCode.PACK
    /// 48 : OpCode.STLOC0
    /// 49 : OpCode.LDARG1
    /// 4A : OpCode.LDLOC0
    /// 4B : OpCode.UNPACK
    /// 4C : OpCode.DROP
    /// 4D : OpCode.REVERSE3
    /// 4E : OpCode.CAT
    /// 4F : OpCode.SWAP
    /// 50 : OpCode.SYSCALL 925DE831
    /// 55 : OpCode.CALLT 0000
    /// 58 : OpCode.STLOC1
    /// 59 : OpCode.LDLOC1
    /// 5A : OpCode.PUSH0
    /// 5B : OpCode.PICKITEM
    /// 5C : OpCode.STLOC2
    /// 5D : OpCode.LDLOC2
    /// 5E : OpCode.SYSCALL F827EC8C
    /// 63 : OpCode.NOT
    /// 64 : OpCode.JMPIFNOT 04
    /// 66 : OpCode.PUSHF
    /// 67 : OpCode.RET
    /// 68 : OpCode.LDLOC2
    /// 69 : OpCode.LDARG0
    /// 6A : OpCode.NOTEQUAL
    /// 6B : OpCode.JMPIFNOT 25
    /// 6D : OpCode.LDARG0
    /// 6E : OpCode.DUP
    /// 6F : OpCode.LDLOC1
    /// 70 : OpCode.PUSH0
    /// 71 : OpCode.ROT
    /// 72 : OpCode.SETITEM
    /// 73 : OpCode.DROP
    /// 74 : OpCode.LDLOC1
    /// 75 : OpCode.CALLT 0100
    /// 78 : OpCode.DUP
    /// 79 : OpCode.LDARG1
    /// 7A : OpCode.LDLOC0
    /// 7B : OpCode.UNPACK
    /// 7C : OpCode.DROP
    /// 7D : OpCode.REVERSE3
    /// 7E : OpCode.CAT
    /// 7F : OpCode.SWAP
    /// 80 : OpCode.SYSCALL E63F1884
    /// 85 : OpCode.DROP
    /// 86 : OpCode.PUSHM1
    /// 87 : OpCode.LDARG1
    /// 88 : OpCode.LDLOC2
    /// 89 : OpCode.CALL 12
    /// 8B : OpCode.PUSH1
    /// 8C : OpCode.LDARG1
    /// 8D : OpCode.LDARG0
    /// 8E : OpCode.CALL 0D
    /// 90 : OpCode.LDARG2
    /// 91 : OpCode.LDARG1
    /// 92 : OpCode.LDARG0
    /// 93 : OpCode.LDLOC2
    /// 94 : OpCode.CALL_L 48000000
    /// 99 : OpCode.PUSHT
    /// 9A : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
