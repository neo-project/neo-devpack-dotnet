using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP11(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP11"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":198,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":357,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":404,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":432,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":525,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/TcDDARURVNUQBBAVwEADAEANA1waErYJgRFENshQFcAAXhB9rRr4kGSXegxQFcBAXhwaAuXJgUIIhF4StkoJAZFCSIGygAUs6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSpcGhK2CYERRDbIUBXBAJBm/ZnznAMAQF4i9socWloQZJd6DFyakrYJgRFENshc2t5nnNrELUmBAlAaxCzJgtpaEEvWMXtIgpraWhB5j8YhAhAVwMBeMoAQLcmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5AVwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDARuYW1laRHO0EBXAQATEYhOEFHQQZv2Z84SwHATaMFFQd8wuJpAVwEBeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYkDB9UaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkOhQRiE4QUdBBm/ZnzhLAcBN4aMFFU4tQQd8wuJpAVwMDeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjKomBAlAaniYJiV4SmkQUdBFaTcBAEp5aMFFU4tQQeY/GIRFD3lqNA8ReXg0Cnp5eGo0RQhAVwIDeng10v3//0VBm/ZnzhQRiE4QUdBQEsBweHmL2yhxehC3JhAQaWjBRVOLUEHmPxiEQGlowUVTi1BBL1jF7UBXAQR6EXl4FMAMCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AgBwaAuXqiYge3oReBTAHwwOb25ORVAxMVBheW1lbnR5QWJ9W1JFQO9Il+8="));

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
    /// Script: VwEBeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYlDFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOgwBeIvbKDSpcGhK2CYERRDbIUA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 11
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.JMPIF 06
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSHF
    /// 15 : OpCode.JMP 06
    /// 17 : OpCode.SIZE
    /// 18 : OpCode.PUSHINT8 14
    /// 1A : OpCode.NUMEQUAL
    /// 1B : OpCode.NOT
    /// 1C : OpCode.JMPIFNOT 25
    /// 1E : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 40 : OpCode.THROW
    /// 41 : OpCode.PUSHDATA1 01
    /// 44 : OpCode.LDARG0
    /// 45 : OpCode.CAT
    /// 46 : OpCode.CONVERT 28
    /// 48 : OpCode.CALL A9
    /// 4A : OpCode.STLOC0
    /// 4B : OpCode.LDLOC0
    /// 4C : OpCode.DUP
    /// 4D : OpCode.ISNULL
    /// 4E : OpCode.JMPIFNOT 04
    /// 50 : OpCode.DROP
    /// 51 : OpCode.PUSH0
    /// 52 : OpCode.CONVERT 21
    /// 54 : OpCode.RET
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
    /// Script: VwEBeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYkDFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQ6FBGIThBR0EGb9mfOEsBwE3howUVTi1BB3zC4mkA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 11
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.JMPIF 06
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSHF
    /// 15 : OpCode.JMP 06
    /// 17 : OpCode.SIZE
    /// 18 : OpCode.PUSHINT8 14
    /// 1A : OpCode.NUMEQUAL
    /// 1B : OpCode.NOT
    /// 1C : OpCode.JMPIFNOT 24
    /// 1E : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964
    /// 3F : OpCode.THROW
    /// 40 : OpCode.PUSH4
    /// 41 : OpCode.PUSH1
    /// 42 : OpCode.NEWBUFFER
    /// 43 : OpCode.TUCK
    /// 44 : OpCode.PUSH0
    /// 45 : OpCode.ROT
    /// 46 : OpCode.SETITEM
    /// 47 : OpCode.SYSCALL 9BF667CE
    /// 4C : OpCode.PUSH2
    /// 4D : OpCode.PACK
    /// 4E : OpCode.STLOC0
    /// 4F : OpCode.PUSH3
    /// 50 : OpCode.LDARG0
    /// 51 : OpCode.LDLOC0
    /// 52 : OpCode.UNPACK
    /// 53 : OpCode.DROP
    /// 54 : OpCode.REVERSE3
    /// 55 : OpCode.CAT
    /// 56 : OpCode.SWAP
    /// 57 : OpCode.SYSCALL DF30B89A
    /// 5C : OpCode.RET
    /// </remarks>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYiDFRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMqiYECUBqeJgmJXhKaRBR0EVpNwEASnlowUVTi1BB5j8YhEUPeWo0DxF5eDQKenl4ajRFCEA=
    /// 00 : OpCode.INITSLOT 0303
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 11
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.JMPIF 06
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSHF
    /// 15 : OpCode.JMP 06
    /// 17 : OpCode.SIZE
    /// 18 : OpCode.PUSHINT8 14
    /// 1A : OpCode.NUMEQUAL
    /// 1B : OpCode.NOT
    /// 1C : OpCode.JMPIFNOT 22
    /// 1E : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 3D : OpCode.THROW
    /// 3E : OpCode.PUSH3
    /// 3F : OpCode.PUSH1
    /// 40 : OpCode.NEWBUFFER
    /// 41 : OpCode.TUCK
    /// 42 : OpCode.PUSH0
    /// 43 : OpCode.ROT
    /// 44 : OpCode.SETITEM
    /// 45 : OpCode.SYSCALL 9BF667CE
    /// 4A : OpCode.PUSH2
    /// 4B : OpCode.PACK
    /// 4C : OpCode.STLOC0
    /// 4D : OpCode.LDARG1
    /// 4E : OpCode.LDLOC0
    /// 4F : OpCode.UNPACK
    /// 50 : OpCode.DROP
    /// 51 : OpCode.REVERSE3
    /// 52 : OpCode.CAT
    /// 53 : OpCode.SWAP
    /// 54 : OpCode.SYSCALL 925DE831
    /// 59 : OpCode.CALLT 0000
    /// 5C : OpCode.STLOC1
    /// 5D : OpCode.LDLOC1
    /// 5E : OpCode.PUSH0
    /// 5F : OpCode.PICKITEM
    /// 60 : OpCode.STLOC2
    /// 61 : OpCode.LDLOC2
    /// 62 : OpCode.SYSCALL F827EC8C
    /// 67 : OpCode.NOT
    /// 68 : OpCode.JMPIFNOT 04
    /// 6A : OpCode.PUSHF
    /// 6B : OpCode.RET
    /// 6C : OpCode.LDLOC2
    /// 6D : OpCode.LDARG0
    /// 6E : OpCode.NOTEQUAL
    /// 6F : OpCode.JMPIFNOT 25
    /// 71 : OpCode.LDARG0
    /// 72 : OpCode.DUP
    /// 73 : OpCode.LDLOC1
    /// 74 : OpCode.PUSH0
    /// 75 : OpCode.ROT
    /// 76 : OpCode.SETITEM
    /// 77 : OpCode.DROP
    /// 78 : OpCode.LDLOC1
    /// 79 : OpCode.CALLT 0100
    /// 7C : OpCode.DUP
    /// 7D : OpCode.LDARG1
    /// 7E : OpCode.LDLOC0
    /// 7F : OpCode.UNPACK
    /// 80 : OpCode.DROP
    /// 81 : OpCode.REVERSE3
    /// 82 : OpCode.CAT
    /// 83 : OpCode.SWAP
    /// 84 : OpCode.SYSCALL E63F1884
    /// 89 : OpCode.DROP
    /// 8A : OpCode.PUSHM1
    /// 8B : OpCode.LDARG1
    /// 8C : OpCode.LDLOC2
    /// 8D : OpCode.CALL 0F
    /// 8F : OpCode.PUSH1
    /// 90 : OpCode.LDARG1
    /// 91 : OpCode.LDARG0
    /// 92 : OpCode.CALL 0A
    /// 94 : OpCode.LDARG2
    /// 95 : OpCode.LDARG1
    /// 96 : OpCode.LDARG0
    /// 97 : OpCode.LDLOC2
    /// 98 : OpCode.CALL 45
    /// 9A : OpCode.PUSHT
    /// 9B : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
