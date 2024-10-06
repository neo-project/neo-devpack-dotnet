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
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SYSCALL F827EC8C
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECDJ7bMNsocHl4EsAfaEGzDICPQA==
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.PUSHDATA1 9E
    /// 0006 : OpCode.CONVERT 30
    /// 0008 : OpCode.CONVERT 28
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.LDARG0
    /// 000D : OpCode.PUSH2
    /// 000E : OpCode.PACK
    /// 000F : OpCode.PUSH15
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.SYSCALL B30C808F
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QUxJktxA
    /// 0000 : OpCode.SYSCALL 4C4992DC
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAEHALQSdDNfFxEHIib2lqznNoaxLOEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqacq1JI9oQA==
    /// 0000 : OpCode.INITSLOT 0400
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.SYSCALL 274335F1
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.PUSH0
    /// 000D : OpCode.STLOC2
    /// 000E : OpCode.JMP 6F
    /// 0010 : OpCode.LDLOC1
    /// 0011 : OpCode.LDLOC2
    /// 0012 : OpCode.PICKITEM
    /// 0013 : OpCode.STLOC3
    /// 0014 : OpCode.LDLOC0
    /// 0015 : OpCode.LDLOC3
    /// 0016 : OpCode.PUSH2
    /// 0017 : OpCode.PICKITEM
    /// 0018 : OpCode.PUSH2
    /// 0019 : OpCode.PICKITEM
    /// 001A : OpCode.ADD
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT32 00000080
    /// 0021 : OpCode.JMPGE 04
    /// 0023 : OpCode.JMP 0A
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSHINT32 FFFFFF7F
    /// 002B : OpCode.JMPLE 1E
    /// 002D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0036 : OpCode.AND
    /// 0037 : OpCode.DUP
    /// 0038 : OpCode.PUSHINT32 FFFFFF7F
    /// 003D : OpCode.JMPLE 0C
    /// 003F : OpCode.PUSHINT64 0000000001000000
    /// 0048 : OpCode.SUB
    /// 0049 : OpCode.STLOC0
    /// 004A : OpCode.LDLOC2
    /// 004B : OpCode.DUP
    /// 004C : OpCode.INC
    /// 004D : OpCode.DUP
    /// 004E : OpCode.PUSHINT32 00000080
    /// 0053 : OpCode.JMPGE 04
    /// 0055 : OpCode.JMP 0A
    /// 0057 : OpCode.DUP
    /// 0058 : OpCode.PUSHINT32 FFFFFF7F
    /// 005D : OpCode.JMPLE 1E
    /// 005F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0068 : OpCode.AND
    /// 0069 : OpCode.DUP
    /// 006A : OpCode.PUSHINT32 FFFFFF7F
    /// 006F : OpCode.JMPLE 0C
    /// 0071 : OpCode.PUSHINT64 0000000001000000
    /// 007A : OpCode.SUB
    /// 007B : OpCode.STLOC2
    /// 007C : OpCode.DROP
    /// 007D : OpCode.LDLOC2
    /// 007E : OpCode.LDLOC1
    /// 007F : OpCode.SIZE
    /// 0080 : OpCode.LT
    /// 0081 : OpCode.JMPIF 8F
    /// 0083 : OpCode.LDLOC0
    /// 0084 : OpCode.RET
    /// </remarks>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QRSI2M5A
    /// 0000 : OpCode.SYSCALL 1488D8CE
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QYQnEUNA
    /// 0000 : OpCode.SYSCALL 84271143
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QcX7oOBA
    /// 0000 : OpCode.SYSCALL C5FBA0E0
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBEHB4QSdDNfFxEHIibWhpas4SzhLOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3JFamnKtSSRaEA=
    /// 0000 : OpCode.INITSLOT 0301
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.SYSCALL 274335F1
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.PUSH0
    /// 000D : OpCode.STLOC2
    /// 000E : OpCode.JMP 6D
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.LDLOC2
    /// 0013 : OpCode.PICKITEM
    /// 0014 : OpCode.PUSH2
    /// 0015 : OpCode.PICKITEM
    /// 0016 : OpCode.PUSH2
    /// 0017 : OpCode.PICKITEM
    /// 0018 : OpCode.ADD
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.PUSHINT32 00000080
    /// 001F : OpCode.JMPGE 04
    /// 0021 : OpCode.JMP 0A
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.PUSHINT32 FFFFFF7F
    /// 0029 : OpCode.JMPLE 1E
    /// 002B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0034 : OpCode.AND
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.PUSHINT32 FFFFFF7F
    /// 003B : OpCode.JMPLE 0C
    /// 003D : OpCode.PUSHINT64 0000000001000000
    /// 0046 : OpCode.SUB
    /// 0047 : OpCode.STLOC0
    /// 0048 : OpCode.LDLOC2
    /// 0049 : OpCode.DUP
    /// 004A : OpCode.INC
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSHINT32 00000080
    /// 0051 : OpCode.JMPGE 04
    /// 0053 : OpCode.JMP 0A
    /// 0055 : OpCode.DUP
    /// 0056 : OpCode.PUSHINT32 FFFFFF7F
    /// 005B : OpCode.JMPLE 1E
    /// 005D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0066 : OpCode.AND
    /// 0067 : OpCode.DUP
    /// 0068 : OpCode.PUSHINT32 FFFFFF7F
    /// 006D : OpCode.JMPLE 0C
    /// 006F : OpCode.PUSHINT64 0000000001000000
    /// 0078 : OpCode.SUB
    /// 0079 : OpCode.STLOC2
    /// 007A : OpCode.DROP
    /// 007B : OpCode.LDLOC2
    /// 007C : OpCode.LDLOC1
    /// 007D : OpCode.SIZE
    /// 007E : OpCode.LT
    /// 007F : OpCode.JMPIF 91
    /// 0081 : OpCode.LDLOC0
    /// 0082 : OpCode.RET
    /// </remarks>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEEnQzXxcGjKQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SYSCALL 274335F1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.SIZE
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbJ5/PZA
    /// 0000 : OpCode.SYSCALL B279FCF6
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QWveqShA
    /// 0000 : OpCode.SYSCALL 6BDEA928
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QbfDiANA
    /// 0000 : OpCode.SYSCALL B7C38803
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaEA=
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBDOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBXOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH5
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBLOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH2
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBfOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH7
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBPOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH3
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBTOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH4
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBbOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH6
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAQS1RCDBwaBHOQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.SYSCALL 2D510830
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qel9OKBA
    /// 0000 : OpCode.SYSCALL E97D38A0
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEHP50eWQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SYSCALL CFE74796
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("log")]
    public abstract void Log(string? message);

    #endregion

}
