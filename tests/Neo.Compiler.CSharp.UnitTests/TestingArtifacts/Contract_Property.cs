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
    [DisplayName("symbol")]
    public abstract string? Symbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeBDOQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.PICKITEM [64 datoshi]
    /// 07 : OpCode.TUCK [2 datoshi]
    /// 08 : OpCode.INC [4 datoshi]
    /// 09 : OpCode.SWAP [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.ROT [2 datoshi]
    /// 0C : OpCode.SETITEM [8192 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.LDARG0 [2 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.PUSH0 [1 datoshi]
    /// 11 : OpCode.PICKITEM [64 datoshi]
    /// 12 : OpCode.TUCK [2 datoshi]
    /// 13 : OpCode.INC [4 datoshi]
    /// 14 : OpCode.SWAP [2 datoshi]
    /// 15 : OpCode.PUSH0 [1 datoshi]
    /// 16 : OpCode.ROT [2 datoshi]
    /// 17 : OpCode.SETITEM [8192 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.LDARG0 [2 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.PUSH0 [1 datoshi]
    /// 1C : OpCode.PICKITEM [64 datoshi]
    /// 1D : OpCode.TUCK [2 datoshi]
    /// 1E : OpCode.INC [4 datoshi]
    /// 1F : OpCode.SWAP [2 datoshi]
    /// 20 : OpCode.PUSH0 [1 datoshi]
    /// 21 : OpCode.ROT [2 datoshi]
    /// 22 : OpCode.SETITEM [8192 datoshi]
    /// 23 : OpCode.DROP [2 datoshi]
    /// 24 : OpCode.LDARG0 [2 datoshi]
    /// 25 : OpCode.PUSH0 [1 datoshi]
    /// 26 : OpCode.PICKITEM [64 datoshi]
    /// 27 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInc")]
    public abstract BigInteger? TestPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEqcYEVYSpxgRVhKnGBFWEA=
    /// 00 : OpCode.LDSFLD0 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD0 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.INC [4 datoshi]
    /// 08 : OpCode.STSFLD0 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.LDSFLD0 [2 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.INC [4 datoshi]
    /// 0D : OpCode.STSFLD0 [2 datoshi]
    /// 0E : OpCode.DROP [2 datoshi]
    /// 0F : OpCode.LDSFLD0 [2 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyInc")]
    public abstract BigInteger? TestStaticPropertyInc();

    #endregion
}
