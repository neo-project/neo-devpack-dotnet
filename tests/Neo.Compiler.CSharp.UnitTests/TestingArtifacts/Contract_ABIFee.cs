using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ABIFee(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ABIFee"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""noFeeMethod"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""fixedFeeMethod"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":false},{""name"":""dynamicFeeMethod"",""parameters"":[],""returntype"":""Integer"",""offset"":4,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.8.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYRQBJAE0C2q2lD").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: E0A=
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("dynamicFeeMethod")]
    public abstract BigInteger? DynamicFeeMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EkA=
    /// PUSH2 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("fixedFeeMethod")]
    public abstract BigInteger? FixedFeeMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EUA=
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("noFeeMethod")]
    public abstract BigInteger? NoFeeMethod();

    #endregion
}
