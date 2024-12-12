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
    /// Script: VwMCEAsSv3l4Ek01Hv///3BoSsFFcXJFaUA=
    /// INITSLOT 0302 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 1EFFFFFF [512 datoshi]
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
    /// Script: VwICEAsSv3l4Ek00y3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzQFcWhA
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
    /// PUSH1 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
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
    /// Script: VwICEAsSv3l4Ek01e////3Bowb95EZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSzSyDAEweIvbKEs0BXFpQA==
    /// INITSLOT 0202 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 7BFFFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// OVER [2 datoshi]
    /// CALL B2 [512 datoshi]
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

    #endregion
}
