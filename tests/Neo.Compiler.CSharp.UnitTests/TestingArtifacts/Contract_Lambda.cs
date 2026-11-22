using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Lambda(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Lambda"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkZero"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZero2"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":13,""safe"":false},{""name"":""checkZero3"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""checkPositiveOdd"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":55,""safe"":false},{""name"":""invokeSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":68,""safe"":false},{""name"":""invokeSum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":82,""safe"":false},{""name"":""fibo"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":182,""safe"":false},{""name"":""changeName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":323,""safe"":false},{""name"":""changeName2"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":359,""safe"":false},{""name"":""forEachVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":413,""safe"":false},{""name"":""forVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":479,""safe"":false},{""name"":""anyGreatThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":576,""safe"":false},{""name"":""anyGreatThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":629,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":652,""safe"":false},{""name"":""testLambdaDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":708,""safe"":false},{""name"":""testLambdaNotDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":764,""safe"":false},{""name"":""testLambdaInModifier"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":820,""safe"":false},{""name"":""testLambdaScopedInModifier"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":875,""safe"":false},{""name"":""testLambdaRefReadonlyModifier"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":930,""safe"":false},{""name"":""expressionVoidAssignment"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":985,""safe"":false},{""name"":""expressionVoidParenthesized"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1013,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1076,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP12BFcBAQo0BAAAcHhoNkBXAAF4CiYEAAA0A0BXAQJ4cHloNkBXAQEQYXgKCAAAADTsQFcAAXhZs0BXAQEKBAQAAHB4aDZAVwECCgcEAABweXhoNkBXAQIRZHl4ChIAAAA0A0BXAQN4cHp5aDZAVwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwIBC2UKDAAAAGVdcXhpNkBXAQF4ErUmBHhAXXB4nUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2g2XXB4Ep9KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9oNp5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwIBeGYKHQAAAHBeStgmBUUMAAwEICEhIYvbKGZocWk2QF5AVwMBeGcHChYAAABwCicAAABxaHJqNkVpcmo2QF8HStgmBUUMAAwEICEhIYvbKGcHXwdAXwdAVwcBwnB4SnHKchBzIhFpa85nCGgKKwAAAM9rnHNrajDvwnFoSnLKcxB0Ig9qbM51aW12bjbPbJx0bGsw8WlAXwhAVwcBwnAQcSI0eGnOZwloCk8AAADPaUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMowy8JxaEpyynMQdCIPamzOdWltdm42z2ycdGxrMPFpQF8JQFcAAQorAAAAeDQDQFcFAnhKcMpxEHIiEmhqznN5dGtsNiYECEBqnHJqaTDuCUBXAAF4ELdAVwACeWcKCgkAAAB4NMtAVwABeF8Kt0BXAAEKLgAAAHg0A0BXBgLCcHhKccpyEHMiE2lrznR5dWxtNiYFaGzPa5xza2ow7WhAVwABeBC3QFcCAQoNAAAAcGhxEXhpNkBXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwICCg0AAABwaHF5eGk2QFcAAnh5nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAgEKDAAAAHBocXhpNkBXAAF4FJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwIBCgwAAABwaHF4aTZAVwABeBWeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcCAQoMAAAAcGhxeGk2QFcAAXgWnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAwF4ZwsKDwAAAHFpcgBjajZfC0BXAAF4ZwtAVwMBEGcMCg8AAABxaXISeGo2XwxAVwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwxAVg5AVwABeBCzQFcAAXgQtyQECUB4EqIRs0BXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A0V8JCQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWcKCgkAAAB4NMtA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// STSFLD 0A [2 datoshi]
    /// PUSHA 09000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL CB [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCisAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 2B000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeGYKHQAAAHBeStgmBUUMAAwEICEhIYvbKGZocWk2QA==
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// PUSHA 1D000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSHDATA1 20212121 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STSFLD6 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeGcHChYAAABwCicAAABxaHJqNkVpcmo2QA==
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// PUSHA 16000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHA 27000000 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLA [512 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgQEAABweGg2QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHA 04040000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCjQEAABweGg2QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHA 34040000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAomBAAANANA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 26040000 [4 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGF4CggAAAA07EA=
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 08000000 [4 datoshi]
    /// CALL EC [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeGcLCg8AAABxaXIAY2o2XwtA
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// PUSHA 0F000000 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHINT8 63 [1 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLA [512 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("expressionVoidAssignment")]
    public abstract BigInteger? ExpressionVoidAssignment(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEGcMCg8AAABxaXISeGo2XwxA
    /// INITSLOT 0301 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD 0C [2 datoshi]
    /// PUSHA 0F000000 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLA [512 datoshi]
    /// LDSFLD 0C [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("expressionVoidParenthesized")]
    public abstract BigInteger? ExpressionVoidParenthesized(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBC2UKDAAAAGVdcXhpNkA=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD5 [2 datoshi]
    /// PUSHA 0C000000 [4 datoshi]
    /// STSFLD5 [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcBwnB4SnHKchBzIhFpa85nCGgKKwAAAM9rnHNrajDvwnFoSnLKcxB0Ig9qbM51aW12bjbPbJx0bGsw8WlA
    /// INITSLOT 0701 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 11 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHA 2B000000 [4 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT EF [2 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0F [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC5 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// STLOC6 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// CALLA [512 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// JMPLT F1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcBwnAQcSI0eGnOZwloCk8AAADPaUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMowy8JxaEpyynMQdCIPamzOdWltdm42z2ycdGxrMPFpQA==
    /// INITSLOT 0701 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 34 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHA 4F000000 [4 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// JMPLT CB [2 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0F [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC5 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// STLOC6 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// CALLA [512 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// JMPLT F1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgcEAABweXhoNkA=
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHA 07040000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEWR5eAoSAAAANANA
    /// INITSLOT 0102 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// STSFLD4 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 12000000 [4 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBCg0AAABwaHEReGk2QA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHA 0D000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBCgwAAABwaHF4aTZA
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHA 0C000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaInModifier")]
    public abstract BigInteger? TestLambdaInModifier(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICCg0AAABwaHF5eGk2QA==
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHA 0D000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBCgwAAABwaHF4aTZA
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHA 0C000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaRefReadonlyModifier")]
    public abstract BigInteger? TestLambdaRefReadonlyModifier(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBCgwAAABwaHF4aTZA
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHA 0C000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaScopedInModifier")]
    public abstract BigInteger? TestLambdaScopedInModifier(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCi4AAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 2E000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
