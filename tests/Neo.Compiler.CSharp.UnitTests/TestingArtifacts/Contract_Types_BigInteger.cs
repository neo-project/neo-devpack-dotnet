using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""attribute"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""zero"",""parameters"":[],""returntype"":""Integer"",""offset"":18,""safe"":false},{""name"":""one"",""parameters"":[],""returntype"":""Integer"",""offset"":20,""safe"":false},{""name"":""minusOne"",""parameters"":[],""returntype"":""Integer"",""offset"":22,""safe"":false},{""name"":""parse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":24,""safe"":false},{""name"":""convertFromChar"",""parameters"":[],""returntype"":""Integer"",""offset"":32,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAIwQAAADk0gzI3NK3UgAAAAAAQBBAEUAPQFcAAXg3AABAAEFASEeaMA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: BAAAAOTSDMjc0rdSAAAAAABA
    /// PUSHINT128 000000E4D20CC8DCD2B7520000000000 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("attribute")]
    public abstract BigInteger? Attribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AEFA
    /// PUSHINT8 41 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("convertFromChar")]
    public abstract BigInteger? ConvertFromChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: D0A=
    /// PUSHM1 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("minusOne")]
    public abstract BigInteger? MinusOne();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EUA=
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("one")]
    public abstract BigInteger? One();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("parse")]
    public abstract BigInteger? Parse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEA=
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("zero")]
    public abstract BigInteger? Zero();

    #endregion
}
