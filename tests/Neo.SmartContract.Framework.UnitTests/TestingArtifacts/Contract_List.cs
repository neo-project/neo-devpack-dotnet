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
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 38
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 00000080
    /// 15 : OpCode.JMPGE 04
    /// 17 : OpCode.JMP 0A
    /// 19 : OpCode.DUP
    /// 1A : OpCode.PUSHINT32 FFFFFF7F
    /// 1F : OpCode.JMPLE 1E
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.AND
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT32 FFFFFF7F
    /// 31 : OpCode.JMPLE 0C
    /// 33 : OpCode.PUSHINT64 0000000001000000
    /// 3C : OpCode.SUB
    /// 3D : OpCode.STLOC1
    /// 3E : OpCode.DROP
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDARG0
    /// 41 : OpCode.LT
    /// 42 : OpCode.JMPIF C7
    /// 44 : OpCode.LDLOC0
    /// 45 : OpCode.CALLT 0000
    /// 48 : OpCode.RET
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 38
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 00000080
    /// 15 : OpCode.JMPGE 04
    /// 17 : OpCode.JMP 0A
    /// 19 : OpCode.DUP
    /// 1A : OpCode.PUSHINT32 FFFFFF7F
    /// 1F : OpCode.JMPLE 1E
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.AND
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT32 FFFFFF7F
    /// 31 : OpCode.JMPLE 0C
    /// 33 : OpCode.PUSHINT64 0000000001000000
    /// 3C : OpCode.SUB
    /// 3D : OpCode.STLOC1
    /// 3E : OpCode.DROP
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDARG0
    /// 41 : OpCode.LT
    /// 42 : OpCode.JMPIF C7
    /// 44 : OpCode.LDLOC0
    /// 45 : OpCode.RET
    /// </remarks>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdo02g3AABA
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 38
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 00000080
    /// 15 : OpCode.JMPGE 04
    /// 17 : OpCode.JMP 0A
    /// 19 : OpCode.DUP
    /// 1A : OpCode.PUSHINT32 FFFFFF7F
    /// 1F : OpCode.JMPLE 1E
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.AND
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT32 FFFFFF7F
    /// 31 : OpCode.JMPLE 0C
    /// 33 : OpCode.PUSHINT64 0000000001000000
    /// 3C : OpCode.SUB
    /// 3D : OpCode.STLOC1
    /// 3E : OpCode.DROP
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDARG0
    /// 41 : OpCode.LT
    /// 42 : OpCode.JMPIF C7
    /// 44 : OpCode.LDLOC0
    /// 45 : OpCode.CLEARITEMS
    /// 46 : OpCode.LDLOC0
    /// 47 : OpCode.CALLT 0000
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoykA=
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 38
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 00000080
    /// 15 : OpCode.JMPGE 04
    /// 17 : OpCode.JMP 0A
    /// 19 : OpCode.DUP
    /// 1A : OpCode.PUSHINT32 FFFFFF7F
    /// 1F : OpCode.JMPLE 1E
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.AND
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT32 FFFFFF7F
    /// 31 : OpCode.JMPLE 0C
    /// 33 : OpCode.PUSHINT64 0000000001000000
    /// 3C : OpCode.SUB
    /// 3D : OpCode.STLOC1
    /// 3E : OpCode.DROP
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDARG0
    /// 41 : OpCode.LT
    /// 42 : OpCode.JMPIF C7
    /// 44 : OpCode.LDLOC0
    /// 45 : OpCode.SIZE
    /// 46 : OpCode.RET
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXi4JhwMSW52YWxpZCB0ZXN0IHBhcmFtZXRlcnM6wnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoedJoNwAAQA==
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.GE
    /// 06 : OpCode.JMPIFNOT 1C
    /// 08 : OpCode.PUSHDATA1 496E76616C6964207465737420706172616D6574657273
    /// 21 : OpCode.THROW
    /// 22 : OpCode.NEWARRAY0
    /// 23 : OpCode.STLOC0
    /// 24 : OpCode.PUSH0
    /// 25 : OpCode.STLOC1
    /// 26 : OpCode.JMP 38
    /// 28 : OpCode.LDLOC0
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.APPEND
    /// 2B : OpCode.LDLOC1
    /// 2C : OpCode.DUP
    /// 2D : OpCode.INC
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSHINT32 00000080
    /// 34 : OpCode.JMPGE 04
    /// 36 : OpCode.JMP 0A
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 1E
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 49 : OpCode.AND
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSHINT32 FFFFFF7F
    /// 50 : OpCode.JMPLE 0C
    /// 52 : OpCode.PUSHINT64 0000000001000000
    /// 5B : OpCode.SUB
    /// 5C : OpCode.STLOC1
    /// 5D : OpCode.DROP
    /// 5E : OpCode.LDLOC1
    /// 5F : OpCode.LDARG0
    /// 60 : OpCode.LT
    /// 61 : OpCode.JMPIF C7
    /// 63 : OpCode.LDLOC0
    /// 64 : OpCode.LDARG1
    /// 65 : OpCode.REMOVE
    /// 66 : OpCode.LDLOC0
    /// 67 : OpCode.CALLT 0000
    /// 6A : OpCode.RET
    /// </remarks>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);

    #endregion
}
