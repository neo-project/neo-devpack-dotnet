using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Returns(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Returns"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""subtract"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""div"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Array"",""offset"":106,""safe"":false},{""name"":""mix"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":121,""safe"":false},{""name"":""byteStringAdd"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":139,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJRXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5n0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACxUp4eaHPSnh5os9AVwICeXg07ErBRXBxRWloNK1AVwACeHmL2yhAGjay3A=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.CAT
    /// 0006 : OpCode.CONVERT 28
    /// 0008 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.LDARG1
    /// 0007 : OpCode.DIV
    /// 0008 : OpCode.APPEND
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.MOD
    /// 000D : OpCode.APPEND
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.CALL EC
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.UNPACK
    /// 0009 : OpCode.DROP
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.DROP
    /// 000D : OpCode.LDLOC1
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.CALL AD
    /// 0011 : OpCode.RET
    /// </remarks>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.SUB
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.RET
    /// </remarks>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.RET
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    #endregion

}
