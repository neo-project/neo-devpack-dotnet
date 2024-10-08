using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPVXBQEQcHhKccpyEHMiO2lrznRobJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMVoQMI0shERwDStnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8TEhLANXj///+eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxUUEsA1QP///55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQAs6n8M="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: wjSyERHANK2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxMSEsA1eP///55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfFRQSwDVA////nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// 00 : OpCode.NEWARRAY0
    /// 01 : OpCode.CALL B2
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.PUSH1
    /// 05 : OpCode.PACK
    /// 06 : OpCode.CALL AD
    /// 08 : OpCode.ADD
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSHINT32 00000080
    /// 0F : OpCode.JMPGE 04
    /// 11 : OpCode.JMP 0A
    /// 13 : OpCode.DUP
    /// 14 : OpCode.PUSHINT32 FFFFFF7F
    /// 19 : OpCode.JMPLE 1E
    /// 1B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 24 : OpCode.AND
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSHINT32 FFFFFF7F
    /// 2B : OpCode.JMPLE 0C
    /// 2D : OpCode.PUSHINT64 0000000001000000
    /// 36 : OpCode.SUB
    /// 37 : OpCode.PUSH3
    /// 38 : OpCode.PUSH2
    /// 39 : OpCode.PUSH2
    /// 3A : OpCode.PACK
    /// 3B : OpCode.CALL_L 78FFFFFF
    /// 40 : OpCode.ADD
    /// 41 : OpCode.DUP
    /// 42 : OpCode.PUSHINT32 00000080
    /// 47 : OpCode.JMPGE 04
    /// 49 : OpCode.JMP 0A
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSHINT32 FFFFFF7F
    /// 51 : OpCode.JMPLE 1E
    /// 53 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 5C : OpCode.AND
    /// 5D : OpCode.DUP
    /// 5E : OpCode.PUSHINT32 FFFFFF7F
    /// 63 : OpCode.JMPLE 0C
    /// 65 : OpCode.PUSHINT64 0000000001000000
    /// 6E : OpCode.SUB
    /// 6F : OpCode.PUSH5
    /// 70 : OpCode.PUSH4
    /// 71 : OpCode.PUSH2
    /// 72 : OpCode.PACK
    /// 73 : OpCode.CALL_L 40FFFFFF
    /// 78 : OpCode.ADD
    /// 79 : OpCode.DUP
    /// 7A : OpCode.PUSHINT32 00000080
    /// 7F : OpCode.JMPGE 04
    /// 81 : OpCode.JMP 0A
    /// 83 : OpCode.DUP
    /// 84 : OpCode.PUSHINT32 FFFFFF7F
    /// 89 : OpCode.JMPLE 1E
    /// 8B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 94 : OpCode.AND
    /// 95 : OpCode.DUP
    /// 96 : OpCode.PUSHINT32 FFFFFF7F
    /// 9B : OpCode.JMPLE 0C
    /// 9D : OpCode.PUSHINT64 0000000001000000
    /// A6 : OpCode.SUB
    /// A7 : OpCode.RET
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    #endregion
}
