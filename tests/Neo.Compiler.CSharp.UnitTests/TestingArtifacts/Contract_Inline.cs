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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO9XAQF4cGgMBmlubGluZZcmBBFAaAwaaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJgQTQGgMHGlubGluZV93aXRoX211bHRpX3BhcmFtZXRlcnOXJgYTEp5AaAwKbm90X2lubGluZZcmBTR0QGgMHm5vdF9pbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBhM0TkBoDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmBxMSNChAaAwNaW5saW5lX25lc3RlZJcmBTQZQAgmBQBjQGg6EUBXAAF4QFcAAnh5nkATQPMsY5Q="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoDGlubGluZZcmBBFAaAxpbmxpbmVfd2l0aF9vbmVfcGFyYW1ldGVyc5cmBBNAaAxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYGExKeQGgMbm90X2lubGluZZcmBTR0QGgMbm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYGEzROQGgMbm90X2lubGluZV93aXRoX211bHRpX3BhcmFtZXRlcnOXJgcTEjQoQGgMaW5saW5lX25lc3RlZJcmBTQZQAgmBQBjQGg6
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHDATA1 696E6C696E65
    /// 0E : OpCode.EQUAL
    /// 0F : OpCode.JMPIFNOT 04
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.RET
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSHDATA1 696E6C696E655F776974685F6F6E655F706172616D6574657273
    /// 30 : OpCode.EQUAL
    /// 31 : OpCode.JMPIFNOT 04
    /// 33 : OpCode.PUSH3
    /// 34 : OpCode.RET
    /// 35 : OpCode.LDLOC0
    /// 36 : OpCode.PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273
    /// 54 : OpCode.EQUAL
    /// 55 : OpCode.JMPIFNOT 06
    /// 57 : OpCode.PUSH3
    /// 58 : OpCode.PUSH2
    /// 59 : OpCode.ADD
    /// 5A : OpCode.RET
    /// 5B : OpCode.LDLOC0
    /// 5C : OpCode.PUSHDATA1 6E6F745F696E6C696E65
    /// 68 : OpCode.EQUAL
    /// 69 : OpCode.JMPIFNOT 05
    /// 6B : OpCode.CALL 74
    /// 6D : OpCode.RET
    /// 6E : OpCode.LDLOC0
    /// 6F : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273
    /// 8F : OpCode.EQUAL
    /// 90 : OpCode.JMPIFNOT 06
    /// 92 : OpCode.PUSH3
    /// 93 : OpCode.CALL 4E
    /// 95 : OpCode.RET
    /// 96 : OpCode.LDLOC0
    /// 97 : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273
    /// B9 : OpCode.EQUAL
    /// BA : OpCode.JMPIFNOT 07
    /// BC : OpCode.PUSH3
    /// BD : OpCode.PUSH2
    /// BE : OpCode.CALL 28
    /// C0 : OpCode.RET
    /// C1 : OpCode.LDLOC0
    /// C2 : OpCode.PUSHDATA1 696E6C696E655F6E6573746564
    /// D1 : OpCode.EQUAL
    /// D2 : OpCode.JMPIFNOT 05
    /// D4 : OpCode.CALL 19
    /// D6 : OpCode.RET
    /// D7 : OpCode.PUSHT
    /// D8 : OpCode.JMPIFNOT 05
    /// DA : OpCode.PUSHINT8 63
    /// DC : OpCode.RET
    /// DD : OpCode.LDLOC0
    /// DE : OpCode.THROW
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
