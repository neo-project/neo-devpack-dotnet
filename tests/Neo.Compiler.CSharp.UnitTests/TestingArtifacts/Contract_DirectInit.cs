using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_DirectInit : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DirectInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testGetUInt160"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":false},{""name"":""testGetECPoint"",""parameters"":[],""returntype"":""PublicKey"",""offset"":4,""safe"":false},{""name"":""testGetUInt256"",""parameters"":[],""returntype"":""Hash256"",""offset"":8,""safe"":false},{""name"":""testGetString"",""parameters"":[],""returntype"":""String"",""offset"":12,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":16,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9YIgJAWSICQFoiAkBbIgJAVgQMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWEMFH7uGqvrZ+0deR1E5PX8866RcahxYAwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSViDAtoZWxsbyB3b3JsZGNAwG5O8A=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetECPoint")]
    public abstract ECPoint? TestGetECPoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetString")]
    public abstract string? TestGetString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetUInt160")]
    public abstract UInt160? TestGetUInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetUInt256")]
    public abstract UInt256? TestGetUInt256();

    #endregion

    #region Constructor for internal use only

    protected Contract_DirectInit(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
