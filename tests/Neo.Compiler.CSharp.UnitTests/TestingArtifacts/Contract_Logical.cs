using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Logical(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Logical"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testConditionalLogicalAnd"",""parameters"":[{""name"":""x"",""type"":""Boolean""},{""name"":""y"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testConditionalLogicalOr"",""parameters"":[{""name"":""x"",""type"":""Boolean""},{""name"":""y"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":10,""safe"":false},{""name"":""testLogicalExclusiveOr"",""parameters"":[{""name"":""x"",""type"":""Boolean""},{""name"":""y"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":20,""safe"":false},{""name"":""testLogicalAnd"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":27,""safe"":false},{""name"":""testLogicalOr"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":34,""safe"":false},{""name"":""testLogicalNegation"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":41,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC9XAAJ4JAQJQHlAVwACeCYECEB5QFcAAnh5k0BXAAJ4eZFAVwACeHmSQFcAAXiqQJ9JgR0="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testConditionalLogicalAnd")]
    public abstract bool? TestConditionalLogicalAnd(bool? x, bool? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testConditionalLogicalOr")]
    public abstract bool? TestConditionalLogicalOr(bool? x, bool? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmRQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLogicalAnd")]
    public abstract BigInteger? TestLogicalAnd(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmTQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// XOR [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLogicalExclusiveOr")]
    public abstract bool? TestLogicalExclusiveOr(bool? x, bool? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeKpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLogicalNegation")]
    public abstract bool? TestLogicalNegation(bool? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmSQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// OR [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLogicalOr")]
    public abstract BigInteger? TestLogicalOr(BigInteger? x, BigInteger? y);

    #endregion
}
