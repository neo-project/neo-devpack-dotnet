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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAASVcBAAsQEsBKNClwaBDONwAAQc/nR5YMA21zZ0HP50eWaBHOQc/nR5ZoNBpBz+dHlkBXAAF4EBDQeBEMBWhlbGxv0EBXAAEMAEChO7M3"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxASwEo0KXBoEM43AABBz+dHlgwDbXNnQc/nR5ZoEc5Bz+dHlmg0GkHP50eWQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : CALL 29 [512 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : PICKITEM [64 datoshi]
    /// 0E : CALLT 0000 [32768 datoshi]
    /// 11 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 16 : PUSHDATA1 6D7367 'msg' [8 datoshi]
    /// 1B : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 20 : LDLOC0 [2 datoshi]
    /// 21 : PUSH1 [1 datoshi]
    /// 22 : PICKITEM [64 datoshi]
    /// 23 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 28 : LDLOC0 [2 datoshi]
    /// 29 : CALL 1A [512 datoshi]
    /// 2B : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion
}
