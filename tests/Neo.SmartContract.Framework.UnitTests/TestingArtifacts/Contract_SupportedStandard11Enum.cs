using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":37,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":180,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":339,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":386,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":414,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":496,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":779,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":781,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/REDQBBAVwEADAEANA1waErYJgRFENshQFcAAXhB9rRr4kGSXegxQFcBAXhK2SgkBkUJIgbKABSzJCUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOgwBAXiL2yg0tHBoStgmBEUQ2yFAVwQCQZv2Z85wDAEBeIvbKHFpaEGSXegxcmpK2CYERRDbIXNreZ5zaxC1JgQJQGuxJAtpaEEvWMXtIgpraWhB5j8YhAhAVwMBeMoAQLcmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5AVwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDARuYW1laRHO0EBXAQATEYhOEFHQQZv2Z84SwHATaMFFQd8wuJpAVwEBeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQFcDA3hK2SgkBkUJIgbKABSzJCIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMJAQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQFcCA3p4Ner9//9FQZv2Z84UEYhOEFHQUBLAcHh5i9socXoQtyYQEGlowUVTi1BB5j8YhEBpaMFFU4tQQS9Yxe1AVwEEehF5eBTADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcCAHBo2KomIHt6EXgUwB8MDm9uTkVQMTFQYXltZW50eUFifVtSRUAIQFcABECxO1mq"));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAXiL2yg0tHBoStgmBEUQ2yFA
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
    /// 11 : OpCode.JMPIF 25
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 35 : OpCode.THROW
    /// 36 : OpCode.PUSHDATA1 01
    /// 39 : OpCode.LDARG0
    /// 3A : OpCode.CAT
    /// 3B : OpCode.CONVERT 28
    /// 3D : OpCode.CALL B4
    /// 3F : OpCode.STLOC0
    /// 40 : OpCode.LDLOC0
    /// 41 : OpCode.DUP
    /// 42 : OpCode.ISNULL
    /// 43 : OpCode.JMPIFNOT 04
    /// 45 : OpCode.DROP
    /// 46 : OpCode.PUSH0
    /// 47 : OpCode.CONVERT 21
    /// 49 : OpCode.RET
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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJAxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkOhQRiE4QUdBBm/ZnzhLAcBN4aMFFU4tQQd8wuJpA
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
    /// 11 : OpCode.JMPIF 24
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964
    /// 34 : OpCode.THROW
    /// 35 : OpCode.PUSH4
    /// 36 : OpCode.PUSH1
    /// 37 : OpCode.NEWBUFFER
    /// 38 : OpCode.TUCK
    /// 39 : OpCode.PUSH0
    /// 3A : OpCode.ROT
    /// 3B : OpCode.SETITEM
    /// 3C : OpCode.SYSCALL 9BF667CE
    /// 41 : OpCode.PUSH2
    /// 42 : OpCode.PACK
    /// 43 : OpCode.STLOC0
    /// 44 : OpCode.PUSH3
    /// 45 : OpCode.LDARG0
    /// 46 : OpCode.LDLOC0
    /// 47 : OpCode.UNPACK
    /// 48 : OpCode.DROP
    /// 49 : OpCode.REVERSE3
    /// 4A : OpCode.CAT
    /// 4B : OpCode.SWAP
    /// 4C : OpCode.SYSCALL DF30B89A
    /// 51 : OpCode.RET
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
    /// Script: VwMDeErZKCQGRQkiBsoAFLMkIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjCQECUBqeJgmJXhKaRBR0EVpNwEASnlowUVTi1BB5j8YhEUPeWo0DxF5eDQKenl4ajRFCEA=
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
    /// 11 : OpCode.JMPIF 22
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 32 : OpCode.THROW
    /// 33 : OpCode.PUSH3
    /// 34 : OpCode.PUSH1
    /// 35 : OpCode.NEWBUFFER
    /// 36 : OpCode.TUCK
    /// 37 : OpCode.PUSH0
    /// 38 : OpCode.ROT
    /// 39 : OpCode.SETITEM
    /// 3A : OpCode.SYSCALL 9BF667CE
    /// 3F : OpCode.PUSH2
    /// 40 : OpCode.PACK
    /// 41 : OpCode.STLOC0
    /// 42 : OpCode.LDARG1
    /// 43 : OpCode.LDLOC0
    /// 44 : OpCode.UNPACK
    /// 45 : OpCode.DROP
    /// 46 : OpCode.REVERSE3
    /// 47 : OpCode.CAT
    /// 48 : OpCode.SWAP
    /// 49 : OpCode.SYSCALL 925DE831
    /// 4E : OpCode.CALLT 0000
    /// 51 : OpCode.STLOC1
    /// 52 : OpCode.LDLOC1
    /// 53 : OpCode.PUSH0
    /// 54 : OpCode.PICKITEM
    /// 55 : OpCode.STLOC2
    /// 56 : OpCode.LDLOC2
    /// 57 : OpCode.SYSCALL F827EC8C
    /// 5C : OpCode.JMPIF 04
    /// 5E : OpCode.PUSHF
    /// 5F : OpCode.RET
    /// 60 : OpCode.LDLOC2
    /// 61 : OpCode.LDARG0
    /// 62 : OpCode.NOTEQUAL
    /// 63 : OpCode.JMPIFNOT 25
    /// 65 : OpCode.LDARG0
    /// 66 : OpCode.DUP
    /// 67 : OpCode.LDLOC1
    /// 68 : OpCode.PUSH0
    /// 69 : OpCode.ROT
    /// 6A : OpCode.SETITEM
    /// 6B : OpCode.DROP
    /// 6C : OpCode.LDLOC1
    /// 6D : OpCode.CALLT 0100
    /// 70 : OpCode.DUP
    /// 71 : OpCode.LDARG1
    /// 72 : OpCode.LDLOC0
    /// 73 : OpCode.UNPACK
    /// 74 : OpCode.DROP
    /// 75 : OpCode.REVERSE3
    /// 76 : OpCode.CAT
    /// 77 : OpCode.SWAP
    /// 78 : OpCode.SYSCALL E63F1884
    /// 7D : OpCode.DROP
    /// 7E : OpCode.PUSHM1
    /// 7F : OpCode.LDARG1
    /// 80 : OpCode.LDLOC2
    /// 81 : OpCode.CALL 0F
    /// 83 : OpCode.PUSH1
    /// 84 : OpCode.LDARG1
    /// 85 : OpCode.LDARG0
    /// 86 : OpCode.CALL 0A
    /// 88 : OpCode.LDARG2
    /// 89 : OpCode.LDARG1
    /// 8A : OpCode.LDARG0
    /// 8B : OpCode.LDLOC2
    /// 8C : OpCode.CALL 45
    /// 8E : OpCode.PUSHT
    /// 8F : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
