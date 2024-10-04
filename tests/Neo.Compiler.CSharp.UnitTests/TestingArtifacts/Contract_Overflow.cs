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
    [DisplayName("addInt")]
    public abstract BigInteger? AddInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPGE 04
    /// 000A : OpCode.JMP 0E
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0016 : OpCode.JMPLE 0C
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("addUInt")]
    public abstract BigInteger? AddUInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.MUL
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
    [DisplayName("mulInt")]
    public abstract BigInteger? MulInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.MUL
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPGE 04
    /// 000A : OpCode.JMP 0E
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0016 : OpCode.JMPLE 0C
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("mulUInt")]
    public abstract BigInteger? MulUInt(BigInteger? a, BigInteger? b);

    #endregion

}
