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
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDJ7bMNsocHl4EsAfaEGzDICPQA==
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 9E [8 datoshi]
    /// 06 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 08 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.LDARG1 [2 datoshi]
    /// 0C : OpCode.LDARG0 [2 datoshi]
    /// 0D : OpCode.PUSH2 [1 datoshi]
    /// 0E : OpCode.PACK [2048 datoshi]
    /// 0F : OpCode.PUSH15 [1 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.SYSCALL B30C808F 'System.Runtime.LoadScript' [32768 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QUxJktxA
    /// 00 : OpCode.SYSCALL 4C4992DC 'System.Runtime.GetAddressVersion' [8 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAEHALQSdDNfFxEHIib2lqznNoaxLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JI9oQA==
    /// 00 : OpCode.INITSLOT 0400 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.PUSH0 [1 datoshi]
    /// 0D : OpCode.STLOC2 [2 datoshi]
    /// 0E : OpCode.JMP 6F [2 datoshi]
    /// 10 : OpCode.LDLOC1 [2 datoshi]
    /// 11 : OpCode.LDLOC2 [2 datoshi]
    /// 12 : OpCode.PICKITEM [64 datoshi]
    /// 13 : OpCode.STLOC3 [2 datoshi]
    /// 14 : OpCode.LDLOC0 [2 datoshi]
    /// 15 : OpCode.LDLOC3 [2 datoshi]
    /// 16 : OpCode.PUSH2 [1 datoshi]
    /// 17 : OpCode.PICKITEM [64 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PICKITEM [64 datoshi]
    /// 1A : OpCode.ADD [8 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 21 : OpCode.JMPGE 04 [2 datoshi]
    /// 23 : OpCode.JMP 0A [2 datoshi]
    /// 25 : OpCode.DUP [2 datoshi]
    /// 26 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2B : OpCode.JMPLE 1E [2 datoshi]
    /// 2D : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 36 : OpCode.AND [8 datoshi]
    /// 37 : OpCode.DUP [2 datoshi]
    /// 38 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3D : OpCode.JMPLE 0C [2 datoshi]
    /// 3F : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 48 : OpCode.SUB [8 datoshi]
    /// 49 : OpCode.STLOC0 [2 datoshi]
    /// 4A : OpCode.LDLOC2 [2 datoshi]
    /// 4B : OpCode.DUP [2 datoshi]
    /// 4C : OpCode.INC [4 datoshi]
    /// 4D : OpCode.DUP [2 datoshi]
    /// 4E : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 53 : OpCode.JMPGE 04 [2 datoshi]
    /// 55 : OpCode.JMP 0A [2 datoshi]
    /// 57 : OpCode.DUP [2 datoshi]
    /// 58 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5D : OpCode.JMPLE 1E [2 datoshi]
    /// 5F : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 68 : OpCode.AND [8 datoshi]
    /// 69 : OpCode.DUP [2 datoshi]
    /// 6A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 6F : OpCode.JMPLE 0C [2 datoshi]
    /// 71 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 7A : OpCode.SUB [8 datoshi]
    /// 7B : OpCode.STLOC2 [2 datoshi]
    /// 7C : OpCode.DROP [2 datoshi]
    /// 7D : OpCode.LDLOC2 [2 datoshi]
    /// 7E : OpCode.LDLOC1 [2 datoshi]
    /// 7F : OpCode.SIZE [4 datoshi]
    /// 80 : OpCode.LT [8 datoshi]
    /// 81 : OpCode.JMPIF 8F [2 datoshi]
    /// 83 : OpCode.LDLOC0 [2 datoshi]
    /// 84 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QRSI2M5A
    /// 00 : OpCode.SYSCALL 1488D8CE 'System.Runtime.GasLeft' [16 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QYQnEUNA
    /// 00 : OpCode.SYSCALL 84271143 'System.Runtime.GetInvocationCounter' [16 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QcX7oOBA
    /// 00 : OpCode.SYSCALL C5FBA0E0 'System.Runtime.GetNetwork' [8 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEHB4QSdDNfFxEHIibWhpas4SzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSRaEA=
    /// 00 : OpCode.INITSLOT 0301 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.PUSH0 [1 datoshi]
    /// 0D : OpCode.STLOC2 [2 datoshi]
    /// 0E : OpCode.JMP 6D [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC2 [2 datoshi]
    /// 13 : OpCode.PICKITEM [64 datoshi]
    /// 14 : OpCode.PUSH2 [1 datoshi]
    /// 15 : OpCode.PICKITEM [64 datoshi]
    /// 16 : OpCode.PUSH2 [1 datoshi]
    /// 17 : OpCode.PICKITEM [64 datoshi]
    /// 18 : OpCode.ADD [8 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 1F : OpCode.JMPGE 04 [2 datoshi]
    /// 21 : OpCode.JMP 0A [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 29 : OpCode.JMPLE 1E [2 datoshi]
    /// 2B : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 34 : OpCode.AND [8 datoshi]
    /// 35 : OpCode.DUP [2 datoshi]
    /// 36 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : OpCode.JMPLE 0C [2 datoshi]
    /// 3D : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 46 : OpCode.SUB [8 datoshi]
    /// 47 : OpCode.STLOC0 [2 datoshi]
    /// 48 : OpCode.LDLOC2 [2 datoshi]
    /// 49 : OpCode.DUP [2 datoshi]
    /// 4A : OpCode.INC [4 datoshi]
    /// 4B : OpCode.DUP [2 datoshi]
    /// 4C : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 51 : OpCode.JMPGE 04 [2 datoshi]
    /// 53 : OpCode.JMP 0A [2 datoshi]
    /// 55 : OpCode.DUP [2 datoshi]
    /// 56 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5B : OpCode.JMPLE 1E [2 datoshi]
    /// 5D : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 66 : OpCode.AND [8 datoshi]
    /// 67 : OpCode.DUP [2 datoshi]
    /// 68 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 6D : OpCode.JMPLE 0C [2 datoshi]
    /// 6F : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 78 : OpCode.SUB [8 datoshi]
    /// 79 : OpCode.STLOC2 [2 datoshi]
    /// 7A : OpCode.DROP [2 datoshi]
    /// 7B : OpCode.LDLOC2 [2 datoshi]
    /// 7C : OpCode.LDLOC1 [2 datoshi]
    /// 7D : OpCode.SIZE [4 datoshi]
    /// 7E : OpCode.LT [8 datoshi]
    /// 7F : OpCode.JMPIF 91 [2 datoshi]
    /// 81 : OpCode.LDLOC0 [2 datoshi]
    /// 82 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEEnQzXxcGjKQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SYSCALL 274335F1 'System.Runtime.GetNotifications' [4096 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.SIZE [4 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbJ5/PZA
    /// 00 : OpCode.SYSCALL B279FCF6 'System.Runtime.Platform' [8 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QWveqShA
    /// 00 : OpCode.SYSCALL 6BDEA928 'System.Runtime.GetRandom' [0 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbfDiANA
    /// 00 : OpCode.SYSCALL B7C38803 'System.Runtime.GetTime' [8 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaEA=
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBDOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBXOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH5 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBLOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH2 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBfOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH7 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBPOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH3 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBTOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH4 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBbOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH6 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBHOQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qel9OKBA
    /// 00 : OpCode.SYSCALL E97D38A0 'System.Runtime.GetTrigger' [8 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHP50eWQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("log")]
    public abstract void Log(string? message);

    #endregion
}
