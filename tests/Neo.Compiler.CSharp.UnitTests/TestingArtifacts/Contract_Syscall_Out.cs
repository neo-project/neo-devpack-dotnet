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
    /// <remarks>
    /// Script: VwEBEGcIXwh4UEVKDHRydWWXJbgAAABKDFRSVUWXJasAAABKDFRydWWXJZ4AAABKDHSXJZQAAABKDFSXJYoAAABKDDGXJYAAAABKDHllc5ckdEoMWUVTlyRrSgx5lyRkSgxZlyRdSgxmYWxzZZckWUoMRkFMU0WXJE5KDEZhbHNllyRDSgxmlyQ8SgxGlyQ1SgwwlyQuSgxub5ckJkoMTk+XJB5KDG6XJBdKDE6XJBBFCWcICSIORQhnCAgiB0UJZwgIcMVKaM9KXwjPQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD 08
    /// 0006 : OpCode.LDSFLD 08
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.DROP
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHDATA1 74727565
    /// 0012 : OpCode.EQUAL
    /// 0013 : OpCode.JMPIF_L B8000000
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.PUSHDATA1 54525545
    /// 001F : OpCode.EQUAL
    /// 0020 : OpCode.JMPIF_L AB000000
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSHDATA1 54727565
    /// 002C : OpCode.EQUAL
    /// 002D : OpCode.JMPIF_L 9E000000
    /// 0032 : OpCode.DUP
    /// 0033 : OpCode.PUSHDATA1 74
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.JMPIF_L 94000000
    /// 003C : OpCode.DUP
    /// 003D : OpCode.PUSHDATA1 54
    /// 0040 : OpCode.EQUAL
    /// 0041 : OpCode.JMPIF_L 8A000000
    /// 0046 : OpCode.DUP
    /// 0047 : OpCode.PUSHDATA1 31
    /// 004A : OpCode.EQUAL
    /// 004B : OpCode.JMPIF_L 80000000
    /// 0050 : OpCode.DUP
    /// 0051 : OpCode.PUSHDATA1 796573
    /// 0056 : OpCode.EQUAL
    /// 0057 : OpCode.JMPIF 74
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.PUSHDATA1 594553
    /// 005F : OpCode.EQUAL
    /// 0060 : OpCode.JMPIF 6B
    /// 0062 : OpCode.DUP
    /// 0063 : OpCode.PUSHDATA1 79
    /// 0066 : OpCode.EQUAL
    /// 0067 : OpCode.JMPIF 64
    /// 0069 : OpCode.DUP
    /// 006A : OpCode.PUSHDATA1 59
    /// 006D : OpCode.EQUAL
    /// 006E : OpCode.JMPIF 5D
    /// 0070 : OpCode.DUP
    /// 0071 : OpCode.PUSHDATA1 66616C7365
    /// 0078 : OpCode.EQUAL
    /// 0079 : OpCode.JMPIF 59
    /// 007B : OpCode.DUP
    /// 007C : OpCode.PUSHDATA1 46414C5345
    /// 0083 : OpCode.EQUAL
    /// 0084 : OpCode.JMPIF 4E
    /// 0086 : OpCode.DUP
    /// 0087 : OpCode.PUSHDATA1 46616C7365
    /// 008E : OpCode.EQUAL
    /// 008F : OpCode.JMPIF 43
    /// 0091 : OpCode.DUP
    /// 0092 : OpCode.PUSHDATA1 66
    /// 0095 : OpCode.EQUAL
    /// 0096 : OpCode.JMPIF 3C
    /// 0098 : OpCode.DUP
    /// 0099 : OpCode.PUSHDATA1 46
    /// 009C : OpCode.EQUAL
    /// 009D : OpCode.JMPIF 35
    /// 009F : OpCode.DUP
    /// 00A0 : OpCode.PUSHDATA1 30
    /// 00A3 : OpCode.EQUAL
    /// 00A4 : OpCode.JMPIF 2E
    /// 00A6 : OpCode.DUP
    /// 00A7 : OpCode.PUSHDATA1 6E6F
    /// 00AB : OpCode.EQUAL
    /// 00AC : OpCode.JMPIF 26
    /// 00AE : OpCode.DUP
    /// 00AF : OpCode.PUSHDATA1 4E4F
    /// 00B3 : OpCode.EQUAL
    /// 00B4 : OpCode.JMPIF 1E
    /// 00B6 : OpCode.DUP
    /// 00B7 : OpCode.PUSHDATA1 6E
    /// 00BA : OpCode.EQUAL
    /// 00BB : OpCode.JMPIF 17
    /// 00BD : OpCode.DUP
    /// 00BE : OpCode.PUSHDATA1 4E
    /// 00C1 : OpCode.EQUAL
    /// 00C2 : OpCode.JMPIF 10
    /// 00C4 : OpCode.DROP
    /// 00C5 : OpCode.PUSHF
    /// 00C6 : OpCode.STSFLD 08
    /// 00C8 : OpCode.PUSHF
    /// 00C9 : OpCode.JMP 0E
    /// 00CB : OpCode.DROP
    /// 00CC : OpCode.PUSHT
    /// 00CD : OpCode.STSFLD 08
    /// 00CF : OpCode.PUSHT
    /// 00D0 : OpCode.JMP 07
    /// 00D2 : OpCode.DROP
    /// 00D3 : OpCode.PUSHF
    /// 00D4 : OpCode.STSFLD 08
    /// 00D6 : OpCode.PUSHT
    /// 00D7 : OpCode.STLOC0
    /// 00D8 : OpCode.NEWSTRUCT0
    /// 00D9 : OpCode.DUP
    /// 00DA : OpCode.LDLOC0
    /// 00DB : OpCode.APPEND
    /// 00DC : OpCode.DUP
    /// 00DD : OpCode.LDSFLD 08
    /// 00DF : OpCode.APPEND
    /// 00E0 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcMVKaM9KWM9A
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD0
    /// 0005 : OpCode.LDSFLD0
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 0E
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.PUSHINT16 0001
    /// 0015 : OpCode.WITHIN
    /// 0016 : OpCode.JMPIFNOT 06
    /// 0018 : OpCode.STSFLD0
    /// 0019 : OpCode.PUSHT
    /// 001A : OpCode.JMP 04
    /// 001C : OpCode.DROP
    /// 001D : OpCode.PUSHF
    /// 001E : OpCode.STLOC0
    /// 001F : OpCode.NEWSTRUCT0
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.LDLOC0
    /// 0022 : OpCode.APPEND
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.LDSFLD0
    /// 0025 : OpCode.APPEND
    /// 0026 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGRceFBFNwAAStgkGEoCAAAAgAMAAACAAAAAALsmBmQIIgRFCXDFSmjPSlzPQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD4
    /// 0005 : OpCode.LDSFLD4
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 18
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 00000080
    /// 0016 : OpCode.PUSHINT64 0000008000000000
    /// 001F : OpCode.WITHIN
    /// 0020 : OpCode.JMPIFNOT 06
    /// 0022 : OpCode.STSFLD4
    /// 0023 : OpCode.PUSHT
    /// 0024 : OpCode.JMP 04
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.PUSHF
    /// 0028 : OpCode.STLOC0
    /// 0029 : OpCode.NEWSTRUCT0
    /// 002A : OpCode.DUP
    /// 002B : OpCode.LDLOC0
    /// 002C : OpCode.APPEND
    /// 002D : OpCode.DUP
    /// 002E : OpCode.LDSFLD4
    /// 002F : OpCode.APPEND
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGZeeFBFNwAAStgkJEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALsmBmYIIgRFCXDFSmjPSl7PQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD6
    /// 0005 : OpCode.LDSFLD6
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 24
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT64 0000000000000080
    /// 001A : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 002B : OpCode.WITHIN
    /// 002C : OpCode.JMPIFNOT 06
    /// 002E : OpCode.STSFLD6
    /// 002F : OpCode.PUSHT
    /// 0030 : OpCode.JMP 04
    /// 0032 : OpCode.DROP
    /// 0033 : OpCode.PUSHF
    /// 0034 : OpCode.STLOC0
    /// 0035 : OpCode.NEWSTRUCT0
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.LDLOC0
    /// 0038 : OpCode.APPEND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.LDSFLD6
    /// 003B : OpCode.APPEND
    /// 003C : OpCode.RET
    /// </remarks>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXDFSmjPSlnPQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD1
    /// 0005 : OpCode.LDSFLD1
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 0F
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT8 80
    /// 0013 : OpCode.PUSHINT16 8000
    /// 0016 : OpCode.WITHIN
    /// 0017 : OpCode.JMPIFNOT 06
    /// 0019 : OpCode.STSFLD1
    /// 001A : OpCode.PUSHT
    /// 001B : OpCode.JMP 04
    /// 001D : OpCode.DROP
    /// 001E : OpCode.PUSHF
    /// 001F : OpCode.STLOC0
    /// 0020 : OpCode.NEWSTRUCT0
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.LDLOC0
    /// 0023 : OpCode.APPEND
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.LDSFLD1
    /// 0026 : OpCode.APPEND
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGJaeFBFNwAAStgkEkoBAIACAIAAALsmBmIIIgRFCXDFSmjPSlrPQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD2
    /// 0005 : OpCode.LDSFLD2
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 12
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT16 0080
    /// 0014 : OpCode.PUSHINT32 00800000
    /// 0019 : OpCode.WITHIN
    /// 001A : OpCode.JMPIFNOT 06
    /// 001C : OpCode.STSFLD2
    /// 001D : OpCode.PUSHT
    /// 001E : OpCode.JMP 04
    /// 0020 : OpCode.DROP
    /// 0021 : OpCode.PUSHF
    /// 0022 : OpCode.STLOC0
    /// 0023 : OpCode.NEWSTRUCT0
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.LDLOC0
    /// 0026 : OpCode.APPEND
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.LDSFLD2
    /// 0029 : OpCode.APPEND
    /// 002A : OpCode.RET
    /// </remarks>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGVdeFBFNwAAStgkFEoQAwAAAAABAAAAuyYGZQgiBEUJcMVKaM9KXc9A
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD5
    /// 0005 : OpCode.LDSFLD5
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 14
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.PUSHINT64 0000000001000000
    /// 001B : OpCode.WITHIN
    /// 001C : OpCode.JMPIFNOT 06
    /// 001E : OpCode.STSFLD5
    /// 001F : OpCode.PUSHT
    /// 0020 : OpCode.JMP 04
    /// 0022 : OpCode.DROP
    /// 0023 : OpCode.PUSHF
    /// 0024 : OpCode.STLOC0
    /// 0025 : OpCode.NEWSTRUCT0
    /// 0026 : OpCode.DUP
    /// 0027 : OpCode.LDLOC0
    /// 0028 : OpCode.APPEND
    /// 0029 : OpCode.DUP
    /// 002A : OpCode.LDSFLD5
    /// 002B : OpCode.APPEND
    /// 002C : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwxUpoz0pfB89A
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD 07
    /// 0006 : OpCode.LDSFLD 07
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.DROP
    /// 000B : OpCode.CALLT 0000
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ISNULL
    /// 0010 : OpCode.JMPIF 1D
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.PUSH0
    /// 0014 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 0025 : OpCode.WITHIN
    /// 0026 : OpCode.JMPIFNOT 07
    /// 0028 : OpCode.STSFLD 07
    /// 002A : OpCode.PUSHT
    /// 002B : OpCode.JMP 04
    /// 002D : OpCode.DROP
    /// 002E : OpCode.PUSHF
    /// 002F : OpCode.STLOC0
    /// 0030 : OpCode.NEWSTRUCT0
    /// 0031 : OpCode.DUP
    /// 0032 : OpCode.LDLOC0
    /// 0033 : OpCode.APPEND
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.LDSFLD 07
    /// 0037 : OpCode.APPEND
    /// 0038 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGNbeFBFNwAAStgkEEoQAgAAAQC7JgZjCCIERQlwxUpoz0pbz0A=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD3
    /// 0005 : OpCode.LDSFLD3
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.CALLT 0000
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIF 10
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.PUSHINT32 00000100
    /// 0017 : OpCode.WITHIN
    /// 0018 : OpCode.JMPIFNOT 06
    /// 001A : OpCode.STSFLD3
    /// 001B : OpCode.PUSHT
    /// 001C : OpCode.JMP 04
    /// 001E : OpCode.DROP
    /// 001F : OpCode.PUSHF
    /// 0020 : OpCode.STLOC0
    /// 0021 : OpCode.NEWSTRUCT0
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.LDLOC0
    /// 0024 : OpCode.APPEND
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.LDSFLD3
    /// 0027 : OpCode.APPEND
    /// 0028 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);

    #endregion

}
