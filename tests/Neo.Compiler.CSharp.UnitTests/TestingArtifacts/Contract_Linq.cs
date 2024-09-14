using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":160,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":255,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":316,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":413,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":434,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":630,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":902,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1016,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1164,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1248,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1256,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1340,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1399,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1485,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1583,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1725,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1818,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1943,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2035,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2226,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2351,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2531,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2631,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

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

}
