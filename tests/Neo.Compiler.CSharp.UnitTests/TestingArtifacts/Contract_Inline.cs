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
    /// Script: VwEBeHBoDAZpbmxpbmWXJgQRQGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYEE0BoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYGExKeQGgMCm5vdF9pbmxpbmWXJgU0dEBoDB5ub3RfaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJgYTNE5AaAwgbm90X2lubGluZV93aXRoX211bHRpX3BhcmFtZXRlcnOXJgcTEjQoQGgMDWlubGluZV9uZXN0ZWSXJgU0GUAIJgUAY0BoOg==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSHDATA1 696E6C696E65 [8 datoshi]
    /// 0E : OpCode.EQUAL [32 datoshi]
    /// 0F : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 11 : OpCode.PUSH1 [1 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSHDATA1 696E6C696E655F776974685F6F6E655F706172616D6574657273 [8 datoshi]
    /// 30 : OpCode.EQUAL [32 datoshi]
    /// 31 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 33 : OpCode.PUSH3 [1 datoshi]
    /// 34 : OpCode.RET [0 datoshi]
    /// 35 : OpCode.LDLOC0 [2 datoshi]
    /// 36 : OpCode.PUSHDATA1 696E6C696E655F776974685F6D756C74695F706172616D6574657273 [8 datoshi]
    /// 54 : OpCode.EQUAL [32 datoshi]
    /// 55 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 57 : OpCode.PUSH3 [1 datoshi]
    /// 58 : OpCode.PUSH2 [1 datoshi]
    /// 59 : OpCode.ADD [8 datoshi]
    /// 5A : OpCode.RET [0 datoshi]
    /// 5B : OpCode.LDLOC0 [2 datoshi]
    /// 5C : OpCode.PUSHDATA1 6E6F745F696E6C696E65 [8 datoshi]
    /// 68 : OpCode.EQUAL [32 datoshi]
    /// 69 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 6B : OpCode.CALL 74 [512 datoshi]
    /// 6D : OpCode.RET [0 datoshi]
    /// 6E : OpCode.LDLOC0 [2 datoshi]
    /// 6F : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6F6E655F706172616D6574657273 [8 datoshi]
    /// 8F : OpCode.EQUAL [32 datoshi]
    /// 90 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 92 : OpCode.PUSH3 [1 datoshi]
    /// 93 : OpCode.CALL 4E [512 datoshi]
    /// 95 : OpCode.RET [0 datoshi]
    /// 96 : OpCode.LDLOC0 [2 datoshi]
    /// 97 : OpCode.PUSHDATA1 6E6F745F696E6C696E655F776974685F6D756C74695F706172616D6574657273 [8 datoshi]
    /// B9 : OpCode.EQUAL [32 datoshi]
    /// BA : OpCode.JMPIFNOT 07 [2 datoshi]
    /// BC : OpCode.PUSH3 [1 datoshi]
    /// BD : OpCode.PUSH2 [1 datoshi]
    /// BE : OpCode.CALL 28 [512 datoshi]
    /// C0 : OpCode.RET [0 datoshi]
    /// C1 : OpCode.LDLOC0 [2 datoshi]
    /// C2 : OpCode.PUSHDATA1 696E6C696E655F6E6573746564 [8 datoshi]
    /// D1 : OpCode.EQUAL [32 datoshi]
    /// D2 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// D4 : OpCode.CALL 19 [512 datoshi]
    /// D6 : OpCode.RET [0 datoshi]
    /// D7 : OpCode.PUSHT [1 datoshi]
    /// D8 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// DA : OpCode.PUSHINT8 63 [1 datoshi]
    /// DC : OpCode.RET [0 datoshi]
    /// DD : OpCode.LDLOC0 [2 datoshi]
    /// DE : OpCode.THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion
}
