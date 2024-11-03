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
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 07 : OpCode.LDARG1 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PICK [2 datoshi]
    /// 0B : OpCode.CALL 05 [512 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEAsSv3hLNAp5SxFR0HBoQA==
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 07 : OpCode.LDARG0 [2 datoshi]
    /// 08 : OpCode.OVER [2 datoshi]
    /// 09 : OpCode.CALL 0A [512 datoshi]
    /// 0B : OpCode.LDARG1 [2 datoshi]
    /// 0C : OpCode.OVER [2 datoshi]
    /// 0D : OpCode.PUSH1 [1 datoshi]
    /// 0E : OpCode.ROT [2 datoshi]
    /// 0F : OpCode.SETITEM [8192 datoshi]
    /// 10 : OpCode.STLOC0 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCEAsSv3l4Ek01ff///3BoSsFFcXJFaUA=
    /// 00 : OpCode.INITSLOT 0302 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 07 : OpCode.LDARG1 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PICK [2 datoshi]
    /// 0B : OpCode.CALL_L 7DFFFFFF [512 datoshi]
    /// 10 : OpCode.STLOC0 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.UNPACK [2048 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.STLOC2 [2 datoshi]
    /// 17 : OpCode.DROP [2 datoshi]
    /// 18 : OpCode.LDLOC1 [2 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00y3Bowb95EZ5LNAVxaEA=
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 07 : OpCode.LDARG1 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PICK [2 datoshi]
    /// 0B : OpCode.CALL CB [512 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.UNPACK [2048 datoshi]
    /// 10 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.PUSH1 [1 datoshi]
    /// 13 : OpCode.ADD [8 datoshi]
    /// 14 : OpCode.OVER [2 datoshi]
    /// 15 : OpCode.CALL 05 [512 datoshi]
    /// 17 : OpCode.STLOC1 [2 datoshi]
    /// 18 : OpCode.LDLOC0 [2 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEAsSv3l4Ek00qXBowb95EZ5LNOMMATB4i9soSzQFcWlA
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 07 : OpCode.LDARG1 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PICK [2 datoshi]
    /// 0B : OpCode.CALL A9 [512 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.UNPACK [2048 datoshi]
    /// 10 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.PUSH1 [1 datoshi]
    /// 13 : OpCode.ADD [8 datoshi]
    /// 14 : OpCode.OVER [2 datoshi]
    /// 15 : OpCode.CALL E3 [512 datoshi]
    /// 17 : OpCode.PUSHDATA1 30 [8 datoshi]
    /// 1A : OpCode.LDARG0 [2 datoshi]
    /// 1B : OpCode.CAT [2048 datoshi]
    /// 1C : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 1E : OpCode.OVER [2 datoshi]
    /// 1F : OpCode.CALL 05 [512 datoshi]
    /// 21 : OpCode.STLOC1 [2 datoshi]
    /// 22 : OpCode.LDLOC1 [2 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion
}
