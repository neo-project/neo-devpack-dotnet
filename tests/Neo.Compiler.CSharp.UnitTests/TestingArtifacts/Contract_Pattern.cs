using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":631,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":643,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":655,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":667,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":679,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":163,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":173,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":286,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":456,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":691,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":703,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/cwCVwECeXBoEbckBQkiBmgAZLVAVwABeDQDQFcAAUBXAQJ5cGgRtyQFCSIGaABktUBXAQJ5cGgRtyQFCSIGaAAytSYFCCIcaAAyuCQFCSIGaABktSYFCCIKCCYFCSIEaDpAVwIBDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2ShQygAUs6sIlxCzCZcmBQgiCggmBQkiBGk6QFcBAnlwaBC2QFcBAXhwaAuXqkBXAQF4cGgA2LUmDQwHVG9vIGxvdyJcaADYuCQFCSIFaBC1JgkMA0xvdyJHaBC4JAUJIgVoGrUmEAwKQWNjZXB0YWJsZSIsaBq4JAUJIgZoABS1JgoMBEhpZ2giFmgAFLgmDgwIVG9vIGhpZ2giBGg6QFcBAXhwaBOXJgUIIgVoFJcmBQgiBWgVlyYPDAZzcHJpbmcjhwAAAGgWlyYFCCIFaBeXJgUIIgVoGJcmDAwGc3VtbWVyImVoGZcmBQgiBWgalyYFCCIFaBuXJgwMBmF1dHVtbiJGaByXJgUIIgVoEZcmBQgiBWgSlyYMDAZ3aW50ZXIiJwgmIgwSVW5leHBlY3RlZCBtb250aDogeDcAAIsMAS6L2yg6aDpAVwQADA1IZWxsbywgV29ybGQhcGhxadkoaXImCGpBz+dHlgwNSGVsbG8sIFdvcmxkIXJqc2vZKCYcDBNncmVldGluZzIgaXMgc3RyaW5nQc/nR5ZAVwECeXBo2TAmBCISaNkoJgQiDWjZICYEIggiBiIEIgJAVwECeXBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBRAiHGjZICYFESIUaNkhCJcmBRIiCggmBRUiBGg6QMJKNZn9//8jgv3//8JKNY39//8jk/3//8JKNYH9//8jmf3//8JKNXX9//8jvv3//8JKNWn9//8j7P3//8JKNV39//8jYv///8JKNVH9//8jd////0DtXS7f"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between")]
    public abstract bool? Between(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between2")]
    public abstract bool? Between2(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between3")]
    public abstract bool? Between3(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between4")]
    public abstract bool? Between4(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNotPattern")]
    public abstract bool? TestNotPattern(object? x = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRecursivePattern")]
    public abstract bool? TestRecursivePattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);

    #endregion

    #region Constructor for internal use only

    protected Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
