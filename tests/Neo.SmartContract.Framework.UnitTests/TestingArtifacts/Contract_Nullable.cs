using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Nullable : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Nullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""bigIntegerNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""bigIntegerNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":47,""safe"":false},{""name"":""bigIntegerNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":96,""safe"":false},{""name"":""h160NullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":115,""safe"":false},{""name"":""h160NullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":233,""safe"":false},{""name"":""h256NullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":244,""safe"":false},{""name"":""h256NullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":416,""safe"":false},{""name"":""byteNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":583,""safe"":false},{""name"":""byteNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":630,""safe"":false},{""name"":""byteNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":679,""safe"":false},{""name"":""sByteNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":698,""safe"":false},{""name"":""sByteNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":745,""safe"":false},{""name"":""sByteNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":794,""safe"":false},{""name"":""shortNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":813,""safe"":false},{""name"":""shortNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":860,""safe"":false},{""name"":""shortNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":909,""safe"":false},{""name"":""uShortNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":928,""safe"":false},{""name"":""uShortNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":975,""safe"":false},{""name"":""uShortNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1024,""safe"":false},{""name"":""intNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1043,""safe"":false},{""name"":""intNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1090,""safe"":false},{""name"":""intNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1139,""safe"":false},{""name"":""uIntNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1158,""safe"":false},{""name"":""uIntNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1205,""safe"":false},{""name"":""uIntNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1254,""safe"":false},{""name"":""longNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1273,""safe"":false},{""name"":""longNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1320,""safe"":false},{""name"":""longNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1369,""safe"":false},{""name"":""uLongNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1388,""safe"":false},{""name"":""uLongNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1435,""safe"":false},{""name"":""uLongNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1484,""safe"":false},{""name"":""boolNullableEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1503,""safe"":false},{""name"":""boolNullableNotEqual"",""parameters"":[],""returntype"":""Boolean"",""offset"":1550,""safe"":false},{""name"":""boolNullableEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":1599,""safe"":false},{""name"":""getNullableValue"",""parameters"":[],""returntype"":""Void"",""offset"":1618,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

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
    public abstract void GetNullableValue();

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
