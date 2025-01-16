using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""arrowMethod"",""parameters"":[],""returntype"":""Integer"",""offset"":331,""safe"":false},{""name"":""arrowMethodNoRerurn"",""parameters"":[],""returntype"":""Void"",""offset"":381,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP24AVcBAXhwaAwGaW5saW5llyYEEUBoDBppbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBBNAaAwcaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmNBMSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AaAwKbm90X2lubGluZZcmBTR0QGgMHm5vdF9pbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhM0TkBoDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmBxMSNChAaAwNaW5saW5lX25lc3RlZJcmBTRHQAgmBQBjQGg6EUBXAAF4QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AE0ASEZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQBERNARFQFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9Arb2CVw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EhGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
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
    /// Script: ERE0BEVA
    /// PUSH1 [1 datoshi]
    /// PUSH1 [1 datoshi]
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
    /// Script: VwEBeHBoDAZpbmxpbmWXJgQRQGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYEE0BoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyY0ExKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BoDApub3RfaW5saW5llyYFNHRAaAwebm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYGEzROQGgMIG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYHExI0KEBoDA1pbmxpbmVfbmVzdGVklyYFNEdACCYFAGNAaDo=
    /// INITSLOT 0101 [64 datoshi]
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
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273 'inline_with_multi_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 34 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
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
    /// JMPIFNOT 05 [2 datoshi]
    /// CALL 74 [512 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273 'not_inline_with_one_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// CALL 4E [512 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273 'not_inline_with_multi_parameters' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// CALL 28 [512 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 696E6C696E655F6E6573746564 'inline_nested' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// CALL 47 [512 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 63 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
