using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":212,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":315,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":384,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":486,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":509,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":811,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1237,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1406,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1609,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1700,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1710,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1808,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1871,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1971,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2074,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2272,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2373,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2557,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2701,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2995,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":3179,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":3370,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":3478,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion

    #region Constructor for internal use only

    protected Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
