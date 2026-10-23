using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_CryptoAndEvents(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_CryptoAndEvents"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""emitMixed"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""verifyR1Legacy"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""publicKey"",""type"":""ByteArray""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":126,""safe"":false},{""name"":""verifyR1"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""publicKey"",""type"":""ByteArray""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":155,""safe"":false},{""name"":""verifyK1"",""parameters"":[{""name"":""message"",""type"":""ByteArray""},{""name"":""publicKey"",""type"":""ByteArray""},{""name"":""signature"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":184,""safe"":false}],""events"":[{""name"":""Mixed"",""parameters"":[{""name"":""arg1"",""type"":""Integer""},{""name"":""arg2"",""type"":""String""},{""name"":""arg3"",""type"":""Boolean""},{""name"":""arg4"",""type"":""ByteArray""},{""name"":""arg5"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""verifyWithECDsa""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEb9XWrEYlohBNhCjWhKIbN4LZscg92ZXJpZnlXaXRoRUNEc2EEAAEFAADVVwACeAwEdGV4dAgMAhAg2zB4eaBKyhgyMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAnxVVFcAMBU1peGVkQZUBb2F4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwADABd62yh5StgkCtsoSsoAISgDOnjbKDcAAEBXAAMAF3rbKHlK2CQK2yhKygAhKAM6eNsoNwAAQFcAAwAWetsoeUrYJArbKErKACEoAzp42yg3AABAavEwXw==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Events

    public delegate void delMixed(BigInteger? arg1, string? arg2, bool? arg3, byte[]? arg4, BigInteger? arg5);

    [DisplayName("Mixed")]
    public event delMixed? OnMixed;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeAwEdGV4dAgMAhAg2zB4eaBKyhgyMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAnxVVFcAMBU1peGVkQZUBb2F4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 74657874 'text' [8 datoshi]
    /// PUSHT [1 datoshi]
    /// PUSHDATA1 1020 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH8 [1 datoshi]
    /// JMPLE 32 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// PUSH5 [1 datoshi]
    /// REVERSEN [16 datoshi]
    /// PUSH5 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 4D69786564 'Mixed' [8 datoshi]
    /// SYSCALL 95016F61 'System.Runtime.Notify' [32768 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("emitMixed")]
    public abstract BigInteger? EmitMixed(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADABZ62yh5StgkCtsoSsoAISgDOnjbKDcAAEA=
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 16 [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("verifyK1")]
    public abstract bool? VerifyK1(byte[]? message, byte[]? publicKey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADABd62yh5StgkCtsoSsoAISgDOnjbKDcAAEA=
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 17 [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("verifyR1")]
    public abstract bool? VerifyR1(byte[]? message, byte[]? publicKey, byte[]? signature);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADABd62yh5StgkCtsoSsoAISgDOnjbKDcAAEA=
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHINT8 17 [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("verifyR1Legacy")]
    public abstract bool? VerifyR1Legacy(byte[]? message, byte[]? publicKey, byte[]? signature);

    #endregion
}
