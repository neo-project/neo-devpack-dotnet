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
    [DisplayName("byteStringAddAssign")]
    public abstract byte[]? ByteStringAddAssign(byte[]? a, byte[]? b, string? c);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0005 : STLOC0
    // 0006 : LDLOC0
    // 0007 : LDARG0
    // 0008 : CAT
    // 0009 : CONVERT
    // 000B : STLOC0
    // 000C : LDLOC0
    // 000D : LDARG1
    // 000E : CAT
    // 000F : CONVERT
    // 0011 : STLOC0
    // 0012 : LDLOC0
    // 0013 : LDARG2
    // 0014 : CAT
    // 0015 : CONVERT
    // 0017 : STLOC0
    // 0018 : LDLOC0
    // 0019 : RET

    #endregion

}
