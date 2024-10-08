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
    /// Script: DERlYnVnIGNvbXBpbGF0aW9uEcAMRGVidWdBlQFvYRFA
    /// 00 : OpCode.PUSHDATA1 446562756720636F6D70696C6174696F6E
    /// 13 : OpCode.PUSH1
    /// 14 : OpCode.PACK
    /// 15 : OpCode.PUSHDATA1 4465627567
    /// 1C : OpCode.SYSCALL 95016F61
    /// 21 : OpCode.PUSH1
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("testElse")]
    public abstract BigInteger? TestElse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEnARcGhA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH2
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH1
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.LDLOC0
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("testIf")]
    public abstract BigInteger? TestIf();

    #endregion
}
