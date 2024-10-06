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
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 38
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.LDLOC1
    /// 000B : OpCode.APPEND
    /// 000C : OpCode.LDLOC1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.INC
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 00000080
    /// 0015 : OpCode.JMPGE 04
    /// 0017 : OpCode.JMP 0A
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.PUSHINT32 FFFFFF7F
    /// 001F : OpCode.JMPLE 1E
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.AND
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT32 FFFFFF7F
    /// 0031 : OpCode.JMPLE 0C
    /// 0033 : OpCode.PUSHINT64 0000000001000000
    /// 003C : OpCode.SUB
    /// 003D : OpCode.STLOC1
    /// 003E : OpCode.DROP
    /// 003F : OpCode.LDLOC1
    /// 0040 : OpCode.LDARG0
    /// 0041 : OpCode.LT
    /// 0042 : OpCode.JMPIF C7
    /// 0044 : OpCode.LDLOC0
    /// 0045 : OpCode.CALLT 0000
    /// 0048 : OpCode.RET
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoQA==
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 38
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.LDLOC1
    /// 000B : OpCode.APPEND
    /// 000C : OpCode.LDLOC1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.INC
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 00000080
    /// 0015 : OpCode.JMPGE 04
    /// 0017 : OpCode.JMP 0A
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.PUSHINT32 FFFFFF7F
    /// 001F : OpCode.JMPLE 1E
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.AND
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT32 FFFFFF7F
    /// 0031 : OpCode.JMPLE 0C
    /// 0033 : OpCode.PUSHINT64 0000000001000000
    /// 003C : OpCode.SUB
    /// 003D : OpCode.STLOC1
    /// 003E : OpCode.DROP
    /// 003F : OpCode.LDLOC1
    /// 0040 : OpCode.LDARG0
    /// 0041 : OpCode.LT
    /// 0042 : OpCode.JMPIF C7
    /// 0044 : OpCode.LDLOC0
    /// 0045 : OpCode.RET
    /// </remarks>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdo02g3AABA
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 38
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.LDLOC1
    /// 000B : OpCode.APPEND
    /// 000C : OpCode.LDLOC1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.INC
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 00000080
    /// 0015 : OpCode.JMPGE 04
    /// 0017 : OpCode.JMP 0A
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.PUSHINT32 FFFFFF7F
    /// 001F : OpCode.JMPLE 1E
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.AND
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT32 FFFFFF7F
    /// 0031 : OpCode.JMPLE 0C
    /// 0033 : OpCode.PUSHINT64 0000000001000000
    /// 003C : OpCode.SUB
    /// 003D : OpCode.STLOC1
    /// 003E : OpCode.DROP
    /// 003F : OpCode.LDLOC1
    /// 0040 : OpCode.LDARG0
    /// 0041 : OpCode.LT
    /// 0042 : OpCode.JMPIF C7
    /// 0044 : OpCode.LDLOC0
    /// 0045 : OpCode.CLEARITEMS
    /// 0046 : OpCode.LDLOC0
    /// 0047 : OpCode.CALLT 0000
    /// 004A : OpCode.RET
    /// </remarks>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoykA=
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 38
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.LDLOC1
    /// 000B : OpCode.APPEND
    /// 000C : OpCode.LDLOC1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.INC
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 00000080
    /// 0015 : OpCode.JMPGE 04
    /// 0017 : OpCode.JMP 0A
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.PUSHINT32 FFFFFF7F
    /// 001F : OpCode.JMPLE 1E
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.AND
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT32 FFFFFF7F
    /// 0031 : OpCode.JMPLE 0C
    /// 0033 : OpCode.PUSHINT64 0000000001000000
    /// 003C : OpCode.SUB
    /// 003D : OpCode.STLOC1
    /// 003E : OpCode.DROP
    /// 003F : OpCode.LDLOC1
    /// 0040 : OpCode.LDARG0
    /// 0041 : OpCode.LT
    /// 0042 : OpCode.JMPIF C7
    /// 0044 : OpCode.LDLOC0
    /// 0045 : OpCode.SIZE
    /// 0046 : OpCode.RET
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXi4JhwMSW52YWxpZCB0ZXN0IHBhcmFtZXRlcnM6wnAQcSI4aGnPaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMdoedJoNwAAQA==
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.GE
    /// 0006 : OpCode.JMPIFNOT 1C
    /// 0008 : OpCode.PUSHDATA1 496E76616C6964207465737420706172616D6574657273
    /// 0021 : OpCode.THROW
    /// 0022 : OpCode.NEWARRAY0
    /// 0023 : OpCode.STLOC0
    /// 0024 : OpCode.PUSH0
    /// 0025 : OpCode.STLOC1
    /// 0026 : OpCode.JMP 38
    /// 0028 : OpCode.LDLOC0
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.APPEND
    /// 002B : OpCode.LDLOC1
    /// 002C : OpCode.DUP
    /// 002D : OpCode.INC
    /// 002E : OpCode.DUP
    /// 002F : OpCode.PUSHINT32 00000080
    /// 0034 : OpCode.JMPGE 04
    /// 0036 : OpCode.JMP 0A
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.PUSHINT32 FFFFFF7F
    /// 003E : OpCode.JMPLE 1E
    /// 0040 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0049 : OpCode.AND
    /// 004A : OpCode.DUP
    /// 004B : OpCode.PUSHINT32 FFFFFF7F
    /// 0050 : OpCode.JMPLE 0C
    /// 0052 : OpCode.PUSHINT64 0000000001000000
    /// 005B : OpCode.SUB
    /// 005C : OpCode.STLOC1
    /// 005D : OpCode.DROP
    /// 005E : OpCode.LDLOC1
    /// 005F : OpCode.LDARG0
    /// 0060 : OpCode.LT
    /// 0061 : OpCode.JMPIF C7
    /// 0063 : OpCode.LDLOC0
    /// 0064 : OpCode.LDARG1
    /// 0065 : OpCode.REMOVE
    /// 0066 : OpCode.LDLOC0
    /// 0067 : OpCode.CALLT 0000
    /// 006A : OpCode.RET
    /// </remarks>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);

    #endregion

}
