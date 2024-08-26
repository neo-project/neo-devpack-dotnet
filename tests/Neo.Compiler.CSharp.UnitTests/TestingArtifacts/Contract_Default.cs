using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Default(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Default"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBooleanDefault"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testByteDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""testSByteDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testInt16Default"",""parameters"":[],""returntype"":""Integer"",""offset"":21,""safe"":false},{""name"":""testUInt16Default"",""parameters"":[],""returntype"":""Integer"",""offset"":28,""safe"":false},{""name"":""testInt32Default"",""parameters"":[],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testUInt32Default"",""parameters"":[],""returntype"":""Integer"",""offset"":42,""safe"":false},{""name"":""testInt64Default"",""parameters"":[],""returntype"":""Integer"",""offset"":49,""safe"":false},{""name"":""testUInt64Default"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""testCharDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":63,""safe"":false},{""name"":""testStringDefault"",""parameters"":[],""returntype"":""String"",""offset"":70,""safe"":false},{""name"":""testObjectDefault"",""parameters"":[],""returntype"":""Any"",""offset"":77,""safe"":false},{""name"":""testBigIntegerDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":84,""safe"":false},{""name"":""testStructDefault"",""parameters"":[],""returntype"":""Array"",""offset"":91,""safe"":false},{""name"":""testClassDefault"",""parameters"":[],""returntype"":""Any"",""offset"":98,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGlXAQAJcGhAVwEAEHBoQFcBABBwaEBXAQAQcGhAVwEAEHBoQFcBABBwaEBXAQAQcGhAVwEAEHBoQFcBABBwaEBXAQAQcGhAVwEAC3BoQFcBAAtwaEBXAQAQcGhAVwEAxXBoQFcBAAtwaEBJsgEj"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBooleanDefault")]
    public abstract bool? TestBooleanDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharDefault")]
    public abstract BigInteger? TestCharDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testClassDefault")]
    public abstract object? TestClassDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInt16Default")]
    public abstract BigInteger? TestInt16Default();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInt32Default")]
    public abstract BigInteger? TestInt32Default();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInt64Default")]
    public abstract BigInteger? TestInt64Default();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testObjectDefault")]
    public abstract object? TestObjectDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStructDefault")]
    public abstract IList<object>? TestStructDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt16Default")]
    public abstract BigInteger? TestUInt16Default();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt32Default")]
    public abstract BigInteger? TestUInt32Default();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt64Default")]
    public abstract BigInteger? TestUInt64Default();

    #endregion

}
