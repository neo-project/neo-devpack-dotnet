using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IOracle(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IOracle"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""onOracleResponse"",""parameters"":[{""name"":""url"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""code"",""type"":""Integer""},{""name"":""result"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEVXAARBOVNuPAwUWIcXEX4KqBByr6tx0t2J/nxLkv6YJhIMDVVuYXV0aG9yaXplZCE6DAxPcmFjbGUgY2FsbCFBz+dHlkCF1SZT"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEQTlTbjwMFFiHFxF+CqgQcq+rcdLdif58S5L+mCYSDA1VbmF1dGhvcml6ZWQhOgwMT3JhY2xlIGNhbGwhQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0004 [64 datoshi]
    /// 03 : OpCode.SYSCALL 39536E3C 'System.Runtime.GetCallingScriptHash' [16 datoshi]
    /// 08 : OpCode.PUSHDATA1 588717117E0AA81072AFAB71D2DD89FE7C4B92FE [8 datoshi]
    /// 1E : OpCode.NOTEQUAL [32 datoshi]
    /// 1F : OpCode.JMPIFNOT 12 [2 datoshi]
    /// 21 : OpCode.PUSHDATA1 556E617574686F72697A656421 'Unauthorized!' [8 datoshi]
    /// 30 : OpCode.THROW [512 datoshi]
    /// 31 : OpCode.PUSHDATA1 4F7261636C652063616C6C21 [8 datoshi]
    /// 3F : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 44 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("onOracleResponse")]
    public abstract void OnOracleResponse(string? url, object? userData, BigInteger? code, string? result);

    #endregion
}
