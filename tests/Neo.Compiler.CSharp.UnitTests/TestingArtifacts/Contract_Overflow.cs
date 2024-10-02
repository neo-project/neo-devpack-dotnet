using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Overflow(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Overflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""mulInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""addUInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":106,""safe"":false},{""name"":""mulUInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":141,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUBXAAJ4eaBKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQFd1rUY="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addInt")]
    public abstract BigInteger? AddInt(BigInteger? a, BigInteger? b);
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

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addUInt")]
    public abstract BigInteger? AddUInt(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : ADD
    // 0006 : DUP
    // 0007 : PUSH0
    // 0008 : JMPGE
    // 000A : JMP
    // 000C : DUP
    // 000D : PUSHINT64
    // 0016 : JMPLE
    // 0018 : PUSHINT64
    // 0021 : AND
    // 0022 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mulInt")]
    public abstract BigInteger? MulInt(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : MUL
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
    [DisplayName("mulUInt")]
    public abstract BigInteger? MulUInt(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : MUL
    // 0006 : DUP
    // 0007 : PUSH0
    // 0008 : JMPGE
    // 000A : JMP
    // 000C : DUP
    // 000D : PUSHINT64
    // 0016 : JMPLE
    // 0018 : PUSHINT64
    // 0021 : AND
    // 0022 : RET

    #endregion

}
