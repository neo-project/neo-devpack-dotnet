using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Map(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Map"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCount"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":75,""safe"":false},{""name"":""testClear"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":106,""safe"":false},{""name"":""testByteArray2"",""parameters"":[],""returntype"":""String"",""offset"":139,""safe"":false},{""name"":""testUnicode"",""parameters"":[{""name"":""key"",""type"":""String""}],""returntype"":""String"",""offset"":169,""safe"":false},{""name"":""testUnicodeValue"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":205,""safe"":false},{""name"":""testUnicodeKeyValue"",""parameters"":[{""name"":""key"",""type"":""String""},{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":225,""safe"":false},{""name"":""testInt"",""parameters"":[{""name"":""key"",""type"":""Integer""}],""returntype"":""String"",""offset"":242,""safe"":false},{""name"":""testBool"",""parameters"":[{""name"":""key"",""type"":""Boolean""}],""returntype"":""String"",""offset"":266,""safe"":false},{""name"":""testDeserialize"",""parameters"":[{""name"":""key"",""type"":""String""}],""returntype"":""Any"",""offset"":292,""safe"":false},{""name"":""testuint160Key"",""parameters"":[],""returntype"":""Any"",""offset"":330,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""jsonDeserialize"",""jsonSerialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABD8DvOc7g5OklxsKgannhRA3Yb86sD2pzb25EZXNlcmlhbGl6ZQEAAQ8AAP13AVcCAchwEHEiPGlKaWhT0EVpSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkw2jKQFcBAchwDAt0ZXN0c3RyaW5nMkp42yhoU9BFaDcAAEBXAQHIcAwLdGVzdHN0cmluZzJKeNsoaFPQRWjTaDcAAEBXAgDIcAwCAQHbMNsocQwANwAASmloU9BFaDcAAEBXAQHIcAwSMTI5ODQwdGVzdDEwMDIyOTM5SnhoU9BFaDcAAEBXAQHIcHhKDAJhYmhT0EVoNwAAQFcBAshweUp4aFPQRWg3AABAVwEByHAMBnN0cmluZ0p4aFPQRWg3AABAVwEByHAMCHRlc3Rib29sSnhoU9BFaDcAAEBXAgHIcAwPdGVzdGRlc2VyaWFsaXplSnhoU9BFaDcAAHFpNwEAQFcDAMhwDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHERSmloU9BFaDcAAHJqNwEAQNbF4Ps=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMCHRlc3Rib29sSnhoU9BFaDcAAEA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 74657374626F6F6C 'testbool' [8 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : LDARG0 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : REVERSE3 [2 datoshi]
    /// 13 : SETITEM [8192 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : CALLT 0000 [32768 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBool")]
    public abstract string? TestBool(bool? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMC3Rlc3RzdHJpbmcySnjbKGhT0EVoNwAAQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 74657374737472696E6732 'teststring2' [8 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : LDARG0 [2 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : REVERSE3 [2 datoshi]
    /// 18 : SETITEM [8192 datoshi]
    /// 19 : DROP [2 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : CALLT 0000 [32768 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArray")]
    public abstract object? TestByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAyHAMAgEB2zDbKHEMADcAAEppaFPQRWg3AABA
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 0101 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0D : STLOC1 [2 datoshi]
    /// 0E : PUSHDATA1 [8 datoshi]
    /// 10 : CALLT 0000 [32768 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : LDLOC1 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : REVERSE3 [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDLOC0 [2 datoshi]
    /// 1A : CALLT 0000 [32768 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArray2")]
    public abstract string? TestByteArray2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMC3Rlc3RzdHJpbmcySnjbKGhT0EVo02g3AABA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 74657374737472696E6732 'teststring2' [8 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : LDARG0 [2 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : REVERSE3 [2 datoshi]
    /// 18 : SETITEM [8192 datoshi]
    /// 19 : DROP [2 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : CLEARITEMS [16 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : CALLT 0000 [32768 datoshi]
    /// 20 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testClear")]
    public abstract object? TestClear(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAQcSI8aUppaFPQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaMpA
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 3C [2 datoshi]
    /// 09 : LDLOC1 [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : LDLOC1 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : REVERSE3 [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : LDLOC1 [2 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : INC [4 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSHINT32 00000080 [1 datoshi]
    /// 19 : JMPGE 04 [2 datoshi]
    /// 1B : JMP 0A [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 23 : JMPLE 1E [2 datoshi]
    /// 25 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2E : AND [8 datoshi]
    /// 2F : DUP [2 datoshi]
    /// 30 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 35 : JMPLE 0C [2 datoshi]
    /// 37 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 40 : SUB [8 datoshi]
    /// 41 : STLOC1 [2 datoshi]
    /// 42 : DROP [2 datoshi]
    /// 43 : LDLOC1 [2 datoshi]
    /// 44 : LDARG0 [2 datoshi]
    /// 45 : LT [8 datoshi]
    /// 46 : JMPIF C3 [2 datoshi]
    /// 48 : LDLOC0 [2 datoshi]
    /// 49 : SIZE [4 datoshi]
    /// 4A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAMD3Rlc3RkZXNlcmlhbGl6ZUp4aFPQRWg3AABxaTcBAEA=
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 74657374646573657269616C697A65 'testdeserialize' [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : LDARG0 [2 datoshi]
    /// 18 : LDLOC0 [2 datoshi]
    /// 19 : REVERSE3 [2 datoshi]
    /// 1A : SETITEM [8192 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : CALLT 0000 [32768 datoshi]
    /// 20 : STLOC1 [2 datoshi]
    /// 21 : LDLOC1 [2 datoshi]
    /// 22 : CALLT 0100 [32768 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeserialize")]
    public abstract object? TestDeserialize(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMBnN0cmluZ0p4aFPQRWg3AABA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : LDARG0 [2 datoshi]
    /// 0F : LDLOC0 [2 datoshi]
    /// 10 : REVERSE3 [2 datoshi]
    /// 11 : SETITEM [8192 datoshi]
    /// 12 : DROP [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : CALLT 0000 [32768 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInt")]
    public abstract string? TestInt(BigInteger? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAyHAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAcRFKaWhT0EVoNwAAcmo3AQBA
    /// 00 : INITSLOT 0300 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1B : STLOC1 [2 datoshi]
    /// 1C : PUSH1 [1 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : LDLOC1 [2 datoshi]
    /// 1F : LDLOC0 [2 datoshi]
    /// 20 : REVERSE3 [2 datoshi]
    /// 21 : SETITEM [8192 datoshi]
    /// 22 : DROP [2 datoshi]
    /// 23 : LDLOC0 [2 datoshi]
    /// 24 : CALLT 0000 [32768 datoshi]
    /// 27 : STLOC2 [2 datoshi]
    /// 28 : LDLOC2 [2 datoshi]
    /// 29 : CALLT 0100 [32768 datoshi]
    /// 2C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testuint160Key")]
    public abstract object? Testuint160Key();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMEjEyOTg0MHRlc3QxMDAyMjkzOUp4aFPQRWg3AABA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 313239383430746573743130303232393339 '129840test10022939' [8 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : LDARG0 [2 datoshi]
    /// 1B : LDLOC0 [2 datoshi]
    /// 1C : REVERSE3 [2 datoshi]
    /// 1D : SETITEM [8192 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : LDLOC0 [2 datoshi]
    /// 20 : CALLT 0000 [32768 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicode")]
    public abstract string? TestUnicode(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECyHB5SnhoU9BFaDcAAEA=
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG1 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : LDARG0 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : SETITEM [8192 datoshi]
    /// 0B : DROP [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : CALLT 0000 [32768 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeKeyValue")]
    public abstract string? TestUnicodeKeyValue(string? key, string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHB4SgwCYWJoU9BFaDcAAEA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : NEWMAP [8 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHDATA1 6162 'ab' [8 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : REVERSE3 [2 datoshi]
    /// 0D : SETITEM [8192 datoshi]
    /// 0E : DROP [2 datoshi]
    /// 0F : LDLOC0 [2 datoshi]
    /// 10 : CALLT 0000 [32768 datoshi]
    /// 13 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);

    #endregion
}
