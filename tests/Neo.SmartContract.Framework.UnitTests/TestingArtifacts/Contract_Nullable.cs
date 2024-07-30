using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Nullable : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Nullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""bigIntegerNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""bigIntegerNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":33,""safe"":false},{""name"":""bigIntegerNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":68,""safe"":false},{""name"":""h160NullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Hash160""},{""name"":""b"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":86,""safe"":false},{""name"":""h160NullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":121,""safe"":false},{""name"":""h256NullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Hash256""},{""name"":""b"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":130,""safe"":false},{""name"":""h256NullableEqual"",""parameters"":[{""name"":""a"",""type"":""Hash256""},{""name"":""b"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":165,""safe"":false},{""name"":""byteNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":198,""safe"":false},{""name"":""byteNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":231,""safe"":false},{""name"":""byteNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":266,""safe"":false},{""name"":""sByteNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":284,""safe"":false},{""name"":""sByteNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":317,""safe"":false},{""name"":""sByteNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":352,""safe"":false},{""name"":""shortNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":370,""safe"":false},{""name"":""shortNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":403,""safe"":false},{""name"":""shortNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":438,""safe"":false},{""name"":""uShortNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":456,""safe"":false},{""name"":""uShortNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":489,""safe"":false},{""name"":""uShortNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":524,""safe"":false},{""name"":""intNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":542,""safe"":false},{""name"":""intNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":575,""safe"":false},{""name"":""intNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":610,""safe"":false},{""name"":""uIntNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":628,""safe"":false},{""name"":""uIntNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":661,""safe"":false},{""name"":""uIntNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":696,""safe"":false},{""name"":""longNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":714,""safe"":false},{""name"":""longNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":747,""safe"":false},{""name"":""longNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":782,""safe"":false},{""name"":""uLongNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":800,""safe"":false},{""name"":""uLongNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":833,""safe"":false},{""name"":""uLongNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":868,""safe"":false},{""name"":""boolNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":886,""safe"":false},{""name"":""boolNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":919,""safe"":false},{""name"":""boolNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":954,""safe"":false},{""name"":""byteNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":972,""safe"":false},{""name"":""sByteNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":997,""safe"":false},{""name"":""shortNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1022,""safe"":false},{""name"":""uShortNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1047,""safe"":false},{""name"":""intNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1072,""safe"":false},{""name"":""uIntNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1097,""safe"":false},{""name"":""longNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1122,""safe"":false},{""name"":""uLongNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1147,""safe"":false},{""name"":""boolNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1172,""safe"":false},{""name"":""bigIntegerNullableToString"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":1214,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/dcEVwACeHmXJAUJIgV4ebMkBQkiBXl4syQFCSIFeXiXIgJAVwACeHmYJAUJIgZ4ebOqJAUJIgZ5eLOqJAUJIgV5eJgiAkBXAAF4C5ckBQkiBnjYqqoiAkBXAAJ4eZgkBQkiBnh5l6okBQkiBnl4l6okBQkiBXl4mCICQFcAAXgLlyICQFcAAnh5mCQFCSIGeHmXqiQFCSIGeXiXqiQFCSIFeXiYIgJAVwACeHmXJAUJIgV4eZckBQkiBXl4lyQFCSIFeXiXIgJAVwACeHmXJAUJIgV4ebMkBQkiBXl4syQFCSIFeXiXIgJAVwACeHmYJAUJIgZ4ebOqJAUJIgZ5eLOqJAUJIgV5eJgiAkBXAAF4C5ckBQkiBnjYqqoiAkBXAAJ4eZckBQkiBXh5syQFCSIFeXizJAUJIgV5eJciAkBXAAJ4eZgkBQkiBnh5s6okBQkiBnl4s6okBQkiBXl4mCICQFcAAXgLlyQFCSIGeNiqqiICQFcAAnh5lyQFCSIFeHmzJAUJIgV5eLMkBQkiBXl4lyICQFcAAnh5mCQFCSIGeHmzqiQFCSIGeXizqiQFCSIFeXiYIgJAVwABeAuXJAUJIgZ42KqqIgJAVwACeHmXJAUJIgV4ebMkBQkiBXl4syQFCSIFeXiXIgJAVwACeHmYJAUJIgZ4ebOqJAUJIgZ5eLOqJAUJIgV5eJgiAkBXAAF4C5ckBQkiBnjYqqoiAkBXAAJ4eZckBQkiBXh5syQFCSIFeXizJAUJIgV5eJciAkBXAAJ4eZgkBQkiBnh5s6okBQkiBnl4s6okBQkiBXl4mCICQFcAAXgLlyQFCSIGeNiqqiICQFcAAnh5lyQFCSIFeHmzJAUJIgV5eLMkBQkiBXl4lyICQFcAAnh5mCQFCSIGeHmzqiQFCSIGeXizqiQFCSIFeXiYIgJAVwABeAuXJAUJIgZ42KqqIgJAVwACeHmXJAUJIgV4ebMkBQkiBXl4syQFCSIFeXiXIgJAVwACeHmYJAUJIgZ4ebOqJAUJIgZ5eLOqJAUJIgV5eJgiAkBXAAF4C5ckBQkiBnjYqqoiAkBXAAJ4eZckBQkiBXh5syQFCSIFeXizJAUJIgV5eJciAkBXAAJ4eZgkBQkiBnh5s6okBQkiBnl4s6okBQkiBXl4mCICQFcAAXgLlyQFCSIGeNiqqiICQFcAAnh5lyQFCSIFeHmzJAUJIgV5eLMkBQkiBXl4lyICQFcAAnh5mCQFCSIGeHmzqiQFCSIGeXizqiQFCSIFeXiYIgJAVwABeAuXJAUJIgZ42KqqIgJAVwABeErYJAU3AABK2CYFRQwADAExlyICQFcAAXhK2CQFNwAAStgmBUUMAAwBMZciAkBXAAF4StgkBTcAAErYJgVFDAAMATGXIgJAVwABeErYJAU3AABK2CYFRQwADAExlyICQFcAAXhK2CQFNwAAStgmBUUMAAwBMZciAkBXAAF4StgkBTcAAErYJgVFDAAMATGXIgJAVwABeErYJAU3AABK2CYFRQwADAExlyICQFcAAXhK2CQFNwAAStgmBUUMAAwBMZciAkBXAAF4StgkEyQLDAVGYWxzZSIIDARUcnVlStgmBUUMAAwEVHJ1ZZciAkBXAAF4StgkBTcAAErYJgVFDAAMATGXIgJA7GhhXA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableToString")]
    public abstract bool? BigIntegerNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual(bool? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual(bool? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableToString")]
    public abstract bool? BoolNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableToString")]
    public abstract bool? ByteNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual(UInt160? a, UInt160? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableToString")]
    public abstract bool? IntNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableToString")]
    public abstract bool? LongNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableToString")]
    public abstract bool? SByteNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableToString")]
    public abstract bool? ShortNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableToString")]
    public abstract bool? UIntNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableToString")]
    public abstract bool? ULongNullableToString(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual(BigInteger? a, object? b = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableToString")]
    public abstract bool? UShortNullableToString(object? a = null);

    #endregion

    #region Constructor for internal use only

    protected Contract_Nullable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
