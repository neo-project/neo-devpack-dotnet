using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1LAVcBAXhwaAwGaW5saW5llyYEEUBoDBppbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBBNAaAwcaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmNBMSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AaAwKbm90X2lubGluZZcmBTR0QGgMHm5vdF9pbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhM0TkBoDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmBxMSNChAaAwNaW5saW5lX25lc3RlZJcmBTRHQAgmBQBjQGg6EUBXAAF4QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AE0Cui6ZE"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDAZpbmxpbmWXJgQRQGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYEE0BoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyY0ExKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BoDApub3RfaW5saW5llyYFNHRAaAwebm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYGEzROQGgMIG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYHExI0KEBoDA1pbmxpbmVfbmVzdGVklyYFNEdACCYFAGNAaDo=
    /// 0000 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 0003 : OpCode.LDARG0 [2 datoshi]
    /// 0004 : OpCode.STLOC0 [2 datoshi]
    /// 0005 : OpCode.LDLOC0 [2 datoshi]
    /// 0006 : OpCode.PUSHDATA1 696E6C696E65 'inline' [8 datoshi]
    /// 000E : OpCode.EQUAL [32 datoshi]
    /// 000F : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0011 : OpCode.PUSH1 [1 datoshi]
    /// 0012 : OpCode.RET [0 datoshi]
    /// 0013 : OpCode.LDLOC0 [2 datoshi]
    /// 0014 : OpCode.PUSHDATA1 696E6C696E655F776974685F6F6E655F706172616D6574657273 'inline_with_one_parameters' [8 datoshi]
    /// 0030 : OpCode.EQUAL [32 datoshi]
    /// 0031 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0033 : OpCode.PUSH3 [1 datoshi]
    /// 0034 : OpCode.RET [0 datoshi]
    /// 0035 : OpCode.LDLOC0 [2 datoshi]
    /// 0036 : OpCode.PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273 'inline_with_multi_parameters' [8 datoshi]
    /// 0054 : OpCode.EQUAL [32 datoshi]
    /// 0055 : OpCode.JMPIFNOT 34 [2 datoshi]
    /// 0057 : OpCode.PUSH3 [1 datoshi]
    /// 0058 : OpCode.PUSH2 [1 datoshi]
    /// 0059 : OpCode.ADD [8 datoshi]
    /// 005A : OpCode.DUP [2 datoshi]
    /// 005B : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0060 : OpCode.JMPGE 04 [2 datoshi]
    /// 0062 : OpCode.JMP 0A [2 datoshi]
    /// 0064 : OpCode.DUP [2 datoshi]
    /// 0065 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 006A : OpCode.JMPLE 1E [2 datoshi]
    /// 006C : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0075 : OpCode.AND [8 datoshi]
    /// 0076 : OpCode.DUP [2 datoshi]
    /// 0077 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 007C : OpCode.JMPLE 0C [2 datoshi]
    /// 007E : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 0087 : OpCode.SUB [8 datoshi]
    /// 0088 : OpCode.RET [0 datoshi]
    /// 0089 : OpCode.LDLOC0 [2 datoshi]
    /// 008A : OpCode.PUSHDATA1 6E6F745F696E6C696E65 'not_inline' [8 datoshi]
    /// 0096 : OpCode.EQUAL [32 datoshi]
    /// 0097 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0099 : OpCode.CALL 74 [512 datoshi]
    /// 009B : OpCode.RET [0 datoshi]
    /// 009C : OpCode.LDLOC0 [2 datoshi]
    /// 009D : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273 'not_inline_with_one_parameters' [8 datoshi]
    /// 00BD : OpCode.EQUAL [32 datoshi]
    /// 00BE : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 00C0 : OpCode.PUSH3 [1 datoshi]
    /// 00C1 : OpCode.CALL 4E [512 datoshi]
    /// 00C3 : OpCode.RET [0 datoshi]
    /// 00C4 : OpCode.LDLOC0 [2 datoshi]
    /// 00C5 : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273 'not_inline_with_multi_parameters' [8 datoshi]
    /// 00E7 : OpCode.EQUAL [32 datoshi]
    /// 00E8 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 00EA : OpCode.PUSH3 [1 datoshi]
    /// 00EB : OpCode.PUSH2 [1 datoshi]
    /// 00EC : OpCode.CALL 28 [512 datoshi]
    /// 00EE : OpCode.RET [0 datoshi]
    /// 00EF : OpCode.LDLOC0 [2 datoshi]
    /// 00F0 : OpCode.PUSHDATA1 696E6C696E655F6E6573746564 'inline_nested' [8 datoshi]
    /// 00FF : OpCode.EQUAL [32 datoshi]
    /// 0100 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0102 : OpCode.CALL 47 [512 datoshi]
    /// 0104 : OpCode.RET [0 datoshi]
    /// 0105 : OpCode.PUSHT [1 datoshi]
    /// 0106 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0108 : OpCode.PUSHINT8 63 [1 datoshi]
    /// 010A : OpCode.RET [0 datoshi]
    /// 010B : OpCode.LDLOC0 [2 datoshi]
    /// 010C : OpCode.THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
