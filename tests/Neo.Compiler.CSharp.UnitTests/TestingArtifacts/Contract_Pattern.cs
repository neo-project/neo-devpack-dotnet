using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":645,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":657,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":669,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":681,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":693,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":173,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":183,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":296,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":466,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":705,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":717,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/dkCVwECeXBoEbckBQkiBmgAZLUiAkBXAAF4NANAVwABQFcBAnlwaBG3JAUJIgZoAGS1IgJAVwECeXBoEbckBQkiBmgAMrUmBQgiHGgAMrgkBQkiBmgAZLUmBQgiCggmBQkiBGg6IgJAVwIBDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2ShQygAUs6sIlxCzCZcmBQgiCggmBQkiBGk6IgJAVwECeXBoELYiAkBXAQF4cGgLl6pAVwEBeHBoANi1Jg0MB1RvbyBsb3ciXGgA2LgkBQkiBWgQtSYJDANMb3ciR2gQuCQFCSIFaBq1JhAMCkFjY2VwdGFibGUiLGgauCQFCSIGaAAUtSYKDARIaWdoIhZoABS4Jg4MCFRvbyBoaWdoIgRoOkBXAQF4cGgTlyYFCCIFaBSXJgUIIgVoFZcmDwwGc3ByaW5nI4cAAABoFpcmBQgiBWgXlyYFCCIFaBiXJgwMBnN1bW1lciJlaBmXJgUIIgVoGpcmBQgiBWgblyYMDAZhdXR1bW4iRmgclyYFCCIFaBGXJgUIIgVoEpcmDAwGd2ludGVyIicIJiIMElVuZXhwZWN0ZWQgbW9udGg6IHg3AACLDAEui9soOmg6QFcEAAwNSGVsbG8sIFdvcmxkIXBocWnZKGlyJghqQc/nR5YMDUhlbGxvLCBXb3JsZCFyanNr2SgmHAwTZ3JlZXRpbmcyIGlzIHN0cmluZ0HP50eWQFcBAnlwaNkwJgQiEmjZKCYEIg1o2SAmBCIIIggiBiIEIgJAVwECeXBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBRAiHGjZICYFESIUaNkhCJcmBRIiCggmBRUiBGg6IgJAwko1jf3//yN0/f//wko1gf3//yOH/f//wko1df3//yOP/f//wko1af3//yO2/f//wko1Xf3//yPm/f//wko1Uf3//yNe////wko1Rf3//yN1////khtkIg=="));

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
    public abstract bool? TestNotPattern(IList<object>? x);

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
