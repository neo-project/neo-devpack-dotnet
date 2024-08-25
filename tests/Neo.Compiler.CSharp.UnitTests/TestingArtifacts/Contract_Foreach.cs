using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":86,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":140,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":149,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":207,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":319,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":357,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":434,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":535,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":598,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":653,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":700,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":856,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":960,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":1091,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":1199,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":1245,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":1316,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1389,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi"",""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion

    #region Constructor for internal use only

    protected Contract_Foreach(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
