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

    public static readonly Neo.SmartContract.Testing.Coverage.NeoDebugInfo DebugInfo = Neo.SmartContract.Testing.Coverage.NeoDebugInfo.FromDebugInfoJson(@"{""hash"":""0xd25751c5e8ca61ec3ba533e509bf9367c7a636de"",""documents"":[""OracleRequest.cs""],""document-root"":""C:\\Red4Sec\\Neo\\neo-devpack-dotnet\\src\\Neo.SmartContract.Template\\bin\\Debug\\net7.0\\oracle"",""static-variables"":[],""methods"":[{""id"":""Neo.SmartContract.Template.OracleRequest.GetResponse()"",""name"":""Neo.SmartContract.Template.OracleRequest,GetResponse"",""range"":""0-22"",""params"":[],""return"":""String"",""variables"":[],""sequence-points"":[""0[0]26:56-26:66"",""10[0]26:32-26:54"",""15[0]26:20-26:67"",""20[0]26:13-26:68"",""22[0]27:9-27:10""]},{""id"":""Neo.SmartContract.Template.OracleRequest.DoRequest()"",""name"":""Neo.SmartContract.Template.OracleRequest,DoRequest"",""range"":""23-133"",""params"":[],""return"":""Void"",""variables"":[""requestUrl,String,0""],""sequence-points"":[""26[0]50:30-50:85"",""81[0]50:17-50:85"",""82[0]51:91-51:116"",""87[0]51:85-51:89"",""88[0]51:65-51:83"",""106[0]51:40-51:63"",""129[0]51:28-51:38"",""130[0]51:13-51:117"",""133[0]52:9-52:10""]},{""id"":""Neo.SmartContract.Template.OracleRequest.onOracleResponse(string, object, Neo.SmartContract.Framework.Native.OracleResponseCode, string)"",""name"":""Neo.SmartContract.Template.OracleRequest,onOracleResponse"",""range"":""134-266"",""params"":[""requestedUrl,String,0"",""userData,Any,1"",""oracleResponse,Integer,2"",""jsonString,String,3""],""return"":""Void"",""variables"":[""jsonArrayValues,Array,0"",""jsonFirstValue,String,1""],""sequence-points"":[""137[0]57:17-57:42"",""142[0]57:46-57:57"",""164[0]57:17-57:57"",""165[0]57:13-58:74"",""167[0]58:53-58:72"",""186[0]58:17-58:74"",""187[0]59:17-59:31"",""188[0]59:35-59:61"",""189[0]59:17-59:61"",""190[0]59:13-60:98"",""192[0]60:37-60:73"",""228[0]60:82-60:96"",""229[0]60:37-60:96"",""232[0]60:37-60:96"",""233[0]60:37-60:96"",""235[0]60:17-60:98"",""236[0]62:68-62:78"",""237[0]62:45-62:79"",""240[0]62:17-62:79"",""241[0]63:42-63:57"",""242[0]63:58-63:59"",""243[0]63:42-63:60"",""244[0]63:17-63:60"",""245[0]65:61-65:75"",""246[0]65:49-65:59"",""256[0]65:25-65:47"",""261[0]65:13-65:76"",""266[0]66:9-66:10""]}],""events"":[]}");

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
