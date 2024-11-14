using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":31,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":212,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":371,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":418,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":446,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":534,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":823,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":825,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":829,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPAAD9QANAEEBYStgmGEUMAQBB9rRr4kGSXegxStgmBEUQSmBAVwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5xaRC1JgQJQGmxJBB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcDAXjKAEC3JjwMN1RoZSBhcmd1bWVudCAidG9rZW5JZCIgc2hvdWxkIGJlIDY0IG9yIGxlc3MgYnl0ZXMgbG9uZy46ExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxStgmNEUMLlRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByahDOQFcCARMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMTcAAHHISgwEbmFtZWkRztBAVwEAExGIThBR0EGb9mfOEsBwE2jBRUHfMLiaQFcBAXhwaNgmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQ6FBGIThBR0EGb9mfOEsBwE3howUVTi1BB3zC4mkBXAwN4cGjYJgUIIg14StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjoTEYhOEFHQQZv2Z84SwHB5aMFFU4tQQZJd6DE3AABxaRDOcmpB+CfsjCQECUBqeJgmJXhKaRBR0EVpNwEASnlowUVTi1BB5j8YhEUPeWo0DxF5eDQKenl4ajRFCEBXAgN6eDXP/f//RUGb9mfOFBGIThBR0FASwHB4eYvbKHF6ELcmEBBpaMFFU4tQQeY/GIRAaWjBRVOLUEEvWMXtQFcBBHoReXgUwAwIVHJhbnNmZXJBlQFvYXlwaNgmBQkiCnk3AgBwaNiqJiB7ehF4FMAfDA5vbk5FUDExUGF5bWVudHlBYn1bUkVACEBXAARAVgFAGpVHeA=="));

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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : PUSHT [1 datoshi]
    /// 0A : JMP 0D [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : SIZE [4 datoshi]
    /// 12 : PUSHINT8 14 [1 datoshi]
    /// 14 : NUMEQUAL [8 datoshi]
    /// 15 : BOOLAND [8 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIFNOT 25 [2 datoshi]
    /// 19 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 3B : THROW [512 datoshi]
    /// 3C : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 41 : PUSH1 [1 datoshi]
    /// 42 : PUSH1 [1 datoshi]
    /// 43 : NEWBUFFER [256 datoshi]
    /// 44 : TUCK [2 datoshi]
    /// 45 : PUSH0 [1 datoshi]
    /// 46 : ROT [2 datoshi]
    /// 47 : SETITEM [8192 datoshi]
    /// 48 : SWAP [2 datoshi]
    /// 49 : PUSH2 [1 datoshi]
    /// 4A : PACK [2048 datoshi]
    /// 4B : STLOC0 [2 datoshi]
    /// 4C : LDARG0 [2 datoshi]
    /// 4D : LDLOC0 [2 datoshi]
    /// 4E : UNPACK [2048 datoshi]
    /// 4F : DROP [2 datoshi]
    /// 50 : REVERSE3 [2 datoshi]
    /// 51 : CAT [2048 datoshi]
    /// 52 : SWAP [2 datoshi]
    /// 53 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 58 : DUP [2 datoshi]
    /// 59 : ISNULL [2 datoshi]
    /// 5A : JMPIFNOT 04 [2 datoshi]
    /// 5C : DROP [2 datoshi]
    /// 5D : PUSH0 [1 datoshi]
    /// 5E : CONVERT 21 'Integer' [8192 datoshi]
    /// 60 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMoAQLcmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5A
    /// 00 : INITSLOT 0301 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : PUSHINT8 40 [1 datoshi]
    /// 07 : GT [8 datoshi]
    /// 08 : JMPIFNOT 3C [2 datoshi]
    /// 0A : PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203634206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// 43 : THROW [512 datoshi]
    /// 44 : PUSH3 [1 datoshi]
    /// 45 : PUSH1 [1 datoshi]
    /// 46 : NEWBUFFER [256 datoshi]
    /// 47 : TUCK [2 datoshi]
    /// 48 : PUSH0 [1 datoshi]
    /// 49 : ROT [2 datoshi]
    /// 4A : SETITEM [8192 datoshi]
    /// 4B : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 50 : PUSH2 [1 datoshi]
    /// 51 : PACK [2048 datoshi]
    /// 52 : STLOC0 [2 datoshi]
    /// 53 : LDARG0 [2 datoshi]
    /// 54 : LDLOC0 [2 datoshi]
    /// 55 : UNPACK [2048 datoshi]
    /// 56 : DROP [2 datoshi]
    /// 57 : REVERSE3 [2 datoshi]
    /// 58 : CAT [2048 datoshi]
    /// 59 : SWAP [2 datoshi]
    /// 5A : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 5F : DUP [2 datoshi]
    /// 60 : ISNULL [2 datoshi]
    /// 61 : JMPIFNOT 34 [2 datoshi]
    /// 63 : DROP [2 datoshi]
    /// 64 : PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E [8 datoshi]
    /// 94 : THROW [512 datoshi]
    /// 95 : STLOC1 [2 datoshi]
    /// 96 : LDLOC1 [2 datoshi]
    /// 97 : CALLT 0000 [32768 datoshi]
    /// 9A : STLOC2 [2 datoshi]
    /// 9B : LDLOC2 [2 datoshi]
    /// 9C : PUSH0 [1 datoshi]
    /// 9D : PICKITEM [64 datoshi]
    /// 9E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDARuYW1laRHO0EA=
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : NEWBUFFER [256 datoshi]
    /// 06 : TUCK [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : ROT [2 datoshi]
    /// 09 : SETITEM [8192 datoshi]
    /// 0A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : PUSH2 [1 datoshi]
    /// 10 : PACK [2048 datoshi]
    /// 11 : STLOC0 [2 datoshi]
    /// 12 : LDARG0 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : UNPACK [2048 datoshi]
    /// 15 : DROP [2 datoshi]
    /// 16 : REVERSE3 [2 datoshi]
    /// 17 : CAT [2048 datoshi]
    /// 18 : SWAP [2 datoshi]
    /// 19 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 1E : CALLT 0000 [32768 datoshi]
    /// 21 : STLOC1 [2 datoshi]
    /// 22 : NEWMAP [8 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : PUSHDATA1 6E616D65 'name' [8 datoshi]
    /// 2A : LDLOC1 [2 datoshi]
    /// 2B : PUSH1 [1 datoshi]
    /// 2C : PICKITEM [64 datoshi]
    /// 2D : SETITEM [8192 datoshi]
    /// 2E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : PUSHT [1 datoshi]
    /// 0A : JMP 0D [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : SIZE [4 datoshi]
    /// 12 : PUSHINT8 14 [1 datoshi]
    /// 14 : NUMEQUAL [8 datoshi]
    /// 15 : BOOLAND [8 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIFNOT 24 [2 datoshi]
    /// 19 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964 [8 datoshi]
    /// 3A : THROW [512 datoshi]
    /// 3B : PUSH4 [1 datoshi]
    /// 3C : PUSH1 [1 datoshi]
    /// 3D : NEWBUFFER [256 datoshi]
    /// 3E : TUCK [2 datoshi]
    /// 3F : PUSH0 [1 datoshi]
    /// 40 : ROT [2 datoshi]
    /// 41 : SETITEM [8192 datoshi]
    /// 42 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 47 : PUSH2 [1 datoshi]
    /// 48 : PACK [2048 datoshi]
    /// 49 : STLOC0 [2 datoshi]
    /// 4A : PUSH3 [1 datoshi]
    /// 4B : LDARG0 [2 datoshi]
    /// 4C : LDLOC0 [2 datoshi]
    /// 4D : UNPACK [2048 datoshi]
    /// 4E : DROP [2 datoshi]
    /// 4F : REVERSE3 [2 datoshi]
    /// 50 : CAT [2048 datoshi]
    /// 51 : SWAP [2 datoshi]
    /// 52 : SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// 57 : RET [0 datoshi]
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
    /// 00 : INITSLOT 0004 [64 datoshi]
    /// 03 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("onNEP11Payment")]
    public abstract void OnNEP11Payment(UInt160? from, BigInteger? amount, string? tokenId, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CEA=
    /// 00 : PUSHT [1 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStandard")]
    public abstract bool? TestStandard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDeHBo2CYFCCINeErZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcWkQznJqQfgn7IwkBAlAaniYJiV4SmkQUdBFaTcBAEp5aMFFU4tQQeY/GIRFD3lqNA8ReXg0Cnp5eGo0RQhA
    /// 00 : INITSLOT 0303 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : PUSHT [1 datoshi]
    /// 0A : JMP 0D [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : SIZE [4 datoshi]
    /// 12 : PUSHINT8 14 [1 datoshi]
    /// 14 : NUMEQUAL [8 datoshi]
    /// 15 : BOOLAND [8 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIFNOT 22 [2 datoshi]
    /// 19 : PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 38 : THROW [512 datoshi]
    /// 39 : PUSH3 [1 datoshi]
    /// 3A : PUSH1 [1 datoshi]
    /// 3B : NEWBUFFER [256 datoshi]
    /// 3C : TUCK [2 datoshi]
    /// 3D : PUSH0 [1 datoshi]
    /// 3E : ROT [2 datoshi]
    /// 3F : SETITEM [8192 datoshi]
    /// 40 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 45 : PUSH2 [1 datoshi]
    /// 46 : PACK [2048 datoshi]
    /// 47 : STLOC0 [2 datoshi]
    /// 48 : LDARG1 [2 datoshi]
    /// 49 : LDLOC0 [2 datoshi]
    /// 4A : UNPACK [2048 datoshi]
    /// 4B : DROP [2 datoshi]
    /// 4C : REVERSE3 [2 datoshi]
    /// 4D : CAT [2048 datoshi]
    /// 4E : SWAP [2 datoshi]
    /// 4F : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 54 : CALLT 0000 [32768 datoshi]
    /// 57 : STLOC1 [2 datoshi]
    /// 58 : LDLOC1 [2 datoshi]
    /// 59 : PUSH0 [1 datoshi]
    /// 5A : PICKITEM [64 datoshi]
    /// 5B : STLOC2 [2 datoshi]
    /// 5C : LDLOC2 [2 datoshi]
    /// 5D : SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// 62 : JMPIF 04 [2 datoshi]
    /// 64 : PUSHF [1 datoshi]
    /// 65 : RET [0 datoshi]
    /// 66 : LDLOC2 [2 datoshi]
    /// 67 : LDARG0 [2 datoshi]
    /// 68 : NOTEQUAL [32 datoshi]
    /// 69 : JMPIFNOT 25 [2 datoshi]
    /// 6B : LDARG0 [2 datoshi]
    /// 6C : DUP [2 datoshi]
    /// 6D : LDLOC1 [2 datoshi]
    /// 6E : PUSH0 [1 datoshi]
    /// 6F : ROT [2 datoshi]
    /// 70 : SETITEM [8192 datoshi]
    /// 71 : DROP [2 datoshi]
    /// 72 : LDLOC1 [2 datoshi]
    /// 73 : CALLT 0100 [32768 datoshi]
    /// 76 : DUP [2 datoshi]
    /// 77 : LDARG1 [2 datoshi]
    /// 78 : LDLOC0 [2 datoshi]
    /// 79 : UNPACK [2048 datoshi]
    /// 7A : DROP [2 datoshi]
    /// 7B : REVERSE3 [2 datoshi]
    /// 7C : CAT [2048 datoshi]
    /// 7D : SWAP [2 datoshi]
    /// 7E : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 83 : DROP [2 datoshi]
    /// 84 : PUSHM1 [1 datoshi]
    /// 85 : LDARG1 [2 datoshi]
    /// 86 : LDLOC2 [2 datoshi]
    /// 87 : CALL 0F [512 datoshi]
    /// 89 : PUSH1 [1 datoshi]
    /// 8A : LDARG1 [2 datoshi]
    /// 8B : LDARG0 [2 datoshi]
    /// 8C : CALL 0A [512 datoshi]
    /// 8E : LDARG2 [2 datoshi]
    /// 8F : LDARG1 [2 datoshi]
    /// 90 : LDARG0 [2 datoshi]
    /// 91 : LDLOC2 [2 datoshi]
    /// 92 : CALL 45 [512 datoshi]
    /// 94 : PUSHT [1 datoshi]
    /// 95 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
