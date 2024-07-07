using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IntegerParse : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IntegerParse"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testSbyteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testByteparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":20,""safe"":false},{""name"":""testUshortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""testShortparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""testUlongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":83,""safe"":false},{""name"":""testLongparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":116,""safe"":false},{""name"":""testUintparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":157,""safe"":false},{""name"":""testIntparse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":182,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEYXRvaQEAAQ8AANNXAAF4NwIASgCAAYAAuyQDOiICQFcAAXg3AgBKEAEAAbskAzoiAkBXAAF4NwIAShACAAABALskAzoiAkBXAAF4NwIASgEAgAIAgAAAuyQDOiICQFcAAXg3AgBKEAQAAAAAAAAAAAEAAAAAAAAAuyQDOiICQFcAAXg3AgBKAwAAAAAAAACABAAAAAAAAACAAAAAAAAAAAC7JAM6IgJAVwABeDcCAEoQAwAAAAABAAAAuyQDOiICQFcAAXg3AgBKAgAAAIADAAAAgAAAAAC7JAM6IgJAtdYzng=="));

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
