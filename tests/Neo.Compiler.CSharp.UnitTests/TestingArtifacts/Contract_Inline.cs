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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1OAVcBAXhwaAwGaW5saW5llyYEEUBoDBppbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBBNAaAwcaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmNBMSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AaAwKbm90X2lubGluZZcmCDV3AAAAQGgMHm5vdF9pbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhM0TkBoDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmBxMSNChAaAwNaW5saW5lX25lc3RlZJcmBTRHQAgmBQBjQGg6EUBXAAF4QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AE0Dshy6n"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHDATA1
    // 000E : EQUAL
    // 000F : JMPIFNOT
    // 0011 : PUSH1
    // 0012 : RET
    // 0013 : LDLOC0
    // 0014 : PUSHDATA1
    // 0030 : EQUAL
    // 0031 : JMPIFNOT
    // 0033 : PUSH3
    // 0034 : RET
    // 0035 : LDLOC0
    // 0036 : PUSHDATA1
    // 0054 : EQUAL
    // 0055 : JMPIFNOT
    // 0057 : PUSH3
    // 0058 : PUSH2
    // 0059 : ADD
    // 005A : DUP
    // 005B : PUSHINT32
    // 0060 : JMPGE
    // 0062 : JMP
    // 0064 : DUP
    // 0065 : PUSHINT32
    // 006A : JMPLE
    // 006C : PUSHINT64
    // 0075 : AND
    // 0076 : DUP
    // 0077 : PUSHINT32
    // 007C : JMPLE
    // 007E : PUSHINT64
    // 0087 : SUB
    // 0088 : RET
    // 0089 : LDLOC0
    // 008A : PUSHDATA1
    // 0096 : EQUAL
    // 0097 : JMPIFNOT
    // 0099 : CALL_L
    // 009E : RET
    // 009F : LDLOC0
    // 00A0 : PUSHDATA1
    // 00C0 : EQUAL
    // 00C1 : JMPIFNOT
    // 00C3 : PUSH3
    // 00C4 : CALL
    // 00C6 : RET
    // 00C7 : LDLOC0
    // 00C8 : PUSHDATA1
    // 00EA : EQUAL
    // 00EB : JMPIFNOT
    // 00ED : PUSH3
    // 00EE : PUSH2
    // 00EF : CALL
    // 00F1 : RET
    // 00F2 : LDLOC0
    // 00F3 : PUSHDATA1
    // 0102 : EQUAL
    // 0103 : JMPIFNOT
    // 0105 : CALL
    // 0107 : RET
    // 0108 : PUSHT
    // 0109 : JMPIFNOT
    // 010B : PUSHINT8
    // 010D : RET
    // 010E : LDLOC0
    // 010F : THROW

    #endregion

}
