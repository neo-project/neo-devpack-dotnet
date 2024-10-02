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
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();
    // 0000 : INITSLOT
    // 0003 : PUSH8
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSH1
    // 0007 : SHL
    // 0008 : DUP
    // 0009 : PUSHINT32
    // 000E : JMPGE
    // 0010 : JMP
    // 0012 : DUP
    // 0013 : PUSHINT32
    // 0018 : JMPLE
    // 001A : PUSHINT64
    // 0023 : AND
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : PUSHINT64
    // 0035 : SUB
    // 0036 : STLOC1
    // 0037 : LDLOC0
    // 0038 : PUSH1
    // 0039 : SHR
    // 003A : STLOC2
    // 003B : LDLOC2
    // 003C : LDLOC1
    // 003D : PUSH2
    // 003E : PACK
    // 003F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();
    // 0000 : INITSLOT
    // 0003 : PUSH8
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSH0
    // 0007 : SHL
    // 0008 : STLOC1
    // 0009 : LDLOC0
    // 000A : PUSH1
    // 000B : SHL
    // 000C : STLOC2
    // 000D : LDLOC0
    // 000E : PUSH1
    // 000F : SHR
    // 0010 : STLOC3
    // 0011 : LDLOC0
    // 0012 : PUSH2
    // 0013 : SHR
    // 0014 : STLOC4
    // 0015 : LDLOC4
    // 0016 : LDLOC3
    // 0017 : LDLOC2
    // 0018 : LDLOC1
    // 0019 : PUSH4
    // 001A : PACK
    // 001B : RET

    #endregion

}
