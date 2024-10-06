using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Switch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Switch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""switchLong"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""switch6"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":242,""safe"":false},{""name"":""switch6Inline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":306,""safe"":false},{""name"":""switchInteger"",""parameters"":[{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":373,""safe"":false},{""name"":""switchLongLong"",""parameters"":[{""name"":""test"",""type"":""String""}],""returntype"":""Any"",""offset"":413,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1pA1cBAXhwaAwBMJcltgAAAGgMATGXJa4AAABoDAEylyWmAAAAaAwBM5clngAAAGgMATSXJZYAAABoDAE1lyWOAAAAaAwBNpclhgAAAGgMATeXJX4AAABoDAE4lyV2AAAAaAwBOZckbmgMAjEwlyRoaAwCMTGXJGJoDAIxMpckXGgMAjEzlyRWaAwCMTSXJFBoDAIxNZckSmgMAjE2lyREaAwCMTeXJD9oDAIxOJckOmgMAjE5lyQ1aAwCMjCXJDAiMRFAEkATQBRAFUAWQBdAGEAZQBpAG0AcQB1AHkAfQCBAABFAABJAABNAABRAABVAAGNAVwEBeHBoDAEwlyQnaAwBMZckImgMATKXJB1oDAEzlyQYaAwBNJckE2gMATWXJA4iDhFAEkATQBRAFUAWQABjQFcBAXhwaAwBMJcmBBFAaAwBMZcmBBJAaAwBMpcmBBNAaAwBM5cmBBRAaAwBNJcmBBVAaAwBNZcmBBZACCYFAGNAaDpXAgERcHhxaRGXJA5pEpckDWkTlyQMIg4ScCIME3AiCBZwIgQQcGhAVwIBEXB4cWkMAWGXJEBpDAFjlyRxaQwBYpcloQAAAGkMAWSXJc8AAABpDAFllyX8AAAAaQwBZpclJgEAAGkMAWeXJVABAAAjfwEAAGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSNLAQAAaBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AjFAEAAGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSPcAAAAaA+gSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AjpQAAAGhooEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wIm5oE6BKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcCI6aBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3AiBmgRoXBoQLFgO0o="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJCdoDDGXJCJoDDKXJB1oDDOXJBhoDDSXJBNoDDWXJA4iDhFAEkATQBRAFUAWQABjQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHDATA1 30
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIF 27
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.PUSHDATA1 31
    /// 0010 : OpCode.EQUAL
    /// 0011 : OpCode.JMPIF 22
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSHDATA1 32
    /// 0017 : OpCode.EQUAL
    /// 0018 : OpCode.JMPIF 1D
    /// 001A : OpCode.LDLOC0
    /// 001B : OpCode.PUSHDATA1 33
    /// 001E : OpCode.EQUAL
    /// 001F : OpCode.JMPIF 18
    /// 0021 : OpCode.LDLOC0
    /// 0022 : OpCode.PUSHDATA1 34
    /// 0025 : OpCode.EQUAL
    /// 0026 : OpCode.JMPIF 13
    /// 0028 : OpCode.LDLOC0
    /// 0029 : OpCode.PUSHDATA1 35
    /// 002C : OpCode.EQUAL
    /// 002D : OpCode.JMPIF 0E
    /// 002F : OpCode.JMP 0E
    /// 0031 : OpCode.PUSH1
    /// 0032 : OpCode.RET
    /// 0033 : OpCode.PUSH2
    /// 0034 : OpCode.RET
    /// 0035 : OpCode.PUSH3
    /// 0036 : OpCode.RET
    /// 0037 : OpCode.PUSH4
    /// 0038 : OpCode.RET
    /// 0039 : OpCode.PUSH5
    /// 003A : OpCode.RET
    /// 003B : OpCode.PUSH6
    /// 003C : OpCode.RET
    /// 003D : OpCode.PUSHINT8 63
    /// 003F : OpCode.RET
    /// </remarks>
    [DisplayName("switch6")]
    public abstract object? Switch6(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJgQRQGgMMZcmBBJAaAwylyYEE0BoDDOXJgQUQGgMNJcmBBVAaAw1lyYEFkAIJgUAY0BoOg==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHDATA1 30
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIFNOT 04
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.RET
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.PUSHDATA1 31
    /// 0012 : OpCode.EQUAL
    /// 0013 : OpCode.JMPIFNOT 04
    /// 0015 : OpCode.PUSH2
    /// 0016 : OpCode.RET
    /// 0017 : OpCode.LDLOC0
    /// 0018 : OpCode.PUSHDATA1 32
    /// 001B : OpCode.EQUAL
    /// 001C : OpCode.JMPIFNOT 04
    /// 001E : OpCode.PUSH3
    /// 001F : OpCode.RET
    /// 0020 : OpCode.LDLOC0
    /// 0021 : OpCode.PUSHDATA1 33
    /// 0024 : OpCode.EQUAL
    /// 0025 : OpCode.JMPIFNOT 04
    /// 0027 : OpCode.PUSH4
    /// 0028 : OpCode.RET
    /// 0029 : OpCode.LDLOC0
    /// 002A : OpCode.PUSHDATA1 34
    /// 002D : OpCode.EQUAL
    /// 002E : OpCode.JMPIFNOT 04
    /// 0030 : OpCode.PUSH5
    /// 0031 : OpCode.RET
    /// 0032 : OpCode.LDLOC0
    /// 0033 : OpCode.PUSHDATA1 35
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.JMPIFNOT 04
    /// 0039 : OpCode.PUSH6
    /// 003A : OpCode.RET
    /// 003B : OpCode.PUSHT
    /// 003C : OpCode.JMPIFNOT 05
    /// 003E : OpCode.PUSHINT8 63
    /// 0040 : OpCode.RET
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.THROW
    /// </remarks>
    [DisplayName("switch6Inline")]
    public abstract object? Switch6Inline(string? method);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEXB4cWkRlyQOaRKXJA1pE5ckDCIOEnAiDBNwIggWcCIEEHBoQA==
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSH1
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.LDLOC1
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIF 0E
    /// 000C : OpCode.LDLOC1
    /// 000D : OpCode.PUSH2
    /// 000E : OpCode.EQUAL
    /// 000F : OpCode.JMPIF 0D
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.PUSH3
    /// 0013 : OpCode.EQUAL
    /// 0014 : OpCode.JMPIF 0C
    /// 0016 : OpCode.JMP 0E
    /// 0018 : OpCode.PUSH2
    /// 0019 : OpCode.STLOC0
    /// 001A : OpCode.JMP 0C
    /// 001C : OpCode.PUSH3
    /// 001D : OpCode.STLOC0
    /// 001E : OpCode.JMP 08
    /// 0020 : OpCode.PUSH6
    /// 0021 : OpCode.STLOC0
    /// 0022 : OpCode.JMP 04
    /// 0024 : OpCode.PUSH0
    /// 0025 : OpCode.STLOC0
    /// 0026 : OpCode.LDLOC0
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("switchInteger")]
    public abstract object? SwitchInteger(BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDDCXJbYAAABoDDGXJa4AAABoDDKXJaYAAABoDDOXJZ4AAABoDDSXJZYAAABoDDWXJY4AAABoDDaXJYYAAABoDDeXJX4AAABoDDiXJXYAAABoDDmXJG5oDDEwlyRoaAwxMZckYmgMMTKXJFxoDDEzlyRWaAwxNJckUGgMMTWXJEpoDDE2lyREaAwxN5ckP2gMMTiXJDpoDDE5lyQ1aAwyMJckMCIxEUASQBNAFEAVQBZAF0AYQBlAGkAbQBxAHUAeQB9AIEAAEUAAEkAAE0AAFEAAFUAAY0A=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHDATA1 30
    /// 0009 : OpCode.EQUAL
    /// 000A : OpCode.JMPIF_L B6000000
    /// 000F : OpCode.LDLOC0
    /// 0010 : OpCode.PUSHDATA1 31
    /// 0013 : OpCode.EQUAL
    /// 0014 : OpCode.JMPIF_L AE000000
    /// 0019 : OpCode.LDLOC0
    /// 001A : OpCode.PUSHDATA1 32
    /// 001D : OpCode.EQUAL
    /// 001E : OpCode.JMPIF_L A6000000
    /// 0023 : OpCode.LDLOC0
    /// 0024 : OpCode.PUSHDATA1 33
    /// 0027 : OpCode.EQUAL
    /// 0028 : OpCode.JMPIF_L 9E000000
    /// 002D : OpCode.LDLOC0
    /// 002E : OpCode.PUSHDATA1 34
    /// 0031 : OpCode.EQUAL
    /// 0032 : OpCode.JMPIF_L 96000000
    /// 0037 : OpCode.LDLOC0
    /// 0038 : OpCode.PUSHDATA1 35
    /// 003B : OpCode.EQUAL
    /// 003C : OpCode.JMPIF_L 8E000000
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.PUSHDATA1 36
    /// 0045 : OpCode.EQUAL
    /// 0046 : OpCode.JMPIF_L 86000000
    /// 004B : OpCode.LDLOC0
    /// 004C : OpCode.PUSHDATA1 37
    /// 004F : OpCode.EQUAL
    /// 0050 : OpCode.JMPIF_L 7E000000
    /// 0055 : OpCode.LDLOC0
    /// 0056 : OpCode.PUSHDATA1 38
    /// 0059 : OpCode.EQUAL
    /// 005A : OpCode.JMPIF_L 76000000
    /// 005F : OpCode.LDLOC0
    /// 0060 : OpCode.PUSHDATA1 39
    /// 0063 : OpCode.EQUAL
    /// 0064 : OpCode.JMPIF 6E
    /// 0066 : OpCode.LDLOC0
    /// 0067 : OpCode.PUSHDATA1 3130
    /// 006B : OpCode.EQUAL
    /// 006C : OpCode.JMPIF 68
    /// 006E : OpCode.LDLOC0
    /// 006F : OpCode.PUSHDATA1 3131
    /// 0073 : OpCode.EQUAL
    /// 0074 : OpCode.JMPIF 62
    /// 0076 : OpCode.LDLOC0
    /// 0077 : OpCode.PUSHDATA1 3132
    /// 007B : OpCode.EQUAL
    /// 007C : OpCode.JMPIF 5C
    /// 007E : OpCode.LDLOC0
    /// 007F : OpCode.PUSHDATA1 3133
    /// 0083 : OpCode.EQUAL
    /// 0084 : OpCode.JMPIF 56
    /// 0086 : OpCode.LDLOC0
    /// 0087 : OpCode.PUSHDATA1 3134
    /// 008B : OpCode.EQUAL
    /// 008C : OpCode.JMPIF 50
    /// 008E : OpCode.LDLOC0
    /// 008F : OpCode.PUSHDATA1 3135
    /// 0093 : OpCode.EQUAL
    /// 0094 : OpCode.JMPIF 4A
    /// 0096 : OpCode.LDLOC0
    /// 0097 : OpCode.PUSHDATA1 3136
    /// 009B : OpCode.EQUAL
    /// 009C : OpCode.JMPIF 44
    /// 009E : OpCode.LDLOC0
    /// 009F : OpCode.PUSHDATA1 3137
    /// 00A3 : OpCode.EQUAL
    /// 00A4 : OpCode.JMPIF 3F
    /// 00A6 : OpCode.LDLOC0
    /// 00A7 : OpCode.PUSHDATA1 3138
    /// 00AB : OpCode.EQUAL
    /// 00AC : OpCode.JMPIF 3A
    /// 00AE : OpCode.LDLOC0
    /// 00AF : OpCode.PUSHDATA1 3139
    /// 00B3 : OpCode.EQUAL
    /// 00B4 : OpCode.JMPIF 35
    /// 00B6 : OpCode.LDLOC0
    /// 00B7 : OpCode.PUSHDATA1 3230
    /// 00BB : OpCode.EQUAL
    /// 00BC : OpCode.JMPIF 30
    /// 00BE : OpCode.JMP 31
    /// 00C0 : OpCode.PUSH1
    /// 00C1 : OpCode.RET
    /// 00C2 : OpCode.PUSH2
    /// 00C3 : OpCode.RET
    /// 00C4 : OpCode.PUSH3
    /// 00C5 : OpCode.RET
    /// 00C6 : OpCode.PUSH4
    /// 00C7 : OpCode.RET
    /// 00C8 : OpCode.PUSH5
    /// 00C9 : OpCode.RET
    /// 00CA : OpCode.PUSH6
    /// 00CB : OpCode.RET
    /// 00CC : OpCode.PUSH7
    /// 00CD : OpCode.RET
    /// 00CE : OpCode.PUSH8
    /// 00CF : OpCode.RET
    /// 00D0 : OpCode.PUSH9
    /// 00D1 : OpCode.RET
    /// 00D2 : OpCode.PUSH10
    /// 00D3 : OpCode.RET
    /// 00D4 : OpCode.PUSH11
    /// 00D5 : OpCode.RET
    /// 00D6 : OpCode.PUSH12
    /// 00D7 : OpCode.RET
    /// 00D8 : OpCode.PUSH13
    /// 00D9 : OpCode.RET
    /// 00DA : OpCode.PUSH14
    /// 00DB : OpCode.RET
    /// 00DC : OpCode.PUSH15
    /// 00DD : OpCode.RET
    /// 00DE : OpCode.PUSH16
    /// 00DF : OpCode.RET
    /// 00E0 : OpCode.PUSHINT8 11
    /// 00E2 : OpCode.RET
    /// 00E3 : OpCode.PUSHINT8 12
    /// 00E5 : OpCode.RET
    /// 00E6 : OpCode.PUSHINT8 13
    /// 00E8 : OpCode.RET
    /// 00E9 : OpCode.PUSHINT8 14
    /// 00EB : OpCode.RET
    /// 00EC : OpCode.PUSHINT8 15
    /// 00EE : OpCode.RET
    /// 00EF : OpCode.PUSHINT8 63
    /// 00F1 : OpCode.RET
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
