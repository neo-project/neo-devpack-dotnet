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
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : CAT
    // 0006 : CONVERT
    // 0008 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : LDARG0
    // 0006 : LDARG1
    // 0007 : DIV
    // 0008 : APPEND
    // 0009 : DUP
    // 000A : LDARG0
    // 000B : LDARG1
    // 000C : MOD
    // 000D : APPEND
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : CALL
    // 0007 : DUP
    // 0008 : UNPACK
    // 0009 : DROP
    // 000A : STLOC0
    // 000B : STLOC1
    // 000C : DROP
    // 000D : LDLOC1
    // 000E : LDLOC0
    // 000F : CALL
    // 0011 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : SUB
    // 0006 : DUP
    // 0007 : PUSHINT32
    // 000C : JMPGE
    // 000E : JMP
    // 0010 : DUP
    // 0011 : PUSHINT32
    // 0016 : JMPLE
    // 0018 : PUSHINT64
    // 0021 : AND
    // 0022 : DUP
    // 0023 : PUSHINT32
    // 0028 : JMPLE
    // 002A : PUSHINT64
    // 0033 : SUB
    // 0034 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : ADD
    // 0006 : DUP
    // 0007 : PUSHINT32
    // 000C : JMPGE
    // 000E : JMP
    // 0010 : DUP
    // 0011 : PUSHINT32
    // 0016 : JMPLE
    // 0018 : PUSHINT64
    // 0021 : AND
    // 0022 : DUP
    // 0023 : PUSHINT32
    // 0028 : JMPLE
    // 002A : PUSHINT64
    // 0033 : SUB
    // 0034 : RET

    #endregion

}
