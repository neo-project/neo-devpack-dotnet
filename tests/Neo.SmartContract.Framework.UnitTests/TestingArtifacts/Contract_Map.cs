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
    [DisplayName("testBool")]
    public abstract string? TestBool(bool? key);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 000F : DUP
    // 0010 : LDARG0
    // 0011 : LDLOC0
    // 0012 : REVERSE3
    // 0013 : SETITEM
    // 0014 : DROP
    // 0015 : LDLOC0
    // 0016 : CALLT
    // 0019 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArray")]
    public abstract object? TestByteArray(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 0012 : DUP
    // 0013 : LDARG0
    // 0014 : CONVERT
    // 0016 : LDLOC0
    // 0017 : REVERSE3
    // 0018 : SETITEM
    // 0019 : DROP
    // 001A : LDLOC0
    // 001B : CALLT
    // 001E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArray2")]
    public abstract string? TestByteArray2();
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 0009 : CONVERT
    // 000B : CONVERT
    // 000D : STLOC1
    // 000E : PUSHDATA1
    // 0010 : CALLT
    // 0013 : DUP
    // 0014 : LDLOC1
    // 0015 : LDLOC0
    // 0016 : REVERSE3
    // 0017 : SETITEM
    // 0018 : DROP
    // 0019 : LDLOC0
    // 001A : CALLT
    // 001D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testClear")]
    public abstract object? TestClear(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 0012 : DUP
    // 0013 : LDARG0
    // 0014 : CONVERT
    // 0016 : LDLOC0
    // 0017 : REVERSE3
    // 0018 : SETITEM
    // 0019 : DROP
    // 001A : LDLOC0
    // 001B : CLEARITEMS
    // 001C : LDLOC0
    // 001D : CALLT
    // 0020 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : LDLOC1
    // 000A : DUP
    // 000B : LDLOC1
    // 000C : LDLOC0
    // 000D : REVERSE3
    // 000E : SETITEM
    // 000F : DROP
    // 0010 : LDLOC1
    // 0011 : DUP
    // 0012 : INC
    // 0013 : DUP
    // 0014 : PUSHINT32
    // 0019 : JMPGE
    // 001B : JMP
    // 001D : DUP
    // 001E : PUSHINT32
    // 0023 : JMPLE
    // 0025 : PUSHINT64
    // 002E : AND
    // 002F : DUP
    // 0030 : PUSHINT32
    // 0035 : JMPLE
    // 0037 : PUSHINT64
    // 0040 : SUB
    // 0041 : STLOC1
    // 0042 : DROP
    // 0043 : LDLOC1
    // 0044 : LDARG0
    // 0045 : LT
    // 0046 : JMPIF
    // 0048 : LDLOC0
    // 0049 : SIZE
    // 004A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeserialize")]
    public abstract object? TestDeserialize(string? key);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 0016 : DUP
    // 0017 : LDARG0
    // 0018 : LDLOC0
    // 0019 : REVERSE3
    // 001A : SETITEM
    // 001B : DROP
    // 001C : LDLOC0
    // 001D : CALLT
    // 0020 : STLOC1
    // 0021 : LDLOC1
    // 0022 : CALLT
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInt")]
    public abstract string? TestInt(BigInteger? key);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 000D : DUP
    // 000E : LDARG0
    // 000F : LDLOC0
    // 0010 : REVERSE3
    // 0011 : SETITEM
    // 0012 : DROP
    // 0013 : LDLOC0
    // 0014 : CALLT
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testuint160Key")]
    public abstract object? Testuint160Key();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUnicode")]
    public abstract string? TestUnicode(string? key);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 0019 : DUP
    // 001A : LDARG0
    // 001B : LDLOC0
    // 001C : REVERSE3
    // 001D : SETITEM
    // 001E : DROP
    // 001F : LDLOC0
    // 0020 : CALLT
    // 0023 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUnicodeKeyValue")]
    public abstract string? TestUnicodeKeyValue(string? key, string? value);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : LDARG1
    // 0006 : DUP
    // 0007 : LDARG0
    // 0008 : LDLOC0
    // 0009 : REVERSE3
    // 000A : SETITEM
    // 000B : DROP
    // 000C : LDLOC0
    // 000D : CALLT
    // 0010 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUnicodeValue")]
    public abstract string? TestUnicodeValue(string? value);
    // 0000 : INITSLOT
    // 0003 : NEWMAP
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : DUP
    // 0007 : PUSHDATA1
    // 000B : LDLOC0
    // 000C : REVERSE3
    // 000D : SETITEM
    // 000E : DROP
    // 000F : LDLOC0
    // 0010 : CALLT
    // 0013 : RET

    #endregion

}
