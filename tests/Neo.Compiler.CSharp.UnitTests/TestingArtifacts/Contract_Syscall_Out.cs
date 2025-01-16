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
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHF [1 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 74727565 'true' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF_L B5000000 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 54525545 'TRUE' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF_L A8000000 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 54727565 'True' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF_L 9B000000 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 74 't' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF_L 91000000 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 54 'T' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF_L 87000000 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 31 '1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 7D [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 796573 'yes' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 74 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 594553 'YES' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 6B [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 79 'y' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 64 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 59 'Y' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 5D [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 66616C7365 'false' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 59 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 46414C5345 'FALSE' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 4E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 43 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 66 'f' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 3C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 46 'F' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 35 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 30 '0' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 2E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6E6F 'no' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 26 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 4E4F 'NO' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 1E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6E 'n' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 17 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 4E 'N' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 10 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 0E [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 07 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcFhoEr9A
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGRceFBFNwAAStgkGEoCAAAAgAMAAACAAAAAALsmBmQIIgRFCXBcaBK/QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD4 [2 datoshi]
    /// LDSFLD4 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 18 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD4 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGZeeFBFNwAAStgkJEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALsmBmYIIgRFCXBeaBK/QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD6 [2 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 24 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXBZaBK/QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0F [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGJaeFBFNwAAStgkEkoBAIACAIAAALsmBmIIIgRFCXBaaBK/QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD2 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 12 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 0080 [1 datoshi]
    /// PUSHINT32 00800000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGVdeFBFNwAAStgkFEoQAwAAAAABAAAAuyYGZQgiBEUJcF1oEr9A
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD5 [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 14 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD5 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwXwdoEr9A
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// LDSFLD 07 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 1D [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD 07 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGNbeFBFNwAAStgkEEoQAgAAAQC7JgZjCCIERQlwW2gSv0A=
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD3 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 10 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);

    #endregion
}
