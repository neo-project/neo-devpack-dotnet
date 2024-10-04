using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PropertyMethod(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PropertyMethod"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testProperty"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testProperty2"",""parameters"":[],""returntype"":""Void"",""offset"":50,""safe"":false},{""name"":""testPropertyInit"",""parameters"":[],""returntype"":""Array"",""offset"":71,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAINXAQALEAsTwBoMBE5FTzMSTTQPcMVKaBDOz0poEc7PQFcAA3lKeBBR0EV6SngRUdBFQFcBAAsQCxPAGgwETkVPMxJNNN1wQFcBAAsQCxPAGgwETkVPMxJNNMgMETEyMyBCbG9ja2NoYWluIFN0SxJR0HDFSmgQzs9KaBHOz0poEs7PQGh8TO4="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testProperty")]
    public abstract IList<object>? TestProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testProperty2")]
    public abstract void TestProperty2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPropertyInit")]
    public abstract IList<object>? TestPropertyInit();

    #endregion

}
