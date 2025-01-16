using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TypeConvert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TypeConvert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testType"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGpXCQAQcGjbMHEScmrbMHMMAQPbMHRs2yF1EIh2btshdwcYxAB3CGhKbwgQUdBFaUpvCBFR0EVqSm8IElHQRWtKbwgTUdBFbEpvCBRR0EVtSm8IFVHQRW5KbwgWUdBFbwdKbwgXUdBFbwhABNuLaQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwkAEHBo2zBxEnJq2zBzDAED2zB0bNshdRCIdm7bIXcHGMQAdwhoSm8IEFHQRWlKbwgRUdBFakpvCBJR0EVrSm8IE1HQRWxKbwgUUdBFbUpvCBVR0EVuSm8IFlHQRW8HSm8IF1HQRW8IQA==
    /// INITSLOT 0900 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// STLOC6 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC 07 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// NEWARRAY_T 00 'Any' [512 datoshi]
    /// STLOC 08 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testType")]
    public abstract object? TestType();

    #endregion
}
