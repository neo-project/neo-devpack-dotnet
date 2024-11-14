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
    /// Script: VwICE3B4eYvbKHFpDAVoZWxsb5cmBhRwIg9pDAV3b3JsZJcmBBVwaEA=
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : LDARG1 [2 datoshi]
    /// 07 : CAT [2048 datoshi]
    /// 08 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0A : STLOC1 [2 datoshi]
    /// 0B : LDLOC1 [2 datoshi]
    /// 0C : PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// 13 : EQUAL [32 datoshi]
    /// 14 : JMPIFNOT 06 [2 datoshi]
    /// 16 : PUSH4 [1 datoshi]
    /// 17 : STLOC0 [2 datoshi]
    /// 18 : JMP 0F [2 datoshi]
    /// 1A : LDLOC1 [2 datoshi]
    /// 1B : PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// 22 : EQUAL [32 datoshi]
    /// 23 : JMPIFNOT 04 [2 datoshi]
    /// 25 : PUSH5 [1 datoshi]
    /// 26 : STLOC0 [2 datoshi]
    /// 27 : LDLOC0 [2 datoshi]
    /// 28 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringAdd")]
    public abstract BigInteger? TestStringAdd(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHk3AACL2yhA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : CALLT 0000 [32768 datoshi]
    /// 08 : CAT [2048 datoshi]
    /// 09 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringAddInt")]
    public abstract string? TestStringAddInt(string? s, BigInteger? i);

    #endregion
}
