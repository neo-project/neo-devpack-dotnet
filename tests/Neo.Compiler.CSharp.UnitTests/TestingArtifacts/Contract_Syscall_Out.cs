using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Syscall_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Syscall_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testSByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":36,""safe"":false},{""name"":""testShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":73,""safe"":false},{""name"":""testUShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":113,""safe"":false},{""name"":""testIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":151,""safe"":false},{""name"":""testUIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":197,""safe"":false},{""name"":""testLongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":239,""safe"":false},{""name"":""testULongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":297,""safe"":false},{""name"":""testBoolTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":351,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":573,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/UACVwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcFhoEr9AVwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXBZaBK/QFcBARBiWnhQRTcAAErYJBJKAQCAAgCAAAC7JgZiCCIERQlwWmgSv0BXAQEQY1t4UEU3AABK2CQQShACAAABALsmBmMIIgRFCXBbaBK/QFcBARBkXHhQRTcAAErYJBhKAgAAAIADAAAAgAAAAAC7JgZkCCIERQlwXGgSv0BXAQEQZV14UEU3AABK2CQUShADAAAAAAEAAAC7JgZlCCIERQlwXWgSv0BXAQEQZl54UEU3AABK2CQkSgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyYGZggiBEUJcF5oEr9AVwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwXwdoEr9AVwEBEGcIXwh4UEVKDAR0cnVllyW4AAAASgwEVFJVRZclqwAAAEoMBFRydWWXJZ4AAABKDAF0lyWUAAAASgwBVJcligAAAEoMATGXJYAAAABKDAN5ZXOXJHRKDANZRVOXJGtKDAF5lyRkSgwBWZckXUoMBWZhbHNllyRZSgwFRkFMU0WXJE5KDAVGYWxzZZckQ0oMAWaXJDxKDAFGlyQ1SgwBMJckLkoMAm5vlyQmSgwCTk+XJB5KDAFulyQXSgwBTpckEEUJZwgJIg5FCGcICCIHRQlnCAhwXwhoEr9AVglAgS9V+Q=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcIXwh4UEVKDHRydWWXJbgAAABKDFRSVUWXJasAAABKDFRydWWXJZ4AAABKDHSXJZQAAABKDFSXJYoAAABKDDGXJYAAAABKDHllc5ckdEoMWUVTlyRrSgx5lyRkSgxZlyRdSgxmYWxzZZckWUoMRkFMU0WXJE5KDEZhbHNllyRDSgxmlyQ8SgxGlyQ1SgwwlyQuSgxub5ckJkoMTk+XJB5KDG6XJBdKDE6XJBBFCWcICSIORQhnCAgiB0UJZwgIcF8IaBK/QA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD 08
    /// 06 : OpCode.LDSFLD 08
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.DROP
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHDATA1 74727565
    /// 12 : OpCode.EQUAL
    /// 13 : OpCode.JMPIF_L B8000000
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSHDATA1 54525545
    /// 1F : OpCode.EQUAL
    /// 20 : OpCode.JMPIF_L AB000000
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSHDATA1 54727565
    /// 2C : OpCode.EQUAL
    /// 2D : OpCode.JMPIF_L 9E000000
    /// 32 : OpCode.DUP
    /// 33 : OpCode.PUSHDATA1 74
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.JMPIF_L 94000000
    /// 3C : OpCode.DUP
    /// 3D : OpCode.PUSHDATA1 54
    /// 40 : OpCode.EQUAL
    /// 41 : OpCode.JMPIF_L 8A000000
    /// 46 : OpCode.DUP
    /// 47 : OpCode.PUSHDATA1 31
    /// 4A : OpCode.EQUAL
    /// 4B : OpCode.JMPIF_L 80000000
    /// 50 : OpCode.DUP
    /// 51 : OpCode.PUSHDATA1 796573
    /// 56 : OpCode.EQUAL
    /// 57 : OpCode.JMPIF 74
    /// 59 : OpCode.DUP
    /// 5A : OpCode.PUSHDATA1 594553
    /// 5F : OpCode.EQUAL
    /// 60 : OpCode.JMPIF 6B
    /// 62 : OpCode.DUP
    /// 63 : OpCode.PUSHDATA1 79
    /// 66 : OpCode.EQUAL
    /// 67 : OpCode.JMPIF 64
    /// 69 : OpCode.DUP
    /// 6A : OpCode.PUSHDATA1 59
    /// 6D : OpCode.EQUAL
    /// 6E : OpCode.JMPIF 5D
    /// 70 : OpCode.DUP
    /// 71 : OpCode.PUSHDATA1 66616C7365
    /// 78 : OpCode.EQUAL
    /// 79 : OpCode.JMPIF 59
    /// 7B : OpCode.DUP
    /// 7C : OpCode.PUSHDATA1 46414C5345
    /// 83 : OpCode.EQUAL
    /// 84 : OpCode.JMPIF 4E
    /// 86 : OpCode.DUP
    /// 87 : OpCode.PUSHDATA1 46616C7365
    /// 8E : OpCode.EQUAL
    /// 8F : OpCode.JMPIF 43
    /// 91 : OpCode.DUP
    /// 92 : OpCode.PUSHDATA1 66
    /// 95 : OpCode.EQUAL
    /// 96 : OpCode.JMPIF 3C
    /// 98 : OpCode.DUP
    /// 99 : OpCode.PUSHDATA1 46
    /// 9C : OpCode.EQUAL
    /// 9D : OpCode.JMPIF 35
    /// 9F : OpCode.DUP
    /// A0 : OpCode.PUSHDATA1 30
    /// A3 : OpCode.EQUAL
    /// A4 : OpCode.JMPIF 2E
    /// A6 : OpCode.DUP
    /// A7 : OpCode.PUSHDATA1 6E6F
    /// AB : OpCode.EQUAL
    /// AC : OpCode.JMPIF 26
    /// AE : OpCode.DUP
    /// AF : OpCode.PUSHDATA1 4E4F
    /// B3 : OpCode.EQUAL
    /// B4 : OpCode.JMPIF 1E
    /// B6 : OpCode.DUP
    /// B7 : OpCode.PUSHDATA1 6E
    /// BA : OpCode.EQUAL
    /// BB : OpCode.JMPIF 17
    /// BD : OpCode.DUP
    /// BE : OpCode.PUSHDATA1 4E
    /// C1 : OpCode.EQUAL
    /// C2 : OpCode.JMPIF 10
    /// C4 : OpCode.DROP
    /// C5 : OpCode.PUSHF
    /// C6 : OpCode.STSFLD 08
    /// C8 : OpCode.PUSHF
    /// C9 : OpCode.JMP 0E
    /// CB : OpCode.DROP
    /// CC : OpCode.PUSHT
    /// CD : OpCode.STSFLD 08
    /// CF : OpCode.PUSHT
    /// D0 : OpCode.JMP 07
    /// D2 : OpCode.DROP
    /// D3 : OpCode.PUSHF
    /// D4 : OpCode.STSFLD 08
    /// D6 : OpCode.PUSHT
    /// D7 : OpCode.STLOC0
    /// D8 : OpCode.LDSFLD 08
    /// DA : OpCode.LDLOC0
    /// DB : OpCode.PUSH2
    /// DC : OpCode.PACKSTRUCT
    /// DD : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcFhoEr9A
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD0
    /// 05 : OpCode.LDSFLD0
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 0E
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.PUSHINT16 0001
    /// 15 : OpCode.WITHIN
    /// 16 : OpCode.JMPIFNOT 06
    /// 18 : OpCode.STSFLD0
    /// 19 : OpCode.PUSHT
    /// 1A : OpCode.JMP 04
    /// 1C : OpCode.DROP
    /// 1D : OpCode.PUSHF
    /// 1E : OpCode.STLOC0
    /// 1F : OpCode.LDSFLD0
    /// 20 : OpCode.LDLOC0
    /// 21 : OpCode.PUSH2
    /// 22 : OpCode.PACKSTRUCT
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGRceFBFNwAAStgkGEoCAAAAgAMAAACAAAAAALsmBmQIIgRFCXBcaBK/QA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD4
    /// 05 : OpCode.LDSFLD4
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 18
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 00000080
    /// 16 : OpCode.PUSHINT64 0000008000000000
    /// 1F : OpCode.WITHIN
    /// 20 : OpCode.JMPIFNOT 06
    /// 22 : OpCode.STSFLD4
    /// 23 : OpCode.PUSHT
    /// 24 : OpCode.JMP 04
    /// 26 : OpCode.DROP
    /// 27 : OpCode.PUSHF
    /// 28 : OpCode.STLOC0
    /// 29 : OpCode.LDSFLD4
    /// 2A : OpCode.LDLOC0
    /// 2B : OpCode.PUSH2
    /// 2C : OpCode.PACKSTRUCT
    /// 2D : OpCode.RET
    /// </remarks>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGZeeFBFNwAAStgkJEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALsmBmYIIgRFCXBeaBK/QA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD6
    /// 05 : OpCode.LDSFLD6
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 24
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT64 0000000000000080
    /// 1A : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 2B : OpCode.WITHIN
    /// 2C : OpCode.JMPIFNOT 06
    /// 2E : OpCode.STSFLD6
    /// 2F : OpCode.PUSHT
    /// 30 : OpCode.JMP 04
    /// 32 : OpCode.DROP
    /// 33 : OpCode.PUSHF
    /// 34 : OpCode.STLOC0
    /// 35 : OpCode.LDSFLD6
    /// 36 : OpCode.LDLOC0
    /// 37 : OpCode.PUSH2
    /// 38 : OpCode.PACKSTRUCT
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXBZaBK/QA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD1
    /// 05 : OpCode.LDSFLD1
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 0F
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT8 80
    /// 13 : OpCode.PUSHINT16 8000
    /// 16 : OpCode.WITHIN
    /// 17 : OpCode.JMPIFNOT 06
    /// 19 : OpCode.STSFLD1
    /// 1A : OpCode.PUSHT
    /// 1B : OpCode.JMP 04
    /// 1D : OpCode.DROP
    /// 1E : OpCode.PUSHF
    /// 1F : OpCode.STLOC0
    /// 20 : OpCode.LDSFLD1
    /// 21 : OpCode.LDLOC0
    /// 22 : OpCode.PUSH2
    /// 23 : OpCode.PACKSTRUCT
    /// 24 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGJaeFBFNwAAStgkEkoBAIACAIAAALsmBmIIIgRFCXBaaBK/QA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD2
    /// 05 : OpCode.LDSFLD2
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 12
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT16 0080
    /// 14 : OpCode.PUSHINT32 00800000
    /// 19 : OpCode.WITHIN
    /// 1A : OpCode.JMPIFNOT 06
    /// 1C : OpCode.STSFLD2
    /// 1D : OpCode.PUSHT
    /// 1E : OpCode.JMP 04
    /// 20 : OpCode.DROP
    /// 21 : OpCode.PUSHF
    /// 22 : OpCode.STLOC0
    /// 23 : OpCode.LDSFLD2
    /// 24 : OpCode.LDLOC0
    /// 25 : OpCode.PUSH2
    /// 26 : OpCode.PACKSTRUCT
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGVdeFBFNwAAStgkFEoQAwAAAAABAAAAuyYGZQgiBEUJcF1oEr9A
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD5
    /// 05 : OpCode.LDSFLD5
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 14
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.PUSHINT64 0000000001000000
    /// 1B : OpCode.WITHIN
    /// 1C : OpCode.JMPIFNOT 06
    /// 1E : OpCode.STSFLD5
    /// 1F : OpCode.PUSHT
    /// 20 : OpCode.JMP 04
    /// 22 : OpCode.DROP
    /// 23 : OpCode.PUSHF
    /// 24 : OpCode.STLOC0
    /// 25 : OpCode.LDSFLD5
    /// 26 : OpCode.LDLOC0
    /// 27 : OpCode.PUSH2
    /// 28 : OpCode.PACKSTRUCT
    /// 29 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwXwdoEr9A
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD 07
    /// 06 : OpCode.LDSFLD 07
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.DROP
    /// 0B : OpCode.CALLT 0000
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISNULL
    /// 10 : OpCode.JMPIF 1D
    /// 12 : OpCode.DUP
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 25 : OpCode.WITHIN
    /// 26 : OpCode.JMPIFNOT 07
    /// 28 : OpCode.STSFLD 07
    /// 2A : OpCode.PUSHT
    /// 2B : OpCode.JMP 04
    /// 2D : OpCode.DROP
    /// 2E : OpCode.PUSHF
    /// 2F : OpCode.STLOC0
    /// 30 : OpCode.LDSFLD 07
    /// 32 : OpCode.LDLOC0
    /// 33 : OpCode.PUSH2
    /// 34 : OpCode.PACKSTRUCT
    /// 35 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGNbeFBFNwAAStgkEEoQAgAAAQC7JgZjCCIERQlwW2gSv0A=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD3
    /// 05 : OpCode.LDSFLD3
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.DROP
    /// 09 : OpCode.CALLT 0000
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIF 10
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.PUSHINT32 00000100
    /// 17 : OpCode.WITHIN
    /// 18 : OpCode.JMPIFNOT 06
    /// 1A : OpCode.STSFLD3
    /// 1B : OpCode.PUSHT
    /// 1C : OpCode.JMP 04
    /// 1E : OpCode.DROP
    /// 1F : OpCode.PUSHF
    /// 20 : OpCode.STLOC0
    /// 21 : OpCode.LDSFLD3
    /// 22 : OpCode.LDLOC0
    /// 23 : OpCode.PUSH2
    /// 24 : OpCode.PACKSTRUCT
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);

    #endregion
}
