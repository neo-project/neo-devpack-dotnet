using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Math : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Math"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""max"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""min"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""sign"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""abs"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":20,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpXAAJ5eLpAVwACeXi5QFcAAXiZQFcAAXiaQGpzeIw="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("abs")]
    public abstract BigInteger Abs(BigInteger a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("max")]
    public abstract BigInteger Max(BigInteger a, BigInteger b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("min")]
    public abstract BigInteger Min(BigInteger a, BigInteger b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sign")]
    public abstract BigInteger Sign(BigInteger a);

    #endregion

    #region Constructor for internal use only

    protected Contract_Math(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
