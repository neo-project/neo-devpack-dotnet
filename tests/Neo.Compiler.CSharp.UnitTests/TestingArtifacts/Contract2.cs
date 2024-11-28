using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract2(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract2"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_002"",""parameters"":[{""name"":""arg1"",""type"":""Any""},{""name"":""arg2"",""type"":""Any""}],""returntype"":""Integer"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABBXAQIMBAECAwTbMHBoEs5AckchYg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDAQBAgME2zBwaBLOQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSHDATA1 01020304 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : PUSH2 [1 datoshi]
    /// 0E : PICKITEM [64 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_002")]
    public abstract BigInteger? UnitTest_002(object? arg1, object? arg2 = null);

    #endregion
}
