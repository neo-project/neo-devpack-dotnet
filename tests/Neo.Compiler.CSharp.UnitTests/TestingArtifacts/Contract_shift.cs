using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_shift(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_shift"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testShift"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testShiftBigInt"",""parameters"":[],""returntype"":""Array"",""offset"":64,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFxXAwAYcGgRqEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaBGpcmppEsBAVwUAGHBoEKhxaBGocmgRqXNoEql0bGtqaRTAQMjG+94="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAGHBoEahKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWgRqXJqaRLAQA==
    /// 00 : OpCode.INITSLOT 0300 [64 datoshi]
    /// 03 : OpCode.PUSH8 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.SHL [8 datoshi]
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
    /// 36 : OpCode.STLOC1 [2 datoshi]
    /// 37 : OpCode.LDLOC0 [2 datoshi]
    /// 38 : OpCode.PUSH1 [1 datoshi]
    /// 39 : OpCode.SHR [8 datoshi]
    /// 3A : OpCode.STLOC2 [2 datoshi]
    /// 3B : OpCode.LDLOC2 [2 datoshi]
    /// 3C : OpCode.LDLOC1 [2 datoshi]
    /// 3D : OpCode.PUSH2 [1 datoshi]
    /// 3E : OpCode.PACK [2048 datoshi]
    /// 3F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAGHBoEKhxaBGocmgRqXNoEql0bGtqaRTAQA==
    /// 00 : OpCode.INITSLOT 0500 [64 datoshi]
    /// 03 : OpCode.PUSH8 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSH0 [1 datoshi]
    /// 07 : OpCode.SHL [8 datoshi]
    /// 08 : OpCode.STLOC1 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.SHL [8 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.PUSH1 [1 datoshi]
    /// 0F : OpCode.SHR [8 datoshi]
    /// 10 : OpCode.STLOC3 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.PUSH2 [1 datoshi]
    /// 13 : OpCode.SHR [8 datoshi]
    /// 14 : OpCode.STLOC4 [2 datoshi]
    /// 15 : OpCode.LDLOC4 [2 datoshi]
    /// 16 : OpCode.LDLOC3 [2 datoshi]
    /// 17 : OpCode.LDLOC2 [2 datoshi]
    /// 18 : OpCode.LDLOC1 [2 datoshi]
    /// 19 : OpCode.PUSH4 [1 datoshi]
    /// 1A : OpCode.PACK [2048 datoshi]
    /// 1B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();

    #endregion
}
