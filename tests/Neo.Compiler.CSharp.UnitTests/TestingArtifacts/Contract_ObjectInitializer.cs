using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_ObjectInitializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ObjectInitializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""indexInitializer"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""fromEndIndexInitializer"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":234,""safe"":false},{""name"":""propertyInitializer"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":369,""safe"":false},{""name"":""fieldInitializer"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":386,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":399,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2UAVcBARjEIRHAShF4UzR8ShJ4nErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1M0VHARaDWoAAAAGqBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8SaDR+nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAMUNAl4EM55etBAVwABWBqgSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfeJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gQFcAAngQznnOQFcBARBgGMQhEcBKNFtLNGBQn3g0aVM0iHAXaDTcARAnoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1ieSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQBE1RP///xFAVwABEjU5////eBDOykBXAAETNSv///94QFcBARAQEsB4SxBR0HBoEM5AVwEBEHhQEsBwaBHOQFYBEGBA7iVD4g==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHhQEsBwaBHOQA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("fieldInitializer")]
    public abstract BigInteger? FieldInitializer(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGAYxCERwEo0W0s0YFCfeDRpUzSIcBdoNNwBECegSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfWJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// DUP [2 datoshi]
    /// CALL 5B [512 datoshi]
    /// OVER [2 datoshi]
    /// CALL 60 [512 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 69 [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CALL 88 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL DC [512 datoshi]
    /// PUSHINT16 1027 [1 datoshi]
    /// MUL [8 datoshi]
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
    /// LDSFLD0 [2 datoshi]
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
    [DisplayName("fromEndIndexInitializer")]
    public abstract BigInteger? FromEndIndexInitializer(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBGMQhEcBKEXhTNHxKEnicSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfUzRUcBFoNagAAAAaoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxJoNH6eSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH8 [1 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CALL 7C [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
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
    /// REVERSE3 [2 datoshi]
    /// CALL 54 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L A8000000 [512 datoshi]
    /// PUSH10 [1 datoshi]
    /// MUL [8 datoshi]
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
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 7E [512 datoshi]
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
    [DisplayName("indexInitializer")]
    public abstract BigInteger? IndexInitializer(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEBASwHhLEFHQcGgQzkA=
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// LDARG0 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("propertyInitializer")]
    public abstract BigInteger? PropertyInitializer(BigInteger? a);

    #endregion
}
