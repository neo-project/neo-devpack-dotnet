using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ABISafe(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ABISafe"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_001"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_002"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":true},{""name"":""unitTest_003"",""parameters"":[],""returntype"":""Integer"",""offset"":4,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYRQBJAE0C2q2lD"));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? UnitTest_002 { [DisplayName("unitTest_002")] get; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EUA=
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_001")]
    public abstract BigInteger? UnitTest_001();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: E0A=
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_003")]
    public abstract BigInteger? UnitTest_003();

    #endregion
}
