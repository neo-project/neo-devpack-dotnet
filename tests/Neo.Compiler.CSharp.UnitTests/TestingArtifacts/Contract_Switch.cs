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
    [DisplayName("switch6")]
    public abstract object? Switch6(string? method);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHDATA1
    // 0009 : EQUAL
    // 000A : JMPIF
    // 000C : LDLOC0
    // 000D : PUSHDATA1
    // 0010 : EQUAL
    // 0011 : JMPIF
    // 0013 : LDLOC0
    // 0014 : PUSHDATA1
    // 0017 : EQUAL
    // 0018 : JMPIF
    // 001A : LDLOC0
    // 001B : PUSHDATA1
    // 001E : EQUAL
    // 001F : JMPIF
    // 0021 : LDLOC0
    // 0022 : PUSHDATA1
    // 0025 : EQUAL
    // 0026 : JMPIF
    // 0028 : LDLOC0
    // 0029 : PUSHDATA1
    // 002C : EQUAL
    // 002D : JMPIF
    // 002F : JMP
    // 0031 : PUSH1
    // 0032 : RET
    // 0033 : PUSH2
    // 0034 : RET
    // 0035 : PUSH3
    // 0036 : RET
    // 0037 : PUSH4
    // 0038 : RET
    // 0039 : PUSH5
    // 003A : RET
    // 003B : PUSH6
    // 003C : RET
    // 003D : PUSHINT8
    // 003F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switch6Inline")]
    public abstract object? Switch6Inline(string? method);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHDATA1
    // 0009 : EQUAL
    // 000A : JMPIFNOT
    // 000C : PUSH1
    // 000D : RET
    // 000E : LDLOC0
    // 000F : PUSHDATA1
    // 0012 : EQUAL
    // 0013 : JMPIFNOT
    // 0015 : PUSH2
    // 0016 : RET
    // 0017 : LDLOC0
    // 0018 : PUSHDATA1
    // 001B : EQUAL
    // 001C : JMPIFNOT
    // 001E : PUSH3
    // 001F : RET
    // 0020 : LDLOC0
    // 0021 : PUSHDATA1
    // 0024 : EQUAL
    // 0025 : JMPIFNOT
    // 0027 : PUSH4
    // 0028 : RET
    // 0029 : LDLOC0
    // 002A : PUSHDATA1
    // 002D : EQUAL
    // 002E : JMPIFNOT
    // 0030 : PUSH5
    // 0031 : RET
    // 0032 : LDLOC0
    // 0033 : PUSHDATA1
    // 0036 : EQUAL
    // 0037 : JMPIFNOT
    // 0039 : PUSH6
    // 003A : RET
    // 003B : PUSHT
    // 003C : JMPIFNOT
    // 003E : PUSHINT8
    // 0040 : RET
    // 0041 : LDLOC0
    // 0042 : THROW

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switchInteger")]
    public abstract object? SwitchInteger(BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : PUSH1
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : STLOC1
    // 0007 : LDLOC1
    // 0008 : PUSH1
    // 0009 : EQUAL
    // 000A : JMPIF
    // 000C : LDLOC1
    // 000D : PUSH2
    // 000E : EQUAL
    // 000F : JMPIF
    // 0011 : LDLOC1
    // 0012 : PUSH3
    // 0013 : EQUAL
    // 0014 : JMPIF
    // 0016 : JMP
    // 0018 : PUSH2
    // 0019 : STLOC0
    // 001A : JMP
    // 001C : PUSH3
    // 001D : STLOC0
    // 001E : JMP
    // 0020 : PUSH6
    // 0021 : STLOC0
    // 0022 : JMP
    // 0024 : PUSH0
    // 0025 : STLOC0
    // 0026 : LDLOC0
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switchLong")]
    public abstract object? SwitchLong(string? method);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHDATA1
    // 0009 : EQUAL
    // 000A : JMPIF_L
    // 000F : LDLOC0
    // 0010 : PUSHDATA1
    // 0013 : EQUAL
    // 0014 : JMPIF_L
    // 0019 : LDLOC0
    // 001A : PUSHDATA1
    // 001D : EQUAL
    // 001E : JMPIF_L
    // 0023 : LDLOC0
    // 0024 : PUSHDATA1
    // 0027 : EQUAL
    // 0028 : JMPIF_L
    // 002D : LDLOC0
    // 002E : PUSHDATA1
    // 0031 : EQUAL
    // 0032 : JMPIF_L
    // 0037 : LDLOC0
    // 0038 : PUSHDATA1
    // 003B : EQUAL
    // 003C : JMPIF_L
    // 0041 : LDLOC0
    // 0042 : PUSHDATA1
    // 0045 : EQUAL
    // 0046 : JMPIF_L
    // 004B : LDLOC0
    // 004C : PUSHDATA1
    // 004F : EQUAL
    // 0050 : JMPIF_L
    // 0055 : LDLOC0
    // 0056 : PUSHDATA1
    // 0059 : EQUAL
    // 005A : JMPIF_L
    // 005F : LDLOC0
    // 0060 : PUSHDATA1
    // 0063 : EQUAL
    // 0064 : JMPIF
    // 0066 : LDLOC0
    // 0067 : PUSHDATA1
    // 006B : EQUAL
    // 006C : JMPIF
    // 006E : LDLOC0
    // 006F : PUSHDATA1
    // 0073 : EQUAL
    // 0074 : JMPIF
    // 0076 : LDLOC0
    // 0077 : PUSHDATA1
    // 007B : EQUAL
    // 007C : JMPIF
    // 007E : LDLOC0
    // 007F : PUSHDATA1
    // 0083 : EQUAL
    // 0084 : JMPIF
    // 0086 : LDLOC0
    // 0087 : PUSHDATA1
    // 008B : EQUAL
    // 008C : JMPIF
    // 008E : LDLOC0
    // 008F : PUSHDATA1
    // 0093 : EQUAL
    // 0094 : JMPIF
    // 0096 : LDLOC0
    // 0097 : PUSHDATA1
    // 009B : EQUAL
    // 009C : JMPIF
    // 009E : LDLOC0
    // 009F : PUSHDATA1
    // 00A3 : EQUAL
    // 00A4 : JMPIF
    // 00A6 : LDLOC0
    // 00A7 : PUSHDATA1
    // 00AB : EQUAL
    // 00AC : JMPIF
    // 00AE : LDLOC0
    // 00AF : PUSHDATA1
    // 00B3 : EQUAL
    // 00B4 : JMPIF
    // 00B6 : LDLOC0
    // 00B7 : PUSHDATA1
    // 00BB : EQUAL
    // 00BC : JMPIF
    // 00BE : JMP
    // 00C0 : PUSH1
    // 00C1 : RET
    // 00C2 : PUSH2
    // 00C3 : RET
    // 00C4 : PUSH3
    // 00C5 : RET
    // 00C6 : PUSH4
    // 00C7 : RET
    // 00C8 : PUSH5
    // 00C9 : RET
    // 00CA : PUSH6
    // 00CB : RET
    // 00CC : PUSH7
    // 00CD : RET
    // 00CE : PUSH8
    // 00CF : RET
    // 00D0 : PUSH9
    // 00D1 : RET
    // 00D2 : PUSH10
    // 00D3 : RET
    // 00D4 : PUSH11
    // 00D5 : RET
    // 00D6 : PUSH12
    // 00D7 : RET
    // 00D8 : PUSH13
    // 00D9 : RET
    // 00DA : PUSH14
    // 00DB : RET
    // 00DC : PUSH15
    // 00DD : RET
    // 00DE : PUSH16
    // 00DF : RET
    // 00E0 : PUSHINT8
    // 00E2 : RET
    // 00E3 : PUSHINT8
    // 00E5 : RET
    // 00E6 : PUSHINT8
    // 00E8 : RET
    // 00E9 : PUSHINT8
    // 00EB : RET
    // 00EC : PUSHINT8
    // 00EE : RET
    // 00EF : PUSHINT8
    // 00F1 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("switchLongLong")]
    public abstract object? SwitchLongLong(string? test);
    // 0000 : INITSLOT
    // 0003 : PUSH1
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : STLOC1
    // 0007 : LDLOC1
    // 0008 : PUSHDATA1
    // 000B : EQUAL
    // 000C : JMPIF
    // 000E : LDLOC1
    // 000F : PUSHDATA1
    // 0012 : EQUAL
    // 0013 : JMPIF
    // 0015 : LDLOC1
    // 0016 : PUSHDATA1
    // 0019 : EQUAL
    // 001A : JMPIF_L
    // 001F : LDLOC1
    // 0020 : PUSHDATA1
    // 0023 : EQUAL
    // 0024 : JMPIF_L
    // 0029 : LDLOC1
    // 002A : PUSHDATA1
    // 002D : EQUAL
    // 002E : JMPIF_L
    // 0033 : LDLOC1
    // 0034 : PUSHDATA1
    // 0037 : EQUAL
    // 0038 : JMPIF_L
    // 003D : LDLOC1
    // 003E : PUSHDATA1
    // 0041 : EQUAL
    // 0042 : JMPIF_L
    // 0047 : JMP_L
    // 004C : LDLOC0
    // 004D : DUP
    // 004E : INC
    // 004F : DUP
    // 0050 : PUSHINT32
    // 0055 : JMPGE
    // 0057 : JMP
    // 0059 : DUP
    // 005A : PUSHINT32
    // 005F : JMPLE
    // 0061 : PUSHINT64
    // 006A : AND
    // 006B : DUP
    // 006C : PUSHINT32
    // 0071 : JMPLE
    // 0073 : PUSHINT64
    // 007C : SUB
    // 007D : STLOC0
    // 007E : DROP
    // 007F : JMP_L
    // 0084 : LDLOC0
    // 0085 : PUSH2
    // 0086 : MUL
    // 0087 : DUP
    // 0088 : PUSHINT32
    // 008D : JMPGE
    // 008F : JMP
    // 0091 : DUP
    // 0092 : PUSHINT32
    // 0097 : JMPLE
    // 0099 : PUSHINT64
    // 00A2 : AND
    // 00A3 : DUP
    // 00A4 : PUSHINT32
    // 00A9 : JMPLE
    // 00AB : PUSHINT64
    // 00B4 : SUB
    // 00B5 : STLOC0
    // 00B6 : JMP_L
    // 00BB : LDLOC0
    // 00BC : DUP
    // 00BD : DEC
    // 00BE : DUP
    // 00BF : PUSHINT32
    // 00C4 : JMPGE
    // 00C6 : JMP
    // 00C8 : DUP
    // 00C9 : PUSHINT32
    // 00CE : JMPLE
    // 00D0 : PUSHINT64
    // 00D9 : AND
    // 00DA : DUP
    // 00DB : PUSHINT32
    // 00E0 : JMPLE
    // 00E2 : PUSHINT64
    // 00EB : SUB
    // 00EC : STLOC0
    // 00ED : DROP
    // 00EE : JMP_L
    // 00F3 : LDLOC0
    // 00F4 : PUSHM1
    // 00F5 : MUL
    // 00F6 : DUP
    // 00F7 : PUSHINT32
    // 00FC : JMPGE
    // 00FE : JMP
    // 0100 : DUP
    // 0101 : PUSHINT32
    // 0106 : JMPLE
    // 0108 : PUSHINT64
    // 0111 : AND
    // 0112 : DUP
    // 0113 : PUSHINT32
    // 0118 : JMPLE
    // 011A : PUSHINT64
    // 0123 : SUB
    // 0124 : STLOC0
    // 0125 : JMP_L
    // 012A : LDLOC0
    // 012B : LDLOC0
    // 012C : MUL
    // 012D : DUP
    // 012E : PUSHINT32
    // 0133 : JMPGE
    // 0135 : JMP
    // 0137 : DUP
    // 0138 : PUSHINT32
    // 013D : JMPLE
    // 013F : PUSHINT64
    // 0148 : AND
    // 0149 : DUP
    // 014A : PUSHINT32
    // 014F : JMPLE
    // 0151 : PUSHINT64
    // 015A : SUB
    // 015B : STLOC0
    // 015C : JMP
    // 015E : LDLOC0
    // 015F : PUSH3
    // 0160 : MUL
    // 0161 : DUP
    // 0162 : PUSHINT32
    // 0167 : JMPGE
    // 0169 : JMP
    // 016B : DUP
    // 016C : PUSHINT32
    // 0171 : JMPLE
    // 0173 : PUSHINT64
    // 017C : AND
    // 017D : DUP
    // 017E : PUSHINT32
    // 0183 : JMPLE
    // 0185 : PUSHINT64
    // 018E : SUB
    // 018F : STLOC0
    // 0190 : JMP
    // 0192 : LDLOC0
    // 0193 : PUSH2
    // 0194 : ADD
    // 0195 : DUP
    // 0196 : PUSHINT32
    // 019B : JMPGE
    // 019D : JMP
    // 019F : DUP
    // 01A0 : PUSHINT32
    // 01A5 : JMPLE
    // 01A7 : PUSHINT64
    // 01B0 : AND
    // 01B1 : DUP
    // 01B2 : PUSHINT32
    // 01B7 : JMPLE
    // 01B9 : PUSHINT64
    // 01C2 : SUB
    // 01C3 : STLOC0
    // 01C4 : JMP
    // 01C6 : LDLOC0
    // 01C7 : PUSH1
    // 01C8 : DIV
    // 01C9 : STLOC0
    // 01CA : LDLOC0
    // 01CB : RET

    #endregion

}
