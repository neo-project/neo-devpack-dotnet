using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Record(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Record"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_CreateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""test_CreateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":28,""safe"":false},{""name"":""test_UpdateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":58,""safe"":false},{""name"":""test_UpdateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":138,""safe"":false},{""name"":""test_DeconstructRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""String"",""offset"":231,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0BAVcBAhALEr95eBJNNAVwaEBXAAN4EHnQeBF60EBXAQIQCxK/eEs0CnlLEVHQcGhAVwACeUp4EFHQRUBXAgIQCxK/eXgSTTTLcGjBv3kRnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LNAVxaEBXAAJ4EXnQQFcCAhALEr95eBJNNXv///9waMG/eRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0sgwBMHiL2yhLNAVxaUBXAAJ4EHnQQFcDAhALEr95eBJNNR7///9waErBRXFyRWlAv30x1g=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3l4Ek00BXBoQA==
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PICK
    /// 0B : OpCode.CALL 05
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3hLNAp5SxFR0HBoQA==
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG0
    /// 08 : OpCode.OVER
    /// 09 : OpCode.CALL 0A
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.OVER
    /// 0D : OpCode.PUSH1
    /// 0E : OpCode.ROT
    /// 0F : OpCode.SETITEM
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCEAsSv3l4Ek01Hv///3BoSsFFcXJFaUA=
    /// 00 : OpCode.INITSLOT 0302
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PICK
    /// 0B : OpCode.CALL_L 1EFFFFFF
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.UNPACK
    /// 14 : OpCode.DROP
    /// 15 : OpCode.STLOC1
    /// 16 : OpCode.STLOC2
    /// 17 : OpCode.DROP
    /// 18 : OpCode.LDLOC1
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00y3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQFcWhA
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PICK
    /// 0B : OpCode.CALL CB
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.UNPACK
    /// 10 : OpCode.PACKSTRUCT
    /// 11 : OpCode.LDARG1
    /// 12 : OpCode.PUSH1
    /// 13 : OpCode.ADD
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT32 00000080
    /// 1A : OpCode.JMPGE 04
    /// 1C : OpCode.JMP 0A
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSHINT32 FFFFFF7F
    /// 24 : OpCode.JMPLE 1E
    /// 26 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2F : OpCode.AND
    /// 30 : OpCode.DUP
    /// 31 : OpCode.PUSHINT32 FFFFFF7F
    /// 36 : OpCode.JMPLE 0C
    /// 38 : OpCode.PUSHINT64 0000000001000000
    /// 41 : OpCode.SUB
    /// 42 : OpCode.OVER
    /// 43 : OpCode.CALL 05
    /// 45 : OpCode.STLOC1
    /// 46 : OpCode.LDLOC0
    /// 47 : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek01e////3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzSyDDB4i9soSzQFcWlA
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PICK
    /// 0B : OpCode.CALL_L 7BFFFFFF
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
    /// 46 : OpCode.CALL B2
    /// 48 : OpCode.PUSHDATA1 30
    /// 4B : OpCode.LDARG0
    /// 4C : OpCode.CAT
    /// 4D : OpCode.CONVERT 28
    /// 4F : OpCode.OVER
    /// 50 : OpCode.CALL 05
    /// 52 : OpCode.STLOC1
    /// 53 : OpCode.LDLOC1
    /// 54 : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion
}
