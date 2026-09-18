using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_DefaultNullable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DefaultNullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testDefaultLiteral"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDefaultExpression"",""parameters"":[],""returntype"":""Boolean"",""offset"":8,""safe"":false},{""name"":""testNullCoalescing"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":false},{""name"":""testNullCoalescingExpression"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""testHasValue"",""parameters"":[],""returntype"":""Boolean"",""offset"":44,""safe"":false},{""name"":""testHasValueExpression"",""parameters"":[],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""testReturnDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":62,""safe"":false},{""name"":""testReturnDefaultExpression"",""parameters"":[],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""testComparison"",""parameters"":[{""name"":""other"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":66,""safe"":false},{""name"":""testComparisonExpression"",""parameters"":[{""name"":""other"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":75,""safe"":false},{""name"":""testNotEqualNull"",""parameters"":[],""returntype"":""Boolean"",""offset"":84,""safe"":false},{""name"":""testNotEqualNullExpression"",""parameters"":[],""returntype"":""Boolean"",""offset"":93,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGZXAQALcGjYQFcBAAtwaNhAVwEAC3BoStgmBUUAKkBXAQALcGhK2CYFRQAqQFcBAAtwaNiqQFcBAAtwaNiqQAtAC0BXAQELcGh4l0BXAQELcGh4l0BXAQALcGjYqkBXAQALcGjYqkCVly+t").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoeJdA
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testComparison")]
    public abstract bool? TestComparison(BigInteger? other);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoeJdA
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testComparisonExpression")]
    public abstract bool? TestComparisonExpression(BigInteger? other);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2EA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultExpression")]
    public abstract bool? TestDefaultExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2EA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultLiteral")]
    public abstract bool? TestDefaultLiteral();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2KpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testHasValue")]
    public abstract bool? TestHasValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2KpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testHasValueExpression")]
    public abstract bool? TestHasValueExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2KpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNotEqualNull")]
    public abstract bool? TestNotEqualNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3Bo2KpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNotEqualNullExpression")]
    public abstract bool? TestNotEqualNullExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoStgmBUUAKkA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNullCoalescing")]
    public abstract BigInteger? TestNullCoalescing();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoStgmBUUAKkA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNullCoalescingExpression")]
    public abstract BigInteger? TestNullCoalescingExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: C0A=
    /// PUSHNULL [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testReturnDefault")]
    public abstract BigInteger? TestReturnDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: C0A=
    /// PUSHNULL [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testReturnDefaultExpression")]
    public abstract BigInteger? TestReturnDefaultExpression();

    #endregion
}
