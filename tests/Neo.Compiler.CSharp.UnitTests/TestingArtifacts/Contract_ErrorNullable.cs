using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ErrorNullable : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ErrorNullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBigInteger"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testInt"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":4,""safe"":false},{""name"":""testUInt"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":8,""safe"":false},{""name"":""testLong"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":12,""safe"":false},{""name"":""testULong"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":16,""safe"":false},{""name"":""testShort"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":20,""safe"":false},{""name"":""testUShort"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":24,""safe"":false},{""name"":""testSByte"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":28,""safe"":false},{""name"":""testByte"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":32,""safe"":false},{""name"":""testChar"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":36,""safe"":false},{""name"":""testBool"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":40,""safe"":false},{""name"":""testUInt160"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":44,""safe"":false},{""name"":""testUInt256"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":48,""safe"":false},{""name"":""testUInt160Array"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Void"",""offset"":52,""safe"":false},{""name"":""testUInt256Array"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Void"",""offset"":56,""safe"":false},{""name"":""testByteArray"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":60,""safe"":false},{""name"":""testString"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Void"",""offset"":64,""safe"":false},{""name"":""testObject"",""parameters"":[{""name"":""a"",""type"":""Any""}],""returntype"":""Void"",""offset"":68,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEhXAAFAVwABQFcAAUBXAAFAVwABQFcAAUBXAAFAVwABQFcAAUBXAAFAVwABQFcAAUBXAAFAVwABQFcAAUBXAAFAVwABQFcAAUA9ZSBz"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBigInteger")]
    public abstract void TestBigInteger(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBool")]
    public abstract void TestBool(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByte")]
    public abstract void TestByte(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArray")]
    public abstract void TestByteArray(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testChar")]
    public abstract void TestChar(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInt")]
    public abstract void TestInt(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLong")]
    public abstract void TestLong(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testObject")]
    public abstract void TestObject(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSByte")]
    public abstract void TestSByte(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testShort")]
    public abstract void TestShort(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testString")]
    public abstract void TestString(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt")]
    public abstract void TestUInt(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt160")]
    public abstract void TestUInt160(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt160Array")]
    public abstract void TestUInt160Array(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt256")]
    public abstract void TestUInt256(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUInt256Array")]
    public abstract void TestUInt256Array(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testULong")]
    public abstract void TestULong(object? a = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUShort")]
    public abstract void TestUShort(object? a = null);

    #endregion

    #region Constructor for internal use only

    protected Contract_ErrorNullable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
