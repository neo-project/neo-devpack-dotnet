using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_GoTo(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_GoTo"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testTry"",""parameters"":[],""returntype"":""Integer"",""offset"":63,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIpXAQARcGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgTlybKaEBXAgARcDtAAGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgTlyYFaD0JPQVxPQIivUACEBmj").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoE5cmymhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : PUSHINT32 00000080 [1 datoshi]
    /// 0E : JMPGE 04 [2 datoshi]
    /// 10 : JMP 0A [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 18 : JMPLE 1E [2 datoshi]
    /// 1A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 23 : AND [8 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 0C [2 datoshi]
    /// 2C : PUSHINT64 0000000001000000 [1 datoshi]
    /// 35 : SUB [8 datoshi]
    /// 36 : STLOC0 [2 datoshi]
    /// 37 : DROP [2 datoshi]
    /// 38 : LDLOC0 [2 datoshi]
    /// 39 : PUSH3 [1 datoshi]
    /// 3A : EQUAL [32 datoshi]
    /// 3B : JMPIFNOT CA [2 datoshi]
    /// 3D : LDLOC0 [2 datoshi]
    /// 3E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXA7QABoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoE5cmBWg9CT0FcT0CIr1A
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : TRY 4000 [4 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : INC [4 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT32 00000080 [1 datoshi]
    /// 11 : JMPGE 04 [2 datoshi]
    /// 13 : JMP 0A [2 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1B : JMPLE 1E [2 datoshi]
    /// 1D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : AND [8 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : JMPLE 0C [2 datoshi]
    /// 2F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 38 : SUB [8 datoshi]
    /// 39 : STLOC0 [2 datoshi]
    /// 3A : DROP [2 datoshi]
    /// 3B : LDLOC0 [2 datoshi]
    /// 3C : PUSH3 [1 datoshi]
    /// 3D : EQUAL [32 datoshi]
    /// 3E : JMPIFNOT 05 [2 datoshi]
    /// 40 : LDLOC0 [2 datoshi]
    /// 41 : ENDTRY 09 [4 datoshi]
    /// 43 : ENDTRY 05 [4 datoshi]
    /// 45 : STLOC1 [2 datoshi]
    /// 46 : ENDTRY 02 [4 datoshi]
    /// 48 : JMP BD [2 datoshi]
    /// 4A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTry")]
    public abstract BigInteger? TestTry();

    #endregion
}
