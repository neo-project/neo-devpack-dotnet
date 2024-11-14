using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Syscall_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Syscall_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testSByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":36,""safe"":false},{""name"":""testShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":73,""safe"":false},{""name"":""testUShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":113,""safe"":false},{""name"":""testIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":151,""safe"":false},{""name"":""testUIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":197,""safe"":false},{""name"":""testLongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":239,""safe"":false},{""name"":""testULongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":297,""safe"":false},{""name"":""testBoolTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":351,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":570,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/T0CVwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcFhoEr9AVwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXBZaBK/QFcBARBiWnhQRTcAAErYJBJKAQCAAgCAAAC7JgZiCCIERQlwWmgSv0BXAQEQY1t4UEU3AABK2CQQShACAAABALsmBmMIIgRFCXBbaBK/QFcBARBkXHhQRTcAAErYJBhKAgAAAIADAAAAgAAAAAC7JgZkCCIERQlwXGgSv0BXAQEQZV14UEU3AABK2CQUShADAAAAAAEAAAC7JgZlCCIERQlwXWgSv0BXAQEQZl54UEU3AABK2CQkSgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyYGZggiBEUJcF5oEr9AVwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwXwdoEr9AVwEBCWcIXwh4UEVKDAR0cnVllyW1AAAASgwEVFJVRZclqAAAAEoMBFRydWWXJZsAAABKDAF0lyWRAAAASgwBVJclhwAAAEoMATGXJH1KDAN5ZXOXJHRKDANZRVOXJGtKDAF5lyRkSgwBWZckXUoMBWZhbHNllyRZSgwFRkFMU0WXJE5KDAVGYWxzZZckQ0oMAWaXJDxKDAFGlyQ1SgwBMJckLkoMAm5vlyQmSgwCTk+XJB5KDAFulyQXSgwBTpckEEUJZwgJIg5FCGcICCIHRQlnCAhwXwhoEr9AVglAhvH4bA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCWcIXwh4UEVKDAR0cnVllyW1AAAASgwEVFJVRZclqAAAAEoMBFRydWWXJZsAAABKDAF0lyWRAAAASgwBVJclhwAAAEoMATGXJH1KDAN5ZXOXJHRKDANZRVOXJGtKDAF5lyRkSgwBWZckXUoMBWZhbHNllyRZSgwFRkFMU0WXJE5KDAVGYWxzZZckQ0oMAWaXJDxKDAFGlyQ1SgwBMJckLkoMAm5vlyQmSgwCTk+XJB5KDAFulyQXSgwBTpckEEUJZwgJIg5FCGcICCIHRQlnCAhwXwhoEr9A
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHF [1 datoshi]
    /// 04 : STSFLD 08 [2 datoshi]
    /// 06 : LDSFLD 08 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : DROP [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHDATA1 74727565 'true' [8 datoshi]
    /// 12 : EQUAL [32 datoshi]
    /// 13 : JMPIF_L B5000000 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSHDATA1 54525545 'TRUE' [8 datoshi]
    /// 1F : EQUAL [32 datoshi]
    /// 20 : JMPIF_L A8000000 [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSHDATA1 54727565 'True' [8 datoshi]
    /// 2C : EQUAL [32 datoshi]
    /// 2D : JMPIF_L 9B000000 [2 datoshi]
    /// 32 : DUP [2 datoshi]
    /// 33 : PUSHDATA1 74 't' [8 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : JMPIF_L 91000000 [2 datoshi]
    /// 3C : DUP [2 datoshi]
    /// 3D : PUSHDATA1 54 'T' [8 datoshi]
    /// 40 : EQUAL [32 datoshi]
    /// 41 : JMPIF_L 87000000 [2 datoshi]
    /// 46 : DUP [2 datoshi]
    /// 47 : PUSHDATA1 31 '1' [8 datoshi]
    /// 4A : EQUAL [32 datoshi]
    /// 4B : JMPIF 7D [2 datoshi]
    /// 4D : DUP [2 datoshi]
    /// 4E : PUSHDATA1 796573 'yes' [8 datoshi]
    /// 53 : EQUAL [32 datoshi]
    /// 54 : JMPIF 74 [2 datoshi]
    /// 56 : DUP [2 datoshi]
    /// 57 : PUSHDATA1 594553 'YES' [8 datoshi]
    /// 5C : EQUAL [32 datoshi]
    /// 5D : JMPIF 6B [2 datoshi]
    /// 5F : DUP [2 datoshi]
    /// 60 : PUSHDATA1 79 'y' [8 datoshi]
    /// 63 : EQUAL [32 datoshi]
    /// 64 : JMPIF 64 [2 datoshi]
    /// 66 : DUP [2 datoshi]
    /// 67 : PUSHDATA1 59 'Y' [8 datoshi]
    /// 6A : EQUAL [32 datoshi]
    /// 6B : JMPIF 5D [2 datoshi]
    /// 6D : DUP [2 datoshi]
    /// 6E : PUSHDATA1 66616C7365 'false' [8 datoshi]
    /// 75 : EQUAL [32 datoshi]
    /// 76 : JMPIF 59 [2 datoshi]
    /// 78 : DUP [2 datoshi]
    /// 79 : PUSHDATA1 46414C5345 'FALSE' [8 datoshi]
    /// 80 : EQUAL [32 datoshi]
    /// 81 : JMPIF 4E [2 datoshi]
    /// 83 : DUP [2 datoshi]
    /// 84 : PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// 8B : EQUAL [32 datoshi]
    /// 8C : JMPIF 43 [2 datoshi]
    /// 8E : DUP [2 datoshi]
    /// 8F : PUSHDATA1 66 'f' [8 datoshi]
    /// 92 : EQUAL [32 datoshi]
    /// 93 : JMPIF 3C [2 datoshi]
    /// 95 : DUP [2 datoshi]
    /// 96 : PUSHDATA1 46 'F' [8 datoshi]
    /// 99 : EQUAL [32 datoshi]
    /// 9A : JMPIF 35 [2 datoshi]
    /// 9C : DUP [2 datoshi]
    /// 9D : PUSHDATA1 30 '0' [8 datoshi]
    /// A0 : EQUAL [32 datoshi]
    /// A1 : JMPIF 2E [2 datoshi]
    /// A3 : DUP [2 datoshi]
    /// A4 : PUSHDATA1 6E6F 'no' [8 datoshi]
    /// A8 : EQUAL [32 datoshi]
    /// A9 : JMPIF 26 [2 datoshi]
    /// AB : DUP [2 datoshi]
    /// AC : PUSHDATA1 4E4F 'NO' [8 datoshi]
    /// B0 : EQUAL [32 datoshi]
    /// B1 : JMPIF 1E [2 datoshi]
    /// B3 : DUP [2 datoshi]
    /// B4 : PUSHDATA1 6E 'n' [8 datoshi]
    /// B7 : EQUAL [32 datoshi]
    /// B8 : JMPIF 17 [2 datoshi]
    /// BA : DUP [2 datoshi]
    /// BB : PUSHDATA1 4E 'N' [8 datoshi]
    /// BE : EQUAL [32 datoshi]
    /// BF : JMPIF 10 [2 datoshi]
    /// C1 : DROP [2 datoshi]
    /// C2 : PUSHF [1 datoshi]
    /// C3 : STSFLD 08 [2 datoshi]
    /// C5 : PUSHF [1 datoshi]
    /// C6 : JMP 0E [2 datoshi]
    /// C8 : DROP [2 datoshi]
    /// C9 : PUSHT [1 datoshi]
    /// CA : STSFLD 08 [2 datoshi]
    /// CC : PUSHT [1 datoshi]
    /// CD : JMP 07 [2 datoshi]
    /// CF : DROP [2 datoshi]
    /// D0 : PUSHF [1 datoshi]
    /// D1 : STSFLD 08 [2 datoshi]
    /// D3 : PUSHT [1 datoshi]
    /// D4 : STLOC0 [2 datoshi]
    /// D5 : LDSFLD 08 [2 datoshi]
    /// D7 : LDLOC0 [2 datoshi]
    /// D8 : PUSH2 [1 datoshi]
    /// D9 : PACKSTRUCT [2048 datoshi]
    /// DA : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcFhoEr9A
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD0 [2 datoshi]
    /// 05 : LDSFLD0 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 0E [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PUSHINT16 0001 [1 datoshi]
    /// 15 : WITHIN [8 datoshi]
    /// 16 : JMPIFNOT 06 [2 datoshi]
    /// 18 : STSFLD0 [2 datoshi]
    /// 19 : PUSHT [1 datoshi]
    /// 1A : JMP 04 [2 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : PUSHF [1 datoshi]
    /// 1E : STLOC0 [2 datoshi]
    /// 1F : LDSFLD0 [2 datoshi]
    /// 20 : LDLOC0 [2 datoshi]
    /// 21 : PUSH2 [1 datoshi]
    /// 22 : PACKSTRUCT [2048 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGRceFBFNwAAStgkGEoCAAAAgAMAAACAAAAAALsmBmQIIgRFCXBcaBK/QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD4 [2 datoshi]
    /// 05 : LDSFLD4 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 18 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 00000080 [1 datoshi]
    /// 16 : PUSHINT64 0000008000000000 [1 datoshi]
    /// 1F : WITHIN [8 datoshi]
    /// 20 : JMPIFNOT 06 [2 datoshi]
    /// 22 : STSFLD4 [2 datoshi]
    /// 23 : PUSHT [1 datoshi]
    /// 24 : JMP 04 [2 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : PUSHF [1 datoshi]
    /// 28 : STLOC0 [2 datoshi]
    /// 29 : LDSFLD4 [2 datoshi]
    /// 2A : LDLOC0 [2 datoshi]
    /// 2B : PUSH2 [1 datoshi]
    /// 2C : PACKSTRUCT [2048 datoshi]
    /// 2D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGZeeFBFNwAAStgkJEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALsmBmYIIgRFCXBeaBK/QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD6 [2 datoshi]
    /// 05 : LDSFLD6 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 24 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT64 0000000000000080 [1 datoshi]
    /// 1A : PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 2B : WITHIN [8 datoshi]
    /// 2C : JMPIFNOT 06 [2 datoshi]
    /// 2E : STSFLD6 [2 datoshi]
    /// 2F : PUSHT [1 datoshi]
    /// 30 : JMP 04 [2 datoshi]
    /// 32 : DROP [2 datoshi]
    /// 33 : PUSHF [1 datoshi]
    /// 34 : STLOC0 [2 datoshi]
    /// 35 : LDSFLD6 [2 datoshi]
    /// 36 : LDLOC0 [2 datoshi]
    /// 37 : PUSH2 [1 datoshi]
    /// 38 : PACKSTRUCT [2048 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXBZaBK/QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD1 [2 datoshi]
    /// 05 : LDSFLD1 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 0F [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT8 80 [1 datoshi]
    /// 13 : PUSHINT16 8000 [1 datoshi]
    /// 16 : WITHIN [8 datoshi]
    /// 17 : JMPIFNOT 06 [2 datoshi]
    /// 19 : STSFLD1 [2 datoshi]
    /// 1A : PUSHT [1 datoshi]
    /// 1B : JMP 04 [2 datoshi]
    /// 1D : DROP [2 datoshi]
    /// 1E : PUSHF [1 datoshi]
    /// 1F : STLOC0 [2 datoshi]
    /// 20 : LDSFLD1 [2 datoshi]
    /// 21 : LDLOC0 [2 datoshi]
    /// 22 : PUSH2 [1 datoshi]
    /// 23 : PACKSTRUCT [2048 datoshi]
    /// 24 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGJaeFBFNwAAStgkEkoBAIACAIAAALsmBmIIIgRFCXBaaBK/QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD2 [2 datoshi]
    /// 05 : LDSFLD2 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 12 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT16 0080 [1 datoshi]
    /// 14 : PUSHINT32 00800000 [1 datoshi]
    /// 19 : WITHIN [8 datoshi]
    /// 1A : JMPIFNOT 06 [2 datoshi]
    /// 1C : STSFLD2 [2 datoshi]
    /// 1D : PUSHT [1 datoshi]
    /// 1E : JMP 04 [2 datoshi]
    /// 20 : DROP [2 datoshi]
    /// 21 : PUSHF [1 datoshi]
    /// 22 : STLOC0 [2 datoshi]
    /// 23 : LDSFLD2 [2 datoshi]
    /// 24 : LDLOC0 [2 datoshi]
    /// 25 : PUSH2 [1 datoshi]
    /// 26 : PACKSTRUCT [2048 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGVdeFBFNwAAStgkFEoQAwAAAAABAAAAuyYGZQgiBEUJcF1oEr9A
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD5 [2 datoshi]
    /// 05 : LDSFLD5 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 14 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 1B : WITHIN [8 datoshi]
    /// 1C : JMPIFNOT 06 [2 datoshi]
    /// 1E : STSFLD5 [2 datoshi]
    /// 1F : PUSHT [1 datoshi]
    /// 20 : JMP 04 [2 datoshi]
    /// 22 : DROP [2 datoshi]
    /// 23 : PUSHF [1 datoshi]
    /// 24 : STLOC0 [2 datoshi]
    /// 25 : LDSFLD5 [2 datoshi]
    /// 26 : LDLOC0 [2 datoshi]
    /// 27 : PUSH2 [1 datoshi]
    /// 28 : PACKSTRUCT [2048 datoshi]
    /// 29 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwXwdoEr9A
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD 07 [2 datoshi]
    /// 06 : LDSFLD 07 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : DROP [2 datoshi]
    /// 0B : CALLT 0000 [32768 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : ISNULL [2 datoshi]
    /// 10 : JMPIF 1D [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 25 : WITHIN [8 datoshi]
    /// 26 : JMPIFNOT 07 [2 datoshi]
    /// 28 : STSFLD 07 [2 datoshi]
    /// 2A : PUSHT [1 datoshi]
    /// 2B : JMP 04 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : PUSHF [1 datoshi]
    /// 2F : STLOC0 [2 datoshi]
    /// 30 : LDSFLD 07 [2 datoshi]
    /// 32 : LDLOC0 [2 datoshi]
    /// 33 : PUSH2 [1 datoshi]
    /// 34 : PACKSTRUCT [2048 datoshi]
    /// 35 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGNbeFBFNwAAStgkEEoQAgAAAQC7JgZjCCIERQlwW2gSv0A=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD3 [2 datoshi]
    /// 05 : LDSFLD3 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : CALLT 0000 [32768 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIF 10 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PUSHINT32 00000100 [1 datoshi]
    /// 17 : WITHIN [8 datoshi]
    /// 18 : JMPIFNOT 06 [2 datoshi]
    /// 1A : STSFLD3 [2 datoshi]
    /// 1B : PUSHT [1 datoshi]
    /// 1C : JMP 04 [2 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : PUSHF [1 datoshi]
    /// 20 : STLOC0 [2 datoshi]
    /// 21 : LDSFLD3 [2 datoshi]
    /// 22 : LDLOC0 [2 datoshi]
    /// 23 : PUSH2 [1 datoshi]
    /// 24 : PACKSTRUCT [2048 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);

    #endregion
}
