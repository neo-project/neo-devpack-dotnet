using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Record(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Record"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_CreateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""test_CreateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":28,""safe"":false},{""name"":""test_UpdateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":58,""safe"":false},{""name"":""test_UpdateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":124,""safe"":false},{""name"":""test_DeconstructRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""String"",""offset"":200,""safe"":false},{""name"":""test_CreateRecordWithExtras"",""parameters"":[{""name"":""name"",""type"":""String""},{""name"":""tag"",""type"":""String""}],""returntype"":""Any"",""offset"":226,""safe"":false},{""name"":""test_WithRecordExtras"",""parameters"":[{""name"":""name"",""type"":""String""},{""name"":""tag"",""type"":""String""},{""name"":""yearIncrement"",""type"":""Integer""}],""returntype"":""Any"",""offset"":256,""safe"":false},{""name"":""test_RecordStructWith"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""},{""name"":""z"",""type"":""Integer""},{""name"":""deltaY"",""type"":""Integer""}],""returntype"":""Any"",""offset"":342,""safe"":false},{""name"":""test_DerivedRecordWith"",""parameters"":[{""name"":""name"",""type"":""String""},{""name"":""tier"",""type"":""Integer""},{""name"":""level"",""type"":""String""},{""name"":""bonus"",""type"":""Integer""}],""returntype"":""Any"",""offset"":427,""safe"":false},{""name"":""test_RecordEquality"",""parameters"":[{""name"":""name"",""type"":""String""},{""name"":""age"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":629,""safe"":false},{""name"":""test_RecordStructIsolation"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""},{""name"":""z"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":664,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0iA1cBAhALEr95eBJNNAVwaEBXAAN4EHnQeBF60EBXAQIQCxK/eEs0CnlLEVHQcGhAVwACeUp4EFHQRUBXAgIQCxK/eXgSTTTLcGjBv3mcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQFcWhAVwACeBF50EBXAgIQCxK/eXgSTTSJcGjBv3mcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzTDDAEweIvbKEs0BXFpQFcAAngQedBAVwMCEAsSv3l4Ek01Pf///3BoSsFFcXJFaUBXAQILAekHCxO/eEs0CnlLElHQcGhAVwACeBB50EBXAgMLAekHCxO/eEs07HlLElHQcGjBv3kMCDp1cGRhdGVki9soSxJR0GgRznqeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSxFR0HFpQFcCBBAQEBO/eXgSTTQ1eksSUdBwaMG/eXueSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQRcWlAVwADeBB50HgRetBAVwACeBF50EBXAgQQCxALFL96eXgTTTWLAAAAcGjBv3l7nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0fnoMBC1WSVCL2yhLNHl5e55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8aoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0sTUdBxaUBXAAR6eXg0D3gQedB4EXrQeBJ70EBXAAN4EHnQeBF60EBXAAJ4EXnQQFcAAngSedBAVwICEAsSv3l4Ek01kP3//3AQCxK/eXgSTTWC/f//cWhpl0BXAgMQEBATv3l4Ek018/7//3pLElHQcGjBv3kankrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s1zP7//3FoND15lyQFCSIqaTQzeRqeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACflyQECUBoEs5pEs6XQFcAAXgRzkA+RruR").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3l4Ek00BXBoQA==
    /// INITSLOT 0102 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 05 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3hLNAp5SxFR0HBoQA==
    /// INITSLOT 0102 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG0 [2 datoshi]
    /// OVER [2 datoshi]
    /// CALL 0A [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCwHpBwsTv3hLNAp5SxJR0HBoQA==
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHINT16 E907 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG0 [2 datoshi]
    /// OVER [2 datoshi]
    /// CALL 0A [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecordWithExtras")]
    public abstract object? Test_CreateRecordWithExtras(string? name, string? tag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCEAsSv3l4Ek01Pf///3BoSsFFcXJFaUA=
    /// INITSLOT 0302 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 3DFFFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIEEAsQCxS/enl4E001iwAAAHBowb95e55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LNH56DAQtVklQi9soSzR5eXueSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfGqBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LE1HQcWlA
    /// INITSLOT 0204 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 8B000000 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG3 [2 datoshi]
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
    /// OVER [2 datoshi]
    /// CALL 7E [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// PUSHDATA1 2D564950 '-VIP' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// OVER [2 datoshi]
    /// CALL 79 [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG3 [2 datoshi]
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
    /// OVER [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_DerivedRecordWith")]
    public abstract object? Test_DerivedRecordWith(string? name, BigInteger? tier, string? level, BigInteger? bonus);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek01kP3//3AQCxK/eXgSTTWC/f//cWhpl0A=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 90FDFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 82FDFFFF [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_RecordEquality")]
    public abstract bool? Test_RecordEquality(string? name, BigInteger? age);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEBAQE795eBJNNfP+//96SxJR0HBowb95Gp5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LNcz+//9xaDQ9eZckBQkiKmk0M3kankrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn5ckBAlAaBLOaRLOl0A=
    /// INITSLOT 0203 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L F3FEFFFF [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH10 [1 datoshi]
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
    /// OVER [2 datoshi]
    /// CALL_L CCFEFFFF [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 3D [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 2A [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 33 [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH10 [1 datoshi]
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
    /// EQUAL [32 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_RecordStructIsolation")]
    public abstract bool? Test_RecordStructIsolation(BigInteger? x, BigInteger? y, BigInteger? z);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIEEBAQE795eBJNNDV6SxJR0HBowb95e55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LNBFxaUA=
    /// INITSLOT 0204 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 35 [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG3 [2 datoshi]
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
    /// OVER [2 datoshi]
    /// CALL 11 [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_RecordStructWith")]
    public abstract object? Test_RecordStructWith(BigInteger? x, BigInteger? y, BigInteger? z, BigInteger? deltaY);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00y3Bowb95nErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0BXFoQA==
    /// INITSLOT 0202 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL CB [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
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
    /// OVER [2 datoshi]
    /// CALL 05 [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00iXBowb95nErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0wwwBMHiL2yhLNAVxaUA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 89 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
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
    /// OVER [2 datoshi]
    /// CALL C3 [512 datoshi]
    /// PUSHDATA1 30 '0' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// OVER [2 datoshi]
    /// CALL 05 [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDCwHpBwsTv3hLNOx5SxJR0HBowb95DAg6dXBkYXRlZIvbKEsSUdBoEc56nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0sRUdBxaUA=
    /// INITSLOT 0203 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHINT16 E907 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG0 [2 datoshi]
    /// OVER [2 datoshi]
    /// CALL EC [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHDATA1 3A75706461746564 ':updated' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// OVER [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDARG2 [2 datoshi]
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
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_WithRecordExtras")]
    public abstract object? Test_WithRecordExtras(string? name, string? tag, BigInteger? yearIncrement);

    #endregion
}
