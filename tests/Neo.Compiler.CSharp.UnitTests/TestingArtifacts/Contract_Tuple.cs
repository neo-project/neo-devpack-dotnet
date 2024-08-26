using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Tuple(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Tuple"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResult"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""t1"",""parameters"":[],""returntype"":""Any"",""offset"":14,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFrFShHPShLPShPPShTPQFcCAMVKC89KC89KEM9KEM9KC89KNDBwxUoLz0oQz0o0KUpoFFHQRRBxNMlKwUVoElHQaBTOEVHQRXFFaUpoE1HQRWhAVwABQFcAAUB+EaX0"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion

}
