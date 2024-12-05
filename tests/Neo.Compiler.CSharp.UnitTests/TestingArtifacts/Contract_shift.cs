using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFxXAwAYcGgRqEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaBGpcmppEsBAVwUAGHBoEKhxaBGocmgRqXNoEql0bGtqaRTAQMjG+94=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAGHBoEahKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWgRqXJqaRLAQA==
    /// 00 : INITSLOT 0300 [64 datoshi]
    /// 03 : PUSH8 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : SHL [8 datoshi]
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
    /// 36 : STLOC1 [2 datoshi]
    /// 37 : LDLOC0 [2 datoshi]
    /// 38 : PUSH1 [1 datoshi]
    /// 39 : SHR [8 datoshi]
    /// 3A : STLOC2 [2 datoshi]
    /// 3B : LDLOC2 [2 datoshi]
    /// 3C : LDLOC1 [2 datoshi]
    /// 3D : PUSH2 [1 datoshi]
    /// 3E : PACK [2048 datoshi]
    /// 3F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAGHBoEKhxaBGocmgRqXNoEql0bGtqaRTAQA==
    /// 00 : INITSLOT 0500 [64 datoshi]
    /// 03 : PUSH8 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : SHL [8 datoshi]
    /// 08 : STLOC1 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : SHL [8 datoshi]
    /// 0C : STLOC2 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSH1 [1 datoshi]
    /// 0F : SHR [8 datoshi]
    /// 10 : STLOC3 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : PUSH2 [1 datoshi]
    /// 13 : SHR [8 datoshi]
    /// 14 : STLOC4 [2 datoshi]
    /// 15 : LDLOC4 [2 datoshi]
    /// 16 : LDLOC3 [2 datoshi]
    /// 17 : LDLOC2 [2 datoshi]
    /// 18 : LDLOC1 [2 datoshi]
    /// 19 : PUSH4 [1 datoshi]
    /// 1A : PACK [2048 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();

    #endregion
}
