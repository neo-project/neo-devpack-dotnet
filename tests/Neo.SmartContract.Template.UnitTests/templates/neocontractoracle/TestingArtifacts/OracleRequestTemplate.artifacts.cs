using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OracleRequestTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""OracleRequest"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResponse"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""doRequest"",""parameters"":[],""returntype"":""Void"",""offset"":21,""safe"":false},{""name"":""onOracleResponse"",""parameters"":[{""name"":""requestedUrl"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""oracleResponse"",""type"":""Integer""},{""name"":""jsonString"",""type"":""String""}],""returntype"":""Void"",""offset"":132,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractoracle/OracleRequest.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANYhxcRfgqoEHKvq3HS3Yn+fEuS/gdyZXF1ZXN0BQAAD8DvOc7g5OklxsKgannhRA3Yb86sBGl0b2EBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwPanNvbkRlc2VyaWFsaXplAQABDwAA/QkBDAhSZXNwb25zZUGb9mfOQZJd6DFAVwEADDVodHRwczovL2FwaS5qc29uYmluLmlvL3YzL3FzLzY1MjBhZDNjMTJhNWQzNzY1OTg4NTQyYXACgJaYAAsMEG9uT3JhY2xlUmVzcG9uc2UMFSQucmVjb3JkLnByb3BlcnR5TmFtZWg3AABAVwIEQTlTbjwMFFiHFxF+CqgQcq+rcdLdif58S5L+mCYWDBFObyBBdXRob3JpemF0aW9uITp6EJgmLgwiT3JhY2xlIHJlc3BvbnNlIGZhaWx1cmUgd2l0aCBjb2RlIHo3AQCL2yg6ezcCAHBoEM5xaQwIUmVzcG9uc2VBm/ZnzkHmPxiEQA/Nop4="));

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
    /// <remarks>
    /// Script: VwEADDVodHRwczovL2FwaS5qc29uYmluLmlvL3YzL3FzLzY1MjBhZDNjMTJhNWQzNzY1OTg4NTQyYXACgJaYAAsMEG9uT3JhY2xlUmVzcG9uc2UMFSQucmVjb3JkLnByb3BlcnR5TmFtZWg3AABA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 68747470733A2F2F6170692E6A736F6E62696E2E696F2F76332F71732F363532306164336331326135643337363539383835343261 'https://api.jsonbin.io/v3/qs/6520ad3c12a5d3765988542a' [8 datoshi]
    /// 3A : OpCode.STLOC0 [2 datoshi]
    /// 3B : OpCode.PUSHINT32 80969800 [1 datoshi]
    /// 40 : OpCode.PUSHNULL [1 datoshi]
    /// 41 : OpCode.PUSHDATA1 6F6E4F7261636C65526573706F6E7365 'onOracleResponse' [8 datoshi]
    /// 53 : OpCode.PUSHDATA1 242E7265636F72642E70726F70657274794E616D65 [8 datoshi]
    /// 6A : OpCode.LDLOC0 [2 datoshi]
    /// 6B : OpCode.CALLT 0000 [32768 datoshi]
    /// 6E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("doRequest")]
    public abstract void DoRequest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIEQTlTbjwMFFiHFxF+CqgQcq+rcdLdif58S5L+mCYWDBFObyBBdXRob3JpemF0aW9uITp6EJgmLgwiT3JhY2xlIHJlc3BvbnNlIGZhaWx1cmUgd2l0aCBjb2RlIHo3AQCL2yg6ezcCAHBoEM5xaQwIUmVzcG9uc2VBm/ZnzkHmPxiEQA==
    /// 00 : OpCode.INITSLOT 0204 [64 datoshi]
    /// 03 : OpCode.SYSCALL 39536E3C 'System.Runtime.GetCallingScriptHash' [16 datoshi]
    /// 08 : OpCode.PUSHDATA1 588717117E0AA81072AFAB71D2DD89FE7C4B92FE [8 datoshi]
    /// 1E : OpCode.NOTEQUAL [32 datoshi]
    /// 1F : OpCode.JMPIFNOT 16 [2 datoshi]
    /// 21 : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 34 : OpCode.THROW [512 datoshi]
    /// 35 : OpCode.LDARG2 [2 datoshi]
    /// 36 : OpCode.PUSH0 [1 datoshi]
    /// 37 : OpCode.NOTEQUAL [32 datoshi]
    /// 38 : OpCode.JMPIFNOT 2E [2 datoshi]
    /// 3A : OpCode.PUSHDATA1 4F7261636C6520726573706F6E7365206661696C757265207769746820636F646520 [8 datoshi]
    /// 5E : OpCode.LDARG2 [2 datoshi]
    /// 5F : OpCode.CALLT 0100 [32768 datoshi]
    /// 62 : OpCode.CAT [2048 datoshi]
    /// 63 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 65 : OpCode.THROW [512 datoshi]
    /// 66 : OpCode.LDARG3 [2 datoshi]
    /// 67 : OpCode.CALLT 0200 [32768 datoshi]
    /// 6A : OpCode.STLOC0 [2 datoshi]
    /// 6B : OpCode.LDLOC0 [2 datoshi]
    /// 6C : OpCode.PUSH0 [1 datoshi]
    /// 6D : OpCode.PICKITEM [64 datoshi]
    /// 6E : OpCode.STLOC1 [2 datoshi]
    /// 6F : OpCode.LDLOC1 [2 datoshi]
    /// 70 : OpCode.PUSHDATA1 526573706F6E7365 'Response' [8 datoshi]
    /// 7A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 7F : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 84 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("onOracleResponse")]
    public abstract void OnOracleResponse(string? requestedUrl, object? userData, BigInteger? oracleResponse, string? jsonString);

    #endregion
}
