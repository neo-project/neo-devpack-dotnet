using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IOracle : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IOracle"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""onOracleResponse"",""parameters"":[{""name"":""url"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""code"",""type"":""Integer""},{""name"":""result"",""type"":""String""}],""returntype"":""Void"",""offset"":80,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM3Rlc3Rob3N0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFZXAAVBOVNuPAwUWIcXEX4KqBByr6tx0t2J/nxLkv6YJhIMDVVuYXV0aG9yaXplZCE6DAxPcmFjbGUgY2FsbCFBz+dHlkBXAAF4NANAVwABQMJKNPMirKokvT8="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onOracleResponse")]
    public abstract void OnOracleResponse(string? url, object? userData, BigInteger? code, string? result);

    #endregion

    #region Constructor for internal use only

    protected Contract_IOracle(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
