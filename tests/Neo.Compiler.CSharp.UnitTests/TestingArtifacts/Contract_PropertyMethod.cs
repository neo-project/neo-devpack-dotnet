using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PropertyMethod(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PropertyMethod"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testProperty"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testProperty2"",""parameters"":[],""returntype"":""Void"",""offset"":50,""safe"":false},{""name"":""testProperty3"",""parameters"":[],""returntype"":""Any"",""offset"":71,""safe"":false},{""name"":""testProperty4"",""parameters"":[],""returntype"":""Map"",""offset"":94,""safe"":false},{""name"":""testProperty5"",""parameters"":[],""returntype"":""Array"",""offset"":110,""safe"":false},{""name"":""testPropertyInit"",""parameters"":[],""returntype"":""Array"",""offset"":120,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALRXAQALEAsTwBoMBE5FTzMSTTQPcMVKaBDOz0poEc7PQFcAA3lKeBBR0EV6SngRUdBFQFcBAAsQCxPAGgwETkVPMxJNNN1wQAsQCxPASjQNDARORU8zSxBR0EBXAAFAyEoMBE5hbWUMBE5FTzPQQMJFFRQTEhEVwEBXAQALEAsTwBoMBE5FTzMSTTSXDBExMjMgQmxvY2tjaGFpbiBTdEsSUdBwxUpoEM7PSmgRzs9KaBLOz0Dp1QFJ"));

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
    [DisplayName("testProperty3")]
    public abstract object? TestProperty3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testProperty4")]
    public abstract IDictionary<object, object>? TestProperty4();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testProperty5")]
    public abstract IList<object>? TestProperty5();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPropertyInit")]
    public abstract IList<object>? TestPropertyInit();

    #endregion

}
