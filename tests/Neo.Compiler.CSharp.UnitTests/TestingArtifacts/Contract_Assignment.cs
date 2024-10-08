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
    /// Script: VwIAEXBoEZc5EkpxcGgSlzlpEpc5QA==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.ASSERT
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.DUP
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.PUSH2
    /// 0F : OpCode.EQUAL
    /// 10 : OpCode.ASSERT
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.EQUAL
    /// 14 : OpCode.ASSERT
    /// 15 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract void TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2CQFaCIFEUpwRWgRlzlo2CQFaCIFEkpwRWgRlzlA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIF 05
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.JMP 05
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.STLOC0
    /// 0F : OpCode.DROP
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.EQUAL
    /// 13 : OpCode.ASSERT
    /// 14 : OpCode.LDLOC0
    /// 15 : OpCode.ISNULL
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.JMP 05
    /// 1B : OpCode.PUSH2
    /// 1C : OpCode.DUP
    /// 1D : OpCode.STLOC0
    /// 1E : OpCode.DROP
    /// 1F : OpCode.LDLOC0
    /// 20 : OpCode.PUSH1
    /// 21 : OpCode.EQUAL
    /// 22 : OpCode.ASSERT
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("testCoalesceAssignment")]
    public abstract void TestCoalesceAssignment();

    #endregion
}
