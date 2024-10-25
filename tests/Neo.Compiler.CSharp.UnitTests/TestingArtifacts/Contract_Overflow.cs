using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Overflow(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Overflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""mulInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""addUInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":106,""safe"":false},{""name"":""mulUInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":141,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x5d46269ff23ec37de131e4791d5e5c964b140704"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEBxRLllxeHXnkMeF9wz7ynyZGXQl0ZXN0QXJnczEBAAEPBAcUS5ZcXh155DHhfcM+8p8mRl0IdGVzdFZvaWQAAAAPAACwVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ4eaBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5nkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAVwACeHmgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUCnRnyD"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.RET
    /// </remarks>
    [DisplayName("addInt")]
    public abstract BigInteger? AddInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPGE 04
    /// 0A : OpCode.JMP 0E
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 16 : OpCode.JMPLE 0C
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("addUInt")]
    public abstract BigInteger? AddUInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.MUL
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.RET
    /// </remarks>
    [DisplayName("mulInt")]
    public abstract BigInteger? MulInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.MUL
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPGE 04
    /// 0A : OpCode.JMP 0E
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 16 : OpCode.JMPLE 0C
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("mulUInt")]
    public abstract BigInteger? MulUInt(BigInteger? a, BigInteger? b);

    #endregion
}
