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
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : LDLOC0
    // 000A : LDLOC1
    // 000B : APPEND
    // 000C : LDLOC1
    // 000D : DUP
    // 000E : INC
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPGE
    // 0017 : JMP
    // 0019 : DUP
    // 001A : PUSHINT32
    // 001F : JMPLE
    // 0021 : PUSHINT64
    // 002A : AND
    // 002B : DUP
    // 002C : PUSHINT32
    // 0031 : JMPLE
    // 0033 : PUSHINT64
    // 003C : SUB
    // 003D : STLOC1
    // 003E : DROP
    // 003F : LDLOC1
    // 0040 : LDARG0
    // 0041 : LT
    // 0042 : JMPIF
    // 0044 : LDLOC0
    // 0045 : CALLT
    // 0048 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : LDLOC0
    // 000A : LDLOC1
    // 000B : APPEND
    // 000C : LDLOC1
    // 000D : DUP
    // 000E : INC
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPGE
    // 0017 : JMP
    // 0019 : DUP
    // 001A : PUSHINT32
    // 001F : JMPLE
    // 0021 : PUSHINT64
    // 002A : AND
    // 002B : DUP
    // 002C : PUSHINT32
    // 0031 : JMPLE
    // 0033 : PUSHINT64
    // 003C : SUB
    // 003D : STLOC1
    // 003E : DROP
    // 003F : LDLOC1
    // 0040 : LDARG0
    // 0041 : LT
    // 0042 : JMPIF
    // 0044 : LDLOC0
    // 0045 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : LDLOC0
    // 000A : LDLOC1
    // 000B : APPEND
    // 000C : LDLOC1
    // 000D : DUP
    // 000E : INC
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPGE
    // 0017 : JMP
    // 0019 : DUP
    // 001A : PUSHINT32
    // 001F : JMPLE
    // 0021 : PUSHINT64
    // 002A : AND
    // 002B : DUP
    // 002C : PUSHINT32
    // 0031 : JMPLE
    // 0033 : PUSHINT64
    // 003C : SUB
    // 003D : STLOC1
    // 003E : DROP
    // 003F : LDLOC1
    // 0040 : LDARG0
    // 0041 : LT
    // 0042 : JMPIF
    // 0044 : LDLOC0
    // 0045 : CLEARITEMS
    // 0046 : LDLOC0
    // 0047 : CALLT
    // 004A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : LDLOC0
    // 000A : LDLOC1
    // 000B : APPEND
    // 000C : LDLOC1
    // 000D : DUP
    // 000E : INC
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPGE
    // 0017 : JMP
    // 0019 : DUP
    // 001A : PUSHINT32
    // 001F : JMPLE
    // 0021 : PUSHINT64
    // 002A : AND
    // 002B : DUP
    // 002C : PUSHINT32
    // 0031 : JMPLE
    // 0033 : PUSHINT64
    // 003C : SUB
    // 003D : STLOC1
    // 003E : DROP
    // 003F : LDLOC1
    // 0040 : LDARG0
    // 0041 : LT
    // 0042 : JMPIF
    // 0044 : LDLOC0
    // 0045 : SIZE
    // 0046 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : GE
    // 0006 : JMPIFNOT
    // 0008 : PUSHDATA1
    // 0021 : THROW
    // 0022 : NEWARRAY0
    // 0023 : STLOC0
    // 0024 : PUSH0
    // 0025 : STLOC1
    // 0026 : JMP
    // 0028 : LDLOC0
    // 0029 : LDLOC1
    // 002A : APPEND
    // 002B : LDLOC1
    // 002C : DUP
    // 002D : INC
    // 002E : DUP
    // 002F : PUSHINT32
    // 0034 : JMPGE
    // 0036 : JMP
    // 0038 : DUP
    // 0039 : PUSHINT32
    // 003E : JMPLE
    // 0040 : PUSHINT64
    // 0049 : AND
    // 004A : DUP
    // 004B : PUSHINT32
    // 0050 : JMPLE
    // 0052 : PUSHINT64
    // 005B : SUB
    // 005C : STLOC1
    // 005D : DROP
    // 005E : LDLOC1
    // 005F : LDARG0
    // 0060 : LT
    // 0061 : JMPIF
    // 0063 : LDLOC0
    // 0064 : LDARG1
    // 0065 : REMOVE
    // 0066 : LDLOC0
    // 0067 : CALLT
    // 006A : RET

    #endregion

}
