using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Delegate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Delegate"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sumFunc"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testDelegate"",""parameters"":[],""returntype"":""Void"",""offset"":118,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAAmFcAAnl4CgcAAAA2QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAgAKyP///3AWFWg2cQwFU3VtOiBpNwAAi9soQc/nR5ZA1O3dOg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgKBwAAADZA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.PUSHA 07000000 [4 datoshi]
    /// 0A : OpCode.CALLA [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumFunc")]
    public abstract BigInteger? SumFunc(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIACsj///9wFhVoNnEMBVN1bTogaTcAAIvbKEHP50eWQA==
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHA C8FFFFFF [4 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.PUSH6 [1 datoshi]
    /// 0A : OpCode.PUSH5 [1 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.CALLA [512 datoshi]
    /// 0D : OpCode.STLOC1 [2 datoshi]
    /// 0E : OpCode.PUSHDATA1 53756D3A20 [8 datoshi]
    /// 15 : OpCode.LDLOC1 [2 datoshi]
    /// 16 : OpCode.CALLT 0000 [32768 datoshi]
    /// 19 : OpCode.CAT [2048 datoshi]
    /// 1A : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 1C : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDelegate")]
    public abstract void TestDelegate();

    #endregion
}
