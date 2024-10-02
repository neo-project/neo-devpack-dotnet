using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PropertyMethod(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PropertyMethod"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testProperty"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testProperty2"",""parameters"":[],""returntype"":""Void"",""offset"":49,""safe"":false},{""name"":""testProperty3"",""parameters"":[],""returntype"":""Any"",""offset"":69,""safe"":false},{""name"":""testProperty4"",""parameters"":[],""returntype"":""Map"",""offset"":91,""safe"":false},{""name"":""testProperty5"",""parameters"":[],""returntype"":""Array"",""offset"":107,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHVXAQAQCxLAGgwETkVPMxJNNA9wxUpoEM7PSmgRzs9AVwADeUp4EFHQRXpKeBFR0EVAVwEAEAsSwBoMBE5FTzMSTTTecEAQCxLASjQNDARORU8zSxBR0EBXAAFAyEoMBE5hbWUMBE5FTzPQQMJFFRQTEhEVwECmDbuk"));

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

    #endregion

}
