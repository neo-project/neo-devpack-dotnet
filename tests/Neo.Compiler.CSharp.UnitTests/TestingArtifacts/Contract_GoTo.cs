using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIpXAQARcGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgTlybKaEBXAgARcDtAAGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgTlyYFaD0JPQVxPQIivUACEBmj"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoE5cmymhA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.INC [4 datoshi]
    /// 08 : OpCode.DUP [2 datoshi]
    /// 09 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0E : OpCode.JMPGE 04 [2 datoshi]
    /// 10 : OpCode.JMP 0A [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 18 : OpCode.JMPLE 1E [2 datoshi]
    /// 1A : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 23 : OpCode.AND [8 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : OpCode.JMPLE 0C [2 datoshi]
    /// 2C : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 35 : OpCode.SUB [8 datoshi]
    /// 36 : OpCode.STLOC0 [2 datoshi]
    /// 37 : OpCode.DROP [2 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.PUSH3 [1 datoshi]
    /// 3A : OpCode.EQUAL [32 datoshi]
    /// 3B : OpCode.JMPIFNOT CA [2 datoshi]
    /// 3D : OpCode.LDLOC0 [2 datoshi]
    /// 3E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXA7QABoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoE5cmBWg9CT0FcT0CIr1A
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 4000 [4 datoshi]
    /// 08 : OpCode.LDLOC0 [2 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.INC [4 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 11 : OpCode.JMPGE 04 [2 datoshi]
    /// 13 : OpCode.JMP 0A [2 datoshi]
    /// 15 : OpCode.DUP [2 datoshi]
    /// 16 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1B : OpCode.JMPLE 1E [2 datoshi]
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : OpCode.AND [8 datoshi]
    /// 27 : OpCode.DUP [2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : OpCode.JMPLE 0C [2 datoshi]
    /// 2F : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 38 : OpCode.SUB [8 datoshi]
    /// 39 : OpCode.STLOC0 [2 datoshi]
    /// 3A : OpCode.DROP [2 datoshi]
    /// 3B : OpCode.LDLOC0 [2 datoshi]
    /// 3C : OpCode.PUSH3 [1 datoshi]
    /// 3D : OpCode.EQUAL [32 datoshi]
    /// 3E : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 40 : OpCode.LDLOC0 [2 datoshi]
    /// 41 : OpCode.ENDTRY 09 [4 datoshi]
    /// 43 : OpCode.ENDTRY 05 [4 datoshi]
    /// 45 : OpCode.STLOC1 [2 datoshi]
    /// 46 : OpCode.ENDTRY 02 [4 datoshi]
    /// 48 : OpCode.JMP BD [2 datoshi]
    /// 4A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTry")]
    public abstract BigInteger? TestTry();

    #endregion
}
