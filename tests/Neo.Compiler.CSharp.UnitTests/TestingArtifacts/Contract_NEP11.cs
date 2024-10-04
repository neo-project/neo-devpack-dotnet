using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP11(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP11"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":903,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":918,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":40,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":66,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":249,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":933,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":450,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":478,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":567,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":868,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/bUDVwABDARURVNUQFcAAXg0A0BXAAF4NANAVwABeDQDQFcAAUBXAAEQQFrYJhcMAQBB9rRr4kGSXegxStgmBEUQSmJAVwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFxaXmecWkQtSYECUBpELMmEHhowUVTi1BBL1jF7SIPaXhowUVTi1BB5j8YhAhAVwMBeMoAQLcmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5AVwICExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcchpEc5LU9BAVwEAExGIThBR0EGb9mfOEsBwE2jBRUHfMLiaQFcBAXhwaAuXJgUIIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkOhQRiE4QUdBBm/ZnzhLAcBN4aMFFU4tQQd8wuJpAVwMDeHBoC5cmBQgiDXhK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMqiYECUBqeJgmJXhKaRBR0EVpNwEASnlowUVTi1BB5j8YhEUPeWo0DxF5eDQKenl4ajRFCEBXAgN6eDXQ/f//RUGb9mfOFBGIThBR0FASwHB4eYvbKHF6ELcmEBBpaMFFU4tQQeY/GIRAaWjBRVOLUEEvWMXtQFcBBMJKeM9Kec9KEc9Kes8MCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AgBwaAuXqiYge3oReBTAHwwOb25ORVAxMVBheW1lbnR5QWJ9W1JFQFYDCjL+//8KuPz//wqQ/P//E8BgCiD+//8Kpvz//wsTwGFAwkpYz0o1fvz//yNv/P//wkpZz0o1dvz//yOD/P//wkpZz0o1Z/z//yPp/f//QMfYOpA="));

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
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 0D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ISTYPE 28
    /// 0011 : OpCode.SWAP
    /// 0012 : OpCode.SIZE
    /// 0013 : OpCode.PUSHINT8 14
    /// 0015 : OpCode.NUMEQUAL
    /// 0016 : OpCode.BOOLAND
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIFNOT 25
    /// 001A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 003C : OpCode.THROW
    /// 003D : OpCode.SYSCALL 9BF667CE
    /// 0042 : OpCode.PUSH1
    /// 0043 : OpCode.PUSH1
    /// 0044 : OpCode.NEWBUFFER
    /// 0045 : OpCode.TUCK
    /// 0046 : OpCode.PUSH0
    /// 0047 : OpCode.ROT
    /// 0048 : OpCode.SETITEM
    /// 0049 : OpCode.SWAP
    /// 004A : OpCode.PUSH2
    /// 004B : OpCode.PACK
    /// 004C : OpCode.STLOC0
    /// 004D : OpCode.LDARG0
    /// 004E : OpCode.LDLOC0
    /// 004F : OpCode.UNPACK
    /// 0050 : OpCode.DROP
    /// 0051 : OpCode.REVERSE3
    /// 0052 : OpCode.CAT
    /// 0053 : OpCode.SWAP
    /// 0054 : OpCode.SYSCALL 925DE831
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.ISNULL
    /// 005B : OpCode.JMPIFNOT 04
    /// 005D : OpCode.DROP
    /// 005E : OpCode.PUSH0
    /// 005F : OpCode.CONVERT 21
    /// 0061 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0301
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.PUSHINT8 40
    /// 0007 : OpCode.GT
    /// 0008 : OpCode.JMPIFNOT 3C
    /// 000A : OpCode.PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203634206F72206C657373206279746573206C6F6E672E
    /// 0043 : OpCode.THROW
    /// 0044 : OpCode.PUSH3
    /// 0045 : OpCode.PUSH1
    /// 0046 : OpCode.NEWBUFFER
    /// 0047 : OpCode.TUCK
    /// 0048 : OpCode.PUSH0
    /// 0049 : OpCode.ROT
    /// 004A : OpCode.SETITEM
    /// 004B : OpCode.SYSCALL 9BF667CE
    /// 0050 : OpCode.PUSH2
    /// 0051 : OpCode.PACK
    /// 0052 : OpCode.STLOC0
    /// 0053 : OpCode.LDARG0
    /// 0054 : OpCode.LDLOC0
    /// 0055 : OpCode.UNPACK
    /// 0056 : OpCode.DROP
    /// 0057 : OpCode.REVERSE3
    /// 0058 : OpCode.CAT
    /// 0059 : OpCode.SWAP
    /// 005A : OpCode.SYSCALL 925DE831
    /// 005F : OpCode.DUP
    /// 0060 : OpCode.ISNULL
    /// 0061 : OpCode.JMPIFNOT 34
    /// 0063 : OpCode.DROP
    /// 0064 : OpCode.PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E
    /// 0094 : OpCode.THROW
    /// 0095 : OpCode.STLOC1
    /// 0096 : OpCode.LDLOC1
    /// 0097 : OpCode.CALLT 0000
    /// 009A : OpCode.STLOC2
    /// 009B : OpCode.LDLOC2
    /// 009C : OpCode.PUSH0
    /// 009D : OpCode.PICKITEM
    /// 009E : OpCode.RET
    /// </remarks>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSH3
    /// 0004 : OpCode.PUSH1
    /// 0005 : OpCode.NEWBUFFER
    /// 0006 : OpCode.TUCK
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.ROT
    /// 0009 : OpCode.SETITEM
    /// 000A : OpCode.SYSCALL 9BF667CE
    /// 000F : OpCode.PUSH2
    /// 0010 : OpCode.PACK
    /// 0011 : OpCode.STLOC0
    /// 0012 : OpCode.LDARG1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.UNPACK
    /// 0015 : OpCode.DROP
    /// 0016 : OpCode.REVERSE3
    /// 0017 : OpCode.CAT
    /// 0018 : OpCode.SWAP
    /// 0019 : OpCode.SYSCALL 925DE831
    /// 001E : OpCode.CALLT 0000
    /// 0021 : OpCode.STLOC1
    /// 0022 : OpCode.NEWMAP
    /// 0023 : OpCode.LDLOC1
    /// 0024 : OpCode.PUSH1
    /// 0025 : OpCode.PICKITEM
    /// 0026 : OpCode.OVER
    /// 0027 : OpCode.REVERSE3
    /// 0028 : OpCode.SETITEM
    /// 0029 : OpCode.RET
    /// </remarks>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 0D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ISTYPE 28
    /// 0011 : OpCode.SWAP
    /// 0012 : OpCode.SIZE
    /// 0013 : OpCode.PUSHINT8 14
    /// 0015 : OpCode.NUMEQUAL
    /// 0016 : OpCode.BOOLAND
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIFNOT 24
    /// 001A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964
    /// 003B : OpCode.THROW
    /// 003C : OpCode.PUSH4
    /// 003D : OpCode.PUSH1
    /// 003E : OpCode.NEWBUFFER
    /// 003F : OpCode.TUCK
    /// 0040 : OpCode.PUSH0
    /// 0041 : OpCode.ROT
    /// 0042 : OpCode.SETITEM
    /// 0043 : OpCode.SYSCALL 9BF667CE
    /// 0048 : OpCode.PUSH2
    /// 0049 : OpCode.PACK
    /// 004A : OpCode.STLOC0
    /// 004B : OpCode.PUSH3
    /// 004C : OpCode.LDARG0
    /// 004D : OpCode.LDLOC0
    /// 004E : OpCode.UNPACK
    /// 004F : OpCode.DROP
    /// 0050 : OpCode.REVERSE3
    /// 0051 : OpCode.CAT
    /// 0052 : OpCode.SWAP
    /// 0053 : OpCode.SYSCALL DF30B89A
    /// 0058 : OpCode.RET
    /// </remarks>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0303
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 0D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ISTYPE 28
    /// 0011 : OpCode.SWAP
    /// 0012 : OpCode.SIZE
    /// 0013 : OpCode.PUSHINT8 14
    /// 0015 : OpCode.NUMEQUAL
    /// 0016 : OpCode.BOOLAND
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIFNOT 22
    /// 001A : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 0039 : OpCode.THROW
    /// 003A : OpCode.PUSH3
    /// 003B : OpCode.PUSH1
    /// 003C : OpCode.NEWBUFFER
    /// 003D : OpCode.TUCK
    /// 003E : OpCode.PUSH0
    /// 003F : OpCode.ROT
    /// 0040 : OpCode.SETITEM
    /// 0041 : OpCode.SYSCALL 9BF667CE
    /// 0046 : OpCode.PUSH2
    /// 0047 : OpCode.PACK
    /// 0048 : OpCode.STLOC0
    /// 0049 : OpCode.LDARG1
    /// 004A : OpCode.LDLOC0
    /// 004B : OpCode.UNPACK
    /// 004C : OpCode.DROP
    /// 004D : OpCode.REVERSE3
    /// 004E : OpCode.CAT
    /// 004F : OpCode.SWAP
    /// 0050 : OpCode.SYSCALL 925DE831
    /// 0055 : OpCode.CALLT 0000
    /// 0058 : OpCode.STLOC1
    /// 0059 : OpCode.LDLOC1
    /// 005A : OpCode.PUSH0
    /// 005B : OpCode.PICKITEM
    /// 005C : OpCode.STLOC2
    /// 005D : OpCode.LDLOC2
    /// 005E : OpCode.SYSCALL F827EC8C
    /// 0063 : OpCode.NOT
    /// 0064 : OpCode.JMPIFNOT 04
    /// 0066 : OpCode.PUSHF
    /// 0067 : OpCode.RET
    /// 0068 : OpCode.LDLOC2
    /// 0069 : OpCode.LDARG0
    /// 006A : OpCode.NOTEQUAL
    /// 006B : OpCode.JMPIFNOT 25
    /// 006D : OpCode.LDARG0
    /// 006E : OpCode.DUP
    /// 006F : OpCode.LDLOC1
    /// 0070 : OpCode.PUSH0
    /// 0071 : OpCode.ROT
    /// 0072 : OpCode.SETITEM
    /// 0073 : OpCode.DROP
    /// 0074 : OpCode.LDLOC1
    /// 0075 : OpCode.CALLT 0100
    /// 0078 : OpCode.DUP
    /// 0079 : OpCode.LDARG1
    /// 007A : OpCode.LDLOC0
    /// 007B : OpCode.UNPACK
    /// 007C : OpCode.DROP
    /// 007D : OpCode.REVERSE3
    /// 007E : OpCode.CAT
    /// 007F : OpCode.SWAP
    /// 0080 : OpCode.SYSCALL E63F1884
    /// 0085 : OpCode.DROP
    /// 0086 : OpCode.PUSHM1
    /// 0087 : OpCode.LDARG1
    /// 0088 : OpCode.LDLOC2
    /// 0089 : OpCode.CALL 0F
    /// 008B : OpCode.PUSH1
    /// 008C : OpCode.LDARG1
    /// 008D : OpCode.LDARG0
    /// 008E : OpCode.CALL 0A
    /// 0090 : OpCode.LDARG2
    /// 0091 : OpCode.LDARG1
    /// 0092 : OpCode.LDARG0
    /// 0093 : OpCode.LDLOC2
    /// 0094 : OpCode.CALL 45
    /// 0096 : OpCode.PUSHT
    /// 0097 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion

}
