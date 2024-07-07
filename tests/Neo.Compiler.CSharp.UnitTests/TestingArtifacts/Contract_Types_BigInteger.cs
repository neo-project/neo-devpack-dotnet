using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types_BigInteger : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""zero"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""one"",""parameters"":[],""returntype"":""Integer"",""offset"":4,""safe"":false},{""name"":""minusOne"",""parameters"":[],""returntype"":""Integer"",""offset"":8,""safe"":false},{""name"":""parse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":12,""safe"":false},{""name"":""convertFromChar"",""parameters"":[],""returntype"":""Integer"",""offset"":22,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEYXRvaQEAAQ8AABsQIgJAESICQA8iAkBXAAF4NwIAIgJAAEEiAkCeDhr6"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("convertFromChar")]
    public abstract BigInteger? ConvertFromChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("minusOne")]
    public abstract BigInteger? MinusOne();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("one")]
    public abstract BigInteger? One();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("parse")]
    public abstract BigInteger? Parse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("zero")]
    public abstract BigInteger? Zero();

    #endregion

    #region Constructor for internal use only

    protected Contract_Types_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
