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
    /// 0000 : OpCode.INITSLOT 0300
    /// 0003 : OpCode.PUSH8
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.SHL
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.PUSHINT32 00000080
    /// 000E : OpCode.JMPGE 04
    /// 0010 : OpCode.JMP 0A
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.PUSHINT32 FFFFFF7F
    /// 0018 : OpCode.JMPLE 1E
    /// 001A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0023 : OpCode.AND
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 0C
    /// 002C : OpCode.PUSHINT64 0000000001000000
    /// 0035 : OpCode.SUB
    /// 0036 : OpCode.STLOC1
    /// 0037 : OpCode.LDLOC0
    /// 0038 : OpCode.PUSH1
    /// 0039 : OpCode.SHR
    /// 003A : OpCode.STLOC2
    /// 003B : OpCode.LDLOC2
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.PUSH2
    /// 003E : OpCode.PACK
    /// 003F : OpCode.RET
    /// </remarks>
    [DisplayName("testShift")]
    public abstract IList<object>? TestShift();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0500
    /// 0003 : OpCode.PUSH8
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.SHL
    /// 0008 : OpCode.STLOC1
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.SHL
    /// 000C : OpCode.STLOC2
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.PUSH1
    /// 000F : OpCode.SHR
    /// 0010 : OpCode.STLOC3
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.SHR
    /// 0014 : OpCode.STLOC4
    /// 0015 : OpCode.LDLOC4
    /// 0016 : OpCode.LDLOC3
    /// 0017 : OpCode.LDLOC2
    /// 0018 : OpCode.LDLOC1
    /// 0019 : OpCode.PUSH4
    /// 001A : OpCode.PACK
    /// 001B : OpCode.RET
    /// </remarks>
    [DisplayName("testShiftBigInt")]
    public abstract IList<object>? TestShiftBigInt();

    #endregion

}
