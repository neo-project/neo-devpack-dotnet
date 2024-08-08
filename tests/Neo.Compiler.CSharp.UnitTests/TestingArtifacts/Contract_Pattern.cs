using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":626,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":638,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":650,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":662,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":674,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":163,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":168,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":281,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":451,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":686,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":698,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/ccCVwECeXBoEbckBQkiBmgAZLVAVwABeDQDQFcAAUBXAQJ5cGgRtyQFCSIGaABktUBXAQJ5cGgRtyQFCSIGaAAytSYFCCIcaAAyuCQFCSIGaABktSYFCCIKCCYFCSIEaDpAVwIBDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2ShQygAUs6sIlxCzCZcmBQgiCggmBQkiBGk6QFcBAnlwaBC2QFcAAXhAVwEBeHBoANi1Jg0MB1RvbyBsb3ciXGgA2LgkBQkiBWgQtSYJDANMb3ciR2gQuCQFCSIFaBq1JhAMCkFjY2VwdGFibGUiLGgauCQFCSIGaAAUtSYKDARIaWdoIhZoABS4Jg4MCFRvbyBoaWdoIgRoOkBXAQF4cGgTlyYFCCIFaBSXJgUIIgVoFZcmDwwGc3ByaW5nI4cAAABoFpcmBQgiBWgXlyYFCCIFaBiXJgwMBnN1bW1lciJlaBmXJgUIIgVoGpcmBQgiBWgblyYMDAZhdXR1bW4iRmgclyYFCCIFaBGXJgUIIgVoEpcmDAwGd2ludGVyIicIJiIMElVuZXhwZWN0ZWQgbW9udGg6IHg3AACLDAEui9soOmg6QFcEAAwNSGVsbG8sIFdvcmxkIXBocWnZKGlyJghqQc/nR5YMDUhlbGxvLCBXb3JsZCFyanNr2SgmHAwTZ3JlZXRpbmcyIGlzIHN0cmluZ0HP50eWQFcBAnlwaNkwJgQiEmjZKCYEIg1o2SAmBCIIIgYiBCICQFcBAnlwaNkwJgUIIgVo2SgmBQgiB2jZKAiXJgUQIhxo2SAmBREiFGjZIQiXJgUSIgoIJgUVIgRoOkDCSjWe/f//I4f9///CSjWS/f//I5j9///CSjWG/f//I579///CSjV6/f//I8P9///CSjVu/f//I/H9///CSjVi/f//I2L////CSjVW/f//I3f///9AWfZUug=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between")]
    public abstract bool Between(BigInteger value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between2")]
    public abstract bool Between2(BigInteger value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between3")]
    public abstract bool Between3(BigInteger value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between4")]
    public abstract bool Between4(BigInteger value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("classify")]
    public abstract string Classify(BigInteger measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getCalendarSeason")]
    public abstract string GetCalendarSeason(BigInteger month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNotPattern")]
    public abstract bool TestNotPattern(bool x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRecursivePattern")]
    public abstract bool TestRecursivePattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger TestTypePattern2(object t = null);

    #endregion

    #region Constructor for internal use only

    protected Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
