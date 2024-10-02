using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":914,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":931,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":45,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":71,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":254,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":946,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":455,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":483,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":572,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":873,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":961,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":879,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHIGc2hhMjU2AQABDwAA/dADEM5AVwABeBAMB0VYQU1QTEXQeDQDQFcAAXg0A0BXAAF4NANAVwABQFcAARBAWtgmFwwBAEH2tGviQZJd6DFK2CYERRBKYkBXAQF4cGgLlyYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5xaRC1JgQJQGkQsyYQeGjBRVOLUEEvWMXtIg9peGjBRVOLUEHmPxiECEBXAwF4ygBAtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDC5UaGUgdG9rZW4gd2l0aCBnaXZlbiAidG9rZW5JZCIgZG9lcyBub3QgZXhpc3QuOnFpNwAAcmoQzkBXAgITEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxyGkRzktT0EBXAQATEYhOEFHQQZv2Z84SwHATaMFFQd8wuJpAVwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQ6FBGIThBR0EGb9mfOEsBwE3howUVTi1BB3zC4mkBXAwN4cGgLlyYFCCINeErZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcWkQznJqQfgn7IyqJgQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQFcCA3p4NdD9//9FQZv2Z84UEYhOEFHQUBLAcHh5i9socXoQtyYQEGlowUVTi1BB5j8YhEBpaMFFU4tQQS9Yxe1AVwEEwkp4z0p5z0oRz0p6zwwIVHJhbnNmZXJBlQFvYXlwaAuXqiQFCSILeTcCAHBoC5eqJiB7ehF4FMAfDA5vbk5FUDExUGF5bWVudHlBYn1bUkVACEBXAAVAVgMKLP7//wqy/P//CoX8//8TwGAKGv7//wqg/P//CxPAYUALEcBKWM9KNWr8//8jYvz//8JKWc9KNW78//8je/z//8JKWc9KNV/8//8j4f3//wsRwEpYz0o1O/z//yKeQBU2L8A="));

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
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDARG0
    // 000E : DUP
    // 000F : ISTYPE
    // 0011 : SWAP
    // 0012 : SIZE
    // 0013 : PUSHINT8
    // 0015 : NUMEQUAL
    // 0016 : BOOLAND
    // 0017 : NOT
    // 0018 : JMPIFNOT
    // 001A : PUSHDATA1
    // 003C : THROW
    // 003D : SYSCALL
    // 0042 : PUSH1
    // 0043 : PUSH1
    // 0044 : NEWBUFFER
    // 0045 : TUCK
    // 0046 : PUSH0
    // 0047 : ROT
    // 0048 : SETITEM
    // 0049 : SWAP
    // 004A : PUSH2
    // 004B : PACK
    // 004C : STLOC0
    // 004D : LDARG0
    // 004E : LDLOC0
    // 004F : UNPACK
    // 0050 : DROP
    // 0051 : REVERSE3
    // 0052 : CAT
    // 0053 : SWAP
    // 0054 : SYSCALL
    // 0059 : DUP
    // 005A : ISNULL
    // 005B : JMPIFNOT
    // 005D : DROP
    // 005E : PUSH0
    // 005F : CONVERT
    // 0061 : RET

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SIZE
    // 0005 : PUSHINT8
    // 0007 : GT
    // 0008 : JMPIFNOT
    // 000A : PUSHDATA1
    // 0043 : THROW
    // 0044 : PUSH3
    // 0045 : PUSH1
    // 0046 : NEWBUFFER
    // 0047 : TUCK
    // 0048 : PUSH0
    // 0049 : ROT
    // 004A : SETITEM
    // 004B : SYSCALL
    // 0050 : PUSH2
    // 0051 : PACK
    // 0052 : STLOC0
    // 0053 : LDARG0
    // 0054 : LDLOC0
    // 0055 : UNPACK
    // 0056 : DROP
    // 0057 : REVERSE3
    // 0058 : CAT
    // 0059 : SWAP
    // 005A : SYSCALL
    // 005F : DUP
    // 0060 : ISNULL
    // 0061 : JMPIFNOT
    // 0063 : DROP
    // 0064 : PUSHDATA1
    // 0094 : THROW
    // 0095 : STLOC1
    // 0096 : LDLOC1
    // 0097 : CALLT
    // 009A : STLOC2
    // 009B : LDLOC2
    // 009C : PUSH0
    // 009D : PICKITEM
    // 009E : RET

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);
    // 0000 : INITSLOT
    // 0003 : PUSH3
    // 0004 : PUSH1
    // 0005 : NEWBUFFER
    // 0006 : TUCK
    // 0007 : PUSH0
    // 0008 : ROT
    // 0009 : SETITEM
    // 000A : SYSCALL
    // 000F : PUSH2
    // 0010 : PACK
    // 0011 : STLOC0
    // 0012 : LDARG1
    // 0013 : LDLOC0
    // 0014 : UNPACK
    // 0015 : DROP
    // 0016 : REVERSE3
    // 0017 : CAT
    // 0018 : SWAP
    // 0019 : SYSCALL
    // 001E : CALLT
    // 0021 : STLOC1
    // 0022 : NEWMAP
    // 0023 : LDLOC1
    // 0024 : PUSH1
    // 0025 : PICKITEM
    // 0026 : OVER
    // 0027 : REVERSE3
    // 0028 : SETITEM
    // 0029 : RET

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDARG0
    // 000E : DUP
    // 000F : ISTYPE
    // 0011 : SWAP
    // 0012 : SIZE
    // 0013 : PUSHINT8
    // 0015 : NUMEQUAL
    // 0016 : BOOLAND
    // 0017 : NOT
    // 0018 : JMPIFNOT
    // 001A : PUSHDATA1
    // 003B : THROW
    // 003C : PUSH4
    // 003D : PUSH1
    // 003E : NEWBUFFER
    // 003F : TUCK
    // 0040 : PUSH0
    // 0041 : ROT
    // 0042 : SETITEM
    // 0043 : SYSCALL
    // 0048 : PUSH2
    // 0049 : PACK
    // 004A : STLOC0
    // 004B : PUSH3
    // 004C : LDARG0
    // 004D : LDLOC0
    // 004E : UNPACK
    // 004F : DROP
    // 0050 : REVERSE3
    // 0051 : CAT
    // 0052 : SWAP
    // 0053 : SYSCALL
    // 0058 : RET

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP11Payment")]
    public abstract void OnNEP11Payment(UInt160? from, BigInteger? amount, string? tokenId, object? data = null);
    // 0000 : INITSLOT
    // 0003 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStandard")]
    public abstract bool? TestStandard();
    // 0000 : PUSHT
    // 0001 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDARG0
    // 000E : DUP
    // 000F : ISTYPE
    // 0011 : SWAP
    // 0012 : SIZE
    // 0013 : PUSHINT8
    // 0015 : NUMEQUAL
    // 0016 : BOOLAND
    // 0017 : NOT
    // 0018 : JMPIFNOT
    // 001A : PUSHDATA1
    // 0039 : THROW
    // 003A : PUSH3
    // 003B : PUSH1
    // 003C : NEWBUFFER
    // 003D : TUCK
    // 003E : PUSH0
    // 003F : ROT
    // 0040 : SETITEM
    // 0041 : SYSCALL
    // 0046 : PUSH2
    // 0047 : PACK
    // 0048 : STLOC0
    // 0049 : LDARG1
    // 004A : LDLOC0
    // 004B : UNPACK
    // 004C : DROP
    // 004D : REVERSE3
    // 004E : CAT
    // 004F : SWAP
    // 0050 : SYSCALL
    // 0055 : CALLT
    // 0058 : STLOC1
    // 0059 : LDLOC1
    // 005A : PUSH0
    // 005B : PICKITEM
    // 005C : STLOC2
    // 005D : LDLOC2
    // 005E : SYSCALL
    // 0063 : NOT
    // 0064 : JMPIFNOT
    // 0066 : PUSHF
    // 0067 : RET
    // 0068 : LDLOC2
    // 0069 : LDARG0
    // 006A : NOTEQUAL
    // 006B : JMPIFNOT
    // 006D : LDARG0
    // 006E : DUP
    // 006F : LDLOC1
    // 0070 : PUSH0
    // 0071 : ROT
    // 0072 : SETITEM
    // 0073 : DROP
    // 0074 : LDLOC1
    // 0075 : CALLT
    // 0078 : DUP
    // 0079 : LDARG1
    // 007A : LDLOC0
    // 007B : UNPACK
    // 007C : DROP
    // 007D : REVERSE3
    // 007E : CAT
    // 007F : SWAP
    // 0080 : SYSCALL
    // 0085 : DROP
    // 0086 : PUSHM1
    // 0087 : LDARG1
    // 0088 : LDLOC2
    // 0089 : CALL
    // 008B : PUSH1
    // 008C : LDARG1
    // 008D : LDARG0
    // 008E : CALL
    // 0090 : LDARG2
    // 0091 : LDARG1
    // 0092 : LDARG0
    // 0093 : LDLOC2
    // 0094 : CALL
    // 0096 : PUSHT
    // 0097 : RET

    #endregion

}
