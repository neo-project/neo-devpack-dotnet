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
    /// 0000 : OpCode.INITSLOT 0103
    /// 0003 : OpCode.PUSHDATA1
    /// 0005 : OpCode.STLOC0
    /// 0006 : OpCode.LDLOC0
    /// 0007 : OpCode.LDARG0
    /// 0008 : OpCode.CAT
    /// 0009 : OpCode.CONVERT 28
    /// 000B : OpCode.STLOC0
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.LDARG1
    /// 000E : OpCode.CAT
    /// 000F : OpCode.CONVERT 28
    /// 0011 : OpCode.STLOC0
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.LDARG2
    /// 0014 : OpCode.CAT
    /// 0015 : OpCode.CONVERT 28
    /// 0017 : OpCode.STLOC0
    /// 0018 : OpCode.LDLOC0
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringAddAssign")]
    public abstract byte[]? ByteStringAddAssign(byte[]? a, byte[]? b, string? c);

    #endregion

}
