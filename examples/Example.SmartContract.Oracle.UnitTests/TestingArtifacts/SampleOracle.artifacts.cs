using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleOracle : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleOracle"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResponse"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""doRequest"",""parameters"":[],""returntype"":""Void"",""offset"":35,""safe"":false},{""name"":""onOracleResponse"",""parameters"":[{""name"":""requestedUrl"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""oracleResponse"",""type"":""Integer""},{""name"":""jsonString"",""type"":""String""}],""returntype"":""Void"",""offset"":333,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use Example.SmartContract.Oracle Service"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIrZmFiMWEyZWVhZGYyMTE2NjhiMjg0ZWZiYTgwYzFhNTU3ZTYuLi4AAANYhxcRfgqoEHKvq3HS3Yn+fEuS/gdyZXF1ZXN0BQAAD8DvOc7g5OklxsKgannhRA3Yb86sBGl0b2EBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwPanNvbkRlc2VyaWFsaXplAQABDwAA/VYBDAhSZXNwb25zZUGb9mfOQZJd6DEiAkBBkl3oMUBBm/ZnzkBXAQAMNWh0dHBzOi8vYXBpLmpzb25iaW4uaW8vdjMvcXMvNjUyMGFkM2MxMmE1ZDM3NjU5ODg1NDJhcAKAlpgACwwQb25PcmFjbGVSZXNwb25zZQwVJC5yZWNvcmQucHJvcGVydHlOYW1laDcAAEA3AABAVwIFQTlTbjwMFFiHFxF+CqgQcq+rcdLdif58S5L+mCYWDBFObyBBdXRob3JpemF0aW9uITp7EJgmLgwiT3JhY2xlIHJlc3BvbnNlIGZhaWx1cmUgd2l0aCBjb2RlIHs3AQCL2yg6fDcCAHBoEM5xaQwIUmVzcG9uc2VBm/ZnzkHmPxiEQEE5U248QAwUWIcXEX4KqBByr6tx0t2J/nxLkv5ANwIAQEHmPxiEQFcAAXg0A0BXAAFAwko08yNF////2BWHng=="));

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

    protected SampleOracle(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
