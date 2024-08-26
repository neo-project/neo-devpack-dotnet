using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IntegerParse(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IntegerParse"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testSbyteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":18,""safe"":false},{""name"":""testUshortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testShortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":54,""safe"":false},{""name"":""testUlongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":75,""safe"":false},{""name"":""testLongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":106,""safe"":false},{""name"":""testUintparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":145,""safe"":false},{""name"":""testIntparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":168,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAw1cAAXg3AABKAIABgAC7JAM6QFcAAXg3AABKEAEAAbskAzpAVwABeDcAAEoQAgAAAQC7JAM6QFcAAXg3AABKAQCAAgCAAAC7JAM6QFcAAXg3AABKEAQAAAAAAAAAAAEAAAAAAAAAuyQDOkBXAAF4NwAASgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOkBXAAF4NwAAShADAAAAAAEAAAC7JAM6QFcAAXg3AABKAgAAAIADAAAAgAAAAAC7JAM6QPp9YiI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteparse")]
    public abstract BigInteger? TestByteparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntparse")]
    public abstract BigInteger? TestIntparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLongparse")]
    public abstract BigInteger? TestLongparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSbyteparse")]
    public abstract BigInteger? TestSbyteparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testShortparse")]
    public abstract BigInteger? TestShortparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUintparse")]
    public abstract BigInteger? TestUintparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUlongparse")]
    public abstract BigInteger? TestUlongparse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUshortparse")]
    public abstract BigInteger? TestUshortparse(string? s);

    #endregion

}
