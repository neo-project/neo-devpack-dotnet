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
    /// 00 : OpCode.LDSFLD6 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD6 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD6 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldDefault")]
    public abstract BigInteger? IncTestFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EmcHXwdA
    /// 00 : OpCode.PUSH2 [1 datoshi]
    /// 01 : OpCode.STSFLD 07 [2 datoshi]
    /// 03 : OpCode.LDSFLD 07 [2 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldValue")]
    public abstract BigInteger? IncTestFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XEqcZEVcQA==
    /// 00 : OpCode.LDSFLD4 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD4 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD4 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldDefault")]
    public abstract BigInteger? IncTestStaticFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XUqcZUVdQA==
    /// 00 : OpCode.LDSFLD5 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD5 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD5 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldValue")]
    public abstract BigInteger? IncTestStaticFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("symbol")]
    public abstract string? Symbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkqcYkVaSpxiRVpA
    /// 00 : OpCode.LDSFLD2 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD2 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD2 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.INC [4 datoshi]
    /// 08 : OpCode.STSFLD2 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.LDSFLD2 [2 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDefaultInc")]
    public abstract BigInteger? TestPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0qcY0VbSpxjRVtA
    /// 00 : OpCode.LDSFLD3 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD3 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD3 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.INC [4 datoshi]
    /// 08 : OpCode.STSFLD3 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.LDSFLD3 [2 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyValueInc")]
    public abstract BigInteger? TestPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEqcYEVZSpxhRVhA
    /// 00 : OpCode.LDSFLD0 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD0 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD1 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.INC [4 datoshi]
    /// 08 : OpCode.STSFLD1 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.LDSFLD0 [2 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDefaultInc")]
    public abstract BigInteger? TestStaticPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUqcYUVZQA==
    /// 00 : OpCode.LDSFLD1 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD1 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD1 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyValueInc")]
    public abstract BigInteger? TestStaticPropertyValueInc();

    #endregion
}
