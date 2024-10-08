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
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SYSCALL F827EC8C
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDJ7bMNsocHl4EsAfaEGzDICPQA==
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSHDATA1 9E
    /// 06 : OpCode.CONVERT 30
    /// 08 : OpCode.CONVERT 28
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.PUSH2
    /// 0E : OpCode.PACK
    /// 0F : OpCode.PUSH15
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.SYSCALL B30C808F
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QUxJktxA
    /// 00 : OpCode.SYSCALL 4C4992DC
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAEHALQSdDNfFxEHIib2lqznNoaxLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JI9oQA==
    /// 00 : OpCode.INITSLOT 0400
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.SYSCALL 274335F1
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.PUSH0
    /// 0D : OpCode.STLOC2
    /// 0E : OpCode.JMP 6F
    /// 10 : OpCode.LDLOC1
    /// 11 : OpCode.LDLOC2
    /// 12 : OpCode.PICKITEM
    /// 13 : OpCode.STLOC3
    /// 14 : OpCode.LDLOC0
    /// 15 : OpCode.LDLOC3
    /// 16 : OpCode.PUSH2
    /// 17 : OpCode.PICKITEM
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.PICKITEM
    /// 1A : OpCode.ADD
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT32 00000080
    /// 21 : OpCode.JMPGE 04
    /// 23 : OpCode.JMP 0A
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSHINT32 FFFFFF7F
    /// 2B : OpCode.JMPLE 1E
    /// 2D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 36 : OpCode.AND
    /// 37 : OpCode.DUP
    /// 38 : OpCode.PUSHINT32 FFFFFF7F
    /// 3D : OpCode.JMPLE 0C
    /// 3F : OpCode.PUSHINT64 0000000001000000
    /// 48 : OpCode.SUB
    /// 49 : OpCode.STLOC0
    /// 4A : OpCode.LDLOC2
    /// 4B : OpCode.DUP
    /// 4C : OpCode.INC
    /// 4D : OpCode.DUP
    /// 4E : OpCode.PUSHINT32 00000080
    /// 53 : OpCode.JMPGE 04
    /// 55 : OpCode.JMP 0A
    /// 57 : OpCode.DUP
    /// 58 : OpCode.PUSHINT32 FFFFFF7F
    /// 5D : OpCode.JMPLE 1E
    /// 5F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 68 : OpCode.AND
    /// 69 : OpCode.DUP
    /// 6A : OpCode.PUSHINT32 FFFFFF7F
    /// 6F : OpCode.JMPLE 0C
    /// 71 : OpCode.PUSHINT64 0000000001000000
    /// 7A : OpCode.SUB
    /// 7B : OpCode.STLOC2
    /// 7C : OpCode.DROP
    /// 7D : OpCode.LDLOC2
    /// 7E : OpCode.LDLOC1
    /// 7F : OpCode.SIZE
    /// 80 : OpCode.LT
    /// 81 : OpCode.JMPIF 8F
    /// 83 : OpCode.LDLOC0
    /// 84 : OpCode.RET
    /// </remarks>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QRSI2M5A
    /// 00 : OpCode.SYSCALL 1488D8CE
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QYQnEUNA
    /// 00 : OpCode.SYSCALL 84271143
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QcX7oOBA
    /// 00 : OpCode.SYSCALL C5FBA0E0
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEHB4QSdDNfFxEHIibWhpas4SzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSRaEA=
    /// 00 : OpCode.INITSLOT 0301
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.SYSCALL 274335F1
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.PUSH0
    /// 0D : OpCode.STLOC2
    /// 0E : OpCode.JMP 6D
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDLOC2
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.PUSH2
    /// 15 : OpCode.PICKITEM
    /// 16 : OpCode.PUSH2
    /// 17 : OpCode.PICKITEM
    /// 18 : OpCode.ADD
    /// 19 : OpCode.DUP
    /// 1A : OpCode.PUSHINT32 00000080
    /// 1F : OpCode.JMPGE 04
    /// 21 : OpCode.JMP 0A
    /// 23 : OpCode.DUP
    /// 24 : OpCode.PUSHINT32 FFFFFF7F
    /// 29 : OpCode.JMPLE 1E
    /// 2B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 34 : OpCode.AND
    /// 35 : OpCode.DUP
    /// 36 : OpCode.PUSHINT32 FFFFFF7F
    /// 3B : OpCode.JMPLE 0C
    /// 3D : OpCode.PUSHINT64 0000000001000000
    /// 46 : OpCode.SUB
    /// 47 : OpCode.STLOC0
    /// 48 : OpCode.LDLOC2
    /// 49 : OpCode.DUP
    /// 4A : OpCode.INC
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSHINT32 00000080
    /// 51 : OpCode.JMPGE 04
    /// 53 : OpCode.JMP 0A
    /// 55 : OpCode.DUP
    /// 56 : OpCode.PUSHINT32 FFFFFF7F
    /// 5B : OpCode.JMPLE 1E
    /// 5D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 66 : OpCode.AND
    /// 67 : OpCode.DUP
    /// 68 : OpCode.PUSHINT32 FFFFFF7F
    /// 6D : OpCode.JMPLE 0C
    /// 6F : OpCode.PUSHINT64 0000000001000000
    /// 78 : OpCode.SUB
    /// 79 : OpCode.STLOC2
    /// 7A : OpCode.DROP
    /// 7B : OpCode.LDLOC2
    /// 7C : OpCode.LDLOC1
    /// 7D : OpCode.SIZE
    /// 7E : OpCode.LT
    /// 7F : OpCode.JMPIF 91
    /// 81 : OpCode.LDLOC0
    /// 82 : OpCode.RET
    /// </remarks>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEEnQzXxcGjKQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SYSCALL 274335F1
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.SIZE
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbJ5/PZA
    /// 00 : OpCode.SYSCALL B279FCF6
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QWveqShA
    /// 00 : OpCode.SYSCALL 6BDEA928
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbfDiANA
    /// 00 : OpCode.SYSCALL B7C38803
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaEA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBDOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBXOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH5
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBLOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH2
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBfOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH7
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBPOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH3
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBTOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH4
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBbOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH6
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBHOQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.SYSCALL 2D510830
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qel9OKBA
    /// 00 : OpCode.SYSCALL E97D38A0
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHP50eWQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SYSCALL CFE74796
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("log")]
    public abstract void Log(string? message);

    #endregion
}
