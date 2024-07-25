using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Nullable : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Nullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""bigIntegerNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""bigIntegerNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":37,""safe"":false},{""name"":""bigIntegerNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":76,""safe"":false},{""name"":""h160NullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":96,""safe"":false},{""name"":""h160NullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":191,""safe"":false},{""name"":""h256NullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":202,""safe"":false},{""name"":""h256NullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":339,""safe"":false},{""name"":""byteNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":506,""safe"":false},{""name"":""byteNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":543,""safe"":false},{""name"":""byteNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":582,""safe"":false},{""name"":""sByteNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":602,""safe"":false},{""name"":""sByteNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":639,""safe"":false},{""name"":""sByteNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":678,""safe"":false},{""name"":""shortNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":698,""safe"":false},{""name"":""shortNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":735,""safe"":false},{""name"":""shortNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":774,""safe"":false},{""name"":""uShortNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":794,""safe"":false},{""name"":""uShortNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":831,""safe"":false},{""name"":""uShortNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":870,""safe"":false},{""name"":""intNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":890,""safe"":false},{""name"":""intNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":927,""safe"":false},{""name"":""intNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":966,""safe"":false},{""name"":""uIntNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":986,""safe"":false},{""name"":""uIntNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1023,""safe"":false},{""name"":""uIntNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1062,""safe"":false},{""name"":""longNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1082,""safe"":false},{""name"":""longNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1119,""safe"":false},{""name"":""longNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1158,""safe"":false},{""name"":""uLongNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1178,""safe"":false},{""name"":""uLongNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1215,""safe"":false},{""name"":""uLongNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1254,""safe"":false},{""name"":""boolNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1274,""safe"":false},{""name"":""boolNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1311,""safe"":false},{""name"":""boolNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1350,""safe"":false},{""name"":""getNullableValue"",""parameters"":[],""returntype"":""Boolean"",""offset"":1370,""safe"":false},{""name"":""nullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1781,""safe"":false},{""name"":""nullableToString"",""parameters"":[],""returntype"":""Boolean"",""offset"":2156,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNullableValue")]
    public abstract bool? GetNullableValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullableEqual")]
    public abstract bool? NullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nullableToString")]
    public abstract bool? NullableToString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual();

    #endregion

    #region Constructor for internal use only

    protected Contract_Nullable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
