using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IntegerParse : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IntegerParse"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testSbyteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":44,""safe"":false},{""name"":""testUshortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":86,""safe"":false},{""name"":""testShortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":132,""safe"":false},{""name"":""testUlongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":179,""safe"":false},{""name"":""testLongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":236,""safe"":false},{""name"":""testUintparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":300,""safe"":false},{""name"":""testIntparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":348,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/Y8BVwABeDcAAEoAgAGAALskHQwYTm90IGEgdmFsaWQgc2J5dGUgdmFsdWUuOkBXAAF4NwAAShABAAG7JBwMF05vdCBhIHZhbGlkIGJ5dGUgdmFsdWUuOkBXAAF4NwAAShACAAABALskHgwZTm90IGEgdmFsaWQgdXNob3J0IHZhbHVlLjpAVwABeDcAAEoBAIACAIAAALskHQwYTm90IGEgdmFsaWQgc2hvcnQgdmFsdWUuOkBXAAF4NwAAShAEAAAAAAAAAAABAAAAAAAAALskHQwYTm90IGEgdmFsaWQgdWxvbmcgdmFsdWUuOkBXAAF4NwAASgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQcDBdOb3QgYSB2YWxpZCBsb25nIHZhbHVlLjpAVwABeDcAAEoQAwAAAAABAAAAuyQcDBdOb3QgYSB2YWxpZCB1aW50IHZhbHVlLjpAVwABeDcAAEoCAAAAgAMAAACAAAAAALskGwwWTm90IGEgdmFsaWQgaW50IHZhbHVlLjpAKMnSNA=="));

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

    #region Constructor for internal use only

    protected Contract_IntegerParse(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
