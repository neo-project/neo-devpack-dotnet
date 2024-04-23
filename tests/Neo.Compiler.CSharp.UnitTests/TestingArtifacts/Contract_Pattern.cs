using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":705,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":717,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":729,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":741,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":753,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":199,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":209,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":328,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":516,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":765,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":777,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ8AAP0VA1cBAnlwaBG3JAcQ2yAiBmgAZLUiAkBXAAF4NANAVwABQFcBAnlwaBG3JAcQ2yAiBmgAZLUiAkBXAQJ5cGgRtyQHENsgIgZoADK1JgcR2yAiJGgAMrgkBxDbICIGaABktSYHEdsgIg4R2yAmBxDbICIEaDoiAkBXAgEMFAAAAAAAAAAAAAAAAAAAAAAAAAAAcGhxaUrZKFDKABSzqxHbIJcQsxDbIJcmBxHbICIOEdsgJgcQ2yAiBGk6IgJAVwECeXBoELYiAkBXAQF4cGgLl6pAVwEBeHBoANi1Jg0MB1RvbyBsb3ciYmgA2LgkBxDbICIFaBC1JgkMA0xvdyJLaBC4JAcQ2yAiBWgatSYQDApBY2NlcHRhYmxlIi5oGrgkBxDbICIGaAAUtSYKDARIaWdoIhZoABS4Jg4MCFRvbyBoaWdoIgRoOkBXAQF4cGgTlyYHEdsgIgVoFJcmBxHbICIFaBWXJg8MBnNwcmluZyOVAAAAaBaXJgcR2yAiBWgXlyYHEdsgIgVoGJcmDAwGc3VtbWVyIm9oGZcmBxHbICIFaBqXJgcR2yAiBWgblyYMDAZhdXR1bW4iTGgclyYHEdsgIgVoEZcmBxHbICIFaBKXJgwMBndpbnRlciIpEdsgJiIMElVuZXhwZWN0ZWQgbW9udGg6IHg3AgCLDAEui9soOmg6QFcEAAwNSGVsbG8sIFdvcmxkIXBocWnZKGlyJghqQc/nR5YMDUhlbGxvLCBXb3JsZCFyanNr2SgmHAwTZ3JlZXRpbmcyIGlzIHN0cmluZ0HP50eWQFcBAnlwaNkwJgQiEmjZKCYEIg1o2SAmBCIIIggiBiIEIgJAVwECeXBo2TAmBxHbICIFaNkoJgcR2yAiCWjZKBHbIJcmBRAiIGjZICYFESIYaNkhEdsglyYFEiIMEdsgJgUVIgRoOiICQMJKNVP9//8jOP3//8JKNUf9//8jTf3//8JKNTv9//8jV/3//8JKNS/9//8jiv3//8JKNSP9//8jxP3//8JKNRf9//8jVP///8JKNQv9//8ja////wzb4fI="));

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
