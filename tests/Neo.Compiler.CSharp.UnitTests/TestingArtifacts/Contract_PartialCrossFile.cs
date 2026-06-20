using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_PartialCrossFile(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PartialCrossFile"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getBaseValue"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testCrossFileCall"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":false},{""name"":""getMultiplier"",""parameters"":[],""returntype"":""Integer"",""offset"":42,""safe"":false},{""name"":""testCrossFileCallReverse"",""parameters"":[],""returntype"":""Integer"",""offset"":44,""safe"":false},{""name"":""expressionBodyTest"",""parameters"":[],""returntype"":""Integer"",""offset"":82,""safe"":false},{""name"":""complexCrossFileExpression"",""parameters"":[],""returntype"":""Integer"",""offset"":85,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIQAZEA0JwBkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0AVQDTUFZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AAGlAVwEAAfQBcGg0o55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEBQRfJt").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAfQBcGg0o55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT16 F401 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL A3 [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("complexCrossFileExpression")]
    public abstract BigInteger? ComplexCrossFileExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AGlA
    /// PUSHINT8 69 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("expressionBodyTest")]
    public abstract BigInteger? ExpressionBodyTest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AGRA
    /// PUSHINT8 64 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getBaseValue")]
    public abstract BigInteger? GetBaseValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getMultiplier")]
    public abstract BigInteger? GetMultiplier();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NCcAZKBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// CALL 27 [512 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCrossFileCall")]
    public abstract BigInteger? TestCrossFileCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NNQVnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// CALL D4 [512 datoshi]
    /// PUSH5 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCrossFileCallReverse")]
    public abstract BigInteger? TestCrossFileCallReverse();

    #endregion
}
