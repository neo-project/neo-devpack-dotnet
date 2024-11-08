using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABDwAA/YwBVwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoykBXAgHCcBBxIjhoac9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkx2g3AABAVwICeXi4JhwMF0ludmFsaWQgdGVzdCBwYXJhbWV0ZXJzOsJwEHEiOGhpz2lKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTHaHnSaDcAAEBXAgHCcBBxIjhoac9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkx2jTaDcAAEBXAgHCcBBxIjhoac9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkx2hARsVhnQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoNwAAQA==
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 38 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 15 : OpCode.JMPGE 04 [2 datoshi]
    /// 17 : OpCode.JMP 0A [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : OpCode.JMPLE 1E [2 datoshi]
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : OpCode.AND [8 datoshi]
    /// 2B : OpCode.DUP [2 datoshi]
    /// 2C : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : OpCode.JMPLE 0C [2 datoshi]
    /// 33 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : OpCode.SUB [8 datoshi]
    /// 3D : OpCode.STLOC1 [2 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDARG0 [2 datoshi]
    /// 41 : OpCode.LT [8 datoshi]
    /// 42 : OpCode.JMPIF C7 [2 datoshi]
    /// 44 : OpCode.LDLOC0 [2 datoshi]
    /// 45 : OpCode.CALLT 0000 [32768 datoshi]
    /// 48 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoQA==
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 38 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 15 : OpCode.JMPGE 04 [2 datoshi]
    /// 17 : OpCode.JMP 0A [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : OpCode.JMPLE 1E [2 datoshi]
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : OpCode.AND [8 datoshi]
    /// 2B : OpCode.DUP [2 datoshi]
    /// 2C : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : OpCode.JMPLE 0C [2 datoshi]
    /// 33 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : OpCode.SUB [8 datoshi]
    /// 3D : OpCode.STLOC1 [2 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDARG0 [2 datoshi]
    /// 41 : OpCode.LT [8 datoshi]
    /// 42 : OpCode.JMPIF C7 [2 datoshi]
    /// 44 : OpCode.LDLOC0 [2 datoshi]
    /// 45 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdo02g3AABA
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 38 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 15 : OpCode.JMPGE 04 [2 datoshi]
    /// 17 : OpCode.JMP 0A [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : OpCode.JMPLE 1E [2 datoshi]
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : OpCode.AND [8 datoshi]
    /// 2B : OpCode.DUP [2 datoshi]
    /// 2C : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : OpCode.JMPLE 0C [2 datoshi]
    /// 33 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : OpCode.SUB [8 datoshi]
    /// 3D : OpCode.STLOC1 [2 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDARG0 [2 datoshi]
    /// 41 : OpCode.LT [8 datoshi]
    /// 42 : OpCode.JMPIF C7 [2 datoshi]
    /// 44 : OpCode.LDLOC0 [2 datoshi]
    /// 45 : OpCode.CLEARITEMS [16 datoshi]
    /// 46 : OpCode.LDLOC0 [2 datoshi]
    /// 47 : OpCode.CALLT 0000 [32768 datoshi]
    /// 4A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoykA=
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 38 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 15 : OpCode.JMPGE 04 [2 datoshi]
    /// 17 : OpCode.JMP 0A [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : OpCode.JMPLE 1E [2 datoshi]
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : OpCode.AND [8 datoshi]
    /// 2B : OpCode.DUP [2 datoshi]
    /// 2C : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : OpCode.JMPLE 0C [2 datoshi]
    /// 33 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : OpCode.SUB [8 datoshi]
    /// 3D : OpCode.STLOC1 [2 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDARG0 [2 datoshi]
    /// 41 : OpCode.LT [8 datoshi]
    /// 42 : OpCode.JMPIF C7 [2 datoshi]
    /// 44 : OpCode.LDLOC0 [2 datoshi]
    /// 45 : OpCode.SIZE [4 datoshi]
    /// 46 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXi4JhwMF0ludmFsaWQgdGVzdCBwYXJhbWV0ZXJzOsJwEHEiOGhpz2lKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTHaHnSaDcAAEA=
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.GE [8 datoshi]
    /// 06 : OpCode.JMPIFNOT 1C [2 datoshi]
    /// 08 : OpCode.PUSHDATA1 496E76616C6964207465737420706172616D6574657273 [8 datoshi]
    /// 21 : OpCode.THROW [512 datoshi]
    /// 22 : OpCode.NEWARRAY0 [16 datoshi]
    /// 23 : OpCode.STLOC0 [2 datoshi]
    /// 24 : OpCode.PUSH0 [1 datoshi]
    /// 25 : OpCode.STLOC1 [2 datoshi]
    /// 26 : OpCode.JMP 38 [2 datoshi]
    /// 28 : OpCode.LDLOC0 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.APPEND [8192 datoshi]
    /// 2B : OpCode.LDLOC1 [2 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.INC [4 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 34 : OpCode.JMPGE 04 [2 datoshi]
    /// 36 : OpCode.JMP 0A [2 datoshi]
    /// 38 : OpCode.DUP [2 datoshi]
    /// 39 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : OpCode.JMPLE 1E [2 datoshi]
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : OpCode.AND [8 datoshi]
    /// 4A : OpCode.DUP [2 datoshi]
    /// 4B : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : OpCode.JMPLE 0C [2 datoshi]
    /// 52 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : OpCode.SUB [8 datoshi]
    /// 5C : OpCode.STLOC1 [2 datoshi]
    /// 5D : OpCode.DROP [2 datoshi]
    /// 5E : OpCode.LDLOC1 [2 datoshi]
    /// 5F : OpCode.LDARG0 [2 datoshi]
    /// 60 : OpCode.LT [8 datoshi]
    /// 61 : OpCode.JMPIF C7 [2 datoshi]
    /// 63 : OpCode.LDLOC0 [2 datoshi]
    /// 64 : OpCode.LDARG1 [2 datoshi]
    /// 65 : OpCode.REMOVE [16 datoshi]
    /// 66 : OpCode.LDLOC0 [2 datoshi]
    /// 67 : OpCode.CALLT 0000 [32768 datoshi]
    /// 6A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);

    #endregion
}
