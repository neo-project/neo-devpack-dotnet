using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Lambda : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Lambda"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkZero"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":63,""safe"":false},{""name"":""checkZero2"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Any"",""offset"":72,""safe"":false},{""name"":""checkZero3"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Any"",""offset"":91,""safe"":false},{""name"":""checkPositiveOdd"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":116,""safe"":false},{""name"":""invokeSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":125,""safe"":false},{""name"":""invokeSum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Any"",""offset"":135,""safe"":false},{""name"":""fibo"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Any"",""offset"":265,""safe"":false},{""name"":""changeName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""Any"",""offset"":450,""safe"":false},{""name"":""changeName2"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""Any"",""offset"":481,""safe"":false},{""name"":""forEachVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":530,""safe"":false},{""name"":""forVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":606,""safe"":false},{""name"":""anyGreatThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":719,""safe"":false},{""name"":""anyGreatThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":779,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":804,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":942,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9yQQ3AABANwEAQFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkBXAAF4WDYiAkBXAAF4WDQFIgJAVwACeXg2IgJAVwEBEHB4aGEKCgAAADTqIgJAVwABeFmXQFcAAXhaNiICQFcAAnl4WzYiAkBXAQIRcHl4aGQKFAAAADQFIgJAVwADenl4NiICQFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9cnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwEBC3BoZQoOAAAASmVFeF02IgJAVwABeBK1Jgh4I5oAAAB4EZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXTZ4Ep9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXTaeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQF4ZgoYAAAAcF4MBCAhISGL2yhKZkVoNiICQF5AVwIBeGcHChQAAABwCiIAAABxaDZFaTYiAkBfBwwEICEhIYvbKEpnB0VfByICQF8HQFcGAcJweEpxynIQcyITaWvOdGhsZwgKLwAAAM9rnHNrajDtwnFoSnLKcxB0Ig1qbM51aW02z2ycdGxrMPNpIgJAwkDPQF8IQMJAz0BXBgHCcBBxIkN4ac5yaGpnCQpdAAAAz2lKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4yrUku8JxaEpyynMQdCINamzOdWltNs9snHRsazDzaSICQF8JQFcAAQoyAAAAeDQFIgJAVwQCeEpwynEQciITaGrOc2t5NiYHEdsgIg5qnHJqaTDtENsgIgJAVwABeBC3QFcAAnlnCgoLAAAAeDTGIgJAVwABeF8Kt0BXAAEKNAAAAHg0BSICQFcFAsJweEpxynIQcyIRaWvOdGx5NiYFaGzPa5xza2ow72giAkDCQM9AVwABeBC3QFcEAnhKcMpxEHIiE2hqznNreTYmBxHbICIOapxyamkw7RDbICICQFcFAsJweEpxynIQcyIRaWvOdGx5NiYFaGzPa5xza2ow72giAkBWDBBnCwoTAAAAYAoVAAAAYgoiAAAAY0BXAAF4XwuXQFcAAXgQtyQHENsgIgd4EqIRl0BXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAA3l6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkBXAAEMBGJhc2UiAkBXAAEMBGJhc2UiAkBXAAN5eqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAVwABeBDOQFcAAnlK2CYaRQwUdmFsdWUgY2Fubm90IGJlIG51bGw6SngQUdBAd5mGbg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("changeName")]
    public abstract object? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("changeName2")]
    public abstract object? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZero2")]
    public abstract object? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZero3")]
    public abstract object? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("fibo")]
    public abstract object? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("forEachVar")]
    public abstract object? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("forVar")]
    public abstract object? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("invokeSum")]
    public abstract object? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("invokeSum2")]
    public abstract object? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion

    #region Constructor for internal use only

    protected Contract_Lambda(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
