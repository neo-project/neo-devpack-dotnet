using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADpXAgARcGgRlzkSSnFwaBKXOWkSlzlAVwEAC3Bo2CQFaCIFEUpwRWgRlzlo2CQFaCIFEkpwRWgRlzlAj66LMw==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEXBoEZc5EkpxcGgSlzlpEpc5QA==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : EQUAL [32 datoshi]
    /// 08 : ASSERT [1 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : STLOC1 [2 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : EQUAL [32 datoshi]
    /// 10 : ASSERT [1 datoshi]
    /// 11 : LDLOC1 [2 datoshi]
    /// 12 : PUSH2 [1 datoshi]
    /// 13 : EQUAL [32 datoshi]
    /// 14 : ASSERT [1 datoshi]
    /// 15 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract void TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2CQFaCIFEUpwRWgRlzlo2CQFaCIFEkpwRWgRlzlA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIF 05 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : JMP 05 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : STLOC0 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : EQUAL [32 datoshi]
    /// 13 : ASSERT [1 datoshi]
    /// 14 : LDLOC0 [2 datoshi]
    /// 15 : ISNULL [2 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : LDLOC0 [2 datoshi]
    /// 19 : JMP 05 [2 datoshi]
    /// 1B : PUSH2 [1 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : STLOC0 [2 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : LDLOC0 [2 datoshi]
    /// 20 : PUSH1 [1 datoshi]
    /// 21 : EQUAL [32 datoshi]
    /// 22 : ASSERT [1 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCoalesceAssignment")]
    public abstract void TestCoalesceAssignment();

    #endregion
}
