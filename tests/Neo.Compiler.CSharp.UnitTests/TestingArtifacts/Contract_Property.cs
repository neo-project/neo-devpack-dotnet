using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Property(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Property"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStaticPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":48,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADMMC1Rva2VuU3ltYm9sQFhKnGBFWEqcYEVYSpxgRVhAWUqcYUVZSpxhRVlKnGFFWUBWAkA1Yowb"));

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
    /// Script: WUqcYUVZSpxhRVlKnGFFWUA=
    /// 00 : OpCode.LDSFLD1 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD1 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD1 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.INC [4 datoshi]
    /// 08 : OpCode.STSFLD1 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.LDSFLD1 [2 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.INC [4 datoshi]
    /// 0D : OpCode.STSFLD1 [2 datoshi]
    /// 0E : OpCode.DROP [2 datoshi]
    /// 0F : OpCode.LDSFLD1 [2 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
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
