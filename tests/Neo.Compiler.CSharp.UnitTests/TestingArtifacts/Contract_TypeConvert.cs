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
    /// 00 : INITSLOT 0900 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 08 : STLOC1 [2 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : STLOC2 [2 datoshi]
    /// 0B : LDLOC2 [2 datoshi]
    /// 0C : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0E : STLOC3 [2 datoshi]
    /// 0F : PUSHDATA1 03 [8 datoshi]
    /// 12 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 14 : STLOC4 [2 datoshi]
    /// 15 : LDLOC4 [2 datoshi]
    /// 16 : CONVERT 21 'Integer' [8192 datoshi]
    /// 18 : STLOC5 [2 datoshi]
    /// 19 : PUSH0 [1 datoshi]
    /// 1A : NEWBUFFER [256 datoshi]
    /// 1B : STLOC6 [2 datoshi]
    /// 1C : LDLOC6 [2 datoshi]
    /// 1D : CONVERT 21 'Integer' [8192 datoshi]
    /// 1F : STLOC 07 [2 datoshi]
    /// 21 : PUSH8 [1 datoshi]
    /// 22 : NEWARRAY_T 00 'Any' [512 datoshi]
    /// 24 : STLOC 08 [2 datoshi]
    /// 26 : LDLOC0 [2 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : LDLOC 08 [2 datoshi]
    /// 2A : PUSH0 [1 datoshi]
    /// 2B : ROT [2 datoshi]
    /// 2C : SETITEM [8192 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : LDLOC1 [2 datoshi]
    /// 2F : DUP [2 datoshi]
    /// 30 : LDLOC 08 [2 datoshi]
    /// 32 : PUSH1 [1 datoshi]
    /// 33 : ROT [2 datoshi]
    /// 34 : SETITEM [8192 datoshi]
    /// 35 : DROP [2 datoshi]
    /// 36 : LDLOC2 [2 datoshi]
    /// 37 : DUP [2 datoshi]
    /// 38 : LDLOC 08 [2 datoshi]
    /// 3A : PUSH2 [1 datoshi]
    /// 3B : ROT [2 datoshi]
    /// 3C : SETITEM [8192 datoshi]
    /// 3D : DROP [2 datoshi]
    /// 3E : LDLOC3 [2 datoshi]
    /// 3F : DUP [2 datoshi]
    /// 40 : LDLOC 08 [2 datoshi]
    /// 42 : PUSH3 [1 datoshi]
    /// 43 : ROT [2 datoshi]
    /// 44 : SETITEM [8192 datoshi]
    /// 45 : DROP [2 datoshi]
    /// 46 : LDLOC4 [2 datoshi]
    /// 47 : DUP [2 datoshi]
    /// 48 : LDLOC 08 [2 datoshi]
    /// 4A : PUSH4 [1 datoshi]
    /// 4B : ROT [2 datoshi]
    /// 4C : SETITEM [8192 datoshi]
    /// 4D : DROP [2 datoshi]
    /// 4E : LDLOC5 [2 datoshi]
    /// 4F : DUP [2 datoshi]
    /// 50 : LDLOC 08 [2 datoshi]
    /// 52 : PUSH5 [1 datoshi]
    /// 53 : ROT [2 datoshi]
    /// 54 : SETITEM [8192 datoshi]
    /// 55 : DROP [2 datoshi]
    /// 56 : LDLOC6 [2 datoshi]
    /// 57 : DUP [2 datoshi]
    /// 58 : LDLOC 08 [2 datoshi]
    /// 5A : PUSH6 [1 datoshi]
    /// 5B : ROT [2 datoshi]
    /// 5C : SETITEM [8192 datoshi]
    /// 5D : DROP [2 datoshi]
    /// 5E : LDLOC 07 [2 datoshi]
    /// 60 : DUP [2 datoshi]
    /// 61 : LDLOC 08 [2 datoshi]
    /// 63 : PUSH7 [1 datoshi]
    /// 64 : ROT [2 datoshi]
    /// 65 : SETITEM [8192 datoshi]
    /// 66 : DROP [2 datoshi]
    /// 67 : LDLOC 08 [2 datoshi]
    /// 69 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testType")]
    public abstract object? TestType();

    #endregion
}
