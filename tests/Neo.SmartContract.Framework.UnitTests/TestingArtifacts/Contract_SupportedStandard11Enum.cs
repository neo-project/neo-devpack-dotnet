using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard11Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard11Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":850,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":24,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":26,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":46,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":224,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":383,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":430,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":458,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":540,""safe"":false},{""name"":""testStandard"",""parameters"":[],""returntype"":""Boolean"",""offset"":833,""safe"":false},{""name"":""onNEP11Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":865,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":839,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPAAD9bgMQzkBXAAF4EAwHRVhBTVBMRdA0A0A0AkAQQAwBAEH2tGviQZJd6DFK2CYERRBAVwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBkUQIgTbIXFpeZ5xaRC1JgQJQGmxJBB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcDAXjKAEC3JjwMN1RoZSBhcmd1bWVudCAidG9rZW5JZCIgc2hvdWxkIGJlIDY0IG9yIGxlc3MgYnl0ZXMgbG9uZy46ExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxStgmNEUMLlRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByahDOQFcCARMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMTcAAHHISgwEbmFtZWkRztBAVwEAExGIThBR0EGb9mfOEsBwE2jBRUHfMLiaQFcBAXhK2SgkBkUJIgbKABSzJCQMH1RoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQ6FBGIThBR0EGb9mfOEsBwE3howUVTi1BB3zC4mkBXAwN4cGjYJgUIIhF4StkoJAZFCSIGygAUs6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ExGIThBR0EGb9mfOEsBweWjBRVOLUEGSXegxNwAAcWkQznJqQfgn7IwkBAlAaniYJiV4SmkQUdBFaTcBAEp5aMFFU4tQQeY/GIRFD3lqNA8ReXg0Cnp5eGo0RQhAVwIDeng1z/3//0VBm/ZnzhQRiE4QUdBQEsBweHmL2yhxehC3JhAQaWjBRVOLUEHmPxiEQGlowUVTi1BBL1jF7UBXAQR6EXl4FMAMCFRyYW5zZmVyQZUBb2F5cGjYJgUJIgp5NwIAcGjYqiYge3oReBTAHwwOb25ORVAxMVBheW1lbnR5QWJ9W1JFQAhAVwAFQFYCCjb+//8RwGBAWAsSwEo1rPz//yOk/P//WAsSwEo1nfz//yLYQIGG12o=").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 25 [2 datoshi]
    /// 13 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 35 : THROW [512 datoshi]
    /// 36 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 3B : PUSH1 [1 datoshi]
    /// 3C : PUSH1 [1 datoshi]
    /// 3D : NEWBUFFER [256 datoshi]
    /// 3E : TUCK [2 datoshi]
    /// 3F : PUSH0 [1 datoshi]
    /// 40 : ROT [2 datoshi]
    /// 41 : SETITEM [8192 datoshi]
    /// 42 : SWAP [2 datoshi]
    /// 43 : PUSH2 [1 datoshi]
    /// 44 : PACK [2048 datoshi]
    /// 45 : STLOC0 [2 datoshi]
    /// 46 : LDARG0 [2 datoshi]
    /// 47 : LDLOC0 [2 datoshi]
    /// 48 : UNPACK [2048 datoshi]
    /// 49 : DROP [2 datoshi]
    /// 4A : REVERSE3 [2 datoshi]
    /// 4B : CAT [2048 datoshi]
    /// 4C : SWAP [2 datoshi]
    /// 4D : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : ISNULL [2 datoshi]
    /// 54 : JMPIFNOT 05 [2 datoshi]
    /// 56 : DROP [2 datoshi]
    /// 57 : PUSH0 [1 datoshi]
    /// 58 : RET [0 datoshi]
    /// 59 : CONVERT 21 'Integer' [8192 datoshi]
    /// 5B : RET [0 datoshi]
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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 24 [2 datoshi]
    /// 13 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964 [8 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : PUSH4 [1 datoshi]
    /// 36 : PUSH1 [1 datoshi]
    /// 37 : NEWBUFFER [256 datoshi]
    /// 38 : TUCK [2 datoshi]
    /// 39 : PUSH0 [1 datoshi]
    /// 3A : ROT [2 datoshi]
    /// 3B : SETITEM [8192 datoshi]
    /// 3C : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 41 : PUSH2 [1 datoshi]
    /// 42 : PACK [2048 datoshi]
    /// 43 : STLOC0 [2 datoshi]
    /// 44 : PUSH3 [1 datoshi]
    /// 45 : LDARG0 [2 datoshi]
    /// 46 : LDLOC0 [2 datoshi]
    /// 47 : UNPACK [2048 datoshi]
    /// 48 : DROP [2 datoshi]
    /// 49 : REVERSE3 [2 datoshi]
    /// 4A : CAT [2048 datoshi]
    /// 4B : SWAP [2 datoshi]
    /// 4C : SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// 51 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAFQA==
    /// 00 : INITSLOT 0005 [64 datoshi]
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
    /// Script: VwMDeHBo2CYFCCIReErZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMJAQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQA==
    /// 00 : INITSLOT 0303 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : PUSHT [1 datoshi]
    /// 0A : JMP 11 [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : JMPIF 06 [2 datoshi]
    /// 12 : DROP [2 datoshi]
    /// 13 : PUSHF [1 datoshi]
    /// 14 : JMP 06 [2 datoshi]
    /// 16 : SIZE [4 datoshi]
    /// 17 : PUSHINT8 14 [1 datoshi]
    /// 19 : NUMEQUAL [8 datoshi]
    /// 1A : NOT [4 datoshi]
    /// 1B : JMPIFNOT 22 [2 datoshi]
    /// 1D : PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 3C : THROW [512 datoshi]
    /// 3D : PUSH3 [1 datoshi]
    /// 3E : PUSH1 [1 datoshi]
    /// 3F : NEWBUFFER [256 datoshi]
    /// 40 : TUCK [2 datoshi]
    /// 41 : PUSH0 [1 datoshi]
    /// 42 : ROT [2 datoshi]
    /// 43 : SETITEM [8192 datoshi]
    /// 44 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 49 : PUSH2 [1 datoshi]
    /// 4A : PACK [2048 datoshi]
    /// 4B : STLOC0 [2 datoshi]
    /// 4C : LDARG1 [2 datoshi]
    /// 4D : LDLOC0 [2 datoshi]
    /// 4E : UNPACK [2048 datoshi]
    /// 4F : DROP [2 datoshi]
    /// 50 : REVERSE3 [2 datoshi]
    /// 51 : CAT [2048 datoshi]
    /// 52 : SWAP [2 datoshi]
    /// 53 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 58 : CALLT 0000 [32768 datoshi]
    /// 5B : STLOC1 [2 datoshi]
    /// 5C : LDLOC1 [2 datoshi]
    /// 5D : PUSH0 [1 datoshi]
    /// 5E : PICKITEM [64 datoshi]
    /// 5F : STLOC2 [2 datoshi]
    /// 60 : LDLOC2 [2 datoshi]
    /// 61 : SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// 66 : JMPIF 04 [2 datoshi]
    /// 68 : PUSHF [1 datoshi]
    /// 69 : RET [0 datoshi]
    /// 6A : LDLOC2 [2 datoshi]
    /// 6B : LDARG0 [2 datoshi]
    /// 6C : NOTEQUAL [32 datoshi]
    /// 6D : JMPIFNOT 25 [2 datoshi]
    /// 6F : LDARG0 [2 datoshi]
    /// 70 : DUP [2 datoshi]
    /// 71 : LDLOC1 [2 datoshi]
    /// 72 : PUSH0 [1 datoshi]
    /// 73 : ROT [2 datoshi]
    /// 74 : SETITEM [8192 datoshi]
    /// 75 : DROP [2 datoshi]
    /// 76 : LDLOC1 [2 datoshi]
    /// 77 : CALLT 0100 [32768 datoshi]
    /// 7A : DUP [2 datoshi]
    /// 7B : LDARG1 [2 datoshi]
    /// 7C : LDLOC0 [2 datoshi]
    /// 7D : UNPACK [2048 datoshi]
    /// 7E : DROP [2 datoshi]
    /// 7F : REVERSE3 [2 datoshi]
    /// 80 : CAT [2048 datoshi]
    /// 81 : SWAP [2 datoshi]
    /// 82 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 87 : DROP [2 datoshi]
    /// 88 : PUSHM1 [1 datoshi]
    /// 89 : LDARG1 [2 datoshi]
    /// 8A : LDLOC2 [2 datoshi]
    /// 8B : CALL 0F [512 datoshi]
    /// 8D : PUSH1 [1 datoshi]
    /// 8E : LDARG1 [2 datoshi]
    /// 8F : LDARG0 [2 datoshi]
    /// 90 : CALL 0A [512 datoshi]
    /// 92 : LDARG2 [2 datoshi]
    /// 93 : LDARG1 [2 datoshi]
    /// 94 : LDARG0 [2 datoshi]
    /// 95 : LDLOC2 [2 datoshi]
    /// 96 : CALL 45 [512 datoshi]
    /// 98 : PUSHT [1 datoshi]
    /// 99 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
