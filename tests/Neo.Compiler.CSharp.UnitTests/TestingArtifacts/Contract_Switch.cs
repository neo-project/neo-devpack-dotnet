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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1gA1cBAXhwaAwBMJclrQAAAGgMATGXJaUAAABoDAEylyWdAAAAaAwBM5cllQAAAGgMATSXJY0AAABoDAE1lyWFAAAAaAwBNpckfWgMATeXJHhoDAE4lyRzaAwBOZckbmgMAjEwlyRoaAwCMTGXJGJoDAIxMpckXGgMAjEzlyRWaAwCMTSXJFBoDAIxNZckSmgMAjE2lyREaAwCMTeXJD9oDAIxOJckOmgMAjE5lyQ1aAwCMjCXJDAiMRFAEkATQBRAFUAWQBdAGEAZQBpAG0AcQB1AHkAfQCBAABFAABJAABNAABRAABVAAGNAVwEBeHBoDAEwlyQnaAwBMZckImgMATKXJB1oDAEzlyQYaAwBNJckE2gMATWXJA4iDhFAEkATQBRAFUAWQABjQFcBAXhwaAwBMJcmBBFAaAwBMZcmBBJAaAwBMpcmBBNAaAwBM5cmBBRAaAwBNJcmBBVAaAwBNZcmBBZACCYFAGNAaDpXAgERcHhxaRGXJA5pEpckDWkTlyQMIg4ScCIME3AiCBZwIgQQcGhAVwIBEXB4cWkMAWGXJEBpDAFjlyRxaQwBYpcloQAAAGkMAWSXJc8AAABpDAFllyX8AAAAaQwBZpclJgEAAGkMAWeXJVABAAAjfwEAAGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSNLAQAAaBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AjFAEAAGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSPcAAAAaA+gSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AjpQAAAGhooEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wIm5oE6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCI6aBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AiBmgRoXBoQH2/0+M="));

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
    /// Script: VwIBEXB4cWkMYZckQGkMY5ckcWkMYpcloQAAAGkMZJclzwAAAGkMZZcl/AAAAGkMZpclJgEAAGkMZ5clUAEAACN/AQAAaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFI0sBAABoEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCMUAQAAaEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFI9wAAABoD6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCOlAAAAaGigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AibmgToEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wIjpoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCIGaBGhcGhA
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSH1
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.LDLOC1
    /// 0008 : OpCode.PUSHDATA1 61
    /// 000B : OpCode.EQUAL
    /// 000C : OpCode.JMPIF 40
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.PUSHDATA1 63
    /// 0012 : OpCode.EQUAL
    /// 0013 : OpCode.JMPIF 71
    /// 0015 : OpCode.LDLOC1
    /// 0016 : OpCode.PUSHDATA1 62
    /// 0019 : OpCode.EQUAL
    /// 001A : OpCode.JMPIF_L A1000000
    /// 001F : OpCode.LDLOC1
    /// 0020 : OpCode.PUSHDATA1 64
    /// 0023 : OpCode.EQUAL
    /// 0024 : OpCode.JMPIF_L CF000000
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.PUSHDATA1 65
    /// 002D : OpCode.EQUAL
    /// 002E : OpCode.JMPIF_L FC000000
    /// 0033 : OpCode.LDLOC1
    /// 0034 : OpCode.PUSHDATA1 66
    /// 0037 : OpCode.EQUAL
    /// 0038 : OpCode.JMPIF_L 26010000
    /// 003D : OpCode.LDLOC1
    /// 003E : OpCode.PUSHDATA1 67
    /// 0041 : OpCode.EQUAL
    /// 0042 : OpCode.JMPIF_L 50010000
    /// 0047 : OpCode.JMP_L 7F010000
    /// 004C : OpCode.LDLOC0
    /// 004D : OpCode.DUP
    /// 004E : OpCode.INC
    /// 004F : OpCode.DUP
    /// 0050 : OpCode.PUSHINT32 00000080
    /// 0055 : OpCode.JMPGE 04
    /// 0057 : OpCode.JMP 0A
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.PUSHINT32 FFFFFF7F
    /// 005F : OpCode.JMPLE 1E
    /// 0061 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 006A : OpCode.AND
    /// 006B : OpCode.DUP
    /// 006C : OpCode.PUSHINT32 FFFFFF7F
    /// 0071 : OpCode.JMPLE 0C
    /// 0073 : OpCode.PUSHINT64 0000000001000000
    /// 007C : OpCode.SUB
    /// 007D : OpCode.STLOC0
    /// 007E : OpCode.DROP
    /// 007F : OpCode.JMP_L 4B010000
    /// 0084 : OpCode.LDLOC0
    /// 0085 : OpCode.PUSH2
    /// 0086 : OpCode.MUL
    /// 0087 : OpCode.DUP
    /// 0088 : OpCode.PUSHINT32 00000080
    /// 008D : OpCode.JMPGE 04
    /// 008F : OpCode.JMP 0A
    /// 0091 : OpCode.DUP
    /// 0092 : OpCode.PUSHINT32 FFFFFF7F
    /// 0097 : OpCode.JMPLE 1E
    /// 0099 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 00A2 : OpCode.AND
    /// 00A3 : OpCode.DUP
    /// 00A4 : OpCode.PUSHINT32 FFFFFF7F
    /// 00A9 : OpCode.JMPLE 0C
    /// 00AB : OpCode.PUSHINT64 0000000001000000
    /// 00B4 : OpCode.SUB
    /// 00B5 : OpCode.STLOC0
    /// 00B6 : OpCode.JMP_L 14010000
    /// 00BB : OpCode.LDLOC0
    /// 00BC : OpCode.DUP
    /// 00BD : OpCode.DEC
    /// 00BE : OpCode.DUP
    /// 00BF : OpCode.PUSHINT32 00000080
    /// 00C4 : OpCode.JMPGE 04
    /// 00C6 : OpCode.JMP 0A
    /// 00C8 : OpCode.DUP
    /// 00C9 : OpCode.PUSHINT32 FFFFFF7F
    /// 00CE : OpCode.JMPLE 1E
    /// 00D0 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 00D9 : OpCode.AND
    /// 00DA : OpCode.DUP
    /// 00DB : OpCode.PUSHINT32 FFFFFF7F
    /// 00E0 : OpCode.JMPLE 0C
    /// 00E2 : OpCode.PUSHINT64 0000000001000000
    /// 00EB : OpCode.SUB
    /// 00EC : OpCode.STLOC0
    /// 00ED : OpCode.DROP
    /// 00EE : OpCode.JMP_L DC000000
    /// 00F3 : OpCode.LDLOC0
    /// 00F4 : OpCode.PUSHM1
    /// 00F5 : OpCode.MUL
    /// 00F6 : OpCode.DUP
    /// 00F7 : OpCode.PUSHINT32 00000080
    /// 00FC : OpCode.JMPGE 04
    /// 00FE : OpCode.JMP 0A
    /// 0100 : OpCode.DUP
    /// 0101 : OpCode.PUSHINT32 FFFFFF7F
    /// 0106 : OpCode.JMPLE 1E
    /// 0108 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0111 : OpCode.AND
    /// 0112 : OpCode.DUP
    /// 0113 : OpCode.PUSHINT32 FFFFFF7F
    /// 0118 : OpCode.JMPLE 0C
    /// 011A : OpCode.PUSHINT64 0000000001000000
    /// 0123 : OpCode.SUB
    /// 0124 : OpCode.STLOC0
    /// 0125 : OpCode.JMP_L A5000000
    /// 012A : OpCode.LDLOC0
    /// 012B : OpCode.LDLOC0
    /// 012C : OpCode.MUL
    /// 012D : OpCode.DUP
    /// 012E : OpCode.PUSHINT32 00000080
    /// 0133 : OpCode.JMPGE 04
    /// 0135 : OpCode.JMP 0A
    /// 0137 : OpCode.DUP
    /// 0138 : OpCode.PUSHINT32 FFFFFF7F
    /// 013D : OpCode.JMPLE 1E
    /// 013F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0148 : OpCode.AND
    /// 0149 : OpCode.DUP
    /// 014A : OpCode.PUSHINT32 FFFFFF7F
    /// 014F : OpCode.JMPLE 0C
    /// 0151 : OpCode.PUSHINT64 0000000001000000
    /// 015A : OpCode.SUB
    /// 015B : OpCode.STLOC0
    /// 015C : OpCode.JMP 6E
    /// 015E : OpCode.LDLOC0
    /// 015F : OpCode.PUSH3
    /// 0160 : OpCode.MUL
    /// 0161 : OpCode.DUP
    /// 0162 : OpCode.PUSHINT32 00000080
    /// 0167 : OpCode.JMPGE 04
    /// 0169 : OpCode.JMP 0A
    /// 016B : OpCode.DUP
    /// 016C : OpCode.PUSHINT32 FFFFFF7F
    /// 0171 : OpCode.JMPLE 1E
    /// 0173 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 017C : OpCode.AND
    /// 017D : OpCode.DUP
    /// 017E : OpCode.PUSHINT32 FFFFFF7F
    /// 0183 : OpCode.JMPLE 0C
    /// 0185 : OpCode.PUSHINT64 0000000001000000
    /// 018E : OpCode.SUB
    /// 018F : OpCode.STLOC0
    /// 0190 : OpCode.JMP 3A
    /// 0192 : OpCode.LDLOC0
    /// 0193 : OpCode.PUSH2
    /// 0194 : OpCode.ADD
    /// 0195 : OpCode.DUP
    /// 0196 : OpCode.PUSHINT32 00000080
    /// 019B : OpCode.JMPGE 04
    /// 019D : OpCode.JMP 0A
    /// 019F : OpCode.DUP
    /// 01A0 : OpCode.PUSHINT32 FFFFFF7F
    /// 01A5 : OpCode.JMPLE 1E
    /// 01A7 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 01B0 : OpCode.AND
    /// 01B1 : OpCode.DUP
    /// 01B2 : OpCode.PUSHINT32 FFFFFF7F
    /// 01B7 : OpCode.JMPLE 0C
    /// 01B9 : OpCode.PUSHINT64 0000000001000000
    /// 01C2 : OpCode.SUB
    /// 01C3 : OpCode.STLOC0
    /// 01C4 : OpCode.JMP 06
    /// 01C6 : OpCode.LDLOC0
    /// 01C7 : OpCode.PUSH1
    /// 01C8 : OpCode.DIV
    /// 01C9 : OpCode.STLOC0
    /// 01CA : OpCode.LDLOC0
    /// 01CB : OpCode.RET
    /// </remarks>
    [DisplayName("switchLongLong")]
    public abstract object? SwitchLongLong(string? test);

    #endregion
}
