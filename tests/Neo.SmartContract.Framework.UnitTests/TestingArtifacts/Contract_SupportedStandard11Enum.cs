using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":29,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":210,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":369,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":416,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":444,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":532,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":821,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":823,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":827,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/T4DQBBAWNgmFwwBAEH2tGviQZJd6DFK2CYERRBKYEBXAQF4cGjYJgUIIg14StkoUMoAFLOrqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshcWl5nnFpELUmBAlAabEkEHhowUVTi1BBL1jF7SIPaXhowUVTi1BB5j8YhAhAVwMBeMoAQLcmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5AVwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDARuYW1laRHO0EBXAQATEYhOEFHQQZv2Z84SwHATaMFFQd8wuJpAVwEBeHBo2CYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQFcDA3hwaNgmBQgiDXhK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMJAQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQFcCA3p4Nc/9//9FQZv2Z84UEYhOEFHQUBLAcHh5i9socXoQtyYQEGlowUVTi1BB5j8YhEBpaMFFU4tQQS9Yxe1AVwEEehF5eBTADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcCAHBo2KomIHt6EXgUwB8MDm9uTkVQMTFQYXltZW50eUFifVtSRUAIQFcABEBWAUAqM5uy"));

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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 25
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 3B : OpCode.THROW
    /// 3C : OpCode.SYSCALL 9BF667CE
    /// 41 : OpCode.PUSH1
    /// 42 : OpCode.PUSH1
    /// 43 : OpCode.NEWBUFFER
    /// 44 : OpCode.TUCK
    /// 45 : OpCode.PUSH0
    /// 46 : OpCode.ROT
    /// 47 : OpCode.SETITEM
    /// 48 : OpCode.SWAP
    /// 49 : OpCode.PUSH2
    /// 4A : OpCode.PACK
    /// 4B : OpCode.STLOC0
    /// 4C : OpCode.LDARG0
    /// 4D : OpCode.LDLOC0
    /// 4E : OpCode.UNPACK
    /// 4F : OpCode.DROP
    /// 50 : OpCode.REVERSE3
    /// 51 : OpCode.CAT
    /// 52 : OpCode.SWAP
    /// 53 : OpCode.SYSCALL 925DE831
    /// 58 : OpCode.DUP
    /// 59 : OpCode.ISNULL
    /// 5A : OpCode.JMPIFNOT 04
    /// 5C : OpCode.DROP
    /// 5D : OpCode.PUSH0
    /// 5E : OpCode.CONVERT 21
    /// 60 : OpCode.RET
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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJAxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkOhQRiE4QUdBBm/ZnzhLAcBN4aMFFU4tQQd8wuJpA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 24
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964
    /// 3A : OpCode.THROW
    /// 3B : OpCode.PUSH4
    /// 3C : OpCode.PUSH1
    /// 3D : OpCode.NEWBUFFER
    /// 3E : OpCode.TUCK
    /// 3F : OpCode.PUSH0
    /// 40 : OpCode.ROT
    /// 41 : OpCode.SETITEM
    /// 42 : OpCode.SYSCALL 9BF667CE
    /// 47 : OpCode.PUSH2
    /// 48 : OpCode.PACK
    /// 49 : OpCode.STLOC0
    /// 4A : OpCode.PUSH3
    /// 4B : OpCode.LDARG0
    /// 4C : OpCode.LDLOC0
    /// 4D : OpCode.UNPACK
    /// 4E : OpCode.DROP
    /// 4F : OpCode.REVERSE3
    /// 50 : OpCode.CAT
    /// 51 : OpCode.SWAP
    /// 52 : OpCode.SYSCALL DF30B89A
    /// 57 : OpCode.RET
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
    /// Script: VwMDeHBo2CYFCCINeErZKFDKABSzq6omIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjCQECUBqeJgmJXhKaRBR0EVpNwEASnlowUVTi1BB5j8YhEUPeWo0DxF5eDQKenl4ajRFCEA=
    /// 00 : OpCode.INITSLOT 0303
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 22
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 38 : OpCode.THROW
    /// 39 : OpCode.PUSH3
    /// 3A : OpCode.PUSH1
    /// 3B : OpCode.NEWBUFFER
    /// 3C : OpCode.TUCK
    /// 3D : OpCode.PUSH0
    /// 3E : OpCode.ROT
    /// 3F : OpCode.SETITEM
    /// 40 : OpCode.SYSCALL 9BF667CE
    /// 45 : OpCode.PUSH2
    /// 46 : OpCode.PACK
    /// 47 : OpCode.STLOC0
    /// 48 : OpCode.LDARG1
    /// 49 : OpCode.LDLOC0
    /// 4A : OpCode.UNPACK
    /// 4B : OpCode.DROP
    /// 4C : OpCode.REVERSE3
    /// 4D : OpCode.CAT
    /// 4E : OpCode.SWAP
    /// 4F : OpCode.SYSCALL 925DE831
    /// 54 : OpCode.CALLT 0000
    /// 57 : OpCode.STLOC1
    /// 58 : OpCode.LDLOC1
    /// 59 : OpCode.PUSH0
    /// 5A : OpCode.PICKITEM
    /// 5B : OpCode.STLOC2
    /// 5C : OpCode.LDLOC2
    /// 5D : OpCode.SYSCALL F827EC8C
    /// 62 : OpCode.JMPIF 04
    /// 64 : OpCode.PUSHF
    /// 65 : OpCode.RET
    /// 66 : OpCode.LDLOC2
    /// 67 : OpCode.LDARG0
    /// 68 : OpCode.NOTEQUAL
    /// 69 : OpCode.JMPIFNOT 25
    /// 6B : OpCode.LDARG0
    /// 6C : OpCode.DUP
    /// 6D : OpCode.LDLOC1
    /// 6E : OpCode.PUSH0
    /// 6F : OpCode.ROT
    /// 70 : OpCode.SETITEM
    /// 71 : OpCode.DROP
    /// 72 : OpCode.LDLOC1
    /// 73 : OpCode.CALLT 0100
    /// 76 : OpCode.DUP
    /// 77 : OpCode.LDARG1
    /// 78 : OpCode.LDLOC0
    /// 79 : OpCode.UNPACK
    /// 7A : OpCode.DROP
    /// 7B : OpCode.REVERSE3
    /// 7C : OpCode.CAT
    /// 7D : OpCode.SWAP
    /// 7E : OpCode.SYSCALL E63F1884
    /// 83 : OpCode.DROP
    /// 84 : OpCode.PUSHM1
    /// 85 : OpCode.LDARG1
    /// 86 : OpCode.LDLOC2
    /// 87 : OpCode.CALL 0F
    /// 89 : OpCode.PUSH1
    /// 8A : OpCode.LDARG1
    /// 8B : OpCode.LDARG0
    /// 8C : OpCode.CALL 0A
    /// 8E : OpCode.LDARG2
    /// 8F : OpCode.LDARG1
    /// 90 : OpCode.LDARG0
    /// 91 : OpCode.LDLOC2
    /// 92 : OpCode.CALL 45
    /// 94 : OpCode.PUSHT
    /// 95 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
