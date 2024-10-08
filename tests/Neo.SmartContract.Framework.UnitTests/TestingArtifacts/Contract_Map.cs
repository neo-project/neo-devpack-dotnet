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
    /// Script: VwEByHAMdGVzdGJvb2xKeGhT0EVoNwAAQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 74657374626F6F6C
    /// 0F : OpCode.DUP
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.REVERSE3
    /// 13 : OpCode.SETITEM
    /// 14 : OpCode.DROP
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.CALLT 0000
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("testBool")]
    public abstract string? TestBool(bool? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdHN0cmluZzJKeNsoaFPQRWg3AABA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 74657374737472696E6732
    /// 12 : OpCode.DUP
    /// 13 : OpCode.LDARG0
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.REVERSE3
    /// 18 : OpCode.SETITEM
    /// 19 : OpCode.DROP
    /// 1A : OpCode.LDLOC0
    /// 1B : OpCode.CALLT 0000
    /// 1E : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArray")]
    public abstract object? TestByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAyHAMAQHbMNsocQw3AABKaWhT0EVoNwAAQA==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 0101
    /// 09 : OpCode.CONVERT 30
    /// 0B : OpCode.CONVERT 28
    /// 0D : OpCode.STLOC1
    /// 0E : OpCode.PUSHDATA1
    /// 10 : OpCode.CALLT 0000
    /// 13 : OpCode.DUP
    /// 14 : OpCode.LDLOC1
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.REVERSE3
    /// 17 : OpCode.SETITEM
    /// 18 : OpCode.DROP
    /// 19 : OpCode.LDLOC0
    /// 1A : OpCode.CALLT 0000
    /// 1D : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArray2")]
    public abstract string? TestByteArray2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdHN0cmluZzJKeNsoaFPQRWjTaDcAAEA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 74657374737472696E6732
    /// 12 : OpCode.DUP
    /// 13 : OpCode.LDARG0
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.REVERSE3
    /// 18 : OpCode.SETITEM
    /// 19 : OpCode.DROP
    /// 1A : OpCode.LDLOC0
    /// 1B : OpCode.CLEARITEMS
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.CALLT 0000
    /// 20 : OpCode.RET
    /// </remarks>
    [DisplayName("testClear")]
    public abstract object? TestClear(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAQcSI8aUppaFPQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaMpA
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 3C
    /// 09 : OpCode.LDLOC1
    /// 0A : OpCode.DUP
    /// 0B : OpCode.LDLOC1
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.REVERSE3
    /// 0E : OpCode.SETITEM
    /// 0F : OpCode.DROP
    /// 10 : OpCode.LDLOC1
    /// 11 : OpCode.DUP
    /// 12 : OpCode.INC
    /// 13 : OpCode.DUP
    /// 14 : OpCode.PUSHINT32 00000080
    /// 19 : OpCode.JMPGE 04
    /// 1B : OpCode.JMP 0A
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 FFFFFF7F
    /// 23 : OpCode.JMPLE 1E
    /// 25 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2E : OpCode.AND
    /// 2F : OpCode.DUP
    /// 30 : OpCode.PUSHINT32 FFFFFF7F
    /// 35 : OpCode.JMPLE 0C
    /// 37 : OpCode.PUSHINT64 0000000001000000
    /// 40 : OpCode.SUB
    /// 41 : OpCode.STLOC1
    /// 42 : OpCode.DROP
    /// 43 : OpCode.LDLOC1
    /// 44 : OpCode.LDARG0
    /// 45 : OpCode.LT
    /// 46 : OpCode.JMPIF C3
    /// 48 : OpCode.LDLOC0
    /// 49 : OpCode.SIZE
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAMdGVzdGRlc2VyaWFsaXplSnhoU9BFaDcAAHFpNwEAQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 74657374646573657269616C697A65
    /// 16 : OpCode.DUP
    /// 17 : OpCode.LDARG0
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.REVERSE3
    /// 1A : OpCode.SETITEM
    /// 1B : OpCode.DROP
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.CALLT 0000
    /// 20 : OpCode.STLOC1
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.CALLT 0100
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeserialize")]
    public abstract object? TestDeserialize(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMc3RyaW5nSnhoU9BFaDcAAEA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 737472696E67
    /// 0D : OpCode.DUP
    /// 0E : OpCode.LDARG0
    /// 0F : OpCode.LDLOC0
    /// 10 : OpCode.REVERSE3
    /// 11 : OpCode.SETITEM
    /// 12 : OpCode.DROP
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.CALLT 0000
    /// 17 : OpCode.RET
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
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 313239383430746573743130303232393339
    /// 19 : OpCode.DUP
    /// 1A : OpCode.LDARG0
    /// 1B : OpCode.LDLOC0
    /// 1C : OpCode.REVERSE3
    /// 1D : OpCode.SETITEM
    /// 1E : OpCode.DROP
    /// 1F : OpCode.LDLOC0
    /// 20 : OpCode.CALLT 0000
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("testUnicode")]
    public abstract string? TestUnicode(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECyHB5SnhoU9BFaDcAAEA=
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG1
    /// 06 : OpCode.DUP
    /// 07 : OpCode.LDARG0
    /// 08 : OpCode.LDLOC0
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.SETITEM
    /// 0B : OpCode.DROP
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.CALLT 0000
    /// 10 : OpCode.RET
    /// </remarks>
    [DisplayName("testUnicodeKeyValue")]
    public abstract string? TestUnicodeKeyValue(string? key, string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHB4SgxhYmhT0EVoNwAAQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.NEWMAP
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHDATA1 6162
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.REVERSE3
    /// 0D : OpCode.SETITEM
    /// 0E : OpCode.DROP
    /// 0F : OpCode.LDLOC0
    /// 10 : OpCode.CALLT 0000
    /// 13 : OpCode.RET
    /// </remarks>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);

    #endregion
}
