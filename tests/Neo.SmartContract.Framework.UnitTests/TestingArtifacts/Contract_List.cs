using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_List(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_List"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCount"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAdd"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""String"",""offset"":71,""safe"":false},{""name"":""testRemoveAt"",""parameters"":[{""name"":""count"",""type"":""Integer""},{""name"":""removeAt"",""type"":""Integer""}],""returntype"":""String"",""offset"":144,""safe"":false},{""name"":""testClear"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""String"",""offset"":251,""safe"":false},{""name"":""testArrayConvert"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Array"",""offset"":326,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""jsonSerialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABDwAA/YwBVwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoykBXAgHCcBBxIjhoac9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkx2g3AABAVwICeXi4JhwMF0ludmFsaWQgdGVzdCBwYXJhbWV0ZXJzOsJwEHEiOGhpz2lKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTHaHnSaDcAAEBXAgHCcBBxIjhoac9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkx2jTaDcAAEBXAgHCcBBxIjhoac9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkx2hARsVhnQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoNwAAQA==
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 38 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : LDLOC1 [2 datoshi]
    /// 0B : APPEND [8192 datoshi]
    /// 0C : LDLOC1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 00000080 [1 datoshi]
    /// 15 : JMPGE 04 [2 datoshi]
    /// 17 : JMP 0A [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : JMPLE 1E [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : JMPLE 0C [2 datoshi]
    /// 33 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : STLOC1 [2 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDARG0 [2 datoshi]
    /// 41 : LT [8 datoshi]
    /// 42 : JMPIF C7 [2 datoshi]
    /// 44 : LDLOC0 [2 datoshi]
    /// 45 : CALLT 0000 [32768 datoshi]
    /// 48 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoQA==
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 38 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : LDLOC1 [2 datoshi]
    /// 0B : APPEND [8192 datoshi]
    /// 0C : LDLOC1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 00000080 [1 datoshi]
    /// 15 : JMPGE 04 [2 datoshi]
    /// 17 : JMP 0A [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : JMPLE 1E [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : JMPLE 0C [2 datoshi]
    /// 33 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : STLOC1 [2 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDARG0 [2 datoshi]
    /// 41 : LT [8 datoshi]
    /// 42 : JMPIF C7 [2 datoshi]
    /// 44 : LDLOC0 [2 datoshi]
    /// 45 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdo02g3AABA
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 38 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : LDLOC1 [2 datoshi]
    /// 0B : APPEND [8192 datoshi]
    /// 0C : LDLOC1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 00000080 [1 datoshi]
    /// 15 : JMPGE 04 [2 datoshi]
    /// 17 : JMP 0A [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : JMPLE 1E [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : JMPLE 0C [2 datoshi]
    /// 33 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : STLOC1 [2 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDARG0 [2 datoshi]
    /// 41 : LT [8 datoshi]
    /// 42 : JMPIF C7 [2 datoshi]
    /// 44 : LDLOC0 [2 datoshi]
    /// 45 : CLEARITEMS [16 datoshi]
    /// 46 : LDLOC0 [2 datoshi]
    /// 47 : CALLT 0000 [32768 datoshi]
    /// 4A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoykA=
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 38 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : LDLOC1 [2 datoshi]
    /// 0B : APPEND [8192 datoshi]
    /// 0C : LDLOC1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 00000080 [1 datoshi]
    /// 15 : JMPGE 04 [2 datoshi]
    /// 17 : JMP 0A [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : JMPLE 1E [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : JMPLE 0C [2 datoshi]
    /// 33 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : STLOC1 [2 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDARG0 [2 datoshi]
    /// 41 : LT [8 datoshi]
    /// 42 : JMPIF C7 [2 datoshi]
    /// 44 : LDLOC0 [2 datoshi]
    /// 45 : SIZE [4 datoshi]
    /// 46 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXi4JhwMF0ludmFsaWQgdGVzdCBwYXJhbWV0ZXJzOsJwEHEiOGhpz2lKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTHaHnSaDcAAEA=
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : GE [8 datoshi]
    /// 06 : JMPIFNOT 1C [2 datoshi]
    /// 08 : PUSHDATA1 496E76616C6964207465737420706172616D6574657273 [8 datoshi]
    /// 21 : THROW [512 datoshi]
    /// 22 : NEWARRAY0 [16 datoshi]
    /// 23 : STLOC0 [2 datoshi]
    /// 24 : PUSH0 [1 datoshi]
    /// 25 : STLOC1 [2 datoshi]
    /// 26 : JMP 38 [2 datoshi]
    /// 28 : LDLOC0 [2 datoshi]
    /// 29 : LDLOC1 [2 datoshi]
    /// 2A : APPEND [8192 datoshi]
    /// 2B : LDLOC1 [2 datoshi]
    /// 2C : DUP [2 datoshi]
    /// 2D : INC [4 datoshi]
    /// 2E : DUP [2 datoshi]
    /// 2F : PUSHINT32 00000080 [1 datoshi]
    /// 34 : JMPGE 04 [2 datoshi]
    /// 36 : JMP 0A [2 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 1E [2 datoshi]
    /// 40 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : AND [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : JMPLE 0C [2 datoshi]
    /// 52 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : SUB [8 datoshi]
    /// 5C : STLOC1 [2 datoshi]
    /// 5D : DROP [2 datoshi]
    /// 5E : LDLOC1 [2 datoshi]
    /// 5F : LDARG0 [2 datoshi]
    /// 60 : LT [8 datoshi]
    /// 61 : JMPIF C7 [2 datoshi]
    /// 63 : LDLOC0 [2 datoshi]
    /// 64 : LDARG1 [2 datoshi]
    /// 65 : REMOVE [16 datoshi]
    /// 66 : LDLOC0 [2 datoshi]
    /// 67 : CALLT 0000 [32768 datoshi]
    /// 6A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);

    #endregion
}
