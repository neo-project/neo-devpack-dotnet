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
    /// 00 : OpCode.NEWARRAY0 	-> 16 datoshi
    /// 01 : OpCode.CALL B2 	-> 512 datoshi
    /// 03 : OpCode.PUSH1 	-> 1 datoshi
    /// 04 : OpCode.PUSH1 	-> 1 datoshi
    /// 05 : OpCode.PACK 	-> 2048 datoshi
    /// 06 : OpCode.CALL AD 	-> 512 datoshi
    /// 08 : OpCode.ADD 	-> 8 datoshi
    /// 09 : OpCode.DUP 	-> 2 datoshi
    /// 0A : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 0F : OpCode.JMPGE 04 	-> 2 datoshi
    /// 11 : OpCode.JMP 0A 	-> 2 datoshi
    /// 13 : OpCode.DUP 	-> 2 datoshi
    /// 14 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 19 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 1B : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 24 : OpCode.AND 	-> 8 datoshi
    /// 25 : OpCode.DUP 	-> 2 datoshi
    /// 26 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 2B : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 2D : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 36 : OpCode.SUB 	-> 8 datoshi
    /// 37 : OpCode.PUSH3 	-> 1 datoshi
    /// 38 : OpCode.PUSH2 	-> 1 datoshi
    /// 39 : OpCode.PUSH2 	-> 1 datoshi
    /// 3A : OpCode.PACK 	-> 2048 datoshi
    /// 3B : OpCode.CALL_L 78FFFFFF 	-> 512 datoshi
    /// 40 : OpCode.ADD 	-> 8 datoshi
    /// 41 : OpCode.DUP 	-> 2 datoshi
    /// 42 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 47 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 49 : OpCode.JMP 0A 	-> 2 datoshi
    /// 4B : OpCode.DUP 	-> 2 datoshi
    /// 4C : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 51 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 53 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 5C : OpCode.AND 	-> 8 datoshi
    /// 5D : OpCode.DUP 	-> 2 datoshi
    /// 5E : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 63 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 65 : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 6E : OpCode.SUB 	-> 8 datoshi
    /// 6F : OpCode.PUSH5 	-> 1 datoshi
    /// 70 : OpCode.PUSH4 	-> 1 datoshi
    /// 71 : OpCode.PUSH2 	-> 1 datoshi
    /// 72 : OpCode.PACK 	-> 2048 datoshi
    /// 73 : OpCode.CALL_L 40FFFFFF 	-> 512 datoshi
    /// 78 : OpCode.ADD 	-> 8 datoshi
    /// 79 : OpCode.DUP 	-> 2 datoshi
    /// 7A : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 7F : OpCode.JMPGE 04 	-> 2 datoshi
    /// 81 : OpCode.JMP 0A 	-> 2 datoshi
    /// 83 : OpCode.DUP 	-> 2 datoshi
    /// 84 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 89 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 8B : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 94 : OpCode.AND 	-> 8 datoshi
    /// 95 : OpCode.DUP 	-> 2 datoshi
    /// 96 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 9B : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 9D : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// A6 : OpCode.SUB 	-> 8 datoshi
    /// A7 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    #endregion
}
