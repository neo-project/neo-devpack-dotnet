using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABD8DvOc7g5OklxsKgannhRA3Yb86sD2pzb25EZXNlcmlhbGl6ZQEAAQ8AAP13AVcCAchwEHEiPGlKaWhT0EVpSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkw2jKQFcBAchwDAt0ZXN0c3RyaW5nMkp42yhoU9BFaDcAAEBXAQHIcAwLdGVzdHN0cmluZzJKeNsoaFPQRWjTaDcAAEBXAgDIcAwCAQHbMNsocQwANwAASmloU9BFaDcAAEBXAQHIcAwSMTI5ODQwdGVzdDEwMDIyOTM5SnhoU9BFaDcAAEBXAQHIcHhKDAJhYmhT0EVoNwAAQFcBAshweUp4aFPQRWg3AABAVwEByHAMBnN0cmluZ0p4aFPQRWg3AABAVwEByHAMCHRlc3Rib29sSnhoU9BFaDcAAEBXAgHIcAwPdGVzdGRlc2VyaWFsaXplSnhoU9BFaDcAAHFpNwEAQFcDAMhwDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHERSmloU9BFaDcAAHJqNwEAQNbF4Ps="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMCHRlc3Rib29sSnhoU9BFaDcAAEA=
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 74657374626F6F6C 'testbool' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwEByHAMC3Rlc3RzdHJpbmcySnjbKGhT0EVoNwAAQA==
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 74657374737472696E6732 'teststring2' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwIAyHAMAgEB2zDbKHEMADcAAEppaFPQRWg3AABA
    /// INITSLOT 0200 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 0101 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwEByHAMC3Rlc3RzdHJpbmcySnjbKGhT0EVo02g3AABA
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 74657374737472696E6732 'teststring2' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwIByHAQcSI8aUppaFPQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaMpA
    /// INITSLOT 0201 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 3C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
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
    /// JMPIF C3 [2 datoshi]
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
    /// Script: VwIByHAMD3Rlc3RkZXNlcmlhbGl6ZUp4aFPQRWg3AABxaTcBAEA=
    /// INITSLOT 0201 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 74657374646573657269616C697A65 'testdeserialize' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwEByHAMBnN0cmluZ0p4aFPQRWg3AABA
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwMAyHAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAcRFKaWhT0EVoNwAAcmo3AQBA
    /// INITSLOT 0300 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwEByHAMEjEyOTg0MHRlc3QxMDAyMjkzOUp4aFPQRWg3AABA
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 313239383430746573743130303232393339 '129840test10022939' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwECyHB5SnhoU9BFaDcAAEA=
    /// INITSLOT 0102 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwEByHB4SgwCYWJoU9BFaDcAAEA=
    /// INITSLOT 0101 [64 datoshi]
    /// NEWMAP [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6162 'ab' [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);

    #endregion
}
