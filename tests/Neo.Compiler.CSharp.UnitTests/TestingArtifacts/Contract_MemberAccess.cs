using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_MemberAccess(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_MemberAccess"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":118,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAAe1cBAAwFaGVsbG8QEsBwaBDONwAAQc/nR5YMA21zZ0HP50eWaBHOQc/nR5ZoNERBz+dHlhBgWBCXOVgRnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gWBGXOUBXAAEMAEBWARNgQLlKFjs="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAVoZWxsbxASwHBoEM43AABBz+dHlgwDbXNnQc/nR5ZoEc5Bz+dHlmg0REHP50eWEGBYEJc5WBGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2BYEZc5QA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// PUSHDATA1 6D7367 'msg' [8 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 44 [512 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion
}
