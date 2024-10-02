using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IndexOrRange(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IndexOrRange"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/a4BVxQADAoBAgMEBQYHCAkK2zBwaErKUBBRS5+McWgTUBBRS5+McmhKylASUUufjHNoFVATUUufjHRoSspQSsoSn1FLn4x1aErKE59QEFFLn4x2aErKFJ9QE1FLn4x3B2hKyhKfUErKFJ9RS5+MdwhoEM53CWnKNwAAQc/nR5ZqyjcAAEHP50eWa8o3AABBz+dHlmzKNwAAQc/nR5ZtyjcAAEHP50eWbso3AABBz+dHlm8HyjcAAEHP50eWbwjKNwAAQc/nR5ZvCTcAAEHP50eWDAkxMjM0NTY3ODl3Cm8KSspQEFFLn4zbKHcLbwoTUBBRS5+M2yh3DG8KSspQElFLn4zbKHcNbwoVUBNRS5+M2yh3Dm8KSspQSsoSn1FLn4zbKHcPbwpKyhOfUBBRS5+M2yh3EG8KSsoUn1ATUUufjNsodxFvCkrKEp9QSsoUn1FLn4zbKHcSbwoQzncTbwvbKEHP50eWbwzbKEHP50eWbw3bKEHP50eWbw7bKEHP50eWbw/bKEHP50eWbxDbKEHP50eWbxHbKEHP50eWbxLbKEHP50eWbxPbKEHP50eWQFm4evg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000F : CONVERT
    // 0011 : STLOC0
    // 0012 : LDLOC0
    // 0013 : DUP
    // 0014 : SIZE
    // 0015 : SWAP
    // 0016 : PUSH0
    // 0017 : ROT
    // 0018 : OVER
    // 0019 : SUB
    // 001A : SUBSTR
    // 001B : STLOC1
    // 001C : LDLOC0
    // 001D : PUSH3
    // 001E : SWAP
    // 001F : PUSH0
    // 0020 : ROT
    // 0021 : OVER
    // 0022 : SUB
    // 0023 : SUBSTR
    // 0024 : STLOC2
    // 0025 : LDLOC0
    // 0026 : DUP
    // 0027 : SIZE
    // 0028 : SWAP
    // 0029 : PUSH2
    // 002A : ROT
    // 002B : OVER
    // 002C : SUB
    // 002D : SUBSTR
    // 002E : STLOC3
    // 002F : LDLOC0
    // 0030 : PUSH5
    // 0031 : SWAP
    // 0032 : PUSH3
    // 0033 : ROT
    // 0034 : OVER
    // 0035 : SUB
    // 0036 : SUBSTR
    // 0037 : STLOC4
    // 0038 : LDLOC0
    // 0039 : DUP
    // 003A : SIZE
    // 003B : SWAP
    // 003C : DUP
    // 003D : SIZE
    // 003E : PUSH2
    // 003F : SUB
    // 0040 : ROT
    // 0041 : OVER
    // 0042 : SUB
    // 0043 : SUBSTR
    // 0044 : STLOC5
    // 0045 : LDLOC0
    // 0046 : DUP
    // 0047 : SIZE
    // 0048 : PUSH3
    // 0049 : SUB
    // 004A : SWAP
    // 004B : PUSH0
    // 004C : ROT
    // 004D : OVER
    // 004E : SUB
    // 004F : SUBSTR
    // 0050 : STLOC6
    // 0051 : LDLOC0
    // 0052 : DUP
    // 0053 : SIZE
    // 0054 : PUSH4
    // 0055 : SUB
    // 0056 : SWAP
    // 0057 : PUSH3
    // 0058 : ROT
    // 0059 : OVER
    // 005A : SUB
    // 005B : SUBSTR
    // 005C : STLOC
    // 005E : LDLOC0
    // 005F : DUP
    // 0060 : SIZE
    // 0061 : PUSH2
    // 0062 : SUB
    // 0063 : SWAP
    // 0064 : DUP
    // 0065 : SIZE
    // 0066 : PUSH4
    // 0067 : SUB
    // 0068 : ROT
    // 0069 : OVER
    // 006A : SUB
    // 006B : SUBSTR
    // 006C : STLOC
    // 006E : LDLOC0
    // 006F : PUSH0
    // 0070 : PICKITEM
    // 0071 : STLOC
    // 0073 : LDLOC1
    // 0074 : SIZE
    // 0075 : CALLT
    // 0078 : SYSCALL
    // 007D : LDLOC2
    // 007E : SIZE
    // 007F : CALLT
    // 0082 : SYSCALL
    // 0087 : LDLOC3
    // 0088 : SIZE
    // 0089 : CALLT
    // 008C : SYSCALL
    // 0091 : LDLOC4
    // 0092 : SIZE
    // 0093 : CALLT
    // 0096 : SYSCALL
    // 009B : LDLOC5
    // 009C : SIZE
    // 009D : CALLT
    // 00A0 : SYSCALL
    // 00A5 : LDLOC6
    // 00A6 : SIZE
    // 00A7 : CALLT
    // 00AA : SYSCALL
    // 00AF : LDLOC
    // 00B1 : SIZE
    // 00B2 : CALLT
    // 00B5 : SYSCALL
    // 00BA : LDLOC
    // 00BC : SIZE
    // 00BD : CALLT
    // 00C0 : SYSCALL
    // 00C5 : LDLOC
    // 00C7 : CALLT
    // 00CA : SYSCALL
    // 00CF : PUSHDATA1
    // 00DA : STLOC
    // 00DC : LDLOC
    // 00DE : DUP
    // 00DF : SIZE
    // 00E0 : SWAP
    // 00E1 : PUSH0
    // 00E2 : ROT
    // 00E3 : OVER
    // 00E4 : SUB
    // 00E5 : SUBSTR
    // 00E6 : CONVERT
    // 00E8 : STLOC
    // 00EA : LDLOC
    // 00EC : PUSH3
    // 00ED : SWAP
    // 00EE : PUSH0
    // 00EF : ROT
    // 00F0 : OVER
    // 00F1 : SUB
    // 00F2 : SUBSTR
    // 00F3 : CONVERT
    // 00F5 : STLOC
    // 00F7 : LDLOC
    // 00F9 : DUP
    // 00FA : SIZE
    // 00FB : SWAP
    // 00FC : PUSH2
    // 00FD : ROT
    // 00FE : OVER
    // 00FF : SUB
    // 0100 : SUBSTR
    // 0101 : CONVERT
    // 0103 : STLOC
    // 0105 : LDLOC
    // 0107 : PUSH5
    // 0108 : SWAP
    // 0109 : PUSH3
    // 010A : ROT
    // 010B : OVER
    // 010C : SUB
    // 010D : SUBSTR
    // 010E : CONVERT
    // 0110 : STLOC
    // 0112 : LDLOC
    // 0114 : DUP
    // 0115 : SIZE
    // 0116 : SWAP
    // 0117 : DUP
    // 0118 : SIZE
    // 0119 : PUSH2
    // 011A : SUB
    // 011B : ROT
    // 011C : OVER
    // 011D : SUB
    // 011E : SUBSTR
    // 011F : CONVERT
    // 0121 : STLOC
    // 0123 : LDLOC
    // 0125 : DUP
    // 0126 : SIZE
    // 0127 : PUSH3
    // 0128 : SUB
    // 0129 : SWAP
    // 012A : PUSH0
    // 012B : ROT
    // 012C : OVER
    // 012D : SUB
    // 012E : SUBSTR
    // 012F : CONVERT
    // 0131 : STLOC
    // 0133 : LDLOC
    // 0135 : DUP
    // 0136 : SIZE
    // 0137 : PUSH4
    // 0138 : SUB
    // 0139 : SWAP
    // 013A : PUSH3
    // 013B : ROT
    // 013C : OVER
    // 013D : SUB
    // 013E : SUBSTR
    // 013F : CONVERT
    // 0141 : STLOC
    // 0143 : LDLOC
    // 0145 : DUP
    // 0146 : SIZE
    // 0147 : PUSH2
    // 0148 : SUB
    // 0149 : SWAP
    // 014A : DUP
    // 014B : SIZE
    // 014C : PUSH4
    // 014D : SUB
    // 014E : ROT
    // 014F : OVER
    // 0150 : SUB
    // 0151 : SUBSTR
    // 0152 : CONVERT
    // 0154 : STLOC
    // 0156 : LDLOC
    // 0158 : PUSH0
    // 0159 : PICKITEM
    // 015A : STLOC
    // 015C : LDLOC
    // 015E : CONVERT
    // 0160 : SYSCALL
    // 0165 : LDLOC
    // 0167 : CONVERT
    // 0169 : SYSCALL
    // 016E : LDLOC
    // 0170 : CONVERT
    // 0172 : SYSCALL
    // 0177 : LDLOC
    // 0179 : CONVERT
    // 017B : SYSCALL
    // 0180 : LDLOC
    // 0182 : CONVERT
    // 0184 : SYSCALL
    // 0189 : LDLOC
    // 018B : CONVERT
    // 018D : SYSCALL
    // 0192 : LDLOC
    // 0194 : CONVERT
    // 0196 : SYSCALL
    // 019B : LDLOC
    // 019D : CONVERT
    // 019F : SYSCALL
    // 01A4 : LDLOC
    // 01A6 : CONVERT
    // 01A8 : SYSCALL
    // 01AD : RET

    #endregion

}
