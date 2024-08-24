using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""attribute"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""zero"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":false},{""name"":""one"",""parameters"":[],""returntype"":""Integer"",""offset"":4,""safe"":false},{""name"":""minusOne"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""parse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":8,""safe"":false},{""name"":""convertFromChar"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":19,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAKFhAEEARQA9AVwABeDcAAEAAQUBWAQQAAADk0gzI3NK3UgAAAAAAYECEDjNf"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("attribute")]
    public abstract BigInteger? Attribute();

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

}
