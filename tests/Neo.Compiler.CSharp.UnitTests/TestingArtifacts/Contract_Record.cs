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
    /// 00 : OpCode.INITSLOT 0102 	-> 64 datoshi
    /// 03 : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 06 : OpCode.APPEND 	-> 8192 datoshi
    /// 07 : OpCode.DUP 	-> 2 datoshi
    /// 08 : OpCode.PUSH0 	-> 1 datoshi
    /// 09 : OpCode.APPEND 	-> 8192 datoshi
    /// 0A : OpCode.LDARG1 	-> 2 datoshi
    /// 0B : OpCode.LDARG0 	-> 2 datoshi
    /// 0C : OpCode.PUSH2 	-> 1 datoshi
    /// 0D : OpCode.PICK 	-> 2 datoshi
    /// 0E : OpCode.CALL 05 	-> 512 datoshi
    /// 10 : OpCode.STLOC0 	-> 2 datoshi
    /// 11 : OpCode.LDLOC0 	-> 2 datoshi
    /// 12 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECxUoLz0oQz3hLNAp5SxFR0HBoQA==
    /// 00 : OpCode.INITSLOT 0102 	-> 64 datoshi
    /// 03 : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 06 : OpCode.APPEND 	-> 8192 datoshi
    /// 07 : OpCode.DUP 	-> 2 datoshi
    /// 08 : OpCode.PUSH0 	-> 1 datoshi
    /// 09 : OpCode.APPEND 	-> 8192 datoshi
    /// 0A : OpCode.LDARG0 	-> 2 datoshi
    /// 0B : OpCode.OVER 	-> 2 datoshi
    /// 0C : OpCode.CALL 0A 	-> 512 datoshi
    /// 0E : OpCode.LDARG1 	-> 2 datoshi
    /// 0F : OpCode.OVER 	-> 2 datoshi
    /// 10 : OpCode.PUSH1 	-> 1 datoshi
    /// 11 : OpCode.ROT 	-> 2 datoshi
    /// 12 : OpCode.SETITEM 	-> 8192 datoshi
    /// 13 : OpCode.STLOC0 	-> 2 datoshi
    /// 14 : OpCode.LDLOC0 	-> 2 datoshi
    /// 15 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCxUoLz0oQz3l4Ek01Ev///3BoSsFFcXJFaUA=
    /// 00 : OpCode.INITSLOT 0302 	-> 64 datoshi
    /// 03 : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 06 : OpCode.APPEND 	-> 8192 datoshi
    /// 07 : OpCode.DUP 	-> 2 datoshi
    /// 08 : OpCode.PUSH0 	-> 1 datoshi
    /// 09 : OpCode.APPEND 	-> 8192 datoshi
    /// 0A : OpCode.LDARG1 	-> 2 datoshi
    /// 0B : OpCode.LDARG0 	-> 2 datoshi
    /// 0C : OpCode.PUSH2 	-> 1 datoshi
    /// 0D : OpCode.PICK 	-> 2 datoshi
    /// 0E : OpCode.CALL_L 12FFFFFF 	-> 512 datoshi
    /// 13 : OpCode.STLOC0 	-> 2 datoshi
    /// 14 : OpCode.LDLOC0 	-> 2 datoshi
    /// 15 : OpCode.DUP 	-> 2 datoshi
    /// 16 : OpCode.UNPACK 	-> 2048 datoshi
    /// 17 : OpCode.DROP 	-> 2 datoshi
    /// 18 : OpCode.STLOC1 	-> 2 datoshi
    /// 19 : OpCode.STLOC2 	-> 2 datoshi
    /// 1A : OpCode.DROP 	-> 2 datoshi
    /// 1B : OpCode.LDLOC1 	-> 2 datoshi
    /// 1C : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICxUoLz0oQz3l4Ek00xXBowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQFcWhA
    /// 00 : OpCode.INITSLOT 0202 	-> 64 datoshi
    /// 03 : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 06 : OpCode.APPEND 	-> 8192 datoshi
    /// 07 : OpCode.DUP 	-> 2 datoshi
    /// 08 : OpCode.PUSH0 	-> 1 datoshi
    /// 09 : OpCode.APPEND 	-> 8192 datoshi
    /// 0A : OpCode.LDARG1 	-> 2 datoshi
    /// 0B : OpCode.LDARG0 	-> 2 datoshi
    /// 0C : OpCode.PUSH2 	-> 1 datoshi
    /// 0D : OpCode.PICK 	-> 2 datoshi
    /// 0E : OpCode.CALL C5 	-> 512 datoshi
    /// 10 : OpCode.STLOC0 	-> 2 datoshi
    /// 11 : OpCode.LDLOC0 	-> 2 datoshi
    /// 12 : OpCode.UNPACK 	-> 2048 datoshi
    /// 13 : OpCode.PACKSTRUCT 	-> 2048 datoshi
    /// 14 : OpCode.LDARG1 	-> 2 datoshi
    /// 15 : OpCode.PUSH1 	-> 1 datoshi
    /// 16 : OpCode.ADD 	-> 8 datoshi
    /// 17 : OpCode.DUP 	-> 2 datoshi
    /// 18 : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 1D : OpCode.JMPGE 04 	-> 2 datoshi
    /// 1F : OpCode.JMP 0A 	-> 2 datoshi
    /// 21 : OpCode.DUP 	-> 2 datoshi
    /// 22 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 27 : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 29 : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 32 : OpCode.AND 	-> 8 datoshi
    /// 33 : OpCode.DUP 	-> 2 datoshi
    /// 34 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 39 : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 3B : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 44 : OpCode.SUB 	-> 8 datoshi
    /// 45 : OpCode.OVER 	-> 2 datoshi
    /// 46 : OpCode.CALL 05 	-> 512 datoshi
    /// 48 : OpCode.STLOC1 	-> 2 datoshi
    /// 49 : OpCode.LDLOC0 	-> 2 datoshi
    /// 4A : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICxUoLz0oQz3l4Ek01cv///3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzSvDDB4i9soSzQFcWlA
    /// 00 : OpCode.INITSLOT 0202 	-> 64 datoshi
    /// 03 : OpCode.NEWSTRUCT0 	-> 16 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 06 : OpCode.APPEND 	-> 8192 datoshi
    /// 07 : OpCode.DUP 	-> 2 datoshi
    /// 08 : OpCode.PUSH0 	-> 1 datoshi
    /// 09 : OpCode.APPEND 	-> 8192 datoshi
    /// 0A : OpCode.LDARG1 	-> 2 datoshi
    /// 0B : OpCode.LDARG0 	-> 2 datoshi
    /// 0C : OpCode.PUSH2 	-> 1 datoshi
    /// 0D : OpCode.PICK 	-> 2 datoshi
    /// 0E : OpCode.CALL_L 72FFFFFF 	-> 512 datoshi
    /// 13 : OpCode.STLOC0 	-> 2 datoshi
    /// 14 : OpCode.LDLOC0 	-> 2 datoshi
    /// 15 : OpCode.UNPACK 	-> 2048 datoshi
    /// 16 : OpCode.PACKSTRUCT 	-> 2048 datoshi
    /// 17 : OpCode.LDARG1 	-> 2 datoshi
    /// 18 : OpCode.PUSH1 	-> 1 datoshi
    /// 19 : OpCode.ADD 	-> 8 datoshi
    /// 1A : OpCode.DUP 	-> 2 datoshi
    /// 1B : OpCode.PUSHINT32 00000080 	-> 1 datoshi
    /// 20 : OpCode.JMPGE 04 	-> 2 datoshi
    /// 22 : OpCode.JMP 0A 	-> 2 datoshi
    /// 24 : OpCode.DUP 	-> 2 datoshi
    /// 25 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 2A : OpCode.JMPLE 1E 	-> 2 datoshi
    /// 2C : OpCode.PUSHINT64 FFFFFFFF00000000 	-> 1 datoshi
    /// 35 : OpCode.AND 	-> 8 datoshi
    /// 36 : OpCode.DUP 	-> 2 datoshi
    /// 37 : OpCode.PUSHINT32 FFFFFF7F 	-> 1 datoshi
    /// 3C : OpCode.JMPLE 0C 	-> 2 datoshi
    /// 3E : OpCode.PUSHINT64 0000000001000000 	-> 1 datoshi
    /// 47 : OpCode.SUB 	-> 8 datoshi
    /// 48 : OpCode.OVER 	-> 2 datoshi
    /// 49 : OpCode.CALL AF 	-> 512 datoshi
    /// 4B : OpCode.PUSHDATA1 30 	-> 8 datoshi
    /// 4E : OpCode.LDARG0 	-> 2 datoshi
    /// 4F : OpCode.CAT 	-> 2048 datoshi
    /// 50 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 52 : OpCode.OVER 	-> 2 datoshi
    /// 53 : OpCode.CALL 05 	-> 512 datoshi
    /// 55 : OpCode.STLOC1 	-> 2 datoshi
    /// 56 : OpCode.LDLOC1 	-> 2 datoshi
    /// 57 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion
}
