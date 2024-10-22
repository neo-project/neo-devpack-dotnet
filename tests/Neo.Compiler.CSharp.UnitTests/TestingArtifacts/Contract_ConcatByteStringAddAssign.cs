using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ConcatByteStringAddAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ConcatByteStringAddAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""byteStringAddAssign"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""},{""name"":""c"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpXAQMMAHBoeIvbKHBoeYvbKHBoeovbKHBoQN/RbZ0="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEDDHBoeIvbKHBoeYvbKHBoeovbKHBoQA==
    /// 00 : OpCode.INITSLOT 0103 	-> 64 datoshi
    /// 03 : OpCode.PUSHDATA1 	-> 8 datoshi
    /// 05 : OpCode.STLOC0 	-> 2 datoshi
    /// 06 : OpCode.LDLOC0 	-> 2 datoshi
    /// 07 : OpCode.LDARG0 	-> 2 datoshi
    /// 08 : OpCode.CAT 	-> 2048 datoshi
    /// 09 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 0B : OpCode.STLOC0 	-> 2 datoshi
    /// 0C : OpCode.LDLOC0 	-> 2 datoshi
    /// 0D : OpCode.LDARG1 	-> 2 datoshi
    /// 0E : OpCode.CAT 	-> 2048 datoshi
    /// 0F : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 11 : OpCode.STLOC0 	-> 2 datoshi
    /// 12 : OpCode.LDLOC0 	-> 2 datoshi
    /// 13 : OpCode.LDARG2 	-> 2 datoshi
    /// 14 : OpCode.CAT 	-> 2048 datoshi
    /// 15 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 17 : OpCode.STLOC0 	-> 2 datoshi
    /// 18 : OpCode.LDLOC0 	-> 2 datoshi
    /// 19 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("byteStringAddAssign")]
    public abstract byte[]? ByteStringAddAssign(byte[]? a, byte[]? b, string? c);

    #endregion
}
