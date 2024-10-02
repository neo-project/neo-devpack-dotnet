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
    [DisplayName("checkWitness")]
    public abstract bool? CheckWitness(UInt160? hash);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SYSCALL
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("dynamicSum")]
    public abstract BigInteger? DynamicSum(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0006 : CONVERT
    // 0008 : CONVERT
    // 000A : STLOC0
    // 000B : LDARG1
    // 000C : LDARG0
    // 000D : PUSH2
    // 000E : PACK
    // 000F : PUSH15
    // 0010 : LDLOC0
    // 0011 : SYSCALL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getAddressVersion")]
    public abstract BigInteger? GetAddressVersion();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getAllNotifications")]
    public abstract BigInteger? GetAllNotifications();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHNULL
    // 0006 : SYSCALL
    // 000B : STLOC1
    // 000C : PUSH0
    // 000D : STLOC2
    // 000E : JMP
    // 0010 : LDLOC1
    // 0011 : LDLOC2
    // 0012 : PICKITEM
    // 0013 : STLOC3
    // 0014 : LDLOC0
    // 0015 : LDLOC3
    // 0016 : PUSH2
    // 0017 : PICKITEM
    // 0018 : PUSH2
    // 0019 : PICKITEM
    // 001A : ADD
    // 001B : DUP
    // 001C : PUSHINT32
    // 0021 : JMPGE
    // 0023 : JMP
    // 0025 : DUP
    // 0026 : PUSHINT32
    // 002B : JMPLE
    // 002D : PUSHINT64
    // 0036 : AND
    // 0037 : DUP
    // 0038 : PUSHINT32
    // 003D : JMPLE
    // 003F : PUSHINT64
    // 0048 : SUB
    // 0049 : STLOC0
    // 004A : LDLOC2
    // 004B : DUP
    // 004C : INC
    // 004D : DUP
    // 004E : PUSHINT32
    // 0053 : JMPGE
    // 0055 : JMP
    // 0057 : DUP
    // 0058 : PUSHINT32
    // 005D : JMPLE
    // 005F : PUSHINT64
    // 0068 : AND
    // 0069 : DUP
    // 006A : PUSHINT32
    // 006F : JMPLE
    // 0071 : PUSHINT64
    // 007A : SUB
    // 007B : STLOC2
    // 007C : DROP
    // 007D : LDLOC2
    // 007E : LDLOC1
    // 007F : SIZE
    // 0080 : LT
    // 0081 : JMPIF
    // 0083 : LDLOC0
    // 0084 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getGasLeft")]
    public abstract BigInteger? GetGasLeft();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getInvocationCounter")]
    public abstract BigInteger? GetInvocationCounter();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNetwork")]
    public abstract BigInteger? GetNetwork();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNotifications")]
    public abstract BigInteger? GetNotifications(UInt160? hash);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : SYSCALL
    // 000B : STLOC1
    // 000C : PUSH0
    // 000D : STLOC2
    // 000E : JMP
    // 0010 : LDLOC0
    // 0011 : LDLOC1
    // 0012 : LDLOC2
    // 0013 : PICKITEM
    // 0014 : PUSH2
    // 0015 : PICKITEM
    // 0016 : PUSH2
    // 0017 : PICKITEM
    // 0018 : ADD
    // 0019 : DUP
    // 001A : PUSHINT32
    // 001F : JMPGE
    // 0021 : JMP
    // 0023 : DUP
    // 0024 : PUSHINT32
    // 0029 : JMPLE
    // 002B : PUSHINT64
    // 0034 : AND
    // 0035 : DUP
    // 0036 : PUSHINT32
    // 003B : JMPLE
    // 003D : PUSHINT64
    // 0046 : SUB
    // 0047 : STLOC0
    // 0048 : LDLOC2
    // 0049 : DUP
    // 004A : INC
    // 004B : DUP
    // 004C : PUSHINT32
    // 0051 : JMPGE
    // 0053 : JMP
    // 0055 : DUP
    // 0056 : PUSHINT32
    // 005B : JMPLE
    // 005D : PUSHINT64
    // 0066 : AND
    // 0067 : DUP
    // 0068 : PUSHINT32
    // 006D : JMPLE
    // 006F : PUSHINT64
    // 0078 : SUB
    // 0079 : STLOC2
    // 007A : DROP
    // 007B : LDLOC2
    // 007C : LDLOC1
    // 007D : SIZE
    // 007E : LT
    // 007F : JMPIF
    // 0081 : LDLOC0
    // 0082 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getNotificationsCount")]
    public abstract BigInteger? GetNotificationsCount(UInt160? hash);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SYSCALL
    // 0009 : STLOC0
    // 000A : LDLOC0
    // 000B : SIZE
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getPlatform")]
    public abstract string? GetPlatform();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getRandom")]
    public abstract BigInteger? GetRandom();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTime")]
    public abstract BigInteger? GetTime();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransaction")]
    public abstract object? GetTransaction();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionHash")]
    public abstract UInt256? GetTransactionHash();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH0
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionNetworkFee")]
    public abstract BigInteger? GetTransactionNetworkFee();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH5
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionNonce")]
    public abstract BigInteger? GetTransactionNonce();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH2
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionScript")]
    public abstract byte[]? GetTransactionScript();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH7
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionSender")]
    public abstract UInt160? GetTransactionSender();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH3
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionSystemFee")]
    public abstract BigInteger? GetTransactionSystemFee();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH4
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionValidUntilBlock")]
    public abstract BigInteger? GetTransactionValidUntilBlock();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH6
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionVersion")]
    public abstract BigInteger? GetTransactionVersion();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSH1
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTrigger")]
    public abstract BigInteger? GetTrigger();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log")]
    public abstract void Log(string? message);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SYSCALL
    // 0009 : RET

    #endregion

}
