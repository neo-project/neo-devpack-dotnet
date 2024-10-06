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
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.PUSH2
    /// 0006 : OpCode.PACK
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.CALL 29
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.PUSH0
    /// 000D : OpCode.PICKITEM
    /// 000E : OpCode.CALLT 0000
    /// 0011 : OpCode.SYSCALL CFE74796
    /// 0016 : OpCode.PUSHDATA1 6D7367
    /// 001B : OpCode.SYSCALL CFE74796
    /// 0020 : OpCode.LDLOC0
    /// 0021 : OpCode.PUSH1
    /// 0022 : OpCode.PICKITEM
    /// 0023 : OpCode.SYSCALL CFE74796
    /// 0028 : OpCode.LDLOC0
    /// 0029 : OpCode.CALL 16
    /// 002B : OpCode.SYSCALL CFE74796
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion

}
