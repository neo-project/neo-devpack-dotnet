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
    /// Script: VwEBeHBoDGlubGluZZcmBBFAaAxpbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBBNAaAxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyY0ExKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BoDG5vdF9pbmxpbmWXJgU0dEBoDG5vdF9pbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhM0TkBoDG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYHExI0KEBoDGlubGluZV9uZXN0ZWSXJgU0R0AIJgUAY0BoOg==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHDATA1 696E6C696E65
    /// 000E : OpCode.EQUAL
    /// 000F : OpCode.JMPIFNOT 04
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.PUSHDATA1 696E6C696E655F776974685F6F6E655F706172616D6574657273
    /// 0030 : OpCode.EQUAL
    /// 0031 : OpCode.JMPIFNOT 04
    /// 0033 : OpCode.PUSH3
    /// 0034 : OpCode.RET
    /// 0035 : OpCode.LDLOC0
    /// 0036 : OpCode.PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273
    /// 0054 : OpCode.EQUAL
    /// 0055 : OpCode.JMPIFNOT 34
    /// 0057 : OpCode.PUSH3
    /// 0058 : OpCode.PUSH2
    /// 0059 : OpCode.ADD
    /// 005A : OpCode.DUP
    /// 005B : OpCode.PUSHINT32 00000080
    /// 0060 : OpCode.JMPGE 04
    /// 0062 : OpCode.JMP 0A
    /// 0064 : OpCode.DUP
    /// 0065 : OpCode.PUSHINT32 FFFFFF7F
    /// 006A : OpCode.JMPLE 1E
    /// 006C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0075 : OpCode.AND
    /// 0076 : OpCode.DUP
    /// 0077 : OpCode.PUSHINT32 FFFFFF7F
    /// 007C : OpCode.JMPLE 0C
    /// 007E : OpCode.PUSHINT64 0000000001000000
    /// 0087 : OpCode.SUB
    /// 0088 : OpCode.RET
    /// 0089 : OpCode.LDLOC0
    /// 008A : OpCode.PUSHDATA1 6E6F745F696E6C696E65
    /// 0096 : OpCode.EQUAL
    /// 0097 : OpCode.JMPIFNOT 05
    /// 0099 : OpCode.CALL 74
    /// 009B : OpCode.RET
    /// 009C : OpCode.LDLOC0
    /// 009D : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273
    /// 00BD : OpCode.EQUAL
    /// 00BE : OpCode.JMPIFNOT 06
    /// 00C0 : OpCode.PUSH3
    /// 00C1 : OpCode.CALL 4E
    /// 00C3 : OpCode.RET
    /// 00C4 : OpCode.LDLOC0
    /// 00C5 : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273
    /// 00E7 : OpCode.EQUAL
    /// 00E8 : OpCode.JMPIFNOT 07
    /// 00EA : OpCode.PUSH3
    /// 00EB : OpCode.PUSH2
    /// 00EC : OpCode.CALL 28
    /// 00EE : OpCode.RET
    /// 00EF : OpCode.LDLOC0
    /// 00F0 : OpCode.PUSHDATA1 696E6C696E655F6E6573746564
    /// 00FF : OpCode.EQUAL
    /// 0100 : OpCode.JMPIFNOT 05
    /// 0102 : OpCode.CALL 47
    /// 0104 : OpCode.RET
    /// 0105 : OpCode.PUSHT
    /// 0106 : OpCode.JMPIFNOT 05
    /// 0108 : OpCode.PUSHINT8 63
    /// 010A : OpCode.RET
    /// 010B : OpCode.LDLOC0
    /// 010C : OpCode.THROW
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
