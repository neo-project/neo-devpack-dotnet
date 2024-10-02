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
    [DisplayName("testAssignment")]
    public abstract void TestAssignment();
    // 0000 : INITSLOT
    // 0003 : PUSH1
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSH1
    // 0007 : EQUAL
    // 0008 : ASSERT
    // 0009 : PUSH2
    // 000A : DUP
    // 000B : STLOC1
    // 000C : STLOC0
    // 000D : LDLOC0
    // 000E : PUSH2
    // 000F : EQUAL
    // 0010 : ASSERT
    // 0011 : LDLOC1
    // 0012 : PUSH2
    // 0013 : EQUAL
    // 0014 : ASSERT
    // 0015 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCoalesceAssignment")]
    public abstract void TestCoalesceAssignment();
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : ISNULL
    // 0007 : JMPIF
    // 0009 : LDLOC0
    // 000A : JMP
    // 000C : PUSH1
    // 000D : DUP
    // 000E : STLOC0
    // 000F : DROP
    // 0010 : LDLOC0
    // 0011 : PUSH1
    // 0012 : EQUAL
    // 0013 : ASSERT
    // 0014 : LDLOC0
    // 0015 : ISNULL
    // 0016 : JMPIF
    // 0018 : LDLOC0
    // 0019 : JMP
    // 001B : PUSH2
    // 001C : DUP
    // 001D : STLOC0
    // 001E : DROP
    // 001F : LDLOC0
    // 0020 : PUSH1
    // 0021 : EQUAL
    // 0022 : ASSERT
    // 0023 : RET

    #endregion

}
