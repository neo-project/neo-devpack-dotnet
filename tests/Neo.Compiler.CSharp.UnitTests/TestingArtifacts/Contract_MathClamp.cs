using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_MathClamp(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_MathClamp"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""clampConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""clampEqualBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":11,""safe"":false},{""name"":""clampInvalidBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""clampDynamicBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""clampNamedArguments"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":82,""safe"":false},{""name"":""clampConstantExpressions"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":93,""safe"":false},{""name"":""clampByteConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":104,""safe"":false},{""name"":""clampLongConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":116,""safe"":false},{""name"":""clampULongConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":134,""safe"":false},{""name"":""clampBigIntegerConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":160,""safe"":false},{""name"":""clampIntExtremeBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":171,""safe"":false},{""name"":""clampSByteConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":189,""safe"":false},{""name"":""clampShortConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":200,""safe"":false},{""name"":""clampUShortConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":214,""safe"":false},{""name"":""clampUIntConstantBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":228,""safe"":false},{""name"":""clampBigIntegerInvalidBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":246,""safe"":false},{""name"":""clampInvalidNamedBounds"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":276,""safe"":false},{""name"":""clampSideEffectValueConstantBounds"",""parameters"":[],""returntype"":""Integer"",""offset"":305,""safe"":false},{""name"":""clampEvaluationOrder"",""parameters"":[],""returntype"":""Integer"",""offset"":476,""safe"":false},{""name"":""clampNested"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":623,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":843,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1QA1cAAXgQAGRTurlAVwABeAAqACpTurlAVwABeABkEEtLMhFFRUUMCW1pbiA+IG1heDpTurlAVwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUBXAAF4EABkU7q5QFcAAXgQAGRTurlAVwABeBoByABTurlAVwABeAMAAAAAAAAAgBBTurlAVwABeBUE//////////8AAAAAAAAAAFO6uUBXAAF4APYaU7q5QFcAAXgCAAAAgAL///9/U7q5QFcAAXgA+xVTurlAVwABeAHU/gEsAVO6uUBXAAF4GgJg6gAAU7q5QFcAAXgRA/////8AAAAAU7q5QFcAAXgaAPZLSzIRRUVFDAltaW4gPiBtYXg6U7q5QFcAAXgaFUtLMhFFRUUMCW1pbiA+IG1heDpTurlAVwQAEGA0fgHIAJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xEHIAZHNpamtTurlwaFgB6AOgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BYSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gRVhAVwMAEGA00xqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcDStcQBkcmhpaktLMhFFRUUMCW1pbiA+IG1heDpTurlYAegDoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwIBEHAQcSOkAAAAaHhpnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxAaU7q5aRKisSQFESIDEqBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+eSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRMxX////2h4EBVTurmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFYBEGBAVnW0gA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAD2GlO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 F6 [1 datoshi]
    /// PUSH10 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampBigIntegerConstantBounds")]
    public abstract BigInteger? ClampBigIntegerConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBoA9ktLMhFFRUUMCW1pbiA+IG1heDpTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSHINT8 F6 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampBigIntegerInvalidBounds")]
    public abstract BigInteger? ClampBigIntegerInvalidBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBoByABTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSHINT16 C800 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampByteConstantBounds")]
    public abstract BigInteger? ClampByteConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAAZFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampConstantBounds")]
    public abstract BigInteger? ClampConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAAZFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampConstantExpressions")]
    public abstract BigInteger? ClampConstantExpressions(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampDynamicBounds")]
    public abstract BigInteger? ClampDynamicBounds(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAAqACpTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampEqualBounds")]
    public abstract BigInteger? ClampEqualBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEGA00xqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcDStcQBkcmhpaktLMhFFRUUMCW1pbiA+IG1heDpTurlYAegDoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// INITSLOT 0300 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// CALL D3 [512 datoshi]
    /// PUSH10 [1 datoshi]
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
    /// STLOC0 [2 datoshi]
    /// CALL AD [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSHINT16 E803 [1 datoshi]
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
    [DisplayName("clampEvaluationOrder")]
    public abstract BigInteger? ClampEvaluationOrder();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIAAACAAv///39TurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampIntExtremeBounds")]
    public abstract BigInteger? ClampIntExtremeBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABkEEtLMhFFRUUMCW1pbiA+IG1heDpTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampInvalidBounds")]
    public abstract BigInteger? ClampInvalidBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBoVS0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampInvalidNamedBounds")]
    public abstract BigInteger? ClampInvalidNamedBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAMAAAAAAAAAgBBTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampLongConstantBounds")]
    public abstract BigInteger? ClampLongConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAAZFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampNamedArguments")]
    public abstract BigInteger? ClampNamedArguments(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHAQcSOkAAAAaHhpnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxAaU7q5aRKisSQFESIDEqBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+eSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRMxX////2h4EBVTurmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP_L A4000000 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// PUSH0 [1 datoshi]
    /// PUSH10 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMP 03 [2 datoshi]
    /// PUSH2 [1 datoshi]
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
    /// PUSH3 [1 datoshi]
    /// JMPLT_L 5FFFFFFF [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
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
    [DisplayName("clampNested")]
    public abstract BigInteger? ClampNested(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAD7FVO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 FB [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampSByteConstantBounds")]
    public abstract BigInteger? ClampSByteConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAHU/gEsAVO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 D4FE [1 datoshi]
    /// PUSHINT16 2C01 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampShortConstantBounds")]
    public abstract BigInteger? ClampShortConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAEGA0fgHIAJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xEHIAZHNpamtTurlwaFgB6AOgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// INITSLOT 0400 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// CALL 7E [512 datoshi]
    /// PUSHINT16 C800 [1 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSHINT16 E803 [1 datoshi]
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
    [DisplayName("clampSideEffectValueConstantBounds")]
    public abstract BigInteger? ClampSideEffectValueConstantBounds();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBED/////wAAAABTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUIntConstantBounds")]
    public abstract BigInteger? ClampUIntConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBUE//////////8AAAAAAAAAAFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampULongConstantBounds")]
    public abstract BigInteger? ClampULongConstantBounds(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBoCYOoAAFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSHINT32 60EA0000 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUShortConstantBounds")]
    public abstract BigInteger? ClampUShortConstantBounds(BigInteger? value);

    #endregion
}
