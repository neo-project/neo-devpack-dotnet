using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""inlineThenUseCallerParameter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":337,""safe"":false},{""name"":""inlineDuplicateParameter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":381,""safe"":false},{""name"":""arrowMethod"",""parameters"":[],""returntype"":""Integer"",""offset"":423,""safe"":false},{""name"":""arrowMethodNoRerurn"",""parameters"":[],""returntype"":""Void"",""offset"":467,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0IAlcDAXhwaAwGaW5saW5llyYEEUBoDBppbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhNxaUBoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYrExJxcmlqnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BoDApub3RfaW5saW5llyYINY4AAABAaAwebm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYGEzRlQGgMIG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYHExI0P0BoDA1pbmxpbmVfbmVzdGVklyYFNFFACCYFAGNADBZObyBzd2l0Y2ggYXJtIG1hdGNoZWQuOhFAVwABeEBXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AE0BXAgF4cWlweGieSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAXhwaGieSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcCABIRcHFoaZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwIAERFwcWloNARFQFcAAnh5nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0CiRGVq").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEhFwcWhpnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
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
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("arrowMethod")]
    public abstract BigInteger? ArrowMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAERFwcWloNARFQA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 04 [512 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("arrowMethodNoRerurn")]
    public abstract void ArrowMethodNoRerurn();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoaJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
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
    [DisplayName("inlineDuplicateParameter")]
    public abstract BigInteger? InlineDuplicateParameter(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeHFpcHhonkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
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
    [DisplayName("inlineThenUseCallerParameter")]
    public abstract BigInteger? InlineThenUseCallerParameter(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeHBoDAZpbmxpbmWXJgQRQGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYGE3FpQGgMHGlubGluZV93aXRoX211bHRpX3BhcmFtZXRlcnOXJisTEnFyaWqeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQGgMCm5vdF9pbmxpbmWXJgg1jgAAAEBoDB5ub3RfaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJgYTNGVAaAwgbm90X2lubGluZV93aXRoX211bHRpX3BhcmFtZXRlcnOXJgcTEjQ/QGgMDWlubGluZV9uZXN0ZWSXJgU0UUAIJgUAY0AMFk5vIHN3aXRjaCBhcm0gbWF0Y2hlZC46
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 696E6C696E65 'inline' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 696E6C696E655F776974685F6F6E655F706172616D6574657273 'inline_with_one_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273 'inline_with_multi_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 2B [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 6E6F745F696E6C696E65 'not_inline' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// CALL_L 8E000000 [512 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273 'not_inline_with_one_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// CALL 65 [512 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273 'not_inline_with_multi_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// CALL 3F [512 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 696E6C696E655F6E6573746564 'inline_nested' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// CALL 51 [512 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 63 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHDATA1 4E6F207377697463682061726D206D6174636865642E [8 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
