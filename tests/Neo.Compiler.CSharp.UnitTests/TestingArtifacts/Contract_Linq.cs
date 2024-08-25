using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":164,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":264,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":333,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":435,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":459,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":659,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":935,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1053,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1205,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1297,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1307,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1405,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1468,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1568,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1671,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1821,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1922,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2055,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2151,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2346,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2479,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2670,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2778,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

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
