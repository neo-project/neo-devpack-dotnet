using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_String(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_String"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStringAdd"",""parameters"":[{""name"":""s1"",""type"":""String""},{""name"":""s2"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testStringAddInt"",""parameters"":[{""name"":""s"",""type"":""String""},{""name"":""i"",""type"":""Integer""}],""returntype"":""String"",""offset"":41,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAANVcCAhNweHmL2yhxaQwFaGVsbG+XJgYUcCIPaQwFd29ybGSXJgQVcGhAVwACeHk3AACL2yhAvuYMzA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd")]
    public abstract BigInteger? TestStringAdd(string? s1, string? s2);
    // 0000 : INITSLOT
    // 0003 : PUSH3
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : LDARG1
    // 0007 : CAT
    // 0008 : CONVERT
    // 000A : STLOC1
    // 000B : LDLOC1
    // 000C : PUSHDATA1
    // 0013 : EQUAL
    // 0014 : JMPIFNOT
    // 0016 : PUSH4
    // 0017 : STLOC0
    // 0018 : JMP
    // 001A : LDLOC1
    // 001B : PUSHDATA1
    // 0022 : EQUAL
    // 0023 : JMPIFNOT
    // 0025 : PUSH5
    // 0026 : STLOC0
    // 0027 : LDLOC0
    // 0028 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAddInt")]
    public abstract string? TestStringAddInt(string? s, BigInteger? i);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : CALLT
    // 0008 : CAT
    // 0009 : CONVERT
    // 000B : RET

    #endregion

}
