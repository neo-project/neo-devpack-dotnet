using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":600,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":612,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":624,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":636,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":648,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":156,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":166,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":273,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":435,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":660,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":672,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/a0CVwECeXBoEbckBAlAaABktUBXAAF4NANAVwABQFcBAnlwaBG3JAQJQGgAZLVAVwECeXBoEbckBQkiBmgAMrUmBAhAaAAyuCQFCSIGaABktSYECEAIJgQJQGg6QFcCAQwUAAAAAAAAAAAAAAAAAAAAAAAAAABwaHFpStkoUMoAFLOrCJcQswmXJgQIQAgmBAlAaTpAVwECeXBoELZAVwEBeHBoC5eqQFcBAXhwaADYtSYMDAdUb28gbG93QGgA2LgkBQkiBWgQtSYIDANMb3dAaBC4JAUJIgVoGrUmDwwKQWNjZXB0YWJsZUBoGrgkBQkiBmgAFLUmCQwESGlnaEBoABS4Jg0MCFRvbyBoaWdoQGg6VwEBeHBoE5cmBQgiBWgUlyYFCCIFaBWXJgsMBnNwcmluZ0BoFpcmBQgiBWgXlyYFCCIFaBiXJgsMBnN1bW1lckBoGZcmBQgiBWgalyYFCCIFaBuXJgsMBmF1dHVtbkBoHJcmBQgiBWgRlyYFCCIFaBKXJgsMBndpbnRlckAIJiIMElVuZXhwZWN0ZWQgbW9udGg6IHg3AACLDAEui9soOmg6VwQADA1IZWxsbywgV29ybGQhcGhxadkoaXImCGpBz+dHlgwNSGVsbG8sIFdvcmxkIXJqc2vZKCYcDBNncmVldGluZzIgaXMgc3RyaW5nQc/nR5ZAVwECeXBo2TAmA0Bo2SgmA0Bo2SAmA0BAQEBAVwECeXBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDpAwko1t/3//yOh/f//wko1q/3//yOx/f//wko1n/3//yO2/f//wko1k/3//yPY/f//wko1h/3//yME/v//wko1e/3//yNs////wko1b/3//yN7////QH3mojo="));

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
    public abstract bool? TestNotPattern(bool? x);

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

}
