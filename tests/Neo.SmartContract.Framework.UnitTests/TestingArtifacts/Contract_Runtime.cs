using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Runtime : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Runtime"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getInvocationCounter"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTime"",""parameters"":[],""returntype"":""Integer"",""offset"":8,""safe"":false},{""name"":""getRandom"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":false},{""name"":""getGasLeft"",""parameters"":[],""returntype"":""Integer"",""offset"":24,""safe"":false},{""name"":""getPlatform"",""parameters"":[],""returntype"":""String"",""offset"":32,""safe"":false},{""name"":""getNetwork"",""parameters"":[],""returntype"":""Integer"",""offset"":40,""safe"":false},{""name"":""getAddressVersion"",""parameters"":[],""returntype"":""Integer"",""offset"":48,""safe"":false},{""name"":""getTrigger"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""log"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":64,""safe"":false},{""name"":""checkWitness"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":74,""safe"":false},{""name"":""getNotificationsCount"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":86,""safe"":false},{""name"":""getAllNotifications"",""parameters"":[],""returntype"":""Integer"",""offset"":101,""safe"":false},{""name"":""getNotifications"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":238,""safe"":false},{""name"":""getTransactionHash"",""parameters"":[],""returntype"":""Hash256"",""offset"":373,""safe"":false},{""name"":""getTransactionVersion"",""parameters"":[],""returntype"":""Integer"",""offset"":388,""safe"":false},{""name"":""getTransactionNonce"",""parameters"":[],""returntype"":""Integer"",""offset"":403,""safe"":false},{""name"":""getTransactionSender"",""parameters"":[],""returntype"":""Hash160"",""offset"":418,""safe"":false},{""name"":""getTransaction"",""parameters"":[],""returntype"":""Any"",""offset"":433,""safe"":false},{""name"":""getTransactionSystemFee"",""parameters"":[],""returntype"":""Integer"",""offset"":446,""safe"":false},{""name"":""getTransactionNetworkFee"",""parameters"":[],""returntype"":""Integer"",""offset"":461,""safe"":false},{""name"":""getTransactionValidUntilBlock"",""parameters"":[],""returntype"":""Integer"",""offset"":476,""safe"":false},{""name"":""getTransactionScript"",""parameters"":[],""returntype"":""ByteArray"",""offset"":491,""safe"":false},{""name"":""dynamicSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":506,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM3Rlc3Rob3N0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0TAkGEJxFDIgJAQbfDiAMiAkBBa96pKCICQEEUiNjOIgJAQbJ5/PYiAkBBxfug4CICQEFMSZLcIgJAQel9OKAiAkBXAAF4Qc/nR5ZAVwABeEH4J+yMIgJAVwEBeEEnQzXxcGjKIgJAVwQAEHALQSdDNfFxEHIicWlqznNoaxLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRWpKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9yRWppyrUkjWgiAkBXAwEQcHhBJ0M18XEQciJvaGlqzhLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRWpKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9yRWppyrUkj2giAkBXAQBBLVEIMHBoEM4iAkBXAQBBLVEIMHBoEc4iAkBXAQBBLVEIMHBoEs4iAkBXAQBBLVEIMHBoE84iAkBXAQBBLVEIMHBoIgJAVwEAQS1RCDBwaBTOIgJAVwEAQS1RCDBwaBXOIgJAVwEAQS1RCDBwaBbOIgJAVwEAQS1RCDBwaBfOIgJAVwECDAGe2zDbKHB5eBLAH2hBswyAjyICQKWSp+E="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log")]
    public abstract void Log(string? message);

    #endregion

    #region Constructor for internal use only

    protected Contract_Runtime(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
