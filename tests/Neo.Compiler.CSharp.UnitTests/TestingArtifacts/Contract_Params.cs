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
    [DisplayName("test")]
    public abstract BigInteger? Test();
    // 0000 : NEWARRAY0
    // 0001 : CALL
    // 0003 : PUSH1
    // 0004 : PUSH1
    // 0005 : PACK
    // 0006 : CALL
    // 0008 : ADD
    // 0009 : DUP
    // 000A : PUSHINT32
    // 000F : JMPGE
    // 0011 : JMP
    // 0013 : DUP
    // 0014 : PUSHINT32
    // 0019 : JMPLE
    // 001B : PUSHINT64
    // 0024 : AND
    // 0025 : DUP
    // 0026 : PUSHINT32
    // 002B : JMPLE
    // 002D : PUSHINT64
    // 0036 : SUB
    // 0037 : PUSH3
    // 0038 : PUSH2
    // 0039 : PUSH2
    // 003A : PACK
    // 003B : CALL_L
    // 0040 : ADD
    // 0041 : DUP
    // 0042 : PUSHINT32
    // 0047 : JMPGE
    // 0049 : JMP
    // 004B : DUP
    // 004C : PUSHINT32
    // 0051 : JMPLE
    // 0053 : PUSHINT64
    // 005C : AND
    // 005D : DUP
    // 005E : PUSHINT32
    // 0063 : JMPLE
    // 0065 : PUSHINT64
    // 006E : SUB
    // 006F : PUSH5
    // 0070 : PUSH4
    // 0071 : PUSH2
    // 0072 : PACK
    // 0073 : CALL_L
    // 0078 : ADD
    // 0079 : DUP
    // 007A : PUSHINT32
    // 007F : JMPGE
    // 0081 : JMP
    // 0083 : DUP
    // 0084 : PUSHINT32
    // 0089 : JMPLE
    // 008B : PUSHINT64
    // 0094 : AND
    // 0095 : DUP
    // 0096 : PUSHINT32
    // 009B : JMPLE
    // 009D : PUSHINT64
    // 00A6 : SUB
    // 00A7 : RET

    #endregion

}
