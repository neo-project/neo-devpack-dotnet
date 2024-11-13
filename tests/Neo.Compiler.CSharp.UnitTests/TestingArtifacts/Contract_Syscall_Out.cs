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
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSHF [1 datoshi]
    /// 04 : OpCode.STSFLD 08 [2 datoshi]
    /// 06 : OpCode.LDSFLD 08 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.SWAP [2 datoshi]
    /// 0A : OpCode.DROP [2 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 74727565 'true' [8 datoshi]
    /// 12 : OpCode.EQUAL [32 datoshi]
    /// 13 : OpCode.JMPIF_L B5000000 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 54525545 'TRUE' [8 datoshi]
    /// 1F : OpCode.EQUAL [32 datoshi]
    /// 20 : OpCode.JMPIF_L A8000000 [2 datoshi]
    /// 25 : OpCode.DUP [2 datoshi]
    /// 26 : OpCode.PUSHDATA1 54727565 'True' [8 datoshi]
    /// 2C : OpCode.EQUAL [32 datoshi]
    /// 2D : OpCode.JMPIF_L 9B000000 [2 datoshi]
    /// 32 : OpCode.DUP [2 datoshi]
    /// 33 : OpCode.PUSHDATA1 74 't' [8 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.JMPIF_L 91000000 [2 datoshi]
    /// 3C : OpCode.DUP [2 datoshi]
    /// 3D : OpCode.PUSHDATA1 54 'T' [8 datoshi]
    /// 40 : OpCode.EQUAL [32 datoshi]
    /// 41 : OpCode.JMPIF_L 87000000 [2 datoshi]
    /// 46 : OpCode.DUP [2 datoshi]
    /// 47 : OpCode.PUSHDATA1 31 '1' [8 datoshi]
    /// 4A : OpCode.EQUAL [32 datoshi]
    /// 4B : OpCode.JMPIF 7D [2 datoshi]
    /// 4D : OpCode.DUP [2 datoshi]
    /// 4E : OpCode.PUSHDATA1 796573 'yes' [8 datoshi]
    /// 53 : OpCode.EQUAL [32 datoshi]
    /// 54 : OpCode.JMPIF 74 [2 datoshi]
    /// 56 : OpCode.DUP [2 datoshi]
    /// 57 : OpCode.PUSHDATA1 594553 'YES' [8 datoshi]
    /// 5C : OpCode.EQUAL [32 datoshi]
    /// 5D : OpCode.JMPIF 6B [2 datoshi]
    /// 5F : OpCode.DUP [2 datoshi]
    /// 60 : OpCode.PUSHDATA1 79 'y' [8 datoshi]
    /// 63 : OpCode.EQUAL [32 datoshi]
    /// 64 : OpCode.JMPIF 64 [2 datoshi]
    /// 66 : OpCode.DUP [2 datoshi]
    /// 67 : OpCode.PUSHDATA1 59 'Y' [8 datoshi]
    /// 6A : OpCode.EQUAL [32 datoshi]
    /// 6B : OpCode.JMPIF 5D [2 datoshi]
    /// 6D : OpCode.DUP [2 datoshi]
    /// 6E : OpCode.PUSHDATA1 66616C7365 'false' [8 datoshi]
    /// 75 : OpCode.EQUAL [32 datoshi]
    /// 76 : OpCode.JMPIF 59 [2 datoshi]
    /// 78 : OpCode.DUP [2 datoshi]
    /// 79 : OpCode.PUSHDATA1 46414C5345 'FALSE' [8 datoshi]
    /// 80 : OpCode.EQUAL [32 datoshi]
    /// 81 : OpCode.JMPIF 4E [2 datoshi]
    /// 83 : OpCode.DUP [2 datoshi]
    /// 84 : OpCode.PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// 8B : OpCode.EQUAL [32 datoshi]
    /// 8C : OpCode.JMPIF 43 [2 datoshi]
    /// 8E : OpCode.DUP [2 datoshi]
    /// 8F : OpCode.PUSHDATA1 66 'f' [8 datoshi]
    /// 92 : OpCode.EQUAL [32 datoshi]
    /// 93 : OpCode.JMPIF 3C [2 datoshi]
    /// 95 : OpCode.DUP [2 datoshi]
    /// 96 : OpCode.PUSHDATA1 46 'F' [8 datoshi]
    /// 99 : OpCode.EQUAL [32 datoshi]
    /// 9A : OpCode.JMPIF 35 [2 datoshi]
    /// 9C : OpCode.DUP [2 datoshi]
    /// 9D : OpCode.PUSHDATA1 30 '0' [8 datoshi]
    /// A0 : OpCode.EQUAL [32 datoshi]
    /// A1 : OpCode.JMPIF 2E [2 datoshi]
    /// A3 : OpCode.DUP [2 datoshi]
    /// A4 : OpCode.PUSHDATA1 6E6F 'no' [8 datoshi]
    /// A8 : OpCode.EQUAL [32 datoshi]
    /// A9 : OpCode.JMPIF 26 [2 datoshi]
    /// AB : OpCode.DUP [2 datoshi]
    /// AC : OpCode.PUSHDATA1 4E4F 'NO' [8 datoshi]
    /// B0 : OpCode.EQUAL [32 datoshi]
    /// B1 : OpCode.JMPIF 1E [2 datoshi]
    /// B3 : OpCode.DUP [2 datoshi]
    /// B4 : OpCode.PUSHDATA1 6E 'n' [8 datoshi]
    /// B7 : OpCode.EQUAL [32 datoshi]
    /// B8 : OpCode.JMPIF 17 [2 datoshi]
    /// BA : OpCode.DUP [2 datoshi]
    /// BB : OpCode.PUSHDATA1 4E 'N' [8 datoshi]
    /// BE : OpCode.EQUAL [32 datoshi]
    /// BF : OpCode.JMPIF 10 [2 datoshi]
    /// C1 : OpCode.DROP [2 datoshi]
    /// C2 : OpCode.PUSHF [1 datoshi]
    /// C3 : OpCode.STSFLD 08 [2 datoshi]
    /// C5 : OpCode.PUSHF [1 datoshi]
    /// C6 : OpCode.JMP 0E [2 datoshi]
    /// C8 : OpCode.DROP [2 datoshi]
    /// C9 : OpCode.PUSHT [1 datoshi]
    /// CA : OpCode.STSFLD 08 [2 datoshi]
    /// CC : OpCode.PUSHT [1 datoshi]
    /// CD : OpCode.JMP 07 [2 datoshi]
    /// CF : OpCode.DROP [2 datoshi]
    /// D0 : OpCode.PUSHF [1 datoshi]
    /// D1 : OpCode.STSFLD 08 [2 datoshi]
    /// D3 : OpCode.PUSHT [1 datoshi]
    /// D4 : OpCode.STLOC0 [2 datoshi]
    /// D5 : OpCode.LDSFLD 08 [2 datoshi]
    /// D7 : OpCode.LDLOC0 [2 datoshi]
    /// D8 : OpCode.PUSH2 [1 datoshi]
    /// D9 : OpCode.PACKSTRUCT [2048 datoshi]
    /// DA : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcFhoEr9A
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD0 [2 datoshi]
    /// 05 : OpCode.LDSFLD0 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 0E [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.PUSHINT16 0001 [1 datoshi]
    /// 15 : OpCode.WITHIN [8 datoshi]
    /// 16 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 18 : OpCode.STSFLD0 [2 datoshi]
    /// 19 : OpCode.PUSHT [1 datoshi]
    /// 1A : OpCode.JMP 04 [2 datoshi]
    /// 1C : OpCode.DROP [2 datoshi]
    /// 1D : OpCode.PUSHF [1 datoshi]
    /// 1E : OpCode.STLOC0 [2 datoshi]
    /// 1F : OpCode.LDSFLD0 [2 datoshi]
    /// 20 : OpCode.LDLOC0 [2 datoshi]
    /// 21 : OpCode.PUSH2 [1 datoshi]
    /// 22 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGRceFBFNwAAStgkGEoCAAAAgAMAAACAAAAAALsmBmQIIgRFCXBcaBK/QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD4 [2 datoshi]
    /// 05 : OpCode.LDSFLD4 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 18 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 16 : OpCode.PUSHINT64 0000008000000000 [1 datoshi]
    /// 1F : OpCode.WITHIN [8 datoshi]
    /// 20 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 22 : OpCode.STSFLD4 [2 datoshi]
    /// 23 : OpCode.PUSHT [1 datoshi]
    /// 24 : OpCode.JMP 04 [2 datoshi]
    /// 26 : OpCode.DROP [2 datoshi]
    /// 27 : OpCode.PUSHF [1 datoshi]
    /// 28 : OpCode.STLOC0 [2 datoshi]
    /// 29 : OpCode.LDSFLD4 [2 datoshi]
    /// 2A : OpCode.LDLOC0 [2 datoshi]
    /// 2B : OpCode.PUSH2 [1 datoshi]
    /// 2C : OpCode.PACKSTRUCT [2048 datoshi]
    /// 2D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGZeeFBFNwAAStgkJEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALsmBmYIIgRFCXBeaBK/QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD6 [2 datoshi]
    /// 05 : OpCode.LDSFLD6 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 24 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSHINT64 0000000000000080 [1 datoshi]
    /// 1A : OpCode.PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 2B : OpCode.WITHIN [8 datoshi]
    /// 2C : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 2E : OpCode.STSFLD6 [2 datoshi]
    /// 2F : OpCode.PUSHT [1 datoshi]
    /// 30 : OpCode.JMP 04 [2 datoshi]
    /// 32 : OpCode.DROP [2 datoshi]
    /// 33 : OpCode.PUSHF [1 datoshi]
    /// 34 : OpCode.STLOC0 [2 datoshi]
    /// 35 : OpCode.LDSFLD6 [2 datoshi]
    /// 36 : OpCode.LDLOC0 [2 datoshi]
    /// 37 : OpCode.PUSH2 [1 datoshi]
    /// 38 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXBZaBK/QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD1 [2 datoshi]
    /// 05 : OpCode.LDSFLD1 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 0F [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSHINT8 80 [1 datoshi]
    /// 13 : OpCode.PUSHINT16 8000 [1 datoshi]
    /// 16 : OpCode.WITHIN [8 datoshi]
    /// 17 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 19 : OpCode.STSFLD1 [2 datoshi]
    /// 1A : OpCode.PUSHT [1 datoshi]
    /// 1B : OpCode.JMP 04 [2 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.PUSHF [1 datoshi]
    /// 1F : OpCode.STLOC0 [2 datoshi]
    /// 20 : OpCode.LDSFLD1 [2 datoshi]
    /// 21 : OpCode.LDLOC0 [2 datoshi]
    /// 22 : OpCode.PUSH2 [1 datoshi]
    /// 23 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 24 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGJaeFBFNwAAStgkEkoBAIACAIAAALsmBmIIIgRFCXBaaBK/QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD2 [2 datoshi]
    /// 05 : OpCode.LDSFLD2 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 12 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSHINT16 0080 [1 datoshi]
    /// 14 : OpCode.PUSHINT32 00800000 [1 datoshi]
    /// 19 : OpCode.WITHIN [8 datoshi]
    /// 1A : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 1C : OpCode.STSFLD2 [2 datoshi]
    /// 1D : OpCode.PUSHT [1 datoshi]
    /// 1E : OpCode.JMP 04 [2 datoshi]
    /// 20 : OpCode.DROP [2 datoshi]
    /// 21 : OpCode.PUSHF [1 datoshi]
    /// 22 : OpCode.STLOC0 [2 datoshi]
    /// 23 : OpCode.LDSFLD2 [2 datoshi]
    /// 24 : OpCode.LDLOC0 [2 datoshi]
    /// 25 : OpCode.PUSH2 [1 datoshi]
    /// 26 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 27 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGVdeFBFNwAAStgkFEoQAwAAAAABAAAAuyYGZQgiBEUJcF1oEr9A
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD5 [2 datoshi]
    /// 05 : OpCode.LDSFLD5 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 14 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 1B : OpCode.WITHIN [8 datoshi]
    /// 1C : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 1E : OpCode.STSFLD5 [2 datoshi]
    /// 1F : OpCode.PUSHT [1 datoshi]
    /// 20 : OpCode.JMP 04 [2 datoshi]
    /// 22 : OpCode.DROP [2 datoshi]
    /// 23 : OpCode.PUSHF [1 datoshi]
    /// 24 : OpCode.STLOC0 [2 datoshi]
    /// 25 : OpCode.LDSFLD5 [2 datoshi]
    /// 26 : OpCode.LDLOC0 [2 datoshi]
    /// 27 : OpCode.PUSH2 [1 datoshi]
    /// 28 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 29 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwXwdoEr9A
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD 07 [2 datoshi]
    /// 06 : OpCode.LDSFLD 07 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.SWAP [2 datoshi]
    /// 0A : OpCode.DROP [2 datoshi]
    /// 0B : OpCode.CALLT 0000 [32768 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.ISNULL [2 datoshi]
    /// 10 : OpCode.JMPIF 1D [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 25 : OpCode.WITHIN [8 datoshi]
    /// 26 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 28 : OpCode.STSFLD 07 [2 datoshi]
    /// 2A : OpCode.PUSHT [1 datoshi]
    /// 2B : OpCode.JMP 04 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.PUSHF [1 datoshi]
    /// 2F : OpCode.STLOC0 [2 datoshi]
    /// 30 : OpCode.LDSFLD 07 [2 datoshi]
    /// 32 : OpCode.LDLOC0 [2 datoshi]
    /// 33 : OpCode.PUSH2 [1 datoshi]
    /// 34 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGNbeFBFNwAAStgkEEoQAgAAAQC7JgZjCCIERQlwW2gSv0A=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD3 [2 datoshi]
    /// 05 : OpCode.LDSFLD3 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIF 10 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.PUSHINT32 00000100 [1 datoshi]
    /// 17 : OpCode.WITHIN [8 datoshi]
    /// 18 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 1A : OpCode.STSFLD3 [2 datoshi]
    /// 1B : OpCode.PUSHT [1 datoshi]
    /// 1C : OpCode.JMP 04 [2 datoshi]
    /// 1E : OpCode.DROP [2 datoshi]
    /// 1F : OpCode.PUSHF [1 datoshi]
    /// 20 : OpCode.STLOC0 [2 datoshi]
    /// 21 : OpCode.LDSFLD3 [2 datoshi]
    /// 22 : OpCode.LDLOC0 [2 datoshi]
    /// 23 : OpCode.PUSH2 [1 datoshi]
    /// 24 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);

    #endregion
}
