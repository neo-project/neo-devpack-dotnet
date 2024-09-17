using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":121,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":133,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":145,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":157,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":166,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALBXAQEQcAg5EXAJOQBkcGhAVwABeDQDQFcAAUBXAQEQcHg033ARcGhAVwIBEHA7CQ54NM9wPQpxEXA9BRJwP2hAVwIBEHA7ERgRcAwJZXhjZXB0aW9uOnF4NKhwPQUScD9oQFcCARBwOwcMEXA9AHEScD0AeDWM////wko1lf///yOA////wko1if///yOP////wko1ff///yOQ////wko1cf///yKcwko1aP///yK1QF5bPYA="));

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

}
