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
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDAGe2zDbKHB5eBLAH2hBswyAj0A=
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSHDATA1 9E '?' [8 datoshi]
    /// 06 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 08 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : LDARG1 [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : PUSH2 [1 datoshi]
    /// 0E : PACK [2048 datoshi]
    /// 0F : PUSH15 [1 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : SYSCALL B30C808F 'System.Runtime.LoadScript' [32768 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QUxJktxA
    /// 00 : SYSCALL 4C4992DC 'System.Runtime.GetAddressVersion' [8 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAEHALQSdDNfFxEHIib2lqznNoaxLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JI9oQA==
    /// 00 : INITSLOT 0400 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// 0B : STLOC1 [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : STLOC2 [2 datoshi]
    /// 0E : JMP 6F [2 datoshi]
    /// 10 : LDLOC1 [2 datoshi]
    /// 11 : LDLOC2 [2 datoshi]
    /// 12 : PICKITEM [64 datoshi]
    /// 13 : STLOC3 [2 datoshi]
    /// 14 : LDLOC0 [2 datoshi]
    /// 15 : LDLOC3 [2 datoshi]
    /// 16 : PUSH2 [1 datoshi]
    /// 17 : PICKITEM [64 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : PICKITEM [64 datoshi]
    /// 1A : ADD [8 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT32 00000080 [1 datoshi]
    /// 21 : JMPGE 04 [2 datoshi]
    /// 23 : JMP 0A [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2B : JMPLE 1E [2 datoshi]
    /// 2D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 36 : AND [8 datoshi]
    /// 37 : DUP [2 datoshi]
    /// 38 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3D : JMPLE 0C [2 datoshi]
    /// 3F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 48 : SUB [8 datoshi]
    /// 49 : STLOC0 [2 datoshi]
    /// 4A : LDLOC2 [2 datoshi]
    /// 4B : DUP [2 datoshi]
    /// 4C : INC [4 datoshi]
    /// 4D : DUP [2 datoshi]
    /// 4E : PUSHINT32 00000080 [1 datoshi]
    /// 53 : JMPGE 04 [2 datoshi]
    /// 55 : JMP 0A [2 datoshi]
    /// 57 : DUP [2 datoshi]
    /// 58 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5D : JMPLE 1E [2 datoshi]
    /// 5F : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 68 : AND [8 datoshi]
    /// 69 : DUP [2 datoshi]
    /// 6A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 6F : JMPLE 0C [2 datoshi]
    /// 71 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 7A : SUB [8 datoshi]
    /// 7B : STLOC2 [2 datoshi]
    /// 7C : DROP [2 datoshi]
    /// 7D : LDLOC2 [2 datoshi]
    /// 7E : LDLOC1 [2 datoshi]
    /// 7F : SIZE [4 datoshi]
    /// 80 : LT [8 datoshi]
    /// 81 : JMPIF 8F [2 datoshi]
    /// 83 : LDLOC0 [2 datoshi]
    /// 84 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QRSI2M5A
    /// 00 : SYSCALL 1488D8CE 'System.Runtime.GasLeft' [16 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QYQnEUNA
    /// 00 : SYSCALL 84271143 'System.Runtime.GetInvocationCounter' [16 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QcX7oOBA
    /// 00 : SYSCALL C5FBA0E0 'System.Runtime.GetNetwork' [8 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEHB4QSdDNfFxEHIibWhpas4SzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSRaEA=
    /// 00 : INITSLOT 0301 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// 0B : STLOC1 [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : STLOC2 [2 datoshi]
    /// 0E : JMP 6D [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : LDLOC1 [2 datoshi]
    /// 12 : LDLOC2 [2 datoshi]
    /// 13 : PICKITEM [64 datoshi]
    /// 14 : PUSH2 [1 datoshi]
    /// 15 : PICKITEM [64 datoshi]
    /// 16 : PUSH2 [1 datoshi]
    /// 17 : PICKITEM [64 datoshi]
    /// 18 : ADD [8 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : PUSHINT32 00000080 [1 datoshi]
    /// 1F : JMPGE 04 [2 datoshi]
    /// 21 : JMP 0A [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 29 : JMPLE 1E [2 datoshi]
    /// 2B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 34 : AND [8 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : JMPLE 0C [2 datoshi]
    /// 3D : PUSHINT64 0000000001000000 [1 datoshi]
    /// 46 : SUB [8 datoshi]
    /// 47 : STLOC0 [2 datoshi]
    /// 48 : LDLOC2 [2 datoshi]
    /// 49 : DUP [2 datoshi]
    /// 4A : INC [4 datoshi]
    /// 4B : DUP [2 datoshi]
    /// 4C : PUSHINT32 00000080 [1 datoshi]
    /// 51 : JMPGE 04 [2 datoshi]
    /// 53 : JMP 0A [2 datoshi]
    /// 55 : DUP [2 datoshi]
    /// 56 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5B : JMPLE 1E [2 datoshi]
    /// 5D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 66 : AND [8 datoshi]
    /// 67 : DUP [2 datoshi]
    /// 68 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 6D : JMPLE 0C [2 datoshi]
    /// 6F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 78 : SUB [8 datoshi]
    /// 79 : STLOC2 [2 datoshi]
    /// 7A : DROP [2 datoshi]
    /// 7B : LDLOC2 [2 datoshi]
    /// 7C : LDLOC1 [2 datoshi]
    /// 7D : SIZE [4 datoshi]
    /// 7E : LT [8 datoshi]
    /// 7F : JMPIF 91 [2 datoshi]
    /// 81 : LDLOC0 [2 datoshi]
    /// 82 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEEnQzXxcGjKQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : SIZE [4 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbJ5/PZA
    /// 00 : SYSCALL B279FCF6 'System.Runtime.Platform' [8 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QWveqShA
    /// 00 : SYSCALL 6BDEA928 'System.Runtime.GetRandom' [0 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbfDiANA
    /// 00 : SYSCALL B7C38803 'System.Runtime.GetTime' [8 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaEA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBDOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBXOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH5 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBLOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH2 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBfOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH7 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBPOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH3 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBTOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH4 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBbOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH6 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBHOQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qel9OKBA
    /// 00 : SYSCALL E97D38A0 'System.Runtime.GetTrigger' [8 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHP50eWQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log")]
    public abstract void Log(string? message);

    #endregion
}
