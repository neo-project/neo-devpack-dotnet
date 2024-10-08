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
    /// 00 : OpCode.INITSLOT 0103
    /// 03 : OpCode.PUSHDATA1
    /// 05 : OpCode.STLOC0
    /// 06 : OpCode.LDLOC0
    /// 07 : OpCode.LDARG0
    /// 08 : OpCode.CAT
    /// 09 : OpCode.CONVERT 28
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.LDARG1
    /// 0E : OpCode.CAT
    /// 0F : OpCode.CONVERT 28
    /// 11 : OpCode.STLOC0
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.LDARG2
    /// 14 : OpCode.CAT
    /// 15 : OpCode.CONVERT 28
    /// 17 : OpCode.STLOC0
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringAddAssign")]
    public abstract byte[]? ByteStringAddAssign(byte[]? a, byte[]? b, string? c);

    #endregion
}
