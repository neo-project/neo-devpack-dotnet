using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":645,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":657,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":669,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":681,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":693,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":173,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":183,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":296,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":466,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":705,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":717,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ8AAP3ZAlcBAnlwaBG3JAUJIgZoAGS1IgJAVwABeDQDQFcAAUBXAQJ5cGgRtyQFCSIGaABktSICQFcBAnlwaBG3JAUJIgZoADK1JgUIIhxoADK4JAUJIgZoAGS1JgUIIgoIJgUJIgRoOiICQFcCAQwUAAAAAAAAAAAAAAAAAAAAAAAAAABwaHFpStkoUMoAFLOrCJcQswmXJgUIIgoIJgUJIgRpOiICQFcBAnlwaBC2IgJAVwEBeHBoC5eqQFcBAXhwaADYtSYNDAdUb28gbG93IlxoANi4JAUJIgVoELUmCQwDTG93IkdoELgkBQkiBWgatSYQDApBY2NlcHRhYmxlIixoGrgkBQkiBmgAFLUmCgwESGlnaCIWaAAUuCYODAhUb28gaGlnaCIEaDpAVwEBeHBoE5cmBQgiBWgUlyYFCCIFaBWXJg8MBnNwcmluZyOHAAAAaBaXJgUIIgVoF5cmBQgiBWgYlyYMDAZzdW1tZXIiZWgZlyYFCCIFaBqXJgUIIgVoG5cmDAwGYXV0dW1uIkZoHJcmBQgiBWgRlyYFCCIFaBKXJgwMBndpbnRlciInCCYiDBJVbmV4cGVjdGVkIG1vbnRoOiB4NwIAiwwBLovbKDpoOkBXBAAMDUhlbGxvLCBXb3JsZCFwaHFp2ShpciYIakHP50eWDA1IZWxsbywgV29ybGQhcmpza9koJhwME2dyZWV0aW5nMiBpcyBzdHJpbmdBz+dHlkBXAQJ5cGjZMCYEIhJo2SgmBCINaNkgJgQiCCIIIgYiBCICQFcBAnlwaNkwJgUIIgVo2SgmBQgiB2jZKAiXJgUQIhxo2SAmBREiFGjZIQiXJgUSIgoIJgUVIgRoOiICQMJKNY39//8jdP3//8JKNYH9//8jh/3//8JKNXX9//8jj/3//8JKNWn9//8jtv3//8JKNV39//8j5v3//8JKNVH9//8jXv///8JKNUX9//8jdf///6BRu5o="));

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
