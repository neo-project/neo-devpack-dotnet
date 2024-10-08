using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Record(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Record"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_CreateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""test_CreateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":31,""safe"":false},{""name"":""test_UpdateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":64,""safe"":false},{""name"":""test_UpdateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":147,""safe"":false},{""name"":""test_DeconstructRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""String"",""offset"":243,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0QAVcBAsVKC89KEM95eBJNNAVwaEBXAAN4EHnQeBF60EBXAQLFSgvPShDPeEs0CnlLEVHQcGhAVwACeUp4EFHQRUBXAgLFSgvPShDPeXgSTTTFcGjBv3kRnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LNAVxaEBXAAJ4EXnQQFcCAsVKC89KEM95eBJNNXL///9waMG/eRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0rwwBMHiL2yhLNAVxaUBXAAJ4EHnQQFcDAsVKC89KEM95eBJNNRL///9waErBRXFyRWlAmNdg3g=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECxUoLz0oQz3l4Ek00BXBoQA==
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.PUSH2
    /// 0D : OpCode.PICK
    /// 0E : OpCode.CALL 05
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECxUoLz0oQz3hLNAp5SxFR0HBoQA==
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.OVER
    /// 0C : OpCode.CALL 0A
    /// 0E : OpCode.LDARG1
    /// 0F : OpCode.OVER
    /// 10 : OpCode.PUSH1
    /// 11 : OpCode.ROT
    /// 12 : OpCode.SETITEM
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.LDLOC0
    /// 15 : OpCode.RET
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCxUoLz0oQz3l4Ek01Ev///3BoSsFFcXJFaUA=
    /// 00 : OpCode.INITSLOT 0302
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.PUSH2
    /// 0D : OpCode.PICK
    /// 0E : OpCode.CALL_L 12FFFFFF
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.LDLOC0
    /// 15 : OpCode.DUP
    /// 16 : OpCode.UNPACK
    /// 17 : OpCode.DROP
    /// 18 : OpCode.STLOC1
    /// 19 : OpCode.STLOC2
    /// 1A : OpCode.DROP
    /// 1B : OpCode.LDLOC1
    /// 1C : OpCode.RET
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICxUoLz0oQz3l4Ek00xXBowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQFcWhA
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.PUSH2
    /// 0D : OpCode.PICK
    /// 0E : OpCode.CALL C5
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.UNPACK
    /// 13 : OpCode.PACKSTRUCT
    /// 14 : OpCode.LDARG1
    /// 15 : OpCode.PUSH1
    /// 16 : OpCode.ADD
    /// 17 : OpCode.DUP
    /// 18 : OpCode.PUSHINT32 00000080
    /// 1D : OpCode.JMPGE 04
    /// 1F : OpCode.JMP 0A
    /// 21 : OpCode.DUP
    /// 22 : OpCode.PUSHINT32 FFFFFF7F
    /// 27 : OpCode.JMPLE 1E
    /// 29 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 32 : OpCode.AND
    /// 33 : OpCode.DUP
    /// 34 : OpCode.PUSHINT32 FFFFFF7F
    /// 39 : OpCode.JMPLE 0C
    /// 3B : OpCode.PUSHINT64 0000000001000000
    /// 44 : OpCode.SUB
    /// 45 : OpCode.OVER
    /// 46 : OpCode.CALL 05
    /// 48 : OpCode.STLOC1
    /// 49 : OpCode.LDLOC0
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICxUoLz0oQz3l4Ek01cv///3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzSvDDB4i9soSzQFcWlA
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.PUSH2
    /// 0D : OpCode.PICK
    /// 0E : OpCode.CALL_L 72FFFFFF
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.LDLOC0
    /// 15 : OpCode.UNPACK
    /// 16 : OpCode.PACKSTRUCT
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.PUSH1
    /// 19 : OpCode.ADD
    /// 1A : OpCode.DUP
    /// 1B : OpCode.PUSHINT32 00000080
    /// 20 : OpCode.JMPGE 04
    /// 22 : OpCode.JMP 0A
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 1E
    /// 2C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 35 : OpCode.AND
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSHINT32 FFFFFF7F
    /// 3C : OpCode.JMPLE 0C
    /// 3E : OpCode.PUSHINT64 0000000001000000
    /// 47 : OpCode.SUB
    /// 48 : OpCode.OVER
    /// 49 : OpCode.CALL AF
    /// 4B : OpCode.PUSHDATA1 30
    /// 4E : OpCode.LDARG0
    /// 4F : OpCode.CAT
    /// 50 : OpCode.CONVERT 28
    /// 52 : OpCode.OVER
    /// 53 : OpCode.CALL 05
    /// 55 : OpCode.STLOC1
    /// 56 : OpCode.LDLOC1
    /// 57 : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion
}
