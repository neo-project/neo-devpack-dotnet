using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Record(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Record"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_CreateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""test_CreateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":28,""safe"":false},{""name"":""test_UpdateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":58,""safe"":false},{""name"":""test_UpdateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":92,""safe"":false},{""name"":""test_DeconstructRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""String"",""offset"":136,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKJXAQIQCxK/eXgSTTQFcGhAVwADeBB50HgRetBAVwECEAsSv3hLNAp5SxFR0HBoQFcAAnlKeBBR0EVAVwICEAsSv3l4Ek00y3Bowb95EZ5LNAVxaEBXAAJ4EXnQQFcCAhALEr95eBJNNKlwaMG/eRGeSzTjDAEweIvbKEs0BXFpQFcAAngQedBAVwMCEAsSv3l4Ek01ff///3BoSsFFcXJFaUBhKjA6"));

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
    /// Script: VwMCEAsSv3l4Ek01ff///3BoSsFFcXJFaUA=
    /// 00 : OpCode.INITSLOT 0302
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PICK
    /// 0B : OpCode.CALL_L 7DFFFFFF
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
    /// Script: VwICEAsSv3l4Ek00y3Bowb95EZ5LNAVxaEA=
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
    /// 14 : OpCode.OVER
    /// 15 : OpCode.CALL 05
    /// 17 : OpCode.STLOC1
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00qXBowb95EZ5LNOMMMHiL2yhLNAVxaUA=
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PICK
    /// 0B : OpCode.CALL A9
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.UNPACK
    /// 10 : OpCode.PACKSTRUCT
    /// 11 : OpCode.LDARG1
    /// 12 : OpCode.PUSH1
    /// 13 : OpCode.ADD
    /// 14 : OpCode.OVER
    /// 15 : OpCode.CALL E3
    /// 17 : OpCode.PUSHDATA1 30
    /// 1A : OpCode.LDARG0
    /// 1B : OpCode.CAT
    /// 1C : OpCode.CONVERT 28
    /// 1E : OpCode.OVER
    /// 1F : OpCode.CALL 05
    /// 21 : OpCode.STLOC1
    /// 22 : OpCode.LDLOC1
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion
}
