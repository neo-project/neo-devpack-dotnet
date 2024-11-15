using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Property(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Property"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStaticPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testStaticPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":26,""safe"":false},{""name"":""testPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":33,""safe"":false},{""name"":""testPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":45,""safe"":false},{""name"":""incTestStaticFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":57,""safe"":false},{""name"":""incTestStaticFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""incTestFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":71,""safe"":false},{""name"":""incTestFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":84,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGgMC1Rva2VuU3ltYm9sQFhKnGBFWUqcYUVYQFlKnGFFWUBaSpxiRVpKnGJFWkBbSpxjRVtKnGNFW0BcSpxkRVxAXUqcZUVdQF5KnGZFXkASZwdfB0BWCBBkEWUQZhJnBxBgGmEQYhtjQDqL34g="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XkqcZkVeQA==
    /// 00 : LDSFLD6 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD6 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD6 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldDefault")]
    public abstract BigInteger? IncTestFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EmcHXwdA
    /// 00 : PUSH2 [1 datoshi]
    /// 01 : STSFLD 07 [2 datoshi]
    /// 03 : LDSFLD 07 [2 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldValue")]
    public abstract BigInteger? IncTestFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XEqcZEVcQA==
    /// 00 : LDSFLD4 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD4 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD4 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldDefault")]
    public abstract BigInteger? IncTestStaticFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XUqcZUVdQA==
    /// 00 : LDSFLD5 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD5 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD5 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldValue")]
    public abstract BigInteger? IncTestStaticFieldValue();

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
    /// Script: WkqcYkVaSpxiRVpA
    /// 00 : LDSFLD2 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD2 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD2 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD2 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD2 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDefaultInc")]
    public abstract BigInteger? TestPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0qcY0VbSpxjRVtA
    /// 00 : LDSFLD3 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD3 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD3 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD3 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD3 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyValueInc")]
    public abstract BigInteger? TestPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEqcYEVZSpxhRVhA
    /// 00 : LDSFLD0 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD0 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD1 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD1 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD0 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDefaultInc")]
    public abstract BigInteger? TestStaticPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUqcYUVZQA==
    /// 00 : LDSFLD1 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD1 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD1 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyValueInc")]
    public abstract BigInteger? TestStaticPropertyValueInc();

    #endregion
}
