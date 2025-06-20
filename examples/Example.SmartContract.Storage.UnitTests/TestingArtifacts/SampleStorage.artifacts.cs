using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleStorage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleStorage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":68,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":113,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":167,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":269,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":307,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":339,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":377,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":425,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":459,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":499,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1111,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1213,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1253,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1378,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1505,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1556,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use storage"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"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").AsSerializable<Neo.SmartContract.NefFile>();

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
}
