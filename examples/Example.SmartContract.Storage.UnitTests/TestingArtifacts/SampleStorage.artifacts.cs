using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleStorage : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleStorage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":70,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":115,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":169,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":271,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":311,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":343,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":381,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":431,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":465,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":505,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1129,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1231,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1273,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1408,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1535,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1588,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use storage"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion

    #region Constructor for internal use only

    protected SampleStorage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
