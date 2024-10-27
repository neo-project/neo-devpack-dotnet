using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABBXAQIMBAECAwTbMHBoEs5AckchYg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDAECAwTbMHBoEs5A
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 01020304 [8 datoshi]
    /// 09 : OpCode.CONVERT 'Buffer' [8192 datoshi]
    /// 0B : OpCode.STLOC0 [2 datoshi]
    /// 0C : OpCode.LDLOC0 [2 datoshi]
    /// 0D : OpCode.PUSH2 [1 datoshi]
    /// 0E : OpCode.PICKITEM [64 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_002")]
    public abstract BigInteger? UnitTest_002(object? arg1, object? arg2 = null);

    #endregion
}
