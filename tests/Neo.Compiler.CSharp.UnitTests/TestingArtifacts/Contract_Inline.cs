using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1LAVcBAXhwaAwGaW5saW5llyYEEUBoDBppbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBBNAaAwcaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmNBMSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AaAwKbm90X2lubGluZZcmBTR0QGgMHm5vdF9pbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhM0TkBoDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmBxMSNChAaAwNaW5saW5lX25lc3RlZJcmBTRHQAgmBQBjQGg6EUBXAAF4QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AE0Cui6ZE").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDAZpbmxpbmWXJgQRQGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYEE0BoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyY0ExKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BoDApub3RfaW5saW5llyYFNHRAaAwebm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYGEzROQGgMIG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYHExI0KEBoDA1pbmxpbmVfbmVzdGVklyYFNEdACCYFAGNAaDo=
    /// 0000 : INITSLOT 0101 [64 datoshi]
    /// 0003 : LDARG0 [2 datoshi]
    /// 0004 : STLOC0 [2 datoshi]
    /// 0005 : LDLOC0 [2 datoshi]
    /// 0006 : PUSHDATA1 696E6C696E65 'inline' [8 datoshi]
    /// 000E : EQUAL [32 datoshi]
    /// 000F : JMPIFNOT 04 [2 datoshi]
    /// 0011 : PUSH1 [1 datoshi]
    /// 0012 : RET [0 datoshi]
    /// 0013 : LDLOC0 [2 datoshi]
    /// 0014 : PUSHDATA1 696E6C696E655F776974685F6F6E655F706172616D6574657273 'inline_with_one_parameters' [8 datoshi]
    /// 0030 : EQUAL [32 datoshi]
    /// 0031 : JMPIFNOT 04 [2 datoshi]
    /// 0033 : PUSH3 [1 datoshi]
    /// 0034 : RET [0 datoshi]
    /// 0035 : LDLOC0 [2 datoshi]
    /// 0036 : PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273 'inline_with_multi_parameters' [8 datoshi]
    /// 0054 : EQUAL [32 datoshi]
    /// 0055 : JMPIFNOT 34 [2 datoshi]
    /// 0057 : PUSH3 [1 datoshi]
    /// 0058 : PUSH2 [1 datoshi]
    /// 0059 : ADD [8 datoshi]
    /// 005A : DUP [2 datoshi]
    /// 005B : PUSHINT32 00000080 [1 datoshi]
    /// 0060 : JMPGE 04 [2 datoshi]
    /// 0062 : JMP 0A [2 datoshi]
    /// 0064 : DUP [2 datoshi]
    /// 0065 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 006A : JMPLE 1E [2 datoshi]
    /// 006C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0075 : AND [8 datoshi]
    /// 0076 : DUP [2 datoshi]
    /// 0077 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 007C : JMPLE 0C [2 datoshi]
    /// 007E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0087 : SUB [8 datoshi]
    /// 0088 : RET [0 datoshi]
    /// 0089 : LDLOC0 [2 datoshi]
    /// 008A : PUSHDATA1 6E6F745F696E6C696E65 'not_inline' [8 datoshi]
    /// 0096 : EQUAL [32 datoshi]
    /// 0097 : JMPIFNOT 05 [2 datoshi]
    /// 0099 : CALL 74 [512 datoshi]
    /// 009B : RET [0 datoshi]
    /// 009C : LDLOC0 [2 datoshi]
    /// 009D : PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273 'not_inline_with_one_parameters' [8 datoshi]
    /// 00BD : EQUAL [32 datoshi]
    /// 00BE : JMPIFNOT 06 [2 datoshi]
    /// 00C0 : PUSH3 [1 datoshi]
    /// 00C1 : CALL 4E [512 datoshi]
    /// 00C3 : RET [0 datoshi]
    /// 00C4 : LDLOC0 [2 datoshi]
    /// 00C5 : PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273 'not_inline_with_multi_parameters' [8 datoshi]
    /// 00E7 : EQUAL [32 datoshi]
    /// 00E8 : JMPIFNOT 07 [2 datoshi]
    /// 00EA : PUSH3 [1 datoshi]
    /// 00EB : PUSH2 [1 datoshi]
    /// 00EC : CALL 28 [512 datoshi]
    /// 00EE : RET [0 datoshi]
    /// 00EF : LDLOC0 [2 datoshi]
    /// 00F0 : PUSHDATA1 696E6C696E655F6E6573746564 'inline_nested' [8 datoshi]
    /// 00FF : EQUAL [32 datoshi]
    /// 0100 : JMPIFNOT 05 [2 datoshi]
    /// 0102 : CALL 47 [512 datoshi]
    /// 0104 : RET [0 datoshi]
    /// 0105 : PUSHT [1 datoshi]
    /// 0106 : JMPIFNOT 05 [2 datoshi]
    /// 0108 : PUSHINT8 63 [1 datoshi]
    /// 010A : RET [0 datoshi]
    /// 010B : LDLOC0 [2 datoshi]
    /// 010C : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
