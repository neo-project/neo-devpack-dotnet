using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Debug(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Debug"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testElse"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testIf"",""parameters"":[],""returntype"":""Integer"",""offset"":35,""safe"":false}],""events"":[{""name"":""Debug"",""parameters"":[{""name"":""message"",""type"":""String""}]}]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACwMEURlYnVnIGNvbXBpbGF0aW9uEcAMBURlYnVnQZUBb2ERQFcBABJwEXBoQPQ+32w="));

    #endregion

    #region Events

    public delegate void delDebug(string? message);

    [DisplayName("Debug")]
    public event delDebug? OnDebug;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBFEZWJ1ZyBjb21waWxhdGlvbhHADAVEZWJ1Z0GVAW9hEUA=
    /// 00 : PUSHDATA1 446562756720636F6D70696C6174696F6E [8 datoshi]
    /// 13 : PUSH1 [1 datoshi]
    /// 14 : PACK [2048 datoshi]
    /// 15 : PUSHDATA1 4465627567 'Debug' [8 datoshi]
    /// 1C : SYSCALL 95016F61 'System.Runtime.Notify' [32768 datoshi]
    /// 21 : PUSH1 [1 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testElse")]
    public abstract BigInteger? TestElse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEnARcGhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH2 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : STLOC0 [2 datoshi]
    /// 07 : LDLOC0 [2 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIf")]
    public abstract BigInteger? TestIf();

    #endregion
}
