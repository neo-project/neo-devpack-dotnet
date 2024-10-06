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
    /// 0000 : OpCode.NEWARRAY0
    /// 0001 : OpCode.CALL B2
    /// 0003 : OpCode.PUSH1
    /// 0004 : OpCode.PUSH1
    /// 0005 : OpCode.PACK
    /// 0006 : OpCode.CALL AD
    /// 0008 : OpCode.ADD
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSHINT32 00000080
    /// 000F : OpCode.JMPGE 04
    /// 0011 : OpCode.JMP 0A
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.PUSHINT32 FFFFFF7F
    /// 0019 : OpCode.JMPLE 1E
    /// 001B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0024 : OpCode.AND
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSHINT32 FFFFFF7F
    /// 002B : OpCode.JMPLE 0C
    /// 002D : OpCode.PUSHINT64 0000000001000000
    /// 0036 : OpCode.SUB
    /// 0037 : OpCode.PUSH3
    /// 0038 : OpCode.PUSH2
    /// 0039 : OpCode.PUSH2
    /// 003A : OpCode.PACK
    /// 003B : OpCode.CALL_L 78FFFFFF
    /// 0040 : OpCode.ADD
    /// 0041 : OpCode.DUP
    /// 0042 : OpCode.PUSHINT32 00000080
    /// 0047 : OpCode.JMPGE 04
    /// 0049 : OpCode.JMP 0A
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSHINT32 FFFFFF7F
    /// 0051 : OpCode.JMPLE 1E
    /// 0053 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 005C : OpCode.AND
    /// 005D : OpCode.DUP
    /// 005E : OpCode.PUSHINT32 FFFFFF7F
    /// 0063 : OpCode.JMPLE 0C
    /// 0065 : OpCode.PUSHINT64 0000000001000000
    /// 006E : OpCode.SUB
    /// 006F : OpCode.PUSH5
    /// 0070 : OpCode.PUSH4
    /// 0071 : OpCode.PUSH2
    /// 0072 : OpCode.PACK
    /// 0073 : OpCode.CALL_L 40FFFFFF
    /// 0078 : OpCode.ADD
    /// 0079 : OpCode.DUP
    /// 007A : OpCode.PUSHINT32 00000080
    /// 007F : OpCode.JMPGE 04
    /// 0081 : OpCode.JMP 0A
    /// 0083 : OpCode.DUP
    /// 0084 : OpCode.PUSHINT32 FFFFFF7F
    /// 0089 : OpCode.JMPLE 1E
    /// 008B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0094 : OpCode.AND
    /// 0095 : OpCode.DUP
    /// 0096 : OpCode.PUSHINT32 FFFFFF7F
    /// 009B : OpCode.JMPLE 0C
    /// 009D : OpCode.PUSHINT64 0000000001000000
    /// 00A6 : OpCode.SUB
    /// 00A7 : OpCode.RET
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    #endregion

}
