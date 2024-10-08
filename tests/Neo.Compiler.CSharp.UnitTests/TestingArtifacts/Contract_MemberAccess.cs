using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_MemberAccess(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_MemberAccess"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAARVcBAAsQEsBKNClwaBDONwAAQc/nR5YMA21zZ0HP50eWaBHOQc/nR5ZoNBZBz+dHlkBXAAF4EQwFaGVsbG/QQFcAAQwAQLz6/NA="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxASwEo0KXBoEM43AABBz+dHlgxtc2dBz+dHlmgRzkHP50eWaDQWQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACK
    /// 07 : OpCode.DUP
    /// 08 : OpCode.CALL 29
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.PUSH0
    /// 0D : OpCode.PICKITEM
    /// 0E : OpCode.CALLT 0000
    /// 11 : OpCode.SYSCALL CFE74796
    /// 16 : OpCode.PUSHDATA1 6D7367
    /// 1B : OpCode.SYSCALL CFE74796
    /// 20 : OpCode.LDLOC0
    /// 21 : OpCode.PUSH1
    /// 22 : OpCode.PICKITEM
    /// 23 : OpCode.SYSCALL CFE74796
    /// 28 : OpCode.LDLOC0
    /// 29 : OpCode.CALL 16
    /// 2B : OpCode.SYSCALL CFE74796
    /// 30 : OpCode.RET
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion
}
