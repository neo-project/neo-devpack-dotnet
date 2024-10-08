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
    /// <remarks>
    /// Script: VwAEe3p5eEFifVtSQA==
    /// 00 : OpCode.INITSLOT 0004
    /// 03 : OpCode.LDARG3
    /// 04 : OpCode.LDARG2
    /// 05 : OpCode.LDARG1
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SYSCALL 627D5B52
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("call")]
    public abstract object? Call(UInt160? scriptHash, string? method, BigInteger? flag, IList<object>? args);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AABA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.CONVERT 28
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHPmYcCQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SYSCALL CF998702
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("createStandardAccount")]
    public abstract UInt160? CreateStandardAccount(ECPoint? pubKey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZXaOoFA
    /// 00 : OpCode.SYSCALL 95DA3A81
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();

    #endregion
}
