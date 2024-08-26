using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Runtime(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Runtime"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getInvocationCounter"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTime"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""getRandom"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":false},{""name"":""getGasLeft"",""parameters"":[],""returntype"":""Integer"",""offset"":18,""safe"":false},{""name"":""getPlatform"",""parameters"":[],""returntype"":""String"",""offset"":24,""safe"":false},{""name"":""getNetwork"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""getAddressVersion"",""parameters"":[],""returntype"":""Integer"",""offset"":36,""safe"":false},{""name"":""getTrigger"",""parameters"":[],""returntype"":""Integer"",""offset"":42,""safe"":false},{""name"":""log"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":48,""safe"":false},{""name"":""checkWitness"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":58,""safe"":false},{""name"":""getNotificationsCount"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":68,""safe"":false},{""name"":""getAllNotifications"",""parameters"":[],""returntype"":""Integer"",""offset"":81,""safe"":false},{""name"":""getNotifications"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":216,""safe"":false},{""name"":""getTransactionHash"",""parameters"":[],""returntype"":""Hash256"",""offset"":349,""safe"":false},{""name"":""getTransactionVersion"",""parameters"":[],""returntype"":""Integer"",""offset"":362,""safe"":false},{""name"":""getTransactionNonce"",""parameters"":[],""returntype"":""Integer"",""offset"":375,""safe"":false},{""name"":""getTransactionSender"",""parameters"":[],""returntype"":""Hash160"",""offset"":388,""safe"":false},{""name"":""getTransaction"",""parameters"":[],""returntype"":""Any"",""offset"":401,""safe"":false},{""name"":""getTransactionSystemFee"",""parameters"":[],""returntype"":""Integer"",""offset"":412,""safe"":false},{""name"":""getTransactionNetworkFee"",""parameters"":[],""returntype"":""Integer"",""offset"":425,""safe"":false},{""name"":""getTransactionValidUntilBlock"",""parameters"":[],""returntype"":""Integer"",""offset"":438,""safe"":false},{""name"":""getTransactionScript"",""parameters"":[],""returntype"":""ByteArray"",""offset"":451,""safe"":false},{""name"":""dynamicSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":464,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3nAUGEJxFDQEG3w4gDQEFr3qkoQEEUiNjOQEGyefz2QEHF+6DgQEFMSZLcQEHpfTigQFcAAXhBz+dHlkBXAAF4Qfgn7IxAVwEBeEEnQzXxcGjKQFcEABBwC0EnQzXxcRByInFpas5zaGsSzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEVqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JI1oQFcDARBweEEnQzXxcRByIm9oaWrOEs4Szp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSPaEBXAQBBLVEIMHBoEM5AVwEAQS1RCDBwaBHOQFcBAEEtUQgwcGgSzkBXAQBBLVEIMHBoE85AVwEAQS1RCDBwaEBXAQBBLVEIMHBoFM5AVwEAQS1RCDBwaBXOQFcBAEEtUQgwcGgWzkBXAQBBLVEIMHBoF85AVwECDAGe2zDbKHB5eBLAH2hBswyAj0DbyRv1"));

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

}
