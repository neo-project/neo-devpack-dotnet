using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Syscall_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Syscall_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testSByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":39,""safe"":false},{""name"":""testShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":79,""safe"":false},{""name"":""testUShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":122,""safe"":false},{""name"":""testIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":163,""safe"":false},{""name"":""testUIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":212,""safe"":false},{""name"":""testLongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":257,""safe"":false},{""name"":""testULongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":318,""safe"":false},{""name"":""testBoolTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":375,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":600,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/VsCVwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcMVKaM9KWM9AVwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXDFSmjPSlnPQFcBARBiWnhQRTcAAErYJBJKAQCAAgCAAAC7JgZiCCIERQlwxUpoz0paz0BXAQEQY1t4UEU3AABK2CQQShACAAABALsmBmMIIgRFCXDFSmjPSlvPQFcBARBkXHhQRTcAAErYJBhKAgAAAIADAAAAgAAAAAC7JgZkCCIERQlwxUpoz0pcz0BXAQEQZV14UEU3AABK2CQUShADAAAAAAEAAAC7JgZlCCIERQlwxUpoz0pdz0BXAQEQZl54UEU3AABK2CQkSgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyYGZggiBEUJcMVKaM9KXs9AVwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwxUpoz0pfB89AVwEBEGcIXwh4UEVKDAR0cnVllyW4AAAASgwEVFJVRZclqwAAAEoMBFRydWWXJZ4AAABKDAF0lyWUAAAASgwBVJcligAAAEoMATGXJYAAAABKDAN5ZXOXJHRKDANZRVOXJGtKDAF5lyRkSgwBWZckXUoMBWZhbHNllyRZSgwFRkFMU0WXJE5KDAVGYWxzZZckQ0oMAWaXJDxKDAFGlyQ1SgwBMJckLkoMAm5vlyQmSgwCTk+XJB5KDAFulyQXSgwBTpckEEUJZwgJIg5FCGcICCIHRQlnCAhwxUpoz0pfCM9AVglA7icQAw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD
    // 0006 : LDSFLD
    // 0008 : LDARG0
    // 0009 : SWAP
    // 000A : DROP
    // 000B : DUP
    // 000C : PUSHDATA1
    // 0012 : EQUAL
    // 0013 : JMPIF_L
    // 0018 : DUP
    // 0019 : PUSHDATA1
    // 001F : EQUAL
    // 0020 : JMPIF_L
    // 0025 : DUP
    // 0026 : PUSHDATA1
    // 002C : EQUAL
    // 002D : JMPIF_L
    // 0032 : DUP
    // 0033 : PUSHDATA1
    // 0036 : EQUAL
    // 0037 : JMPIF_L
    // 003C : DUP
    // 003D : PUSHDATA1
    // 0040 : EQUAL
    // 0041 : JMPIF_L
    // 0046 : DUP
    // 0047 : PUSHDATA1
    // 004A : EQUAL
    // 004B : JMPIF_L
    // 0050 : DUP
    // 0051 : PUSHDATA1
    // 0056 : EQUAL
    // 0057 : JMPIF
    // 0059 : DUP
    // 005A : PUSHDATA1
    // 005F : EQUAL
    // 0060 : JMPIF
    // 0062 : DUP
    // 0063 : PUSHDATA1
    // 0066 : EQUAL
    // 0067 : JMPIF
    // 0069 : DUP
    // 006A : PUSHDATA1
    // 006D : EQUAL
    // 006E : JMPIF
    // 0070 : DUP
    // 0071 : PUSHDATA1
    // 0078 : EQUAL
    // 0079 : JMPIF
    // 007B : DUP
    // 007C : PUSHDATA1
    // 0083 : EQUAL
    // 0084 : JMPIF
    // 0086 : DUP
    // 0087 : PUSHDATA1
    // 008E : EQUAL
    // 008F : JMPIF
    // 0091 : DUP
    // 0092 : PUSHDATA1
    // 0095 : EQUAL
    // 0096 : JMPIF
    // 0098 : DUP
    // 0099 : PUSHDATA1
    // 009C : EQUAL
    // 009D : JMPIF
    // 009F : DUP
    // 00A0 : PUSHDATA1
    // 00A3 : EQUAL
    // 00A4 : JMPIF
    // 00A6 : DUP
    // 00A7 : PUSHDATA1
    // 00AB : EQUAL
    // 00AC : JMPIF
    // 00AE : DUP
    // 00AF : PUSHDATA1
    // 00B3 : EQUAL
    // 00B4 : JMPIF
    // 00B6 : DUP
    // 00B7 : PUSHDATA1
    // 00BA : EQUAL
    // 00BB : JMPIF
    // 00BD : DUP
    // 00BE : PUSHDATA1
    // 00C1 : EQUAL
    // 00C2 : JMPIF
    // 00C4 : DROP
    // 00C5 : PUSHF
    // 00C6 : STSFLD
    // 00C8 : PUSHF
    // 00C9 : JMP
    // 00CB : DROP
    // 00CC : PUSHT
    // 00CD : STSFLD
    // 00CF : PUSHT
    // 00D0 : JMP
    // 00D2 : DROP
    // 00D3 : PUSHF
    // 00D4 : STSFLD
    // 00D6 : PUSHT
    // 00D7 : STLOC0
    // 00D8 : NEWSTRUCT0
    // 00D9 : DUP
    // 00DA : LDLOC0
    // 00DB : APPEND
    // 00DC : DUP
    // 00DD : LDSFLD
    // 00DF : APPEND
    // 00E0 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD0
    // 0005 : LDSFLD0
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSH0
    // 0012 : PUSHINT16
    // 0015 : WITHIN
    // 0016 : JMPIFNOT
    // 0018 : STSFLD0
    // 0019 : PUSHT
    // 001A : JMP
    // 001C : DROP
    // 001D : PUSHF
    // 001E : STLOC0
    // 001F : NEWSTRUCT0
    // 0020 : DUP
    // 0021 : LDLOC0
    // 0022 : APPEND
    // 0023 : DUP
    // 0024 : LDSFLD0
    // 0025 : APPEND
    // 0026 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD4
    // 0005 : LDSFLD4
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSHINT32
    // 0016 : PUSHINT64
    // 001F : WITHIN
    // 0020 : JMPIFNOT
    // 0022 : STSFLD4
    // 0023 : PUSHT
    // 0024 : JMP
    // 0026 : DROP
    // 0027 : PUSHF
    // 0028 : STLOC0
    // 0029 : NEWSTRUCT0
    // 002A : DUP
    // 002B : LDLOC0
    // 002C : APPEND
    // 002D : DUP
    // 002E : LDSFLD4
    // 002F : APPEND
    // 0030 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD6
    // 0005 : LDSFLD6
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSHINT64
    // 001A : PUSHINT128
    // 002B : WITHIN
    // 002C : JMPIFNOT
    // 002E : STSFLD6
    // 002F : PUSHT
    // 0030 : JMP
    // 0032 : DROP
    // 0033 : PUSHF
    // 0034 : STLOC0
    // 0035 : NEWSTRUCT0
    // 0036 : DUP
    // 0037 : LDLOC0
    // 0038 : APPEND
    // 0039 : DUP
    // 003A : LDSFLD6
    // 003B : APPEND
    // 003C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD1
    // 0005 : LDSFLD1
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSHINT8
    // 0013 : PUSHINT16
    // 0016 : WITHIN
    // 0017 : JMPIFNOT
    // 0019 : STSFLD1
    // 001A : PUSHT
    // 001B : JMP
    // 001D : DROP
    // 001E : PUSHF
    // 001F : STLOC0
    // 0020 : NEWSTRUCT0
    // 0021 : DUP
    // 0022 : LDLOC0
    // 0023 : APPEND
    // 0024 : DUP
    // 0025 : LDSFLD1
    // 0026 : APPEND
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD2
    // 0005 : LDSFLD2
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSHINT16
    // 0014 : PUSHINT32
    // 0019 : WITHIN
    // 001A : JMPIFNOT
    // 001C : STSFLD2
    // 001D : PUSHT
    // 001E : JMP
    // 0020 : DROP
    // 0021 : PUSHF
    // 0022 : STLOC0
    // 0023 : NEWSTRUCT0
    // 0024 : DUP
    // 0025 : LDLOC0
    // 0026 : APPEND
    // 0027 : DUP
    // 0028 : LDSFLD2
    // 0029 : APPEND
    // 002A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD5
    // 0005 : LDSFLD5
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSH0
    // 0012 : PUSHINT64
    // 001B : WITHIN
    // 001C : JMPIFNOT
    // 001E : STSFLD5
    // 001F : PUSHT
    // 0020 : JMP
    // 0022 : DROP
    // 0023 : PUSHF
    // 0024 : STLOC0
    // 0025 : NEWSTRUCT0
    // 0026 : DUP
    // 0027 : LDLOC0
    // 0028 : APPEND
    // 0029 : DUP
    // 002A : LDSFLD5
    // 002B : APPEND
    // 002C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD
    // 0006 : LDSFLD
    // 0008 : LDARG0
    // 0009 : SWAP
    // 000A : DROP
    // 000B : CALLT
    // 000E : DUP
    // 000F : ISNULL
    // 0010 : JMPIF
    // 0012 : DUP
    // 0013 : PUSH0
    // 0014 : PUSHINT128
    // 0025 : WITHIN
    // 0026 : JMPIFNOT
    // 0028 : STSFLD
    // 002A : PUSHT
    // 002B : JMP
    // 002D : DROP
    // 002E : PUSHF
    // 002F : STLOC0
    // 0030 : NEWSTRUCT0
    // 0031 : DUP
    // 0032 : LDLOC0
    // 0033 : APPEND
    // 0034 : DUP
    // 0035 : LDSFLD
    // 0037 : APPEND
    // 0038 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD3
    // 0005 : LDSFLD3
    // 0006 : LDARG0
    // 0007 : SWAP
    // 0008 : DROP
    // 0009 : CALLT
    // 000C : DUP
    // 000D : ISNULL
    // 000E : JMPIF
    // 0010 : DUP
    // 0011 : PUSH0
    // 0012 : PUSHINT32
    // 0017 : WITHIN
    // 0018 : JMPIFNOT
    // 001A : STSFLD3
    // 001B : PUSHT
    // 001C : JMP
    // 001E : DROP
    // 001F : PUSHF
    // 0020 : STLOC0
    // 0021 : NEWSTRUCT0
    // 0022 : DUP
    // 0023 : LDLOC0
    // 0024 : APPEND
    // 0025 : DUP
    // 0026 : LDSFLD3
    // 0027 : APPEND
    // 0028 : RET

    #endregion

}
