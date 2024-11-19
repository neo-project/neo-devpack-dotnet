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
    /// Script: VxQADAoBAgMEBQYHCAkK2zBwaErKUBBRS5+McWgTUBBRS5+McmhKylASUUufjHNoFVATUUufjHRoSspQSsoSn1FLn4x1aErKE59QEFFLn4x2aErKFJ9QE1FLn4x3B2hKyhKfUErKFJ9RS5+MdwhoEM53CWnKNwAAQc/nR5ZqyjcAAEHP50eWa8o3AABBz+dHlmzKNwAAQc/nR5ZtyjcAAEHP50eWbso3AABBz+dHlm8HyjcAAEHP50eWbwjKNwAAQc/nR5ZvCTcAAEHP50eWDAkxMjM0NTY3ODl3Cm8KSspQEFFLn4zbKHcLbwoTUBBRS5+M2yh3DG8KSspQElFLn4zbKHcNbwoVUBNRS5+M2yh3Dm8KSspQSsoSn1FLn4zbKHcPbwpKyhOfUBBRS5+M2yh3EG8KSsoUn1ATUUufjNsodxFvCkrKEp9QSsoUn1FLn4zbKHcSbwoQzncTbwvbKEHP50eWbwzbKEHP50eWbw3bKEHP50eWbw7bKEHP50eWbw/bKEHP50eWbxDbKEHP50eWbxHbKEHP50eWbxLbKEHP50eWbxPbKEHP50eWQA==
    /// 0000 : INITSLOT 1400 [64 datoshi]
    /// 0003 : PUSHDATA1 0102030405060708090A [8 datoshi]
    /// 000F : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0011 : STLOC0 [2 datoshi]
    /// 0012 : LDLOC0 [2 datoshi]
    /// 0013 : DUP [2 datoshi]
    /// 0014 : SIZE [4 datoshi]
    /// 0015 : SWAP [2 datoshi]
    /// 0016 : PUSH0 [1 datoshi]
    /// 0017 : ROT [2 datoshi]
    /// 0018 : OVER [2 datoshi]
    /// 0019 : SUB [8 datoshi]
    /// 001A : SUBSTR [2048 datoshi]
    /// 001B : STLOC1 [2 datoshi]
    /// 001C : LDLOC0 [2 datoshi]
    /// 001D : PUSH3 [1 datoshi]
    /// 001E : SWAP [2 datoshi]
    /// 001F : PUSH0 [1 datoshi]
    /// 0020 : ROT [2 datoshi]
    /// 0021 : OVER [2 datoshi]
    /// 0022 : SUB [8 datoshi]
    /// 0023 : SUBSTR [2048 datoshi]
    /// 0024 : STLOC2 [2 datoshi]
    /// 0025 : LDLOC0 [2 datoshi]
    /// 0026 : DUP [2 datoshi]
    /// 0027 : SIZE [4 datoshi]
    /// 0028 : SWAP [2 datoshi]
    /// 0029 : PUSH2 [1 datoshi]
    /// 002A : ROT [2 datoshi]
    /// 002B : OVER [2 datoshi]
    /// 002C : SUB [8 datoshi]
    /// 002D : SUBSTR [2048 datoshi]
    /// 002E : STLOC3 [2 datoshi]
    /// 002F : LDLOC0 [2 datoshi]
    /// 0030 : PUSH5 [1 datoshi]
    /// 0031 : SWAP [2 datoshi]
    /// 0032 : PUSH3 [1 datoshi]
    /// 0033 : ROT [2 datoshi]
    /// 0034 : OVER [2 datoshi]
    /// 0035 : SUB [8 datoshi]
    /// 0036 : SUBSTR [2048 datoshi]
    /// 0037 : STLOC4 [2 datoshi]
    /// 0038 : LDLOC0 [2 datoshi]
    /// 0039 : DUP [2 datoshi]
    /// 003A : SIZE [4 datoshi]
    /// 003B : SWAP [2 datoshi]
    /// 003C : DUP [2 datoshi]
    /// 003D : SIZE [4 datoshi]
    /// 003E : PUSH2 [1 datoshi]
    /// 003F : SUB [8 datoshi]
    /// 0040 : ROT [2 datoshi]
    /// 0041 : OVER [2 datoshi]
    /// 0042 : SUB [8 datoshi]
    /// 0043 : SUBSTR [2048 datoshi]
    /// 0044 : STLOC5 [2 datoshi]
    /// 0045 : LDLOC0 [2 datoshi]
    /// 0046 : DUP [2 datoshi]
    /// 0047 : SIZE [4 datoshi]
    /// 0048 : PUSH3 [1 datoshi]
    /// 0049 : SUB [8 datoshi]
    /// 004A : SWAP [2 datoshi]
    /// 004B : PUSH0 [1 datoshi]
    /// 004C : ROT [2 datoshi]
    /// 004D : OVER [2 datoshi]
    /// 004E : SUB [8 datoshi]
    /// 004F : SUBSTR [2048 datoshi]
    /// 0050 : STLOC6 [2 datoshi]
    /// 0051 : LDLOC0 [2 datoshi]
    /// 0052 : DUP [2 datoshi]
    /// 0053 : SIZE [4 datoshi]
    /// 0054 : PUSH4 [1 datoshi]
    /// 0055 : SUB [8 datoshi]
    /// 0056 : SWAP [2 datoshi]
    /// 0057 : PUSH3 [1 datoshi]
    /// 0058 : ROT [2 datoshi]
    /// 0059 : OVER [2 datoshi]
    /// 005A : SUB [8 datoshi]
    /// 005B : SUBSTR [2048 datoshi]
    /// 005C : STLOC 07 [2 datoshi]
    /// 005E : LDLOC0 [2 datoshi]
    /// 005F : DUP [2 datoshi]
    /// 0060 : SIZE [4 datoshi]
    /// 0061 : PUSH2 [1 datoshi]
    /// 0062 : SUB [8 datoshi]
    /// 0063 : SWAP [2 datoshi]
    /// 0064 : DUP [2 datoshi]
    /// 0065 : SIZE [4 datoshi]
    /// 0066 : PUSH4 [1 datoshi]
    /// 0067 : SUB [8 datoshi]
    /// 0068 : ROT [2 datoshi]
    /// 0069 : OVER [2 datoshi]
    /// 006A : SUB [8 datoshi]
    /// 006B : SUBSTR [2048 datoshi]
    /// 006C : STLOC 08 [2 datoshi]
    /// 006E : LDLOC0 [2 datoshi]
    /// 006F : PUSH0 [1 datoshi]
    /// 0070 : PICKITEM [64 datoshi]
    /// 0071 : STLOC 09 [2 datoshi]
    /// 0073 : LDLOC1 [2 datoshi]
    /// 0074 : SIZE [4 datoshi]
    /// 0075 : CALLT 0000 [32768 datoshi]
    /// 0078 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 007D : LDLOC2 [2 datoshi]
    /// 007E : SIZE [4 datoshi]
    /// 007F : CALLT 0000 [32768 datoshi]
    /// 0082 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0087 : LDLOC3 [2 datoshi]
    /// 0088 : SIZE [4 datoshi]
    /// 0089 : CALLT 0000 [32768 datoshi]
    /// 008C : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0091 : LDLOC4 [2 datoshi]
    /// 0092 : SIZE [4 datoshi]
    /// 0093 : CALLT 0000 [32768 datoshi]
    /// 0096 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 009B : LDLOC5 [2 datoshi]
    /// 009C : SIZE [4 datoshi]
    /// 009D : CALLT 0000 [32768 datoshi]
    /// 00A0 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 00A5 : LDLOC6 [2 datoshi]
    /// 00A6 : SIZE [4 datoshi]
    /// 00A7 : CALLT 0000 [32768 datoshi]
    /// 00AA : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 00AF : LDLOC 07 [2 datoshi]
    /// 00B1 : SIZE [4 datoshi]
    /// 00B2 : CALLT 0000 [32768 datoshi]
    /// 00B5 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 00BA : LDLOC 08 [2 datoshi]
    /// 00BC : SIZE [4 datoshi]
    /// 00BD : CALLT 0000 [32768 datoshi]
    /// 00C0 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 00C5 : LDLOC 09 [2 datoshi]
    /// 00C7 : CALLT 0000 [32768 datoshi]
    /// 00CA : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 00CF : PUSHDATA1 313233343536373839 '123456789' [8 datoshi]
    /// 00DA : STLOC 0A [2 datoshi]
    /// 00DC : LDLOC 0A [2 datoshi]
    /// 00DE : DUP [2 datoshi]
    /// 00DF : SIZE [4 datoshi]
    /// 00E0 : SWAP [2 datoshi]
    /// 00E1 : PUSH0 [1 datoshi]
    /// 00E2 : ROT [2 datoshi]
    /// 00E3 : OVER [2 datoshi]
    /// 00E4 : SUB [8 datoshi]
    /// 00E5 : SUBSTR [2048 datoshi]
    /// 00E6 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00E8 : STLOC 0B [2 datoshi]
    /// 00EA : LDLOC 0A [2 datoshi]
    /// 00EC : PUSH3 [1 datoshi]
    /// 00ED : SWAP [2 datoshi]
    /// 00EE : PUSH0 [1 datoshi]
    /// 00EF : ROT [2 datoshi]
    /// 00F0 : OVER [2 datoshi]
    /// 00F1 : SUB [8 datoshi]
    /// 00F2 : SUBSTR [2048 datoshi]
    /// 00F3 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00F5 : STLOC 0C [2 datoshi]
    /// 00F7 : LDLOC 0A [2 datoshi]
    /// 00F9 : DUP [2 datoshi]
    /// 00FA : SIZE [4 datoshi]
    /// 00FB : SWAP [2 datoshi]
    /// 00FC : PUSH2 [1 datoshi]
    /// 00FD : ROT [2 datoshi]
    /// 00FE : OVER [2 datoshi]
    /// 00FF : SUB [8 datoshi]
    /// 0100 : SUBSTR [2048 datoshi]
    /// 0101 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0103 : STLOC 0D [2 datoshi]
    /// 0105 : LDLOC 0A [2 datoshi]
    /// 0107 : PUSH5 [1 datoshi]
    /// 0108 : SWAP [2 datoshi]
    /// 0109 : PUSH3 [1 datoshi]
    /// 010A : ROT [2 datoshi]
    /// 010B : OVER [2 datoshi]
    /// 010C : SUB [8 datoshi]
    /// 010D : SUBSTR [2048 datoshi]
    /// 010E : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0110 : STLOC 0E [2 datoshi]
    /// 0112 : LDLOC 0A [2 datoshi]
    /// 0114 : DUP [2 datoshi]
    /// 0115 : SIZE [4 datoshi]
    /// 0116 : SWAP [2 datoshi]
    /// 0117 : DUP [2 datoshi]
    /// 0118 : SIZE [4 datoshi]
    /// 0119 : PUSH2 [1 datoshi]
    /// 011A : SUB [8 datoshi]
    /// 011B : ROT [2 datoshi]
    /// 011C : OVER [2 datoshi]
    /// 011D : SUB [8 datoshi]
    /// 011E : SUBSTR [2048 datoshi]
    /// 011F : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0121 : STLOC 0F [2 datoshi]
    /// 0123 : LDLOC 0A [2 datoshi]
    /// 0125 : DUP [2 datoshi]
    /// 0126 : SIZE [4 datoshi]
    /// 0127 : PUSH3 [1 datoshi]
    /// 0128 : SUB [8 datoshi]
    /// 0129 : SWAP [2 datoshi]
    /// 012A : PUSH0 [1 datoshi]
    /// 012B : ROT [2 datoshi]
    /// 012C : OVER [2 datoshi]
    /// 012D : SUB [8 datoshi]
    /// 012E : SUBSTR [2048 datoshi]
    /// 012F : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0131 : STLOC 10 [2 datoshi]
    /// 0133 : LDLOC 0A [2 datoshi]
    /// 0135 : DUP [2 datoshi]
    /// 0136 : SIZE [4 datoshi]
    /// 0137 : PUSH4 [1 datoshi]
    /// 0138 : SUB [8 datoshi]
    /// 0139 : SWAP [2 datoshi]
    /// 013A : PUSH3 [1 datoshi]
    /// 013B : ROT [2 datoshi]
    /// 013C : OVER [2 datoshi]
    /// 013D : SUB [8 datoshi]
    /// 013E : SUBSTR [2048 datoshi]
    /// 013F : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0141 : STLOC 11 [2 datoshi]
    /// 0143 : LDLOC 0A [2 datoshi]
    /// 0145 : DUP [2 datoshi]
    /// 0146 : SIZE [4 datoshi]
    /// 0147 : PUSH2 [1 datoshi]
    /// 0148 : SUB [8 datoshi]
    /// 0149 : SWAP [2 datoshi]
    /// 014A : DUP [2 datoshi]
    /// 014B : SIZE [4 datoshi]
    /// 014C : PUSH4 [1 datoshi]
    /// 014D : SUB [8 datoshi]
    /// 014E : ROT [2 datoshi]
    /// 014F : OVER [2 datoshi]
    /// 0150 : SUB [8 datoshi]
    /// 0151 : SUBSTR [2048 datoshi]
    /// 0152 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0154 : STLOC 12 [2 datoshi]
    /// 0156 : LDLOC 0A [2 datoshi]
    /// 0158 : PUSH0 [1 datoshi]
    /// 0159 : PICKITEM [64 datoshi]
    /// 015A : STLOC 13 [2 datoshi]
    /// 015C : LDLOC 0B [2 datoshi]
    /// 015E : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0160 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0165 : LDLOC 0C [2 datoshi]
    /// 0167 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0169 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 016E : LDLOC 0D [2 datoshi]
    /// 0170 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0172 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0177 : LDLOC 0E [2 datoshi]
    /// 0179 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 017B : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0180 : LDLOC 0F [2 datoshi]
    /// 0182 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0184 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0189 : LDLOC 10 [2 datoshi]
    /// 018B : CONVERT 28 'ByteString' [8192 datoshi]
    /// 018D : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0192 : LDLOC 11 [2 datoshi]
    /// 0194 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0196 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 019B : LDLOC 12 [2 datoshi]
    /// 019D : CONVERT 28 'ByteString' [8192 datoshi]
    /// 019F : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 01A4 : LDLOC 13 [2 datoshi]
    /// 01A6 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 01A8 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 01AD : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion
}
