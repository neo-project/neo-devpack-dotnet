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
    /// 00 : OpCode.INITSLOT 0200	[64 datoshi]
    /// 03 : OpCode.PUSH1	[1 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSH1	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.ASSERT	[1 datoshi]
    /// 09 : OpCode.PUSH2	[1 datoshi]
    /// 0A : OpCode.DUP	[2 datoshi]
    /// 0B : OpCode.STLOC1	[2 datoshi]
    /// 0C : OpCode.STLOC0	[2 datoshi]
    /// 0D : OpCode.LDLOC0	[2 datoshi]
    /// 0E : OpCode.PUSH2	[1 datoshi]
    /// 0F : OpCode.EQUAL	[32 datoshi]
    /// 10 : OpCode.ASSERT	[1 datoshi]
    /// 11 : OpCode.LDLOC1	[2 datoshi]
    /// 12 : OpCode.PUSH2	[1 datoshi]
    /// 13 : OpCode.EQUAL	[32 datoshi]
    /// 14 : OpCode.ASSERT	[1 datoshi]
    /// 15 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract void TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2CQFaCIFEUpwRWgRlzlo2CQFaCIFEkpwRWgRlzlA
    /// 00 : OpCode.INITSLOT 0100	[64 datoshi]
    /// 03 : OpCode.PUSHNULL	[1 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.ISNULL	[2 datoshi]
    /// 07 : OpCode.JMPIF 05	[2 datoshi]
    /// 09 : OpCode.LDLOC0	[2 datoshi]
    /// 0A : OpCode.JMP 05	[2 datoshi]
    /// 0C : OpCode.PUSH1	[1 datoshi]
    /// 0D : OpCode.DUP	[2 datoshi]
    /// 0E : OpCode.STLOC0	[2 datoshi]
    /// 0F : OpCode.DROP	[2 datoshi]
    /// 10 : OpCode.LDLOC0	[2 datoshi]
    /// 11 : OpCode.PUSH1	[1 datoshi]
    /// 12 : OpCode.EQUAL	[32 datoshi]
    /// 13 : OpCode.ASSERT	[1 datoshi]
    /// 14 : OpCode.LDLOC0	[2 datoshi]
    /// 15 : OpCode.ISNULL	[2 datoshi]
    /// 16 : OpCode.JMPIF 05	[2 datoshi]
    /// 18 : OpCode.LDLOC0	[2 datoshi]
    /// 19 : OpCode.JMP 05	[2 datoshi]
    /// 1B : OpCode.PUSH2	[1 datoshi]
    /// 1C : OpCode.DUP	[2 datoshi]
    /// 1D : OpCode.STLOC0	[2 datoshi]
    /// 1E : OpCode.DROP	[2 datoshi]
    /// 1F : OpCode.LDLOC0	[2 datoshi]
    /// 20 : OpCode.PUSH1	[1 datoshi]
    /// 21 : OpCode.EQUAL	[32 datoshi]
    /// 22 : OpCode.ASSERT	[1 datoshi]
    /// 23 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testCoalesceAssignment")]
    public abstract void TestCoalesceAssignment();

    #endregion
}
