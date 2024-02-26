using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OracleRequest : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""OracleRequest"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResponse"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""doRequest"",""parameters"":[],""returntype"":""Void"",""offset"":23,""safe"":false},{""name"":""onOracleResponse"",""parameters"":[{""name"":""requestedUrl"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""oracleResponse"",""type"":""Integer""},{""name"":""jsonString"",""type"":""String""}],""returntype"":""Void"",""offset"":134,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM25jY3MAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACJaHR0cHM6Ly9naXRodWIuY29tL25lby1wcm9qZWN0L25lby1kZXZwYWNrLWRvdG5ldC90cmVlL21hc3Rlci9zcmMvTmVvLlNtYXJ0Q29udHJhY3QuVGVtcGxhdGUvdGVtcGxhdGVzL25lb2NvbnRyYWN0b3JhY2xlL09yYWNsZVJlcXVlc3QuY3MAA1iHFxF+CqgQcq+rcdLdif58S5L+B3JlcXVlc3QFAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrA9qc29uRGVzZXJpYWxpemUBAAEPAAD9CwEMCFJlc3BvbnNlQZv2Z85Bkl3oMSICQFcBAAw1aHR0cHM6Ly9hcGkuanNvbmJpbi5pby92My9xcy82NTIwYWQzYzEyYTVkMzc2NTk4ODU0MmFwAoCWmAALDBBvbk9yYWNsZVJlc3BvbnNlDBUkLnJlY29yZC5wcm9wZXJ0eU5hbWVoNwAAQFcCBEE5U248DBRYhxcRfgqoEHKvq3HS3Yn+fEuS/pgmFgwRTm8gQXV0aG9yaXphdGlvbiE6ehCYJi4MIk9yYWNsZSByZXNwb25zZSBmYWlsdXJlIHdpdGggY29kZSB6NwEAi9soOns3AgBwaBDOcWkMCFJlc3BvbnNlQZv2Z85B5j8YhECdGZuJ"));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Response { [DisplayName("getResponse")] get; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("doRequest")]
    public abstract void DoRequest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onOracleResponse")]
    public abstract void OnOracleResponse(string? requestedUrl, object? userData, BigInteger? oracleResponse, string? jsonString);

    #endregion

    #region Constructor for internal use only

    protected OracleRequest(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
