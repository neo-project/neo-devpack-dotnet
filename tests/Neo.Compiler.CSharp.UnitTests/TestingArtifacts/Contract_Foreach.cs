using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":134,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":188,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":197,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":255,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":367,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":405,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":482,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":583,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":644,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":727,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":774,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":1032,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":1190,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":1423,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":1529,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":1575,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":1694,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1815,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

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
