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
    /// 00 : OpCode.INITSLOT 0900 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 08 : OpCode.STLOC1 [2 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.STLOC2 [2 datoshi]
    /// 0B : OpCode.LDLOC2 [2 datoshi]
    /// 0C : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 0E : OpCode.STLOC3 [2 datoshi]
    /// 0F : OpCode.PUSHDATA1 03 [8 datoshi]
    /// 12 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 14 : OpCode.STLOC4 [2 datoshi]
    /// 15 : OpCode.LDLOC4 [2 datoshi]
    /// 16 : OpCode.CONVERT 21 'Integer' [8192 datoshi]
    /// 18 : OpCode.STLOC5 [2 datoshi]
    /// 19 : OpCode.PUSH0 [1 datoshi]
    /// 1A : OpCode.NEWBUFFER [256 datoshi]
    /// 1B : OpCode.STLOC6 [2 datoshi]
    /// 1C : OpCode.LDLOC6 [2 datoshi]
    /// 1D : OpCode.CONVERT 21 'Integer' [8192 datoshi]
    /// 1F : OpCode.STLOC 07 [2 datoshi]
    /// 21 : OpCode.PUSH8 [1 datoshi]
    /// 22 : OpCode.NEWARRAY_T 00 [512 datoshi]
    /// 24 : OpCode.STLOC 08 [2 datoshi]
    /// 26 : OpCode.LDLOC0 [2 datoshi]
    /// 27 : OpCode.DUP [2 datoshi]
    /// 28 : OpCode.LDLOC 08 [2 datoshi]
    /// 2A : OpCode.PUSH0 [1 datoshi]
    /// 2B : OpCode.ROT [2 datoshi]
    /// 2C : OpCode.SETITEM [8192 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.LDLOC1 [2 datoshi]
    /// 2F : OpCode.DUP [2 datoshi]
    /// 30 : OpCode.LDLOC 08 [2 datoshi]
    /// 32 : OpCode.PUSH1 [1 datoshi]
    /// 33 : OpCode.ROT [2 datoshi]
    /// 34 : OpCode.SETITEM [8192 datoshi]
    /// 35 : OpCode.DROP [2 datoshi]
    /// 36 : OpCode.LDLOC2 [2 datoshi]
    /// 37 : OpCode.DUP [2 datoshi]
    /// 38 : OpCode.LDLOC 08 [2 datoshi]
    /// 3A : OpCode.PUSH2 [1 datoshi]
    /// 3B : OpCode.ROT [2 datoshi]
    /// 3C : OpCode.SETITEM [8192 datoshi]
    /// 3D : OpCode.DROP [2 datoshi]
    /// 3E : OpCode.LDLOC3 [2 datoshi]
    /// 3F : OpCode.DUP [2 datoshi]
    /// 40 : OpCode.LDLOC 08 [2 datoshi]
    /// 42 : OpCode.PUSH3 [1 datoshi]
    /// 43 : OpCode.ROT [2 datoshi]
    /// 44 : OpCode.SETITEM [8192 datoshi]
    /// 45 : OpCode.DROP [2 datoshi]
    /// 46 : OpCode.LDLOC4 [2 datoshi]
    /// 47 : OpCode.DUP [2 datoshi]
    /// 48 : OpCode.LDLOC 08 [2 datoshi]
    /// 4A : OpCode.PUSH4 [1 datoshi]
    /// 4B : OpCode.ROT [2 datoshi]
    /// 4C : OpCode.SETITEM [8192 datoshi]
    /// 4D : OpCode.DROP [2 datoshi]
    /// 4E : OpCode.LDLOC5 [2 datoshi]
    /// 4F : OpCode.DUP [2 datoshi]
    /// 50 : OpCode.LDLOC 08 [2 datoshi]
    /// 52 : OpCode.PUSH5 [1 datoshi]
    /// 53 : OpCode.ROT [2 datoshi]
    /// 54 : OpCode.SETITEM [8192 datoshi]
    /// 55 : OpCode.DROP [2 datoshi]
    /// 56 : OpCode.LDLOC6 [2 datoshi]
    /// 57 : OpCode.DUP [2 datoshi]
    /// 58 : OpCode.LDLOC 08 [2 datoshi]
    /// 5A : OpCode.PUSH6 [1 datoshi]
    /// 5B : OpCode.ROT [2 datoshi]
    /// 5C : OpCode.SETITEM [8192 datoshi]
    /// 5D : OpCode.DROP [2 datoshi]
    /// 5E : OpCode.LDLOC 07 [2 datoshi]
    /// 60 : OpCode.DUP [2 datoshi]
    /// 61 : OpCode.LDLOC 08 [2 datoshi]
    /// 63 : OpCode.PUSH7 [1 datoshi]
    /// 64 : OpCode.ROT [2 datoshi]
    /// 65 : OpCode.SETITEM [8192 datoshi]
    /// 66 : OpCode.DROP [2 datoshi]
    /// 67 : OpCode.LDLOC 08 [2 datoshi]
    /// 69 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testType")]
    public abstract object? TestType();

    #endregion
}
