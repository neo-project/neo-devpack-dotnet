using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnh5oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUBXAAJ4eaBKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQFd1rUY=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("addInt")]
    public abstract BigInteger? AddInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPGE 04 [2 datoshi]
    /// 0A : JMP 0E [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 16 : JMPLE 0C [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("addUInt")]
    public abstract BigInteger? AddUInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : MUL [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mulInt")]
    public abstract BigInteger? MulInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : MUL [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPGE 04 [2 datoshi]
    /// 0A : JMP 0E [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 16 : JMPLE 0C [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mulUInt")]
    public abstract BigInteger? MulUInt(BigInteger? a, BigInteger? b);

    #endregion
}
