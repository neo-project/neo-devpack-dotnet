using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Params(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Params"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Integer"",""offset"":77,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPVXBQEQcHhKccpyEHMiO2lrznRobJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMVoQMI0shERwDStnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8TEhLANXj///+eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxUUEsA1QP///55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQAs6n8M=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: wjSyERHANK2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxMSEsA1eP///55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfFRQSwDVA////nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// 00 : NEWARRAY0 [16 datoshi]
    /// 01 : CALL B2 [512 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : PACK [2048 datoshi]
    /// 06 : CALL AD [512 datoshi]
    /// 08 : ADD [8 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSHINT32 00000080 [1 datoshi]
    /// 0F : JMPGE 04 [2 datoshi]
    /// 11 : JMP 0A [2 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 19 : JMPLE 1E [2 datoshi]
    /// 1B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 24 : AND [8 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2B : JMPLE 0C [2 datoshi]
    /// 2D : PUSHINT64 0000000001000000 [1 datoshi]
    /// 36 : SUB [8 datoshi]
    /// 37 : PUSH3 [1 datoshi]
    /// 38 : PUSH2 [1 datoshi]
    /// 39 : PUSH2 [1 datoshi]
    /// 3A : PACK [2048 datoshi]
    /// 3B : CALL_L 78FFFFFF [512 datoshi]
    /// 40 : ADD [8 datoshi]
    /// 41 : DUP [2 datoshi]
    /// 42 : PUSHINT32 00000080 [1 datoshi]
    /// 47 : JMPGE 04 [2 datoshi]
    /// 49 : JMP 0A [2 datoshi]
    /// 4B : DUP [2 datoshi]
    /// 4C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 51 : JMPLE 1E [2 datoshi]
    /// 53 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 5C : AND [8 datoshi]
    /// 5D : DUP [2 datoshi]
    /// 5E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 63 : JMPLE 0C [2 datoshi]
    /// 65 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 6E : SUB [8 datoshi]
    /// 6F : PUSH5 [1 datoshi]
    /// 70 : PUSH4 [1 datoshi]
    /// 71 : PUSH2 [1 datoshi]
    /// 72 : PACK [2048 datoshi]
    /// 73 : CALL_L 40FFFFFF [512 datoshi]
    /// 78 : ADD [8 datoshi]
    /// 79 : DUP [2 datoshi]
    /// 7A : PUSHINT32 00000080 [1 datoshi]
    /// 7F : JMPGE 04 [2 datoshi]
    /// 81 : JMP 0A [2 datoshi]
    /// 83 : DUP [2 datoshi]
    /// 84 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 89 : JMPLE 1E [2 datoshi]
    /// 8B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 94 : AND [8 datoshi]
    /// 95 : DUP [2 datoshi]
    /// 96 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 9B : JMPLE 0C [2 datoshi]
    /// 9D : PUSHINT64 0000000001000000 [1 datoshi]
    /// A6 : SUB [8 datoshi]
    /// A7 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    #endregion
}
