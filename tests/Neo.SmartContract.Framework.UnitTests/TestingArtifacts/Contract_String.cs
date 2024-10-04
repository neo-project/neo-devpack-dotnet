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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSH3
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.LDARG1
    /// 0007 : OpCode.CAT
    /// 0008 : OpCode.CONVERT 28
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.LDLOC1
    /// 000C : OpCode.PUSHDATA1 68656C6C6F
    /// 0013 : OpCode.EQUAL
    /// 0014 : OpCode.JMPIFNOT 06
    /// 0016 : OpCode.PUSH4
    /// 0017 : OpCode.STLOC0
    /// 0018 : OpCode.JMP 0F
    /// 001A : OpCode.LDLOC1
    /// 001B : OpCode.PUSHDATA1 776F726C64
    /// 0022 : OpCode.EQUAL
    /// 0023 : OpCode.JMPIFNOT 04
    /// 0025 : OpCode.PUSH5
    /// 0026 : OpCode.STLOC0
    /// 0027 : OpCode.LDLOC0
    /// 0028 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAdd")]
    public abstract BigInteger? TestStringAdd(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.CALLT 0000
    /// 0008 : OpCode.CAT
    /// 0009 : OpCode.CONVERT 28
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAddInt")]
    public abstract string? TestStringAddInt(string? s, BigInteger? i);

    #endregion

}
