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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 1400
    /// 0003 : OpCode.PUSHDATA1 0102030405060708090A
    /// 000F : OpCode.CONVERT 30
    /// 0011 : OpCode.STLOC0
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.SIZE
    /// 0015 : OpCode.SWAP
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.ROT
    /// 0018 : OpCode.OVER
    /// 0019 : OpCode.SUB
    /// 001A : OpCode.SUBSTR
    /// 001B : OpCode.STLOC1
    /// 001C : OpCode.LDLOC0
    /// 001D : OpCode.PUSH3
    /// 001E : OpCode.SWAP
    /// 001F : OpCode.PUSH0
    /// 0020 : OpCode.ROT
    /// 0021 : OpCode.OVER
    /// 0022 : OpCode.SUB
    /// 0023 : OpCode.SUBSTR
    /// 0024 : OpCode.STLOC2
    /// 0025 : OpCode.LDLOC0
    /// 0026 : OpCode.DUP
    /// 0027 : OpCode.SIZE
    /// 0028 : OpCode.SWAP
    /// 0029 : OpCode.PUSH2
    /// 002A : OpCode.ROT
    /// 002B : OpCode.OVER
    /// 002C : OpCode.SUB
    /// 002D : OpCode.SUBSTR
    /// 002E : OpCode.STLOC3
    /// 002F : OpCode.LDLOC0
    /// 0030 : OpCode.PUSH5
    /// 0031 : OpCode.SWAP
    /// 0032 : OpCode.PUSH3
    /// 0033 : OpCode.ROT
    /// 0034 : OpCode.OVER
    /// 0035 : OpCode.SUB
    /// 0036 : OpCode.SUBSTR
    /// 0037 : OpCode.STLOC4
    /// 0038 : OpCode.LDLOC0
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.SIZE
    /// 003B : OpCode.SWAP
    /// 003C : OpCode.DUP
    /// 003D : OpCode.SIZE
    /// 003E : OpCode.PUSH2
    /// 003F : OpCode.SUB
    /// 0040 : OpCode.ROT
    /// 0041 : OpCode.OVER
    /// 0042 : OpCode.SUB
    /// 0043 : OpCode.SUBSTR
    /// 0044 : OpCode.STLOC5
    /// 0045 : OpCode.LDLOC0
    /// 0046 : OpCode.DUP
    /// 0047 : OpCode.SIZE
    /// 0048 : OpCode.PUSH3
    /// 0049 : OpCode.SUB
    /// 004A : OpCode.SWAP
    /// 004B : OpCode.PUSH0
    /// 004C : OpCode.ROT
    /// 004D : OpCode.OVER
    /// 004E : OpCode.SUB
    /// 004F : OpCode.SUBSTR
    /// 0050 : OpCode.STLOC6
    /// 0051 : OpCode.LDLOC0
    /// 0052 : OpCode.DUP
    /// 0053 : OpCode.SIZE
    /// 0054 : OpCode.PUSH4
    /// 0055 : OpCode.SUB
    /// 0056 : OpCode.SWAP
    /// 0057 : OpCode.PUSH3
    /// 0058 : OpCode.ROT
    /// 0059 : OpCode.OVER
    /// 005A : OpCode.SUB
    /// 005B : OpCode.SUBSTR
    /// 005C : OpCode.STLOC 07
    /// 005E : OpCode.LDLOC0
    /// 005F : OpCode.DUP
    /// 0060 : OpCode.SIZE
    /// 0061 : OpCode.PUSH2
    /// 0062 : OpCode.SUB
    /// 0063 : OpCode.SWAP
    /// 0064 : OpCode.DUP
    /// 0065 : OpCode.SIZE
    /// 0066 : OpCode.PUSH4
    /// 0067 : OpCode.SUB
    /// 0068 : OpCode.ROT
    /// 0069 : OpCode.OVER
    /// 006A : OpCode.SUB
    /// 006B : OpCode.SUBSTR
    /// 006C : OpCode.STLOC 08
    /// 006E : OpCode.LDLOC0
    /// 006F : OpCode.PUSH0
    /// 0070 : OpCode.PICKITEM
    /// 0071 : OpCode.STLOC 09
    /// 0073 : OpCode.LDLOC1
    /// 0074 : OpCode.SIZE
    /// 0075 : OpCode.CALLT 0000
    /// 0078 : OpCode.SYSCALL CFE74796
    /// 007D : OpCode.LDLOC2
    /// 007E : OpCode.SIZE
    /// 007F : OpCode.CALLT 0000
    /// 0082 : OpCode.SYSCALL CFE74796
    /// 0087 : OpCode.LDLOC3
    /// 0088 : OpCode.SIZE
    /// 0089 : OpCode.CALLT 0000
    /// 008C : OpCode.SYSCALL CFE74796
    /// 0091 : OpCode.LDLOC4
    /// 0092 : OpCode.SIZE
    /// 0093 : OpCode.CALLT 0000
    /// 0096 : OpCode.SYSCALL CFE74796
    /// 009B : OpCode.LDLOC5
    /// 009C : OpCode.SIZE
    /// 009D : OpCode.CALLT 0000
    /// 00A0 : OpCode.SYSCALL CFE74796
    /// 00A5 : OpCode.LDLOC6
    /// 00A6 : OpCode.SIZE
    /// 00A7 : OpCode.CALLT 0000
    /// 00AA : OpCode.SYSCALL CFE74796
    /// 00AF : OpCode.LDLOC 07
    /// 00B1 : OpCode.SIZE
    /// 00B2 : OpCode.CALLT 0000
    /// 00B5 : OpCode.SYSCALL CFE74796
    /// 00BA : OpCode.LDLOC 08
    /// 00BC : OpCode.SIZE
    /// 00BD : OpCode.CALLT 0000
    /// 00C0 : OpCode.SYSCALL CFE74796
    /// 00C5 : OpCode.LDLOC 09
    /// 00C7 : OpCode.CALLT 0000
    /// 00CA : OpCode.SYSCALL CFE74796
    /// 00CF : OpCode.PUSHDATA1 313233343536373839
    /// 00DA : OpCode.STLOC 0A
    /// 00DC : OpCode.LDLOC 0A
    /// 00DE : OpCode.DUP
    /// 00DF : OpCode.SIZE
    /// 00E0 : OpCode.SWAP
    /// 00E1 : OpCode.PUSH0
    /// 00E2 : OpCode.ROT
    /// 00E3 : OpCode.OVER
    /// 00E4 : OpCode.SUB
    /// 00E5 : OpCode.SUBSTR
    /// 00E6 : OpCode.CONVERT 28
    /// 00E8 : OpCode.STLOC 0B
    /// 00EA : OpCode.LDLOC 0A
    /// 00EC : OpCode.PUSH3
    /// 00ED : OpCode.SWAP
    /// 00EE : OpCode.PUSH0
    /// 00EF : OpCode.ROT
    /// 00F0 : OpCode.OVER
    /// 00F1 : OpCode.SUB
    /// 00F2 : OpCode.SUBSTR
    /// 00F3 : OpCode.CONVERT 28
    /// 00F5 : OpCode.STLOC 0C
    /// 00F7 : OpCode.LDLOC 0A
    /// 00F9 : OpCode.DUP
    /// 00FA : OpCode.SIZE
    /// 00FB : OpCode.SWAP
    /// 00FC : OpCode.PUSH2
    /// 00FD : OpCode.ROT
    /// 00FE : OpCode.OVER
    /// 00FF : OpCode.SUB
    /// 0100 : OpCode.SUBSTR
    /// 0101 : OpCode.CONVERT 28
    /// 0103 : OpCode.STLOC 0D
    /// 0105 : OpCode.LDLOC 0A
    /// 0107 : OpCode.PUSH5
    /// 0108 : OpCode.SWAP
    /// 0109 : OpCode.PUSH3
    /// 010A : OpCode.ROT
    /// 010B : OpCode.OVER
    /// 010C : OpCode.SUB
    /// 010D : OpCode.SUBSTR
    /// 010E : OpCode.CONVERT 28
    /// 0110 : OpCode.STLOC 0E
    /// 0112 : OpCode.LDLOC 0A
    /// 0114 : OpCode.DUP
    /// 0115 : OpCode.SIZE
    /// 0116 : OpCode.SWAP
    /// 0117 : OpCode.DUP
    /// 0118 : OpCode.SIZE
    /// 0119 : OpCode.PUSH2
    /// 011A : OpCode.SUB
    /// 011B : OpCode.ROT
    /// 011C : OpCode.OVER
    /// 011D : OpCode.SUB
    /// 011E : OpCode.SUBSTR
    /// 011F : OpCode.CONVERT 28
    /// 0121 : OpCode.STLOC 0F
    /// 0123 : OpCode.LDLOC 0A
    /// 0125 : OpCode.DUP
    /// 0126 : OpCode.SIZE
    /// 0127 : OpCode.PUSH3
    /// 0128 : OpCode.SUB
    /// 0129 : OpCode.SWAP
    /// 012A : OpCode.PUSH0
    /// 012B : OpCode.ROT
    /// 012C : OpCode.OVER
    /// 012D : OpCode.SUB
    /// 012E : OpCode.SUBSTR
    /// 012F : OpCode.CONVERT 28
    /// 0131 : OpCode.STLOC 10
    /// 0133 : OpCode.LDLOC 0A
    /// 0135 : OpCode.DUP
    /// 0136 : OpCode.SIZE
    /// 0137 : OpCode.PUSH4
    /// 0138 : OpCode.SUB
    /// 0139 : OpCode.SWAP
    /// 013A : OpCode.PUSH3
    /// 013B : OpCode.ROT
    /// 013C : OpCode.OVER
    /// 013D : OpCode.SUB
    /// 013E : OpCode.SUBSTR
    /// 013F : OpCode.CONVERT 28
    /// 0141 : OpCode.STLOC 11
    /// 0143 : OpCode.LDLOC 0A
    /// 0145 : OpCode.DUP
    /// 0146 : OpCode.SIZE
    /// 0147 : OpCode.PUSH2
    /// 0148 : OpCode.SUB
    /// 0149 : OpCode.SWAP
    /// 014A : OpCode.DUP
    /// 014B : OpCode.SIZE
    /// 014C : OpCode.PUSH4
    /// 014D : OpCode.SUB
    /// 014E : OpCode.ROT
    /// 014F : OpCode.OVER
    /// 0150 : OpCode.SUB
    /// 0151 : OpCode.SUBSTR
    /// 0152 : OpCode.CONVERT 28
    /// 0154 : OpCode.STLOC 12
    /// 0156 : OpCode.LDLOC 0A
    /// 0158 : OpCode.PUSH0
    /// 0159 : OpCode.PICKITEM
    /// 015A : OpCode.STLOC 13
    /// 015C : OpCode.LDLOC 0B
    /// 015E : OpCode.CONVERT 28
    /// 0160 : OpCode.SYSCALL CFE74796
    /// 0165 : OpCode.LDLOC 0C
    /// 0167 : OpCode.CONVERT 28
    /// 0169 : OpCode.SYSCALL CFE74796
    /// 016E : OpCode.LDLOC 0D
    /// 0170 : OpCode.CONVERT 28
    /// 0172 : OpCode.SYSCALL CFE74796
    /// 0177 : OpCode.LDLOC 0E
    /// 0179 : OpCode.CONVERT 28
    /// 017B : OpCode.SYSCALL CFE74796
    /// 0180 : OpCode.LDLOC 0F
    /// 0182 : OpCode.CONVERT 28
    /// 0184 : OpCode.SYSCALL CFE74796
    /// 0189 : OpCode.LDLOC 10
    /// 018B : OpCode.CONVERT 28
    /// 018D : OpCode.SYSCALL CFE74796
    /// 0192 : OpCode.LDLOC 11
    /// 0194 : OpCode.CONVERT 28
    /// 0196 : OpCode.SYSCALL CFE74796
    /// 019B : OpCode.LDLOC 12
    /// 019D : OpCode.CONVERT 28
    /// 019F : OpCode.SYSCALL CFE74796
    /// 01A4 : OpCode.LDLOC 13
    /// 01A6 : OpCode.CONVERT 28
    /// 01A8 : OpCode.SYSCALL CFE74796
    /// 01AD : OpCode.RET
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion

}
