using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Json : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Json"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""serialize"",""parameters"":[{""name"":""obj"",""type"":""Any""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""deserialize"",""parameters"":[{""name"":""json"",""type"":""String""}],""returntype"":""Any"",""offset"":10,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""jsonDeserialize"",""jsonSerialize""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABD8DvOc7g5OklxsKgannhRA3Yb86sD2pzb25EZXNlcmlhbGl6ZQEAAQ8AABRXAAF4NwAAIgJAVwABeDcBACICQFNXWLI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deserialize")]
    public abstract object? Deserialize(string? json);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("serialize")]
    public abstract string? Serialize(object? obj = null);

    #endregion

    #region Constructor for internal use only

    protected Contract_Json(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
