using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class SampleNeoFS(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleNeoFS"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getObjectUri"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""getRangeUri"",""parameters"":[{""name"":""offset"",""type"":""Integer""},{""name"":""length"",""type"":""Integer""}],""returntype"":""String"",""offset"":102,""safe"":true},{""name"":""getHeaderUri"",""parameters"":[],""returntype"":""String"",""offset"":142,""safe"":true},{""name"":""getHashUri"",""parameters"":[],""returntype"":""String"",""offset"":162,""safe"":true},{""name"":""getStoredPayload"",""parameters"":[],""returntype"":""String"",""offset"":180,""safe"":true},{""name"":""requestObject"",""parameters"":[],""returntype"":""Void"",""offset"":236,""safe"":false},{""name"":""requestRange"",""parameters"":[{""name"":""offset"",""type"":""Integer""},{""name"":""length"",""type"":""Integer""}],""returntype"":""Void"",""offset"":276,""safe"":false},{""name"":""onOracleResponse"",""parameters"":[{""name"":""requestedUrl"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""oracleResponse"",""type"":""Integer""},{""name"":""payload"",""type"":""String""}],""returntype"":""Void"",""offset"":317,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to request NeoFS objects through the Oracle service"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy45LjErNWZhOTU2NmU1MTY1ZWRlMjE2NWE5YmUxZjRhMDEyMGMxNzYuLi4AAALA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD1iHFxF+CqgQcq+rcdLdif58S5L+B3JlcXVlc3QFAAAPAAD92wEMYW5lb2ZzOi8vQzNzd2ZnOE1pTUo5YlhiZUZHNmRXSlRDb0hwOWhBRVprSGV6dmJTd0sxQ2MvM25RSDFMOHUzZU05anQybVpDczZNeWp6ZGplcmRTekJrWENZWWo0TTRabmsiAkBXAAI0lwwHL3JhbmdlL4vbKHg3AACL2ygMAXyL2yh5NwAAi9soIgJANXL///8MBy9oZWFkZXKL2ygiAkA1Xv///wwFL2hhc2iL2ygiAkBXAgAMDE5lb0ZTUGF5bG9hZEH2tGviQZJd6DFwaHFpC5cmBgwAIgNoIgJAQZJd6DFAQfa0a+JAQAKAlpgADAAMEG9uT3JhY2xlUmVzcG9uc2UMADX5/v//NwEAQDcBAEBXAAICgJaYAAwADBBvbk9yYWNsZVJlc3BvbnNlDAB5eDUy////NwEAQFcABEE5U248DBRYhxcRfgqoEHKvq3HS3Yn+fEuS/pgmFgwRTm8gQXV0aG9yaXphdGlvbiE6ehCYJi4MIk9yYWNsZSByZXNwb25zZSBmYWlsdXJlIHdpdGggY29kZSB6NwAAi9soOnsMDE5lb0ZTUGF5bG9hZEE5DOMKQEE5U248QAwUWIcXEX4KqBByr6tx0t2J/nxLkv5AQTkM4wpA1P1v3Q==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? HashUri { [DisplayName("getHashUri")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? HeaderUri { [DisplayName("getHeaderUri")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? ObjectUri { [DisplayName("getObjectUri")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? StoredPayload { [DisplayName("getStoredPayload")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getRangeUri")]
    public abstract string? GetRangeUri(BigInteger? offset, BigInteger? length);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onOracleResponse")]
    public abstract void OnOracleResponse(string? requestedUrl, object? userData, BigInteger? oracleResponse, string? payload);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("requestObject")]
    public abstract void RequestObject();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("requestRange")]
    public abstract void RequestRange(BigInteger? offset, BigInteger? length);

    #endregion
}
