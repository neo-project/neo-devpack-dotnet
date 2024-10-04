using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssignment"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testCoalesceAssignment"",""parameters"":[],""returntype"":""Void"",""offset"":22,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADpXAgARcGgRlzkSSnFwaBKXOWkSlzlAVwEAC3Bo2CQFaCIFEUpwRWgRlzlo2CQFaCIFEkpwRWgRlzlAj66LMw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH1
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.ASSERT
    /// 0009 : OpCode.PUSH2
    /// 000A : OpCode.DUP
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.PUSH2
    /// 000F : OpCode.EQUAL
    /// 0010 : OpCode.ASSERT
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.EQUAL
    /// 0014 : OpCode.ASSERT
    /// 0015 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract void TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.ISNULL
    /// 0007 : OpCode.JMPIF 05
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.JMP 05
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.STLOC0
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.EQUAL
    /// 0013 : OpCode.ASSERT
    /// 0014 : OpCode.LDLOC0
    /// 0015 : OpCode.ISNULL
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.LDLOC0
    /// 0019 : OpCode.JMP 05
    /// 001B : OpCode.PUSH2
    /// 001C : OpCode.DUP
    /// 001D : OpCode.STLOC0
    /// 001E : OpCode.DROP
    /// 001F : OpCode.LDLOC0
    /// 0020 : OpCode.PUSH1
    /// 0021 : OpCode.EQUAL
    /// 0022 : OpCode.ASSERT
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("testCoalesceAssignment")]
    public abstract void TestCoalesceAssignment();

    #endregion

}
