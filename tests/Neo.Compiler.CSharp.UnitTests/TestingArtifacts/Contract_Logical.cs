using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Logical : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Logical"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testConditionalLogicalAnd"",""parameters"":[{""name"":""x"",""type"":""Boolean""},{""name"":""y"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testConditionalLogicalOr"",""parameters"":[{""name"":""x"",""type"":""Boolean""},{""name"":""y"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":13,""safe"":false},{""name"":""testLogicalExclusiveOr"",""parameters"":[{""name"":""x"",""type"":""Boolean""},{""name"":""y"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":26,""safe"":false},{""name"":""testLogicalAnd"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testLogicalOr"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":44,""safe"":false},{""name"":""testLogicalNegation"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":53,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAA9VwACeCQFCSIDeSICQFcAAngmBQgiA3kiAkBXAAJ4eZMiAkBXAAJ4eZEiAkBXAAJ4eZIiAkBXAAF4qiICQPNvTtA="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testConditionalLogicalAnd")]
    public abstract bool? TestConditionalLogicalAnd(bool? x, bool? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testConditionalLogicalOr")]
    public abstract bool? TestConditionalLogicalOr(bool? x, bool? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLogicalAnd")]
    public abstract BigInteger? TestLogicalAnd(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLogicalExclusiveOr")]
    public abstract bool? TestLogicalExclusiveOr(bool? x, bool? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLogicalNegation")]
    public abstract bool? TestLogicalNegation(bool? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLogicalOr")]
    public abstract BigInteger? TestLogicalOr(BigInteger? x, BigInteger? y);

    #endregion

    #region Constructor for internal use only

    protected Contract_Logical(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
