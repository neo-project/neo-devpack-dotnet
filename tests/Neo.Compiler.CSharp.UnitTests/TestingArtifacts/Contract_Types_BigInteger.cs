using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sumOne"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""attribute"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":false},{""name"":""zero"",""parameters"":[],""returntype"":""Integer"",""offset"":20,""safe"":false},{""name"":""one"",""parameters"":[],""returntype"":""Integer"",""offset"":22,""safe"":false},{""name"":""minusOne"",""parameters"":[],""returntype"":""Integer"",""offset"":24,""safe"":false},{""name"":""parse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":26,""safe"":false},{""name"":""convertFromChar"",""parameters"":[],""returntype"":""Integer"",""offset"":34,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAJRJABAAAAOTSDMjc0rdSAAAAAABAEEARQA9AVwABeDcAAEAAQUBkJDKz"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: BAAAAOTSDMjc0rdSAAAAAABA
    /// 00 : PUSHINT128 000000E4D20CC8DCD2B7520000000000 [4 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("attribute")]
    public abstract BigInteger? Attribute();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AEFA
    /// 00 : PUSHINT8 41 [1 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("convertFromChar")]
    public abstract BigInteger? ConvertFromChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: D0A=
    /// 00 : PUSHM1 [1 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("minusOne")]
    public abstract BigInteger? MinusOne();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EUA=
    /// 00 : PUSH1 [1 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("one")]
    public abstract BigInteger? One();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALLT 0000 [32768 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("parse")]
    public abstract BigInteger? Parse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EkA=
    /// 00 : PUSH2 [1 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumOne")]
    public abstract BigInteger? SumOne();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEA=
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("zero")]
    public abstract BigInteger? Zero();

    #endregion
}
