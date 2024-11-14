using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Record(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Record"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_CreateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""test_CreateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":32,""safe"":false},{""name"":""test_UpdateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":66,""safe"":false},{""name"":""test_UpdateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":146,""safe"":false},{""name"":""test_DeconstructRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""String"",""offset"":239,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0JAVcBAhALEr95eBJNNAVwaEBXAAN4ERDQeBB50HgRetBAVwECEAsSv3hLNAp5SxFR0HBoQFcAAngRENB5SngQUdBFQFcCAhALEr95eBJNNMNwaMG/eRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0BXFoQFcAAngRedBAVwICEAsSv3l4Ek01c////3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzSyDAEweIvbKEs0BXFpQFcAAngQedBAVwMCEAsSv3l4Ek01Fv///3BoSsFFcXJFaUDAuPzr"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3l4Ek00BXBoQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACKSTRUCT [2048 datoshi]
    /// 07 : LDARG1 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PICK [2 datoshi]
    /// 0B : CALL 05 [512 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3hLNAp5SxFR0HBoQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACKSTRUCT [2048 datoshi]
    /// 07 : LDARG0 [2 datoshi]
    /// 08 : OVER [2 datoshi]
    /// 09 : CALL 0A [512 datoshi]
    /// 0B : LDARG1 [2 datoshi]
    /// 0C : OVER [2 datoshi]
    /// 0D : PUSH1 [1 datoshi]
    /// 0E : ROT [2 datoshi]
    /// 0F : SETITEM [8192 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCEAsSv3l4Ek01Fv///3BoSsFFcXJFaUA=
    /// 00 : INITSLOT 0302 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACKSTRUCT [2048 datoshi]
    /// 07 : LDARG1 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PICK [2 datoshi]
    /// 0B : CALL_L 16FFFFFF [512 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : UNPACK [2048 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : STLOC1 [2 datoshi]
    /// 16 : STLOC2 [2 datoshi]
    /// 17 : DROP [2 datoshi]
    /// 18 : LDLOC1 [2 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00w3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQFcWhA
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACKSTRUCT [2048 datoshi]
    /// 07 : LDARG1 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PICK [2 datoshi]
    /// 0B : CALL C3 [512 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : UNPACK [2048 datoshi]
    /// 10 : PACKSTRUCT [2048 datoshi]
    /// 11 : LDARG1 [2 datoshi]
    /// 12 : PUSH1 [1 datoshi]
    /// 13 : ADD [8 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT32 00000080 [1 datoshi]
    /// 1A : JMPGE 04 [2 datoshi]
    /// 1C : JMP 0A [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 24 : JMPLE 1E [2 datoshi]
    /// 26 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2F : AND [8 datoshi]
    /// 30 : DUP [2 datoshi]
    /// 31 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 36 : JMPLE 0C [2 datoshi]
    /// 38 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 41 : SUB [8 datoshi]
    /// 42 : OVER [2 datoshi]
    /// 43 : CALL 05 [512 datoshi]
    /// 45 : STLOC1 [2 datoshi]
    /// 46 : LDLOC0 [2 datoshi]
    /// 47 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek01c////3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzSyDAEweIvbKEs0BXFpQA==
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACKSTRUCT [2048 datoshi]
    /// 07 : LDARG1 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PICK [2 datoshi]
    /// 0B : CALL_L 73FFFFFF [512 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : UNPACK [2048 datoshi]
    /// 13 : PACKSTRUCT [2048 datoshi]
    /// 14 : LDARG1 [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : ADD [8 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : PUSHINT32 00000080 [1 datoshi]
    /// 1D : JMPGE 04 [2 datoshi]
    /// 1F : JMP 0A [2 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 27 : JMPLE 1E [2 datoshi]
    /// 29 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 32 : AND [8 datoshi]
    /// 33 : DUP [2 datoshi]
    /// 34 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 39 : JMPLE 0C [2 datoshi]
    /// 3B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 44 : SUB [8 datoshi]
    /// 45 : OVER [2 datoshi]
    /// 46 : CALL B2 [512 datoshi]
    /// 48 : PUSHDATA1 30 '0' [8 datoshi]
    /// 4B : LDARG0 [2 datoshi]
    /// 4C : CAT [2048 datoshi]
    /// 4D : CONVERT 28 'ByteString' [8192 datoshi]
    /// 4F : OVER [2 datoshi]
    /// 50 : CALL 05 [512 datoshi]
    /// 52 : STLOC1 [2 datoshi]
    /// 53 : LDLOC1 [2 datoshi]
    /// 54 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion
}
