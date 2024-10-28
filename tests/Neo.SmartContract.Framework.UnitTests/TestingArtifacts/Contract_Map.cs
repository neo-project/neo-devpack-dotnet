using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Map(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Map"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCount"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":29,""safe"":false},{""name"":""testClear"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":60,""safe"":false},{""name"":""testByteArray2"",""parameters"":[],""returntype"":""String"",""offset"":93,""safe"":false},{""name"":""testUnicode"",""parameters"":[{""name"":""key"",""type"":""String""}],""returntype"":""String"",""offset"":123,""safe"":false},{""name"":""testUnicodeValue"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":159,""safe"":false},{""name"":""testUnicodeKeyValue"",""parameters"":[{""name"":""key"",""type"":""String""},{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":179,""safe"":false},{""name"":""testInt"",""parameters"":[{""name"":""key"",""type"":""Integer""}],""returntype"":""String"",""offset"":196,""safe"":false},{""name"":""testBool"",""parameters"":[{""name"":""key"",""type"":""Boolean""}],""returntype"":""String"",""offset"":220,""safe"":false},{""name"":""testDeserialize"",""parameters"":[{""name"":""key"",""type"":""String""}],""returntype"":""Any"",""offset"":246,""safe"":false},{""name"":""testuint160Key"",""parameters"":[],""returntype"":""Any"",""offset"":284,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""jsonDeserialize"",""jsonSerialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABD8DvOc7g5OklxsKgannhRA3Yb86sD2pzb25EZXNlcmlhbGl6ZQEAAQ8AAP1JAVcCAchwEHEiDmlKaWhT0EVpSpxxRWl4tSTxaMpAVwEByHAMC3Rlc3RzdHJpbmcySnjbKGhT0EVoNwAAQFcBAchwDAt0ZXN0c3RyaW5nMkp42yhoU9BFaNNoNwAAQFcCAMhwDAIBAdsw2yhxDAA3AABKaWhT0EVoNwAAQFcBAchwDBIxMjk4NDB0ZXN0MTAwMjI5MzlKeGhT0EVoNwAAQFcBAchweEoMAmFiaFPQRWg3AABAVwECyHB5SnhoU9BFaDcAAEBXAQHIcAwGc3RyaW5nSnhoU9BFaDcAAEBXAQHIcAwIdGVzdGJvb2xKeGhT0EVoNwAAQFcCAchwDA90ZXN0ZGVzZXJpYWxpemVKeGhT0EVoNwAAcWk3AQBAVwMAyHAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAcRFKaWhT0EVoNwAAcmo3AQBAEVDNyA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdGJvb2xKeGhT0EVoNwAAQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 74657374626F6F6C [8 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.LDARG0 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.REVERSE3 [2 datoshi]
    /// 13 : OpCode.SETITEM [8192 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.LDLOC0 [2 datoshi]
    /// 16 : OpCode.CALLT 0000 [32768 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBool")]
    public abstract string? TestBool(bool? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdHN0cmluZzJKeNsoaFPQRWg3AABA
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 74657374737472696E6732 [8 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.LDARG0 [2 datoshi]
    /// 14 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.REVERSE3 [2 datoshi]
    /// 18 : OpCode.SETITEM [8192 datoshi]
    /// 19 : OpCode.DROP [2 datoshi]
    /// 1A : OpCode.LDLOC0 [2 datoshi]
    /// 1B : OpCode.CALLT 0000 [32768 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArray")]
    public abstract object? TestByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAyHAMAQHbMNsocQw3AABKaWhT0EVoNwAAQA==
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 0101 [8 datoshi]
    /// 09 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0D : OpCode.STLOC1 [2 datoshi]
    /// 0E : OpCode.PUSHDATA1 [8 datoshi]
    /// 10 : OpCode.CALLT 0000 [32768 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.LDLOC1 [2 datoshi]
    /// 15 : OpCode.LDLOC0 [2 datoshi]
    /// 16 : OpCode.REVERSE3 [2 datoshi]
    /// 17 : OpCode.SETITEM [8192 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.LDLOC0 [2 datoshi]
    /// 1A : OpCode.CALLT 0000 [32768 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArray2")]
    public abstract string? TestByteArray2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdHN0cmluZzJKeNsoaFPQRWjTaDcAAEA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 74657374737472696E6732 [8 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.LDARG0 [2 datoshi]
    /// 14 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.REVERSE3 [2 datoshi]
    /// 18 : OpCode.SETITEM [8192 datoshi]
    /// 19 : OpCode.DROP [2 datoshi]
    /// 1A : OpCode.LDLOC0 [2 datoshi]
    /// 1B : OpCode.CLEARITEMS [16 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.CALLT 0000 [32768 datoshi]
    /// 20 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testClear")]
    public abstract object? TestClear(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAQcSIOaUppaFPQRWlKnHFFaXi1JPFoykA=
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 0E [2 datoshi]
    /// 09 : OpCode.LDLOC1 [2 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.LDLOC1 [2 datoshi]
    /// 0C : OpCode.LDLOC0 [2 datoshi]
    /// 0D : OpCode.REVERSE3 [2 datoshi]
    /// 0E : OpCode.SETITEM [8192 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.LDLOC1 [2 datoshi]
    /// 11 : OpCode.DUP [2 datoshi]
    /// 12 : OpCode.INC [4 datoshi]
    /// 13 : OpCode.STLOC1 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.LDLOC1 [2 datoshi]
    /// 16 : OpCode.LDARG0 [2 datoshi]
    /// 17 : OpCode.LT [8 datoshi]
    /// 18 : OpCode.JMPIF F1 [2 datoshi]
    /// 1A : OpCode.LDLOC0 [2 datoshi]
    /// 1B : OpCode.SIZE [4 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAMdGVzdGRlc2VyaWFsaXplSnhoU9BFaDcAAHFpNwEAQA==
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 74657374646573657269616C697A65 [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.LDARG0 [2 datoshi]
    /// 18 : OpCode.LDLOC0 [2 datoshi]
    /// 19 : OpCode.REVERSE3 [2 datoshi]
    /// 1A : OpCode.SETITEM [8192 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.CALLT 0000 [32768 datoshi]
    /// 20 : OpCode.STLOC1 [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.CALLT 0100 [32768 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeserialize")]
    public abstract object? TestDeserialize(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMc3RyaW5nSnhoU9BFaDcAAEA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 737472696E67 [8 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.LDARG0 [2 datoshi]
    /// 0F : OpCode.LDLOC0 [2 datoshi]
    /// 10 : OpCode.REVERSE3 [2 datoshi]
    /// 11 : OpCode.SETITEM [8192 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.CALLT 0000 [32768 datoshi]
    /// 17 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInt")]
    public abstract string? TestInt(BigInteger? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testuint160Key")]
    public abstract object? Testuint160Key();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMMTI5ODQwdGVzdDEwMDIyOTM5SnhoU9BFaDcAAEA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 313239383430746573743130303232393339 [8 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.LDARG0 [2 datoshi]
    /// 1B : OpCode.LDLOC0 [2 datoshi]
    /// 1C : OpCode.REVERSE3 [2 datoshi]
    /// 1D : OpCode.SETITEM [8192 datoshi]
    /// 1E : OpCode.DROP [2 datoshi]
    /// 1F : OpCode.LDLOC0 [2 datoshi]
    /// 20 : OpCode.CALLT 0000 [32768 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicode")]
    public abstract string? TestUnicode(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECyHB5SnhoU9BFaDcAAEA=
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG1 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.LDARG0 [2 datoshi]
    /// 08 : OpCode.LDLOC0 [2 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.SETITEM [8192 datoshi]
    /// 0B : OpCode.DROP [2 datoshi]
    /// 0C : OpCode.LDLOC0 [2 datoshi]
    /// 0D : OpCode.CALLT 0000 [32768 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeKeyValue")]
    public abstract string? TestUnicodeKeyValue(string? key, string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHB4SgxhYmhT0EVoNwAAQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.NEWMAP [8 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.PUSHDATA1 6162 [8 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.REVERSE3 [2 datoshi]
    /// 0D : OpCode.SETITEM [8192 datoshi]
    /// 0E : OpCode.DROP [2 datoshi]
    /// 0F : OpCode.LDLOC0 [2 datoshi]
    /// 10 : OpCode.CALLT 0000 [32768 datoshi]
    /// 13 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);

    #endregion
}
