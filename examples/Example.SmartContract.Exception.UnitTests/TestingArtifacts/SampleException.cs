using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleException : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleException"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[],""returntype"":""Any"",""offset"":77,""safe"":false},{""name"":""try03"",""parameters"":[],""returntype"":""Any"",""offset"":164,""safe"":false},{""name"":""tryNest"",""parameters"":[],""returntype"":""Any"",""offset"":253,""safe"":false},{""name"":""tryFinally"",""parameters"":[],""returntype"":""Any"",""offset"":337,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[],""returntype"":""Any"",""offset"":407,""safe"":false},{""name"":""tryCatch"",""parameters"":[],""returntype"":""Any"",""offset"":480,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[],""returntype"":""Any"",""offset"":555,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[],""returntype"":""Any"",""offset"":847,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[],""returntype"":""Any"",""offset"":937,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Any"",""offset"":1027,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Any"",""offset"":1117,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Any"",""offset"":1207,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Any"",""offset"":1297,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[],""returntype"":""Array"",""offset"":1389,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[],""returntype"":""Array"",""offset"":1551,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[],""returntype"":""Array"",""offset"":1713,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[],""returntype"":""Array"",""offset"":1875,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[],""returntype"":""Any"",""offset"":2026,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2102,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""A sample contract to demonstrate how to handle exception"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2gCFcCABBwOwkQEkpwRT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7ExoSSnBFDAlleGNlcHRpb246cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7CRASSnBFNEFxE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkAMCWV4Y2VwdGlvbjpXAgAQcDsVADsJEBJKcEU05XETSnBFNN403HFoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9AmgiAkBXAQAQcDsACRJKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwEAEHA7AAwSSnBFNU7///9oSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOwwAEkpwRTUF////cWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT0CaCICQFcCABBwPK8AAADmAAAAOzhvaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPW5xaBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRT03aBOeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRT89bnFoFJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFPTdoFZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFP2giAkBXAgAQcDsWHRJKcEVYStgkCUrKACEoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVZStgkCUrKACEoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVYStgkCUrKABQoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVaStgkCUrKABQoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVYStgkCUrKACAoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsYHxJKcEVb2yhK2CQJSsoAICgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcDABBwACGI2yhK2CQJSsoAISgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcDABBwABSI2yhK2CQJSsoAFCgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcDABBwACCI2yhK2CQJSsoAICgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcDABBwDAMxMjNxOw0UEkpwRQtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAgAQcDsIDxJKcEU4cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVgQMBgoLDA0OD2AMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWEMFH7uGqvrZ+0deR1E5PX8866RcahxYgwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVjQIYh1CE="));

    #endregion

    #region Unsafe methods

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

    protected SampleException(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
