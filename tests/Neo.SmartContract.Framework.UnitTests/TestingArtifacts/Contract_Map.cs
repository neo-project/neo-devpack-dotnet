using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Map(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Map"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCount"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":61,""safe"":false},{""name"":""testClear"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":91,""safe"":false},{""name"":""testByteArray2"",""parameters"":[],""returntype"":""String"",""offset"":123,""safe"":false},{""name"":""testUnicode"",""parameters"":[{""name"":""key"",""type"":""String""}],""returntype"":""String"",""offset"":152,""safe"":false},{""name"":""testUnicodeValue"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":187,""safe"":false},{""name"":""testUnicodeKeyValue"",""parameters"":[{""name"":""key"",""type"":""String""},{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":206,""safe"":false},{""name"":""testInt"",""parameters"":[{""name"":""key"",""type"":""Integer""}],""returntype"":""String"",""offset"":222,""safe"":false},{""name"":""testBool"",""parameters"":[{""name"":""key"",""type"":""Boolean""}],""returntype"":""String"",""offset"":245,""safe"":false},{""name"":""testDeserialize"",""parameters"":[{""name"":""key"",""type"":""String""}],""returntype"":""Any"",""offset"":270,""safe"":false},{""name"":""testuint160Key"",""parameters"":[],""returntype"":""Any"",""offset"":307,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""jsonDeserialize"",""jsonSerialize""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABBcDvOc7g5OklxsKgannhRA3Yb86sD2pzb25EZXNlcmlhbGl6ZQEAAQUAAP1fAVcCAchwEHEiLmhpaVNT0GlKnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JNFoykBXAQHIcGh42ygMC3Rlc3RzdHJpbmcyU1PQaDcAAEBXAQHIcGh42ygMC3Rlc3RzdHJpbmcyU1PQaNNoNwAAQFcCAMhwDAIBARKN2yhxaGkMADcAAFNT0Gg3AABAVwEByHBoeAwSMTI5ODQwdGVzdDEwMDIyOTM5U1PQaDcAAEBXAQHIcGgMAmFieFNT0Gg3AABAVwECyHBoeHlTU9BoNwAAQFcBAchwaHgMBnN0cmluZ1NT0Gg3AABAVwEByHBoeAwIdGVzdGJvb2xTU9BoNwAAQFcCAchwaHgMD3Rlc3RkZXNlcmlhbGl6ZVNT0Gg3AABxaTcBAEBXAwDIcAwUAAAAAAAAAAAAAAAAAAAAAAAAAABxaGkRU1PQaDcAAHJqNwEAQM1/vjU=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHBoeAwIdGVzdGJvb2xTU9BoNwAAQA==
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 74657374626F6F6C 'testbool' [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBool")]
    public abstract string? TestBool(bool? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHBoeNsoDAt0ZXN0c3RyaW5nMlNT0Gg3AABA
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 74657374737472696E6732 'teststring2' [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArray")]
    public abstract object? TestByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAyHAMAgEBEo3bKHFoaQwANwAAU1PQaDcAAEA=
    /// INITSLOT 0200 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 0101 [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArray2")]
    public abstract string? TestByteArray2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHBoeNsoDAt0ZXN0c3RyaW5nMlNT0GjTaDcAAEA=
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 74657374737472696E6732 'teststring2' [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CLEARITEMS [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testClear")]
    public abstract object? TestClear(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAQcSIuaGlpU1PQaUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUk0WjKQA==
    /// INITSLOT 0201 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 2E [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LT [8 datoshi]
    /// JMPIF D1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHBoeAwPdGVzdGRlc2VyaWFsaXplU1PQaDcAAHFpNwEAQA==
    /// INITSLOT 0201 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 74657374646573657269616C697A65 'testdeserialize' [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeserialize")]
    public abstract object? TestDeserialize(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHBoeAwGc3RyaW5nU1PQaDcAAEA=
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInt")]
    public abstract string? TestInt(BigInteger? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAyHAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAcWhpEVNT0Gg3AAByajcBAEA=
    /// INITSLOT 0300 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testuint160Key")]
    public abstract object? Testuint160Key();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHBoeAwSMTI5ODQwdGVzdDEwMDIyOTM5U1PQaDcAAEA=
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 313239383430746573743130303232393339 '129840test10022939' [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicode")]
    public abstract string? TestUnicode(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECyHBoeHlTU9BoNwAAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeKeyValue")]
    public abstract string? TestUnicodeKeyValue(string? key, string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHBoDAJhYnhTU9BoNwAAQA==
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 6162 'ab' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);

    #endregion
}
