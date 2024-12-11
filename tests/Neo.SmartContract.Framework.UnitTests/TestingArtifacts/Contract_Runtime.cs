using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Runtime(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Runtime"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getInvocationCounter"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTime"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""getRandom"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":false},{""name"":""getGasLeft"",""parameters"":[],""returntype"":""Integer"",""offset"":18,""safe"":false},{""name"":""getPlatform"",""parameters"":[],""returntype"":""String"",""offset"":24,""safe"":false},{""name"":""getNetwork"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""getAddressVersion"",""parameters"":[],""returntype"":""Integer"",""offset"":36,""safe"":false},{""name"":""getTrigger"",""parameters"":[],""returntype"":""Integer"",""offset"":42,""safe"":false},{""name"":""log"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":48,""safe"":false},{""name"":""checkWitness"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":58,""safe"":false},{""name"":""getNotificationsCount"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":68,""safe"":false},{""name"":""getAllNotifications"",""parameters"":[],""returntype"":""Integer"",""offset"":81,""safe"":false},{""name"":""getNotifications"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":214,""safe"":false},{""name"":""getTransactionHash"",""parameters"":[],""returntype"":""Hash256"",""offset"":345,""safe"":false},{""name"":""getTransactionVersion"",""parameters"":[],""returntype"":""Integer"",""offset"":358,""safe"":false},{""name"":""getTransactionNonce"",""parameters"":[],""returntype"":""Integer"",""offset"":371,""safe"":false},{""name"":""getTransactionSender"",""parameters"":[],""returntype"":""Hash160"",""offset"":384,""safe"":false},{""name"":""getTransaction"",""parameters"":[],""returntype"":""Any"",""offset"":397,""safe"":false},{""name"":""getTransactionSystemFee"",""parameters"":[],""returntype"":""Integer"",""offset"":408,""safe"":false},{""name"":""getTransactionNetworkFee"",""parameters"":[],""returntype"":""Integer"",""offset"":421,""safe"":false},{""name"":""getTransactionValidUntilBlock"",""parameters"":[],""returntype"":""Integer"",""offset"":434,""safe"":false},{""name"":""getTransactionScript"",""parameters"":[],""returntype"":""ByteArray"",""offset"":447,""safe"":false},{""name"":""dynamicSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":460,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3jAUGEJxFDQEG3w4gDQEFr3qkoQEEUiNjOQEGyefz2QEHF+6DgQEFMSZLcQEHpfTigQFcAAXhBz+dHlkBXAAF4Qfgn7IxAVwEBeEEnQzXxcGjKQFcEABBwC0EnQzXxcRByIm9pas5zaGsSzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSPaEBXAwEQcHhBJ0M18XEQciJtaGlqzhLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JJFoQFcBAEEtUQgwcGgQzkBXAQBBLVEIMHBoEc5AVwEAQS1RCDBwaBLOQFcBAEEtUQgwcGgTzkBXAQBBLVEIMHBoQFcBAEEtUQgwcGgUzkBXAQBBLVEIMHBoFc5AVwEAQS1RCDBwaBbOQFcBAEEtUQgwcGgXzkBXAQIMAZ7bMNsocHl4EsAfaEGzDICPQBcc8cM="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEH4J+yMQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDAGe2zDbKHB5eBLAH2hBswyAj0A=
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHDATA1 9E '?' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH15 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL B30C808F 'System.Runtime.LoadScript' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QUxJktxA
    /// SYSCALL 4C4992DC 'System.Runtime.GetAddressVersion' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAEHALQSdDNfFxEHIib2lqznNoaxLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JI9oQA==
    /// INITSLOT 0400 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP 6F [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC2 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// LT [8 datoshi]
    /// JMPIF 8F [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QRSI2M5A
    /// SYSCALL 1488D8CE 'System.Runtime.GasLeft' [16 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QYQnEUNA
    /// SYSCALL 84271143 'System.Runtime.GetInvocationCounter' [16 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QcX7oOBA
    /// SYSCALL C5FBA0E0 'System.Runtime.GetNetwork' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEHB4QSdDNfFxEHIibWhpas4SzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSRaEA=
    /// INITSLOT 0301 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP 6D [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC2 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// LT [8 datoshi]
    /// JMPIF 91 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEEnQzXxcGjKQA==
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbJ5/PZA
    /// SYSCALL B279FCF6 'System.Runtime.Platform' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QWveqShA
    /// SYSCALL 6BDEA928 'System.Runtime.GetRandom' [0 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbfDiANA
    /// SYSCALL B7C38803 'System.Runtime.GetTime' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaEA=
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBDOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBXOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBLOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBfOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBPOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBTOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBbOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBHOQA==
    /// INITSLOT 0100 [64 datoshi]
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qel9OKBA
    /// SYSCALL E97D38A0 'System.Runtime.GetTrigger' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHP50eWQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log")]
    public abstract void Log(string? message);

    #endregion
}
