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
    /// 00 : INITSLOT 0004 [64 datoshi]
    /// 03 : LDARG3 [2 datoshi]
    /// 04 : LDARG2 [2 datoshi]
    /// 05 : LDARG1 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("call")]
    public abstract object? Call(UInt160? scriptHash, string? method, BigInteger? flag, IList<object>? args);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AABA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHPmYcCQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SYSCALL CF998702 'System.Contract.CreateStandardAccount' [0 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createStandardAccount")]
    public abstract UInt160? CreateStandardAccount(ECPoint? pubKey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZXaOoFA
    /// 00 : SYSCALL 95DA3A81 'System.Contract.GetCallFlags' [1024 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();

    #endregion
}
