using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Abort : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Abort"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAbort"",""parameters"":[],""returntype"":""Integer"",""offset"":239,""safe"":false},{""name"":""testAbortMsg"",""parameters"":[],""returntype"":""Integer"",""offset"":251,""safe"":false},{""name"":""testAbortInFunction"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":263,""safe"":false},{""name"":""testAbortInTry"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":275,""safe"":false},{""name"":""testAbortInCatch"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":287,""safe"":false},{""name"":""testAbortInFinally"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":299,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP03AVcBARBwOABkSnBFaCICQDhAVwABeDQDQFcAAUBXAQEQcAwJQUJPUlQgTVNH4ABkSnBFaCICQOBAVwECEHB5Jgp4NNtKcEUiCHg0t0pwRRFKcEVoIgJAVwICEHA7Fh15Jgp4NLpKcEUiCHg0lkpwRT0OcRFKcEU9BxJKcEU/AWgiAkBXAgIQcDsVLxFKcEUMCWV4Y2VwdGlvbjo9IXF5Jg14NXz///9KcEUiC3g1Vf///0pwRT0HEkpwRT8BaCICQFcCAhBwOwkQEUpwRT0hcRJKcEU9GnkmDXg1Rf///0pwRSILeDUe////SnBFPwFoIgJAwko1IP///yMK////wko1FP///yMa////wko1CP///yMq////wko1/P7//yM8////wko18P7//yNb////wko15P7//yKMT1c8Zg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion

    #region Constructor for internal use only

    protected Contract_Abort(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
