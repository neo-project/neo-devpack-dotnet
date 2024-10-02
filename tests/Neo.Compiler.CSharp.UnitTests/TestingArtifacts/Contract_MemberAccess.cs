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
    [DisplayName("testMain")]
    public abstract void TestMain();
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : PUSH0
    // 0005 : PUSH2
    // 0006 : PACK
    // 0007 : DUP
    // 0008 : CALL
    // 000A : STLOC0
    // 000B : LDLOC0
    // 000C : PUSH0
    // 000D : PICKITEM
    // 000E : CALLT
    // 0011 : SYSCALL
    // 0016 : PUSHDATA1
    // 001B : SYSCALL
    // 0020 : LDLOC0
    // 0021 : PUSH1
    // 0022 : PICKITEM
    // 0023 : SYSCALL
    // 0028 : LDLOC0
    // 0029 : CALL
    // 002B : SYSCALL
    // 0030 : RET

    #endregion

}
