using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Property(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Property"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStaticPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":84,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":79,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAF0MC1Rva2VuU3ltYm9sQFhKnGBFWEqcYEVYSpxgRVhAVwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeBDOQFcAAXgQENBAVgEQYEAQEcBKNO8ixUAqXXaJ"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtUb2tlblN5bWJvbEA=
    /// 00 : PUSHDATA1 546F6B656E53796D626F6C 'TokenSymbol' [8 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("symbol")]
    public abstract string? Symbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeBDOQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PICKITEM [64 datoshi]
    /// 07 : TUCK [2 datoshi]
    /// 08 : INC [4 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : SETITEM [8192 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : LDARG0 [2 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSH0 [1 datoshi]
    /// 11 : PICKITEM [64 datoshi]
    /// 12 : TUCK [2 datoshi]
    /// 13 : INC [4 datoshi]
    /// 14 : SWAP [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDARG0 [2 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : PUSH0 [1 datoshi]
    /// 1C : PICKITEM [64 datoshi]
    /// 1D : TUCK [2 datoshi]
    /// 1E : INC [4 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : PUSH0 [1 datoshi]
    /// 21 : ROT [2 datoshi]
    /// 22 : SETITEM [8192 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : LDARG0 [2 datoshi]
    /// 25 : PUSH0 [1 datoshi]
    /// 26 : PICKITEM [64 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInc")]
    public abstract BigInteger? TestPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEqcYEVYSpxgRVhKnGBFWEA=
    /// 00 : LDSFLD0 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD0 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD0 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD0 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : INC [4 datoshi]
    /// 0D : STSFLD0 [2 datoshi]
    /// 0E : DROP [2 datoshi]
    /// 0F : LDSFLD0 [2 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyInc")]
    public abstract BigInteger? TestStaticPropertyInc();

    #endregion
}
