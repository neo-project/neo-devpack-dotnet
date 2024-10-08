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
    /// Script: VwICE3B4eYvbKHFpDGhlbGxvlyYGFHAiD2kMd29ybGSXJgQVcGhA
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.LDARG1
    /// 07 : OpCode.CAT
    /// 08 : OpCode.CONVERT 28
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.LDLOC1
    /// 0C : OpCode.PUSHDATA1 68656C6C6F
    /// 13 : OpCode.EQUAL
    /// 14 : OpCode.JMPIFNOT 06
    /// 16 : OpCode.PUSH4
    /// 17 : OpCode.STLOC0
    /// 18 : OpCode.JMP 0F
    /// 1A : OpCode.LDLOC1
    /// 1B : OpCode.PUSHDATA1 776F726C64
    /// 22 : OpCode.EQUAL
    /// 23 : OpCode.JMPIFNOT 04
    /// 25 : OpCode.PUSH5
    /// 26 : OpCode.STLOC0
    /// 27 : OpCode.LDLOC0
    /// 28 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAdd")]
    public abstract BigInteger? TestStringAdd(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHk3AACL2yhA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.CALLT 0000
    /// 08 : OpCode.CAT
    /// 09 : OpCode.CONVERT 28
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAddInt")]
    public abstract string? TestStringAddInt(string? s, BigInteger? i);

    #endregion
}
