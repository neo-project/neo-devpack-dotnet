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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAAOlcBAAwFaGVsbG8QEsBwaBDONwAAQc/nR5YMA21zZ0HP50eWaBHOQc/nR5ZoNAhBz+dHlkBXAAEMAEA44zQf"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAVoZWxsbxASwHBoEM43AABBz+dHlgwDbXNnQc/nR5ZoEc5Bz+dHlmg0CEHP50eWQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : CALLT 0000 [32768 datoshi]
    /// 14 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 19 : PUSHDATA1 6D7367 'msg' [8 datoshi]
    /// 1E : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 23 : LDLOC0 [2 datoshi]
    /// 24 : PUSH1 [1 datoshi]
    /// 25 : PICKITEM [64 datoshi]
    /// 26 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 2B : LDLOC0 [2 datoshi]
    /// 2C : CALL 08 [512 datoshi]
    /// 2E : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion
}
