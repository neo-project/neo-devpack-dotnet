using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":153,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":165,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":177,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":189,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":198,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAM9XAQEQcAg5EUpwRQk5AGRKcEVoIgJAVwABeDQDQFcAAUBXAQEQcHg02UpwRRFKcEVoIgJAVwIBEHA7CxJ4NMNKcEU9AHERSnBFPQASSnBFP1cCARBwOxMcEUpwRQwJZXhjZXB0aW9uOnF4NJZKcEU9ABJKcEU/VwIBEHA7CRARSnBFPRNxEkpwRT0MeDV0////SnBFP2giAkDCSjV7////I2D////CSjVv////I3X////CSjVj////I3z////CSjVX////IozCSjVO////Iqmb/XCA"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion

    #region Constructor for internal use only

    protected Contract_Assert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
