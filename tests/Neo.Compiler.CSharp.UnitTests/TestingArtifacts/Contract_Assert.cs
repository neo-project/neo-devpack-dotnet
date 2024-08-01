using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":165,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":177,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":189,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":201,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":210,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANtXAQEQcAg5EUpwRQk5AGRKcEVoIgJAOUBXAAF4NANAVwABQFcBARBweDTXSnBFEUpwRWgiAkBXAgEQcDsLEng0wUpwRT0OcRFKcEU9BxJKcEU/aCICQFcCARBwOxUeEUpwRQwJZXhjZXB0aW9uOj0QcXg0jkpwRT0HEkpwRT9oIgJAVwIBEHA7CRARSnBFPRNxEkpwRT0MeDVo////SnBFP2giAkDCSjVx////I1T////CSjVl////I2v////CSjVZ////I3L////CSjVN////IobCSjVE////Iqkbgcvu"));

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
