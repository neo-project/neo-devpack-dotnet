using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":596,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":608,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":620,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":632,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":644,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":154,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":164,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":271,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":433,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":656,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":668,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/akCVwECeXBoEbckBAlAaABktUBXAAF4NANAVwABQFcBAnlwaBG3JAQJQGgAZLVAVwECeXBoEbckBQkiBmgAMrUmBAhAaAAyuCQFCSIGaABktSYECEAIJgQJQGg6VwIBDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2ShQygAUs6sIlxCzCZcmBAhACCYECUBpOlcBAnlwaBC2QFcBAXhwaAuXqkBXAQF4cGgA2LUmDAwHVG9vIGxvd0BoANi4JAUJIgVoELUmCAwDTG93QGgQuCQFCSIFaBq1Jg8MCkFjY2VwdGFibGVAaBq4JAUJIgZoABS1JgkMBEhpZ2hAaAAUuCYNDAhUb28gaGlnaEBoOlcBAXhwaBOXJgUIIgVoFJcmBQgiBWgVlyYLDAZzcHJpbmdAaBaXJgUIIgVoF5cmBQgiBWgYlyYLDAZzdW1tZXJAaBmXJgUIIgVoGpcmBQgiBWgblyYLDAZhdXR1bW5AaByXJgUIIgVoEZcmBQgiBWgSlyYLDAZ3aW50ZXJACCYiDBJVbmV4cGVjdGVkIG1vbnRoOiB4NwAAiwwBLovbKDpoOlcEAAwNSGVsbG8sIFdvcmxkIXBocWnZKGlyJghqQc/nR5YMDUhlbGxvLCBXb3JsZCFyanNr2SgmHAwTZ3JlZXRpbmcyIGlzIHN0cmluZ0HP50eWQFcBAnlwaNkwJgNAaNkoJgNAaNkgJgNAQEBAVwECeXBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDrCSjW7/f//I6X9///CSjWv/f//I7X9///CSjWj/f//I7r9///CSjWX/f//I9v9///CSjWL/f//Iwb+///CSjV//f//I27////CSjVz/f//I3z///9AAqAueQ=="));

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
