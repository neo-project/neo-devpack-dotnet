using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Contract(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Contract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""call"",""parameters"":[{""name"":""scriptHash"",""type"":""Hash160""},{""name"":""method"",""type"":""String""},{""name"":""flag"",""type"":""Integer""},{""name"":""args"",""type"":""Array""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""create"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Any"",""offset"":13,""safe"":false},{""name"":""getCallFlags"",""parameters"":[],""returntype"":""Integer"",""offset"":25,""safe"":false},{""name"":""createStandardAccount"",""parameters"":[{""name"":""pubKey"",""type"":""PublicKey""}],""returntype"":""Hash160"",""offset"":31,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wZkZXBsb3kDAAEPAAApVwAEe3p5eEFifVtSQFcAAgt5eNsoNwAAQEGV2jqBQFcAAXhBz5mHAkAP/HGK"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("call")]
    public abstract object? Call(UInt160? scriptHash, string? method, BigInteger? flag, IList<object>? args);
    // 0000 : INITSLOT
    // 0003 : LDARG3
    // 0004 : LDARG2
    // 0005 : LDARG1
    // 0006 : LDARG0
    // 0007 : SYSCALL
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : LDARG1
    // 0005 : LDARG0
    // 0006 : CONVERT
    // 0008 : CALLT
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createStandardAccount")]
    public abstract UInt160? CreateStandardAccount(ECPoint? pubKey);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SYSCALL
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();
    // 0000 : SYSCALL
    // 0005 : RET

    #endregion

}
