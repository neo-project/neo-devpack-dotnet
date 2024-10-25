using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Switch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Switch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""switchLong"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""switch6"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":233,""safe"":false},{""name"":""switch6Inline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":297,""safe"":false},{""name"":""switchInteger"",""parameters"":[{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":364,""safe"":false},{""name"":""switchLongLong"",""parameters"":[{""name"":""test"",""type"":""String""}],""returntype"":""Any"",""offset"":404,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0AAlcBAXhwaAwBMJclrQAAAGgMATGXJaUAAABoDAEylyWdAAAAaAwBM5cllQAAAGgMATSXJY0AAABoDAE1lyWFAAAAaAwBNpckfWgMATeXJHhoDAE4lyRzaAwBOZckbmgMAjEwlyRoaAwCMTGXJGJoDAIxMpckXGgMAjEzlyRWaAwCMTSXJFBoDAIxNZckSmgMAjE2lyREaAwCMTeXJD9oDAIxOJckOmgMAjE5lyQ1aAwCMjCXJDAiMRFAEkATQBRAFUAWQBdAGEAZQBpAG0AcQB1AHkAfQCBAABFAABJAABNAABRAABVAAGNAVwEBeHBoDAEwlyQnaAwBMZckImgMATKXJB1oDAEzlyQYaAwBNJckE2gMATWXJA4iDhFAEkATQBRAFUAWQABjQFcBAXhwaAwBMJcmBBFAaAwBMZcmBBJAaAwBMpcmBBNAaAwBM5cmBBRAaAwBNJcmBBVAaAwBNZcmBBZACCYFAGNAaDpXAgERcHhxaRGXJA5pEpckDWkTlyQMIg4ScCIME3AiCBZwIgQQcGhAVwIBEXB4cWkMAWGXJC5pDAFjlyQuaQwBYpckLWkMAWSXJC1pDAFllyQsaQwBZpckK2kMAWeXJCoiLmhKnHBFIitoEqBwIiVoSp1wRSIeaA+gcCIYaGigcCISaBOgcCIMaBKecCIGaBGhcGhADierhw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJCdoDDGXJCJoDDKXJB1oDDOXJBhoDDSXJBNoDDWXJA4iDhFAEkATQBRAFUAWQABjQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHDATA1 30
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIF 27
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.PUSHDATA1 31
    /// 10 : OpCode.EQUAL
    /// 11 : OpCode.JMPIF 22
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSHDATA1 32
    /// 17 : OpCode.EQUAL
    /// 18 : OpCode.JMPIF 1D
    /// 1A : OpCode.LDLOC0
    /// 1B : OpCode.PUSHDATA1 33
    /// 1E : OpCode.EQUAL
    /// 1F : OpCode.JMPIF 18
    /// 21 : OpCode.LDLOC0
    /// 22 : OpCode.PUSHDATA1 34
    /// 25 : OpCode.EQUAL
    /// 26 : OpCode.JMPIF 13
    /// 28 : OpCode.LDLOC0
    /// 29 : OpCode.PUSHDATA1 35
    /// 2C : OpCode.EQUAL
    /// 2D : OpCode.JMPIF 0E
    /// 2F : OpCode.JMP 0E
    /// 31 : OpCode.PUSH1
    /// 32 : OpCode.RET
    /// 33 : OpCode.PUSH2
    /// 34 : OpCode.RET
    /// 35 : OpCode.PUSH3
    /// 36 : OpCode.RET
    /// 37 : OpCode.PUSH4
    /// 38 : OpCode.RET
    /// 39 : OpCode.PUSH5
    /// 3A : OpCode.RET
    /// 3B : OpCode.PUSH6
    /// 3C : OpCode.RET
    /// 3D : OpCode.PUSHINT8 63
    /// 3F : OpCode.RET
    /// </remarks>
    [DisplayName("switch6")]
    public abstract object? Switch6(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJgQRQGgMMZcmBBJAaAwylyYEE0BoDDOXJgQUQGgMNJcmBBVAaAw1lyYEFkAIJgUAY0BoOg==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHDATA1 30
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIFNOT 04
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.RET
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.PUSHDATA1 31
    /// 12 : OpCode.EQUAL
    /// 13 : OpCode.JMPIFNOT 04
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.RET
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.PUSHDATA1 32
    /// 1B : OpCode.EQUAL
    /// 1C : OpCode.JMPIFNOT 04
    /// 1E : OpCode.PUSH3
    /// 1F : OpCode.RET
    /// 20 : OpCode.LDLOC0
    /// 21 : OpCode.PUSHDATA1 33
    /// 24 : OpCode.EQUAL
    /// 25 : OpCode.JMPIFNOT 04
    /// 27 : OpCode.PUSH4
    /// 28 : OpCode.RET
    /// 29 : OpCode.LDLOC0
    /// 2A : OpCode.PUSHDATA1 34
    /// 2D : OpCode.EQUAL
    /// 2E : OpCode.JMPIFNOT 04
    /// 30 : OpCode.PUSH5
    /// 31 : OpCode.RET
    /// 32 : OpCode.LDLOC0
    /// 33 : OpCode.PUSHDATA1 35
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.JMPIFNOT 04
    /// 39 : OpCode.PUSH6
    /// 3A : OpCode.RET
    /// 3B : OpCode.PUSHT
    /// 3C : OpCode.JMPIFNOT 05
    /// 3E : OpCode.PUSHINT8 63
    /// 40 : OpCode.RET
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.THROW
    /// </remarks>
    [DisplayName("switch6Inline")]
    public abstract object? Switch6Inline(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEXB4cWkRlyQOaRKXJA1pE5ckDCIOEnAiDBNwIggWcCIEEHBoQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.LDLOC1
    /// 08 : OpCode.PUSH1
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIF 0E
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.PUSH2
    /// 0E : OpCode.EQUAL
    /// 0F : OpCode.JMPIF 0D
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.PUSH3
    /// 13 : OpCode.EQUAL
    /// 14 : OpCode.JMPIF 0C
    /// 16 : OpCode.JMP 0E
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.STLOC0
    /// 1A : OpCode.JMP 0C
    /// 1C : OpCode.PUSH3
    /// 1D : OpCode.STLOC0
    /// 1E : OpCode.JMP 08
    /// 20 : OpCode.PUSH6
    /// 21 : OpCode.STLOC0
    /// 22 : OpCode.JMP 04
    /// 24 : OpCode.PUSH0
    /// 25 : OpCode.STLOC0
    /// 26 : OpCode.LDLOC0
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("switchInteger")]
    public abstract object? SwitchInteger(BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJa0AAABoDDGXJaUAAABoDDKXJZ0AAABoDDOXJZUAAABoDDSXJY0AAABoDDWXJYUAAABoDDaXJH1oDDeXJHhoDDiXJHNoDDmXJG5oDDEwlyRoaAwxMZckYmgMMTKXJFxoDDEzlyRWaAwxNJckUGgMMTWXJEpoDDE2lyREaAwxN5ckP2gMMTiXJDpoDDE5lyQ1aAwyMJckMCIxEUASQBNAFEAVQBZAF0AYQBlAGkAbQBxAHUAeQB9AIEAAEUAAEkAAE0AAFEAAFUAAY0A=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHDATA1 30
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIF_L AD000000
    /// 0F : OpCode.LDLOC0
    /// 10 : OpCode.PUSHDATA1 31
    /// 13 : OpCode.EQUAL
    /// 14 : OpCode.JMPIF_L A5000000
    /// 19 : OpCode.LDLOC0
    /// 1A : OpCode.PUSHDATA1 32
    /// 1D : OpCode.EQUAL
    /// 1E : OpCode.JMPIF_L 9D000000
    /// 23 : OpCode.LDLOC0
    /// 24 : OpCode.PUSHDATA1 33
    /// 27 : OpCode.EQUAL
    /// 28 : OpCode.JMPIF_L 95000000
    /// 2D : OpCode.LDLOC0
    /// 2E : OpCode.PUSHDATA1 34
    /// 31 : OpCode.EQUAL
    /// 32 : OpCode.JMPIF_L 8D000000
    /// 37 : OpCode.LDLOC0
    /// 38 : OpCode.PUSHDATA1 35
    /// 3B : OpCode.EQUAL
    /// 3C : OpCode.JMPIF_L 85000000
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.PUSHDATA1 36
    /// 45 : OpCode.EQUAL
    /// 46 : OpCode.JMPIF 7D
    /// 48 : OpCode.LDLOC0
    /// 49 : OpCode.PUSHDATA1 37
    /// 4C : OpCode.EQUAL
    /// 4D : OpCode.JMPIF 78
    /// 4F : OpCode.LDLOC0
    /// 50 : OpCode.PUSHDATA1 38
    /// 53 : OpCode.EQUAL
    /// 54 : OpCode.JMPIF 73
    /// 56 : OpCode.LDLOC0
    /// 57 : OpCode.PUSHDATA1 39
    /// 5A : OpCode.EQUAL
    /// 5B : OpCode.JMPIF 6E
    /// 5D : OpCode.LDLOC0
    /// 5E : OpCode.PUSHDATA1 3130
    /// 62 : OpCode.EQUAL
    /// 63 : OpCode.JMPIF 68
    /// 65 : OpCode.LDLOC0
    /// 66 : OpCode.PUSHDATA1 3131
    /// 6A : OpCode.EQUAL
    /// 6B : OpCode.JMPIF 62
    /// 6D : OpCode.LDLOC0
    /// 6E : OpCode.PUSHDATA1 3132
    /// 72 : OpCode.EQUAL
    /// 73 : OpCode.JMPIF 5C
    /// 75 : OpCode.LDLOC0
    /// 76 : OpCode.PUSHDATA1 3133
    /// 7A : OpCode.EQUAL
    /// 7B : OpCode.JMPIF 56
    /// 7D : OpCode.LDLOC0
    /// 7E : OpCode.PUSHDATA1 3134
    /// 82 : OpCode.EQUAL
    /// 83 : OpCode.JMPIF 50
    /// 85 : OpCode.LDLOC0
    /// 86 : OpCode.PUSHDATA1 3135
    /// 8A : OpCode.EQUAL
    /// 8B : OpCode.JMPIF 4A
    /// 8D : OpCode.LDLOC0
    /// 8E : OpCode.PUSHDATA1 3136
    /// 92 : OpCode.EQUAL
    /// 93 : OpCode.JMPIF 44
    /// 95 : OpCode.LDLOC0
    /// 96 : OpCode.PUSHDATA1 3137
    /// 9A : OpCode.EQUAL
    /// 9B : OpCode.JMPIF 3F
    /// 9D : OpCode.LDLOC0
    /// 9E : OpCode.PUSHDATA1 3138
    /// A2 : OpCode.EQUAL
    /// A3 : OpCode.JMPIF 3A
    /// A5 : OpCode.LDLOC0
    /// A6 : OpCode.PUSHDATA1 3139
    /// AA : OpCode.EQUAL
    /// AB : OpCode.JMPIF 35
    /// AD : OpCode.LDLOC0
    /// AE : OpCode.PUSHDATA1 3230
    /// B2 : OpCode.EQUAL
    /// B3 : OpCode.JMPIF 30
    /// B5 : OpCode.JMP 31
    /// B7 : OpCode.PUSH1
    /// B8 : OpCode.RET
    /// B9 : OpCode.PUSH2
    /// BA : OpCode.RET
    /// BB : OpCode.PUSH3
    /// BC : OpCode.RET
    /// BD : OpCode.PUSH4
    /// BE : OpCode.RET
    /// BF : OpCode.PUSH5
    /// C0 : OpCode.RET
    /// C1 : OpCode.PUSH6
    /// C2 : OpCode.RET
    /// C3 : OpCode.PUSH7
    /// C4 : OpCode.RET
    /// C5 : OpCode.PUSH8
    /// C6 : OpCode.RET
    /// C7 : OpCode.PUSH9
    /// C8 : OpCode.RET
    /// C9 : OpCode.PUSH10
    /// CA : OpCode.RET
    /// CB : OpCode.PUSH11
    /// CC : OpCode.RET
    /// CD : OpCode.PUSH12
    /// CE : OpCode.RET
    /// CF : OpCode.PUSH13
    /// D0 : OpCode.RET
    /// D1 : OpCode.PUSH14
    /// D2 : OpCode.RET
    /// D3 : OpCode.PUSH15
    /// D4 : OpCode.RET
    /// D5 : OpCode.PUSH16
    /// D6 : OpCode.RET
    /// D7 : OpCode.PUSHINT8 11
    /// D9 : OpCode.RET
    /// DA : OpCode.PUSHINT8 12
    /// DC : OpCode.RET
    /// DD : OpCode.PUSHINT8 13
    /// DF : OpCode.RET
    /// E0 : OpCode.PUSHINT8 14
    /// E2 : OpCode.RET
    /// E3 : OpCode.PUSHINT8 15
    /// E5 : OpCode.RET
    /// E6 : OpCode.PUSHINT8 63
    /// E8 : OpCode.RET
    /// </remarks>
    [DisplayName("switchLong")]
    public abstract object? SwitchLong(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEXB4cWkMYZckLmkMY5ckLmkMYpckLWkMZJckLWkMZZckLGkMZpckK2kMZ5ckKiIuaEqccEUiK2gSoHAiJWhKnXBFIh5oD6BwIhhoaKBwIhJoE6BwIgxoEp5wIgZoEaFwaEA=
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.LDLOC1
    /// 08 : OpCode.PUSHDATA1 61
    /// 0B : OpCode.EQUAL
    /// 0C : OpCode.JMPIF 2E
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.PUSHDATA1 63
    /// 12 : OpCode.EQUAL
    /// 13 : OpCode.JMPIF 2E
    /// 15 : OpCode.LDLOC1
    /// 16 : OpCode.PUSHDATA1 62
    /// 19 : OpCode.EQUAL
    /// 1A : OpCode.JMPIF 2D
    /// 1C : OpCode.LDLOC1
    /// 1D : OpCode.PUSHDATA1 64
    /// 20 : OpCode.EQUAL
    /// 21 : OpCode.JMPIF 2D
    /// 23 : OpCode.LDLOC1
    /// 24 : OpCode.PUSHDATA1 65
    /// 27 : OpCode.EQUAL
    /// 28 : OpCode.JMPIF 2C
    /// 2A : OpCode.LDLOC1
    /// 2B : OpCode.PUSHDATA1 66
    /// 2E : OpCode.EQUAL
    /// 2F : OpCode.JMPIF 2B
    /// 31 : OpCode.LDLOC1
    /// 32 : OpCode.PUSHDATA1 67
    /// 35 : OpCode.EQUAL
    /// 36 : OpCode.JMPIF 2A
    /// 38 : OpCode.JMP 2E
    /// 3A : OpCode.LDLOC0
    /// 3B : OpCode.DUP
    /// 3C : OpCode.INC
    /// 3D : OpCode.STLOC0
    /// 3E : OpCode.DROP
    /// 3F : OpCode.JMP 2B
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.PUSH2
    /// 43 : OpCode.MUL
    /// 44 : OpCode.STLOC0
    /// 45 : OpCode.JMP 25
    /// 47 : OpCode.LDLOC0
    /// 48 : OpCode.DUP
    /// 49 : OpCode.DEC
    /// 4A : OpCode.STLOC0
    /// 4B : OpCode.DROP
    /// 4C : OpCode.JMP 1E
    /// 4E : OpCode.LDLOC0
    /// 4F : OpCode.PUSHM1
    /// 50 : OpCode.MUL
    /// 51 : OpCode.STLOC0
    /// 52 : OpCode.JMP 18
    /// 54 : OpCode.LDLOC0
    /// 55 : OpCode.LDLOC0
    /// 56 : OpCode.MUL
    /// 57 : OpCode.STLOC0
    /// 58 : OpCode.JMP 12
    /// 5A : OpCode.LDLOC0
    /// 5B : OpCode.PUSH3
    /// 5C : OpCode.MUL
    /// 5D : OpCode.STLOC0
    /// 5E : OpCode.JMP 0C
    /// 60 : OpCode.LDLOC0
    /// 61 : OpCode.PUSH2
    /// 62 : OpCode.ADD
    /// 63 : OpCode.STLOC0
    /// 64 : OpCode.JMP 06
    /// 66 : OpCode.LDLOC0
    /// 67 : OpCode.PUSH1
    /// 68 : OpCode.DIV
    /// 69 : OpCode.STLOC0
    /// 6A : OpCode.LDLOC0
    /// 6B : OpCode.RET
    /// </remarks>
    [DisplayName("switchLongLong")]
    public abstract object? SwitchLongLong(string? test);

    #endregion
}
