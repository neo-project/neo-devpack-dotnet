using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sumOne"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""sumOverflow"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":false},{""name"":""attribute"",""parameters"":[],""returntype"":""Integer"",""offset"":61,""safe"":false},{""name"":""zero"",""parameters"":[],""returntype"":""Integer"",""offset"":79,""safe"":false},{""name"":""one"",""parameters"":[],""returntype"":""Integer"",""offset"":81,""safe"":false},{""name"":""minusOne"",""parameters"":[],""returntype"":""Integer"",""offset"":83,""safe"":false},{""name"":""parse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":85,""safe"":false},{""name"":""convertFromChar"",""parameters"":[],""returntype"":""Integer"",""offset"":93,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAYBJAVwEAAv///39waBGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0AEAAAA5NIMyNzSt1IAAAAAAEAQQBFAD0BXAAF4NwAAQABBQLKPnJE="));

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
    /// Script: VwEAAv///39waBGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : ADD [8 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT32 00000080 [1 datoshi]
    /// 12 : JMPGE 04 [2 datoshi]
    /// 14 : JMP 0A [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1C : JMPLE 1E [2 datoshi]
    /// 1E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 27 : AND [8 datoshi]
    /// 28 : DUP [2 datoshi]
    /// 29 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2E : JMPLE 0C [2 datoshi]
    /// 30 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 39 : SUB [8 datoshi]
    /// 3A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumOverflow")]
    public abstract BigInteger? SumOverflow();

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
