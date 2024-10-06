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
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 74657374626F6F6C
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.REVERSE3
    /// 0013 : OpCode.SETITEM
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.CALLT 0000
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("testBool")]
    public abstract string? TestBool(bool? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdHN0cmluZzJKeNsoaFPQRWg3AABA
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 74657374737472696E6732
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.LDARG0
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.REVERSE3
    /// 0018 : OpCode.SETITEM
    /// 0019 : OpCode.DROP
    /// 001A : OpCode.LDLOC0
    /// 001B : OpCode.CALLT 0000
    /// 001E : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArray")]
    public abstract object? TestByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAyHAMAQHbMNsocQw3AABKaWhT0EVoNwAAQA==
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 0101
    /// 0009 : OpCode.CONVERT 30
    /// 000B : OpCode.CONVERT 28
    /// 000D : OpCode.STLOC1
    /// 000E : OpCode.PUSHDATA1
    /// 0010 : OpCode.CALLT 0000
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.LDLOC1
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.REVERSE3
    /// 0017 : OpCode.SETITEM
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.LDLOC0
    /// 001A : OpCode.CALLT 0000
    /// 001D : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArray2")]
    public abstract string? TestByteArray2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMdGVzdHN0cmluZzJKeNsoaFPQRWjTaDcAAEA=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 74657374737472696E6732
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.LDARG0
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.REVERSE3
    /// 0018 : OpCode.SETITEM
    /// 0019 : OpCode.DROP
    /// 001A : OpCode.LDLOC0
    /// 001B : OpCode.CLEARITEMS
    /// 001C : OpCode.LDLOC0
    /// 001D : OpCode.CALLT 0000
    /// 0020 : OpCode.RET
    /// </remarks>
    [DisplayName("testClear")]
    public abstract object? TestClear(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAQcSI8aUppaFPQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaMpA
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 3C
    /// 0009 : OpCode.LDLOC1
    /// 000A : OpCode.DUP
    /// 000B : OpCode.LDLOC1
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.REVERSE3
    /// 000E : OpCode.SETITEM
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.LDLOC1
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.INC
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.PUSHINT32 00000080
    /// 0019 : OpCode.JMPGE 04
    /// 001B : OpCode.JMP 0A
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 FFFFFF7F
    /// 0023 : OpCode.JMPLE 1E
    /// 0025 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002E : OpCode.AND
    /// 002F : OpCode.DUP
    /// 0030 : OpCode.PUSHINT32 FFFFFF7F
    /// 0035 : OpCode.JMPLE 0C
    /// 0037 : OpCode.PUSHINT64 0000000001000000
    /// 0040 : OpCode.SUB
    /// 0041 : OpCode.STLOC1
    /// 0042 : OpCode.DROP
    /// 0043 : OpCode.LDLOC1
    /// 0044 : OpCode.LDARG0
    /// 0045 : OpCode.LT
    /// 0046 : OpCode.JMPIF C3
    /// 0048 : OpCode.LDLOC0
    /// 0049 : OpCode.SIZE
    /// 004A : OpCode.RET
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIByHAMdGVzdGRlc2VyaWFsaXplSnhoU9BFaDcAAHFpNwEAQA==
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 74657374646573657269616C697A65
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.LDARG0
    /// 0018 : OpCode.LDLOC0
    /// 0019 : OpCode.REVERSE3
    /// 001A : OpCode.SETITEM
    /// 001B : OpCode.DROP
    /// 001C : OpCode.LDLOC0
    /// 001D : OpCode.CALLT 0000
    /// 0020 : OpCode.STLOC1
    /// 0021 : OpCode.LDLOC1
    /// 0022 : OpCode.CALLT 0100
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeserialize")]
    public abstract object? TestDeserialize(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHAMc3RyaW5nSnhoU9BFaDcAAEA=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 737472696E67
    /// 000D : OpCode.DUP
    /// 000E : OpCode.LDARG0
    /// 000F : OpCode.LDLOC0
    /// 0010 : OpCode.REVERSE3
    /// 0011 : OpCode.SETITEM
    /// 0012 : OpCode.DROP
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.CALLT 0000
    /// 0017 : OpCode.RET
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
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 313239383430746573743130303232393339
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.LDARG0
    /// 001B : OpCode.LDLOC0
    /// 001C : OpCode.REVERSE3
    /// 001D : OpCode.SETITEM
    /// 001E : OpCode.DROP
    /// 001F : OpCode.LDLOC0
    /// 0020 : OpCode.CALLT 0000
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("testUnicode")]
    public abstract string? TestUnicode(string? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECyHB5SnhoU9BFaDcAAEA=
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG1
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.LDARG0
    /// 0008 : OpCode.LDLOC0
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.SETITEM
    /// 000B : OpCode.DROP
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.CALLT 0000
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("testUnicodeKeyValue")]
    public abstract string? TestUnicodeKeyValue(string? key, string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEByHB4SgxhYmhT0EVoNwAAQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.NEWMAP
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHDATA1 6162
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.REVERSE3
    /// 000D : OpCode.SETITEM
    /// 000E : OpCode.DROP
    /// 000F : OpCode.LDLOC0
    /// 0010 : OpCode.CALLT 0000
    /// 0013 : OpCode.RET
    /// </remarks>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);

    #endregion

}
