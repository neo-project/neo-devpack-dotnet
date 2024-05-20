using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[],""returntype"":""Any"",""offset"":77,""safe"":false},{""name"":""try03"",""parameters"":[],""returntype"":""Any"",""offset"":164,""safe"":false},{""name"":""tryNest"",""parameters"":[],""returntype"":""Any"",""offset"":253,""safe"":false},{""name"":""throwInCatch"",""parameters"":[],""returntype"":""Any"",""offset"":337,""safe"":false},{""name"":""tryFinally"",""parameters"":[],""returntype"":""Any"",""offset"":383,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[],""returntype"":""Any"",""offset"":453,""safe"":false},{""name"":""tryCatch"",""parameters"":[],""returntype"":""Any"",""offset"":526,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[],""returntype"":""Any"",""offset"":601,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[],""returntype"":""Any"",""offset"":893,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[],""returntype"":""Any"",""offset"":983,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Any"",""offset"":1073,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Any"",""offset"":1163,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Any"",""offset"":1253,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Any"",""offset"":1343,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[],""returntype"":""Array"",""offset"":1422,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[],""returntype"":""Array"",""offset"":1584,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[],""returntype"":""Array"",""offset"":1746,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[],""returntype"":""Array"",""offset"":1908,""safe"":false},{""name"":""throwcall"",""parameters"":[],""returntype"":""Any"",""offset"":241,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[],""returntype"":""Any"",""offset"":2059,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2135,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9wQhXAgAQcDsJEBJKcEU9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxMaEkpwRQwJZXhjZXB0aW9uOnETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOwkQEkpwRTRBcRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJADAlleGNlcHRpb246VwIAEHA7FQA7CRASSnBFNOVxE0pwRTTeNNxxaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPQJoIgJAVwIAEHA7EyQRSnBFDAlleGNlcHRpb246cRJKcEUMCWV4Y2VwdGlvbjoTSnBFP1cBABBwOwAJEkpwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAQAQcDsADBJKcEU1IP///2hKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7DAASSnBFNdf+//9xaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPQJoIgJAVwIAEHA8rwAAAOYAAAA7OG9oSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9bnFoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFPTdoE55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFPz1ucWgUnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU9N2gVnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU/aCICQFcCABBwOxYdEkpwRVhK2CQJSsoAISgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxYdEkpwRVlK2CQJSsoAISgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxYdEkpwRVhK2CQJSsoAFCgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxYdEkpwRVpK2CQJSsoAFCgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxYdEkpwRVhK2CQJSsoAICgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOwsSEkpwRVtxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAwAQcAAhiNsoStgkCUrKACEoAzpxOw0UEkpwRQtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAwAQcAAUiNsoStgkCUrKABQoAzpxOw0UEkpwRQtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAwAQcAAgiNsoStgkCUrKACAoAzpxOw0UEkpwRQtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAwAQcAwDMTIzcTsNFBJKcEULSnFFPXdyE0pwRT1waEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJqC5cmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT/FSmjPSmnPIgJAVwIAEHA7CA8SSnBFOHETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFYEDAYKCwwNDg9gDCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lhDBR+7hqr62ftHXkdROT1/POukXGocWIMIO3PhnkQTsKRGk/imtfbIypJPluZD7HaevDHuYmUjIklY0DPqvqE"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwcall")]
    public abstract object? Throwcall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwInCatch")]
    public abstract object? ThrowInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try01")]
    public abstract object? Try01();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try02")]
    public abstract object? Try02();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try03")]
    public abstract object? Try03();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryCatch")]
    public abstract object? TryCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryecpointCast")]
    public abstract object? TryecpointCast();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinally")]
    public abstract object? TryFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinallyAndRethrow")]
    public abstract object? TryFinallyAndRethrow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt160")]
    public abstract object? TryinvalidByteArray2UInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt256")]
    public abstract object? TryinvalidByteArray2UInt256();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNest")]
    public abstract object? TryNest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Bytestring_1")]
    public abstract IList<object>? TryNULL2Bytestring_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Ecpoint_1")]
    public abstract IList<object>? TryNULL2Ecpoint_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint160_1")]
    public abstract IList<object>? TryNULL2Uint160_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint256_1")]
    public abstract IList<object>? TryNULL2Uint256_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryUncatchableException")]
    public abstract object? TryUncatchableException();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt160")]
    public abstract object? TryvalidByteArray2UInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt256")]
    public abstract object? TryvalidByteArray2UInt256();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteString2Ecpoint")]
    public abstract object? TryvalidByteString2Ecpoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryWithTwoFinally")]
    public abstract object? TryWithTwoFinally();

    #endregion

    #region Constructor for internal use only

    protected Contract_TryCatch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
