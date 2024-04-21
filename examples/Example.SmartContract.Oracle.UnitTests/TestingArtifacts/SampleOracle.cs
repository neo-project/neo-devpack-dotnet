using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleOracle : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleOracle"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResponse"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""doRequest"",""parameters"":[],""returntype"":""Void"",""offset"":23,""safe"":false},{""name"":""onOracleResponse"",""parameters"":[{""name"":""requestedUrl"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""oracleResponse"",""type"":""Integer""},{""name"":""jsonString"",""type"":""String""}],""returntype"":""Void"",""offset"":278,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use Example.SmartContract.Oracle Service"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANYhxcRfgqoEHKvq3HS3Yn+fEuS/gdyZXF1ZXN0BQAAD8DvOc7g5OklxsKgannhRA3Yb86sBGl0b2EBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwPanNvbkRlc2VyaWFsaXplAQABDwAA/R8BDAhSZXNwb25zZUGb9mfOQZJd6DEiAkBXAQAMNWh0dHBzOi8vYXBpLmpzb25iaW4uaW8vdjMvcXMvNjUyMGFkM2MxMmE1ZDM3NjU5ODg1NDJhcAKAlpgACwwQb25PcmFjbGVSZXNwb25zZQwVJC5yZWNvcmQucHJvcGVydHlOYW1laDcAAEBXAgVBOVNuPAwUWIcXEX4KqBByr6tx0t2J/nxLkv6YJhYMEU5vIEF1dGhvcml6YXRpb24hOnsQmCYuDCJPcmFjbGUgcmVzcG9uc2UgZmFpbHVyZSB3aXRoIGNvZGUgezcBAIvbKDp8NwIAcGgQznFpDAhSZXNwb25zZUGb9mfOQeY/GIRAVwABeDQDQFcAAUDCSjTzI2z///8B5jWd"));

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
