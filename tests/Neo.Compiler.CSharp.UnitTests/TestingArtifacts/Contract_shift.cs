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
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH8
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.SHL
    /// 08 : OpCode.DUP
    /// 09 : OpCode.PUSHINT32 00000080
    /// 0E : OpCode.JMPGE 04
    /// 10 : OpCode.JMP 0A
    /// 12 : OpCode.DUP
    /// 13 : OpCode.PUSHINT32 FFFFFF7F
    /// 18 : OpCode.JMPLE 1E
    /// 1A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 23 : OpCode.AND
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 0C
    /// 2C : OpCode.PUSHINT64 0000000001000000
    /// 35 : OpCode.SUB
    /// 36 : OpCode.STLOC1
    /// 37 : OpCode.LDLOC0
    /// 38 : OpCode.PUSH1
    /// 39 : OpCode.SHR
    /// 3A : OpCode.STLOC2
    /// 3B : OpCode.LDLOC2
    /// 3C : OpCode.LDLOC1
    /// 3D : OpCode.PUSH2
    /// 3E : OpCode.PACK
    /// 3F : OpCode.RET
    /// </remarks>
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAGHBoEKhxaBGocmgRqXNoEql0bGtqaRTAQA==
    /// 00 : OpCode.INITSLOT 0500
    /// 03 : OpCode.PUSH8
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.SHL
    /// 08 : OpCode.STLOC1
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.SHL
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.PUSH1
    /// 0F : OpCode.SHR
    /// 10 : OpCode.STLOC3
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.SHR
    /// 14 : OpCode.STLOC4
    /// 15 : OpCode.LDLOC4
    /// 16 : OpCode.LDLOC3
    /// 17 : OpCode.LDLOC2
    /// 18 : OpCode.LDLOC1
    /// 19 : OpCode.PUSH4
    /// 1A : OpCode.PACK
    /// 1B : OpCode.RET
    /// </remarks>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();

    #endregion
}
